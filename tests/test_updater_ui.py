"""Source contracts for explicit in-app updates; offline Windows checks exercise the controls."""

import re
import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
PROJ = ROOT / "SPI Data Comp Tool"


def source(name):
    return (PROJ / name).read_text(encoding="utf-8-sig")


class UpdaterUiContracts(unittest.TestCase):
    def test_updates_reuses_settings_instead_of_another_form(self):
        designer = source("User_Settings.Designer.cs")
        for tab in ("tab_General", "tab_Rules", "tab_Updates"):
            self.assertIn(f"this.tabs_Settings.Controls.Add(this.{tab});", designer)
        self.assertIn("internal sealed class UpdatesControl : UserControl", source("UpdatesControl.cs"))
        main = source("MainForm.cs")
        entry = main.split("internal User_Settings ShowUpdatesSettings()", 1)[1].split("private void checkForUpdates", 1)[0]
        self.assertIn('workspaces.Open(this, "settings.preferences")', entry)
        self.assertIn("settings.ShowUpdates();", entry)
        self.assertNotIn("new Form", entry)
        self.assertIn('Name = "checkForUpdatesToolStripMenuItem"', main)
        self.assertIn('"settings.preferences"', source("WorkspaceCatalog.cs"))

    def test_opening_and_navigation_never_check_download_or_install(self):
        ui = source("UpdatesControl.cs")
        ctor = ui.split("internal UpdatesControl(", 1)[1].split("private static Button", 1)[0]
        self.assertNotIn("feed.CheckAsync(", ctor)
        self.assertNotIn("feed.DownloadAsync(", ctor)
        self.assertNotIn("installer.InstallAndRestart(", ctor)
        settings = source("User_Settings.cs")
        navigation = settings.split("public void ShowUpdates()", 1)[1].split("private void UpdateAcceptButton", 1)[0]
        self.assertIn("FocusCheckButton()", navigation)
        self.assertNotIn("CheckForUpdatesAsync()", navigation)
        self.assertIn("gb_Logs.Visible = !showingUpdates", settings)
        self.assertIn("AcceptButton = showingUpdates ? null", settings)

    def test_explicit_filters_and_numeric_sorting(self):
        ui = source("UpdatesControl.cs")
        self.assertIn("UpdateChannel.Normal", ui)
        self.assertIn('"Normal / stable", "Beta", "All releases"', ui)
        self.assertIn("release.IsCompatible(installer.Installation.Kind, is64BitOperatingSystem)", ui)
        self.assertIn("right.Version.CompareTo(left.Version)", ui)
        self.assertIn('labels.Add("Current")', ui)
        self.assertIn('labels.Add("Latest normal")', ui)
        self.assertIn('labels.Add("Latest beta")', ui)
        self.assertIn('this.currentVersion == null ? "Manual / unrecognised build"', ui)
        listing = ui.split("private void PopulateReleases()", 1)[1].split("private void SelectionChanged", 1)[0]
        self.assertIn("releases.SelectedIndices.Clear()", listing)
        self.assertNotRegex(listing, r"\.Selected\s*=\s*true")

    def test_install_requires_matching_verified_selection_and_confirmation(self):
        ui = source("UpdatesControl.cs")
        download = ui.split("internal async Task DownloadSelectedAsync()", 1)[1].split("internal void CancelOperation()", 1)[0]
        self.assertIn("package.Kind != kind", download)
        self.assertIn("package.Release.Tag, selectedTag", download)
        self.assertIn("package.Release.Version.ToString() != selectedVersion", download)
        self.assertIn("package.Sha256", download)
        install = ui.split("internal void InstallSelected()", 1)[1].split("private void UpdateButtons()", 1)[0]
        self.assertIn("!DownloadMatchesSelection()", install)
        self.assertIn("MessageBoxDefaultButton.Button2", install)
        self.assertLess(install.index("if (!confirmed)"), install.index("installer.InstallAndRestart"))
        self.assertGreaterEqual(install.count("installer.CanInstall(out reason)"), 2)
        for phrase in ("DOWNGRADE", "reinstalls", "unsigned", "UAC", "close other tools"):
            self.assertIn(phrase, install)
        self.assertIn("handingOff = true;", install)

    def test_historical_high_version_beta_is_not_recommended_as_latest(self):
        ui = source("UpdatesControl.cs")
        listing = ui.split("private void PopulateReleases()", 1)[1].split("private void SelectionChanged", 1)[0]
        self.assertIn("r.PublishedAt > latestNormal.PublishedAt", listing)
        self.assertIn("r.Version.CompareTo(latestNormal.Version) > 0", listing)
        self.assertIn('labels.Add("Earlier beta")', listing)
        self.assertIn("release.PublishedAt < latestNormal.PublishedAt", ui)
        self.assertIn("release.Version.CompareTo(latestNormal.Version) <= 0", ui)
        self.assertIn("release.PublishedAt == latestNormal.PublishedAt", ui)
        self.assertIn('"Earlier beta: published before normal v"', ui)
        self.assertIn("its version is not newer than normal v", ui)
        self.assertIn("published at the same time as normal v", ui)
        self.assertIn("it may lack later fixes", ui)
        selection = ui.split("private void SelectionChanged()", 1)[1].split("private string EarlierBetaWarning", 1)[0]
        confirmation = ui.split("internal void InstallSelected()", 1)[1].split("private void UpdateButtons()", 1)[0]
        self.assertIn("EarlierBetaWarning(selected)", selection)
        self.assertIn("EarlierBetaWarning(selected)", confirmation)
        checks = (ROOT / "tests" / "UpdateUiChecks.cs").read_text(encoding="utf-8")
        self.assertIn('Release("3.0.0-beta001", installer: false)', checks)
        self.assertIn('Release("2.5.8")', checks)

    def test_async_work_is_cancellable_and_stale_safe(self):
        ui = source("UpdatesControl.cs")
        self.assertIn("ReferenceEquals(operation, captured)", ui)
        self.assertGreaterEqual(ui.count("CancellationToken token = captured.Token;"), 2)
        self.assertIn("operation?.Cancel();", ui)
        self.assertIn("captured.Dispose();", ui)
        self.assertIn("if (ownsFeed) (feed as IDisposable)?.Dispose();", ui)
        self.assertIn("Common_Data.RedactConnectionCredentials", ui)
        self.assertNotIn(".ConfigureAwait(false)", ui)
        self.assertNotIn(".Wait()", ui)
        self.assertNotRegex(ui, r"\bTask\.Run\(")

    def test_invalidated_or_terminal_work_clears_stale_ready_status(self):
        ui = source("UpdatesControl.cs")
        selection = ui.split("private void SelectionChanged()", 1)[1].split("private void ShowReleaseListStatus()", 1)[0]
        self.assertIn("operation == null && !handingOff && !DownloadMatchesSelection()", selection)
        self.assertIn("progress.Value = 0", selection)
        self.assertIn("No verified download selected", selection)
        self.assertIn("else ShowReleaseListStatus();", selection)
        for terminal in ("Check failed — no package downloaded", "Check cancelled — no package downloaded",
                         "Download failed — no verified package", "Download cancelled — no verified package"):
            self.assertIn(terminal, ui)
        cancel = ui.split("internal void CancelOperation()", 1)[1].split("private bool DownloadMatchesSelection()", 1)[0]
        self.assertLess(cancel.index('SetStatus("Cancelling'), cancel.index("captured.Cancel();"))

    def test_runtime_checks_are_fake_and_offline(self):
        checks = (ROOT / "tests" / "UpdateUiChecks.cs").read_text(encoding="utf-8")
        self.assertIn("private sealed class FakeFeed : IUpdateFeed", checks)
        self.assertIn("private sealed class FakeInstaller : IUpdateInstaller", checks)
        self.assertIn("TaskCompletionSource", checks)
        self.assertNotIn("new UpdateFeedClient", checks)
        self.assertNotIn("new UpdateInstaller", checks)
        self.assertNotIn("UpdatePreferences.Channel =", checks)
        self.assertNotRegex(checks, r"File\.(Write|Delete)|Directory\.Create")
        self.assertIn("UpdateUiChecks.Run(passed);", (ROOT / "tests" / "UiWorkflowChecks.cs").read_text(encoding="utf-8"))

    def test_runtime_async_checks_keep_a_real_ui_message_loop(self):
        checks = (ROOT / "tests" / "UpdateUiChecks.cs").read_text(encoding="utf-8")
        self.assertIn("Application.Run(loopHost);", checks)
        self.assertIn("loopHost.BeginInvoke(new Action", checks)
        self.assertIn("Control.CheckForIllegalCrossThreadCalls = true", checks)
        self.assertIn("Control.CheckForIllegalCrossThreadCalls = previousCrossThreadChecks", checks)
        self.assertIn("SynchronizationContext.Current is WindowsFormsSynchronizationContext", checks)
        self.assertIn("ExceptionDispatchInfo.Capture", checks)
        self.assertIn("if (failure != null) failure.Throw();", checks)
        self.assertIn("CheckUiThreadMarshalling();", checks)
        self.assertIn("Task.Run(() => reporter.Report", checks)
        self.assertIn("Task.Run(() => checkResult.SetResult", checks)
        self.assertIn("Task.Run(() => downloadResult.SetResult", checks)
        self.assertIn("Interlocked.CompareExchange(ref wrongThread, 0, 0) == 0", checks)


if __name__ == "__main__":
    unittest.main()
