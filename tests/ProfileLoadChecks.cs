using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// A new copy of the app finds the last profiles file chosen on this computer, and the step strip says
    /// what the current step still needs. Uses temporary files only; the user's settings and AppData are never read.
    /// </summary>
    internal static class ProfileLoadChecks
    {
        private const string Legacy = "[" +
            "{\"profileName\":\"legacy one\",\"dbSRC\":{\"str_DBType\":\"oracle\",\"str_DataSource\":\"ora-host\",\"str_DBName\":\"ORA\",\"str_UserName\":\"u1\",\"str_Password\":\"\"}," +
            "\"dbTRG\":{\"str_DBType\":\"sql\",\"str_DataSource\":\"sql-host\",\"str_DBName\":\"SQLDB\",\"str_UserName\":\"sa\",\"str_Password\":\"\"}}]";

        public static void Run(Action<string> passed)
        {
            string folder = Path.Combine(Path.GetTempPath(), "spi-profile-load-" + Guid.NewGuid().ToString("N"));
            List<DBC> previous = Profile.ProfileList;
            string previousRemembered = ProfileStore.RememberedPathFileOverride;
            string previousSetting = Properties.Settings.Default.ProfilePathUser;
            Directory.CreateDirectory(folder);
            try
            {
                ProfileStore.RememberedPathFileOverride = Path.Combine(folder, "profile-path.txt");
                string saved = Path.Combine(folder, "profiles.json");
                File.WriteAllText(saved, Legacy);
                CheckRememberedPath(passed, saved, Path.Combine(folder, "moved-away.json"));
                CheckDeletionDestination(passed, folder);
                CheckFailedLoadDeletion(passed, folder);
                CheckDeletionChangesAndReadErrors(passed, folder);
                CheckStepReason(passed);
            }
            finally
            {
                Profile.ProfileList = previous;
                ProfileStore.RememberedPathFileOverride = previousRemembered;
                Properties.Settings.Default.ProfilePathUser = previousSetting;
                try { Directory.Delete(folder, true); } catch (Exception) { }
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void CheckRememberedPath(Action<string> passed, string saved, string missing)
        {
            Profile.ProfileList = null;
            ProfileLoadResult nothing = ProfileStore.LoadRememberedProfiles(() => string.Empty);
            Require(!nothing.Loaded && !nothing.HasError, "Without any saved or remembered file nothing must be loaded or reported as an error.");

            ProfileLoadResult first = ProfileStore.LoadRememberedProfiles(() => saved);
            Require(first.Loaded && Profile.ProfileList != null && Profile.ProfileList.Count == 1, "The saved profiles file must load: " + first.Message);
            Require(string.Equals(ProfileStore.ReadRememberedPath(), Path.GetFullPath(saved), StringComparison.OrdinalIgnoreCase),
                "A loaded profiles file must be remembered for a new copy of the app.");

            Profile.ProfileList = null;
            ProfileLoadResult newCopy = ProfileStore.LoadRememberedProfiles(() => string.Empty);
            Require(newCopy.Loaded && !newCopy.HasError && Profile.ProfileList != null && Profile.ProfileList.Count == 1 &&
                newCopy.Message.Contains("last one chosen"),
                "A new copy of the app (empty setting) must load the remembered profiles file: " + newCopy.Message);
            Require(Profile.ProfileList[0].dbSRC.bool_Rb_Oracle && Profile.ProfileList[0].dbTRG.bool_Rb_SQL,
                "The remembered early-format file must still select Oracle and SQL Server.");

            Profile.ProfileList = null;
            ProfileLoadResult moved = ProfileStore.LoadRememberedProfiles(() => missing);
            Require(moved.Loaded && string.Equals(moved.FilePath, Path.GetFullPath(saved), StringComparison.OrdinalIgnoreCase) &&
                moved.Message.Contains("could not be found"),
                "A copy whose own saved file is missing must say so and use the remembered file: " + moved.Message);

            File.WriteAllText(saved, "{ invalid json");
            Profile.ProfileList = null;
            ProfileLoadResult malformed = ProfileStore.LoadRememberedProfiles(() => string.Empty);
            Require(!malformed.Loaded && malformed.HasError && malformed.FilePath == saved
                && malformed.Message.Contains("Load profile"),
                "A malformed remembered file was silently ignored by a new copy: " + malformed.Message);
            File.WriteAllText(saved, Legacy);
            using (var held = new FileStream(saved, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                Profile.ProfileList = null;
                ProfileLoadResult locked = ProfileStore.LoadRememberedProfiles(() => string.Empty);
                Require(!locked.Loaded && locked.HasError && locked.FilePath == saved,
                    "A locked remembered file was silently ignored by a new copy: " + locked.Message);
            }

            File.Delete(saved);
            Profile.ProfileList = null;
            ProfileLoadResult missingReminder = ProfileStore.LoadRememberedProfiles(() => string.Empty);
            Require(!missingReminder.Loaded && missingReminder.HasError,
                "A missing remembered file was silently ignored by a new copy: " + missingReminder.Message);
            Profile.ProfileList = null;
            ProfileLoadResult gone = ProfileStore.LoadRememberedProfiles(() => missing);
            Require(!gone.Loaded && gone.HasError && Profile.ProfileList == null,
                "When neither file exists the normal recovery message must be shown: " + gone.Message);
            passed("A new copy finds the shared profiles file; missing, malformed or locked shared files report recovery instead of disappearing silently");
        }

        private static string TwoProfiles()
        {
            List<DBC> profiles = ProfileStore.ParseJson(Legacy);
            DBC second = ProfileStore.ParseJson(Legacy)[0];
            second.profileName = "legacy two";
            profiles.Add(second);
            return ProfileStore.SerializeForExport(profiles);
        }

        private static void SelectFirstProfile(UserProfileForm form)
        {
            var panel = (ConnectionPanel)form.Controls.Find("connection", true)[0];
            Require(panel.ProfileList.Items.Count > 0, "The fixture profile picker is empty.");
            panel.ProfileList.SelectedIndex = -1;
            panel.ProfileList.SelectedIndex = 0;
        }

        private static void CheckDeletionDestination(Action<string> passed, string folder)
        {
            string saved = Path.Combine(folder, "remembered-catalog.json");
            string other = Path.Combine(folder, "unrelated-catalog.json");
            string original = TwoProfiles();
            File.WriteAllText(saved, original);
            File.WriteAllText(other, original);
            ProfileStore.RememberPath(saved);
            Properties.Settings.Default.ProfilePathUser = string.Empty;
            using (var form = new UserProfileForm())
            using (var staleEditor = new UserProfileForm())
            {
                SelectFirstProfile(form);
                SelectFirstProfile(staleEditor);
                List<DBC> active = Profile.ProfileList;
                Require(!form.DeleteSelectedProfile(name => false) && File.ReadAllText(saved) == original &&
                    ReferenceEquals(Profile.ProfileList, active), "Cancelling a fallback-profile deletion changed the file or active profiles.");
                bool confirmed = false;
                Require(form.DeleteSelectedProfile(name => { confirmed = name == "legacy one"; return true; }) && confirmed,
                    "A profile loaded from the remembered file could not be deleted when this copy has an empty setting.");
                Require(ProfileStore.ReadFile(saved).Count == 1 && ProfileStore.ReadFile(saved)[0].profileName == "legacy two" &&
                    Profile.ProfileList.Count == 1 && File.ReadAllText(other) == original &&
                    Properties.Settings.Default.ProfilePathUser == string.Empty,
                    "Fallback deletion used another file, changed settings, or failed to publish the saved result.");
                bool stalePrompted = false;
                Require(!staleEditor.DeleteSelectedProfile(name => { stalePrompted = true; return true; }) && !stalePrompted &&
                    ProfileStore.ReadFile(saved).Count == 1 && Profile.ProfileList.Count == 1,
                    "A second editor did not become stale after confirmed deletion published a new catalog.");
                using (var nextWindow = new UserProfileForm())
                {
                    var picker = ((ConnectionPanel)nextWindow.Controls.Find("connection", true)[0]).ProfileList;
                    Require(picker.Items.Count == 1 && picker.Items[0].ToString() == "legacy two" &&
                        ProfileStore.ActiveCatalog != null && ProfileStore.ActiveCatalog.FilePath == Path.GetFullPath(saved),
                        "A later window lost the catalog published by confirmed fallback deletion.");
                }
            }

            File.WriteAllText(saved, original);
            ProfileStore.RememberPath(saved);
            // Simulate a new application session: later windows in the same session now
            // intentionally retain the catalog successfully published by the first window.
            Profile.ProfileList = null;
            Properties.Settings.Default.ProfilePathUser = Path.Combine(folder, "moved-catalog.json");
            using (var form = new UserProfileForm())
            {
                SelectFirstProfile(form);
                // Another app copy can change both hints. The already displayed file remains the destination.
                Properties.Settings.Default.ProfilePathUser = other;
                ProfileStore.RememberPath(other);
                Require(form.DeleteSelectedProfile(name => true), "A fallback-loaded profile could not be deleted after its settings changed.");
                Require(ProfileStore.ReadFile(saved).Count == 1 && File.ReadAllText(other) == original &&
                    Properties.Settings.Default.ProfilePathUser == other,
                    "Deletion followed an unrelated setting/remembered hint instead of the file shown by this form.");
            }
            passed("Fallback-loaded profiles delete from the displayed file; cancellation, empty/stale settings and later path changes cannot redirect deletion");
        }

        private static void CheckFailedLoadDeletion(Action<string> passed, string folder)
        {
            string original = TwoProfiles();
            string invalid = Path.Combine(folder, "invalid-catalog.json");
            string locked = Path.Combine(folder, "locked-catalog.json");
            string untouched = Path.Combine(folder, "untouched-catalog.json");
            File.WriteAllText(invalid, "{ damaged json");
            File.WriteAllText(locked, original);
            File.WriteAllText(untouched, original);
            File.Delete(ProfileStore.RememberedPathFileOverride);
            foreach (string path in new[] { Path.Combine(folder, "absent-catalog.json"), invalid, locked })
            {
                var active = ProfileStore.ParseJson(original);
                Profile.ProfileList = active;
                Properties.Settings.Default.ProfilePathUser = path;
                using (FileStream held = path == locked
                    ? new FileStream(locked, FileMode.Open, FileAccess.Read, FileShare.None) : null)
                using (var form = new UserProfileForm())
                {
                    SelectFirstProfile(form);
                    // Even a later valid setting must not give a failed load a deletion destination.
                    Properties.Settings.Default.ProfilePathUser = untouched;
                    bool prompted = false;
                    Require(!form.DeleteSelectedProfile(name => { prompted = true; return true; }) && !prompted &&
                        ReferenceEquals(Profile.ProfileList, active) && File.ReadAllText(untouched) == original,
                        "A missing, invalid or unreadable profile load retained an unsafe deletion destination.");
                }
            }
            Require(File.ReadAllText(invalid) == "{ damaged json" && File.ReadAllText(locked) == original,
                "Failed profile loading or deletion altered an input file.");
            passed("Missing, damaged and locked profile loads preserve the active catalog and never acquire an unrelated deletion destination");
        }

        private static void CheckDeletionChangesAndReadErrors(Action<string> passed, string folder)
        {
            string saved = Path.Combine(folder, "concurrent-catalog.json");
            string original = TwoProfiles();
            string changed = original.Replace("ora-host", "changed-host");
            File.WriteAllText(saved, original);
            Properties.Settings.Default.ProfilePathUser = saved;
            using (var form = new UserProfileForm())
            {
                SelectFirstProfile(form);
                List<DBC> active = Profile.ProfileList;
                Require(!form.DeleteSelectedProfile(name => { File.WriteAllText(saved, changed); return true; }) &&
                    ReferenceEquals(Profile.ProfileList, active) && File.ReadAllText(saved) == changed,
                    "Deletion overwrote a file modified while confirmation was open.");
            }

            File.WriteAllText(saved, original);
            using (var form = new UserProfileForm())
            {
                SelectFirstProfile(form);
                var replacement = ProfileStore.ParseJson(changed);
                Require(!form.DeleteSelectedProfile(name => { Profile.ProfileList = replacement; return true; }) &&
                    ReferenceEquals(Profile.ProfileList, replacement) && File.ReadAllText(saved) == original,
                    "Deletion overwrote a different catalog selected while confirmation was open.");
            }

            foreach (bool moveFile in new[] { false, true })
            {
                File.WriteAllText(saved, original);
                using (var form = new UserProfileForm())
                {
                    SelectFirstProfile(form);
                    List<DBC> active = Profile.ProfileList;
                    FileStream held = null;
                    string moved = saved + ".moved";
                    bool refused = false;
                    try
                    {
                        form.DeleteSelectedProfile(name =>
                        {
                            if (moveFile) File.Move(saved, moved);
                            else held = new FileStream(saved, FileMode.Open, FileAccess.Read, FileShare.None);
                            return true;
                        });
                    }
                    catch (IOException) { refused = true; }
                    finally { held?.Dispose(); }
                    Require(refused && ReferenceEquals(Profile.ProfileList, active) &&
                        File.ReadAllText(moveFile ? moved : saved) == original && (!moveFile || !File.Exists(saved)),
                        "Deletion changed data or recreated a missing file after a read failure at confirmation.");
                    if (moveFile) File.Move(moved, saved);
                }
            }
            passed("Deletion rechecks the file/catalog after confirmation; concurrent changes and read failures preserve the existing data");
        }

        private static void CheckStepReason(Action<string> passed)
        {
            using (var form = new Form())
            {
                var server = new TextBox();
                var next = new Button();
                form.Controls.Add(server);
                form.Controls.Add(next);
                var flow = new StepFlow(form, null)
                    .Add("Connection", "Fill in the connection", () => StepFlow.Passes(() =>
                        FormValidation.Require(server.Text.Length > 0, server, "Enter the Source data source.")))
                    .Add("Next", "Go on", () => false, next);
                flow.Refresh();
                Require(flow.NextText().Contains("Enter the Source data source."), "The strip must say what the open step still needs: " + flow.NextText());
                Require(flow.LockedText(flow.Steps[1]).Contains("Enter the Source data source"), "A locked step must say what is still missing.");
                server.Text = "db-host";
                flow.Refresh();
                Require(!flow.NextText().Contains("data source"), "A done step must not keep its old reason.");
            }
            passed("The step strip says what the current step still needs (e.g. the password a profile does not save)");
        }
    }
}
