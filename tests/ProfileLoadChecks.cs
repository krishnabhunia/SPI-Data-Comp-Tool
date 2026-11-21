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
            Directory.CreateDirectory(folder);
            try
            {
                ProfileStore.RememberedPathFileOverride = Path.Combine(folder, "profile-path.txt");
                string saved = Path.Combine(folder, "profiles.json");
                File.WriteAllText(saved, Legacy);
                CheckRememberedPath(passed, saved, Path.Combine(folder, "moved-away.json"));
                CheckStepReason(passed);
            }
            finally
            {
                Profile.ProfileList = previous;
                ProfileStore.RememberedPathFileOverride = previousRemembered;
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

            File.Delete(saved);
            Profile.ProfileList = null;
            ProfileLoadResult gone = ProfileStore.LoadRememberedProfiles(() => missing);
            Require(!gone.Loaded && gone.HasError && Profile.ProfileList == null,
                "When neither file exists the normal recovery message must be shown: " + gone.Message);
            passed("A new copy of the app finds the last profiles file chosen on this computer; a missing one still reports how to recover");
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
