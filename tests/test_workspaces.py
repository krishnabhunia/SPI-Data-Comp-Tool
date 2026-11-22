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
    def test_catalog_contains_only_supported_task_routes(self):
        catalog = source("WorkspaceCatalog.cs")
        rail = catalog.split("private static readonly ReadOnlyCollection<WorkspaceTask> tasks", 1)[1].split("});", 1)[0]
        modes = catalog.split("private static readonly ReadOnlyCollection<WorkspaceTask> modes", 1)[1].split("});", 1)[0]
        routes = re.findall(r'new WorkspaceTask\("([^"]+)"', rail)
        self.assertEqual(len(routes), 7)
        self.assertEqual(len(set(routes)), 7)
        expected = {"comparison": 2, "reports": 3, "settings": 2}
        for group, count in expected.items():
            self.assertEqual(sum(route.startswith(group + ".") for route in routes), count)
        self.assertEqual(routes[:2], ["comparison.databases", "comparison.workbooks"])
        # The six existing comparison workflows stay, reachable only through the two Compare tasks' mode bars.
        mode_keys = re.findall(r'new WorkspaceTask\("([^"]+)"', modes)
        self.assertEqual(mode_keys, ["comparison.database", "comparison.batch", "comparison.excel", "comparison.excel-audit",
                                     "comparison.identity", "comparison.batch-folders"])
        self.assertFalse(set(mode_keys) & set(routes))
        self.assertIn("() => new User_Settings(), false", catalog)
        self.assertIn("() => new UserProfileForm(), false", catalog)

    def test_all_eleven_pages_keep_the_lifecycle_bridge(self):
        pages = []
        for path in APP.glob("*.cs"):
            pages += re.findall(r"public (?:(?:partial|sealed) )?class (\w+) : WorkspacePageForm", path.read_text(encoding="utf-8-sig"))
        self.assertEqual(len(pages), 12)  # 11 workflow pages + the Compare mode host
        self.assertEqual(len(set(pages)), 12)
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
        self.assertIn("!workspace.Grouped && workspace.Workspace != WorkspaceKind.Settings", policy)
        self.assertIn("!(form is AboutBox1)", policy)
        self.assertIn("workspace.LoadedPages.Cast<Form>()", policy)
        self.assertIn("HasBusyWorker(workspace)", policy)
        self.assertNotIn(".Visible", policy)

    def test_one_rail_is_the_only_way_to_open_a_task(self):
        main = source("MainForm.cs")
        self.assertIn('new WorkspaceForm(WorkspaceCatalog.Tasks, "SPI Data Comparison")', main)
        self.assertIn('shell.OpenTask("settings.preferences")', main)
        self.assertIn("shell.TryPrepareWorkspaceClose(e.CloseReason)", main)
        self.assertIn("workspaces.TryPrepareCloseAll(e.CloseReason)", main)
        self.assertIn("OwnedForms.OfType<WorkspaceForm>()", main)
        designer = source("MainForm.Designer.cs")
        for removed in ("helpToolStripMenuItem", "menuStrip1", "TileButton", "btn_Workspace"):
            self.assertNotIn(removed, designer)
        workspace = source("WorkspaceForm.cs")
        # The shell lists every task once, under its group; a New session window has no rail of its own.
        self.assertIn('Name = "group_" + task.Workspace', workspace)
        self.assertIn("internal bool ShowNavigation", workspace)
        navigator = source("WindowNavigator.cs")
        self.assertIn("if (openInMain != null && !separate) return openInMain(routeKey);", navigator)
        self.assertIn("window.ShowNavigation = false;", navigator)

    def test_offline_windows_checks_are_compiled_and_invoked(self):
        project = source("SPI Data Comp Tool.csproj")
        runner = (ROOT / "tests/UiWorkflowChecks.cs").read_text()
        self.assertIn("..\\tests\\WorkspaceUiChecks.cs", project)
        self.assertIn("WorkspaceUiChecks.Run(passed);", runner)
        for file in ("WorkspacePageForm.cs", "WorkspaceCatalog.cs", "WorkspaceForm.cs"):
            self.assertIn('<Compile Include="' + file + '"', project)

    def test_generator_escapes_text_and_rail_tasks_use_catalog_titles(self):
        spec = importlib.util.spec_from_file_location("workspace_layout_generator", ROOT / "tools/layout_gen.py")
        generator = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(generator)
        self.assertEqual(generator.cs('one\ntwo\r\n"quoted"\\path\tvalue'), 'one\\ntwo\\r\\n\\"quoted\\"\\\\path\\tvalue')
        workspace = source("WorkspaceForm.cs")
        # Rail buttons show the catalog title and expose the description to screen readers.
        self.assertIn("Text = task.Title", workspace)
        self.assertIn("AccessibleDescription = task.Description", workspace)
        self.assertIn("UseMnemonic = false", workspace)


if __name__ == "__main__":
    unittest.main()
