"""Static regression contracts for legacy WinForms data and logging code.

These checks do not execute the desktop UI or connect to live databases.
"""

import unittest
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"


def source(name: str) -> str:
    return (ROOT / name).read_text(encoding="utf-8-sig")


class DataIntegrityContracts(unittest.TestCase):
    def test_oracle_reference_matches_nuget_restore_version(self) -> None:
        packages = ET.fromstring(source("packages.config"))
        versions = [package.get("version") for package in packages.findall("package")
                    if package.get("id") == "Oracle.ManagedDataAccess"]
        self.assertEqual(len(versions), 1)
        project = ET.fromstring(source("SPI Data Comp Tool.csproj"))
        ns = {"msbuild": "http://schemas.microsoft.com/developer/msbuild/2003"}
        references = [reference.findtext("msbuild:HintPath", namespaces=ns)
                      for reference in project.findall(".//msbuild:Reference", namespaces=ns)
                      if reference.get("Include", "").startswith("Oracle.ManagedDataAccess,")]
        self.assertEqual(len(references), 1)
        self.assertIn(f"Oracle.ManagedDataAccess.{versions[0]}", references[0])

    def test_target_connection_uses_target_windows_authentication(self) -> None:
        # Every window that connects Source and Target this way: the Target connection must read
        # the Target "Windows authentication" box, never the Source one. (This contract first
        # covered UserControlDataBase.cs, an unused copy removed in the duplication clean-up;
        # the same bug was still live in these windows.)
        for name in ("Table Extraction After Delta.cs", "Pre_Merging_Duplicacy_Form.cs"):
            with self.subTest(window=name):
                source_code = source(name)
                target = source_code.split('"Trying to connect to Target Oracle', 1)[1]
                target = target.split("\n        private ", 1)[0]
                self.assertEqual(target.count("cb_WindowAuthentication_TRG.Checked"), 2)
                self.assertNotIn("cb_WindowAuthentication_SRC.Checked", target)

    def test_legacy_mapper_tests_only_its_active_source_connection(self) -> None:
        mapper = source("SPIDeltaChangeLegacyID.cs")
        test = mapper.split("private void btn_Connect_Reference_ID_Click", 1)[1]
        test = test.split("private void ValidateLegacySource", 1)[0]
        self.assertIn("ValidateLegacyConnectionInputs()", test)
        self.assertIn("dBConnectionStatus_SRC.DatabaseConnection", test)
        self.assertNotIn("dBConnectionStatus_TRG.DatabaseConnection", test)
        self.assertNotIn("cb_WindowAuthentication_TRG.Checked", test)

    def test_optimized_tracker_source_uses_source_windows_authentication(self) -> None:
        tracker = source("SPIDeltaChangeTrackerOptimised.cs")
        source_load = tracker.split("private void connect_Source_DB(", 1)[1]
        source_load = source_load.split("private void connect_Target_DB(", 1)[0]
        self.assertIn("cb_WindowAuthentication_SRC.CheckState", source_load)
        self.assertNotIn("cb_WindowAuthentication_TRG.CheckState", source_load)

    def test_target_oracle_reads_target_not_source(self) -> None:
        tracker = source("SPIDeltaChangeTrackerOptimised.cs")
        target = tracker.split("private void connect_Target_DB(", 1)[1]
        target = target.split("private void btn_Load_SRC_Table_Click", 1)[0]
        target_oracle = target.split("if (rb_TRG_Oracle.Checked)", 1)[1]
        target_oracle = target_oracle.split("else if (rb_TRG_SQL.Checked)", 1)[0]
        self.assertIn("string targetOracleConnection =", target_oracle)
        self.assertIn("new OracleDataAdapter(strQuery, targetOracleConnection)", target_oracle)
        self.assertNotIn("new OracleDataAdapter(strQuery, strOracleConnection)", target_oracle)

    def test_new_profile_exports_do_not_persist_passwords(self) -> None:
        form = source("UserProfileForm.cs")
        fields = form.split("private DBConnectionDetailsProfile GetDBConnectionDetailsProfile", 1)[1]
        fields = fields.split("private void btnDeleteUserProfile_Click", 1)[0]
        self.assertEqual(fields.count('str_Password = ""'), 2)
        self.assertNotIn("str_Password = GlobalDebug.boolIsGlobalDebug", fields)

    def test_connection_passwords_are_redacted_in_ui_and_disk_logs(self) -> None:
        common = source("Common_Data.cs")
        self.assertIn("strMsg = RedactConnectionCredentials(strMsg);", common)
        self.assertIn("ConnectionCredentialPattern.Replace(message", common)
        self.assertEqual(source("Logs.cs").count("Common_Data.RedactConnectionCredentials(strLog)"), 3)
        premerge = source("Pre_Merging_Duplicacy_Form.cs")
        self.assertNotIn('And {strConnectionString}', premerge)


if __name__ == "__main__":
    unittest.main()
