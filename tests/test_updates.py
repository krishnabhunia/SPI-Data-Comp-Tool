"""Contracts for reconciling the merged updater with one explicit, exact-version workflow."""

import re
import unittest
import xml.etree.ElementTree as ET
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
PROJ = REPO / "SPI Data Comp Tool"


def text(path: Path) -> str:
    return path.read_text(encoding="utf-8-sig")


def code(name: str) -> str:
    return re.sub(r"//[^\n]*", "", text(PROJ / name))


class Updater(unittest.TestCase):
    def test_one_canonical_engine_and_its_compatibility_checks_are_compiled(self) -> None:
        project = ET.parse(PROJ / "SPI Data Comp Tool.csproj")
        ns = {"msb": "http://schemas.microsoft.com/developer/msbuild/2003"}
        paths = [node.get("Include") for node in project.findall(".//msb:Compile", ns)]
        self.assertEqual(len(paths), len(set(paths)), "A source file is compiled twice")
        for name in ("UpdateContracts.cs", "UpdateVersion.cs", "UpdateFeedClient.cs", "UpdateInstaller.cs",
                     "UpdateApplyWorker.cs", "UpdatePreferences.cs", "UpdatesControl.cs", "MainForm.Updates.cs",
                     r"..\tests\UpdateChecks.cs"):
            self.assertIn(name, paths)
        for name in ("UpdateService.cs", "UpdateDialog.cs", "UpdateSettingsPage.cs"):
            self.assertNotIn(name, paths)
            self.assertFalse((PROJ / name).exists(), f"Retired competing updater remains: {name}")
        declarations = sum(len(re.findall(r"\benum\s+UpdateChannel\b", text(path))) for path in PROJ.glob("*.cs"))
        self.assertEqual(declarations, 1, "Competing update channel definitions remain")
        references = [node.get("Include") for node in project.findall(".//msb:Reference", ns)]
        self.assertNotIn("System.IO.Compression.FileSystem", references)

    def test_feed_asset_contract_matches_the_existing_release_job(self) -> None:
        feed = text(PROJ / "UpdateFeedClient.cs")
        workflow = text(REPO / ".github" / "workflows" / "build.yml")
        self.assertIn('Repository = "krishnabhunia/SPI-Data-Comp-Tool"', feed)
        self.assertIn("request.UserAgent", feed)
        self.assertIn('"_x64_setup.exe" : ".zip"', feed)
        self.assertIn('GetAssetName(version, UpdateInstallKind.Portable) + ".sha256"', feed)
        self.assertIn('setup_name="SPI_Data_Comp_Tool_${VERSION}_x64_setup.exe"', workflow)
        self.assertIn('"${zip_name}.sha256"', workflow)

    def test_only_the_verified_selected_package_can_reach_the_installer(self) -> None:
        ui = code("UpdatesControl.cs")
        install = ui.split("internal void InstallSelected()", 1)[1].split("private void UpdateButtons()", 1)[0]
        self.assertIn("!DownloadMatchesSelection()", install)
        self.assertIn("MessageBoxDefaultButton.Button2", install)
        self.assertLess(install.index("if (!confirmed)"), install.index("installer.InstallAndRestart"))
        self.assertIn("installer.InstallAndRestart(verifiedPackage, FindForm())", install)
        feed = code("UpdateFeedClient.cs")
        self.assertLess(feed.index("string.Equals(actualHash, expectedHash"),
                        feed.index("new DownloadedUpdate"))
        self.assertIn("VerifyHash", code("UpdateApplyWorker.cs"))

    def test_main_window_has_no_competing_startup_or_close_time_updater(self) -> None:
        main = code("MainForm.Updates.cs")
        self.assertIn("InitializeUpdates();", code("MainForm.cs"))
        for forbidden in ("UpdateService", "InstallWaitingUpdate", "FormClosing +=", "FindUpdate(",
                          "DownloadAsync(", "Process.Start(", "Task.Run("):
            self.assertNotIn(forbidden, main)
        self.assertNotIn("InstallAndRestart(", main)
        self.assertNotIn("CheckAsync(", main)

    def test_settings_has_exactly_one_designer_owned_updates_page(self) -> None:
        settings = code("User_Settings.cs")
        designer = code("User_Settings.Designer.cs")
        self.assertNotIn("UpdateSettingsPage", settings)
        self.assertNotIn("tabs_Settings.TabPages.Add(", settings)
        self.assertEqual(designer.count("this.tabs_Settings.Controls.Add(this.tab_Updates);"), 1)
        self.assertEqual(designer.count("new SPI_Data_Comp_Tool.UpdatesControl()"), 1)
        checks = text(REPO / "tests" / "UiWorkflowChecks.cs")
        self.assertIn("tabs.TabPages.Count == 4", checks)   # General, Comparison rules, Updates, Profiles
        for index, name in enumerate(("tab_General", "tab_Rules", "tab_Updates")):
            self.assertIn(f'tabs.TabPages[{index}].Name == "{name}"', checks)

    def test_old_installer_relaunch_is_preserved_without_double_restarting_new_updates(self) -> None:
        iss = text(REPO / "windows" / "installer" / "SPI_Data_Comp_Tool.iss")
        self.assertIn("Check: RelaunchRequested", iss)
        self.assertIn("{param:RELAUNCH|0}", iss)
        self.assertIn("skipifnotsilent runasoriginaluser", iss)
        worker = code("UpdateApplyWorker.cs")
        self.assertNotIn("/RELAUNCH=1", worker)
        self.assertIn("/NORESTARTAPPLICATIONS", worker)
        self.assertIn("VerifyApplicationVersion", worker)

    def test_ci_check_updates_uses_shared_offline_safety_checks(self) -> None:
        workflow = text(REPO / ".github" / "workflows" / "build.yml")
        self.assertIn("'--check-updates'", workflow)
        self.assertIn('"--check-updates"', code("SelfTest.cs"))
        checks = text(REPO / "tests" / "UpdateChecks.cs")
        self.assertIn("UpdateFeedChecks.Run(passed);", checks)
        self.assertIn("UpdateApplyChecks.Run(passed);", checks)
        self.assertLess(checks.index("UpdateFeedChecks.Run(passed)"), checks.index("UpdateApplyChecks.Run(passed)"))
        for forbidden in ("UpdateService", "FindUpdate(", "Download(", "Process.Start(", "UpdatePreferences.Initialize("):
            self.assertNotIn(forbidden, checks)
        self.assertIn('lines.Add("FAIL " + ex)', checks)
        self.assertIn("return 1;", checks)
        self.assertEqual(workflow.count("-ArgumentList '--check-ui'"), 2)

    def test_release_cleanup_remains_manual_and_requires_explicit_delete_confirmation(self) -> None:
        workflow = text(REPO / ".github" / "workflows" / "release-cleanup.yml")
        events = workflow.split("on:\n", 1)[1].split("\npermissions:", 1)[0]
        self.assertEqual(re.findall(r"(?m)^  ([a-z_]+):", events), ["workflow_dispatch"])
        self.assertIn("default: ''", events)
        self.assertIn('[ "${CONFIRM}" = "DELETE" ] && delete=true', workflow)
        self.assertIn('if [ "$delete" = true ]; then gh api -X DELETE', workflow)
        self.assertIn('if [ "$delete" = true ]; then gh release delete', workflow)


if __name__ == "__main__":
    unittest.main()
