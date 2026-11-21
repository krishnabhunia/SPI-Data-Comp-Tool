"""Additive integration contracts; the Windows fixture suite verifies actual workbook semantics."""

import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


def source(name):
    return (APP / name).read_text(encoding="utf-8-sig")


class ExcelAuditContracts(unittest.TestCase):
    def test_additive_native_route_and_compiled_sources(self):
        catalog = source("WorkspaceCatalog.cs")
        self.assertIn('new WorkspaceTask("comparison.excel",', catalog)
        self.assertIn('new WorkspaceTask("comparison.excel-audit",', catalog)
        self.assertIn("() => new Excel_Excel_Comparison()", catalog)
        self.assertIn("() => new ExcelSheetAuditForm()", catalog)
        project = source("SPI Data Comp Tool.csproj")
        for name in ("ExcelSheetAuditForm", "ExcelAuditContracts", "ExcelAuditReader", "ExcelAuditEngine", "ExcelAuditReportWriter"):
            self.assertIn('<Compile Include="' + name + '.cs"', project)

    def test_report_separates_data_and_other_details_and_is_literal(self):
        writer = source("ExcelAuditReportWriter.cs")
        self.assertIn('"Summary", 1U, result, false', writer)
        self.assertIn('"Data Differences", 2U, result, true', writer)
        self.assertIn("data ? result.DataDetails : result.OtherDetails", writer)
        self.assertIn("CellValues.InlineString", writer)
        self.assertNotIn("new CellFormula", writer)
        self.assertIn("result.IsValidationOnly", writer)
        for safeguard in ("FileMode.CreateNew", "File.Replace(temporary, destination, null)", "File.Move(temporary, destination)",
                          "FormValidation.DifferentFiles(destination, result.Options.FileA", "FileAttributes.ReparsePoint"):
            self.assertIn(safeguard, writer)

    def test_independent_detail_caps_and_full_totals(self):
        contracts = source("ExcelAuditContracts.cs")
        for text in ("dataBytes", "otherBytes", "DataCount++", "ScopeCount++", "Options.MaxDetails", "Options.MaxDetailBytes",
                     "DataCount - DataDetails.Count", "OtherCount - OtherDetails.Count", "!IsValidationOnly && Complete"):
            self.assertIn(text, contracts)

    def test_worker_and_workspace_lifecycle_remain_visible(self):
        form = source("ExcelSheetAuditForm.cs")
        for text in ("class ExcelSheetAuditForm : WorkspacePageForm", "BackgroundWorker worker", "CancellationTokenSource",
                     "FormValidation.LockInputs", "OnFormClosing", "ExcelAuditEngine.Validate", "ExcelAuditEngine.Compare",
                     "ExcelAuditReportWriter.Write", "UiTheme.Apply", "MessageBoxDefaultButton.Button2"):
            self.assertIn(text, form)
        self.assertNotIn("Microsoft.Office.Interop", form)
        self.assertNotIn('"python', form.lower())

    def test_reader_and_key_engine_have_bounded_streaming_contract(self):
        reader = source("ExcelAuditReader.cs")
        engine = source("ExcelAuditEngine.cs")
        for text in ("OpenXmlReader", "FileAccess.Read", "DiskSharedStrings", "MaxRowCharacters", "ThrowIfCancellationRequested"):
            self.assertIn(text, reader)
        for text in ("PartitionStore", "MaxPartitionBytes", "StringComparer.OrdinalIgnoreCase", "DuplicateKey",
                     "AppendPart", "EnsureRowColumns", "result.IsValidationOnly = validationOnly"):
            self.assertIn(text, engine)

    def test_native_fixture_suite_is_compiled_and_wired(self):
        self.assertIn("..\\tests\\ExcelAuditChecks.cs", source("SPI Data Comp Tool.csproj"))
        self.assertIn("ExcelAuditChecks.Run(args[1])", source("SelfTest.cs"))
        workflow = (ROOT / ".github/workflows/build.yml").read_text()
        self.assertIn("--check-excel-audit", workflow)
        fixture = (ROOT / "tests/ExcelAuditChecks.cs").read_text()
        for text in ("OpenXmlValidator", "Duplicate", "CancellationToken", "File.ReadAllBytes", "PresentResult"):
            self.assertIn(text.lower(), fixture.lower())


if __name__ == "__main__":
    unittest.main()
