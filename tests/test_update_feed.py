"""Portable updater source contracts; UpdateFeedChecks.cs executes the C# parser/streaming tests on Windows."""

import re
import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
PROJECT = ROOT / "SPI Data Comp Tool"


def source(name: str) -> str:
    return (PROJECT / name).read_text(encoding="utf-8-sig")


class UpdateFeedContracts(unittest.TestCase):
    def test_versions_are_canonical_three_part_not_assembly_versions(self) -> None:
        version = source("UpdateVersion.cs")
        pattern = re.search(r'@"(\\Av\?.*?)"', version).group(1).replace(r"\z", r"\Z")
        for accepted in ("0.0.0", "2.5.6", "v2.10.0", "2.5.6-beta001", "v2.5.6-beta999"):
            self.assertIsNotNone(re.fullmatch(pattern, accepted), accepted)
        for rejected in ("2.5.6.7", "v2.5.6.0", "2.5", "2.5.6-beta1", "2.5.6-beta1000", "2.5.6-rc001", "v02.5.6", "2.5.6+build"):
            self.assertIsNone(re.fullmatch(pattern, rejected), rejected)
        self.assertIn("beta < 1", version)
        self.assertIn("int.TryParse", version)
        self.assertIn("IComparable<UpdateVersion>", version)
        for component in ("Major", "Minor", "Patch"):
            self.assertIn(f"{component}.CompareTo(other.{component})", version)
        self.assertIn("if (!Beta.HasValue) return other.Beta.HasValue ? 1 : 0", version)
        self.assertIn('Beta.Value.ToString("000", CultureInfo.InvariantCulture)', version)

    def test_feed_lists_pages_not_just_latest(self) -> None:
        feed = source("UpdateFeedClient.cs")
        self.assertIn('Repository = "krishnabhunia/SPI-Data-Comp-Tool"', feed)
        self.assertIn("page <= MaximumPages", feed)
        self.assertIn('"/releases?per_page="', feed)
        self.assertIn('"&page="', feed)
        self.assertIn("items.Count < PageSize", feed)
        self.assertIn("tags.Add(release.Tag)", feed)
        self.assertIn("right.Version.CompareTo(left.Version)", feed)
        self.assertNotIn("/releases/latest", feed)
        self.assertIn("release list exceeds the updater's safety limit", feed)

    def test_release_schema_and_asset_names_match_publishing(self) -> None:
        feed = source("UpdateFeedClient.cs")
        release = feed.split("internal static UpdateRelease ParseRelease", 1)[1].split("internal static string ParseChecksum", 1)[0]
        self.assertIn('item["draft"].Type != JTokenType.Boolean', release)
        self.assertIn('item["prerelease"].Type != JTokenType.Boolean', release)
        self.assertIn('(bool)item["prerelease"] != version.IsBeta', release)
        self.assertIn('tag != "v" + version', release)
        self.assertIn('Text(item["published_at"])', release)
        self.assertIn("checksums == null || (installer == null && portable == null)", release)
        self.assertIn('"SPI_Data_Comp_Tool_" + version', feed)
        self.assertIn('"_x64_setup.exe" : ".zip"', feed)
        self.assertIn('GetAssetName(version, UpdateInstallKind.Portable) + ".sha256"', feed)
        workflow = (ROOT / ".github" / "workflows" / "build.yml").read_text()
        self.assertIn('SPI_Data_Comp_Tool_${VERSION}_x64_setup.exe', workflow)
        self.assertIn('SPI_Data_Comp_Tool_${VERSION}.zip', workflow)
        self.assertIn('sha256sum "${zip_name}" "${setup_name}"', workflow)

    def test_only_scoped_https_and_explicit_redirects(self) -> None:
        feed = source("UpdateFeedClient.cs")
        self.assertIn("request.AllowAutoRedirect = false", feed)
        self.assertIn("uri.Scheme == Uri.UriSchemeHttps && uri.Port == 443", feed)
        self.assertIn("string.IsNullOrEmpty(uri.UserInfo)", feed)
        self.assertIn("string.IsNullOrEmpty(uri.Fragment)", feed)
        redirect = feed.split("internal static bool IsAllowedRedirect", 1)[1].split("internal static bool IsReleaseApiUri", 1)[0]
        self.assertIn("if (!IsRepositoryAssetUri(original)) return false", redirect)
        self.assertIn('"release-assets.githubusercontent.com"', redirect)
        self.assertIn('"objects.githubusercontent.com"', redirect)
        self.assertIn("original.AbsolutePath, StringComparison.Ordinal", redirect)
        self.assertIn("releaseMetadata || redirects == 5", feed)
        self.assertNotIn("Authorization", feed)
        self.assertNotIn("ServerCertificateValidationCallback", feed)
        self.assertNotIn("ServicePointManager.SecurityProtocol", feed)
        self.assertNotIn("System.Net.Http", feed)

    def test_async_timeouts_abort_legacy_requests_and_reads(self) -> None:
        feed = source("UpdateFeedClient.cs")
        self.assertIn("request.Timeout = 30000", feed)
        self.assertIn("request.ReadWriteTimeout = 30000", feed)
        self.assertIn("timeout.Token.Register(request.Abort)", feed)
        self.assertIn("timeout.Token.Register(() => { if (abort != null) abort(); })", feed)
        self.assertIn("timeout.CancelAfter(TimeSpan.FromSeconds(30))", feed)
        self.assertIn("deadline.CancelAfter(TimeSpan.FromMinutes(20))", feed)
        self.assertIn("deadline.CancelAfter(TimeSpan.FromMinutes(3))", feed)
        self.assertIn("ConfigureAwait(false)", feed)

    def test_packages_are_streamed_bounded_and_hash_verified_before_handoff(self) -> None:
        feed = source("UpdateFeedClient.cs")
        download = feed.split("public async Task<DownloadedUpdate> DownloadAsync", 1)[1].split("internal static string GetAssetName", 1)[0]
        self.assertIn("SnapshotForDownload(release, kind)", download)
        self.assertLess(download.index("ParseChecksum("), download.index("transport.OpenAsync(asset.DownloadUri"))
        self.assertIn("received > asset.Size || received > MaximumPackageBytes", download)
        self.assertIn("received != asset.Size", download)
        self.assertIn("SHA256.Create()", download)
        self.assertIn("hash.TransformBlock", download)
        self.assertIn("hash.TransformFinalBlock", download)
        self.assertLess(download.index("string.Equals(actualHash, expectedHash"), download.index("File.Move(partialPath, finalPath)"))
        self.assertLess(download.index("File.Move(partialPath, finalPath)"), download.index("new DownloadedUpdate"))
        self.assertIn("FileMode.CreateNew", download)
        self.assertIn("FileShare.None", download)
        self.assertIn('Guid.NewGuid().ToString("N")', download)
        self.assertIn('asset.Name + ".partial"', download)
        self.assertIn("if (!complete && directory != null) ReleaseOwnedDirectory(directory)", download)
        self.assertNotIn("ReadAllBytes", feed)
        self.assertNotIn("Process.Start", feed)

    def test_manifest_match_is_exact_and_duplicates_rejected(self) -> None:
        feed = source("UpdateFeedClient.cs")
        manifest = feed.split("internal static string ParseChecksum", 1)[1].split("private static UpdateRelease SnapshotForDownload", 1)[0]
        self.assertIn("ChecksumLine.Match(line)", manifest)
        self.assertIn("!seen.Add(name)", manifest)
        self.assertIn("name.IndexOfAny", manifest)
        self.assertIn("string.Equals(name, filename, StringComparison.Ordinal)", manifest)
        self.assertIn("if (found == null)", manifest)
        self.assertIn("[a-fA-F0-9]{64}", feed)

    def test_disposal_cleans_only_instance_owned_cache_directories(self) -> None:
        feed = source("UpdateFeedClient.cs")
        self.assertIn("IUpdateFeed, IDisposable", feed)
        self.assertIn("ownedDirectories.Add(directory, asset.Name)", feed)
        self.assertIn("if (!ownedDirectories.TryGetValue(directory, out filename)) return", feed)
        self.assertIn("ownedDirectories.Keys.CopyTo(directories, 0)", feed)
        self.assertIn("disposal.Cancel()", feed)
        self.assertIn("CancellationTokenSource.CreateLinkedTokenSource(token, disposal.Token)", feed)
        self.assertNotIn("Directory.Delete(cacheRoot", feed)
        self.assertNotIn("Directory.GetDirectories(cacheRoot", feed)
        self.assertNotIn("Directory.Delete(directory, true)", feed)
        self.assertIn("Directory.Delete(directory, false)", feed)
        self.assertIn("UpdateApplyWorker.RejectReparsePoints(cacheRoot)", feed)
        self.assertIn("UpdateApplyWorker.RejectReparsePoints(directory)", feed)
        self.assertIn("UpdateApplyWorker.RejectReparsePoints(partialPath)", feed)
        self.assertIn("UpdateApplyWorker.RejectReparsePoints(finalPath)", feed)

    def test_windows_runtime_checks_exercise_real_parser_and_streaming(self) -> None:
        checks = (ROOT / "tests" / "UpdateFeedChecks.cs").read_text()
        for scenario in ("CheckVersions()", "CheckReleaseFilters()", "CheckUrisAndChecksums()", "CheckPagination(directory)",
                         "CheckVerifiedDownloads(directory)", "CheckDownloadFailures(directory)", "CheckDisposal(directory)", "CheckCacheGuards(directory)"):
            self.assertIn(scenario, checks)
        self.assertIn("IUpdateTransport", checks)
        self.assertIn("InterruptingStream", checks)
        self.assertIn("unknownLarge", checks)
        self.assertIn("snapshotTransport", checks)
        self.assertIn("RequireNoStaging", checks)
        self.assertIn("new InlineProgress(value => active.Dispose())", checks)
        self.assertIn("TryCreateDirectoryLink", checks)
        self.assertIn("unexpected-child", checks)
        self.assertNotIn("new HttpUpdateTransport", checks)


if __name__ == "__main__":
    unittest.main()
