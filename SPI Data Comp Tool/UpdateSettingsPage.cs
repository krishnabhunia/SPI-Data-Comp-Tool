using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Settings › Updates: this version, the update channel, silent install and "check now".</summary>
    internal sealed class UpdateSettingsPage : TabPage
    {
        private readonly RadioButton rbStable = new RadioButton { AutoSize = true, Text = "&Stable (recommended) - normal releases only", Margin = new Padding(0, 2, 0, 2) };
        private readonly RadioButton rbBeta = new RadioButton { AutoSize = true, Text = "&Beta (optional) - also offers builds from open pull requests, named like v2.6.0-beta003", Margin = new Padding(0, 2, 0, 2) };
        private readonly CheckBox cbSilent = new CheckBox { AutoSize = true, Text = "&Download updates in the background and install them when I close the app", Margin = new Padding(0, 8, 0, 2) };
        private readonly Button btnCheck = new Button { Text = "Check for &updates now", AutoSize = true, MinimumSize = new Size(170, 32), Margin = new Padding(0, 10, 0, 4) };
        private readonly Label lblStatus = new Label { AutoSize = true, Margin = new Padding(0, 4, 0, 0) };
        private bool loading;

        public UpdateSettingsPage()
        {
            Text = "Updates";
            Name = "tab_Updates";
            Padding = new Padding(12);
            UseVisualStyleBackColor = true;

            var version = new Label
            {
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 8),
                Text = "This version: " + AppVersion.Display + (UpdateService.IsInstalled ? "  (installed)" : "  (portable)"),
            };
            var channel = new Label { AutoSize = true, Text = "Update channel", Font = new Font(Font, FontStyle.Bold), Margin = new Padding(0, 0, 0, 2) };
            var always = new Label { AutoSize = true, Margin = new Padding(0, 8, 0, 0), Text = "The app checks for updates every time it starts." };

            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
            flow.Controls.AddRange(new Control[] { version, channel, rbStable, rbBeta, always, cbSilent, btnCheck, lblStatus });
            Controls.Add(flow);

            loading = true;
            UpdateSettings settings = UpdateSettings.Load();
            rbStable.Checked = settings.Channel == UpdateChannel.Stable;
            rbBeta.Checked = settings.Channel == UpdateChannel.Beta;
            cbSilent.Checked = settings.InstallSilently;
            loading = false;

            rbStable.CheckedChanged += (s, e) => SaveChoices();
            cbSilent.CheckedChanged += (s, e) => SaveChoices();
            btnCheck.Click += (s, e) => CheckNow();
        }

        private void SaveChoices()
        {
            if (loading) return;
            try
            {
                new UpdateSettings { Channel = rbBeta.Checked ? UpdateChannel.Beta : UpdateChannel.Stable, InstallSilently = cbSilent.Checked }.Save();
                lblStatus.Text = "Saved.";
                UpdateService.NotifySettingsChanged();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Could not save: " + ex.Message;
            }
        }

        private async void CheckNow()
        {
            btnCheck.Enabled = false;
            lblStatus.Text = "Checking…";
            UpdateChannel channel = rbBeta.Checked ? UpdateChannel.Beta : UpdateChannel.Stable;
            try
            {
                ReleaseInfo found = await Task.Run(() => UpdateService.FindUpdate(channel));
                if (IsDisposed) return;
                if (found == null)
                {
                    lblStatus.Text = "You have the latest " + UpdateService.ChannelName(channel) + " version.";
                }
                else
                {
                    lblStatus.Text = "v" + found.Version + " is available.";
                    using (var dialog = new UpdateDialog(found))
                    {
                        dialog.ShowDialog(FindForm());
                    }
                }
            }
            catch (Exception ex)
            {
                if (!IsDisposed) lblStatus.Text = "Could not check for updates: " + ex.Message;
            }
            finally
            {
                if (!IsDisposed) btnCheck.Enabled = true;
            }
        }
    }
}
