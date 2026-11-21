using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Validation;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Real temporary Open XML fixtures; offline, no customer files and no database connections.</summary>
    internal static class ExcelAuditChecks
    {
        private sealed class FixtureCell
        {
            internal object Value;
            internal string Kind;
            internal uint Style;
        }

        internal static int Run(string logFile)
        {
            var messages = new List<string>();
            string directory = Path.Combine(Path.GetTempPath(), "spi-audit-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                Action<string, Action> check = (name, action) =>
                {
                    try { action(); messages.Add("PASS " + name); }
                    catch (Exception ex) { messages.Add("FAIL " + name + ": " + ex); }
                };
                string a = Path.Combine(directory, "A.xlsx");
                string b = Path.Combine(directory, "B.xlsx");
                Func<ExcelAuditOptions> options = () => new ExcelAuditOptions
                { FileA = a, FileB = b, SheetA = "Data", SheetB = "Data", TempDirectory = directory };

                check("Identical worksheets and validation-only scan", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "Alpha"), Row("2", 20)));
                    Make(b, Rows(Row("ID", "Value"), Row("1", "Alpha"), Row("2", 20)));
                    var result = Compare(options());
                    Require(result.Matches && result.TotalCount == 0 && result.MatchedRows == 2, "Identical fixture did not match.");
                    var validation = ExcelAuditEngine.Validate(options(), CancellationToken.None);
                    Require(validation.IsValidationOnly && !validation.Matches && validation.DataCount == 0 && validation.RowsA == 2,
                        "Validation did not scan safely or claimed comparison equality.");
                });
                check("Shared-string storage and inline strings compare identically", () =>
                {
                    object[][] rows = Rows(Row("ID", "Value"), Row("1", "Unicode \u0995\u09c3\u09b7\u09cd\u09a3"), Row("2", "Unicode \u0995\u09c3\u09b7\u09cd\u09a3"));
                    Make(a, rows); Make(b, rows); UseSharedStrings(b);
                    Require(Compare(options()).Matches, "Shared-string values/indexes differ from equivalent inline text.");
                    EditWorksheet(b, part => part.Worksheet.Descendants<Cell>().Last().CellValue = new CellValue("99999"));
                    Rejected(() => Compare(options()));
                });
                check("Positional comparison does not sort records", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "Alpha"), Row("2", "Beta")));
                    Make(b, Rows(Row("ID", "Value"), Row("2", "Beta"), Row("1", "Alpha")));
                    Require(Compare(options()).DataCount == 4, "Positional record reordering was hidden.");
                });
                check("Positional missing rows include explicit and sparse interior blanks", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "Alpha"), Row("2", "Beta"), Row("3", "Gamma")));
                    Make(b, Rows(Row("ID", "Value"), Row("1", "Alpha"), Row(null, null), Row("3", "Gamma")));
                    var r = Compare(options());
                    Require(r.RowsA == 3 && r.RowsB == 2 && r.MatchedRows == 2 && r.OnlyA == 1 && r.OnlyB == 0,
                        "An explicit blank interior row broke record counts.");
                    EditWorksheet(b, part => part.Worksheet.GetFirstChild<SheetData>().Elements<DocumentFormat.OpenXml.Spreadsheet.Row>()
                        .Single(row => row.RowIndex.Value == 3U).Remove());
                    r = Compare(options());
                    Require(r.OnlyA == 1 && r.MatchedRows == 2 && r.DataCount > 0, "An implicit blank interior row broke record counts.");
                });
                check("Single key mapping ignores header case and preserves original addresses", () =>
                {
                    Make(a, Rows(Row("a1", "Name"), Row("1", "Alpha"), Row("2", "Beta")));
                    Make(b, Rows(Row("b1", "name"), Row("2", "Beta"), Row("1", "Alpha")));
                    var o = options(); o.KeysA = new[] { "A1" }; o.KeysB = new[] { "B1" };
                    var result = Compare(o);
                    Require(result.DataCount == 0 && result.HeaderCount >= 2 && result.MatchedRows == 2,
                        "Mapped key names or header case prevented correct row alignment.");
                    o.CompareHeaders = false;
                    Require(Compare(o).Matches, "Excluded header-only differences polluted value equality.");
                });
                check("Composite keys align records and expose missing/additional records", () =>
                {
                    Make(a, Rows(Row("a2", "a3", "Amount"), Row("101", "INR", 10), Row("102", "INR", 20), Row("103", "INR", 30)));
                    Make(b, Rows(Row("b2", "b3", "Amount"), Row("102", "INR", 25), Row("101", "INR", 10), Row("104", "INR", 40)));
                    var o = options(); o.KeysA = new[] { "A2", "A3" }; o.KeysB = new[] { "B2", "B3" };
                    var result = Compare(o);
                    Require(result.MatchedRows == 2 && result.OnlyA == 1 && result.OnlyB == 1 && result.DataCount == 3,
                        "Composite match or unmatched-row counts are wrong.");
                    Require(result.DataDetails.Any(f => f.CellA == "C3" && f.CellB == "C2"), "Original reordered cell addresses were lost.");
                });
                check("Composite components may repeat; encoded tuples do not collide", () =>
                {
                    object[][] rows = Rows(Row("K1", "K2", "Value"), Row("a|b", "c", "One"), Row("a", "b|c", "Two"), Row("a", "c", "Three"));
                    Make(a, rows); Make(b, rows);
                    var o = options(); o.KeysA = new[] { "K1", "K2" }; o.KeysB = new[] { "K1", "K2" };
                    Require(Compare(o).Matches, "Tuple encoding collided or individual repeated components were rejected.");
                });
                check("Null, whitespace, duplicate and formula primary keys are rejected", () =>
                {
                    var o = options(); o.KeysA = new[] { "ID" }; o.KeysB = new[] { "ID" };
                    Make(b, Rows(Row("ID", "Value"), Row("1", "A"), Row("2", "B")));
                    foreach (object bad in new object[] { null, "  ", F("1+1") })
                    {
                        Make(a, Rows(Row("ID", "Value"), Row(bad, "A")));
                        Rejected(() => Compare(o));
                    }
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A"), Row("1", "B")));
                    Rejected(() => Compare(o));
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(b, Rows(Row("ID", "Value"), Row("1", "A"), Row("1", "B")));
                    Rejected(() => Compare(o));
                });
                check("Key definitions require both sides and equal distinct components", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(b, Rows(Row("ID", "Value"), Row("1", "A")));
                    var o = options(); o.KeysA = new[] { "ID" }; Rejected(() => Compare(o));
                    o.KeysB = new[] { "ID", "Value" }; Rejected(() => Compare(o));
                    o.KeysA = new[] { "ID", "id" }; Rejected(() => Compare(o));
                    o.KeysA = new[] { "Missing" }; o.KeysB = new[] { "ID" }; Rejected(() => Compare(o));
                });
                check("Blank and case-ambiguous headers are not silently skipped", () =>
                {
                    Make(b, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(a, Rows(Row("ID", "id"), Row("1", "A"))); Rejected(() => Compare(options()));
                    Make(a, Rows(Row("ID", null), Row("1", "A"))); Rejected(() => Compare(options()));
                });
                check("Key values are case-sensitive and typed; leading zeros remain text", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("abc", "A"), Row("001", "B"), Row(1, "C")));
                    Make(b, Rows(Row("ID", "Value"), Row("ABC", "A"), Row("001", "B"), Row("1", "C")));
                    var o = options(); o.KeysA = new[] { "ID" }; o.KeysB = new[] { "ID" };
                    var r = Compare(o);
                    Require(r.MatchedRows == 1 && r.OnlyA == 2 && r.OnlyB == 2, "Key case/types were silently normalized.");
                });
                check("Exact numeric canonicalization and text whitespace", () =>
                {
                    Make(a, Rows(Row("ID", "N", "Text"), Row("1", N("1.00"), "a"), Row("2", N("1234567890123451"), "b")));
                    Make(b, Rows(Row("ID", "N", "Text"), Row("1", N("1e0"), " a"), Row("2", N("1234567890123452"), "b")));
                    Require(Compare(options()).DataCount == 2, "Numbers were rounded or lexical equivalents miscompared.");
                });
                check("Cell types and saved formulas are compared without evaluation", () =>
                {
                    Make(a, Rows(Row("Value"), Row(1), Row(F("1+1"))));
                    Make(b, Rows(Row("Value"), Row("1"), Row(F("2+0"))));
                    Require(Compare(options()).DataCount == 2, "Type/formula differences were lost.");
                });
                check("Semantic font and alignment differences, not workbook style indexes", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(b, Rows(Row("ID", "Value"), Row("1", S("A", 2U))));
                    Require(Compare(options()).FormattingCount == 0, "Equivalent formats with different IDs were treated as different.");
                    Make(b, Rows(Row("ID", "Value"), Row("1", S("A", 1U))));
                    var r = Compare(options());
                    Require(r.DataCount == 0 && r.FormattingCount >= 2, "Font/alignment changes were not separated from data.");
                });
                check("Column order is structural, not a data mismatch", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(b, Rows(Row("Value", "ID"), Row("A", "1")));
                    var r = Compare(options());
                    Require(r.DataCount == 0 && r.HeaderCount > 0, "Column name alignment failed.");
                });
                check("Header row height and visibility are checked as layout", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(b, Rows(Row("ID", "Value"), Row("1", "A")));
                    EditWorksheet(b, part =>
                    {
                        var row = part.Worksheet.GetFirstChild<SheetData>().Elements<DocumentFormat.OpenXml.Spreadsheet.Row>().First();
                        row.Height = 35D; row.CustomHeight = true; row.Hidden = true;
                    });
                    var r = Compare(options());
                    Require(r.StructureCount >= 2 && r.DataCount == 0 && !r.Matches, "Header row layout was silently skipped.");
                });
                check("Column-inherited formatting reaches omitted blank data cells", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", null)));
                    Make(b, Rows(Row("ID", "Value"), Row("1", null)));
                    EditWorksheet(b, part => part.Worksheet.PrependChild(new Columns(new Column { Min = 2U, Max = 2U, Style = 1U })));
                    var r = Compare(options());
                    Require(r.FormattingCount > 0 && r.OtherDetails.Any(item => item.Category == ExcelAuditCategory.Formatting && item.CellB == "B2"),
                        "An omitted cell lost its column-inherited formatting.");
                });
                check("Unmatched key records cannot certify a layout-only match", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(b, Rows(Row("ID", "Value"), Row("2", "A")));
                    var o = options(); o.KeysA = new[] { "ID" }; o.KeysB = new[] { "ID" };
                    o.CompareValues = o.CompareFormatting = o.CompareHeaders = false;
                    var r = Compare(o);
                    Require(!r.Complete && !r.Matches && r.OnlyA == 1 && r.OnlyB == 1, "Unaligned layout was claimed equal.");
                });
                check("Unpaired columns cannot silently establish a full match", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(b, Rows(Row("ID", "Value", "Extra"), Row("1", "A", "Hidden difference")));
                    var o = options(); o.CompareHeaders = false;
                    var r = Compare(o);
                    Require(!r.Matches && (r.DataCount > 0 || !r.Complete), "An unpaired populated column was silently ignored.");
                });
                check("Independent data/other detail caps preserve full counts", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A"), Row("2", "B")));
                    Make(b, Rows(Row("id", "value"), Row("1", S("X", 1U)), Row("2", "Y")));
                    var o = options(); o.MaxDetails = 1;
                    var r = Compare(o);
                    Require(r.DataCount == 2 && r.OtherCount >= 2 && r.DataDetails.Count == 1 && r.OtherDetails.Count == 1,
                        "A capped section changed counts or consumed the other section's allowance.");
                });
                check("Worksheet selection and independent header rows", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(b, Rows(Row("Report title"), Row("ID", "Value"), Row("1", "A")));
                    var o = options(); o.HeaderRowB = 2;
                    Require(Compare(o).DataCount == 0, "Logical rows did not align below different headers.");
                    o.SheetB = "Missing sheet"; Rejected(() => Compare(o));
                });
                check("Dates with different workbook date systems normalize consistently", () =>
                {
                    double serial = new DateTime(2026, 11, 21).ToOADate();
                    Make(a, Rows(Row("Date"), Row(S(N(serial.ToString("R", CultureInfo.InvariantCulture)), 3U))));
                    Make(b, Rows(Row("Date"), Row(S(N((serial - 1462D).ToString("R", CultureInfo.InvariantCulture)), 3U))), true);
                    Require(Compare(options()).DataCount == 0, "1900/1904 date systems changed an equal actual date.");
                });
                check("Elapsed durations do not shift with workbook date systems", () =>
                {
                    Make(a, Rows(Row("Duration"), Row(S(N("2.5"), 3U))));
                    Make(b, Rows(Row("Duration"), Row(S(N("2.5"), 3U))), true);
                    foreach (string path in new[] { a, b })
                        using (var document = SpreadsheetDocument.Open(path, true))
                        {
                            var styles = document.WorkbookPart.WorkbookStylesPart;
                            styles.Stylesheet.CellFormats.Elements<CellFormat>().ElementAt(3).NumberFormatId = 46U;
                            styles.Stylesheet.Save();
                        }
                    Require(Compare(options()).DataCount == 0, "Elapsed duration was incorrectly treated as a calendar date.");
                });
                check("High-precision ISO dates cannot round into a false match", () =>
                {
                    Make(a, Rows(Row("Timestamp"), Row(D("2026-11-21T08:00:00.12345671"))));
                    Make(b, Rows(Row("Timestamp"), Row(D("2026-11-21T08:00:00.12345672"))));
                    var r = Compare(options());
                    Require(r.DataCount == 1 && !r.Complete && !r.Matches, "Sub-tick ISO precision was silently rounded away.");
                });
                check("Cancellation is propagated and comparison inputs remain unchanged", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A")));
                    Make(b, Rows(Row("ID", "Value"), Row("1", "B")));
                    byte[] beforeA = File.ReadAllBytes(a), beforeB = File.ReadAllBytes(b);
                    Compare(options());
                    Require(beforeA.SequenceEqual(File.ReadAllBytes(a)) && beforeB.SequenceEqual(File.ReadAllBytes(b)), "An input was modified.");
                    using (var cancel = new CancellationTokenSource())
                    {
                        cancel.Cancel();
                        bool cancelled = false;
                        try { ExcelAuditEngine.Compare(options(), cancel.Token); }
                        catch (OperationCanceledException) { cancelled = true; }
                        Require(cancelled, "Cancellation was swallowed.");
                    }
                });
                check("Partition-budget failure and mid-run cancellation clean their working files", () =>
                {
                    Make(a, Rows(Row("ID", "Value"), Row("1", "A"), Row("2", "B")));
                    Make(b, Rows(Row("ID", "Value"), Row("1", "A"), Row("2", "B")));
                    var o = options(); o.KeysA = new[] { "ID" }; o.KeysB = new[] { "ID" }; o.MaxPartitionBytes = 1;
                    string[] before = Directory.GetFileSystemEntries(directory).OrderBy(path => path).ToArray();
                    Rejected(() => Compare(o));
                    Require(before.SequenceEqual(Directory.GetFileSystemEntries(directory).OrderBy(path => path)), "Budget failure left audit temporary files.");
                    o.MaxPartitionBytes = 128L * 1024 * 1024;
                    using (var cancel = new CancellationTokenSource())
                    {
                        bool cancelled = false;
                        try
                        {
                            ExcelAuditEngine.Compare(o, cancel.Token, update =>
                            {
                                if (update.Phase.StartsWith("Matching key partition", StringComparison.Ordinal)) cancel.Cancel();
                            });
                        }
                        catch (OperationCanceledException) { cancelled = true; }
                        Require(cancelled, "Mid-run cancellation was ignored.");
                    }
                    Require(before.SequenceEqual(Directory.GetFileSystemEntries(directory).OrderBy(path => path)), "Cancellation left audit temporary files.");
                });
                check("Report has exactly two valid sheets and literal data values", () =>
                {
                    var r = new ExcelAuditResult(options());
                    r.Add(new ExcelAuditFinding { Category = ExcelAuditCategory.Data, Difference = "Value", ValueA = "=1+1", ValueB = "+SUM(A1:A2)" });
                    r.Add(new ExcelAuditFinding { Category = ExcelAuditCategory.Data, Difference = "Literal Unicode", ValueA = "broken \ud800 and valid \ud83d\ude00", ValueB = "\u0001control" });
                    r.Add(new ExcelAuditFinding { Category = ExcelAuditCategory.Formatting, Difference = "Font", ValueA = "Calibri", ValueB = "Arial" });
                    string report = Path.Combine(directory, "Report.xlsx");
                    ExcelAuditReportWriter.Write(r, report, false, CancellationToken.None);
                    using (var doc = SpreadsheetDocument.Open(report, false))
                    {
                        var sheets = doc.WorkbookPart.Workbook.Sheets.Elements<Sheet>().ToArray();
                        Require(sheets.Select(s => s.Name.Value).SequenceEqual(new[] { "Summary", "Data Differences" }), "Wrong report sheet contract.");
                        var data = (WorksheetPart)doc.WorkbookPart.GetPartById(sheets[1].Id.Value);
                        var summary = (WorksheetPart)doc.WorkbookPart.GetPartById(sheets[0].Id.Value);
                        Require(data.Worksheet.Descendants<CellFormula>().Count() == 0 && summary.Worksheet.Descendants<CellFormula>().Count() == 0,
                            "Formula-shaped report text became executable.");
                        Require(data.Worksheet.InnerText.Contains("=1+1") && !data.Worksheet.InnerText.Contains("Calibri"), "Data sheet mixed non-data details.");
                        Require(summary.Worksheet.InnerText.Contains("Calibri"), "Summary omitted non-data details.");
                        var errors = new OpenXmlValidator(FileFormatVersions.Office2010).Validate(doc).Take(3).ToArray();
                        Require(errors.Length == 0, "Invalid report XML: " + string.Join("; ", errors.Select(e => e.Description)));
                    }
                    byte[] before = File.ReadAllBytes(report);
                    Rejected(() => ExcelAuditReportWriter.Write(r, report, false, CancellationToken.None));
                    Require(before.SequenceEqual(File.ReadAllBytes(report)), "An unconfirmed report was overwritten.");
                    Rejected(() => ExcelAuditReportWriter.Write(r, a, true, CancellationToken.None));
                    var validation = new ExcelAuditResult(options()) { IsValidationOnly = true };
                    Rejected(() => ExcelAuditReportWriter.Write(validation, Path.Combine(directory, "Validation.xlsx"), false, CancellationToken.None));
                    using (var cancel = new CancellationTokenSource())
                    {
                        cancel.Cancel();
                        try { ExcelAuditReportWriter.Write(r, report, true, cancel.Token); }
                        catch (OperationCanceledException) { }
                    }
                    Require(before.SequenceEqual(File.ReadAllBytes(report)), "Cancellation replaced an existing report.");
                });
                check("New route and native form remain separate from existing Excel comparison", () =>
                {
                    using (var existing = WorkspaceCatalog.Find("comparison.excel").CreatePage())
                        Require(existing.GetType() == typeof(Excel_Excel_Comparison), "Existing Excel task was replaced.");
                    using (var form = new ExcelSheetAuditForm())
                    {
                        Require(form is WorkspacePageForm, "New form bypasses workspace lifecycle.");
                        Rejected(() => form.CaptureOptions());
                        form.ShowInTaskbar = false;
                        form.Show(); Application.DoEvents();
                        var r = new ExcelAuditResult(options());
                        r.Add(new ExcelAuditFinding { Category = ExcelAuditCategory.Data, Difference = "Value", ValueA = "A", ValueB = "B" });
                        form.PresentResult(r);
                        Application.DoEvents();
                        form.Close();
                    }
                });
            }
            finally
            {
                // This test owns this exact unique directory, never a user supplied path.
                try { Directory.Delete(directory, true); }
                catch (IOException ex) { messages.Add("FAIL fixture cleanup: " + ex.Message); }
                catch (UnauthorizedAccessException ex) { messages.Add("FAIL fixture cleanup: " + ex.Message); }
                File.WriteAllLines(logFile, messages);
            }
            return messages.Any(message => message.StartsWith("FAIL ", StringComparison.Ordinal)) ? 1 : 0;
        }

        private static ExcelAuditResult Compare(ExcelAuditOptions options) => ExcelAuditEngine.Compare(options, CancellationToken.None);
        private static object[] Row(params object[] values) => values;
        private static object[][] Rows(params object[][] values) => values;
        private static FixtureCell N(string value) => new FixtureCell { Kind = "Number", Value = value };
        private static FixtureCell F(string value) => new FixtureCell { Kind = "Formula", Value = value };
        private static FixtureCell D(string value) => new FixtureCell { Kind = "Date", Value = value };
        private static FixtureCell S(object value, uint style) => new FixtureCell { Value = value, Style = style };
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
        private static void Rejected(Action action)
        {
            try { action(); }
            catch (ExcelAuditException) { return; }
            catch (ValidationFailure) { return; }
            throw new InvalidOperationException("Invalid audit input/output was accepted.");
        }

        private static void EditWorksheet(string path, Action<WorksheetPart> edit)
        {
            using (var document = SpreadsheetDocument.Open(path, true))
            {
                var part = document.WorkbookPart.WorksheetParts.Single();
                edit(part);
                part.Worksheet.Save();
            }
        }

        private static void UseSharedStrings(string path)
        {
            using (var document = SpreadsheetDocument.Open(path, true))
            {
                var workbook = document.WorkbookPart;
                var part = workbook.AddNewPart<SharedStringTablePart>();
                part.SharedStringTable = new SharedStringTable();
                var indexes = new Dictionary<string, int>(StringComparer.Ordinal);
                var worksheet = workbook.WorksheetParts.Single().Worksheet;
                foreach (var cell in worksheet.Descendants<Cell>().Where(cell => cell.DataType?.Value == CellValues.InlineString))
                {
                    string text = cell.InlineString.InnerText;
                    int index;
                    if (!indexes.TryGetValue(text, out index))
                    {
                        index = indexes.Count; indexes.Add(text, index);
                        part.SharedStringTable.Append(new SharedStringItem(new Text(text) { Space = SpaceProcessingModeValues.Preserve }));
                    }
                    cell.InlineString = null;
                    cell.DataType = CellValues.SharedString;
                    cell.CellValue = new CellValue(index.ToString(CultureInfo.InvariantCulture));
                }
                part.SharedStringTable.Save();
                worksheet.Save();
            }
        }

        private static void Make(string path, object[][] rows, bool date1904 = false)
        {
            using (var document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                var workbook = document.AddWorkbookPart();
                workbook.Workbook = new Workbook(new WorkbookProperties { Date1904 = date1904 });
                var styles = workbook.AddNewPart<WorkbookStylesPart>();
                styles.Stylesheet = new Stylesheet(
                    new Fonts(new Font(new FontSize { Val = 11D }, new FontName { Val = "Calibri" }),
                        new Font(new FontSize { Val = 12D }, new FontName { Val = "Arial" })) { Count = 2U },
                    new Fills(new Fill(new PatternFill { PatternType = PatternValues.None }),
                        new Fill(new PatternFill { PatternType = PatternValues.Gray125 })) { Count = 2U },
                    new Borders(new Border()) { Count = 1U },
                    new CellStyleFormats(new CellFormat()) { Count = 1U },
                    new CellFormats(
                        new CellFormat { FontId = 0U, FillId = 0U, BorderId = 0U, NumberFormatId = 0U },
                        new CellFormat(new Alignment { Horizontal = HorizontalAlignmentValues.Center })
                        { FontId = 1U, FillId = 0U, BorderId = 0U, NumberFormatId = 0U, ApplyFont = true, ApplyAlignment = true },
                        new CellFormat { FontId = 0U, FillId = 0U, BorderId = 0U, NumberFormatId = 0U },
                        new CellFormat { FontId = 0U, FillId = 0U, BorderId = 0U, NumberFormatId = 14U, ApplyNumberFormat = true }) { Count = 4U });
                styles.Stylesheet.Save();
                var part = workbook.AddNewPart<WorksheetPart>();
                var data = new SheetData();
                part.Worksheet = new Worksheet(data);
                for (int i = 0; i < rows.Length; i++)
                {
                    var row = new DocumentFormat.OpenXml.Spreadsheet.Row { RowIndex = (uint)i + 1U };
                    data.Append(row);
                    for (int j = 0; j < rows[i].Length; j++)
                    {
                        object value = rows[i][j];
                        if (value == null) continue;
                        var cell = new Cell { CellReference = ExcelAuditReportWriter.ColumnName(j + 1) + (i + 1).ToString(CultureInfo.InvariantCulture) };
                        var wrapper = value as FixtureCell;
                        string kind = null;
                        while (wrapper != null)
                        {
                            if (wrapper.Style != 0U) cell.StyleIndex = wrapper.Style;
                            if (wrapper.Kind != null) kind = wrapper.Kind;
                            value = wrapper.Value;
                            wrapper = value as FixtureCell;
                        }
                        string text = Convert.ToString(value, CultureInfo.InvariantCulture);
                        if (kind == "Formula") { cell.CellFormula = new CellFormula(text); cell.CellValue = new CellValue("2"); }
                        else if (kind == "Date") { cell.DataType = CellValues.Date; cell.CellValue = new CellValue(text); }
                        else if (kind == "Number" || value is int || value is double || value is decimal)
                        { cell.DataType = CellValues.Number; cell.CellValue = new CellValue(text); }
                        else if (value is bool) { cell.DataType = CellValues.Boolean; cell.CellValue = new CellValue((bool)value ? "1" : "0"); }
                        else { cell.DataType = CellValues.InlineString; cell.InlineString = new InlineString(new Text(text) { Space = SpaceProcessingModeValues.Preserve }); }
                        row.Append(cell);
                    }
                }
                part.Worksheet.Save();
                workbook.Workbook.Append(new Sheets(new Sheet { Id = workbook.GetIdOfPart(part), SheetId = 1U, Name = "Data" }));
                workbook.Workbook.Save();
            }
        }
    }
}
