"""Cross-platform contracts; executable WinForms/file tests live in ProfileWorkflowChecks.cs."""

import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PROJECT = ROOT / "SPI Data Comp Tool"


def source(name: str) -> str:
    return (PROJECT / name).read_text(encoding="utf-8-sig")


class ProfileSafety(unittest.TestCase):
    def test_new_names_and_stale_selection_do_not_apply_empty_profiles(self) -> None:
        panel = source("ConnectionPanel.cs")
        method = panel.split("public void ApplySelectedProfile()", 1)[1].split("public void RefreshProfiles", 1)[0]
        self.assertIn("ProfileList.SelectedIndex < 0", method)
        self.assertIn("Profile.ProfileList.FindIndex", method)
        self.assertIn("string.Equals(p.profileName, selectedName, StringComparison.Ordinal)", method)
        self.assertIn("if (profileIndex < 0)", method)
        self.assertNotIn("new DBConnectionDetails(ProfileList.SelectedIndex)", method)
        self.assertIn("Failed(this, error)", method)

    def test_opening_picker_refreshes_only_names(self) -> None:
        panel = source("ConnectionPanel.cs")
        self.assertIn("ProfileList.DropDown += (s, e) => Run(RefreshAvailableProfiles);", panel)
        method = panel.split("internal void RefreshAvailableProfiles()", 1)[1].split("private void PopulateProfileNames", 1)[0]
        self.assertIn("AllowNewProfileName && selectedName == null ? ProfileList.Text : null", method)
        self.assertIn("PopulateProfileNames(selectedName, false, enteredName)", method)
        self.assertNotIn("ApplySelectedProfile", method)
        self.assertIn("if (refreshingProfiles || ProfileList.SelectedIndex < 0)", panel)

    def test_saving_uses_a_new_snapshot_and_publishes_only_after_writing(self) -> None:
        form = source("UserProfileForm.cs")
        method = form.split("private void btnSaveUserProfile_Click", 1)[1].split("private DBConnectionDetailsProfile", 1)[0]
        self.assertNotIn("dbc_AddNewUserProfile", form)
        self.assertIn("var replacement = new DBC", method)
        self.assertIn("ProfileStore.Upsert(Profile.ProfileList, replacement)", method)
        self.assertIn("SaveSelectedProfile(candidate, profileName", method)
        self.assertIn("if (!saveFileDialogFile.BoolFileSaveStatus) return null", method)
        store_save = source("ProfileStore.cs").split("internal static ProfileLoadResult SaveFile", 1)[1].split("internal static void RequireCurrentCatalog", 1)[0]
        self.assertLess(store_save.index("WriteAtomic(fullPath, json)"), store_save.index("Publish(candidate, fullPath)"))
        self.assertLess(store_save.index("Publish(candidate, fullPath)"), store_save.index("RememberChoice("))
        self.assertIn("connection.RefreshProfiles(selectedName, false)", form)
        self.assertNotIn("items.Remove", method)

    def test_deletion_is_confirmed_and_published_only_after_writing(self) -> None:
        form = source("UserProfileForm.cs")
        method = form.split("private void btnDeleteUserProfile_Click", 1)[1].split("private static void SetProfilePath", 1)[0]
        self.assertIn("MessageBoxDefaultButton.Button2", method)
        self.assertIn("== DialogResult.Yes", method)
        self.assertIn("if (!confirm(selectedName))", method)
        self.assertIn("string profilePath = editorCatalog?.FilePath;", method)
        self.assertLess(method.index("MessageBox.Show"), method.index("DeleteProfileFile(selectedName)"))
        self.assertNotIn("Properties.Settings.Default.ProfilePathUser", method)
        delete = source("ProfileStore.cs").split("internal static ProfileCatalog DeleteProfile", 1)[1].split("private static ProfileCatalog Publish", 1)[0]
        self.assertLess(delete.index("RequireUnchangedFile(expected)"), delete.index("WriteAtomic"))
        self.assertLess(delete.index("WriteAtomic"), delete.index("Publish(candidate, expected.FilePath)"))

    def test_save_and_delete_sanitize_every_exported_profile(self) -> None:
        form = source("UserProfileForm.cs")
        self.assertNotIn("JsonConvert.SerializeObject", form)
        store = source("ProfileStore.cs")
        save = store.split("internal static ProfileLoadResult SaveFile", 1)[1].split("internal static void RequireCurrentCatalog", 1)[0]
        delete = store.split("internal static ProfileCatalog DeleteProfile", 1)[1].split("private static ProfileCatalog Publish", 1)[0]
        self.assertIn("SerializeForExport(candidate)", save)
        self.assertIn("SerializeForExport(candidate)", delete)
        export = store.split("internal static List<DBC> CreateExportSnapshot", 1)[1].split("internal static void Validate", 1)[0]
        self.assertIn("foreach (DBC profile in profiles)", export)
        self.assertIn("DBC snapshot = Clone(profile)", export)
        self.assertIn("snapshot.dbSRC.str_Password = string.Empty", export)
        self.assertIn("snapshot.dbTRG.str_Password = string.Empty", export)
        self.assertIn("JsonConvert.SerializeObject(CreateExportSnapshot(profiles), Formatting.Indented)", export)
        self.assertNotIn("profile.dbSRC.str_Password =", export)
        self.assertNotIn("profile.dbTRG.str_Password =", export)

    def test_profile_status_and_exception_log_uses_shared_redaction(self) -> None:
        method = source("UserProfileForm.cs").split("private void DisplayMessage", 1)[1].split("private void FillCheckListBox", 1)[0]
        self.assertLess(method.index("Common_Data.RedactConnectionCredentials(strMsg)"), method.index("rtb_Log.SelectionStart"))

    def test_import_is_validated_before_replacing_active_profiles(self) -> None:
        form = source("UserProfileForm.cs")
        method = form.split("private void l_lb_ReadProfile_LinkClicked", 1)[1].split("private void boolToggleControlEnable", 1)[0]
        self.assertIn("ImportProfileFile(openFileDialogData.strFileNameWithPath)", method)
        self.assertIn("ProfileStore.ImportFile(path", method)
        store = source("ProfileStore.cs")
        load = store.split("internal static ProfileLoadResult LoadSavedProfiles(string path)", 1)[1].split("internal static ProfileLoadResult ImportFile", 1)[0]
        self.assertLess(load.index("ReadFile(path)"), load.index("Publish(candidate, path)"))
        self.assertIn('profile.GetValue("dbSRC", StringComparison.OrdinalIgnoreCase) is JObject', store)
        self.assertIn('profile.GetValue("dbTRG", StringComparison.OrdinalIgnoreCase) is JObject', store)
        self.assertLess(store.index('profile.GetValue("dbSRC"'), store.index("array.ToObject<List<DBC>>()"))

    def test_saved_profiles_use_the_same_validated_reader_without_applying_early(self) -> None:
        store = source("ProfileStore.cs")
        load = store.split("internal static ProfileLoadResult LoadSavedProfiles(string path)", 1)[1].split("internal static ProfileLoadResult ImportFile", 1)[0]
        self.assertLess(load.index("ReadFile(path)"), load.index("Publish(candidate, path)"))
        self.assertIn("error is InvalidDataException", load)
        self.assertNotIn("Properties.Settings.Default.Save", load)
        self.assertIn("Settings > Connection profiles > Load profile", store)
        self.assertIn("LoadRememberedProfiles(() => Properties.Settings.Default.ProfilePathUser)", store)
        self.assertIn("error is ConfigurationErrorsException", store)
        panel = source("ConnectionPanel.cs")
        panel_load = panel.split("internal ProfileLoadResult LoadSavedProfiles()", 1)[1].split("internal void RefreshAvailableProfiles()", 1)[0]
        self.assertIn("RefreshAvailableProfiles()", panel_load)
        self.assertNotIn("ApplySelectedProfile", panel_load)
        form = source("UserProfileForm.cs")
        self.assertIn("ProfileLoadResult load = connection.LoadSavedProfiles()", form)
        constructor = form.split("public UserProfileForm()", 1)[1].split("private void Connection_ProfileApplied", 1)[0]
        self.assertNotIn("SelfTest.IsRunning", constructor)
        self.assertIn("if (!SelfTest.IsRunning) Properties.Settings.Default.Save();", form)
        legacy = source("Common_Data.cs").split("public ComboBox ReadUserProfile", 1)[1].split("public DataTable GenerateTransposeTable", 1)[0]
        self.assertNotIn("DeserializeObject", legacy)
        self.assertNotIn("StreamReader", legacy)
        self.assertIn("ProfileStore.LoadSavedProfiles()", legacy)

    def test_file_provenance_cannot_follow_an_unrelated_catalog(self) -> None:
        store = source("ProfileStore.cs")
        self.assertIn("ReferenceEquals(Profile.ProfileList, publishedCatalog.Profiles)", store)
        self.assertIn("ReferenceEquals(expected, ActiveCatalog)", store)
        form = source("UserProfileForm.cs")
        self.assertIn("ReferenceEquals(editorProfiles, Profile.ProfileList)", form)
        self.assertIn("ReferenceEquals(editorCatalog, ProfileStore.ActiveCatalog)", form)

    def test_atomic_write_never_truncates_the_original(self) -> None:
        store = source("ProfileStore.cs")
        method = store.split("internal static void WriteAtomic", 1)[1]
        self.assertIn("FileMode.CreateNew", method)
        self.assertIn("File.Replace(temporary, destination, null)", method)
        self.assertIn("File.Move(temporary, destination)", method)
        self.assertNotIn("File.WriteAllText(destination", method)
        self.assertIn("if (File.Exists(temporary)) File.Delete(temporary)", method)

    def test_executable_checks_cover_candidates_locked_files_and_selection(self) -> None:
        checks = (ROOT / "tests" / "ProfileWorkflowChecks.cs").read_text()
        for scenario in ("CheckCandidateLists(passed)", "CheckExportSnapshots(passed)", "CheckValidation(passed)", "CheckAtomicFiles(passed)", "CheckSelection(passed)", "CheckSavedProfileLoading(passed)", "CheckSavedProfileFailures(passed)", "CheckUnavailableSettings(passed)", "CheckEditorPersistenceRecovery(passed)", "CheckEditorFileOwnership(passed)"):
            self.assertIn(scenario, checks)
        self.assertIn("FileShare.None", checks)
        self.assertIn("Profile.ProfileList = previous", checks)


if __name__ == "__main__":
    unittest.main()
