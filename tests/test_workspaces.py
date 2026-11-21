"""Source and generator contracts; the Windows suite exercises real workspace controls."""

import importlib.util
import re
import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


def source(name):
    return (APP / name).read_text(encoding="utf-8-sig")


class WorkspaceContracts(unittest.TestCase):
    def test_catalog_retains_existing_routes_and_adds_excel_audit(self):
        catalog = source("WorkspaceCatalog.cs")
        routes = re.findall(r'new WorkspaceTask\("([^"]+)"', catalog)
        self.assertEqual(len(routes), 20)
        self.assertEqual(len(set(routes)), 20)
        expected = {"comparison": 4, "analysis": 6, "premerge": 3, "scripts": 2, "reports": 3, "settings": 2}
        self.assertIn("comparison.excel", routes)
        self.assertIn("comparison.excel-audit", routes)
        for group, count in expected.items():
            self.assertEqual(sum(route.startswith(group + ".") for route in routes), count)
        self.assertIn("() => new Pre_Merging_Duplicacy_Form(true), false", catalog)
        self.assertIn("() => new User_Settings(), false", catalog)
        self.assertIn("() => new UserProfileForm(), false", catalog)

    def test_all_twenty_pages_keep_the_lifecycle_bridge(self):
        pages = []
        for path in APP.glob("*.cs"):
            pages += re.findall(r"public (?:(?:partial|sealed) )?class (\w+) : WorkspacePageForm", path.read_text(encoding="utf-8-sig"))
        self.assertEqual(len(pages), 20)
        self.assertEqual(len(set(pages)), 20)
        self.assertIn("public abstract class WorkspacePageForm : Form", source("WorkspacePageForm.cs"))

    def test_preparation_does_not_dispose_or_close_pages(self):
        page = source("WorkspacePageForm.cs")
        prepare = page.split("internal bool PrepareWorkspaceClose", 1)[1].split("internal void CompleteWorkspaceClose", 1)[0]
        self.assertIn("OnFormClosing(args)", prepare)
        self.assertNotIn("Dispose()", prepare)
        self.assertNotIn("Close()", prepare)
        host = source("WorkspaceForm.cs")
        prepare = host.split("internal bool TryPrepareWorkspaceClose", 1)[1].split("protected override void OnFormClosing", 1)[0]
        self.assertIn("pages.ToArray()", prepare)
        self.assertIn("catch (Exception ex)", prepare)
        self.assertNotIn("CompleteWorkspaceClose", prepare)
        self.assertNotIn("Dispose()", prepare)
        self.assertIn("foreach (var page in snapshot) page.CompleteWorkspaceClose();", host)

    def test_transient_mapping_and_singleton_navigation_use_correct_scope(self):
        host = source("WorkspaceForm.cs")
        navigation = host.split("private void NavigateSafely", 1)[1].split("internal WorkspacePageForm OpenTask", 1)[0]
        self.assertIn('!task.Key.StartsWith("transient.", StringComparison.Ordinal)', navigation)
        self.assertIn("!task.AllowSeparateSession || separate", navigation)
        self.assertIn("else OpenTask(task.Key)", navigation)
        for file in ("DB Comparison.cs", "Excel_Excel_Comparison.cs"):
            self.assertIn("WorkspaceCatalog.OpenAdvancedMapping(this, () => new ViewForMultiReference(ds, ds))", source(file))

    def test_update_policy_inspects_all_loaded_tasks(self):
        policy = source("UpdateInstaller.cs").split("internal static bool CanInstallForms", 1)[1].split("internal static bool HasBusyWorker", 1)[0]
        self.assertIn("workspace.Workspace != WorkspaceKind.Settings", policy)
        self.assertIn("workspace.LoadedPages.Cast<Form>()", policy)
        self.assertIn("HasBusyWorker(workspace)", policy)
        self.assertNotIn(".Visible", policy)

    def test_menu_and_home_navigation_share_catalog_routes(self):
        main = source("MainForm.cs")
        self.assertIn("WorkspaceCatalog.TasksFor(kind)", main)
        self.assertIn("Tag = task.Key", main)
        self.assertIn('workspaces.Open(this, "settings.preferences")', main)
        self.assertIn("workspaces.TryPrepareCloseAll(e.CloseReason)", main)
        self.assertIn("OwnedForms.OfType<WorkspaceForm>()", main)
        designer = source("MainForm.Designer.cs")
        self.assertNotIn("this.helpToolStripMenuItem", designer)
        tiles = re.findall(r"private SPI_Data_Comp_Tool.TileButton (btn_Workspace\w+);", designer)
        self.assertEqual(set(tiles), {"btn_Workspace" + group for group in
                                     ("Comparison", "Analysis", "PreMerge", "Scripts", "Reports", "Settings")})
        self.assertEqual(len(tiles), 6)

    def test_offline_windows_checks_are_compiled_and_invoked(self):
        project = source("SPI Data Comp Tool.csproj")
        runner = (ROOT / "tests/UiWorkflowChecks.cs").read_text()
        self.assertIn("..\\tests\\WorkspaceUiChecks.cs", project)
        self.assertIn("WorkspaceUiChecks.Run(passed);", runner)
        for file in ("WorkspacePageForm.cs", "WorkspaceCatalog.cs", "WorkspaceForm.cs"):
            self.assertIn('<Compile Include="' + file + '"', project)

    def test_generator_escapes_text_and_workspace_tiles_use_distinct_hints(self):
        spec = importlib.util.spec_from_file_location("workspace_layout_generator", ROOT / "tools/layout_gen.py")
        generator = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(generator)
        self.assertEqual(generator.cs('one\ntwo\r\n"quoted"\\path\tvalue'), 'one\\ntwo\\r\\n\\"quoted\\"\\\\path\\tvalue')
        designer = source("MainForm.Designer.cs")
        titles = re.findall(r'this\.(btn_Workspace\w+)\.Text = "([^"\n]+)";', designer)
        hints = dict(re.findall(r'this\.(btn_Workspace\w+)\.Hint = "([^"\n]+)";', designer))
        self.assertEqual(len(titles), 6)
        self.assertEqual(len(hints), 6)
        for tile, title in titles:
            with self.subTest(tile=tile):
                self.assertNotIn("\\n", title)
                self.assertNotIn("\\r", title)
                self.assertIn(tile, hints)
                self.assertTrue(hints[tile].strip())
                self.assertNotEqual(title, hints[tile])


if __name__ == "__main__":
    unittest.main()
