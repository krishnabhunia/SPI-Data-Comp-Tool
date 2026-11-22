"""Source contracts for the Settings / Rules consolidation; Windows CI checks the live form."""

import re
import unittest
from pathlib import Path


PROJ = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"


def source(name):
    return re.sub(r"//[^\n]*", "", (PROJ / name).read_text(encoding="utf-8-sig"))


class SettingsRules(unittest.TestCase):
    def test_standalone_rules_window_is_retired(self):
        for name in ("Form_Rules.cs", "Form_Rules.Designer.cs", "Form_Rules.resx"):
            self.assertFalse((PROJ / name).exists(), name)
        self.assertNotIn("Form_Rules", source("SPI Data Comp Tool.csproj"))

    def test_rules_are_chosen_only_in_settings(self):
        # One way to reach the rules: Settings > Rules. Each comparison shows a read-only line instead of a button.
        for name in ("DB Comparison.cs", "Excel_Excel_Comparison.cs", "Compare_Multiples_Forms.cs",
                     "ViewForMultiReference.cs"):
            with self.subTest(form=name):
                src = source(name)
                self.assertIn("Common_Data.ShowRulesFile(lbl_RulesFile, this);", src)
                self.assertNotIn("OpenRuleForm", src)
                self.assertNotIn("btn_RuleForm", src)
                self.assertNotIn("btn_RuleForm", source(name.replace(".cs", ".Designer.cs")))
                self.assertIn("lbl_RulesFile", source(name.replace(".cs", ".Designer.cs")))
        src = source("Common_Data.cs")
        self.assertNotIn("OpenRuleForm", src)
        line = src.split("internal static string RulesFileLine()", 1)[1].split("internal static void ShowRulesFile", 1)[0]
        self.assertIn("AppPreferences.RuleFilePath", line)
        self.assertIn("(set in Settings)", line)
        shown = src.split("internal static void ShowRulesFile(Label label, Control page)", 1)[1].split("public DataTable AddColumns", 1)[0]
        self.assertIn("page.VisibleChanged +=", shown)
        status = src.split("public bool GetRulesStatus()", 1)[1].split("public DataTable RemoveColumn", 1)[0]
        self.assertIn("AppPreferences.RuleFilePath", status)
        settings = source("User_Settings.cs")
        self.assertIn("AppPreferences.TrySaveRulesPath(fileName, out warning)", settings)

    def test_rules_are_a_settings_tab(self):
        designer = source("User_Settings.Designer.cs")
        self.assertIn("this.tabs_Settings.Controls.Add(this.tab_General);", designer)
        self.assertIn("this.tabs_Settings.Controls.Add(this.tab_Rules);", designer)
        self.assertIn("this.tb_RulesPath.ReadOnly = true;", designer)
        src = source("User_Settings.cs")
        self.assertIn("tabs_Settings.SelectedTab = tab_Rules;", src)
        # No Close button of its own: like every task, Settings closes with the window's "Close task".
        self.assertNotIn("btn_Close", src)
        self.assertNotIn("CancelButton =", src)
        self.assertNotIn("btn_Close", designer)
        self.assertIn("btn_ClearLogs.Click += (sender, e) => rtb_Log.Clear();", src)

    def test_startup_never_reads_rules_or_writes_preferences(self):
        src = source("User_Settings.cs")
        ctor = src.split("public User_Settings(bool showRules)", 1)[1].split("public void ShowRules()", 1)[0]
        self.assertNotIn("GetDatasetRules", ctor)
        self.assertNotIn(".Save()", ctor)
        self.assertIn("private bool initializing = true;", src)
        self.assertIn("if (!initializing && rb_EPPlus.Checked)", src)
        self.assertIn("if (!initializing && rb_XLSave.Checked)", src)

    def test_cancel_precedes_import_and_import_precedes_commit(self):
        src = source("User_Settings.cs")
        click = src.split("private void btn_ImportRules_Click", 1)[1].split("internal void ImportRulesFile", 1)[0]
        self.assertLess(click.index("DialogResult.OK) return;"), click.index("ImportRulesFile(dialog.FileName)"))
        imp = src.split("internal void ImportRulesFile", 1)[1].split("private void rb_EPPlus_CheckedChanged", 1)[0]
        commit = "AppPreferences.TrySaveRulesPath(fileName, out warning)"
        self.assertLess(imp.index("GetDatasetRules(fileName)"), imp.index(commit))
        self.assertLess(imp.index(commit), imp.index("ImportedRules = candidate;"))
        self.assertIn("candidate.Tables[0].Rows.Count == 0", imp)
        failed_commit = imp.split(commit, 1)[1].split("ImportedRules = candidate;", 1)[0]
        self.assertIn("candidate.Dispose();", failed_commit)
        self.assertIn("throw new InvalidOperationException(warning);", failed_commit)
        self.assertLess(imp.index("ValidateRulesCandidate(candidate)"), imp.index(commit))
        preferences = source("AppPreferences.cs")
        save = preferences.split("internal static bool TrySaveRulesPath", 1)[1].split("internal static bool TrySaveExcelWriter", 1)[0]
        self.assertLess(save.index("previous = current.RuleFilePath;"), save.index("current.RuleFilePath = path;"))
        self.assertLess(save.index("current.Save();"), save.index("return true;"))
        self.assertIn("TryRestore(() => current.RuleFilePath = previous)", save)

    def test_rule_schema_matches_the_evaluator_before_persistence(self):
        src = source("User_Settings.cs")
        validate = src.split("internal static void ValidateRulesCandidate", 1)[1].split("private void rb_EPPlus_CheckedChanged", 1)[0]
        for column in ("srno", "Source", "Target", "RuleType", "Source_ColName", "Target_Colname"):
            self.assertIn('"' + column + '"', validate)
        self.assertIn('rule["RuleType"].ToString().Trim() == "3" && rules.Columns.Count < 10', validate)


if __name__ == "__main__":
    unittest.main()
