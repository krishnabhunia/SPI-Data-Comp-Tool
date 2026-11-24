using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace SPI_Data_Comp_Tool
{
    [JsonObject(ItemRequired = Required.Always)]
    internal sealed class UpdateApplyManifest
    {
        public int Schema { get; set; } = 1;
        public UpdateInstallKind Kind { get; set; }
        public string TargetDirectory { get; set; }
        public string TargetExecutable { get; set; }
        public bool IsMachineInstall { get; set; }
        public string PackageFileName { get; set; }
        public string PackageSha256 { get; set; }
        public string HelperSha256 { get; set; }
        public string Tag { get; set; }
        public int ParentProcessId { get; set; }
        public long ParentStartUtcTicks { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string CurrentAssemblyVersion { get; set; }
        public string SourceUserConfig { get; set; }
        public UpdatePreferenceSnapshot Preferences { get; set; }
    }

    internal sealed class UpdatePortableFile
    {
        internal string ArchiveName;
        internal string RelativePath;
    }

    internal static class UpdateApplyWorker
    {
        internal const string HelperName = "SPI.UpdateApply.exe";
        internal const long MaximumPackageBytes = 536870912;
        private const long MaximumEntryBytes = 268435456;
        private const long MaximumExpandedBytes = 805306368;
        // Older releases shipped these files. Keep their exact allowlist for downgrade support;
        // current builds contain no SQL templates or code that executes them.
        private static readonly string[] LegacyTemplateNames =
        {
            "Source/drawing.txt", "Source/equipment.txt", "Source/instrument.txt", "Source/nozzle.txt", "Source/opc.txt",
            "Source/piperun.txt", "Source/piping_component.txt", "Source/PlantItem.txt", "Source/PlantItem_Antwerp.txt", "Source/representation.txt",
            "Target_IPEP/drawing.txt", "Target_IPEP/equipment.txt", "Target_IPEP/instrument.txt", "Target_IPEP/nozzle.txt", "Target_IPEP/opc.txt",
            "Target_IPEP/piperun.txt", "Target_IPEP/piping_component.txt", "Target_IPEP/PlantItem.txt", "Target_IPEP/representation.txt",
        };

        public static bool TryRun(string[] args)
        {
            if (args == null || !args.Any(x => string.Equals(x, "--apply-update", StringComparison.OrdinalIgnoreCase))) return false;
            string stage = null;
            try
            {
                if (args.Length != 2 || !string.Equals(args[0], "--apply-update", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("The update-helper arguments are invalid.");
                string manifestPath = Path.GetFullPath(args[1]);
                if (Path.GetFileName(manifestPath) != "apply.json") throw new InvalidDataException("Invalid update manifest filename.");
                RejectReparsePoints(manifestPath);
                var manifest = JsonConvert.DeserializeObject<UpdateApplyManifest>(UpdatePreferences.ReadBoundedText(manifestPath, 131072),
                    new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error, MaxDepth = 12 });
                string candidate = Path.GetDirectoryName(manifestPath);
                ValidateManifest(candidate, manifest, true);
                stage = candidate;
                Preflight(stage, manifest);
                // Pin and validate the original process before acknowledging readiness. Its exit
                // immediately after ready must not race a later PID lookup/StartTime access.
                using (var parent = OpenParent(manifest))
                {
                    File.WriteAllText(Path.Combine(stage, "ready"), "Validated; waiting for the original application to close.");
                    WaitForParent(stage, parent);
                }
                EnsureNoOtherInstance(manifest.TargetExecutable, Process.GetCurrentProcess().Id);
                ValidateOriginalTarget(manifest);
                RejectReparsePoints(stage);
                var version = ParseTag(manifest.Tag);
                // Keep a read-only handle denying writes/deletion through the complete apply, not
                // merely during an earlier hash check (which would leave a substitution window).
                using (var packageLock = new FileStream(Path.Combine(stage, manifest.PackageFileName), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // Recheck registry kind/scope and original binary after waiting, too.
                    Preflight(stage, manifest);
                    var settings = UpdatePreferences.PrepareMigration(manifest.SourceUserConfig, manifest.CurrentAssemblyVersion,
                        version, manifest.Preferences, stage);
                    WriteLog(stage, "Starting " + manifest.Kind + " update to " + manifest.Tag + ".");
                    if (manifest.Kind == UpdateInstallKind.Portable)
                    {
                        ApplyPortable(Path.Combine(stage, manifest.PackageFileName), manifest.TargetDirectory,
                            Path.GetFileName(manifest.TargetExecutable), version, stage, settings.Apply);
                    }
                    else if (manifest.Kind == UpdateInstallKind.Msi) ApplyMsi(stage, manifest, version, settings);
                    else ApplyInstalled(stage, manifest, version, settings);
                }
                VerifyApplicationVersion(manifest.TargetExecutable, version);
                File.WriteAllText(Path.Combine(stage, "result.txt"), "Applied " + manifest.Tag + ". Backups have been retained.");
                Process.Start(new ProcessStartInfo(manifest.TargetExecutable) { UseShellExecute = true, WorkingDirectory = manifest.TargetDirectory });
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;
                string message = Common_Data.RedactConnectionCredentials(ex.Message);
                if (stage != null)
                {
                    try
                    {
                        WriteLog(stage, "Update failed: " + message);
                        File.WriteAllText(Path.Combine(stage, "result.txt"), "Update did not complete. " + message);
                    }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
                MessageBox.Show("The update did not complete. " + message +
                    (stage == null ? string.Empty : "\r\nBackups and diagnostics: " + stage) +
                    "\r\nNo process was forcibly closed. Review the result before restarting SPI manually.",
                    "SPI update", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return true;
        }

        internal static void Preflight(string stage, UpdateApplyManifest manifest)
        {
            ValidateManifest(stage, manifest, false);
            string package = Path.Combine(stage, manifest.PackageFileName);
            VerifyHash(package, manifest.PackageSha256);
            string detectionError;
            var detected = UpdateInstaller.DetectInstallation(manifest.TargetExecutable, out detectionError);
            if (detectionError != null || detected.Kind != manifest.Kind || detected.IsMachineInstall != manifest.IsMachineInstall)
                throw new InvalidOperationException("The installation type or scope changed; the update was not applied.");
            var version = ParseTag(manifest.Tag);
            if (manifest.Kind == UpdateInstallKind.Portable) ValidatePortableArchive(package, version, Path.GetFileName(manifest.TargetExecutable));
            else if (manifest.Kind == UpdateInstallKind.Msi)
            {
                if (!Environment.Is64BitOperatingSystem) throw new InvalidOperationException("This MSI requires 64-bit Windows.");
                RequireMsiPackage(package);
            }
            else
            {
                if (!Environment.Is64BitOperatingSystem) throw new InvalidOperationException("This installer requires 64-bit Windows.");
                var info = FileVersionInfo.GetVersionInfo(package);
                if (info.FileMajorPart != version.Major || info.FileMinorPart != version.Minor || info.FileBuildPart != version.Patch)
                    throw new InvalidDataException("The setup executable does not match its selected release version.");
            }
            // Reading/merging a bounded configuration must succeed before any application file changes.
            UpdatePreferences.PrepareMigration(manifest.SourceUserConfig, manifest.CurrentAssemblyVersion,
                version, manifest.Preferences, stage);
        }

        private static void ValidateManifest(string stage, UpdateApplyManifest manifest, bool executingHelper)
        {
            if (manifest == null || manifest.Schema != 1 || !Enum.IsDefined(typeof(UpdateInstallKind), manifest.Kind) ||
                manifest.ParentProcessId <= 0 || manifest.ParentStartUtcTicks <= 0 || manifest.CreatedUtc > DateTime.UtcNow.AddMinutes(5) ||
                manifest.CreatedUtc < DateTime.UtcNow.AddDays(-1)) throw new InvalidDataException("Invalid or expired update manifest.");
            ValidateTargetDirectory(manifest.TargetDirectory);
            string target = Path.GetFullPath(manifest.TargetExecutable);
            if (!UpdateInstaller.SameDirectory(Path.GetDirectoryName(target), manifest.TargetDirectory) ||
                !string.Equals(Path.GetExtension(target), ".exe", StringComparison.OrdinalIgnoreCase) || !File.Exists(target))
                throw new InvalidDataException("The target application path is invalid.");
            RejectReparsePoints(target);
            if (manifest.Kind != UpdateInstallKind.Installed && manifest.IsMachineInstall) throw new InvalidDataException("Portable and MSI scope cannot be elevated.");
            if (manifest.Kind != UpdateInstallKind.Portable && !string.Equals(Path.GetFileName(target), UpdateInstaller.InstalledExecutableName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Unexpected installed executable filename.");
            string stagingRoot = Path.Combine(UpdatePreferences.GetStorageDirectory(manifest.TargetDirectory), "staging");
            Guid identifier;
            if (!UpdateInstaller.SameDirectory(Path.GetDirectoryName(stage), stagingRoot) || !Guid.TryParseExact(Path.GetFileName(stage), "N", out identifier))
                throw new InvalidDataException("The helper is not in an application-owned staging folder.");
            RejectReparsePoints(stage);
            var version = ParseTag(manifest.Tag);
            string expected = UpdateFeedClient.GetPackageFileName(version, manifest.Kind);
            if (manifest.PackageFileName != expected || manifest.CurrentAssemblyVersion != Assembly.GetExecutingAssembly().GetName().Version.ToString())
                throw new InvalidDataException("The staged package/helper version is inconsistent.");
            string package = Path.Combine(stage, expected);
            RejectReparsePoints(package);
            if (!File.Exists(package) || new FileInfo(package).Length <= 0 || new FileInfo(package).Length > MaximumPackageBytes)
                throw new InvalidDataException("The staged package is unavailable or too large.");
            UpdatePreferences.ValidateSnapshot(manifest.Preferences);
            if (executingHelper && !string.Equals(Path.GetFullPath(Assembly.GetExecutingAssembly().Location),
                Path.Combine(stage, HelperName), StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("The apply helper must run from its own staging folder.");
            VerifyHash(Path.Combine(stage, HelperName), manifest.HelperSha256);
            ValidateOriginalTarget(manifest);
        }

        private static void ValidateOriginalTarget(UpdateApplyManifest manifest)
        {
            RejectReparsePoints(manifest.TargetExecutable);
            var original = AssemblyName.GetAssemblyName(manifest.TargetExecutable);
            if (original.Name != Assembly.GetExecutingAssembly().GetName().Name || original.Version.ToString() != manifest.CurrentAssemblyVersion)
                throw new InvalidDataException("The target is not the original SPI application.");
            VerifyHash(manifest.TargetExecutable, manifest.HelperSha256);
        }

        private static UpdateVersion ParseTag(string tag)
        {
            UpdateVersion version;
            if (!UpdateVersion.TryParse(tag, out version) || tag != "v" + version || version.Major > 65534 || version.Minor > 65534 || version.Patch > 65534)
                throw new InvalidDataException("The release tag is not supported by this application.");
            return version;
        }

        private static Process OpenParent(UpdateApplyManifest manifest)
        {
            Process parent;
            try { parent = Process.GetProcessById(manifest.ParentProcessId); }
            catch (ArgumentException) { return null; }
            try
            {
                IntPtr handle = parent.Handle;
                if (handle == IntPtr.Zero) throw new InvalidOperationException("The original process handle is unavailable.");
                if (parent.HasExited) { parent.Dispose(); return null; }
                var module = parent.MainModule;
                if (module == null) throw new InvalidOperationException("The original process module is unavailable.");
                if (parent.StartTime.ToUniversalTime().Ticks != manifest.ParentStartUtcTicks ||
                    !string.Equals(Path.GetFullPath(module.FileName), Path.GetFullPath(manifest.TargetExecutable), StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("The original process identity changed. No update was applied.");
                return parent;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is Win32Exception)
            {
                bool exited;
                try { exited = parent.HasExited; }
                catch (Exception checkError) when (checkError is InvalidOperationException || checkError is Win32Exception) { exited = false; }
                parent.Dispose();
                if (exited) return null;
                throw;
            }
            catch { parent.Dispose(); throw; }
        }

        private static void WaitForParent(string stage, Process parent)
        {
            var watch = Stopwatch.StartNew();
            while (parent != null && !parent.WaitForExit(500))
            {
                if (File.Exists(Path.Combine(stage, "cancel"))) throw new OperationCanceledException("Application shutdown was canceled.");
                if (watch.Elapsed > TimeSpan.FromMinutes(5)) throw new TimeoutException("The application stayed open; its running work was not interrupted.");
            }
            if (File.Exists(Path.Combine(stage, "cancel"))) throw new OperationCanceledException("Application shutdown was canceled.");
        }

        internal static void EnsureNoOtherInstance(string executable, int excludedPid)
        {
            string directory = Path.GetDirectoryName(Path.GetFullPath(executable));
            var processes = Process.GetProcesses();
            if (processes.Length > 4096)
            {
                foreach (var process in processes) process.Dispose();
                throw new InvalidOperationException("There are too many processes to safely verify an exclusive update.");
            }
            foreach (var process in processes)
                using (process)
                {
                    if (process.Id == excludedPid) continue;
                    string name = null;
                    bool sameFolder = false;
                    bool sharedInstallation = false;
                    try
                    {
                        name = process.ProcessName;
                        if (process.HasExited) continue;
                        var module = process.MainModule;
                        if (module == null)
                        {
                            if (IsLikelySpiProcess(name) || string.Equals(name, Path.GetFileNameWithoutExtension(executable), StringComparison.OrdinalIgnoreCase))
                                throw new InvalidOperationException("An SPI process has no inspectable executable module.");
                            continue;
                        }
                        string path = module.FileName;
                        sameFolder = UpdateInstaller.SameDirectory(Path.GetDirectoryName(path), directory);
                        if (sameFolder)
                        {
                            string assemblyName;
                            try { assemblyName = AssemblyName.GetAssemblyName(path).Name; }
                            catch (BadImageFormatException) { continue; }
                            sharedInstallation = IsSharedInstallationProcess(directory, path, assemblyName);
                        }
                    }
                    catch (Exception ex) when (ex is Win32Exception || ex is InvalidOperationException || ex is IOException || ex is UnauthorizedAccessException)
                    {
                        // Inaccessible system processes are unrelated; a likely SPI process is not
                        // assumed safe merely because its executable path cannot be inspected.
                        if (sameFolder || IsLikelySpiProcess(name) || string.Equals(name, Path.GetFileNameWithoutExtension(executable), StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("Another SPI process could not be inspected safely. Close it before updating.");
                    }
                    if (sharedInstallation)
                        throw new InvalidOperationException("Another SPI instance shares this application folder. Save its work and close it before updating.");
                }
        }

        internal static bool IsSharedInstallationProcess(string directory, string executable, string assemblyName)
        {
            return UpdateInstaller.SameDirectory(directory, Path.GetDirectoryName(Path.GetFullPath(executable))) &&
                string.Equals(assemblyName, Assembly.GetExecutingAssembly().GetName().Name, StringComparison.Ordinal);
        }

        private static bool IsLikelySpiProcess(string name)
        {
            if (name == null) return false;
            string normalized = new string(name.Where(char.IsLetterOrDigit).ToArray());
            return normalized.IndexOf("SPIDataComp", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        internal static void ValidateTargetDirectory(string directory)
        {
            string full = Path.GetFullPath(directory);
            if (!Path.IsPathRooted(directory) || directory.Length < 3 || directory[1] != ':' ||
                (directory[2] != '\\' && directory[2] != '/') || full.StartsWith(@"\\", StringComparison.Ordinal) ||
                UpdateInstaller.SameDirectory(full, Path.GetPathRoot(full)) || !Directory.Exists(full))
                throw new InvalidDataException("Automatic updates require an existing local application folder, not a root or network folder.");
            RejectReparsePoints(full);
        }

        internal static void RequireContained(string root, string candidate)
        {
            string prefix = Path.GetFullPath(root).TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
            if (!Path.GetFullPath(candidate).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("An update path escapes its permitted folder.");
        }

        internal static void RejectReparsePoints(string path)
        {
            string current = Path.GetFullPath(path);
            while (!string.IsNullOrEmpty(current))
            {
                try
                {
                    if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                        throw new InvalidDataException("Updates do not follow symbolic links or junctions.");
                }
                catch (FileNotFoundException) { }
                catch (DirectoryNotFoundException) { }
                current = Path.GetDirectoryName(current);
            }
        }

        internal static string HashFile(string path)
        {
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(file)).Replace("-", string.Empty).ToLowerInvariant();
        }

        internal static void VerifyHash(string path, string expected)
        {
            if (expected == null || !Regex.IsMatch(expected, @"\A[0-9a-fA-F]{64}\z") ||
                !string.Equals(HashFile(path), expected, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("SHA-256 verification failed; no update was applied.");
        }

        internal static IList<UpdatePortableFile> ValidatePortableArchive(string package, UpdateVersion version, string executableName, int? installedFrameworkRelease = null)
        {
            if (Path.GetFileName(executableName) != executableName || !executableName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Invalid portable executable filename.");
            string archiveExe = "SPI_Data_Comp_Tool_" + version + ".exe";
            // From 4.6.0 portable/ holds only the exe; older releases also carry its .config.
            var allowed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "portable/" + archiveExe, executableName },
                { "portable/" + archiveExe + ".config", executableName + ".config" },
            };
            // The other packages of the release zip are validated by name and never extracted here.
            var bundled = new HashSet<string>(StringComparer.Ordinal)
            {
                "windows-x64/" + archiveExe, "windows-msi/SPI_Data_Comp_Tool_" + version + "_x64.msi",
                "microsoft-store/SPI_Data_Comp_Tool_" + version + "_x64.msix", "THIRD_PARTY_NOTICES.md",
            };
            foreach (string name in LegacyTemplateNames) allowed.Add("portable/PNID_Reports/" + name, "PNID_Reports/" + name);
            var allowedDirectories = new HashSet<string>(new[] { "portable", "portable/PNID_Reports", "portable/PNID_Reports/Source", "portable/PNID_Reports/Target_IPEP", "windows-x64", "windows-msi", "microsoft-store" }, StringComparer.OrdinalIgnoreCase);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var files = new List<UpdatePortableFile>();
            long total = 0;
            using (var file = new FileStream(package, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var attributes = ReadZipAttributes(file);
                file.Position = 0;
                using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
                {
                    if (zip.Entries.Count > 128) throw new InvalidDataException("The update archive contains too many entries.");
                    if (zip.Entries.Count != attributes.Count) throw new InvalidDataException("Inconsistent ZIP entry metadata.");
                    foreach (var entry in zip.Entries)
                    {
                        string name = entry.FullName;
                        ValidateEntryName(name);
                        string unique = name.TrimEnd('/');
                        if (!seen.Add(unique)) throw new InvalidDataException("Duplicate/case-colliding ZIP entries are not permitted.");
                        uint external;
                        if (!attributes.TryGetValue(name, out external)) throw new InvalidDataException("Inconsistent ZIP filename metadata.");
                        uint mode = (external >> 16) & 0xF000;
                        if (mode == 0xA000 || (external & (uint)FileAttributes.ReparsePoint) != 0)
                            throw new InvalidDataException("Symbolic-link ZIP entries are not permitted.");
                        total = checked(total + entry.Length);
                        if (entry.Length < 0 || entry.Length > MaximumEntryBytes || total > MaximumExpandedBytes)
                            throw new InvalidDataException("The expanded archive exceeds the safe update limits.");
                        if (entry.Length > 1048576 && (entry.CompressedLength <= 0 || entry.Length / entry.CompressedLength > 2000))
                            throw new InvalidDataException("The archive compression ratio is unsafe.");
                        if (name.EndsWith("/", StringComparison.Ordinal))
                        {
                            if (entry.Length != 0 || !allowedDirectories.Contains(unique)) throw new InvalidDataException("Unexpected ZIP directory.");
                            continue;
                        }
                        // Current releases intentionally bundle the installer beside portable/. Validate
                        // its exact name and metadata, but never extract or execute it in portable mode.
                        if (bundled.Contains(name)) continue;
                        string destination;
                        if (!allowed.TryGetValue(name, out destination)) throw new InvalidDataException("The archive contains an unsupported application file.");
                        if (string.Equals(destination, executableName + ".config", StringComparison.OrdinalIgnoreCase))
                        {
                            if (entry.Length > 131072) throw new InvalidDataException("The update runtime configuration exceeds the safe size limit.");
                            using (var reader = new StreamReader(entry.Open(), Encoding.UTF8, true))
                            {
                                var characters = new char[131073];
                                int count = reader.ReadBlock(characters, 0, characters.Length);
                                if (count > 131072) throw new InvalidDataException("The update runtime configuration exceeds the safe size limit.");
                                RuntimeRequirements.ValidateConfiguration(new string(characters, 0, count),
                                    installedFrameworkRelease ?? RuntimeRequirements.InstalledFrameworkRelease());
                            }
                        }
                        files.Add(new UpdatePortableFile { ArchiveName = name, RelativePath = destination });
                    }
                }
            }
            var requiredFiles = new List<string> { executableName };
            // Core-only packages are complete now. If legacy files are supplied, keep the old
            // two-anchor requirement: a partially formed legacy package must still fail closed.
            // New portable releases include only empty anchors for already-released updaters.
            if (files.Any(x => x.RelativePath.StartsWith("PNID_Reports/", StringComparison.OrdinalIgnoreCase)))
                requiredFiles.AddRange(new[] { "PNID_Reports/Source/instrument.txt", "PNID_Reports/Target_IPEP/instrument.txt" });
            foreach (string required in requiredFiles)
                if (!files.Any(x => string.Equals(x.RelativePath, required, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidDataException("The portable archive is missing the executable or a required legacy compatibility anchor.");
            return files;
        }

        // Read the bounded classic central directory to verify duplicate names and link metadata
        // consistently with the declared ZIP entry count before extracting any payload.
        // Our release size/count limits cannot require ZIP64; unsupported archive variants fail closed.
        private static Dictionary<string, uint> ReadZipAttributes(Stream input)
        {
            if (input.Length < 22 || input.Length > MaximumPackageBytes) throw new InvalidDataException("Invalid ZIP size.");
            int tailLength = (int)Math.Min(input.Length, 65557);
            input.Position = input.Length - tailLength;
            byte[] tail = new byte[tailLength];
            int read = 0;
            while (read < tail.Length)
            {
                int count = input.Read(tail, read, tail.Length - read);
                if (count == 0) throw new InvalidDataException("Truncated ZIP directory.");
                read += count;
            }
            int end = -1;
            for (int index = tail.Length - 22; index >= 0; index--)
                if (BitConverter.ToUInt32(tail, index) == 0x06054B50 && index + 22 + BitConverter.ToUInt16(tail, index + 20) == tail.Length)
                { end = index; break; }
            if (end < 0) throw new InvalidDataException("ZIP end record is missing.");
            int entries = BitConverter.ToUInt16(tail, end + 10);
            uint size = BitConverter.ToUInt32(tail, end + 12);
            uint offset = BitConverter.ToUInt32(tail, end + 16);
            long directoryEnd = (long)offset + size;
            if (entries > 128 || BitConverter.ToUInt16(tail, end + 4) != 0 || BitConverter.ToUInt16(tail, end + 6) != 0 ||
                BitConverter.ToUInt16(tail, end + 8) != entries || directoryEnd != input.Length - tailLength + end)
                throw new InvalidDataException("ZIP64, multi-disk, oversized, or inconsistent archives are not supported.");
            var result = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
            input.Position = offset;
            using (var reader = new BinaryReader(input, Encoding.UTF8, true))
                for (int index = 0; index < entries; index++)
                {
                    if (input.Position + 46 > directoryEnd || reader.ReadUInt32() != 0x02014B50)
                        throw new InvalidDataException("Invalid ZIP central-directory entry.");
                    reader.ReadUInt16(); // version made by
                    ushort needed = reader.ReadUInt16();
                    ushort flags = reader.ReadUInt16();
                    ushort method = reader.ReadUInt16();
                    reader.ReadUInt32(); reader.ReadUInt32(); // date/time and CRC
                    uint compressed = reader.ReadUInt32();
                    uint expanded = reader.ReadUInt32();
                    ushort nameLength = reader.ReadUInt16();
                    ushort extraLength = reader.ReadUInt16();
                    ushort commentLength = reader.ReadUInt16();
                    ushort disk = reader.ReadUInt16();
                    reader.ReadUInt16(); // internal attributes
                    uint external = reader.ReadUInt32();
                    uint localOffset = reader.ReadUInt32();
                    if (needed > 20 || (flags & 1) != 0 || (method != 0 && method != 8) || disk != 0 ||
                        compressed == uint.MaxValue || expanded == uint.MaxValue || localOffset >= offset || nameLength == 0 || nameLength > 240 ||
                        input.Position + nameLength + extraLength + commentLength > directoryEnd)
                        throw new InvalidDataException("Unsupported/encrypted ZIP entry or invalid metadata.");
                    byte[] nameBytes = reader.ReadBytes(nameLength);
                    if (nameBytes.Any(x => x > 127)) throw new InvalidDataException("Unsupported non-ASCII ZIP filename.");
                    string name = Encoding.ASCII.GetString(nameBytes);
                    if (result.ContainsKey(name)) throw new InvalidDataException("Duplicate/case-colliding ZIP entries are not permitted.");
                    result.Add(name, external);
                    input.Position += extraLength + commentLength;
                }
            if (input.Position != directoryEnd) throw new InvalidDataException("ZIP directory length is inconsistent.");
            return result;
        }

        private static void ValidateEntryName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 240 || name.Contains("\\") || name.StartsWith("/", StringComparison.Ordinal))
                throw new InvalidDataException("Unsafe ZIP entry path.");
            foreach (string part in name.TrimEnd('/').Split('/'))
            {
                if (part.Length == 0 || part == "." || part == ".." || part.EndsWith(".", StringComparison.Ordinal) || part.EndsWith(" ", StringComparison.Ordinal) ||
                    part.Any(c => c < 32 || "<>:\"|?*".IndexOf(c) >= 0)) throw new InvalidDataException("Unsafe ZIP entry path or alternate data stream.");
                string stem = part.Split('.')[0];
                if (Regex.IsMatch(stem, @"\A(?:CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9]|CONIN\$|CONOUT\$)\z", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                    throw new InvalidDataException("Reserved Windows filenames are not permitted.");
            }
        }

        internal static void ApplyPortable(string package, string targetDirectory, string executableName, UpdateVersion version,
            string stage, Action migrateSettings, Action<string, string> replaceFile = null, Action<string, UpdateVersion> validateExecutable = null,
            int? installedFrameworkRelease = null)
        {
            ValidateTargetDirectory(targetDirectory);
            var files = ValidatePortableArchive(package, version, executableName, installedFrameworkRelease);
            string payload = Path.Combine(stage, "payload-" + Guid.NewGuid().ToString("N"));
            string backup = Path.Combine(stage, "backup");
            RejectReparsePoints(payload); RejectReparsePoints(backup);
            Directory.CreateDirectory(payload);
            Directory.CreateDirectory(backup);
            long extracted = 0;
            using (var input = new FileStream(package, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var zip = new ZipArchive(input, ZipArchiveMode.Read))
                foreach (var file in files)
                {
                    string path = Path.Combine(payload, file.RelativePath.Replace('/', Path.DirectorySeparatorChar));
                    RequireContained(payload, path);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    var entry = zip.GetEntry(file.ArchiveName);
                    using (var source = entry.Open())
                    using (var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[65536];
                        long written = 0;
                        int count;
                        while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            written += count; extracted += count;
                            if (written > entry.Length || written > MaximumEntryBytes || extracted > MaximumExpandedBytes)
                                throw new InvalidDataException("An extracted file exceeds the declared safe ZIP limits.");
                            output.Write(buffer, 0, count);
                        }
                        if (written != entry.Length) throw new InvalidDataException("An extracted file length does not match the archive.");
                    }
                }
            (validateExecutable ?? VerifyApplicationVersion)(Path.Combine(payload, executableName), version);
            string configName = executableName + ".config";
            bool packageHasConfig = files.Any(x => string.Equals(x.RelativePath, configName, StringComparison.OrdinalIgnoreCase));
            if (packageHasConfig) UpdatePreferences.ParseXml(UpdatePreferences.ReadBoundedXmlText(Path.Combine(payload, configName)));
            // A single-exe release has no .config: an older one beside the exe would apply stale
            // assembly redirects, so it is backed up and removed with the update (and restored on failure).
            string staleConfig = packageHasConfig ? null : Path.Combine(targetDirectory, configName);
            if (staleConfig != null) { RejectReparsePoints(staleConfig); if (!File.Exists(staleConfig)) staleConfig = null; }
            var originals = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var stagedHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in files)
            {
                string relative = file.RelativePath.Replace('/', Path.DirectorySeparatorChar);
                string target = Path.Combine(targetDirectory, relative);
                RequireContained(targetDirectory, target); RejectReparsePoints(target);
                string previous = Path.Combine(backup, relative);
                if (File.Exists(previous)) throw new IOException("A previous backup already exists; it will not be overwritten.");
                if (File.Exists(target))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(previous));
                    File.Copy(target, previous, false);
                    originals[relative] = HashFile(previous);
                }
                else originals[relative] = null;
                stagedHashes[relative] = HashFile(Path.Combine(payload, relative));
            }
            string staleBackup = null, staleHash = null;
            if (staleConfig != null)
            {
                staleBackup = Path.Combine(backup, configName);
                if (File.Exists(staleBackup)) throw new IOException("A previous backup already exists; it will not be overwritten.");
                File.Copy(staleConfig, staleBackup, false);
                staleHash = HashFile(staleBackup);
                originals[configName] = staleHash;
            }
            UpdatePreferences.AtomicWrite(Path.Combine(stage, "portable-backup-index.json"), JsonConvert.SerializeObject(originals, Formatting.Indented));
            bool staleRemoved = false;
            var applied = new List<string>();
            try
            {
                foreach (var file in files)
                {
                    string relative = file.RelativePath.Replace('/', Path.DirectorySeparatorChar);
                    string target = Path.Combine(targetDirectory, relative);
                    RejectReparsePoints(target);
                    string actual = File.Exists(target) ? HashFile(target) : null;
                    if (actual != originals[relative]) throw new IOException("An application file changed while the update was prepared.");
                    (replaceFile ?? AtomicCopy)(Path.Combine(payload, relative), target);
                    applied.Add(relative);
                }
                if (staleConfig != null)
                {
                    RejectReparsePoints(staleConfig);
                    if (!File.Exists(staleConfig) || HashFile(staleConfig) != staleHash) throw new IOException("An application file changed while the update was prepared.");
                    File.Delete(staleConfig);
                    staleRemoved = true;
                }
                migrateSettings?.Invoke();
            }
            catch (Exception originalError)
            {
                var failures = new List<Exception>();
                if (staleRemoved)
                {
                    try { RejectReparsePoints(staleConfig); AtomicCopy(staleBackup, staleConfig); }
                    catch (Exception rollbackError) { failures.Add(rollbackError); }
                }
                foreach (string relative in applied.AsEnumerable().Reverse())
                {
                    try
                    {
                        string target = Path.Combine(targetDirectory, relative);
                        RejectReparsePoints(target);
                        if (!File.Exists(target) || HashFile(target) != stagedHashes[relative]) throw new IOException("A replaced file changed again; recover it from the retained backup.");
                        if (originals[relative] == null) File.Delete(target);
                        else AtomicCopy(Path.Combine(backup, relative), target);
                    }
                    catch (Exception rollbackError) { failures.Add(rollbackError); }
                }
                if (failures.Count > 0) throw new AggregateException("The update failed and some files need recovery from the retained backup.", new[] { originalError }.Concat(failures));
                throw new IOException("The portable update failed. Replaced application files were restored; your other files were not changed.", originalError);
            }
        }

        internal static void AtomicCopy(string source, string destination)
        {
            RejectReparsePoints(destination);
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            string temporary = destination + ".spi-update-" + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.Copy(source, temporary, false);
                if (File.Exists(destination)) File.Replace(temporary, destination, null);
                else File.Move(temporary, destination);
            }
            finally
            {
                try { if (File.Exists(temporary)) File.Delete(temporary); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        private static void ApplyInstalled(string stage, UpdateApplyManifest manifest, UpdateVersion version, UpdateSettingsMigration settings)
        {
            string setup = Path.Combine(stage, manifest.PackageFileName);
            VerifyHash(setup, manifest.PackageSha256);
            settings.Apply();
            try
            {
                // The helper stays unelevated. Only Inno Setup requests normal Windows elevation;
                // /SILENT shows progress and skips the installer's postinstall launch checkbox.
                using (var lockedPackage = new FileStream(setup, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var process = Process.Start(new ProcessStartInfo(setup,
                    "/SILENT /SP- /NORESTART /NOCLOSEAPPLICATIONS /NOFORCECLOSEAPPLICATIONS /NORESTARTAPPLICATIONS /RESTARTEXITCODE=3010 " +
                    (manifest.IsMachineInstall ? "/ALLUSERS" : "/CURRENTUSER") + " /DIR=\"" + manifest.TargetDirectory + "\" /LOG=\"" + Path.Combine(stage, "setup.log") + "\"")
                { UseShellExecute = true, Verb = manifest.IsMachineInstall ? "runas" : string.Empty, WorkingDirectory = stage }))
                {
                    if (process == null) throw new IOException("Windows did not start the installer.");
                    process.WaitForExit();
                    if (process.ExitCode != 0) throw new IOException("Setup was canceled, failed, or requires a restart (exit code " + process.ExitCode + "). No automatic restart was attempted.");
                }
                string error;
                var current = UpdateInstaller.DetectInstallation(manifest.TargetExecutable, out error);
                if (error != null || current.Kind != UpdateInstallKind.Installed || current.IsMachineInstall != manifest.IsMachineInstall)
                    throw new IOException("Setup did not preserve the installation location and scope.");
                VerifyApplicationVersion(manifest.TargetExecutable, version);
            }
            catch
            {
                settings.Rollback();
                throw;
            }
        }

        /// <summary>An MSI is a compound document; anything else is not handed to Windows Installer.</summary>
        internal static void RequireMsiPackage(string package)
        {
            var header = new byte[8];
            using (var file = new FileStream(package, FileMode.Open, FileAccess.Read, FileShare.Read))
                if (file.Read(header, 0, 8) != 8 || !header.SequenceEqual(new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }))
                    throw new InvalidDataException("The staged MSI is not a Windows Installer package.");
        }

        private static void ApplyMsi(string stage, UpdateApplyManifest manifest, UpdateVersion version, UpdateSettingsMigration settings)
        {
            string msi = Path.Combine(stage, manifest.PackageFileName);
            VerifyHash(msi, manifest.PackageSha256);
            RequireMsiPackage(msi);
            settings.Apply();
            try
            {
                // Per-user MSI with a fixed folder: no elevation. The major upgrade replaces the installed version in place.
                string msiexec = Path.Combine(Environment.SystemDirectory, "msiexec.exe");
                using (var lockedPackage = new FileStream(msi, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var process = Process.Start(new ProcessStartInfo(msiexec,
                    "/i \"" + msi + "\" /passive /norestart /l*v \"" + Path.Combine(stage, "msi.log") + "\"")
                { UseShellExecute = false, WorkingDirectory = stage }))
                {
                    if (process == null) throw new IOException("Windows did not start the MSI installer.");
                    process.WaitForExit();
                    if (process.ExitCode != 0) throw new IOException("The MSI was canceled, failed, or requires a restart (exit code " + process.ExitCode + "). No automatic restart was attempted.");
                }
                string error;
                var current = UpdateInstaller.DetectInstallation(manifest.TargetExecutable, out error);
                if (error != null || current.Kind != UpdateInstallKind.Msi)
                    throw new IOException("The MSI did not keep the installation location.");
                VerifyApplicationVersion(manifest.TargetExecutable, version);
            }
            catch
            {
                settings.Rollback();
                throw;
            }
        }

        internal static void VerifyApplicationVersion(string executable, UpdateVersion version)
        {
            var actual = AssemblyName.GetAssemblyName(executable);
            var expected = new Version(version.Major, version.Minor, version.Patch, 0);
            string product = FileVersionInfo.GetVersionInfo(executable).ProductVersion;
            if (actual.Name != Assembly.GetExecutingAssembly().GetName().Name || actual.Version != expected ||
                (product != version.ToString() && (version.IsBeta || product != expected.ToString())))
                throw new InvalidDataException("The application executable does not match the selected release.");
        }

        private static void WriteLog(string stage, string message)
        {
            RejectReparsePoints(stage);
            File.AppendAllText(Path.Combine(stage, "update.log"), DateTime.UtcNow.ToString("O") + " " + Common_Data.RedactConnectionCredentials(message) + Environment.NewLine);
        }
    }
}
