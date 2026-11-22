using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Compatibility checks for the two retained formerly grouped tools, hosted by task
    /// workspaces. Uses real controls without running database, import or export actions.
    /// Run by "SPI Data Comp Tool.exe" --check-groups OUT_FILE (see SelfTest.cs).
    /// </summary>
    internal static class ToolGroupChecks
    {
        public static int Run(string outFile)
        {
            var lines = new List<string>();
            int failed = 0;
            var expected = new Dictionary<WorkspaceKind, Dictionary<string, Type>>
            {
                { WorkspaceKind.DataReports, new Dictionary<string, Type>
                    {
                        { "reports.export", typeof(CompareModePage) },
                        { "reports.logs", typeof(MergeLogsExport) },
                    }
                },
            };
            foreach (var group in expected)
            {
                try
                {
                    CheckRoutes(group.Key, group.Value);
                    lines.Add("ok   existing grouped tools in " + WorkspaceCatalog.Title(group.Key));
                }
                catch (Exception ex)
                {
                    failed++;
                    lines.Add("FAIL " + group.Key + ": " + ex.Message.Replace("\r", " ").Replace("\n", " "));
                }
            }
            try
            {
                BusyToolKeepsEveryPage();
                lines.Add("ok   a hidden busy tool preserves every page and its input until close succeeds");
            }
            catch (Exception ex)
            {
                failed++;
                lines.Add("FAIL busy tool close: " + ex.Message.Replace("\r", " ").Replace("\n", " "));
            }
            lines.Add(failed == 0 ? "All grouped-tool compatibility checks passed." : failed + " grouped-tool compatibility check(s) failed.");
            File.WriteAllLines(outFile, lines);
            return failed == 0 ? 0 : 1;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void RequireFitsScreen(WorkspaceForm window)
        {
            var screen = Screen.FromControl(window).WorkingArea;
            Require(window.Width <= screen.Width && window.Height <= screen.Height
                && window.MinimumSize.Width <= screen.Width && window.MinimumSize.Height <= screen.Height,
                "The workspace or its minimum size is larger than the screen.");
        }

        private static void CheckRoutes(WorkspaceKind kind, Dictionary<string, Type> expected)
        {
            var tools = new Dictionary<string, WorkspacePageForm>();
            using (var window = new WorkspaceForm(kind, WorkspaceCatalog.TasksFor(kind)))
            {
                Require(window.Workspace == kind && window.Text.Contains(WorkspaceCatalog.Title(kind)),
                    "The tools have the wrong workspace host or title.");
                Require(window.LoadedPages.Count == 0, "The workspace constructed tools before a task was selected.");
                // Force a too-large minimum even on large CI screens: the host must lower the
                // minimum before clamping its outer size, or Windows silently grows it again.
                var screen = Screen.FromControl(window).WorkingArea;
                window.MinimumSize = new System.Drawing.Size(screen.Width + 100, screen.Height + 100);
                window.Size = window.MinimumSize;
                window.ShowInTaskbar = false;
                window.Show();
                Application.DoEvents();
                RequireFitsScreen(window);
                Require(window.LoadedPages.Count == 0, "Showing the workspace eagerly created tools.");
                foreach (var route in expected)
                {
                    Require(WorkspaceCatalog.Find(route.Key).Workspace == kind, route.Key + " moved to the wrong workspace.");
                    WorkspacePageForm tool = window.OpenTask(route.Key);
                    Application.DoEvents();
                    Require(tool != null && !tool.IsDisposed && tool.GetType() == route.Value,
                        route.Key + " did not build the expected tool.");
                    Require(ReferenceEquals(tool.HostWorkspace, window) && ReferenceEquals(window.ActivePage, tool)
                        && !tool.TopLevel && tool.Parent != null && tool.FormBorderStyle == FormBorderStyle.None
                        && tool.Dock == DockStyle.Fill && tool.Visible, route.Key + " is not shown inside its workspace.");
                    Require(tool.BackColor == UiTheme.Current.WindowBack, route.Key + " is not in the colour theme.");
                    RequireFitsScreen(window);
                    tools.Add(route.Key, tool);
                    Require(window.LoadedPages.Count == tools.Count, "Selecting a task created an unrelated tool.");
                }
                Require(tools.Values.Distinct().Count() == tools.Count, "Two routes share one mutable tool instance.");
                foreach (var route in tools)
                {
                    Require(ReferenceEquals(window.OpenTask(route.Key), route.Value)
                        && ReferenceEquals(window.ActivePage, route.Value) && window.LoadedPages.Count == tools.Count,
                        "Returning to " + route.Key + " replaced a tool or failed to activate it.");
                }
                bool unknown = false;
                try { window.OpenTask("no such tool"); }
                catch (ArgumentException) { unknown = true; }
                Require(unknown && window.LoadedPages.Count == tools.Count, "An unknown task changed the loaded tools.");
                window.Close();
                Require(window.IsDisposed, "An idle workspace did not close.");
            }
            Require(tools.Values.All(tool => tool.IsDisposed), "Closing the workspace left a tool open.");
        }

        /// <summary>A stand-in tool with editable input and a cancellable close guard.</summary>
        private sealed class ProbeTool : WorkspacePageForm
        {
            internal readonly TextBox Draft = new TextBox { Dock = DockStyle.Top };
            internal bool Busy;
            internal int ClosedCount;
            internal int DisposedCount;

            internal ProbeTool()
            {
                Controls.Add(Draft);
                FormClosing += (sender, args) => { if (Busy) args.Cancel = true; };
                FormClosed += (sender, args) => ClosedCount++;
                Disposed += (sender, args) => DisposedCount++;
            }
        }

        private static void BusyToolKeepsEveryPage()
        {
            string[] keys = { "reports.first", "reports.busy", "reports.last" };
            var definitions = keys.Select(key => new WorkspaceTask(key, key, "Synthetic close-guard probe",
                WorkspaceKind.DataReports, () => new ProbeTool())).ToArray();
            using (var window = new WorkspaceForm(WorkspaceKind.DataReports, definitions))
            {
                window.ShowInTaskbar = false;
                window.Show();
                var tools = keys.Select(key => (ProbeTool)window.OpenTask(key)).ToArray();
                for (int i = 0; i < tools.Length; i++) tools[i].Draft.Text = "Retained input " + i;
                tools[1].Busy = true;
                window.OpenTask(keys[0]);
                Application.DoEvents();
                Require(!tools[1].Visible, "The refusing tool must start hidden for this regression check.");
                window.Close();
                Application.DoEvents();
                Require(!window.IsDisposed && window.Visible && window.LoadedPages.Count == tools.Length,
                    "The window or one of its pages closed although a hidden tool was busy.");
                Require(ReferenceEquals(window.ActivePage, tools[1]) && tools[1].Visible,
                    "The refusing tool was not made active.");
                for (int i = 0; i < tools.Length; i++)
                {
                    Require(!tools[i].IsDisposed && tools[i].ClosedCount == 0 && tools[i].DisposedCount == 0
                        && tools[i].Draft.Text == "Retained input " + i
                        && ReferenceEquals(window.OpenTask(keys[i]), tools[i]),
                        "A refused close discarded, reset or replaced page " + i + ".");
                }
                tools[1].Busy = false;
                window.Close();
                Application.DoEvents();
                Require(window.IsDisposed && tools.All(tool => tool.IsDisposed
                    && tool.ClosedCount == 1 && tool.DisposedCount == 1),
                    "Successful close must close and dispose every retained page exactly once.");
            }
        }
    }
}
