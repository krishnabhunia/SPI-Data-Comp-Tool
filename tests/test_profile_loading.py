"""Contracts for loading the saved connection profiles file in every window."""

import pathlib
import unittest


ROOT = pathlib.Path(__file__).resolve().parents[1]
APP = ROOT / "SPI Data Comp Tool"


def source(name: str) -> str:
    return (APP / name).read_text(encoding="utf-8-sig")


class ProfileLoadingContracts(unittest.TestCase):
    def test_every_profile_window_uses_the_shared_loader(self) -> None:
        for name in ("DB Comparison.cs", "Compare_Multiples_Forms.cs", "Execute Excel Scripts Form.cs", "SQL_DB_To_Excel.cs"):
            with self.subTest(window=name):
                text = source(name)
                self.assertIn("common_Data.ReadUserProfile(cb_UserProfileList)", text)
                shown = "common_Data.LastProfileLoad" if name in ("DB Comparison.cs", "SQL_DB_To_Excel.cs") \
                    else "common_Data.ShowProfileLoad(rtb_Log)"
                self.assertIn(shown, text)   # the window says what happened
                # a case-sensitive ".json" test silently skipped files named *.JSON
                self.assertNotIn('ProfilePathUser, ".json", false, true', text)
        self.assertIn("ProfileStore.LoadSaved()", source("UserProfileForm.cs"))

    def test_reading_the_saved_file_never_throws(self) -> None:
        common = source("Common_Data.cs")
        method = common.split("public ComboBox ReadUserProfile", 1)[1].split("internal void ShowProfileLoad", 1)[0]
        self.assertIn("ProfileStore.LoadSaved()", method)
        self.assertNotIn("throw", method)
        store = source("ProfileStore.cs")
        loader = store.split("internal static ProfileLoadResult LoadSaved()", 1)[1].split("internal static List<DBC> ParseTolerant", 1)[0]
        self.assertIn("catch (Exception ex)", loader)
        self.assertIn("ParseJson(json)", loader)          # strict first ...
        self.assertIn("ParseTolerant(json, out skipped)", loader)   # ... then entry by entry, with a message

    def test_build_checks_never_read_the_users_profiles(self) -> None:
        store = source("ProfileStore.cs")
        self.assertIn("SelfTest.IsRunning ? SelfTestProfilePath : Properties.Settings.Default.ProfilePathUser", store)
        self.assertIn("return SelfTest.IsRunning ? null :", store)

    def test_early_profiles_choose_their_platform(self) -> None:
        common = source("Common_Data.cs")
        self.assertIn("ProfileStore.PlatformOf(items[switchIndex].dbSRC, out srcOracle, out srcSql)", common)
        self.assertIn("ProfileStore.PlatformOf(items[switchIndex].dbTRG, out trgOracle, out trgSql)", common)

    def test_executable_checks_load_real_files(self) -> None:
        checks = (ROOT / "tests" / "UiWorkflowChecks.cs").read_text(encoding="utf-8-sig")
        self.assertIn("ProfileLoadChecks.Run(passed)", checks)
        load = (ROOT / "tests" / "ProfileLoadChecks.cs").read_text(encoding="utf-8-sig")
        for scenario in ("CheckFiles(", "CheckEarlyFormat(", "CheckEveryTaskOpens(", "CheckStepReason("):
            self.assertIn(scenario, load)


if __name__ == "__main__":
    unittest.main()
