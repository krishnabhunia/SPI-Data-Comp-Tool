"""Code signing: master builds sign every published exe once the owner sets up signing; pull requests never see the secrets."""

import re
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
WORKFLOW = (ROOT / ".github/workflows/build.yml").read_text(encoding="utf-8")
SCRIPT = (ROOT / "tools/sign_windows.ps1").read_text(encoding="utf-8")
SECRETS = ("SIGN_ES_USERNAME", "SIGN_ES_PASSWORD", "SIGN_ES_CREDENTIAL_ID", "SIGN_ES_TOTP_SECRET")


def job(name: str) -> str:
    return WORKFLOW.split(f"\n  {name}:\n", 1)[1].split("\n\n  ", 1)[0]


def step_index(text: str, name: str) -> int:
    index = text.find("- name: " + name)
    if index < 0:
        raise AssertionError("missing step: " + name)
    return index


class CodeSigning(unittest.TestCase):
    def test_pull_requests_never_see_the_signing_secrets(self) -> None:
        # Codex security review on #115: unmerged code must not read the secrets or get itself signed.
        sign = job("sign")
        self.assertIn("if: github.event_name == 'push' && github.ref == 'refs/heads/master'", sign)
        self.assertIn("environment: code-signing", sign)
        self.assertNotIn("secrets.SIGN_ES_", job("windows-build"))
        for name in SECRETS:
            with self.subTest(secret=name):
                # only the check and the two signing steps receive them, as step env
                self.assertEqual(WORKFLOW.count(f"secrets.{name}"), 3)
                self.assertEqual(sign.count(f"{name}: ${{{{ secrets.{name} }}}}"), 3)
        self.assertNotRegex(sign.split("    steps:", 1)[0], "SIGN_ES_")  # not job-level env

    def test_every_published_exe_is_signed_before_upload_and_hashing(self) -> None:
        sign = job("sign")
        self.assertIn("./tools/sign_windows.ps1 -CheckOnly", sign)
        both = step_index(sign, "Sign the portable exe and the 64-bit exe the installer copies")
        self.assertLess(step_index(sign, "Rebuild the 64-bit application"), both)
        self.assertLess(both, step_index(sign, "Rebuild the installer around the signed exe"))
        setup = step_index(sign, "Sign the installer")
        self.assertLess(step_index(sign, "Rebuild the installer around the signed exe"), setup)
        self.assertLess(setup, step_index(sign, "Signed installer still installs a signed app"))
        self.assertLess(setup, sign.index("actions/upload-artifact"))
        self.assertIn("overwrite: true", sign)
        self.assertEqual(sign.count("if: steps.setup.outputs.configured == 'true'"), 9)
        release = job("release")
        self.assertIn("needs: [version, windows-build, sign, documentation, secrets-guard]", release)
        self.assertIn('contains(fromJSON(\'["success", "skipped"]\'), needs.sign.result)', release)
        self.assertIn("sha256sum", release)  # checksums come from the uploaded (signed) files

    def test_unconfigured_signing_is_skipped_quietly_and_partial_set_up_fails(self) -> None:
        self.assertIn("if ($set.Count -eq 0) { return $false }", SCRIPT)
        self.assertIn("Code signing is only partly set up", SCRIPT)
        skipped = SCRIPT.split("if (-not $configured) {", 1)[1].split("}", 1)[0]
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

    def test_check_only_reports_to_the_workflow(self) -> None:
        self.assertIn("[switch] $CheckOnly", SCRIPT)
        self.assertIn('"configured=$($configured.ToString().ToLowerInvariant())"', SCRIPT)
        self.assertIn("$env:GITHUB_OUTPUT", SCRIPT)

    def test_the_owner_has_a_set_up_guide(self) -> None:
        guide = (ROOT / "docs/CODE_SIGNING.md").read_text(encoding="utf-8")
        for name in SECRETS:
            self.assertIn(name, guide)
        self.assertIn("SmartScreen", guide)
        self.assertIn("code-signing", guide)
        self.assertIn("master", guide)
        self.assertIn("docs/CODE_SIGNING.md", (ROOT / "README.md").read_text(encoding="utf-8"))


if __name__ == "__main__":
    unittest.main()
