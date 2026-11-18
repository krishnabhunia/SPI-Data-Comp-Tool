"""Static contracts for the GitHub Actions workflows: no deprecation warnings or migration notices."""

import re
import unittest
from pathlib import Path

WORKFLOWS = sorted((Path(__file__).resolve().parents[1] / ".github" / "workflows").glob("*.yml"))

# first major version of each action that runs on Node.js 24 (older ones raise a deprecation warning)
NODE24_FROM = {
    "actions/checkout": 6,
    "actions/upload-artifact": 6,
    "actions/download-artifact": 7,
    "actions/setup-python": 6,
    "microsoft/setup-msbuild": 3,
    "nuget/setup-nuget": 3,
}


class Workflows(unittest.TestCase):
    def test_there_are_workflows_to_check(self) -> None:
        self.assertGreaterEqual(len(WORKFLOWS), 2)

    def test_every_action_runs_on_node_24(self) -> None:
        for path in WORKFLOWS:
            for action, version in re.findall(r"uses:\s*([\w.-]+/[\w.-]+)@v(\d+)", path.read_text(encoding="utf-8")):
                with self.subTest(workflow=path.name, action=action):
                    self.assertIn(action.lower(), NODE24_FROM, f"{action}: add its first Node.js 24 version to this test")
                    self.assertGreaterEqual(int(version), NODE24_FROM[action.lower()])

    def test_every_action_is_pinned_to_a_major_version(self) -> None:
        for path in WORKFLOWS:
            for ref in re.findall(r"uses:\s*(\S+)", path.read_text(encoding="utf-8")):
                with self.subTest(workflow=path.name, action=ref):
                    self.assertRegex(ref, r"@v\d+$")

    def test_runner_images_are_named_not_latest(self) -> None:
        for path in WORKFLOWS:
            for image in re.findall(r"runs-on:\s*(\S+)", path.read_text(encoding="utf-8")):
                with self.subTest(workflow=path.name, image=image):
                    self.assertNotIn("latest", image)


if __name__ == "__main__":
    unittest.main()
