using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Checks of the updater's decisions, run by CI as "exe --check-updates FILE". Synthetic data only:
    /// nothing is downloaded and no setting is read or saved.
    /// </summary>
    internal static class UpdateChecks
    {
        private const string Releases = @"[
 {""tag_name"":""v2.7.0-beta001"",""prerelease"":true,""draft"":false,""published_at"":""2026-11-10T08:00:00Z"",""body"":""leftover"",
  ""assets"":[{""name"":""SPI_Data_Comp_Tool_2.7.0-beta001.zip"",""size"":1,""browser_download_url"":""https://example.invalid/s.zip""},
             {""name"":""SPI_Data_Comp_Tool_2.7.0-beta001.zip.sha256"",""size"":1,""browser_download_url"":""https://example.invalid/s.sha""},
             {""name"":""SPI_Data_Comp_Tool_2.7.0-beta001_x64_setup.exe"",""size"":1,""browser_download_url"":""https://example.invalid/s.exe""}]},
 {""tag_name"":""v2.6.0-beta002"",""prerelease"":true,""draft"":false,""published_at"":""2026-11-20T08:00:00Z"",""body"":""beta two"",""html_url"":""https://example.invalid/b2"",
  ""assets"":[{""name"":""SPI_Data_Comp_Tool_2.6.0-beta002.zip"",""size"":9000000,""browser_download_url"":""https://example.invalid/b2.zip""},
             {""name"":""SPI_Data_Comp_Tool_2.6.0-beta002.zip.sha256"",""size"":200,""browser_download_url"":""https://example.invalid/b2.sha""},
             {""name"":""SPI_Data_Comp_Tool_2.6.0-beta002_x64_setup.exe"",""size"":10000000,""browser_download_url"":""https://example.invalid/b2.exe""}]},
 {""tag_name"":""v2.5.7"",""prerelease"":false,""draft"":false,""published_at"":""2026-11-19T08:00:00Z"",""body"":""fixes"",""html_url"":""https://example.invalid/257"",
  ""assets"":[{""name"":""SPI_Data_Comp_Tool_2.5.7.zip"",""size"":9000000,""browser_download_url"":""https://example.invalid/257.zip""},
             {""name"":""SPI_Data_Comp_Tool_2.5.7.zip.sha256"",""size"":200,""browser_download_url"":""https://example.invalid/257.sha""},
             {""name"":""SPI_Data_Comp_Tool_2.5.7_x64_setup.exe"",""size"":10000000,""browser_download_url"":""https://example.invalid/257.exe""}]},
 {""tag_name"":""v2.5.8"",""prerelease"":false,""draft"":true,""body"":""draft"",""assets"":[]},
 {""tag_name"":""v2.9.0"",""prerelease"":false,""draft"":false,""published_at"":""2026-11-02T08:00:00Z"",""body"":""no checksum"",
  ""assets"":[{""name"":""SPI_Data_Comp_Tool_2.9.0.zip"",""size"":1,""browser_download_url"":""https://example.invalid/290.zip""}]},
 {""tag_name"":""v2.4.0"",""prerelease"":false,""draft"":false,""published_at"":""2026-11-01T08:00:00Z"",""body"":""old, zip only"",
  ""assets"":[{""name"":""SPI_Data_Comp_Tool_2.4.0.zip"",""size"":1,""browser_download_url"":""https://example.invalid/240.zip""},
             {""name"":""SPI_Data_Comp_Tool_2.4.0_screenshots.zip"",""size"":1,""browser_download_url"":""https://example.invalid/240s.zip""},
             {""name"":""SPI_Data_Comp_Tool_2.4.0.zip.sha256"",""size"":1,""browser_download_url"":""https://example.invalid/240.sha""}]},
 {""tag_name"":""nightly"",""prerelease"":false,""draft"":false,""assets"":[]},
 {""tag_name"":""v3.0.0-beta001"",""prerelease"":false,""draft"":false,""assets"":[]}
]";

        public static int Run(string outputFile)
        {
            var lines = new List<string>();
            try
            {
                Versions(lines);
                Picking(lines);
                Checksums(lines);
                PortableFiles(lines);
                File.WriteAllLines(outputFile, lines);
                return 0;
            }
            catch (Exception ex)
            {
                lines.Add("FAIL " + ex.Message);
                File.WriteAllLines(outputFile, lines);
                return 1;
            }
        }

        private static void Check(bool condition, string what, List<string> lines)
        {
            if (!condition) throw new InvalidOperationException(what);
            lines.Add("PASS " + what);
        }

        private static ReleaseVersion V(string text)
        {
            ReleaseVersion v;
            if (!ReleaseVersion.TryParse(text, out v)) throw new InvalidOperationException("cannot read version " + text);
            return v;
        }

        private static void Versions(List<string> lines)
        {
            ReleaseVersion ignored;
            Check(V("v2.5.0-beta007").CompareTo(V("2.5.0")) < 0, "a beta ranks below the normal version with the same number", lines);
            Check(V("2.5.0-beta007").CompareTo(V("2.4.9")) > 0, "a beta of a newer number ranks above an older normal version", lines);
            Check(V("2.5.0-beta010").CompareTo(V("2.5.0-beta009")) > 0, "beta numbers compare as numbers", lines);
            Check(V("2.10.0").CompareTo(V("2.9.9")) > 0, "version parts compare as numbers, not text", lines);
            Check(V("v2.5.0-beta007").ToString() == "2.5.0-beta007", "a version prints the way CI names it", lines);
            Check(!ReleaseVersion.TryParse("nightly", out ignored) && !ReleaseVersion.TryParse("2.5", out ignored)
                  && !ReleaseVersion.TryParse("2.5.0-beta000", out ignored) && !ReleaseVersion.TryParse("2.5.0-manual", out ignored),
                  "names CI does not make are not versions", lines);
        }

        private static void Picking(List<string> lines)
        {
            List<ReleaseInfo> releases = UpdateService.ParseReleases(Releases);
            Check(releases.Count == 5 && releases.All(r => r.Tag != "v2.5.8" && r.Tag != "nightly" && r.Tag != "v3.0.0-beta001"),
                  "drafts, foreign tags and a beta name not marked pre-release are left out", lines);
            Check(releases.First(r => r.Tag == "v2.4.0").ZipName == "SPI_Data_Comp_Tool_2.4.0.zip", "a screenshots zip is never taken for the app zip", lines);

            ReleaseInfo stable = UpdateService.PickUpdate(releases, V("2.5.6"), UpdateChannel.Stable, true);
            Check(stable != null && stable.Tag == "v2.5.7", "Stable is offered the newest normal release, never a beta", lines);
            ReleaseInfo beta = UpdateService.PickUpdate(releases, V("2.5.6"), UpdateChannel.Beta, true);
            Check(beta != null && beta.Tag == "v2.6.0-beta002", "Beta is offered the newest of normal releases and betas", lines);
            Check(beta.Tag != "v2.7.0-beta001" && UpdateService.PickUpdate(releases.Where(r => r.Tag != "v2.6.0-beta002"), V("2.5.7"), UpdateChannel.Beta, true) == null,
                  "a beta published before the newest normal release (older code, or a leftover) is not offered", lines);
            Check(UpdateService.PickUpdate(releases, V("2.5.7"), UpdateChannel.Stable, true) == null, "the version already running is not offered again", lines);
            Check(UpdateService.PickUpdate(releases, V("2.6.0"), UpdateChannel.Beta, false) == null, "an older build is never offered (no downgrade)", lines);
            Check(UpdateService.PickUpdate(releases, V("2.6.0-beta001"), UpdateChannel.Stable, true) == null, "a beta user on Stable waits for the normal release", lines);
            Check(releases.All(r => r.Tag != "v2.9.0" || UpdateService.PickUpdate(new[] { r }, V("2.5.6"), UpdateChannel.Stable, false) == null),
                  "a release without a checksum file is not offered", lines);
            Check(UpdateService.PickUpdate(releases.Where(r => r.Tag == "v2.4.0"), V("2.3.0"), UpdateChannel.Stable, true) == null
                  && UpdateService.PickUpdate(releases.Where(r => r.Tag == "v2.4.0"), V("2.3.0"), UpdateChannel.Stable, false) != null,
                  "an installed copy needs the installer; a portable copy needs the zip", lines);
        }

        private static void Checksums(List<string> lines)
        {
            string a = new string('a', 64), b = new string('B', 64);
            string text = a + "  SPI_Data_Comp_Tool_2.5.7.zip\n" + b + " *SPI_Data_Comp_Tool_2.5.7_x64_setup.exe\r\nnot a checksum line\n";
            Check(UpdateService.ExpectedSha256(text, "SPI_Data_Comp_Tool_2.5.7.zip") == a, "the zip's checksum is read from the release file", lines);
            Check(UpdateService.ExpectedSha256(text, "SPI_Data_Comp_Tool_2.5.7_x64_setup.exe") == b.ToLowerInvariant(), "the installer's checksum is read, in lower case", lines);
            Check(UpdateService.ExpectedSha256(text, "other.zip") == null && UpdateService.ExpectedSha256(null, "x") == null, "a file the release does not list has no checksum", lines);

            string file = Path.Combine(Path.GetTempPath(), "spi-update-check-" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                File.WriteAllText(file, "abc");
                Check(UpdateService.Sha256Of(file) == "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", "SHA-256 of a file is computed correctly", lines);
            }
            finally
            {
                File.Delete(file);
            }
        }

        private static void PortableFiles(List<string> lines)
        {
            string work = Path.Combine(Path.GetTempPath(), "spi-update-check-" + Guid.NewGuid().ToString("N"));
            string app = Path.Combine(work, "app");
            Directory.CreateDirectory(Path.Combine(app, "PNID_Reports"));
            try
            {
                File.WriteAllText(Path.Combine(app, "SPI_Data_Comp_Tool_2.5.6.exe"), "old exe");
                File.WriteAllText(Path.Combine(app, "PNID_Reports", "instrument.txt"), "edited by the user");
                string zip = Path.Combine(work, "update.zip");
                using (ZipArchive archive = ZipFile.Open(zip, ZipArchiveMode.Create))
                {
                    Add(archive, "portable/SPI_Data_Comp_Tool_2.5.7.exe", "new exe");
                    Add(archive, "portable/SPI_Data_Comp_Tool_2.5.7.exe.config", "config");
                    Add(archive, "portable/PNID_Reports/instrument.txt", "template from the release");
                    Add(archive, "portable/PNID_Reports/new.txt", "new template");
                    Add(archive, "windows-x64/SPI_Data_Comp_Tool_2.5.7.exe", "installer");
                }
                string exe = UpdateService.ExtractPortable(zip, app);
                Check(Path.GetFileName(exe) == "SPI_Data_Comp_Tool_2.5.7.exe" && File.ReadAllText(exe) == "new exe", "a portable update puts the new exe beside the old one", lines);
                Check(File.ReadAllText(Path.Combine(app, "SPI_Data_Comp_Tool_2.5.6.exe")) == "old exe", "the old exe stays as the way back", lines);
                Check(File.ReadAllText(Path.Combine(app, "PNID_Reports", "instrument.txt")) == "edited by the user", "a file the user already has is never overwritten", lines);
                Check(File.Exists(Path.Combine(app, "PNID_Reports", "new.txt")) && !Directory.Exists(Path.Combine(app, "windows-x64")), "only the portable files are copied", lines);

                string bad = Path.Combine(work, "bad.zip");
                using (ZipArchive archive = ZipFile.Open(bad, ZipArchiveMode.Create))
                {
                    Add(archive, "portable/../../escaped.exe", "x");
                }
                bool refused = false;
                try { UpdateService.ExtractPortable(bad, app); }
                catch (InvalidDataException) { refused = true; }
                Check(refused && !File.Exists(Path.Combine(work, "escaped.exe")), "a zip entry that points outside the app folder is refused", lines);
            }
            finally
            {
                Directory.Delete(work, true);
            }
        }

        private static void Add(ZipArchive archive, string name, string content)
        {
            using (var writer = new StreamWriter(archive.CreateEntry(name).Open()))
            {
                writer.Write(content);
            }
        }
    }
}
