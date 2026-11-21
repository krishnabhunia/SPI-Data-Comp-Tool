"""Contracts for retiring proven-unused UI prototypes without removing workflows."""

import pathlib
import unittest


ROOT = pathlib.Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


class WorkspaceCleanupContracts(unittest.TestCase):
    def read(self, name):
        return (APP / name).read_text(encoding="utf-8-sig")

    def test_database_comparison_has_only_the_working_profile_picker(self):
        source = self.read("DB Comparison.cs")
        designer = self.read("DB Comparison.Designer.cs")
        for obsolete in (
            "groupBox16", "lbl_ProfileStatus", "tb_ProfileName", "btn_SaveProfile",
            "btn_LoadProfile", "treeView1", "DisplayTreeView", "D:\\path.json",
        ):
            with self.subTest(obsolete=obsolete):
                self.assertNotIn(obsolete, source + designer)
        self.assertIn("cb_UserProfileList => connection.ProfileList", source)
        self.assertIn("connection.ProfileApplied += Connection_ProfileApplied", source)
        self.assertIn("connection.LoadSavedProfiles()", source)

    def test_retired_database_profile_controls_stay_out_of_generated_layout(self):
        layout = (ROOT / "tools" / "layouts" / "DB_Comparison.py").read_text()
        self.assertIn('FROZEN = ["groupBox15"]', layout)
        self.assertIn('"groupBox16", "lbl_ProfileStatus", "label10", "tb_ProfileName"', layout)
        self.assertIn('"btn_SaveProfile", "btn_LoadProfile", "treeView1"', layout)
        self.assertIn("(panel(), 0, 0)", layout)

    def test_inactive_excel_worker_scaffold_is_fully_removed(self):
        source = self.read("Excel_Excel_Comparison.cs")
        designer = self.read("Excel_Excel_Comparison.Designer.cs")
        for obsolete in ("btn_ProcessBy_MultiThreading", "bgw_multiThreading", "multiThreadingParameter"):
            with self.subTest(obsolete=obsolete):
                self.assertNotIn(obsolete, source + designer)

    def test_active_excel_workers_keep_their_events_and_lifecycle_guards(self):
        source = self.read("Excel_Excel_Comparison.cs")
        designer = self.read("Excel_Excel_Comparison.Designer.cs")
        for worker in ("backgroundWorker1", "backgroundWorkerInstrument"):
            with self.subTest(worker=worker):
                self.assertIn(worker + ".RunWorkerAsync()", source)
                self.assertIn(worker + ".CancelAsync()", source)
                self.assertIn("FormValidation.IsBusy(" + worker, source)
                for event in ("DoWork", "ProgressChanged", "RunWorkerCompleted"):
                    self.assertIn("this." + worker + "." + event + " +=", designer)
        self.assertIn("isLoading || backgroundWorker1.IsBusy || backgroundWorkerInstrument.IsBusy", source)
        self.assertIn("args.Cancel = true;", source)

    def test_mapping_counts_describe_the_loaded_snapshots(self):
        source = self.read("ViewForMultiReference.cs")
        self.assertIn("dsSource.Tables.Add(dsS.Tables[0].Copy())", source)
        self.assertIn("dsTarget.Tables.Add(dsT.Tables[1].Copy())", source)
        self.assertIn('lbl_Source.Text = string.Format("Rows : {0} and Columns : {1}", dsSource.Tables[0].Rows.Count, dsSource.Tables[0].Columns.Count)', source)
        self.assertIn('lbl_Target.Text = string.Format("Rows : {0} and Columns : {1}", dsTarget.Tables[0].Rows.Count, dsTarget.Tables[0].Columns.Count)', source)

    def test_mapping_count_runtime_fixture_uses_distinct_worksheets(self):
        checks = (ROOT / "tests" / "ExcelFormValidationChecks.cs").read_text()
        self.assertIn("CheckMultipleReferenceCounts(passed);", checks)
        self.assertIn('"UnselectedTargetSheet"', checks)
        self.assertIn('"Rows : 3 and Columns : 3"', checks)
        self.assertIn('Get<DataSet>(form, "dsTarget").Tables[0].Rows.Count == 3', checks)


if __name__ == "__main__":
    unittest.main()
