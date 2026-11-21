using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

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
            CheckScriptGeneration(passed);
            CheckExecutionInputs(passed);
            CheckUtilities(passed);
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

        private static void CheckScriptGeneration(Action<string> passed)
        {
            using (var form = new Generate_IUD_Script_Form())
            using (var data = new DataSet())
            {
                Rejected(form.ValidateGenerationInput);
                data.Tables.Add(Example("Items"));
                Find<RadioButton>(form, "rb_SheetName").Checked = true;
                Find<RadioButton>(form, "rb_Update").Checked = true;
                form.SetWorkbook(data);
                Find<TextBox>(form, "tb_TableName").Text = "dbo.Items";
                Rejected(form.ValidateGenerationInput);
                var columns = Find<CheckedListBox>(form, "clb_Checked");
                columns.SetItemChecked(columns.FindStringExact("ID"), true);
                form.ValidateGenerationInput();
                columns.SetItemChecked(columns.FindStringExact("Value"), true);
                Rejected(form.ValidateGenerationInput);
                Find<RadioButton>(form, "rb_Delete").Checked = true;
                columns.SetItemChecked(columns.FindStringExact("ID"), true);
                form.ValidateGenerationInput();
                Find<TextBox>(form, "tb_TableName").Text = "dbo.Items; DELETE FROM dbo.Other";
                Rejected(form.ValidateGenerationInput);
            }
            using (var form = new Generate_IUD_Script_Form())
            using (var previous = new DataSet())
            using (var replacement = new DataSet())
            using (var invalid = new DataSet())
            {
                Find<RadioButton>(form, "rb_SheetName").Checked = true;
                Find<RadioButton>(form, "rb_Update").Checked = true;
                previous.Tables.Add(Example("Previous"));
                form.SetWorkbook(previous);
                var columns = Find<CheckedListBox>(form, "clb_Checked");
                columns.SetItemChecked(columns.FindStringExact("ID"), true);
                Set(form, "boolGenerate", true);
                Set(form, "strFullPathFileName", "previous-output.xlsx");
                Find<LinkLabel>(form, "llb_File").Text = "previous-output.xlsx";

                DataTable headerOnly = replacement.Tables.Add("Headers only");
                headerOnly.Columns.Add("EmptyId");
                replacement.Tables.Add(Example("LaterData"));
                form.SetWorkbook(replacement);
                var sheets = Find<ComboBox>(form, "cb_SelectSheet");
                Require(sheets.Items.Count == 2 && Convert.ToString(sheets.Items[0]) == "Headers only"
                    && Convert.ToString(sheets.Items[1]) == "LaterData", "A header-only first sheet retained stale worksheet choices or hid later data.");
                Require(columns.CheckedItems.Count == 0 && !columns.Items.Contains("ID"), "A new workbook retained stale WHERE selections.");
                Require(!Get<bool>(form, "boolGenerate") && string.IsNullOrEmpty(Get<string>(form, "strFullPathFileName"))
                    && string.IsNullOrEmpty(Find<LinkLabel>(form, "llb_File").Text), "Import retained a stale generated-output link.");
                Rejected(form.ValidateGenerationInput);
                sheets.SelectedIndex = 1;
                Require(ReferenceEquals(Get<DataTable>(form, "dtGenerateScript"), replacement.Tables[1]), "Selecting the later worksheet did not select its data.");
                columns.SetItemChecked(columns.FindStringExact("ID"), true);
                string sqlTable = "[database].[schema].[long table identifier beyond Excel sheet limits]";
                Find<TextBox>(form, "tb_TableName").Text = sqlTable;
                form.ValidateGenerationInput();
                Require(form.OutputWorksheetName() == "LaterData"
                    && Find<TextBox>(form, "tb_TableName").Text == sqlTable,
                    "The Excel worksheet name was coupled to a quoted/long SQL identifier or changed the SQL target.");
                Rejected(() => form.SetWorkbook(invalid));
                Require(sheets.Items.Count == 2 && sheets.SelectedIndex == 1 && form.OutputWorksheetName() == "LaterData",
                    "A rejected workbook destroyed the previous worksheet selection.");
            }
            Require(Generate_IUD_Script_Form.SqlLiteral("  O'Brien  ") == "N'  O''Brien  '", "SQL literal changed whitespace or lost apostrophe escaping.");
            Require(Generate_IUD_Script_Form.SqlLiteral(string.Empty) == "N''", "Empty text was silently converted to NULL.");
            Require(Generate_IUD_Script_Form.SqlLiteral(DBNull.Value) == "NULL", "DBNull was not emitted as SQL NULL.");
            Require(Generate_IUD_Script_Form.SqlPredicate("A]B", DBNull.Value) == "[A]]B] IS NULL", "Null WHERE predicate or identifier quoting is wrong.");
            Generate_IUD_Script_Form.ValidateTableName("[database].[schema].[table with spaces]", null);
            Rejected(() => Generate_IUD_Script_Form.ValidateTableName("dbo.[unfinished", null));
            passed("Script generation validates WHERE/SET and literals, replaces stale workbook selections, permits later populated sheets after a header-only first sheet, and keeps Excel names independent of SQL identifiers.");
        }

        private static void CheckExecutionInputs(Action<string> passed)
        {
            string folder = Path.Combine(Path.GetTempPath(), "spi-excel-validation-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            try
            {
                using (var form = new Execute_Excel_Scripts_Form()) Rejected(form.ValidateTestedConnection);
                using (var table = new DataTable())
                {
                    table.Columns.Add("SQL");
                    table.Columns.Add("Name");
                    table.Rows.Add("select 1 -- retain comment\n", "First");
                    Execute_Excel_Scripts_Form.ValidateScriptTable(table, new[] { "SQL" }, true, "Name", folder, null);
                    Rejected(() => Execute_Excel_Scripts_Form.ValidateScriptTable(table, new[] { "SQL", "Name" }, false, null, null, null));
                    table.Rows.Add(" ", "Second");
                    Rejected(() => Execute_Excel_Scripts_Form.ValidateScriptTable(table, new[] { "SQL" }, false, null, null, null));
                    table.Rows[1]["SQL"] = "select 2";
                    table.Rows[1]["Name"] = "first";
                    Rejected(() => Execute_Excel_Scripts_Form.ValidateScriptTable(table, new[] { "SQL" }, true, "Name", folder, null));
                    table.Rows[1]["Name"] = "../escape";
                    Rejected(() => Execute_Excel_Scripts_Form.ValidateScriptTable(table, new[] { "SQL" }, true, "Name", folder, null));
                    table.Rows[1]["Name"] = "Second";
                    Execute_Excel_Scripts_Form.ValidateScriptTable(table, new[] { "SQL" }, true, "Name", folder, null);
                    Require(Directory.GetFiles(folder).Length == 0, "Validation unexpectedly wrote output files.");
                }
            }
            finally { Directory.Delete(folder); }
            passed("Script execution validates one nonempty statement column, tested connection and all safe unique export filenames before any SQL or output writes.");
        }

        private static void CheckUtilities(Action<string> passed)
        {
            using (var control = new ExcelFileReadCustomControl())
            using (var customised = new CustomisedExcel())
            using (var data = new DataSet())
            {
                Rejected(() => ExcelFileReadCustomControl.ValidateWorkbook(data, control));
                Rejected(customised.ValidateExportInput);
                data.Tables.Add(Example("Input"));
                ExcelFileReadCustomControl.ValidateWorkbook(data, control);
                Set(customised, "dsCustomisedExcel", data);
                Rejected(customised.ValidateExportInput);
                Find<CheckedListBox>(customised, "checkedListBox1").Items.Add("Value", true);
                customised.ValidateExportInput();
            }
            using (var input = new DataTable())
            {
                input.Columns.Add("CMPNT_ID");
                input.Columns.Add("NAME");
                input.Columns.Add("TYPE");
                input.Columns.Add("Remark");
                input.Rows.Add("1", "Name", "Type", DBNull.Value);
                using (var notes = SPI_Spec_Note_Creation_Utility.BuildNotes(input, null))
                    Require(notes.Rows[0]["NOTE"].ToString() == "" && (int)notes.Rows[0]["LengthNote"] == 0,
                        "All-empty note data did not generate an empty note safely.");
                input.Rows[0]["Remark"] = "Text";
                using (var notes = SPI_Spec_Note_Creation_Utility.BuildNotes(input, null))
                    Require(notes.Rows[0]["NOTE"].ToString() == "Remark:Text", "Note values changed unexpectedly.");
                Require(input.Columns.Count == 4, "Note generation mutated its source table.");
                input.Columns[0].ColumnName = "Wrong";
                Rejected(() => SPI_Spec_Note_Creation_Utility.BuildNotes(input, null));
            }
            string log = "Table Name: Synthetic;\nTarget Sql: select 1\nSource Sql: select 1\nFirst Id: example\n"
                + "Total Source Rows: 2; Inserted Rows: 1; Updated Rows: 0; Rejected Rows: 1;\n";
            using (var parsed = MergeLogsExport.ParseMergeLog(log, null))
                Require(parsed.Tables.Count == 2 && (int)parsed.Tables[0].Rows[0]["Rejected Rows"] == 1,
                    "Valid log counts or rejection details were lost.");
            Rejected(() => MergeLogsExport.ParseMergeLog("garbage", null));
            Rejected(() => MergeLogsExport.ParseMergeLog(log.Replace("Inserted Rows: 1", "Inserted Rows: -1"), null));
            Rejected(() => MergeLogsExport.ParseMergeLog(log.Replace("Total Source Rows: 2", "Total Source Rows: 999999999999"), null));
            passed("Workbook/custom export, specification notes and merge-log import validate required data, selected columns, schemas, empty notes and numeric log records without mutating inputs.");
        }
    }
}
