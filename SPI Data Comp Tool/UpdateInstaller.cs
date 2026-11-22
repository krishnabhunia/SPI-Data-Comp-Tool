using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace SPI_Data_Comp_Tool
{
    internal sealed class UpdateInstaller : IUpdateInstaller
    {
        internal const string UninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{DF6AFF7A-0A5A-4F1E-BF54-F5301F101D26}_is1";
        internal const string InstalledExecutableName = "SPI Data Comp Tool.exe";
        private readonly string detectionError;
        public UpdateInstallation Installation { get; private set; }
        internal static bool IsApplying { get; private set; }

        public UpdateInstaller()
        {
            string error;
            Installation = DetectInstallation(Assembly.GetExecutingAssembly().Location, out error);
            detectionError = error;
        }

        internal static UpdateInstallation DetectInstallation(string executable, out string error)
        {
            error = null;
            string directory = Path.GetDirectoryName(Path.GetFullPath(executable));
            var result = new UpdateInstallation { Kind = UpdateInstallKind.Portable, Directory = directory, ExecutablePath = executable };
            var matches = new List<bool>();
            try
            {
                foreach (RegistryHive hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
                    foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
                    {
                        if (view == RegistryView.Registry64 && !Environment.Is64BitOperatingSystem) continue;
                        using (var key = RegistryKey.OpenBaseKey(hive, view))
                        using (var uninstall = key.OpenSubKey(UninstallKey, false))
                        {
                            string installedAt = uninstall == null ? null : uninstall.GetValue("InstallLocation") as string;
                            if (!string.IsNullOrWhiteSpace(installedAt) && SameDirectory(installedAt, directory))
                                matches.Add(hive == RegistryHive.LocalMachine);
                        }
                    }
                if (matches.Distinct().Count() > 1) error = "Both per-user and all-users installations claim this folder. Resolve the installation registration first.";
                else if (matches.Count > 0)
                {
                    if (!string.Equals(Path.GetFileName(executable), InstalledExecutableName, StringComparison.OrdinalIgnoreCase))
                        error = "An installed application was renamed. Use the registered application executable to update it.";
                    else
                    {
                        result.Kind = UpdateInstallKind.Installed;
                        result.IsMachineInstall = matches[0];
                    }
                }
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is System.Security.SecurityException || ex is IOException || ex is ArgumentException)
            {
                error = "Windows installation registration could not be safely inspected. Update manually or correct registry access.";
            }
            return result;
        }

        internal static bool SameDirectory(string left, string right)
        {
            return string.Equals(Path.GetFullPath(left).TrimEnd('\\', '/'), Path.GetFullPath(right).TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase);
        }

        public bool CanInstall(out string reason)
        {
            if (detectionError != null) { reason = detectionError; return false; }
            if (!UpdatePreferences.IsInitialized) { reason = "Preferences could not be backed up safely. Restart after correcting the startup preferences warning."; return false; }
            if (IsApplying) { reason = "An update is already being handed off."; return false; }
            if (!CanInstallForms(Application.OpenForms.Cast<Form>(), out reason)) return false;
            try
            {
                UpdateApplyWorker.ValidateTargetDirectory(Installation.Directory);
                UpdateApplyWorker.EnsureNoOtherInstance(Installation.ExecutablePath, Process.GetCurrentProcess().Id);
                if (Installation.Kind == UpdateInstallKind.Installed && !Environment.Is64BitOperatingSystem)
                    throw new InvalidOperationException("The installer requires 64-bit Windows.");
            }
            catch (Exception ex)
            {
                reason = Common_Data.RedactConnectionCredentials(ex.Message);
                return false;
            }
            reason = null;
            return true;
        }

        internal static bool CanInstallForms(IEnumerable<Form> forms, out string reason)
        {
            foreach (Form form in forms)
            {
                var workspace = form as WorkspaceForm;
                if (workspace != null)
                {
                    // Inspect every loaded page, including hidden tabs. A hidden profile or data
                    // task still holds user work and must block replacement of the executable.
                    // The main window's grouped shell lists every task; judge it by the pages it has loaded.
                    if ((!workspace.Grouped && workspace.Workspace != WorkspaceKind.Settings) ||
                        !CanInstallForms(workspace.LoadedPages.Cast<Form>(), out reason))
                    {
                        reason = "Close all data workspaces and profile tasks before installing an update.";
                        return false;
                    }
                    if (HasBusyWorker(workspace))
                    {
                        reason = "A background operation is still running. Wait for it to finish before updating.";
                        return false;
                    }
                    continue;
                }
                // AboutBox1 is embedded in Settings > About and holds no work.
                if (!(form is MainForm) && !(form is User_Settings) && !(form is AboutBox1))
                {
                    reason = "Save your work and close all task windows before installing an update.";
                    return false;
                }
                if (HasBusyWorker(form))
                {
                    reason = "A background operation is still running. Wait for it to finish before updating.";
                    return false;
                }
            }
            reason = null;
            return true;
        }

        internal static bool HasBusyWorker(object instance)
        {
            for (Type type = instance.GetType(); type != null; type = type.BaseType)
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (typeof(BackgroundWorker).IsAssignableFrom(field.FieldType))
                    {
                        var worker = field.GetValue(instance) as BackgroundWorker;
                        if (worker != null && worker.IsBusy) return true;
                    }
                    if (typeof(IContainer).IsAssignableFrom(field.FieldType))
                    {
                        var container = field.GetValue(instance) as IContainer;
                        if (container != null && container.Components.OfType<BackgroundWorker>().Any(x => x.IsBusy)) return true;
                    }
                }
            var control = instance as Control;
            return control != null && control.Controls.Cast<Control>().Any(child => HasBusyWorker(child));
        }

        internal static UpdateVersion ValidatePackage(DownloadedUpdate package, UpdateInstallKind expectedKind)
        {
            if (package == null || package.Release == null || package.Release.Version == null || package.Kind != expectedKind)
                throw new InvalidDataException("The update package does not match this installation type.");
            UpdateVersion version;
            if (!UpdateVersion.TryParse(package.Release.Tag, out version) || package.Release.Tag != "v" + version ||
                package.Release.Version.ToString() != version.ToString()) throw new InvalidDataException("The release version is inconsistent.");
            string baseName = "SPI_Data_Comp_Tool_" + version;
            var asset = expectedKind == UpdateInstallKind.Portable ? package.Release.Portable : package.Release.Installer;
            string expectedName = baseName + (expectedKind == UpdateInstallKind.Portable ? ".zip" : "_x64_setup.exe");
            ValidateAsset(asset, package.Release.Tag, expectedName);
            ValidateAsset(package.Release.Checksums, package.Release.Tag, baseName + ".zip.sha256");
            if (string.IsNullOrWhiteSpace(package.FilePath) || !File.Exists(package.FilePath)) throw new FileNotFoundException("The verified download is no longer available.");
            UpdateApplyWorker.RejectReparsePoints(package.FilePath);
            long length = new FileInfo(package.FilePath).Length;
            if (length <= 0 || length > UpdateApplyWorker.MaximumPackageBytes || length != asset.Size)
                throw new InvalidDataException("The downloaded package size changed.");
            UpdateApplyWorker.VerifyHash(package.FilePath, package.Sha256);
            return version;
        }

        private static void ValidateAsset(UpdateAsset asset, string tag, string expectedName)
        {
            if (asset == null || asset.Name != expectedName || asset.DownloadUri == null || !asset.DownloadUri.IsAbsoluteUri)
                throw new InvalidDataException("The release asset is not a supported SPI package.");
            Uri uri = asset.DownloadUri;
            if (uri.Scheme != Uri.UriSchemeHttps || !string.Equals(uri.Host, "github.com", StringComparison.OrdinalIgnoreCase) ||
                !uri.IsDefaultPort || uri.UserInfo.Length != 0 || uri.Query.Length != 0 || uri.Fragment.Length != 0 ||
                uri.AbsolutePath != "/krishnabhunia/SPI-Data-Comp-Tool/releases/download/" + tag + "/" + expectedName)
                throw new InvalidDataException("The release asset does not belong to the approved GitHub repository.");
        }

        public void InstallAndRestart(DownloadedUpdate package, IWin32Window owner)
        {
            string reason;
            if (!CanInstall(out reason)) throw new InvalidOperationException(reason);
            var version = ValidatePackage(package, Installation.Kind);
            UpdatePreferences.SaveCurrent();
            string stagingRoot = Path.Combine(UpdatePreferences.GetStorageDirectory(Installation.Directory), "staging");
            UpdateApplyWorker.RejectReparsePoints(stagingRoot);
            string stage = Path.Combine(stagingRoot, Guid.NewGuid().ToString("N"));
            if (Directory.Exists(stage)) throw new IOException("The update staging folder already exists.");
            Directory.CreateDirectory(stage);
            string stagedPackage = Path.Combine(stage, Path.GetFileName(package.FilePath));
            string expectedPackageName = "SPI_Data_Comp_Tool_" + version + (package.Kind == UpdateInstallKind.Portable ? ".zip" : "_x64_setup.exe");
            if (Path.GetFileName(stagedPackage) != expectedPackageName) throw new InvalidDataException("The download filename changed.");
            File.Copy(package.FilePath, stagedPackage, false);
            UpdateApplyWorker.VerifyHash(stagedPackage, package.Sha256);
            string helper = Path.Combine(stage, UpdateApplyWorker.HelperName);
            File.Copy(Assembly.GetExecutingAssembly().Location, helper, false);
            File.Copy(Assembly.GetExecutingAssembly().Location + ".config", helper + ".config", false);
            var snapshot = UpdatePreferences.Capture();
            UpdatePreferences.WriteSnapshot(Path.Combine(stage, "settings-before-update.json"), snapshot);
            var manifest = new UpdateApplyManifest
            {
                Kind = Installation.Kind, TargetDirectory = Installation.Directory, TargetExecutable = Installation.ExecutablePath,
                IsMachineInstall = Installation.IsMachineInstall, PackageFileName = expectedPackageName, PackageSha256 = package.Sha256,
                Tag = package.Release.Tag, ParentProcessId = Process.GetCurrentProcess().Id,
                ParentStartUtcTicks = Process.GetCurrentProcess().StartTime.ToUniversalTime().Ticks,
                HelperSha256 = UpdateApplyWorker.HashFile(helper), CreatedUtc = DateTime.UtcNow,
                CurrentAssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                SourceUserConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath,
                Preferences = snapshot,
            };
            string manifestPath = Path.Combine(stage, "apply.json");
            UpdatePreferences.AtomicWrite(manifestPath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
            // Validate configuration and archive before closing the running application.
            UpdateApplyWorker.Preflight(stage, manifest);
            if (!CanInstall(out reason)) throw new InvalidOperationException(reason);
            using (var child = Process.Start(new ProcessStartInfo(helper, "--apply-update \"" + manifestPath + "\"")
            { UseShellExecute = false, WorkingDirectory = stage }))
            {
                try
                {
                    if (child == null) throw new IOException("The update helper did not start.");
                    var watch = Stopwatch.StartNew();
                    while (!File.Exists(Path.Combine(stage, "ready")))
                    {
                        if (child.HasExited || watch.ElapsedMilliseconds > 10000) throw new IOException("The update helper did not acknowledge safe startup. The application remains open.");
                        Thread.Sleep(50);
                    }
                    IsApplying = true;
                    var canceled = new CancelEventArgs();
                    Application.Exit(canceled);
                    if (canceled.Cancel) throw new InvalidOperationException("Closing was canceled. No update will be applied.");
                }
                catch
                {
                    File.WriteAllText(Path.Combine(stage, "cancel"), "Application shutdown was canceled.");
                    IsApplying = false;
                    throw;
                }
            }
        }
    }
}
