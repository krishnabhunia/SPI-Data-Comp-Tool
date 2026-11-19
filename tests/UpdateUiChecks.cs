using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Offline UI checks: fake release feed and installer, no downloads, installs or preference saves.</summary>
    internal static class UpdateUiChecks
    {
        public static void Run(Action<string> passed)
        {
            // Unlike normal startup, --check-ui has not entered Application.Run. A lone
            // DoEvents loop can install and then uninstall the WinForms context, so Progress<T>
            // and incomplete awaits would otherwise resume on the thread pool. Keep one real
            // message-loop lifetime around every async UI check, just as production does.
            bool previousCrossThreadChecks = Control.CheckForIllegalCrossThreadCalls;
            SynchronizationContext previousContext = SynchronizationContext.Current;
            ExceptionDispatchInfo failure = null;
            bool ranChecks = false;
            ThreadExceptionEventHandler threadFailure = (sender, e) =>
            {
                if (failure == null) failure = ExceptionDispatchInfo.Capture(e.Exception);
            };
            Control.CheckForIllegalCrossThreadCalls = true;
            Application.ThreadException += threadFailure;
            try
            {
                using (var loopHost = new Form { ClientSize = new Size(1, 1), ShowInTaskbar = false, Text = "Offline update UI checks" })
                {
                    loopHost.Shown += (sender, e) => loopHost.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            RequireUiContext();
                            ranChecks = true;
                            RunChecks(passed);
                        }
                        catch (Exception ex)
                        {
                            if (failure == null) failure = ExceptionDispatchInfo.Capture(ex);
                        }
                        finally { loopHost.Close(); }
                    }));
                    Application.Run(loopHost);
                }
            }
            finally
            {
                Application.ThreadException -= threadFailure;
                Control.CheckForIllegalCrossThreadCalls = previousCrossThreadChecks;
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
            if (failure != null) failure.Throw();
            Require(ranChecks, "The update UI message loop closed before running the checks.");
        }

        private static void RunChecks(Action<string> passed)
        {
            CheckSelectionAndChannels();
            passed("Updates opens offline, filters compatible Normal/Beta releases, sorts numeric versions and requires explicit selection.");
            CheckEarlierBetaLabels();
            passed("Historical higher-numbered betas remain selectable but are not labeled Latest beta; notes and confirmation warn about later Normal fixes.");
            CheckDownloadAndConfirmation();
            passed("Only a matching verified download can install, with explicit downgrade/reinstall confirmation and active-tool checks.");
            CheckUiThreadMarshalling();
            passed("Worker-thread release results and download progress return to the UI thread; late progress after completion/disposal is ignored.");
            CheckCancellationAndDisposal();
            passed("Update check/download cancellation, stale completions and disposal cannot enable installation or overwrite newer results.");
            CheckConsolidatedUpdater();
            passed("The app has one updater implementation; build status shows the running Normal/Beta channel, not the release filter.");
            CheckSettingsNavigation();
            passed("Help reuses Settings > Updates, preserves other settings sections and makes update actions usable at minimum size.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void RequireUiContext()
        {
            Require(Application.MessageLoop && SynchronizationContext.Current is WindowsFormsSynchronizationContext,
                "Async updater checks must run inside the production-style WinForms message loop and synchronization context.");
        }

        private static T Find<T>(Control root, string name) where T : Control
        {
            var result = root.Controls.Find(name, true).OfType<T>().SingleOrDefault();
            if (result == null) throw new InvalidOperationException("Missing update control: " + name);
            return result;
        }

        private static void Complete(Task task)
        {
            RequireUiContext();
            var timer = Stopwatch.StartNew();
            while (!task.IsCompleted)
            {
                Application.DoEvents();
                if (timer.Elapsed > TimeSpan.FromSeconds(10)) throw new InvalidOperationException("Offline update UI check timed out.");
                Thread.Sleep(1);
            }
            task.GetAwaiter().GetResult();
            Application.DoEvents();
            RequireUiContext();
        }

        private static Form Host(UpdatesControl updates)
        {
            var host = new Form { ClientSize = new Size(860, 620), Text = "Synthetic update UI check" };
            host.Controls.Add(updates);
            host.Show();
            Application.DoEvents();
            RequireUiContext();
            return host;
        }

        private static UpdateRelease Release(string version, bool installer = true, bool portable = true, bool checksum = true)
        {
            UpdateVersion parsed;
            Require(UpdateVersion.TryParse(version, out parsed), "Synthetic version is invalid.");
            return new UpdateRelease
            {
                Version = parsed, Tag = "v" + version, Name = "Synthetic release " + version,
                PublishedAt = new DateTimeOffset(2026, 11, 18, 12, 0, 0, TimeSpan.Zero),
                Notes = "Synthetic plain text notes.\r\n<b>This is text, not an HTML control.</b>",
                Installer = installer ? new UpdateAsset { Name = "synthetic-setup.exe" } : null,
                Portable = portable ? new UpdateAsset { Name = "synthetic-portable.zip" } : null,
                Checksums = checksum ? new UpdateAsset { Name = "SHA256SUMS.txt" } : null,
            };
        }

        private static DownloadedUpdate Package(UpdateRelease release, UpdateInstallKind kind)
        {
            return new DownloadedUpdate
            {
                Release = release, Kind = kind, FilePath = "synthetic-only-package-not-on-disk",
                Sha256 = new string('a', 64),
            };
        }

        private static void CheckSelectionAndChannels()
        {
            var feed = new FakeFeed
            {
                Releases = new List<UpdateRelease>
                {
                    Release("2.9.0"), Release("2.10.0-beta001"), Release("10.0.0"), Release("2.10.0"),
                    Release("2.10.0-beta002"), Release("20.0.0", installer: false),
                    Release("30.0.0", checksum: false),
                },
            };
            var installer = new FakeInstaller();
            var updates = new UpdatesControl(feed, installer, false, "2.9.0", true);
            using (var host = Host(updates))
            {
                var channel = Find<ComboBox>(updates, "cb_UpdateChannel");
                var list = Find<ListView>(updates, "lv_UpdateReleases");
                Require(feed.Checks == 0 && feed.Downloads == 0 && installer.Installs == 0, "Opening Updates performed external work.");
                Require(channel.SelectedIndex == (int)UpdateChannel.Normal && list.Items.Count == 0,
                    "Updates must start with the Normal channel and no assumed release list.");
                Require(!Find<Button>(updates, "btn_DownloadUpdate").Enabled && !Find<Button>(updates, "btn_InstallUpdate").Enabled,
                    "Download or install was enabled without an explicitly selected release.");
                Complete(updates.CheckForUpdatesAsync());
                Require(list.Items.Count == 3 && list.Items[0].Text == "v10.0.0" && list.Items[1].Text == "v2.10.0",
                    "Normal releases were not filtered or numerically sorted.");
                Require(list.Items[0].SubItems[3].Text.Contains("Latest normal") && list.Items[2].SubItems[3].Text.Contains("Current"),
                    "Current/latest normal labels are missing.");
                Require(updates.SelectedRelease == null && !Find<Button>(updates, "btn_InstallUpdate").Enabled,
                    "Checking automatically selected or installed the latest release.");
                channel.SelectedIndex = (int)UpdateChannel.Beta;
                Require(list.Items.Count == 2 && list.Items[0].Text == "v2.10.0-beta002" &&
                    list.Items.Cast<ListViewItem>().All(item => !item.SubItems[3].Text.Contains("Latest beta")),
                    "Beta filtering/ordering is wrong or a beta older than the latest Normal version was labeled latest.");
                Require(Find<Label>(updates, "lbl_UpdateStatus").Text.StartsWith("2 compatible versions shown", StringComparison.Ordinal),
                    "Changing channel retained the previous channel's release count.");
                channel.SelectedIndex = (int)UpdateChannel.All;
                Require(list.Items.Count == 5 && list.Items[1].Text == "v2.10.0" && list.Items[2].Text == "v2.10.0-beta002",
                    "All releases did not order stable/beta versions correctly.");
                updates.SelectRelease("v2.10.0-beta001");
                Require(updates.SelectedRelease.Tag == "v2.10.0-beta001" && Find<Button>(updates, "btn_DownloadUpdate").Enabled,
                    "An older beta cannot be explicitly selected.");
                Require(Find<TextBox>(updates, "tb_UpdateReleaseNotes").Text.Contains("<b>This is text"),
                    "Release notes are not displayed as plain text.");
                Require(!Find<Button>(updates, "btn_InstallUpdate").Enabled && installer.Installs == 0,
                    "Selecting a release enabled installation before download verification.");
                host.Close();
                Require(feed.Disposals == 0, "Updates disposed an injected, caller-owned feed.");
            }

            var architectureFeed = new FakeFeed { Releases = new List<UpdateRelease> { Release("1.0.0"), Release("2.0.0", checksum: false) } };
            using (var installed32 = Host(new UpdatesControl(architectureFeed, new FakeInstaller(), false, "1.0.0", false)))
            {
                var updates32 = installed32.Controls.OfType<UpdatesControl>().Single();
                Complete(updates32.CheckForUpdatesAsync());
                Require(Find<ListView>(updates32, "lv_UpdateReleases").Items.Count == 0,
                    "A 64-bit installer was offered to 32-bit Windows or the app silently switched installation kind.");
            }
            var portableInstaller = new FakeInstaller(UpdateInstallKind.Portable);
            using (var portable32 = Host(new UpdatesControl(architectureFeed, portableInstaller, false, "1.0.0", false)))
            {
                var updates32 = portable32.Controls.OfType<UpdatesControl>().Single();
                Complete(updates32.CheckForUpdatesAsync());
                Require(Find<ListView>(updates32, "lv_UpdateReleases").Items.Count == 1,
                    "The compatible portable build was hidden on 32-bit Windows or missing checksums were accepted.");
            }
            var manualFeed = new FakeFeed();
            using (var manual = Host(new UpdatesControl(manualFeed, new FakeInstaller(), false, "2.9.0-dev", true)))
            {
                Require(Find<Label>(manual, "lbl_CurrentUpdateVersion").Text.Contains("Manual / unrecognised build") && manualFeed.Checks == 0,
                    "Unrecognised current build metadata was mislabeled as a Normal release or triggered a network check.");
            }
            using (var beta = Host(new UpdatesControl(new FakeFeed(), new FakeInstaller(), false, "2.9.0-beta002", true)))
            {
                string label = Find<Label>(beta, "lbl_CurrentUpdateVersion").Text;
                Require(label.Contains(" |  Beta  | ") && !label.Contains("unrecognised"), "A canonical current beta lost its Beta channel label.");
            }
        }

        private static void CheckEarlierBetaLabels()
        {
            // Mirror the repository's higher-numbered, portable-only beta that predates later fixes.
            var earlier = Release("3.0.0-beta001", installer: false);
            earlier.PublishedAt = new DateTimeOffset(2026, 11, 17, 12, 0, 0, TimeSpan.Zero);
            var normal = Release("2.5.8");
            normal.PublishedAt = new DateTimeOffset(2026, 11, 18, 12, 0, 0, TimeSpan.Zero);
            var feed = new FakeFeed { Releases = new List<UpdateRelease> { earlier, normal } };
            var installer = new FakeInstaller(UpdateInstallKind.Portable);
            string confirmation = null;
            var updates = new UpdatesControl(feed, installer, false, "2.5.8", true, text => { confirmation = text; return false; });
            using (var host = Host(updates))
            {
                Complete(updates.CheckForUpdatesAsync());
                var channel = Find<ComboBox>(updates, "cb_UpdateChannel");
                channel.SelectedIndex = (int)UpdateChannel.Beta;
                var list = Find<ListView>(updates, "lv_UpdateReleases");
                Require(list.Items.Count == 1 && list.Items[0].Text == "v3.0.0-beta001" &&
                    list.Items[0].SubItems[3].Text.Contains("Earlier beta") && !list.Items[0].SubItems[3].Text.Contains("Latest beta"),
                    "A historical higher-numbered beta was hidden or incorrectly recommended as Latest beta.");
                Require(updates.SelectedRelease == null, "The historical beta was automatically selected.");
                updates.SelectRelease(earlier.Tag);
                const string warning = "published before normal v2.5.8; it may lack later fixes";
                Require(Find<TextBox>(updates, "tb_UpdateReleaseNotes").Text.Contains(warning),
                    "Historical beta notes omitted the newer Normal release warning.");
                Complete(updates.DownloadSelectedAsync());
                updates.InstallSelected();
                Require(confirmation != null && confirmation.Contains(warning) && installer.Installs == 0,
                    "Historical beta confirmation omitted the warning or ignored explicit cancellation.");

                var newer = Release("2.5.9-beta001");
                newer.PublishedAt = normal.PublishedAt.AddDays(1);
                var lowerVersion = Release("2.5.8-beta002");
                lowerVersion.PublishedAt = normal.PublishedAt.AddDays(1);
                var equalPublication = Release("2.6.0-beta001");
                equalPublication.PublishedAt = normal.PublishedAt;
                feed.Releases.Add(newer);
                feed.Releases.Add(lowerVersion);
                feed.Releases.Add(equalPublication);
                Complete(updates.CheckForUpdatesAsync());
                Require(list.Items.Count == 4 && list.Items[0].Text == "v3.0.0-beta001" && list.Items[1].Text == "v2.6.0-beta001" &&
                    list.Items[2].Text == "v2.5.9-beta001" && list.Items[2].SubItems[3].Text.Contains("Latest beta") &&
                    list.Items.Cast<ListViewItem>().Count(item => item.SubItems[3].Text.Contains("Latest beta")) == 1,
                    "Latest beta did not require both a later publication and a newer numeric version while preserving list order.");
                updates.SelectRelease(newer.Tag);
                Require(!Find<TextBox>(updates, "tb_UpdateReleaseNotes").Text.Contains(warning),
                    "A genuinely newer beta inherited the historical release warning.");
                updates.SelectRelease(lowerVersion.Tag);
                string lowerNotes = Find<TextBox>(updates, "tb_UpdateReleaseNotes").Text;
                Require(lowerNotes.Contains("version is not newer than normal v2.5.8") && !lowerNotes.Contains("published before"),
                    "A later-published, lower-version beta was given a false publication-date warning.");
                Complete(updates.DownloadSelectedAsync());
                updates.InstallSelected();
                Require(confirmation.Contains("version is not newer than normal v2.5.8") && !confirmation.Contains("published before"),
                    "Lower-version beta confirmation inaccurately described its publication date.");
                updates.SelectRelease(equalPublication.Tag);
                string equalNotes = Find<TextBox>(updates, "tb_UpdateReleaseNotes").Text;
                Require(equalNotes.Contains("published at the same time as normal v2.5.8") && !equalNotes.Contains("published before"),
                    "A simultaneous beta was described as published before Normal.");
                Complete(updates.DownloadSelectedAsync());
                updates.InstallSelected();
                Require(confirmation.Contains("published at the same time as normal v2.5.8") && !confirmation.Contains("published before") && installer.Installs == 0,
                    "Simultaneous-beta confirmation inaccurately described its publication date or installed without approval.");
                host.Close();
            }
        }

        private static void CheckDownloadAndConfirmation()
        {
            var selected = Release("2.0.0");
            var current = Release("3.0.0");
            var feed = new FakeFeed { Releases = new List<UpdateRelease> { selected, current } };
            var installer = new FakeInstaller();
            bool confirm = false;
            int prompts = 0;
            string prompt = null;
            var updates = new UpdatesControl(feed, installer, false, "3.0.0", true, text =>
            {
                prompts++;
                prompt = text;
                return confirm;
            });
            using (var host = Host(updates))
            {
                Complete(updates.CheckForUpdatesAsync());
                updates.SelectRelease(selected.Tag);
                feed.DownloadHandler = (release, kind, progress, token) =>
                {
                    progress.Report(new UpdateDownloadProgress { BytesReceived = 20, TotalBytes = 100 });
                    var wrong = Package(release, kind);
                    wrong.Sha256 = "not-a-valid-checksum";
                    return Task.FromResult(wrong);
                };
                Complete(updates.DownloadSelectedAsync());
                Require(!Find<Button>(updates, "btn_InstallUpdate").Enabled && Find<Label>(updates, "lbl_UpdateProgress").Text.Contains("failed") &&
                    Find<ProgressBar>(updates, "pb_UpdateProgress").Value == 0,
                    "An invalid checksum enabled installation or retained in-progress/verified display state.");
                feed.DownloadHandler = (release, kind, progress, token) => Task.FromResult(Package(current, kind));
                Complete(updates.DownloadSelectedAsync());
                Require(!Find<Button>(updates, "btn_InstallUpdate").Enabled, "A different version's download enabled installation.");
                feed.DownloadHandler = null;
                Complete(updates.DownloadSelectedAsync());
                Require(Find<Button>(updates, "btn_InstallUpdate").Enabled && installer.Installs == 0,
                    "Verified download did not become installable, or installation began automatically.");
                var channel = Find<ComboBox>(updates, "cb_UpdateChannel");
                channel.SelectedIndex = (int)UpdateChannel.Beta;
                Require(!Find<Button>(updates, "btn_InstallUpdate").Enabled &&
                    Find<Label>(updates, "lbl_UpdateStatus").Text.Contains("No compatible published versions") &&
                    !Find<Label>(updates, "lbl_UpdateProgress").Text.Contains("SHA-256 verified"),
                    "Changing channel retained a ready-to-install message or stale verified progress.");
                channel.SelectedIndex = (int)UpdateChannel.Normal;
                updates.SelectRelease(selected.Tag);
                Complete(updates.DownloadSelectedAsync());
                updates.SelectRelease(current.Tag);
                Require(!Find<Button>(updates, "btn_InstallUpdate").Enabled &&
                    Find<Label>(updates, "lbl_UpdateStatus").Text.StartsWith("v3.0.0 selected", StringComparison.Ordinal) &&
                    !Find<Label>(updates, "lbl_UpdateProgress").Text.Contains("SHA-256 verified"),
                    "Changing the selection retained another version's download or ready/verified text.");
                Complete(updates.DownloadSelectedAsync());
                updates.InstallSelected();
                Require(prompt.Contains("reinstalls the current version") && installer.Installs == 0,
                    "Reinstall did not request explicit confirmation.");
                updates.SelectRelease(selected.Tag);
                Complete(updates.DownloadSelectedAsync());
                updates.InstallSelected();
                Require(prompt.Contains("DOWNGRADE") && prompt.Contains("UAC") && prompt.Contains("unsigned") && prompt.Contains("close other tools") &&
                    prompt.Contains("v2.0.0") && installer.Installs == 0,
                    "Downgrade/unsigned/unfinished-work warnings or explicit cancellation failed.");
                confirm = true;
                int promptsBeforeBlock = prompts;
                installer.Allowed = false;
                updates.InstallSelected();
                Require(installer.Installs == 0 && prompts == promptsBeforeBlock &&
                    Find<Label>(updates, "lbl_UpdateStatus").Text.Contains("Close the synthetic busy tool"),
                    "Busy tools did not block the installation handoff.");
                installer.Allowed = true;
                installer.ThrowOnInstall = true;
                updates.InstallSelected();
                Require(installer.Installs == 0 && !Find<Button>(updates, "btn_InstallUpdate").Enabled &&
                    Find<Label>(updates, "lbl_UpdateProgress").Text.Contains("download again"),
                    "A failed handoff retained an installable package or stale ready status.");
                installer.ThrowOnInstall = false;
                Complete(updates.DownloadSelectedAsync());
                updates.InstallSelected();
                updates.InstallSelected();
                Require(installer.Installs == 1 && installer.LastPackage.Release.Tag == selected.Tag &&
                    !Find<Button>(updates, "btn_InstallUpdate").Enabled,
                    "Installation did not hand off the selected version exactly once.");
                host.Close();
            }
        }

        private static void CheckUiThreadMarshalling()
        {
            int uiThread = Thread.CurrentThread.ManagedThreadId;
            int wrongThread = 0;
            var selected = Release("2.0.0");
            var checkResult = new TaskCompletionSource<IList<UpdateRelease>>();
            var downloadResult = new TaskCompletionSource<DownloadedUpdate>();
            IProgress<UpdateDownloadProgress> reporter = null;
            var feed = new FakeFeed
            {
                CheckHandler = token => checkResult.Task,
                DownloadHandler = (release, kind, progress, token) => { reporter = progress; return downloadResult.Task; },
            };
            var installer = new FakeInstaller();
            var updates = new UpdatesControl(feed, installer, false, "1.0.0", true);
            using (var host = Host(updates))
            {
                var progressLabel = Find<Label>(updates, "lbl_UpdateProgress");
                var statusLabel = Find<Label>(updates, "lbl_UpdateStatus");
                EventHandler observeThread = (sender, e) =>
                {
                    if (Thread.CurrentThread.ManagedThreadId != uiThread) Interlocked.Exchange(ref wrongThread, 1);
                };
                progressLabel.TextChanged += observeThread;
                statusLabel.TextChanged += observeThread;

                Task checkWork = updates.CheckForUpdatesAsync();
                Complete(Task.WhenAll(checkWork, Task.Run(() => checkResult.SetResult(new List<UpdateRelease> { selected }))));
                updates.SelectRelease(selected.Tag);
                Task downloadWork = updates.DownloadSelectedAsync();
                Require(reporter != null && !downloadWork.IsCompleted, "The worker-thread download fixture did not remain pending.");
                Complete(Task.Run(() => reporter.Report(new UpdateDownloadProgress { BytesReceived = 1048576, TotalBytes = 4194304 })));
                Require(Find<ProgressBar>(updates, "pb_UpdateProgress").Value == 25 && progressLabel.Text.Contains("MB"),
                    "Worker-thread progress was not delivered to the UI while the download was pending.");
                Complete(Task.WhenAll(downloadWork, Task.Run(() => downloadResult.SetResult(Package(selected, UpdateInstallKind.Installed)))));
                Require(Find<Button>(updates, "btn_InstallUpdate").Enabled && progressLabel.Text.Contains("SHA-256 verified"),
                    "Worker-thread completion failed to publish the verified package on the UI thread.");
                Complete(Task.Run(() => reporter.Report(new UpdateDownloadProgress { BytesReceived = 1, TotalBytes = 100 })));
                Require(progressLabel.Text.Contains("SHA-256 verified") && Find<ProgressBar>(updates, "pb_UpdateProgress").Value == 100,
                    "Late worker progress overwrote the completed download state.");
                host.Close();
                Complete(Task.Run(() => reporter.Report(new UpdateDownloadProgress { BytesReceived = 2, TotalBytes = 100 })));
                Require(updates.IsDisposed && Interlocked.CompareExchange(ref wrongThread, 0, 0) == 0 && installer.Installs == 0,
                    "Progress/completion touched controls off-thread, resurrected disposed controls or installed automatically.");
            }
        }

        private static void CheckCancellationAndDisposal()
        {
            var feed = new FakeFeed();
            var installer = new FakeInstaller();
            var updates = new UpdatesControl(feed, installer, false, "1.0.0", true);
            using (var host = Host(updates))
            {
                var oldCheck = new TaskCompletionSource<IList<UpdateRelease>>();
                var newCheck = new TaskCompletionSource<IList<UpdateRelease>>();
                feed.CheckHandler = token => feed.Checks == 1 ? oldCheck.Task : newCheck.Task;
                Task oldWork = updates.CheckForUpdatesAsync();
                CancellationToken oldToken = feed.LastToken;
                Task newWork = updates.CheckForUpdatesAsync();
                Require(oldToken.IsCancellationRequested, "A refreshed check did not cancel the previous request.");
                newCheck.SetResult(new List<UpdateRelease> { Release("2.0.0") });
                Complete(newWork);
                oldCheck.SetResult(new List<UpdateRelease> { Release("9.0.0") });
                Complete(oldWork);
                var list = Find<ListView>(updates, "lv_UpdateReleases");
                Require(list.Items.Count == 1 && list.Items[0].Text == "v2.0.0", "A stale check overwrote a newer result.");
                updates.SelectRelease("v2.0.0");
                var pendingDownload = new TaskCompletionSource<DownloadedUpdate>();
                feed.DownloadHandler = (release, kind, reporter, token) => pendingDownload.Task;
                Task downloadWork = updates.DownloadSelectedAsync();
                CancellationToken downloadToken = feed.LastToken;
                Require(Find<Button>(updates, "btn_CancelUpdate").Enabled && !Find<Button>(updates, "btn_CheckUpdates").Enabled,
                    "An active download did not expose cancellation or disable conflicting actions.");
                updates.CancelOperation();
                pendingDownload.SetResult(Package(Release("2.0.0"), UpdateInstallKind.Installed));
                Complete(downloadWork);
                Require(downloadToken.IsCancellationRequested && !Find<Button>(updates, "btn_InstallUpdate").Enabled &&
                    Find<Label>(updates, "lbl_UpdateStatus").Text.Contains("cancelled") &&
                    Find<Label>(updates, "lbl_UpdateProgress").Text.Contains("cancelled") &&
                    Find<ProgressBar>(updates, "pb_UpdateProgress").Value == 0,
                    "A cancelled download became installable when its service ignored cancellation.");

                var cancelledCheck = new TaskCompletionSource<IList<UpdateRelease>>();
                feed.CheckHandler = token => cancelledCheck.Task;
                Task cancelWork = updates.CheckForUpdatesAsync();
                CancellationToken cancelToken = feed.LastToken;
                updates.CancelOperation();
                cancelledCheck.SetResult(new List<UpdateRelease> { Release("8.0.0") });
                Complete(cancelWork);
                Require(cancelToken.IsCancellationRequested && list.Items.Count == 0 &&
                    Find<Label>(updates, "lbl_UpdateStatus").Text.Contains("cancelled") &&
                    Find<Label>(updates, "lbl_UpdateProgress").Text.Contains("cancelled"),
                    "A cancelled check published releases or remained stuck in Cancelling state.");

                var immediateCancellation = new TaskCompletionSource<IList<UpdateRelease>>();
                feed.CheckHandler = token =>
                {
                    token.Register(() => immediateCancellation.TrySetCanceled());
                    return immediateCancellation.Task;
                };
                Task immediateWork = updates.CheckForUpdatesAsync();
                updates.CancelOperation();
                Complete(immediateWork);
                Require(Find<Label>(updates, "lbl_UpdateStatus").Text.Contains("cancelled") &&
                    Find<Label>(updates, "lbl_UpdateProgress").Text.Contains("cancelled") &&
                    Find<Button>(updates, "btn_CheckUpdates").Enabled,
                    "Synchronous cancellation completion was overwritten by a stale Cancelling status.");

                feed.CheckHandler = token => { throw new InvalidOperationException("Synthetic offline failure."); };
                Complete(updates.CheckForUpdatesAsync());
                Require(Find<Label>(updates, "lbl_UpdateProgress").Text.Contains("Check failed") &&
                    !Find<Button>(updates, "btn_InstallUpdate").Enabled && Find<Button>(updates, "btn_CheckUpdates").Enabled,
                    "A failed check retained Working state or did not allow retry.");

                var disposedCheck = new TaskCompletionSource<IList<UpdateRelease>>();
                feed.CheckHandler = token => disposedCheck.Task;
                Task disposedWork = updates.CheckForUpdatesAsync();
                CancellationToken disposedToken = feed.LastToken;
                host.Close();
                disposedCheck.SetResult(new List<UpdateRelease> { Release("7.0.0") });
                Complete(disposedWork);
                Require(disposedToken.IsCancellationRequested && updates.IsDisposed && installer.Installs == 0,
                    "Closing Settings failed to cancel a pending check.");
            }

            var disposeFeed = new FakeFeed { Releases = new List<UpdateRelease> { Release("2.0.0") } };
            var pending = new TaskCompletionSource<DownloadedUpdate>();
            disposeFeed.DownloadHandler = (release, kind, reporter, token) => pending.Task;
            var disposeUpdates = new UpdatesControl(disposeFeed, new FakeInstaller(), false, "1.0.0", true);
            using (var host = Host(disposeUpdates))
            {
                Complete(disposeUpdates.CheckForUpdatesAsync());
                disposeUpdates.SelectRelease("v2.0.0");
                Task work = disposeUpdates.DownloadSelectedAsync();
                CancellationToken token = disposeFeed.LastToken;
                host.Close();
                pending.SetResult(Package(Release("2.0.0"), UpdateInstallKind.Installed));
                Complete(work);
                Require(token.IsCancellationRequested && disposeUpdates.IsDisposed, "Closing Updates failed to cancel a pending download.");
            }
        }

        private static void CheckConsolidatedUpdater()
        {
            Require(MainForm.BuildVersionStatus("2.6.0") == "v2.6.0  ·  Normal" &&
                MainForm.BuildVersionStatus("2.6.0-beta006") == "v2.6.0-beta006  ·  Beta" &&
                MainForm.BuildVersionStatus("2.6.0-manual").Contains("Manual / unrecognised build") &&
                MainForm.BuildVersionStatus(null).StartsWith("Version unavailable", StringComparison.Ordinal),
                "Main window status misidentifies a normal, beta or unrecognised running build.");
            foreach (string legacyType in new[] { "UpdateDialog", "UpdateSettingsPage", "UpdateService" })
                Require(typeof(MainForm).Assembly.GetType("SPI_Data_Comp_Tool." + legacyType, false) == null,
                    "The superseded updater is still compiled: " + legacyType + ".");
        }

        private static void CheckSettingsNavigation()
        {
            using (var main = new MainForm())
            {
                main.Show();
                Application.DoEvents();
                var menu = Find<MenuStrip>(main, "menuStrip1");
                Require(Find<Label>(main, "lbl_Version").Text == MainForm.BuildVersionStatus(AppVersion.Full),
                    "The main window does not show its actual build version and channel.");
                Require(menu.Items.Find("updateToolStripMenuItem", true).Length == 0,
                    "A second automatic-update menu entry survived consolidation.");
                menu.Items.Find("userSettingsToolStripMenuItem", true).OfType<ToolStripMenuItem>().Single().PerformClick();
                Application.DoEvents();
                var settings = Application.OpenForms.OfType<User_Settings>().Single();
                var log = Find<RichTextBox>(settings, "rtb_Log");
                log.Text = "Synthetic retained rules log";
                menu.Items.Find("checkForUpdatesToolStripMenuItem", true).OfType<ToolStripMenuItem>().Single().PerformClick();
                Application.DoEvents();
                Require(ReferenceEquals(settings, main.ShowUpdatesSettings()) && Application.OpenForms.OfType<User_Settings>().Count() == 1,
                    "Help created a duplicate Settings/Updates window.");
                var tabs = Find<TabControl>(settings, "tabs_Settings");
                Require(tabs.TabPages.Count == 3 && tabs.SelectedTab.Name == "tab_Updates" && settings.AcceptButton == null,
                    "Help did not open Updates or Enter can implicitly start an action.");
                Require(tabs.TabPages.Cast<TabPage>().Count(tab => tab.Name == "tab_Updates") == 1 &&
                    settings.Controls.Find("updatesControl", true).Length == 1,
                    "Settings contains duplicate Updates pages or updater controls.");
                Require(!Find<GroupBox>(settings, "gb_Logs").Visible && Find<TableLayoutPanel>(settings, "tlp_Root").RowStyles[1].Height == 0,
                    "The unrelated log consumes the Updates workspace.");
                settings.Size = settings.MinimumSize;
                settings.PerformLayout();
                Application.DoEvents();
                var updates = Find<UpdatesControl>(settings, "updatesControl");
                Rectangle viewport = updates.RectangleToScreen(updates.ClientRectangle);
                foreach (string name in new[] { "btn_CheckUpdates", "btn_DownloadUpdate", "btn_CancelUpdate", "btn_InstallUpdate" })
                {
                    var button = Find<Button>(updates, name);
                    Require(button.Visible && viewport.Contains(button.RectangleToScreen(button.ClientRectangle)),
                        "Minimum Settings size clips the update action " + name + ".");
                }
                settings.ShowRules();
                Application.DoEvents();
                Require(tabs.SelectedTab.Name == "tab_Rules" && log.Text == "Synthetic retained rules log" &&
                    Find<GroupBox>(settings, "gb_Logs").Visible, "Switching back to Rules discarded its log or state.");
                settings.Close();
                main.Close();
            }
        }

        private sealed class FakeFeed : IUpdateFeed, IDisposable
        {
            public IList<UpdateRelease> Releases = new List<UpdateRelease>();
            public Func<CancellationToken, Task<IList<UpdateRelease>>> CheckHandler;
            public Func<UpdateRelease, UpdateInstallKind, IProgress<UpdateDownloadProgress>, CancellationToken, Task<DownloadedUpdate>> DownloadHandler;
            public int Checks;
            public int Downloads;
            public int Disposals;
            public CancellationToken LastToken;

            public void Dispose() { Disposals++; }

            public Task<IList<UpdateRelease>> CheckAsync(CancellationToken cancellationToken)
            {
                Checks++;
                LastToken = cancellationToken;
                return CheckHandler != null ? CheckHandler(cancellationToken) : Task.FromResult(Releases);
            }

            public Task<DownloadedUpdate> DownloadAsync(UpdateRelease release, UpdateInstallKind kind,
                IProgress<UpdateDownloadProgress> progress, CancellationToken cancellationToken)
            {
                Downloads++;
                LastToken = cancellationToken;
                return DownloadHandler != null ? DownloadHandler(release, kind, progress, cancellationToken) : Task.FromResult(Package(release, kind));
            }
        }

        private sealed class FakeInstaller : IUpdateInstaller
        {
            public UpdateInstallation Installation { get; private set; }
            public bool Allowed = true;
            public bool ThrowOnInstall;
            public int Installs;
            public DownloadedUpdate LastPackage;

            public FakeInstaller(UpdateInstallKind kind = UpdateInstallKind.Installed)
            {
                Installation = new UpdateInstallation { Kind = kind, Directory = "synthetic-only-directory" };
            }

            public bool CanInstall(out string reason)
            {
                reason = Allowed ? null : "Close the synthetic busy tool before installing.";
                return Allowed;
            }

            public void InstallAndRestart(DownloadedUpdate package, IWin32Window owner)
            {
                if (ThrowOnInstall) throw new InvalidOperationException("Synthetic handoff failure.");
                Installs++;
                LastPackage = package;
            }
        }
    }
}
