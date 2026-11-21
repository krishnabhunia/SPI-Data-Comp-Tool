using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SPI_Data_Comp_Tool
{
    /// <summary>What to compare and how rows are matched. The defaults compare everything, by row position.</summary>
    internal sealed class IdentityOptions
    {
        internal int HeaderRow { get; set; } = 1;
        /// <summary>Allowed absolute difference between two numbers.</summary>
        internal double Tolerance { get; set; }
        /// <summary>Primary-key column names of each file; null = match rows by position. One side alone applies to both.</summary>
        internal string[] KeyA { get; set; }
        internal string[] KeyB { get; set; }
        /// <summary>Compare only these sheets; null = pair all sheets by name.</summary>
        internal string SheetA { get; set; }
        internal string SheetB { get; set; }
        internal bool CompareFormats { get; set; } = true;
        internal bool CompareLayout { get; set; } = true;
        internal bool CompareFormulas { get; set; } = true;
        /// <summary>One "Abs Diff &gt; x" report column each.</summary>
        internal double[] Thresholds { get; set; } = { 0, 0.01, 0.1, 1 };
    }

    internal sealed class IdentityCheck
    {
        internal IdentityCheck(string label, string key, string unit, string sheet) { Label = label; Key = key; Unit = unit; Sheet = sheet; }
        internal string Label { get; }
        internal string Key { get; }
        internal string Unit { get; }
        internal string Sheet { get; }
    }

    /// <summary>Everything one comparison found. The rows are ready for the report sheets and for a results grid.</summary>
    internal sealed class IdentityResult
    {
        /// <summary>The 18 summary checks, in report order.</summary>
        internal static readonly IdentityCheck[] Checks =
        {
            new IdentityCheck("Sheet count and sheet names", "workbook", "check(s)", "Layout & Other"),
            new IdentityCheck("Number of headers", "header_count", "sheet(s)", "Headers"),
            new IdentityCheck("Header names (missing, extra or renamed columns)", "header_names", "header(s)", "Headers"),
            new IdentityCheck("Header case and spacing", "header_case", "header(s)", "Headers"),
            new IdentityCheck("Header order", "header_order", "header(s)", "Headers"),
            new IdentityCheck("Header font", "header_font", "header(s)", "Headers"),
            new IdentityCheck("Header alignment", "header_align", "header(s)", "Headers"),
            new IdentityCheck("Number of data rows", "row_count", "sheet(s)", "Data Differences"),
            new IdentityCheck("Rows present in only one file", "rows_only", "row(s)", "Data Differences"),
            new IdentityCheck("Row order (when matched by key)", "row_order", "row(s)", "Layout & Other"),
            new IdentityCheck("Cell values (row by row)", "values", "cell(s)", "Data Differences"),
            new IdentityCheck("Data types (same value, different type)", "types", "cell(s)", "Data Differences"),
            new IdentityCheck("Formulas", "formulas", "cell(s)", "Data Differences"),
            new IdentityCheck("Cell font (data rows)", "fmt_font", "cell(s)", "Format Differences"),
            new IdentityCheck("Cell alignment (data rows)", "fmt_align", "cell(s)", "Format Differences"),
            new IdentityCheck("Number format, fill colour and borders", "fmt_other", "cell(s)", "Format Differences"),
            new IdentityCheck("Sheet layout (widths, heights, merged cells, freeze panes, hidden rows/columns, filter)", "layout", "check(s)", "Layout & Other"),
            new IdentityCheck("Comments, hyperlinks, data validation, conditional formatting", "extras", "check(s)", "Layout & Other"),
        };

        internal readonly List<object[]> Headers = new List<object[]>();      // 14 columns
        internal readonly List<object[]> Data = new List<object[]>();         // 12 columns + one per threshold; [2] is the key
        internal readonly List<object[]> Formats = new List<object[]>();      // 9 columns
        internal readonly List<object[]> Layout = new List<object[]>();       // 5 columns
        internal readonly List<string> Notes = new List<string>();
        internal readonly List<string> KeyMissing = new List<string>();
        internal readonly HashSet<string> Skipped = new HashSet<string>(StringComparer.Ordinal);
        private readonly Dictionary<string, long> counts = new Dictionary<string, long>(StringComparer.Ordinal);

        internal string FileA, FileB, RowMatching = "";
        internal bool Keyed;
        internal IdentityOptions Options = new IdentityOptions();
        internal DateTime Generated = DateTime.Now;

        internal long Count(string key) { long value; return counts.TryGetValue(key, out value) ? value : 0; }
        internal void Add(string key, long by = 1) { if (by != 0) counts[key] = Count(key) + by; }
        internal bool Identical { get { return Checks.All(check => Count(check.Key) == 0); } }
        internal int FailedChecks { get { return Checks.Count(check => Count(check.Key) != 0); } }

        internal string Verdict
        {
            get
            {
                return Identical ? "IDENTICAL - no differences found"
                    : "NOT IDENTICAL - " + FailedChecks.ToString(CultureInfo.InvariantCulture) + " of "
                      + Checks.Length.ToString(CultureInfo.InvariantCulture) + " checks found differences";
            }
        }
    }

    /// <summary>
    /// Answers "are these two workbooks identical?" and lists every difference: headers, values, data types,
    /// formulas, cell formatting and sheet layout. Columns are matched by header name (ignoring case and outer
    /// spaces); rows by position or by an optional primary key. No UI code; nothing is written to the inputs.
    /// The rules are the ones of the approved Python reference tool, so both produce the same report.
    /// </summary>
    internal static class WorkbookIdentityEngine
    {
        internal const string BlankText = "(blank)";
        private const string NullKey = "\0null";
        private static readonly Regex NumericText = new Regex(@"^[+-]?(\d+\.?\d*|\.\d+)([eE][+-]?\d+)?$", RegexOptions.Compiled);
        private static readonly int[] FontProperties = { 0, 1, 2, 3, 4, 5 };
        private static readonly int[] AlignProperties = { 6, 7, 8 };

        internal static IdentityResult Compare(string pathA, string pathB, IdentityOptions options = null,
            Action<int, string> progress = null, Func<bool> cancelled = null)
        {
            IdentityOptions o = options ?? new IdentityOptions();
            if (o.HeaderRow < 1) throw new ArgumentException("The header row must be 1 or more.");
            if (o.Tolerance < 0 || double.IsNaN(o.Tolerance)) throw new ArgumentException("The number tolerance cannot be negative.");
            string[] keyA = CleanNames(o.KeyA) ?? CleanNames(o.KeyB);      // one side given -> same names in both files
            string[] keyB = CleanNames(o.KeyB) ?? CleanNames(o.KeyA);
            if (keyA != null && keyA.Length != keyB.Length)
                throw new ArgumentException("The key has " + keyA.Length + " column(s) for File A but " + keyB.Length + " for File B.");

            var res = new IdentityResult { FileA = pathA, FileB = pathB, Options = o };
            if (!o.CompareFormats) foreach (string key in new[] { "header_font", "header_align", "fmt_font", "fmt_align", "fmt_other" }) res.Skipped.Add(key);
            if (!o.CompareLayout) { res.Skipped.Add("layout"); res.Skipped.Add("extras"); }
            if (!o.CompareFormulas) res.Skipped.Add("formulas");

            using (var a = new IdentityWorkbook(pathA))
            using (var b = new IdentityWorkbook(pathB))
            {
                List<string> na = a.SheetNames, nb = b.SheetNames;
                if (na.Count == 0) throw new InvalidDataException(Path.GetFileName(pathA) + " has no worksheets.");
                if (nb.Count == 0) throw new InvalidDataException(Path.GetFileName(pathB) + " has no worksheets.");
                bool chosen = !string.IsNullOrEmpty(o.SheetA) || !string.IsNullOrEmpty(o.SheetB);
                var pairs = new List<KeyValuePair<string, string>>();
                if (chosen)
                {
                    pairs.Add(new KeyValuePair<string, string>(string.IsNullOrEmpty(o.SheetA) ? na[0] : o.SheetA, string.IsNullOrEmpty(o.SheetB) ? nb[0] : o.SheetB));
                    res.Notes.Add("Only the selected sheets were compared.");
                }
                else if (na.Count == 1 && nb.Count == 1) pairs.Add(new KeyValuePair<string, string>(na[0], nb[0]));
                else
                {
                    var lookup = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (string name in nb) lookup[name.ToLowerInvariant()] = name;
                    foreach (string name in na)
                    {
                        string partner;
                        if (lookup.TryGetValue(name.ToLowerInvariant(), out partner)) pairs.Add(new KeyValuePair<string, string>(name, partner));
                    }
                }
                foreach (var pair in pairs)
                {
                    if (!na.Contains(pair.Key)) throw new ArgumentException("Sheet '" + pair.Key + "' not found in File A.");
                    if (!nb.Contains(pair.Value)) throw new ArgumentException("Sheet '" + pair.Value + "' not found in File B.");
                }
                if (!chosen)
                {
                    Check(res, "Workbook", "Number of sheets", Show(na.Count), Show(nb.Count), na.Count == nb.Count, "workbook");
                    Check(res, "Workbook", "Sheet names (in order)", Show(na), Show(nb), na.SequenceEqual(nb), "workbook");
                    var aloneA = na.Where(name => pairs.All(pair => pair.Key != name)).ToList();
                    var aloneB = nb.Where(name => pairs.All(pair => pair.Value != name)).ToList();
                    Check(res, "Workbook", "Sheets without a partner in the other file", Show(aloneA), Show(aloneB), aloneA.SequenceEqual(aloneB), "workbook");
                }

                bool usedKey = false;
                for (int i = 0; i < pairs.Count; i++)
                {
                    int from = i * 100 / pairs.Count, span = 100 / pairs.Count;
                    Report(progress, from, "Reading sheet '" + pairs[i].Key + "' of File A");
                    IdentitySheet sheetA = a.ReadSheet(pairs[i].Key, cancelled);
                    Report(progress, from + span / 4, "Reading sheet '" + pairs[i].Value + "' of File B");
                    IdentitySheet sheetB = b.ReadSheet(pairs[i].Value, cancelled);
                    Report(progress, from + span / 2, "Comparing sheet '" + pairs[i].Key + "'");
                    usedKey |= CompareSheets(sheetA, sheetB, o, keyA, keyB, res, cancelled);
                }
                if (keyA != null && !usedKey) throw new ArgumentException("Key column(s) not found - " + string.Join(" | ", res.KeyMissing));
                res.Keyed = usedKey;
            }
            res.RowMatching = res.Keyed
                ? "By primary key - File A: " + string.Join(" + ", keyA) + "  ↔  File B: " + string.Join(" + ", keyB) + " (key column names matched ignoring case)"
                : "By position (header in row " + o.HeaderRow + "; row N of File A against row N of File B)";
            Report(progress, 100, "Comparison finished");
            return res;
        }

        private static void Report(Action<int, string> progress, int percent, string message)
        {
            if (progress != null) progress(Math.Max(0, Math.Min(100, percent)), message);
        }

        private static string[] CleanNames(string[] names)
        {
            if (names == null) return null;
            string[] kept = names.Where(name => !string.IsNullOrWhiteSpace(name)).Select(name => name.Trim()).ToArray();
            return kept.Length == 0 ? null : kept;
        }

        // ------------------------------------------------------------------ small helpers
        private sealed class Header
        {
            internal int Col;
            internal string Raw, Key;
            internal IdentityCell Cell;
        }

        private sealed class RowPair { internal int A, B; internal string Key; }
        private sealed class KeyRow { internal int Row; internal string Join, Text; internal bool NullKey; internal int Occurrence; }
        private struct FormatDiff { internal int Row, RowB, ColA, ColB, Property; internal string A, B; internal bool Header; }

        private static string Letter(int column) { return IdentityWorkbook.ColumnLetters(column); }
        private static string Num(long value) { return value.ToString(CultureInfo.InvariantCulture); }
        private static string Quote(string value) { return value == null ? BlankText : "\"" + value + "\""; }

        internal static string Limit(string text, int limit = 300)
        {
            if (string.IsNullOrEmpty(text)) return "none";
            return text.Length <= limit ? text : text.Substring(0, limit - 1) + "…";
        }

        private static string Show(int value) { return Num(value); }
        private static string Show(string value) { return Limit(value); }
        private static string Show(IEnumerable<string> ordered) { return Limit(string.Join(", ", ordered)); }

        private static string ShowSet(IEnumerable<string> items)
        {
            return Limit(string.Join(", ", items.OrderBy(item => item.Length).ThenBy(item => item, StringComparer.Ordinal)));
        }

        private static string ShowMap(IEnumerable<KeyValuePair<string, string>> items)
        {
            return Limit(string.Join(", ", items.Select(item => item.Key + "=" + item.Value)));
        }

        /// <summary>A number the way the reference tool prints a float: 22.0, 21.57.</summary>
        private static string FloatText(double value)
        {
            string text = value.ToString("R", CultureInfo.InvariantCulture);
            return text.IndexOfAny(new[] { '.', 'E', 'e' }) < 0 ? text + ".0" : text;
        }

        private static void Check(IdentityResult res, string scope, string name, string a, string b, bool same, string category)
        {
            res.Layout.Add(new object[] { scope, name, a, b, same ? "MATCH" : "DIFF" });
            if (!same) res.Add(category);
        }

        private static void CheckSet(IdentityResult res, string scope, string name, ICollection<string> a, ICollection<string> b, string category)
        {
            Check(res, scope, name, ShowSet(a), ShowSet(b), new HashSet<string>(a, StringComparer.Ordinal).SetEquals(b), category);
        }

        private static void CheckMap(IdentityResult res, string scope, string name, IDictionary<string, string> a, IDictionary<string, string> b,
            string category, string missing)
        {
            var keys = new List<string>();
            foreach (string key in a.Keys.Concat(b.Keys).Distinct(StringComparer.Ordinal))
            {
                string left, right;
                bool hasLeft = a.TryGetValue(key, out left), hasRight = b.TryGetValue(key, out right);
                if (hasLeft != hasRight || !string.Equals(left, right, StringComparison.Ordinal)) keys.Add(key);
            }
            if (keys.Count == 0)
            {
                res.Layout.Add(new object[] { scope, name, "same in both files", "same in both files", "MATCH" });
                return;
            }
            Func<IDictionary<string, string>, string> show = map => ShowMap(keys.Select(key =>
            {
                string value;
                return new KeyValuePair<string, string>(key, map.TryGetValue(key, out value) ? value : missing);
            }));
            res.Layout.Add(new object[] { scope, name, show(a), show(b), "DIFF" });
            res.Add(category);
        }

        private static List<Header> HeaderList(IdentitySheet sheet, int headerRow, Dictionary<int, string> forced = null)
        {
            var seen = new Dictionary<string, int>(StringComparer.Ordinal);
            var list = new List<Header>();
            for (int col = 1; col <= sheet.LastCol; col++)
            {
                IdentityCell cell = sheet.Cell(headerRow, col);
                string key;
                if (forced == null || !forced.TryGetValue(col, out key))
                {
                    string basis = cell.Value != null ? cell.Value.Trim().ToLowerInvariant() : "\0blank" + Num(col);
                    int times;
                    seen.TryGetValue(basis, out times);
                    seen[basis] = ++times;
                    key = times == 1 ? basis : basis + "\0" + Num(times);              // duplicate header names
                }
                list.Add(new Header { Col = col, Raw = cell.Value, Key = key, Cell = cell });
            }
            return list;
        }

        /// <summary>Key column names -> column numbers, ignoring case. Null when any name is missing.</summary>
        private static int[] ResolveKey(List<Header> headers, string[] wanted)
        {
            var index = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (Header header in headers)
            {
                if (header.Raw == null) continue;
                string name = header.Raw.Trim().ToLowerInvariant();
                if (!index.ContainsKey(name)) index[name] = header.Col;
            }
            var cols = new int[wanted.Length];
            for (int i = 0; i < wanted.Length; i++)
                if (!index.TryGetValue(wanted[i].Trim().ToLowerInvariant(), out cols[i])) return null;
            return cols;
        }

        private static string FontText(string[] style)
        {
            string text = style[IdentityStyle.FontName] + " " + style[IdentityStyle.FontSize] + "pt";
            if (style[IdentityStyle.Bold] == "Yes") text += " Bold";
            if (style[IdentityStyle.Italic] == "Yes") text += " Italic";
            if (style[IdentityStyle.Underline] != "none") text += " Underline";
            string colour = style[IdentityStyle.FontColour];
            if (colour != "none" && colour != "theme 1") text += ", colour " + colour;
            return text;
        }

        private static string AlignText(string[] style)
        {
            return style[IdentityStyle.HAlign] + " / " + style[IdentityStyle.VAlign] + (style[IdentityStyle.Wrap] == "Yes" ? " / wrap" : "");
        }

        private static string NameCheck(string a, string b)
        {
            if (a == b) return "Exact match";
            if (a == null || b == null) return "Blank in one file";
            if (a.Trim().ToLowerInvariant() != b.Trim().ToLowerInvariant()) return "Name differs";      // only for a key column pair
            if (a.Trim() == b.Trim()) return "Spacing differs";
            return a.ToLowerInvariant() == b.ToLowerInvariant() ? "Case differs" : "Case and spacing differ";
        }

        private static double? AsNumber(IdentityCell cell)
        {
            if (cell.Kind == "number") return cell.Number;
            if (cell.Kind != "text" || cell.Value == null) return null;
            string text = cell.Value.Trim();
            double number;
            if (!NumericText.IsMatch(text)) return null;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number) && !double.IsInfinity(number) ? number : (double?)null;
        }

        private static string Shown(string value, string kind, string difference)
        {
            if (value == null) return BlankText;
            bool padded = kind == "text" && value != value.Trim();
            string text = padded || difference.Contains("pacing") ? "\"" + value + "\"" : value;
            return difference.Contains("ata type") ? text + "  (" + kind + ")" : text;
        }

        /// <summary>Number difference without floating-point noise (0.009999999999999787 -> 0.01).</summary>
        private static object CleanDelta(double? delta)
        {
            if (delta == null) return null;
            double rounded = Math.Round(delta.Value, 9);
            return rounded != 0 ? rounded : delta.Value;              // keep the raw value when it is below 0.000000001
        }

        private static void AddFlags(object[] row, int start, double? delta, double[] thresholds)
        {
            if (delta == null) return;
            double size = Math.Abs(delta.Value);
            for (int i = 0; i < thresholds.Length; i++)
                row[start + i] = (thresholds[i] == 0 ? size > 0 : Math.Round(size, 9) > thresholds[i]) ? "Yes" : "No";
        }

        private static List<KeyRow> KeyRows(IdentitySheet sheet, int[] cols, int headerRow)
        {
            var rows = new SortedSet<int>();
            foreach (var item in sheet.Cells)
            {
                if (item.Value.Value == null) continue;
                int row = (int)(item.Key >> 15);
                if (row > headerRow) rows.Add(row);
            }
            var seen = new Dictionary<string, int>(StringComparer.Ordinal);
            var list = new List<KeyRow>(rows.Count);
            foreach (int row in rows)
            {
                var join = new StringBuilder();
                var text = new StringBuilder();
                bool blank = false;
                for (int i = 0; i < cols.Length; i++)
                {
                    string part = sheet.Cell(row, cols[i]).Value;
                    if (i > 0) { join.Append('\u0001'); text.Append(" | "); }
                    blank |= part == null;
                    join.Append(part ?? NullKey);
                    text.Append(part ?? BlankText);
                }
                string key = join.ToString();
                int times;
                seen.TryGetValue(key, out times);
                seen[key] = times + 1;
                list.Add(new KeyRow { Row = row, Join = key, Text = text.ToString(), NullKey = blank, Occurrence = times });
            }
            return list;
        }

        private static void ReportOnlyRows(IdentitySheet sheet, List<Header> headers, IEnumerable<KeyValuePair<int, string>> rows, bool sideA,
            string label, IdentityResult res, int flags)
        {
            foreach (var entry in rows)
            {
                var parts = new List<string>();
                foreach (Header header in headers)
                {
                    string value = sheet.Cell(entry.Key, header.Col).Value;
                    if (value != null) parts.Add((header.Raw ?? BlankText) + "=" + value);
                }
                if (parts.Count == 0) continue;
                var row = new object[12 + flags];
                row[0] = label; row[1] = entry.Key; row[2] = entry.Value; row[3] = "(entire row)";
                row[4] = sideA ? "row " + Num(entry.Key) : ""; row[5] = sideA ? "" : "row " + Num(entry.Key);
                row[6] = sideA ? Limit(string.Join(" | ", parts)) : ""; row[7] = sideA ? "" : Limit(string.Join(" | ", parts));
                row[8] = "Row only in File " + (sideA ? "A" : "B"); row[10] = "True";
                res.Data.Add(row);
                res.Add("rows_only");
            }
        }

        // ------------------------------------------------------------------ one pair of sheets
        /// <summary>Compares one sheet pair. Returns true when rows were matched by primary key.</summary>
        private static bool CompareSheets(IdentitySheet A, IdentitySheet B, IdentityOptions o, string[] keyA, string[] keyB,
            IdentityResult res, Func<bool> cancelled)
        {
            int headerRow = o.HeaderRow, flags = o.Thresholds.Length;
            string label = A.Name == B.Name ? A.Name : A.Name + " ↔ " + B.Name;
            List<Header> ha = HeaderList(A, headerRow), hb = HeaderList(B, headerRow);

            bool keyed = false;
            int[] colsA = null, colsB = null;
            if (keyA != null)
            {
                colsA = ResolveKey(ha, keyA);
                colsB = ResolveKey(hb, keyB);
                if (colsA != null && colsB != null)
                {
                    keyed = true;        // pair the key columns with each other even when their names differ
                    var forcedA = new Dictionary<int, string>();
                    var forcedB = new Dictionary<int, string>();
                    for (int i = 0; i < colsA.Length; i++) { forcedA[colsA[i]] = "\0key" + Num(i); forcedB[colsB[i]] = "\0key" + Num(i); }
                    ha = HeaderList(A, headerRow, forcedA);
                    hb = HeaderList(B, headerRow, forcedB);
                }
                else
                {
                    var missing = new List<string>();
                    string[] goneA = keyA.Where(name => ResolveKey(ha, new[] { name }) == null).ToArray();
                    string[] goneB = keyB.Where(name => ResolveKey(hb, new[] { name }) == null).ToArray();
                    if (goneA.Length > 0) missing.Add("File A has no column named " + string.Join(", ", goneA.Select(name => "'" + name + "'")));
                    if (goneB.Length > 0) missing.Add("File B has no column named " + string.Join(", ", goneB.Select(name => "'" + name + "'")));
                    res.KeyMissing.Add("sheet '" + label + "', header row " + Num(headerRow) + ": " + string.Join("; ", missing));
                    res.Notes.Add("Key not found in " + res.KeyMissing[res.KeyMissing.Count - 1] + ". This sheet was matched by row position instead.");
                }
            }
            var byA = new Dictionary<string, Header>(StringComparer.Ordinal);
            var byB = new Dictionary<string, Header>(StringComparer.Ordinal);
            foreach (Header header in ha) byA[header.Key] = header;
            foreach (Header header in hb) byB[header.Key] = header;

            // ---- 1. headers
            if (ha.Count != hb.Count) res.Add("header_count");
            foreach (string key in ha.Select(header => header.Key).Concat(hb.Select(header => header.Key)).Distinct(StringComparer.Ordinal))
            {
                Header a, b;
                byA.TryGetValue(key, out a);
                byB.TryGetValue(key, out b);
                string la = a == null ? "" : Letter(a.Col), lb = b == null ? "" : Letter(b.Col);
                string name, position, font, align;
                if (a != null && b != null)
                {
                    string[] sa = A.StyleOf(a.Cell), sb = B.StyleOf(b.Cell);
                    name = NameCheck(a.Raw, b.Raw);
                    position = a.Col == b.Col ? "Same" : "Moved (" + la + " → " + lb + ")";
                    font = !o.CompareFormats ? "—" : FontProperties.All(p => sa[p] == sb[p]) ? "Same" : "Differs";
                    align = !o.CompareFormats ? "—" : AlignProperties.All(p => sa[p] == sb[p]) ? "Same" : "Differs";
                    if (name == "Name differs") res.Add("header_names");
                    else if (name != "Exact match") res.Add("header_case");
                    if (position != "Same") res.Add("header_order");
                    if (font == "Differs") res.Add("header_font");
                    if (align == "Differs") res.Add("header_align");
                }
                else
                {
                    name = a != null ? "Only in File A" : "Only in File B";
                    position = font = align = "—";
                    res.Add("header_names");
                }
                bool spaced = name.Contains("pacing");
                Func<Header, string> show = header => header == null ? "" : header.Raw == null ? BlankText : spaced ? Quote(header.Raw) : header.Raw;
                bool ok = name == "Exact match" && position == "Same" && font != "Differs" && align != "Differs";
                res.Headers.Add(new object[]
                {
                    label, la, show(a), lb, show(b), name, position,
                    a == null ? "" : FontText(A.StyleOf(a.Cell)), b == null ? "" : FontText(B.StyleOf(b.Cell)), font,
                    a == null ? "" : AlignText(A.StyleOf(a.Cell)), b == null ? "" : AlignText(B.StyleOf(b.Cell)), align,
                    ok ? "MATCH" : "DIFF",
                });
            }

            // ---- pair the rows: by key, or by position
            int commonRows = Math.Min(A.LastRow, B.LastRow);
            int top = Math.Min(headerRow, commonRows);                    // header and title rows always pair by position
            var pairs = new List<RowPair>();
            for (int row = 1; row <= top; row++) pairs.Add(new RowPair { A = row, B = row });
            int fixedPairs = pairs.Count;
            var onlyA = new List<KeyValuePair<int, string>>();
            var onlyB = new List<KeyValuePair<int, string>>();
            List<KeyRow> ka = null, kb = null;
            if (keyed)
            {
                ka = KeyRows(A, colsA, headerRow);
                kb = KeyRows(B, colsB, headerRow);
                var target = new Dictionary<string, KeyRow>(StringComparer.Ordinal);
                foreach (KeyRow row in kb) target[row.Join + "\u0002" + Num(row.Occurrence)] = row;
                foreach (KeyRow row in ka)
                {
                    KeyRow partner;
                    string joined = row.Join + "\u0002" + Num(row.Occurrence);
                    if (target.TryGetValue(joined, out partner))
                    {
                        pairs.Add(new RowPair { A = row.Row, B = partner.Row, Key = row.Text });
                        target.Remove(joined);
                    }
                    else onlyA.Add(new KeyValuePair<int, string>(row.Row, row.Text));
                }
                foreach (KeyRow row in target.Values.OrderBy(item => item.Row)) onlyB.Add(new KeyValuePair<int, string>(row.Row, row.Text));
            }
            else
            {
                for (int row = top + 1; row <= commonRows; row++) pairs.Add(new RowPair { A = row, B = row });
                for (int row = commonRows + 1; row <= A.LastRow; row++) onlyA.Add(new KeyValuePair<int, string>(row, null));
                for (int row = commonRows + 1; row <= B.LastRow; row++) onlyB.Add(new KeyValuePair<int, string>(row, null));
            }

            // ---- columns present in both files, in File A order
            List<Header> common = ha.Where(header => byB.ContainsKey(header.Key)).ToList();
            Func<Header, string> columnName = header => header.Raw ?? BlankText;

            // ---- 2. values and formulas, 3. cell formatting
            var formulaRows = new List<object[]>();
            var formatDiffs = new List<FormatDiff>();
            var styleCache = new Dictionary<long, int[]>();
            double tolerance = o.Tolerance;
            int counter = 0;
            foreach (RowPair pair in pairs)
            {
                if (cancelled != null && (++counter & 0xFF) == 0 && cancelled()) throw new OperationCanceledException();
                bool isHeader = pair.A == headerRow;
                foreach (Header column in common)
                {
                    Header other = byB[column.Key];
                    IdentityCell x = A.Cell(pair.A, column.Col), y = B.Cell(pair.B, other.Col);

                    if (o.CompareFormats)
                    {
                        long styleKey = ((long)x.Style << 32) | (uint)y.Style;
                        int[] differing;
                        if (!styleCache.TryGetValue(styleKey, out differing))
                        {
                            string[] sa = A.StyleOf(x), sb = B.StyleOf(y);
                            differing = Enumerable.Range(0, IdentityStyle.Labels.Length).Where(p => sa[p] != sb[p]).ToArray();
                            styleCache[styleKey] = differing;
                        }
                        if (differing.Length > 0)
                        {
                            string[] sa = A.StyleOf(x), sb = B.StyleOf(y);
                            foreach (int property in differing)
                                formatDiffs.Add(new FormatDiff { Row = pair.A, RowB = pair.B, ColA = column.Col, ColB = other.Col, Property = property, A = sa[property], B = sb[property], Header = isHeader });
                        }
                    }
                    if (isHeader) continue;

                    bool bothNumbers = x.Kind == "number" && y.Kind == "number";
                    bool same = bothNumbers ? Math.Abs(x.Number - y.Number) <= tolerance
                        : string.Equals(x.Value, y.Value, StringComparison.Ordinal) && x.Kind == y.Kind;
                    string cellA = Letter(column.Col) + Num(pair.A), cellB = Letter(other.Col) + Num(pair.B);
                    if (!same)
                    {
                        string kind;
                        if (x.Kind == "blank") kind = "Blank in File A";
                        else if (y.Kind == "blank") kind = "Blank in File B";
                        else if (bothNumbers) kind = "Number changed";
                        else if (x.Kind != y.Kind)
                            kind = (x.Value == y.Value ? "Data type differs (" : "Value and data type differ (") + x.Kind + " vs " + y.Kind + ")";
                        else if (x.Kind == "text")
                            kind = x.Value.Trim() == y.Value.Trim() ? "Spacing differs only"
                                : x.Value.ToLowerInvariant() == y.Value.ToLowerInvariant() ? "Case differs only"
                                : x.Value.Trim().ToLowerInvariant() == y.Value.Trim().ToLowerInvariant() ? "Case and spacing differ" : "Text changed";
                        else kind = x.Kind == "date" ? "Date changed" : "Value changed";

                        // Bring both values to the same platform: a number, or text that reads as a number, becomes a
                        // number; everything else is compared as text.
                        double? numberA = AsNumber(x), numberB = AsNumber(y);
                        bool both = numberA != null && numberB != null;
                        double? delta = both ? numberB.Value - numberA.Value : (double?)null;
                        bool numeric = both || (x.Kind == "blank" && numberB != null) || (y.Kind == "blank" && numberA != null);
                        bool differs = both ? Math.Abs(delta.Value) > tolerance : !string.Equals(x.Value, y.Value, StringComparison.Ordinal);
                        string shownA = Shown(x.Value, x.Kind, kind), shownB = Shown(y.Value, y.Kind, kind);
                        if (kind == "Number changed" && shownA == shownB)             // differ only beyond the 15th digit
                        {
                            shownA = x.Number.ToString("R", CultureInfo.InvariantCulture);
                            shownB = y.Number.ToString("R", CultureInfo.InvariantCulture);
                        }
                        var row = new object[12 + flags];
                        row[0] = label; row[1] = pair.A; row[2] = pair.Key; row[3] = columnName(column); row[4] = cellA; row[5] = cellB;
                        row[6] = shownA; row[7] = shownB; row[8] = kind; row[9] = numeric ? "Number" : "Text"; row[10] = differs ? "True" : "False";
                        row[11] = CleanDelta(delta);
                        AddFlags(row, 12, delta, o.Thresholds);
                        res.Data.Add(row);
                        res.Add(kind.StartsWith("Data type differs", StringComparison.Ordinal) ? "types" : "values");
                    }
                    // Formulas: the calculated results were compared above.
                    if (o.CompareFormulas && !string.Equals(x.Formula, y.Formula, StringComparison.Ordinal))
                    {
                        var row = new object[12 + flags];
                        row[0] = label; row[1] = pair.A; row[2] = pair.Key; row[3] = columnName(column); row[4] = cellA; row[5] = cellB;
                        row[6] = x.Formula ?? "(no formula)"; row[7] = y.Formula ?? "(no formula)";
                        row[8] = x.Formula == null ? "Formula only in File B" : y.Formula == null ? "Formula only in File A" : "Formula differs";
                        row[9] = "Text"; row[10] = "True";
                        formulaRows.Add(row);
                        res.Add("formulas");
                    }
                }
            }
            res.Data.AddRange(formulaRows);

            // rows that exist in only one file
            if (A.LastRow != B.LastRow) res.Add("row_count");
            ReportOnlyRows(A, ha, onlyA, true, label, res, flags);
            ReportOnlyRows(B, hb, onlyB, false, label, res, flags);

            // ---- 3. cell formatting: collapse consecutive rows with the same difference into one range
            if (formatDiffs.Count > 0)
            {
                formatDiffs.Sort((p, q) =>
                {
                    int order = p.ColA.CompareTo(q.ColA);
                    if (order == 0) order = p.Property.CompareTo(q.Property);
                    if (order == 0) order = p.Header.CompareTo(q.Header);
                    if (order == 0) order = string.CompareOrdinal(p.A, q.A);
                    if (order == 0) order = string.CompareOrdinal(p.B, q.B);
                    if (order == 0) order = (p.RowB - p.Row).CompareTo(q.RowB - q.Row);
                    return order != 0 ? order : p.Row.CompareTo(q.Row);
                });
                var blocks = new List<object[]>();                  // first row, column, property label, then the report row
                int start = 0;
                for (int i = 1; i <= formatDiffs.Count; i++)
                {
                    FormatDiff first = formatDiffs[start];
                    if (i < formatDiffs.Count)
                    {
                        FormatDiff previous = formatDiffs[i - 1], current = formatDiffs[i];
                        if (current.Row == previous.Row + 1 && current.ColA == first.ColA && current.Property == first.Property && current.Header == first.Header
                            && current.A == first.A && current.B == first.B && current.RowB - current.Row == first.RowB - first.Row) continue;
                    }
                    FormatDiff last = formatDiffs[i - 1];
                    int cells = i - start, offset = first.RowB - first.Row;
                    string category = first.Property <= 5 ? "font" : first.Property <= 8 ? "align" : "other";
                    string la = Letter(first.ColA), lb = Letter(first.ColB);
                    Func<string, int, int, string> range = (letters, lo, hi) => lo == hi ? letters + Num(lo) : letters + Num(lo) + ":" + letters + Num(hi);
                    blocks.Add(new object[]
                    {
                        first.Row, first.ColA, IdentityStyle.Labels[first.Property],
                        new object[]
                        {
                            label, first.Header ? "Header" : "Data", columnName(byA.Values.First(header => header.Col == first.ColA)),
                            range(la, first.Row, last.Row), range(lb, first.Row + offset, last.Row + offset), cells,
                            IdentityStyle.Labels[first.Property], first.A, first.B,
                        },
                    });
                    if (!first.Header) res.Add("fmt_" + category, cells);
                    else if (category == "other") res.Add("fmt_other", cells);
                    start = i;
                }
                foreach (object[] block in blocks.OrderBy(item => (int)item[0]).ThenBy(item => (int)item[1]).ThenBy(item => (string)item[2], StringComparer.Ordinal))
                    res.Formats.Add((object[])block[3]);
            }

            // ---- 4. sheet layout and everything else
            string scope = label;
            var a2b = new Dictionary<int, int>();
            var b2a = new Dictionary<int, int>();
            foreach (RowPair pair in pairs) { a2b[pair.A] = pair.B; b2a[pair.B] = pair.A; }
            var colB2A = new Dictionary<int, int>();
            foreach (Header column in common) colB2A[byB[column.Key].Col] = column.Col;

            if (keyed)
            {
                res.Layout.Add(new object[] { scope, "Row matching", "By key: " + string.Join(" + ", keyA), "By key: " + string.Join(" + ", keyB), "—" });
                var warnings = new[]
                {
                    new KeyValuePair<string, Func<List<KeyRow>, int>>("Primary key: rows with a blank key value", rows => rows.Count(row => row.NullKey)),
                    new KeyValuePair<string, Func<List<KeyRow>, int>>("Primary key: rows with a repeated key value", rows =>
                    {
                        var repeated = new HashSet<string>(rows.Where(row => row.Occurrence > 0).Select(row => row.Join), StringComparer.Ordinal);
                        return rows.Count(row => !row.NullKey && repeated.Contains(row.Join));
                    }),
                };
                foreach (var warning in warnings)
                {
                    int inA = warning.Value(ka), inB = warning.Value(kb);
                    res.Layout.Add(new object[] { scope, warning.Key, inA, inB, inA == 0 && inB == 0 ? "OK" : "WARN" });
                    if (inA != 0 || inB != 0)
                        res.Notes.Add("Sheet '" + label + "' - " + warning.Key.ToLowerInvariant() + ": " + Num(inA) + " in File A, " + Num(inB)
                            + " in File B. These rows were matched in order of appearance.");
                }
                int moved = 0, highest = int.MinValue;
                for (int i = fixedPairs; i < pairs.Count; i++)
                {
                    if (pairs[i].B < highest) moved++;
                    highest = Math.Max(highest, pairs[i].B);
                }
                res.Layout.Add(new object[] { scope, "Row order (rows present in both files)", "order as in File A",
                    moved == 0 ? "same order" : moved.ToString("N0", CultureInfo.InvariantCulture) + " row(s) out of sequence", moved == 0 ? "MATCH" : "DIFF" });
                res.Add("row_order", moved);
            }
            else res.Layout.Add(new object[] { scope, "Row matching", "By row position", "By row position", "—" });

            Check(res, scope, "Number of headers", Show(ha.Count), Show(hb.Count), ha.Count == hb.Count, "x");
            int dataA = Math.Max(A.LastRow - headerRow, 0), dataB = Math.Max(B.LastRow - headerRow, 0);
            Check(res, scope, "Number of data rows", Show(dataA), Show(dataB), dataA == dataB, "x");
            List<Header> orderA = common, orderB = hb.Where(header => byA.ContainsKey(header.Key)).ToList();
            res.Layout.Add(new object[] { scope, "Column order (columns present in both files)", Show(orderA.Select(columnName)), Show(orderB.Select(columnName)),
                orderA.Select(header => header.Key).SequenceEqual(orderB.Select(header => header.Key)) ? "MATCH" : "DIFF" });

            if (o.CompareLayout)
            {
                var widthsA = new Dictionary<string, string>(StringComparer.Ordinal);
                var widthsB = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (Header column in common)
                {
                    double width;
                    widthsA[columnName(column)] = A.Widths.TryGetValue(column.Col, out width) ? FloatText(width) : "default";
                    widthsB[columnName(column)] = B.Widths.TryGetValue(byB[column.Key].Col, out width) ? FloatText(width) : "default";
                }
                CheckMap(res, scope, "Column widths", widthsA, widthsB, "layout", "default");
                var heightsA = new Dictionary<string, string>(StringComparer.Ordinal);
                var heightsB = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var item in A.Heights.OrderBy(entry => entry.Key)) if (a2b.ContainsKey(item.Key)) heightsA["row " + Num(item.Key)] = FloatText(item.Value);
                foreach (var item in B.Heights.OrderBy(entry => entry.Key)) if (b2a.ContainsKey(item.Key)) heightsB["row " + Num(b2a[item.Key])] = FloatText(item.Value);
                CheckMap(res, scope, "Row heights", heightsA, heightsB, "layout", "default");
                CheckSet(res, scope, "Merged cells", A.Merged, B.Merged, "layout");
                Check(res, scope, "Freeze panes", Show(A.Freeze), Show(B.Freeze), A.Freeze == B.Freeze, "layout");
                Check(res, scope, "Auto-filter range", Show(A.Filter), Show(B.Filter), A.Filter == B.Filter, "layout");
                CheckSet(res, scope, "Hidden columns", A.HiddenColumns, B.HiddenColumns, "layout");
                CheckSet(res, scope, "Hidden rows", A.HiddenRows.Select(row => Num(row)).ToList(),
                    B.HiddenRows.Select(row => b2a.ContainsKey(row) ? Num(b2a[row]) : Num(row) + " (File B row)").ToList(), "layout");
                Check(res, scope, "Sheet visibility", Show(A.State), Show(B.State), A.State == B.State, "layout");

                // A cell address in File B -> the address of the matching cell in File A.
                Func<string, string> toA = address =>
                {
                    int row, col;
                    if (!IdentityWorkbook.SplitReference(address, out row, out col)) return address;
                    return colB2A.ContainsKey(col) && b2a.ContainsKey(row) ? Letter(colB2A[col]) + Num(b2a[row]) : address + " (File B cell)";
                };
                Func<Dictionary<string, string>, Dictionary<string, string>> moved = map =>
                {
                    var result = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (var item in map) result[toA(item.Key)] = item.Value;
                    return result;
                };
                CheckMap(res, scope, "Cell comments", A.Comments, moved(B.Comments), "extras", "none");
                CheckMap(res, scope, "Hyperlinks", A.Links, moved(B.Links), "extras", "none");
                Check(res, scope, "Data validation rules", Show(A.Validations), Show(B.Validations), A.Validations.SequenceEqual(B.Validations), "extras");
                Check(res, scope, "Conditional formatting rules", Show(A.ConditionalFormats), Show(B.ConditionalFormats),
                    A.ConditionalFormats.SequenceEqual(B.ConditionalFormats), "extras");
            }
            if (A.UncachedFormulas > 0) res.Notes.Add(UncachedNote("A", A));
            if (B.UncachedFormulas > 0) res.Notes.Add(UncachedNote("B", B));
            return keyed;
        }

        private static string UncachedNote(string side, IdentitySheet sheet)
        {
            return "File " + side + ", sheet '" + sheet.Name + "': " + Num(sheet.UncachedFormulas)
                + " formula cell(s) have no saved result (the file was never calculated in Excel), so their values compare as blank.";
        }
    }
}
