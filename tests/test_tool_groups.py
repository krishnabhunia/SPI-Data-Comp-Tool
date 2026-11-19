"""Related tools share one window with a tab per tool (ToolGroups.cs); the tools themselves are unchanged."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJ = ROOT / "SPI Data Comp Tool"

# group -> the windows shown as its tabs, in tab order
GROUPS = {
    "PreMerging": ["PreMergingTool_Form", "Pre_Merging_Duplicacy_Form", "Pre_Merging_Duplicacy_Form"],
    "ExcelTools": ["CustomisedExcel", "MergeLogsExport", "SPI_Spec_Note_Creation_Utility", "Generate_IUD_Script_Form"],
    "Delta": ["Table_Extraction_After_Delta_Form", "SPIDeltaChangeLegacyID"],
}


def code(name: str) -> str:
    return (PROJ / name).read_text(encoding="utf-8-sig")


class ToolGroups(unittest.TestCase):
    def test_each_group_lists_its_tools_in_order(self) -> None:
        groups = code("ToolGroups.cs")
        for group, windows in GROUPS.items():
            with self.subTest(group=group):
                block = groups.split(f"case {group}:", 1)[1].split("break;", 1)[0]
                self.assertEqual(re.findall(r"window\.AddTool\(\"[^\"]+\", typeof\((\w+)\)", block), windows)
        self.assertIn("public static readonly string[] All = { PreMerging, ExcelTools, Delta };", groups)

    def test_menu_entries_open_the_tab_and_ctrl_still_opens_a_separate_window(self) -> None:
        main = code("MainForm.cs")
        for group, windows in GROUPS.items():
            for window in set(windows):
                with self.subTest(window=window):
                    self.assertRegex(main, rf"OpenTool\(ToolGroups\.{group}, \(\) => new {window}\(")
                    self.assertNotRegex(main, rf"OpenWindow\(\(\) => new {window}\(")
        opener = main.split("private void OpenTool<T>", 1)[1].split("private void aboutUsToolStripMenuItem_Click", 1)[0]
        self.assertIn("if (allowSeparateSession && (ModifierKeys & Keys.Control) == Keys.Control)", opener)
        self.assertIn("windows.Open(this, create, mode, true);", opener)
        self.assertIn("windows.Open(this, () => ToolGroups.Create(group), group);", opener)   # one window per group
        self.assertIn("host.ShowTool(ToolGroupWindow.KeyOf(typeof(T), mode), () => create());", opener)

    def test_the_two_pre_merging_modes_stay_separate_tabs(self) -> None:
        groups = code("ToolGroups.cs")
        self.assertIn('typeof(Pre_Merging_Duplicacy_Form), "check", () => new Pre_Merging_Duplicacy_Form());', groups)
        self.assertIn('typeof(Pre_Merging_Duplicacy_Form), "configuration", () => new Pre_Merging_Duplicacy_Form(true));', groups)

    def test_a_tool_is_built_once_inside_its_tab_and_closes_with_the_window(self) -> None:
        window = code("ToolGroups.cs").split("internal sealed class ToolGroupWindow", 1)[1]
        fill = window.split("private void Fill(TabPage page)", 1)[1].split("private void MakeRoom", 1)[0]
        self.assertIn("if (slot.Tool != null && !slot.Tool.IsDisposed) return;", fill)   # built once
        self.assertLess(fill.index("tool.TopLevel = false;"), fill.index("page.Controls.Add(tool);"))
        self.assertLess(fill.index("UiTheme.EnsureApplied(tool);"), fill.index("page.Controls.Add(tool);"))
        self.assertIn("tool.Dispose();", fill)                                             # a tool that fails to open leaves nothing behind
        closing = window.split("private void CloseTools", 1)[1]
        # all tools or none: every tool is asked before any is closed, so a busy tool keeps every tab as it was
        self.assertLess(closing.index("if (!Refuses(pair.Value, e.CloseReason)) continue;"), closing.index("pair.Value.Close();"))
        self.assertGreaterEqual(closing.count("e.Cancel = true;"), 2)
        self.assertIn("Math.Min(screen.Width", window)                                      # never larger than the screen

    def test_windows_have_readable_titles(self) -> None:
        for designer in sorted(PROJ.glob("*.Designer.cs")):
            titles = re.findall(r'^\s*this\.Text = "([^"]*)";', designer.read_text(encoding="utf-8-sig"), re.M)
            for title in titles[-1:]:
                with self.subTest(window=designer.name):
                    self.assertNotIn("_", title)
                    self.assertNotRegex(title, r"[a-z][A-Z]")   # no CodeNames

    def test_ci_builds_every_tab(self) -> None:
        workflow = (ROOT / ".github/workflows/build.yml").read_text(encoding="utf-8")
        self.assertIn("-ArgumentList '--check-groups'", workflow)
        self.assertIn('args[0] == "--check-groups"', code("SelfTest.cs"))
        project = code("SPI Data Comp Tool.csproj")
        self.assertIn('<Compile Include="ToolGroups.cs" />', project)
        self.assertIn(r'<Compile Include="..\tests\ToolGroupChecks.cs">', project)


if __name__ == "__main__":
    unittest.main()
