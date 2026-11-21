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


if __name__ == "__main__":
    unittest.main()
