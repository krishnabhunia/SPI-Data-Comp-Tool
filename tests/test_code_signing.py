"""Code signing: the build signs every published exe once the owner adds the signing secrets, and is unchanged before."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
WORKFLOW = (ROOT / ".github/workflows/build.yml").read_text(encoding="utf-8")
SCRIPT = (ROOT / "tools/sign_windows.ps1").read_text(encoding="utf-8")
SECRETS = ("SIGN_ES_USERNAME", "SIGN_ES_PASSWORD", "SIGN_ES_CREDENTIAL_ID", "SIGN_ES_TOTP_SECRET")


def windows_build() -> str:
    return WORKFLOW.split("\n  windows-build:\n", 1)[1].split("\n  release:\n", 1)[0]


def step_index(job: str, name: str) -> int:
    index = job.find("- name: " + name)
    if index < 0:
        raise AssertionError("missing step: " + name)
    return index


class CodeSigning(unittest.TestCase):
    def test_secrets_reach_only_the_windows_build(self) -> None:
        job = windows_build()
        for name in SECRETS:
            with self.subTest(secret=name):
                self.assertIn(f"{name}: ${{{{ secrets.{name} }}}}", job)
                self.assertEqual(WORKFLOW.count(f"secrets.{name}"), 1)

    def test_every_published_exe_is_signed_before_it_is_tested_packaged_or_hashed(self) -> None:
        job = windows_build()
        portable = step_index(job, "Sign the portable exe")
        self.assertLess(step_index(job, "Package portable build"), portable)
        self.assertLess(portable, step_index(job, "Portable build is one exe (libraries embedded)"))
        app = step_index(job, "Sign the 64-bit exe that the installer copies")
        self.assertLess(step_index(job, "Build 64-bit application"), app)
        self.assertLess(app, step_index(job, "Build 64-bit installer"))
        setup = step_index(job, "Sign the installer")
        self.assertLess(step_index(job, "Build 64-bit installer"), setup)
        self.assertLess(setup, step_index(job, "Test the installer (silent install, check, uninstall)"))
        self.assertLess(setup, job.index("actions/upload-artifact"))
        self.assertEqual(job.count("./tools/sign_windows.ps1"), 3)
        # The release checksums are computed from the uploaded (signed) files, after this job.
        self.assertIn("sha256sum", WORKFLOW.split("\n  release:\n", 1)[1])

    def test_unconfigured_signing_is_skipped_quietly_and_partial_set_up_fails(self) -> None:
        self.assertIn("if ($set.Count -eq 0) { return $false }", SCRIPT)
        self.assertIn("Code signing is only partly set up", SCRIPT)
        skipped = SCRIPT.split("if (-not (Test-SigningConfigured)) {", 1)[1].split("}", 1)[0]
        self.assertIn("exit 0", skipped)
        self.assertNotIn("::warning", skipped)
        self.assertNotIn("::notice", skipped)
        for name in SECRETS:
            self.assertIn(f"'{name}'", SCRIPT)

    def test_the_signing_tool_is_pinned_and_checked(self) -> None:
        self.assertIn("$ToolVersion = 'v1.3.0'", SCRIPT)
        self.assertRegex(SCRIPT, r"\$ToolSha256 = '[0-9A-F]{64}'")
        download = SCRIPT.index("Invoke-WebRequest")
        self.assertLess(download, SCRIPT.index("Get-FileHash $zip -Algorithm SHA256"))
        self.assertLess(SCRIPT.index("Get-FileHash $zip -Algorithm SHA256"), SCRIPT.index("Expand-Archive"))

    def test_signatures_are_verified_and_time_stamped_and_secrets_never_printed(self) -> None:
        self.assertIn("Get-AuthenticodeSignature $file", SCRIPT)
        self.assertIn("$signature.Status -ne 'Valid'", SCRIPT)
        self.assertIn("TimeStamperCertificate", SCRIPT)
        self.assertNotRegex(SCRIPT, r"Write-Host[^\n]*\$env:SIGN_ES_")
        self.assertNotIn("CodeSignTool.bat sign", SCRIPT)  # java is started directly: no cmd quoting of secrets

    def test_the_owner_has_a_set_up_guide(self) -> None:
        guide = (ROOT / "docs/CODE_SIGNING.md").read_text(encoding="utf-8")
        for name in SECRETS:
            self.assertIn(name, guide)
        self.assertIn("SmartScreen", guide)
        self.assertIn("docs/CODE_SIGNING.md", (ROOT / "README.md").read_text(encoding="utf-8"))


if __name__ == "__main__":
    unittest.main()
