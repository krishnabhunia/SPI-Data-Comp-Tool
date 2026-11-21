"""Colour themes: four palettes, chosen in Settings, applied to every window, remembered outside the app folder."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJ = ROOT / "SPI Data Comp Tool"


def code(name: str) -> str:
    return (PROJ / name).read_text(encoding="utf-8-sig")


class Themes(unittest.TestCase):
    def test_theme_files_are_part_of_the_build(self) -> None:
        project = code("SPI Data Comp Tool.csproj")
        for file in ('"UiTheme.cs"', '"ThemeControls.cs"', r'"..\tests\ThemeChecks.cs"'):
            self.assertIn(f"<Compile Include={file}", project)

    def test_choice_is_saved_outside_the_app_folder_and_bad_files_fall_back(self) -> None:
        theme = code("UiTheme.cs")
        store = theme.split("internal static string StorePath", 1)[1].split("private static UiPalette Load()", 1)[0]
        self.assertIn("Environment.SpecialFolder.ApplicationData", store)
        self.assertIn('"SPI Data Comparison Tool", "theme.txt"', store)
        read = theme.split("internal static UiPalette ReadChoice(string path)", 1)[1].split("internal static void SaveChoice", 1)[0]
        self.assertIn("file.Length <= 64", read)                 # never reads a large file as a theme name
        self.assertIn("?? All[0]", read)                        # an unknown name is the default theme
        self.assertGreaterEqual(read.count("return All[0];"), 1)  # a missing or unreadable file too

    def test_build_checks_never_touch_the_users_choice(self) -> None:
        theme = code("UiTheme.cs")
        self.assertIn("return SelfTest.IsRunning ? All[0] : ReadChoice(StorePath);", theme)
        self.assertIn("if (!save || SelfTest.IsRunning) return string.Empty;", theme)

    def test_a_theme_change_reaches_every_open_window(self) -> None:
        theme = code("UiTheme.cs")
        use = theme.split("public static string Use(UiPalette palette, bool save = true)", 1)[1].split("// ---------------- applying a theme", 1)[0]
        self.assertIn("foreach (Form form in themed.ToArray())", use)
        self.assertIn("ToolStripManager.Renderer = new MenuRenderer();", use)   # menus and their drop-downs
        self.assertLess(use.index("Paint(form, darknessChanged);"), use.index("SaveChoice(StorePath, palette);"))
        self.assertIn("form.Disposed += (s, e) => themed.Remove((Form)s);", theme)  # closed windows are forgotten
        self.assertIn("form.ControlAdded += OnControlAdded;", theme)                # controls created later are themed too

    def test_every_tool_window_is_themed(self) -> None:
        navigator = code("WindowNavigator.cs")
        self.assertLess(navigator.index("UiTheme.EnsureApplied(window);"), navigator.index("window.Show(owner);"))
        host = code("WorkspaceForm.cs")
        self.assertLess(host.index("UiTheme.EnsureApplied(page);"), host.index("content.Controls.Add(page);"))
        self.assertLess(host.index("UiTheme.EnsureApplied(page);"), host.index("page.Show();"))
        self.assertIn("UiTheme.Apply(this);", code("AboutBox1.cs"))
        self.assertIn("Load += (sender, e) => UiTheme.Apply(this);", code("ViewForMultiReference.cs"))

    def test_settings_offers_the_themes_without_adding_a_tab(self) -> None:
        designer = code("User_Settings.Designer.cs")
        self.assertIn("this.tlp_General.Controls.Add(this.gb_Theme, 0, 1);", designer)
        self.assertIn("this.tlp_Theme.Controls.Add(this.cb_Theme, 1, 0);", designer)
        self.assertIn("this.cb_Theme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;", designer)
        self.assertEqual(len(re.findall(r"this\.tabs_Settings\.Controls\.Add\(", designer)), 3)
        settings = code("User_Settings.cs")
        self.assertIn("cb_Theme.Items.AddRange(UiTheme.All);", settings)
        self.assertIn("cb_Theme.SelectedItem = UiTheme.Current;", settings)
        handler = settings.split("private void cb_Theme_SelectedIndexChanged", 1)[1].split("private void rb_EPPlus_CheckedChanged", 1)[0]
        self.assertIn("if (initializing || palette == null || palette == UiTheme.Current) return;", handler)
        self.assertIn("string notSaved = UiTheme.Use(palette);", handler)
        self.assertIn("could not be saved", handler)   # the user is told when the choice will not survive a restart

    def test_main_window_tiles_open_the_same_tasks_as_the_menus(self) -> None:
        main = code("MainForm.cs")
        catalog = code("WorkspaceCatalog.cs")
        for tile, route in (("Comparison", "comparison.database"), ("Scripts", "scripts.generate"),
                            ("Reports", "reports.database"), ("Settings", "settings.preferences")):
            with self.subTest(tile=tile):
                self.assertIn(f'btn_Workspace{tile}.Click += (sender, e) => OpenTask("{route}");', main)
                self.assertIn(f'new WorkspaceTask("{route}"', catalog)
        # Every menu entry and the four home tiles use the same route dispatcher and catalog.
        self.assertIn("foreach (var task in WorkspaceCatalog.TasksFor(kind))", main)
        self.assertIn("Tag = task.Key", main)
        self.assertIn("item.Click += (sender, e) => OpenTask(route.Key);", main)
        self.assertIn("workspaces.Open(this, route,", main)
        self.assertIn("UiTheme.MarkHeader(pnl_Header);", main)
        self.assertIn("UiTheme.MarkStatus(pnl_Status);", main)
        # File > Exit closes the application root through the same busy-page preflight as its X.
        self.assertIn('file.DropDownItems.Add("E&xit", null, (sender, e) => Close());', main)
        self.assertIn("protected override void OnFormClosing(FormClosingEventArgs e)", main)
        self.assertIn("workspaces.TryPrepareCloseAll(e.CloseReason)", main)

    def test_workspace_selection_and_hidden_pages_follow_theme_changes(self) -> None:
        host = code("WorkspaceForm.cs")
        self.assertIn("UiTheme.Changed += Theme_Changed;", host)
        self.assertIn("Disposed += (sender, args) => UiTheme.Changed -= Theme_Changed;", host)
        selection = host.split("private void ApplyNavigationTheme()", 1)[1].split("private void NavigateSafely", 1)[0]
        self.assertIn("selected ? palette.Accent : palette.CardBack", selection)
        self.assertIn("selected ? palette.AccentText : palette.TextColor", selection)
        self.assertIn("selected ? palette.AccentHover : palette.HoverBack", selection)
        self.assertIn("selected ? palette.AccentPressed : palette.PressedBack", selection)
        checks = (ROOT / "tests/ThemeChecks.cs").read_text(encoding="utf-8")
        self.assertIn("HostedWorkspaces", checks)
        self.assertIn("foreach (WorkspacePageForm page in comparison.LoadedPages)", checks)
        self.assertIn("Require(Inspect(page, p));", checks)

    def test_main_window_ships_no_unused_pictures(self) -> None:
        designer = code("MainForm.Designer.cs")
        self.assertNotIn("pictureBox", designer)
        self.assertFalse((PROJ / "Resources" / "app_banner.png").exists())
        self.assertFalse((PROJ / "Resources" / "compare_diagram.png").exists())
        self.assertNotIn("app_banner", code("Properties/Resources.resx"))
        self.assertTrue((PROJ / "Resources" / "about_side.png").exists())

    def test_source_and_target_keep_their_colours(self) -> None:
        self.assertIn("UiTheme.MarkSide(Source.Group, true);", code("ConnectionPanel.cs"))
        self.assertIn("UiTheme.MarkSide(Target.Group, false);", code("ConnectionPanel.cs"))
        theme = code("UiTheme.cs")
        self.assertIn("side == SideSource ? p.SourceTint : p.TargetTint", theme)
        self.assertIn("grid.ColumnAdded += (s, e) => TintColumn(e.Column, Current);", theme)

    def test_log_colours_go_through_the_theme(self) -> None:
        log = code("Common_Data.cs").split("internal static void DisplayMessage(RichTextBox rtb_Log", 1)[1].split("internal static", 1)[0]
        self.assertEqual(len(re.findall(r"rtb_Log\.SelectionColor = (?!UiTheme\.Readable\()", log)), 0)
        self.assertGreaterEqual(log.count("rtb_Log.SelectionColor = UiTheme.Readable("), 4)

    def test_no_window_writes_log_colours_past_the_theme(self) -> None:
        # a logger of its own (User profiles has one) must use the same mapping as the shared logger
        for file in sorted(PROJ.glob("*.cs")):
            if file.name.endswith(".Designer.cs") or file.name == "UiTheme.cs":
                continue
            for number, line in enumerate(file.read_text(encoding="utf-8-sig").splitlines(), 1):
                if line.strip().startswith("//") or "SelectionColor =" not in line:
                    continue
                with self.subTest(file=file.name, line=number):
                    self.assertIn("SelectionColor = UiTheme.Readable(", line)

    def test_every_open_settings_window_shows_the_theme_in_use(self) -> None:
        settings = code("User_Settings.cs")
        self.assertIn("UiTheme.Changed += Theme_Changed;", settings)
        self.assertIn("Disposed += (sender, e) => UiTheme.Changed -= Theme_Changed;", settings)   # no closed window is kept alive
        sync = settings.split("private void Theme_Changed", 1)[1].split("private void cb_Theme_SelectedIndexChanged", 1)[0]
        self.assertIn("cb_Theme.SelectedItem = UiTheme.Current;", sync)
        self.assertLess(sync.index("initializing = true;"), sync.index("cb_Theme.SelectedItem = UiTheme.Current;"))

    def test_ci_checks_every_window_in_every_theme(self) -> None:
        workflow = (ROOT / ".github/workflows/build.yml").read_text(encoding="utf-8")
        self.assertIn("-ArgumentList '--check-themes'", workflow)
        self.assertIn('args[0] == "--check-themes"', code("SelfTest.cs"))
        checks = (ROOT / "tests/ThemeChecks.cs").read_text(encoding="utf-8")
        self.assertIn("foreach (UiPalette palette in UiTheme.All)", checks)
        self.assertIn("private const double Readable = 4.5;", checks)
        self.assertIn("LiveSwitch", checks)
        # a self-test that does not finish says which dialog it was waiting on
        self.assertGreaterEqual(workflow.count("& tools/ci_window_report.ps1 -ProcessId"), 4)
        self.assertTrue((ROOT / "tools/ci_window_report.ps1").exists())

    def test_windows_only_calls_cannot_stop_the_app(self) -> None:
        theme = code("UiTheme.cs")
        tail = theme.split("// ---------------- the parts Windows draws itself", 1)[1]
        self.assertEqual(tail.count("DwmSetWindowAttribute(form.Handle"), 2)
        self.assertEqual(tail.count("SetWindowTheme(c.Handle"), 1)
        self.assertGreaterEqual(tail.count("catch (Exception)"), 4)   # title bar, scroll bars, progress bar (twice)
        self.assertGreaterEqual(tail.count("if (!OnWindows"), 2)
        # a light theme asks Windows for nothing unless a dark theme was applied to that window before
        self.assertIn("|| dark == look.DarkParts) return;", tail)
        self.assertIn("|| p.Dark == look.DarkParts) return;", tail)
        self.assertNotIn("RecreateHandle", theme)
        # the progress bar takes the accent colour only after its window exists, outside its creation
        self.assertEqual(tail.count("SetWindowTheme(bar.Handle, string.Empty, string.Empty);"), 1)
        self.assertIn("bar.BeginInvoke((MethodInvoker)(() => PlainBar(bar)));", tail)
        self.assertIn("if (bar.IsDisposed || !bar.IsHandleCreated) return;", tail)
        self.assertNotIn(".Style = ProgressBarStyle", theme)   # changing it rebuilds the bar's window


if __name__ == "__main__":
    unittest.main()
