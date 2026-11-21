"""Controlled steps (G1 locked steps, R2 keep later choices that still fit): static contracts."""
import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


def read(name):
    return (APP / name).read_text(encoding="utf-8-sig")


def body(src, signature):
    start = src.index(signature)
    depth, i = 0, src.index("{", start)
    for j in range(i, len(src)):
        depth += {"{": 1, "}": -1}.get(src[j], 0)
        if depth == 0:
            return src[i:j + 1]
    raise AssertionError("unbalanced " + signature)


WINDOWS = {
    "Excel_Excel_Comparison": ("Excel_Excel_Comparison.cs", "Excel_Excel_Comparison.Designer.cs"),
    "DB_Comparison": ("DB Comparison.cs", "DB Comparison.Designer.cs"),
    "Compare_Multiples_Forms": ("Compare_Multiples_Forms.cs", "Compare_Multiples_Forms.Designer.cs"),
    "SPIDeltaChangeTrackerOptimised": ("SPIDeltaChangeTrackerOptimised.cs", "SPIDeltaChangeTrackerOptimised.Designer.cs"),
    "SPIDeltaChangeLegacyID": ("SPIDeltaChangeLegacyID.cs", "SPIDeltaChangeLegacyID.Designer.cs"),
    "SPI_FK_Extraction": ("SPI FK Extraction.cs", "SPI FK Extraction.Designer.cs"),
    "Table_Extraction_After_Delta": ("Table Extraction After Delta.cs", "Table Extraction After Delta.Designer.cs"),
    "SPI_Spec_Note_Creation_Utility": ("SPI_Spec_Note_Creation_Utility.cs", "SPI_Spec_Note_Creation_Utility.Designer.cs"),
}


