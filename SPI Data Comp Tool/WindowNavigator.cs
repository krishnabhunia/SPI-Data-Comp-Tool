using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>One default workspace per task group, with explicit independent parallel sessions.</summary>
    internal sealed class WorkspaceNavigator
    {
        private readonly Dictionary<WorkspaceKind, WorkspaceForm> defaults = new Dictionary<WorkspaceKind, WorkspaceForm>();
        private readonly List<WorkspaceForm> workspaces = new List<WorkspaceForm>();
        private readonly Dictionary<string, WorkspaceTask> tasks;
        private readonly List<WorkspaceTask> orderedTasks;

        private readonly Func<string, WorkspacePageForm> openInMain;

        internal WorkspaceNavigator(IEnumerable<WorkspaceTask> definitions = null)
            : this(definitions, null)
        {
        }

        /// <summary>
        /// With <paramref name="openInMain"/> (the main window's shell), every ordinary open goes to that one window
        /// and only "New session" creates a window: one task, no rail of its own, so no second way to reach a task.
        /// </summary>
        internal WorkspaceNavigator(IEnumerable<WorkspaceTask> definitions, Func<string, WorkspacePageForm> openInMain)
        {
            orderedTasks = (definitions ?? WorkspaceCatalog.Tasks).ToList();
            tasks = orderedTasks.ToDictionary(item => item.Key);
            this.openInMain = openInMain;
        }

        internal IReadOnlyList<WorkspaceForm> LoadedWorkspaces => workspaces.Where(window => !window.IsDisposed).ToList().AsReadOnly();

        internal WorkspacePageForm Open(IWin32Window owner, string routeKey, bool separateSession = false)
        {
            WorkspaceTask task;
            if (!tasks.TryGetValue(routeKey, out task)) throw new ArgumentException("Unknown workspace task: " + routeKey, nameof(routeKey));
            bool separate = separateSession && task.AllowSeparateSession;
            if (openInMain != null && !separate) return openInMain(routeKey);
            WorkspaceForm window = null;
            bool created = separate || !defaults.TryGetValue(task.Workspace, out window) || window.IsDisposed;
            if (created)
            {
                if (openInMain != null)
                {
                    window = new WorkspaceForm(task.Workspace, new[] { task });
                    window.ShowNavigation = false;
                    window.NavigateTask = (item, newSession) => newSession ? Open(owner, item.Key, true) : openInMain(item.Key);
                }
                else
                {
                    window = new WorkspaceForm(task.Workspace, orderedTasks.Where(item => item.Workspace == task.Workspace));
                    window.NavigateTask = (item, newSession) => Open(owner, item.Key, newSession);
                }
                WorkspaceForm captured = window;
                workspaces.Add(window);
                if (!separate) defaults[task.Workspace] = window;
                window.FormClosed += (sender, args) =>
                {
                    workspaces.Remove(captured);
                    WorkspaceForm current;
                    if (defaults.TryGetValue(task.Workspace, out current) && ReferenceEquals(current, captured))
                        defaults.Remove(task.Workspace);
                };
            }
            try
            {
                UiTheme.EnsureApplied(window);
                var page = window.OpenTask(routeKey);
                if (!window.Visible)
                {
                    if (owner == null) window.Show();
                    else window.Show(owner);
                }
                if (window.WindowState == FormWindowState.Minimized) window.WindowState = FormWindowState.Normal;
                window.BringToFront();
                window.Activate();
                return page;
            }
            catch
            {
                if (created)
                {
                    workspaces.Remove(window);
                    if (!separate) defaults.Remove(task.Workspace);
                    window.Dispose();
                }
                throw;
            }
        }

        internal bool TryPrepareCloseAll(CloseReason reason)
        {
            // Do not close the first workspace before knowing whether a later one vetoes exit.
            foreach (var workspace in LoadedWorkspaces)
                if (!workspace.TryPrepareWorkspaceClose(reason))
                {
                    if (workspace.WindowState == FormWindowState.Minimized) workspace.WindowState = FormWindowState.Normal;
                    workspace.BringToFront();
                    workspace.Activate();
                    return false;
                }
            return true;
        }
    }
}
