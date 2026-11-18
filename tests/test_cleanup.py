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


if __name__ == "__main__":
    unittest.main()
