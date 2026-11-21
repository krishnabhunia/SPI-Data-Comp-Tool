"""Static contracts for the B6 clean-up: no unused libraries, no Excel at start-up, named panel boxes."""

import re
import unittest
import xml.etree.ElementTree as ET
from pathlib import Path

PROJ = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"
UNUSED = ("EntityFramework", "System.IO.Packaging", "System.IO.FileSystem.Primitives")


def text(name: str) -> str:
    return (PROJ / name).read_text(encoding="utf-8-sig")


def code(name: str) -> str:
    """Source without // comments, so commented-out lines don't count."""
    return re.sub(r"//[^\n]*", "", text(name))


class Cleanup(unittest.TestCase):
    def test_unused_libraries_are_not_shipped(self) -> None:
        packages = {p.get("id") for p in ET.fromstring(text("packages.config"))}
        project = text("SPI Data Comp Tool.csproj")
        config = text("App.config")
        for lib in UNUSED:
            with self.subTest(lib=lib):
                self.assertNotIn(lib, packages)
                self.assertNotIn(lib, project)
                self.assertNotIn(lib, config)

    def test_libraries_still_needed_are_kept(self) -> None:
        packages = {p.get("id") for p in ET.fromstring(text("packages.config"))}
        for lib in ("System.ValueTuple", "System.ComponentModel.Annotations", "DocumentFormat.OpenXml"):
            self.assertIn(lib, packages)

    def test_excel_window_does_not_start_excel_when_it_opens(self) -> None:
        src = code("Excel_Excel_Comparison.cs")
        ctor = src.split("public Excel_Excel_Comparison(", 1)[1].split("\n        }\n", 1)[0]
        self.assertNotIn("new _Excel.Application", ctor)

    def test_excel_is_closed_after_reading_the_sheet_name(self) -> None:
        src = code("Excel_Excel_Comparison.cs")
        load = src.split("private DataSet LoadExcel(", 1)[1].split("\n        }\n", 1)[0]
        self.assertIn("finally", load)
        self.assertIn("ReleaseExcel();", load)
        release = src.split("private void ReleaseExcel()", 1)[1].split("\n        }\n", 1)[0]
        for step in ("excelWorkbook.Close(false);", "excelApp.Quit();", "Marshal.ReleaseComObject(excelApp);"):
            self.assertIn(step, release)

    def test_panel_text_boxes_have_names(self) -> None:
        src = code("ConnectionSide.cs")
        for box in ("Port", "DataSource", "Database", "User", "Password"):
            self.assertRegex(src, rf"NameField\({box}, \"[a-z ]+\"\);")
        # the name follows the title when a window renames a half
        self.assertIn("set { title = value; NameFields();", src)


class EveryWindowCanBeOpened(unittest.TestCase):
    """A window nobody can open is dead weight: delete it instead of hiding its menu entry."""

    def test_the_first_delta_tracker_window_is_gone(self) -> None:
        for name in ("SPIDeltaChangeTracker.cs", "SPIDeltaChangeTracker.Designer.cs", "SPIDeltaChangeTracker.resx"):
            self.assertFalse((PROJ / name).exists(), name)
        project = text("SPI Data Comp Tool.csproj")
        self.assertNotIn("SPIDeltaChangeTracker", project)

    def test_main_menu_has_no_hidden_entries(self) -> None:
        hidden = re.findall(r"this\.(\w+ToolStripMenuItem)\.Visible = false;", text("MainForm.Designer.cs"))
        self.assertEqual(hidden, [])

    def test_every_window_is_opened_somewhere(self) -> None:
        sources = {p.name: re.sub(r"//[^\n]*", "", p.read_text(encoding="utf-8-sig")) for p in PROJ.glob("*.cs")}
        windows = {}
        for name, src in sources.items():
            if not name.endswith(".Designer.cs"):
                for m in re.finditer(r"(?<!abstract )class\s+(\w+)\s*:\s*(?:Form|WorkspacePageForm|UserControl)\b", src):
                    windows[m.group(1)] = name
        self.assertEqual(len(windows), 16)
        for window, home in windows.items():
            own_designer = home[:-3] + ".Designer.cs"
            opened = any(re.search(rf"new\s+(?:SPI_Data_Comp_Tool\.)?{window}\s*\(", src)
                         for name, src in sources.items() if name != own_designer)
            with self.subTest(window=window):
                self.assertTrue(opened, f"{window} is never created - delete it or give it a menu entry")


class NoScreenshots(unittest.TestCase):
    """Builds open every window as a check, but take and publish no pictures."""

    REPO = PROJ.parent

    def test_nothing_takes_or_publishes_screenshots(self) -> None:
        self.assertFalse((PROJ / "FormScreenshots.cs").exists())
        for rel in (".github/workflows/build.yml", "tools/release.py", "docs/PROJECT_GUIDE.md", "README.md"):
            with self.subTest(file=rel):
                self.assertNotIn("screenshot", (self.REPO / rel).read_text(encoding="utf-8").lower())
        selftest = code("SelfTest.cs")
        for word in ("--screenshot", "Bitmap", "DrawToBitmap", ".png"):
            self.assertNotIn(word, selftest)

    def test_build_fails_when_a_window_cannot_open(self) -> None:
        workflow = (self.REPO / ".github/workflows/build.yml").read_text(encoding="utf-8")
        step = workflow.split("- name: Open every window", 1)[1].split("\n      - name:", 1)[0]
        self.assertIn("'--open-form'", step)
        self.assertIn('if ($failed) { throw', step)
        self.assertIn('"--open-form"', code("SelfTest.cs"))
        self.assertIn("if (SelfTest.TryRun(args)) return;", code("Program.cs"))


if __name__ == "__main__":
    unittest.main()
