"""Static safety contracts; UpdateApplyChecks.cs exercises temp-only real Windows file operations."""

import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


class UpdateApplyContracts(unittest.TestCase):
    def test_helper_handles_special_mode_before_normal_startup(self):
        program = (APP / "Program.cs").read_text(encoding="utf-8-sig")
        self.assertLess(program.index("UpdateApplyWorker.TryRun(args)"), program.index("UpdatePreferences.Initialize()"))
        worker = (APP / "UpdateApplyWorker.cs").read_text()
        self.assertIn("args.Any", worker)
        self.assertIn("Environment.ExitCode = 1", worker)
        self.assertNotIn(".Kill(", worker)
        self.assertIn("ParentStartUtcTicks", worker)
        self.assertIn("ValidateOriginalTarget(manifest);", worker)

    def test_installer_preserves_scope_and_normal_windows_security(self):
        installer = (APP / "UpdateInstaller.cs").read_text()
        worker = (APP / "UpdateApplyWorker.cs").read_text()
        self.assertIn("{DF6AFF7A-0A5A-4F1E-BF54-F5301F101D26}_is1", installer)
        for marker in ("RegistryHive.CurrentUser", "RegistryHive.LocalMachine", "RegistryView.Registry64", "RegistryView.Registry32", '"InstallLocation"'):
            self.assertIn(marker, installer)
        for marker in ("/SILENT", "/NORESTART", "/NOCLOSEAPPLICATIONS", "/NOFORCECLOSEAPPLICATIONS", "/NORESTARTAPPLICATIONS", "/ALLUSERS", "/CURRENTUSER", '"runas"'):
            self.assertIn(marker, worker)
        self.assertNotIn("ExecutionPolicy", worker + installer)
        self.assertIn("CanInstallForms", installer)
        self.assertIn("worker.IsBusy", installer)

    def test_apply_hashes_and_constrains_each_path_and_payload(self):
        worker = (APP / "UpdateApplyWorker.cs").read_text()
        installer = (APP / "UpdateInstaller.cs").read_text()
        for marker in ("SHA256.Create()", "VerifyHash", "RejectReparsePoints", "RequireContained", "ReadZipAttributes", "0xA000", "MaximumExpandedBytes", "written > entry.Length", "written != entry.Length"):
            self.assertIn(marker, worker)
        self.assertNotIn("entry.ExternalAttributes", worker)  # unavailable in .NET Framework 4.6.2
        self.assertIn("FileShare.Read", worker)
        self.assertIn("github.com", installer)
        self.assertIn("/krishnabhunia/SPI-Data-Comp-Tool/releases/download/", installer)
        self.assertIn("package.Kind != expectedKind", installer)
        self.assertNotIn("ExtractToDirectory", worker)
        self.assertNotIn("Directory.Delete", worker)

    def test_migration_is_whitelisted_bounded_and_atomic(self):
        preferences = (APP / "UpdatePreferences.cs").read_text()
        self.assertIn('"ProfilePathUser", "RuleFilePath", "EPPlus_Excel_Save", "XL_Excel_Save"', preferences)
        self.assertIn("DtdProcessing.Prohibit", preferences)
        self.assertIn("XmlResolver = null", preferences)
        self.assertIn("MaxCharactersInDocument", preferences)
        self.assertIn("File.Replace", preferences)
        self.assertLess(preferences.index("lastSaved = SavePersistedSnapshot(stableFile"), preferences.index("initialized = true;"))
        self.assertIn("ReadPersistedPreferences", preferences)
        self.assertIn("FileShare.None", preferences)
        initialize = preferences.split("public static void Initialize()", 1)[1].split("public static void SaveCurrent()", 1)[0]
        self.assertNotIn("Settings.Default.Save()", initialize)
        self.assertNotIn("GetLastWriteTimeUtc", initialize)
        self.assertIn("!File.Exists(currentConfig) && File.Exists(stableFile)", initialize)
        self.assertIn("if (File.Exists(currentUserConfig))", initialize)
        self.assertIn("Properties.Settings.Default.Reload();", initialize)
        self.assertIn("else Apply(lastSaved);", initialize)
        self.assertIn("Encoding.UTF8.GetByteCount(json) > 131072", preferences)
        self.assertNotIn("str_Password", preferences)
        self.assertNotIn("File.Copy", preferences)
        self.assertIn("Required = Required.Always", preferences)

    def test_runtime_cases_include_real_build_and_failure_recovery(self):
        checks = (ROOT / "tests/UpdateApplyChecks.cs").read_text()
        for marker in ("CheckArchiveGuards", "CheckPackage", "CheckBusyGuards", "CheckSettings", "CheckRollback", "CheckRealExecutable", "Assembly.GetExecutingAssembly().Location", "Synthetic fourth-copy failure", "Synthetic migration failure"):
            self.assertIn(marker, checks)
        self.assertNotIn("Process.Start", checks)
        self.assertNotIn("InstallAndRestart(", checks)
        self.assertNotIn("RegistryKey", checks)


if __name__ == "__main__":
    unittest.main()
