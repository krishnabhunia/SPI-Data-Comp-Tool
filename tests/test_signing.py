"""Signing eligibility, tamper/provenance rejection and release-status tests."""
import importlib.util
import json
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[1]


def module(name):
    spec = importlib.util.spec_from_file_location(name, ROOT / "tools" / f"{name}.py")
    result = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(result)
    return result


policy, handoff, release = (module(name) for name in ("signing_policy", "signing_handoff", "release"))


class SigningPolicy(unittest.TestCase):
    def config(self, **changes):
        args = dict(mode="store", event="push", ref="refs/heads/master", protected="true", thumbprint="a" * 40,
                    store="CurrentUser", timestamp="https://timestamp.example.test/rfc3161", isolated="true")
        args.update(changes)
        return args

    def test_unconfigured_and_off_need_no_credentials(self):
        for mode in ("", "off"):
            self.assertEqual(policy.plan(mode, "push", "refs/heads/master", "false"), {"enabled": "false", "status": "unsigned"})

    def test_all_prs_stay_unsigned_without_a_signing_identity(self):
        self.assertEqual(policy.plan("store", "pull_request", "refs/pull/116/merge", "false"), {"enabled": "false", "status": "unsigned"})

    def test_valid_protected_release_is_eligible(self):
        self.assertEqual(policy.plan(**self.config()), {"enabled": "true", "status": "signed"})

    def test_unknown_mode_is_not_an_unsigned_fallback(self):
        for mode in ("STORE", "true", "disabled", "off\nstore"):
            with self.subTest(mode=mode), self.assertRaises(ValueError):
                policy.plan(**self.config(mode=mode))

    def test_incomplete_or_unsafe_enabled_configuration_fails(self):
        for changes in ({"event": "workflow_dispatch"}, {"event": "pull_request_target"}, {"ref": "refs/heads/topic"},
                        {"protected": "false"}, {"thumbprint": ""}, {"thumbprint": "a" * 39}, {"store": "Root"},
                        {"isolated": ""}, {"timestamp": ""}, {"timestamp": "file:///tmp/anything"},
                        {"timestamp": "https://name:secret@example.test"}, {"timestamp": "https://example.test/?token=secret"},
                        {"timestamp": "https://example.test/`command"}, {"timestamp": 'https://example.test/"'}):
            with self.subTest(changes=changes), self.assertRaises(ValueError):
                policy.plan(**self.config(**changes))


class Handoff(unittest.TestCase):
    version = "4.4.1"
    sha = "a" * 40

    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.dist, self.x64, self.bundle = (self.root / n for n in ("dist", "x64", "input"))
        for path in handoff.portable_paths(self.version) | {"windows-x64/SPI_Data_Comp_Tool_4.4.1.exe", "THIRD_PARTY_NOTICES.md", handoff.store_path(self.version)}:
            file = self.dist / path
            file.parent.mkdir(parents=True, exist_ok=True)
            file.write_bytes(b"" if path.endswith("instrument.txt") else b"test build bytes")
        self.x64.mkdir()
        for name in ("SPI Data Comp Tool.exe", "SPI Data Comp Tool.exe.config"):
            (self.x64 / name).write_bytes(b"x64 test bytes")
        handoff.prepare(self.dist, self.x64, self.bundle, self.version, self.sha, "123", "2")

    def verify(self, **changes):
        args = dict(root=self.bundle, version=self.version, sha=self.sha, run_id="123", attempt="2")
        args.update(changes)
        handoff.verify(**args)

    def test_valid_exact_current_run_handoff_and_distribution(self):
        self.verify()
        handoff.check_distribution(self.dist, self.version)

    def test_altered_payload_is_rejected(self):
        (self.bundle / "x64/SPI Data Comp Tool.exe").write_bytes(b"altered")
        with self.assertRaises(ValueError):
            self.verify()

    def test_store_submission_is_carried_unchanged_and_tampering_is_rejected(self):
        name = handoff.store_path(self.version)
        self.assertEqual((self.bundle / name).read_bytes(), (self.dist / name).read_bytes())
        (self.bundle / name).write_bytes(b"altered store submission")
        with self.assertRaises(ValueError):
            self.verify()

    def test_missing_or_extra_store_submission_is_rejected(self):
        file = self.bundle / handoff.store_path(self.version)
        original = file.read_bytes()
        file.unlink()
        with self.assertRaises(ValueError):
            self.verify()
        file.write_bytes(original)
        (file.parent / "unexpected.msix").write_bytes(original)
        with self.assertRaises(ValueError):
            self.verify()

    def test_wrong_commit_run_attempt_and_version_are_rejected(self):
        for changes in ({"sha": "b" * 40}, {"run_id": "124"}, {"attempt": "3"}, {"version": "4.4.2"}):
            with self.subTest(changes=changes), self.assertRaises(ValueError):
                self.verify(**changes)

    def test_manifest_cannot_select_additional_targets(self):
        path = self.bundle / "handoff.json"
        record = json.loads(path.read_text())
        record["sha256"]["../../other.exe"] = "0" * 64
        path.write_text(json.dumps(record))
        with self.assertRaises(ValueError):
            self.verify()

    def test_extra_or_missing_files_and_nonempty_anchors_are_rejected(self):
        extra = self.bundle / "extra.exe"
        extra.write_bytes(b"extra")
        with self.assertRaises(ValueError):
            self.verify()
        extra.unlink()
        anchor = self.bundle / "portable/PNID_Reports/Source/instrument.txt"
        anchor.write_bytes(b"unexpected content")
        with self.assertRaises(ValueError):
            self.verify()
        anchor.unlink()
        with self.assertRaises(ValueError):
            self.verify()

    def test_unsafe_version_and_source_metadata_are_rejected(self):
        for version in ("../../4.4.1", "4.4.1/other", "4.4.1\n", "4.4.1-beta1"):
            with self.subTest(version=version), self.assertRaises(ValueError):
                handoff.portable_paths(version)
        with self.assertRaises(ValueError):
            handoff.metadata(self.version, self.sha, "123/other", "2")


class SigningReleaseStatus(unittest.TestCase):
    def test_unsigned_is_the_default_and_does_not_claim_a_fix(self):
        text = release.comment_body("beta", "owner/repo", "v4.4.1-beta001", "a" * 40)
        self.assertIn("Windows signing: unsigned", text)
        self.assertIn("warning is not fixed", text)

    def test_signed_status_explains_remaining_reputation(self):
        text = release.comment_body("stable", "owner/repo", "v4.4.1", "a" * 40, "signed")
        self.assertIn("signed and verified", text)
        self.assertIn("warnings can still appear", text)

    def test_invalid_status_cannot_claim_signing(self):
        with self.assertRaises(ValueError):
            release.signing_notice("success")

    def test_cloud_backend_does_not_claim_complete_installer_coverage(self):
        text = release.comment_body("stable", "owner/repo", "v4.4.1", "a" * 40, "signed-app-and-setup")
        self.assertIn("application and installer signed", text)
        self.assertIn("not covered by this backend", text)
        self.assertNotIn("signing: signed and verified", text)

    def test_exe_signing_never_claims_a_store_published_msix(self):
        for signing in ("unsigned", "signed", "signed-app-and-setup"):
            text = release.comment_body("stable", "owner/repo", "v4.4.1", "a" * 40, signing)
            self.assertIn("unsigned Partner Center submission package", text)
            self.assertIn("Store certification and Microsoft-signed distribution are still required", text)


if __name__ == "__main__":
    unittest.main()
