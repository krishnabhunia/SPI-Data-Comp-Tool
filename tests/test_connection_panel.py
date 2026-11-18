"""Static contracts for the shared Connection panel (no UI is run)."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1] / "SPI Data Comp Tool"
CONVERTED = ("DB Comparison.cs", "UserProfileForm.cs", "Table Extraction After Delta.cs",
             "Pre_Merging_Duplicacy_Form.cs", "SPIDeltaChangeLegacyID.cs", "Compare_Multiples_Forms.cs")
# handlers that the panel now owns; a converted window must not grow its own copy again
COPIED = ("CheckChangeRadioButton", "btn_Copy_SRC_TRG_Click", "btn_TRG_SRC_Click",
          "cb_WindowAuthentication_SRC_CheckedChanged", "cb_UserProfileList_SelectedIndexChanged")


def source(name: str) -> str:
    return (ROOT / name).read_text(encoding="utf-8-sig")


class ConnectionPanelContracts(unittest.TestCase):
    def test_profile_fills_source_from_src_and_target_from_trg(self) -> None:
        panel = source("ConnectionPanel.cs")
        apply = panel.split("public void ApplySelectedProfile()", 1)[1].split("private void Run(", 1)[0]
        source_call, target_call = apply.split("Target.Apply(", 1)
        self.assertNotIn("_TRG_", source_call)
        self.assertNotIn("_SRC_", target_call.split(");", 1)[0])
        self.assertEqual(len(re.findall(r"d\.\w+_TRG_\w+", target_call)), 9)

    def test_radio_text_is_the_database_type(self) -> None:
        # DatabaseConnection(rb_..._Oracle.Text, ...) compares this text with "oracle"
        side = source("ConnectionSide.cs")
        self.assertIn('Text = "Oracle"', side)
        self.assertIn('Text = "SQL"', side)

    def test_profile_editor_can_type_a_new_profile_name(self) -> None:
        # Save profile names the new profile from the picker's text, so the picker must be editable there
        code = source("UserProfileForm.cs")
        self.assertIn("profileName = cb_UserProfileList.Text", code)
        self.assertIn("connection.AllowNewProfileName = true;", code)
        self.assertIn("ComboBoxStyle.DropDown :", source("ConnectionPanel.cs"))

    def test_converted_windows_use_the_panel_not_copies(self) -> None:
        for name in CONVERTED:
            with self.subTest(window=name):
                code = source(name)
                designer = source(name.replace(".cs", ".Designer.cs"))
                self.assertIn("new SPI_Data_Comp_Tool.ConnectionPanel()", designer)
                self.assertIn("connection.ProfileApplied += Connection_ProfileApplied;", code)
                for handler in COPIED:
                    self.assertNotIn(f"void {handler}(", code)


if __name__ == "__main__":
    unittest.main()
