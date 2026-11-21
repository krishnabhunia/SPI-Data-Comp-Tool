using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Explicit integration mode. Uses only a dedicated LocalDB instance prepared by CI;
    /// accepts no external connection string and never uses the user's MSSQLLocalDB instance.
    /// </summary>
    internal static class DatabaseIntegrationChecks
    {
        private const string Server = @"(localdb)\SPI_Comparison_Checks_CI";

        private const string FixtureName = "Order O'Brien Details] v2";
        private const string FixtureTable = "[dbo].[Order O'Brien Details]] v2]";

        internal static int Run(string outputFile)
        {
            var messages = new List<string>();
            string prefix = "SPI_Comparison_Checks_" + Guid.NewGuid().ToString("N");
            string sourceName = prefix + "_Source";
            string targetName = prefix + "_Target";
            string sourceLogin = prefix + "_SourceReader";
            string targetLogin = prefix + "_TargetReader";
            // These are ephemeral credentials on the disposable CI instance only. Never print
            // them or write them to profiles; punctuation also verifies literal credential handling.
            string sourcePassword = "SpiCi9!" + Guid.NewGuid().ToString("N") + ";'quoted'";
            string targetPassword = "SpiCi8!" + Guid.NewGuid().ToString("N") + ";'quoted'";
            string folder = Path.Combine(Path.GetTempPath(), prefix);
            Directory.CreateDirectory(folder);
            bool savedEpplus = Properties.Settings.Default.EPPlus_Excel_Save;
            bool savedClosedXml = Properties.Settings.Default.XL_Excel_Save;
            bool succeeded = false;
            try
            {
                Properties.Settings.Default.EPPlus_Excel_Save = true;
                Properties.Settings.Default.XL_Excel_Save = false;
                using (var master = new SqlConnection(Connection("master")))
                {
                    master.Open();
                    CreateDatabase(master, sourceName, folder);
                    CreateDatabase(master, targetName, folder);
                    CreateLogin(master, sourceLogin, sourcePassword, sourceName);
                    CreateLogin(master, targetLogin, targetPassword, targetName);
                }
                CreateFixture(sourceName, false);
                CreateFixture(targetName, true);
                GrantReadOnlyFixture(sourceName, sourceLogin);
                GrantReadOnlyFixture(targetName, targetLogin);

                foreach (bool sourceWindows in new[] { true, false })
                foreach (bool targetWindows in new[] { true, false })
                using (var form = new DB_Comparison_Form())
                {
                    string mode = (sourceWindows ? "Windows" : "SQL") + "-" + (targetWindows ? "Windows" : "SQL");
                    ConnectionPanel panel = ComparisonCorrectnessChecks.Control<ConnectionPanel>(form, "connection");
                    SetConnection(panel.Source, sourceName, sourceWindows, sourceLogin, sourcePassword);
                    SetConnection(panel.Target, targetName, targetWindows, targetLogin, targetPassword);
                    CheckMetadataLoader(form, true, sourceName);
                    CheckMetadataLoader(form, false, targetName);
                    // Exercise the real Connect/Load button, not a test-only database reader.
                    ComparisonCorrectnessChecks.Invoke(form, "btn_Connect_Reference_ID_Click", null, EventArgs.Empty);
                    using (DataSet loaded = ComparisonCorrectnessChecks.Get<DataSet>(form, "ds"))
                    {
                        ComparisonCorrectnessChecks.Require(ComparisonCorrectnessChecks.Get<bool>(form, "boolStatus")
                            && ComparisonCorrectnessChecks.Get<bool>(form, "comparisonInputsLoaded") && loaded.Tables.Count == 2,
                            "Actual SQL Server source/target loading failed for " + mode + ".");
                        DataTable source = loaded.Tables[0];
                        DataTable target = loaded.Tables[1];
                        ComparisonCorrectnessChecks.Require(source.Select("Id = 'changed'")[0]["Value"].ToString() == "before"
                            && target.Select("Id = 'changed'")[0]["Value"].ToString() == "after",
                            "Target SQL loading reused the source database or connection for " + mode + ".");
                        string referenceReport = Path.Combine(folder, "reference-report-" + mode + ".xlsx");
                        ComparisonCorrectnessChecks.RunDatabaseComparison(form, source, target, false, referenceReport);
                        VerifyWorkbook(referenceReport, 1, 2);
                        using (DataTable sourceUnique = UniqueFixture(source))
                        using (DataTable targetUnique = UniqueFixture(target))
                        {
                            string primaryReport = Path.Combine(folder, "primary-report-" + mode + ".xlsx");
                            ComparisonCorrectnessChecks.RunDatabaseComparison(form, sourceUnique, targetUnique, true, primaryReport);
                            VerifyWorkbook(primaryReport, 1, 1);
                        }
                    }
                    messages.Add("PASS " + mode + " Source/Target metadata, odd-named table reads, primary/reference comparisons and exact exported worksheet values.");
                }
                CheckRejectedConnections(sourceName, targetName, sourceLogin, sourcePassword, targetLogin, targetPassword, messages);
                CheckReadOnlyPermissions(sourceName, sourceLogin, sourcePassword);
                CheckReadOnlyPermissions(targetName, targetLogin, targetPassword);
                messages.Add("PASS Both generated SQL readers can read their fixture but cannot modify it or open the other database.");
                CheckDatabaseExport(sourceName, sourceLogin, sourcePassword, folder, messages);
                succeeded = true;
            }
            catch (Exception ex)
            {
                messages.Add("FAIL SQL Server integration: " + Common_Data.RedactConnectionCredentials(ex.ToString()));
            }
            finally
            {
                Properties.Settings.Default.EPPlus_Excel_Save = savedEpplus;
                Properties.Settings.Default.XL_Excel_Save = savedClosedXml;
                // Drop only resources bearing this run's freshly generated GUID. Clearing this
                // process's pools releases reader sessions before removing the synthetic databases.
                SqlConnection.ClearAllPools();
                try
                {
                    using (var master = new SqlConnection(Connection("master")))
                    {
                        master.Open();
                        DropDatabase(master, targetName);
                        DropDatabase(master, sourceName);
                        DropLogin(master, targetLogin);
                        DropLogin(master, sourceLogin);
                    }
                    Directory.Delete(folder, true);
                    messages.Add("PASS Disposable SQL databases, generated logins and temporary output were removed.");
                }
                catch (Exception ex)
                {
                    succeeded = false;
                    messages.Add("FAIL Disposable SQL cleanup: " + Common_Data.RedactConnectionCredentials(ex.ToString()));
                }
            }
            File.WriteAllLines(outputFile, messages);
            return succeeded ? 0 : 1;
        }

        private static void CheckMetadataLoader(DB_Comparison_Form form, bool source, string database)
        {
            ComparisonCorrectnessChecks.Invoke(form, source ? "btn_Load_SRC_Table_Click" : "btn_Load_TRG_Table_Click", null, EventArgs.Empty);
            ComboBox picker = ComparisonCorrectnessChecks.Control<ComboBox>(form, source ? "cb_SRC_TableName" : "cb_TRG_TableName");
            DataTable metadata = picker.DataSource as DataTable;
            ComparisonCorrectnessChecks.Require(metadata != null && metadata.Rows.Count == 1
                && metadata.Rows[0]["TABLE_CATALOG"].ToString() == database
                && metadata.Rows[0]["TABLE_NAME"].ToString() == FixtureName,
                "The actual metadata loader read another database or hid the readable fixture.");
            string name = "[" + database + "]." + FixtureTable;
            ComparisonCorrectnessChecks.Require(metadata.Rows[0]["CompleteName"].ToString() == name,
                "Metadata table names did not preserve spaces and closing brackets as identifiers.");
            picker.Text = name;
        }

        private static void CheckRejectedConnections(string sourceName, string targetName, string sourceLogin,
            string sourcePassword, string targetLogin, string targetPassword, List<string> messages)
        {
            foreach (bool sourceFailure in new[] { false, true })
            using (var form = new DB_Comparison_Form())
            {
                ConnectionPanel panel = ComparisonCorrectnessChecks.Control<ConnectionPanel>(form, "connection");
                SetConnection(panel.Source, sourceName, !sourceFailure, sourceLogin, sourcePassword);
                SetConnection(panel.Target, targetName, sourceFailure, targetLogin, targetPassword);
                ConnectionSide failedSide = sourceFailure ? panel.Source : panel.Target;
                string wrongPassword = "SpiCi7!" + Guid.NewGuid().ToString("N");
                failedSide.Password.Text = wrongPassword;
                ComparisonCorrectnessChecks.Control<ComboBox>(form, "cb_SRC_TableName").Text = FixtureTable;
                ComparisonCorrectnessChecks.Control<ComboBox>(form, "cb_TRG_TableName").Text = FixtureTable;
                ComparisonCorrectnessChecks.Invoke(form, "btn_Connect_Reference_ID_Click", null, EventArgs.Empty);
                ComparisonCorrectnessChecks.Require(!ComparisonCorrectnessChecks.Get<bool>(form, "boolStatus")
                    && !ComparisonCorrectnessChecks.Get<bool>(form, "comparisonInputsLoaded"),
                    "Rejected SQL credentials fell back to Windows authentication or the other side's connection.");
                Exception error = ComparisonCorrectnessChecks.Get<Exception>(form, "myException_BG");
                ComparisonCorrectnessChecks.Require(error is SqlException, "The wrong-password case did not reach the SQL provider.");
                foreach (string control in new[] { "lblStatus", "lblExpressionStatus" })
                {
                    string text = ComparisonCorrectnessChecks.Control<Label>(form, control).Text;
                    ComparisonCorrectnessChecks.Require(!text.Contains(wrongPassword) && !text.Contains(sourcePassword)
                        && !text.Contains(targetPassword), "A database failure displayed generated credentials.");
                }
            }
            foreach (string[] attempt in new[]
            {
                new[] { targetName, sourceLogin, sourcePassword },
                new[] { sourceName, targetLogin, targetPassword },
                new[] { sourceName + "_Absent", sourceLogin, sourcePassword },
            })
            {
                DBConnectionStatus result = new DBConnectionStatus().DatabaseConnection("sql", Server,
                    attempt[0], false, attempt[1], attempt[2], null);
                ComparisonCorrectnessChecks.Require(!result.boolConnection && result.exceptionConnection is SqlException,
                    "A SQL reader opened an inaccessible or nonexistent database.");
            }
            messages.Add("PASS Wrong passwords on either side, cross-database access and a missing database fail without source/Windows fallback or displayed credentials.");
        }

        private static void CheckReadOnlyPermissions(string database, string user, string password)
        {
            using (var connection = new SqlConnection(ConnectionStrings.Build("sql", Server, database, false, user, password, null)))
            {
                connection.Open();
                // A zero-row update still needs UPDATE permission and cannot alter any fixture row.
                bool denied = false;
                try { Execute(connection, "UPDATE " + FixtureTable + " SET Value = Value WHERE 1 = 0"); }
                catch (SqlException ex) { denied = ex.Number == 229; }
                ComparisonCorrectnessChecks.Require(denied, "The generated comparison reader unexpectedly has write permission.");
            }
        }

        private static void CheckDatabaseExport(string database, string user, string password, string folder, List<string> messages)
        {
            foreach (bool windowsAuthentication in new[] { true, false })
            using (var form = new SQL_DB_To_Excel())
            {
                string mode = windowsAuthentication ? "Windows" : "SQL";
                string outputFolder = Path.Combine(folder, "database-export-" + mode);
                Directory.CreateDirectory(outputFolder);
                ConnectionPanel panel = ComparisonCorrectnessChecks.Control<ConnectionPanel>(form, "connection");
                SetConnection(panel.Source, database, windowsAuthentication, user, password);
                ComparisonCorrectnessChecks.Require(form.ValidateLoadInputs(), "Database-to-Excel rejected valid fixture connection inputs.");
                var load = new DoWorkEventArgs(null);
                ComparisonCorrectnessChecks.Invoke(form, "backgroundWorker1_DoWork", null, load);
                Exception failure = ComparisonCorrectnessChecks.Get<Exception>(form, "myException");
                ComparisonCorrectnessChecks.Require(!load.Cancel && failure == null,
                    "Database-to-Excel could not count the odd-named fixture table: " + failure?.Message);
                ComparisonCorrectnessChecks.Invoke(form, "backgroundWorker1_RunWorkerCompleted", null,
                    new RunWorkerCompletedEventArgs(null, null, false));
                var tables = ComparisonCorrectnessChecks.Control<CheckedListBox>(form, "clb_TableList");
                string label = database + ".dbo." + FixtureName + "  [ 8 ]";
                ComparisonCorrectnessChecks.Require(tables.Items.Count == 1 && tables.Items[0].ToString() == label
                    && tables.CheckedItems.Count == 1, "The actual export table/count list lost or changed the fixture metadata.");
                ComparisonCorrectnessChecks.Control<LinkLabel>(form, "linkLabel1").Text = outputFolder;
                ComparisonCorrectnessChecks.Require(form.ValidateExportInputs(), "Database-to-Excel rejected its actual loaded table and output folder.");
                ComparisonCorrectnessChecks.Set(form, "exportEntries", new[] { label });
                ComparisonCorrectnessChecks.Set(form, "exportFolder", outputFolder);
                ComparisonCorrectnessChecks.Set(form, "exportSkipEmpty", false);
                var export = new DoWorkEventArgs(null);
                ComparisonCorrectnessChecks.Invoke(form, "backgroundWorker2_DoWork", null, export);
                failure = ComparisonCorrectnessChecks.Get<Exception>(form, "myException");
                string output = Path.Combine(outputFolder, label + ".xlsx");
                ComparisonCorrectnessChecks.Require(!export.Cancel && failure == null && File.Exists(output),
                    "Database-to-Excel could not export the odd-named table: " + failure?.Message);
                using (var book = new ExcelPackage(new FileInfo(output)))
                {
                    ExcelWorksheet sheet = book.Workbook.Worksheets.Single();
                    ComparisonCorrectnessChecks.Require(sheet.Dimension.End.Row == 9 && sheet.Dimension.End.Column == 3
                        && sheet.Cells[1, 1].Text == "Id" && sheet.Cells[1, 2].Text == "Value",
                        "Database-to-Excel wrote incorrect row counts or headers.");
                    int changed = Enumerable.Range(2, 8).Single(row => sheet.Cells[row, 1].Text == "changed");
                    ComparisonCorrectnessChecks.Require(sheet.Cells[changed, 2].Text == "before",
                        "Database-to-Excel changed the source value while exporting.");
                }
                messages.Add("PASS " + mode + " Database-to-Excel metadata/count and export workers preserve spaces, apostrophes and closing brackets; the real workbook contains the expected rows and values.");
            }
        }

        private static void CreateLogin(SqlConnection master, string login, string password, string database)
        {
            Execute(master, "CREATE LOGIN [" + login + "] WITH PASSWORD = '" + password.Replace("'", "''")
                + "', DEFAULT_DATABASE = [" + database + "], CHECK_POLICY = ON, CHECK_EXPIRATION = OFF");
        }

        private static void GrantReadOnlyFixture(string database, string login)
        {
            using (var connection = new SqlConnection(Connection(database)))
            {
                connection.Open();
                Execute(connection, "CREATE USER [" + login + "] FOR LOGIN [" + login + "]");
                Execute(connection, "GRANT SELECT ON OBJECT::" + FixtureTable + " TO [" + login + "]");
            }
        }

        private static void DropLogin(SqlConnection master, string login)
        {
            Execute(master, "IF SUSER_ID(N'" + login + "') IS NOT NULL DROP LOGIN [" + login + "]");
        }

        private static string Connection(string database)
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = Server, InitialCatalog = database, IntegratedSecurity = true,
                Pooling = false, ConnectTimeout = 10,
            }.ConnectionString;
        }

        private static void SetConnection(ConnectionSide side, string database, bool windowsAuthentication, string user, string password)
        {
            side.Sql.Checked = true;
            side.WindowsLogin.Checked = windowsAuthentication;
            side.DataSource.Text = Server;
            side.Database.Text = database;
            side.User.Text = user;
            side.Password.Text = password;
        }

        private static void CreateDatabase(SqlConnection connection, string name, string folder)
        {
            string data = Path.Combine(folder, name + ".mdf").Replace("'", "''");
            string log = Path.Combine(folder, name + ".ldf").Replace("'", "''");
            Execute(connection, "CREATE DATABASE [" + name + "] ON PRIMARY (NAME=N'" + name
                + "', FILENAME=N'" + data + "') LOG ON (NAME=N'" + name + "_log', FILENAME=N'" + log + "')");
        }

        private static void DropDatabase(SqlConnection connection, string name)
        {
            // name is always a locally generated prefix + GUID + Source/Target suffix.
            Execute(connection, "IF DB_ID(N'" + name + "') IS NOT NULL BEGIN ALTER DATABASE [" + name
                + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [" + name + "]; END");
        }

        private static void Execute(SqlConnection connection, string sql)
        {
            using (var command = new SqlCommand(sql, connection)) { command.CommandTimeout = 30; command.ExecuteNonQuery(); }
        }

        private static void CreateFixture(string database, bool target)
        {
            using (var connection = new SqlConnection(Connection(database)))
            {
                connection.Open();
                Execute(connection, "CREATE TABLE " + FixtureTable + " (Id nvarchar(60) NULL, Value nvarchar(80) NULL, Context nvarchar(80) NULL)");
                using (var command = new SqlCommand("INSERT INTO " + FixtureTable + " (Id, Value, Context) VALUES (@id, @value, N'same')", connection))
                {
                    command.Parameters.Add("@id", SqlDbType.NVarChar, 60);
                    command.Parameters.Add("@value", SqlDbType.NVarChar, 80);
                    object[][] rows = {
                        new object[] { "equal", "same" }, new object[] { "changed", target ? "after" : "before" },
                        new object[] { target ? "target-only" : "source-only", "unpaired" },
                        // Real SELECT order is unspecified. Identical duplicate payloads test
                        // multiplicity here; the offline snapshot suite tests occurrence order.
                        new object[] { "repeat", "repeated-value" }, new object[] { "repeat", "repeated-value" },
                        new object[] { DBNull.Value, "null-key" }, new object[] { "", "empty-key" },
                        new object[] { "O'Brien", DBNull.Value },
                    };
                    foreach (object[] row in rows)
                    {
                        command.Parameters["@id"].Value = row[0];
                        command.Parameters["@value"].Value = row[1];
                        command.ExecuteNonQuery();
                    }
                    if (target)
                    {
                        command.Parameters["@id"].Value = "repeat";
                        command.Parameters["@value"].Value = "repeated-value";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static DataTable UniqueFixture(DataTable table)
        {
            DataTable result = table.Clone();
            foreach (DataRow row in table.Select("Id IS NOT NULL AND Id <> 'repeat'")) result.ImportRow(row);
            return result;
        }

        private static void VerifyWorkbook(string file, int sourceOnly, int targetOnly)
        {
            using (var book = new ExcelPackage(new FileInfo(file)))
            {
                ComparisonCorrectnessChecks.Require(book.Workbook.Worksheets["Source Rows Only"].Dimension.End.Row == sourceOnly + 1
                    && book.Workbook.Worksheets["Target Rows Only"].Dimension.End.Row == targetOnly + 1,
                    "SQL comparison lost or duplicated unmatched rows in its actual workbook.");
                ExcelWorksheet mismatch = book.Workbook.Worksheets["Mismatch"];
                int valueColumn = Enumerable.Range(1, mismatch.Dimension.End.Column).Single(column => mismatch.Cells[1, column].Text == "Value");
                ComparisonCorrectnessChecks.Require(mismatch.Dimension.End.Row == 3
                    && mismatch.Cells[2, valueColumn].Text == "before" && mismatch.Cells[3, valueColumn].Text == "after",
                    "SQL report did not preserve exact source and target values.");
                ExcelWorksheet summary = book.Workbook.Worksheets["Summary"];
                int valueRow = Enumerable.Range(2, summary.Dimension.End.Row - 1).Single(row => summary.Cells[row, 1].Text == "Value");
                ComparisonCorrectnessChecks.Require(summary.Cells[valueRow, 2].GetValue<int>() == 1,
                    "SQL report mismatch count was incorrect.");
            }
        }
    }
}
