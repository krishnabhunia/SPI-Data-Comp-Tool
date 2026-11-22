using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Offline Windows checks for the task workspaces. These exercise real controls and close
    /// events, but never click a database, import, export, SQL-execution or updater action.
    /// </summary>
    internal static class WorkspaceUiChecks
    {
        internal static void Run(Action<string> passed)
        {
            CheckRetiredWorkflowsAbsent();
            passed("Retired workflow routes, workspace categories and implementation types are absent from the application.");
            CheckCatalogAndRealPages();
            passed("All 7 rail tasks open lazily in three groups; the 6 comparison workflows keep their types behind the two Compare mode bars.");
            CheckCompareModes();
            passed("Each comparison workflow opens only through its Compare task's mode bar; unavailable combinations are disabled and each list mode shows only its own tab.");
            CheckCompareModeLifecycle();
            passed("Compare modes keep their state while switching, pass on the workspace, refuse to close while one mode refuses and close every mode once.");
            CheckNavigation();
            passed("Workspace navigation reuses pages and restores windows without replacing independent sessions or modes.");
            CheckCloseLifecycle();
            passed("Workspace closure is all-or-nothing when a page vetoes or its close guard fails; accepted pages close once.");
            CheckTransientPagesAndFailedFactory();
            passed("Advanced mapping sessions retain independent input state, and a failed page factory preserves existing pages.");
            CheckAdvancedMappingCounts();
            passed("Advanced mapping displays the row and column counts of its actual target worksheet snapshot.");
            CheckOwnerClose();
            passed("Closing the main owner cannot discard a hosted page that vetoes shutdown.");
            CheckHiddenWorker();
            passed("Background workers remain detectable and protected while their workspace page is hidden.");
            CheckUpdaterPolicy();
            passed("Only a Settings workspace containing preferences alone is eligible for updating; hidden profile pages still block installation.");
            CheckShellUpdaterPolicy();
            passed("The main window's rail shell can install an update once only Settings pages (with About) are loaded; a loaded data task still blocks it.");
            CheckSessionMappingNavigation();
            passed("A New session window has no rail, but shows its own list while an advanced-mapping snapshot is open so both pages stay reachable.");
            CheckDialogKeys();
            passed("Enter and Escape use only the active page's buttons; null buttons never trigger inactive tasks or recursive navigation.");
            CheckHostedInputChecks();
            passed("Hosted input checks share the page's provider, keep other pages' marks and logs isolated, and dispose safely.");
            CheckSuccessfulApplicationExit();
            passed("An isolated successful Application.Exit closes and disposes each hosted page exactly once.");
        }

        private static void Require(bool value, string message)
        {
            if (!value) throw new InvalidOperationException(message);
        }

        private static void PumpUntil(Func<bool> complete, string message)
        {
            var watch = Stopwatch.StartNew();
            while (!complete() && watch.ElapsedMilliseconds < 5000)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Require(complete(), message);
        }

        private static WorkspaceTask Task(string key, WorkspaceKind workspace, Func<WorkspacePageForm> create,
            bool separate = true)
        {
            return new WorkspaceTask(key, key, "Synthetic offline workspace task", workspace, create, separate);
        }

        private static void CheckDialogKeys()
        {
            int firstAccept = 0, firstCancel = 0, secondAccept = 0, secondCancel = 0;
            Func<string, Action, Action, TestPage> create = (name, acceptAction, cancelAction) =>
            {
                var page = new TestPage(name);
                var accept = new Button { Text = "Synthetic accept", Dock = DockStyle.Bottom };
                var cancel = new Button { Text = "Synthetic cancel", Dock = DockStyle.Bottom };
                accept.Click += (sender, args) => acceptAction();
                cancel.Click += (sender, args) => cancelAction();
                page.Controls.Add(accept);
                page.Controls.Add(cancel);
                page.AcceptButton = accept;
                page.CancelButton = cancel;
                return page;
            };
            var tasks = new[]
            {
                Task("comparison.keys-one", WorkspaceKind.Comparison,
                    () => create("keys one", () => firstAccept++, () => firstCancel++)),
                Task("comparison.keys-two", WorkspaceKind.Comparison,
                    () => create("keys two", () => secondAccept++, () => secondCancel++)),
            };
            MethodInfo processKey = typeof(WorkspaceForm).GetMethod("ProcessDialogKey",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            Require(processKey != null, "The workspace keyboard forwarding hook was not found.");
            using (var workspace = new WorkspaceForm(WorkspaceKind.Comparison, tasks))
            {
                workspace.ShowInTaskbar = false;
                workspace.Show();
                workspace.OpenTask("comparison.keys-one");
                processKey.Invoke(workspace, new object[] { Keys.Enter });
                processKey.Invoke(workspace, new object[] { Keys.Escape });
                Require(firstAccept == 1 && firstCancel == 1,
                    "Workspace Enter/Escape did not use the active page's accept/cancel buttons.");
                workspace.OpenTask("comparison.keys-two");
                workspace.ActivePage.AcceptButton = null;
                processKey.Invoke(workspace, new object[] { Keys.Enter });
                processKey.Invoke(workspace, new object[] { Keys.Escape });
                Require(firstAccept == 1 && firstCancel == 1 && secondAccept == 0 && secondCancel == 1,
                    "A null AcceptButton triggered an action, or Escape used an inactive page's CancelButton.");
                workspace.ActivePage.CancelButton = null;
                processKey.Invoke(workspace, new object[] { Keys.Enter });
                processKey.Invoke(workspace, new object[] { Keys.Escape });
                Require(firstAccept == 1 && firstCancel == 1 && secondAccept == 0 && secondCancel == 1 &&
                    !workspace.IsDisposed && workspace.LoadedPages.Count == 2,
                    "Null dialog buttons caused recursive forwarding, closed a page, or ran another task.");
                workspace.OpenTask("comparison.keys-one");
                processKey.Invoke(workspace, new object[] { Keys.Enter });
                Require(firstAccept == 2 && secondAccept == 0,
                    "Returning to a page did not restore its own Enter action.");
                workspace.Close();
            }
        }

        private static void CheckHostedInputChecks()
        {
            Func<string, TestPage> create = name =>
            {
                var page = new TestPage(name);
                page.Draft.Visible = false;
                page.Controls.Add(new RichTextBox { Name = "rtb_Log", Dock = DockStyle.Bottom, Height = 140 });
                page.Controls.Add(new ConnectionPanel { Dock = DockStyle.Top, Mode = ConnectionMode.SourceOnly });
                return page;
            };
            var tasks = new[]
            {
                Task("comparison.inputs-one", WorkspaceKind.Comparison, () => create("inputs one")),
                Task("comparison.inputs-two", WorkspaceKind.Comparison, () => create("inputs two")),
            };
            using (var workspace = new WorkspaceForm(WorkspaceKind.Comparison, tasks))
            {
                workspace.ShowInTaskbar = false;
                workspace.Show();
                var first = (TestPage)workspace.OpenTask("comparison.inputs-one");
                var firstPanel = first.Controls.OfType<ConnectionPanel>().Single();
                var firstLog = first.Controls.OfType<RichTextBox>().Single();
                Application.DoEvents();
                Require(!firstPanel.AddChecks(InputCheck.For(first)).Report(),
                    "An empty hosted Source connection unexpectedly passed validation.");
                string firstReason = InputCheck.ReasonAt(firstPanel.Source.DataSource);
                string firstMessages = firstLog.Text;
                Require(firstReason == "Source data source is empty." &&
                    firstMessages.Contains("Source database is empty.") &&
                    firstMessages.Contains("4 inputs need fixing. Nothing was started."),
                    "The hosted page did not expose its Source marks and validation log.");

                var second = (TestPage)workspace.OpenTask("comparison.inputs-two");
                var secondPanel = second.Controls.OfType<ConnectionPanel>().Single();
                var secondLog = second.Controls.OfType<RichTextBox>().Single();
                Application.DoEvents();
                Require(secondLog.TextLength == 0 && InputCheck.ReasonAt(secondPanel.Source.DataSource) == string.Empty,
                    "Opening another page inherited the first page's validation marks or log.");
                Require(!secondPanel.AddChecks(InputCheck.For(secondPanel.Source.DataSource)).Report(),
                    "Validation from a child control did not check its hosted page.");
                Require(InputCheck.ReasonAt(secondPanel.Source.DataSource) == firstReason && secondLog.TextLength > 0,
                    "Validation from a child control did not use its own page's provider and log.");
                InputCheck.Clear(second);
                Require(InputCheck.ReasonAt(secondPanel.Source.DataSource) == string.Empty &&
                    InputCheck.ReasonAt(firstPanel.Source.DataSource) == firstReason && firstLog.Text == firstMessages,
                    "Clearing another page erased the hidden first page's marks or changed its log.");

                workspace.OpenTask("comparison.inputs-one");
                InputCheck.Clear(firstPanel.Source.DataSource);
                Require(InputCheck.ReasonAt(firstPanel.Source.DataSource) == string.Empty,
                    "Clearing through a child did not find the provider created through its page.");
                Require(!firstPanel.AddChecks(InputCheck.For(firstPanel)).Report(),
                    "Rechecking the hosted panel unexpectedly accepted its unchanged empty inputs.");
                first.Dispose();
                Require(first.IsDisposed && firstPanel.IsDisposed && workspace.LoadedPages.Count == 1 &&
                    ReferenceEquals(workspace.ActivePage, second),
                    "Disposing a page with active error marks left it registered or damaged the remaining page.");
                Require(!secondPanel.AddChecks(InputCheck.For(second)).Report(),
                    "Disposing the first page broke validation in the remaining page.");
                workspace.Close();
                Require(second.IsDisposed && secondPanel.IsDisposed,
                    "Closing the workspace left the other validation page or its controls undisposed.");
            }
        }

        private static void CheckSuccessfulApplicationExit()
        {
            string logFile = Path.Combine(Path.GetTempPath(), "spi-workspace-exit-" + Guid.NewGuid().ToString("N") + ".log");
            try
            {
                // Run only this build's synthetic exit mode. Never terminate the shared --check-ui process.
                using (var child = Process.Start(new ProcessStartInfo(Application.ExecutablePath,
                    "--check-workspace-exit \"" + logFile + "\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath),
                }))
                {
                    Require(child != null, "The isolated workspace exit probe did not start.");
                    if (!child.WaitForExit(14000))
                    {
                        // The only kill target is the exact Process returned by the launch above.
                        if (!child.HasExited) child.Kill();
                        child.WaitForExit(1000);
                        throw new InvalidOperationException("The isolated workspace exit probe exceeded its 15-second limit.");
                    }
                    string result = File.Exists(logFile) ? File.ReadAllText(logFile) : "No probe result was written.";
                    Require(child.ExitCode == 0 && result.StartsWith("PASS ", StringComparison.Ordinal),
                        "Isolated workspace exit failed: " + result);
                }
            }
            finally
            {
                if (File.Exists(logFile)) File.Delete(logFile);
            }
        }

        internal static int RunExitProbe(string logFile)
        {
            MainForm main = null;
            WorkspaceForm workspace = null;
            TestPage first = null, second = null;
            Exception failure = null;
            bool exitAccepted = false;
            int mainClosed = 0, workspaceClosed = 0;
            string result;
            int exitCode;
            try
            {
                var tasks = new[]
                {
                    Task("comparison.exit-one", WorkspaceKind.Comparison, () => new TestPage("exit one")),
                    Task("comparison.exit-two", WorkspaceKind.Comparison, () => new TestPage("exit two")),
                };
                var navigator = new WorkspaceNavigator(tasks);
                main = new MainForm { ShowInTaskbar = false };
                main.FormClosed += (sender, args) => mainClosed++;
                main.Shown += (sender, args) => main.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        first = (TestPage)navigator.Open(main, "comparison.exit-one");
                        second = (TestPage)navigator.Open(main, "comparison.exit-two");
                        workspace = navigator.LoadedWorkspaces.Single();
                        workspace.FormClosed += (closingSender, closingArgs) => workspaceClosed++;
                        Require(first.IsHandleCreated && second.IsHandleCreated,
                            "The exit probe did not create both hosted page handles.");
                        var canceled = new CancelEventArgs();
                        Application.Exit(canceled);
                        exitAccepted = !canceled.Cancel;
                        if (canceled.Cancel) throw new InvalidOperationException("Idle synthetic pages canceled application exit.");
                    }
                    catch (Exception ex)
                    {
                        failure = ex;
                        Application.ExitThread();
                    }
                }));
                Application.Run(main);
                if (failure != null) throw new InvalidOperationException("The isolated exit callback failed.", failure);
                Require(exitAccepted && first != null && second != null && workspace != null,
                    "The isolated application exited without completing its probe.");
                Require(first.ClosedCount == 1 && second.ClosedCount == 1 && workspaceClosed == 1 && mainClosed == 1,
                    "Successful application exit raised duplicate or missing FormClosed notifications.");
                Require(first.IsDisposed && second.IsDisposed && workspace.IsDisposed && main.IsDisposed,
                    "Successful application exit left hosted pages or their owners undisposed.");
                result = "PASS Application.Exit closed and disposed the main window, workspace and two hosted pages exactly once.";
                exitCode = 0;
            }
            catch (Exception ex)
            {
                result = "FAIL " + ex;
                exitCode = 1;
            }
            finally
            {
                // Probe-only objects; no live workflow or user-owned window is touched here.
                first?.Dispose();
                second?.Dispose();
                workspace?.Dispose();
                main?.Dispose();
            }
            File.WriteAllText(logFile, result);
            return exitCode;
        }

        private static IEnumerable<Control> AllControls(Control root)
        {
            foreach (Control child in root.Controls)
            {
                yield return child;
                foreach (Control nested in AllControls(child)) yield return nested;
            }
        }

        private static void CheckRetiredWorkflowsAbsent()
        {
            string[] retiredRoutes = {
                "analysis.gaps", "analysis.notes", "analysis.changes", "analysis.legacy",
                "analysis.relationships", "analysis.extraction", "premerge.integrity",
                "premerge.duplicates", "premerge.configuration", "scripts.generate", "scripts.execute"
            };
            foreach (string route in retiredRoutes)
            {
                bool rejected = false;
                try { WorkspaceCatalog.Find(route); }
                catch (ArgumentException) { rejected = true; }
                Require(rejected, "A retired workflow remains addressable: " + route);
            }
            string[] retiredTypes = {
                "SPI_Analysis", "SPI_Analysis_View", "OracleAndSQLConnection",
                "SPI_Spec_Note_Creation_Utility", "ExcelFileReadCustomControl",
                "SPIDeltaChangeTrackerOptimised", "SPIDeltaChangeLegacyID", "SPI_FK_Extraction",
                "Table_Extraction_After_Delta_Form", "PreMergingTool_Form", "Pre_Merging_Duplicacy_Form",
                "Generate_IUD_Script_Form", "Execute_Excel_Scripts_Form"
            };
            Assembly assembly = typeof(MainForm).Assembly;
            foreach (string name in retiredTypes)
                Require(assembly.GetType("SPI_Data_Comp_Tool." + name, false) == null,
                    "A retired implementation is still compiled into the application: " + name);
            Require(Enum.GetNames(typeof(WorkspaceKind)).OrderBy(name => name).SequenceEqual(
                    new[] { "Comparison", "DataReports", "Settings" }),
                "The application exposes a retired or unexpected workspace category.");
            using (var main = new MainForm())
            {
                // One rail is the only way in: no menu bar, no home tiles, and no rail entry for a retired route.
                Require(!main.Controls.Find("menuStrip1", true).Any() && !AllControls(main).OfType<MenuStrip>().Any(),
                    "The main window still has a menu bar that repeats the rail.");
                Require(!AllControls(main).OfType<TileButton>().Any(), "The main window still has home tiles that repeat the rail.");
                foreach (string route in retiredRoutes)
                    Require(!main.Controls.Find("task_" + route.Replace('.', '_'), true).Any(),
                        "A retired workflow still has a rail entry: " + route);
                foreach (WorkspaceTask task in WorkspaceCatalog.Tasks)
                    Require(main.Controls.Find("task_" + task.Key.Replace('.', '_'), true).Length == 1,
                        "The rail does not list this task exactly once: " + task.Key);
            }
        }

        private static void CheckCatalogAndRealPages()
        {
            var expected = new Dictionary<WorkspaceKind, Dictionary<string, Type>>
            {
                { WorkspaceKind.Comparison, new Dictionary<string, Type>
                    {
                        { "comparison.databases", typeof(CompareModePage) },
                        { "comparison.workbooks", typeof(CompareModePage) },
                    }
                },
                { WorkspaceKind.DataReports, new Dictionary<string, Type>
                    {
                        { "reports.database", typeof(SQL_DB_To_Excel) },
                        { "reports.excel", typeof(CustomisedExcel) },
                        { "reports.logs", typeof(MergeLogsExport) },
                    }
                },
                { WorkspaceKind.Settings, new Dictionary<string, Type>
                    {
                        { "settings.preferences", typeof(User_Settings) },
                        { "settings.profiles", typeof(UserProfileForm) },
                    }
                },
            };
            Require(WorkspaceCatalog.Tasks.Count() == 7, "A supported task route was lost or duplicated.");
            foreach (var group in expected)
            {
                Require(WorkspaceCatalog.TasksFor(group.Key).Count() == group.Value.Count,
                    "Unexpected task count in " + group.Key + ".");
                using (var workspace = new WorkspaceForm(group.Key, WorkspaceCatalog.TasksFor(group.Key)))
                {
                    Require(!workspace.LoadedPages.Any(), "Constructing a workspace eagerly created its workflows.");
                    workspace.ShowInTaskbar = false;
                    workspace.Show();
                    Application.DoEvents();
                    Require(!workspace.LoadedPages.Any(), "Showing a workspace eagerly loaded an unselected workflow.");
                    int loaded = 0;
                    foreach (var route in group.Value)
                    {
                        Require(WorkspaceCatalog.Find(route.Key) != null, "Missing task route " + route.Key + ".");
                        workspace.OpenTask(route.Key);
                        Application.DoEvents();
                        WorkspacePageForm page = workspace.ActivePage;
                        Require(page != null && page.GetType() == route.Value,
                            "The route " + route.Key + " opens the wrong workflow.");
                        Require(!page.TopLevel && page.FormBorderStyle == FormBorderStyle.None,
                            "A workflow is still a separate top-level window: " + route.Key + ".");
                        Require(page.Parent != null && !page.IsDisposed && page.Visible,
                            "The selected workflow is not hosted visibly: " + route.Key + ".");
                        Require(workspace.LoadedPages.Count == ++loaded,
                            "Opening a task eagerly created or replaced another page.");
                    }
                    workspace.Close();
                    Require(workspace.IsDisposed, "An idle workspace failed to close.");
                }
            }
        }

        private static void CheckCompareModes()
        {
            var types = new Dictionary<string, Type>
            {
                { "comparison.database", typeof(DB_Comparison_Form) },
                { "comparison.batch", typeof(Compare_Multiples_List_Tables_Forms) },
                { "comparison.excel", typeof(Excel_Excel_Comparison) },
                { "comparison.excel-audit", typeof(ExcelSheetAuditForm) },
                { "comparison.identity", typeof(WorkbookIdentityForm) },
                { "comparison.batch-folders", typeof(Compare_Multiples_List_Tables_Forms) },
            };
            Require(WorkspaceCatalog.Modes.Select(mode => mode.Key).SequenceEqual(types.Keys), "The comparison modes changed.");
            using (var workspace = new WorkspaceForm(WorkspaceKind.Comparison, WorkspaceCatalog.TasksFor(WorkspaceKind.Comparison)))
            {
                workspace.ShowInTaskbar = false;
                workspace.Show();
                foreach (var mode in types)
                {
                    string[] options;
                    string railKey = WorkspaceCatalog.TaskOfMode(mode.Key, out options);
                    var host = (CompareModePage)workspace.OpenTask(railKey);
                    WorkspacePageForm page = host.Choose(options);
                    Application.DoEvents();
                    Require(page.GetType() == mode.Value && ReferenceEquals(host.ActiveMode, page) && host.ActiveModeKey == mode.Key,
                        "The mode bar opened the wrong workflow for " + mode.Key + ".");
                    Require(!page.TopLevel && page.Visible && ReferenceEquals(page.HostWorkspace, workspace),
                        "A comparison mode is not hosted visibly inside its task: " + mode.Key + ".");
                    for (int i = 0; i < options.Length; i++)
                        Require(host.Controls.Find(CompareModePage.ChoiceName(i == 0 && railKey == "comparison.databases" ? "scope" : (i == 0 ? "check" : "files"), options[i]), true)
                            .OfType<RadioButton>().Single().Checked, "The mode bar does not show the chosen mode: " + mode.Key + ".");
                }
                Require(workspace.LoadedPages.Count == 2, "Choosing modes opened extra rail tasks.");

                var workbooks = (CompareModePage)workspace.OpenTask("comparison.workbooks");
                workbooks.Choose("formatting", "pair");
                Func<string, string, RadioButton> choice = (axis, option) =>
                    workbooks.Controls.Find(CompareModePage.ChoiceName(axis, option), true).OfType<RadioButton>().Single();
                Require(!choice("files", "folders").Enabled && choice("files", "pair").Enabled,
                    "Two folders stayed available for a check it cannot run.");
                workbooks.Choose("values", "folders");
                Require(!choice("check", "formatting").Enabled && !choice("check", "identical").Enabled && choice("check", "values").Enabled,
                    "A check that cannot run on two folders stayed available.");
                bool refused = false;
                try { workbooks.Choose("identical", "folders"); }
                catch (ArgumentException) { refused = true; }
                Require(refused, "An unavailable mode combination was accepted.");

                var databases = (CompareModePage)workspace.OpenTask("comparison.databases");
                var list = (Compare_Multiples_List_Tables_Forms)databases.Choose("list");
                var folders = (Compare_Multiples_List_Tables_Forms)workbooks.Choose("values", "folders");
                Func<Form, string[]> tabs = form => form.Controls.Find("tabControl1", true).OfType<TabControl>().Single()
                    .TabPages.Cast<TabPage>().Select(tab => tab.Name).ToArray();
                Require(tabs(list).SequenceEqual(new[] { "tabPage_DB" }) && tabs(folders).SequenceEqual(new[] { "tabPage_Excel" }),
                    "A list mode shows the other mode's tab, a second way to reach it.");
                workspace.Close();
            }
        }

        private static void CheckCompareModeLifecycle()
        {
            var created = new Dictionary<string, TestPage>();
            Func<string, WorkspacePageForm> create = key => { var page = new TestPage(key); created[key] = page; return page; };
            var tasks = new[] { Task("comparison.synthetic", WorkspaceKind.Comparison, () => CompareModePage.Databases(create)) };
            using (var workspace = new WorkspaceForm(WorkspaceKind.Comparison, tasks))
            {
                workspace.ShowInTaskbar = false;
                workspace.Show();
                var host = (CompareModePage)workspace.OpenTask("comparison.synthetic");
                var table = (TestPage)host.ActiveMode;
                Require(table != null && table.Name == "comparison.database", "Compare databases did not start with One table.");
                table.Draft.Text = "Retained table input";
                var list = (TestPage)host.Choose("list");
                Require(!table.Visible && list.Visible && ReferenceEquals(list.HostWorkspace, workspace),
                    "Switching modes did not hide the other mode or pass on the workspace.");
                Require(ReferenceEquals(table, host.Choose("table")) && table.Draft.Text == "Retained table input" && created.Count == 2,
                    "Switching modes recreated a mode or lost its input.");
                host.Choose("list");
                table.VetoClose = true;
                workspace.CloseActivePage();
                Require(!host.IsDisposed && !table.IsDisposed && !list.IsDisposed && ReferenceEquals(host.ActiveMode, table),
                    "A refusing mode did not stop the task closing, or was not selected.");
                Require(!workspace.TryPrepareWorkspaceClose(CloseReason.UserClosing), "Workspace close ignored a refusing mode.");
                table.VetoClose = false;
                workspace.CloseActivePage();
                Require(host.IsDisposed && table.IsDisposed && list.IsDisposed && table.ClosedCount == 1 && list.ClosedCount == 1,
                    "Closing the task did not close every mode exactly once.");
                workspace.Close();
            }
        }

        private static void CheckNavigation()
        {
            int alphaCreated = 0;
            int configurationCreated = 0;
            var tasks = new[]
            {
                Task("comparison.alpha", WorkspaceKind.Comparison, () => { alphaCreated++; return new TestPage("alpha"); }),
                Task("comparison.beta", WorkspaceKind.Comparison, () => new TestPage("beta")),
                Task("settings.single", WorkspaceKind.Settings, () => new TestPage("settings"), false),
                Task("reports.check", WorkspaceKind.DataReports, () => new TestPage("check")),
                Task("reports.configuration", WorkspaceKind.DataReports,
                    () => { configurationCreated++; return new TestPage("configuration"); }, false),
            };
            var navigator = new WorkspaceNavigator(tasks);
            using (var owner = new Form { ShowInTaskbar = false })
            {
                owner.Show();
                var first = (TestPage)navigator.Open(owner, "comparison.alpha");
                first.Draft.Text = "Retained source selection";
                var originalHost = navigator.LoadedWorkspaces.Single();
                Require(alphaCreated == 1 && ReferenceEquals(first, navigator.Open(owner, "comparison.alpha")),
                    "Repeated navigation recreated the selected page.");
                navigator.Open(owner, "comparison.beta");
                Require(originalHost.LoadedPages.Count == 2 && !first.IsDisposed,
                    "Switching workflow discarded its previous page.");
                navigator.Open(owner, "comparison.alpha");
                Require(first.Draft.Text == "Retained source selection", "Navigation discarded a workflow's draft input.");
                originalHost.WindowState = FormWindowState.Minimized;
                navigator.Open(owner, "comparison.alpha");
                Require(originalHost.WindowState != FormWindowState.Minimized, "Navigation did not restore the workspace.");

                var mapping = (TestPage)originalHost.OpenTransient("Advanced mapping", () => new TestPage("mapping"));
                mapping.Draft.Text = "Retained mapping snapshot";
                navigator.Open(owner, "comparison.alpha");
                originalHost.Controls.Find("task_transient_1", true).OfType<Button>().Single().PerformClick();
                Require(ReferenceEquals(mapping, originalHost.ActivePage) && mapping.Draft.Text == "Retained mapping snapshot",
                    "A transient mapping could not be revisited through its local workspace navigation.");

                var separate = navigator.Open(owner, "comparison.alpha", true);
                Require(!ReferenceEquals(first, separate) && alphaCreated == 2 &&
                    ReferenceEquals(first, navigator.Open(owner, "comparison.alpha")),
                    "A separate session overwrote the default workflow session.");
                var single = navigator.Open(owner, "settings.single");
                Require(ReferenceEquals(single, navigator.Open(owner, "settings.single", true)),
                    "A singleton task accepted an unintended parallel session.");
                var configuration = navigator.Open(owner, "reports.configuration");
                var parallelCheck = navigator.Open(owner, "reports.check", true);
                var parallelHost = navigator.LoadedWorkspaces.Single(window => window.LoadedPages.Contains(parallelCheck));
                var configurationButton = parallelHost.Controls.Find("task_reports_configuration", true).OfType<Button>().Single();
                configurationButton.PerformClick();
                Require(configurationCreated == 1 && parallelHost.LoadedPages.Count == 1 &&
                    ReferenceEquals(configuration, navigator.Open(owner, "reports.configuration")),
                    "A singleton task was duplicated by navigation inside a parallel workspace.");

                first.Close();
                var replacement = (TestPage)navigator.Open(owner, "comparison.alpha");
                Require(first.IsDisposed && !ReferenceEquals(first, replacement) && replacement.Draft.Text.Length == 0,
                    "A closed page was reused instead of creating fresh state.");
                Require(navigator.TryPrepareCloseAll(CloseReason.UserClosing), "Idle navigation unexpectedly vetoed closure.");
                foreach (var workspace in navigator.LoadedWorkspaces.ToArray()) workspace.Close();
                Require(!navigator.LoadedWorkspaces.Any(), "Disposed workspaces remain registered for navigation.");
                owner.Close();
            }
        }

        private static void CheckCloseLifecycle()
        {
            TestPage first = null;
            TestPage second = null;
            var tasks = new[]
            {
                Task("comparison.first", WorkspaceKind.Comparison, () => first = new TestPage("first")),
                Task("comparison.second", WorkspaceKind.Comparison, () => second = new TestPage("second")),
            };
            using (var workspace = new WorkspaceForm(WorkspaceKind.Comparison, tasks))
            {
                workspace.ShowInTaskbar = false;
                workspace.Show();
                workspace.OpenTask("comparison.first");
                first.Draft.Text = "Must survive a refused close";
                workspace.OpenTask("comparison.second");
                second.VetoClose = true;
                workspace.OpenTask("comparison.first");
                workspace.Close();
                Require(!workspace.IsDisposed && !first.IsDisposed && !second.IsDisposed &&
                    first.ClosedCount == 0 && second.ClosedCount == 0 && workspace.LoadedPages.Count == 2,
                    "A later page's close veto discarded earlier pages.");
                Require(first.Draft.Text == "Must survive a refused close", "A refused close reset page input.");
                second.VetoClose = false;
                second.ThrowOnClose = true;
                Require(!workspace.TryPrepareWorkspaceClose(CloseReason.UserClosing) &&
                    !first.IsDisposed && !second.IsDisposed && first.ClosedCount == 0,
                    "An exception in a close guard discarded a page or authorised shutdown.");
                second.ThrowOnClose = false;
                workspace.OpenTask("comparison.second");
                second.VetoClose = true;
                workspace.CloseActivePage();
                Require(ReferenceEquals(second, workspace.ActivePage) && !second.IsDisposed,
                    "Closing an individual page bypassed its close guard.");
                second.VetoClose = false;
                workspace.Close();
                Require(workspace.IsDisposed && first.IsDisposed && second.IsDisposed,
                    "An accepted close left hosted pages alive.");
                Require(first.ClosedCount == 1 && second.ClosedCount == 1,
                    "A completed workspace close raised duplicate or missing FormClosed events.");
            }

            using (var unshown = new TestPage("unshown"))
            {
                unshown.VetoClose = true;
                Require(!unshown.IsHandleCreated && !unshown.PrepareWorkspaceClose(CloseReason.UserClosing) &&
                    !unshown.IsDisposed, "Preflight disposed a handle-less page instead of respecting its guard.");
                unshown.VetoClose = false;
                Require(unshown.PrepareWorkspaceClose(CloseReason.UserClosing), "An idle handle-less page refused preflight.");
                unshown.CompleteWorkspaceClose();
                Require(unshown.IsDisposed, "Completing an accepted handle-less close did not release the page.");
            }
        }

        private static void CheckTransientPagesAndFailedFactory()
        {
            var tasks = new[]
            {
                Task("comparison.good", WorkspaceKind.Comparison, () => new TestPage("good")),
                Task("comparison.invalid", WorkspaceKind.Comparison,
                    () => { throw new InvalidOperationException("Synthetic page construction failure"); }),
            };
            using (var workspace = new WorkspaceForm(WorkspaceKind.Comparison, tasks))
            {
                workspace.ShowInTaskbar = false;
                workspace.Show();
                workspace.OpenTask("comparison.good");
                var good = (TestPage)workspace.ActivePage;
                good.Draft.Text = "Existing input";
                try { workspace.OpenTask("comparison.invalid"); }
                catch (InvalidOperationException) { }
                Require(workspace.LoadedPages.Count == 1 && ReferenceEquals(workspace.ActivePage, good) &&
                    !good.IsDisposed && good.Draft.Text == "Existing input",
                    "A failed new page replaced or disposed the existing page.");
                workspace.OpenTransient("Advanced mapping", () => new TestPage("mapping one"));
                var first = (TestPage)workspace.ActivePage;
                first.Draft.Text = "First source/target snapshot";
                workspace.OpenTransient("Advanced mapping", () => new TestPage("mapping two"));
                var second = (TestPage)workspace.ActivePage;
                Require(!ReferenceEquals(first, second) && first.Draft.Text == "First source/target snapshot" &&
                    !first.IsDisposed && workspace.LoadedPages.Count == 3,
                    "Opening another advanced mapping session replaced an existing dataset context.");
                workspace.CloseActivePage();
                Require(second.IsDisposed && !first.IsDisposed && !good.IsDisposed,
                    "Closing one mapping session disposed unrelated pages.");
                workspace.Close();
            }
        }

        private static void CheckOwnerClose()
        {
            TestPage protectedPage = null;
            var tasks = new[]
            {
                Task("comparison.owner", WorkspaceKind.Comparison,
                    () => protectedPage = new TestPage("owner protected") { VetoClose = true }),
            };
            var navigator = new WorkspaceNavigator(tasks);
            using (var main = new MainForm())
            {
                main.ShowInTaskbar = false;
                main.Show();
                navigator.Open(main, "comparison.owner");
                var workspace = navigator.LoadedWorkspaces.Single();
                main.Close();
                Require(!main.IsDisposed && !workspace.IsDisposed && !protectedPage.IsDisposed,
                    "Closing the main owner bypassed a hosted page's shutdown veto.");
                var exit = new CancelEventArgs();
                Application.Exit(exit);
                Require(exit.Cancel && !main.IsDisposed && !workspace.IsDisposed && !protectedPage.IsDisposed &&
                    protectedPage.ClosedCount == 0,
                    "Application.Exit bypassed the hosted page's shutdown veto or closed other pages first.");
                protectedPage.VetoClose = false;
                workspace.Close();
                main.Close();
            }
        }

        private static void CheckAdvancedMappingCounts()
        {
            using (var source = new DataSet())
            using (var target = new DataSet())
            {
                var left = new DataTable("Source");
                left.Columns.Add("ID");
                left.Columns.Add("Value");
                left.Rows.Add("1", "left one");
                left.Rows.Add("2", "left two");
                source.Tables.Add(left);
                var unrelated = new DataTable("Not the target");
                unrelated.Columns.Add("Ignored");
                unrelated.Rows.Add("This table must not supply the displayed count");
                target.Tables.Add(unrelated);
                var right = new DataTable("Actual target");
                right.Columns.Add("TargetID");
                right.Columns.Add("Value");
                right.Columns.Add("Remark");
                right.Rows.Add("1", "right one", "A");
                right.Rows.Add("2", "right two", "B");
                right.Rows.Add("3", "right three", "C");
                target.Tables.Add(right);
                using (var mapping = new ViewForMultiReference(source, target))
                {
                    var count = mapping.Controls.Find("lbl_Target", true).OfType<Label>().Single();
                    Require(count.Text == "Rows : 3 and Columns : 3",
                        "Advanced mapping counts describe the unrelated first target sheet.");
                    var columns = mapping.Controls.Find("clb_Target", true).OfType<CheckedListBox>().Single();
                    Require(columns.Items.Count == 3 && columns.Items[0].ToString() == "TargetID",
                        "Advanced mapping no longer uses the intended target worksheet columns.");
                }
            }
        }

        private static void CheckHiddenWorker()
        {
            var release = new ManualResetEvent(false);
            BusyTestPage busy = null;
            var tasks = new[]
            {
                Task("comparison.busy", WorkspaceKind.Comparison, () => busy = new BusyTestPage(release)),
                Task("comparison.other", WorkspaceKind.Comparison, () => new TestPage("other")),
            };
            using (var workspace = new WorkspaceForm(WorkspaceKind.Comparison, tasks))
            {
                try
                {
                    workspace.ShowInTaskbar = false;
                    workspace.Show();
                    workspace.OpenTask("comparison.busy");
                    busy.StartOperation();
                    workspace.OpenTask("comparison.other");
                    Require(busy.IsBusy && UpdateInstaller.HasBusyWorker(workspace),
                        "A hidden page's worker was not found through the workspace.");
                    Require(!workspace.TryPrepareWorkspaceClose(CloseReason.UserClosing) &&
                        !busy.IsDisposed && workspace.LoadedPages.Count == 2,
                        "A hidden busy page was discarded while its worker was running.");
                }
                finally
                {
                    release.Set();
                    if (busy != null) PumpUntil(() => !busy.IsBusy, "The synthetic background worker failed to stop.");
                    workspace.Close();
                    release.Dispose();
                }
            }
        }

        private static void CheckUpdaterPolicy()
        {
            using (var settings = new WorkspaceForm(WorkspaceKind.Settings, WorkspaceCatalog.TasksFor(WorkspaceKind.Settings)))
            using (var comparison = new WorkspaceForm(WorkspaceKind.Comparison, WorkspaceCatalog.TasksFor(WorkspaceKind.Comparison)))
            {
                settings.ShowInTaskbar = false;
                settings.Show();
                settings.OpenTask("settings.preferences");
                var preferences = (User_Settings)settings.ActivePage;
                preferences.ShowUpdates();
                string reason;
                Require(UpdateInstaller.CanInstallForms(new Form[] { settings, preferences }, out reason),
                    "The Settings workspace prevents use of its own updater.");
                Require(preferences.AcceptButton == null && settings.AcceptButton == null,
                    "The update workspace has an implicit Enter-button action.");
                settings.OpenTask("settings.profiles");
                var profile = settings.ActivePage;
                settings.OpenTask("settings.preferences");
                Require(!UpdateInstaller.CanInstallForms(new Form[] { settings, preferences }, out reason) &&
                    !string.IsNullOrWhiteSpace(reason), "An inactive but loaded Profiles page did not block installation.");
                profile.Close();
                Require(UpdateInstaller.CanInstallForms(new Form[] { settings, preferences }, out reason),
                    "Closing Profiles left Settings permanently ineligible for updating.");
                Require(!UpdateInstaller.CanInstallForms(new Form[] { settings, comparison, preferences }, out reason),
                    "An open non-Settings workspace failed to block installation.");
                settings.Close();
                comparison.Close();
            }
        }

        private static void CheckShellUpdaterPolicy()
        {
            using (var shell = new WorkspaceForm(WorkspaceCatalog.Tasks, "SPI Data Comparison"))
            {
                shell.ShowInTaskbar = false;
                shell.Show();
                string reason;
                Require(UpdateInstaller.CanInstallForms(new Form[] { shell }, out reason),
                    "The empty rail shell blocked installation because of its first task's workspace kind.");
                shell.OpenTask("settings.preferences");
                var preferences = (User_Settings)shell.ActivePage;
                var about = AllControls(preferences).OfType<AboutBox1>().SingleOrDefault();
                Require(about != null, "Settings has no embedded About page.");
                Require(UpdateInstaller.CanInstallForms(new Form[] { shell, preferences, about }, out reason),
                    "Settings with its embedded About page prevented use of its own updater: " + reason);
                shell.OpenTask("comparison.databases");
                var comparison = shell.ActivePage;
                shell.OpenTask("settings.preferences");
                Require(!UpdateInstaller.CanInstallForms(new Form[] { shell, preferences, about }, out reason) &&
                    !string.IsNullOrWhiteSpace(reason), "A loaded but hidden data task did not block installation.");
                comparison.Close();
                Require(UpdateInstaller.CanInstallForms(new Form[] { shell, preferences, about }, out reason),
                    "Closing the data task left the shell permanently ineligible for updating.");
                shell.Close();
            }
        }

        private static void CheckSessionMappingNavigation()
        {
            var tasks = new[] { Task("comparison.session", WorkspaceKind.Comparison, () => new TestPage("session")) };
            using (var session = new WorkspaceForm(WorkspaceKind.Comparison, tasks))
            {
                session.ShowInTaskbar = false;
                session.ShowNavigation = false;
                session.Show();
                session.OpenTask("comparison.session");
                var page = (TestPage)session.ActivePage;
                Require(!session.ShowNavigation, "A New session window showed a rail.");
                var mapping = (TestPage)session.OpenTransient("Advanced mapping", () => new TestPage("mapping"));
                Require(session.ShowNavigation && session.Controls.Find("task_transient_1", true).Length == 1 &&
                    session.Controls.Find("task_comparison_session", true).Length == 1,
                    "A New session window hid the way back to its advanced-mapping snapshot.");
                session.Controls.Find("task_comparison_session", true).OfType<Button>().Single().PerformClick();
                Require(ReferenceEquals(page, session.ActivePage), "The session's own task could not be reselected.");
                session.Controls.Find("task_transient_1", true).OfType<Button>().Single().PerformClick();
                Require(ReferenceEquals(mapping, session.ActivePage), "The mapping snapshot could not be revisited.");
                mapping.Close();
                Require(!session.ShowNavigation, "The session list stayed after its last mapping snapshot closed.");
                session.Close();
            }
        }

        private class TestPage : WorkspacePageForm
        {
            internal readonly TextBox Draft = new TextBox { Dock = DockStyle.Top };
            internal bool VetoClose;
            internal bool ThrowOnClose;
            internal int ClosedCount;

            internal TestPage(string name)
            {
                Name = name;
                Controls.Add(Draft);
                FormClosing += (sender, args) =>
                {
                    if (ThrowOnClose) throw new InvalidOperationException("Synthetic close-guard failure");
                    if (VetoClose) args.Cancel = true;
                };
                FormClosed += (sender, args) => ClosedCount++;
            }
        }

        private sealed class BusyTestPage : TestPage
        {
            private readonly BackgroundWorker worker = new BackgroundWorker();
            internal bool IsBusy => worker.IsBusy;

            internal BusyTestPage(ManualResetEvent release) : base("busy")
            {
                worker.DoWork += (sender, args) => release.WaitOne(15000);
                FormClosing += (sender, args) => { if (worker.IsBusy) args.Cancel = true; };
            }

            internal void StartOperation() { worker.RunWorkerAsync(); }

            protected override void Dispose(bool disposing)
            {
                if (disposing) worker.Dispose();
                base.Dispose(disposing);
            }
        }
    }
}
