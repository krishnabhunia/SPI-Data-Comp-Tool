import pathlib
import unittest


ROOT = pathlib.Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


class ExcelValidationContracts(unittest.TestCase):
    def read(self, name):
        return (APP / name).read_text(encoding="utf-8-sig")

    def test_every_excel_utility_has_validation_boundaries(self):
        for name in (
            "Excel_Excel_Comparison.cs", "ViewForMultiReference.cs",
            "CustomisedExcel.cs", "MergeLogsExport.cs",
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

    def test_merge_logs_do_not_assume_fixed_log_lines(self):
        logs = self.read("MergeLogsExport.cs")
        self.assertIn("ParseMergeLog", logs)
        self.assertIn("int.TryParse", logs)
        self.assertNotIn("strDetails[4]", logs)


if __name__ == "__main__":
    unittest.main()
