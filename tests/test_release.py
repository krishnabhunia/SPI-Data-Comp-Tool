"""Unit tests for tools/release.py (automatic x.y.z versioning)."""

import importlib.util
import unittest
from pathlib import Path

SPEC = importlib.util.spec_from_file_location("release", Path(__file__).resolve().parents[1] / "tools" / "release.py")
release = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(release)


class BumpKind(unittest.TestCase):
    def test_house_title_types(self):
        self.assertEqual(release.bump_kind("Claude : Feature: update button (U1 A, #21) — 2.4.0"), "minor")
        self.assertEqual(release.bump_kind("Claude : Fix: keep logs (#5) — 2.3.1"), "patch")
        self.assertEqual(release.bump_kind("Claude : CI: automatic versioning (#22)"), "patch")
        self.assertEqual(release.bump_kind("Claude : Docs: guide"), "patch")

    def test_conventional_titles(self):
        self.assertEqual(release.bump_kind("feat(ui): new grid"), "minor")
        self.assertEqual(release.bump_kind("fix: crash"), "patch")
        self.assertEqual(release.bump_kind("feat!: drop .NET 4.6.2"), "major")
        self.assertEqual(release.bump_kind("refactor: tidy", body="BREAKING CHANGE: config file renamed"), "major")

    def test_mentioning_breaking_change_is_not_a_breaking_change(self):
        body = "| 2 | Major | breaking/`!`/BREAKING CHANGE -> major |\nNothing breaks here."
        self.assertEqual(release.bump_kind("Claude : CI: automatic versioning (#21)", body=body), "patch")
        self.assertEqual(release.bump_kind("fix: x", body="Notes\n  BREAKING CHANGE: removed option"), "major")

    def test_titles_without_a_type_are_patches(self):
        self.assertEqual(release.bump_kind("ChatGPT-SPI-SQL-Script-Execution-Safety (#18)"), "patch")
        self.assertEqual(release.bump_kind(""), "patch")

    def test_overrides(self):
        self.assertEqual(release.bump_kind("ChatGPT-SPI-Big-Change", labels=["release:minor"]), "minor")
        self.assertEqual(release.bump_kind("Claude : Fix: x", labels=["bug", "release:major"]), "major")
        self.assertEqual(release.bump_kind("Add Excel preview [minor]"), "minor")
        self.assertEqual(release.bump_kind("Claude : Feature: tiny [patch]"), "patch")


class Versions(unittest.TestCase):
    def test_bump(self):
        self.assertEqual(release.bump((2, 3, 0), "patch"), (2, 3, 1))
        self.assertEqual(release.bump((2, 3, 4), "minor"), (2, 4, 0))
        self.assertEqual(release.bump((2, 3, 4), "major"), (3, 0, 0))

    def test_latest_stable_ignores_betas_and_junk(self):
        tags = ["v2.4.0-beta001", "v2.3.1", "v2.3.0", "v10.0.0-beta002", "release-old", "v2.3"]
        self.assertEqual(release.latest_stable(tags, (2, 3, 0)), (2, 3, 1))
        self.assertEqual(release.latest_stable(["v9.9.9-beta001"], (2, 3, 0)), (2, 3, 0))

    def test_numeric_not_text_order(self):
        self.assertEqual(release.latest_stable(["v2.9.0", "v2.10.0"], (0, 0, 0)), (2, 10, 0))

    def test_beta_numbers_count_per_version(self):
        tags = ["v2.4.0-beta001", "v2.4.0-beta002", "v2.3.1-beta007"]
        self.assertEqual(release.beta_number(tags, (2, 4, 0), []), (3, True))
        self.assertEqual(release.beta_number(tags, (2, 5, 0), []), (1, True))

    def test_rerun_of_same_commit_keeps_its_number(self):
        tags = ["v2.4.0-beta001", "v2.4.0-beta002"]
        self.assertEqual(release.beta_number(tags, (2, 4, 0), ["v2.4.0-beta001"]), (1, False))

    def test_names_and_ranking(self):
        self.assertEqual(release.fmt((2, 4, 0), 7), "2.4.0-beta007")
        self.assertEqual(release.fmt((2, 4, 0)), "2.4.0")
        ranked = sorted(["v2.4.0", "v2.4.0-beta010", "v2.3.9", "v2.4.0-beta002"], key=release.sort_key)
        self.assertEqual(ranked, ["v2.3.9", "v2.4.0-beta002", "v2.4.0-beta010", "v2.4.0"])


class Stamp(unittest.TestCase):
    SAMPLE = '[assembly: AssemblyVersion("2.3.0.0")]\n[assembly: AssemblyFileVersion("2.3.0.0")]\n'

    def test_beta_stamp(self):
        out = release.stamp_text(self.SAMPLE, "2.4.0-beta003")
        self.assertIn('AssemblyVersion("2.4.0.0")', out)
        self.assertIn('AssemblyFileVersion("2.4.0.0")', out)
        self.assertIn('AssemblyInformationalVersion("2.4.0-beta003")', out)

    def test_stamp_twice_replaces(self):
        out = release.stamp_text(release.stamp_text(self.SAMPLE, "2.4.0-beta003"), "2.4.0")
        self.assertEqual(out.count("AssemblyInformationalVersion"), 1)
        self.assertIn('AssemblyInformationalVersion("2.4.0")', out)

    def test_baseline_from_repo_file(self):
        text = (Path(__file__).resolve().parents[1] / "SPI Data Comp Tool" / "Properties" / "AssemblyInfo.cs").read_text(encoding="utf-8-sig")
        self.assertEqual(len(release.baseline_from_assembly_info(text)), 3)


if __name__ == "__main__":
    unittest.main()
