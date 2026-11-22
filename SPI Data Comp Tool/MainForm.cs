using System;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// The single application window. Its left rail (every task from <see cref="WorkspaceCatalog"/>,
    /// grouped) is the only way to open a task: there is no menu bar, no home tiles and no Help menu.
    /// About and Updates live in Settings. "New session" is the only way to open a second copy of a task.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly WorkspaceForm shell;
        private readonly WorkspaceNavigator workspaces;

        public MainForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new System.Drawing.Size(1100, 700);
            ClientSize = new System.Drawing.Size(1360, 860);
            lbl_LoginStatus.Text = GlobalDebug.ISGlobalDebug(Environment.UserName, string.Empty) ? "Developer mode" : "Ready";
            InitializeUpdates();

            shell = new WorkspaceForm(WorkspaceCatalog.Tasks, "SPI Data Comparison") { Name = "workspaceShell" };
            workspaces = new WorkspaceNavigator(WorkspaceCatalog.Tasks, OpenInShell);
            shell.NavigateTask = (task, separate) => separate ? workspaces.Open(this, task.Key, true) : shell.OpenTask(task.Key);
            shell.TopLevel = false;
            shell.FormBorderStyle = FormBorderStyle.None;
            shell.ShowInTaskbar = false;
            shell.Dock = DockStyle.Fill;
            pnl_Shell.Controls.Add(shell);
            // The shell's own header names the open task; a second title bar above it would repeat the app name.
            pnl_Header.Visible = false;
            UiTheme.MarkHeader(pnl_Header);
            UiTheme.MarkStatus(pnl_Status);
            UiTheme.Apply(this);
            shell.Show();
        }

        /// <summary>The embedded rail and task host: the one place every task opens.</summary>
        internal WorkspaceForm Shell => shell;

        private WorkspacePageForm OpenInShell(string route)
        {
            return shell.OpenTask(route);
        }

        /// <summary>Opens a task in this window (or, with <paramref name="separateSession"/>, in a new session window).</summary>
        internal WorkspacePageForm OpenTask(string route, bool separateSession = false)
        {
            try { return workspaces.Open(this, route, separateSession); }
            catch (Exception ex)
            {
                ShowNavigationError(ex);
                return null;
            }
        }

        private void ShowNavigationError(Exception ex)
        {
            lbl_LoginStatus.Text = "Unable to open task";
            MessageBox.Show(this, Common_Data.RedactConnectionCredentials(ex.Message), "Unable to open task",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>Settings › Updates: the only place updates are offered.</summary>
        internal User_Settings ShowUpdatesSettings()
        {
            var settings = (User_Settings)shell.OpenTask("settings.preferences");
            settings.ShowUpdates();
            return settings;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (!e.Cancel && (!shell.TryPrepareWorkspaceClose(e.CloseReason) ||
                !workspaces.TryPrepareCloseAll(e.CloseReason) ||
                OwnedForms.OfType<WorkspaceForm>().Except(workspaces.LoadedWorkspaces)
                    .Any(workspace => !workspace.TryPrepareWorkspaceClose(e.CloseReason))))
            {
                e.Cancel = true;
                lbl_LoginStatus.Text = "Finish or cancel active tasks before closing SPI.";
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // The embedded shell is a child, not an owned window: close it so its pages finish once.
            if (!shell.IsDisposed) shell.Close();
            base.OnFormClosed(e);
        }
    }
}
