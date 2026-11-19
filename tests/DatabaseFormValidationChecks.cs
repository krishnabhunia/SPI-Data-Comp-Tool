using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Form action gates and workbook contracts exercised without a database or file dialog.</summary>
    internal static class DatabaseFormValidationChecks
    {
        internal static void Run(Action<string> passed)
        {
            CheckComparison(passed);
            CheckConnectionState(passed);
            CheckWorkbookContracts(passed);
            CheckEmptyForms(passed);
            CheckCompletionAndExportMetadata(passed);
        }

        private static void Require(bool value, string message)
        {
            if (!value) throw new InvalidOperationException(message);
        }

        private static T Find<T>(Control owner, string name) where T : Control
        {
            T result = owner.Controls.Find(name, true).OfType<T>().SingleOrDefault();
            if (result == null) throw new InvalidOperationException("Missing validation control: " + name);
            return result;
        }

        private static void Set(object owner, string field, object value)
        {
            FieldInfo info = owner.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            if (info == null) throw new InvalidOperationException("Missing validation state: " + field);
            info.SetValue(owner, value);
        }

        private static ConnectionPanel Connection(Control owner)
        {
            return Find<ConnectionPanel>(owner, "connection");
        }

        private static void Valid(ConnectionSide side)
        {
            side.Sql.Checked = true;
            side.WindowsLogin.Checked = true;
            side.DataSource.Text = "localhost";
            side.Database.Text = "SyntheticValidationOnly";
        }

        private static DataSet Workbook(params string[] columns)
        {
            var data = new DataSet();
            var table = data.Tables.Add("Table List");
            foreach (string column in columns) table.Columns.Add(column, typeof(string));
            table.Rows.Add(columns.Select(column => (object)(column == "ignore_columns" ? "" : "Example")).ToArray());
            return data;
        }

        private static void ExpectRejected(Form owner, Action action, string message)
        {
            Require(!FormValidation.Check(owner, action) && !string.IsNullOrWhiteSpace(FormValidation.LastMessage(owner)), message);
        }

        private static void CheckComparison(Action<string> passed)
        {
            using (var form = new DB_Comparison_Form())
            using (var data = new DataSet())
            {
                ConnectionPanel panel = Connection(form);
                Valid(panel.Source);
                panel.Target.DataSource.Text = string.Empty;
                Require(!form.ValidateTableLoadInputs(), "Database comparison accepted an incomplete Target after valid Source.");
                Require(FormValidation.LastMessage(form).IndexOf("Target", StringComparison.OrdinalIgnoreCase) >= 0,
                    "The missing Target field was not identified.");
                Valid(panel.Target);
                Find<ComboBox>(form, "cb_SRC_TableName").Text = "dbo.Example";
                Find<ComboBox>(form, "cb_TRG_TableName").Text = "dbo.Example";
                Require(form.ValidateTableLoadInputs(), "Valid database table-load fields were rejected.");
                Require(!form.ValidateComparisonInputs(false), "Comparison ran before both table snapshots were loaded.");
                foreach (string name in new[] { "Source", "Target" })
                {
                    DataTable table = data.Tables.Add(name);
                    table.Columns.Add("Id", typeof(int));
                    table.Columns.Add("Value", typeof(string));
                    // Empty tables with known schemas are valid comparison inputs.
                }
                Set(form, "ds", data);
                Find<CheckBox>(form, "cb_IncludeRule").Checked = false;
                Find<CheckedListBox>(form, "clb_ReferenceID").Items.Add("Id", true);
                Find<CheckedListBox>(form, "cbL_SRC_Cols").Items.Add("Value", true);
                Set(form, "comparisonInputsLoaded", true);
                Require(form.ValidateComparisonInputs(false), "Valid empty source/target schemas were rejected.");
                Require(!form.ValidateComparisonInputs(true), "Output ran without the Set/preparation step.");
                Set(form, "comparisonPrepared", true);
                Require(form.ValidateComparisonInputs(true), "A prepared, valid comparison was rejected.");
                panel.Target.Database.Text = "ChangedDatabase";
                Require(!form.ValidateComparisonInputs(true), "A changed target connection reused stale comparison data.");
            }
            passed("Database comparison validates both connections, actual loaded columns and Set state; empty schemas remain valid and edited inputs invalidate stale results.");
        }

        private static void CheckConnectionState(Action<string> passed)
        {
            using (var form = new SPI_FK_Extraction())
            {
                Valid(Connection(form).Source);
                Set(form, "boolSRCConnection", true);
                Set(form, "dbSRCConnection", new DBConnectionStatus { boolConnection = true });
                Require(form.ValidateExtractionInputs(), "A tested synthetic FK connection was rejected.");
                Connection(form).Source.Database.Text = "ChangedDatabase";
                Require(!form.ValidateExtractionInputs(), "FK extraction accepted a stale successful connection test.");
            }
            using (var form = new PreMergingTool_Form())
            {
                Valid(Connection(form).Source);
                Find<CheckBox>(form, "cb_Schema").Checked = false;
                Set(form, "boolConnection", true);
                Set(form, "strConnection", "synthetic-not-opened");
                Require(form.ValidatePreMergeInputs(), "A tested synthetic pre-merging connection was rejected.");
                Connection(form).Source.DataSource.Text = "ChangedServer";
                Require(!form.ValidatePreMergeInputs(), "Pre-merging accepted a stale connection string.");
            }
            using (var form = new SPIDeltaChangeLegacyID())
            {
                Valid(Connection(form).Source);
                Connection(form).Target.DataSource.Text = string.Empty;
                Require(form.ValidateLegacyConnectionInputs(), "A Source-only legacy-ID connection was incorrectly blocked by unused Target fields.");
                Set(form, "dBConnectionStatus_SRC", new DBConnectionStatus { boolConnection = true });
                Require(form.ValidateLegacyInputs(), "A tested SQL Server legacy-ID source was rejected.");
                Connection(form).Source.Database.Text = "ChangedDatabase";
                Require(!form.ValidateLegacyInputs(), "Legacy-ID mapping accepted a stale source connection.");
            }
            using (var form = new Table_Extraction_After_Delta_Form())
            using (var source = Workbook("ID"))
            {
                Connection(form).Source.DataSource.Text = string.Empty;
                Valid(Connection(form).Target);
                Require(form.ValidateDeltaConnectionInputs(false), "Excel-source delta mode required an unused Source database.");
                Require(!form.ValidateDeltaConnectionInputs(true), "Database-source delta mode accepted a missing Source database.");
                Set(form, "dBConnectionStatus_TRG", new DBConnectionStatus { boolConnection = true });
                Set(form, "dsExcel", source);
                var sheets = Find<ComboBox>(form, "cb_ExcelSheetName");
                sheets.Items.Add(source.Tables[0].TableName);
                sheets.SelectedIndex = 0;
                Set(form, "dtSRC", source.Tables[0]);
                Require(form.ValidateDeltaInputs(false), "Excel-source delta mode rejected a loaded worksheet and tested Target-only connection.");
            }
            passed("FK extraction, pre-merging and legacy-ID mapping invalidate cached connection status after edits without contacting a database.");
            passed("Legacy-ID Source-only and delta Excel-source Target-only modes validate only the database connections each operation actually uses.");
        }

        private static void CheckWorkbookContracts(Action<string> passed)
        {
            using (var empty = new DataSet())
            using (var batch = Workbook("src_table_names", "trg_table_names", "primary_key_columns", "ignore_columns"))
            using (var form = new Compare_Multiples_List_Tables_Forms())
            {
                ExpectRejected(form, () => form.ValidateBatchWorkbook(empty), "Empty batch workbook was accepted.");
                Require(FormValidation.Check(form, () => form.ValidateBatchWorkbook(batch)), "Valid batch worksheet schema was rejected.");
                batch.Tables[0].Rows[0]["trg_table_names"] = " ";
                ExpectRejected(form, () => form.ValidateBatchWorkbook(batch), "A blank target table was accepted in batch input.");
            }
            using (var form = new SPIDeltaChangeTrackerOptimised())
            using (var input = Workbook("src_table_names", "trg_table_names", "primary_key_columns", "ignore_columns"))
            {
                Require(FormValidation.Check(form, () => form.ValidateTrackerWorkbook(input)), "Valid delta tracker schema was rejected.");
                input.Tables[0].Columns.Remove("primary_key_columns");
                ExpectRejected(form, () => form.ValidateTrackerWorkbook(input), "Tracker accepted a missing reference-key column.");
            }
            using (var form = new SPI_FK_Extraction())
            using (var input = Workbook("table_name_fk", "table_fk_primary_key"))
            {
                Require(FormValidation.Check(form, () => form.ValidateExtractionWorkbook(input)), "Valid FK extraction schema was rejected.");
                input.Tables[0].Rows.Clear();
                ExpectRejected(form, () => form.ValidateExtractionWorkbook(input), "Empty FK extraction list was accepted.");
            }
            using (var form = new PreMergingTool_Form())
            using (var input = Workbook("TABLE_NAME", "DUPLICACY_COLUMNS"))
            {
                Require(FormValidation.Check(form, () => form.ValidatePreMergeWorkbook(input, true)), "Valid duplicate-check schema was rejected.");
                ExpectRejected(form, () => form.ValidatePreMergeWorkbook(input, false), "Duplicate-only schema was accepted for foreign-key pre-merging.");
            }
            using (var form = new Pre_Merging_Duplicacy_Form())
            using (var input = Workbook("TABLE_NAME", "PRIMARY_KEY", "DUPLICACY_COLUMNS"))
            {
                Find<CheckBox>(form, "cb_ConfigurationMode").Checked = false;
                Require(FormValidation.Check(form, () => form.ValidateDuplicacyWorkbook(input)), "Valid duplicacy schema was rejected.");
                input.Tables[0].Columns.Add("WHERE_CLAUSE");
                ExpectRejected(form, () => form.ValidateDuplicacyWorkbook(input), "An unpaired duplicacy filter was accepted.");
            }
            using (var form = new Table_Extraction_After_Delta_Form())
            using (var input = Workbook("src_table_name", "trg_table_name", "column_name", "table_id"))
            {
                input.Tables[0].Rows.Add("", "", "AnotherColumn", "2");
                Require(FormValidation.Check(form, () => form.ValidateDeltaWorkbook(input, "old")),
                    "Old-vs-new rows with first-row table names were incorrectly rejected.");
                ExpectRejected(form, () => form.ValidateDeltaWorkbook(input, "extract"), "Old-vs-new schema was accepted for extraction without primary_key.");
            }
            using (var form = new Table_Extraction_After_Delta_Form())
            using (var extraction = Workbook("src_table_name", "trg_table_name", "primary_key", "table_id"))
            using (var changes = Workbook("src_table_name", "trg_table_name", "src_column_name", "trg_column_name", "table_id"))
            {
                Require(FormValidation.Check(form, () => form.ValidateDeltaWorkbook(extraction, "extract")), "Valid extraction schema was rejected.");
                Require(FormValidation.Check(form, () => form.ValidateDeltaWorkbook(changes, "change")), "Valid change-number schema was rejected.");
                Require(FormValidation.Check(form, () => form.ValidateDeltaWorkbook(changes, "change-pair")), "Valid change-pair schema was rejected.");
                changes.Tables[0].Rows[0]["trg_column_name"] = " ";
                ExpectRejected(form, () => form.ValidateDeltaWorkbook(changes, "change"), "A change-number row without Target column was accepted.");
                ExpectRejected(form, () => form.ValidateDeltaWorkbook(changes, "change-pair"), "A change-pair row without Target column was accepted.");
            }
            using (var form = new SPIDeltaChangeLegacyID())
            using (var input = new DataSet())
            {
                DataTable source = input.Tables.Add("Source rows only");
                source.Columns.Add("Id");
                source.Rows.Add("1");
                DataTable mappings = input.Tables.Add("References_Source_Rows_Only");
                foreach (string name in new[] { "Field_to_be_taken", "Refrence_Field_Name_for_Mapping", "Reference_Table_Name", "Source_Rows_Field_Name" }) mappings.Columns.Add(name);
                mappings.Rows.Add("Id", "Id", "dbo.Example", "Id");
                Require(FormValidation.Check(form, () => form.ValidateLegacyWorkbook(input)), "Valid legacy-ID schema was rejected.");
                mappings.Rows.Add("Id", "Id", "dbo.Example", "Id");
                ExpectRejected(form, () => form.ValidateLegacyWorkbook(input), "Duplicate legacy-ID output mappings were accepted.");
            }
            passed("Batch, tracker, FK, pre-merging, duplicacy/configuration, all delta extraction modes and legacy-ID workbook schemas reject missing/empty/mismatched inputs before queries or output mutation.");
        }

        private static void CheckEmptyForms(Action<string> passed)
        {
            using (var form = new SQL_DB_To_Excel())
            {
                Connection(form).Source.DataSource.Text = " ";
                Require(!form.ValidateLoadInputs() && !form.ValidateExportInputs(), "Database export accepted missing connection or tables.");
                Valid(Connection(form).Source);
                Require(form.ValidateLoadInputs(), "Valid export connection fields were rejected.");
                Require(!form.ValidateExportInputs(), "Database export ran without a current loaded table list.");
            }
            using (var form = new Compare_Multiples_List_Tables_Forms())
                Require(!form.ValidateBatchInputs(), "Batch comparison accepted a missing input list/output folder.");
            using (var form = new SPIDeltaChangeTrackerOptimised())
                Require(!form.ValidateTrackerInputs(), "Tracker accepted missing connection/table-list state.");
            using (var form = new Pre_Merging_Duplicacy_Form())
                Require(!form.ValidateDuplicacyInputs(), "Duplicacy accepted untested database connections.");
            using (var form = new Table_Extraction_After_Delta_Form())
                Require(!form.ValidateDeltaInputs(), "Delta extraction accepted untested database connections.");
            passed("All nine database workflow forms expose offline action-boundary checks; empty/incomplete workflows are rejected with correctable nonmodal feedback.");
        }

        private static void CheckCompletionAndExportMetadata(Action<string> passed)
        {
            using (var form = new Pre_Merging_Duplicacy_Form())
            {
                Set(form, "boolPreMergingDuplicacyTest", true); // An earlier table succeeded.
                Set(form, "operationFailed", true); // A later table failed.
                MethodInfo completed = typeof(Pre_Merging_Duplicacy_Form).GetMethod("backgroundWorker1_RunWorkerCompleted",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                completed.Invoke(form, new object[] { null, new RunWorkerCompletedEventArgs(null, null, false) });
                string log = Find<RichTextBox>(form, "rtb_Log").Text;
                Require(log.Contains("finished with errors") && !log.Contains("PreMerging Completed"),
                    "A partial-failure pre-merging run was reported as all-success.");
            }
            using (var form = new Compare_Multiples_List_Tables_Forms())
            {
                Set(form, "boolCompare_List", true);
                Set(form, "operationFailed", true);
                MethodInfo completed = typeof(Compare_Multiples_List_Tables_Forms).GetMethod("bgw_List_RunWorkerCompleted",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                completed.Invoke(form, new object[] { null, new RunWorkerCompletedEventArgs(null, null, false) });
                string log = Find<RichTextBox>(form, "rtb_Log").Text;
                Require(log.Contains("finished with errors") && !log.Contains("Completed SuccessFully"),
                    "A partial-failure batch comparison run was reported as all-success.");
            }
            using (var form = new SPIDeltaChangeTrackerOptimised())
            {
                Set(form, "operationFailed", true);
                MethodInfo completed = typeof(SPIDeltaChangeTrackerOptimised).GetMethod("backgroundWorker2_RunWorkerCompleted",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                completed.Invoke(form, new object[] { null, new RunWorkerCompletedEventArgs(null, null, false) });
                string log = Find<RichTextBox>(form, "rtb_Log").Text;
                Require(log.Contains("finished with errors") && !log.Contains("Done and Opening"),
                    "A partial-failure tracker run was reported as all-success.");
            }
            string table;
            int rows;
            Require(SQL_DB_To_Excel.TryParseTableEntry("db.dbo.Table[Detail]  [ 12 ]", out table, out rows)
                && table == "db.dbo.Table[Detail]" && rows == 12, "Bracketed table text was truncated by the row-count parser.");
            Require(SQL_DB_To_Excel.TryParseTableEntry("db.dbo.Items  [ 0 ]", out table, out rows) && rows == 0,
                "A valid empty-table count was rejected.");
            foreach (string entry in new[] { "db.dbo.Items", "db.dbo.Items  [ -1 ]", "db.dbo.Items  [ unknown ]", "db.dbo.Items  [ ]", "db.dbo.Items  [  ]", "  [ 1 ]" })
                Require(!SQL_DB_To_Excel.TryParseTableEntry(entry, out table, out rows), "Malformed export metadata was accepted: " + entry);
            passed("Partial pre-merging/batch/tracker failures cannot report all-success; export table/count parsing preserves bracketed names and rejects malformed counts.");
        }
    }
}
