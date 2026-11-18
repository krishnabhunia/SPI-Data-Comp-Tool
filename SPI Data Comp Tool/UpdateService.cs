using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Stable = normal releases only. Beta = also builds made from open pull requests.</summary>
    internal enum UpdateChannel
    {
        Stable,
        Beta,
    }

    /// <summary>A version as CI names it: x.y.z, or x.y.z-betaNNN (which ranks below x.y.z).</summary>
    internal struct ReleaseVersion : IComparable<ReleaseVersion>
    {
        private static readonly Regex Pattern = new Regex(@"^v?(\d+)\.(\d+)\.(\d+)(?:-beta(\d{1,3}))?$", RegexOptions.IgnoreCase);

        public int Major;
        public int Minor;
        public int Patch;
        public int Beta;      // 0 = not a beta
        public bool IsBeta { get { return Beta > 0; } }

        public static bool TryParse(string text, out ReleaseVersion version)
        {
            version = new ReleaseVersion();
            Match m = Pattern.Match((text ?? string.Empty).Trim());
            if (!m.Success) return false;
            version.Major = int.Parse(m.Groups[1].Value);
            version.Minor = int.Parse(m.Groups[2].Value);
            version.Patch = int.Parse(m.Groups[3].Value);
            version.Beta = m.Groups[4].Success ? int.Parse(m.Groups[4].Value) : 0;
            // "-beta000" is not a name CI makes; treat it as unreadable rather than as a normal release
            return !(m.Groups[4].Success && version.Beta == 0);
        }

        public int CompareTo(ReleaseVersion other)
        {
            if (Major != other.Major) return Major.CompareTo(other.Major);
            if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
            if (Patch != other.Patch) return Patch.CompareTo(other.Patch);
            if (IsBeta != other.IsBeta) return IsBeta ? -1 : 1;   // 2.5.0-beta007 < 2.5.0
            return Beta.CompareTo(other.Beta);
        }

        public override string ToString()
        {
            return Major + "." + Minor + "." + Patch + (IsBeta ? "-beta" + Beta.ToString("000") : string.Empty);
        }
    }

    /// <summary>One GitHub release, reduced to what the updater needs.</summary>
    internal sealed class ReleaseInfo
    {
        public string Tag;
        public ReleaseVersion Version;
        public string Notes;
        public string PageUrl;
        public string SetupName, SetupUrl;
        public long SetupSize;
        public string ZipName, ZipUrl;
        public long ZipSize;
        public string ShaUrl;
        public DateTime Published;

        public bool IsBeta { get { return Version.IsBeta; } }
    }

    /// <summary>The user's update choices. Kept outside the app folder, so an update never resets them.</summary>
    internal sealed class UpdateSettings
    {
        public UpdateChannel Channel = UpdateChannel.Stable;
        public bool InstallSilently;

        public static string FilePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                    "SPI Data Comparison Tool", "updates.json");
            }
        }

        public static UpdateSettings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    UpdateSettings loaded = JsonConvert.DeserializeObject<UpdateSettings>(File.ReadAllText(FilePath));
                    if (loaded != null) return loaded;
                }
            }
            catch (Exception)
            {
                // an unreadable file means the defaults: Stable, no silent install
            }
            return new UpdateSettings();
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }

    /// <summary>
    /// Finds, downloads, checks and installs updates from the app's GitHub releases.
    /// An installed copy (64-bit installer) is updated by running the new installer; a portable copy gets the
    /// new portable files beside the running exe, and the old exe stays as the way back.
    /// </summary>
    internal static class UpdateService
    {
        public const string Repository = "krishnabhunia/SPI-Data-Comp-Tool";

        /// <summary>A verified download waiting to be installed when the app closes.</summary>
        public static string PendingFile;

        /// <summary>Raised (on any thread) after the channel or silent-install choice was saved.</summary>
        public static event EventHandler SettingsChanged;

        public static void NotifySettingsChanged()
        {
            EventHandler handler = SettingsChanged;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        public static ReleaseVersion Current
        {
            get
            {
                ReleaseVersion v;
                return ReleaseVersion.TryParse(AppVersion.Full, out v) ? v : new ReleaseVersion();
            }
        }

        public static string AppFolder { get { return Path.GetDirectoryName(Application.ExecutablePath); } }

        /// <summary>True for a copy put there by the 64-bit installer (it leaves its uninstaller beside the exe).</summary>
        public static bool IsInstalled { get { return File.Exists(Path.Combine(AppFolder, "unins000.exe")); } }

        public static string ChannelName(UpdateChannel channel)
        {
            return channel == UpdateChannel.Beta ? "Beta" : "Stable";
        }

        // ---------- pure logic (covered by tests/UpdateChecks.cs) ----------

        /// <summary>Reads the GitHub "list releases" JSON. Drafts and tags CI did not make are left out.</summary>
        public static List<ReleaseInfo> ParseReleases(string json)
        {
            var result = new List<ReleaseInfo>();
            foreach (JToken item in JArray.Parse(json))
            {
                ReleaseVersion version;
                string tag = (string)item["tag_name"];
                if ((bool?)item["draft"] == true || !ReleaseVersion.TryParse(tag, out version)) continue;
                // GitHub's own pre-release mark and the tag name must agree; a mismatch is not a build CI published
                if (((bool?)item["prerelease"] ?? false) != version.IsBeta) continue;
                var release = new ReleaseInfo
                {
                    Tag = tag,
                    Version = version,
                    Notes = (string)item["body"] ?? string.Empty,
                    PageUrl = (string)item["html_url"],
                    Published = (DateTime?)item["published_at"] ?? DateTime.MinValue,
                };
                JToken assets = item["assets"];
                if (assets != null)
                {
                    foreach (JToken asset in assets)
                    {
                        string name = (string)asset["name"] ?? string.Empty;
                        string url = (string)asset["browser_download_url"];
                        long size = (long?)asset["size"] ?? 0;
                        if (name.EndsWith("_x64_setup.exe", StringComparison.OrdinalIgnoreCase)) { release.SetupName = name; release.SetupUrl = url; release.SetupSize = size; }
                        else if (name.EndsWith(".zip.sha256", StringComparison.OrdinalIgnoreCase)) { release.ShaUrl = url; }
                        else if (name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) && name.IndexOf("screenshots", StringComparison.OrdinalIgnoreCase) < 0) { release.ZipName = name; release.ZipUrl = url; release.ZipSize = size; }
                    }
                }
                result.Add(release);
            }
            return result;
        }

        /// <summary>The newest release above <paramref name="current"/> that this copy can install, or null.</summary>
        public static ReleaseInfo PickUpdate(IEnumerable<ReleaseInfo> releases, ReleaseVersion current, UpdateChannel channel, bool installed)
        {
            List<ReleaseInfo> all = releases.ToList();
            // A beta published before the newest normal release was built on older code (or is a leftover):
            // installing it would take fixes away, so it is not offered.
            DateTime newestNormal = DateTime.MinValue;
            foreach (ReleaseInfo release in all)
            {
                if (!release.IsBeta && release.Published > newestNormal) newestNormal = release.Published;
            }
            ReleaseInfo best = null;
            foreach (ReleaseInfo release in all)
            {
                if (release.IsBeta && channel != UpdateChannel.Beta) continue;
                if (release.IsBeta && release.Published < newestNormal) continue;
                if (release.Version.CompareTo(current) <= 0) continue;
                // without its checksum file a download cannot be verified, so the release is not offered
                if (string.IsNullOrEmpty(release.ShaUrl)) continue;
                if (string.IsNullOrEmpty(installed ? release.SetupUrl : release.ZipUrl)) continue;
                if (best == null || release.Version.CompareTo(best.Version) > 0) best = release;
            }
            return best;
        }

        /// <summary>The SHA-256 the release's checksum file lists for <paramref name="fileName"/>, or null.</summary>
        public static string ExpectedSha256(string checksumFileText, string fileName)
        {
            foreach (string line in (checksumFileText ?? string.Empty).Split('\n'))
            {
                string[] parts = line.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2 || parts[0].Length != 64) continue;
                string listed = parts[parts.Length - 1].TrimStart('*');
                if (string.Equals(listed, fileName, StringComparison.OrdinalIgnoreCase)) return parts[0].ToLowerInvariant();
            }
            return null;
        }

        public static string Sha256Of(string file)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream stream = File.OpenRead(file))
            {
                var text = new StringBuilder();
                foreach (byte b in sha.ComputeHash(stream)) text.Append(b.ToString("x2"));
                return text.ToString();
            }
        }

        public static string SizeText(long bytes)
        {
            return bytes <= 0 ? "unknown size" : (bytes / 1048576.0).ToString("0.0") + " MB";
        }

        // ---------- network ----------

        private static HttpWebRequest Request(string url)
        {
            // .NET Framework 4.6.2 does not offer TLS 1.2 by default; GitHub accepts nothing older
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "SPI-Data-Comparison-Tool/" + AppVersion.Full;
            request.Accept = "application/vnd.github+json, application/octet-stream, */*";
            request.Timeout = 30000;
            request.ReadWriteTimeout = 30000;
            return request;
        }

        private static string GetText(string url)
        {
            using (WebResponse response = Request(url).GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>Asks GitHub for the releases and returns the update for this copy and channel, or null.</summary>
        public static ReleaseInfo FindUpdate(UpdateChannel channel)
        {
            string json = GetText("https://api.github.com/repos/" + Repository + "/releases?per_page=40");
            return PickUpdate(ParseReleases(json), Current, channel, IsInstalled);
        }

        /// <summary>
        /// Downloads the installer (installed copy) or the zip (portable copy) and checks it against the
        /// release's SHA-256. Returns the verified file; throws if the check fails.
        /// </summary>
        public static string Download(ReleaseInfo release, Action<int> progress)
        {
            bool installed = IsInstalled;
            string name = installed ? release.SetupName : release.ZipName;
            string url = installed ? release.SetupUrl : release.ZipUrl;
            long size = installed ? release.SetupSize : release.ZipSize;

            string expected = ExpectedSha256(GetText(release.ShaUrl), name);
            if (expected == null) throw new InvalidDataException("The release has no checksum for " + name + ".");

            string folder = Path.Combine(Path.GetTempPath(), "SPI Data Comparison Tool updates", release.Tag);
            Directory.CreateDirectory(folder);
            string target = Path.Combine(folder, name);
            if (!File.Exists(target) || Sha256Of(target) != expected)
            {
                string partial = target + ".part";
                using (WebResponse response = Request(url).GetResponse())
                using (Stream input = response.GetResponseStream())
                using (FileStream output = File.Create(partial))
                {
                    var buffer = new byte[81920];
                    long done = 0;
                    int read, last = -1;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                        done += read;
                        int percent = size > 0 ? (int)Math.Min(100, done * 100 / size) : 0;
                        if (percent != last && progress != null) { last = percent; progress(percent); }
                    }
                }
                if (File.Exists(target)) File.Delete(target);
                File.Move(partial, target);
            }
            string actual = Sha256Of(target);
            if (actual != expected)
            {
                File.Delete(target);
                throw new InvalidDataException("The download did not match the release's SHA-256 and was deleted. Nothing was installed.");
            }
            return target;
        }

        // ---------- install ----------

        /// <summary>
        /// Installs a file <see cref="Download"/> verified. Installed copy: runs the installer (it asks Windows for
        /// permission). Portable copy: puts the new portable files beside this exe; nothing existing is overwritten.
        /// Returns the exe to start afterwards, or null when the installer restarts the app itself.
        /// </summary>
        public static string Install(string verifiedFile, bool restartAfterwards)
        {
            if (verifiedFile.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                string arguments = "/SILENT /SUPPRESSMSGBOXES /NORESTART /CLOSEAPPLICATIONS" + (restartAfterwards ? " /RELAUNCH=1" : string.Empty);
                Process.Start(new ProcessStartInfo(verifiedFile, arguments) { UseShellExecute = true });
                return null;
            }
            return ExtractPortable(verifiedFile, AppFolder);
        }

        /// <summary>Copies the zip's portable/ files into <paramref name="folder"/>, keeping every existing file.</summary>
        public static string ExtractPortable(string zipFile, string folder)
        {
            string newExe = null;
            string root = Path.GetFullPath(folder).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            using (ZipArchive zip = ZipFile.OpenRead(zipFile))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    string name = entry.FullName.Replace('\\', '/');
                    if (!name.StartsWith("portable/", StringComparison.OrdinalIgnoreCase) || name.EndsWith("/")) continue;
                    string target = Path.GetFullPath(Path.Combine(folder, name.Substring("portable/".Length).Replace('/', Path.DirectorySeparatorChar)));
                    // a zip entry must never land outside the app folder
                    if (!target.StartsWith(root, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Unsafe path in the update: " + entry.FullName);
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    if (!File.Exists(target)) entry.ExtractToFile(target);
                    if (target.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && Path.GetDirectoryName(target) + Path.DirectorySeparatorChar == root) newExe = target;
                }
            }
            if (newExe == null) throw new InvalidDataException("The update has no portable exe.");
            return newExe;
        }
    }
}
