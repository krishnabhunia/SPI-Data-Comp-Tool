"""Contracts for finding the saved profiles file from a new copy of the app, and for step reasons."""

import pathlib
import unittest


ROOT = pathlib.Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


def source(name: str) -> str:
    return (APP / name).read_text(encoding="utf-8-sig")


class ProfileLoadingContracts(unittest.TestCase):
    def test_new_copy_of_the_app_finds_the_remembered_file(self) -> None:
        store = source("ProfileStore.cs")
        self.assertIn("try { return LoadWithRememberedFallback(readPath()); }", store)
        fallback = store.split("private static ProfileLoadResult LoadWithRememberedFallback", 1)[1].split("internal static string RememberedPathFileOverride", 1)[0]
        self.assertIn("ReadRememberedPath()", fallback)
        self.assertIn("if (result.Loaded) RememberPath(path);", fallback)
        self.assertNotIn("Properties.Settings.Default", fallback)   # never written into user.config
        self.assertIn("ProfileStore.RememberPath(path)", source("UserProfileForm.cs"))

    def test_build_checks_never_touch_the_users_appdata(self) -> None:
        self.assertIn("return SelfTest.IsRunning ? null :", source("ProfileStore.cs"))

    def test_step_strip_says_what_is_missing(self) -> None:
        flow = source("StepFlow.cs")
        self.assertIn("lastFailure = ex.Message;", flow)
        self.assertIn("step.Reason = open && !done ? lastFailure : null;", flow)

    def test_executable_checks_are_wired(self) -> None:
        checks = (ROOT / "tests" / "UiWorkflowChecks.cs").read_text(encoding="utf-8-sig")
        self.assertIn("ProfileLoadChecks.Run(passed)", checks)
        load = (ROOT / "tests" / "ProfileLoadChecks.cs").read_text(encoding="utf-8-sig")
        for scenario in ("CheckRememberedPath(", "CheckStepReason("):
            self.assertIn(scenario, load)


if __name__ == "__main__":
    unittest.main()
