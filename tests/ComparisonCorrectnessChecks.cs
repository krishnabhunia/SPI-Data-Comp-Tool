using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Runs the real legacy comparison/report paths on deterministic synthetic snapshots.</summary>
    internal static class ComparisonCorrectnessChecks
    {
        internal static void Run(Action<string> passed)
        {
            CheckTypedKeys(passed);
            CheckCommonColumns(passed);
            string folder = Path.Combine(Path.GetTempPath(), "spi-comparison-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            bool savedEpplus = Properties.Settings.Default.EPPlus_Excel_Save;
            bool savedClosedXml = Properties.Settings.Default.XL_Excel_Save;
            try
            {
                Properties.Settings.Default.EPPlus_Excel_Save = true;
                Properties.Settings.Default.XL_Excel_Save = false;
                CheckDuplicateReferences(folder, passed);
                CheckTargetRulesAndColumnOrder(folder, passed);
                CheckListComparison(folder, passed);
                CheckExcelComparison(folder, passed);
            }
            finally
            {
                Properties.Settings.Default.EPPlus_Excel_Save = savedEpplus;
                Properties.Settings.Default.XL_Excel_Save = savedClosedXml;
                Directory.Delete(folder, true);
            }
        }

        internal static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        internal static T Get<T>(object owner, string field)
        {
            FieldInfo info = owner.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
            if (info == null) throw new InvalidOperationException("Missing comparison state: " + field);
            return (T)info.GetValue(owner);
        }

        internal static void Set(object owner, string field, object value)
        {
            FieldInfo info = owner.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
            if (info == null) throw new InvalidOperationException("Missing comparison state: " + field);
            info.SetValue(owner, value);
        }

        internal static object Invoke(object owner, string method, params object[] args)
        {
            MethodInfo info = owner.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic);
            if (info == null) throw new InvalidOperationException("Missing comparison method: " + method);
            try { return info.Invoke(owner, args); }
            catch (TargetInvocationException ex) { throw new InvalidOperationException("Comparison failed: " + method, ex.InnerException); }
        }

        internal static T Control<T>(System.Windows.Forms.Control owner, string name) where T : System.Windows.Forms.Control
        {
            return owner.Controls.Find(name, true).OfType<T>().Single();
        }

        private static DataTable Table(string name)
        {
            var table = new DataTable(name);
            table.Columns.Add("Id", typeof(string));
            table.Columns.Add("Value", typeof(string));
            table.Columns.Add("Context", typeof(string));
            return table;
        }

        private static void CheckTypedKeys(Action<string> passed)
        {
            CultureInfo saved = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                using (var target = new DataTable())
                {
                    target.Columns.Add("A]B\\C", typeof(string));
                    target.Columns.Add("Amount", typeof(decimal));
                    target.Columns.Add("When", typeof(DateTime));
                    target.Columns.Add("Enabled", typeof(bool));
                    var when = new DateTime(2026, 11, 22, 17, 25, 43).AddTicks(1234567);
                    target.Rows.Add("O'Brien", 1.25m, when, true);
                    target.Rows.Add(DBNull.Value, 1.25m, when, true);
                    target.Rows.Add("", 1.25m, when, true);
                    string[] keys = target.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
                    foreach (DataRow row in target.Rows)
                    {
                        DataRow[] matches = target.Select(ComparisonRows.KeyFilter(row, keys));
                        Require(matches.Length == 1 && ReferenceEquals(matches[0], row),
                            "Typed/composite reference keys changed with locale, quotes, brackets, nulls or date precision.");
                    }
                    var repeated = new ComparisonRows(target, keys);
                    Require(ReferenceEquals(repeated.TakeNext(target.Rows[0]), target.Rows[0]) && repeated.TakeNext(target.Rows[0]) == null,
                        "A target row was reused by more than one source row.");
                    Require(repeated.Remaining().Rows.Count == 2 && target.Rows.Count == 3,
                        "Matching changed the input snapshot or dropped unconsumed rows.");
                }
                using (var duplicates = new DataTable())
                {
                    duplicates.Columns.Add("Id");
                    duplicates.Columns.Add("Ordinal", typeof(int));
                    for (int index = 0; index < 2000; index++) duplicates.Rows.Add("repeated", index);
                    var matches = new ComparisonRows(duplicates, new[] { "Id" });
                    for (int index = 0; index < 2000; index++)
                        Require((int)matches.TakeNext(duplicates.Rows[0])["Ordinal"] == index,
                            "A repeated-key group did not consume target rows once in original order.");
                    Require(matches.TakeNext(duplicates.Rows[0]) == null && matches.Remaining().Rows.Count == 0,
                        "An exhausted repeated-key group yielded another match.");
                }
                foreach (object[] values in new[]
                {
                    new object[] { 9223372036854775808m, 9223372036854775809m },
                    new object[] { decimal.MaxValue - 1m, decimal.MaxValue },
                    new object[] { ulong.MaxValue - 1, ulong.MaxValue },
                    new object[] { long.MinValue, long.MinValue + 1 },
                    new object[] { float.Epsilon, float.MaxValue },
                    new object[] { double.Epsilon, double.MaxValue },
                    new object[] { Guid.Parse("11111111-2222-3333-4444-555555555555"), Guid.Parse("11111111-2222-3333-4444-555555555556") },
                })
                using (var precise = new DataTable())
                {
                    precise.Columns.Add("Id", values[0].GetType());
                    precise.Rows.Add(values[0]);
                    precise.Rows.Add(values[1]);
                    foreach (DataRow row in precise.Rows)
                    {
                        DataRow[] found = precise.Select(ComparisonRows.KeyFilter(row, new[] { "Id" }));
                        Require(found.Length == 1 && ReferenceEquals(found[0], row),
                            "Adjacent high-precision numeric keys collapsed through floating-point literals.");
                    }
                }
            }
            finally { Thread.CurrentThread.CurrentCulture = saved; }
            passed("Typed composite reference keys handle apostrophes, bracket/backslash names, null versus empty, decimals, Booleans and subsecond dates independently of UI culture.");
        }

        private static void CheckCommonColumns(Action<string> passed)
        {
            using (var form = new DB_Comparison_Form())
            using (var source = new DataTable())
            using (var target = new DataTable())
            using (var ignored = new ListBox())
            {
                foreach (string column in new[] { "OnlyA", "Id", "OnlyB", "Value", "OnlyC" }) source.Columns.Add(column);
                target.Columns.Add("Id");
                target.Columns.Add("Value");
                Invoke(form, "getCommonColumns", source, target, ignored);
                Require(source.Columns.Count == 2 && source.Columns.Contains("Id") && source.Columns.Contains("Value")
                    && ignored.Items.Count == 3, "Removing uncommon columns stopped after the first collection mutation.");
            }
            passed("Database loading removes every uncommon column and reports each ignored name.");
        }

        internal static void RunDatabaseComparison(DB_Comparison_Form form, DataTable source, DataTable target,
            bool primaryKey, string file, bool rules = false)
        {
            Control<CheckBox>(form, "cb_PrimaryKey").Checked = primaryKey;
            Control<CheckBox>(form, "cb_IncludeRule").Checked = rules;
            Control<CheckBox>(form, "cb_IgnoreCase").Checked = false;
            var keys = Control<CheckedListBox>(form, "clb_ReferenceID");
            var columns = Control<CheckedListBox>(form, "cbL_SRC_Cols");
            keys.Items.Clear();
            columns.Items.Clear();
            foreach (DataColumn column in source.Columns)
            {
                keys.Items.Add(column.ColumnName, column.ColumnName == "Id");
                columns.Items.Add(column.ColumnName, column.ColumnName != "Id");
            }
            Set(form, "saveFileDialogFile", new SaveFileDialogFile { BoolFileSaveStatus = true, Str_saveFileNameWithPath = file });
            Set(form, "dt_oracle", source.Copy());
            Set(form, "dt_sql", target.Copy());
            var args = new DoWorkEventArgs(null);
            Invoke(form, "backgroundWorker1_DoWork", null, args);
            Exception failure = Get<Exception>(form, "myException_BG");
            Require(!args.Cancel && Get<bool>(form, "boolStatus") && File.Exists(file),
                "Database comparison did not produce a successful workbook: " + (failure == null ? "unknown" : failure.Message));
        }

        private static void CheckDuplicateReferences(string folder, Action<string> passed)
        {
            using (var source = Table("Source"))
            using (var target = Table("Target"))
            using (var form = new DB_Comparison_Form())
            {
                source.Rows.Add("duplicate", "first", "same");
                source.Rows.Add("duplicate", "second", "same");
                source.Rows.Add("duplicate", "excess-source", "same");
                source.Rows.Add("O'Brien", "same", "same");
                source.Rows.Add(DBNull.Value, "null-key", "same");
                source.Rows.Add("", "empty-key", "same");
                target.Rows.Add("duplicate", "first", "same");
                target.Rows.Add("duplicate", "second", "same");
                target.Rows.Add("O'Brien", "same", "same");
                target.Rows.Add(DBNull.Value, "null-key", "same");
                target.Rows.Add("", "empty-key", "same");
                target.Rows.Add("target-only", "one", "same");
                target.Rows.Add("target-only", "two", "same");
                string file = Path.Combine(folder, "duplicates.xlsx");
                RunDatabaseComparison(form, source, target, false, file);
                using (var book = new ExcelPackage(new FileInfo(file)))
                {
                    Require(book.Workbook.Worksheets["Mismatch"].Dimension.End.Row == 1, "Equal duplicate pairs produced false differences.");
                    Require(book.Workbook.Worksheets["Source Rows Only"].Dimension.End.Row == 2,
                        "An excess source duplicate reused an already matched target.");
                    Require(book.Workbook.Worksheets["Target Rows Only"].Dimension.End.Row == 3,
                        "Unmatched target duplicates disappeared from the report.");
                    Require(book.Workbook.Worksheets["Source Rows Only"].Cells[2, 2].Text == "excess-source",
                        "Duplicate reference matching did not preserve source/target order.");
                }
            }
            passed("Database reference-key comparison exports equal/null/quoted keys and preserves excess source and target duplicates without reusing or deleting matches.");
        }

        private static DataSet Rules()
        {
            var rules = new DataSet();
            DataTable table = rules.Tables.Add("Rules");
            foreach (string column in new[] { "srno", "Source", "Target", "RuleType", "Source_ColName", "Target_Colname",
                "ConditionSourceColumn", "ConditionTargetColumn", "ConditionSourceValue", "ConditionTargetValue" }) table.Columns.Add(column);
            table.Rows.Add("1", "old", "new", "3", "Value", "Value", "Context", "Context", "source", "target");
            return rules;
        }

        private static void CheckTargetRulesAndColumnOrder(string folder, Action<string> passed)
        {
            foreach (bool primary in new[] { false, true })
            using (var form = new DB_Comparison_Form())
            using (var source = Table("Source"))
            using (var target = Table("Target"))
            using (DataSet rules = Rules())
            {
                source.Rows.Add("1", "old", "source");
                target.Rows.Add("1", "new", "target");
                // An equal key with a changed target condition must not take the rule.
                source.Rows.Add("2", "old", "source");
                target.Rows.Add("2", "new", "other");
                target.Columns["Context"].SetOrdinal(1);
                Get<Common_Data>(form, "common_Data").dsRules = rules;
                string file = Path.Combine(folder, "rules-" + primary + ".xlsx");
                RunDatabaseComparison(form, source, target, primary, file, true);
                using (var book = new ExcelPackage(new FileInfo(file)))
                {
                    ExcelWorksheet summary = book.Workbook.Worksheets["Summary"];
                    int valueRow = Enumerable.Range(2, summary.Dimension.End.Row - 1)
                        .Single(row => summary.Cells[row, 1].Text == "Value");
                    Require(summary.Cells[valueRow, 2].GetValue<int>() == 1,
                        "Conditional rules evaluated a source row in place of the real target context.");
                    ExcelWorksheet mismatches = book.Workbook.Worksheets["Mismatch"];
                    int valueColumn = Enumerable.Range(1, mismatches.Dimension.End.Column)
                        .Single(column => mismatches.Cells[1, column].Text == "Value");
                    Require(mismatches.Cells[3, valueColumn].Text == "new",
                        "Target values were written under the wrong source header when column order differed.");
                }
            }
            passed("Primary and reference-key database comparisons use real target conditions and write reordered target columns under the correct report headers.");
        }

        private static void CheckListComparison(string folder, Action<string> passed)
        {
            using (var form = new Compare_Multiples_List_Tables_Forms())
            using (var source = Table("ListSource"))
            using (var target = Table("ListTarget"))
            {
                source.Rows.Add("O'Brien", "one", "same");
                target.Rows.Add("O'Brien", "one", "same");
                target.Rows.Add("O'Brien", "unpaired", "same");
                Set(form, "strFolderPath", folder);
                Control<CheckBox>(form, "cb_IncludeRule").Checked = false;
                Invoke(form, "CompareTwoDataTable", source, target, "Id", new DoWorkEventArgs(null));
                Require(!Get<bool>(form, "operationFailed"), "List comparison failed for quoted duplicate keys.");
                DataTable summary = Get<DataTable>(form, "dtSummaryForList");
                Require(summary.Rows.Count == 1 && Convert.ToInt32(summary.Rows[0]["Target Rows Count"]) == 1,
                    "List comparison removed every duplicate after comparing only the first target row.");
                using (var book = new ExcelPackage(new FileInfo(Get<string>(form, "strFileName"))))
                    Require(book.Workbook.Worksheets["Target Rows Only"].Cells[2, 2].Text == "unpaired",
                        "The unpaired target duplicate is missing from the exported list report.");
            }
            passed("List comparison leaves unpaired target duplicates in both its summary and actual workbook.");
        }

        private static void CheckExcelComparison(string folder, Action<string> passed)
        {
            using (var form = new Excel_Excel_Comparison())
            using (var source = Table("ExcelSource"))
            using (var target = Table("ExcelTarget"))
            using (var data = new DataSet())
            {
                source.Columns.Add("boolChecked", typeof(string));
                target.Columns.Add("boolChecked", typeof(string));
                source.Rows.Add("repeat", "one", "same", "user-data");
                source.Rows.Add("repeat", "two", "same", "user-data");
                source.Rows.Add("repeat", "extra", "same", "user-data");
                target.Rows.Add("repeat", "one", "same", "user-data");
                target.Rows.Add("repeat", "two", "same", "user-data");
                Set(form, "dt_firstTable", source);
                Set(form, "dt_secondTable", target);
                data.Tables.Add(source.Copy());
                data.Tables.Add(target.Copy());
                Set(form, "ds", data);
                Set(form, "boolSelectedBothFile", true);
                Control<CheckBox>(form, "cb_Primary_Key").Checked = false;
                Control<CheckBox>(form, "cb_IncludeRule").Checked = false;
                Control<CheckBox>(form, "cb_IncludeDiff_Columns").Checked = false;
                var keys = Control<CheckedListBox>(form, "clb_reference_ID");
                var columns = Control<CheckedListBox>(form, "clb_firstExcel");
                foreach (DataColumn column in source.Columns)
                {
                    keys.Items.Add(column.ColumnName, column.ColumnName == "Id");
                    columns.Items.Add(column.ColumnName, column.ColumnName != "Id");
                }
                form.ValidateComparisonInput();
                Set(form, "boolValidate", true);
                string file = Path.Combine(folder, "excel-duplicates.xlsx");
                Set(form, "saveFileDialogFile", new SaveFileDialogFile { BoolFileSaveStatus = true, Str_saveFileNameWithPath = file });
                Invoke(form, "backgroundWorker1_DoWork", null, new DoWorkEventArgs(null));
                Require(Get<Exception>(form, "myException") == null && File.Exists(file), "Excel reference-key comparison failed.");
                using (var book = new ExcelPackage(new FileInfo(file)))
                {
                    ExcelWorksheet only = book.Workbook.Worksheets["Source Rows Only"];
                    Require(only.Dimension.End.Row == 2 && only.Cells[2, 2].Text == "extra",
                        "The final available Excel target row was reused by an excess source duplicate.");
                    Require(only.Cells[1, 4].Text == "boolChecked" && only.Cells[2, 4].Text == "user-data",
                        "An internal comparison marker overwrote a user column named boolChecked.");
                }
            }
            passed("Excel comparison consumes the last target match once and preserves user columns formerly overwritten by internal markers.");
        }
    }
}
