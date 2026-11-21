using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    internal static class ProfileWorkflowChecks
    {
        private const string SyntheticManualPassword = "synthetic-manual-password";
        private const string SyntheticUnsavedPassword = "synthetic-unsaved-password";

        public static void Run(Action<string> passed)
        {
            CheckCandidateLists(passed);
            CheckExportSnapshots(passed);
            CheckValidation(passed);
            CheckAtomicFiles(passed);
            CheckSelection(passed);
            CheckSavedProfileLoading(passed);
            CheckSavedProfileFailures(passed);
            CheckUnavailableSettings(passed);
            CheckEditorPersistenceRecovery(passed);
            CheckEditorFileOwnership(passed);
        }

        private static DBC Example(string name, string server)
        {
            return new DBC
            {
                profileName = name,
                dbSRC = new DBConnectionDetailsProfile { str_DataSource = server, bool_Rb_SQL = true, str_Password = string.Empty },
                dbTRG = new DBConnectionDetailsProfile { str_DataSource = server + "-target", bool_Rb_SQL = true, str_Password = string.Empty },
            };
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void CheckCandidateLists(Action<string> passed)
        {
            DBC firstEntry = Example("First", "server-one");
            List<DBC> first = ProfileStore.Upsert(null, firstEntry);
            List<DBC> second = ProfileStore.Upsert(first, Example("Second", "server-two"));
            firstEntry.dbSRC.str_DataSource = "edited-outside-the-candidate";
            Check(first.Count == 1 && first[0].profileName == "First" && first[0].dbSRC.str_DataSource == "server-one",
                "Saving a later profile or editing an input object changed an earlier saved profile.");
            Check(second.Count == 2 && second[0].profileName == "First" && second[1].profileName == "Second",
                "A second profile replaced the first profile.");
            List<DBC> updated = ProfileStore.Upsert(second, Example("First", "updated-server"));
            Check(updated.Count == 2 && updated[0].dbSRC.str_DataSource == "updated-server" && second[0].dbSRC.str_DataSource == "server-one",
                "Updating a candidate changed the active list before a successful write.");
            List<DBC> deleted = ProfileStore.Remove(second, "Second");
            Check(deleted.Count == 1 && second.Count == 2, "Preparing deletion mutated the active list.");
            deleted[0].dbTRG.str_DataSource = "changed-target";
            Check(second[0].dbTRG.str_DataSource == "server-one-target", "Candidate connection details share mutable state.");
            passed("Profile save/update/delete candidates are independent snapshots; discarded candidates do not change active profiles.");
        }

        private static void CheckValidation(Action<string> passed)
        {
            ExpectInvalid(() => ProfileStore.Validate(null));
            ExpectInvalid(() => ProfileStore.Validate(new List<DBC> { Example("Repeated", "one"), Example("Repeated", "two") }));
            ExpectInvalid(() => ProfileStore.Validate(new List<DBC> { new DBC { profileName = "Incomplete", dbSRC = null } }));
            ExpectInvalid(() => ProfileStore.Validate(new List<DBC> { Example(" ", "one") }));
            ProfileStore.Validate(new List<DBC>());
            var tooMany = new List<DBC>();
            for (int i = 0; i <= 10000; i++) tooMany.Add(Example("Profile " + i, "synthetic-server"));
            ExpectInvalid(() => ProfileStore.Validate(tooMany));
            passed("Invalid/duplicate profile imports are rejected before publication; an empty profile list is valid.");
        }

        private static void CheckExportSnapshots(Action<string> passed)
        {
            // Synthetic markers only: no database credentials, user profile files or live services.
            string sourceMarker = "synthetic-source-" + Guid.NewGuid().ToString("N");
            string targetMarker = "synthetic-target-" + Guid.NewGuid().ToString("N");
            DBC legacy = Example("Imported legacy profile", "legacy-server");
            legacy.dbSRC.str_Password = sourceMarker;
            legacy.dbTRG.str_Password = targetMarker;
            legacy.dbSRC.str_DBName = "synthetic-database";
            legacy.dbSRC.str_UserName = "synthetic-user";
            legacy.dbSRC.str_TableName = "dbo.SyntheticTable";
            legacy.dbSRC.int_Oracle_DefaultPort = 1521;
            legacy.dbSRC.bool_SQL_WA = true;
            var imported = new List<DBC> { legacy };
            List<DBC> savingAnother = ProfileStore.Upsert(imported, Example("New profile", "new-server"));
            string saveJson = ProfileStore.SerializeForExport(savingAnother);
            List<DBC> exported = ProfileStore.ParseJson(saveJson);
            Check(exported.Count == 2 && !saveJson.Contains(sourceMarker) && !saveJson.Contains(targetMarker),
                "Saving another profile re-exported credentials from an imported legacy profile.");
            foreach (DBC profile in exported)
                Check(profile.dbSRC.str_Password == string.Empty && profile.dbTRG.str_Password == string.Empty,
                    "An exported Source or Target password was not blank.");
            Check(exported[0].profileName == legacy.profileName && exported[0].dbSRC.str_DataSource == "legacy-server"
                && exported[0].dbTRG.str_DataSource == "legacy-server-target"
                && exported[0].dbSRC.str_DBName == "synthetic-database"
                && exported[0].dbSRC.str_UserName == "synthetic-user"
                && exported[0].dbSRC.str_TableName == "dbo.SyntheticTable"
                && exported[0].dbSRC.int_Oracle_DefaultPort == 1521 && exported[0].dbSRC.bool_SQL_WA,
                "Sanitizing exported credentials changed noncredential connection details.");

            List<DBC> deletingAnother = ProfileStore.Remove(savingAnother, "New profile");
            string deleteJson = ProfileStore.SerializeForExport(deletingAnother);
            Check(ProfileStore.ParseJson(deleteJson).Count == 1 && !deleteJson.Contains(sourceMarker) && !deleteJson.Contains(targetMarker),
                "Deleting another profile re-exported credentials from a retained legacy profile.");
            Check(legacy.dbSRC.str_Password == sourceMarker && legacy.dbTRG.str_Password == targetMarker
                && savingAnother[0].dbSRC.str_Password == sourceMarker && savingAnother[0].dbTRG.str_Password == targetMarker
                && deletingAnother[0].dbSRC.str_Password == sourceMarker && deletingAnother[0].dbTRG.str_Password == targetMarker,
                "Export serialization modified active/imported profile objects.");
            List<DBC> snapshot = ProfileStore.CreateExportSnapshot(imported);
            Check(!ReferenceEquals(snapshot[0], legacy) && !ReferenceEquals(snapshot[0].dbSRC, legacy.dbSRC)
                && !ReferenceEquals(snapshot[0].dbTRG, legacy.dbTRG), "An export snapshot shares mutable connection objects.");
            passed("Save/delete exports clear every legacy Source/Target password on independent snapshots, preserving active objects and noncredential fields.");
        }

        private static void ExpectInvalid(Action action)
        {
            bool rejected = false;
            try { action(); }
            catch (InvalidDataException) { rejected = true; }
            Check(rejected, "An invalid profile list was accepted.");
        }

        private static void CheckAtomicFiles(Action<string> passed)
        {
            string directory = Path.Combine(Path.GetTempPath(), "spi-profile-check-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string path = Path.Combine(directory, "synthetic-profiles.json");
                const string original = "[{\"profileName\":\"Synthetic original\"}]";
                const string updated = "[{\"profileName\":\"Synthetic updated\"}]";
                ProfileStore.WriteAtomic(path, original);
                Check(File.ReadAllText(path) == original, "A new profile file was not written.");
                ProfileStore.WriteAtomic(path, updated);
                Check(File.ReadAllText(path) == updated, "An existing profile file was not atomically replaced.");

                bool rejected = false;
                using (var held = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    try { ProfileStore.WriteAtomic(path, original); }
                    catch (IOException) { rejected = true; }
                    catch (UnauthorizedAccessException) { rejected = true; }
                }
                Check(rejected, "Replacing a locked profile file should fail.");
                Check(File.ReadAllText(path) == updated, "A failed profile save changed the existing file.");
                Check(Directory.GetFiles(directory, ".spi-profile-*.tmp").Length == 0, "A failed save left a temporary profile file behind.");
                passed("Profile creation/replacement succeeds; failed writes preserve the existing file and clean their temporary file.");
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }

        private static void CheckSelection(Action<string> passed)
        {
            List<DBC> previous = Profile.ProfileList;
            try
            {
                using (var panel = new ConnectionPanel())
                {
                    Exception selectionError = null;
                    panel.Failed += (sender, error) => selectionError = error;
                    panel.AllowNewProfileName = true;
                    Profile.ProfileList = new List<DBC> { Example("First", "server-one"), Example("Second", "server-two") };
                    panel.RefreshProfiles("First");
                    Check(panel.Source.DataSource.Text == "server-one", "The selected profile did not load.");
                    panel.Source.DataSource.Text = "unsaved-entry";
                    panel.ProfileList.SelectedIndex = -1;
                    panel.ProfileList.Text = "New profile name";
                    panel.ApplySelectedProfile();
                    Check(panel.Source.DataSource.Text == "unsaved-entry", "Typing a new name cleared the entered connection.");
                    Profile.ProfileList.Add(Example("New saved profile", "new-server"));
                    panel.RefreshAvailableProfiles();
                    Check(panel.ProfileList.Items.Count == 3 && panel.ProfileList.Text == "New profile name"
                        && panel.Source.DataSource.Text == "unsaved-entry",
                        "Opening the profile picker lost a typed name or connection, or did not show a newly saved profile.");
                    panel.RefreshProfiles("First", false);
                    Check(panel.Source.DataSource.Text == "unsaved-entry", "Refreshing after save reapplied a password-free profile.");
                    passed("Typing a new profile name and refreshing after save preserve the entered connection fields.");

                    Profile.ProfileList = new List<DBC> { Example("Second", "server-two"), Example("First", "correct-server") };
                    panel.ApplySelectedProfile();
                    Check(panel.Source.DataSource.Text == "correct-server", "A stale picker index loaded the wrong profile.");
                    panel.Source.DataSource.Text = "unsaved-after-reorder";
                    panel.RefreshAvailableProfiles();
                    Check(panel.ProfileList.SelectedIndex == 1 && panel.Source.DataSource.Text == "unsaved-after-reorder",
                        "Dropdown refresh did not retain the exact selected name across a catalog reorder without overwriting edits.");
                    Profile.ProfileList = new List<DBC> { Example("Second", "server-two") };
                    panel.Source.DataSource.Text = "preserve-on-removed-profile";
                    panel.ApplySelectedProfile();
                    Check(panel.Source.DataSource.Text == "preserve-on-removed-profile" && selectionError != null
                        && panel.ProfileList.SelectedIndex == -1,
                        "A removed profile erased the fields or silently kept an unavailable saved name selected.");
                    panel.RefreshProfiles("Second");
                    panel.Source.DataSource.Text = "preserve-on-catalog-clear";
                    Profile.ProfileList = new List<DBC>();
                    panel.RefreshAvailableProfiles();
                    Check(panel.ProfileList.Items.Count == 0 && panel.ProfileList.SelectedIndex == -1
                        && panel.Source.DataSource.Text == "preserve-on-catalog-clear",
                        "An empty catalog did not clear stale choices or erased manual connection fields.");
                    Profile.ProfileList = new List<DBC> { Example("name", "lower"), Example("NAME", "upper") };
                    panel.RefreshProfiles("NAME");
                    Check(panel.ProfileList.SelectedIndex == 1 && panel.Source.DataSource.Text == "upper", "Profile refresh ignored name case and selected another profile.");
                    passed("Dropdown refresh sees new profiles and retains edits; stale selections report errors without clearing fields; name casing remains exact.");
                }
            }
            finally
            {
                Profile.ProfileList = previous;
            }
        }

        private static void CheckSavedProfileLoading(Action<string> passed)
        {
            List<DBC> previous = Profile.ProfileList;
            string previousPath = Properties.Settings.Default.ProfilePathUser;
            string directory = Path.Combine(Path.GetTempPath(), "spi-profile-load-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                // This is the shape used by the repository's example and older saved files:
                // str_DBType identifies the provider without the later Boolean radio fields.
                const string legacyJson = "[{\"profileName\":\"Example Oracle to SQL\","
                    + "\"dbSRC\":{\"str_DBType\":\"oracle\",\"str_DataSource\":\"synthetic-oracle\",\"str_DBName\":\"service\",\"str_UserName\":\"example-user\",\"str_Password\":\"\"},"
                    + "\"dbTRG\":{\"str_DBType\":\"sql\",\"str_DataSource\":\"synthetic-sql\",\"str_DBName\":\"database\",\"str_UserName\":\"example-user\",\"str_Password\":\"\"}}]";
                string path = Path.Combine(directory, "saved-profiles.JSON");
                File.WriteAllText(path, legacyJson, new UTF8Encoding(true));
                byte[] originalBytes = File.ReadAllBytes(path);
                Properties.Settings.Default.ProfilePathUser = path; // In memory only; never save test settings.
                Profile.ProfileList = new List<DBC> { Example("Previous", "previous-server") };
                using (var panel = new ConnectionPanel())
                {
                    panel.AllowNewProfileName = true;
                    panel.ProfileList.Text = "Unsaved name";
                    panel.Source.DataSource.Text = "manual-server";
                    panel.Source.Password.Text = SyntheticManualPassword;
                    int applied = 0;
                    int changed = 0;
                    panel.ProfileApplied += (sender, details) => applied++;
                    panel.InputChanged += (sender, e) => changed++;

                    ProfileLoadResult loaded = panel.LoadSavedProfiles();
                    Check(loaded.Loaded && !loaded.HasError && loaded.FilePath == path,
                        "A valid remembered uppercase .JSON file was not loaded through the startup path.");
                    Check(Profile.ProfileList.Count == 1 && Profile.ProfileList[0].dbSRC.bool_Rb_Oracle
                        && !Profile.ProfileList[0].dbSRC.bool_Rb_SQL && Profile.ProfileList[0].dbTRG.bool_Rb_SQL
                        && !Profile.ProfileList[0].dbTRG.bool_Rb_Oracle,
                        "Legacy provider names did not restore both platform selections.");
                    Check(panel.ProfileList.Items.Count == 1 && panel.ProfileList.SelectedIndex == -1
                        && panel.ProfileList.Text == "Unsaved name" && panel.Source.DataSource.Text == "manual-server"
                        && panel.Source.Password.Text == SyntheticManualPassword && applied == 0 && changed == 0,
                        "Loading a profile catalog applied an early selection or changed manually entered fields.");
                    panel.ProfileList.SelectedIndex = 0;
                    Check(applied == 1 && panel.Source.Oracle.Checked && panel.Target.Sql.Checked
                        && panel.Source.Port.Text == "1521" && panel.Source.DataSource.Text == "synthetic-oracle"
                        && panel.Target.DataSource.Text == "synthetic-sql" && panel.Source.Password.Text == string.Empty,
                        "Selecting a legacy profile did not apply its Source/Target provider and fields.");
                    Check(Properties.Settings.Default.ProfilePathUser == path
                        && Convert.ToBase64String(File.ReadAllBytes(path)) == Convert.ToBase64String(originalBytes),
                        "Reading/normalizing a saved profile changed its file or remembered setting.");

                    string explicitPath = Path.Combine(directory, "explicit-platforms.json");
                    var explicitProfile = Example("Explicit SQL", "explicit-server");
                    explicitProfile.dbSRC.bool_SQL_WA = true;
                    ProfileStore.WriteAtomic(explicitPath, ProfileStore.SerializeForExport(new[] { explicitProfile }));
                    ProfileLoadResult explicitLoad = panel.LoadSavedProfiles(explicitPath);
                    Check(explicitLoad.Loaded && Profile.ProfileList[0].dbSRC.bool_Rb_SQL
                        && Profile.ProfileList[0].dbSRC.bool_SQL_WA && Properties.Settings.Default.ProfilePathUser == path,
                        "Explicit provider/authentication choices were changed or an optional load rewrote the saved path.");
                }
                passed("The actual saved-path loader reads BOM/uppercase JSON and legacy provider names, preserves fields until selection, and never rewrites profile files or settings.");
            }
            finally
            {
                Profile.ProfileList = previous;
                Properties.Settings.Default.ProfilePathUser = previousPath;
                Directory.Delete(directory, true);
            }
        }

        private static void CheckSavedProfileFailures(Action<string> passed)
        {
            List<DBC> previous = Profile.ProfileList;
            string directory = Path.Combine(Path.GetTempPath(), "spi-profile-recovery-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                var active = new List<DBC> { Example("Keep active", "active-server") };
                Profile.ProfileList = active;
                using (var panel = new ConnectionPanel())
                {
                    Exception selectionError = null;
                    panel.Failed += (sender, error) => selectionError = error;
                    panel.Source.DataSource.Text = "keep-manual-server";
                    panel.Source.Password.Text = SyntheticUnsavedPassword;
                    ProfileLoadResult absent = panel.LoadSavedProfiles((string)null);
                    Check(!absent.Loaded && !absent.HasError && string.IsNullOrEmpty(absent.Message)
                        && ReferenceEquals(Profile.ProfileList, active), "An unset saved path was treated as an error or erased active profiles.");

                    string path = Path.Combine(directory, "broken.json");
                    foreach (string json in new[]
                    {
                        "", "{", "null", "[null]", "{}",
                        "[{\"profileName\":\"Missing sides\"}]",
                        "[{\"profileName\":\"Null source\",\"dbSRC\":null,\"dbTRG\":{}}]",
                        "[{\"profileName\":\"Null target\",\"dbSRC\":{},\"dbTRG\":null}]",
                        "[{\"profileName\":\"Valid first\",\"dbSRC\":{},\"dbTRG\":{}},null]",
                        "[{\"profileName\":\"Conflicting provider\",\"dbSRC\":{\"str_DBType\":\"oracle\",\"bool_Rb_SQL\":true},\"dbTRG\":{}}]",
                        "[{\"profileName\":\"Invalid authentication\",\"dbSRC\":{\"str_DBType\":\"oracle\",\"bool_SQL_WA\":true},\"dbTRG\":{}}]",
                        "[{\"profileName\":\"Overflow port\",\"dbSRC\":{},\"dbTRG\":{\"int_Oracle_DefaultPort\":99999999999999999999}}]",
                    })
                    {
                        File.WriteAllText(path, json);
                        CheckLoadFailure(panel, path, active);
                        Check(File.ReadAllText(path) == json, "A rejected profile document was overwritten during recovery.");
                    }
                    CheckLoadFailure(panel, Path.Combine(directory, "missing.json"), active);
                    CheckLoadFailure(panel, "\0invalid.json", active);
                    string wrongExtension = Path.Combine(directory, "profiles.txt");
                    File.WriteAllText(wrongExtension, "[]");
                    CheckLoadFailure(panel, wrongExtension, active);
                    File.WriteAllText(path, ProfileStore.SerializeForExport(active));
                    using (var held = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
                        CheckLoadFailure(panel, path, active);
                    File.WriteAllBytes(path, new byte[] { 0xff, 0x7b });
                    CheckLoadFailure(panel, path, active);
                    Check(File.ReadAllBytes(path)[0] == 0xff, "Invalid text encoding was repaired by overwriting the profile file.");
                    using (var large = new FileStream(path, FileMode.Create, FileAccess.Write))
                        large.SetLength(16 * 1024 * 1024 + 1);
                    CheckLoadFailure(panel, path, active);
                    Check(new FileInfo(path).Length == 16 * 1024 * 1024 + 1, "An oversized profile was changed during loading.");

                    Profile.ProfileList = null;
                    ProfileLoadResult noCatalog = panel.LoadSavedProfiles(Path.Combine(directory, "missing.json"));
                    Check(noCatalog.HasError && Profile.ProfileList == null && panel.ProfileList.Items.Count == 0,
                        "Missing saved profiles did not leave a null catalog usable for manual entry.");
                    Profile.ProfileList = new List<DBC> { null, new DBC { profileName = "Broken entry", dbSRC = null } };
                    Common_Data.GetUserProfile(panel.ProfileList);
                    Check(panel.ProfileList.Items.Count == 1 && panel.ProfileList.SelectedIndex == -1,
                        "Refreshing a legacy catalog with a null entry failed or automatically selected an invalid profile.");
                    panel.ProfileList.SelectedIndex = 0;
                    Check(selectionError is InvalidDataException && panel.Source.DataSource.Text == "keep-manual-server"
                        && panel.Source.Password.Text == SyntheticUnsavedPassword,
                        "Selecting a malformed in-memory profile partly changed the entered connection or did not report its error.");
                    Check(Common_Data.GetUserProfile(null) == null, "A missing legacy profile picker was not handled safely.");
                    using (var standalone = new ComboBox())
                    {
                        Common_Data.GetUserProfile(standalone);
                        Check(standalone.Items.Count == 1, "A legacy picker failed on a null catalog entry.");
                        Profile.ProfileList = null;
                        Common_Data.GetUserProfile(standalone);
                        Check(standalone.Items.Count == 0, "A null catalog left stale legacy choices visible.");
                    }

                    File.WriteAllText(path, ProfileStore.SerializeForExport(active));
                    ProfileLoadResult recovered = panel.LoadSavedProfiles(path);
                    Check(recovered.Loaded && !recovered.HasError && panel.ProfileList.Items.Count == 1
                        && panel.Source.DataSource.Text == "keep-manual-server",
                        "A valid file did not recover the same panel after earlier profile-loading failures.");
                    File.WriteAllText(path, "[]");
                    ProfileLoadResult empty = panel.LoadSavedProfiles(path);
                    Check(empty.Loaded && Profile.ProfileList.Count == 0 && panel.ProfileList.Items.Count == 0
                        && panel.Source.DataSource.Text == "keep-manual-server", "An empty valid catalog erased manual connection fields.");
                }
                passed("Missing/locked/invalid/oversized profiles and null catalog entries remain recoverable, preserve active data and manual fields, and a later valid file loads successfully.");
            }
            finally
            {
                Profile.ProfileList = previous;
                Directory.Delete(directory, true);
            }
        }

        private static void CheckLoadFailure(ConnectionPanel panel, string path, List<DBC> active)
        {
            string rememberedPath = Properties.Settings.Default.ProfilePathUser;
            ProfileLoadResult failed = panel.LoadSavedProfiles(path);
            Check(failed.HasError && !failed.Loaded && ReferenceEquals(Profile.ProfileList, active)
                && panel.ProfileList.Items.Count == active.Count
                && panel.Source.DataSource.Text == "keep-manual-server"
                && panel.Source.Password.Text == SyntheticUnsavedPassword
                && failed.Message.Contains("Settings > Connection profiles > Load profile")
                && Properties.Settings.Default.ProfilePathUser == rememberedPath,
                "An unavailable profile file aborted loading, lost the active catalog/fields, changed settings, or omitted its recovery path.");
        }

        private static void CheckUnavailableSettings(Action<string> passed)
        {
            List<DBC> previous = Profile.ProfileList;
            try
            {
                var active = new List<DBC> { Example("Keep active", "active-server") };
                Profile.ProfileList = active;
                ProfileLoadResult failed = ProfileStore.LoadRememberedProfiles(() =>
                {
                    throw new ConfigurationErrorsException("Synthetic invalid user.config; no real settings file was accessed.");
                });
                Check(failed.HasError && !failed.Loaded && failed.FilePath == null
                    && ReferenceEquals(Profile.ProfileList, active)
                    && failed.Message.Contains("settings file has not been changed"),
                    "An unreadable saved setting escaped the optional profile-load boundary or changed the active catalog.");
                passed("An unreadable remembered setting reports a recovery warning without aborting startup or changing active profiles/settings files.");
            }
            finally { Profile.ProfileList = previous; }
        }

        private static void CheckEditorPersistenceRecovery(Action<string> passed)
        {
            List<DBC> previous = Profile.ProfileList;
            string previousPath = Properties.Settings.Default.ProfilePathUser;
            string previousReminder = ProfileStore.RememberedPathFileOverride;
            string directory = Path.Combine(Path.GetTempPath(), "spi-profile-editor-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string oldPath = Path.Combine(directory, "old.json");
                string importedPath = Path.Combine(directory, "imported.JSON");
                string savedPath = Path.Combine(directory, "saved.json");
                string oldJson = ProfileStore.SerializeForExport(new[] { Example("Old", "old-server") });
                string importedJson = ProfileStore.SerializeForExport(new[] { Example("Imported", "imported-server") });
                File.WriteAllText(oldPath, oldJson);
                File.WriteAllText(importedPath, importedJson);
                ProfileStore.RememberedPathFileOverride = Path.Combine(directory, "profile-path.txt");
                Properties.Settings.Default.ProfilePathUser = oldPath;
                Profile.ProfileList = null;
                Action<string> unavailableSettings = path =>
                {
                    throw new ConfigurationErrorsException("Synthetic settings persistence failure; no user file was touched.");
                };
                using (var editor = new UserProfileForm())
                {
                    var panel = (ConnectionPanel)editor.Controls.Find("connection", true)[0];
                    ProfileLoadResult imported = editor.ImportProfileFile(importedPath, unavailableSettings);
                    Check(imported.Loaded && imported.HasError && imported.Message.Contains("usable for this session")
                        && ProfileStore.ActiveCatalog.FilePath == importedPath
                        && panel.Source.DataSource.Text == "imported-server",
                        "A valid manual import was blocked or misreported when remembering its path failed.");
                    Check(Properties.Settings.Default.ProfilePathUser == oldPath && File.ReadAllText(oldPath) == oldJson
                        && File.ReadAllText(importedPath) == importedJson,
                        "Manual import changed settings or rewrote a source profile file.");
                    ProfileCatalog active = ProfileStore.ActiveCatalog;
                    using (var laterWindow = new ConnectionPanel())
                    {
                        ProfileLoadResult reopened = laterWindow.LoadSavedProfiles();
                        Check(reopened.Loaded && ReferenceEquals(ProfileStore.ActiveCatalog, active)
                            && (string)laterWindow.ProfileList.Items[0] == "Imported",
                            "Opening another task reloaded the stale setting and replaced a valid session-only import.");
                    }

                    // Delete must follow the successfully imported file, not the old setting.
                    editor.DeleteProfileFile("Imported");
                    Check(ProfileStore.ReadFile(importedPath).Count == 0 && File.ReadAllText(oldPath) == oldJson,
                        "Deleting an imported profile targeted the old remembered file.");
                    File.WriteAllText(importedPath, importedJson);
                    editor.ImportProfileFile(importedPath, unavailableSettings);
                    panel.Source.Password.Text = SyntheticManualPassword;
                    var candidate = ProfileStore.Upsert(Profile.ProfileList, Example("New", "new-server"));
                    candidate[0].dbSRC.str_Password = SyntheticUnsavedPassword;
                    ProfileLoadResult saved = editor.SaveProfileFile(savedPath, candidate, "New", unavailableSettings);
                    Check(saved.Loaded && saved.HasError && ProfileStore.ActiveCatalog.FilePath == savedPath
                        && ReferenceEquals(Profile.ProfileList, candidate)
                        && panel.Source.Password.Text == SyntheticManualPassword
                        && !File.ReadAllText(savedPath).Contains(SyntheticUnsavedPassword),
                        "A successful file save became unusable after a settings failure, changed entered credentials, or exported a password.");
                    Check(Properties.Settings.Default.ProfilePathUser == oldPath && File.ReadAllText(oldPath) == oldJson,
                        "A file save with unavailable settings replaced the old remembered catalog.");

                    string savedJson = File.ReadAllText(savedPath);
                    active = ProfileStore.ActiveCatalog;
                    bool rejected = false;
                    using (var held = new FileStream(savedPath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        try { editor.SaveProfileFile(savedPath, ProfileStore.Remove(candidate, "New"), "Imported", unavailableSettings); }
                        catch (IOException) { rejected = true; }
                        catch (UnauthorizedAccessException) { rejected = true; }
                    }
                    Check(rejected && ReferenceEquals(ProfileStore.ActiveCatalog, active)
                        && ReferenceEquals(Profile.ProfileList, candidate) && File.ReadAllText(savedPath) == savedJson,
                        "A failed file save changed the active catalog/file provenance or existing file.");
                }

                // A new copy has no per-copy setting; its editor must delete from the actual
                // shared fallback file that loaded, without pretending a setting was saved.
                File.WriteAllText(importedPath, importedJson);
                ProfileStore.RememberPath(importedPath);
                Properties.Settings.Default.ProfilePathUser = string.Empty;
                Profile.ProfileList = null;
                using (var editor = new UserProfileForm())
                {
                    Check(ProfileStore.ActiveCatalog.FilePath == importedPath, "The editor did not retain its fallback file provenance.");
                    editor.DeleteProfileFile("Imported");
                    Check(ProfileStore.ReadFile(importedPath).Count == 0
                        && Properties.Settings.Default.ProfilePathUser == string.Empty && File.ReadAllText(oldPath) == oldJson,
                        "A shared-fallback profile could not be deleted from its real file without changing settings.");
                }
                passed("Actual editor import/save paths remain usable after settings failures; later tasks retain the session catalog and deletion uses the loaded file, including shared-path fallback.");
            }
            finally
            {
                Profile.ProfileList = previous;
                Properties.Settings.Default.ProfilePathUser = previousPath;
                ProfileStore.RememberedPathFileOverride = previousReminder;
                Directory.Delete(directory, true);
            }
        }

        private static void CheckEditorFileOwnership(Action<string> passed)
        {
            List<DBC> previous = Profile.ProfileList;
            string previousPath = Properties.Settings.Default.ProfilePathUser;
            string previousReminder = ProfileStore.RememberedPathFileOverride;
            string directory = Path.Combine(Path.GetTempPath(), "spi-profile-owner-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string first = Path.Combine(directory, "first.json"), second = Path.Combine(directory, "second.json");
                string original = ProfileStore.SerializeForExport(new[] { Example("Same name", "same-server") });
                File.WriteAllText(first, original);
                File.WriteAllText(second, original); // Identical content must not make these the same owned file.
                ProfileStore.RememberedPathFileOverride = Path.Combine(directory, "profile-path.txt");
                Properties.Settings.Default.ProfilePathUser = first;
                Profile.ProfileList = null;
                using (var editor = new UserProfileForm())
                using (var otherWindow = new ConnectionPanel())
                {
                    otherWindow.LoadSavedProfiles(second);
                    ExpectStaleProfile(() => editor.DeleteProfileFile("Same name"));
                    ExpectStaleProfile(() => editor.SaveProfileFile(first,
                        ProfileStore.Upsert(Profile.ProfileList, Example("New", "new-server")), "New", path => { }));
                    Check(File.ReadAllText(first) == original && File.ReadAllText(second) == original,
                        "A stale editor changed one of two different files with identical profiles.");

                    editor.ImportProfileFile(first, path => { });
                    ProfileCatalog loaded = ProfileStore.ActiveCatalog;
                    string externalEdit = ProfileStore.SerializeForExport(new[] { Example("External edit", "external-server") });
                    File.WriteAllText(first, externalEdit);
                    ExpectStaleProfile(() => editor.DeleteProfileFile("Same name"));
                    ExpectStaleProfile(() => editor.SaveProfileFile(first,
                        ProfileStore.Upsert(Profile.ProfileList, Example("New", "new-server")), "New", path => { }));
                    Check(ReferenceEquals(ProfileStore.ActiveCatalog, loaded) && File.ReadAllText(first) == externalEdit,
                        "An edit after loading was overwritten or failure changed catalog ownership.");

                    File.WriteAllText(first, original);
                    editor.ImportProfileFile(first, path => { });
                    var beforeDialog = ProfileStore.Upsert(Profile.ProfileList, Example("New", "new-server"));
                    ExpectStaleProfile(() => editor.SaveSelectedProfile(beforeDialog, "New", () =>
                    {
                        editor.ImportProfileFile(second, path => { });
                        return first;
                    }, path => { }));
                    Check(File.ReadAllText(first) == original && File.ReadAllText(second) == original
                        && ProfileStore.ActiveCatalog.FilePath == second && Profile.ProfileList.Count == 1,
                        "A same-editor import during Save selection authorized a stale candidate or changed either file.");

                    editor.ImportProfileFile(first, path => { });
                    var picker = ((ConnectionPanel)editor.Controls.Find("connection", true)[0]).ProfileList;
                    picker.SelectedIndex = -1;
                    picker.SelectedIndex = 0;
                    Check(!editor.DeleteSelectedProfile(name =>
                    {
                        editor.ImportProfileFile(second, path => { });
                        return true;
                    }) && File.ReadAllText(first) == original && File.ReadAllText(second) == original
                        && ProfileStore.ActiveCatalog.FilePath == second && Profile.ProfileList.Count == 1,
                        "A same-editor import redirected confirmed deletion to an unconfirmed catalog.");

                    Profile.ProfileList = new List<DBC>(Profile.ProfileList);
                    Check(ProfileStore.ActiveCatalog == null,
                        "Direct catalog replacement retained an unproven delete destination.");
                    ExpectStaleProfile(() => editor.DeleteProfileFile("Same name"));
                    Check(File.ReadAllText(first) == original && File.ReadAllText(second) == original,
                        "An unproven catalog replacement allowed deletion from an older active file.");
                }
                passed("Stale editors and same-editor dialog changes cannot redirect save/delete; external edits are preserved and direct list replacement invalidates file provenance.");
            }
            finally
            {
                Profile.ProfileList = previous;
                Properties.Settings.Default.ProfilePathUser = previousPath;
                ProfileStore.RememberedPathFileOverride = previousReminder;
                Directory.Delete(directory, true);
            }
        }

        private static void ExpectStaleProfile(Action action)
        {
            bool rejected = false;
            try { action(); }
            catch (InvalidOperationException) { rejected = true; }
            Check(rejected, "A stale profile-file operation was accepted.");
        }
    }
}
