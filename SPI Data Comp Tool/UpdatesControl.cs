using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>An explicit check, select, download and install workflow. Opening it never uses the network.</summary>
    internal sealed class UpdatesControl : UserControl
    {
        private readonly IUpdateFeed feed;
        private readonly IUpdateInstaller installer;
        private readonly bool ownsFeed;
        private readonly bool persistPreferences;
        private readonly bool is64BitOperatingSystem;
        private readonly string currentVersionText;
        private readonly UpdateVersion currentVersion;
        private readonly Func<string, bool> confirmInstall;
        private readonly ComboBox channel;
        private readonly ListView releases;
        private readonly TextBox notes;
        private readonly Label status;
        private readonly Label progressText;
        private readonly ProgressBar progress;
        private readonly Button check;
        private readonly Button download;
        private readonly Button cancel;
        private readonly Button install;
        private readonly List<UpdateRelease> available = new List<UpdateRelease>();
        private CancellationTokenSource operation;
        private DownloadedUpdate verifiedPackage;
        private string verifiedTag;
        private string verifiedVersion;
        private bool initializing;
        private bool checkedReleases;
        private bool handingOff;
        private bool disposed;

        public UpdatesControl() : this(new UpdateFeedClient(), new UpdateInstaller(), true)
        {
            ownsFeed = true;
        }

        internal UpdatesControl(IUpdateFeed feed, IUpdateInstaller installer, bool persistPreferences = false,
            string currentVersion = null, bool? is64BitOperatingSystem = null, Func<string, bool> confirmInstall = null)
        {
            this.feed = feed ?? throw new ArgumentNullException(nameof(feed));
            this.installer = installer ?? throw new ArgumentNullException(nameof(installer));
            this.persistPreferences = persistPreferences;
            this.is64BitOperatingSystem = is64BitOperatingSystem ?? Environment.Is64BitOperatingSystem;
            this.currentVersionText = currentVersion ?? AppVersion.Full;
            UpdateVersion parsed;
            UpdateVersion.TryParse(this.currentVersionText, out parsed);
            this.currentVersion = parsed;
            this.confirmInstall = confirmInstall;
            Dock = DockStyle.Fill;
            AutoScroll = true;
            Font = new Font("Segoe UI", 9F);
            Name = "updatesControl";

            var root = new TableLayoutPanel
            {
                Name = "tlp_Updates",
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8,
                Padding = new Padding(4),
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            // The workspace host can be shorter than a standalone Settings window. Keep
            // action/status rows at their preferred height and let these two scrollable
            // content areas use the remaining height without forcing the actions offscreen.
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var installed = new Label
            {
                Name = "lbl_CurrentUpdateVersion", AutoSize = true, Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 10),
                Text = "Current version: v" + currentVersionText + "  |  " +
                    (this.currentVersion == null ? "Manual / unrecognised build" : this.currentVersion.IsBeta ? "Beta" : "Normal / stable") +
                    "  |  " + (installer.Installation.Kind == UpdateInstallKind.Msi ? "MSI" : installer.Installation.Kind.ToString()) + "  |  " + (this.is64BitOperatingSystem ? "64-bit Windows" : "32-bit Windows"),
            };
            root.Controls.Add(installed, 0, 0);
            var choices = new FlowLayoutPanel { Name = "flp_UpdateChoices", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill, WrapContents = true, Margin = new Padding(0, 0, 0, 8) };
            choices.Controls.Add(new Label { Text = "Show releases", AutoSize = true, Margin = new Padding(0, 8, 10, 0) });
            channel = new ComboBox { Name = "cb_UpdateChannel", AccessibleName = "Release channel", DropDownStyle = ComboBoxStyle.DropDownList, Width = 150, Margin = new Padding(0, 4, 10, 0) };
            channel.Items.AddRange(new object[] { "Normal / stable", "Beta", "All releases" });
            check = MakeButton("btn_CheckUpdates", "Check for updates", 160);
            choices.Controls.Add(channel);
            choices.Controls.Add(check);
            root.Controls.Add(choices, 0, 1);

            releases = new ListView
            {
                Name = "lv_UpdateReleases", AccessibleName = "Available versions: select one to download",
                Dock = DockStyle.Fill, View = View.Details, FullRowSelect = true,
                MultiSelect = false, HideSelection = false,
                Margin = new Padding(0, 0, 0, 8),
            };
            releases.Columns.Add("Version", 150);
            releases.Columns.Add("Channel", 85);
            releases.Columns.Add("Published", 125);
            releases.Columns.Add("Status", 250);
            root.Controls.Add(releases, 0, 2);
            var notesGroup = new GroupBox { Text = "Selected version — release notes", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 8), Padding = new Padding(8) };
            notes = new TextBox
            {
                Name = "tb_UpdateReleaseNotes", AccessibleName = "Selected version release notes",
                Dock = DockStyle.Fill, Multiline = true, ReadOnly = true,
                ScrollBars = ScrollBars.Vertical, WordWrap = true,
                Text = "Check for updates, then select the exact version you want. No version is selected automatically.",
            };
            notesGroup.Controls.Add(notes);
            root.Controls.Add(notesGroup, 0, 3);

            progress = new ProgressBar { Name = "pb_UpdateProgress", Dock = DockStyle.Fill, Height = 16, Margin = new Padding(0, 2, 0, 2) };
            progressText = new Label { Name = "lbl_UpdateProgress", Dock = DockStyle.Fill, AutoSize = true, Text = " ", Margin = new Padding(0, 2, 0, 4) };
            root.Controls.Add(progress, 0, 4);
            root.Controls.Add(progressText, 0, 5);
            var actions = new FlowLayoutPanel { Name = "flp_UpdateActions", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill, WrapContents = true, Margin = new Padding(0, 2, 0, 6) };
            download = MakeButton("btn_DownloadUpdate", "Download selected", 165);
            cancel = MakeButton("btn_CancelUpdate", "Cancel", 90);
            install = MakeButton("btn_InstallUpdate", "Install && restart", 165);
            actions.Controls.Add(download);
            actions.Controls.Add(cancel);
            actions.Controls.Add(install);
            root.Controls.Add(actions, 0, 6);
            status = new Label
            {
                Name = "lbl_UpdateStatus", AutoSize = true, Dock = DockStyle.Fill,
                Text = "Select Check for updates. Downloads and installation require your explicit actions.",
                Margin = new Padding(0, 0, 0, 4),
            };
            root.Controls.Add(status, 0, 7);
            Controls.Add(root);

            initializing = true;
            try { channel.SelectedIndex = persistPreferences ? (int)UpdatePreferences.Channel : (int)UpdateChannel.Normal; }
            catch { channel.SelectedIndex = (int)UpdateChannel.Normal; }
            if (channel.SelectedIndex < 0 || channel.SelectedIndex > 2) channel.SelectedIndex = (int)UpdateChannel.Normal;
            initializing = false;
            channel.SelectedIndexChanged += (sender, e) => ChangeChannel();
            releases.SelectedIndexChanged += (sender, e) => SelectionChanged();
            check.Click += async (sender, e) => await CheckForUpdatesAsync();
            download.Click += async (sender, e) => await DownloadSelectedAsync();
            cancel.Click += (sender, e) => CancelOperation();
            install.Click += (sender, e) => InstallSelected();
            UpdateButtons();
            if (persistPreferences && !string.IsNullOrWhiteSpace(UpdatePreferences.InitializationWarning))
                SetStatus(UpdatePreferences.InitializationWarning);
        }

        private static Button MakeButton(string name, string text, int width)
        {
            return new Button { Name = name, Text = text, MinimumSize = new Size(width, 32), Margin = new Padding(0, 0, 8, 0),
                AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
        }

        internal UpdateRelease SelectedRelease
        {
            get { return releases.SelectedItems.Count == 1 ? releases.SelectedItems[0].Tag as UpdateRelease : null; }
        }

        internal void SelectRelease(string tag)
        {
            releases.SelectedIndices.Clear();
            foreach (ListViewItem item in releases.Items)
            {
                if (string.Equals(((UpdateRelease)item.Tag).Tag, tag, StringComparison.Ordinal))
                {
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                    break;
                }
            }
            SelectionChanged();
        }

        public void FocusCheckButton() { check.Select(); }

        private void ChangeChannel()
        {
            if (initializing) return;
            ClearVerifiedDownload();
            PopulateReleases();
            if (persistPreferences)
            {
                try { UpdatePreferences.Channel = (UpdateChannel)channel.SelectedIndex; }
                catch (Exception ex) { SetStatus("Channel filter changed, but the preference could not be saved. " + ex.Message); }
            }
        }

        private void PopulateReleases()
        {
            releases.BeginUpdate();
            try
            {
                releases.Items.Clear();
                UpdateRelease latestNormal = available.FirstOrDefault(r => !r.Version.IsBeta);
                UpdateRelease latestBeta = available.FirstOrDefault(r => r.Version.IsBeta &&
                    (latestNormal == null || (r.PublishedAt > latestNormal.PublishedAt && r.Version.CompareTo(latestNormal.Version) > 0)));
                foreach (UpdateRelease release in available)
                {
                    if (channel.SelectedIndex == (int)UpdateChannel.Normal && release.Version.IsBeta) continue;
                    if (channel.SelectedIndex == (int)UpdateChannel.Beta && !release.Version.IsBeta) continue;
                    var labels = new List<string>();
                    if (currentVersion != null && currentVersion.CompareTo(release.Version) == 0) labels.Add("Current");
                    if (ReferenceEquals(release, latestNormal)) labels.Add("Latest normal");
                    if (ReferenceEquals(release, latestBeta)) labels.Add("Latest beta");
                    if (EarlierBetaWarning(release).Length > 0) labels.Add("Earlier beta");
                    var item = new ListViewItem("v" + release.Version) { Tag = release };
                    item.SubItems.Add(release.Version.IsBeta ? "Beta" : "Normal");
                    item.SubItems.Add(release.PublishedAt.ToLocalTime().ToString("dd MMM yyyy"));
                    item.SubItems.Add(string.Join(" · ", labels));
                    releases.Items.Add(item);
                }
                releases.SelectedIndices.Clear();
            }
            finally { releases.EndUpdate(); }
            SelectionChanged();
        }

        private void SelectionChanged()
        {
            UpdateRelease selected = SelectedRelease;
            if (!DownloadMatchesSelection()) ClearVerifiedDownload();
            notes.Text = selected == null
                ? "Select the exact published version to inspect its release notes and download it."
                : "v" + selected.Version + " — " + (selected.Version.IsBeta ? "Beta" : "Normal / stable") +
                  Environment.NewLine + "Published: " + selected.PublishedAt.ToLocalTime().ToString("dd MMM yyyy, HH:mm") +
                  (EarlierBetaWarning(selected).Length == 0 ? string.Empty : Environment.NewLine + EarlierBetaWarning(selected)) +
                  Environment.NewLine + Environment.NewLine + (string.IsNullOrWhiteSpace(selected.Notes) ? "No release notes supplied." : selected.Notes);
            if (operation == null && !handingOff && !DownloadMatchesSelection())
            {
                progress.Value = 0;
                progress.Style = ProgressBarStyle.Continuous;
                progressText.Text = "No verified download selected";
                if (selected != null)
                    SetStatus("v" + selected.Version + " selected. Download this version before installing it.");
                else if (!checkedReleases)
                    SetStatus("Select Check for updates. Nothing has been downloaded or installed.");
                else ShowReleaseListStatus();
            }
            UpdateButtons();
        }

        private string EarlierBetaWarning(UpdateRelease release)
        {
            UpdateRelease latestNormal = available.FirstOrDefault(r => !r.Version.IsBeta);
            if (release == null || !release.Version.IsBeta || latestNormal == null) return string.Empty;
            if (release.PublishedAt < latestNormal.PublishedAt)
                return "Earlier beta: published before normal v" + latestNormal.Version + "; it may lack later fixes.";
            if (release.Version.CompareTo(latestNormal.Version) <= 0)
                return "Earlier beta: its version is not newer than normal v" + latestNormal.Version + "; it may lack later fixes.";
            if (release.PublishedAt == latestNormal.PublishedAt)
                return "Earlier beta: published at the same time as normal v" + latestNormal.Version + "; it is not demonstrably newer and may lack later fixes.";
            return string.Empty;
        }

        private void ShowReleaseListStatus()
        {
            SetStatus(releases.Items.Count == 0
                ? "No compatible published versions match this channel. Try another channel or check again later."
                : releases.Items.Count + " compatible versions shown. Select one explicitly; nothing has been downloaded.");
        }

        private void ShowUnverifiedOutcome(string message, string progressMessage)
        {
            ClearVerifiedDownload();
            progress.Value = 0;
            progressText.Text = progressMessage;
            SetStatus(message);
        }

        private CancellationTokenSource BeginOperation(string message)
        {
            operation?.Cancel();
            operation = new CancellationTokenSource();
            handingOff = false;
            ClearVerifiedDownload();
            progress.Value = 0;
            progress.Style = ProgressBarStyle.Marquee;
            progressText.Text = "Working…";
            SetStatus(message);
            UpdateButtons();
            return operation;
        }

        private bool IsCurrent(CancellationTokenSource captured)
        {
            return !disposed && !IsDisposed && ReferenceEquals(operation, captured);
        }

        private void EndOperation(CancellationTokenSource captured)
        {
            if (IsCurrent(captured))
            {
                operation = null;
                progress.Style = ProgressBarStyle.Continuous;
                UpdateButtons();
            }
            captured.Dispose();
        }

        internal async Task CheckForUpdatesAsync()
        {
            if (disposed || handingOff) return;
            CancellationTokenSource captured = BeginOperation("Checking published releases on GitHub…");
            CancellationToken token = captured.Token;
            checkedReleases = false;
            available.Clear();
            PopulateReleases();
            try
            {
                IList<UpdateRelease> found = await feed.CheckAsync(token);
                if (!IsCurrent(captured)) return;
                if (token.IsCancellationRequested)
                {
                    ShowUnverifiedOutcome("Update check cancelled. Nothing was installed.", "Check cancelled — no package downloaded");
                    return;
                }
                var seen = new HashSet<string>(StringComparer.Ordinal);
                foreach (UpdateRelease release in found ?? new List<UpdateRelease>())
                {
                    if (release != null && release.Version != null && !string.IsNullOrWhiteSpace(release.Tag) &&
                        release.IsCompatible(installer.Installation.Kind, is64BitOperatingSystem) && seen.Add(release.Tag))
                        available.Add(release);
                }
                available.Sort((left, right) => right.Version.CompareTo(left.Version));
                checkedReleases = true;
                PopulateReleases();
                progress.Value = 100;
                progressText.Text = "Check complete";
                ShowReleaseListStatus();
            }
            catch (OperationCanceledException)
            {
                if (IsCurrent(captured)) ShowUnverifiedOutcome("Update check cancelled. Nothing was installed.", "Check cancelled — no package downloaded");
            }
            catch (Exception ex)
            {
                if (IsCurrent(captured)) ShowUnverifiedOutcome("Unable to check updates. " + ex.Message, "Check failed — no package downloaded");
            }
            finally { EndOperation(captured); }
        }

        internal async Task DownloadSelectedAsync()
        {
            if (disposed || handingOff || operation != null) return;
            UpdateRelease selected = SelectedRelease;
            if (selected == null) { SetStatus("Select the version you want to download first."); return; }
            string selectedTag = selected.Tag;
            string selectedVersion = selected.Version.ToString();
            UpdateInstallKind kind = installer.Installation.Kind;
            CancellationTokenSource captured = BeginOperation("Downloading v" + selectedVersion + "; installation will still require confirmation…");
            CancellationToken token = captured.Token;
            var reporter = new Progress<UpdateDownloadProgress>(value =>
            {
                if (!IsCurrent(captured) || token.IsCancellationRequested || value == null) return;
                progress.Style = value.TotalBytes > 0 ? ProgressBarStyle.Continuous : ProgressBarStyle.Marquee;
                progress.Value = Math.Max(0, Math.Min(100, value.Percentage));
                progressText.Text = (value.BytesReceived / 1048576.0).ToString("0.0") + " MB" +
                    (value.TotalBytes > 0 ? " / " + (value.TotalBytes.Value / 1048576.0).ToString("0.0") + " MB" : " downloaded");
            });
            try
            {
                DownloadedUpdate package = await feed.DownloadAsync(selected, kind, reporter, token);
                if (!IsCurrent(captured)) return;
                if (token.IsCancellationRequested)
                {
                    ShowUnverifiedOutcome("Download cancelled. Nothing was installed.", "Download cancelled — no verified package");
                    return;
                }
                if (package == null || package.Release == null || package.Release.Version == null ||
                    package.Kind != kind || !string.Equals(package.Release.Tag, selectedTag, StringComparison.Ordinal) ||
                    package.Release.Version.ToString() != selectedVersion || string.IsNullOrWhiteSpace(package.FilePath) ||
                    !Regex.IsMatch(package.Sha256 ?? string.Empty, "\\A[0-9a-fA-F]{64}\\z"))
                    throw new InvalidOperationException("The verified download does not match the selected version.");
                verifiedPackage = package;
                verifiedTag = selectedTag;
                verifiedVersion = selectedVersion;
                if (!DownloadMatchesSelection())
                {
                    ShowUnverifiedOutcome("Selection changed; download again for the selected version.", "No verified download selected");
                    return;
                }
                progress.Value = 100;
                progressText.Text = "Download complete — SHA-256 verified";
                SetStatus("v" + selectedVersion + " is ready. Save work and close other tools, then select Install & restart.");
            }
            catch (OperationCanceledException)
            {
                if (IsCurrent(captured)) ShowUnverifiedOutcome("Download cancelled. Nothing was installed.", "Download cancelled — no verified package");
            }
            catch (Exception ex)
            {
                if (IsCurrent(captured)) ShowUnverifiedOutcome("Download failed; installation is disabled. " + ex.Message, "Download failed — no verified package");
            }
            finally { EndOperation(captured); }
        }

        internal void CancelOperation()
        {
            CancellationTokenSource captured = operation;
            if (captured == null) return;
            ClearVerifiedDownload();
            cancel.Enabled = false;
            progressText.Text = "Cancellation requested…";
            SetStatus("Cancelling… Nothing will be installed.");
            // A cancellation callback can complete its task and resume the UI await inline.
            // Publish request state first so its terminal Cancelled state always wins.
            captured.Cancel();
        }

        private bool DownloadMatchesSelection()
        {
            UpdateRelease selected = SelectedRelease;
            return verifiedPackage != null && selected != null &&
                verifiedPackage.Kind == installer.Installation.Kind &&
                string.Equals(verifiedTag, selected.Tag, StringComparison.Ordinal) &&
                selected.Version != null && verifiedVersion == selected.Version.ToString();
        }

        private void ClearVerifiedDownload()
        {
            verifiedPackage = null;
            verifiedTag = verifiedVersion = null;
            install.Enabled = false;
        }

        internal void InstallSelected()
        {
            if (disposed || handingOff || operation != null || !DownloadMatchesSelection()) return;
            try
            {
                string reason;
                if (!installer.CanInstall(out reason)) { SetStatus(reason); return; }
                UpdateRelease selected = SelectedRelease;
                string direction = currentVersion == null ? "The current build version cannot be compared automatically." :
                    selected.Version.CompareTo(currentVersion) < 0 ? "This is a DOWNGRADE. Older versions may not support your current settings or data." :
                    selected.Version.CompareTo(currentVersion) == 0 ? "This reinstalls the current version." : "This updates the current version.";
                string message = "Install v" + selected.Version + " (" + (selected.Version.IsBeta ? "Beta" : "Normal / stable") + ")?" +
                    Environment.NewLine + "Currently running: v" + currentVersionText + Environment.NewLine + direction +
                    (EarlierBetaWarning(selected).Length == 0 ? string.Empty : Environment.NewLine + EarlierBetaWarning(selected)) +
                    Environment.NewLine + Environment.NewLine + "Save your work and close other tools. SPI will close and restart." +
                    Environment.NewLine + "Packages are currently unsigned. Windows may show a UAC or SmartScreen prompt; verify the publisher/source before accepting." +
                    Environment.NewLine + "A checksum verifies download integrity, not publisher signing.";
                bool confirmed = confirmInstall != null ? confirmInstall(message) : MessageBox.Show(FindForm(), message,
                    "Confirm selected update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
                if (!confirmed) { SetStatus("Installation cancelled. Your current version is unchanged."); return; }
                if (!DownloadMatchesSelection() || !installer.CanInstall(out reason)) { SetStatus(reason ?? "The selected package changed; download it again."); return; }
                handingOff = true;
                UpdateButtons();
                SetStatus("Starting the selected update. SPI will close after the updater is ready…");
                installer.InstallAndRestart(verifiedPackage, FindForm());
            }
            catch (Exception ex)
            {
                handingOff = false;
                ShowUnverifiedOutcome("Installation could not start. The app remains open. " + ex.Message, "No verified package — download again");
                UpdateButtons();
            }
        }

        private void UpdateButtons()
        {
            bool idle = operation == null && !handingOff;
            check.Enabled = idle;
            channel.Enabled = idle;
            releases.Enabled = idle;
            download.Enabled = idle && SelectedRelease != null;
            cancel.Enabled = operation != null && !operation.IsCancellationRequested;
            install.Enabled = idle && DownloadMatchesSelection();
        }

        private void SetStatus(string message)
        {
            if (!disposed && !IsDisposed) status.Text = Common_Data.RedactConnectionCredentials(message ?? string.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposed = true;
                operation?.Cancel();
                // Default feed owns its cache. Installer stages a verified copy before requesting exit.
                // Injected services are caller-owned (including the offline test fixture).
                if (ownsFeed) (feed as IDisposable)?.Dispose();
                // Each pending operation owns/disposes its captured source after it completes.
            }
            base.Dispose(disposing);
        }
    }
}
