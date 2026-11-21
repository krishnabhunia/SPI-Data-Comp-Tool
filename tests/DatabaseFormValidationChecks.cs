using System;
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
            passed("Batch comparison workbook schemas reject missing, empty and mismatched inputs before queries or output mutation.");
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
            passed("Database comparison, batch comparison and database export expose offline action-boundary checks; incomplete workflows are rejected with correctable nonmodal feedback.");
        }

        private static void CheckCompletionAndExportMetadata(Action<string> passed)
        {
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
            string table;
            int rows;
            Require(SQL_DB_To_Excel.TryParseTableEntry("db.dbo.Table[Detail]  [ 12 ]", out table, out rows)
                && table == "db.dbo.Table[Detail]" && rows == 12, "Bracketed table text was truncated by the row-count parser.");
            Require(SQL_DB_To_Excel.TryParseTableEntry("db.dbo.Items  [ 0 ]", out table, out rows) && rows == 0,
                "A valid empty-table count was rejected.");
            foreach (string entry in new[] { "db.dbo.Items", "db.dbo.Items  [ -1 ]", "db.dbo.Items  [ unknown ]", "db.dbo.Items  [ ]", "db.dbo.Items  [  ]", "  [ 1 ]" })
                Require(!SQL_DB_To_Excel.TryParseTableEntry(entry, out table, out rows), "Malformed export metadata was accepted: " + entry);
            passed("Partial batch failures cannot report all-success; export table/count parsing preserves bracketed names and rejects malformed counts.");
        }
    }
}
