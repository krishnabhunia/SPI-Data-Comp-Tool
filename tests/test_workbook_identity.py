"""Source contracts of the workbook identity engine; the Windows C# checks exercise real workbooks."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJ = ROOT / "SPI Data Comp Tool"
ENGINE_FILES = ("WorkbookIdentityReader.cs", "WorkbookIdentityEngine.cs", "IdentityReportWriter.cs")


def code(name: str) -> str:
    return (PROJ / name).read_text(encoding="utf-8-sig")


class WorkbookIdentityContracts(unittest.TestCase):
    def test_engine_and_checks_are_built_and_run(self) -> None:
        project = code("SPI Data Comp Tool.csproj")
        for name in ENGINE_FILES:
            self.assertIn(f'<Compile Include="{name}" />', project)
        self.assertIn(r'<Compile Include="..\tests\WorkbookIdentityChecks.cs">', project)
        runner = (ROOT / "tests/UiWorkflowChecks.cs").read_text(encoding="utf-8-sig")
        self.assertIn("WorkbookIdentityChecks.Run(passed);", runner)

    def test_engine_has_no_window_code(self) -> None:
        # The engine is called from a background worker, so it must not touch controls or show dialogs.
        for name in ENGINE_FILES:
            text = code(name)
            for forbidden in ("System.Windows.Forms", "MessageBox", "System.Drawing", "Application.DoEvents"):
                self.assertNotIn(forbidden, text, f"{name} uses {forbidden}")

    def test_no_new_library_is_needed(self) -> None:
        packages = code("packages.config")
        for library in ("ClosedXML", "DocumentFormat.OpenXml"):
            self.assertIn(f'id="{library}"', packages)
        for name in ENGINE_FILES:
            for used in re.findall(r"^using ([\w.]+);", code(name), re.MULTILINE):
                self.assertTrue(used.startswith(("System", "ClosedXML", "DocumentFormat.OpenXml")), f"{name}: {used}")
        # EPPlus needs a licence and Excel automation needs Excel installed; the engine uses neither.
        for name in ENGINE_FILES:
            self.assertNotIn("OfficeOpenXml", code(name))
            self.assertNotIn("Interop", code(name))

    def test_inputs_are_opened_read_only_and_shared(self) -> None:
        reader = code("WorkbookIdentityReader.cs")
        self.assertIn("FileMode.Open, FileAccess.Read, FileShare.ReadWrite", reader)
        self.assertIn("SpreadsheetDocument.Open(stream, false)", reader)
        self.assertNotIn(".Save", reader)
        self.assertNotIn(".Save", code("WorkbookIdentityEngine.cs"))

    def test_reader_finds_children_whatever_their_order(self) -> None:
        # Typed child properties depend on the element order inside a font; GetFirstChild does not.
        reader = code("WorkbookIdentityReader.cs")
        for typed in ("font.Bold", "font.Italic", "font.FontSize", "font.FontName", "font.Color", "sheet.Fonts", "sheet.CellFormats"):
            self.assertNotIn(typed, reader)
        self.assertGreaterEqual(reader.count("GetFirstChild<"), 20)

    def test_summary_has_its_eighteen_checks(self) -> None:
        engine = code("WorkbookIdentityEngine.cs")
        checks = re.findall(r'new IdentityCheck\("([^"]+)", "(\w+)", "[^"]+", "([^"]+)"\)', engine)
        self.assertEqual(len(checks), 18)
        self.assertEqual(len({key for _, key, _ in checks}), 18)
        writer = code("IdentityReportWriter.cs")
        sheets = re.search(r"SheetNames = \{([^}]+)\}", writer).group(1)
        self.assertEqual(re.findall(r'"([^"]+)"', sheets),
                         ["Summary", "Headers", "Data Differences", "Format Differences", "Layout & Other"])
        for _, _, sheet in checks:
            self.assertIn(f'"{sheet}"', sheets)

    def test_report_text_is_never_stored_as_a_formula_and_summary_is_not_frozen(self) -> None:
        writer = code("IdentityReportWriter.cs")
        self.assertIn("cell.DataType = XLDataType.Text;", writer)
        self.assertNotIn("FormulaA1", writer)
        # A frozen pane without a split makes Excel repair the workbook; the Summary table starts at row 12.
        summary = writer.split("private static void Summary(", 1)[1].split("private static int Table(", 1)[0]
        self.assertRegex(summary, r'"No checks", false\);')
        self.assertNotIn("Freeze", summary.replace("No frozen pane here", ""))

    def test_same_platform_rules_match_the_reference_tool(self) -> None:
        engine = code("WorkbookIdentityEngine.cs")
        self.assertIn(r'@"^[+-]?(\d+\.?\d*|\.\d+)([eE][+-]?\d+)?$"', engine)       # text that reads as a number
        self.assertIn("Thresholds { get; set; } = { 0, 0.01, 0.1, 1 };", engine)
        self.assertIn("Math.Round(size, 9) > thresholds[i]", engine)                 # 0.01 is not "> 0.01"
        for kind in ("Number changed", "Text changed", "Case differs only", "Spacing differs only", "Case and spacing differ",
                     "Date changed", "Blank in File A", "Blank in File B", "Data type differs (", "Value and data type differ (",
                     "Formula differs", "Row only in File "):
            self.assertIn(kind, engine)

    def test_guide_describes_the_engine(self) -> None:
        guide = (ROOT / "docs/PROJECT_GUIDE.md").read_text(encoding="utf-8")
        for name in ENGINE_FILES:
            self.assertIn(name, guide)
        self.assertIn("Workbook identity check", guide)


if __name__ == "__main__":
    unittest.main()
