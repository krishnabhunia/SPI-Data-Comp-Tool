using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// A separate, strict worksheet auditor. It never invokes or changes the legacy comparison engine.
    /// Row XML is streamed. Keyed runs partition their row snapshots on disk, then compare one bounded
    /// partition at a time. Hashes select a partition only; full typed keys decide equality.
    /// </summary>
    internal static class ExcelAuditEngine
    {
        private const int PartitionCount = 64;

        internal static ExcelAuditResult Compare(ExcelAuditOptions options, CancellationToken token,
            Action<ExcelAuditProgress> progress = null)
        {
            return Run(options, token, progress, false);
        }

        /// <summary>Fully scans headers and keys, without publishing a comparison or claiming equality.</summary>
        internal static ExcelAuditResult Validate(ExcelAuditOptions options, CancellationToken token,
            Action<ExcelAuditProgress> progress = null)
        {
            return Run(options, token, progress, true);
        }

        private static ExcelAuditResult Run(ExcelAuditOptions supplied, CancellationToken token,
            Action<ExcelAuditProgress> progress, bool validationOnly)
        {
            if (supplied == null) throw new ArgumentNullException(nameof(supplied));
            ExcelAuditOptions options = supplied.Snapshot();
            ValidateOptions(options);
            token.ThrowIfCancellationRequested();
            var watch = Stopwatch.StartNew();
            var result = new ExcelAuditResult(options);
            result.IsValidationOnly = validationOnly;
            Notify(progress, "Opening worksheets", 0);
            ExcelAuditReader source = null, target = null;
            Exception primaryFailure = null;
            try
            {
                source = new ExcelAuditReader(options.FileA, options.SheetA, options.HeaderRowA, options.TempDirectory, token);
                target = new ExcelAuditReader(options.FileB, options.SheetB, options.HeaderRowB, options.TempDirectory, token);
                result.Source = source.Info;
                result.Target = target.Info;
                Dictionary<string, int> columnsA = HeaderIndex(source.Info, "File A");
                Dictionary<string, int> columnsB = HeaderIndex(target.Info, "File B");
                int[] keysA = KeyColumns(options.KeysA, columnsA, "File A");
                int[] keysB = KeyColumns(options.KeysB, columnsB, "File B");
                List<ColumnPair> pairs = PairColumns(source.Info, target.Info, keysA, keysB);
                if (!validationOnly) CompareMetadata(result, pairs, token);
                if (options.UseKeys)
                    CompareKeyed(source, target, keysA, keysB, pairs, result, token, progress, validationOnly);
                else
                    ComparePositional(source, target, pairs, result, token, progress, validationOnly);

                // Reader warnings may be discovered while streaming, not only while opening a workbook.
                foreach (string warning in source.Info.Warnings.Distinct(StringComparer.Ordinal)) result.Warn("File A: " + warning);
                foreach (string warning in target.Info.Warnings.Distinct(StringComparer.Ordinal)) result.Warn("File B: " + warning);
                if (!validationOnly && options.CompareLayout && result.RowsA != result.RowsB)
                    result.Add(new ExcelAuditFinding
                    {
                        Category = ExcelAuditCategory.Structure,
                        Difference = "Data record count",
                        ValueA = Invariant(result.RowsA),
                        ValueB = Invariant(result.RowsB),
                        Details = "Counts include rows containing values; entirely blank rows are not data records."
                    });
                if (!validationOnly && (options.CompareFormatting || options.CompareLayout)
                    && (result.OnlyA != 0 || result.OnlyB != 0))
                    result.Warn("Some records exist only in one file. Per-record formatting/layout cannot be fully compared for records without a counterpart.");
            }
            catch (Exception ex) { primaryFailure = ex; throw; }
            finally { DisposeResources(primaryFailure, target, source); }
            watch.Stop();
            result.Elapsed = watch.Elapsed;
            Notify(progress, validationOnly ? "Validation complete" : "Comparison complete", result.RowsA + result.RowsB);
            return result;
        }

        private static void ValidateOptions(ExcelAuditOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.FileA) || string.IsNullOrWhiteSpace(options.FileB))
                throw new ExcelAuditException("Select both input workbooks.");
            if (string.IsNullOrWhiteSpace(options.SheetA) || string.IsNullOrWhiteSpace(options.SheetB))
                throw new ExcelAuditException("Select a worksheet from each workbook.");
            if (options.HeaderRowA < 1 || options.HeaderRowA > 1048576 || options.HeaderRowB < 1 || options.HeaderRowB > 1048576)
                throw new ExcelAuditException("Each header row must be between 1 and 1,048,576.");
            if (options.KeysA.Length != options.KeysB.Length)
                throw new ExcelAuditException("Supply the same number of corresponding key columns for both files, or no keys for either.");
            if (options.KeysA.Length > 16384)
                throw new ExcelAuditException("The key contains more columns than an Excel worksheet supports.");
            if (options.MaxDetails < 0 || options.MaxDetails > 1000000)
                throw new ExcelAuditException("The detail limit must be between 0 and 1,000,000 per report section.");
            if (options.MaxDetailBytes <= 0 || options.MaxPartitionBytes <= 0)
                throw new ExcelAuditException("The detail and partition memory limits must be positive.");
            if (string.IsNullOrWhiteSpace(options.TempDirectory) || !Directory.Exists(options.TempDirectory))
                throw new ExcelAuditException("Select an existing temporary folder with space for the audit working files.");
            if (!options.CompareValues && !options.CompareHeaders && !options.CompareFormatting && !options.CompareLayout)
                throw new ExcelAuditException("Choose at least one comparison check.");
        }

        private static Dictionary<string, int> HeaderIndex(ExcelAuditSheetInfo info, string label)
        {
            if (info.Headers.Count == 0)
                throw new ExcelAuditException(label + " has no headers in the selected header row.");
            var names = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<int, string> header in info.Headers)
            {
                if (string.IsNullOrWhiteSpace(header.Value))
                    throw new ExcelAuditException(label + " has a blank header at " + Address(header.Key, info.HeaderRow)
                        + ". Every compared data column needs an unambiguous header.");
                if (names.ContainsKey(header.Value))
                    throw new ExcelAuditException(label + " has duplicate headers ignoring case: '" + header.Value
                        + "'. Rename the ambiguous columns before comparing.");
                names.Add(header.Value, header.Key);
            }
            return names;
        }

        private static int[] KeyColumns(string[] keys, Dictionary<string, int> headers, string label)
        {
            var found = new List<int>();
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string key in keys)
            {
                int column;
                if (string.IsNullOrWhiteSpace(key) || !headers.TryGetValue(key, out column))
                    throw new ExcelAuditException(label + " key column '" + key + "' is not present in the selected headers.");
                if (!names.Add(key)) throw new ExcelAuditException(label + " selects the key column '" + key + "' more than once.");
                found.Add(column);
            }
            return found.ToArray();
        }

        private static List<ColumnPair> PairColumns(ExcelAuditSheetInfo a, ExcelAuditSheetInfo b, int[] keysA, int[] keysB)
        {
            var pairs = new List<ColumnPair>();
            var usedA = new HashSet<int>();
            var usedB = new HashSet<int>();
            for (int i = 0; i < keysA.Length; i++)
            {
                pairs.Add(new ColumnPair(keysA[i], keysB[i]));
                usedA.Add(keysA[i]);
                usedB.Add(keysB[i]);
            }
            Dictionary<string, int> remainingB = b.Headers.Where(item => !usedB.Contains(item.Key))
                .ToDictionary(item => item.Value, item => item.Key, StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<int, string> header in a.Headers)
            {
                int other;
                if (usedA.Contains(header.Key) || !remainingB.TryGetValue(header.Value, out other)) continue;
                pairs.Add(new ColumnPair(header.Key, other));
                usedA.Add(header.Key);
                usedB.Add(other);
            }
            return pairs.OrderBy(pair => pair.A).ToList();
        }

        private static void CompareMetadata(ExcelAuditResult result, List<ColumnPair> pairs, CancellationToken token)
        {
            ExcelAuditSheetInfo a = result.Source, b = result.Target;
            var matchedA = new HashSet<int>(pairs.Select(pair => pair.A));
            var matchedB = new HashSet<int>(pairs.Select(pair => pair.B));
            if (result.Options.CompareHeaders && a.Headers.Count != b.Headers.Count)
                result.Add(new ExcelAuditFinding
                {
                    Category = ExcelAuditCategory.Header, Difference = "Header count",
                    ValueA = Invariant(a.Headers.Count), ValueB = Invariant(b.Headers.Count)
                });
            foreach (KeyValuePair<int, string> column in a.Headers.Where(item => !matchedA.Contains(item.Key)))
                if (result.Options.CompareHeaders)
                    result.Add(new ExcelAuditFinding
                    {
                        Category = ExcelAuditCategory.Header, Difference = "Column only in File A",
                        ColumnA = column.Value, CellA = Address(column.Key, a.HeaderRow), ValueA = column.Value,
                        Details = "No case-insensitive counterpart or explicit key mapping exists in File B."
                    });
            foreach (KeyValuePair<int, string> column in b.Headers.Where(item => !matchedB.Contains(item.Key)))
                if (result.Options.CompareHeaders)
                    result.Add(new ExcelAuditFinding
                    {
                        Category = ExcelAuditCategory.Header, Difference = "Column only in File B",
                        ColumnB = column.Value, CellB = Address(column.Key, b.HeaderRow), ValueB = column.Value,
                        Details = "No case-insensitive counterpart or explicit key mapping exists in File A."
                    });
            if ((matchedA.Count != a.Headers.Count || matchedB.Count != b.Headers.Count)
                && (result.Options.CompareValues || result.Options.CompareFormatting || result.Options.CompareLayout))
                result.Warn("Columns without a counterpart were not value/style/layout compared. Enable header checks to identify the unmatched column names.");
            if (result.Options.CompareFormatting || result.Options.CompareLayout)
            {
                if (a.Columns.Keys.Any(column => !a.Headers.ContainsKey(column)))
                    result.Warn("File A has column formatting/layout outside the named table headers; those extra columns were not compared.");
                if (b.Columns.Keys.Any(column => !b.Headers.ContainsKey(column)))
                    result.Warn("File B has column formatting/layout outside the named table headers; those extra columns were not compared.");
            }

            foreach (ColumnPair pair in pairs)
            {
                token.ThrowIfCancellationRequested();
                string nameA = a.Headers[pair.A], nameB = b.Headers[pair.B];
                if (result.Options.CompareHeaders)
                {
                    if (!string.Equals(nameA, nameB, StringComparison.Ordinal))
                        result.Add(new ExcelAuditFinding
                        {
                            Category = ExcelAuditCategory.Header,
                            Difference = string.Equals(nameA, nameB, StringComparison.OrdinalIgnoreCase) ? "Header case" : "Header name",
                            ColumnA = nameA, ColumnB = nameB, CellA = Address(pair.A, a.HeaderRow), CellB = Address(pair.B, b.HeaderRow),
                            ValueA = nameA, ValueB = nameB,
                            Details = "Column pairing is case-insensitive; explicitly mapped key names may differ."
                        });
                    if (pair.A != pair.B)
                        result.Add(new ExcelAuditFinding
                        {
                            Category = ExcelAuditCategory.Header, Difference = "Column order",
                            ColumnA = nameA, ColumnB = nameB, CellA = Address(pair.A, a.HeaderRow), CellB = Address(pair.B, b.HeaderRow),
                            ValueA = Invariant(pair.A), ValueB = Invariant(pair.B), Details = "One-based worksheet column positions."
                        });
                    ExcelAuditCell ha = Cell(a.HeaderCells, pair.A, a.HeaderRow), hb = Cell(b.HeaderCells, pair.B, b.HeaderRow);
                    if (!string.Equals(ha.Kind, hb.Kind, StringComparison.Ordinal))
                        result.Add(new ExcelAuditFinding
                        {
                            Category = ExcelAuditCategory.Header, Difference = "Header data type",
                            ColumnA = nameA, ColumnB = nameB, CellA = ha.Address, CellB = hb.Address, ValueA = ha.Kind, ValueB = hb.Kind
                        });
                }
                if (result.Options.CompareFormatting)
                    CompareProperties(Cell(a.HeaderCells, pair.A, a.HeaderRow).Format,
                        Cell(b.HeaderCells, pair.B, b.HeaderRow).Format, ExcelAuditCategory.Formatting,
                        "Header ", string.Empty, nameA, nameB, Address(pair.A, a.HeaderRow), Address(pair.B, b.HeaderRow), result);
                if (result.Options.CompareLayout)
                {
                    Dictionary<string, string> ca, cb;
                    a.Columns.TryGetValue(pair.A, out ca);
                    b.Columns.TryGetValue(pair.B, out cb);
                    CompareProperties(ca, cb, ExcelAuditCategory.Structure, "Column ", string.Empty,
                        nameA, nameB, Address(pair.A, a.HeaderRow), Address(pair.B, b.HeaderRow), result);
                }
            }
            if (result.Options.CompareLayout)
            {
                CompareProperties(a.Structure, b.Structure, ExcelAuditCategory.Structure, "Worksheet ", string.Empty,
                    string.Empty, string.Empty, string.Empty, string.Empty, result);
                if (!string.Equals(a.Name, b.Name, StringComparison.Ordinal))
                    result.Add(new ExcelAuditFinding { Category = ExcelAuditCategory.Structure,
                        Difference = "Selected worksheet name", ValueA = a.Name, ValueB = b.Name });
                if (!a.SheetNames.SequenceEqual(b.SheetNames, StringComparer.Ordinal))
                    result.Add(new ExcelAuditFinding { Category = ExcelAuditCategory.Structure,
                        Difference = "Workbook worksheet names/order", ValueA = string.Join(" | ", a.SheetNames),
                        ValueB = string.Join(" | ", b.SheetNames), Details = "Only the selected worksheet pair is content-compared." });
            }
        }

        private static void ComparePositional(ExcelAuditReader source, ExcelAuditReader target, List<ColumnPair> pairs,
            ExcelAuditResult result, CancellationToken token, Action<ExcelAuditProgress> progress, bool validationOnly)
        {
            Notify(progress, validationOnly ? "Validating rows" : "Comparing row positions", 0);
            using (IEnumerator<ExcelAuditRow> a = source.ReadRows(token).GetEnumerator())
            using (IEnumerator<ExcelAuditRow> b = target.ReadRows(token).GetEnumerator())
            {
                bool hasA = a.MoveNext(), hasB = b.MoveNext();
                long visited = 0;
                while (hasA || hasB)
                {
                    token.ThrowIfCancellationRequested();
                    long indexA = hasA ? (long)a.Current.Number - source.Info.HeaderRow : long.MaxValue;
                    long indexB = hasB ? (long)b.Current.Number - target.Info.HeaderRow : long.MaxValue;
                    if (indexA == indexB)
                    {
                        EnsureRowColumns(a.Current, source.Info, "File A", result);
                        EnsureRowColumns(b.Current, target.Info, "File B", result);
                        CountRow(result, a.Current, true);
                        CountRow(result, b.Current, false);
                        if (a.Current.HasValues && b.Current.HasValues) result.MatchedRows++;
                        else if (a.Current.HasValues) result.OnlyA++;
                        else if (b.Current.HasValues) result.OnlyB++;
                        if (!validationOnly) CompareRows(a.Current, b.Current, pairs, string.Empty, result, token);
                        hasA = a.MoveNext();
                        hasB = b.MoveNext();
                    }
                    else if (indexA < indexB)
                    {
                        EnsureRowColumns(a.Current, source.Info, "File A", result);
                        CountRow(result, a.Current, true);
                        if (a.Current.HasValues) result.OnlyA++;
                        if (!hasB && a.Current.HasValues)
                        {
                            if (!validationOnly) MissingRecord(a.Current, true, string.Empty, result);
                        }
                        else if (!validationOnly)
                            CompareRows(a.Current, target.BlankRow(CheckedRow(indexA, target.Info.HeaderRow)), pairs, string.Empty, result, token);
                        hasA = a.MoveNext();
                    }
                    else
                    {
                        EnsureRowColumns(b.Current, target.Info, "File B", result);
                        CountRow(result, b.Current, false);
                        if (b.Current.HasValues) result.OnlyB++;
                        if (!hasA && b.Current.HasValues)
                        {
                            if (!validationOnly) MissingRecord(b.Current, false, string.Empty, result);
                        }
                        else if (!validationOnly)
                            CompareRows(source.BlankRow(CheckedRow(indexB, source.Info.HeaderRow)), b.Current, pairs, string.Empty, result, token);
                        hasB = b.MoveNext();
                    }
                    if ((++visited & 255) == 0) Notify(progress, "Scanning row positions", result.RowsA + result.RowsB);
                }
            }
        }

        private static int CheckedRow(long index, int header)
        {
            long row = index + header;
            if (row > 1048576) throw new ExcelAuditException("The selected header rows align a logical row outside Excel's row limit.");
            return (int)row;
        }

        private static void CountRow(ExcelAuditResult result, ExcelAuditRow row, bool source)
        {
            if (!row.HasValues) return;
            if (source) result.RowsA++; else result.RowsB++;
        }

        private static void EnsureRowColumns(ExcelAuditRow row, ExcelAuditSheetInfo info, string label, ExcelAuditResult result)
        {
            foreach (KeyValuePair<int, ExcelAuditCell> item in row.Cells)
            {
                if (info.Headers.ContainsKey(item.Key)) continue;
                if (!item.Value.IsBlank)
                    throw new ExcelAuditException(label + " contains data at " + item.Value.Address
                        + " outside its named header columns. Correct the selected header row before comparing.");
                if (result.Options.CompareFormatting && item.Value.Format.Count != 0)
                    result.Warn(label + " has blank cells outside the named table headers; their formatting was not compared.");
            }
        }

        private static void CompareKeyed(ExcelAuditReader source, ExcelAuditReader target, int[] keysA, int[] keysB,
            List<ColumnPair> pairs, ExcelAuditResult result, CancellationToken token, Action<ExcelAuditProgress> progress,
            bool validationOnly)
        {
            var spool = new PartitionStore(result.Options.TempDirectory, result.Options.MaxPartitionBytes);
            Exception primaryFailure = null;
            try
            {
                WritePartitions(source, keysA, true, spool, result, token, progress);
                WritePartitions(target, keysB, false, spool, result, token, progress);
                spool.FinishWriting();
                for (int partition = 0; partition < PartitionCount; partition++)
                {
                    token.ThrowIfCancellationRequested();
                    Notify(progress, "Matching key partition " + (partition + 1) + " of " + PartitionCount, result.RowsA + result.RowsB);
                    var targetRows = new Dictionary<string, StoredRow>(StringComparer.Ordinal);
                    long memory = 0;
                    foreach (StoredRow stored in spool.Read(false, partition, token))
                    {
                        AddMemory(ref memory, Estimate(stored), result.Options.MaxPartitionBytes);
                        StoredRow duplicate;
                        if (targetRows.TryGetValue(stored.Key, out duplicate)) DuplicateKey("File B", duplicate.Row.Number, stored.Row.Number, stored.DisplayKey);
                        targetRows.Add(stored.Key, stored);
                    }
                    var seenSource = new Dictionary<string, int>(StringComparer.Ordinal);
                    foreach (StoredRow stored in spool.Read(true, partition, token))
                    {
                        token.ThrowIfCancellationRequested();
                        AddMemory(ref memory, 96L + stored.Key.Length * 2L, result.Options.MaxPartitionBytes);
                        int prior;
                        if (seenSource.TryGetValue(stored.Key, out prior)) DuplicateKey("File A", prior, stored.Row.Number, stored.DisplayKey);
                        seenSource.Add(stored.Key, stored.Row.Number);
                        StoredRow matching;
                        if (targetRows.TryGetValue(stored.Key, out matching))
                        {
                            if (stored.Row.HasValues) result.MatchedRows++;
                            if (!validationOnly) CompareRows(stored.Row, matching.Row, pairs, stored.DisplayKey, result, token);
                            targetRows.Remove(stored.Key);
                        }
                        else if (stored.Row.HasValues)
                        {
                            result.OnlyA++;
                            if (!validationOnly) MissingRecord(stored.Row, true, stored.DisplayKey, result);
                        }
                        else if (!validationOnly && (result.Options.CompareFormatting || result.Options.CompareLayout))
                            result.Warn("File A has blank non-record rows without corresponding blank rows in File B; their formatting/layout was not compared in key mode.");
                    }
                    foreach (StoredRow remaining in targetRows.Values.OrderBy(item => item.Row.Number))
                    {
                        token.ThrowIfCancellationRequested();
                        if (remaining.Row.HasValues)
                        {
                            result.OnlyB++;
                            if (!validationOnly) MissingRecord(remaining.Row, false, remaining.DisplayKey, result);
                        }
                        else if (!validationOnly && (result.Options.CompareFormatting || result.Options.CompareLayout))
                            result.Warn("File B has blank non-record rows without corresponding blank rows in File A; their formatting/layout was not compared in key mode.");
                    }
                }
            }
            catch (Exception ex) { primaryFailure = ex; throw; }
            finally { DisposeResources(primaryFailure, spool); }
        }

        private static void WritePartitions(ExcelAuditReader reader, int[] keys, bool source, PartitionStore spool,
            ExcelAuditResult result, CancellationToken token, Action<ExcelAuditProgress> progress)
        {
            string label = source ? "File A" : "File B";
            Notify(progress, "Validating and indexing " + label, result.RowsA + result.RowsB);
            long visited = 0;
            foreach (ExcelAuditRow row in reader.ReadRows(token))
            {
                token.ThrowIfCancellationRequested();
                EnsureRowColumns(row, reader.Info, label, result);
                CountRow(result, row, source);
                string display;
                string key = row.HasValues ? EncodeKey(row, keys, reader.Info, label, out display)
                    : EmptyRowKey(row, reader.Info, out display);
                spool.Write(source, new StoredRow { Key = key, DisplayKey = display, Row = row });
                if ((++visited & 255) == 0) Notify(progress, "Validating and indexing " + label, result.RowsA + result.RowsB);
            }
        }

        private static string EmptyRowKey(ExcelAuditRow row, ExcelAuditSheetInfo info, out string display)
        {
            // Non-record blank rows have no primary key. Pair them only with another blank row at
            // the same logical position; an unmatched blank row must not impersonate a keyed record.
            display = string.Empty;
            return "E" + Invariant(row.Number - info.HeaderRow);
        }

        private static string EncodeKey(ExcelAuditRow row, int[] columns, ExcelAuditSheetInfo info,
            string label, out string display)
        {
            var key = new StringBuilder("K");
            var shown = new List<string>();
            foreach (int column in columns)
            {
                ExcelAuditCell cell = Cell(row.Cells, column, row.Number);
                if (cell.IsBlank || string.IsNullOrWhiteSpace(cell.Value))
                    throw new ExcelAuditException(label + " has a blank primary-key component at " + cell.Address
                        + " ('" + info.Headers[column] + "'). No row-by-row fallback was performed.");
                if (cell.Kind == "Formula" || cell.Kind == "Error")
                    throw new ExcelAuditException(label + " primary-key cell " + cell.Address
                        + " contains a formula or Excel error. Use stored, nonblank key values.");
                AppendPart(key, cell.Kind);
                AppendPart(key, cell.Canonical ?? cell.Value);
                shown.Add(info.Headers[column] + "=" + cell.Value + " [" + cell.Kind + "]");
            }
            display = string.Join(" | ", shown);
            return key.ToString();
        }

        private static void AppendPart(StringBuilder builder, string value)
        {
            value = value ?? string.Empty;
            builder.Append(value.Length.ToString(CultureInfo.InvariantCulture)).Append(':').Append(value);
        }

        private static void DuplicateKey(string label, int firstRow, int nextRow, string display)
        {
            throw new ExcelAuditException(label + " contains a duplicate complete primary key at Excel rows "
                + firstRow + " and " + nextRow + ": " + Abbreviate(display, 500) + ". No comparison report was published.");
        }

        private static void CompareRows(ExcelAuditRow a, ExcelAuditRow b, List<ColumnPair> pairs, string key,
            ExcelAuditResult result, CancellationToken token)
        {
            foreach (ColumnPair pair in pairs)
            {
                token.ThrowIfCancellationRequested();
                ExcelAuditCell ca = Cell(a.Cells, pair.A, a.Number), cb = Cell(b.Cells, pair.B, b.Number);
                string nameA = result.Source.Headers[pair.A], nameB = result.Target.Headers[pair.B];
                if (result.Options.CompareValues && (!string.Equals(ca.Kind, cb.Kind, StringComparison.Ordinal)
                    || !string.Equals(ca.Canonical, cb.Canonical, StringComparison.Ordinal)))
                    result.Add(new ExcelAuditFinding
                    {
                        Category = ExcelAuditCategory.Data,
                        Difference = ca.Kind != cb.Kind ? "Data type" : ca.Kind == "Formula" ? "Formula" : "Value",
                        Key = key, ColumnA = nameA, ColumnB = nameB, CellA = ca.Address, CellB = cb.Address,
                        ValueA = ca.Value, ValueB = cb.Value,
                        Details = ca.Kind + " → " + cb.Kind + "; text case and whitespace are significant."
                    });
                if (result.Options.CompareFormatting)
                    CompareProperties(ca.Format, cb.Format, ExcelAuditCategory.Formatting, string.Empty,
                        key, nameA, nameB, ca.Address, cb.Address, result);
            }
            if (result.Options.CompareLayout)
                CompareProperties(a.Layout, b.Layout, ExcelAuditCategory.Structure, "Row ", key,
                    string.Empty, string.Empty, Invariant(a.Number), Invariant(b.Number), result);
        }

        private static void CompareProperties(Dictionary<string, string> a, Dictionary<string, string> b,
            ExcelAuditCategory category, string prefix, string key, string columnA, string columnB,
            string cellA, string cellB, ExcelAuditResult result)
        {
            IEnumerable<string> names = (a == null ? Enumerable.Empty<string>() : a.Keys)
                .Union(b == null ? Enumerable.Empty<string>() : b.Keys, StringComparer.Ordinal)
                .OrderBy(name => name, StringComparer.Ordinal);
            foreach (string name in names)
            {
                string va = null, vb = null;
                if (a != null) a.TryGetValue(name, out va);
                if (b != null) b.TryGetValue(name, out vb);
                if (string.Equals(va, vb, StringComparison.Ordinal)) continue;
                result.Add(new ExcelAuditFinding
                {
                    Category = category, Difference = prefix + name, Key = key,
                    ColumnA = columnA, ColumnB = columnB, CellA = cellA, CellB = cellB,
                    ValueA = va ?? "(default / not set)", ValueB = vb ?? "(default / not set)"
                });
            }
        }

        private static void MissingRecord(ExcelAuditRow row, bool source, string key, ExcelAuditResult result)
        {
            if (!result.Options.CompareValues) return;
            ExcelAuditSheetInfo info = source ? result.Source : result.Target;
            string range = Address(info.Headers.Keys.Min(), row.Number) + ":" + Address(info.Headers.Keys.Max(), row.Number);
            string description = Abbreviate(string.Join(" | ", info.Headers.Select(column =>
                column.Value + "=" + Cell(row.Cells, column.Key, row.Number).Value)), 3000);
            result.Add(new ExcelAuditFinding
            {
                Category = ExcelAuditCategory.Data, Difference = source ? "Record only in File A" : "Record only in File B",
                Key = key, CellA = source ? range : string.Empty, CellB = source ? string.Empty : range,
                ValueA = source ? description : "(not present)", ValueB = source ? "(not present)" : description,
                Details = "Record summary, limited to 3,000 characters; original cell range is retained."
            });
        }

        private static ExcelAuditCell Cell(SortedDictionary<int, ExcelAuditCell> cells, int column, int row)
        {
            ExcelAuditCell cell;
            return cells.TryGetValue(column, out cell) ? cell : new ExcelAuditCell { Column = column, Address = Address(column, row) };
        }

        private static string Address(int column, int row)
        {
            string name = string.Empty;
            while (column > 0) { column--; name = (char)('A' + column % 26) + name; column /= 26; }
            return name + Invariant(row);
        }

        private static string Invariant(long value) { return value.ToString(CultureInfo.InvariantCulture); }
        private static string Abbreviate(string value, int max) { return value.Length <= max ? value : value.Substring(0, max - 14) + " … [shortened]"; }
        private static void Notify(Action<ExcelAuditProgress> progress, string phase, long rows) { progress?.Invoke(new ExcelAuditProgress(phase, rows)); }

        private static void DisposeResources(Exception primaryFailure, params IDisposable[] resources)
        {
            Exception cleanupFailure = null;
            foreach (IDisposable resource in resources)
            {
                if (resource == null) continue;
                try { resource.Dispose(); }
                catch (Exception ex) { if (cleanupFailure == null) cleanupFailure = ex; }
            }
            if (cleanupFailure == null) return;
            // Cancellation and the original validation/read failure remain the primary outcome.
            if (primaryFailure != null)
            {
                primaryFailure.Data["ExcelAuditCleanupFailure"] = cleanupFailure.ToString();
                return;
            }
            throw new ExcelAuditException("The audit could not finish cleaning its temporary resources. "
                + cleanupFailure.Message, cleanupFailure);
        }

        private static long Estimate(StoredRow item)
        {
            long size = 256L + 2L * (item.Key.Length + item.DisplayKey.Length);
            foreach (ExcelAuditCell cell in item.Row.Cells.Values)
            {
                size += 192L + 2L * ((cell.Address ?? "").Length + (cell.Kind ?? "").Length
                    + (cell.Value ?? "").Length + (cell.Canonical ?? "").Length);
                foreach (KeyValuePair<string, string> pair in cell.Format)
                    size += 96L + 2L * (pair.Key.Length + (pair.Value ?? "").Length);
            }
            foreach (KeyValuePair<string, string> pair in item.Row.Layout)
                size += 96L + 2L * (pair.Key.Length + (pair.Value ?? "").Length);
            return size;
        }

        private static void AddMemory(ref long consumed, long next, long maximum)
        {
            if (next > maximum - consumed)
                throw new ExcelAuditException("A key-matching partition exceeds the configured memory budget. Split the selected worksheet into smaller inputs "
                    + "or raise MaxPartitionBytes on a machine with sufficient memory. No partial report was published.");
            consumed += next;
        }

        private sealed class ColumnPair
        {
            internal ColumnPair(int a, int b) { A = a; B = b; }
            internal int A { get; }
            internal int B { get; }
        }

        private sealed class StoredRow
        {
            internal string Key;
            internal string DisplayKey;
            internal ExcelAuditRow Row;
        }

        private sealed class PartitionStore : IDisposable
        {
            private readonly string directory;
            private readonly long maximum;
            private readonly Dictionary<string, PartitionWriter> writers = new Dictionary<string, PartitionWriter>(StringComparer.Ordinal);

            internal PartitionStore(string parent, long maximumBytes)
            {
                maximum = maximumBytes;
                directory = Path.Combine(Path.GetFullPath(parent), "spi-excel-audit-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(directory);
            }

            internal void Write(bool source, StoredRow value)
            {
                int partition = Bucket(value.Key);
                string path = PartitionPath(source, partition);
                PartitionWriter handle;
                if (!writers.TryGetValue(path, out handle))
                {
                    var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 8192);
                    try { handle = new PartitionWriter { Stream = stream, Writer = new BinaryWriter(stream, Encoding.UTF8) }; }
                    catch { stream.Dispose(); throw; }
                    writers.Add(path, handle);
                }
                BinaryWriter writer = handle.Writer;
                writer.Write(value.Key);
                writer.Write(value.DisplayKey);
                writer.Write(value.Row.Number);
                WriteProperties(writer, value.Row.Layout);
                writer.Write(value.Row.Cells.Count);
                foreach (KeyValuePair<int, ExcelAuditCell> pair in value.Row.Cells)
                {
                    ExcelAuditCell cell = pair.Value;
                    writer.Write(pair.Key);
                    writer.Write(cell.Address ?? string.Empty);
                    writer.Write(cell.Kind ?? "Blank");
                    writer.Write(cell.Value ?? string.Empty);
                    writer.Write(cell.Canonical ?? string.Empty);
                    WriteProperties(writer, cell.Format);
                }
                if (handle.Stream.Position > maximum)
                    throw new ExcelAuditException("A temporary key partition exceeds MaxPartitionBytes. Split very wide or large worksheets "
                        + "into smaller inputs or increase the documented resource budget. No partial report was published.");
            }

            internal void FinishWriting()
            {
                PartitionWriter[] handles = writers.Values.ToArray();
                writers.Clear();
                Exception failure = null;
                foreach (PartitionWriter handle in handles)
                {
                    try { handle.Writer.Dispose(); }
                    catch (Exception ex) { if (failure == null) failure = ex; }
                    // Dispose the FileStream directly too: a BinaryWriter flush failure must not
                    // strand later handles or prevent deletion of this run's files on Windows.
                    try { handle.Stream.Dispose(); }
                    catch (Exception ex) { if (failure == null) failure = ex; }
                }
                if (failure != null)
                    throw new ExcelAuditException("Could not flush/close the audit's temporary key files. " + failure.Message, failure);
            }

            internal IEnumerable<StoredRow> Read(bool source, int partition, CancellationToken token)
            {
                string path = PartitionPath(source, partition);
                if (!File.Exists(path)) yield break;
                using (var reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 8192), Encoding.UTF8))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        token.ThrowIfCancellationRequested();
                        var stored = new StoredRow { Key = reader.ReadString(), DisplayKey = reader.ReadString() };
                        var row = new ExcelAuditRow { Number = reader.ReadInt32(), Layout = ReadProperties(reader) };
                        int count = reader.ReadInt32();
                        if (count < 0 || count > 16384) throw new ExcelAuditException("The audit temporary row index is invalid.");
                        for (int i = 0; i < count; i++)
                        {
                            int column = reader.ReadInt32();
                            row.Cells.Add(column, new ExcelAuditCell
                            {
                                Column = column, Address = reader.ReadString(), Kind = reader.ReadString(),
                                Value = reader.ReadString(), Canonical = reader.ReadString(), Format = ReadProperties(reader)
                            });
                        }
                        stored.Row = row;
                        yield return stored;
                    }
                }
            }

            private string PartitionPath(bool source, int partition)
            {
                return Path.Combine(directory, (source ? "A-" : "B-") + partition.ToString("D2", CultureInfo.InvariantCulture) + ".bin");
            }

            private static int Bucket(string value)
            {
                unchecked
                {
                    uint hash = 2166136261;
                    foreach (char character in value) { hash = (hash ^ (byte)character) * 16777619; hash = (hash ^ (byte)(character >> 8)) * 16777619; }
                    return (int)(hash % PartitionCount);
                }
            }

            private static void WriteProperties(BinaryWriter writer, Dictionary<string, string> values)
            {
                writer.Write(values.Count);
                foreach (KeyValuePair<string, string> pair in values)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value ?? string.Empty);
                }
            }

            private static Dictionary<string, string> ReadProperties(BinaryReader reader)
            {
                int count = reader.ReadInt32();
                if (count < 0 || count > 4096) throw new ExcelAuditException("The audit temporary style index is invalid.");
                var result = new Dictionary<string, string>(StringComparer.Ordinal);
                for (int i = 0; i < count; i++) result.Add(reader.ReadString(), reader.ReadString());
                return result;
            }

            public void Dispose()
            {
                Exception failure = null;
                try { FinishWriting(); }
                catch (Exception ex) { failure = ex; }
                // Only the GUID-named directory created by this run is eligible for removal.
                try { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
                catch (Exception ex) { if (failure == null) failure = ex; }
                if (failure != null)
                    throw new ExcelAuditException("Could not completely clean the audit working folder '" + directory + "'. "
                        + failure.Message, failure);
            }

            private sealed class PartitionWriter
            {
                internal FileStream Stream;
                internal BinaryWriter Writer;
            }
        }
    }
}
