"""A control hidden in a designer must be used by its window's code; otherwise it is dead and is removed."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJ = ROOT / "SPI Data Comp Tool"

REMOVED = {  # hidden, never shown and never used: removed in the clean-up
    "DB Comparison": ["label12", "btn_Delete", "btn_Save", "comboBox2", "button1"],
    "Execute Excel Scripts Form": ["label11"],
    "ViewForMultiReference": ["label6", "label5", "label4", "label3", "dataGridView1"],
}


def without_comments(text: str) -> str:
    return re.sub(r"/\*.*?\*/", "", re.sub(r"//[^\n]*", "", text), flags=re.S)


class HiddenControls(unittest.TestCase):
    def test_removed_controls_and_their_empty_handlers_are_gone(self) -> None:
        for window, names in REMOVED.items():
            designer = (PROJ / f"{window}.Designer.cs").read_text(encoding="utf-8-sig")
            for name in names:
                with self.subTest(window=window, control=name):
                    self.assertNotRegex(designer, rf"\bthis\.{name}\b")
                    self.assertNotRegex(designer, rf"\b{name};")
        code = (PROJ / "DB Comparison.cs").read_text(encoding="utf-8-sig")
        for handler in ("btn_Delete_Click", "btn_Save_Click", "button1_Click"):
            self.assertNotIn(handler, code)

    def test_every_hidden_control_is_used_by_its_window(self) -> None:
        tests = "".join(path.read_text(encoding="utf-8-sig") for path in (ROOT / "tests").glob("*.cs"))
        for designer_path in sorted(PROJ.glob("*.Designer.cs")):
            designer = designer_path.read_text(encoding="utf-8-sig")
            code_path = PROJ / (designer_path.name[:-len(".Designer.cs")] + ".cs")
            code = without_comments(code_path.read_text(encoding="utf-8-sig")) if code_path.exists() else ""
            children = {}
            for parent, child in re.findall(r"this\.(\w+)\.Controls\.Add\(this\.(\w+)", designer):
                children.setdefault(parent, []).append(child)

            def family(name):
                found = [name]
                for child in children.get(name, []):
                    found += family(child)
                return found

            for hidden in re.findall(r"this\.(\w+)\.Visible = false;", designer):
                if hidden.endswith("ToolStripMenuItem"):
                    continue
                with self.subTest(window=designer_path.name, control=hidden):
                    used = any(re.search(rf"\b{re.escape(name)}\b", code) or f'"{name}"' in tests for name in family(hidden))
                    self.assertTrue(used, "hidden in the designer and never used: remove it")


if __name__ == "__main__":
    unittest.main()
