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

    def test_form_is_a_workspace_task_with_its_own_route(self) -> None:
        catalog = code("WorkspaceCatalog.cs")
        self.assertIn('new WorkspaceTask("comparison.identity", "Workbook identity check",', catalog)
        self.assertIn("() => new WorkbookIdentityForm()", catalog)
        # The two Excel tasks that existed before keep their routes and windows.
        self.assertIn('new WorkspaceTask("comparison.excel",', catalog)
        self.assertIn("() => new Excel_Excel_Comparison()", catalog)
        self.assertIn("() => new ExcelSheetAuditForm()", catalog)
        project = code("SPI Data Comp Tool.csproj")
        self.assertIn('<Compile Include="WorkbookIdentityForm.cs">', project)
        self.assertIn(r'<Compile Include="..\tests\WorkbookIdentityFormChecks.cs">', project)
        runner = (ROOT / "tests/UiWorkflowChecks.cs").read_text(encoding="utf-8-sig")
        self.assertIn("WorkbookIdentityFormChecks.Run(passed);", runner)

    def test_form_works_in_the_background_and_guards_its_files(self) -> None:
        form = code("WorkbookIdentityForm.cs")
        for needed in ("public sealed class WorkbookIdentityForm : WorkspacePageForm", "BackgroundWorker worker",
                       "FormValidation.LockInputs(this, cancel)", "WorkbookIdentityEngine.Compare(", "WorkbookIdentityEngine.Inspect(",
                       "FormValidation.WriteAtomically(path, temporary =>", "FormValidation.DifferentFiles(report, a, output)",
                       "FormValidation.DifferentFiles(report, b, output)", "MessageBoxDefaultButton.Button2", "protected override void OnFormClosing",
                       "UiTheme.Apply(this, compare)", "IdentityReportWriter.Tables(finished)", "if (SelfTest.IsRunning) return;"):
            self.assertIn(needed, form)
        # One question only: replacing an existing report. Wrong inputs are shown beside the field instead.
        self.assertEqual(form.count("MessageBox.Show("), 1)
        # The engine is never called from the UI thread.
        work = form.split("private void Work(object sender, DoWorkEventArgs args)", 1)[1].split("private void Completed(", 1)[0]
        self.assertEqual(form.count("WorkbookIdentityEngine.Compare("), work.count("WorkbookIdentityEngine.Compare("))
        self.assertEqual(form.count("WriteReport(compared, request.Report, cancelled);"), 1)
        self.assertIn("WriteReport(compared, request.Report, cancelled);", work)
        # Cancel pressed while the report is written is honoured before the report is put in place.
        write = form.split("internal static void WriteReport(", 1)[1].split("private static IdentityFileInfo Inspect(", 1)[0]
        self.assertEqual(form.count("IdentityReportWriter.Write("), write.count("IdentityReportWriter.Write("))
        self.assertLess(write.index("IdentityReportWriter.Write(compared, temporary);"), write.index("throw new OperationCanceledException();"))
        for forbidden in ("Microsoft.Office.Interop", "OfficeOpenXml", "Application.DoEvents", "python"):
            self.assertNotIn(forbidden, form)

    def test_result_tabs_and_report_sheets_share_one_source(self) -> None:
        writer = code("IdentityReportWriter.cs")
        self.assertIn("internal static IdentityTable[] Tables(IdentityResult res)", writer)
        write = writer.split("internal static void Write(IdentityResult res, string path)", 1)[1].split("private static void Summary(", 1)[0]
        self.assertIn("IdentityTable[] tables = Tables(res);", write)
        self.assertNotIn('"Header in File A"', write)       # titles are not repeated in a second place
        form = code("WorkbookIdentityForm.cs")
        self.assertNotIn('"Header in File A"', form)
        self.assertIn("internal const int PreviewRows = 1000;", form)

    def test_guides_describe_the_form(self) -> None:
        for name in ("docs/PROJECT_GUIDE.md", "docs/WORKSPACES.md", "docs/WORKBOOK_IDENTITY_CHECK.md", "README.md"):
            self.assertTrue("Workbook identity check" in (ROOT / name).read_text(encoding="utf-8"), name)
        self.assertTrue("`comparison.identity`" in (ROOT / "docs/WORKSPACES.md").read_text(encoding="utf-8"))
        self.assertTrue("| `WorkbookIdentityForm` |" in (ROOT / "docs/VALIDATION.md").read_text(encoding="utf-8"))

    def test_guide_describes_the_engine(self) -> None:
        guide = (ROOT / "docs/PROJECT_GUIDE.md").read_text(encoding="utf-8")
        for name in ENGINE_FILES:
            self.assertIn(name, guide)
        self.assertIn("Workbook identity check", guide)


if __name__ == "__main__":
    unittest.main()
