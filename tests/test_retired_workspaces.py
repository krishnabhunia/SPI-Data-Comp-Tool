"""Removed workflows must be absent from sources, packaging and every navigation entry point."""

import ast
import re
import unittest
import xml.etree.ElementTree as ET
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"
RETIRED_FILES = (
    "SPI_Analysis", "SPI_Analysis_View", "OracleAndSQLConnection",
    "SPI_Spec_Note_Creation_Utility", "ExcelFileReadCustomControl",
    "SPIDeltaChangeTrackerOptimised", "SPIDeltaChangeLegacyID", "SPI FK Extraction",
    "Table Extraction After Delta", "PreMergingTool_Form", "Pre_Merging_Duplicacy_Form",
)
RETIRED_LAYOUTS = (
    "SPI_Spec_Note_Creation_Utility",
    "SPIDeltaChangeTrackerOptimised", "SPIDeltaChangeLegacyID", "SPI_FK_Extraction",
    "Table_Extraction_After_Delta", "PreMergingTool_Form", "Pre_Merging_Duplicacy_Form",
)


class RetiredWorkspaces(unittest.TestCase):
    def test_removed_implementations_and_embedded_resources_are_not_shipped(self):
        ns = {"msbuild": "http://schemas.microsoft.com/developer/msbuild/2003"}
        project = ET.parse(APP / "SPI Data Comp Tool.csproj")
        included = {
            item.get("Include")
            for kind in ("Compile", "EmbeddedResource", "None", "Content")
            for item in project.findall(".//msbuild:" + kind, ns)
        }
        for stem in RETIRED_FILES:
            for suffix in (".cs", ".Designer.cs", ".resx"):
                name = stem + suffix
                with self.subTest(file=name):
                    self.assertFalse((APP / name).exists(), "Retired implementation left in the source tree")
                    self.assertNotIn(name, included, "Retired implementation remains a project item")
        self.assertFalse((APP / "Properties/DataSources/ExcelFileReadCustomControl.datasource").exists())
        self.assertNotIn(r"Properties\DataSources\ExcelFileReadCustomControl.datasource", included)

    def test_removed_layouts_cannot_regenerate_retired_windows(self):
        for stem in RETIRED_LAYOUTS:
            with self.subTest(layout=stem):
                self.assertFalse((ROOT / "tools/layouts" / (stem + ".py")).exists())

    def test_routes_menus_and_home_tiles_have_no_retired_entry_points(self):
        for relative in (
            "SPI Data Comp Tool/WorkspaceCatalog.cs", "SPI Data Comp Tool/MainForm.cs",
            "SPI Data Comp Tool/MainForm.Designer.cs", "tools/layouts/MainForm.py",
        ):
            with self.subTest(file=relative):
                text = (ROOT / relative).read_text(encoding="utf-8-sig")
                if relative.endswith(".py"):
                    # The generator's removal list may name retired controls; its active layout may not.
                    lines = text.splitlines(keepends=True)
                    for node in ast.parse(text).body:
                        if isinstance(node, ast.Assign) and any(
                            isinstance(target, ast.Name) and target.id == "DROP" for target in node.targets
                        ):
                            lines[node.lineno - 1:node.end_lineno] = [""] * (node.end_lineno - node.lineno + 1)
                    text = "".join(lines)
                self.assertNotRegex(text, r"\b(?:analysis|premerge)\.")
                self.assertNotRegex(text, r"\bWorkspaceKind\.(?:Analysis|PreMerge)\b")
                self.assertNotRegex(text, r"\bbtn_Workspace(?:Analysis|PreMerge)\b")

    def test_shared_workbook_validation_remains_available_to_retained_tools(self):
        for name in ("CustomisedExcel.cs", "Execute Excel Scripts Form.cs"):
            with self.subTest(form=name):
                text = (APP / name).read_text(encoding="utf-8-sig")
                self.assertIn("FormValidation.ValidateWorkbook(", text)
                self.assertNotIn("ExcelFileReadCustomControl", text)
        checks = (ROOT / "tests/ExcelFormValidationChecks.cs").read_text()
        self.assertIn("Rejected(() => FormValidation.ValidateWorkbook(data, customised))", checks)
        self.assertIn("CheckWorkbookReader(passed);", checks)

    def test_windows_gate_checks_compiled_absence_and_every_supported_route(self):
        checks = (ROOT / "tests/WorkspaceUiChecks.cs").read_text()
        self.assertIn("CheckRetiredWorkflowsAbsent();", checks)
        self.assertIn('assembly.GetType("SPI_Data_Comp_Tool." + name, false) == null', checks)
        routes = re.findall(r'\{ "((?:comparison|scripts|reports|settings)\.[^"]+)", typeof\(', checks)
        self.assertEqual(len(routes), 12)
        self.assertEqual(len(set(routes)), 12)
        self.assertIn("workspace.OpenTask(route.Key);", checks)


if __name__ == "__main__":
    unittest.main()
