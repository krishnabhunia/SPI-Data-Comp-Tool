using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace SPI_Data_Comp_Tool
{
    // Only preferences are retained here. Connection profiles and database passwords are never copied.
    internal sealed class UpdatePreferenceSnapshot
    {
        [JsonProperty(Required = Required.Always)] public int Schema { get; set; } = 1;
        [JsonProperty(Required = Required.AllowNull)] public string ProfilePathUser { get; set; }
        [JsonProperty(Required = Required.AllowNull)] public string RuleFilePath { get; set; }
        [JsonProperty(Required = Required.Always)] public bool EPPlus_Excel_Save { get; set; } = true;
        [JsonProperty(Required = Required.Always)] public bool XL_Excel_Save { get; set; }
        [JsonProperty(Required = Required.Always)] public UpdateChannel Channel { get; set; }
    }

    internal static class UpdatePreferences
    {
        internal const string SettingsSection = "SPI_Data_Comp_Tool.Properties.Settings";
        internal static readonly string[] MigratedNames = { "ProfilePathUser", "RuleFilePath", "EPPlus_Excel_Save", "XL_Excel_Save" };
        private static bool initialized;
        private static UpdateChannel channel = UpdateChannel.Normal;
        private static string storageDirectory;
        private static string currentUserConfig;
        private static bool channelChanged;
        private static UpdatePreferenceSnapshot lastSaved;
        internal static bool IsInitialized { get { return initialized; } }

        public static UpdateChannel Channel
        {
            get { return channel; }
            set
            {
                if (!Enum.IsDefined(typeof(UpdateChannel), value)) throw new ArgumentOutOfRangeException(nameof(value));
                var previous = channel;
                bool previouslyChanged = channelChanged;
                channel = value;
                channelChanged = true;
                try { SaveCurrent(); }
                catch { channel = previous; channelChanged = previouslyChanged; throw; }
            }
        }

        // Called on normal startup only, never by the helper or CI's UI smoke checks.
        public static void Initialize()
        {
            if (initialized) return;
            storageDirectory = GetStorageDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            string stableFile = Path.Combine(storageDirectory, "preferences.json");
            string currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            currentUserConfig = currentConfig;
            // Existing current-version settings are authoritative. Never replace them by comparing
            // mtimes: another running instance may have saved a newer value during startup.
            if (!File.Exists(currentConfig) && File.Exists(stableFile))
            {
                Apply(ReadSnapshot(stableFile));
            }
            else if (!File.Exists(currentConfig))
            {
                // ApplicationSettingsBase.Upgrade would migrate every setting. Keep an explicit whitelist.
                foreach (string name in MigratedNames)
                {
                    object previous = Properties.Settings.Default.GetPreviousVersion(name);
                    if (previous != null) Properties.Settings.Default[name] = previous;
                }
            }
            if (File.Exists(stableFile)) channel = ReadSnapshot(stableFile).Channel;
            // A failed initialization must not later overwrite the last-good snapshot on exit.
            lastSaved = SavePersistedSnapshot(stableFile, currentUserConfig, CaptureInMemory(), null);
            // Reload the whitelist observed under the snapshot read handle. If a second instance
            // created user.config during migration, its actual persisted values win. Initialization
            // does not write user.config; normal edits already Save(), and apply bridges it prelaunch.
            if (File.Exists(currentUserConfig))
            {
                // Assigning even an equal Settings value marks it dirty. Reload keeps unrelated
                // preferences clean, so a later one-field save cannot publish stale startup fields.
                Properties.Settings.Default.Reload();
                channel = lastSaved.Channel;
            }
            else Apply(lastSaved);
            initialized = true;
        }

        public static void SaveCurrent()
        {
            if (!initialized) return;
            lastSaved = SavePersistedSnapshot(Path.Combine(storageDirectory, "preferences.json"), currentUserConfig,
                CaptureInMemory(), channelChanged ? (UpdateChannel?)channel : null);
            channel = lastSaved.Channel;
            channelChanged = false;
        }

        internal static UpdatePreferenceSnapshot Capture()
        {
            return lastSaved ?? CaptureInMemory();
        }

        private static UpdatePreferenceSnapshot CaptureInMemory()
        {
            return new UpdatePreferenceSnapshot
            {
                ProfilePathUser = Properties.Settings.Default.ProfilePathUser,
                RuleFilePath = Properties.Settings.Default.RuleFilePath,
                EPPlus_Excel_Save = Properties.Settings.Default.EPPlus_Excel_Save,
                XL_Excel_Save = Properties.Settings.Default.XL_Excel_Save,
                Channel = channel,
            };
        }

        // Settings forms already persist their four fields immediately. An idle second instance
        // must read that current disk state instead of publishing its stale Settings.Default cache.
        internal static UpdatePreferenceSnapshot SavePersistedSnapshot(string stableFile, string userConfig,
            UpdatePreferenceSnapshot fallback, UpdateChannel? explicitChannel)
        {
            UpdateApplyWorker.RejectReparsePoints(stableFile);
            Directory.CreateDirectory(Path.GetDirectoryName(stableFile));
            string lockPath = stableFile + ".lock";
            UpdateApplyWorker.RejectReparsePoints(lockPath);
            // Contention is reported safely, never handled by overwriting another instance's file.
            using (var gate = new FileStream(lockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                UpdatePreferenceSnapshot latest = File.Exists(stableFile) ? ReadSnapshot(stableFile) : null;
                UpdatePreferenceSnapshot result;
                UpdateApplyWorker.RejectReparsePoints(userConfig);
                if (File.Exists(userConfig))
                {
                    // Deny concurrent replacement while taking and publishing this short snapshot.
                    // This avoids a newer user.config being followed by an older preferences.json.
                    using (var input = new FileStream(userConfig, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (input.Length > 1048576) throw new InvalidDataException("The user configuration is unexpectedly large.");
                        using (var reader = new StreamReader(input, Encoding.UTF8, true, 4096, true))
                            result = ReadPersistedPreferences(reader.ReadToEnd());
                        result.Channel = explicitChannel ?? (latest == null ? fallback.Channel : latest.Channel);
                        WriteSnapshot(stableFile, result);
                    }
                }
                else
                {
                    result = latest ?? fallback;
                    result.Channel = explicitChannel ?? result.Channel;
                    WriteSnapshot(stableFile, result);
                }
                return result;
            }
        }

        private static UpdatePreferenceSnapshot ReadPersistedPreferences(string text)
        {
            var xml = ParseXml(text);
            if (xml.Root == null || xml.Root.Name != "configuration") throw new InvalidDataException("Invalid user configuration document.");
            var sections = xml.Root.Elements("userSettings").SelectMany(x => x.Elements(SettingsSection)).ToList();
            if (sections.Count > 1) throw new InvalidDataException("Duplicate preference sections are not supported.");
            var snapshot = new UpdatePreferenceSnapshot();
            var section = sections.SingleOrDefault();
            if (section == null) return snapshot;
            foreach (string name in MigratedNames)
            {
                var entries = section.Elements("setting").Where(x => (string)x.Attribute("name") == name).ToList();
                if (entries.Count > 1) throw new InvalidDataException("Duplicate preference values are not supported.");
                if (entries.Count == 0) continue;
                var value = entries[0].Element("value");
                if (value == null || value.Elements().Any()) throw new InvalidDataException("A preference value is malformed.");
                if (name == "ProfilePathUser") snapshot.ProfilePathUser = value.Value;
                else if (name == "RuleFilePath") snapshot.RuleFilePath = value.Value;
                else
                {
                    bool enabled;
                    if (!bool.TryParse(value.Value, out enabled)) throw new InvalidDataException("An Excel preference is not a Boolean value.");
                    if (name == "EPPlus_Excel_Save") snapshot.EPPlus_Excel_Save = enabled;
                    else snapshot.XL_Excel_Save = enabled;
                }
            }
            ValidateSnapshot(snapshot);
            return snapshot;
        }

        private static void Apply(UpdatePreferenceSnapshot snapshot)
        {
            ValidateSnapshot(snapshot);
            Properties.Settings.Default.ProfilePathUser = snapshot.ProfilePathUser ?? string.Empty;
            Properties.Settings.Default.RuleFilePath = snapshot.RuleFilePath ?? string.Empty;
            Properties.Settings.Default.EPPlus_Excel_Save = snapshot.EPPlus_Excel_Save;
            Properties.Settings.Default.XL_Excel_Save = snapshot.XL_Excel_Save;
            channel = snapshot.Channel;
        }

        internal static void ValidateSnapshot(UpdatePreferenceSnapshot snapshot)
        {
            if (snapshot == null || snapshot.Schema != 1 || !Enum.IsDefined(typeof(UpdateChannel), snapshot.Channel))
                throw new InvalidDataException("The saved update preferences have an unsupported format.");
            foreach (string value in new[] { snapshot.ProfilePathUser, snapshot.RuleFilePath })
                if (value != null && (value.Length > 32760 || value.IndexOf('\0') >= 0))
                    throw new InvalidDataException("A saved preference path is invalid.");
        }

        internal static string GetStorageDirectory(string installationDirectory)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SPI Data Comp Tool", "Updates", InstallationIdentity(installationDirectory));
        }

        internal static string InstallationIdentity(string installationDirectory)
        {
            string canonical = Path.GetFullPath(installationDirectory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(canonical))).Replace("-", string.Empty).ToLowerInvariant();
        }

        internal static UpdatePreferenceSnapshot ReadSnapshot(string path)
        {
            UpdateApplyWorker.RejectReparsePoints(path);
            var value = JsonConvert.DeserializeObject<UpdatePreferenceSnapshot>(ReadBoundedText(path, 131072),
                new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error, MaxDepth = 8 });
            ValidateSnapshot(value);
            return value;
        }

        internal static void WriteSnapshot(string path, UpdatePreferenceSnapshot snapshot)
        {
            ValidateSnapshot(snapshot);
            string json = JsonConvert.SerializeObject(snapshot, Newtonsoft.Json.Formatting.Indented);
            if (Encoding.UTF8.GetByteCount(json) > 131072)
                throw new InvalidDataException("The preference snapshot exceeds its safe size limit; the previous file was retained.");
            AtomicWrite(path, json);
        }

        internal static void AtomicWrite(string path, string content)
        {
            UpdateApplyWorker.RejectReparsePoints(path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var file = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(file, new UTF8Encoding(false))) writer.Write(content);
                if (File.Exists(path)) File.Replace(temporary, path, null);
                else File.Move(temporary, path);
            }
            finally
            {
                try { if (File.Exists(temporary)) File.Delete(temporary); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        // Version-directory bridging also supports releases that predate the updater. No assumptions
        // about an Inno AppId or a new version's startup code are needed: the executable path is stable.
        internal static UpdateSettingsMigration PrepareMigration(string sourceConfig, string currentVersion,
            UpdateVersion target, UpdatePreferenceSnapshot snapshot, string backupDirectory, string allowedRoot = null)
        {
            ValidateSnapshot(snapshot);
            string root = Path.GetFullPath(allowedRoot ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            string source = Path.GetFullPath(sourceConfig);
            UpdateApplyWorker.RequireContained(root, source);
            UpdateApplyWorker.RejectReparsePoints(source);
            if (!string.Equals(Path.GetFileName(source), "user.config", StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(Path.GetFileName(Path.GetDirectoryName(source)), currentVersion, StringComparison.Ordinal))
                throw new InvalidDataException("The .NET preference location could not be validated; update was not applied.");
            string identity = Path.GetDirectoryName(Path.GetDirectoryName(source));
            string product = new string(Path.GetFileName(identity).Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
            if (!product.Contains("spidatacomp"))
                throw new InvalidDataException("The .NET preference location is not an SPI application identity.");
            Version parsedCurrent;
            if (!Version.TryParse(currentVersion, out parsedCurrent) || parsedCurrent.ToString() != currentVersion || target == null)
                throw new InvalidDataException("The preference version is invalid.");
            string targetVersion = new Version(target.Major, target.Minor, target.Patch, 0).ToString();
            string destination = Path.Combine(identity, targetVersion, "user.config");
            UpdateApplyWorker.RequireContained(root, destination);
            UpdateApplyWorker.RejectReparsePoints(destination);
            string original = File.Exists(destination) ? ReadBoundedXmlText(destination) : null;
            var xml = original == null ? new XDocument(new XElement("configuration")) : ParseXml(original);
            if (xml.Root == null || xml.Root.Name != "configuration") throw new InvalidDataException("Invalid user configuration document.");
            var userGroups = xml.Root.Elements("userSettings").ToList();
            if (userGroups.Count > 1) throw new InvalidDataException("Duplicate user-settings groups are not supported.");
            var group = userGroups.SingleOrDefault();
            if (group == null) { group = new XElement("userSettings"); xml.Root.Add(group); }
            var sections = group.Elements(SettingsSection).ToList();
            if (sections.Count > 1) throw new InvalidDataException("Duplicate application-settings sections are not supported.");
            var section = sections.SingleOrDefault();
            if (section == null) { section = new XElement(SettingsSection); group.Add(section); }
            // Back up only the four known fields. Unknown fields remain in the destination document
            // but are never copied into a backup which could inadvertently contain a secret.
            var previous = new Dictionary<string, string>();
            string[] values = { snapshot.ProfilePathUser ?? string.Empty, snapshot.RuleFilePath ?? string.Empty,
                snapshot.EPPlus_Excel_Save.ToString(), snapshot.XL_Excel_Save.ToString() };
            for (int i = 0; i < MigratedNames.Length; i++)
            {
                string name = MigratedNames[i];
                var matches = section.Elements("setting").Where(x => (string)x.Attribute("name") == name).ToList();
                if (matches.Count > 1) throw new InvalidDataException("Duplicate preference values are not supported.");
                var setting = matches.SingleOrDefault();
                previous[name] = setting == null ? null : (string)setting.Element("value");
                if (setting == null) { setting = new XElement("setting", new XAttribute("name", name)); section.Add(setting); }
                setting.SetAttributeValue("serializeAs", "String");
                setting.ReplaceNodes(new XElement("value", values[i]));
            }
            AtomicWrite(Path.Combine(backupDirectory, "previous-target-preferences.json"), JsonConvert.SerializeObject(previous, Newtonsoft.Json.Formatting.Indented));
            return new UpdateSettingsMigration(destination, original, xml.ToString());
        }

        internal static string ReadBoundedXmlText(string path)
        {
            return ReadBoundedText(path, 1048576);
        }

        internal static string ReadBoundedText(string path, int maximumBytes)
        {
            RejectMissingPath(path);
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (file.Length > maximumBytes) throw new InvalidDataException("The update configuration is unexpectedly large.");
                using (var reader = new StreamReader(file, Encoding.UTF8, true))
                {
                    var buffer = new char[maximumBytes + 1];
                    int count = reader.ReadBlock(buffer, 0, buffer.Length);
                    if (count > maximumBytes) throw new InvalidDataException("The update configuration exceeds its size limit.");
                    return new string(buffer, 0, count);
                }
            }
        }

        private static void RejectMissingPath(string path)
        {
            UpdateApplyWorker.RejectReparsePoints(path);
            if (!File.Exists(path)) throw new FileNotFoundException("The update configuration is unavailable.");
        }

        internal static XDocument ParseXml(string text)
        {
            using (var input = new StringReader(text))
            using (var reader = XmlReader.Create(input, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 1048576,
            })) return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
        }
    }

    internal sealed class UpdateSettingsMigration
    {
        private readonly string destination;
        private readonly string original;
        private readonly string replacement;
        private bool applied;
        internal UpdateSettingsMigration(string destination, string original, string replacement)
        { this.destination = destination; this.original = original; this.replacement = replacement; }
        internal void Apply()
        {
            // Do not overwrite a concurrent settings edit made after the migration was prepared.
            string current = File.Exists(destination) ? UpdatePreferences.ReadBoundedXmlText(destination) : null;
            if (!string.Equals(current, original, StringComparison.Ordinal)) throw new IOException("Settings changed while the update was prepared.");
            UpdatePreferences.AtomicWrite(destination, replacement);
            applied = true;
        }
        internal void Rollback()
        {
            if (!applied) return;
            if (!File.Exists(destination) || UpdatePreferences.ReadBoundedXmlText(destination) != replacement)
                throw new IOException("Settings were changed after migration; the update did not overwrite that change.");
            if (original == null) File.Delete(destination);
            else UpdatePreferences.AtomicWrite(destination, original);
            applied = false;
        }
    }
}
