using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SPI_Data_Comp_Tool
{
    /// <summary>One worksheet cell as the identity check sees it: text, data type, formula and style number.</summary>
    internal struct IdentityCell
    {
        public string Value;      // null = blank
        public string Kind;       // blank, number, text, boolean, date, time, duration
        public double Number;     // only meaningful when Kind is "number"
        public string Formula;    // "=..." or null
        public int Style;         // index into IdentityWorkbook.Styles
    }

    /// <summary>Positions and report labels of the 12 style properties kept for every cell format.</summary>
    internal static class IdentityStyle
    {
        internal const int FontName = 0, FontSize = 1, Bold = 2, Italic = 3, Underline = 4, FontColour = 5,
            HAlign = 6, VAlign = 7, Wrap = 8, NumberFormat = 9, FillColour = 10, Borders = 11;

        internal static readonly string[] Labels =
        {
            "Font name", "Font size", "Bold", "Italic", "Underline", "Font colour",
            "Horizontal alignment", "Vertical alignment", "Wrap text", "Number format", "Fill colour", "Borders",
        };
    }

    /// <summary>Everything the identity check compares for one worksheet. Read once, then only looked up.</summary>
    internal sealed class IdentitySheet
    {
        internal static readonly IdentityCell Blank = new IdentityCell { Kind = "blank" };

        internal string Name;
        internal readonly Dictionary<long, IdentityCell> Cells = new Dictionary<long, IdentityCell>();
        internal int LastRow, LastCol;                       // last row / column that holds a value or a formula
        internal string[][] Styles;
        internal readonly Dictionary<int, double> Widths = new Dictionary<int, double>();
        internal readonly Dictionary<int, double> Heights = new Dictionary<int, double>();
        internal readonly HashSet<string> HiddenColumns = new HashSet<string>();
        internal readonly HashSet<int> HiddenRows = new HashSet<int>();
        internal readonly HashSet<string> Merged = new HashSet<string>();
        internal readonly Dictionary<string, string> Comments = new Dictionary<string, string>();
        internal readonly Dictionary<string, string> Links = new Dictionary<string, string>();
        internal readonly List<string> Validations = new List<string>();
        internal readonly List<string> ConditionalFormats = new List<string>();
        internal string Freeze, Filter, State = "visible";
        internal int UncachedFormulas;

        internal static long Key(int row, int col) { return ((long)row << 15) | (uint)col; }

        internal IdentityCell Cell(int row, int col)
        {
            IdentityCell cell;
            return Cells.TryGetValue(Key(row, col), out cell) ? cell : Blank;
        }

        /// <summary>The 12 style properties of a cell, in the order of <see cref="IdentityStyle.Labels"/>.</summary>
        internal string[] StyleOf(IdentityCell cell)
        {
            return cell.Style >= 0 && cell.Style < Styles.Length ? Styles[cell.Style] : Styles[0];
        }
    }

    /// <summary>
    /// Reads an .xlsx / .xlsm workbook with the Open XML SDK: the values, formulas and styles exactly as the file
    /// stores them. Nothing is recalculated and nothing is written. Sheets are streamed row by row, and only what
    /// the file sets is kept, so a column width or row height that was never set stays "default".
    /// Children are looked up by type (GetFirstChild), so their order inside an element does not matter.
    /// </summary>
    internal sealed class IdentityWorkbook : IDisposable
    {
        private static readonly Dictionary<uint, string> BuiltInFormats = new Dictionary<uint, string>
        {
            { 0, "General" }, { 1, "0" }, { 2, "0.00" }, { 3, "#,##0" }, { 4, "#,##0.00" },
            { 5, "\"$\"#,##0_);(\"$\"#,##0)" }, { 6, "\"$\"#,##0_);[Red](\"$\"#,##0)" },
            { 7, "\"$\"#,##0.00_);(\"$\"#,##0.00)" }, { 8, "\"$\"#,##0.00_);[Red](\"$\"#,##0.00)" },
            { 9, "0%" }, { 10, "0.00%" }, { 11, "0.00E+00" }, { 12, "# ?/?" }, { 13, "# ??/??" },
            { 14, "mm-dd-yy" }, { 15, "d-mmm-yy" }, { 16, "d-mmm" }, { 17, "mmm-yy" }, { 18, "h:mm AM/PM" },
            { 19, "h:mm:ss AM/PM" }, { 20, "h:mm" }, { 21, "h:mm:ss" }, { 22, "m/d/yy h:mm" },
            { 37, "#,##0_);(#,##0)" }, { 38, "#,##0_);[Red](#,##0)" }, { 39, "#,##0.00_);(#,##0.00)" },
            { 40, "#,##0.00_);[Red](#,##0.00)" }, { 41, "_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)" },
            { 42, "_(\"$\"* #,##0_);_(\"$\"* \\(#,##0\\);_(\"$\"* \"-\"_);_(@_)" },
            { 43, "_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)" },
            { 44, "_(\"$\"* #,##0.00_)_(\"$\"* \\(#,##0.00\\)_(\"$\"* \"-\"??_)_(@_)" },
            { 45, "mm:ss" }, { 46, "[h]:mm:ss" }, { 47, "mmss.0" }, { 48, "##0.0E+0" }, { 49, "@" },
        };

        private static readonly Regex FormatNoise = new Regex("\"[^\"]*\"|\\[(?!hh?\\]|mm?\\]|ss?\\])[^\\]]*\\]", RegexOptions.Compiled);
        private static readonly Regex DateLetters = new Regex(@"(?<![_\\])[dmhysDMHYS]", RegexOptions.Compiled);
        private static readonly Regex DurationFormat = new Regex(@"\[hh?\]|\[mm?\]|\[ss?\]", RegexOptions.Compiled);

        private readonly FileStream stream;
        private readonly SpreadsheetDocument document;
        private readonly WorkbookPart workbook;
        private readonly Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>(StringComparer.Ordinal);
        private readonly List<string> sharedStrings = new List<string>();
        private readonly bool date1904;
        private string[][] styles;
        private bool[] dateStyle, durationStyle;

        internal string FilePath { get; }
        internal List<string> SheetNames { get; } = new List<string>();
        internal string[][] Styles { get { return styles; } }

        internal IdentityWorkbook(string path)
        {
            FilePath = path;
            string extension = Path.GetExtension(path) ?? "";
            if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase) && !extension.Equals(".xlsm", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException(Path.GetFileName(path) + ": only .xlsx and .xlsm workbooks are supported. Save .xls or .csv as .xlsx first.");
            if (!File.Exists(path)) throw new FileNotFoundException("File not found: " + path, path);
            try
            {
                // FileShare.ReadWrite: a workbook that is open in Excel can still be compared.
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                document = SpreadsheetDocument.Open(stream, false);
                workbook = document.WorkbookPart;
                if (workbook == null || workbook.Workbook == null) throw new InvalidDataException("no workbook part");
            }
            catch (Exception ex) when (!(ex is FileNotFoundException))
            {
                Dispose();
                throw new InvalidDataException(Path.GetFileName(path) + " could not be opened as an Excel workbook: " + ex.Message, ex);
            }

            var properties = workbook.Workbook.GetFirstChild<WorkbookProperties>();
            date1904 = properties != null && properties.Date1904 != null && properties.Date1904.Value;
            var sheetList = workbook.Workbook.GetFirstChild<Sheets>();
            if (sheetList != null)
            {
                foreach (Sheet sheet in sheetList.Elements<Sheet>())
                {
                    if (sheet.Name == null || sheet.Id == null) continue;
                    if (!(workbook.GetPartById(sheet.Id.Value) is WorksheetPart)) continue;      // chart sheets hold no cells
                    sheets[sheet.Name.Value] = sheet;
                    SheetNames.Add(sheet.Name.Value);
                }
            }
            LoadSharedStrings();
            LoadStyles();
        }

        public void Dispose()
        {
            if (document != null) document.Dispose();
            if (stream != null) stream.Dispose();
        }

        // ------------------------------------------------------------------ shared strings and styles
        private void LoadSharedStrings()
        {
            SharedStringTablePart part = workbook.SharedStringTablePart;
            if (part == null || part.SharedStringTable == null) return;
            foreach (SharedStringItem item in part.SharedStringTable.Elements<SharedStringItem>()) sharedStrings.Add(RichText(item));
        }

        /// <summary>Plain text of a shared string, inline string or comment: its own text plus every run; phonetic text is left out.</summary>
        private static string RichText(OpenXmlElement holder)
        {
            if (holder == null) return "";
            var text = new StringBuilder();
            foreach (OpenXmlElement child in holder.ChildElements)
            {
                var plain = child as Text;
                if (plain != null) { text.Append(plain.Text); continue; }
                var run = child as Run;
                if (run == null) continue;
                foreach (Text part in run.Elements<Text>()) text.Append(part.Text);
            }
            return text.ToString();
        }

        private void LoadStyles()
        {
            var described = new List<string[]>();
            var dates = new List<bool>();
            var durations = new List<bool>();
            Stylesheet sheet = workbook.WorkbookStylesPart == null ? null : workbook.WorkbookStylesPart.Stylesheet;
            if (sheet != null)
            {
                // GetFirstChild / Elements find children whatever their order; the typed properties do not.
                var fonts = Children<Fonts, Font>(sheet);
                var fills = Children<Fills, Fill>(sheet);
                var borders = Children<Borders, Border>(sheet);
                var custom = new Dictionary<uint, string>();
                var numberFormats = sheet.GetFirstChild<NumberingFormats>();
                if (numberFormats != null)
                    foreach (NumberingFormat format in numberFormats.Elements<NumberingFormat>())
                        if (format.NumberFormatId != null && format.FormatCode != null) custom[format.NumberFormatId.Value] = format.FormatCode.Value;

                var cellFormats = sheet.GetFirstChild<CellFormats>();
                if (cellFormats != null)
                {
                    foreach (CellFormat xf in cellFormats.Elements<CellFormat>())
                    {
                        var style = new string[IdentityStyle.Labels.Length];
                        DescribeFont(At(fonts, xf.FontId), style);
                        var alignment = xf.GetFirstChild<Alignment>();
                        style[IdentityStyle.HAlign] = alignment != null && alignment.Horizontal != null ? alignment.Horizontal.InnerText : "general";
                        style[IdentityStyle.VAlign] = alignment != null && alignment.Vertical != null ? alignment.Vertical.InnerText : "bottom";
                        style[IdentityStyle.Wrap] = YesNo(alignment != null && alignment.WrapText != null && alignment.WrapText.Value);
                        uint formatId = xf.NumberFormatId == null ? 0 : xf.NumberFormatId.Value;
                        string code;
                        if (!custom.TryGetValue(formatId, out code) && !BuiltInFormats.TryGetValue(formatId, out code)) code = "General";
                        style[IdentityStyle.NumberFormat] = code;
                        style[IdentityStyle.FillColour] = DescribeFill(At(fills, xf.FillId));
                        style[IdentityStyle.Borders] = DescribeBorder(At(borders, xf.BorderId));
                        described.Add(style);
                        string first = FormatNoise.Replace(code.Split(';')[0], "");
                        dates.Add(DateLetters.IsMatch(first));
                        durations.Add(DurationFormat.IsMatch(code.Split(';')[0]));
                    }
                }
            }
            if (described.Count == 0)
            {
                var plain = new string[IdentityStyle.Labels.Length];
                DescribeFont(null, plain);
                plain[IdentityStyle.HAlign] = "general"; plain[IdentityStyle.VAlign] = "bottom"; plain[IdentityStyle.Wrap] = "No";
                plain[IdentityStyle.NumberFormat] = "General"; plain[IdentityStyle.FillColour] = "none"; plain[IdentityStyle.Borders] = "none";
                described.Add(plain); dates.Add(false); durations.Add(false);
            }
            styles = described.ToArray();
            dateStyle = dates.ToArray();
            durationStyle = durations.ToArray();
        }

        private static List<TItem> Children<TList, TItem>(Stylesheet sheet) where TList : OpenXmlElement where TItem : OpenXmlElement
        {
            var list = sheet.GetFirstChild<TList>();
            return list == null ? new List<TItem>() : list.Elements<TItem>().ToList();
        }

        private static T At<T>(List<T> items, UInt32Value index) where T : class
        {
            uint i = index == null ? 0 : index.Value;
            return i < items.Count ? items[(int)i] : null;
        }

        private static string YesNo(bool flag) { return flag ? "Yes" : "No"; }

        private static bool Flag(BooleanPropertyType element)
        {
            return element != null && (element.Val == null || element.Val.Value);       // <b/> means true
        }

        private static void DescribeFont(Font font, string[] style)
        {
            FontName name = font == null ? null : font.GetFirstChild<FontName>();
            FontSize size = font == null ? null : font.GetFirstChild<FontSize>();
            Underline underline = font == null ? null : font.GetFirstChild<Underline>();
            style[IdentityStyle.FontName] = name != null && name.Val != null ? name.Val.Value : "";
            style[IdentityStyle.FontSize] = size != null && size.Val != null ? size.Val.Value.ToString("0.####", CultureInfo.InvariantCulture) : "";
            style[IdentityStyle.Bold] = YesNo(font != null && Flag(font.GetFirstChild<Bold>()));
            style[IdentityStyle.Italic] = YesNo(font != null && Flag(font.GetFirstChild<Italic>()));
            style[IdentityStyle.Underline] = underline == null ? "none" : (underline.Val == null ? "single" : underline.Val.InnerText);
            style[IdentityStyle.FontColour] = DescribeColour(font == null ? null : font.GetFirstChild<Color>());
        }

        private static string DescribeColour(ColorType colour)
        {
            if (colour == null) return "none";
            if (colour.Indexed != null) return "indexed " + colour.Indexed.Value.ToString(CultureInfo.InvariantCulture);
            if (colour.Theme != null)
            {
                string text = "theme " + colour.Theme.Value.ToString(CultureInfo.InvariantCulture);
                if (colour.Tint != null && colour.Tint.Value != 0)
                    text += " tint " + colour.Tint.Value.ToString("+0.00;-0.00", CultureInfo.InvariantCulture);
                return text;
            }
            if (colour.Auto != null) return "auto";
            string rgb = colour.Rgb == null ? null : colour.Rgb.Value;
            if (string.IsNullOrEmpty(rgb)) return "#000000";
            return "#" + (rgb.Length > 6 ? rgb.Substring(rgb.Length - 6) : rgb).ToUpperInvariant();
        }

        private static string DescribeFill(Fill fill)
        {
            if (fill == null) return "none";
            var pattern = fill.GetFirstChild<PatternFill>();
            if (pattern == null) return fill.GetFirstChild<GradientFill>() != null ? "gradient" : "none";
            string kind = pattern.PatternType == null ? null : pattern.PatternType.InnerText;
            if (string.IsNullOrEmpty(kind) || kind == "none") return "none";
            var foreground = pattern.GetFirstChild<ForegroundColor>();
            string colour = foreground == null ? "#000000" : DescribeColour(foreground);
            return kind == "solid" ? colour : kind + " " + colour;
        }

        private static string DescribeBorder(Border border)
        {
            if (border == null) return "none";
            var parts = new List<string>();
            AddSide(parts, "L", border.GetFirstChild<LeftBorder>());
            AddSide(parts, "R", border.GetFirstChild<RightBorder>());
            AddSide(parts, "T", border.GetFirstChild<TopBorder>());
            AddSide(parts, "B", border.GetFirstChild<BottomBorder>());
            return parts.Count == 0 ? "none" : string.Join(" ", parts);
        }

        private static void AddSide(List<string> parts, string tag, BorderPropertiesType side)
        {
            if (side == null || side.Style == null) return;
            string style = side.Style.InnerText;
            if (!string.IsNullOrEmpty(style) && style != "none") parts.Add(tag + ":" + style);
        }

        // ------------------------------------------------------------------ one worksheet
        internal IdentitySheet ReadSheet(string name, Func<bool> cancelled = null)
        {
            Sheet entry;
            if (!sheets.TryGetValue(name, out entry)) throw new ArgumentException("Sheet '" + name + "' not found in " + Path.GetFileName(FilePath));
            var part = (WorksheetPart)workbook.GetPartById(entry.Id.Value);
            var sheet = new IdentitySheet { Name = name, Styles = styles };
            if (entry.State != null && !string.IsNullOrEmpty(entry.State.InnerText)) sheet.State = entry.State.InnerText;

            var columns = new List<Column>();
            var shared = new Dictionary<uint, SharedFormula>();
            using (OpenXmlReader reader = OpenXmlReader.Create(part))
            {
                bool more = reader.Read();
                while (more)
                {
                    if (!reader.IsStartElement) { more = reader.Read(); continue; }
                    Type type = reader.ElementType;
                    if (type == typeof(Row))
                    {
                        if (cancelled != null && cancelled()) throw new OperationCanceledException();
                        ReadRow((Row)reader.LoadCurrentElement(), sheet, shared);
                    }
                    else if (type == typeof(Column)) columns.Add((Column)reader.LoadCurrentElement());
                    else if (type == typeof(Pane))
                    {
                        var pane = (Pane)reader.LoadCurrentElement();
                        if (pane.TopLeftCell != null) sheet.Freeze = pane.TopLeftCell.Value;
                    }
                    else if (type == typeof(MergeCell))
                    {
                        var merge = (MergeCell)reader.LoadCurrentElement();
                        if (merge.Reference != null) sheet.Merged.Add(merge.Reference.Value);
                    }
                    else if (type == typeof(AutoFilter))
                    {
                        var filter = (AutoFilter)reader.LoadCurrentElement();
                        if (filter.Reference != null) sheet.Filter = filter.Reference.Value;
                    }
                    else if (type == typeof(Hyperlink)) ReadHyperlink((Hyperlink)reader.LoadCurrentElement(), part, sheet);
                    else if (type == typeof(DataValidation))
                    {
                        var rule = (DataValidation)reader.LoadCurrentElement();
                        var formula = rule.GetFirstChild<Formula1>();
                        sheet.Validations.Add(((rule.SequenceOfReferences == null ? "" : rule.SequenceOfReferences.InnerText) + ": "
                            + (rule.Type == null ? "none" : rule.Type.InnerText) + " " + (formula == null ? "" : formula.Text)).Trim());
                    }
                    else if (type == typeof(ConditionalFormatting))
                    {
                        var block = (ConditionalFormatting)reader.LoadCurrentElement();
                        string range = block.SequenceOfReferences == null ? "" : block.SequenceOfReferences.InnerText;
                        foreach (ConditionalFormattingRule rule in block.Elements<ConditionalFormattingRule>())
                            sheet.ConditionalFormats.Add(range + ": " + (rule.Type == null ? "" : rule.Type.InnerText));
                    }
                    else { more = reader.Read(); continue; }
                    // LoadCurrentElement leaves the reader on the element after the one it loaded.
                    more = !reader.EOF;
                }
            }

            foreach (Column column in columns)
            {
                int from = column.Min == null ? 1 : (int)column.Min.Value;
                int to = Math.Min(column.Max == null ? from : (int)column.Max.Value, sheet.LastCol);
                for (int i = from; i <= to; i++)
                {
                    if (column.Width != null) sheet.Widths[i] = Math.Round(column.Width.Value, 2);
                    if (column.Hidden != null && column.Hidden.Value) sheet.HiddenColumns.Add(ColumnLetters(i));
                }
            }
            sheet.Validations.Sort(StringComparer.Ordinal);
            sheet.ConditionalFormats.Sort(StringComparer.Ordinal);

            WorksheetCommentsPart commentsPart = part.WorksheetCommentsPart;
            CommentList comments = commentsPart == null || commentsPart.Comments == null ? null : commentsPart.Comments.GetFirstChild<CommentList>();
            if (comments != null)
                foreach (Comment comment in comments.Elements<Comment>())
                    if (comment.Reference != null) sheet.Comments[comment.Reference.Value] = RichText(comment.GetFirstChild<CommentText>()).Trim();
            return sheet;
        }

        private static void ReadHyperlink(Hyperlink link, WorksheetPart part, IdentitySheet sheet)
        {
            if (link.Reference == null) return;
            string target = null;
            if (link.Id != null)
            {
                HyperlinkRelationship relation = part.HyperlinkRelationships.FirstOrDefault(item => item.Id == link.Id.Value);
                if (relation != null && relation.Uri != null) target = relation.Uri.OriginalString;
            }
            if (string.IsNullOrEmpty(target)) target = link.Location == null ? "" : link.Location.Value;
            sheet.Links[link.Reference.Value] = target;
        }

        private sealed class SharedFormula { internal string Text; internal int Row, Col; }

        private void ReadRow(Row row, IdentitySheet sheet, Dictionary<uint, SharedFormula> shared)
        {
            int rowNumber = row.RowIndex == null ? 0 : (int)row.RowIndex.Value;
            if (rowNumber > 0)
            {
                if (row.Height != null) sheet.Heights[rowNumber] = Math.Round(row.Height.Value, 2);
                if (row.Hidden != null && row.Hidden.Value) sheet.HiddenRows.Add(rowNumber);
            }
            int position = 0;
            foreach (Cell source in row.Elements<Cell>())
            {
                position++;
                int r = rowNumber, c = position;
                if (source.CellReference != null && !SplitReference(source.CellReference.Value, out r, out c)) continue;
                if (r <= 0) continue;
                position = c;
                var cell = new IdentityCell { Kind = "blank", Style = source.StyleIndex == null ? 0 : (int)source.StyleIndex.Value };
                if (cell.Style >= styles.Length) cell.Style = 0;

                var formula = source.GetFirstChild<CellFormula>();
                if (formula != null) cell.Formula = FormulaText(formula, r, c, shared);
                ReadValue(source, ref cell);
                if (cell.Formula != null && cell.Value == null) sheet.UncachedFormulas++;

                sheet.Cells[IdentitySheet.Key(r, c)] = cell;
                if (cell.Value != null || cell.Formula != null)
                {
                    if (r > sheet.LastRow) sheet.LastRow = r;
                    if (c > sheet.LastCol) sheet.LastCol = c;
                }
            }
        }

        private static string FormulaText(CellFormula formula, int row, int col, Dictionary<uint, SharedFormula> shared)
        {
            string text = formula.Text ?? "";
            if (formula.FormulaType != null && formula.FormulaType.Value == CellFormulaValues.Shared && formula.SharedIndex != null)
            {
                SharedFormula master;
                if (text.Length > 0) shared[formula.SharedIndex.Value] = new SharedFormula { Text = text, Row = row, Col = col };
                else if (shared.TryGetValue(formula.SharedIndex.Value, out master)) text = ShiftFormula(master.Text, row - master.Row, col - master.Col);
            }
            return "=" + text;
        }

        private void ReadValue(Cell source, ref IdentityCell cell)
        {
            CellValues type = source.DataType == null ? CellValues.Number : source.DataType.Value;
            if (type == CellValues.InlineString)
            {
                cell.Value = RichText(source.GetFirstChild<InlineString>());
                cell.Kind = "text";
                return;
            }
            var holder = source.GetFirstChild<CellValue>();
            string raw = holder == null ? null : holder.Text;
            if (raw == null) return;
            switch (type)
            {
                case CellValues.SharedString:
                    int index;
                    cell.Value = int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out index) && index >= 0 && index < sharedStrings.Count
                        ? sharedStrings[index] : "";
                    cell.Kind = "text";
                    return;
                case CellValues.Boolean:
                    cell.Value = raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase) ? "TRUE" : "FALSE";
                    cell.Kind = "boolean";
                    return;
                case CellValues.Number:
                    double number;
                    if (raw.Length == 0) return;
                    if (!double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out number)) break;
                    if (dateStyle[cell.Style] && TryDate(number, durationStyle[cell.Style], ref cell)) return;
                    cell.Number = number;
                    cell.Value = NumberText(number);
                    cell.Kind = "number";
                    return;
                case CellValues.Date:
                    DateTime moment;
                    if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out moment))
                    {
                        cell.Value = MomentText(moment);
                        cell.Kind = "date";
                        return;
                    }
                    break;
            }
            cell.Value = raw;              // formula strings, errors such as #N/A, anything unparsable
            cell.Kind = "text";
        }

        /// <summary>Excel serial number to text, the way the reference Python tool shows it.</summary>
        private bool TryDate(double serial, bool duration, ref IdentityCell cell)
        {
            try
            {
                double day = Math.Floor(serial);
                long milliseconds = (long)Math.Round((serial - day) * 86400000.0);
                if (duration)
                {
                    TimeSpan span = TimeSpan.FromDays(day) + TimeSpan.FromMilliseconds(milliseconds);
                    cell.Value = ((long)Math.Floor(span.TotalHours)).ToString(CultureInfo.InvariantCulture) + span.ToString(@"\:mm\:ss", CultureInfo.InvariantCulture);
                    cell.Kind = "duration";
                    return true;
                }
                if (serial >= 0 && serial < 1 && milliseconds < 86400000)
                {
                    cell.Value = TimeSpan.FromMilliseconds(milliseconds).ToString(milliseconds % 1000 == 0 ? @"hh\:mm\:ss" : @"hh\:mm\:ss\.ffffff", CultureInfo.InvariantCulture);
                    cell.Kind = "time";
                    return true;
                }
                if (!date1904 && serial > 0 && serial < 60) day += 1;                  // Excel's 1900 leap-year quirk
                DateTime epoch = date1904 ? new DateTime(1904, 1, 1) : new DateTime(1899, 12, 30);
                cell.Value = MomentText(epoch.AddDays(day).AddMilliseconds(milliseconds));
                cell.Kind = "date";
                return true;
            }
            catch (ArgumentOutOfRangeException) { return false; }
            catch (OverflowException) { return false; }
        }

        private static string MomentText(DateTime moment)
        {
            if (moment.TimeOfDay == TimeSpan.Zero) return moment.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return moment.ToString(moment.Millisecond == 0 ? "yyyy-MM-dd HH:mm:ss" : "yyyy-MM-dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
        }

        internal static string NumberText(double number)
        {
            if (number == Math.Floor(number) && Math.Abs(number) < 1e15) return ((long)number).ToString(CultureInfo.InvariantCulture);
            return number.ToString("G15", CultureInfo.InvariantCulture).Replace("E", "e");
        }

        // ------------------------------------------------------------------ cell references and shared formulas
        internal static string ColumnLetters(int column)
        {
            string letters = "";
            while (column > 0)
            {
                int rest = (column - 1) % 26;
                letters = (char)('A' + rest) + letters;
                column = (column - 1) / 26;
            }
            return letters;
        }

        internal static int ColumnNumber(string letters)
        {
            int number = 0;
            foreach (char ch in letters) number = number * 26 + (char.ToUpperInvariant(ch) - 'A' + 1);
            return number;
        }

        /// <summary>"BC12" -> row 12, column 55. False when the text is not a single cell reference.</summary>
        internal static bool SplitReference(string reference, out int row, out int col)
        {
            row = 0; col = 0;
            if (string.IsNullOrEmpty(reference)) return false;
            int i = 0;
            while (i < reference.Length && char.IsLetter(reference[i])) i++;
            if (i == 0 || i > 3 || i == reference.Length) return false;
            col = ColumnNumber(reference.Substring(0, i));
            return int.TryParse(reference.Substring(i), NumberStyles.None, CultureInfo.InvariantCulture, out row) && row > 0;
        }

        private static readonly Regex CellRef = new Regex(@"\G(\$?)([A-Za-z]{1,3})(\$?)(\d{1,7})(?![A-Za-z0-9_(])", RegexOptions.Compiled);
        private static readonly Regex ColumnRange = new Regex(@"\G(\$?)([A-Za-z]{1,3}):(\$?)([A-Za-z]{1,3})(?![A-Za-z0-9_(])", RegexOptions.Compiled);
        private static readonly Regex RowRange = new Regex(@"\G(\$?)(\d{1,7}):(\$?)(\d{1,7})(?![A-Za-z0-9_(.])", RegexOptions.Compiled);

        /// <summary>
        /// Excel stores a filled-down formula once ("shared") and leaves the other cells empty. This rebuilds a
        /// dependent cell's formula by moving every relative reference of the master formula; $ parts stay fixed.
        /// </summary>
        internal static string ShiftFormula(string formula, int rows, int cols)
        {
            if (rows == 0 && cols == 0) return formula;
            var result = new StringBuilder(formula.Length + 8);
            int i = 0;
            while (i < formula.Length)
            {
                char ch = formula[i];
                if (ch == '"' || ch == '\'')                        // text literal or quoted sheet name: copy unchanged
                {
                    int end = i + 1;
                    while (end < formula.Length)
                    {
                        if (formula[end] == ch)
                        {
                            if (end + 1 < formula.Length && formula[end + 1] == ch) { end += 2; continue; }
                            break;
                        }
                        end++;
                    }
                    end = Math.Min(end + 1, formula.Length);
                    result.Append(formula, i, end - i);
                    i = end;
                    continue;
                }
                bool boundary = i == 0 || !(char.IsLetterOrDigit(formula[i - 1]) || formula[i - 1] == '_' || formula[i - 1] == '.' || formula[i - 1] == '$');
                if (boundary && (ch == '$' || char.IsLetterOrDigit(ch)))
                {
                    Match match = ColumnRange.Match(formula, i);
                    if (match.Success)
                    {
                        result.Append(ShiftColumn(match.Groups[1].Value, match.Groups[2].Value, cols)).Append(':')
                              .Append(ShiftColumn(match.Groups[3].Value, match.Groups[4].Value, cols));
                        i += match.Length;
                        continue;
                    }
                    match = RowRange.Match(formula, i);
                    if (match.Success)
                    {
                        result.Append(ShiftRow(match.Groups[1].Value, match.Groups[2].Value, rows)).Append(':')
                              .Append(ShiftRow(match.Groups[3].Value, match.Groups[4].Value, rows));
                        i += match.Length;
                        continue;
                    }
                    match = CellRef.Match(formula, i);
                    if (match.Success && ColumnNumber(match.Groups[2].Value) <= 16384)
                    {
                        string column = ShiftColumn(match.Groups[1].Value, match.Groups[2].Value, cols);
                        string row = ShiftRow(match.Groups[3].Value, match.Groups[4].Value, rows);
                        result.Append(column == "#REF!" || row == "#REF!" ? "#REF!" : column + row);
                        i += match.Length;
                        continue;
                    }
                    // A name or function: copy the whole word so that its tail is not read as a reference.
                    int stop = i;
                    while (stop < formula.Length && (char.IsLetterOrDigit(formula[stop]) || formula[stop] == '_' || formula[stop] == '.' || formula[stop] == '$')) stop++;
                    if (stop == i) stop = i + 1;
                    result.Append(formula, i, stop - i);
                    i = stop;
                    continue;
                }
                result.Append(ch);
                i++;
            }
            return result.ToString();
        }

        private static string ShiftColumn(string dollar, string letters, int by)
        {
            if (dollar.Length > 0) return dollar + letters;
            int moved = ColumnNumber(letters) + by;
            return moved < 1 || moved > 16384 ? "#REF!" : ColumnLetters(moved);
        }

        private static string ShiftRow(string dollar, string digits, int by)
        {
            if (dollar.Length > 0) return dollar + digits;
            int moved = int.Parse(digits, CultureInfo.InvariantCulture) + by;
            return moved < 1 || moved > 1048576 ? "#REF!" : moved.ToString(CultureInfo.InvariantCulture);
        }
    }
}
