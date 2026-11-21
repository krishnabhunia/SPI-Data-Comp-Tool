using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// The saved profiles file as windows really load it (the other checks put profiles straight into
    /// Profile.ProfileList): early-format files, a moved, damaged or partly invalid file, a new copy of the
    /// app, and that every task still opens. Uses temporary files only; the user's settings are never read.
    /// </summary>
    internal static class ProfileLoadChecks
    {
        private const string Legacy = "[" +
            "{\"profileName\":\"legacy one\",\"dbSRC\":{\"str_DBType\":\"oracle\",\"str_DataSource\":\"ora-host\",\"str_DBName\":\"ORA\",\"str_UserName\":\"u1\",\"str_Password\":\"\"}," +
            "\"dbTRG\":{\"str_DBType\":\"sql\",\"str_DataSource\":\"sql-host\",\"str_DBName\":\"SQLDB\",\"str_UserName\":\"sa\",\"str_Password\":\"\"}}," +
            "{\"profileName\":\"legacy two\",\"dbSRC\":{\"str_DBType\":\"SQL\",\"str_DataSource\":\"sql-two\"},\"dbTRG\":{\"str_DBType\":\"Oracle\",\"str_DataSource\":\"ora-two\"}}]";

        private const string Repeated = "[" +
            "{\"profileName\":\"same\",\"dbSRC\":{\"bool_Rb_SQL\":true,\"str_DataSource\":\"first\"},\"dbTRG\":{}}," +
            "{\"profileName\":\"same\",\"dbSRC\":{\"bool_Rb_SQL\":true,\"str_DataSource\":\"second\"},\"dbTRG\":{}}," +
            "{\"profileName\":\"other\",\"dbSRC\":{\"bool_Rb_Oracle\":true,\"str_DataSource\":\"third\"},\"dbTRG\":{}}]";

        public static void Run(Action<string> passed)
        {
            string folder = Path.Combine(Path.GetTempPath(), "spi-profile-load-" + Guid.NewGuid().ToString("N"));
            List<DBC> previous = Profile.ProfileList;
            string previousPath = ProfileStore.SelfTestProfilePath;
            string previousRemembered = ProfileStore.RememberedPathFileOverride;
            Directory.CreateDirectory(folder);
            try
            {
                ProfileStore.RememberedPathFileOverride = Path.Combine(folder, "profile-path.txt");
                string legacy = Write(folder, "legacy.json", Legacy);
                string upper = Write(folder, "UPPER.JSON", Legacy);
                string broken = Write(folder, "broken.json", "[{\"profileName\":\"cut off\"");
                string repeated = Write(folder, "repeated.json", Repeated);
                string missing = Path.Combine(folder, "moved-away.json");

                CheckFiles(passed, legacy, upper, broken, repeated, missing);
                CheckEarlyFormat(passed, legacy);
                CheckEveryTaskOpens(passed, missing, broken, legacy);
                CheckStepReason(passed);
            }
            finally
            {
                Profile.ProfileList = previous;
                ProfileStore.SelfTestProfilePath = previousPath;
                ProfileStore.RememberedPathFileOverride = previousRemembered;
                try { Directory.Delete(folder, true); } catch (Exception) { }
            }
        }

        private static string Write(string folder, string name, string text)
        {
            string path = Path.Combine(folder, name);
            File.WriteAllText(path, text);
            return path;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static ProfileLoadResult Load(string path)
        {
            ProfileStore.SelfTestProfilePath = path;
            return ProfileStore.LoadSaved();
        }

        private static void CheckFiles(Action<string> passed, string legacy, string upper, string broken, string repeated, string missing)
        {
            ProfileLoadResult none = Load(null);
            Require(none.Profiles == null && !none.IsError && none.Message.Contains("Connection profiles"),
                "Without a chosen file the windows must say where to choose one.");

            ProfileLoadResult early = Load(legacy);
            Require(early.Profiles != null && early.Profiles.Count == 2 && !early.IsError,
                "An early-format profiles file must load: " + early.Message);
            Require(File.Exists(ProfileStore.RememberedPathFileOverride) && ProfileStore.ReadRememberedPath() == legacy,
                "A loaded profiles file must be remembered for a new copy of the app.");

            ProfileLoadResult capitals = Load(upper);
            Require(capitals.Profiles != null && capitals.Profiles.Count == 2,
                "A profiles file ending in .JSON must load in every window: " + capitals.Message);

            File.WriteAllText(ProfileStore.RememberedPathFileOverride, string.Empty);
            ProfileLoadResult moved = Load(missing);
            Require(moved.Profiles == null && moved.IsError && moved.Message.Contains("not found") && moved.Message.Contains("Read profile"),
                "A moved profiles file must say it was not found and how to choose it again: " + moved.Message);

            ProfileStore.RememberPath(legacy);
            ProfileLoadResult newCopy = Load(missing);
            Require(newCopy.Profiles != null && newCopy.Profiles.Count == 2 && newCopy.Path == legacy && newCopy.Message.Contains("not found"),
                "A copy of the app whose own path is missing must find the remembered profiles file: " + newCopy.Message);

            ProfileLoadResult damaged = Load(broken);
            Require(damaged.Profiles == null && damaged.IsError && damaged.Message.Contains(broken),
                "A damaged profiles file must give a message, not an exception: " + damaged.Message);

            ProfileLoadResult partly = Load(repeated);
            Require(partly.Profiles != null && partly.Profiles.Count == 2 && partly.IsError && partly.Message.Contains("skipped"),
                "A file with a repeated profile name must keep the usable profiles and say how many were skipped: " + partly.Message);
            Require(partly.Profiles[0].dbSRC.str_DataSource == "first" && partly.Profiles[1].profileName == "other",
                "The first of two profiles with the same name must be kept.");
            ProfileStore.Validate(partly.Profiles);   // what was kept can be saved again
            passed("Saved profiles: none, early format, .JSON, moved, new copy of the app, damaged and repeated names all load or explain");
        }

        private static void CheckEarlyFormat(Action<string> passed, string legacy)
        {
            Profile.ProfileList = Load(legacy).Profiles;
            var first = new DBConnectionDetails(0);
            Require(first.bool_Rb_SRC_Oracle && !first.bool_Rb_SRC_SQL && first.bool_SRC_Oracle_DefaultPort,
                "An early profile with str_DBType oracle must select Oracle with the default port.");
            Require(first.bool_Rb_TRG_SQL && !first.bool_Rb_TRG_Oracle, "An early profile with str_DBType sql must select SQL Server.");
            var second = new DBConnectionDetails(1);
            Require(second.bool_Rb_SRC_SQL && second.bool_Rb_TRG_Oracle, "str_DBType must be read without regard to case.");

            using (var form = new DB_Comparison_Form())
            {
                UiTheme.EnsureApplied(form);
                form.Show();
                Application.DoEvents();
                ConnectionPanel panel = Panels(form).Single();
                Require(panel.ProfileList.Items.Count == 2, "Database comparison must list the early-format profiles.");
                panel.ProfileList.SelectedIndex = 0;
                Application.DoEvents();
                Require(panel.Source.Oracle.Checked && !panel.Source.Sql.Checked && panel.Source.DataSource.Text == "ora-host",
                    "Choosing an early profile must select Oracle and fill the Source.");
                Require(panel.Target.Sql.Checked && !panel.Target.Oracle.Checked && panel.Target.DataSource.Text == "sql-host",
                    "Choosing an early profile must select SQL Server and fill the Target.");
                form.Close();
            }
            passed("Early-format profiles (str_DBType only) select Oracle / SQL Server when chosen");
        }

        private static void CheckEveryTaskOpens(Action<string> passed, string missing, string broken, string legacy)
        {
            foreach (string path in new[] { missing, broken, legacy })
            {
                ProfileStore.SelfTestProfilePath = path;
                File.WriteAllText(ProfileStore.RememberedPathFileOverride, string.Empty);
                Profile.ProfileList = null;
                foreach (WorkspaceTask task in WorkspaceCatalog.Tasks)
                {
                    WorkspacePageForm page;
                    try { page = task.CreatePage(); }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(task.Title + " did not open with the profiles file " +
                            Path.GetFileName(path) + ": " + ex.Message, ex);
                    }
                    using (page)
                    {
                        UiTheme.EnsureApplied(page);
                        page.Show();
                        Application.DoEvents();
                        foreach (ConnectionPanel panel in Panels(page))
                            Require(panel.ProfileList.Items.Count == (path == legacy ? 2 : 0),
                                task.Title + " listed the wrong profiles for " + Path.GetFileName(path) + ".");
                        page.Close();
                    }
                }
            }
            passed("Every task opens with a moved, damaged and early-format profiles file (" + WorkspaceCatalog.Tasks.Count + " tasks)");
        }

        private static void CheckStepReason(Action<string> passed)
        {
            using (var form = new Form())
            {
                var password = new TextBox();
                var next = new Button();
                form.Controls.Add(password);
                form.Controls.Add(next);
                var flow = new StepFlow(form, null)
                    .Add("Connection", "Fill in the connection", () => StepFlow.Passes(() =>
                        FormValidation.Require(password.Text.Length > 0, password, "Enter the Source password.")))
                    .Add("Next", "Go on", () => false, next);
                flow.Refresh();
                Require(flow.NextText().Contains("Enter the Source password."), "The strip must say what the open step still needs: " + flow.NextText());
                Require(flow.LockedText(flow.Steps[1]).Contains("Enter the Source password"), "A locked step must say what is still missing.");
                password.Text = "x";
                flow.Refresh();
                Require(!flow.NextText().Contains("password"), "A done step must not keep its old reason.");
            }
            passed("The step strip says what the current step still needs (e.g. a profile without a saved password)");
        }

        private static IEnumerable<ConnectionPanel> Panels(Control root)
        {
            foreach (Control child in root.Controls)
            {
                var panel = child as ConnectionPanel;
                if (panel != null) yield return panel;
                foreach (ConnectionPanel nested in Panels(child)) yield return nested;
            }
        }
    }
}
