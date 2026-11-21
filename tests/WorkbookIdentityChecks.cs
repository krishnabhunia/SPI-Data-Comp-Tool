using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using S = DocumentFormat.OpenXml.Spreadsheet;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Synthetic checks of the workbook identity engine and its report. Every workbook is created in a temporary
    /// folder and deleted again; no database, no user file and no window is touched.
    /// </summary>
    internal static class WorkbookIdentityChecks
    {
        // positions inside one row of IdentityResult.Data
        private const int RowAt = 1, KeyAt = 2, ColumnAt = 3, CellA = 4, CellB = 5, ValueA = 6, ValueB = 7, KindAt = 8, TypeFlag = 9, ValueFlag = 10, Delta = 11, Flags = 12;

        internal static void Run(Action<string> passed)
        {
            string folder = Path.Combine(Path.GetTempPath(), "spi-identity-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            try
            {
                CheckReader(folder, passed);
                CheckIdentical(folder, passed);
                CheckValueDifferences(folder, passed);
                CheckKeys(folder, passed);
                CheckColumnsAndOptions(folder, passed);
                CheckFormatsAndLayout(folder, passed);
                CheckReport(folder, passed);
            }
            finally
            {
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
        }

        // ------------------------------------------------------------------ helpers
        private static void Require(bool valid, string message)
        {
            if (!valid) throw new InvalidOperationException("Workbook identity check failed: " + message);
        }

        private static void Rejected<T>(Action action, string what) where T : Exception
        {
            try { action(); }
            catch (T) { return; }
            throw new InvalidOperationException("Workbook identity check failed: " + what + " was accepted.");
        }

        private static object[] R(params object[] cells) { return cells; }

        /// <summary>Text stays text, "=..." becomes a formula, numbers, dates and booleans keep their type.</summary>
        private static void Put(IXLCell cell, object value)
        {
            if (value == null) return;
            var text = value as string;
            if (text != null)
            {
                if (text.StartsWith("=", StringComparison.Ordinal)) { cell.FormulaA1 = text.Substring(1); return; }
                cell.SetValue(text);
                cell.DataType = XLDataType.Text;
            }
            else if (value is int) cell.SetValue((int)value);
            else if (value is double) cell.SetValue((double)value);
            else if (value is bool) cell.SetValue((bool)value);
            else if (value is DateTime) cell.SetValue((DateTime)value);
            else throw new ArgumentException("Unsupported test value: " + value.GetType().Name);
        }

        private static string Book(string folder, string name, object[][] rows, Action<IXLWorksheet> shape = null, string sheet = "Data")
        {
            string path = Path.Combine(folder, name);
            using (var workbook = new XLWorkbook())
            {
                Sheet(workbook, sheet, rows, shape);
                workbook.SaveAs(path);
            }
            return path;
        }

        private static void Sheet(XLWorkbook workbook, string name, object[][] rows, Action<IXLWorksheet> shape = null)
        {
            IXLWorksheet sheet = workbook.Worksheets.Add(name);
            for (int r = 0; r < rows.Length; r++)
                for (int c = 0; c < rows[r].Length; c++) Put(sheet.Cell(r + 1, c + 1), rows[r][c]);
            if (shape != null) shape(sheet);
        }

        private static IdentityResult Compare(string a, string b, Action<IdentityOptions> set = null)
        {
            var options = new IdentityOptions();
            if (set != null) set(options);
            return WorkbookIdentityEngine.Compare(a, b, options);
        }

        private static object[] DataRow(IdentityResult res, string cellA)
        {
            object[] row = res.Data.FirstOrDefault(item => Equals(item[CellA], cellA));
            Require(row != null, "no difference was reported for cell " + cellA);
            return row;
        }

        private static string Status(IdentityResult res, string check)
        {
            object[] row = res.Layout.FirstOrDefault(item => Equals(item[1], check));
            Require(row != null, "layout check '" + check + "' is missing");
            return (string)row[4];
        }

        private static void Expect(IdentityResult res, string cellA, string kind, string typeFlag, string valueFlag, object delta, string flags)
        {
            object[] row = DataRow(res, cellA);
            string got = string.Join(",", row.Skip(Flags).Select(item => item == null ? "-" : (string)item));
            Require((string)row[KindAt] == kind && (string)row[TypeFlag] == typeFlag && (string)row[ValueFlag] == valueFlag
                && Equals(row[Delta], delta) && got == flags,
                cellA + " expected " + kind + " / " + typeFlag + " / " + valueFlag + " / " + (delta ?? "null") + " / " + flags
                + " but got " + row[KindAt] + " / " + row[TypeFlag] + " / " + row[ValueFlag] + " / " + (row[Delta] ?? "null") + " / " + got);
        }

        // ------------------------------------------------------------------ reader
        private static void CheckReader(string folder, Action<string> passed)
        {
            // Font children deliberately out of schema order (name before bold and size), as some writers produce.
            string path = Path.Combine(folder, "unordered-font.xlsx");
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbook = document.AddWorkbookPart();
                var styles = workbook.AddNewPart<WorkbookStylesPart>();
                styles.Stylesheet = new S.Stylesheet(
                    new S.Fonts(
                        new S.Font(new S.FontName { Val = "Calibri" }, new S.FontSize { Val = 11 }),
                        new S.Font(new S.FontName { Val = "Arial" }, new S.Italic(), new S.Bold { Val = true }, new S.Color { Rgb = "FFFF0000" }, new S.FontSize { Val = 14.5 }, new S.Underline())),
                    new S.Fills(new S.Fill(new S.PatternFill { PatternType = S.PatternValues.None }), new S.Fill(new S.PatternFill { PatternType = S.PatternValues.Gray125 })),
                    new S.Borders(new S.Border()),
                    new S.CellFormats(new S.CellFormat { FontId = 0 }, new S.CellFormat { FontId = 1, NumberFormatId = 10 }));
                styles.Stylesheet.Save();
                var sheetPart = workbook.AddNewPart<WorksheetPart>();
                sheetPart.Worksheet = new S.Worksheet(
                    new S.Columns(new S.Column { Min = 3, Max = 3, Style = 1, Width = 9, CustomWidth = true }),
                    new S.SheetData(
                        new S.Row(new S.Cell { CellReference = "A1", StyleIndex = 1, DataType = S.CellValues.InlineString, InlineString = new S.InlineString(new S.Text("Head")) },
                                  new S.Cell { CellReference = "B1", CellValue = new S.CellValue("0.25"), StyleIndex = 1 }) { RowIndex = 1 },
                        new S.Row(new S.Cell { CellReference = "A2", CellValue = new S.CellValue("1") },
                                  new S.Cell { CellReference = "D2", CellValue = new S.CellValue("2") }) { RowIndex = 2, StyleIndex = 1, CustomFormat = true },
                        new S.Row(new S.Cell { CellReference = "D3", CellValue = new S.CellValue("3") }) { RowIndex = 3 }));
                sheetPart.Worksheet.Save();
                workbook.Workbook = new S.Workbook(new S.Sheets(new S.Sheet { Name = "One", SheetId = 1, Id = workbook.GetIdOfPart(sheetPart) }));
                workbook.Workbook.Save();
            }
            using (var book = new IdentityWorkbook(path))
            {
                IdentitySheet sheet = book.ReadSheet("One");
                string[] style = sheet.StyleOf(sheet.Cell(1, 1));
                Require(style[IdentityStyle.FontName] == "Arial" && style[IdentityStyle.FontSize] == "14.5" && style[IdentityStyle.Bold] == "Yes"
                    && style[IdentityStyle.Italic] == "Yes" && style[IdentityStyle.Underline] == "single" && style[IdentityStyle.FontColour] == "#FF0000"
                    && style[IdentityStyle.NumberFormat] == "0.00%",
                    "font properties were lost when their XML order differs: " + string.Join(" / ", style));
                Require(sheet.Cell(1, 1).Value == "Head" && sheet.Cell(1, 1).Kind == "text" && sheet.Cell(1, 2).Kind == "number" && sheet.Cell(1, 2).Value == "0.25"
                    && sheet.LastRow == 3 && sheet.LastCol == 4 && sheet.Cell(5, 5).Kind == "blank", "cell values or sheet bounds were read wrongly");
                // A cell the file does not store shows its row's format, else its column's; a stored cell without a style number uses style 0.
                Func<int, int, string> font = (r, c) => sheet.StyleOf(sheet.Cell(r, c))[IdentityStyle.FontName];
                Require(font(2, 2) == "Arial" && font(3, 3) == "Arial" && font(2, 1) == "Calibri" && font(3, 2) == "Calibri" && font(3, 4) == "Calibri"
                    && sheet.Cell(3, 3).Kind == "blank", "a blank cell must take the format of its row or column");
            }
            passed("Workbook identity reader keeps bold, italic, size, colour and underline whatever the XML order of the font.");

            Require(IdentityWorkbook.ShiftFormula("B2*C2", 1, 0) == "B3*C3", "relative references must move");
            Require(IdentityWorkbook.ShiftFormula("$B$2*C2+SUM(A:A)+SUM($1:2)", 1, 1) == "$B$2*D3+SUM(B:B)+SUM($1:3)", "absolute parts must stay fixed");
            Require(IdentityWorkbook.ShiftFormula("\"A1\"&'My Sheet'!A1&LOG10(B2)&Rate1", 1, 0) == "\"A1\"&'My Sheet'!A2&LOG10(B3)&Rate1",
                "text literals, sheet names, functions and names must not be shifted");
            Require(IdentityWorkbook.ShiftFormula("A1+1", -1, 0) == "#REF!+1", "a reference moved off the sheet must become #REF!");
            Require(IdentityWorkbook.ShiftFormula("SUM(Table1[Q1])+A2+Table1[[#This Row],[B2]]+[1]Sheet1!C3", 1, 0) == "SUM(Table1[Q1])+A3+Table1[[#This Row],[B2]]+[1]Sheet1!C4",
                "table columns and external workbook numbers in [ ] are not cell references");
            Require(IdentityWorkbook.NumberText(62000) == "62000" && IdentityWorkbook.NumberText(0.1 + 0.2) == "0.3" && IdentityWorkbook.NumberText(1e-7) == "1e-07",
                "number text must match the reference tool");
            int row, col;
            Require(IdentityWorkbook.SplitReference("BC12", out row, out col) && row == 12 && col == 55 && IdentityWorkbook.ColumnLetters(16384) == "XFD"
                && !IdentityWorkbook.SplitReference("A1:B2", out row, out col), "cell references were parsed wrongly");
            Rejected<InvalidDataException>(() => new IdentityWorkbook(Path.Combine(folder, "old.xls")).Dispose(), "an .xls file");
            Rejected<FileNotFoundException>(() => new IdentityWorkbook(Path.Combine(folder, "absent.xlsx")).Dispose(), "a missing file");
            passed("Shared formulas are rebuilt, numbers are shown like the reference tool, and unsupported or missing files are refused.");
        }

        // ------------------------------------------------------------------ engine
        private static readonly DateTime Day = new DateTime(2026, 11, 2);

        private static object[][] Base()
        {
            return new[]
            {
                R("Txn ID", "Account", "Amount", "Rate", "Status", "Value Date"),
                R("T001", "ACC-01", 100.0, 1.25, "Settled", Day),
                R("T002", "ACC-02", 1.12, 0.06, "Settled", Day),
                R("T003", "ACC-03", 250.1, 5.5, "Settled", Day),
                R("T004", "ACC-04", 500, 1.25, "Settled", Day),
                R("T005", "ACC-05", 300, 1.25, "Pending", Day),
                R("T006", "ACC-06", 750, 1.25, "Settled", Day),
                R("T007", "ACC-07", 900, 1.25, "Settled", Day),
                R("T008", "ACC-08", "1.50", 1.25, "Settled", Day),
                R("T009", "ACC-09", 1200, 1.25, "Settled", Day),
                R("T010", "ACC-10", 45, 1.25, "Settled", Day),
            };
        }

        private static void CheckIdentical(string folder, Action<string> passed)
        {
            string a = Book(folder, "same-a.xlsx", Base()), b = Book(folder, "same-b.xlsx", Base());
            IdentityResult byPosition = Compare(a, b);
            IdentityResult byKey = Compare(a, b, o => o.KeyA = new[] { "TXN id" });
            Require(byPosition.Identical && byPosition.Data.Count == 0 && byPosition.Formats.Count == 0 && !byPosition.Keyed, "equal workbooks must be identical by position");
            Require(byKey.Identical && byKey.Keyed && byKey.Verdict.StartsWith("IDENTICAL", StringComparison.Ordinal), "equal workbooks must be identical by key");
            passed("Two equal workbooks are reported IDENTICAL, by row position and by primary key.");
        }

        private static void CheckValueDifferences(string folder, Action<string> passed)
        {
            object[][] changed = Base();
            changed[1][2] = 100.005;                         // C2  difference 0.005
            changed[2][2] = 1.13;                            // C3  exactly 0.01
            changed[2][3] = 0.07;                            // D3  0.07 - 0.06 = 0.010000000000000009 in floating point
            changed[3][3] = 5.6;                             // D4  exactly 0.1
            changed[4][2] = 501;                             // C5  exactly 1
            changed[5][2] = 297.5;                           // C6  -2.5
            changed[5][4] = "Failed";                        // E6  text changed
            changed[6][2] = "750";                           // C7  number stored as text, same value
            changed[7][2] = "905";                           // C8  number against text, different value
            changed[8][2] = "1.5";                           // C9  text against text, same number
            changed[9][2] = "1,200";                         // C10 not readable as a number
            changed[10][2] = " 45.5 ";                       // C11 padded text that reads as a number
            changed[1][4] = "SETTLED";                       // E2  case only
            changed[2][1] = "ACC-02 ";                       // B3  spacing only
            changed[3][5] = new DateTime(2026, 11, 5);       // F4  date changed
            changed[4][4] = null;                            // E5  blank text
            changed[10][3] = null;                           // D11 blank number
            IdentityResult res = Compare(Book(folder, "values-a.xlsx", Base()), Book(folder, "values-b.xlsx", changed), o => o.KeyA = new[] { "txn id" });

            Expect(res, "C2", "Number changed", "Number", "True", 0.005, "Yes,No,No,No");
            Expect(res, "C3", "Number changed", "Number", "True", 0.01, "Yes,No,No,No");                 // exactly 0.01 is not "> 0.01"
            Expect(res, "D3", "Number changed", "Number", "True", 0.01, "Yes,No,No,No");                 // noise is rounded away
            Expect(res, "D4", "Number changed", "Number", "True", 0.1, "Yes,Yes,No,No");
            Expect(res, "C5", "Number changed", "Number", "True", 1.0, "Yes,Yes,Yes,No");
            Expect(res, "C6", "Number changed", "Number", "True", -2.5, "Yes,Yes,Yes,Yes");
            Expect(res, "E6", "Text changed", "Text", "True", null, "-,-,-,-");
            Expect(res, "C7", "Data type differs (number vs text)", "Number", "False", 0.0, "No,No,No,No");
            Expect(res, "C8", "Value and data type differ (number vs text)", "Number", "True", 5.0, "Yes,Yes,Yes,Yes");
            Expect(res, "C9", "Text changed", "Number", "False", 0.0, "No,No,No,No");
            Expect(res, "C10", "Value and data type differ (number vs text)", "Text", "True", null, "-,-,-,-");
            Expect(res, "C11", "Value and data type differ (number vs text)", "Number", "True", 0.5, "Yes,Yes,Yes,No");
            Expect(res, "E2", "Case differs only", "Text", "True", null, "-,-,-,-");
            Expect(res, "B3", "Spacing differs only", "Text", "True", null, "-,-,-,-");
            Expect(res, "F4", "Date changed", "Text", "True", null, "-,-,-,-");
            Expect(res, "E5", "Blank in File B", "Text", "True", null, "-,-,-,-");
            Expect(res, "D11", "Blank in File B", "Number", "True", null, "-,-,-,-");
            Require((string)DataRow(res, "B3")[ValueB] == "\"ACC-02 \"" && (string)DataRow(res, "C7")[ValueA] == "750  (number)"
                && (string)DataRow(res, "C11")[ValueB] == "\" 45.5 \"  (text)" && (string)DataRow(res, "F4")[ValueA] == "2026-11-02"
                && (string)DataRow(res, "E5")[ValueB] == WorkbookIdentityEngine.BlankText && (string)DataRow(res, "C2")[KeyAt] == "T001",
                "values, quotes, data types or keys are shown wrongly");
            Require(res.Count("values") == 16 && res.Count("types") == 1 && res.Data.Count == 17 && !res.Identical && res.FailedChecks == 2,
                "difference counts are wrong: " + res.Count("values") + " values, " + res.Count("types") + " types");
            passed("Value differences carry the right type, same-platform flags, number difference and threshold columns.");
        }

        private static void CheckKeys(string folder, Action<string> passed)
        {
            string a = Book(folder, "key-a.xlsx", new[]
                { R("Emp ID", "Name", "Score"), R(101, "Asha", 50), R(102, "Vikram", 60), R(103, "Meera", 70), R(104, "Rohan", 80), R(105, "Sana", 90) });
            string b = Book(folder, "key-b.xlsx", new[]
                { R("EMP id", "Name", "Score"), R(105, "Sana", 90), R(101, "Asha", 50), R(106, "Neha", 10), R(104, "Rohan", 85), R(102, "Vikram", 60) });
            IdentityResult keyed = Compare(a, b, o => o.KeyA = new[] { "emp id" });
            Require(keyed.Count("values") == 1 && keyed.Count("rows_only") == 2 && keyed.Count("row_order") == 2 && keyed.Count("header_case") == 1,
                "reordered, inserted and deleted rows were not recognised by key");
            Require(keyed.Data.Select(row => row[KeyAt] + ":" + row[KindAt]).SequenceEqual(new[] { "104:Number changed", "103:Row only in File A", "106:Row only in File B" })
                && (string)DataRow(keyed, "C5")[CellB] == "C5" && (int)keyed.Data[2][RowAt] == 4, "keyed difference rows are wrong");
            Require(Compare(a, b).Count("values") > 10, "the same files compared by position must show many more differences");

            string c = Book(folder, "composite-a.xlsx", new[] { R("a2", "a3", "Amount"), R("IN", 1, 10), R("IN", 2, 20), R("US", 1, 30) });
            string d = Book(folder, "composite-b.xlsx", new[] { R("B2", "B3", "Amount"), R("US", 1, 30), R("IN", 1, 10), R("IN", 2, 25) });
            IdentityResult composite = Compare(c, d, o => { o.KeyA = new[] { "A2", "A3" }; o.KeyB = new[] { "b2", " b3 " }; });
            Require(composite.Keyed && composite.Count("header_names") == 2 && composite.Count("values") == 1 && (string)composite.Data[0][KeyAt] == "IN | 2"
                && Equals(composite.Data[0][Delta], 5.0) && (string)composite.Data[0][CellA] == "C3" && (string)composite.Data[0][CellB] == "C4",
                "a composite key with different column names per file was not paired");

            object[][] loose = { R("id", "val"), R(1, "x"), R(1, "y"), R(null, "z"), R(2, "w") };
            IdentityResult warned = Compare(Book(folder, "loose-a.xlsx", loose), Book(folder, "loose-b.xlsx", loose), o => o.KeyA = new[] { "ID" });
            Require(warned.Identical && warned.Notes.Count == 2 && Status(warned, "Primary key: rows with a blank key value") == "WARN"
                && Status(warned, "Primary key: rows with a repeated key value") == "WARN", "blank and repeated keys must be warnings, not differences");

            // A row that holds only formulas without a saved result, under a blank key, is still paired and compared.
            IdentityResult formulas = Compare(
                Book(folder, "formula-a.xlsx", new[] { R("ID", "Value"), R(null, "=1+1"), R(7, "x") }),
                Book(folder, "formula-b.xlsx", new[] { R("ID", "Value"), R(null, "=1+2"), R(7, "x") }), o => o.KeyA = new[] { "id" });
            Require(formulas.Notes.Any(note => note.Contains("no saved result")), "this check needs formulas without a saved result");
            Require(formulas.Count("formulas") == 1 && !formulas.Identical && (string)formulas.Data[0][KindAt] == "Formula differs"
                && (string)formulas.Data[0][KeyAt] == WorkbookIdentityEngine.BlankText, "a formula-only row under a blank key was left out of key matching");
            IdentityResult extra = Compare(
                Book(folder, "formula-c.xlsx", new[] { R("ID", "Value"), R(7, "x") }),
                Book(folder, "formula-d.xlsx", new[] { R("ID", "Value"), R(7, "x"), R(null, "=1+2") }), o => o.KeyA = new[] { "id" });
            Require(extra.Count("rows_only") == 1 && (string)extra.Data[0][KindAt] == "Row only in File B" && ((string)extra.Data[0][ValueB]).Contains("Value==1+2"),
                "a formula-only row that exists in one file must be listed");

            Rejected<ArgumentException>(() => Compare(a, b, o => o.KeyA = new[] { "nope" }), "a key column that does not exist");
            Rejected<ArgumentException>(() => Compare(a, b, o => { o.KeyA = new[] { "emp id", "name" }; o.KeyB = new[] { "emp id" }; }), "a key with a different column count per file");
            passed("Primary keys pair rows across reordering, insertion and deletion; composite and differently named keys work; bad keys are refused.");
        }

        private static void CheckColumnsAndOptions(string folder, Action<string> passed)
        {
            IdentityResult moved = Compare(
                Book(folder, "order-a.xlsx", new[] { R("ID", "Name", "City"), R(1, "A", "Pune"), R(2, "B", "Delhi") }),
                Book(folder, "order-b.xlsx", new[] { R("ID", "city", "Name", "Extra"), R(1, "Pune", "A", "x"), R(2, "Goa", "B", "y") }));
            Require(moved.Count("header_order") == 2 && moved.Count("header_case") == 1 && moved.Count("header_names") == 1 && moved.Count("header_count") == 1
                && moved.Count("values") == 1 && (string)moved.Data[0][CellA] == "C3" && (string)moved.Data[0][CellB] == "B3",
                "moved, renamed or extra columns were not matched by header name");

            string one = Book(folder, "tolerance-a.xlsx", new[] { R("v"), R(1.0) }), two = Book(folder, "tolerance-b.xlsx", new[] { R("v"), R(1.004) });
            Require(!Compare(one, two).Identical && Compare(one, two, o => o.Tolerance = 0.01).Identical, "the number tolerance is not applied");
            Rejected<ArgumentException>(() => Compare(one, two, o => o.Tolerance = -1), "a negative tolerance");
            Rejected<ArgumentException>(() => Compare(one, two, o => o.HeaderRow = 0), "header row 0");

            IdentityResult lower = Compare(
                Book(folder, "header-a.xlsx", new[] { R("Report title"), R(), R("ID", "Amt"), R(1, 5) }),
                Book(folder, "header-b.xlsx", new[] { R("Report title"), R(), R("ID", "Amt"), R(1, 6) }), o => o.HeaderRow = 3);
            Require(lower.Count("values") == 1 && (string)lower.Data[0][ColumnAt] == "Amt" && (string)lower.Data[0][CellA] == "B4", "a header row other than 1 is not honoured");

            string first = Path.Combine(folder, "sheets-a.xlsx"), second = Path.Combine(folder, "sheets-b.xlsx");
            using (var workbook = new XLWorkbook())
            {
                Sheet(workbook, "Orders", new[] { R("Qty", "Price", "Total"), R(2, 5, "=A2*B2") });
                Sheet(workbook, "Notes", new[] { R("Key", "Value"), R("owner", "ops") });
                Sheet(workbook, "OnlyA", new[] { R("x") });
                workbook.SaveAs(first);
            }
            using (var workbook = new XLWorkbook())
            {
                Sheet(workbook, "Orders", new[] { R("Qty", "Price", "Total"), R(2, 5, "=A2*B2*1") });
                Sheet(workbook, "notes", new[] { R("Key", "Value"), R("owner", "finance") });
                Sheet(workbook, "OnlyB", new[] { R("y") });
                workbook.SaveAs(second);
            }
            IdentityResult sheets = Compare(first, second);
            Require(sheets.Count("workbook") == 2 && sheets.Count("formulas") == 1 && sheets.Count("values") == 1
                && sheets.Data.Any(row => (string)row[KindAt] == "Formula differs" && (string)row[ValueB] == "=A2*B2*1")
                && sheets.Data.Any(row => (string)row[0] == "Notes ↔ notes" && (string)row[KindAt] == "Text changed"),
                "sheets were not paired by name, or formulas were not compared");
            IdentityResult picked = Compare(first, second, o => { o.SheetA = "Notes"; o.SheetB = "notes"; o.CompareFormulas = false; });
            Require(picked.Count("values") == 1 && picked.Count("workbook") == 0 && picked.Skipped.Contains("formulas"), "comparing one chosen sheet failed");
            Rejected<ArgumentException>(() => Compare(first, second, o => o.SheetA = "Missing"), "a sheet that does not exist");
            Rejected<OperationCanceledException>(() => WorkbookIdentityEngine.Compare(first, second, null, null, () => true), "a cancelled comparison");

            // Elapsed-time cells keep their fraction, so 1.1 s and 1.9 s are different values.
            Func<double, string, string> timed = (seconds, name) => Book(folder, name, new[] { R("Run", "Took"), R("a", seconds / 86400.0) },
                sheet => sheet.Cell("B2").Style.NumberFormat.Format = "[h]:mm:ss.000");
            IdentityResult durations = Compare(timed(1.1, "took-a.xlsx"), timed(1.9, "took-b.xlsx"));
            Require(durations.Count("values") == 1 && (string)durations.Data[0][ValueA] == "0:00:01.100000" && (string)durations.Data[0][ValueB] == "0:00:01.900000",
                "durations that differ by a fraction of a second must not compare as equal");
            Require(Compare(timed(3725, "took-c.xlsx"), timed(3725, "took-d.xlsx")).Identical, "equal durations must be identical");

            int reported = -1;
            WorkbookIdentityEngine.Compare(first, second, null, (percent, message) => reported = Math.Max(reported, percent));
            Require(reported == 100, "progress must end at 100");
            passed("Columns are matched by name; tolerance, header row, sheet pairing, sheet choice, progress and cancellation work.");
        }

        private static void CheckFormatsAndLayout(string folder, Action<string> passed)
        {
            Func<bool, string> build = second => Book(folder, second ? "look-b.xlsx" : "look-a.xlsx", new[]
            {
                R("Emp ID", "Name", "Salary"), R(1, "Asha", 58000), R(2, "Vikram", 62000), R(3, "Meera", 71000), R(4, "Rohan", 49000), R(5, "Sana", 66000),
            }, sheet =>
            {
                sheet.Row(1).Style.Font.Bold = true;
                sheet.Range("C2:C6").Style.NumberFormat.Format = second ? "#,##0.00" : "#,##0";
                sheet.Column(2).Width = second ? 30 : 22;
                if (!second) { sheet.SheetView.FreezeRows(1); return; }
                sheet.Cell("B1").Style.Font.FontName = "Arial";
                sheet.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                sheet.Cell("A2").Style.Font.Bold = true;
                sheet.Cell("B3").Style.Font.FontSize = 14;
                sheet.Cell("B4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                sheet.Cell("B5").Style.Fill.BackgroundColor = XLColor.Yellow;
                sheet.Row(4).Hide();
                sheet.Range("E1:F1").Merge();
                sheet.Cell("A2").Comment.AddText("check");
            });
            string a = build(false), b = build(true);
            IdentityResult res = Compare(a, b);
            Require(res.Count("values") == 0 && res.Count("header_font") == 1 && res.Count("header_align") == 1 && res.Count("fmt_font") == 2
                && res.Count("fmt_align") == 1 && res.Count("fmt_other") == 6, "format counts are wrong: font " + res.Count("fmt_font") + ", alignment "
                + res.Count("fmt_align") + ", other " + res.Count("fmt_other") + ", header font " + res.Count("header_font") + ", header alignment " + res.Count("header_align"));
            object[] range = res.Formats.SingleOrDefault(row => (string)row[6] == "Number format");
            Require(range != null && (string)range[3] == "C2:C6" && (string)range[4] == "C2:C6" && (int)range[5] == 5 && (string)range[7] == "#,##0" && (string)range[8] == "#,##0.00",
                "consecutive cells with the same format difference must collapse into one range");
            Require(res.Formats.Any(row => (string)row[1] == "Header" && (string)row[6] == "Font name" && (string)row[8] == "Arial")
                && res.Formats.Any(row => (string)row[3] == "B5" && (string)row[6] == "Fill colour" && (string)row[8] == "#FFFF00"), "header font or fill colour was not reported");
            foreach (string check in new[] { "Column widths", "Merged cells", "Freeze panes", "Hidden rows", "Cell comments" })
                Require(Status(res, check) == "DIFF", check + " must be reported as different");
            Require(Status(res, "Hidden columns") == "MATCH" && res.Count("layout") == 4 && res.Count("extras") == 1, "layout counts are wrong");

            // A comment, a link and a hidden row below the last value belong to no record: a workbook must still equal itself.
            Func<string, string> outside = name => Book(folder, name, new[] { R("ID", "Name"), R(1, "Asha"), R(2, "Sana") }, sheet =>
            {
                sheet.Cell("A10").Comment.AddText("reviewed");
                sheet.Cell("D12").Hyperlink = new XLHyperlink("https://example.com/notes");
                sheet.Row(14).Hide();
                sheet.Cell("B2").Comment.AddText("inside");
            });
            string noteA = outside("outside-a.xlsx"), noteB = outside("outside-b.xlsx");
            Require(Compare(noteA, noteB).Identical && Compare(noteA, noteB, o => o.KeyA = new[] { "id" }).Identical,
                "comments, links and hidden rows outside the data must not make equal workbooks differ");
            string noteC = Book(folder, "outside-c.xlsx", new[] { R("ID", "Name"), R(1, "Asha"), R(2, "Sana") }, sheet => sheet.Cell("A10").Comment.AddText("changed"));
            IdentityResult noted = Compare(noteA, noteC);
            Require(Status(noted, "Cell comments") == "DIFF" && Status(noted, "Hyperlinks") == "DIFF" && Status(noted, "Hidden rows") == "DIFF",
                "a changed comment, a missing link and a missing hidden row outside the data must be reported");

            // Two columns with the same header text keep separate widths.
            Func<double, string, string> twin = (width, name) => Book(folder, name, new[] { R("Value", "Value"), R(1, 2) },
                sheet => { sheet.Column(1).Width = width; sheet.Column(2).Width = 20; });
            IdentityResult twins = Compare(twin(10, "twin-a.xlsx"), twin(30, "twin-b.xlsx"));
            Require(Status(twins, "Column widths") == "DIFF" && !twins.Identical && Compare(twin(10, "twin-c.xlsx"), twin(10, "twin-d.xlsx")).Identical,
                "the width of the first of two equally named columns was lost");

            // Conditional formats and validations on the same range are compared by what the rule does.
            Func<bool, string, string> ruled = (second, name) => Book(folder, name, new[] { R("ID", "Qty"), R(1, 4), R(2, 9) }, sheet =>
            {
                IXLConditionalFormat format = sheet.Range("B2:B3").AddConditionalFormat();
                if (second) format.WhenLessThan(5).Fill.SetBackgroundColor(XLColor.Red);
                else format.WhenGreaterThan(5).Fill.SetBackgroundColor(XLColor.Red);
                sheet.Range("A2:A3").SetDataValidation().WholeNumber.Between(1, second ? 5 : 9);
            });
            string ruleA = ruled(false, "rule-a.xlsx"), ruleB = ruled(true, "rule-b.xlsx");
            IdentityResult rules = Compare(ruleA, ruleB);
            Require(Status(rules, "Conditional formatting rules") == "DIFF" && Status(rules, "Data validation rules") == "DIFF" && rules.Count("extras") == 2
                && Compare(ruleA, ruled(false, "rule-c.xlsx")).Identical, "a changed rule on the same range must be reported, and an equal rule must match");

            // A format set on a whole column also shows on its blank cells, so it is a difference there.
            Func<bool, string, string> banded = (bold, name) => Book(folder, name, new[] { R("ID", "Note", "Qty"), R(1, null, 4), R(2, null, 9) },
                sheet => { if (bold) sheet.Column(2).Style.Fill.BackgroundColor = XLColor.Yellow; });
            IdentityResult bands = Compare(banded(false, "band-a.xlsx"), banded(true, "band-b.xlsx"));
            Require(bands.Count("values") == 0 && bands.Count("fmt_other") >= 2 && bands.Formats.Any(row => (string)row[6] == "Fill colour" && (string)row[8] == "#FFFF00"),
                "a column format on blank cells was not compared");

            IdentityResult skipped = Compare(a, b, o => { o.CompareFormats = false; o.CompareLayout = false; });
            Require(skipped.Identical && skipped.Formats.Count == 0 && skipped.Skipped.Contains("fmt_font") && skipped.Skipped.Contains("layout")
                && skipped.Layout.All(row => (string)row[1] != "Freeze panes"), "switched-off checks must be skipped, not counted");
            passed("Font, alignment, number format, fill and layout differences are found, ranged and counted; switched-off checks are skipped.");
        }

        // ------------------------------------------------------------------ report
        private static void CheckReport(string folder, Action<string> passed)
        {
            object[][] changed = Base();
            changed[1][2] = 105.5;
            changed[6][2] = "750";
            changed[2][4] = "=SUM(1,2)";                    // becomes a formula in File B; the report must show it as text
            string a = Book(folder, "report-a.xlsx", Base()), b = Book(folder, "report-b.xlsx", changed);
            IdentityResult res = Compare(a, b, o => o.KeyA = new[] { "txn id" });
            string report = Path.Combine(folder, "report.xlsx");
            IdentityReportWriter.Write(res, report);

            using (var book = new IdentityWorkbook(report))
            {
                Require(book.SheetNames.SequenceEqual(IdentityReportWriter.SheetNames), "the report must have its five sheets in order");
                IdentitySheet summary = book.ReadSheet("Summary");
                Require(summary.Cell(1, 1).Value == "Excel Comparison Report" && summary.Cell(10, 3).Value == res.Verdict && summary.Cell(12, 1).Value == "SrNo."
                    && summary.Cell(12 + IdentityResult.Checks.Length, 2).Value == IdentityResult.Checks.Last().Label && summary.Freeze == null,
                    "the Summary sheet is laid out wrongly");
                IdentitySheet data = book.ReadSheet("Data Differences");
                var titles = Enumerable.Range(1, data.LastCol).Select(col => data.Cell(1, col).Value).ToList();
                Require(titles.SequenceEqual(new[] { "SrNo.", "Sheet", "Row", "Key", "Column", "Cell (A)", "Cell (B)", "Value in File A", "Value in File B", "Difference Type",
                    "Difference Type Flag", "Difference Type Value Flag", "Number Diff (B − A)", "Abs Diff > 0", "Abs Diff > 0.01", "Abs Diff > 0.1", "Abs Diff > 1" }),
                    "the Data Differences columns changed: " + string.Join(" | ", titles));
                Require(data.LastRow == 1 + res.Data.Count && data.Freeze == "A2" && data.Filter != null, "the Data Differences table is incomplete");
                bool formulaAsText = false, keyAsText = false, deltaAsNumber = false;
                for (int row = 2; row <= data.LastRow; row++)
                {
                    for (int col = 1; col <= data.LastCol; col++) Require(data.Cell(row, col).Formula == null, "the report must not contain formulas");
                    formulaAsText |= data.Cell(row, 9).Value == "=SUM(1,2)" && data.Cell(row, 9).Kind == "text";
                    keyAsText |= data.Cell(row, 8).Value == "750  (number)" && data.Cell(row, 9).Value == "750  (text)";
                    deltaAsNumber |= data.Cell(row, 13).Kind == "number" && data.Cell(row, 13).Number == 5.5;
                }
                Require(formulaAsText && keyAsText && deltaAsNumber, "report cells lost their text or number type");
            }
            // Excel repairs a workbook whose frozen pane has neither a row nor a column split.
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(report, false))
            {
                foreach (WorksheetPart part in document.WorkbookPart.WorksheetParts)
                    foreach (S.Pane pane in part.Worksheet.Descendants<S.Pane>())
                        Require(pane.HorizontalSplit != null || pane.VerticalSplit != null, "a report sheet has a frozen pane without a split");
            }

            IdentityResult plain = Compare(a, a);
            IdentityReportWriter.Write(plain, report);
            using (var book = new IdentityWorkbook(report))
            {
                IdentitySheet data = book.ReadSheet("Data Differences");
                Require(book.ReadSheet("Summary").Cell(10, 3).Value == "IDENTICAL - no differences found" && data.Cell(1, 4).Value == "Column"
                    && data.Cell(2, 2).Value == "No differences found", "the report of identical workbooks is wrong");
            }
            Require(IdentityReportWriter.Clean("ok\u0001\u0002") == "ok??" && IdentityReportWriter.Clean(null) == "", "control characters must be replaced");
            passed("The identity report has its five sheets and columns, stores text as text, and opens without an Excel repair prompt.");
        }
    }
}
