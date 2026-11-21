using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Synthetic/temp-only checks: no installs, elevation, registry writes, application exits, or network.</summary>
    internal static class UpdateApplyChecks
    {
        public static void Run(Action<string> passed)
        {
            string root = Path.Combine(Path.GetTempPath(), "spi-update-checks-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            try
            {
                UpdateVersion version;
                Require(UpdateVersion.TryParse("9.8.7", out version), "Synthetic version could not be parsed.");
                CheckArchiveGuards(root, version);
                passed("Update ZIP guards reject traversal, alternate data streams, duplicate paths, link attributes, unsupported files and oversized entries.");
                CheckPackageVariants(root, version);
                passed("Core-only portable packages and two empty upgrade-compatibility anchors are accepted; partial legacy packages still fail validation.");
                CheckPackage(root, version);
                passed("Apply revalidates SHA-256, exact repository/asset, version, package size and installed/portable kind before handoff.");
                CheckBusyGuards();
                passed("Updater refuses unfinished tool windows and active background workers without closing or killing them.");
                CheckSettings(root, version);
                CheckInterleavedPreferences(root);
                passed("Preference snapshots and version-directory migration preserve whitelisted paths/Excel options, retain unknown destination fields, reject invalid XML/paths and exclude secrets from backups.");
                CheckLegacyChannels(root);
                passed("Previous updater channels migrate once without enabling automation; canonical preferences win, invalid or locked legacy files warn safely, and original files remain untouched.");
                CheckRollback(root, version);
                passed("Portable copy and settings failures restore replaced files, remove only newly created app-owned files and retain unrelated user files plus backups.");
                CheckRealExecutable(root);
                passed("The actual core-only application/configuration passes strict extraction, version and SHA-256 verification while preserving existing legacy and user files.");
            }
            finally
            {
                // This exact GUID directory was created above solely for these synthetic checks.
                Directory.Delete(root, true);
            }
        }

        private static IList<KeyValuePair<string, string>> Entries(UpdateVersion version)
        {
            string name = "portable/SPI_Data_Comp_Tool_" + version + ".exe";
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(name, "synthetic-new-binary"),
                new KeyValuePair<string, string>(name + ".config", "<configuration/>"),
                new KeyValuePair<string, string>("portable/PNID_Reports/Source/instrument.txt", "synthetic source template"),
                new KeyValuePair<string, string>("portable/PNID_Reports/Target_IPEP/instrument.txt", "synthetic target template"),
                new KeyValuePair<string, string>("windows-x64/SPI_Data_Comp_Tool_" + version + ".exe", "ignored synthetic setup"),
            };
        }

        private static string Archive(string root, IList<KeyValuePair<string, string>> entries)
        {
            string path = Path.Combine(root, Guid.NewGuid().ToString("N") + ".zip");
            using (var file = new FileStream(path, FileMode.CreateNew))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Create))
                foreach (var entry in entries)
                    using (var stream = zip.CreateEntry(entry.Key).Open())
                    using (var writer = new StreamWriter(stream, new UTF8Encoding(false))) writer.Write(entry.Value);
            return path;
        }

        private static void CheckArchiveGuards(string root, UpdateVersion version)
        {
            string good = Archive(root, Entries(version));
            var selected = UpdateApplyWorker.ValidatePortableArchive(good, version, "MySPI.exe");
            Require(selected.Count == 4 && selected.Any(x => x.RelativePath == "MySPI.exe.config") &&
                selected.All(x => !x.RelativePath.StartsWith("windows-x64", StringComparison.Ordinal)), "Portable mapping selected the installer or lost the current executable name.");
            foreach (string unsafeName in new[] { "portable/../personal.txt", "/portable/personal.txt", "portable\\personal.txt",
                "portable/SPI.exe:stream", "portable/CON.txt", "portable/other-user-file.txt", "portable/PNID_Reports/Source/extra.txt" })
            {
                var entries = Entries(version); entries.Add(new KeyValuePair<string, string>(unsafeName, "synthetic"));
                string path = Archive(root, entries);
                Expect<InvalidDataException>(() => UpdateApplyWorker.ValidatePortableArchive(path, version, "MySPI.exe"), "Unsafe archive name was accepted.");
            }
            var duplicate = Entries(version);
            duplicate.Add(new KeyValuePair<string, string>(duplicate[0].Key.ToUpperInvariant(), "duplicate"));
            string duplicateZip = Archive(root, duplicate);
            Expect<InvalidDataException>(() => UpdateApplyWorker.ValidatePortableArchive(duplicateZip, version, "MySPI.exe"), "Case-colliding duplicate was accepted.");
            string symlink = Archive(root, Entries(version));
            PatchCentralUInt32(symlink, 38, 0xA1FF0000);
            Expect<InvalidDataException>(() => UpdateApplyWorker.ValidatePortableArchive(symlink, version, "MySPI.exe"), "Symlink metadata was accepted.");
            string oversized = Archive(root, Entries(version));
            PatchCentralUInt32(oversized, 24, 268435457);
            Expect<InvalidDataException>(() => UpdateApplyWorker.ValidatePortableArchive(oversized, version, "MySPI.exe"), "Oversized ZIP entry was accepted.");
            string understated = Archive(root, Entries(version));
            PatchCentralUInt32(understated, 24, 1);
            string untouched = Path.Combine(root, "understated-target"); Directory.CreateDirectory(untouched);
            string extraction = Path.Combine(root, "understated-stage"); Directory.CreateDirectory(extraction);
            Expect<InvalidDataException>(() => UpdateApplyWorker.ApplyPortable(understated, untouched, "MySPI.exe", version, extraction, null, null, (path, v) => { }),
                "Actual decompressed bytes were allowed to exceed the declared ZIP length.");
            Require(!File.Exists(Path.Combine(untouched, "MySPI.exe")), "An invalid compressed stream reached the destination.");
            var missing = Entries(version); missing.RemoveAt(3);
            string incomplete = Archive(root, missing);
            Expect<InvalidDataException>(() => UpdateApplyWorker.ValidatePortableArchive(incomplete, version, "MySPI.exe"), "A partial legacy package without both compatibility anchors was accepted.");
        }

        private static void CheckPackageVariants(string root, UpdateVersion version)
        {
            const string legacyPrefix = "portable/PNID_Reports/";
            var coreEntries = Entries(version).Where(entry => !entry.Key.StartsWith(legacyPrefix, StringComparison.Ordinal)).ToList();
            var core = UpdateApplyWorker.ValidatePortableArchive(Archive(root, coreEntries), version, "MySPI.exe");
            Require(core.Count == 2 && core.Any(file => file.RelativePath == "MySPI.exe")
                && core.Any(file => file.RelativePath == "MySPI.exe.config"),
                "A core-only portable package was rejected or mapped unexpected files.");
            foreach (string required in new[] { ".exe", ".exe.config" })
            {
                string name = "portable/SPI_Data_Comp_Tool_" + version + required;
                var missing = coreEntries.Where(entry => entry.Key != name).ToList();
                Expect<InvalidDataException>(() => UpdateApplyWorker.ValidatePortableArchive(Archive(root, missing), version, "MySPI.exe"),
                    "A core-only package without its executable or configuration was accepted.");
            }

            var compatibilityEntries = Entries(version).Select(entry => entry.Key.StartsWith(legacyPrefix, StringComparison.Ordinal)
                ? new KeyValuePair<string, string>(entry.Key, string.Empty) : entry).ToList();
            string compatibilityPackage = Archive(root, compatibilityEntries);
            var compatible = UpdateApplyWorker.ValidatePortableArchive(compatibilityPackage, version, "MySPI.exe");
            Require(compatible.Count == 4, "The portable compatibility package lost one of the required legacy paths.");
            using (var file = File.OpenRead(compatibilityPackage))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                var anchors = zip.Entries.Where(entry => entry.FullName.StartsWith(legacyPrefix, StringComparison.Ordinal)).ToList();
                Require(anchors.Count == 2 && anchors.All(entry => entry.Length == 0)
                    && anchors.Any(entry => entry.FullName == legacyPrefix + "Source/instrument.txt")
                    && anchors.Any(entry => entry.FullName == legacyPrefix + "Target_IPEP/instrument.txt"),
                    "The old-updater compatibility paths must be exactly two empty files, without SQL content.");
            }
        }

        private static void PatchCentralUInt32(string file, int relativeOffset, uint value)
        {
            byte[] bytes = File.ReadAllBytes(file);
            for (int index = 0; index <= bytes.Length - 46; index++)
                if (BitConverter.ToUInt32(bytes, index) == 0x02014B50)
                {
                    Array.Copy(BitConverter.GetBytes(value), 0, bytes, index + relativeOffset, 4);
                    File.WriteAllBytes(file, bytes);
                    return;
                }
            throw new InvalidOperationException("Synthetic ZIP central record not found.");
        }

        private static void CheckPackage(string root, UpdateVersion version)
        {
            string path = Archive(root, Entries(version));
            string name = "SPI_Data_Comp_Tool_" + version + ".zip";
            string prefix = "https://github.com/krishnabhunia/SPI-Data-Comp-Tool/releases/download/v" + version + "/";
            var package = new DownloadedUpdate
            {
                Kind = UpdateInstallKind.Portable, FilePath = path, Sha256 = UpdateApplyWorker.HashFile(path),
                Release = new UpdateRelease
                {
                    Tag = "v" + version, Version = version,
                    Portable = new UpdateAsset { Name = name, Size = new FileInfo(path).Length, DownloadUri = new Uri(prefix + name) },
                    Checksums = new UpdateAsset { Name = name + ".sha256", DownloadUri = new Uri(prefix + name + ".sha256"), Size = 100 },
                },
            };
            Require(UpdateInstaller.ValidatePackage(package, UpdateInstallKind.Portable).ToString() == version.ToString(), "A valid synthetic package was rejected.");
            Expect<InvalidDataException>(() => UpdateInstaller.ValidatePackage(package, UpdateInstallKind.Installed), "Portable package was accepted for an installed app.");
            string validHash = package.Sha256;
            package.Sha256 = new string('0', 64);
            Expect<InvalidDataException>(() => UpdateInstaller.ValidatePackage(package, UpdateInstallKind.Portable), "Hash mismatch was accepted.");
            package.Sha256 = validHash;
            package.Release.Portable.DownloadUri = new Uri("https://github.com/not-the-owner/SPI-Data-Comp-Tool/releases/download/v" + version + "/" + name);
            Expect<InvalidDataException>(() => UpdateInstaller.ValidatePackage(package, UpdateInstallKind.Portable), "A different repository was accepted.");
        }

        private sealed class BusyHolder : IDisposable
        {
            internal readonly BackgroundWorker Worker = new BackgroundWorker();
            public void Dispose() { Worker.Dispose(); }
        }

        private static void CheckBusyGuards()
        {
            UpdateApplyWorker.EnsureNoOtherInstance(Assembly.GetExecutingAssembly().Location, Process.GetCurrentProcess().Id);
            string reason;
            Require(UpdateInstaller.CanInstallForms(new Form[0], out reason), "An empty tool list was incorrectly blocked.");
            using (var unfinished = new Form())
                Require(!UpdateInstaller.CanInstallForms(new[] { unfinished }, out reason) && !string.IsNullOrWhiteSpace(reason), "An unfinished form was not blocked.");
            string directory = Path.Combine(Path.GetTempPath(), "synthetic-spi-directory");
            string identity = Assembly.GetExecutingAssembly().GetName().Name;
            Require(UpdateApplyWorker.IsSharedInstallationProcess(directory, Path.Combine(directory, "another-old-version.exe"), identity),
                "A differently named SPI executable sharing the application folder was not blocked.");
            Require(!UpdateApplyWorker.IsSharedInstallationProcess(directory, Path.Combine(directory, "unrelated.exe"), "UnrelatedApplication"),
                "An unrelated executable was classified as SPI.");
            Require(!UpdateApplyWorker.IsSharedInstallationProcess(directory, Path.Combine(directory + "-different", "SPI.exe"), identity),
                "A separate portable installation was incorrectly classified as sharing this folder.");
            using (var holder = new BusyHolder())
            using (var gate = new ManualResetEvent(false))
            {
                Require(!UpdateInstaller.HasBusyWorker(holder), "An idle background worker was blocked.");
                holder.Worker.DoWork += (sender, args) => gate.WaitOne();
                holder.Worker.RunWorkerAsync();
                try { Require(UpdateInstaller.HasBusyWorker(holder), "An active background worker was not detected."); }
                finally
                {
                    gate.Set();
                    var watch = Stopwatch.StartNew();
                    while (holder.Worker.IsBusy && watch.ElapsedMilliseconds < 5000) { Application.DoEvents(); Thread.Sleep(5); }
                    Require(!holder.Worker.IsBusy, "The synthetic background worker did not finish.");
                }
            }
        }

        private static void CheckSettings(string root, UpdateVersion version)
        {
            string local = Path.Combine(root, "local");
            string identity = Path.Combine(local, "SPI", "SPI_Data_Comp_Tool.exe_Url_synthetic");
            string source = Path.Combine(identity, "1.0.0.0", "user.config");
            string target = Path.Combine(identity, "9.8.7.0", "user.config");
            Directory.CreateDirectory(Path.GetDirectoryName(source)); Directory.CreateDirectory(Path.GetDirectoryName(target));
            string original = "<configuration><userSettings><SPI_Data_Comp_Tool.Properties.Settings>" +
                "<setting name=\"ProfilePathUser\" serializeAs=\"String\"><value>old-profile-path</value></setting>" +
                "<setting name=\"OtherPreference\" serializeAs=\"String\"><value>not-for-backup</value></setting>" +
                "</SPI_Data_Comp_Tool.Properties.Settings></userSettings></configuration>";
            File.WriteAllText(source, original); File.WriteAllText(target, original);
            var snapshot = new UpdatePreferenceSnapshot
            {
                ProfilePathUser = @"C:\Synthetic\not-read-profile.json", RuleFilePath = @"C:\Synthetic\not-read-rules.xlsx",
                EPPlus_Excel_Save = false, XL_Excel_Save = true, Channel = UpdateChannel.Beta,
            };
            string preferences = Path.Combine(root, "snapshot.json");
            UpdatePreferences.WriteSnapshot(preferences, snapshot);
            Require(UpdatePreferences.ReadSnapshot(preferences).Channel == UpdateChannel.Beta, "The channel preference was lost.");
            Require(!File.ReadAllText(preferences).Contains("Password"), "Snapshot contained an unexpected credential field.");
            string goodSnapshot = File.ReadAllText(preferences);
            var tooLarge = new UpdatePreferenceSnapshot { ProfilePathUser = new string('\u4e2d', 30000), RuleFilePath = new string('\u4e2d', 30000) };
            Expect<InvalidDataException>(() => UpdatePreferences.WriteSnapshot(preferences, tooLarge), "An unreadably large UTF-8 snapshot was written.");
            Require(File.ReadAllText(preferences) == goodSnapshot, "A rejected snapshot overwrote the previous preferences.");
            string backup = Path.Combine(root, "settings-backup");
            var migration = UpdatePreferences.PrepareMigration(source, "1.0.0.0", version, snapshot, backup, local);
            Require(File.ReadAllText(target) == original, "Preparing a settings migration changed the destination.");
            migration.Apply();
            var document = XDocument.Load(target);
            var settings = document.Root.Element("userSettings").Element(UpdatePreferences.SettingsSection);
            Require(settings.Elements("setting").Single(x => (string)x.Attribute("name") == "RuleFilePath").Element("value").Value == snapshot.RuleFilePath,
                "Whitelisted settings were not migrated.");
            Require(File.ReadAllText(target).Contains("not-for-backup"), "An unrelated target preference was lost.");
            Require(!File.ReadAllText(Path.Combine(backup, "previous-target-preferences.json")).Contains("not-for-backup"), "Unknown settings were copied to a backup.");
            migration.Rollback();
            Require(File.ReadAllText(target) == original, "A canceled settings migration did not restore its original content.");
            Expect<InvalidDataException>(() => UpdatePreferences.PrepareMigration(Path.Combine(root, "outside", "1.0.0.0", "user.config"), "1.0.0.0", version, snapshot, backup, local), "An arbitrary settings path was accepted.");
            File.WriteAllText(target, "<!DOCTYPE configuration [<!ENTITY x SYSTEM 'file:///synthetic-not-read'>]><configuration>&x;</configuration>");
            Expect<XmlException>(() => UpdatePreferences.PrepareMigration(source, "1.0.0.0", version, snapshot, backup, local), "DTD settings were accepted.");
            File.WriteAllText(preferences, "{}");
            Expect<JsonException>(() => UpdatePreferences.ReadSnapshot(preferences), "An incomplete snapshot silently reset preferences.");
            Require(UpdatePreferences.InstallationIdentity(Path.Combine(root, "one")) != UpdatePreferences.InstallationIdentity(Path.Combine(root, "two")), "Portable installation settings identities collide.");
        }

        private static void CheckRollback(string root, UpdateVersion version)
        {
            string target = Path.Combine(root, "rollback-target");
            Directory.CreateDirectory(target);
            File.WriteAllText(Path.Combine(target, "MySPI.exe"), "original-binary");
            File.WriteAllText(Path.Combine(target, "MySPI.exe.config"), "original-config");
            File.WriteAllText(Path.Combine(target, "my-report.xlsx"), "unrelated-user-content");
            string package = Archive(root, Entries(version));
            string stage = Path.Combine(root, "rollback-stage"); Directory.CreateDirectory(stage);
            int copies = 0;
            Action<string, string> failingCopy = (source, destination) =>
            {
                if (++copies == 4) throw new IOException("Synthetic fourth-copy failure");
                UpdateApplyWorker.AtomicCopy(source, destination);
            };
            Expect<IOException>(() => UpdateApplyWorker.ApplyPortable(package, target, "MySPI.exe", version, stage, null, failingCopy, (path, v) => { }), "A synthetic replacement failure did not propagate.");
            Require(File.ReadAllText(Path.Combine(target, "MySPI.exe")) == "original-binary" && File.ReadAllText(Path.Combine(target, "MySPI.exe.config")) == "original-config",
                "The original application files were not restored.");
            Require(!File.Exists(Path.Combine(target, "PNID_Reports/Source/instrument.txt")), "A new app-owned file was left after rollback.");
            Require(File.ReadAllText(Path.Combine(target, "my-report.xlsx")) == "unrelated-user-content", "An unrelated user file changed.");
            Require(File.Exists(Path.Combine(stage, "backup/MySPI.exe")), "Recovery backup was not retained.");
            string settingsStage = Path.Combine(root, "settings-failure-stage"); Directory.CreateDirectory(settingsStage);
            Expect<IOException>(() => UpdateApplyWorker.ApplyPortable(package, target, "MySPI.exe", version, settingsStage,
                () => { throw new IOException("Synthetic migration failure"); }, null, (path, v) => { }), "Migration failure did not roll back the portable transaction.");
            Require(File.ReadAllText(Path.Combine(target, "MySPI.exe")) == "original-binary", "A settings failure left the new binary installed.");
        }

        private static void CheckInterleavedPreferences(string root)
        {
            string directory = Path.Combine(root, "interleaved-preferences");
            Directory.CreateDirectory(directory);
            string config = Path.Combine(directory, "user.config");
            string stable = Path.Combine(directory, "preferences.json");
            var stale = new UpdatePreferenceSnapshot { ProfilePathUser = "old-profile", RuleFilePath = "old-rules", Channel = UpdateChannel.Normal };
            UpdatePreferences.WriteSnapshot(stable, stale);
            File.WriteAllText(config, "<configuration><userSettings><SPI_Data_Comp_Tool.Properties.Settings>" +
                "<setting name=\"ProfilePathUser\"><value>new-profile-saved-by-A</value></setting>" +
                "<setting name=\"RuleFilePath\"><value>new-rules-saved-by-A</value></setting>" +
                "<setting name=\"EPPlus_Excel_Save\"><value>False</value></setting>" +
                "<setting name=\"XL_Excel_Save\"><value>True</value></setting>" +
                "</SPI_Data_Comp_Tool.Properties.Settings></userSettings></configuration>");
            UpdatePreferences.SavePersistedSnapshot(stable, config, stale, UpdateChannel.Beta);
            var passive = UpdatePreferences.SavePersistedSnapshot(stable, config, stale, null);
            Require(passive.ProfilePathUser == "new-profile-saved-by-A" && passive.RuleFilePath == "new-rules-saved-by-A" &&
                passive.Channel == UpdateChannel.Beta && !passive.EPPlus_Excel_Save && passive.XL_Excel_Save,
                "A passive stale instance overwrote a newer persisted preference or channel.");
            string previous = File.ReadAllText(stable);
            using (var held = new FileStream(stable + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                Expect<IOException>(() => UpdatePreferences.SavePersistedSnapshot(stable, config, stale, UpdateChannel.Normal), "A competing preferences writer was not blocked.");
            Require(File.ReadAllText(stable) == previous, "A blocked writer changed the last-good preferences file.");
            var explicitChoice = UpdatePreferences.SavePersistedSnapshot(stable, config, stale, UpdateChannel.Normal);
            Require(explicitChoice.Channel == UpdateChannel.Normal && explicitChoice.RuleFilePath == passive.RuleFilePath,
                "An explicit local channel selection did not preserve the latest saved paths.");
        }

        private static void CheckLegacyChannels(string root)
        {
            string directory = Path.Combine(root, "legacy-preferences");
            Directory.CreateDirectory(directory);
            string canonical = Path.Combine(directory, "preferences.json");
            string legacy = Path.Combine(directory, "updates.json");
            string warning;
            Require(UpdatePreferences.ResolveInitialChannel(canonical, legacy, out warning) == UpdateChannel.Normal && warning.Length == 0,
                "A missing previous preferences file did not leave the normal default without a warning.");
            foreach (var item in new[]
            {
                new KeyValuePair<string, UpdateChannel>("{\"Channel\":0,\"InstallSilently\":false}", UpdateChannel.Normal),
                new KeyValuePair<string, UpdateChannel>("{\"Channel\":1,\"InstallSilently\":true}", UpdateChannel.All),
                new KeyValuePair<string, UpdateChannel>("{\"InstallSilently\":true,\"Channel\":\"Beta\"}", UpdateChannel.All),
                new KeyValuePair<string, UpdateChannel>("{\"Channel\":\"Stable\"}", UpdateChannel.Normal),
            })
            {
                File.WriteAllText(legacy, item.Key);
                Require(UpdatePreferences.ResolveInitialChannel(canonical, legacy, out warning) == item.Value && warning.Length == 0,
                    "The previous stable/beta channel changed its effective filter during migration.");
                Require(File.ReadAllText(legacy) == item.Key && !File.Exists(canonical),
                    "Inspecting a previous channel changed either preferences file.");
            }
            foreach (string invalid in new[]
            {
                "{}", "null", "[]", "{", "{\"Channel\":2}", "{\"Channel\":null}", "{\"Channel\":1.0}",
                "{\"Channel\":0x1}", "{\"Channel\":01}",
                "{\"Channel\":\"All\"}", "{\"Channel\":0,\"Channel\":1}", "{\"Channel\":0} {\"Channel\":1}",
                "{\"Channel\":0,\"InstallSilently\":\"true\"}", "{\"Channel\":0,\"PendingFile\":\"never-run.exe\"}",
                "{\"Channel\":0,}", "{\"Channel\":/*not-json*/1}", "{'Channel':1}",
                new string(' ', UpdatePreferences.MaximumLegacyPreferenceBytes + 1),
            })
            {
                File.WriteAllText(legacy, invalid);
                Require(UpdatePreferences.ResolveInitialChannel(canonical, legacy, out warning) == UpdateChannel.Normal &&
                    warning.Contains("Normal") && warning.Contains("Updates"), "Invalid previous preferences did not report a safe fallback.");
                Require(File.ReadAllText(legacy) == invalid && !File.Exists(canonical),
                    "Rejecting previous preferences modified their source or created an implicit snapshot.");
            }
            File.WriteAllBytes(legacy, new byte[] { 0xFF, 0xFF, 0xFF });
            Require(UpdatePreferences.ResolveInitialChannel(canonical, legacy, out warning) == UpdateChannel.Normal && warning.Length > 0,
                "Malformed preference text did not report a safe fallback.");

            const string beta = "{\"Channel\":1,\"InstallSilently\":true}";
            File.WriteAllText(legacy, beta);
            using (var held = new FileStream(legacy, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                Require(UpdatePreferences.ResolveInitialChannel(canonical, legacy, out warning) == UpdateChannel.Normal && warning.Length > 0,
                    "Locked previous preferences disabled initialization instead of falling back safely.");
            string directoryCollision = Path.Combine(directory, "not-a-file");
            Directory.CreateDirectory(directoryCollision);
            Require(UpdatePreferences.ResolveInitialChannel(canonical, directoryCollision, out warning) == UpdateChannel.Normal && warning.Length > 0,
                "A previous preferences directory collision did not warn safely.");
            Expect<InvalidDataException>(() => UpdatePreferences.ResolveInitialChannel(directoryCollision, legacy, out warning),
                "A canonical preferences directory collision incorrectly fell back to older preferences.");

            UpdateChannel imported = UpdatePreferences.ResolveInitialChannel(canonical, legacy, out warning);
            var snapshot = new UpdatePreferenceSnapshot { Channel = imported };
            UpdatePreferences.SavePersistedSnapshot(canonical, Path.Combine(directory, "absent-user.config"), snapshot, null);
            Require(UpdatePreferences.ReadSnapshot(canonical).Channel == UpdateChannel.All &&
                !File.ReadAllText(canonical).Contains("InstallSilently") && File.ReadAllText(legacy) == beta,
                "Migration lost stable+beta semantics, imported automation, or modified the previous file.");
            // A deliberate new Beta-only choice differs from the old Beta meaning; never remap it.
            snapshot.Channel = UpdateChannel.Beta;
            UpdatePreferences.WriteSnapshot(canonical, snapshot);
            string canonicalText = File.ReadAllText(canonical);
            using (var held = new FileStream(legacy, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                Require(UpdatePreferences.ResolveInitialChannel(canonical, legacy, out warning) == UpdateChannel.Beta && warning.Length == 0,
                    "Existing canonical preferences did not win without reading the previous file.");
            Require(File.ReadAllText(canonical) == canonicalText, "Reading the existing canonical choice changed it.");
            var concurrent = UpdatePreferences.SavePersistedSnapshot(canonical, Path.Combine(directory, "absent-user.config"),
                new UpdatePreferenceSnapshot { Channel = imported }, null);
            Require(concurrent.Channel == UpdateChannel.Beta, "An imported fallback replaced a concurrently-created canonical choice.");
            using (var held = new FileStream(canonical, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                Expect<IOException>(() => UpdatePreferences.ResolveInitialChannel(canonical, legacy, out warning),
                    "A locked canonical snapshot silently fell back to previous preferences.");
            File.WriteAllText(canonical, "{}");
            Expect<JsonException>(() => UpdatePreferences.ResolveInitialChannel(canonical, legacy, out warning),
                "A malformed canonical snapshot silently fell back to previous preferences.");
        }

        private static void CheckRealExecutable(string root)
        {
            UpdateVersion version;
            Require(UpdateVersion.TryParse(AppVersion.Full, out version), "The actual build has no release version.");
            string executable = Assembly.GetExecutingAssembly().Location;
            string package = Path.Combine(root, "real-build.zip");
            using (var file = new FileStream(package, FileMode.CreateNew))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Create))
            {
                string name = "portable/SPI_Data_Comp_Tool_" + version + ".exe";
                foreach (var pair in new[] { new KeyValuePair<string, string>(name, executable), new KeyValuePair<string, string>(name + ".config", executable + ".config") })
                    using (var input = File.OpenRead(pair.Value))
                    using (var output = zip.CreateEntry(pair.Key).Open()) input.CopyTo(output);
            }
            string target = Path.Combine(root, "real-target"); string stage = Path.Combine(root, "real-stage");
            Directory.CreateDirectory(target); Directory.CreateDirectory(stage);
            File.WriteAllText(Path.Combine(target, "user-notes.txt"), "preserved");
            string legacyFolder = Path.Combine(target, "PNID_Reports", "Source");
            Directory.CreateDirectory(legacyFolder);
            string legacyTemplate = Path.Combine(legacyFolder, "instrument.txt");
            string userLegacyFile = Path.Combine(legacyFolder, "my-saved-notes.txt");
            File.WriteAllText(legacyTemplate, "existing legacy template");
            File.WriteAllText(userLegacyFile, "unrelated user content in an older folder");
            string local = Path.Combine(root, "real-local");
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string sourceConfig = Path.Combine(local, "SPI_Data_Comp_Tool.exe_Url_real_fixture", currentVersion, "user.config");
            Directory.CreateDirectory(Path.GetDirectoryName(sourceConfig));
            File.WriteAllText(sourceConfig, "<configuration><userSettings><SPI_Data_Comp_Tool.Properties.Settings>" +
                "<setting name=\"ProfilePathUser\" serializeAs=\"String\"><value>preserved-profile-selection</value></setting>" +
                "</SPI_Data_Comp_Tool.Properties.Settings></userSettings></configuration>");
            var preferences = new UpdatePreferenceSnapshot { ProfilePathUser = "preserved-profile-selection", RuleFilePath = "preserved-rules-selection" };
            var migration = UpdatePreferences.PrepareMigration(sourceConfig, currentVersion, version, preferences, stage, local);
            bool settingsMigrated = false;
            UpdateApplyWorker.ApplyPortable(package, target, "RetainedPortableName.exe", version, stage, () => { migration.Apply(); settingsMigrated = true; });
            string applied = Path.Combine(target, "RetainedPortableName.exe");
            Require(settingsMigrated && UpdateApplyWorker.HashFile(applied) == UpdateApplyWorker.HashFile(executable), "The real executable was not applied correctly.");
            Require(File.ReadAllText(Path.Combine(target, "user-notes.txt")) == "preserved", "The real apply changed a user file.");
            Require(File.ReadAllText(legacyTemplate) == "existing legacy template"
                && File.ReadAllText(userLegacyFile) == "unrelated user content in an older folder",
                "A core-only apply removed or overwrote files left in an older application folder.");
            Require(File.ReadAllText(sourceConfig).Contains("preserved-profile-selection") && File.ReadAllText(sourceConfig).Contains("preserved-rules-selection"),
                "The actual application apply did not preserve its migrated selections.");
            UpdateVersion wrong;
            Require(UpdateVersion.TryParse(version.ToString() == "0.0.0" ? "0.0.1" : "0.0.0", out wrong), "Cannot construct mismatch version.");
            Expect<InvalidDataException>(() => UpdateApplyWorker.VerifyApplicationVersion(applied, wrong), "A mismatched actual executable version was accepted.");
            string wrongBeta = string.Format("{0}.{1}.{2}-beta{3}", version.Major, version.Minor, version.Patch, version.Beta == 999 ? "998" : "999");
            Require(UpdateVersion.TryParse(wrongBeta, out wrong), "Cannot construct mismatch beta version.");
            Expect<InvalidDataException>(() => UpdateApplyWorker.VerifyApplicationVersion(applied, wrong), "A mismatched same-base beta product version was accepted.");
        }

        private static void Expect<T>(Action action, string message) where T : Exception
        {
            try { action(); }
            catch (T) { return; }
            throw new InvalidOperationException(message);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }
    }
}
