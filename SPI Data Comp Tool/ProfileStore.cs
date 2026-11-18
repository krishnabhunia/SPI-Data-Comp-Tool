using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Builds candidate profile lists without changing the active list, then saves atomically.</summary>
    internal static class ProfileStore
    {
        internal static List<DBC> ParseJson(string json)
        {
            var array = JToken.Parse(json) as JArray;
            if (array == null) throw new InvalidDataException("The profile file must contain a JSON list.");
            foreach (JToken token in array)
            {
                var profile = token as JObject;
                // DBC's constructor supplies blank connection objects when a JSON property is
                // absent. Validate the original document so an incomplete import cannot appear valid.
                if (profile == null ||
                    !(profile.GetValue("dbSRC", StringComparison.OrdinalIgnoreCase) is JObject) ||
                    !(profile.GetValue("dbTRG", StringComparison.OrdinalIgnoreCase) is JObject))
                    throw new InvalidDataException("Every profile must explicitly contain Source (dbSRC) and Target (dbTRG) connection objects.");
            }
            List<DBC> profiles = array.ToObject<List<DBC>>();
            Validate(profiles);
            return profiles;
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
