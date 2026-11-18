"""Static contracts for the in-app updater (its behaviour is checked in C#: tests/UpdateChecks.cs)."""

import re
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
PROJ = REPO / "SPI Data Comp Tool"


def text(path: Path) -> str:
    return path.read_text(encoding="utf-8-sig")


def code(name: str) -> str:
    return re.sub(r"//[^\n]*", "", text(PROJ / name))


class Updater(unittest.TestCase):
    def test_new_files_are_in_the_project(self) -> None:
        project = text(PROJ / "SPI Data Comp Tool.csproj")
        for name in ("UpdateService.cs", "UpdateDialog.cs", "UpdateSettingsPage.cs", "MainForm.Updates.cs", r"..\tests\UpdateChecks.cs"):
            self.assertIn(f'<Compile Include="{name}"', project)
        self.assertIn('<Reference Include="System.IO.Compression.FileSystem" />', project)

    def test_updater_reads_this_repository_over_tls_1_2(self) -> None:
        service = code("UpdateService.cs")
        workflow = text(REPO / ".github" / "workflows" / "build.yml")
        self.assertIn('Repository = "krishnabhunia/SPI-Data-Comp-Tool"', service)
        self.assertIn("SecurityProtocolType.Tls12", service)
        self.assertIn("request.UserAgent", service)   # GitHub refuses requests without one
        # the names the updater looks for are the names the release job uploads
        self.assertIn('"_x64_setup.exe"', service)
        self.assertIn('".zip.sha256"', service)
        self.assertIn('setup_name="SPI_Data_Comp_Tool_${VERSION}_x64_setup.exe"', workflow)
        self.assertIn('"${zip_name}.sha256"', workflow)

    def test_nothing_is_installed_without_a_matching_checksum(self) -> None:
        download = code("UpdateService.cs").split("public static string Download(", 1)[1].split("public static string Install(", 1)[0]
        self.assertIn("ExpectedSha256(", download)
        self.assertIn("if (actual != expected)", download)
        self.assertIn("File.Delete(target);", download)
        self.assertLess(download.index("if (actual != expected)"), download.index("return target;"))
        # Install is only ever given what Download returned
        for name in ("UpdateDialog.cs", "MainForm.Updates.cs"):
            for call in re.findall(r"UpdateService\.Install\((\w+)", code(name)):
                self.assertEqual(call, "file")

    def test_main_window_checks_at_start_but_never_during_build_checks(self) -> None:
        main = code("MainForm.Updates.cs")
        self.assertIn("Shown += (s, e) => CheckForUpdate();", main)
        self.assertIn("if (SelfTest.IsRunning) return;", main)
        self.assertIn("Visible = false", main)                    # the Update entry is hidden until an update exists
        self.assertIn("updateMenuItem.Visible = found != null;", main)
        self.assertIn("Alignment = ToolStripItemAlignment.Right", main)
        self.assertIn("InitializeUpdates();", code("MainForm.cs"))

    def test_settings_has_the_updates_tab(self) -> None:
        self.assertIn("tabs_Settings.TabPages.Add(new UpdateSettingsPage());", code("User_Settings.cs"))
        page = code("UpdateSettingsPage.cs")
        for word in ("Stable (recommended)", "Beta (optional)", "install them when I close the app", "Check for &updates now"):
            self.assertIn(word, page)

    def test_installer_restarts_the_app_only_for_the_updater(self) -> None:
        iss = text(REPO / "windows" / "installer" / "SPI_Data_Comp_Tool.iss")
        self.assertIn("Check: RelaunchRequested", iss)
        self.assertIn("{param:RELAUNCH|0}", iss)
        self.assertIn("skipifnotsilent runasoriginaluser", iss)
        self.assertIn("/RELAUNCH=1", code("UpdateService.cs"))

    def test_ci_runs_the_updater_checks(self) -> None:
        workflow = text(REPO / ".github" / "workflows" / "build.yml")
        self.assertIn("'--check-updates'", workflow)
        self.assertIn('"--check-updates"', code("SelfTest.cs"))
        checks = text(REPO / "tests" / "UpdateChecks.cs")
        self.assertGreaterEqual(checks.count("Check("), 20)
        self.assertNotIn("github.com", checks)   # synthetic data only


if __name__ == "__main__":
    unittest.main()
