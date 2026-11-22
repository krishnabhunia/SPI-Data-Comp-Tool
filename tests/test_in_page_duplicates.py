"""Redesign R2 (D1): one control per action inside each page — no second button for the same job."""

import re
import unittest
from pathlib import Path

PROJ = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"


def source(name: str) -> str:
    return re.sub(r"//[^\n]*", "", (PROJ / name).read_text(encoding="utf-8-sig"))


GONE = {  # control -> why it was a duplicate
    "DB Comparison": {
        "btn_Load_TRG_Table": "one 'Load table lists' button fills both pickers",
        "btn_Set": "Compare prepares the mapping itself",
        "btn_RuleForm": "rules are chosen only in Settings > Rules",
    },
    "Excel_Excel_Comparison": {
        "btn_Validate": "Compare checks the inputs itself",
        "btn_RuleForm": "rules are chosen only in Settings > Rules",
        "button1": "developer test button",
        "btn_TestSystemMemoryOutException": "developer test button",
    },
    "Compare_Multiples_Forms": {"btn_RuleForm": "rules are chosen only in Settings > Rules"},
    "ViewForMultiReference": {"btn_RuleForm": "rules are chosen only in Settings > Rules"},
    "UserProfileForm": {
        "btn_Load_SRC_Table": "table lists are loaded on the comparison page",
        "btn_Load_TRG_Table": "table lists are loaded on the comparison page",
    },
}


class InPageDuplicates(unittest.TestCase):
    def test_duplicate_controls_are_gone(self) -> None:
        for window, names in GONE.items():
            designer = source(f"{window}.Designer.cs")
            for name, why in names.items():
                with self.subTest(window=window, control=name):
                    self.assertNotRegex(designer, rf"\bthis\.{name}\b", why)

    def test_no_task_page_has_its_own_close_button(self) -> None:
        # One way to close a task in every page: the window's "Close task" button.
        for designer in sorted(PROJ.glob("*.Designer.cs")):
            if designer.name.startswith("AboutBox1"):
                continue  # its OK button is hidden when About is embedded in Settings
            text = source(designer.name)
            with self.subTest(designer=designer.name):
                self.assertNotRegex(text, r'\.Text = "&?(Close|Exit|E&xit)";')
        for code in ("WorkbookIdentityForm.cs", "ExcelSheetAuditForm.cs", "CompareModePage.cs", "WorkspaceForm.cs"):
            path = PROJ / code
            if path.exists() and code != "WorkspaceForm.cs":
                with self.subTest(code=code):
                    self.assertNotRegex(source(code), r'Text = "&?(Close|Exit)"')
        self.assertIn('Text = "Close task"', source("WorkspaceForm.cs"))

    def test_one_load_button_fills_both_table_pickers(self) -> None:
        code = source("DB Comparison.cs")
        both = code.split("private void LoadBothTableLists(", 1)[1].split("\n        }\n", 1)[0]
        self.assertLess(both.index("btn_Load_SRC_Table_Click"), both.index("btn_Load_TRG_Table_Click"))
        designer = source("DB Comparison.Designer.cs")
        self.assertIn('this.btn_Load_SRC_Table.Text = "Load table lists";', designer)

    def test_compare_prepares_and_validates_itself(self) -> None:
        db = source("DB Comparison.cs")
        click = db.split("private void btnGenerateOutput_Click(", 1)[1]
        self.assertIn("PrepareComparison();\n                if (!comparisonPrepared) return;", click.split("\n        }\n        private", 1)[0])
        self.assertNotIn("btn_Set_Click", db)
        excel = source("Excel_Excel_Comparison.cs")
        compare = excel.split("private void btn_Compare_Click(", 1)[1]
        self.assertLess(compare.index("ValidateComparisonInput"), compare.index("UntickKeysFromColumns();"))
        self.assertNotIn("btn_Validate", excel)
        self.assertNotIn("TestSystemMemoryOutException", excel)

    def test_profile_editor_only_tests_the_connection(self) -> None:
        designer = source("UserProfileForm.Designer.cs")
        self.assertIn('this.btn_Connect_Reference_ID.Text = "Test connection";', designer)
        code = source("UserProfileForm.cs")
        self.assertNotIn("btn_Load_SRC_Table", code)
        self.assertNotIn("private void LoadTable(", code)


if __name__ == "__main__":
    unittest.main()
