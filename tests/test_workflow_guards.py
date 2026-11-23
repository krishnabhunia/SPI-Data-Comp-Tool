"""Keep executable UI checks and supported Actions runtimes in the Windows pipeline."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJ = ROOT / "SPI Data Comp Tool"


class WorkflowGuards(unittest.TestCase):
    def test_windows_build_keeps_checks_and_logs_without_custom_summary(self):
        workflow = (ROOT / ".github/workflows/build.yml").read_text()
        windows = workflow.split("  windows-build:\n", 1)[1].split("\n  release:\n", 1)[0]
        self.assertNotIn("GITHUB_STEP_SUMMARY", windows)
        for step in ("Build desktop application", "UI workflow regression checks", "Open every window",
                     "Package portable build", "Portable build is one exe (libraries embedded)",
                     "Build 64-bit application", "Build 64-bit installer",
                     "Test the installer (silent install, check, uninstall)"):
            with self.subTest(step=step):
                self.assertIn("- name: " + step, windows)
        self.assertIn("Get-Content $log | ForEach-Object { Write-Host $_ }", windows)
        self.assertIn('Write-Host "Opened $ok of $($forms.Count) windows"', windows)
        self.assertIn('Write-Host "::error::window did not open: $x"', windows)
        self.assertIn("if ($failed) { throw", windows)
        installer = (ROOT / "tools/test_windows_installer.ps1").read_text()
        self.assertIn("./tools/test_windows_installer.ps1", windows)
        self.assertIn("64-bit installer: installed, 64-bit exe, no DLL files, libraries load, uninstalled cleanly", installer)
        for check in ("'assemblies','ui','startup'", "installed exe is not 64-bit", "uninstall left the exe behind"):
            self.assertIn(check, installer)
        self.assertIn("actions/upload-artifact@v6", windows)

    def test_release_and_version_summaries_are_preserved(self):
        workflow = (ROOT / ".github/workflows/build.yml").read_text()
        release = workflow.split("\n  release:\n", 1)[1]
        self.assertEqual(release.count("GITHUB_STEP_SUMMARY"), 3)
        self.assertIn('echo "### Not published: ${TAG}', release)
        self.assertIn('echo "### Released ${TAG}"', release)
        self.assertIn('cat "${RUNNER_TEMP}/version-comment.md" >> "$GITHUB_STEP_SUMMARY"', release)
        self.assertIn("needs: [version, windows-build, documentation, secrets-guard, signing-policy, windows-sign, cleanup-signing-input]", release)
        self.assertIn("needs.windows-sign.result == 'success'", release)
        self.assertIn("needs.cleanup-signing-input.result == 'success'", release)

    def test_actions_keep_the_reviewed_major_version_policy(self):
        workflow = (ROOT / ".github/workflows/build.yml").read_text()
        actions = re.findall(r"uses:\s+([^\s]+)", workflow)
        self.assertEqual(len(actions), 15)
        for action in actions:
            with self.subTest(action=action):
                self.assertRegex(action, r"@v\d+$")

    def test_all_build_warnings_fail_both_platform_builds(self):
        workflow = (ROOT / ".github/workflows/build.yml").read_text()
        commands = [line for line in workflow.splitlines() if "run: msbuild" in line]
        self.assertEqual(len(commands), 2)
        for command in commands:
            self.assertRegex(command, r"/warnaserror$")
        self.assertNotIn("/nowarn", workflow.lower())

    def test_unused_http_reference_is_not_forced_to_an_older_version(self):
        project = (PROJ / "SPI Data Comp Tool.csproj").read_text(encoding="utf-8-sig")
        config = (PROJ / "App.config").read_text(encoding="utf-8-sig")
        self.assertNotIn('<Reference Include="System.Net.Http"', project)
        self.assertNotIn('<assemblyIdentity name="System.Net.Http"', config)
        self.assertIn("<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>", project)

    def test_every_form_must_open(self):
        workflow = (ROOT / ".github/workflows/build.yml").read_text()
        self.assertIn('if ($failed) { throw', workflow)
        self.assertIn("'--open-form'", workflow)
        self.assertIn("Cannot enumerate forms", workflow)
        self.assertIn("$p.ExitCode -ne 0", workflow)

    def test_real_control_checks_run_before_packaging_without_images(self):
        workflow = (ROOT / ".github/workflows/build.yml").read_text()
        self.assertLess(workflow.index("UI workflow regression checks"), workflow.index("Open every window"))
        self.assertIn("--check-ui", workflow)
        self.assertIn("UiWorkflowChecks.Run(args[1])", (PROJ / "SelfTest.cs").read_text(encoding="utf-8-sig"))
        project = (PROJ / "SPI Data Comp Tool.csproj").read_text(encoding="utf-8-sig")
        for file in ("UiWorkflowChecks.cs", "ProfileWorkflowChecks.cs", "ImportSchemaChecks.cs", "CredentialRedactionChecks.cs"):
            self.assertIn(file, project)
        self.assertIn("python3 -m unittest discover -s tests -p 'test_*.py'", workflow)
        self.assertNotIn("DrawToBitmap", (ROOT / "tests/UiWorkflowChecks.cs").read_text())

    def test_window_navigation_preserves_modes_and_explicit_parallel_sessions(self):
        navigation = (PROJ / "WindowNavigator.cs").read_text()
        main = (PROJ / "MainForm.cs").read_text(encoding="utf-8-sig")
        catalog = (PROJ / "WorkspaceCatalog.cs").read_text()
        self.assertIn("Dictionary<WorkspaceKind, WorkspaceForm>", navigation)
        self.assertIn("FormClosed +=", navigation)
        self.assertIn("bool separateSession = false", navigation)
        self.assertIn("separateSession && task.AllowSeparateSession", navigation)
        # "New session" is the only way to a parallel session: no hidden Ctrl+click route.
        self.assertNotIn("ModifierKeys", main)
        self.assertIn("separate ? workspaces.Open(this, task.Key, true) : shell.OpenTask(task.Key)", main)
        self.assertIn('() => new User_Settings(), false)', catalog)
        self.assertIn('() => new UserProfileForm(), false)', catalog)
        self.assertIn("TryPrepareCloseAll(e.CloseReason)", main)
        self.assertNotIn("MyException", main)


if __name__ == "__main__":
    unittest.main()
