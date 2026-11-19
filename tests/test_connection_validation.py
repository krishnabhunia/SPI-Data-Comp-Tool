"""Action gates are backed by Windows ConnectionValidationChecks, not source checks alone."""

import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJECT = ROOT / "SPI Data Comp Tool"


def source(name):
    return (PROJECT / name).read_text(encoding="utf-8-sig")


class ConnectionValidation(unittest.TestCase):
    def test_shared_connection_gate_checks_platform_port_and_authentication(self):
        side = source("ConnectionSide.cs")
        self.assertIn("oracle.Checked != sql.Checked", side)
        self.assertIn("FormValidation.Port(port", side)
        self.assertIn("if (!windowsLogin.Checked)", side)
        self.assertIn("password.Text.Length > 0", side)
        self.assertNotIn("password.Text.Trim()", side)

    def test_connection_edits_invalidate_tested_values(self):
        panel = source("ConnectionPanel.cs")
        self.assertIn("public event EventHandler InputChanged;", panel)
        self.assertIn("field.TextChanged += RaiseInputChanged", panel)
        self.assertIn("mode == ConnectionMode.SourceAndTarget", panel)
        profile = source("UserProfileForm.cs")
        self.assertIn("connection.InputChanged +=", profile)
        self.assertIn("connection.ValidateInput(source, target)", profile)
        self.assertNotIn("tb_SRC_Password.Text.Trim()", profile)
        self.assertNotIn("tb_TRG_Password.Text.Trim()", profile)

    def test_profile_imports_are_bounded_and_strict_before_publication(self):
        store = source("ProfileStore.cs")
        for expected in ("MaxDepth = 32", "DuplicatePropertyNameHandling.Error", "StringComparer.OrdinalIgnoreCase",
                         "JTokenType.Boolean", "JTokenType.Integer", "ValidateConnectionProfile", "side.int_Oracle_DefaultPort > 65535"):
            self.assertIn(expected, store)
        self.assertIn("file.Length > 16 * 1024 * 1024", source("UserProfileForm.cs"))

    def test_profile_ports_are_range_checked_before_integer_materialization(self):
        store = source("ProfileStore.cs")
        check = store.split("private static void ValidateConnectionJson", 1)[1].split("internal static List<DBC> Upsert", 1)[0]
        self.assertIn("int.TryParse(port.ToString(Formatting.None), NumberStyles.Integer, CultureInfo.InvariantCulture", check)
        self.assertIn("savedPort < 0 || savedPort > 65535", check)
        self.assertLess(store.index("ValidateConnectionJson((JObject)profile.GetValue"), store.index("array.ToObject<List<DBC>>()"))
        profile = source("UserProfileForm.cs")
        self.assertIn('rb_SRC_oracle.Checked ? FormValidation.Port(tb_SRC_DefaultPort, "Source Oracle port") : 0', profile)
        self.assertIn('rb_TRG_Oracle.Checked ? FormValidation.Port(tb_TRG_DefaultPort, "Target Oracle port") : 0', profile)
        self.assertNotIn("Convert.ToInt32(tb_SRC_DefaultPort.Text)", profile)
        self.assertNotIn("Convert.ToInt32(tb_TRG_DefaultPort.Text)", profile)

    def test_analysis_checks_before_connecting_and_uses_literal_connection_values(self):
        analysis = source("OracleAndSQLConnection.cs")
        load = analysis.split("public void Load_DB_Oracle_SQL()", 1)[1].split("private void btn_Connect", 1)[0]
        self.assertLess(load.index("FormValidation.Check"), load.index("connect_Oracle_DB()"))
        self.assertIn("TextChanged += InvalidateLoadedData", analysis)
        self.assertIn("ValidateTableName", analysis)
        self.assertEqual(analysis.count("ConnectionStrings.Build("), 3)
        self.assertNotIn('"Password = "', analysis)
        self.assertIn("DataSet previous = ds;", load)
        self.assertNotIn("ds.Tables.Clear()", load)

    def test_rule_rows_are_validated_before_persistence(self):
        settings = source("User_Settings.cs")
        for expected in ("identifiers.Add(identifier)", 'ruleType != "1" && ruleType != "2" && ruleType != "3"',
                         'Path.GetExtension(fileName), ".xlsx"', 'FileMode.Open, FileAccess.Read'):
            self.assertIn(expected, settings)

    def test_windows_regressions_cover_invalid_and_compatible_input(self):
        checks = (ROOT / "tests" / "ConnectionValidationChecks.cs").read_text()
        for expected in ("CheckConnectionInputs(passed)", "CheckProfileDocuments(passed)", "CheckRuleRows(passed)",
                         "CheckAnalysisInputs(passed)", "65536", "SourceOnly", "RefreshAvailableProfiles", "ReferenceEquals(snapshot"):
            self.assertIn(expected, checks)


if __name__ == "__main__":
    unittest.main()
