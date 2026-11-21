using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Synthetic action-boundary checks; never opens dialogs, executes SQL or changes real preferences.</summary>
    internal static class ExcelFormValidationChecks
    {
        internal static void Run(Action<string> passed)
        {
            CheckExcelComparison(passed);
            CheckMultipleReference(passed);
            CheckMultipleReferenceCounts(passed);
            CheckUtilities(passed);
            CheckWorkbookReader(passed);
        }

        private static void Require(bool valid, string message)
        {
            if (!valid) throw new InvalidOperationException(message);
        }

        private static void Rejected(Action action)
        {
            try { action(); }
            catch (ValidationFailure) { return; }
            throw new InvalidOperationException("Invalid Excel/workflow input was accepted.");
        }

        private static T Find<T>(Control owner, string name) where T : Control
        {
            return owner.Controls.Find(name, true).OfType<T>().Single();
        }

        private static void Set(object owner, string name, object value)
        {
            owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(owner, value);
        }

        private static T Get<T>(object owner, string name)
        {
            return (T)owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(owner);
        }

        private static DataTable Example(string name)
        {
            var table = new DataTable(name);
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Value", typeof(string));
            table.Rows.Add("1", "Original");
            return table;
        }

        private static void CheckExcelComparison(Action<string> passed)
        {
            using (var form = new Excel_Excel_Comparison())
            using (var data = new DataSet())
            {
                Rejected(form.ValidateComparisonInput);
                data.Tables.Add(Example("Source"));
                data.Tables.Add(Example("Target"));
                Set(form, "ds", data);
                Set(form, "boolSelectedBothFile", true);
                var keys = Find<CheckedListBox>(form, "clb_reference_ID");
                var compared = Find<CheckedListBox>(form, "clb_firstExcel");
                keys.Items.Add("ID", true);
                compared.Items.Add("Value", true);
                Find<CheckBox>(form, "cb_IncludeRule").Checked = false;
                form.ValidateComparisonInput();
                data.Tables[1].Rows.Add("1", "Duplicate");
                Rejected(form.ValidateComparisonInput);
                Find<CheckBox>(form, "cb_Primary_Key").Checked = false;
                form.ValidateComparisonInput(); // Duplicates remain supported outside explicit Primary-key mode.
                Find<CheckBox>(form, "cb_Primary_Key").Checked = true;
                data.Tables[1].Rows.RemoveAt(1);
                data.Tables[1].Rows[0]["ID"] = DBNull.Value;
                Rejected(form.ValidateComparisonInput);
                data.Tables[1].Rows[0]["ID"] = "1";
                Set(form, "boolValidate", true);
                compared.SetItemChecked(0, false);
                Require(!Get<bool>(form, "boolValidate"), "Changing a checked comparison column retained prior validation.");
                Rejected(form.ValidateComparisonInput);
                compared.SetItemChecked(0, true);
                keys.Items.Add("Missing", true);
                Rejected(form.ValidateComparisonInput);
                keys.Items.Remove("Missing");
                form.ValidateComparisonInput();
            }
            using (var table = new DataTable())
            {
                table.Columns.Add("A]B\\C", typeof(string));
                table.Rows.Add("O'Brien");
                table.Rows.Add(DBNull.Value);
                Require(table.Select(Excel_Excel_Comparison.FilterPredicate("A]B\\C", "O'Brien")).Length == 1,
                    "Reference filter did not preserve escaped identifier/value text.");
                Require(table.Select(Excel_Excel_Comparison.FilterPredicate("A]B\\C", DBNull.Value)).Length == 1,
                    "Null reference filters did not use IS NULL.");
            }
            passed("Excel comparison revalidates loaded keys/columns, rejects invalid explicit primary keys and invalidates stale selections; escaped/null filters remain usable.");
        }

        private static void CheckMultipleReference(Action<string> passed)
        {
            using (var source = new DataSet())
            using (var target = new DataSet())
            using (var form = new ViewForMultiReference())
            {
                Rejected(form.ValidateMapping);
                source.Tables.Add(Example("Source"));
                target.Tables.Add(Example("Target"));
                Set(form, "dsSource", source);
                Set(form, "dsTarget", target);
                var sourceKeys = Find<ListBox>(form, "lb_Source_ID");
                var targetKeys = Find<ListBox>(form, "lb_Target_ID");
                var sourceColumns = Find<ListBox>(form, "lb_Source_Col");
                var targetColumns = Find<ListBox>(form, "lb_Target_Col");
                sourceKeys.Items.Add("ID");
                targetKeys.Items.Add("ID");
                sourceColumns.Items.Add("Value");
                targetColumns.Items.Add("Value");
                form.ValidateMapping();
                targetColumns.Items.Clear();
                Rejected(form.ValidateMapping);
                targetColumns.Items.Add("ID");
                Rejected(form.ValidateMapping);
                targetColumns.Items.Clear();
                targetColumns.Items.Add("Missing");
                Rejected(form.ValidateMapping);
            }
            passed("Multiple-reference mapping rejects empty, unequal, overlapping and missing column mappings before replacing prepared data.");
        }

        private static void CheckMultipleReferenceCounts(Action<string> passed)
        {
            using (var source = new DataSet())
            using (var target = new DataSet())
            {
                source.Tables.Add(Example("Source"));
                target.Tables.Add(Example("UnselectedTargetSheet"));
                DataTable selected = target.Tables.Add("SelectedTargetSheet");
                selected.Columns.Add("ID", typeof(string));
                selected.Columns.Add("Value", typeof(string));
                selected.Columns.Add("Extra", typeof(string));
                selected.Rows.Add("1", "One", "A");
                selected.Rows.Add("2", "Two", "B");
                selected.Rows.Add("3", "Three", "C");

                using (var form = new ViewForMultiReference(source, target))
                {
                    Require(Find<Label>(form, "lbl_Source").Text == "Rows : 1 and Columns : 2",
                        "Advanced mapping did not display the loaded source dimensions.");
                    Require(Find<Label>(form, "lbl_Target").Text == "Rows : 3 and Columns : 3",
                        "Advanced mapping displayed dimensions from the unselected target worksheet.");
                    Require(Find<CheckedListBox>(form, "clb_Target").Items.Contains("Extra"),
                        "Target mapping columns did not come from the selected worksheet.");
                    selected.Rows.Clear();
                    Require(Get<DataSet>(form, "dsTarget").Tables[0].Rows.Count == 3,
                        "Advanced mapping no longer retained its independent target snapshot.");
                }
            }
            passed("Advanced mapping displays counts and columns from its selected target worksheet, preserving its independent snapshot.");
        }

        private static void CheckUtilities(Action<string> passed)
        {
            using (var customised = new CustomisedExcel())
            using (var data = new DataSet())
            {
                Rejected(() => FormValidation.ValidateWorkbook(data, customised));
                Rejected(customised.ValidateExportInput);
                data.Tables.Add(Example("Input"));
                FormValidation.ValidateWorkbook(data, customised);
                Set(customised, "dsCustomisedExcel", data);
                Rejected(customised.ValidateExportInput);
                Find<CheckedListBox>(customised, "checkedListBox1").Items.Add("Value", true);
                customised.ValidateExportInput();
            }
            string log = "Table Name: Synthetic;\nTarget Sql: select 1\nSource Sql: select 1\nFirst Id: example\n"
                + "Total Source Rows: 2; Inserted Rows: 1; Updated Rows: 0; Rejected Rows: 1;\n";
            using (var parsed = MergeLogsExport.ParseMergeLog(log, null))
                Require(parsed.Tables.Count == 2 && (int)parsed.Tables[0].Rows[0]["Rejected Rows"] == 1,
                    "Valid log counts or rejection details were lost.");
            Rejected(() => MergeLogsExport.ParseMergeLog("garbage", null));
            Rejected(() => MergeLogsExport.ParseMergeLog(log.Replace("Inserted Rows: 1", "Inserted Rows: -1"), null));
            Rejected(() => MergeLogsExport.ParseMergeLog(log.Replace("Total Source Rows: 2", "Total Source Rows: 999999999999"), null));
            passed("Workbook/custom export and merge-log import validate required data, selected columns and numeric log records without mutating inputs.");
        }

        private static void CheckWorkbookReader(Action<string> passed)
        {
            string folder = Path.Combine(Path.GetTempPath(), "spi-workbook-validation-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "input.xlsx");
            try
            {
                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Input");
                    sheet.Cells[1, 1].Value = "Column1";
                    sheet.Cells[1, 2].Value = " Id ";
                    sheet.Cells[1, 4].Value = "Label";
                    sheet.Cells[2, 1].Value = "001";
                    sheet.Cells[2, 2].Value = 1;
                    sheet.Cells[2, 4].Value = "नाम";
                    package.SaveAs(new FileInfo(path));
                }
                byte[] original = File.ReadAllBytes(path);
                using (DataSet loaded = ReadExcel.LoadExcelFromDataReader(path))
                {
                    Require(loaded.Tables.Count == 1 && loaded.Tables[0].Columns.Count == 3 && loaded.Tables[0].Rows.Count == 1,
                        "The workbook reader did not ignore only the unused spacer column.");
                    Require(loaded.Tables[0].Columns.Contains("COLUMN1") && loaded.Tables[0].Columns.Contains("ID"),
                        "Header normalization collided with the reader's generated column names.");
                    Require((string)loaded.Tables[0].Rows[0]["COLUMN1"] == "001" && (string)loaded.Tables[0].Rows[0]["LABEL"] == "नाम",
                        "Workbook validation changed leading zeros or Unicode data.");
                }
                Require(original.SequenceEqual(File.ReadAllBytes(path)), "Reading a workbook modified the input.");
                using (DataSet preserved = new Common_Data().LoadExcelFromDataReader(path))
                {
                    Require(preserved.Tables[0].Columns[0].ColumnName == "Column1"
                        && preserved.Tables[0].Columns[1].ColumnName == " Id ",
                        "The case-preserving reader changed existing header spelling or whitespace.");
                }
                using (var package = new ExcelPackage(new FileInfo(path)))
                {
                    package.Workbook.Worksheets[0].Cells[1, 4].Value = " id ";
                    package.Save();
                }
                ExpectInvalidWorkbook(path);
                using (var package = new ExcelPackage(new FileInfo(path)))
                {
                    package.Workbook.Worksheets[0].Cells[1, 4].Value = null;
                    package.Save();
                }
                ExpectInvalidWorkbook(path); // Nonempty data under a blank header must not disappear.
                File.WriteAllBytes(path, new byte[0]);
                ExpectInvalidWorkbook(path);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
                Directory.Delete(folder);
            }
            passed("Real synthetic workbook imports preserve Unicode/leading zeros, handle generated-name collisions and blank spacers, reject duplicate/missing headers and empty files, and never modify inputs.");
        }

        private static void ExpectInvalidWorkbook(string path)
        {
            bool rejected = false;
            try { using (ReadExcel.LoadExcelFromDataReader(path)) { } }
            catch (InvalidDataException) { rejected = true; }
            Require(rejected, "An invalid synthetic workbook was accepted.");
            rejected = false;
            try { using (new Common_Data().LoadExcelFromDataReader(path)) { } }
            catch (InvalidDataException) { rejected = true; }
            Require(rejected, "The case-preserving reader accepted an invalid synthetic workbook.");
        }
    }
}
