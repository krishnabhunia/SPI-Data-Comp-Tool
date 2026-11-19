using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Pure/synthetic connection, profile and rules checks. Never connects to a database.</summary>
    internal static class ConnectionValidationChecks
    {
        internal static void Run(Action<string> passed)
        {
            CheckConnectionInputs(passed);
            CheckProfileDocuments(passed);
            CheckRuleRows(passed);
            CheckAnalysisInputs(passed);
            CheckSharedConnectionMigrations(passed);
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
            Require(rejected, "Invalid connection input was accepted.");
        }

        private static T Find<T>(Control root, string name) where T : Control
        {
            return root.Controls.Find(name, true).OfType<T>().Single();
        }

        private static void CheckConnectionInputs(Action<string> passed)
        {
            List<DBC> previous = Profile.ProfileList;
            try
            {
                Profile.ProfileList = new List<DBC>();
                using (var panel = new ConnectionPanel())
                {
                    Invalid(() => panel.Source.ValidateInput());
                    panel.Source.DataSource.Text = "synthetic-server";
                    panel.Source.Database.Text = "synthetic-service";
                    panel.Source.User.Text = "synthetic-user";
                    string passwordMarker = "  synthetic-" + Guid.NewGuid().ToString("N") + ";quoted='value'  ";
                    panel.Source.Password.Text = passwordMarker;
                    panel.Source.ValidateInput();
                    panel.Source.DefaultPort.Checked = false;
                    foreach (string value in new[] { "", " ", "0", "65536", "-1", "+1", "1.5", "port", "9999999999999999999" })
                    {
                        panel.Source.Port.Text = value;
                        Invalid(() => panel.Source.ValidateInput());
                    }
                    foreach (string value in new[] { "1", "1521", "65535" })
                    {
                        panel.Source.Port.Text = value;
                        panel.Source.ValidateInput();
                    }
                    Require(panel.Source.Password.Text == passwordMarker, "Validation modified a password.");
                    panel.Source.DataSource.Text = "host)(DESCRIPTION=";
                    Invalid(() => panel.Source.ValidateInput());
                    panel.Source.DataSource.Text = "synthetic-server";
                    panel.Source.Sql.Checked = true;
                    panel.Source.WindowsLogin.Checked = true;
                    panel.Source.User.Clear();
                    panel.Source.Password.Clear();
                    panel.Source.Port.Text = "not-an-oracle-port";
                    panel.Source.ValidateInput();
                    panel.Mode = ConnectionMode.SourceOnly;
                    panel.ValidateInput();
                    panel.Mode = ConnectionMode.SourceAndTarget;
                    Invalid(() => panel.ValidateInput());
                    panel.Source.WindowsLogin.Checked = false;
                    Invalid(() => panel.Source.ValidateInput());
                    panel.Source.ValidateInput(false); // A partial profile is not a live connection.
                    panel.Source.Sql.Checked = false;
                    Invalid(() => panel.Source.ValidateInput(false));
                    panel.Source.Sql.Checked = true;
                    int changes = 0;
                    panel.InputChanged += (sender, e) => changes++;
                    panel.Source.Database.Text = "edited-database";
                    Require(changes > 0, "Connection edits did not invalidate loaded/tested state.");
                    int beforeRefresh = changes;
                    panel.RefreshAvailableProfiles();
                    Require(changes == beforeRefresh, "Refreshing profile names falsely invalidated entered data.");
                }
            }
            finally { Profile.ProfileList = previous; }
            passed("Connection fields reject incomplete platforms/credentials and pasted invalid ports; integrated login and partial profiles remain valid, passwords stay exact, and edits invalidate tested state.");
        }

        private static void CheckProfileDocuments(Action<string> passed)
        {
            foreach (string source in new[]
            {
                "{\"bool_Rb_Oracle\":true,\"bool_Rb_SQL\":true}",
                "{\"bool_Rb_Oracle\":true,\"bool_SQL_WA\":true}",
                "{\"bool_Rb_Oracle\":true,\"str_DBType\":\"SQL\"}",
                "{\"str_DBType\":\"unknown\"}",
                "{\"int_Oracle_DefaultPort\":-1}",
                "{\"int_Oracle_DefaultPort\":65536}",
                "{\"int_Oracle_DefaultPort\":2147483648}",
                "{\"int_Oracle_DefaultPort\":9223372036854775808}",
                "{\"int_Oracle_DefaultPort\":99999999999999999999}",
                "{\"int_Oracle_DefaultPort\":-99999999999999999999}",
                "{\"int_Oracle_DefaultPort\":\"1521\"}",
                "{\"bool_Rb_SQL\":\"true\"}",
                "{\"str_DataSource\":12}",
                "{\"str_DataSource\":\"server\\nname\"}",
                "{\"str_DataSource\":\"a\",\"str_datasource\":\"b\"}",
            })
                InvalidProfile("[{\"profileName\":\"Synthetic\",\"dbSRC\":" + source + ",\"dbTRG\":{}}]");
            InvalidProfile("[{\"profileName\":\"a\",\"profileName\":\"b\",\"dbSRC\":{},\"dbTRG\":{}}]");
            InvalidProfile("[{\"profileName\":\"a\",\"ProfileName\":\"b\",\"dbSRC\":{},\"dbTRG\":{}}]");
            InvalidProfile("[{\"profileName\":123,\"dbSRC\":{},\"dbTRG\":{}}]");
            InvalidProfile("[] []");
            InvalidProfile(" ");
            Require(ProfileStore.ParseJson("[{\"profileName\":\"Partial\",\"dbSRC\":{},\"dbTRG\":{}}]").Count == 1,
                "A legacy partial profile was rejected.");
            Require(ProfileStore.ParseJson("[{\"profileName\":\"नाम\",\"dbSRC\":{\"bool_Rb_Oracle\":true,\"int_Oracle_DefaultPort\":0},\"dbTRG\":{}}]").Count == 1,
                "Unicode names or the legacy default-port sentinel were rejected.");
            foreach (string value in new[] { "0", "1", "1521", "65535" })
            {
                string side = "{\"int_Oracle_DefaultPort\":" + value + "}";
                List<DBC> imported = ProfileStore.ParseJson("[{\"profileName\":\"Boundary\",\"dbSRC\":" + side + ",\"dbTRG\":" + side + "}]");
                Require(imported[0].dbSRC.int_Oracle_DefaultPort.ToString(System.Globalization.CultureInfo.InvariantCulture) == value
                    && imported[0].dbTRG.int_Oracle_DefaultPort.ToString(System.Globalization.CultureInfo.InvariantCulture) == value,
                    "A valid saved port was changed during import.");
            }
            InvalidProfile("[{\"profileName\":\"Target overflow\",\"dbSRC\":{},\"dbTRG\":{\"int_Oracle_DefaultPort\":99999999999999999999}}]");
            CheckProfileExportPorts();
            passed("Profile imports reject malformed/ambiguous JSON, wrong field types, contradictory platforms and invalid ports while preserving Unicode names, partial templates and legacy default ports.");
        }

        private static void CheckProfileExportPorts()
        {
            List<DBC> previous = Profile.ProfileList;
            try
            {
                Profile.ProfileList = new List<DBC>();
                using (var form = new UserProfileForm())
                {
                    var panel = Find<ConnectionPanel>(form, "connection");
                    MethodInfo capture = typeof(UserProfileForm).GetMethod("GetDBConnectionDetailsProfile",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                    panel.Source.Sql.Checked = panel.Target.Sql.Checked = true;
                    panel.Source.Port.Text = "999999999999999999999";
                    panel.Target.Port.Text = "inactive-port";
                    panel.ValidateInput(true, true, false);
                    var source = (DBConnectionDetailsProfile)capture.Invoke(form, new object[] { "SRC" });
                    var target = (DBConnectionDetailsProfile)capture.Invoke(form, new object[] { "TRG" });
                    Require(source.int_Oracle_DefaultPort == 0 && target.int_Oracle_DefaultPort == 0,
                        "SQL profile capture parsed an inactive Oracle port instead of storing its unset sentinel.");
                    Require(panel.Source.Port.Text == "999999999999999999999" && panel.Target.Port.Text == "inactive-port",
                        "Profile capture changed inactive input text.");
                    panel.Source.Oracle.Checked = true;
                    panel.Source.DefaultPort.Checked = false;
                    panel.Source.Port.Text = "65535";
                    source = (DBConnectionDetailsProfile)capture.Invoke(form, new object[] { "SRC" });
                    Require(source.int_Oracle_DefaultPort == 65535, "Profile capture changed a valid active Oracle port.");
                    panel.Source.Port.Text = "999999999999999999999";
                    bool rejected = false;
                    try { capture.Invoke(form, new object[] { "SRC" }); }
                    catch (TargetInvocationException ex) { rejected = ex.InnerException is ValidationFailure; }
                    Require(rejected, "Profile capture accepted an invalid active Oracle port.");
                }
            }
            finally { Profile.ProfileList = previous; }
        }

        private static void InvalidProfile(string json)
        {
            bool rejected = false;
            try { ProfileStore.ParseJson(json); }
            catch (InvalidDataException) { rejected = true; }
            Require(rejected, "An invalid profile document was accepted.");
        }

        private static void CheckRuleRows(Action<string> passed)
        {
            using (var data = new DataSet())
            {
                var table = data.Tables.Add("Rules");
                foreach (string column in new[] { "srno", "Source", "Target", "RuleType", "Source_ColName", "Target_Colname" })
                    table.Columns.Add(column);
                table.Rows.Add("1", "", "", "1", "", "");
                User_Settings.ValidateRulesCandidate(data); // Empty values are legitimate equality rules.
                table.Rows[0]["RuleType"] = "unknown";
                InvalidRules(data);
                table.Rows[0]["RuleType"] = "2";
                InvalidRules(data);
                table.Rows[0]["Source_ColName"] = "SOURCE";
                table.Rows[0]["Target_Colname"] = "TARGET";
                User_Settings.ValidateRulesCandidate(data);
                table.Rows.Add("1", "a", "b", "1", "", "");
                InvalidRules(data);
                table.Rows.RemoveAt(1);
                table.Rows[0]["srno"] = " ";
                InvalidRules(data);
            }
            passed("Comparison rules reject unsupported types, missing scoped columns and blank/duplicate identifiers before replacing saved rules; blank comparison values remain supported.");
        }

        private static void InvalidRules(DataSet data)
        {
            bool rejected = false;
            try { User_Settings.ValidateRulesCandidate(data); }
            catch (InvalidOperationException) { rejected = true; }
            Require(rejected, "Invalid comparison rule content was accepted.");
        }

        private static void CheckAnalysisInputs(Action<string> passed)
        {
            foreach (string value in new[] { "dbo.Table_1", "[database].[schema].[table name]", "dbo.[name.with.dot]", "[name]]escaped]", "\"name\"\"escaped\"", "Table$1" })
                Require(OracleAndSQLConnection.ValidateTableName(value, false) == value, "A valid table identifier was changed.");
            foreach (string value in new[] { "SCHEMA.TABLE", "\"Schema\".\"Table with space\"" })
                OracleAndSQLConnection.ValidateTableName(value, true);
            foreach (string value in new[] { "", " ", "dbo..table", "dbo.", "table;DELETE FROM x", "table -- comment", "table/*comment*/", "[unclosed", "[]", "[table]suffix", "a.b.c.d.e" })
            {
                bool rejected = false;
                try { OracleAndSQLConnection.ValidateTableName(value, false); }
                catch (ArgumentException) { rejected = true; }
                Require(rejected, "Invalid table-name syntax was accepted.");
            }
            using (var control = new OracleAndSQLConnection())
            {
                control.Load_DB_Oracle_SQL(); // Empty inputs must stop before either data adapter runs.
                Require(!control.HasLoadedData && FormValidation.LastMessage(control).Length > 0,
                    "Analysis load did not report missing connection input.");
                foreach (string name in new[] { "tb_oracle_dsource", "tb_oracle_dbName", "tb_oracle_username", "tb_oracle_pwd",
                    "tb_sql_dsource", "tb_sql_dbName", "tb_sql_username", "tb_sql_pwd" }) Find<TextBox>(control, name).Text = "synthetic";
                Find<TextBox>(control, "tb_Source_tableName").Text = "SCHEMA.TABLE";
                Find<TextBox>(control, "tb_destination_tableName").Text = "dbo.Target";
                control.ValidateInput(); // Validation-only, never call Load with usable credentials.
                var query = Find<CheckBox>(control, "cb_query");
                query.Checked = true;
                Find<TextBox>(control, "tb_querybox").Text = " ";
                Invalid(control.ValidateInput);
                Find<TextBox>(control, "tb_querybox").Text = "SELECT 1";
                control.ValidateInput();
                DataSet snapshot = control.FetchDataSet();
                snapshot.Tables.Add("Synthetic source");
                snapshot.Tables.Add("Synthetic target");
                typeof(OracleAndSQLConnection).GetField("boolStatus", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(control, true);
                Require(control.HasLoadedData, "Synthetic loaded state was not established.");
                Find<TextBox>(control, "tb_oracle_dsource").Clear();
                Require(!control.HasLoadedData, "Edited Analysis credentials left stale results marked current.");
                control.Load_DB_Oracle_SQL();
                Require(ReferenceEquals(snapshot, control.FetchDataSet()) && snapshot.Tables.Count == 2,
                    "Rejected Analysis input destroyed the previous loaded snapshot.");
            }
            passed("Analysis validates both connections and query/table modes offline, rejects malformed table names, and invalidates edited input without destroying the previous snapshot.");
        }

        private static void CheckSharedConnectionMigrations(Action<string> passed)
        {
            List<DBC> previous = Profile.ProfileList;
            try
            {
                Profile.ProfileList = new List<DBC>();
                using (var control = new OracleAndSQLConnection())
                {
                    ConnectionPanel panel = Find<ConnectionPanel>(control, "connection");
                    Require(panel.Target.SqlServerOnly && panel.Target.Sql.Checked && !panel.Target.Oracle.Enabled,
                        "Analysis lost its SQL Server-only Target restriction.");
                    string profilePassword = "  synthetic;'profile'  ";
                    panel.Target.Apply(true, false, false, true, 0,
                        " synthetic-host ", " synthetic-service ", " synthetic-user ", profilePassword);
                    Require(panel.Target.Sql.Checked && !panel.Target.Oracle.Checked && !panel.Target.Oracle.Enabled
                        && !panel.Target.DefaultPort.Checked && !panel.Target.DefaultPort.Enabled
                        && !panel.Target.Port.Enabled && panel.Target.Port.ReadOnly && panel.Target.Port.Text.Length == 0,
                        "Applying an Oracle profile enabled Oracle-only fields on the SQL Server-only Target.");
                    Require(panel.Target.DataSource.Text == " synthetic-host "
                        && panel.Target.Database.Text == " synthetic-service "
                        && panel.Target.User.Text == " synthetic-user " && panel.Target.Password.Text == profilePassword,
                        "Applying a profile changed literal connection field values or password whitespace.");
                    var sourceTable = Find<TextBox>(control, "tb_Source_tableName");
                    var targetTable = Find<TextBox>(control, "tb_destination_tableName");
                    var query = Find<TextBox>(control, "tb_querybox");
                    var useQuery = Find<CheckBox>(control, "cb_query");
                    string initialQuery = query.Text;
                    sourceTable.Text = "SOURCE.SPEC_FORM";
                    targetTable.Text = "dbo.SPEC_FORM";
                    useQuery.Checked = true;
                    query.Text = "SELECT 'synthetic draft'";
                    useQuery.Checked = false;
                    Require(sourceTable.Text == "SOURCE.SPEC_FORM" && targetTable.Text == "dbo.SPEC_FORM"
                        && query.Text == "SELECT 'synthetic draft'" && !query.Enabled,
                        "Switching Analysis to table mode discarded a draft.");
                    useQuery.Checked = true;
                    Require(query.Text == "SELECT 'synthetic draft'" && !sourceTable.Enabled && !targetTable.Enabled,
                        "Switching Analysis to query mode overwrote custom SQL or left table inputs active.");
                    query.Clear();
                    useQuery.Checked = false;
                    useQuery.Checked = true;
                    Require(query.Text.Length == 0, "Mode switching replaced an intentionally empty query draft.");
                    typeof(OracleAndSQLConnection).GetMethod("btn_ResetQuery_Click", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(control, new object[] { null, EventArgs.Empty });
                    Require(initialQuery.Length > 0 && query.Text == initialQuery,
                        "Explicit Analysis query reset did not restore the specification template.");

                    panel.Source.Oracle.Checked = true;
                    panel.Source.DataSource.Text = "synthetic-source";
                    panel.Source.Database.Text = "synthetic-service";
                    panel.Source.User.Text = "synthetic-user";
                    panel.Source.Password.Text = "synthetic-password";
                    panel.Source.DefaultPort.Checked = false;
                    panel.Source.Port.Text = "1527";
                    panel.Target.CopyFrom(panel.Source);
                    Require(panel.Target.Sql.Checked && !panel.Target.Oracle.Checked && !panel.Target.Oracle.Enabled
                        && !panel.Target.DefaultPort.Checked && panel.Target.Port.Text.Length == 0
                        && !panel.Target.Port.Enabled && panel.Target.Port.ReadOnly
                        && panel.Source.Port.Text == "1527" && !panel.Source.DefaultPort.Checked,
                        "Copying Oracle into Analysis Target changed provider restrictions or the Source port draft.");
                    panel.Target.WindowsLogin.Checked = true;
                    panel.Target.User.Clear();
                    panel.Target.Password.Clear();
                    control.ValidateInput(); // Validation only: no adapters or connections are invoked.
                    panel.Source.Port.Text = "0";
                    Invalid(control.ValidateInput);
                    panel.Source.Port.Text = "1527";
                    control.ValidateInput();
                    panel.Target.WindowsLogin.Checked = false;
                    Invalid(control.ValidateInput);
                    panel.Target.WindowsLogin.Checked = true;
                    control.ValidateInput();

                    DataSet snapshot = control.FetchDataSet();
                    snapshot.Tables.Add("Synthetic source");
                    snapshot.Tables.Add("Synthetic target");
                    typeof(OracleAndSQLConnection).GetField("boolStatus", BindingFlags.Instance | BindingFlags.NonPublic)
                        .SetValue(control, true);
                    panel.Source.Port.Text = "1528";
                    Require(!control.HasLoadedData && snapshot.Tables.Count == 2,
                        "Editing the shared Analysis port failed to invalidate the loaded state or destroyed its snapshot.");
                }

                using (var panel = new ConnectionPanel())
                {
                    panel.Source.Apply(true, false, false, false, 1527,
                        "synthetic-host", "synthetic-service", "synthetic-user", "synthetic-password");
                    Require(panel.Source.Oracle.Checked && !panel.Source.DefaultPort.Checked
                        && panel.Source.Port.Text == "1527" && panel.Source.Port.Enabled && !panel.Source.Port.ReadOnly,
                        "Applying an Oracle profile reset a nondefault port or locked the custom-port field.");
                    panel.Source.DefaultPort.Checked = false;
                    panel.Source.Port.Text = "custom-invalid-draft";
                    panel.Target.CopyFrom(panel.Source);
                    Require(panel.Target.Oracle.Checked && !panel.Target.DefaultPort.Checked
                        && panel.Target.Port.Text == "custom-invalid-draft",
                        "Connection copy normalized or discarded a custom Oracle port before validation.");
                    panel.Source.Sql.Checked = true;
                    panel.Source.WindowsLogin.Checked = true;
                    panel.Target.CopyFrom(panel.Source);
                    Require(panel.Target.Sql.Checked && panel.Target.WindowsLogin.Checked
                        && !panel.Target.User.Enabled && !panel.Target.Password.Enabled,
                        "Shared copy lost SQL Server Windows-authentication presentation.");
                }

                using (var tracker = new SPIDeltaChangeTrackerOptimised())
                {
                    ConnectionPanel panel = Find<ConnectionPanel>(tracker, "connection");
                    Require(!panel.Source.SqlServerOnly && !panel.Target.SqlServerOnly,
                        "Tracker consolidation unexpectedly restricted a database provider.");
                    var source = new DBConnectionStatus { boolConnection = true };
                    var target = new DBConnectionStatus { boolConnection = true };
                    typeof(SPIDeltaChangeTrackerOptimised).GetField("dbSRCConnection", BindingFlags.Instance | BindingFlags.NonPublic)
                        .SetValue(tracker, source);
                    typeof(SPIDeltaChangeTrackerOptimised).GetField("dbTRGConnection", BindingFlags.Instance | BindingFlags.NonPublic)
                        .SetValue(tracker, target);
                    panel.Target.DataSource.Text += "-synthetic-edit";
                    Require(!source.boolConnection && !target.boolConnection,
                        "Editing the shared Tracker connection did not invalidate tested connections.");
                }
            }
            finally { Profile.ProfileList = previous; }
            passed("Analysis and Tracker use shared connections; draft switching, explicit query reset, SQL-only copy, custom Oracle ports, Windows-authentication state and cache invalidation work offline.");
        }
    }
}
