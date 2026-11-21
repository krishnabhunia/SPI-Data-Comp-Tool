using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Literal-text, bounded, two-worksheet export. Never saves either input workbook.</summary>
    internal static class ExcelAuditReportWriter
    {
        private static readonly string[] DetailHeaders =
        {
            "No.", "Category", "Difference", "Primary key", "File A cell", "File B cell",
            "File A column", "File B column", "File A value", "File B value", "Details"
        };

        internal static void Write(ExcelAuditResult result, string outputPath, bool overwrite, CancellationToken token)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (result.IsValidationOnly) throw new ExcelAuditException("Run a comparison before exporting; validation alone is not an equality result.");
            token.ThrowIfCancellationRequested();
            string destination = FormValidation.OutputFile(outputPath, null, "audit report", ".xlsx");
            FormValidation.DifferentFiles(destination, result.Options.FileA, null);
            FormValidation.DifferentFiles(destination, result.Options.FileB, null);
            RejectLinkedPath(destination);
            RejectLinkedPath(result.Options.FileA);
            RejectLinkedPath(result.Options.FileB);
            if (File.Exists(destination) && !overwrite)
                throw new ExcelAuditException("The report already exists. Confirm replacement or choose another report path.");
            if (File.Exists(destination) && (File.GetAttributes(destination) & FileAttributes.ReparsePoint) != 0)
                throw new ExcelAuditException("The report path is a file link. Choose a regular output file.");
            if (result.DataDetails.Count > 1048000 || result.OtherDetails.Count > 1048000)
                throw new ExcelAuditException("The detail section exceeds Excel's row limit. Reduce the detail limit and run again.");

            string temporary = Path.Combine(Path.GetDirectoryName(destination), ".spi-audit-" + Guid.NewGuid().ToString("N") + ".xlsx");
            bool owned = false;
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
                {
                    owned = true;
                    using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                    {
                        var workbook = document.AddWorkbookPart();
                        workbook.Workbook = new Workbook(new BookViews(new WorkbookView()));
                        var styles = workbook.AddNewPart<WorkbookStylesPart>();
                        styles.Stylesheet = Styles();
                        styles.Stylesheet.Save();
                        var sheets = workbook.Workbook.AppendChild(new Sheets());
                        WriteSheet(workbook, sheets, "Summary", 1U, result, false, token);
                        WriteSheet(workbook, sheets, "Data Differences", 2U, result, true, token);
                        workbook.Workbook.Save();
                    }
                }
                token.ThrowIfCancellationRequested();
                // Move is deliberately used without replacement unless the user explicitly allowed it.
                // A destination created by somebody else during export is not silently overwritten.
                if (overwrite && File.Exists(destination)) File.Replace(temporary, destination, null);
                else File.Move(temporary, destination);
            }
            finally
            {
                if (owned && File.Exists(temporary))
                {
                    try { File.Delete(temporary); }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            }
        }

        private static void RejectLinkedPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ExcelAuditException("An audit input/output path is missing.");
            var file = new FileInfo(Path.GetFullPath(path));
            if (file.Exists && (file.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new ExcelAuditException("Audit export requires regular input/output files, not linked file aliases.");
            for (DirectoryInfo folder = file.Directory; folder != null; folder = folder.Parent)
                if ((folder.Attributes & FileAttributes.ReparsePoint) != 0)
                    throw new ExcelAuditException("Audit export cannot safely use paths through linked folders or junctions. Choose regular paths for both inputs and the report.");
        }

        private static void WriteSheet(WorkbookPart workbook, Sheets sheets, string name, uint id,
            ExcelAuditResult result, bool data, CancellationToken token)
        {
            var part = workbook.AddNewPart<WorksheetPart>();
            sheets.Append(new Sheet { Id = workbook.GetIdOfPart(part), SheetId = id, Name = name });
            var overview = data ? DataOverview(result) : SummaryOverview(result);
            uint detailHeaderRow = (uint)overview.Count + 2U;
            var findings = data ? result.DataDetails : result.OtherDetails;
            using (var writer = OpenXmlWriter.Create(part))
            {
                writer.WriteStartElement(new Worksheet());
                writer.WriteElement(new SheetViews(new SheetView(new Pane
                {
                    VerticalSplit = data ? detailHeaderRow : 1U,
                    TopLeftCell = "A" + (data ? detailHeaderRow + 1U : 2U).ToString(CultureInfo.InvariantCulture),
                    ActivePane = PaneValues.BottomLeft,
                    State = PaneStateValues.Frozen
                }) { WorkbookViewId = 0U }));
                var columns = new Columns();
                double[] widths = { 7, 18, 26, 28, 18, 18, 24, 24, 38, 38, 64 };
                for (int i = 0; i < widths.Length; i++)
                    columns.Append(new Column { Min = (uint)i + 1U, Max = (uint)i + 1U, Width = widths[i], CustomWidth = true });
                writer.WriteElement(columns);
                writer.WriteStartElement(new SheetData());
                uint row = 1;
                foreach (string[] line in overview)
                {
                    token.ThrowIfCancellationRequested();
                    string[] cells = row == 1 ? line : new[] { line[0], string.Empty, string.Join("  |  ", line.Skip(1)) };
                    WriteRow(writer, row, cells, row == 1 ? 2U : 0U, row == 1 ? 29 : 36);
                    row++;
                }
                row++;
                WriteRow(writer, row++, DetailHeaders, 1U, 32);
                long number = 0;
                foreach (ExcelAuditFinding finding in findings)
                {
                    token.ThrowIfCancellationRequested();
                    WriteRow(writer, row++, new[]
                    {
                        (++number).ToString(CultureInfo.InvariantCulture), finding.Category.ToString(), finding.Difference,
                        finding.Key, finding.CellA, finding.CellB, finding.ColumnA, finding.ColumnB,
                        finding.ValueA, finding.ValueB, finding.Details
                    }, 0U, 46);
                }
                writer.WriteEndElement();
                writer.WriteElement(new AutoFilter { Reference = "A" + detailHeaderRow.ToString(CultureInfo.InvariantCulture)
                    + ":K" + Math.Max(detailHeaderRow, row - 1U).ToString(CultureInfo.InvariantCulture) });
                var merges = new MergeCells(new MergeCell { Reference = "A1:K1" });
                for (uint i = 2; i <= overview.Count; i++)
                {
                    string r = i.ToString(CultureInfo.InvariantCulture);
                    merges.Append(new MergeCell { Reference = "A" + r + ":B" + r });
                    merges.Append(new MergeCell { Reference = "C" + r + ":K" + r });
                }
                writer.WriteElement(merges);
                writer.WriteEndElement();
            }
        }

        private static List<string[]> DataOverview(ExcelAuditResult result)
        {
            return new List<string[]>
            {
                new[] { "Excel Sheet Audit — Data Differences" },
                new[] { "File A", result.Options.FileA, "Worksheet", result.Options.SheetA },
                new[] { "File B", result.Options.FileB, "Worksheet", result.Options.SheetB },
                new[] { "Matching", Matching(result.Options) },
                new[] { "Details", DetailStatus(result.DataCount, result.DataDetails.Count),
                    "Scope", "Only changed values, types, formulas and unmatched records. Non-data findings are in Summary." },
                new[] { "Long report text", "Individual report cells are limited to 32,767 characters; shortened values carry [TRUNCATED]." }
            };
        }

        private static List<string[]> SummaryOverview(ExcelAuditResult result)
        {
            ExcelAuditOptions options = result.Options;
            return new List<string[]>
            {
                new[] { "Excel Sheet Audit — Summary" },
                new[] { "Overall result", !result.Complete ? "INCOMPLETE — review scope warnings"
                    : result.Matches ? "Matches under selected rules" : "Differences found" },
                new[] { "File A", options.FileA, "Worksheet", options.SheetA, "Header row", options.HeaderRowA.ToString(CultureInfo.InvariantCulture) },
                new[] { "File B", options.FileB, "Worksheet", options.SheetB, "Header row", options.HeaderRowB.ToString(CultureInfo.InvariantCulture) },
                new[] { "Started (UTC)", result.StartedUtc.ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture),
                    "Elapsed", result.Elapsed.ToString("c", CultureInfo.InvariantCulture) },
                new[] { "Matching method", Matching(options) },
                new[] { "Column names", "Ordinal case-insensitive matching; case/name changes are separate header findings." },
                new[] { "Values and keys", "Exact typed values. Text case and whitespace are significant. No rounding tolerance is applied." },
                new[] { "Rows / records A", result.RowsA.ToString(CultureInfo.InvariantCulture), "Rows / records B", result.RowsB.ToString(CultureInfo.InvariantCulture) },
                new[] { "Matched rows", result.MatchedRows.ToString(CultureInfo.InvariantCulture), "Only A", result.OnlyA.ToString(CultureInfo.InvariantCulture),
                    "Only B", result.OnlyB.ToString(CultureInfo.InvariantCulture) },
                new[] { "Values / records", Status(options.CompareValues, result.DataCount) },
                new[] { "Headers", Status(options.CompareHeaders, result.HeaderCount) },
                new[] { "Cell formatting", Status(options.CompareFormatting, result.FormattingCount) },
                new[] { "Worksheet layout", Status(options.CompareLayout, result.StructureCount) },
                new[] { "Scope warnings", result.ScopeCount.ToString(CultureInfo.InvariantCulture) },
                new[] { "Data detail output", DetailStatus(result.DataCount, result.DataDetails.Count), "Worksheet", "Data Differences" },
                new[] { "Other detail output", DetailStatus(result.OtherCount, result.OtherDetails.Count), "Worksheet", "Summary" },
                new[] { "Detail limits", options.MaxDetails.ToString(CultureInfo.InvariantCulture) + " findings and "
                    + options.MaxDetailBytes.ToString(CultureInfo.InvariantCulture) + " estimated bytes per section; full counts retained." },
                new[] { "Checked scope", "Named table columns at/below each selected header row, plus reported worksheet/workbook metadata. This is not whole-workbook, byte-for-byte or pixel-perfect identity." },
                new[] { "Not checked", "Charts, images, macros, comments, external connections, print settings and calculated formula results. No formulas or macros are executed." },
                new[] { "Long report text", "Individual report cells are limited to 32,767 characters; shortened values carry [TRUNCATED]." },
                new[] { "Other differences", "Header, formatting, layout and scope findings follow. Data findings are never mixed into this table." }
            };
        }

        internal static string Matching(ExcelAuditOptions options)
        {
            return options.UseKeys
                ? "Primary key: (" + string.Join(", ", options.KeysA) + ") → (" + string.Join(", ", options.KeysB)
                    + "); record order ignored; original Excel coordinates retained."
                : "Row by row: data row index = Excel row minus selected header row (A: "
                    + options.HeaderRowA.ToString(CultureInfo.InvariantCulture) + "; B: "
                    + options.HeaderRowB.ToString(CultureInfo.InvariantCulture) + "). No sorting or key inference.";
        }

        private static string Status(bool enabled, long count)
        {
            return !enabled ? "NOT CHECKED" : count == 0 ? "No differences found in checked scope"
                : count.ToString(CultureInfo.InvariantCulture) + " differences";
        }

        private static string DetailStatus(long total, int written)
        {
            long omitted = total - written;
            return written.ToString(CultureInfo.InvariantCulture) + " shown of " + total.ToString(CultureInfo.InvariantCulture)
                + (omitted > 0 ? "; WARNING: " + omitted.ToString(CultureInfo.InvariantCulture) + " detail findings omitted by limits." : "; none omitted.");
        }

        private static void WriteRow(OpenXmlWriter writer, uint number, IEnumerable<string> values, uint style, double height)
        {
            writer.WriteStartElement(new Row { RowIndex = number, Height = height, CustomHeight = true });
            int column = 0;
            foreach (string value in values)
            {
                // InlineString makes =, +, -, @ and formula-shaped source text inert in the report.
                writer.WriteElement(new Cell
                {
                    CellReference = ColumnName(++column) + number.ToString(CultureInfo.InvariantCulture),
                    StyleIndex = style,
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString(new Text(SafeText(value)) { Space = SpaceProcessingModeValues.Preserve })
                });
            }
            writer.WriteEndElement();
        }

        internal static string ColumnName(int column)
        {
            string value = string.Empty;
            while (column > 0) { column--; value = (char)('A' + column % 26) + value; column /= 26; }
            return value;
        }

        private static string SafeText(string value)
        {
            value = value ?? string.Empty;
            var clean = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                if (char.IsHighSurrogate(character) && i + 1 < value.Length && char.IsLowSurrogate(value[i + 1]))
                {
                    clean.Append(character).Append(value[++i]);
                    continue;
                }
                bool valid = !char.IsSurrogate(character) && (character == '\t' || character == '\n' || character == '\r'
                    || character >= 0x20 && character != 0xfffe && character != 0xffff);
                clean.Append(valid ? character : '\ufffd');
            }
            value = clean.ToString();
            const string suffix = " [TRUNCATED]";
            if (value.Length <= 32767) return value;
            int length = 32767 - suffix.Length;
            if (char.IsHighSurrogate(value[length - 1])) length--;
            return value.Substring(0, length) + suffix;
        }

        private static Stylesheet Styles()
        {
            var fonts = new Fonts(
                new Font(new FontSize { Val = 11D }, new FontName { Val = "Calibri" }),
                new Font(new Bold(), new FontSize { Val = 11D }, new Color { Rgb = "FFFFFFFF" }, new FontName { Val = "Calibri" }),
                new Font(new Bold(), new FontSize { Val = 16D }, new Color { Rgb = "FF0B4F9E" }, new FontName { Val = "Calibri" })) { Count = 3U };
            var fills = new Fills(
                new Fill(new PatternFill { PatternType = PatternValues.None }),
                new Fill(new PatternFill { PatternType = PatternValues.Gray125 }),
                new Fill(new PatternFill(new ForegroundColor { Rgb = "FF0B5CAD" }, new BackgroundColor { Indexed = 64U })
                    { PatternType = PatternValues.Solid })) { Count = 3U };
            var borders = new Borders(new Border(new LeftBorder(), new RightBorder(), new TopBorder(), new BottomBorder(), new DiagonalBorder())) { Count = 1U };
            var formats = new CellFormats { Count = 3U };
            for (uint i = 0; i < 3U; i++)
                formats.Append(new CellFormat(new Alignment { Horizontal = HorizontalAlignmentValues.Center,
                    Vertical = VerticalAlignmentValues.Center, WrapText = true })
                { FontId = i, FillId = i == 1U ? 2U : 0U, BorderId = 0U, NumberFormatId = 0U,
                    ApplyFont = true, ApplyFill = true, ApplyAlignment = true, FormatId = 0U });
            return new Stylesheet(fonts, fills, borders, new CellStyleFormats(new CellFormat()) { Count = 1U }, formats,
                new CellStyles(new CellStyle { Name = "Normal", FormatId = 0U, BuiltinId = 0U }) { Count = 1U });
        }
    }
}
