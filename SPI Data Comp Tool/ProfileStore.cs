using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPI_Data_Comp_Tool
{
    /// <summary>The outcome of optional saved-profile loading; failure keeps the active catalog intact.</summary>
    internal sealed class ProfileLoadResult
    {
        internal ProfileLoadResult(string filePath, bool loaded, bool hasError, string message)
        {
            FilePath = filePath;
            Loaded = loaded;
            HasError = hasError;
            Message = message;
        }

        internal string FilePath { get; }
        internal bool Loaded { get; }
        internal bool HasError { get; }
        internal string Message { get; }
    }

    /// <summary>The exact catalog and file published together by a successful read or write.</summary>
    internal sealed class ProfileCatalog
    {
        internal ProfileCatalog(List<DBC> profiles, string filePath, string exportedContent)
        {
            Profiles = profiles;
            FilePath = filePath;
            ExportedContent = exportedContent;
        }

        internal List<DBC> Profiles { get; }
        internal string FilePath { get; }
        internal string ExportedContent { get; }
    }

    /// <summary>Builds candidate profile lists without changing the active list, then saves atomically.</summary>
    internal static class ProfileStore
    {
        private const int MaximumProfileBytes = 16 * 1024 * 1024;
        private static ProfileCatalog publishedCatalog;

        // Some legacy callers/tests can replace ProfileList directly. Such a list has no proven
        // source file; never carry the previous catalog's delete destination over to it.
        internal static ProfileCatalog ActiveCatalog
        {
            get { return publishedCatalog != null && ReferenceEquals(Profile.ProfileList, publishedCatalog.Profiles)
                    ? publishedCatalog : null; }
        }

        /// <summary>Reads and validates one complete document without changing settings or active profiles.</summary>
        internal static List<DBC> ReadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) ||
                !string.Equals(Path.GetExtension(path), ".json", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Select a JSON profile file.");
            using (var stream = new FileStream(Path.GetFullPath(path), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (stream.Length == 0 || stream.Length > MaximumProfileBytes)
                    throw new InvalidDataException("Select a nonempty profile JSON file no larger than 16 MiB.");
                try
                {
                    using (var reader = new StreamReader(stream, new UTF8Encoding(false, true), true))
                    {
                        var text = new StringBuilder();
                        var buffer = new char[4096];
                        int count;
                        while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (text.Length + count > MaximumProfileBytes || stream.Length > MaximumProfileBytes)
                                throw new InvalidDataException("Select a nonempty profile JSON file no larger than 16 MiB.");
                            text.Append(buffer, 0, count);
                        }
                        return ParseJson(text.ToString());
                    }
                }
                catch (DecoderFallbackException)
                {
                    throw new InvalidDataException("The profile file has invalid text encoding. Select a valid JSON profile file.");
                }
            }
        }

        internal static ProfileLoadResult LoadSavedProfiles()
        {
            ProfileCatalog active = ActiveCatalog;
            if (active != null)
                return new ProfileLoadResult(active.FilePath, true, false, "Using the active profiles from: " + active.FilePath);
            return LoadRememberedProfiles(() => Properties.Settings.Default.ProfilePathUser);
        }

        // The setting itself can be unreadable when user.config is damaged. Keep the optional
        // read inside this boundary and never repair/overwrite that file as a side effect.
        internal static ProfileLoadResult LoadRememberedProfiles(Func<string> readPath)
        {
            try { return LoadWithRememberedFallback(readPath()); }
            catch (Exception error) when (error is ConfigurationErrorsException ||
                error is SettingsPropertyWrongTypeException || error is SettingsPropertyNotFoundException ||
                error is IOException || error is UnauthorizedAccessException || error is SecurityException ||
                (error is TypeInitializationException && error.InnerException is ConfigurationErrorsException))
            {
                return LoadFailure(null, "The saved profile setting could not be read. Your settings file has not been changed.");
            }
        }

        /// <summary>
        /// A new copy of the app (a new download folder) starts with an empty ProfilePathUser setting.
        /// When the setting is empty or names a file that no longer exists, the last profiles file
        /// chosen on this Windows account is used instead; it is never written into the settings.
        /// </summary>
        private static ProfileLoadResult LoadWithRememberedFallback(string path)
        {
            bool usable = !string.IsNullOrWhiteSpace(path) && File.Exists(path);
            if (!usable)
            {
                string remembered = ReadRememberedPath();
                if (remembered != null && !string.Equals(remembered, path, StringComparison.OrdinalIgnoreCase))
                {
                    ProfileLoadResult fallback = LoadSavedProfiles(remembered);
                    if (fallback.Loaded)
                        return new ProfileLoadResult(remembered, true, false, (string.IsNullOrWhiteSpace(path)
                            ? "This copy of the app has no saved profile file yet; "
                            : "The saved profile file " + path + " could not be found; ") +
                            "using the last one chosen on this computer. " + fallback.Message);
                    // A new copy with an empty per-copy setting must still explain a damaged,
                    // missing or locked shared file instead of silently showing no profiles.
                    return fallback;
                }
            }
            ProfileLoadResult result = LoadSavedProfiles(path);
            if (result.Loaded) RememberPath(path);
            return result;
        }

        /// <summary>Build checks only: where the remembered path is kept instead of the user's AppData.</summary>
        internal static string RememberedPathFileOverride;

        /// <summary>The last profiles file chosen on this Windows account, kept beside theme.txt. Null while the build checks run.</summary>
        internal static string RememberedPathFile
        {
            get
            {
                if (RememberedPathFileOverride != null) return RememberedPathFileOverride;
                return SelfTest.IsRunning ? null : Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SPI Data Comparison Tool", "profile-path.txt");
            }
        }

        internal static string ReadRememberedPath()
        {
            try
            {
                string file = RememberedPathFile;
                if (file == null || !File.Exists(file) || new FileInfo(file).Length > 32768) return null;
                string path = File.ReadAllText(file).Trim();
                return path.Length == 0 || path.IndexOf('\0') >= 0 ? null : path;
            }
            catch (Exception)
            {
                return null;   // a missing or unreadable reminder is not worth an error
            }
        }

        /// <summary>Remembers the chosen profiles file for other copies of the app. Best effort.</summary>
        internal static void RememberPath(string path)
        {
            TryRememberPath(path);
        }

        private static bool TryRememberPath(string path)
        {
            try
            {
                string file = RememberedPathFile;
                if (file == null) return true; // Deliberately disabled during isolated self-tests.
                if (string.IsNullOrWhiteSpace(path)) return false;
                string full = Path.GetFullPath(path);
                if (string.Equals(ReadRememberedPath(), full, StringComparison.OrdinalIgnoreCase)) return true;
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                WriteAtomic(file, full);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Optional startup load: publish only validated documents; report expected file problems without aborting a form.</summary>
        internal static ProfileLoadResult LoadSavedProfiles(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return new ProfileLoadResult(path, false, false, string.Empty);
            try
            {
                List<DBC> candidate = ReadFile(path);
                ProfileCatalog catalog = Publish(candidate, path);
                return new ProfileLoadResult(catalog.FilePath, true, false, "Profiles loaded from: " + catalog.FilePath);
            }
            catch (Exception error) when (error is InvalidDataException || error is IOException || error is UnauthorizedAccessException ||
                error is SecurityException || error is ArgumentException || error is NotSupportedException)
            {
                string reason;
                if (error is InvalidDataException) reason = error.Message;
                else if (error is FileNotFoundException || error is DirectoryNotFoundException)
                    reason = "The saved profile file could not be found.";
                else if (error is UnauthorizedAccessException || error is SecurityException)
                    reason = "The saved profile file cannot be read with the current permissions.";
                else if (error is ArgumentException || error is NotSupportedException)
                    reason = "The saved profile location is invalid.";
                else reason = "The saved profile file could not be read. It may be locked or unavailable.";
                return LoadFailure(path, reason);
            }
        }

        internal static ProfileLoadResult ImportFile(string path, Action<string> persistPath)
        {
            ProfileLoadResult result = LoadSavedProfiles(path);
            return result.Loaded ? RememberChoice(result, persistPath) : result;
        }

        internal static ProfileLoadResult SaveFile(string path, List<DBC> candidate, ProfileCatalog expected,
            Action<string> persistPath)
        {
            RequireCurrentCatalog(expected);
            if (!string.Equals(Path.GetExtension(path), ".json", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Select a JSON profile file.");
            string fullPath = Path.GetFullPath(path);
            if (expected != null && string.Equals(expected.FilePath, fullPath, StringComparison.OrdinalIgnoreCase))
                RequireUnchangedFile(expected);
            string json = SerializeForExport(candidate);
            if (Encoding.UTF8.GetByteCount(json) > MaximumProfileBytes)
                throw new InvalidDataException("A saved profile JSON file cannot exceed 16 MiB.");
            WriteAtomic(fullPath, json);
            ProfileCatalog catalog = Publish(candidate, fullPath);
            return RememberChoice(new ProfileLoadResult(catalog.FilePath, true, false,
                "Profiles saved to: " + catalog.FilePath), persistPath);
        }

        internal static void RequireCurrentCatalog(ProfileCatalog expected)
        {
            if (!ReferenceEquals(expected, ActiveCatalog) || (expected != null &&
                !string.Equals(SerializeForExport(expected.Profiles), expected.ExportedContent, StringComparison.Ordinal)))
                throw new InvalidOperationException("The active profiles changed in another window. Load the profile file again before saving or deleting.");
        }

        internal static void RequireUnchangedFile(ProfileCatalog expected)
        {
            if (expected == null)
                throw new InvalidOperationException("Load or save a profile file before deleting a profile.");
            RequireCurrentCatalog(expected);
            List<DBC> currentFile = ReadFile(expected.FilePath);
            if (!string.Equals(SerializeForExport(currentFile), expected.ExportedContent, StringComparison.Ordinal))
                throw new InvalidOperationException("The profile file changed after it was loaded. Reload it before saving or deleting a profile.");
        }

        internal static ProfileCatalog DeleteProfile(ProfileCatalog expected, string name)
        {
            // Recheck after the confirmation dialog, which can dispatch other windows' events.
            RequireUnchangedFile(expected);
            List<DBC> candidate = Remove(expected.Profiles, name);
            if (candidate.Count == expected.Profiles.Count)
                throw new InvalidOperationException("Select a saved profile to delete.");
            WriteAtomic(expected.FilePath, SerializeForExport(candidate));
            return Publish(candidate, expected.FilePath);
        }

        private static ProfileCatalog Publish(List<DBC> profiles, string path)
        {
            string exported = SerializeForExport(profiles);
            var catalog = new ProfileCatalog(profiles, Path.GetFullPath(path), exported);
            Profile.ProfileList = profiles;
            publishedCatalog = catalog;
            return catalog;
        }

        private static ProfileLoadResult RememberChoice(ProfileLoadResult result, Action<string> persistPath)
        {
            bool localSaved = true;
            try { persistPath?.Invoke(result.FilePath); }
            catch (Exception) { localSaved = false; }
            bool sharedSaved = TryRememberPath(result.FilePath);
            if (localSaved && sharedSaved) return result;
            string warning = !localSaved
                ? " This copy could not remember the profile location. The profiles are usable for this session; load this file again if needed after restarting."
                : " The profile location could not be remembered for other copies of the app. These profiles remain usable in this copy.";
            return new ProfileLoadResult(result.FilePath, true, true, result.Message + warning);
        }

        private static ProfileLoadResult LoadFailure(string path, string reason)
        {
            return new ProfileLoadResult(path, false, true, reason +
                " Open Settings > Connection profiles > Load profile to select a valid JSON file." +
                " Current profiles and entered connection details are unchanged.");
        }

        internal static List<DBC> ParseJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json.Length > 16 * 1024 * 1024)
                throw new InvalidDataException("Select a nonempty profile JSON file no larger than 16 MiB.");
            JArray array;
            try
            {
                using (var text = new StringReader(json))
                using (var reader = new JsonTextReader(text) { MaxDepth = 32, DateParseHandling = DateParseHandling.None })
                {
                    array = JToken.ReadFrom(reader, new JsonLoadSettings
                    {
                        DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
                    }) as JArray;
                    if (reader.Read()) throw new InvalidDataException("The profile file contains additional JSON content.");
                }
            }
            catch (JsonException)
            {
                throw new InvalidDataException("The profile file is not valid JSON or contains duplicate properties.");
            }
            if (array == null) throw new InvalidDataException("The profile file must contain a JSON list.");
            if (array.Count > 10000) throw new InvalidDataException("A profile file cannot contain more than 10,000 profiles.");
            foreach (JToken token in array)
            {
                var profile = token as JObject;
                // DBC's constructor supplies blank connection objects when a JSON property is
                // absent. Validate the original document so an incomplete import cannot appear valid.
                if (profile == null ||
                    !(profile.GetValue("dbSRC", StringComparison.OrdinalIgnoreCase) is JObject) ||
                    !(profile.GetValue("dbTRG", StringComparison.OrdinalIgnoreCase) is JObject))
                    throw new InvalidDataException("Every profile must explicitly contain Source (dbSRC) and Target (dbTRG) connection objects.");
                ValidateJsonProperties(profile);
                if (profile.GetValue("profileName", StringComparison.OrdinalIgnoreCase)?.Type != JTokenType.String)
                    throw new InvalidDataException("Every profile name must be text.");
                ValidateConnectionJson((JObject)profile.GetValue("dbSRC", StringComparison.OrdinalIgnoreCase));
                ValidateConnectionJson((JObject)profile.GetValue("dbTRG", StringComparison.OrdinalIgnoreCase));
            }
            List<DBC> profiles;
            try { profiles = array.ToObject<List<DBC>>(); }
            catch (JsonException) { throw new InvalidDataException("The profile file contains invalid connection values."); }
            Validate(profiles);
            foreach (DBC profile in profiles)
            {
                RestoreProviderChoice(profile.dbSRC);
                RestoreProviderChoice(profile.dbTRG);
            }
            return profiles;
        }

        private static void RestoreProviderChoice(DBConnectionDetailsProfile side)
        {
            // Existing example/legacy documents express the provider through str_DBType alone.
            // Validation has already rejected conflicting explicit flags and unsupported names.
            if (side.bool_Rb_Oracle || side.bool_Rb_SQL) return;
            side.bool_Rb_Oracle = string.Equals(side.str_DBType, "Oracle", StringComparison.OrdinalIgnoreCase);
            side.bool_Rb_SQL = string.Equals(side.str_DBType, "SQL", StringComparison.OrdinalIgnoreCase);
        }

        private static void ValidateJsonProperties(JObject value)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (JProperty property in value.Properties())
                if (!names.Add(property.Name))
                    throw new InvalidDataException("The profile file contains ambiguous repeated property names.");
        }

        private static void ValidateConnectionJson(JObject connection)
        {
            ValidateJsonProperties(connection);
            foreach (string name in new[] { "bool_Rb_Oracle", "bool_Rb_SQL", "bool_Oracle_DefaultPort", "bool_SQL_WA" })
            {
                JToken value = connection.GetValue(name, StringComparison.OrdinalIgnoreCase);
                if (value != null && value.Type != JTokenType.Boolean)
                    throw new InvalidDataException("Connection platform and authentication choices must be JSON true or false.");
            }
            JToken port = connection.GetValue("int_Oracle_DefaultPort", StringComparison.OrdinalIgnoreCase);
            if (port != null)
            {
                if (port.Type != JTokenType.Integer)
                    throw new InvalidDataException("The saved Oracle port must be an integer.");
                // JSON integers can be backed by BigInteger. Check the token before ToObject
                // attempts an Int32 conversion, which otherwise throws an uncaught overflow.
                int savedPort;
                if (!int.TryParse(port.ToString(Formatting.None), NumberStyles.Integer, CultureInfo.InvariantCulture,
                    out savedPort) || savedPort < 0 || savedPort > 65535)
                    throw new InvalidDataException("The saved Oracle port must be between 1 and 65535, or 0 for the default.");
            }
            foreach (string name in new[] { "str_DBType", "str_DataSource", "str_DBName", "str_UserName", "str_Password", "str_SchemaName", "str_TableName" })
            {
                JToken value = connection.GetValue(name, StringComparison.OrdinalIgnoreCase);
                if (value != null && value.Type != JTokenType.String && value.Type != JTokenType.Null)
                    throw new InvalidDataException("Saved connection names and credentials must be text.");
            }
        }

        internal static List<DBC> Upsert(IList<DBC> current, DBC replacement)
        {
            if (replacement == null || string.IsNullOrWhiteSpace(replacement.profileName))
                throw new ArgumentException("Enter a profile name before saving.");
            ValidateProfile(replacement);

            var candidate = new List<DBC>();
            bool replaced = false;
            if (current != null)
            {
                foreach (DBC profile in current)
                {
                    ValidateProfile(profile);
                    if (string.Equals(profile.profileName, replacement.profileName, StringComparison.Ordinal))
                    {
                        if (!replaced) candidate.Add(Clone(replacement));
                        replaced = true;
                    }
                    else candidate.Add(Clone(profile));
                }
            }
            if (!replaced) candidate.Add(Clone(replacement));
            return candidate;
        }

        internal static List<DBC> Remove(IList<DBC> current, string name)
        {
            var candidate = new List<DBC>();
            if (current == null) return candidate;
            foreach (DBC profile in current)
            {
                ValidateProfile(profile);
                if (!string.Equals(profile.profileName, name, StringComparison.Ordinal))
                    candidate.Add(Clone(profile));
            }
            return candidate;
        }

        /// <summary>
        /// Every persisted entry, including unedited profiles imported from a legacy file, must be
        /// password-free. Clear only a deep export copy: serialization must not change the active
        /// profiles or credentials the user is currently using in memory.
        /// </summary>
        internal static List<DBC> CreateExportSnapshot(IList<DBC> profiles)
        {
            Validate(profiles);
            var export = new List<DBC>();
            foreach (DBC profile in profiles)
            {
                DBC snapshot = Clone(profile);
                snapshot.dbSRC.str_Password = string.Empty;
                snapshot.dbTRG.str_Password = string.Empty;
                export.Add(snapshot);
            }
            return export;
        }

        internal static string SerializeForExport(IList<DBC> profiles)
        {
            return JsonConvert.SerializeObject(CreateExportSnapshot(profiles), Formatting.Indented);
        }

        internal static void Validate(IList<DBC> profiles)
        {
            if (profiles == null) throw new InvalidDataException("The profile file must contain a JSON list.");
            if (profiles.Count > 10000) throw new InvalidDataException("A profile file cannot contain more than 10,000 profiles.");
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (DBC profile in profiles)
            {
                ValidateProfile(profile);
                if (!names.Add(profile.profileName))
                    throw new InvalidDataException("The profile file contains duplicate profile names.");
            }
        }

        private static void ValidateProfile(DBC profile)
        {
            if (profile == null || string.IsNullOrWhiteSpace(profile.profileName) || profile.dbSRC == null || profile.dbTRG == null)
                throw new InvalidDataException("Every profile needs a name, Source and Target connection details.");
            if (ConnectionSide.ContainsControlCharacters(profile.profileName))
                throw new InvalidDataException("Profile names cannot contain line breaks or control characters.");
            ValidateConnectionProfile(profile.dbSRC);
            ValidateConnectionProfile(profile.dbTRG);
        }

        private static void ValidateConnectionProfile(DBConnectionDetailsProfile side)
        {
            // Empty connection objects are existing partial templates, not credentials. Require
            // complete values only when connecting; reject declared values that contradict each other.
            if (side.bool_Rb_Oracle && side.bool_Rb_SQL)
                throw new InvalidDataException("A saved connection cannot select Oracle and SQL Server together.");
            if ((side.bool_Rb_Oracle || string.Equals(side.str_DBType, "Oracle", StringComparison.OrdinalIgnoreCase)) && side.bool_SQL_WA)
                throw new InvalidDataException("Oracle profiles cannot use SQL Server Windows authentication.");
            if (!string.IsNullOrWhiteSpace(side.str_DBType))
            {
                bool oracle = string.Equals(side.str_DBType, "Oracle", StringComparison.OrdinalIgnoreCase);
                bool sql = string.Equals(side.str_DBType, "SQL", StringComparison.OrdinalIgnoreCase);
                if ((!oracle && !sql) || (side.bool_Rb_Oracle && !oracle) || (side.bool_Rb_SQL && !sql))
                    throw new InvalidDataException("The saved database type is invalid or disagrees with its platform choice.");
            }
            // Zero is the historic unset/default-port sentinel and remains importable.
            if (side.int_Oracle_DefaultPort < 0 || side.int_Oracle_DefaultPort > 65535)
                throw new InvalidDataException("The saved Oracle port must be between 1 and 65535, or 0 for the default.");
            foreach (string value in new[] { side.str_DataSource, side.str_DBName, side.str_UserName, side.str_SchemaName, side.str_TableName })
                if (ConnectionSide.ContainsControlCharacters(value))
                    throw new InvalidDataException("Saved connection names cannot contain line breaks or control characters.");
        }

        private static DBC Clone(DBC profile)
        {
            return new DBC { profileName = profile.profileName, dbSRC = Clone(profile.dbSRC), dbTRG = Clone(profile.dbTRG) };
        }

        private static DBConnectionDetailsProfile Clone(DBConnectionDetailsProfile side)
        {
            return new DBConnectionDetailsProfile
            {
                str_DBType = side.str_DBType,
                bool_Rb_Oracle = side.bool_Rb_Oracle,
                bool_Rb_SQL = side.bool_Rb_SQL,
                bool_Oracle_DefaultPort = side.bool_Oracle_DefaultPort,
                int_Oracle_DefaultPort = side.int_Oracle_DefaultPort,
                bool_SQL_WA = side.bool_SQL_WA,
                str_DataSource = side.str_DataSource,
                str_DBName = side.str_DBName,
                str_UserName = side.str_UserName,
                str_Password = side.str_Password,
                str_SchemaName = side.str_SchemaName,
                str_TableName = side.str_TableName,
            };
        }

        internal static void WriteAtomic(string path, string json)
        {
            string destination = Path.GetFullPath(path);
            string temporary = Path.Combine(Path.GetDirectoryName(destination), ".spi-profile-" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    writer.Write(json);
                    writer.Flush();
                    stream.Flush(true);
                }
                // Same-directory replacement keeps the existing file intact if writing fails.
                // Never fall back to truncating it on a filesystem that cannot replace atomically.
                if (File.Exists(destination)) File.Replace(temporary, destination, null);
                else File.Move(temporary, destination);
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }
    }
}
