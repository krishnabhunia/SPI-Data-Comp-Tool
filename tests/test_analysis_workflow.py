"""Regression contracts for the consolidated Analysis setup/results workspace.

The Windows --check-ui runner additionally exercises the real controls with synthetic
DataSets. These source checks protect routing and startup safety on non-Windows CI.
"""

import re
import unittest
from pathlib import Path

PROJECT = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"


def source(name: str) -> str:
    return (PROJECT / name).read_text(encoding="utf-8-sig")


class AnalysisWorkflow(unittest.TestCase):
    def test_results_are_a_control_in_the_existing_window(self) -> None:
        view = source("SPI_Analysis_View.cs")
        host = source("SPI_Analysis.cs")
        designer = source("SPI_Analysis.Designer.cs")
        self.assertIn("class SPI_Analysis_View : UserControl", view)
        self.assertNotRegex(host, r"\b(?:results|sPI_Analysis_View|replacement)\.Show\(")
        self.assertIn("resultsPage.Controls.Add(replacement)", host)
        self.assertIn("workspaceTabs.SelectedTab = resultsPage", host)
        self.assertIn('this.setupPage.Text = "Connection setup"', designer)
        self.assertIn('this.resultsPage.Text = "Results"', designer)

    def test_reload_keeps_setup_and_preserves_old_results_until_replacement_is_valid(self) -> None:
        host = source("SPI_Analysis.cs")
        method = host.split("internal void DisplayResults(DataSet dataSet)", 1)[1]
        self.assertLess(method.index("new SPI_Analysis_View(dataSet)"), method.index("resultsPage.Controls.Clear()"))
        self.assertLess(method.index("resultsPage.Controls.Add(replacement)"), method.index("previous.Dispose()"))
        self.assertNotIn("setupPage.Controls.Clear", method)
        self.assertIn("MessageBoxDefaultButton.Button2", host)
        self.assertIn("!= DialogResult.Yes", host)

    def test_results_use_a_private_snapshot_and_validate_both_tables(self) -> None:
        view = source("SPI_Analysis_View.cs")
        self.assertIn("dataSnapshot = ds.Copy()", view)
        self.assertIn("dataSet.Tables.Count < 2", view)
        self.assertIn("side < 2", view)
        for column in ("SPEC_FORM_CNUM", "COLUMN_NAME", "HEADER_TEXT"):
            self.assertIn('"' + column + '"', view.split("private static void ValidateInput", 1)[1])
        self.assertIn("dataSnapshot.Dispose()", source("SPI_Analysis_View.Designer.cs"))

    def test_no_database_load_on_window_startup(self) -> None:
        host = source("SPI_Analysis.cs")
        constructor = host.split("public SPI_Analysis()", 1)[1].split("private void", 1)[0]
        self.assertNotIn("LoadDebuggingInformation", constructor)
        self.assertNotIn("Load_DB_Oracle_SQL", constructor)
        self.assertIn("btn_Show.Enabled = oracleAndSQLConnection1.HasLoadedData", host)
        self.assertIn("this.btn_Show.Enabled = false", source("SPI_Analysis.Designer.cs"))
        view = source("SPI_Analysis_View.cs")
        empty = view.split("public SPI_Analysis_View()", 1)[1].split("private void", 1)[0]
        self.assertIn("btn_Arrange_target.Enabled = btn_SaveInExcel.Enabled = false", empty)

    def test_dead_profile_controls_are_removed_not_hidden(self) -> None:
        code = source("OracleAndSQLConnection.cs")
        designer = source("OracleAndSQLConnection.Designer.cs")
        for name in ("cb_UserProfileList", "btn_Save_Click", "btn_Delete_Click", "groupBox4"):
            self.assertNotIn(name, code + designer)
        self.assertIn("public event EventHandler DataLoadStateChanged", code)
        self.assertIn("boolStatus && ds.Tables.Count == 2", code)

    def test_analysis_features_and_responsive_grids_remain(self) -> None:
        view = source("SPI_Analysis_View.cs")
        for method in ("LoadDataGridView", "LoadTargetAndSourceArranged", "btn_SaveInExcel_Click", "Scroll_Sync"):
            self.assertRegex(view, rf"\b{method}\s*\(")
        for label in ("Column and Header Match", "Technically Match (Partial Match)", "No Match Found in target", "No Match Found in source"):
            self.assertIn(label, view)
        self.assertIn("grid.Dock = DockStyle.Fill", view)
        self.assertIn("DataGridViewAutoSizeColumnsMode.DisplayedCells", view)
        self.assertNotIn("this.WindowState", source("SPI_Analysis_View.Designer.cs"))

    def test_invalid_selection_cannot_reuse_previous_rows(self) -> None:
        view = source("SPI_Analysis_View.cs")
        selection = view.split("private DataTable LoadDataGridView(", 1)[1].split("private void UpdateSelectionAvailability", 1)[0]
        self.assertIn("DataTable selected = dt_Load == null ? new DataTable() : dt_Load.Clone()", selection)
        self.assertIn("dgv.DataSource = selected", selection)
        self.assertIn("return selected", selection)
        self.assertNotIn("return dt_ToBeLoaded", selection)
        self.assertIn("dt_source_spec.Rows.Count > 0 && dt_target_spec.Rows.Count > 0", view)


if __name__ == "__main__":
    unittest.main()
