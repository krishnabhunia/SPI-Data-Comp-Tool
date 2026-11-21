using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Read-only OOXML reader for the audit task. Worksheet cells are streamed; the shared-string
    /// table and its offset index are held on disk. Format dictionaries are shared, immutable snapshots.
    /// This does not launch Excel, recalculate formulas, or execute workbook content.
    /// </summary>
    internal sealed class ExcelAuditReader : IDisposable
    {
        private const int MaxColumns = 16384;
        private const int MaxRows = 1048576;
        private const long MaxRowCharacters = 8L * 1024 * 1024;
        private static readonly XNamespace S = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private static readonly Regex Numeric = new Regex(@"^([+-]?)(\d*)(?:\.(\d*))?(?:[eE]([+-]?\d+))?$", RegexOptions.CultureInvariant);
        private readonly Dictionary<int, uint> columnStyles = new Dictionary<int, uint>();
        private readonly Dictionary<uint, Dictionary<string, string>> formatCache = new Dictionary<uint, Dictionary<string, string>>();
        private readonly Dictionary<uint, string> numberFormats = new Dictionary<uint, string>();
        private readonly Dictionary<int, string> themeColors = new Dictionary<int, string>();
        private readonly List<string> indexedColors = new List<string>();
        private readonly List<XElement> fonts = new List<XElement>();
        private readonly List<XElement> fills = new List<XElement>();
        private readonly List<XElement> borders = new List<XElement>();
        private readonly List<XElement> cellFormats = new List<XElement>();
        private readonly List<XElement> baseFormats = new List<XElement>();
        private FileStream input;
        private SpreadsheetDocument document;
        private WorksheetPart sheetPart;
        private DiskSharedStrings strings;
        private string majorFont;
        private string minorFont;
        private string defaultRowHeight = "15";
        private string defaultColumnWidth = "Default";
        private bool defaultRowsHidden;
        private bool date1904;
        private bool disposed;

        internal ExcelAuditSheetInfo Info { get; private set; }

        internal ExcelAuditReader(string path, string sheetName, int headerRow, string tempDirectory, CancellationToken token)
        {
            ValidatePath(path);
            if (headerRow < 1 || headerRow > MaxRows) throw new ExcelAuditException("Header row must be between 1 and 1,048,576.");
            if (!Directory.Exists(tempDirectory)) throw new ExcelAuditException("The audit temporary folder does not exist.");
            Info = new ExcelAuditSheetInfo { FilePath = Path.GetFullPath(path), Name = sheetName, HeaderRow = headerRow };
            try
            {
                token.ThrowIfCancellationRequested();
                input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                document = SpreadsheetDocument.Open(input, false);
                WorkbookPart workbook = document.WorkbookPart;
                if (workbook == null || workbook.Workbook == null || workbook.Workbook.Sheets == null)
                    throw new ExcelAuditException("The workbook does not contain a worksheet list.");
                var sheets = workbook.Workbook.Sheets.Elements<Sheet>().ToArray();
                Info.SheetNames = sheets.Select(item => item.Name == null ? string.Empty : item.Name.Value).ToArray();
                Sheet selected = sheets.SingleOrDefault(item => string.Equals(item.Name?.Value, sheetName, StringComparison.Ordinal));
                if (selected == null || selected.Id == null)
                    throw new ExcelAuditException("Select a worksheet that exists in " + Path.GetFileName(path) + ".");
                sheetPart = workbook.GetPartById(selected.Id.Value) as WorksheetPart;
                if (sheetPart == null) throw new ExcelAuditException("The selected tab is not a worksheet (for example, it may be a chart sheet).");
                date1904 = workbook.Workbook.WorkbookProperties?.Date1904?.Value == true;
                Info.Structure["Worksheet visibility"] = Attribute(selected, "state", "visible");
                LoadTheme(workbook.ThemePart);
                LoadStyles(workbook.WorkbookStylesPart);
                strings = new DiskSharedStrings(tempDirectory);
                LoadSharedStrings(workbook.SharedStringTablePart, token);
                ReadMetadata(token);
                ReadHeaders(token);
                if (workbook.VbaProjectPart != null) Warn("Macros are present but are neither executed nor compared.");
                if (sheetPart.DrawingsPart != null) Warn("Drawings, charts and images are outside the checked scope.");
                if (sheetPart.WorksheetCommentsPart != null) Warn("Cell comments are outside the checked scope.");
            }
            catch (OperationCanceledException ex) { DisposeAfterFailure(ex); throw; }
            catch (ExcelAuditException ex) { DisposeAfterFailure(ex); throw; }
            catch (Exception ex) when (ex is IOException || ex is InvalidDataException || ex is OpenXmlPackageException
                || ex is XmlException || ex is InvalidOperationException || ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                DisposeAfterFailure(ex);
                throw new ExcelAuditException("Cannot read " + Path.GetFileName(path)
                    + ". Use a valid, unencrypted .xlsx or .xlsm workbook and close applications locking it. " + ex.Message, ex);
            }
        }

        internal static string[] ListSheets(string path)
        {
            ValidatePath(path);
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var package = SpreadsheetDocument.Open(stream, false))
                {
                    WorkbookPart workbook = package.WorkbookPart;
                    if (workbook?.Workbook?.Sheets == null) throw new ExcelAuditException("This workbook has no worksheets.");
                    return workbook.Workbook.Sheets.Elements<Sheet>()
                        .Where(item => item.Id != null && item.Name != null && workbook.GetPartById(item.Id.Value) is WorksheetPart)
                        .Select(item => item.Name.Value).ToArray();
                }
            }
            catch (ExcelAuditException) { throw; }
            catch (Exception ex) when (ex is IOException || ex is InvalidDataException || ex is OpenXmlPackageException
                || ex is XmlException || ex is InvalidOperationException || ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                throw new ExcelAuditException("Cannot load worksheet names. Use a valid, unencrypted .xlsx or .xlsm workbook. " + ex.Message, ex);
            }
        }

        private static void ValidatePath(string path)
        {
            string extension = Path.GetExtension(path ?? string.Empty);
            if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(extension, ".xlsm", StringComparison.OrdinalIgnoreCase))
                throw new ExcelAuditException("Excel Sheet Audit accepts .xlsx and .xlsm files. Convert legacy .xls files before auditing.");
            if (!File.Exists(path)) throw new ExcelAuditException("The selected workbook no longer exists.");
        }

        private void LoadTheme(ThemePart part)
        {
            if (part == null) return;
            XElement root = ReadSmallXml(part, 4L * 1024 * 1024);
            XElement scheme = root.Descendants().FirstOrDefault(element => element.Name.LocalName == "clrScheme");
            string[] order = { "lt1", "dk1", "lt2", "dk2", "accent1", "accent2", "accent3", "accent4", "accent5", "accent6", "hlink", "folHlink" };
            if (scheme != null)
            {
                for (int i = 0; i < order.Length; i++)
                {
                    XElement entry = scheme.Elements().FirstOrDefault(element => element.Name.LocalName == order[i])?.Elements().FirstOrDefault();
                    if (entry != null)
                    {
                        string rgb = entry.Name.LocalName == "sysClr" ? Attr(entry, "lastClr", string.Empty) : Attr(entry, "val", string.Empty);
                        if (IsRgb(rgb)) themeColors[i] = Argb(rgb);
                    }
                }
            }
            XElement fontScheme = root.Descendants().FirstOrDefault(element => element.Name.LocalName == "fontScheme");
            majorFont = FontTypeface(fontScheme, "majorFont");
            minorFont = FontTypeface(fontScheme, "minorFont");
        }

        private static string FontTypeface(XElement root, string name)
        {
            XElement group = root?.Elements().FirstOrDefault(element => element.Name.LocalName == name);
            XElement latin = group?.Elements().FirstOrDefault(element => element.Name.LocalName == "latin");
            return latin == null ? null : Attr(latin, "typeface", null);
        }

        private void LoadStyles(WorkbookStylesPart part)
        {
            if (part == null) return;
            XElement root = ReadSmallXml(part, 16L * 1024 * 1024);
            fonts.AddRange(root.Element(S + "fonts")?.Elements() ?? Enumerable.Empty<XElement>());
            fills.AddRange(root.Element(S + "fills")?.Elements() ?? Enumerable.Empty<XElement>());
            borders.AddRange(root.Element(S + "borders")?.Elements() ?? Enumerable.Empty<XElement>());
            cellFormats.AddRange(root.Element(S + "cellXfs")?.Elements() ?? Enumerable.Empty<XElement>());
            baseFormats.AddRange(root.Element(S + "cellStyleXfs")?.Elements() ?? Enumerable.Empty<XElement>());
            if (cellFormats.Count > 65536) throw new ExcelAuditException("This workbook exceeds the supported Excel cell-format count.");
            foreach (XElement format in root.Element(S + "numFmts")?.Elements() ?? Enumerable.Empty<XElement>())
            {
                uint id;
                if (uint.TryParse(Attr(format, "numFmtId", ""), NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    numberFormats[id] = Attr(format, "formatCode", "General");
            }
            XElement palette = root.Element(S + "colors")?.Element(S + "indexedColors");
            if (palette != null)
                foreach (XElement color in palette.Elements())
                {
                    string rgb = Attr(color, "rgb", string.Empty);
                    if (!IsRgb(rgb)) throw new ExcelAuditException("The workbook contains an invalid indexed colour.");
                    indexedColors.Add(Argb(rgb));
                }
        }

        private static XElement ReadSmallXml(OpenXmlPart part, long limit)
        {
            using (Stream stream = part.GetStream(FileMode.Open, FileAccess.Read))
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = limit
            }))
            {
                XElement root = XElement.Load(reader);
                // Strict OOXML uses another SpreadsheetML namespace; the semantic names are the same.
                foreach (XElement element in root.DescendantsAndSelf())
                    if (element.Name.NamespaceName == "http://purl.oclc.org/ooxml/spreadsheetml/main")
                        element.Name = S + element.Name.LocalName;
                return root;
            }
        }

        private void LoadSharedStrings(SharedStringTablePart part, CancellationToken token)
        {
            if (part != null)
            {
                using (OpenXmlReader reader = OpenXmlReader.Create(part))
                {
                    while (reader.Read())
                    {
                        token.ThrowIfCancellationRequested();
                        if (reader.IsStartElement && reader.ElementType == typeof(SharedStringItem))
                        {
                            var item = (SharedStringItem)reader.LoadCurrentElement();
                            strings.Add(TextOf(item), item.Elements<Run>().Any());
                        }
                    }
                }
            }
            strings.Flush();
        }

        private void ReadMetadata(CancellationToken token)
        {
            var merged = new List<string>();
            int metadataItems = 0;
            using (Stream stream = sheetPart.GetStream(FileMode.Open, FileAccess.Read))
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
            {
                while (reader.Read())
                {
                    token.ThrowIfCancellationRequested();
                    if (reader.NodeType != XmlNodeType.Element) continue;
                    switch (reader.LocalName)
                    {
                        case "sheetFormatPr":
                            defaultRowHeight = reader.GetAttribute("defaultRowHeight") ?? "15";
                            defaultColumnWidth = reader.GetAttribute("defaultColWidth") ?? "Default";
                            defaultRowsHidden = Bool(reader.GetAttribute("zeroHeight"));
                            Info.Structure["Default row height"] = NormaliseNumber(defaultRowHeight);
                            Info.Structure["Default column width"] = NormaliseNumber(defaultColumnWidth);
                            Info.Structure["Base column width"] = NormaliseNumber(reader.GetAttribute("baseColWidth") ?? "8");
                            break;
                        case "col":
                            ReadColumn(reader);
                            break;
                        case "mergeCell":
                            merged.Add(reader.GetAttribute("ref") ?? string.Empty);
                            if (++metadataItems > 100000) throw new ExcelAuditException("This worksheet has too many layout entries to audit safely.");
                            break;
                        case "pane":
                            foreach (string attribute in new[] { "state", "xSplit", "ySplit", "topLeftCell", "activePane" })
                                Info.Structure["Pane " + attribute] = reader.GetAttribute(attribute) ?? string.Empty;
                            break;
                        case "conditionalFormatting":
                            Warn("Conditional formatting rules and their rendered effects are outside the checked scope.");
                            break;
                        case "extLst":
                            Warn("Extended worksheet features are present and are not fully compared.");
                            break;
                    }
                }
            }
            merged.Sort(StringComparer.Ordinal);
            Info.Structure["Merged ranges"] = string.Join("; ", merged);
            if (!Info.Structure.ContainsKey("Default row height")) Info.Structure["Default row height"] = NormaliseNumber(defaultRowHeight);
            if (!Info.Structure.ContainsKey("Default column width")) Info.Structure["Default column width"] = defaultColumnWidth;
            if (!Info.Structure.ContainsKey("Base column width")) Info.Structure["Base column width"] = "8";
        }

        private void ReadColumn(XmlReader reader)
        {
            int first, last;
            if (!int.TryParse(reader.GetAttribute("min"), NumberStyles.None, CultureInfo.InvariantCulture, out first)
                || !int.TryParse(reader.GetAttribute("max"), NumberStyles.None, CultureInfo.InvariantCulture, out last)
                || first < 1 || last < first || last > MaxColumns)
                throw new ExcelAuditException("A column range is invalid or exceeds Excel's column limit.");
            uint style;
            bool hasStyle = uint.TryParse(reader.GetAttribute("style"), NumberStyles.None, CultureInfo.InvariantCulture, out style);
            for (int column = first; column <= last; column++)
            {
                Info.Columns[column] = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    { "Width", NormaliseNumber(reader.GetAttribute("width") ?? defaultColumnWidth) },
                    { "Hidden", BoolText(reader.GetAttribute("hidden")) },
                    { "Outline level", reader.GetAttribute("outlineLevel") ?? "0" },
                    { "Collapsed", BoolText(reader.GetAttribute("collapsed")) }
                };
                if (hasStyle) columnStyles[column] = style;
            }
        }

        private void ReadHeaders(CancellationToken token)
        {
            ExcelAuditRow row = null;
            foreach (ExcelAuditRow candidate in EnumerateRows(token))
            {
                if (candidate.Number < Info.HeaderRow)
                {
                    if (candidate.HasValues) Warn("Populated rows above the selected header are outside the selected table's checked scope.");
                    continue;
                }
                if (candidate.Number == Info.HeaderRow) row = candidate;
                break;
            }
            if (row == null || !row.HasValues) throw new ExcelAuditException("The selected header row is empty. Choose the row containing column names.");
            foreach (var entry in row.Layout) Info.Structure["Header row " + entry.Key] = entry.Value;
            int last = row.Cells.Where(item => !item.Value.IsBlank).Max(item => item.Key);
            if (row.Cells.Any(item => item.Key > last && item.Value.Format.Count != 0))
                Warn("Blank header-row cells beyond the last named column are outside the selected table's formatting scope.");
            for (int column = 1; column <= last; column++)
            {
                ExcelAuditCell cell;
                if (!row.Cells.TryGetValue(column, out cell)) cell = BlankCell(row.Number, column, 0);
                Info.HeaderCells[column] = cell;
                Info.Headers[column] = cell.IsBlank ? string.Empty : cell.Value;
                if (!Info.Columns.ContainsKey(column)) Info.Columns[column] = DefaultColumn();
            }
        }

        private Dictionary<string, string> DefaultColumn()
        {
            return new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "Width", NormaliseNumber(defaultColumnWidth) }, { "Hidden", "False" },
                { "Outline level", "0" }, { "Collapsed", "False" }
            };
        }

        internal IEnumerable<ExcelAuditRow> ReadRows(CancellationToken token)
        {
            if (disposed) throw new ObjectDisposedException(nameof(ExcelAuditReader));
            foreach (ExcelAuditRow row in EnumerateRows(token))
            {
                if (row.Number <= Info.HeaderRow) continue;
                foreach (var entry in row.Cells)
                {
                    if (!Info.Headers.ContainsKey(entry.Key) && !entry.Value.IsBlank)
                        throw new ExcelAuditException("Data in " + Info.Name + "!" + entry.Value.Address
                            + " has no column header. Add the missing header before comparing.");
                }
                // The raw iterator already supplies effective styles for all logical columns.
                yield return row;
            }
        }

        internal ExcelAuditRow BlankRow(int rowNumber)
        {
            var row = new ExcelAuditRow { Number = rowNumber, Layout = RowLayout(null) };
            foreach (int column in Info.Headers.Keys) row.Cells[column] = BlankCell(rowNumber, column, 0);
            return row;
        }

        private IEnumerable<ExcelAuditRow> EnumerateRows(CancellationToken token)
        {
            ExcelAuditRow row = null;
            uint rowStyle = 0;
            bool hasRowStyle = false;
            int previousRow = 0;
            long rowCharacters = 0;
            using (OpenXmlReader reader = OpenXmlReader.Create(sheetPart))
            {
                while (reader.Read())
                {
                    token.ThrowIfCancellationRequested();
                    if (reader.ElementType == typeof(Row) && reader.IsStartElement)
                    {
                        if (row != null) { FillBlankCells(row, rowStyle, hasRowStyle); yield return row; }
                        var attributes = reader.Attributes.ToDictionary(attribute => attribute.LocalName, attribute => attribute.Value, StringComparer.Ordinal);
                        string index;
                        int number = attributes.TryGetValue("r", out index) ? ParseIndex(index, "row", MaxRows) : previousRow + 1;
                        if (number <= previousRow || number > MaxRows) throw new ExcelAuditException("Worksheet row indexes must be unique, increasing and within Excel's row limit.");
                        previousRow = number;
                        row = new ExcelAuditRow { Number = number, Layout = RowLayout(attributes) };
                        hasRowStyle = attributes.ContainsKey("s") && Bool(Get(attributes, "customFormat", "1"));
                        rowStyle = hasRowStyle ? ParseStyle(attributes["s"]) : 0;
                        rowCharacters = 0;
                    }
                    else if (reader.ElementType == typeof(Cell) && reader.IsStartElement && row != null)
                    {
                        var cell = (Cell)reader.LoadCurrentElement();
                        int column = cell.CellReference == null ? (row.Cells.Count == 0 ? 1 : row.Cells.Keys.Last() + 1)
                            : ColumnFromAddress(cell.CellReference.Value, row.Number);
                        if (column < 1 || column > MaxColumns) throw new ExcelAuditException("A worksheet row exceeds Excel's column limit.");
                        if (row.Cells.ContainsKey(column)) throw new ExcelAuditException("Duplicate cell address in worksheet: " + Address(row.Number, column));
                        uint style = cell.StyleIndex?.Value ?? (hasRowStyle ? rowStyle : ColumnStyle(column));
                        ExcelAuditCell value = ReadCell(cell, row.Number, column, style);
                        rowCharacters += value.Value.Length + value.Canonical.Length;
                        if (rowCharacters > MaxRowCharacters)
                            throw new ExcelAuditException("A worksheet row exceeds the audit's 8 MiB text budget. Split unusually wide/long records before comparing.");
                        row.Cells[column] = value;
                    }
                    else if (reader.ElementType == typeof(Row) && reader.IsEndElement && row != null)
                    {
                        FillBlankCells(row, rowStyle, hasRowStyle);
                        yield return row;
                        row = null;
                    }
                }
                if (row != null) { FillBlankCells(row, rowStyle, hasRowStyle); yield return row; }
            }
        }

        private void FillBlankCells(ExcelAuditRow row, uint style, bool hasRowStyle)
        {
            foreach (int column in Info.Headers.Keys)
                if (!row.Cells.ContainsKey(column)) row.Cells[column] = BlankCell(row.Number, column, hasRowStyle ? style : ColumnStyle(column), true);
        }

        private Dictionary<string, string> RowLayout(Dictionary<string, string> attributes)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "Height", NormaliseNumber(Get(attributes, "ht", defaultRowHeight)) },
                { "Hidden", BoolText(Get(attributes, "hidden", defaultRowsHidden ? "1" : "0")) },
                { "Outline level", Get(attributes, "outlineLevel", "0") },
                { "Collapsed", BoolText(Get(attributes, "collapsed", "0")) },
                { "Thick top", BoolText(Get(attributes, "thickTop", "0")) },
                { "Thick bottom", BoolText(Get(attributes, "thickBot", "0")) }
            };
        }

        private ExcelAuditCell BlankCell(int row, int column, uint style, bool explicitStyle = false)
        {
            return new ExcelAuditCell { Column = column, Address = Address(row, column), Format = Format(explicitStyle ? style : ColumnStyle(column)) };
        }

        private uint ColumnStyle(int column)
        {
            uint style;
            return columnStyles.TryGetValue(column, out style) ? style : 0;
        }

        private ExcelAuditCell ReadCell(Cell cell, int row, int column, uint style)
        {
            var result = new ExcelAuditCell { Column = column, Address = Address(row, column), Format = Format(style) };
            string raw = cell.CellValue?.Text ?? string.Empty;
            if (cell.CellFormula != null)
            {
                string formulaType = Attribute(cell.CellFormula, "t", "normal");
                if (formulaType != "normal") Warn("Shared, array and data-table formulas are not expanded or recalculated; their saved formula definitions are compared only.");
                result.Kind = "Formula";
                result.Value = "=" + cell.CellFormula.Text;
                result.Canonical = result.Value;
                if (formulaType != "normal") result.Canonical += " | " + formulaType + " | " + Attribute(cell.CellFormula, "ref", "") + " | " + Attribute(cell.CellFormula, "si", "");
            }
            else if (cell.DataType?.Value == CellValues.SharedString)
            {
                long index;
                if (!long.TryParse(raw, NumberStyles.None, CultureInfo.InvariantCulture, out index)) throw new ExcelAuditException("Invalid shared-string index at " + result.Address + ".");
                result.Kind = "Text";
                bool richText;
                result.Value = strings.Get(index, out richText);
                if (richText) Warn("Rich-text run formatting is not fully compared; only combined text and the cell-level format are checked.");
                result.Canonical = result.Value;
            }
            else if (cell.DataType?.Value == CellValues.InlineString)
            {
                if (cell.InlineString?.Elements<Run>().Any() == true) Warn("Rich-text run formatting is not fully compared; only combined text and the cell-level format are checked.");
                result.Kind = "Text";
                result.Value = cell.InlineString == null ? string.Empty : TextOf(cell.InlineString);
                result.Canonical = result.Value;
            }
            else if (cell.DataType?.Value == CellValues.String)
            {
                result.Kind = "Text"; result.Value = raw; result.Canonical = raw;
            }
            else if (cell.DataType?.Value == CellValues.Boolean)
            {
                if (raw != "0" && raw != "1" && raw != "true" && raw != "false") throw new ExcelAuditException("Invalid Boolean at " + result.Address + ".");
                result.Kind = "Boolean"; result.Value = BoolText(raw); result.Canonical = result.Value;
            }
            else if (cell.DataType?.Value == CellValues.Error)
            {
                result.Kind = "Error"; result.Value = raw; result.Canonical = raw;
            }
            else if (cell.DataType?.Value == CellValues.Date)
            {
                DateTime value;
                if (!Regex.IsMatch(raw, @"^\d{4}-\d{2}-\d{2}") || !DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out value))
                    throw new ExcelAuditException("Invalid ISO date at " + result.Address + ".");
                result.Kind = "Date"; result.Value = raw; result.Canonical = value.Ticks.ToString(CultureInfo.InvariantCulture);
                if (Regex.IsMatch(raw, @"\.\d{8,}", RegexOptions.CultureInvariant))
                {
                    // DateTime rounds beyond seven fractional digits; never hide a saved precision difference.
                    result.Canonical = "HighPrecisionISO:" + raw;
                    Warn("An ISO date exceeds DateTime's seven fractional-second digits; its exact saved text is compared, but cross-representation timestamp equality is not inferred.");
                }
                if (value.Kind != DateTimeKind.Unspecified) Warn("Timezone-bearing ISO dates are compared as saved timestamps; workbook timezone interpretation is not inferred.");
            }
            else if (!string.IsNullOrEmpty(raw))
            {
                Match numeric = Numeric.Match(raw);
                if (!numeric.Success || numeric.Groups[2].Length + numeric.Groups[3].Length == 0)
                    throw new ExcelAuditException("Invalid stored numeric value at " + result.Address + ".");
                result.Kind = "Number"; result.Value = raw; result.Canonical = NormaliseNumber(raw);
                if (IsDateFormat(result.Format["Number format"])) SetDate(result, raw);
            }
            if (result.Value.Length > 32767 || result.Canonical.Length > 65536)
                throw new ExcelAuditException("Cell " + result.Address + " exceeds the supported Excel text length.");
            return result;
        }

        private void SetDate(ExcelAuditCell cell, string raw)
        {
            decimal serial;
            if (!decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out serial))
                throw new ExcelAuditException("Date serial is outside the supported range at " + cell.Address + ".");
            if (NormaliseNumber(raw) != NormaliseNumber(serial.ToString("G29", CultureInfo.InvariantCulture)))
                Warn("A date serial exceeds decimal precision; its converted timestamp may be rounded. Full identity cannot be established.");
            if (!date1904 && serial <= 0)
                Warn("A zero or negative 1900-system date is present; Excel's non-Gregorian early-date display is not inferred.");
            if (!date1904 && serial >= 60 && serial < 61)
            {
                Warn("The fictitious Excel 1900 leap day is present; it is compared as an Excel serial, not a Gregorian date.");
                cell.Kind = "Date"; cell.Canonical = "Excel1900LeapDay:" + NormaliseNumber(raw); return;
            }
            try
            {
                decimal days = date1904 ? serial + 1462 : serial > 0 && serial < 60 ? serial + 1 : serial;
                long epochTicks = new DateTime(1899, 12, 30).Ticks;
                decimal ticks = epochTicks + days * TimeSpan.TicksPerDay;
                if ((ticks - epochTicks) / TimeSpan.TicksPerDay != days)
                    Warn("A date serial exceeds decimal timestamp precision; full identity cannot be established.");
                long roundedTicks = decimal.ToInt64(decimal.Round(ticks, 0, MidpointRounding.AwayFromZero));
                var date = new DateTime(roundedTicks);
                cell.Kind = "Date";
                cell.Value = date.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
                cell.Canonical = ticks.ToString("G29", CultureInfo.InvariantCulture);
            }
            catch (ArgumentOutOfRangeException ex) { throw new ExcelAuditException("Date serial is outside the supported range at " + cell.Address + ".", ex); }
            catch (OverflowException ex) { throw new ExcelAuditException("Date serial is outside the supported range at " + cell.Address + ".", ex); }
        }

        internal static string NormaliseNumber(string value)
        {
            Match match = Numeric.Match(value ?? string.Empty);
            if (!match.Success || match.Groups[2].Length + match.Groups[3].Length == 0) return value ?? string.Empty;
            int exponent = 0;
            if (match.Groups[4].Success && (!int.TryParse(match.Groups[4].Value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out exponent) || Math.Abs((long)exponent) > 10000))
                throw new ExcelAuditException("Numeric exponent is outside the supported range.");
            string digits = (match.Groups[2].Value + match.Groups[3].Value).TrimStart('0');
            if (digits.Length == 0) return "0";
            exponent -= match.Groups[3].Length;
            int length = digits.Length;
            digits = digits.TrimEnd('0');
            exponent += length - digits.Length;
            string sign = match.Groups[1].Value == "-" ? "-" : "";
            // Keep ordinary font sizes, widths and values readable while retaining exact decimal equivalence.
            if (exponent >= 0 && digits.Length + exponent <= 40) return sign + digits + new string('0', exponent);
            if (exponent < 0 && exponent >= -20 && digits.Length <= 40)
            {
                int position = digits.Length + exponent;
                return sign + (position > 0 ? digits.Insert(position, ".") : "0." + new string('0', -position) + digits);
            }
            return sign + digits + "e" + exponent.ToString(CultureInfo.InvariantCulture);
        }

        private static bool IsDateFormat(string format)
        {
            string cleaned = Regex.Replace(format ?? string.Empty, "\"[^\"]*\"|\\\\.|_.|\\*.", string.Empty);
            // Elapsed [h]/[m]/[s] formats represent durations, not calendar dates; date-system shifts must not alter them.
            if (Regex.IsMatch(cleaned, @"\[[hms]+\]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) return false;
            cleaned = Regex.Replace(cleaned, @"\[(?![hms]+\])[^\]]*\]", string.Empty, RegexOptions.IgnoreCase);
            return Regex.IsMatch(cleaned, "[ymdhs]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        private Dictionary<string, string> Format(uint index)
        {
            Dictionary<string, string> cached;
            if (formatCache.TryGetValue(index, out cached)) return cached;
            if (index >= cellFormats.Count && (cellFormats.Count > 0 || index != 0)) throw new ExcelAuditException("A cell references a missing workbook style.");
            XElement format = index < cellFormats.Count ? cellFormats[(int)index] : null;
            uint baseIndex = UInt(Attr(format, "xfId", "0"));
            if (baseIndex >= baseFormats.Count && baseIndex != 0) throw new ExcelAuditException("A cell references a missing base style.");
            XElement inherited = baseIndex < baseFormats.Count ? baseFormats[(int)baseIndex] : null;
            uint fontId = EffectiveId(format, inherited, "fontId", "applyFont");
            uint fillId = EffectiveId(format, inherited, "fillId", "applyFill");
            uint borderId = EffectiveId(format, inherited, "borderId", "applyBorder");
            uint numberId = EffectiveId(format, inherited, "numFmtId", "applyNumberFormat");
            XElement font = At(fonts, fontId, "font"), fill = At(fills, fillId, "fill"), border = At(borders, borderId, "border");
            XElement alignment = EffectiveElement(format, inherited, "alignment", "applyAlignment");
            XElement protection = EffectiveElement(format, inherited, "protection", "applyProtection");
            string scheme = ChildVal(font, "scheme", "none");
            string fontName = ChildVal(font, "name", "Default");
            if (scheme == "major" && !string.IsNullOrEmpty(majorFont)) fontName = majorFont;
            if (scheme == "minor" && !string.IsNullOrEmpty(minorFont)) fontName = minorFont;
            cached = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "Font name", fontName }, { "Font size", NormaliseNumber(ChildVal(font, "sz", "11")) },
                { "Font bold", ChildBool(font, "b") }, { "Font italic", ChildBool(font, "i") },
                { "Font strike-through", ChildBool(font, "strike") }, { "Font outline", ChildBool(font, "outline") },
                { "Font shadow", ChildBool(font, "shadow") }, { "Font underline", font?.Element(S + "u") == null ? "none" : ChildVal(font, "u", "single") },
                { "Font vertical position", ChildVal(font, "vertAlign", "baseline") }, { "Font scheme", scheme },
                { "Font family", ChildVal(font, "family", "0") }, { "Font character set", ChildVal(font, "charset", "0") },
                { "Font colour", Color(font?.Element(S + "color"), "Automatic") },
                { "Horizontal alignment", Attr(alignment, "horizontal", "general") }, { "Vertical alignment", Attr(alignment, "vertical", "bottom") },
                { "Text wrapping", BoolText(Attr(alignment, "wrapText", "0")) }, { "Shrink to fit", BoolText(Attr(alignment, "shrinkToFit", "0")) },
                { "Text rotation", Attr(alignment, "textRotation", "0") }, { "Indent", Attr(alignment, "indent", "0") },
                { "Relative indent", Attr(alignment, "relativeIndent", "0") }, { "Reading order", Attr(alignment, "readingOrder", "0") },
                { "Justify last line", BoolText(Attr(alignment, "justifyLastLine", "0")) },
                { "Locked", BoolText(Attr(protection, "locked", "1")) }, { "Hidden formula", BoolText(Attr(protection, "hidden", "0")) },
                { "Number format", NumberFormat(numberId) }, { "Quote prefix", BoolText(Attr(format, "quotePrefix", "0")) }
            };
            AddFill(cached, fill);
            foreach (string side in new[] { "left", "right", "top", "bottom", "diagonal", "vertical", "horizontal", "start", "end" })
            {
                XElement edge = border?.Element(S + side);
                string style = Attr(edge, "style", "none");
                cached["Border " + side] = style + (style == "none" ? string.Empty : " / " + Color(edge?.Element(S + "color"), "Automatic"));
            }
            cached["Border diagonal up"] = BoolText(Attr(border, "diagonalUp", "0"));
            cached["Border diagonal down"] = BoolText(Attr(border, "diagonalDown", "0"));
            cached["Border outline"] = BoolText(Attr(border, "outline", "1"));
            if (formatCache.Count >= 8192) formatCache.Clear();
            formatCache[index] = cached;
            return cached;
        }

        private void AddFill(Dictionary<string, string> format, XElement fill)
        {
            XElement pattern = fill?.Element(S + "patternFill"), gradient = fill?.Element(S + "gradientFill");
            string type = Attr(pattern, "patternType", "none");
            format["Fill pattern"] = gradient == null ? type : "gradient";
            format["Fill foreground"] = type == "none" ? "none" : Color(pattern?.Element(S + "fgColor"), "Automatic");
            format["Fill background"] = type == "none" ? "none" : Color(pattern?.Element(S + "bgColor"), "Automatic");
            if (gradient == null) return;
            foreach (string attribute in new[] { "type", "degree", "left", "right", "top", "bottom" })
                format["Gradient " + attribute] = Attr(gradient, attribute, attribute == "type" ? "linear" : "0");
            int count = 0;
            foreach (XElement stop in gradient.Elements(S + "stop"))
            {
                if (++count > 128) throw new ExcelAuditException("A gradient fill has too many colour stops to audit safely.");
                format["Gradient stop " + count.ToString(CultureInfo.InvariantCulture)] = Attr(stop, "position", "0") + " / " + Color(stop.Element(S + "color"), "Automatic");
            }
        }

        private static uint EffectiveId(XElement value, XElement inherited, string attribute, string apply)
        {
            return UInt(!Bool(Attr(value, apply, "1")) ? Attr(inherited, attribute, "0") : Attr(value, attribute, Attr(inherited, attribute, "0")));
        }

        private static XElement EffectiveElement(XElement value, XElement inherited, string element, string apply)
        {
            return !Bool(Attr(value, apply, "1")) ? inherited?.Element(S + element) : value?.Element(S + element) ?? inherited?.Element(S + element);
        }

        private static XElement At(List<XElement> values, uint index, string label)
        {
            if (values.Count == 0 && index == 0) return null;
            if (index >= values.Count) throw new ExcelAuditException("A cell references a missing workbook " + label + ".");
            return values[(int)index];
        }

        private string NumberFormat(uint id)
        {
            string custom;
            if (numberFormats.TryGetValue(id, out custom)) return custom;
            string[] basic = { "General", "0", "0.00", "#,##0", "#,##0.00" };
            if (id < basic.Length) return basic[id];
            switch (id)
            {
                case 5: return "\"$\"#,##0_);(\"$\"#,##0)";
                case 6: return "\"$\"#,##0_);[Red](\"$\"#,##0)";
                case 7: return "\"$\"#,##0.00_);(\"$\"#,##0.00)";
                case 8: return "\"$\"#,##0.00_);[Red](\"$\"#,##0.00)";
                case 9: return "0%"; case 10: return "0.00%"; case 11: return "0.00E+00";
                case 12: return "# ?/?"; case 13: return "# ??/??"; case 14: return "mm-dd-yy";
                case 15: return "d-mmm-yy"; case 16: return "d-mmm"; case 17: return "mmm-yy";
                case 18: return "h:mm AM/PM"; case 19: return "h:mm:ss AM/PM"; case 20: return "h:mm";
                case 21: return "h:mm:ss"; case 22: return "m/d/yy h:mm";
                case 37: return "#,##0 ;(#,##0)"; case 38: return "#,##0 ;[Red](#,##0)";
                case 39: return "#,##0.00;(#,##0.00)"; case 40: return "#,##0.00;[Red](#,##0.00)";
                case 41: return "_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)";
                case 42: return "_(\"$\"* #,##0_);_(\"$\"* \\(#,##0\\);_(\"$\"* \"-\"_);_(@_)";
                case 43: return "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)";
                case 44: return "_(\"$\"* #,##0.00_)_(\"$\"* \\(#,##0.00\\)_(\"$\"* \"-\"??_)_(@_)";
                case 45: return "mm:ss"; case 46: return "[h]:mm:ss"; case 47: return "mmss.0";
                case 48: return "##0.0E+0"; case 49: return "@";
                default:
                    Warn("Locale-specific or unknown built-in number formats are compared by stored format identifier; their rendered appearance is not inferred.");
                    return "Built-in " + id.ToString(CultureInfo.InvariantCulture);
            }
        }

        private string Color(XElement color, string fallback)
        {
            if (color == null) return fallback;
            if (Bool(Attr(color, "auto", "0"))) return "Automatic";
            string rgb = Attr(color, "rgb", string.Empty);
            if (rgb.Length != 0 && !IsRgb(rgb)) throw new ExcelAuditException("The workbook contains an invalid colour value.");
            double tint;
            if (!double.TryParse(Attr(color, "tint", "0"), NumberStyles.Float, CultureInfo.InvariantCulture, out tint)
                || double.IsNaN(tint) || double.IsInfinity(tint) || tint < -1 || tint > 1)
                throw new ExcelAuditException("The workbook contains an invalid colour tint.");
            int index;
            if (color.Attribute("theme") != null && !int.TryParse(Attr(color, "theme", ""), NumberStyles.None, CultureInfo.InvariantCulture, out index))
                throw new ExcelAuditException("The workbook contains an invalid theme colour index.");
            if (color.Attribute("indexed") != null && !int.TryParse(Attr(color, "indexed", ""), NumberStyles.None, CultureInfo.InvariantCulture, out index))
                throw new ExcelAuditException("The workbook contains an invalid indexed colour reference.");
            if (rgb.Length == 0 && int.TryParse(Attr(color, "theme", ""), NumberStyles.None, CultureInfo.InvariantCulture, out index))
            {
                if (!themeColors.TryGetValue(index, out rgb))
                {
                    Warn("A theme colour could not be resolved; its stored index is compared instead.");
                    return "Theme " + index.ToString(CultureInfo.InvariantCulture) + "/tint=" + Attr(color, "tint", "0");
                }
            }
            if (rgb.Length == 0 && int.TryParse(Attr(color, "indexed", ""), NumberStyles.None, CultureInfo.InvariantCulture, out index))
            {
                if (index >= 0 && index < indexedColors.Count) rgb = indexedColors[index];
                else if (index >= 0 && index < DefaultIndexedColors.Length) rgb = DefaultIndexedColors[index];
                else return "System indexed " + index.ToString(CultureInfo.InvariantCulture);
            }
            if (!IsRgb(rgb)) return fallback;
            rgb = Argb(rgb);
            if (tint != 0) rgb = ApplyTint(rgb, tint);
            return rgb;
        }

        private static string ApplyTint(string argb, double tint)
        {
            tint = Math.Max(-1, Math.Min(1, tint));
            double r = int.Parse(argb.Substring(2, 2), NumberStyles.HexNumber) / 255.0;
            double g = int.Parse(argb.Substring(4, 2), NumberStyles.HexNumber) / 255.0;
            double b = int.Parse(argb.Substring(6, 2), NumberStyles.HexNumber) / 255.0;
            double max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Min(g, b));
            double l = (max + min) / 2, h = 0, saturation = 0;
            if (max != min)
            {
                double delta = max - min;
                saturation = l > .5 ? delta / (2 - max - min) : delta / (max + min);
                if (max == r) h = (g - b) / delta + (g < b ? 6 : 0);
                else if (max == g) h = (b - r) / delta + 2;
                else h = (r - g) / delta + 4;
                h /= 6;
            }
            l = tint < 0 ? l * (1 + tint) : l * (1 - tint) + tint;
            if (saturation == 0) r = g = b = l;
            else
            {
                double q = l < .5 ? l * (1 + saturation) : l + saturation - l * saturation;
                double p = 2 * l - q;
                r = Hue(p, q, h + 1.0 / 3); g = Hue(p, q, h); b = Hue(p, q, h - 1.0 / 3);
            }
            return "FF" + ByteHex(r) + ByteHex(g) + ByteHex(b);
        }

        private static double Hue(double p, double q, double t)
        {
            if (t < 0) t += 1; if (t > 1) t -= 1;
            if (t < 1.0 / 6) return p + (q - p) * 6 * t;
            if (t < .5) return q;
            if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
            return p;
        }

        private static string ByteHex(double value) => Math.Max(0, Math.Min(255, (int)Math.Round(value * 255, MidpointRounding.AwayFromZero))).ToString("X2", CultureInfo.InvariantCulture);
        private static bool IsRgb(string value) => Regex.IsMatch(value ?? string.Empty, @"^(?:[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$");
        private static string Argb(string value) => value.Length == 6 ? "FF" + value.ToUpperInvariant() : value.ToUpperInvariant();
        private static bool Bool(string value) => value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        private static string BoolText(string value) => Bool(value) ? "True" : "False";
        private static string Attr(XElement element, string name, string fallback) => element?.Attribute(name)?.Value ?? fallback;
        private static string ChildVal(XElement element, string name, string fallback) => Attr(element?.Element(S + name), "val", fallback);
        private static string ChildBool(XElement element, string name) => element?.Element(S + name) == null ? "False" : BoolText(ChildVal(element, name, "1"));
        private static string Get(Dictionary<string, string> values, string name, string fallback)
        {
            string value;
            return values != null && values.TryGetValue(name, out value) ? value : fallback;
        }
        private static string Attribute(OpenXmlElement element, string name, string fallback)
        {
            OpenXmlAttribute attribute = element.GetAttributes().FirstOrDefault(item => item.LocalName == name);
            return string.IsNullOrEmpty(attribute.LocalName) ? fallback : attribute.Value;
        }
        private static uint UInt(string value)
        {
            uint number;
            if (!uint.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out number)) throw new ExcelAuditException("Invalid workbook style reference.");
            return number;
        }
        private static uint ParseStyle(string value) => UInt(value);
        private static int ParseIndex(string value, string label, int maximum)
        {
            int number;
            if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out number) || number < 1 || number > maximum)
                throw new ExcelAuditException("Invalid Excel " + label + " index.");
            return number;
        }
        private static int ColumnFromAddress(string address, int row)
        {
            int column = 0, i = 0;
            while (i < address.Length && address[i] >= 'A' && address[i] <= 'Z')
            {
                column = checked(column * 26 + address[i] - 'A' + 1); i++;
                if (column > MaxColumns) throw new ExcelAuditException("Cell reference exceeds Excel's column limit.");
            }
            if (column == 0 || ParseIndex(address.Substring(i), "row", MaxRows) != row)
                throw new ExcelAuditException("A cell reference does not belong to its worksheet row.");
            return column;
        }
        internal static string Address(int row, int column)
        {
            string name = string.Empty;
            while (column > 0) { column--; name = (char)('A' + column % 26) + name; column /= 26; }
            return name + row.ToString(CultureInfo.InvariantCulture);
        }
        private static string TextOf(OpenXmlElement element)
        {
            var text = new StringBuilder();
            foreach (OpenXmlElement child in element.ChildElements)
            {
                if (child.LocalName == "t") text.Append(child.InnerText);
                else if (child.LocalName == "r")
                    foreach (OpenXmlElement run in child.ChildElements)
                        if (run.LocalName == "t") text.Append(run.InnerText);
            }
            if (text.Length > 32767) throw new ExcelAuditException("A shared or inline string exceeds Excel's text length limit.");
            return text.ToString();
        }
        private void Warn(string message)
        {
            if (!Info.Warnings.Contains(message)) Info.Warnings.Add(message);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            Exception failure = null;
            DisposeResources(ref failure, strings, document, input);
            if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
        }

        private void DisposeAfterFailure(Exception primary)
        {
            try { Dispose(); }
            catch (Exception cleanup) { primary.Data["ExcelAuditCleanupError"] = cleanup.Message; }
        }

        private static void DisposeResources(ref Exception failure, params IDisposable[] resources)
        {
            foreach (IDisposable resource in resources)
            {
                try { resource?.Dispose(); }
                catch (Exception ex) { if (failure == null) failure = ex; }
            }
        }

        private sealed class DiskSharedStrings : IDisposable
        {
            private readonly string dataPath;
            private readonly string indexPath;
            private FileStream data;
            private FileStream index;
            private BinaryWriter dataWriter;
            private BinaryWriter indexWriter;
            private BinaryReader dataReader;
            private BinaryReader indexReader;
            private long count;
            private int cacheCharacters;
            private bool disposed;
            private readonly Dictionary<long, string> cache = new Dictionary<long, string>();
            private readonly Dictionary<long, bool> richCache = new Dictionary<long, bool>();
            private readonly Queue<long> order = new Queue<long>();

            internal DiskSharedStrings(string directory)
            {
                string id = Guid.NewGuid().ToString("N");
                dataPath = Path.Combine(directory, ".spi-audit-strings-" + id + ".tmp");
                indexPath = Path.Combine(directory, ".spi-audit-string-index-" + id + ".tmp");
                try
                {
                    data = new FileStream(dataPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
                    index = new FileStream(indexPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
                    dataWriter = new BinaryWriter(data, Encoding.UTF8, true);
                    indexWriter = new BinaryWriter(index, Encoding.UTF8, true);
                    dataReader = new BinaryReader(data, Encoding.UTF8, true);
                    indexReader = new BinaryReader(index, Encoding.UTF8, true);
                }
                catch (Exception primary)
                {
                    try { Dispose(); }
                    catch (Exception cleanup) { primary.Data["ExcelAuditCleanupError"] = cleanup.Message; }
                    throw;
                }
            }
            internal void Add(string text, bool richText)
            {
                indexWriter.Write(data.Position);
                dataWriter.Write(richText);
                dataWriter.Write(text);
                count++;
            }
            internal void Flush() { dataWriter.Flush(); indexWriter.Flush(); }
            internal string Get(long position, out bool richText)
            {
                if (position < 0 || position >= count) throw new ExcelAuditException("A cell references a missing shared string.");
                string text;
                if (cache.TryGetValue(position, out text)) { richText = richCache[position]; return text; }
                index.Position = checked(position * sizeof(long));
                data.Position = indexReader.ReadInt64();
                richText = dataReader.ReadBoolean();
                text = dataReader.ReadString();
                while (order.Count > 0 && (cacheCharacters + text.Length > 256 * 1024 || order.Count >= 256))
                {
                    long removed = order.Dequeue(); cacheCharacters -= cache[removed].Length; cache.Remove(removed); richCache.Remove(removed);
                }
                cache[position] = text; richCache[position] = richText; order.Enqueue(position); cacheCharacters += text.Length;
                return text;
            }
            public void Dispose()
            {
                if (disposed) return;
                disposed = true;
                Exception failure = null;
                // A disk-full flush must not strand the remaining handles or prevent either deletion attempt.
                DisposeResources(ref failure, dataReader, indexReader, dataWriter, indexWriter, data, index);
                TryDelete(dataPath, ref failure);
                TryDelete(indexPath, ref failure);
                cache.Clear(); richCache.Clear(); order.Clear();
                if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
            }
            private static void TryDelete(string path, ref Exception failure)
            {
                if (path == null) return;
                try { File.Delete(path); }
                catch (Exception ex)
                {
                    if (failure == null) failure = new IOException("Could not remove the audit's temporary shared-string file: " + path, ex);
                    else failure.Data["ExcelAuditTemporaryFileCleanupError:" + path] = ex.Message;
                }
            }
        }

        private static readonly string[] DefaultIndexedColors =
        {
            "FF000000", "FFFFFFFF", "FFFF0000", "FF00FF00", "FF0000FF", "FFFFFF00", "FFFF00FF", "FF00FFFF",
            "FF000000", "FFFFFFFF", "FFFF0000", "FF00FF00", "FF0000FF", "FFFFFF00", "FFFF00FF", "FF00FFFF",
            "FF800000", "FF008000", "FF000080", "FF808000", "FF800080", "FF008080", "FFC0C0C0", "FF808080",
            "FF9999FF", "FF993366", "FFFFFFCC", "FFCCFFFF", "FF660066", "FFFF8080", "FF0066CC", "FFCCCCFF",
            "FF000080", "FFFF00FF", "FFFFFF00", "FF00FFFF", "FF800080", "FF800000", "FF008080", "FF0000FF",
            "FF00CCFF", "FFCCFFFF", "FFCCFFCC", "FFFFFF99", "FF99CCFF", "FFFF99CC", "FFCC99FF", "FFFFCC99",
            "FF3366FF", "FF33CCCC", "FF99CC00", "FFFFCC00", "FFFF9900", "FFFF6600", "FF666699", "FF969696",
            "FF003366", "FF339966", "FF003300", "FF333300", "FF993300", "FF993366", "FF333399", "FF333333"
        };
    }
}
