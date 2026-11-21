"""Keep optional preference recovery separate from update persistence and compiled Windows fixtures."""

import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


def source(name):
    return (APP / name).read_text(encoding="utf-8-sig")


class PreferenceContracts(unittest.TestCase):
    def test_optional_consumers_do_not_read_the_configuration_provider_directly(self):
        for name in ("User_Settings.cs", "DB Comparison.cs", "Excel_Excel_Comparison.cs", "Common_Data.cs"):
            with self.subTest(source=name):
                code = "\n".join(line for line in source(name).splitlines() if not line.strip().startswith("//"))
                self.assertNotIn("Properties.Settings.Default", code)
        for name in ("DB Comparison.cs", "Excel_Excel_Comparison.cs"):
            self.assertIn("AppPreferences.ExcelWriter == ExcelWriterMode.EPPlus", source(name))

    def test_recovery_never_resets_user_files_or_weakens_update_snapshot_requirements(self):
        preferences = source("AppPreferences.cs")
        for forbidden in (".Reset(", ".Reload(", "File.Delete(", "File.Move(", "File.Write", "UpdatePreferences.Initialize("):
            self.assertNotIn(forbidden, preferences)
        self.assertIn("if (!SelfTest.IsRunning) Properties.Settings.Default.Save();", preferences)
        self.assertNotIn("AppPreferences", source("UpdatePreferences.cs"))
        self.assertIn("if (!UpdatePreferences.IsInitialized)", source("UpdateInstaller.cs"))

    def test_isolated_windows_failure_suite_is_compiled_and_run(self):
        self.assertIn('Compile Include="AppPreferences.cs"', source("SPI Data Comp Tool.csproj"))
        self.assertIn('Compile Include="..\\tests\\PreferenceChecks.cs"', source("SPI Data Comp Tool.csproj"))
        self.assertIn("PreferenceChecks.Run(passed)", (ROOT / "tests/UiWorkflowChecks.cs").read_text())
        checks = (ROOT / "tests/PreferenceChecks.cs").read_text()
        for marker in ("CheckFailedSaves(true)", "CheckPartialSetterFailure()", "CheckMappedConfiguration",
                       "OpenMappedExeConfiguration", "FileShare.None", "settings.Show();", "AppPreferences.UseForChecks"):
            self.assertIn(marker, checks)
        self.assertNotIn("Properties.Settings.Default", checks)


if __name__ == "__main__":
    unittest.main()
