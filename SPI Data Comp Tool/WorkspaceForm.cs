using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>A lazy task workspace. Switching tasks never discards their inputs or running jobs.</summary>
    internal sealed class WorkspaceForm : Form
    {
        private readonly Dictionary<string, WorkspaceTask> tasks;
        private readonly Dictionary<string, WorkspacePageForm> pages = new Dictionary<string, WorkspacePageForm>();
        private readonly Dictionary<string, Button> taskButtons = new Dictionary<string, Button>();
        private readonly FlowLayoutPanel navigation;
        private readonly Panel content;
        private readonly Label description;
        private readonly Label status;
        private readonly Label empty;
        private readonly Label heading;
        private readonly string shellTitle;
        private readonly TableLayoutPanel layout;
        private readonly Button closeTask;
        private readonly Button separateTask;
        private string activeKey;
        private int transientNumber;
        private bool completingClose;
        private bool forwardingDialogKey;

        // A parameterless constructor keeps the existing offline --open-form check meaningful.
        public WorkspaceForm() : this(WorkspaceKind.Comparison, WorkspaceCatalog.TasksFor(WorkspaceKind.Comparison)) { }

        internal WorkspaceForm(WorkspaceKind workspace, IEnumerable<WorkspaceTask> definitions)
            : this(workspace, definitions.Where(item => item.Workspace == workspace).ToList(), WorkspaceCatalog.Title(workspace), false)
        {
        }

        /// <summary>
        /// The application shell's rail: every task of every group, each listed once under its group name.
        /// </summary>
        internal WorkspaceForm(IEnumerable<WorkspaceTask> definitions, string title)
            : this(definitions.First().Workspace, definitions.ToList(), title, true)
        {
        }

        private WorkspaceForm(WorkspaceKind workspace, List<WorkspaceTask> orderedTasks, string title, bool grouped)
        {
            Workspace = workspace;
            Grouped = grouped;
            shellTitle = title;
            tasks = orderedTasks.ToDictionary(item => item.Key);
            Text = title + " — SPI";
            Name = "Workspace_" + workspace;
            Font = new Font("Segoe UI", 9F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1320, 820);
            MinimumSize = new Size(1000, 640);
            StartPosition = FormStartPosition.CenterParent;

            var root = layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(12) };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 235F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, grouped ? 70F : 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            var header = new TableLayoutPanel { Name = "workspaceHeader", Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
                Padding = new Padding(12, 0, 12, 0) };
            header.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            header.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            header.Controls.Add(heading = new Label { Name = "workspaceHeading", Text = title, Font = new Font(Font, FontStyle.Bold),
                Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, UseMnemonic = false, AccessibleName = "Workspace name" }, 0, 0);
            description = new Label { Name = "workspaceDescription", Dock = DockStyle.Fill, AutoEllipsis = true,
                Text = "Choose a task. Inputs and results stay with their own page when you switch." };
            header.Controls.Add(description, 0, 1);
            root.Controls.Add(header, 0, 0);
            root.SetColumnSpan(header, 2);

            navigation = new FlowLayoutPanel { Name = "workspaceNavigation", Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
                WrapContents = false, AutoScroll = true, Padding = new Padding(0, 0, 10, 0), AccessibleName = "Workspace tasks" };
            root.Controls.Add(navigation, 0, 1);
            content = new Panel { Name = "workspaceContent", Dock = DockStyle.Fill, Padding = Padding.Empty };
            empty = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter,
                Text = "Select a task on the left.\r\nOnly the task you select is opened; no data is loaded automatically." };
            content.Controls.Add(empty);
            root.Controls.Add(content, 1, 1);

            var footer = new TableLayoutPanel { Name = "workspaceFooter", Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            status = new Label { Name = "workspaceStatus", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true, Text = "Ready. Each task keeps independent inputs and results." };
            closeTask = new Button { Name = "workspaceCloseTask", Text = "Close task", AutoSize = true, Enabled = false, Height = 32 };
            // The only way to open a second, independent copy of a task (no Ctrl+click shortcut).
            separateTask = new Button { Name = "workspaceSeparateTask", Text = "New session", AutoSize = true, Enabled = false, Height = 32 };
            closeTask.Click += (sender, args) => CloseActivePage();
            separateTask.Click += (sender, args) =>
            {
                WorkspaceTask task;
                if (activeKey != null && tasks.TryGetValue(activeKey, out task) && task.AllowSeparateSession && NavigateTask != null)
                    NavigateSafely(task, true);
            };
            footer.Controls.Add(status, 0, 0);
            footer.Controls.Add(separateTask, 1, 0);
            footer.Controls.Add(closeTask, 2, 0);
            root.Controls.Add(footer, 0, 2);
            root.SetColumnSpan(footer, 2);
            Controls.Add(root);
            WorkspaceKind? group = null;
            foreach (var task in orderedTasks)
            {
                if (grouped && group != task.Workspace)
                {
                    group = task.Workspace;
                    navigation.Controls.Add(new Label
                    {
                        Name = "group_" + task.Workspace, Text = WorkspaceCatalog.Title(task.Workspace), AutoSize = false, Width = 205,
                        Height = 26, Margin = new Padding(2, navigation.Controls.Count == 0 ? 0 : 10, 0, 2),
                        TextAlign = ContentAlignment.BottomLeft, Font = new Font(Font, FontStyle.Bold), UseMnemonic = false,
                        AccessibleRole = AccessibleRole.StaticText,
                    });
                }
                AddTaskButton(task);
            }
            UiTheme.MarkHeader(header);
            UiTheme.MarkStatus(footer);
            UiTheme.Apply(this);
            UiTheme.Changed += Theme_Changed;
            Disposed += (sender, args) => UiTheme.Changed -= Theme_Changed;
            ApplyNavigationTheme();
            // Never install a workspace-wide AcceptButton: an Enter key must not start a job/update.
        }

        internal WorkspaceKind Workspace { get; }

        /// <summary>True for the application shell, whose rail lists every group.</summary>
        internal bool Grouped { get; }

        private bool navigationWanted = true;

        /// <summary>A "New session" window holds one task and has no rail of its own. Its own advanced-mapping
        /// snapshots still need a way back, so the list reappears (with only this window's pages) while one is open.</summary>
        internal bool ShowNavigation
        {
            get { return navigation.Visible; }
            set
            {
                navigationWanted = value;
                ApplyNavigation();
            }
        }

        private void ApplyNavigation()
        {
            bool show = navigationWanted || tasks.Keys.Any(key => key.StartsWith("transient.", StringComparison.Ordinal));
            navigation.Visible = show;
            layout.ColumnStyles[0].Width = show ? 235F : 0F;
        }
        internal Func<WorkspaceTask, bool, WorkspacePageForm> NavigateTask { get; set; }
        internal IReadOnlyList<WorkspacePageForm> LoadedPages => pages.Values.Where(page => !page.IsDisposed).ToList().AsReadOnly();
        internal WorkspacePageForm ActivePage { get; private set; }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            FitToWorkingArea();
        }

        private void FitToWorkingArea()
        {
            if (!TopLevel) return;   // the shell is sized by the main window
            // Preserve the grouped windows' screen bounds after initial scaling and on task changes.
            Rectangle area = Screen.FromControl(this).WorkingArea;
            MinimumSize = new Size(Math.Min(MinimumSize.Width, area.Width), Math.Min(MinimumSize.Height, area.Height));
            if (WindowState != FormWindowState.Normal) return;
            Size = new Size(Math.Min(Width, area.Width), Math.Min(Height, area.Height));
            Location = new Point(Math.Max(area.Left, Math.Min(Left, area.Right - Width)),
                Math.Max(area.Top, Math.Min(Top, area.Bottom - Height)));
        }

        private void AddTaskButton(WorkspaceTask task)
        {
            var button = new Button { Name = "task_" + task.Key.Replace('.', '_'), Text = task.Title, Width = 205, Height = Grouped ? 38 : 52,
                Margin = new Padding(0, 0, 0, Grouped ? 3 : 7), TextAlign = ContentAlignment.MiddleLeft,
                AccessibleName = task.Title, AccessibleDescription = task.Description, UseMnemonic = false,
                FlatStyle = FlatStyle.Flat, UseVisualStyleBackColor = false, BackColor = UiTheme.CardBack,
                ForeColor = UiTheme.TextColor, Cursor = Cursors.Hand };
            button.FlatAppearance.BorderColor = UiTheme.Border;
            button.Click += (sender, args) => NavigateSafely(task, false);
            taskButtons.Add(task.Key, button);
            navigation.Controls.Add(button);
            // The theme's ControlAdded handler styles new transient buttons before selection colours.
            ApplyNavigationTheme();
        }

        private void Theme_Changed(object sender, EventArgs e)
        {
            if (!IsDisposed && !Disposing) ApplyNavigationTheme();
        }

        private void ApplyNavigationTheme()
        {
            UiPalette palette = UiTheme.Current;
            foreach (var pair in taskButtons)
            {
                Button button = pair.Value;
                if (button.IsDisposed) continue;
                bool selected = pair.Key == activeKey;
                button.BackColor = selected ? palette.Accent : palette.CardBack;
                button.ForeColor = selected ? palette.AccentText : palette.TextColor;
                button.FlatAppearance.BorderColor = selected ? palette.Accent : palette.FieldBorder;
                button.FlatAppearance.MouseOverBackColor = selected ? palette.AccentHover : palette.HoverBack;
                button.FlatAppearance.MouseDownBackColor = selected ? palette.AccentPressed : palette.PressedBack;
                button.Invalidate();
            }
        }

        private void NavigateSafely(WorkspaceTask task, bool separate)
        {
            try
            {
                if (!task.Key.StartsWith("transient.", StringComparison.Ordinal) &&
                    NavigateTask != null && (!task.AllowSeparateSession || separate))
                    NavigateTask(task, separate && task.AllowSeparateSession);
                else OpenTask(task.Key);
            }
            catch (Exception ex)
            {
                status.Text = Common_Data.RedactConnectionCredentials("Unable to open task: " + ex.Message);
            }
        }

        internal WorkspacePageForm OpenTask(string key)
        {
            WorkspaceTask task;
            if (!tasks.TryGetValue(key, out task)) throw new ArgumentException("This task is not in the workspace.", nameof(key));
            WorkspacePageForm page;
            if (!pages.TryGetValue(key, out page) || page.IsDisposed)
            {
                // Build/initialize a replacement before hiding the previous page.
                page = task.CreatePage();
                if (page == null) throw new InvalidOperationException("The task did not create a page.");
                try
                {
                    // Register the page before parenting: generic recursive control styling must
                    // not make an unregistered legacy Form look as though it registered itself.
                    UiTheme.EnsureApplied(page);
                    // Some standalone workflows start maximized. A child page must use normal
                    // window state before parenting; maximizing it under an unshown workspace
                    // can prevent its native window from becoming visible on the first open.
                    page.WindowState = FormWindowState.Normal;
                    page.TopLevel = false;
                    page.FormBorderStyle = FormBorderStyle.None;
                    page.ShowInTaskbar = false;
                    page.MinimumSize = Size.Empty;
                    page.Dock = DockStyle.Fill;
                    page.AutoScroll = true;
                    page.HostWorkspace = this;
                    pages[key] = page;
                    content.Controls.Add(page);
                    WorkspacePageForm captured = page;
                    page.FormClosed += (sender, args) => RemovePage(key, captured);
                    page.Disposed += (sender, args) => RemovePage(key, captured);
                    page.Show();
                    if (page.IsDisposed) throw new InvalidOperationException("The task closed during initialization.");
                }
                catch
                {
                    pages.Remove(key);
                    content.Controls.Remove(page);
                    page.Dispose();
                    throw;
                }
            }
            SelectPage(key, page);
            if (Visible) FitToWorkingArea();
            return page;
        }

        private void SelectPage(string key, WorkspacePageForm page)
        {
            foreach (var other in pages.Values.ToArray()) if (!other.IsDisposed) other.Visible = ReferenceEquals(other, page);
            ActivePage = page;
            activeKey = key;
            empty.Visible = false;
            page.BringToFront();
            page.Select();
            WorkspaceTask task = tasks[key];
            description.Text = task.Description;
            if (Grouped) heading.Text = task.Title;
            closeTask.Enabled = true;
            separateTask.Enabled = task.AllowSeparateSession && NavigateTask != null;
            ApplyNavigationTheme();
            status.Text = pages.Count + " open task(s). Switching tasks keeps their current state.";
        }

        internal WorkspacePageForm OpenTransient(string title, Func<WorkspacePageForm> create)
        {
            string key = "transient." + (++transientNumber);
            var task = new WorkspaceTask(key, title + " " + transientNumber,
                "Independent snapshot for advanced mapping. Close this task when its results are no longer needed.", Workspace, create, false);
            tasks.Add(key, task);
            AddTaskButton(task);
            ApplyNavigation();
            try { return OpenTask(key); }
            catch
            {
                tasks.Remove(key);
                RemoveTaskButton(key);
                ApplyNavigation();
                throw;
            }
        }

        private void RemoveTaskButton(string key)
        {
            Button button;
            if (!taskButtons.TryGetValue(key, out button)) return;
            taskButtons.Remove(key);
            navigation.Controls.Remove(button);
            button.Dispose();
        }

        private void RemovePage(string key, WorkspacePageForm page)
        {
            WorkspacePageForm current;
            if (!pages.TryGetValue(key, out current) || !ReferenceEquals(current, page)) return;
            pages.Remove(key);
            page.HostWorkspace = null;
            if (key.StartsWith("transient.", StringComparison.Ordinal))
            {
                tasks.Remove(key);
                if (!completingClose && !IsDisposed && !Disposing)
                {
                    RemoveTaskButton(key);
                    ApplyNavigation();
                }
            }
            if (completingClose || IsDisposed || Disposing) return;
            if (ReferenceEquals(ActivePage, page))
            {
                ActivePage = null;
                activeKey = null;
                var replacement = pages.FirstOrDefault(pair => !pair.Value.IsDisposed);
                if (replacement.Value != null) SelectPage(replacement.Key, replacement.Value);
                else
                {
                    empty.Visible = true;
                    empty.BringToFront();
                    closeTask.Enabled = false;
                    separateTask.Enabled = false;
                    description.Text = "Choose a task to continue.";
                    heading.Text = shellTitle;
                    status.Text = "No open tasks. Select a task on the left.";
                    ApplyNavigationTheme();
                }
            }
        }

        internal void CloseActivePage()
        {
            // A normal page close still runs its own guards and only removes that page if accepted.
            try { if (ActivePage != null && !ActivePage.IsDisposed) ActivePage.Close(); }
            catch (Exception ex)
            {
                status.Text = Common_Data.RedactConnectionCredentials("Unable to close task safely: " + ex.Message);
            }
        }

        internal bool TryPrepareWorkspaceClose(CloseReason reason)
        {
            foreach (var pair in pages.ToArray())
            {
                var page = pair.Value;
                if (page.IsDisposed) continue;
                try
                {
                    if (page.PrepareWorkspaceClose(reason)) continue;
                    SelectPage(pair.Key, page);
                    status.Text = "This task is not ready to close. Finish or cancel its operation first.";
                    return false;
                }
                catch (Exception ex)
                {
                    SelectPage(pair.Key, page);
                    status.Text = Common_Data.RedactConnectionCredentials("Unable to close task safely: " + ex.Message);
                    return false;
                }
            }
            return true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (!e.Cancel && !TryPrepareWorkspaceClose(e.CloseReason)) e.Cancel = true;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (completingClose) return;
            completingClose = true;
            var snapshot = pages.Values.ToArray();
            foreach (var page in snapshot) page.CompleteWorkspaceClose();
            base.OnFormClosed(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (!forwardingDialogKey && ActivePage != null && !ActivePage.IsDisposed &&
                (key == Keys.Enter || key == Keys.Escape))
            {
                forwardingDialogKey = true;
                try { if (ActivePage.ProcessWorkspaceDialogKey(keyData)) return true; }
                finally { forwardingDialogKey = false; }
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}
