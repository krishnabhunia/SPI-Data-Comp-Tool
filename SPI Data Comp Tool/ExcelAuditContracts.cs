using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPI_Data_Comp_Tool
{
    internal enum ExcelAuditCategory { Data, Header, Formatting, Structure, Scope }

    /// <summary>A captured run configuration, independent of mutable form controls.</summary>
    internal sealed class ExcelAuditOptions
    {
        internal string FileA { get; set; }
        internal string FileB { get; set; }
        internal string SheetA { get; set; }
        internal string SheetB { get; set; }
        internal int HeaderRowA { get; set; } = 1;
        internal int HeaderRowB { get; set; } = 1;
        internal string[] KeysA { get; set; } = new string[0];
        internal string[] KeysB { get; set; } = new string[0];
        internal bool CompareValues { get; set; } = true;
        internal bool CompareHeaders { get; set; } = true;
        internal bool CompareFormatting { get; set; } = true;
        internal bool CompareLayout { get; set; } = true;
        internal int MaxDetails { get; set; } = 100000;
        internal long MaxDetailBytes { get; set; } = 64L * 1024 * 1024;
        internal long MaxPartitionBytes { get; set; } = 128L * 1024 * 1024;
        internal string TempDirectory { get; set; } = Path.GetTempPath();
        internal bool UseKeys => KeysA != null && KeysA.Length > 0;

        internal ExcelAuditOptions Snapshot()
        {
            var copy = (ExcelAuditOptions)MemberwiseClone();
            copy.KeysA = KeysA == null ? new string[0] : (string[])KeysA.Clone();
            copy.KeysB = KeysB == null ? new string[0] : (string[])KeysB.Clone();
            return copy;
        }
    }

    internal sealed class ExcelAuditProgress
    {
        internal ExcelAuditProgress(string phase, long rows)
        {
            Phase = phase;
            Rows = rows;
        }
        internal string Phase { get; }
        internal long Rows { get; }
    }

    internal sealed class ExcelAuditException : Exception
    {
        internal ExcelAuditException(string message) : base(message) { }
        internal ExcelAuditException(string message, Exception inner) : base(message, inner) { }
    }

    internal sealed class ExcelAuditCell
    {
        internal int Column { get; set; }
        internal string Address { get; set; } = string.Empty;
        // Blank, Text, Number, Boolean, Date, Error or Formula. Canonical is never a lossy hash.
        internal string Kind { get; set; } = "Blank";
        internal string Value { get; set; } = string.Empty;
        internal string Canonical { get; set; } = string.Empty;
        internal Dictionary<string, string> Format { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
        internal bool IsBlank => Kind == "Blank";
    }

    internal sealed class ExcelAuditRow
    {
        internal int Number { get; set; }
        internal SortedDictionary<int, ExcelAuditCell> Cells { get; set; } = new SortedDictionary<int, ExcelAuditCell>();
        internal Dictionary<string, string> Layout { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
        internal bool HasValues => Cells.Values.Any(cell => !cell.IsBlank);
    }

    internal sealed class ExcelAuditSheetInfo
    {
        internal string FilePath { get; set; }
        internal string Name { get; set; }
        internal int HeaderRow { get; set; }
        internal string[] SheetNames { get; set; } = new string[0];
        internal SortedDictionary<int, string> Headers { get; set; } = new SortedDictionary<int, string>();
        internal SortedDictionary<int, ExcelAuditCell> HeaderCells { get; set; } = new SortedDictionary<int, ExcelAuditCell>();
        internal Dictionary<string, string> Structure { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
        internal Dictionary<int, Dictionary<string, string>> Columns { get; set; } = new Dictionary<int, Dictionary<string, string>>();
        internal List<string> Warnings { get; } = new List<string>();
    }

    internal sealed class ExcelAuditFinding
    {
        internal ExcelAuditCategory Category { get; set; }
        internal string Difference { get; set; } = string.Empty;
        internal string Key { get; set; } = string.Empty;
        internal string ColumnA { get; set; } = string.Empty;
        internal string ColumnB { get; set; } = string.Empty;
        internal string CellA { get; set; } = string.Empty;
        internal string CellB { get; set; } = string.Empty;
        internal string ValueA { get; set; } = string.Empty;
        internal string ValueB { get; set; } = string.Empty;
        internal string Details { get; set; } = string.Empty;

        internal long EstimatedBytes => 128L + 2L * new[]
        { Difference, Key, ColumnA, ColumnB, CellA, CellB, ValueA, ValueB, Details }
            .Sum(value => (long)(value ?? string.Empty).Length);
    }

    /// <summary>Separate bounded detail sections; totals include findings not retained for display/export.</summary>
    internal sealed class ExcelAuditResult
    {
        private long dataBytes;
        private long otherBytes;
        private readonly HashSet<string> warnings = new HashSet<string>(StringComparer.Ordinal);

        internal ExcelAuditResult(ExcelAuditOptions options)
        {
            Options = options.Snapshot();
        }

        internal ExcelAuditOptions Options { get; }
        internal ExcelAuditSheetInfo Source { get; set; }
        internal ExcelAuditSheetInfo Target { get; set; }
        internal DateTime StartedUtc { get; set; } = DateTime.UtcNow;
        internal TimeSpan Elapsed { get; set; }
        internal long RowsA { get; set; }
        internal long RowsB { get; set; }
        internal long MatchedRows { get; set; }
        internal long OnlyA { get; set; }
        internal long OnlyB { get; set; }
        internal long DataCount { get; private set; }
        internal long HeaderCount { get; private set; }
        internal long FormattingCount { get; private set; }
        internal long StructureCount { get; private set; }
        internal long ScopeCount { get; private set; }
        internal bool Complete { get; private set; } = true;
        internal bool IsValidationOnly { get; set; }
        internal List<ExcelAuditFinding> DataDetails { get; } = new List<ExcelAuditFinding>();
        internal List<ExcelAuditFinding> OtherDetails { get; } = new List<ExcelAuditFinding>();
        internal long OtherCount => HeaderCount + FormattingCount + StructureCount + ScopeCount;
        internal long TotalCount => DataCount + OtherCount;
        internal long OmittedData => DataCount - DataDetails.Count;
        internal long OmittedOther => OtherCount - OtherDetails.Count;
        internal bool Matches => !IsValidationOnly && Complete && DataCount + HeaderCount + FormattingCount + StructureCount == 0;

        internal void Add(ExcelAuditFinding finding)
        {
            switch (finding.Category)
            {
                case ExcelAuditCategory.Data: DataCount++; break;
                case ExcelAuditCategory.Header: HeaderCount++; break;
                case ExcelAuditCategory.Formatting: FormattingCount++; break;
                case ExcelAuditCategory.Structure: StructureCount++; break;
                case ExcelAuditCategory.Scope: ScopeCount++; Complete = false; break;
                default: throw new ArgumentOutOfRangeException(nameof(finding));
            }
            bool data = finding.Category == ExcelAuditCategory.Data;
            List<ExcelAuditFinding> section = data ? DataDetails : OtherDetails;
            long bytes = data ? dataBytes : otherBytes;
            if (section.Count >= Options.MaxDetails || finding.EstimatedBytes > Options.MaxDetailBytes - bytes) return;
            section.Add(finding);
            if (data) dataBytes += finding.EstimatedBytes;
            else otherBytes += finding.EstimatedBytes;
        }

        internal void Warn(string message)
        {
            if (!warnings.Add(message)) return;
            Add(new ExcelAuditFinding { Category = ExcelAuditCategory.Scope, Difference = "Not fully checked", Details = message });
        }
    }
}
