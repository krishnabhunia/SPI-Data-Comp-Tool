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
    "SQL_DB_To_Excel.cs": ["btn_Connect_Click"],
    "UserProfileForm.cs": ["btn_Load_SRC_Table_Click", "btn_Load_TRG_Table_Click", "btn_Connect_Reference_ID_Click"],
}


def code(path: Path) -> str:
    return re.sub(r"//[^\n]*", "", path.read_text(encoding="utf-8-sig"))


class InputChecks(unittest.TestCase):
    # anything that opens a connection or builds its connection string
    CONNECTS = ("DatabaseConnection(", "ConnectDB(", "ConnectToDB(", "ConnectionStrings.Build(", "DataAdapter(", ".Open(")

    @staticmethod
    def handler_body(src: str, handler: str) -> str:
        start = re.search(rf"void {handler}\(object sender, EventArgs e\)\s*\{{", src)
        assert start is not None, handler
        return src[start.end():].split("\n        }\n", 1)[0]

    def test_every_connect_button_checks_its_inputs_before_it_connects(self) -> None:
        # the window's own validation and state resets (added on master) may come first; a connection may not
        for name, handlers in GUARDED.items():
            src = code(PROJ / name)
            for handler in handlers:
                with self.subTest(window=name, button=handler):
                    body = self.handler_body(src, handler)
                    guard = re.search(r"if \(!(connection\.AddChecks\(InputCheck\.For\(this\)|InputCheck\.For\(this\)\.Connection\()"
                                      r"[^;]*\.Report\(\)\) return;", body)
                    self.assertIsNotNone(guard, "no input check in this handler")
                    before = body[:guard.start()]
                    for connect in self.CONNECTS:
                        self.assertNotIn(connect, before, f"{connect} runs before the input check")

    def test_source_and_target_buttons_check_their_own_half(self) -> None:
        for name in ("DB Comparison.cs", "UserProfileForm.cs"):
            src = code(PROJ / name)
            for handler, half in (("btn_Load_SRC_Table_Click", "target: false"), ("btn_Load_TRG_Table_Click", "source: false")):
                self.assertIn(f"connection.AddChecks(InputCheck.For(this), {half}).Report()", self.handler_body(src, handler),
                              f"{name} {handler}")

    def test_the_windows_own_validation_still_runs_after_the_input_check(self) -> None:
        # both gates stay: the input check lists every wrong field, the window's validation (FormValidation) decides
        for name, handler, gate in (("SQL_DB_To_Excel.cs", "btn_Connect_Click", "ValidateLoadInputs()"),
                                    ("UserProfileForm.cs", "btn_Connect_Reference_ID_Click", "connection.ValidateInput()")):
            with self.subTest(window=name, button=handler):
                body = self.handler_body(code(PROJ / name), handler)
                self.assertIn(gate, body)
                self.assertLess(body.index(".Report()) return;"), body.index(gate))

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
