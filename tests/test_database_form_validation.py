"""Source contracts supplement real-control Windows checks; they do not contact a database."""
from pathlib import Path
import unittest


ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


class DatabaseFormValidation(unittest.TestCase):
    def test_every_database_workflow_has_an_action_gate(self):
        expected = {
            "DB Comparison.cs": "ValidateComparisonInputs",
            "Compare_Multiples_Forms.cs": "ValidateBatchInputs",
            "SPIDeltaChangeTrackerOptimised.cs": "ValidateTrackerInputs",
            "PreMergingTool_Form.cs": "ValidatePreMergeInputs",
            "Pre_Merging_Duplicacy_Form.cs": "ValidateDuplicacyInputs",
            "Table Extraction After Delta.cs": "ValidateDeltaInputs",
            "SPI FK Extraction.cs": "ValidateExtractionInputs",
            "SPIDeltaChangeLegacyID.cs": "ValidateLegacyInputs",
            "SQL_DB_To_Excel.cs": "ValidateExportInputs",
        }
        for filename, gate in expected.items():
            with self.subTest(form=filename):
                text = (APP / filename).read_text(encoding="utf-8-sig")
                self.assertIn("internal bool " + gate, text)
                self.assertIn("if (!" + gate + "(", text)
                self.assertIn("FormValidation.Check", text)

    def test_batch_imports_validate_candidates_before_publishing(self):
        for filename in ["Compare_Multiples_Forms.cs", "SPIDeltaChangeTrackerOptimised.cs", "PreMergingTool_Form.cs",
                         "Pre_Merging_Duplicacy_Form.cs", "Table Extraction After Delta.cs", "SPI FK Extraction.cs", "SPIDeltaChangeLegacyID.cs"]:
            with self.subTest(form=filename):
                text = (APP / filename).read_text(encoding="utf-8-sig")
                self.assertIn("DataSet candidate =", text)
                self.assertIn("FormValidation.Columns", text)
                self.assertIn("FormValidation.Table", text)

    def test_cached_connections_and_comparisons_invalidate_after_edits(self):
        for filename in ["DB Comparison.cs", "Compare_Multiples_Forms.cs", "PreMergingTool_Form.cs", "Pre_Merging_Duplicacy_Form.cs",
                         "Table Extraction After Delta.cs", "SPI FK Extraction.cs", "SPIDeltaChangeLegacyID.cs", "SQL_DB_To_Excel.cs"]:
            self.assertIn("connection.InputChanged +=", (APP / filename).read_text(encoding="utf-8-sig"), filename)
        tracker = (APP / "SPIDeltaChangeTrackerOptimised.cs").read_text(encoding="utf-8-sig")
        self.assertIn("InvalidateTrackerConnections", tracker)
        self.assertIn("ConnectionSide.ValidateFields", tracker)

    def test_workers_lock_inputs_and_release_before_completion_state(self):
        for filename in ["DB Comparison.cs", "Compare_Multiples_Forms.cs", "SPIDeltaChangeTrackerOptimised.cs",
                         "Pre_Merging_Duplicacy_Form.cs", "Table Extraction After Delta.cs", "SQL_DB_To_Excel.cs"]:
            text = (APP / filename).read_text(encoding="utf-8-sig")
            with self.subTest(form=filename):
                self.assertIn("FormValidation.StartWorker", text)
                self.assertIn("operationInputs?.Dispose();", text)
                self.assertIn("FormClosing +=", text)
                self.assertNotIn(".RunWorkerAsync();", text)

    def test_passwords_are_not_trimmed_and_private_connections_use_builders(self):
        for filename in ["DB Comparison.cs", "Compare_Multiples_Forms.cs", "SPIDeltaChangeTrackerOptimised.cs", "PreMergingTool_Form.cs",
                         "Pre_Merging_Duplicacy_Form.cs", "Table Extraction After Delta.cs", "SPI FK Extraction.cs",
                         "SPIDeltaChangeLegacyID.cs", "SQL_DB_To_Excel.cs"]:
            text = (APP / filename).read_text(encoding="utf-8-sig")
            self.assertNotIn("tb_SRC_Password.Text.Trim()", text, filename)
            self.assertNotIn("tb_TRG_Password.Text.Trim()", text, filename)
        for filename in ["DB Comparison.cs", "SPIDeltaChangeTrackerOptimised.cs", "PreMergingTool_Form.cs", "Pre_Merging_Duplicacy_Form.cs", "SQL_DB_To_Excel.cs"]:
            self.assertIn("ConnectionStrings.Build", (APP / filename).read_text(encoding="utf-8-sig"), filename)

    def test_target_oracle_does_not_reuse_source_connection(self):
        text = (APP / "DB Comparison.cs").read_text(encoding="utf-8-sig")
        target = text.split("private DataSet connect_SQL_DB", 1)[1].split("return ds;", 1)[0]
        self.assertIn('strOracleConnection = ConnectionStrings.Build("oracle", tb_TRG_DataSource.Text', target)
        self.assertIn("new OracleDataAdapter(strQuery, strOracleConnection)", target)
        source = text.split("private DataSet connect_Oracle_DB", 1)[1].split("return ds;", 1)[0]
        self.assertNotIn("cb_WindowAuthentication_TRG", source)

    def test_partial_errors_and_export_metadata_are_not_reported_as_success(self):
        duplicate = (APP / "Pre_Merging_Duplicacy_Form.cs").read_text(encoding="utf-8-sig")
        completion = duplicate.split("private void backgroundWorker1_RunWorkerCompleted", 1)[1].split("private DataTable", 1)[0]
        self.assertIn("if (operationFailed)", completion)
        self.assertIn("operationFailed = false;", duplicate)
        self.assertGreaterEqual(duplicate.count("operationFailed = true;"), 4)
        for filename in ["Compare_Multiples_Forms.cs", "SPIDeltaChangeTrackerOptimised.cs"]:
            source = (APP / filename).read_text(encoding="utf-8-sig")
            self.assertIn("if (operationFailed)", source)
            self.assertIn("operationFailed = false;", source)
            self.assertIn("operationFailed = true;", source)
        export = (APP / "SQL_DB_To_Excel.cs").read_text(encoding="utf-8-sig")
        self.assertIn("TryParseTableEntry(item, out itemCheck", export)
        self.assertIn("FormValidation.ExcelTable(table", export)
        self.assertIn("FormValidation.WriteAtomically(output", export)

    def test_local_report_writers_validate_limits_and_publish_atomically(self):
        for filename in ["DB Comparison.cs", "Compare_Multiples_Forms.cs", "SPIDeltaChangeTrackerOptimised.cs",
                         "PreMergingTool_Form.cs", "Pre_Merging_Duplicacy_Form.cs", "SQL_DB_To_Excel.cs"]:
            source = (APP / filename).read_text(encoding="utf-8-sig")
            self.assertIn("FormValidation.ExcelTable(", source, filename)
            self.assertIn("FormValidation.WriteAtomically(", source, filename)
            self.assertNotIn("pck.SaveAs(new System.IO.FileInfo", source, filename)


if __name__ == "__main__":
    unittest.main()
