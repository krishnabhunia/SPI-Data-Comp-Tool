using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Repeatable matching measurements; elapsed time is reported, not used as a hardware-dependent gate.</summary>
    internal static class ComparisonPerformanceChecks
    {
        internal static int Run(string output)
        {
            var results = new List<object>();
            try
            {
                // Warm up the real matcher before recording the representative cases.
                Measure("warmup", 100, 4, 100);
                results.Add(Measure("unique-10000", 10000, 16, 10000));
                results.Add(Measure("duplicates-50000", 50000, 16, 500));
                results.Add(Measure("wide-10000", 10000, 128, 1000));
                File.WriteAllText(output, JsonConvert.SerializeObject(new
                {
                    schema = 1,
                    scope = "In-memory reference-key matching only; excludes database reads and Excel report creation.",
                    clrVersion = Environment.Version.ToString(),
                    frameworkRelease = RuntimeRequirements.InstalledFrameworkRelease(),
                    operatingSystem = Environment.OSVersion.VersionString,
                    processBits = IntPtr.Size * 8,
                    processorCount = Environment.ProcessorCount,
                    measurements = results,
                }, Formatting.Indented));
                return 0;
            }
            catch (Exception ex)
            {
                File.WriteAllText(output, "FAIL " + Common_Data.RedactConnectionCredentials(ex.ToString()));
                return 1;
            }
        }

        private static object Measure(string name, int rows, int columns, int distinctKeys)
        {
            GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
            long beforeFixtures = GC.GetTotalMemory(false);
            var totalWatch = Stopwatch.StartNew();
            using (var source = new DataTable("Source") { Locale = CultureInfo.InvariantCulture })
            using (var target = new DataTable("Target") { Locale = CultureInfo.InvariantCulture })
            {
                source.Columns.Add("Key", typeof(string));
                source.Columns.Add("Ordinal", typeof(int));
                for (int col = 2; col < columns; col++) source.Columns.Add("Value" + col, typeof(decimal));
                foreach (DataColumn column in source.Columns) target.Columns.Add(column.ColumnName, column.DataType);
                for (int row = 0; row < rows; row++)
                {
                    var values = new object[columns];
                    values[0] = "key'" + (row % distinctKeys).ToString(CultureInfo.InvariantCulture);
                    values[1] = row;
                    for (int col = 2; col < columns; col++) values[col] = row * 10m + col / 100m;
                    source.Rows.Add(values); target.Rows.Add(values);
                }
                long fixtureMilliseconds = totalWatch.ElapsedMilliseconds;
                var watch = Stopwatch.StartNew();
                var matcher = new ComparisonRows(target, new[] { "Key" });
                int matched = 0;
                foreach (DataRow row in source.Rows)
                {
                    DataRow pair = matcher.TakeNext(row);
                    if (pair == null || (int)pair["Ordinal"] != (int)row["Ordinal"])
                        throw new InvalidOperationException("The measurement lost/reused/reordered a target occurrence.");
                    foreach (DataColumn column in source.Columns)
                        if (!Equals(row[column], pair[column.ColumnName]))
                            throw new InvalidOperationException("The measurement paired unequal row data.");
                    matched++;
                }
                using (DataTable remaining = matcher.Remaining())
                    if (matched != rows || remaining.Rows.Count != 0)
                        throw new InvalidOperationException("The measurement produced incorrect matched/unmatched counts.");
                watch.Stop();
                long managedBytes = Math.Max(0, GC.GetTotalMemory(false) - beforeFixtures);
                using (var process = Process.GetCurrentProcess())
                {
                    process.Refresh();
                    return new
                    {
                        name, rowsPerSide = rows, columns, distinctKeys,
                        cellsBothSides = (long)rows * columns * 2,
                        fixtureMilliseconds, matchingAndVerificationMilliseconds = watch.ElapsedMilliseconds,
                        managedBytesOverBaseline = managedBytes,
                        processWorkingSetBytes = process.WorkingSet64,
                        processPeakWorkingSetBytes = process.PeakWorkingSet64,
                        matchedRows = matched, unmatchedRows = 0,
                    };
                }
            }
        }
    }
}
