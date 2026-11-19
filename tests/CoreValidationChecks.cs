using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Synthetic local checks: no database connections, real preferences or user documents.</summary>
    internal static class CoreValidationChecks
    {
        internal static void Run(Action<string> passed)
        {
            CheckFeedbackAndFields(passed);
            CheckConnectionStrings(passed);
            CheckTablesAndWorkbook(passed);
            CheckPathsAndAtomicWrites(passed);
            CheckInputLock(passed);
            CheckWorkbookHeaders(passed);
            CheckSharedFormHelpers(passed);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void Invalid(Action action)
        {
            bool rejected = false;
            try { action(); }
            catch (ValidationFailure) { rejected = true; }
            Require(rejected, "An invalid input was accepted by a core validation gate.");
        }

        private static void InvalidArgument(Action action)
        {
            bool rejected = false;
            try { action(); }
            catch (ArgumentException) { rejected = true; }
            Require(rejected, "Invalid connection parameters were accepted.");
        }

        private static void CheckFeedbackAndFields(Action<string> passed)
        {
            using (var owner = new Form())
            using (var field = new TextBox { AccessibleDescription = "Original description" })
            using (var tabs = new TabControl())
            {
                var first = new TabPage("First");
                var second = new TabPage("Second");
                tabs.TabPages.Add(first);
                tabs.TabPages.Add(second);
                second.Controls.Add(field);
                owner.Controls.Add(tabs);
                string reported = null;
                Require(!FormValidation.Check(owner, () => FormValidation.Required(field, "name"), value => reported = value),
                    "Empty input passed a required-field gate.");
                Require(!string.IsNullOrWhiteSpace(reported) && FormValidation.LastMessage(owner) == reported
                    && field.AccessibleDescription == reported && tabs.SelectedTab == second,
                    "Inline validation did not report/reveal the failing field.");
                field.Text = "नाम বাংলা";
                Require(FormValidation.LastMessage(owner).Length == 0 && field.AccessibleDescription == "Original description",
                    "Editing input did not clear stale inline feedback or restore accessibility text.");
                Require(FormValidation.Check(owner, () => FormValidation.Required(field, "name")) && field.Text == "नाम বাংলা",
                    "Valid Unicode input was rejected or modified.");
                field.Text = " \t ";
                Require(!FormValidation.Check(owner, () => FormValidation.Required(field, "name")),
                    "An earlier successful check bypassed revalidation.");
                foreach (string value in new[] { "-1", "0", "65536", "2147483648", "+1521", "1.5", "١٥٢١", "１２３", "" })
                {
                    field.Text = value;
                    Invalid(() => FormValidation.Port(field, "port"));
                }
                foreach (int value in new[] { 1, 1521, 65535 })
                {
                    field.Text = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    Require(FormValidation.Port(field, "port") == value, "A valid port boundary was rejected.");
                }
                bool unexpectedPropagated = false;
                try { FormValidation.Check(owner, () => { throw new InvalidOperationException("Synthetic unexpected failure"); }); }
                catch (InvalidOperationException) { unexpectedPropagated = true; }
                Require(unexpectedPropagated, "Input validation swallowed an unexpected programming failure.");
                using (var choice = new ComboBox())
                {
                    choice.Items.Add("Valid");
                    choice.Text = "Typed but not selected";
                    Invalid(() => FormValidation.Selection(choice, "item"));
                    choice.SelectedIndex = 0;
                    FormValidation.Selection(choice, "item");
                }
                owner.Dispose();
                bool invoked = false;
                Require(!FormValidation.Check(owner, () => invoked = true) && !invoked,
                    "A disposed window performed validation work.");
            }
            passed("Required fields, Unicode input, port boundaries and list selection revalidate on every action; inline feedback reveals the field, clears on edits and never hides unexpected failures.");
        }

        private static void CheckConnectionStrings(Action<string> passed)
        {
            string marker = "  synthetic-" + Guid.NewGuid().ToString("N") + ";\"quoted\"='value'  ";
            string server = "synthetic-server;User ID=not-a-setting";
            string user = "synthetic-user;Initial Catalog=not-a-setting";
            var sql = new SqlConnectionStringBuilder(ConnectionStrings.Build("SQL", server, "synthetic-db", false, user, marker, null));
            Require(sql.DataSource == server && sql.UserID == user && sql.Password == marker && !sql.IntegratedSecurity
                && sql.InitialCatalog == "synthetic-db", "SQL connection literals changed meaning or lost password characters.");
            var oracle = new OracleConnectionStringBuilder(ConnectionStrings.Build("Oracle", "synthetic-host", "synthetic-service", false,
                "synthetic-user", marker, "65535"));
            Require(oracle.Password == marker && oracle.UserID == "synthetic-user" && oracle.DataSource.Contains("PORT=65535"),
                "Oracle connection literals changed meaning or lost password characters.");
            var integrated = new SqlConnectionStringBuilder(ConnectionStrings.Build("sql", "synthetic-host", "synthetic-db", true,
                "ignored\nuser", marker, "not-an-oracle-port"));
            Require(integrated.IntegratedSecurity && integrated.UserID.Length == 0 && integrated.Password.Length == 0,
                "Windows authentication used inactive username/password/port fields.");
            InvalidArgument(() => ConnectionStrings.Build("Other", "host", "db", true, null, null, null));
            InvalidArgument(() => ConnectionStrings.Build("SQL", " ", "db", true, null, null, null));
            InvalidArgument(() => ConnectionStrings.Build("SQL", "host", " ", true, null, null, null));
            InvalidArgument(() => ConnectionStrings.Build("SQL", "host", "db", false, null, marker, null));
            InvalidArgument(() => ConnectionStrings.Build("SQL", "host", "db", false, "user", string.Empty, null));
            InvalidArgument(() => ConnectionStrings.Build("Oracle", "host", "db", true, "user", marker, "1521"));
            InvalidArgument(() => ConnectionStrings.Build("Oracle", "host)(PORT=1)", "db", false, "user", marker, "1521"));
            foreach (string port in new[] { "0", "-1", "65536", "+1", "999999999999999999999", "١٥٢١" })
                InvalidArgument(() => ConnectionStrings.Build("Oracle", "host", "db", false, "user", marker, port));
            DBConnectionStatus invalid = new DBConnectionStatus().DatabaseConnection("invalid", null, null, false, null, null, null);
            Require(!invalid.boolConnection && invalid.exceptionConnection is ArgumentException,
                "The public database boundary did not reject invalid input before opening a provider.");
            passed("Provider builders preserve synthetic password spaces/quotes/semicolons as literal values; integrated authentication ignores inactive credentials and invalid parameters fail before database access.");
        }

        private static void CheckTablesAndWorkbook(Action<string> passed)
        {
            using (var table = new DataTable("Results"))
            {
                Invalid(() => FormValidation.Table(null, null, "input"));
                Invalid(() => FormValidation.Table(table, null, "input", false));
                table.Columns.Add("Identifier");
                table.Columns.Add("Value");
                FormValidation.Table(table, null, "input", false);
                Invalid(() => FormValidation.Table(table, null, "input"));
                table.Rows.Add("1", "value");
                FormValidation.Table(table, null, "input");
                FormValidation.Columns(table, new[] { "identifier", "VALUE" }, null, "matching");
                Invalid(() => FormValidation.Columns(table, null, null, "matching"));
                Invalid(() => FormValidation.Columns(table, new[] { "Identifier", "identifier" }, null, "matching"));
                Invalid(() => FormValidation.Columns(table, new[] { "Missing" }, null, "matching"));
                Invalid(() => FormValidation.Columns(table, new[] { " " }, null, "matching"));
                using (var workbook = new DataSet())
                {
                    Invalid(() => FormValidation.Workbook(workbook, null, "output"));
                    workbook.Tables.Add(table.Copy());
                    FormValidation.Workbook(workbook, null, "output");
                    workbook.Tables.Add(new DataTable("results"));
                    workbook.Tables[1].Columns.Add("ID");
                    Invalid(() => FormValidation.Workbook(workbook, null, "output"));
                }
            }
            foreach (string name in new[] { "", " ", "Bad/Name", "Bad[Name]", "Bad:Name", "Bad\u0001Name", "'Leading", "Trailing'", new string('x', 32) })
                Invalid(() => FormValidation.WorksheetName(name, null));
            foreach (string name in new[] { "Results", "नाम বাংলা", new string('x', 31) })
                Require(FormValidation.WorksheetName(name, null) == name, "A valid worksheet name changed.");
            foreach (string file in new[] { "", "[invalid] long output.xlsx", new string('x', 70) + ".xlsx", "''.xlsx" })
                FormValidation.WorksheetName(FormValidation.WorksheetNameFromFile(file), null);
            using (var wide = new DataTable("Wide"))
            {
                for (int column = 0; column < 16384; column++) wide.Columns.Add("C" + column);
                FormValidation.ExcelTable(wide, null, "wide table");
                wide.Columns.Add("Overflow");
                Invalid(() => FormValidation.ExcelTable(wide, null, "wide table"));
            }
            passed("Data/header selection rejects missing, ambiguous or duplicate columns; exports enforce Excel worksheet names, unique sheets and the 16,384-column boundary without allocating million-row fixtures.");
        }

        private static void CheckPathsAndAtomicWrites(Action<string> passed)
        {
            string directory = Path.Combine(Path.GetTempPath(), "spi-core-validation-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            string input = Path.Combine(directory, "synthetic.xlsx");
            string output = Path.Combine(directory, "output.xlsx");
            try
            {
                File.WriteAllText(input, "synthetic input");
                File.WriteAllText(output, "existing output");
                Require(FormValidation.ReadableFile(input, null, "workbook", ".xlsx") == input, "A readable path changed.");
                FormValidation.OutputFile(output, null, "report", ".xlsx");
                Require(File.ReadAllText(output) == "existing output", "Output preflight modified an existing file.");
                Invalid(() => FormValidation.ReadableFile(input, null, "workbook", ".json"));
                Invalid(() => FormValidation.ReadableFile(Path.Combine(directory, "missing.xlsx"), null, "workbook", ".xlsx"));
                string empty = Path.Combine(directory, "empty.xlsx");
                File.WriteAllText(empty, string.Empty);
                Invalid(() => FormValidation.ReadableFile(empty, null, "workbook", ".xlsx"));
                Invalid(() => FormValidation.OutputFile(Path.Combine(directory, "missing", "report.xlsx"), null, "report", ".xlsx"));
                Invalid(() => FormValidation.OutputFile(directory, null, "report"));
                Invalid(() => FormValidation.OutputFile(output + ".", null, "report"));
                Invalid(() => FormValidation.OutputFile(output + " ", null, "report"));
                Invalid(() => FormValidation.OutputFile(" " + output, null, "report"));
                Invalid(() => FormValidation.DifferentFiles(Path.Combine(directory, ".", "synthetic.xlsx"), input, null));
                Invalid(() => FormValidation.DifferentFiles(input.ToUpperInvariant(), input, null));
                FormValidation.DifferentFiles(output, input, null);
                foreach (string name in new[] { "../escape.xlsx", @"..\escape.xlsx", "CON.xlsx", "AUX", "NUL.txt", "COM1.xlsx", "LPT9.txt", "COM¹.txt", "LPT²", "CONIN$", "CONOUT$", "report.", "report ", "x:y.xlsx" })
                    Invalid(() => FormValidation.FileName(name, null, "output"));
                FormValidation.FileName("नाम report.xlsx", null, "output");
                using (new FileStream(input, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    Invalid(() => FormValidation.ReadableFile(input, null, "workbook", ".xlsx"));
                using (new FileStream(output, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    Invalid(() => FormValidation.OutputFile(output, null, "report", ".xlsx"));
                File.SetAttributes(output, FileAttributes.ReadOnly);
                Invalid(() => FormValidation.OutputFile(output, null, "report", ".xlsx"));
                File.SetAttributes(output, FileAttributes.Normal);
                IoFailure(() => FormValidation.WriteAtomically(output, temporary =>
                {
                    File.WriteAllText(temporary, "partial output");
                    throw new IOException("Synthetic writer failure");
                }));
                Require(File.ReadAllText(output) == "existing output", "A failed write damaged the existing destination.");
                IoFailure(() => FormValidation.WriteAtomically(output, temporary => { }));
                Require(File.ReadAllText(output) == "existing output", "An empty writer replaced the destination.");
                string unpublished = Path.Combine(directory, "unpublished.xlsx");
                IoFailure(() => FormValidation.WriteAtomically(unpublished, temporary => { throw new IOException("Synthetic failure"); }));
                Require(!File.Exists(unpublished), "A failed write published a new output file.");
                string collision = Path.Combine(directory, "collision.xlsx");
                IoFailure(() => FormValidation.WriteAtomically(collision, temporary =>
                {
                    File.WriteAllText(temporary, "our candidate");
                    File.WriteAllText(collision, "other writer won");
                }));
                Require(File.ReadAllText(collision) == "other writer won", "A new output overwrote a concurrent writer's file.");
                FormValidation.WriteAtomically(output, temporary => File.WriteAllText(temporary, "complete output"));
                Require(File.ReadAllText(output) == "complete output", "A successful atomic replacement was not published.");
                FormValidation.WriteAtomically(unpublished, temporary => File.WriteAllText(temporary, "new complete output"));
                Require(File.ReadAllText(unpublished) == "new complete output", "A successful new output was not published.");
                Require(Directory.GetFiles(directory, ".spi-*").Length == 0, "Owned temporary output files were not cleaned up.");
                Require(File.ReadAllText(input) == "synthetic input", "Path/output validation changed an input file.");
            }
            finally
            {
                if (File.Exists(output)) File.SetAttributes(output, FileAttributes.Normal);
                Directory.Delete(directory, true);
            }
            passed("File gates reject missing, empty, locked, read-only, reserved and redirected paths; self-overwrite aliases are blocked, failed/empty atomic writes preserve files, and new-output races do not overwrite another writer.");
        }

        private static void IoFailure(Action action)
        {
            bool rejected = false;
            try { action(); }
            catch (IOException) { rejected = true; }
            Require(rejected, "A failed/empty/colliding writer unexpectedly published an output.");
        }

        private static void CheckInputLock(Action<string> passed)
        {
            using (var owner = new Form())
            {
                var input = new TextBox();
                var unavailable = new TextBox { Enabled = false };
                var log = new RichTextBox { ReadOnly = true };
                var start = new Button();
                var cancel = new Button();
                var selection = new ComboBox();
                var disabledGroup = new Panel { Enabled = false };
                var inherited = new TextBox();
                disabledGroup.Controls.Add(inherited);
                owner.Controls.AddRange(new Control[] { input, unavailable, log, start, cancel, selection, disabledGroup });
                var gate = FormValidation.LockInputs(owner, cancel);
                Require(!input.Enabled && !start.Enabled && !selection.Enabled && cancel.Enabled && log.Enabled && !unavailable.Enabled,
                    "The input lock disabled cancellation/read-only output or left inputs editable.");
                gate.Dispose();
                gate.Dispose();
                disabledGroup.Enabled = true;
                Require(input.Enabled && start.Enabled && selection.Enabled && !unavailable.Enabled && inherited.Enabled,
                    "The input lock did not restore explicit/inherited enabled states.");
                using (FormValidation.LockInputs(owner, cancel)) input.Dispose();
            }
            using (var worker = new BackgroundWorker()) FormValidation.IsBusy(worker, null);
            passed("Worker input locking preserves cancellation and read-only logs, restores prior explicit/inherited enabled states, and tolerates disposed controls and repeated cleanup.");
        }

        private static DataSet RawWorkbook(object[] headers, params object[][] rows)
        {
            var data = new DataSet();
            var table = data.Tables.Add("Synthetic");
            for (int column = 0; column < headers.Length; column++) table.Columns.Add("Column" + column, typeof(object));
            table.Rows.Add(headers);
            foreach (object[] row in rows) table.Rows.Add(row);
            return data;
        }

        private static void InvalidHeaders(DataSet data)
        {
            bool rejected = false;
            try { ReadExcel.NormalizeWorkbookHeaders(data); }
            catch (InvalidDataException) { rejected = true; }
            Require(rejected, "An ambiguous or missing workbook header was accepted.");
        }

        private static void CheckWorkbookHeaders(Action<string> passed)
        {
            using (var data = RawWorkbook(new object[] { " Column1 ", "Column0", " ", "नाम" },
                new object[] { " keep spaces ", 123, DBNull.Value, DBNull.Value }))
            {
                ReadExcel.NormalizeWorkbookHeaders(data);
                DataTable table = data.Tables[0];
                Require(table.Columns.Count == 3 && table.Columns[0].ColumnName == "COLUMN1" && table.Columns[1].ColumnName == "COLUMN0"
                    && table.Columns[2].ColumnName == "नाम", "Header normalization collided with reader-generated Column names or lost Unicode.");
                Require(table.Rows.Count == 1 && (string)table.Rows[0][0] == " keep spaces " && (int)table.Rows[0][1] == 123
                    && table.Rows[0][2] == DBNull.Value, "Header normalization changed data values or dropped the first data row.");
            }
            using (var duplicate = RawWorkbook(new object[] { "Name", " name " }, new object[] { "a", "b" })) InvalidHeaders(duplicate);
            using (var blank = RawWorkbook(new object[] { "ID", " " }, new object[] { 1, "valuable data" })) InvalidHeaders(blank);
            using (var empty = new DataSet()) InvalidHeaders(empty);
            using (var allBlank = RawWorkbook(new object[] { DBNull.Value, " " })) InvalidHeaders(allBlank);
            using (var onlyHeaders = RawWorkbook(new object[] { "ID", "Value" }))
            {
                ReadExcel.NormalizeWorkbookHeaders(onlyHeaders);
                Require(onlyHeaders.Tables[0].Rows.Count == 0 && onlyHeaders.Tables[0].Columns.Count == 2,
                    "A valid headers-only workbook was incorrectly treated as unreadable.");
            }
            using (var withBlankSheet = RawWorkbook(new object[] { "ID" }, new object[] { 7 }))
            {
                withBlankSheet.Tables.Add(new DataTable("Empty"));
                ReadExcel.NormalizeWorkbookHeaders(withBlankSheet);
                Require(withBlankSheet.Tables.Count == 1 && (int)withBlankSheet.Tables[0].Rows[0][0] == 7,
                    "An empty worksheet removed valid data from another sheet.");
            }
            passed("Workbook headers reject case/space duplicates and data under blank headers; unused spacers, empty sheets and Column0/Column1 collisions are handled without changing typed/null data or rejecting header-only tables.");
        }

        private static void CheckSharedFormHelpers(Action<string> passed)
        {
            Require(Common_Data.ProgressPercentage(0, 0) == 0 && Common_Data.ProgressPercentage(1, 0) == 0
                && Common_Data.ProgressPercentage(-1, 10) == 0 && Common_Data.ProgressPercentage(10, -1) == 0
                && Common_Data.ProgressPercentage(1, 2) == 50 && Common_Data.ProgressPercentage(10, 10) == 100
                && Common_Data.ProgressPercentage(int.MaxValue, int.MaxValue) == 100
                && Common_Data.ProgressPercentage(int.MaxValue, 1) == 100,
                "Progress arithmetic failed on zero, negative, complete or overflow-size counts.");
            using (var progress = new ProgressBar { Minimum = 20, Maximum = 40 })
            {
                Common_Data.UpdateProgressBar(progress, 0, 0);
                Require(progress.Value == 20, "Empty progress did not map to the control minimum.");
                Common_Data.UpdateProgressBar(progress, 1, 2);
                Require(progress.Value == 30, "Half progress did not respect the control's range.");
                Common_Data.UpdateProgressBar(progress, int.MaxValue, 1);
                Require(progress.Value == 40, "Overflow-size progress exceeded the control maximum.");
            }
            InvalidArgument(() => FormHelpers.Transpose(null));
            using (var summary = new DataTable())
            {
                InvalidArgument(() => FormHelpers.Transpose(summary));
                summary.Columns.Add("Metric");
                summary.Columns.Add("Value");
                InvalidArgument(() => FormHelpers.Transpose(summary));
                summary.Rows.Add("Total", "2");
                using (DataTable transposed = FormHelpers.Transpose(summary))
                    Require(transposed.Columns.Count == 2 && transposed.Rows.Count == 1
                        && transposed.Columns[0].ColumnName == "Column Names" && transposed.Columns[1].ColumnName == "Summarize Count"
                        && transposed.Rows[0][1].ToString() == "2", "A valid summary transpose changed its result.");
                summary.Rows[0][0] = "Column Names";
                using (DataTable transposed = FormHelpers.Transpose(summary))
                    Require(transposed.Columns[1].ColumnName == "Summarize Count" && transposed.Rows[0][1].ToString() == "2",
                        "The first summary row collided with the final fixed output header.");
                summary.Rows[0][0] = "Total";
                summary.Rows.Add("total", "3");
                InvalidArgument(() => FormHelpers.Transpose(summary));
                summary.Rows[1][0] = " ";
                InvalidArgument(() => FormHelpers.Transpose(summary));
                summary.Rows[1][0] = "Column Names";
                InvalidArgument(() => FormHelpers.Transpose(summary));
                summary.Rows[1][0] = "Summarize Count";
                InvalidArgument(() => FormHelpers.Transpose(summary));
            }
            using (var choice = new ComboBox())
            using (var metadata = new DataTable())
            {
                object original = new[] { "preserved choice" };
                choice.DataSource = original;
                InvalidArgument(() => FormHelpers.FillTableNames(choice, null, "sql"));
                InvalidArgument(() => FormHelpers.FillTableNames(choice, metadata, "other"));
                InvalidArgument(() => FormHelpers.FillTableNames(choice, metadata, "sql"));
                Require(ReferenceEquals(choice.DataSource, original), "Invalid metadata erased existing table choices.");
                foreach (string column in new[] { "TABLE_CATALOG", "TABLE_SCHEMA", "TABLE_NAME" }) metadata.Columns.Add(column);
                Require(FormHelpers.FillTableNames(choice, metadata, "sql") == null, "An empty valid database table list returned a stale table.");
                metadata.Rows.Add("synthetic-db", "dbo", "Items");
                Require(FormHelpers.FillTableNames(choice, metadata, "sql") == "synthetic-db.dbo.Items"
                    && metadata.Rows[0]["CompleteName"].ToString() == "synthetic-db.dbo.Items", "Valid SQL metadata produced an incorrect table choice.");
            }
            passed("Shared helpers handle empty/large progress safely and reject missing schemas or invalid summary names without erasing existing choices; valid summaries and empty metadata remain usable.");
        }
    }
}