class ControlledSteps(unittest.TestCase):
    def test_engine_is_built_and_checked_on_windows(self):
        project = read("SPI Data Comp Tool.csproj")
        self.assertIn('<Compile Include="StepFlow.cs" />', project)
        self.assertIn('<Compile Include="..\\tests\\StepChecks.cs">', project)
        self.assertIn('"--check-steps"', read("SelfTest.cs"))
        workflow = (ROOT / ".github/workflows/build.yml").read_text(encoding="utf-8")
        self.assertIn("'--check-steps'", workflow)
        self.assertIn("throw 'step checks failed'", workflow)

    def test_every_step_window_has_a_strip_and_builds_its_steps(self):
        for layout, (code, designer) in WINDOWS.items():
            with self.subTest(window=layout):
                spec = (ROOT / "tools/layouts" / (layout + ".py")).read_text(encoding="utf-8")
                self.assertIn('C("step_Strip", new="SPI_Data_Comp_Tool.StepStrip"', spec)
                self.assertIn("this.step_Strip = new SPI_Data_Comp_Tool.StepStrip();", read(designer))
                src = read(code)
                self.assertIn("new StepFlow(this, step_Strip)", src)
                self.assertIn("BuildSteps();", src)

    def test_busy_windows_disable_every_step(self):
        self.assertIn("steps.Busy = !boolEnabledStatus;", body(read("Excel_Excel_Comparison.cs"), "private void changeEnabledStatus("))
        self.assertIn("steps.Busy = !enabledStatus;", body(read("DB Comparison.cs"), "private void toggleCondition("))
        compare = body(read("Compare_Multiples_Forms.cs"), "private void btn_Compare_Click(")
        self.assertLess(compare.index("steps.Busy = true;"), compare.index("FormValidation.StartWorker("))
        self.assertIn("steps.Busy = false;", body(read("Compare_Multiples_Forms.cs"), "private void bgw_List_RunWorkerCompleted("))

    def test_excel_target_waits_for_source_and_keys_wait_for_loading(self):
        src = read("Excel_Excel_Comparison.cs")
        steps = body(src, "private void BuildSteps()")
        order = ['"Source workbook"', '"Target workbook"', '"Load both files"', '"Reference key"',
                 '"Columns to compare"', '"Options and validate"', '"Compare"']
        positions = [steps.index(name) for name in order]
        self.assertEqual(positions, sorted(positions), "steps out of order")
        self.assertRegex(steps, r'(?s)"Target workbook".*?btn_SecondExcelSheet\)')
        self.assertRegex(steps, r'(?s)"Reference key".*?clb_reference_ID, cb_Primary_Key\)')
        load = body(src, "private async void btn_SelectLoad_Click(")
        self.assertNotIn("openFileDialog(", load, "Load must use the chosen workbooks, not open dialogs")
        self.assertIn("this.btn_FirstExcelSheet = new System.Windows.Forms.Button();", read("Excel_Excel_Comparison.Designer.cs"))
        self.assertIn("this.btn_SecondExcelSheet = new System.Windows.Forms.Button();", read("Excel_Excel_Comparison.Designer.cs"))

    def test_reload_keeps_choices_that_still_fit(self):
        excel = body(read("Excel_Excel_Comparison.cs"), "private async void btn_SelectLoad_Click(")
        self.assertIn("StepFlow.Restore(clb_reference_ID, keysBefore)", excel)
        self.assertIn("StepFlow.Restore(clb_firstExcel, columnsBefore)", excel)
        db = body(read("DB Comparison.cs"), "private void btn_Connect_Reference_ID_Click(")
        self.assertIn("StepFlow.Restore(clb_ReferenceID, keysBefore)", db)
        self.assertIn("StepFlow.Restore(cbL_SRC_Cols, columnsBefore)", db)
        choose = body(read("Excel_Excel_Comparison.cs"), "private void InvalidateLoad()")
        self.assertIn("boolSelectedBothFile = false;", choose)
        self.assertIn("InvalidateComparison();", choose)

    def test_database_target_connection_waits_for_source(self):
        steps = body(read("DB Comparison.cs"), "private void BuildSteps()")
        self.assertRegex(steps, r'(?s)"Target connection".*?connection\.Target\.Group\)')
        self.assertLess(steps.index('"Source connection"'), steps.index('"Target connection"'))
        self.assertLess(steps.index('"Load"'), steps.index('"Reference key"'))

    def test_locked_controls_cannot_be_enabled_by_window_code(self):
        engine = read("StepFlow.cs")
        self.assertIn("watched.EnabledChanged += (sender, args) => KeepLocked(watched);", engine)
        self.assertIn("if (Busy || step.State == StepState.Locked)", engine)

    def test_batch_two_windows_lock_their_later_steps(self):
        cases = {
            "SPIDeltaChangeTrackerOptimised.cs": [('"Target connection"', "connection.Target.Group"), ('"Table list"', "btn_ListTableExcel"),
                                                  ('"Compare"', "btnGenerateOutput")],
            "SPIDeltaChangeLegacyID.cs": [('"Test connection"', "btn_Connect_Reference_ID"), ('"Map legacy IDs"', "btn_ListTableExcel")],
            "SPI FK Extraction.cs": [('"Test connection"', "btn_TestDBConnection"), ('"Import Excel"', "btn_ImportExcel")],
            "Table Extraction After Delta.cs": [('"Target connection"', "connection.Target.Group"), ('"Run"', "btn_Chg_NumVsChg_Num")],
            "SPI_Spec_Note_Creation_Utility.cs": [('"Create notes"', "btn_Spec_Create"), ('"Export"', "btn_GenerateExcel")],
        }
        for name, pairs in cases.items():
            steps = body(read(name), "private void BuildSteps()")
            for title, control in pairs:
                with self.subTest(window=name, step=title):
                    start = steps.index(title)
                    end = steps.find(".Add(", start)
                    self.assertIn(control, steps[start:end if end > 0 else len(steps)])

    def test_strip_is_drawn_from_the_theme(self):
        self.assertIn("c is TileButton || c is GlyphIcon || c is StepStrip", read("UiTheme.cs"))
        self.assertIn("UiTheme.Changed += ThemeChanged;", read("StepFlow.cs"))


if __name__ == "__main__":
    unittest.main()
