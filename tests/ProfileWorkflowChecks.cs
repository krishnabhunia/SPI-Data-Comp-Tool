using System;
using System.Collections.Generic;
using System.IO;

namespace SPI_Data_Comp_Tool
{
    internal static class ProfileWorkflowChecks
    {
        public static void Run(Action<string> passed)
        {
            CheckCandidateLists(passed);
            CheckValidation(passed);
            CheckAtomicFiles(passed);
            CheckSelection(passed);
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
            passed("Invalid/duplicate profile imports are rejected before publication; an empty profile list is valid.");
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
    }
}
