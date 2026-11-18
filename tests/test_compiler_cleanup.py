"""Guard behaviour around the warning-only dead-state cleanup.

These are source contracts, not database end-to-end tests. Windows CI compiles
the actual C# project with warnings as errors and exercises UI smoke scenarios.
"""

import re
import unittest
from pathlib import Path


PROJ = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"


def code(name):
    return re.sub(r"//[^\n]*", "", (PROJ / name).read_text(encoding="utf-8-sig"))


class CompilerCleanup(unittest.TestCase):
    def test_optimised_tracker_has_no_uninitialised_legacy_workbook_helpers(self):
        src = code("SPIDeltaChangeTrackerOptimised.cs")
        for name in ("wb", "dt_summarised_difference", "dt_sql_only", "dt_transpose", "cellValue",
                     "boolCellValueIntType", "deleteColumnInExcel", "generateTransposeTable",
                     "formatExcelTable", "formatExcelTableByDataGridView", "delete_current_row_from_sql_table"):
            with self.subTest(symbol=name):
                self.assertNotRegex(src, rf"\b{name}\b")
        self.assertIn("using (ExcelPackage pck = new ExcelPackage())", src)
        self.assertIn("pck.SaveAs(new System.IO.FileInfo(strFilePathChangeSave));", src)
        self.assertIn("delete_current_row_from_target_table", src)
        self.assertIn("private void backgroundWorker2_DoWork", src)

    def test_local_row_alternation_remains(self):
        src = code("SPIDeltaChangeTrackerOptimised.cs")
        self.assertNotRegex(src, r"private\s+bool\s+boolToggle\s*;")
        self.assertIn("bool boolToggle = true;", src)
        self.assertIn("if (boolToggle)", src)
        self.assertIn("boolToggle = false;", src)
        self.assertIn("boolToggle = true;", src)

    def test_database_never_enabled_dead_include_branch(self):
        src = code("DB Comparison.cs")
        self.assertNotRegex(src, r"\bboolInclude\b")
        self.assertIn("dtSummary = common_Data.GenerateTransposeTable(dtSummary);", src)
        self.assertIn('dtUnmatched.Columns.Add("DBType", typeof(string)).SetOrdinal(0);', src)
        # These similarly named helpers ARE called by Database Comparison; do not remove them globally.
        self.assertIn("generateTransposeTable();", src)
        self.assertIn("formatExcelTableByDataGridView(1, dataGridView2);", src)
        self.assertIn("delete_current_row_from_sql_table();", src)

    def test_ignore_case_uses_live_checkbox_in_all_comparison_routes(self):
        for name in ("Compare_Multiples_Forms.cs", "Excel_Excel_Comparison.cs"):
            src = code(name)
            with self.subTest(form=name):
                self.assertNotRegex(src, r"\bstringComparisonTypeOrdincalCase\b")
                calls = re.findall(r"common_Data\.MismatchTest\([^;]+", src)
                self.assertGreaterEqual(len(calls), 2)
                self.assertTrue(all("cb_IgnoreCase.Checked" in call for call in calls))

    def test_excel_only_difference_columns_and_debug_names_are_preserved(self):
        src = code("Excel_Excel_Comparison.cs")
        for statement in ("if (cb_IncludeDiff_Columns.Checked)", "boolInclude = true;",
                          "boolInclude = false;", "if (boolInclude)",
                          'Replace("###C###", "_IgnoreCase")', 'Replace("###C###", "_Ordinal")'):
            self.assertIn(statement, src)

    def test_file_picker_uses_selection_result_not_unused_shadow(self):
        src = code("ExcelFileReadCustomControl.cs")
        self.assertNotRegex(src, r"private\s+bool\s+boolFileSelected\s*;")
        self.assertNotRegex(src, r"(?<!\.)\bboolFileSelected\s*=")
        self.assertIn("openFileDialogData.boolFileSelected", src)
        self.assertIn("common_Data.LoadExcelFromDataReader", src)


if __name__ == "__main__":
    unittest.main()
