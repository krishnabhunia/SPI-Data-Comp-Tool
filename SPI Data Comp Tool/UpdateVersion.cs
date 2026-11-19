using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SPI_Data_Comp_Tool
{
    /// <summary>The three-part release version, independent of four-part assembly versions.</summary>
    internal sealed class UpdateVersion : IComparable<UpdateVersion>
    {
        private static readonly Regex Pattern = new Regex(
            @"\Av?(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(?:-beta([0-9]{3}))?\z",
            RegexOptions.CultureInvariant);

        public int Major { get; private set; }
        public int Minor { get; private set; }
        public int Patch { get; private set; }
        public int? Beta { get; private set; }
        public bool IsBeta { get { return Beta.HasValue; } }

        private UpdateVersion(int major, int minor, int patch, int? beta)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Beta = beta;
        }

        public static bool TryParse(string text, out UpdateVersion version)
        {
            version = null;
            if (string.IsNullOrEmpty(text) || text.Length > 80) return false;
            Match match = Pattern.Match(text);
            int major, minor, patch, beta;
            if (!match.Success ||
                !int.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out major) ||
                !int.TryParse(match.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out minor) ||
                !int.TryParse(match.Groups[3].Value, NumberStyles.None, CultureInfo.InvariantCulture, out patch))
                return false;

            int? betaNumber = null;
            if (match.Groups[4].Success)
            {
                if (!int.TryParse(match.Groups[4].Value, NumberStyles.None, CultureInfo.InvariantCulture, out beta) || beta < 1)
                    return false;
                betaNumber = beta;
            }
            version = new UpdateVersion(major, minor, patch, betaNumber);
            return true;
        }

        public int CompareTo(UpdateVersion other)
        {
            if (other == null) return 1;
            int result = Major.CompareTo(other.Major);
            if (result == 0) result = Minor.CompareTo(other.Minor);
            if (result == 0) result = Patch.CompareTo(other.Patch);
            if (result != 0) return result;
            if (!Beta.HasValue) return other.Beta.HasValue ? 1 : 0;
            return other.Beta.HasValue ? Beta.Value.CompareTo(other.Beta.Value) : -1;
        }

        public override string ToString()
        {
            string value = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", Major, Minor, Patch);
            return Beta.HasValue ? value + "-beta" + Beta.Value.ToString("000", CultureInfo.InvariantCulture) : value;
        }
    }
}
