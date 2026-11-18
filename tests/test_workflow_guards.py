"""Keep executable UI checks and supported Actions runtimes in the Windows pipeline."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJ = ROOT / "SPI Data Comp Tool"


class WorkflowGuards(unittest.TestCase):
    def test_actions_keep_the_reviewed_major_version_policy(self):
        workflow = (ROOT / ".github/workflows/build.yml").read_text()
        actions = re.findall(r"uses:\s+([^\s]+)", workflow)
        self.assertEqual(len(actions), 9)
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
        self.assertIn("<TargetFrameworkVersion>v4.6.2</TargetFrameworkVersion>", project)

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
        for file in ("UiWorkflowChecks.cs", "ProfileWorkflowChecks.cs", "ImportSchemaChecks.cs"):
            self.assertIn(file, project)
        self.assertIn("python3 -m unittest discover -s tests -p 'test_*.py'", workflow)
        self.assertNotIn("DrawToBitmap", (ROOT / "tests/UiWorkflowChecks.cs").read_text())

    def test_window_navigation_preserves_modes_and_explicit_parallel_sessions(self):
        navigation = (PROJ / "WindowNavigator.cs").read_text()
        main = (PROJ / "MainForm.cs").read_text(encoding="utf-8-sig")
        self.assertIn('typeof(T).FullName + ":" + mode', navigation)
        self.assertIn("FormClosed +=", navigation)
        self.assertIn("newWindow = false", navigation)
        self.assertIn("ModifierKeys & Keys.Control", main)
        self.assertIn('new Pre_Merging_Duplicacy_Form(), "check"', main)
        self.assertIn('new Pre_Merging_Duplicacy_Form(true), "configuration", false', main)
        self.assertNotIn("MyException", main)


if __name__ == "__main__":
    unittest.main()
