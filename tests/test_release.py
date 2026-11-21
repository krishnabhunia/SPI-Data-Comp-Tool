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


class VersionOnThePullRequest(unittest.TestCase):
    """The version is written on the PR by CI (one comment), not typed into the PR title."""

    REPO = "owner/repo"
    WORKFLOW = (Path(__file__).resolve().parents[1] / ".github" / "workflows" / "build.yml").read_text(encoding="utf-8")

    def test_beta_comment_names_the_beta_and_links_its_files(self):
        body = release.comment_body("beta", self.REPO, "v2.5.3-beta004", "abcdef1234567")
        self.assertTrue(body.startswith(release.COMMENT_MARKER))
        self.assertIn("### Beta version: `v2.5.3-beta004`", body)
        self.assertIn("commit `abcdef1`", body)
        self.assertIn("https://github.com/owner/repo/releases/tag/v2.5.3-beta004", body)
        self.assertIn("https://github.com/owner/repo/releases/download/v2.5.3-beta004/SPI_Data_Comp_Tool_2.5.3-beta004_x64_setup.exe", body)
        self.assertIn("can differ from this beta's number", body)
        self.assertIn("Requires Microsoft .NET Framework 4.8 or later", body)
        self.assertIn("before updating an older portable copy", body)
        self.assertIn("--notes-file docs/RELEASE_PREREQUISITES.md", self.WORKFLOW)

    def test_stable_comment_names_the_normal_version(self):
        body = release.comment_body("stable", self.REPO, "v2.5.3", "abcdef1234567")
        self.assertIn("### Released as `v2.5.3`", body)
        self.assertIn("/releases/download/v2.5.3/SPI_Data_Comp_Tool_2.5.3.zip)", body)
        self.assertNotIn("beta", body.lower())

    def test_outdated_comment_says_nothing_was_published_and_links_no_files(self):
        body = release.comment_body("outdated", self.REPO, "v2.5.1-beta001", "abcdef1234567")
        self.assertIn("`v2.5.1` was released while this build was running", body)
        self.assertIn("`v2.5.1-beta001` was **not published**", body)
        self.assertNotIn("/releases/download/", body)

    def test_unknown_state_or_tag_is_refused(self):
        with self.assertRaises(SystemExit):
            release.comment_body("draft", self.REPO, "v2.5.3", "abc")
        with self.assertRaises(SystemExit):
            release.comment_body("beta", self.REPO, "2.5.3", "abc")

    def test_comment_links_match_the_files_the_workflow_uploads(self):
        for _, name in release_assets_as_workflow_names():
            if name.endswith(".zip.sha256"):   # the workflow builds this name from the zip's name
                name = '"${zip_name}.sha256"'
            self.assertTrue(name in self.WORKFLOW, f"{name} is not uploaded by build.yml")

    def test_workflow_shows_the_version_and_skips_an_outdated_beta(self):
        self.assertIn("name: Publish ${{ needs.version.outputs.version }}", self.WORKFLOW)
        self.assertIn("pull-requests: write", self.WORKFLOW)
        self.assertIn(f'startswith("{release.COMMENT_MARKER}")', self.WORKFLOW)
        self.assertIn('stable="v${VERSION%%-beta*}"', self.WORKFLOW)
        self.assertIn("state=outdated", self.WORKFLOW)
        # the outdated check runs before anything is published
        self.assertLess(self.WORKFLOW.index("state=outdated"), self.WORKFLOW.index("gh release create"))

    def test_a_late_pr_run_cannot_replace_the_released_comment(self):
        step = self.WORKFLOW.split("- name: Show the version on the pull request", 1)[1]
        guard = 'if [ "${STATE}" != "stable" ] && [ "$(gh api "repos/${GH_REPO}/pulls/${PR}" --jq .merged)" = "true" ]; then'
        self.assertIn(guard, step)
        # the merged check comes before the comment is written or sent
        self.assertLess(step.index(guard), step.index("tools/release.py comment"))
        self.assertLess(step.index(guard), step.index("-X PATCH"))


def release_assets_as_workflow_names():
    """release_assets() with the version written the way the workflow's shell does."""
    return [(what, name.replace("{V}", "${VERSION}")) for what, name in release.release_assets("{V}")]


if __name__ == "__main__":
    unittest.main()
