using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// The main window's part of updating: the version and channel in the status line, the check at start-up,
    /// the "Update to v…" entry at the right end of the menu bar (only there when an update exists), and the
    /// install of a waiting update when the app closes.
    /// </summary>
    public partial class MainForm
    {
        private ToolStripMenuItem updateMenuItem;
        private ReleaseInfo availableUpdate;

        private void InitializeUpdates()
        {
            updateMenuItem = new ToolStripMenuItem
            {
                Name = "updateToolStripMenuItem",
                Alignment = ToolStripItemAlignment.Right,
                Visible = false,
                Font = new Font(menuStrip1.Font, FontStyle.Bold),
                ForeColor = UiTheme.Accent,
            };
            updateMenuItem.Click += (s, e) => ShowUpdateDialog();
            menuStrip1.Items.Add(updateMenuItem);

            ShowVersionAndChannel();
            UpdateService.SettingsChanged += UpdateSettingsChanged;
            FormClosed += (s, e) => UpdateService.SettingsChanged -= UpdateSettingsChanged;
            FormClosing += (s, e) => InstallWaitingUpdate();
            Shown += (s, e) => CheckForUpdate();
        }

        private void ShowVersionAndChannel()
        {
            lbl_Version.Text = "v" + AppVersion.Full + "  ·  " + UpdateService.ChannelName(UpdateSettings.Load().Channel) + " channel";
        }

        private void UpdateSettingsChanged(object sender, EventArgs e)
        {
            if (IsDisposed || !IsHandleCreated) return;
            BeginInvoke((Action)(() =>
            {
                ShowVersionAndChannel();
                CheckForUpdate();
            }));
        }

        /// <summary>Runs at every start, in the background; a failed check (offline, GitHub busy) stays silent.</summary>
        private async void CheckForUpdate()
        {
            if (SelfTest.IsRunning) return;   // build checks open this window; they must not go online
            UpdateSettings settings = UpdateSettings.Load();
            try
            {
                ReleaseInfo found = await Task.Run(() => UpdateService.FindUpdate(settings.Channel));
                if (IsDisposed) return;
                availableUpdate = found;
                updateMenuItem.Visible = found != null;
                if (found == null) return;
                updateMenuItem.Text = "Update to v" + found.Version;
                updateMenuItem.ToolTipText = found.IsBeta ? "A beta build is available" : "A new version is available";
                if (settings.InstallSilently && UpdateService.PendingFile == null)
                {
                    string file = await Task.Run(() => UpdateService.Download(found, null));
                    if (IsDisposed) return;
                    UpdateService.PendingFile = file;
                    updateMenuItem.ToolTipText = "Downloaded - installs when you close the app";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Update check failed: " + ex.Message);
            }
        }

        private void ShowUpdateDialog()
        {
            if (availableUpdate == null) return;
            using (var dialog = new UpdateDialog(availableUpdate))
            {
                dialog.ShowDialog(this);
            }
        }

        private void InstallWaitingUpdate()
        {
            string file = UpdateService.PendingFile;
            if (file == null) return;
            UpdateService.PendingFile = null;
            try
            {
                UpdateService.Install(file, restartAfterwards: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "The downloaded update could not be installed: " + ex.Message, "Update not installed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
