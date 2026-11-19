"""Static contracts for the shared input checker (its behaviour is checked in C#: tests/InputCheckChecks.cs)."""

import re
import unittest
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
PROJ = REPO / "SPI Data Comp Tool"

# every button that connects, and the halves it must check first
GUARDED = {
    "Compare_Multiples_Forms.cs": ["btn_TestDBConnection_Click"],
    "DB Comparison.cs": ["btn_Load_SRC_Table_Click", "btn_Load_TRG_Table_Click"],
    "Execute Excel Scripts Form.cs": ["btnTestConnecton_Click"],
    "PreMergingTool_Form.cs": ["btn_Connect_Click"],
    "Pre_Merging_Duplicacy_Form.cs": ["btn_Load_SRC_Table_Click", "btn_Load_TRG_Table_Click", "btn_Connect_Reference_ID_Click"],
    "SPI FK Extraction.cs": ["btn_TestDBConnection_Click"],
    "SPIDeltaChangeLegacyID.cs": ["btn_Connect_Reference_ID_Click"],
    "SPIDeltaChangeTrackerOptimised.cs": ["btn_Load_SRC_Table_Click", "btn_Load_TRG_Table_Click"],
    "SQL_DB_To_Excel.cs": ["btn_Connect_Click"],
    "Table Extraction After Delta.cs": ["btn_TestDBConnection_Click"],
    "UserProfileForm.cs": ["btn_Load_SRC_Table_Click", "btn_Load_TRG_Table_Click", "btn_Connect_Reference_ID_Click"],
}


def code(path: Path) -> str:
    return re.sub(r"//[^\n]*", "", path.read_text(encoding="utf-8-sig"))


class InputChecks(unittest.TestCase):
    def test_every_connect_button_checks_its_inputs_first(self) -> None:
        for name, handlers in GUARDED.items():
            src = code(PROJ / name)
            for handler in handlers:
                with self.subTest(window=name, button=handler):
                    start = re.search(rf"void {handler}\(object sender, EventArgs e\)\s*\{{\s*try\s*\{{\s*", src)
                    self.assertIsNotNone(start)
                    first = src[start.end():].split(";", 1)[0]
                    self.assertTrue(first.startswith("if (!") and first.endswith(".Report()) return"), first[:80])
                    self.assertTrue("connection.AddChecks(InputCheck.For(this)" in first or "InputCheck.For(this).Connection(" in first)

    def test_source_and_target_buttons_check_their_own_half(self) -> None:
        for name in ("DB Comparison.cs", "Pre_Merging_Duplicacy_Form.cs", "UserProfileForm.cs"):
            src = code(PROJ / name)
            for handler, half in (("btn_Load_SRC_Table_Click", "target: false"), ("btn_Load_TRG_Table_Click", "source: false")):
                body = src.split(f"void {handler}(", 1)[1][:400]
                self.assertIn(half, body, f"{name} {handler}")

    def test_wrong_inputs_are_shown_at_the_field_not_in_a_message_box(self) -> None:
        checker = code(PROJ / "InputCheck.cs")
        self.assertIn("ErrorProvider", checker)
        self.assertIn("provider.SetError(field, reasons);", checker)
        self.assertIn("first.Focus();", checker)
        self.assertIn('"rtb_Log"', checker)
        self.assertNotIn("MessageBox", checker)

    def test_ci_runs_the_checks_of_the_checker(self) -> None:
        workflow = (REPO / ".github" / "workflows" / "build.yml").read_text(encoding="utf-8")
        self.assertIn("'--check-inputs'", workflow)
        self.assertIn('"--check-inputs"', code(PROJ / "SelfTest.cs"))
        project = (PROJ / "SPI Data Comp Tool.csproj").read_text(encoding="utf-8-sig")
        self.assertIn('<Compile Include="InputCheck.cs" />', project)
        self.assertIn(r'<Compile Include="..\tests\InputCheckChecks.cs">', project)
        self.assertGreaterEqual((REPO / "tests" / "InputCheckChecks.cs").read_text(encoding="utf-8").count("Check("), 25)


if __name__ == "__main__":
    unittest.main()
