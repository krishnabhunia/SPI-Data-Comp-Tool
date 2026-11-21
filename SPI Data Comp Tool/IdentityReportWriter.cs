using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ClosedXML.Excel;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Writes the workbook identity report: Summary, Headers, Data Differences, Format Differences and
    /// Layout &amp; Other. Same sheets and columns as the Python reference tool.
    /// </summary>
    internal static class IdentityReportWriter
    {
        /// <summary>Rows listed per report sheet; further rows are counted in the Summary but not listed.</summary>
        internal const int MaxRows = 200000;

        internal static readonly string[] SheetNames = { "Summary", "Headers", "Data Differences", "Format Differences", "Layout & Other" };

        private static readonly HashSet<string> GoodWords = new HashSet<string>(StringComparer.Ordinal)
            { "MATCH", "Exact match", "Same", "IDENTICAL", "OK", "False" };
        private static readonly HashSet<string> NeutralWords = new HashSet<string>(StringComparer.Ordinal) { "—", "SKIPPED" };

        private static readonly XLColor HeadFill = XLColor.FromHtml("#DDEBF7"), HeadText = XLColor.FromHtml("#1F3864"),
            Zebra = XLColor.FromHtml("#F2F2F2"), Good = XLColor.FromHtml("#1A3E8C"), Bad = XLColor.FromHtml("#C00000");

        internal static void Write(IdentityResult res, string path)
        {
            using (var workbook = new XLWorkbook())
            {
                workbook.Style.Font.FontName = "Calibri";
                workbook.Style.Font.FontSize = 11;
                Summary(workbook.Worksheets.Add(SheetNames[0]), res);

                Table(workbook.Worksheets.Add(SheetNames[1]), 1,
                    new[] { "Sheet", "Col (A)", "Header in File A", "Col (B)", "Header in File B", "Name Check", "Position", "Font in File A", "Font in File B",
                            "Font Check", "Alignment in File A", "Alignment in File B", "Alignment Check", "Status" },
                    new double[] { 22, 8, 22, 8, 22, 18, 14, 24, 24, 11, 18, 18, 11, 9 },
                    res.Headers, new[] { 6, 7, 10, 13, 14 }, "No headers found", true);

                var titles = new List<string> { "Sheet", "Row", "Key", "Column", "Cell (A)", "Cell (B)", "Value in File A", "Value in File B",
                    "Difference Type", "Difference Type Flag", "Difference Type Value Flag", "Number Diff (B − A)" };
                var widths = new List<double> { 22, 7, 22, 20, 9, 9, 30, 30, 34, 12, 12, 13 };
                foreach (double threshold in res.Options.Thresholds)
                {
                    titles.Add("Abs Diff > " + threshold.ToString("G", CultureInfo.InvariantCulture));
                    widths.Add(11);
                }
                IList<object[]> data = res.Data;
                if (!res.Keyed)                                  // the Key column appears only when a primary key was used
                {
                    titles.RemoveAt(2);
                    widths.RemoveAt(2);
                    data = res.Data.Select(row => row.Take(2).Concat(row.Skip(3)).ToArray()).ToList();
                }
                Table(workbook.Worksheets.Add(SheetNames[2]), 1, titles.ToArray(), widths.ToArray(), data,
                    new[] { titles.IndexOf("Difference Type Value Flag") + 1 }, "No differences found", true);

                Table(workbook.Worksheets.Add(SheetNames[3]), 1,
                    new[] { "Sheet", "Area", "Column", "Cells (A)", "Cells (B)", "Cell Count", "Property", "File A", "File B" },
                    new double[] { 22, 9, 20, 11, 11, 8, 22, 24, 24 },
                    res.Formats, new int[0], res.Options.CompareFormats ? "No differences found" : "Cell formatting was not compared", true);

                Table(workbook.Worksheets.Add(SheetNames[4]), 1,
                    new[] { "Scope", "Check", "File A", "File B", "Status" }, new double[] { 22, 40, 44, 44, 9 },
                    res.Layout, new[] { 5 }, "No checks", true);

                workbook.SaveAs(path);
            }
        }

        private static void Summary(IXLWorksheet sheet, IdentityResult res)
        {
            sheet.ShowGridLines = false;
            IXLCell title = sheet.Cell(1, 1);
            title.SetValue("Excel Comparison Report");
            title.Style.Font.FontSize = 16;
            title.Style.Font.Bold = true;
            title.Style.Font.FontColor = HeadText;

            var info = new[]
            {
                new KeyValuePair<string, object>("File A", res.FileA),
                new KeyValuePair<string, object>("File B", res.FileB),
                new KeyValuePair<string, object>("Generated", res.Generated.ToString("dd-MMM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)),
                new KeyValuePair<string, object>("Column matching", "By header name, ignoring case and leading/trailing spaces"),
                new KeyValuePair<string, object>("Row matching", res.RowMatching),
                new KeyValuePair<string, object>("Number tolerance", res.Options.Tolerance),
            };
            for (int i = 0; i < info.Length; i++)
            {
                sheet.Cell(3 + i, 1).SetValue(info[i].Key).Style.Font.Bold = true;
                Put(sheet.Cell(3 + i, 3), info[i].Value);
                sheet.Cell(3 + i, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            }
            sheet.Cell(10, 1).SetValue("RESULT").Style.Font.Bold = true;
            IXLCell verdict = sheet.Cell(10, 3);
            verdict.SetValue(res.Verdict);
            verdict.Style.Font.FontSize = 14;
            verdict.Style.Font.Bold = true;
            verdict.Style.Font.FontColor = res.Identical ? Good : Bad;

            var rows = new List<object[]>();
            foreach (IdentityCheck check in IdentityResult.Checks)
            {
                long count = res.Count(check.Key);
                bool skipped = res.Skipped.Contains(check.Key);
                rows.Add(new object[]
                {
                    check.Label, skipped ? "SKIPPED" : count != 0 ? "DIFF" : "MATCH",
                    count != 0 ? (object)(count.ToString("N0", CultureInfo.InvariantCulture) + " " + check.Unit) : 0, check.Sheet,
                });
            }
            // No frozen pane here: a pane without a row or column split makes Excel repair the workbook.
            int next = Table(sheet, 12, new[] { "Check", "Result", "Differences", "Details in sheet" }, new double[] { 62, 10, 16, 20 },
                rows, new[] { 2 }, "No checks", false);
            foreach (string note in res.Notes) sheet.Cell(next++, 2).SetValue(Clean("Note: " + note));
        }

        /// <summary>
        /// Writes a bordered table with a "SrNo." column in front. <paramref name="statusColumns"/> are 1-based
        /// positions within <paramref name="titles"/>. Returns the next free row.
        /// </summary>
        private static int Table(IXLWorksheet sheet, int top, string[] titles, double[] widths, IList<object[]> rows, int[] statusColumns,
            string empty, bool freeze)
        {
            int columns = titles.Length + 1;
            sheet.Column(1).Width = 6;
            sheet.Cell(top, 1).SetValue("SrNo.");
            for (int c = 0; c < titles.Length; c++)
            {
                sheet.Column(c + 2).Width = widths[c];
                sheet.Cell(top, c + 2).SetValue(titles[c]);
            }
            int shown = Math.Min(rows.Count, MaxRows);
            bool zebra = shown > 6;

            IXLRange whole = sheet.Range(top, 1, top + shown, columns);
            whole.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            whole.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            whole.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            whole.Style.Alignment.WrapText = true;
            IXLRange head = sheet.Range(top, 1, top, columns);
            head.Style.Font.Bold = true;
            head.Style.Font.FontColor = HeadText;
            head.Style.Fill.BackgroundColor = HeadFill;
            head.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            if (shown > 0) sheet.Range(top + 1, 1, top + shown, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            for (int i = 0; i < shown; i++)
            {
                int r = top + 1 + i;
                object[] row = rows[i];
                if (zebra && i % 2 == 1) sheet.Range(r, 1, r, columns).Style.Fill.BackgroundColor = Zebra;
                sheet.Cell(r, 1).SetValue(i + 1);
                for (int c = 0; c < row.Length && c < titles.Length; c++)
                {
                    object value = row[c];
                    if (value == null) continue;
                    IXLCell cell = sheet.Cell(r, c + 2);
                    bool number = Put(cell, value);
                    if (number) cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    else if (Array.IndexOf(statusColumns, c + 1) >= 0)
                    {
                        string word = Convert.ToString(value, CultureInfo.InvariantCulture);
                        if (NeutralWords.Contains(word)) continue;
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.FontColor = GoodWords.Contains(word) ? Good : Bad;
                    }
                }
            }
            int next = top + 1 + shown;
            if (shown == 0) sheet.Cell(next++, 2).SetValue(empty);
            if (rows.Count > MaxRows)
                sheet.Cell(next++, 2).SetValue((rows.Count - MaxRows).ToString("N0", CultureInfo.InvariantCulture) + " more row(s) not listed (limit "
                    + MaxRows.ToString("N0", CultureInfo.InvariantCulture) + ").").Style.Font.Bold = true;
            if (shown > 0) whole.SetAutoFilter();
            if (freeze) sheet.SheetView.FreezeRows(top);
            sheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            sheet.PageSetup.FitToPages(1, 0);
            sheet.PageSetup.SetRowsToRepeatAtTop(top, top);
            return next + 1;
        }

        /// <summary>Writes one value. Numbers stay numbers; all text is stored as text, never as a formula or a guessed type.</summary>
        private static bool Put(IXLCell cell, object value)
        {
            if (value is int) { cell.SetValue((int)value); return true; }
            if (value is long) { cell.SetValue((double)(long)value); return true; }
            if (value is double) { cell.SetValue((double)value); return true; }
            cell.SetValue(Clean(Convert.ToString(value, CultureInfo.InvariantCulture)));
            cell.DataType = XLDataType.Text;
            return false;
        }

        /// <summary>Removes characters that are not allowed in a workbook and keeps a cell within Excel's text limit.</summary>
        internal static string Clean(string text)
        {
            if (string.IsNullOrEmpty(text)) return text ?? "";
            StringBuilder cleaned = null;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                bool allowed = ch == '\t' || ch == '\n' || ch == '\r' || (ch >= ' ' && ch != '￾' && ch != '￿');
                if (allowed && char.IsSurrogate(ch))
                    allowed = char.IsHighSurrogate(ch) ? i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]) : i > 0 && char.IsHighSurrogate(text[i - 1]);
                if (!allowed && cleaned == null) cleaned = new StringBuilder(text, 0, i, text.Length);
                if (cleaned != null) cleaned.Append(allowed ? ch : '?');
            }
            string result = cleaned == null ? text : cleaned.ToString();
            return result.Length <= 32000 ? result : result.Substring(0, 31999) + "…";
        }
    }
}
