using System;
using System.Collections.Generic;
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

        internal static int Run(string outputFile)
        {
            var messages = new List<string>();
            string prefix = "SPI_Comparison_Checks_" + Guid.NewGuid().ToString("N");
            string sourceName = prefix + "_Source";
            string targetName = prefix + "_Target";
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
                }
                CreateFixture(sourceName, false);
                CreateFixture(targetName, true);
                using (var form = new DB_Comparison_Form())
                {
                    ConnectionPanel panel = ComparisonCorrectnessChecks.Control<ConnectionPanel>(form, "connection");
                    SetConnection(panel.Source, sourceName);
                    SetConnection(panel.Target, targetName);
                    ComparisonCorrectnessChecks.Control<ComboBox>(form, "cb_SRC_TableName").Text = "dbo.ComparisonFixture";
                    ComparisonCorrectnessChecks.Control<ComboBox>(form, "cb_TRG_TableName").Text = "dbo.ComparisonFixture";
                    ComparisonCorrectnessChecks.Require(form.ValidateTableLoadInputs(), "Synthetic SQL inputs failed connection validation.");
                    ComparisonCorrectnessChecks.Set(form, "ds", new DataSet());
                    ComparisonCorrectnessChecks.Invoke(form, "connect_Oracle_DB", " * ");
                    ComparisonCorrectnessChecks.Invoke(form, "connect_SQL_DB", " * ");
                    DataSet loaded = ComparisonCorrectnessChecks.Get<DataSet>(form, "ds");
                    ComparisonCorrectnessChecks.Require(ComparisonCorrectnessChecks.Get<bool>(form, "boolStatus")
                        && loaded.Tables.Count == 2, "Actual SQL Server source/target loading failed.");
                    DataTable source = loaded.Tables[0];
                    DataTable target = loaded.Tables[1];
                    ComparisonCorrectnessChecks.Require(source.Select("Id = 'changed'")[0]["Value"].ToString() == "before"
                        && target.Select("Id = 'changed'")[0]["Value"].ToString() == "after",
                        "Target SQL loading reused the source database or connection.");
                    messages.Add("PASS Actual Source and Target form loaders read distinct disposable SQL Server databases.");
                    string referenceReport = Path.Combine(folder, "reference-report.xlsx");
                    ComparisonCorrectnessChecks.RunDatabaseComparison(form, source, target, false, referenceReport);
                    VerifyWorkbook(referenceReport, 1, 2);
                    messages.Add("PASS SQL reference comparison and Excel export preserve equal/different, source-only/target-only, duplicate, null, empty and quoted keys.");

                    using (DataTable sourceUnique = UniqueFixture(source))
                    using (DataTable targetUnique = UniqueFixture(target))
                    {
                        string primaryReport = Path.Combine(folder, "primary-report.xlsx");
                        ComparisonCorrectnessChecks.RunDatabaseComparison(form, sourceUnique, targetUnique, true, primaryReport);
                        VerifyWorkbook(primaryReport, 1, 1);
                    }
                    messages.Add("PASS SQL primary-key comparison writes the expected real worksheet values and mismatch totals.");

                    // Use a generated nonexistent database on the dedicated instance to exercise a
                    // genuine provider failure without waiting for an unreachable external server.
                    panel.Target.Database.Text = prefix + "_Absent";
                    var failedLoad = new DataSet();
                    failedLoad.Tables.Add(source.Copy());
                    ComparisonCorrectnessChecks.Set(form, "ds", failedLoad);
                    ComparisonCorrectnessChecks.Set(form, "boolStatus", true);
                    ComparisonCorrectnessChecks.Invoke(form, "connect_SQL_DB", " * ");
                    ComparisonCorrectnessChecks.Require(!ComparisonCorrectnessChecks.Get<bool>(form, "boolStatus")
                        && failedLoad.Tables.Count == 1, "A failing target connection appeared to load source data successfully.");
                    failedLoad.Dispose();
                    messages.Add("PASS A real SQL target connection failure fails without reusing the source snapshot.");
                }
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
                try
                {
                    using (var master = new SqlConnection(Connection("master")))
                    {
                        master.Open();
                        DropDatabase(master, targetName);
                        DropDatabase(master, sourceName);
                    }
                    Directory.Delete(folder, true);
                    messages.Add("PASS Disposable SQL databases and temporary output were removed.");
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

        private static string Connection(string database)
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = Server, InitialCatalog = database, IntegratedSecurity = true,
                Pooling = false, ConnectTimeout = 10,
            }.ConnectionString;
        }

        private static void SetConnection(ConnectionSide side, string database)
        {
            side.Sql.Checked = true;
            side.WindowsLogin.Checked = true;
            side.DataSource.Text = Server;
            side.Database.Text = database;
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
                Execute(connection, "CREATE TABLE dbo.ComparisonFixture (Id nvarchar(60) NULL, Value nvarchar(80) NULL, Context nvarchar(80) NULL)");
                using (var command = new SqlCommand("INSERT INTO dbo.ComparisonFixture (Id, Value, Context) VALUES (@id, @value, N'same')", connection))
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
