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
        self.assertNotIn("entry.ExternalAttributes", worker)  # central-directory validation also checks duplicate names/counts
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
        for marker in ("CheckArchiveGuards", "CheckPackageVariants", "CheckPackage", "CheckBusyGuards", "CheckSettings", "CheckRollback", "CheckRealExecutable", "Assembly.GetExecutingAssembly().Location", "Synthetic fourth-copy failure", "Synthetic migration failure"):
            self.assertIn(marker, checks)
        self.assertNotIn("Process.Start", checks)
        self.assertNotIn("InstallAndRestart(", checks)
        self.assertNotIn("RegistryKey", checks)

    def test_core_only_and_legacy_archives_keep_a_closed_file_allowlist(self):
        worker = (APP / "UpdateApplyWorker.cs").read_text()
        self.assertIn("LegacyTemplateNames", worker)
        # 4.6.0 onwards: the exe is the only required file; a .config is optional and validated when present.
        self.assertIn('var requiredFiles = new List<string> { executableName };', worker)
        self.assertIn('"windows-msi/SPI_Data_Comp_Tool_" + version + "_x64.msi"', worker)
        self.assertIn('"microsoft-store/SPI_Data_Comp_Tool_" + version + "_x64.msix"', worker)
        self.assertIn("if (bundled.Contains(name)) continue;", worker)
        self.assertIn("staleConfig", worker)
        self.assertIn('if (files.Any(x => x.RelativePath.StartsWith("PNID_Reports/", StringComparison.OrdinalIgnoreCase)))', worker)
        self.assertIn('"PNID_Reports/Source/instrument.txt", "PNID_Reports/Target_IPEP/instrument.txt"', worker)
        self.assertIn("!allowed.TryGetValue(name, out destination)", worker)
        checks = (ROOT / "tests/UpdateApplyChecks.cs").read_text()
        self.assertIn("anchors.All(entry => entry.Length == 0)", checks)
        self.assertIn("File.ReadAllText(legacyTemplate)", checks)
        self.assertIn("File.ReadAllText(userLegacyFile)", checks)
        self.assertIn("CheckSingleExeApply", checks)
        self.assertIn("A single-exe update kept the older .config beside the new exe.", checks)

    def test_previous_channel_migration_is_read_only_bounded_and_keeps_canonical_precedence(self):
        preferences = (APP / "UpdatePreferences.cs").read_text()
        migration = preferences.split("internal static UpdateChannel ResolveInitialChannel", 1)[1].split("public static void SaveCurrent()", 1)[0]
        self.assertIn("MaximumLegacyPreferenceBytes", migration)
        self.assertIn("File.GetAttributes(stableFile)", migration)
        self.assertLess(migration.index("return ReadSnapshot(stableFile).Channel"), migration.index("new FileStream(legacyFile"))
        self.assertIn("UpdateApplyWorker.RejectReparsePoints(legacyFile)", migration)
        self.assertIn("FileAccess.Read, FileShare.Read", migration)
        self.assertIn("new UTF8Encoding(false, true)", migration)
        self.assertIn("value == \"1\") selected = UpdateChannel.All", migration)
        self.assertIn("value == \"0\") selected = UpdateChannel.Normal", migration)
        self.assertIn("!names.Add(name)", migration)
        self.assertIn("reader.TokenType != JsonToken.Boolean", migration)
        self.assertIn("ex is InvalidDataException", migration)
        self.assertNotIn("File.Write", migration)
        self.assertNotIn("File.Delete", migration)
        self.assertNotIn("Process.Start", migration)
        initialize = preferences.split("public static void Initialize()", 1)[1].split("internal static UpdateChannel ResolveInitialChannel", 1)[0]
        self.assertIn('"SPI Data Comparison Tool", "updates.json"', initialize)
        self.assertIn("CaptureInMemory(), null", initialize)
        self.assertIn("InitializationWarning = warning", initialize)
        checks = (ROOT / "tests/UpdateApplyChecks.cs").read_text()
        self.assertIn("CheckLegacyChannels(root)", checks)
        self.assertIn("FileShare.None", checks)
        self.assertIn("concurrently-created canonical choice", checks)


if __name__ == "__main__":
    unittest.main()
