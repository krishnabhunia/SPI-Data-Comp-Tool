"""Source-routing contracts; CredentialRedactionChecks.cs executes synthetic cases on Windows."""

import re
import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
PROJECT = ROOT / "SPI Data Comp Tool"


def source(name):
    return (PROJECT / name).read_text(encoding="utf-8-sig")


class CredentialRedaction(unittest.TestCase):
    def test_shared_filter_is_bounded_and_fails_closed(self):
        common = source("Common_Data.cs")
        method = common.split("internal static string RedactConnectionCredentials", 1)[1].split("private SqlDataAdapter", 1)[0]
        self.assertIn("RegexOptions.CultureInvariant", common)
        self.assertIn("TimeSpan.FromSeconds(1)", common)
        self.assertIn("catch (RegexMatchTimeoutException)", method)
        fallback = method.split("catch (RegexMatchTimeoutException)", 1)[1]
        self.assertIn('return "[REDACTED: credential filtering timed out]";', fallback)
        self.assertNotIn("return message", fallback)

    def test_ui_helper_masks_messages_details_and_fallback(self):
        helpers = source("FormHelpers.cs")
        show_text = helpers.split("internal static void ShowText", 1)[1].split("internal static void ShowError", 1)[0]
        self.assertIn("control.Text = Common_Data.RedactConnectionCredentials(text);", show_text)
        show_error = helpers.split("internal static void ShowError", 1)[1].split("internal static string FormatElapsed", 1)[0]
        for value in ("ex.Message", "ex.ToString()", "ex1.Message"):
            self.assertIn(f"Common_Data.RedactConnectionCredentials({value})", show_error)
        self.assertIn("control.Text = string.Empty", show_error)

    def test_disk_and_rich_text_routes_still_use_shared_filter(self):
        self.assertIn("strMsg = RedactConnectionCredentials(strMsg);", source("Common_Data.cs"))
        self.assertEqual(source("Logs.cs").count("Common_Data.RedactConnectionCredentials(strLog)"), 3)
        self.assertIn("strMsg = Common_Data.RedactConnectionCredentials(strMsg);", source("UserProfileForm.cs"))

    def test_runtime_regressions_cover_actual_dangerous_value_shapes(self):
        checks = (ROOT / "tests" / "CredentialRedactionChecks.cs").read_text()
        for case in ("single quotes and doubled quote", "double quotes and doubled quote", "quoted multiline password",
                     "unquoted multiline password", "unterminated single quote", "unterminated double quote",
                     "unterminated quoted semicolons", "unterminated double-quoted semicolons",
                     "malformed quoted suffix", "multiple assignments", "long escaped value", "long unquoted value",
                     "non-credential property names", "ThrowOnceControl"):
            self.assertIn(case, checks)

    def test_form_exception_assignments_do_not_bypass_credential_filter(self):
        """Catch routes outside the shared helper, including worker error/status labels."""
        unsafe = []
        for path in PROJECT.glob("*.cs"):
            if path.name.endswith(".Designer.cs"):
                continue
            for number, line in enumerate(path.read_text(encoding="utf-8-sig").splitlines(), 1):
                if line.lstrip().startswith("//"):
                    continue
                assignment = re.search(r"\.Text\s*=\s*(.*);", line)
                if assignment and re.search(r"\b(?:ex|ex1|except|myException|myException_BG|e\.Error)\b", assignment[1]):
                    if "RedactConnectionCredentials(" not in assignment[1]:
                        unsafe.append(f"{path.name}:{number}")
        self.assertEqual(unsafe, [], "Exception text bypasses credential filtering: " + ", ".join(unsafe))


if __name__ == "__main__":
    unittest.main()
