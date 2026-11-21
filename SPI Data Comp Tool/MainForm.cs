using System;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class MainForm : Form
    {
        private readonly WorkspaceNavigator workspaces = new WorkspaceNavigator();

        public MainForm()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            label3.Text = "Choose a workspace. Each task keeps its own inputs and results. Reopen a task to return to it; hold Ctrl for a separate session.";
            BuildWorkspaceMenus();
            btn_WorkspaceComparison.Click += (sender, e) => OpenTask("comparison.database");
            btn_WorkspaceReports.Click += (sender, e) => OpenTask("reports.database");
            btn_WorkspaceSettings.Click += (sender, e) => OpenTask("settings.preferences");
            lbl_LoginStatus.Text = GlobalDebug.ISGlobalDebug(Environment.UserName, string.Empty) ? "Developer mode" : "Ready";
            InitializeUpdates();
            UiTheme.MarkHeader(pnl_Header);
            UiTheme.MarkStatus(pnl_Status);
            UiTheme.Apply(this);
            foreach (var button in new[] { btn_WorkspaceComparison,
                btn_WorkspaceReports, btn_WorkspaceSettings }) button.UseMnemonic = false;
        }

        private void BuildWorkspaceMenus()
        {
            menuStrip1.Items.Clear();
            var file = new ToolStripMenuItem("&File");
            file.DropDownItems.Add("E&xit", null, (sender, e) => Close());
            menuStrip1.Items.Add(file);
            foreach (WorkspaceKind kind in Enum.GetValues(typeof(WorkspaceKind)))
            {
                var menu = new ToolStripMenuItem(WorkspaceCatalog.Title(kind).Replace("&", "&&"))
                    { Name = "workspaceMenu_" + kind };
                foreach (var task in WorkspaceCatalog.TasksFor(kind))
                {
                    WorkspaceTask route = task;
                    var item = new ToolStripMenuItem(task.Title.Replace("&", "&&"))
                    {
                        Name = "taskMenu_" + task.Key.Replace('.', '_'), Tag = task.Key, ToolTipText = task.Description
                    };
                    item.Click += (sender, e) => OpenTask(route.Key);
                    menu.DropDownItems.Add(item);
                }
                menuStrip1.Items.Add(menu);
            }
            var help = new ToolStripMenuItem("&Help");
            var updates = new ToolStripMenuItem("Check for &updates…") { Name = "checkForUpdatesToolStripMenuItem" };
            updates.Click += (sender, e) =>
            {
                try { ShowUpdatesSettings(); }
                catch (Exception ex) { ShowNavigationError(ex); }
            };
            help.DropDownItems.Add(updates);
            help.DropDownItems.Add("&About", null, (sender, e) =>
            {
                using (var about = new AboutBox1())
                {
                    about.ShowInTaskbar = false;
                    about.ShowDialog(this);
                }
            });
            menuStrip1.Items.Add(help);
        }

        private void OpenTask(string route)
        {
            try { workspaces.Open(this, route, (ModifierKeys & Keys.Control) == Keys.Control); }
            catch (Exception ex) { ShowNavigationError(ex); }
        }

        private void ShowNavigationError(Exception ex)
        {
            lbl_LoginStatus.Text = "Unable to open task";
            MessageBox.Show(this, Common_Data.RedactConnectionCredentials(ex.Message), "Unable to open task",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal User_Settings ShowUpdatesSettings()
        {
            // Help activates the exact same page as Settings > Preferences, rules & updates.
            var settings = (User_Settings)workspaces.Open(this, "settings.preferences");
            settings.ShowUpdates();
            return settings;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (!e.Cancel && (!workspaces.TryPrepareCloseAll(e.CloseReason) ||
                OwnedForms.OfType<WorkspaceForm>().Except(workspaces.LoadedWorkspaces)
                    .Any(workspace => !workspace.TryPrepareWorkspaceClose(e.CloseReason))))
            {
                e.Cancel = true;
                lbl_LoginStatus.Text = "Finish or cancel active tasks before closing SPI.";
            }
        }
    }
}
