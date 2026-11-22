"""Inventory/source contracts; Windows C# checks exercise actual controls and synthetic files."""

import re
import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


def source(name):
    return (APP / name).read_text(encoding="utf-8-sig")


class FormValidationContracts(unittest.TestCase):
    def test_every_form_is_in_the_validation_inventory(self):
        forms = set()
        for path in APP.glob("*.cs"):
            forms.update(re.findall(r"(?<!abstract )\bclass\s+(\w+)\s*:\s*(?:Form|WorkspacePageForm)\b",
                                    path.read_text(encoding="utf-8-sig")))
        documented = set(re.findall(r"^\| `(\w+)` \|", (ROOT / "docs/VALIDATION.md").read_text(), re.MULTILINE))
        self.assertEqual(len(forms), 15)  # 11 workflow pages, the Compare mode host, main window, About and workspace host
        self.assertNotIn("WorkspacePageForm", forms)  # abstract lifecycle bridge, not an extra window
        self.assertTrue(forms <= documented, forms - documented)

    def test_all_new_runtime_suites_are_built_and_run(self):
        project = source("SPI Data Comp Tool.csproj")
        runner = (ROOT / "tests/UiWorkflowChecks.cs").read_text()
        for suite in ("CoreValidationChecks", "ConnectionValidationChecks", "DatabaseFormValidationChecks",
                      "ExcelFormValidationChecks"):
            self.assertIn(suite + ".cs", project)
            self.assertIn(suite + ".Run(passed);", runner)
        self.assertLess(runner.index("UpdateFeedChecks.Run"), runner.index("CoreValidationChecks.Run"))

    def test_inline_feedback_does_not_swallow_programming_failures(self):
        text = source("FormValidation.cs")
        check = text.split("internal static bool Check(", 1)[1].split("internal static string LastMessage", 1)[0]
        self.assertIn("catch (ValidationFailure failure)", check)
        self.assertNotIn("catch (Exception", check)
        for required in ("ErrorBlinkStyle.NeverBlink", "AccessibleDescription", "SelectedTab = pages[index]",
                         "field.TextChanged += Corrected", "target.TextChanged -= Corrected"):
            self.assertIn(required, text)

    def test_worker_start_checks_busy_and_restores_failed_starts(self):
        text = source("FormValidation.cs")
        start = text.split("internal static void StartWorker", 1)[1].split("internal static string ReadableFile", 1)[0]
        for required in ("IsBusy(worker, owner)", "inputLock == null", "inputLock.Dispose()", "inputLock = null"):
            self.assertIn(required, start)
        self.assertIn("!owner.Enabled || keepEnabled.Contains(owner)", text)

    def test_atomic_output_preflight_never_truncates_an_existing_file(self):
        text = source("FormValidation.cs")
        output = text.split("internal static string OutputFile", 1)[1].split("internal static void DifferentFiles", 1)[0]
        self.assertIn("FileMode.Open, FileAccess.Write, FileShare.None", output)
        self.assertNotIn("FileMode.Create", output)
        atomic = text.split("internal static void WriteAtomically", 1)[1].split("internal static void Columns", 1)[0]
        self.assertLess(atomic.index("bool replaceExisting = File.Exists(destination)"), atomic.index("writer(temporary)"))
        for required in ("FileMode.CreateNew", "ownedTemporary", "File.Replace(temporary, destination, null)",
                         "File.Move(temporary, destination)"):
            self.assertIn(required, atomic)

    def test_shared_export_overloads_enforce_bounds_and_atomic_writes(self):
        text = source("Common_Data.cs")
        single = text.split("public static bool ExportToExcel(DataTable", 1)[1].split("public static DataSet ExportToExcelNoLimitRows", 1)[0]
        for required in ("FormValidation.ExcelTable", "FormValidation.WorksheetName", "FormValidation.OutputFile",
                         "FormValidation.WriteAtomically"):
            self.assertIn(required, single)
        multi = text.split("public static bool ExportToExcel(DataSet", 1)[1].split("public class ReadExcel", 1)[0]
        self.assertIn("FormValidation.Workbook", multi)
        self.assertIn("FormValidation.WriteAtomically", multi)

    def test_both_readers_validate_headers_before_publishing(self):
        text = source("Common_Data.cs")
        self.assertIn("return ReadExcel.LoadWorkbook(str_file_to_load_path, dataTableName, false);", text)
        self.assertIn("return LoadWorkbook(str_file_to_load_path, dataTableName, true);", text)
        self.assertIn("NormalizeWorkbookHeaders(dsResult, normalizeNames);", text)
        self.assertIn("if (!seen.Add(key))", text)
        self.assertIn("__SPI_HEADER_", text)
        self.assertIn("dsResult?.Dispose();", text)

    def test_provider_builders_keep_credentials_literal(self):
        text = source("ConnectionStrings.cs")
        self.assertIn("SqlConnectionStringBuilder", text)
        self.assertIn("OracleConnectionStringBuilder", text)
        self.assertNotIn("password.Trim()", text)
        self.assertEqual(source("Common_Data.cs").count("ConnectionStrings.Build("), 2)

    def test_progress_calculation_handles_zero_and_overflow(self):
        text = source("Common_Data.cs")
        self.assertIn("if (total <= 0 || completed <= 0) return 0;", text)
        self.assertIn("Math.Min(100L, (long)completed * 100L / total)", text)
        self.assertIn("(long)pg.Maximum - pg.Minimum", text)


if __name__ == "__main__":
    unittest.main()
