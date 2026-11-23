"""Guard the updater's normal-startup and CI boundaries without running an installer."""

import unittest
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


class UpdateIntegrationTests(unittest.TestCase):
    def test_helper_and_self_tests_do_not_initialize_user_preferences(self):
        source = (APP / "Program.cs").read_text(encoding="utf-8-sig")
        helper = source.index("if (UpdateApplyWorker.TryRun(args)) return;")
        checks = source.index("if (SelfTest.TryRun(args)) return;")
        preferences = source.index("UpdatePreferences.Initialize()")
        normal = source.index("Application.Run(new MainForm())")
        self.assertLess(helper, source.index("Application.EnableVisualStyles()"))
        self.assertLess(helper, checks)
        self.assertLess(checks, preferences)
        self.assertLess(preferences, normal)
        self.assertLess(normal, source.index("UpdatePreferences.SaveCurrent()"))

    def test_all_updater_components_and_executable_checks_are_compiled(self):
        project = ET.parse(APP / "SPI Data Comp Tool.csproj")
        ns = {"msb": "http://schemas.microsoft.com/developer/msbuild/2003"}
        paths = {node.get("Include") for node in project.findall(".//msb:Compile", ns)}
        for name in ("UpdateContracts", "UpdateVersion", "UpdateFeedClient", "UpdateInstaller",
                     "UpdateApplyWorker", "UpdatePreferences", "UpdatesControl"):
            self.assertIn(name + ".cs", paths)
        checks = (ROOT / "tests/UiWorkflowChecks.cs").read_text()
        for name in ("UpdateFeedChecks", "UpdateApplyChecks", "UpdateUiChecks"):
            self.assertIn("..\\tests\\" + name + ".cs", paths)
            self.assertIn(name + ".Run(passed)", checks)
        self.assertLess(checks.index("UpdateFeedChecks.Run(passed)"), checks.index("CheckNavigation(passed)"))

    def test_tls_uses_secure_runtime_switches_not_certificate_bypass(self):
        config = ET.parse(APP / "App.config")
        switches = config.find("runtime/AppContextSwitchOverrides").get("value")
        self.assertIn("Switch.System.Net.DontEnableSchUseStrongCrypto=false", switches)
        self.assertIn("Switch.System.Net.DontEnableSystemDefaultTlsVersions=false", switches)
        for name in ("UpdateFeedClient.cs", "UpdateInstaller.cs", "UpdateApplyWorker.cs"):
            source = (APP / name).read_text()
            self.assertNotIn("ServerCertificateValidationCallback", source)
            self.assertNotIn("ServicePointManager.SecurityProtocol =", source)

    def test_first_manual_upgrade_and_unsigned_limits_are_documented(self):
        guide = (ROOT / "docs/UPDATING.md").read_text()
        self.assertIn("manually", guide)
        self.assertIn("unsigned", guide)
        self.assertIn("not** publisher/code-signing authentication", guide)
        self.assertIn("without this feature removes the update page", guide)
        self.assertIn("not presented as an automatic rollback", guide)

    def test_both_portable_and_installed_builds_execute_update_regressions(self):
        workflow = (ROOT / ".github/workflows/build.yml").read_text()
        self.assertEqual(workflow.count("-ArgumentList '--check-ui'"), 1)
        self.assertIn("./tools/test_windows_installer.ps1", workflow)
        installer = (ROOT / "tools/test_windows_installer.ps1").read_text()
        self.assertIn("'assemblies','ui','startup'", installer)
        self.assertIn('Invoke-CheckedProcess $exe @("--check-$check"', installer)
        self.assertIn('Missing x64 $check check log', installer)

    def test_json_formatting_is_unambiguous_alongside_xml(self):
        source = (APP / "UpdatePreferences.cs").read_text()
        self.assertIn("using System.Xml;", source)
        self.assertEqual(source.count("Newtonsoft.Json.Formatting.Indented"), 2)
        self.assertNotIn(", Formatting.Indented", source)

    def test_ignored_legacy_preferences_are_reported_without_background_work(self):
        source = (APP / "UpdatesControl.cs").read_text()
        constructor = source.split("internal UpdatesControl(", 1)[1].split("private static Button", 1)[0]
        self.assertIn("persistPreferences && !string.IsNullOrWhiteSpace(UpdatePreferences.InitializationWarning)", constructor)
        self.assertIn("SetStatus(UpdatePreferences.InitializationWarning)", constructor)
        self.assertNotIn("feed.CheckAsync(", constructor)
        self.assertNotIn("feed.DownloadAsync(", constructor)
        self.assertNotIn("UpdatePreferences.Initialize()", constructor)


if __name__ == "__main__":
    unittest.main()
