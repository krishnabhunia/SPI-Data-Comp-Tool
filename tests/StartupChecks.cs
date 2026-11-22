using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Cold workspace navigation with synthetic optional-profile files. Never opens a database or saves preferences.</summary>
    internal static class StartupChecks
    {
        private sealed class Scenario
        {
            internal string Name;
            internal string Path;
            internal bool Valid;
            internal bool Invalid;
            internal bool Locked;
            internal bool AllRoutes;
        }

        private static readonly HashSet<string> ProfileRoutes = new HashSet<string>(StringComparer.Ordinal)
        {
            "comparison.database", "comparison.batch", "reports.database", "settings.profiles"
        };

        // Deliberately omit bool_Rb_SQL/bool_Rb_Oracle: older saved files used str_DBType alone.
        private const string LegacyProfileJson = @"[
            {""profileName"":""Legacy SQL profile"",
             ""dbSRC"":{""str_DBType"":""SQL"",""bool_SQL_WA"":true,
                ""str_DataSource"":""fixture-source"",""str_DBName"":""FixtureSource"",""str_TableName"":""dbo.SourceTable""},
             ""dbTRG"":{""str_DBType"":""SQL"",""bool_SQL_WA"":true,
                ""str_DataSource"":""fixture-target"",""str_DBName"":""FixtureTarget"",""str_TableName"":""dbo.TargetTable""}}
        ]";

        internal static int Run(string outFile)
        {
            var messages = new List<string>();
            var eventErrors = new List<Exception>();
            string previousProfilePath = Properties.Settings.Default.ProfilePathUser;
            string previousRulePath = Properties.Settings.Default.RuleFilePath;
            List<DBC> previousProfiles = Profile.ProfileList;
            string folder = Path.Combine(Path.GetTempPath(), "spi-startup-" + Guid.NewGuid().ToString("N"));
            int failed = 0;
            int checkedRoutes = 0;
            ThreadExceptionEventHandler onThreadException = (sender, args) => eventErrors.Add(args.Exception);
            Application.ThreadException += onThreadException;
            try
            {
                Directory.CreateDirectory(folder);
                string valid = Path.Combine(folder, "legacy-profiles.JSON");
                string malformed = Path.Combine(folder, "malformed.json");
                string incomplete = Path.Combine(folder, "incomplete.json");
                string nullEntry = Path.Combine(folder, "null-entry.json");
                string locked = Path.Combine(folder, "locked.json");
                File.WriteAllText(valid, LegacyProfileJson);
                File.WriteAllText(malformed, "{ invalid json");
                File.WriteAllText(incomplete, "[{\"profileName\":\"Incomplete\",\"dbSRC\":{\"str_DBType\":\"SQL\"}}]");
                File.WriteAllText(nullEntry, "[null]");
                File.WriteAllText(locked, LegacyProfileJson);
                Properties.Settings.Default.RuleFilePath = string.Empty;
                var scenarios = new[]
                {
                    new Scenario { Name = "no remembered profile", Path = string.Empty, AllRoutes = true },
                    new Scenario { Name = "provider-string-only .JSON profile", Path = valid, Valid = true, AllRoutes = true },
                    new Scenario { Name = "missing profile", Path = Path.Combine(folder, "missing.json"), Invalid = true },
                    new Scenario { Name = "malformed profile", Path = malformed, Invalid = true },
                    new Scenario { Name = "incomplete profile", Path = incomplete, Invalid = true },
                    new Scenario { Name = "null profile entry", Path = nullEntry, Invalid = true },
                    new Scenario { Name = "locked profile", Path = locked, Invalid = true, Locked = true },
                };
                Require(WorkspaceCatalog.Tasks.Count == 7 && WorkspaceCatalog.Modes.Count == 6,
                    "The startup matrix must cover the five other rail tasks and the six modes of the two Compare tasks.");
                // The two Compare tasks are opened through each of their modes; the other rail tasks directly.
                List<WorkspaceTask> routes = WorkspaceCatalog.Tasks.Where(item => !(item.Key == "comparison.databases" || item.Key == "comparison.workbooks"))
                    .Concat(WorkspaceCatalog.Modes).ToList();
                foreach (Scenario scenario in scenarios)
                {
                    using (FileStream held = scenario.Locked
                        ? new FileStream(scenario.Path, FileMode.Open, FileAccess.Read, FileShare.None) : null)
                    {
                        foreach (WorkspaceTask task in routes.Where(candidate => scenario.AllRoutes || ProfileRoutes.Contains(candidate.Key)))
                        {
                            eventErrors.Clear();
                            try
                            {
                                CheckRoute(task, scenario, eventErrors);
                                messages.Add("ok   " + scenario.Name + " / " + task.Key);
                            }
                            catch (Exception error)
                            {
                                failed++;
                                Exception cause = error is TargetInvocationException && error.InnerException != null ? error.InnerException : error;
                                messages.Add("FAIL " + scenario.Name + " / " + task.Key + ": " +
                                    Common_Data.RedactConnectionCredentials(cause.Message).Replace("\r", " ").Replace("\n", " "));
                            }
                            checkedRoutes++;
                        }
                    }
                }
                Require(checkedRoutes == 42, "The startup matrix must exercise eleven routes twice and all four profile consumers with five unavailable-profile scenarios.");
                Require(File.ReadAllText(valid) == LegacyProfileJson && File.ReadAllText(locked) == LegacyProfileJson,
                    "Opening a task changed a profile fixture.");
            }
            catch (Exception error)
            {
                failed++;
                messages.Add("FAIL startup fixtures: " + Common_Data.RedactConnectionCredentials(error.Message)
                    .Replace("\r", " ").Replace("\n", " "));
            }
            finally
            {
                Application.ThreadException -= onThreadException;
                Profile.ProfileList = previousProfiles;
                Properties.Settings.Default.ProfilePathUser = previousProfilePath;
                Properties.Settings.Default.RuleFilePath = previousRulePath;
                if (Directory.Exists(folder)) Directory.Delete(folder, true);
            }
            messages.Add(failed == 0 ? "startup checks passed: " + checkedRoutes + " cold route scenarios"
                : failed + " startup check(s) failed across " + checkedRoutes + " cold route scenarios");
            File.WriteAllLines(outFile, messages);
            return failed == 0 ? 0 : 1;
        }

        private static void CheckRoute(WorkspaceTask task, Scenario scenario, List<Exception> eventErrors)
        {
            Properties.Settings.Default.ProfilePathUser = scenario.Path;
            List<DBC> memory = string.IsNullOrEmpty(scenario.Path) ? null : new List<DBC> { MemoryProfile() };
            Profile.ProfileList = memory;
            var navigator = new WorkspaceNavigator();
            using (var owner = new Form { ShowInTaskbar = false, Text = "Synthetic startup owner" })
            {
                try
                {
                    owner.Show();
                    // Open through the real cold navigator: its new workspace has never been
                    // shown, and OpenTask must attach the page before the host gets a handle.
                    string[] options = null;
                    bool mode = WorkspaceCatalog.Modes.Contains(task);
                    string railKey = mode ? WorkspaceCatalog.TaskOfMode(task.Key, out options) : task.Key;
                    WorkspacePageForm railPage = navigator.Open(owner, railKey);
                    WorkspacePageForm page = mode ? ((CompareModePage)railPage).Choose(options) : railPage;
                    PumpEvents();
                    NoEventErrors(eventErrors, "opening the task");
                    Require(page != null && !page.IsDisposed && page.Visible && page.IsHandleCreated,
                        "The task did not become a live, visible workspace page. " + PageState(page));
                    Require(page.WindowState == FormWindowState.Normal,
                        "A hosted page retained standalone window state. " + PageState(page));
                    Require(navigator.LoadedWorkspaces.Count == 1 && page.HostWorkspace != null
                        && ReferenceEquals(page.HostWorkspace.ActivePage, railPage), "The navigator did not activate the new task.");
                    CheckInitialization(page);

                    bool usesProfiles = ProfileRoutes.Contains(task.Key);
                    if (usesProfiles)
                    {
                        var connection = Find<ConnectionPanel>(page, "connection");
                        if (scenario.Valid)
                        {
                            Require(!ReferenceEquals(Profile.ProfileList, memory) && Profile.ProfileList != null
                                && Profile.ProfileList.Count == 1 && Profile.ProfileList[0].profileName == "Legacy SQL profile",
                                "A valid profile file did not replace the previous in-memory catalog.");
                            Require(connection.ProfileList.Items.Count == 1,
                                "The valid saved profile was missing from the picker.");
                            // Use the actual selection event even in the profile editor, which
                            // may already have selected the first item during startup.
                            connection.ProfileList.SelectedIndex = -1;
                            connection.ProfileList.SelectedIndex = 0;
                            PumpEvents();
                            Require(connection.Source.Sql.Checked && !connection.Source.Oracle.Checked
                                && connection.Source.WindowsLogin.Checked,
                                "The legacy SQL provider was not applied to the Source controls.");
                            if (connection.Mode == ConnectionMode.SourceAndTarget)
                                Require(connection.Target.Sql.Checked && !connection.Target.Oracle.Checked
                                    && connection.Target.WindowsLogin.Checked,
                                    "The legacy SQL provider was not applied to the Target controls.");
                        }
                        else
                        {
                            Require(ReferenceEquals(Profile.ProfileList, memory),
                                "An absent or invalid optional profile changed the active catalog.");
                            if (scenario.Invalid)
                            {
                                Require(connection.ProfileList.Items.Count == 1
                                    && (string)connection.ProfileList.Items[0] == "In-memory profile",
                                    "A profile warning discarded the existing picker entries.");
                                Require(Descendants(page).Any(control => control.Text.Contains("Load profile")),
                                    "The optional-profile failure was not explained with a recovery action.");
                            }
                        }
                        CheckManualConnection(connection);
                    }
                    else
                    {
                        Require(ReferenceEquals(Profile.ProfileList, memory), "An unrelated task unexpectedly reloaded connection profiles.");
                    }

                    WorkspacePageForm reopened = navigator.Open(owner, railKey);
                    PumpEvents();
                    Require(ReferenceEquals(railPage, reopened) && navigator.LoadedWorkspaces.Count == 1
                        && (!mode || ReferenceEquals(page, ((CompareModePage)railPage).ActiveMode)),
                        "Reopening the same route replaced the user's task instance.");
                    NoEventErrors(eventErrors, "selecting a profile or reopening the task");
                    Require(Properties.Settings.Default.ProfilePathUser == scenario.Path
                        && Properties.Settings.Default.RuleFilePath == string.Empty,
                        "Opening a task changed remembered file settings.");
                }
                finally
                {
                    foreach (WorkspaceForm workspace in navigator.LoadedWorkspaces.ToArray())
                    {
                        try { workspace.Close(); }
                        finally { workspace.Dispose(); }
                    }
                    owner.Close();
                    PumpEvents();
                }
            }
            NoEventErrors(eventErrors, "closing the task");
        }

        private static void CheckManualConnection(ConnectionPanel connection)
        {
            Require(connection.ProfileList.Enabled && connection.Source.DataSource.Enabled && connection.Source.Database.Enabled,
                "An optional-profile warning or loaded profile left the manual Source controls disabled.");
            connection.Source.Sql.Checked = true;
            connection.Source.WindowsLogin.Checked = true;
            connection.Source.DataSource.Text = "manual-source";
            connection.Source.Database.Text = "ManualSource";
            PumpEvents();
            if (connection.Mode == ConnectionMode.SourceAndTarget)
            {
                Require(connection.Target.Group.Enabled && connection.Target.DataSource.Enabled,
                    "Valid manual Source fields did not make Target editable.");
                connection.Target.Sql.Checked = true;
                connection.Target.WindowsLogin.Checked = true;
                connection.Target.DataSource.Text = "manual-target";
                connection.Target.Database.Text = "ManualTarget";
                PumpEvents();
            }
            connection.ValidateInput(); // Validate syntax/required fields only; never opens a database.
            Require(connection.Source.DataSource.Text == "manual-source", "Manual Source input was overwritten during refresh.");
        }

        private static string PageState(WorkspacePageForm page)
        {
            if (page == null) return "Page=null.";
            return "Page=" + page.Name + ", Disposed=" + page.IsDisposed + ", Visible=" + page.Visible
                + ", Handle=" + page.IsHandleCreated + ", WindowState=" + page.WindowState
                + ", HostVisible=" + (page.HostWorkspace != null && page.HostWorkspace.Visible)
                + ", ParentVisible=" + (page.Parent != null && page.Parent.Visible) + ".";
        }

        private static void CheckInitialization(WorkspacePageForm page)
        {
            if (page is DB_Comparison_Form)
                Fields(page, "common_Data", "dsRules", "dsAllColumns", "dt_LoadSRC", "dt_LoadTRG", "dt_CommonSRC", "dt_CommonTRG", "saveFileDialogFile", "steps");
            else if (page is Compare_Multiples_List_Tables_Forms)
            {
                Fields(page, "common_Data", "dsRules", "dtTableList", "dsTableList", "dsAllColumns", "dbSRCConnection", "dbTRGConnection", "steps");
                Require(Get<DataTable>(page, "dtSummaryForList").Columns.Count == 5,
                    "Batch startup stopped before constructing its five-column summary table.");
            }
            else if (page is SQL_DB_To_Excel)
                Fields(page, "common_Data", "ds", "dt", "dt_sql", "dt_oracle", "lstTableDetails", "steps");
            else if (page is CustomisedExcel)
                Fields(page, "common_Data", "dsCustomisedExcel", "openFileDialogData", "steps");
            else if (page is Excel_Excel_Comparison)
                Fields(page, "common_Data", "ds", "dsRules", "dsUnmatched", "firstExcelData", "secondExcelData", "steps");
            else if (page is UserProfileForm)
                Fields(page, "common_Data", "dBConnectionDetails", "dBConnectionStatus_SRC", "dBConnectionStatus_TRG",
                    "openFileDialogData", "dBConnectionDetailsProfile", "saveFileDialogFile");
            else if (page is User_Settings)
            {
                var settings = (User_Settings)page;
                var tabs = Find<TabControl>(page, "tabs_Settings");
                var themes = Find<ComboBox>(page, "cb_Theme");
                var close = Find<Button>(page, "btn_Close");
                Require(tabs.TabPages.Count == 4 && tabs.TabPages[3].Name == "tab_About", "Settings startup did not complete its four sections (About last).");
                Require(themes.Items.Count == UiTheme.All.Length && ReferenceEquals(themes.SelectedItem, UiTheme.Current),
                    "Settings startup stopped before initializing the colour-theme picker.");
                Require(ReferenceEquals(settings.AcceptButton, close) && ReferenceEquals(settings.CancelButton, close),
                    "Settings startup stopped before wiring its default buttons.");
                TabPage initial = tabs.SelectedTab;
                settings.ShowRules();
                Require(tabs.SelectedTab.Name == "tab_Rules"
                    && ReferenceEquals(settings.AcceptButton, Find<Button>(page, "btn_ImportRules")),
                    "Opening the Rules section did not activate its import action.");
                tabs.SelectedTab = initial;
                Require(ReferenceEquals(settings.AcceptButton, close),
                    "Returning to General did not restore its default action.");
            }
            Require(page.Controls.Count > 0, "The task has no initialized controls.");
            FieldInfo steps = page.GetType().GetField("steps", BindingFlags.Instance | BindingFlags.NonPublic);
            if (steps != null)
            {
                var flow = (StepFlow)steps.GetValue(page);
                Require(flow != null && flow.Steps.Count > 0 && flow.Strip != null && !flow.Strip.IsDisposed,
                    "The controlled steps were not initialized completely.");
            }
        }

        private static DBC MemoryProfile()
        {
            return new DBC
            {
                profileName = "In-memory profile",
                dbSRC = new DBConnectionDetailsProfile { str_DBType = "SQL", bool_Rb_SQL = true, bool_SQL_WA = true,
                    str_DataSource = "memory-source", str_DBName = "MemorySource", str_Password = string.Empty },
                dbTRG = new DBConnectionDetailsProfile { str_DBType = "SQL", bool_Rb_SQL = true, bool_SQL_WA = true,
                    str_DataSource = "memory-target", str_DBName = "MemoryTarget", str_Password = string.Empty }
            };
        }

        private static IEnumerable<Control> Descendants(Control owner)
        {
            foreach (Control child in owner.Controls)
            {
                yield return child;
                foreach (Control descendant in Descendants(child)) yield return descendant;
            }
        }

        private static T Find<T>(Control owner, string name) where T : Control
        {
            T control = owner.Controls.Find(name, true).OfType<T>().FirstOrDefault();
            Require(control != null, "Missing initialized control: " + name);
            return control;
        }

        private static T Get<T>(object owner, string name) where T : class
        {
            FieldInfo field = owner.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Require(field != null, "Missing startup field: " + name);
            T value = field.GetValue(owner) as T;
            Require(value != null, "Startup did not initialize: " + name);
            return value;
        }

        private static void Fields(object owner, params string[] names)
        {
            foreach (string name in names) Get<object>(owner, name);
        }

        private static void PumpEvents()
        {
            for (int i = 0; i < 4; i++) Application.DoEvents();
        }

        private static void NoEventErrors(List<Exception> errors, string phase)
        {
            if (errors.Count > 0)
                throw new InvalidOperationException("A UI event failed while " + phase + ": " +
                    Common_Data.RedactConnectionCredentials(errors[0].Message), errors[0]);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
