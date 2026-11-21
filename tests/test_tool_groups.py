"""The remaining grouped tools retain their routes and close protection inside task workspaces."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJ = ROOT / "SPI Data Comp Tool"

# The retained group routes use task workspaces and unchanged workflow types.
ROUTES = {
    "reports.excel": ("DataReports", "CustomisedExcel", ""),
    "reports.logs": ("DataReports", "MergeLogsExport", ""),
}


def code(name: str) -> str:
    return (PROJ / name).read_text(encoding="utf-8-sig")


class GroupedToolCompatibility(unittest.TestCase):
    def test_every_former_group_route_keeps_its_workflow_and_current_workspace(self) -> None:
        catalog = code("WorkspaceCatalog.cs")
        routes = {
            key: (workspace, form, args)
            for key, workspace, form, args in re.findall(
                r'new WorkspaceTask\("([^"]+)",\s*"[^"]*",\s*"[^"]*",\s*'
                r'WorkspaceKind\.(\w+),\s*\(\) => new (\w+)\(([^)]*)\)', catalog
            )
        }
        for key, expected in ROUTES.items():
            with self.subTest(route=key):
                self.assertEqual(routes.get(key), expected)

    def test_navigation_has_one_workspace_authority(self) -> None:
        main = code("MainForm.cs")
        self.assertIn("WorkspaceCatalog.TasksFor(kind)", main)
        self.assertIn("OpenTask(route.Key)", main)
        self.assertIn("workspaces.Open(this, route, (ModifierKeys & Keys.Control) == Keys.Control)", main)
        self.assertNotIn("OpenTool<T>", main)
        for path in PROJ.glob("*.cs"):
            with self.subTest(file=path.name):
                self.assertNotRegex(path.read_text(encoding="utf-8-sig"), r"\bToolGroupWindow\b|\bToolGroups\.")
        self.assertFalse((PROJ / "ToolGroups.cs").exists())

    def test_windows_have_readable_titles(self) -> None:
        for designer in sorted(PROJ.glob("*.Designer.cs")):
            titles = re.findall(r'^\s*this\.Text = "([^"]*)";', designer.read_text(encoding="utf-8-sig"), re.M)
            for title in titles[-1:]:
                with self.subTest(window=designer.name):
                    self.assertNotIn("_", title)
                    self.assertNotRegex(title, r"[a-z][A-Z]")

    def test_ci_keeps_the_group_compatibility_gate_and_workspace_checks(self) -> None:
        workflow = (ROOT / ".github/workflows/build.yml").read_text(encoding="utf-8")
        self.assertIn("-ArgumentList '--check-groups'", workflow)
        self.assertIn('args[0] == "--check-groups"', code("SelfTest.cs"))
        self.assertIn("ToolGroupChecks.Run(args[1])", code("SelfTest.cs"))
        project = code("SPI Data Comp Tool.csproj")
        self.assertNotIn('<Compile Include="ToolGroups.cs"', project)
        for source in ("WorkspaceCatalog.cs", "WorkspaceForm.cs", "WorkspacePageForm.cs"):
            self.assertIn('<Compile Include="' + source + '"', project)
        self.assertIn(r'<Compile Include="..\tests\ToolGroupChecks.cs">', project)
        self.assertIn(r'<Compile Include="..\tests\WorkspaceUiChecks.cs">', project)
        runner = (ROOT / "tests/UiWorkflowChecks.cs").read_text(encoding="utf-8-sig")
        self.assertIn("WorkspaceUiChecks.Run(passed);", runner)


if __name__ == "__main__":
    unittest.main()
