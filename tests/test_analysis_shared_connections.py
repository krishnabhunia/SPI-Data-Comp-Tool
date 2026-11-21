"""Source contracts for the two Analysis connection-panel migrations.

Real-control offline scenarios live in ConnectionValidationChecks; these checks do
not claim database interoperability, query safety, or comparison-engine parity.
"""
from pathlib import Path
import unittest


ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


def source(name):
    return (APP / name).read_text(encoding="utf-8-sig")


class AnalysisSharedConnections(unittest.TestCase):
    def test_both_analysis_connection_surfaces_use_the_shared_panel(self):
        for name in ("OracleAndSQLConnection", "SPIDeltaChangeTrackerOptimised"):
            with self.subTest(surface=name):
                code = source(name + ".cs")
                designer = source(name + ".Designer.cs")
                self.assertIn("new SPI_Data_Comp_Tool.ConnectionPanel()", designer)
                self.assertIn("connection.InputChanged +=", code)
                self.assertIn("connection.ProfileApplied +=", code)
                self.assertNotIn("new System.Windows.Forms.RadioButton()", designer)

    def test_gap_source_and_target_keep_their_supported_providers(self):
        code = source("OracleAndSQLConnection.cs")
        self.assertIn("connection.Target.SqlServerOnly = true", code)
        self.assertIn("connection.ValidateInput()", code)
        self.assertIn("connection.Source.Port.Text", code)
        source_load = code.split("private DataTable connect_Oracle_DB", 1)[1].split("private void controlEnableToggle", 1)[0]
        target_load = code.split("private DataTable connect_SQL_DB", 1)[1].split("private void UseQueryForCheckBoxFunction", 1)[0]
        self.assertIn("connection.Source.WindowsLogin.Checked", source_load)
        self.assertNotIn("connection.Target.WindowsLogin", source_load)
        self.assertIn("connection.Target.WindowsLogin.Checked", target_load)
        self.assertNotIn("OracleDataAdapter", target_load)

    def test_mode_switch_keeps_drafts_and_reset_is_explicit(self):
        code = source("OracleAndSQLConnection.cs")
        switch = code.split("private void UseQueryForCheckBoxFunction()", 1)[1].split("private void btn_ResetQuery_Click", 1)[0]
        self.assertNotIn("ResetText", switch)
        self.assertNotIn(".Text =", switch)
        self.assertIn("tb_querybox.Enabled = !loading && cb_query.Checked", switch)
        self.assertIn("tb_Source_tableName.Enabled = !loading && !cb_query.Checked", switch)
        self.assertIn("tb_destination_tableName.Enabled = !loading && !cb_query.Checked", switch)
        reset = code.split("private void btn_ResetQuery_Click", 1)[1].split("private void cb_query_CheckedChanged", 1)[0]
        self.assertIn("tb_querybox.Text = DefaultSpecificationQuery", reset)

    def test_gap_legacy_automation_names_remain_on_shared_fields(self):
        code = source("OracleAndSQLConnection.cs")
        for name in ("tb_oracle_dsource", "tb_oracle_dbName", "tb_oracle_username", "tb_oracle_pwd",
                     "tb_sql_dsource", "tb_sql_dbName", "tb_sql_username", "tb_sql_pwd"):
            self.assertIn(f'{name}.Name = "{name}"', code)

    def test_tracker_keeps_profile_extras_and_removes_copied_handlers(self):
        code = source("SPIDeltaChangeTrackerOptimised.cs")
        self.assertIn("cb_SRC_TableName.Text = details.str_SRC_TableName", code)
        self.assertIn("cb_TRG_TableName.Text = details.str_TRG_TableName", code)
        for handler in ("rb_oracle_CheckedChanged", "rb_sql_CheckedChanged", "rb_Target_SQL_CheckedChanged",
                        "rb_Target_Oracle_CheckedChanged", "cb_windowsAuthentication_CheckedChanged",
                        "cb_UserProfileList_SelectedIndexChanged", "btn_Copy_SRC_TRG_Click", "btn_TRG_SRC_Click",
                        "cb_Default_Port_CheckedChanged", "cb_Target_DefaultPort_CheckedChanged", "btn_SaveProfile_Click"):
            self.assertNotIn("void " + handler + "(", code)
            self.assertNotIn("this." + handler, source("SPIDeltaChangeTrackerOptimised.Designer.cs"))
        self.assertTrue((ROOT / "tools/layouts/SPIDeltaChangeTrackerOptimised.py").exists())

    def test_container_locking_does_not_override_authentication_field_state(self):
        for name, method, following in (
            ("OracleAndSQLConnection.cs", "private void controlEnableToggle", "public void Load_DB_Oracle_SQL"),
            ("SPIDeltaChangeTrackerOptimised.cs", "private void toggleCondition", "//private void cb_selectAll_toggle"),
        ):
            body = source(name).split(method, 1)[1].split(following, 1)[0]
            self.assertIn("connection.Enabled =", body)
            self.assertNotIn("Password.Enabled", body)
            self.assertNotIn("UserName.Enabled", body)
            self.assertNotIn("DefaultPort.Enabled", body)

    def test_copy_honours_receiver_provider_restrictions_and_port_drafts(self):
        body = source("ConnectionSide.cs").split("public void CopyFrom(ConnectionSide other)", 1)[1].split("internal void Apply(", 1)[0]
        self.assertIn("Oracle.Checked = !sqlServerOnly && other.Oracle.Checked", body)
        self.assertIn("Sql.Checked = sqlServerOnly || other.Sql.Checked", body)
        self.assertIn("WindowsLogin.Checked = Sql.Checked && other.WindowsLogin.Checked", body)
        self.assertIn("Port.Text = Oracle.Checked ? other.Port.Text : string.Empty", body)

    def test_profile_application_keeps_only_supported_connection_options(self):
        body = source("ConnectionSide.cs").split("internal void Apply(", 1)[1].split("private void NameFields", 1)[0]
        self.assertIn("if (sqlServerOnly)", body)
        self.assertIn("WindowsLogin.Checked = Sql.Checked && windowsLogin", body)
        self.assertIn("DefaultPort.Checked = Oracle.Checked && defaultPort", body)
        self.assertIn("Port.Enabled = Oracle.Checked", body)
        self.assertIn("Port.ReadOnly = !Oracle.Checked || DefaultPort.Checked", body)
        self.assertIn("Password.Text = password", body)

    def test_windows_offline_checks_cover_migration_behaviour(self):
        tests = (ROOT / "tests/ConnectionValidationChecks.cs").read_text(encoding="utf-8-sig")
        self.assertIn("CheckSharedConnectionMigrations(passed)", tests)
        method = tests.split("private static void CheckSharedConnectionMigrations", 1)[1]
        for expected in ("panel.Target.CopyFrom(panel.Source)", "custom-invalid-draft", "btn_ResetQuery_Click",
                         "Snapshot", "WindowsLogin", "new SPIDeltaChangeTrackerOptimised"):
            self.assertIn(expected.lower(), method.lower())
        self.assertNotIn("Load_DB_Oracle_SQL()", method)


if __name__ == "__main__":
    unittest.main()
