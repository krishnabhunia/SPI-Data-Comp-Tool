"""Startup logging contracts; LoggingChecks exercises injected I/O failures on Windows."""

import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


class LoggingContracts(unittest.TestCase):
    def test_logging_does_not_run_file_io_in_a_static_constructor(self):
        source = (APP / "Logs.cs").read_text(encoding="utf-8-sig")
        self.assertNotIn("static Logs()", source)
        self.assertNotIn("new Common_Data()", source)
        self.assertIn("Environment.SpecialFolder.LocalApplicationData", source)
        self.assertIn("if (SelfTest.IsRunning) return new string[0];", source)

    def test_windows_fault_suite_is_compiled_and_run(self):
        project = (APP / "SPI Data Comp Tool.csproj").read_text(encoding="utf-8-sig")
        checks = (ROOT / "tests" / "UiWorkflowChecks.cs").read_text(encoding="utf-8-sig")
        self.assertTrue('Compile Include="..\\tests\\LoggingChecks.cs"' in project,
                        "The Windows logging fault suite must be included in the application project.")
        self.assertTrue("LoggingChecks.Run(passed)" in checks,
                        "The Windows UI runner must execute the logging fault suite.")

    def test_fault_checks_include_ui_errors_redaction_and_parallel_writes(self):
        checks = (ROOT / "tests" / "LoggingChecks.cs").read_text(encoding="utf-8-sig")
        self.assertIn("Common_Data.DisplayError", checks)
        self.assertIn("new UnauthorizedAccessException", checks)
        self.assertIn("Synthetic disk/connection loss during flush", checks)
        self.assertIn("Synthetic close failure", checks)
        self.assertIn("Task.WaitAll(writers)", checks)
        self.assertIn("Path.GetTempPath()", checks)
        self.assertIn("Logs.UseForChecks(log)", checks)


if __name__ == "__main__":
    unittest.main()
