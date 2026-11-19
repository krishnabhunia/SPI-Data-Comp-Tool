import pathlib
import unittest


ROOT = pathlib.Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


class ExcelValidationContracts(unittest.TestCase):
    def read(self, name):
        return (APP / name).read_text(encoding="utf-8-sig")

    def test_every_excel_utility_has_validation_boundaries(self):
        for name in (
            "Excel_Excel_Comparison.cs", "ExcelFileReadCustomControl.cs", "ViewForMultiReference.cs",
            "CustomisedExcel.cs", "MergeLogsExport.cs", "SPI_Spec_Note_Creation_Utility.cs",
            "Generate IUD Script Form.cs", "Execute Excel Scripts Form.cs",
        ):
            with self.subTest(name=name):
                self.assertIn("FormValidation.Check(", self.read(name))

    def test_comparison_rechecks_and_invalidates_inputs(self):
        source = self.read("Excel_Excel_Comparison.cs")
        self.assertGreaterEqual(source.count("FormValidation.Check(this, ValidateComparisonInput"), 2)
        self.assertIn("clb_reference_ID.ItemCheck +=", source)
        self.assertIn("clb_firstExcel.ItemCheck +=", source)
        self.assertIn("ValidatePrimaryKeys", source)
        self.assertNotIn("await Task.WhenAll(taskLoadSRCExcelFile, taskLoadTRGExcelFile).ContinueWith", source)

    def test_execution_checks_all_rows_and_retains_query_newlines(self):
        source = self.read("Execute Excel Scripts Form.cs")
        self.assertIn("selectedColumns.Length == 1", source)
        self.assertIn("connection.InputChanged +=", source)
        self.assertIn("outputs.Add(path)", source)
        self.assertNotIn('strQuery.Replace("\\r", " ")', source)
        self.assertNotIn("tb_SRC_Password.Text.Trim()", source)

    def test_generation_cannot_emit_unrestricted_updates(self):
        source = self.read("Generate IUD Script Form.cs")
        self.assertIn("keys.Length > 0", source)
        self.assertIn("columns.Except(keys).Any()", source)
        self.assertIn("SqlPredicate", source)
        self.assertIn('text.Replace("\'", "\'\'")', source)
        self.assertNotIn('strUpdateTable.Replace("  ", " ")', source)
        self.assertNotIn("scripts will be generated without 'WHERE'", source)

    def test_generation_imports_all_sheets_and_separates_sql_from_excel_names(self):
        source = self.read("Generate IUD Script Form.cs")
        self.assertIn("SetWorkbook(candidate);", source)
        setter = source.split("internal void SetWorkbook(DataSet candidate)", 1)[1].split("internal string OutputWorksheetName()", 1)[0]
        self.assertIn("ValidateScriptWorkbook(candidate);", setter)
        self.assertIn("cb_SelectSheet.Items.Clear();", setter)
        self.assertIn("candidate.Tables.Cast<DataTable>()", setter)
        self.assertIn("cb_SelectSheet.SelectedIndex = 0;", setter)
        self.assertNotIn("Rows.Count", setter)
        self.assertEqual(source.count("WriteExcel.ExportToExcel(dtGenerateScript, OutputWorksheetName(),"), 2)
        self.assertNotIn("WriteExcel.ExportToExcel(dtGenerateScript, strTableName,", source)

    def test_result_builders_do_not_assume_nonempty_notes_or_fixed_log_lines(self):
        notes = self.read("SPI_Spec_Note_Creation_Utility.cs")
        logs = self.read("MergeLogsExport.cs")
        self.assertIn("BuildNotes", notes)
        self.assertNotIn("str_Note.Substring", notes)
        self.assertIn("ParseMergeLog", logs)
        self.assertIn("int.TryParse", logs)
        self.assertNotIn("strDetails[4]", logs)


if __name__ == "__main__":
    unittest.main()
