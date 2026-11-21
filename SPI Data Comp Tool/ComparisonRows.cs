using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Matches repeated reference keys one row at a time, in original target order.
    /// A consumed row cannot match again and unconsumed duplicates remain in Target Rows Only.
    /// </summary>
    internal sealed class ComparisonRows
    {
        private readonly DataTable target;
        private readonly string[] keys;
        private readonly HashSet<DataRow> consumed = new HashSet<DataRow>();
        private readonly Dictionary<DataRow, int> originalOrder;
        private readonly Dictionary<string, Queue<DataRow>> candidates = new Dictionary<string, Queue<DataRow>>(StringComparer.Ordinal);

        internal ComparisonRows(DataTable target, IEnumerable<string> keys)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            this.target = target;
            this.keys = (keys ?? throw new ArgumentNullException(nameof(keys))).ToArray();
            if (this.keys.Length == 0 || this.keys.Any(key => !target.Columns.Contains(key)))
                throw new ArgumentException("Select reference-key columns that exist in both tables.");
            originalOrder = target.Rows.Cast<DataRow>().Select((row, index) => new { row, index })
                .ToDictionary(item => item.row, item => item.index);
        }

        internal DataRow TakeNext(DataRow source)
        {
            // DataTable.Select handles the provider's typed values; a separate set preserves
            // the loaded snapshots and does not require a reserved user-data marker column.
            string filter = KeyFilter(source, keys);
            Queue<DataRow> matches;
            if (!candidates.TryGetValue(filter, out matches))
            {
                matches = new Queue<DataRow>(target.Select(filter).OrderBy(row => originalOrder[row]));
                candidates.Add(filter, matches);
            }
            while (matches.Count > 0)
            {
                DataRow row = matches.Dequeue();
                // Equivalent typed filter literals can share candidates. The global set still
                // prevents reuse while cached queues avoid rescanning a large duplicate group.
                if (consumed.Add(row)) return row;
            }
            return null;
        }

        internal DataTable Remaining()
        {
            DataTable remaining = target.Clone();
            foreach (DataRow row in target.Rows)
                if (!consumed.Contains(row)) remaining.ImportRow(row);
            return remaining;
        }

        internal static string KeyFilter(DataRow source, IEnumerable<string> keys)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            string[] columns = (keys ?? throw new ArgumentNullException(nameof(keys))).ToArray();
            if (columns.Length == 0) throw new ArgumentException("Select at least one reference-key column.");
            return string.Join(" AND ", columns.Select(column =>
            {
                if (!source.Table.Columns.Contains(column))
                    throw new ArgumentException("A selected reference-key column is missing from the source table.");
                string name = "[" + column.Replace("\\", "\\\\").Replace("]", "\\]") + "]";
                object value = source[column];
                return value == null || value == DBNull.Value ? name + " IS NULL" : name + " = " + Literal(value);
            }));
        }

        private static string Literal(object value)
        {
            if (value is DateTime)
                return "#" + ((DateTime)value).ToString("MM/dd/yyyy HH:mm:ss.fffffff", CultureInfo.InvariantCulture) + "#";
            if (value is bool) return (bool)value ? "true" : "false";
            if (value is Guid)
                return "Convert('" + ((Guid)value).ToString("D") + "', 'System.Guid')";
            if (value is string || value is char)
                return "'" + Convert.ToString(value, CultureInfo.InvariantCulture).Replace("'", "''") + "'";
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte: case TypeCode.SByte: case TypeCode.Int16: case TypeCode.UInt16:
                case TypeCode.Int32: case TypeCode.UInt32:
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
                case TypeCode.Int64:
                    return "Convert('" + ((long)value).ToString(CultureInfo.InvariantCulture) + "', 'System.Int64')";
                case TypeCode.UInt64:
                    return "Convert('" + ((ulong)value).ToString(CultureInfo.InvariantCulture) + "', 'System.UInt64')";
                case TypeCode.Decimal:
                    // Integer-looking literals above Int64.MaxValue are parsed as Double by
                    // DataColumn expressions. A decimal point preserves all Decimal digits.
                    string decimalText = ((decimal)value).ToString(CultureInfo.InvariantCulture);
                    return decimalText.IndexOf('.') >= 0 ? decimalText : decimalText + ".0";
                case TypeCode.Single:
                    float single = (float)value;
                    if (!float.IsNaN(single) && !float.IsInfinity(single))
                        return "Convert(" + single.ToString("R", CultureInfo.InvariantCulture) + ", 'System.Single')";
                    break;
                case TypeCode.Double:
                    double number = (double)value;
                    if (!double.IsNaN(number) && !double.IsInfinity(number))
                        return "Convert(" + number.ToString("R", CultureInfo.InvariantCulture) + ", 'System.Double')";
                    break;
            }
            throw new ArgumentException("A reference key has an unsupported value type. Use text, numeric, date, Boolean or Guid keys.");
        }

        internal static object[] ValuesForColumns(DataRow row, DataTable schema)
        {
            return schema.Columns.Cast<DataColumn>().Select(column => row[column.ColumnName]).ToArray();
        }
    }
}
