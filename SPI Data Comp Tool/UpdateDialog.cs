using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// The Update window: what the new version is, what is new in it, and three choices -
    /// update now, install when the app closes, or later.
    /// </summary>
    internal sealed class UpdateDialog : Form
    {
        private readonly ReleaseInfo release;
        private readonly Label lblProgress = new Label { AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 0, 4) };
        private readonly ProgressBar progress = new ProgressBar { Dock = DockStyle.Fill, Height = 14, Margin = new Padding(0, 0, 0, 8), Visible = false };
        private readonly Button btnNow = new Button { Text = "&Update now", AutoSize = true, MinimumSize = new Size(120, 32), Margin = new Padding(8, 0, 0, 0) };
        private readonly Button btnOnClose = new Button { Text = "Install when I &close the app", AutoSize = true, MinimumSize = new Size(120, 32), Margin = new Padding(8, 0, 0, 0) };
        private readonly Button btnLater = new Button { Text = "&Later", AutoSize = true, MinimumSize = new Size(100, 32), Margin = new Padding(8, 0, 0, 0), DialogResult = DialogResult.Cancel };

        /// <summary>For the build check that opens every window: shown with no update to offer.</summary>
        public UpdateDialog() : this(null)
        {
        }

        public UpdateDialog(ReleaseInfo release)
        {
            this.release = release;
            Text = "Update";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(620, 460);
            MinimumSize = new Size(520, 380);
            Padding = new Padding(16);
            CancelButton = btnLater;

            bool installed = UpdateService.IsInstalled;
            var title = new Label
            {
                AutoSize = true,
                Font = new Font(Font.FontFamily, 14F, FontStyle.Bold),
                Text = release == null ? "You have the latest version" : "Update to v" + release.Version + (release.IsBeta ? "   (Beta)" : string.Empty),
                Margin = new Padding(0, 0, 0, 6),
            };
            var info = new Label
            {
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6),
                Text = release == null
                    ? "This is " + AppVersion.Display + "."
                    : "You have v" + AppVersion.Full + "  ·  download about " + UpdateService.SizeText(installed ? release.SetupSize : release.ZipSize)
                      + (installed ? " (64-bit installer)" : " (portable zip)"),
            };
            var betaNote = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(580, 0),
                Margin = new Padding(0, 0, 0, 6),
                Visible = release != null && release.IsBeta,
                Text = "Beta builds come from an open pull request and may still have bugs. "
                     + "You can go back to Stable any time in Settings › Updates.",
            };
            var notesTitle = new Label { AutoSize = true, Text = "What's new", Font = new Font(Font, FontStyle.Bold), Margin = new Padding(0, 6, 0, 4) };
            var notes = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                AccessibleName = "What's new",
                Text = release == null ? string.Empty
                     : string.IsNullOrWhiteSpace(release.Notes) ? "No release notes were published for this version."
                     : release.Notes.Replace("\r\n", "\n").Replace("\n", Environment.NewLine),
            };

            var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 8, 0, 0), WrapContents = false };
            buttons.Controls.Add(btnNow);
            buttons.Controls.Add(btnOnClose);
            buttons.Controls.Add(btnLater);

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 8; i++) root.RowStyles.Add(new RowStyle(i == 4 ? SizeType.Percent : SizeType.AutoSize, i == 4 ? 100F : 0F));
            root.Controls.Add(title, 0, 0);
            root.Controls.Add(info, 0, 1);
            root.Controls.Add(betaNote, 0, 2);
            root.Controls.Add(notesTitle, 0, 3);
            root.Controls.Add(notes, 0, 4);
            root.Controls.Add(lblProgress, 0, 5);
            root.Controls.Add(progress, 0, 6);
            root.Controls.Add(buttons, 0, 7);
            Controls.Add(root);

            btnNow.Enabled = btnOnClose.Enabled = release != null;
            if (release == null) btnLater.Text = "&Close";
            btnNow.Click += (s, e) => Start(installNow: true);
            btnOnClose.Click += (s, e) => Start(installNow: false);
            AcceptButton = btnNow;
            // a read-only text box that gets the focus first would show its whole text selected
            Shown += (s, e) => { notes.Select(0, 0); (release == null ? btnLater : btnNow).Select(); };
            UiTheme.Apply(this, btnNow);
        }

        private async void Start(bool installNow)
        {
            btnNow.Enabled = btnOnClose.Enabled = false;
            progress.Visible = true;
            lblProgress.Text = "Downloading…";
            try
            {
                string file = await Task.Run(() => UpdateService.Download(release, ReportProgress));
                if (IsDisposed) return;
                lblProgress.Text = "Download checked (SHA-256 matches).";
                if (!installNow)
                {
                    UpdateService.PendingFile = file;
                    MessageBox.Show(this, "v" + release.Version + " is ready and will be installed when you close the app.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    return;
                }
                UpdateService.PendingFile = null;
                string newExe = UpdateService.Install(file, restartAfterwards: true);
                if (newExe != null) Process.Start(newExe);
                Application.Exit();
            }
            catch (Exception ex)
            {
                if (IsDisposed) return;
                progress.Visible = false;
                lblProgress.Text = "The update was not installed.";
                btnNow.Enabled = btnOnClose.Enabled = true;
                MessageBox.Show(this, ex.Message, "Update not installed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ReportProgress(int percent)
        {
            try
            {
                if (IsDisposed || !IsHandleCreated) return;
                BeginInvoke((Action)(() =>
                {
                    progress.Value = Math.Max(0, Math.Min(100, percent));
                    lblProgress.Text = "Downloading… " + percent + " %";
                }));
            }
            catch (InvalidOperationException)
            {
                // the window was closed while downloading
            }
        }
    }
}
