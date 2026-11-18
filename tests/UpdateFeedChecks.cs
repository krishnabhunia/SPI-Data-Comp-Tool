using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Synthetic updater feed/download checks. No network, installation or user settings are used.</summary>
    internal static class UpdateFeedChecks
    {
        public static void Run(Action<string> passed)
        {
            CheckVersions();
            passed("Updater versions use exactly three numeric components and beta001-beta999; stable/beta ordering is numeric.");
            CheckReleaseFilters();
            passed("Updater feed excludes drafts, malformed tags, mismatched channels and missing/unsafe release assets.");
            CheckUrisAndChecksums();
            passed("Updater allows only repository-scoped HTTPS assets and explicit GitHub redirect hosts; checksum matching is strict.");
            string directory = Path.Combine(Path.GetTempPath(), "spi-update-feed-checks-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                CheckPagination(directory).GetAwaiter().GetResult();
                passed("Updater paginates all published releases, sorts numerically, deduplicates tags and bounds metadata/pages.");
                CheckVerifiedDownloads(directory).GetAwaiter().GetResult();
                passed("Updater streams and verifies SHA-256 before publishing a selected package; release snapshots and progress remain consistent.");
                CheckDownloadFailures(directory).GetAwaiter().GetResult();
                passed("Updater rejects tampered/truncated/oversized downloads and cleans staged files on I/O failure or cancellation.");
                CheckDisposal(directory).GetAwaiter().GetResult();
                passed("Updater disposal cancels active downloads and removes only its own staging directories, preserving unrelated cache contents.");
                bool checkedLinks = CheckCacheGuards(directory).GetAwaiter().GetResult();
                passed("Updater cache cleanup preserves unexpected child content; " + (checkedLinks
                    ? "symbolic-link creation and cleanup refusal verified."
                    : "symbolic-link runtime case unavailable under this account (static guard contracts checked separately)."));
            }
            finally { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
        }

        private static void CheckVersions()
        {
            UpdateVersion version;
            Require(UpdateVersion.TryParse("v2.10.3-beta009", out version) && version.Major == 2 && version.Minor == 10 &&
                version.Patch == 3 && version.Beta == 9 && version.IsBeta && version.ToString() == "2.10.3-beta009", "The canonical beta version was not parsed.");
            Require(Parse("2.10.3").CompareTo(Parse("2.10.3-beta999")) > 0, "A stable release must sort after its betas.");
            Require(Parse("2.10.3-beta010").CompareTo(Parse("2.10.3-beta009")) > 0, "Beta versions were ordered as text.");
            Require(Parse("2.10.0").CompareTo(Parse("2.9.9")) > 0 && Parse("10.0.0").CompareTo(Parse("9.99.99")) > 0, "Base versions were ordered as text.");
            Require(Parse("3.0.0-beta001").CompareTo(Parse("2.99.99")) > 0, "The base version must be compared before the channel.");
            Require(Parse("0.0.0").CompareTo(Parse("v0.0.0")) == 0 && Parse("0.0.0").CompareTo(null) > 0, "Version equality/null ordering is invalid.");
            foreach (string invalid in new[] { null, "", "2.5", "2.5.4.1", "v2.5.4.0", "2.5.4-beta1", "2.5.4-beta000",
                "2.5.4-beta1000", "2.5.4-BETA001", "2.5.4-beta001+build", "2.5.4.1-beta001", "v02.5.4", "v2.05.4", "v2.5.04",
                "v2147483648.0.0", "-2.5.4", "2.5.4\n", " 2.5.4", "2.5.4 ", "V2.5.4", "v2.5.4/anything" })
                Require(!UpdateVersion.TryParse(invalid, out version) && version == null, "A noncanonical version was accepted.");
        }

        private static void CheckReleaseFilters()
        {
            JObject valid = ReleaseJson("2.5.6-beta002", 24, 128);
            UpdateRelease release = UpdateFeedClient.ParseRelease(valid);
            Require(release != null && release.Version.IsBeta && release.IsCompatible(UpdateInstallKind.Portable, false) &&
                release.IsCompatible(UpdateInstallKind.Installed, true) && !release.IsCompatible(UpdateInstallKind.Installed, false), "Valid compatible release assets were rejected.");
            RejectMutation(valid, item => item["draft"] = true);
            RejectMutation(valid, item => item["draft"] = "false");
            RejectMutation(valid, item => item.Remove("draft"));
            RejectMutation(valid, item => item["prerelease"] = false);
            RejectMutation(valid, item => item["prerelease"] = "true");
            RejectMutation(valid, item => item["tag_name"] = "v2.5.6.9");
            RejectMutation(valid, item => item["tag_name"] = "2.5.6-beta002");
            RejectMutation(valid, item => item["published_at"] = JValue.CreateNull());
            RejectMutation(valid, item => item["html_url"] = "https://github.com/another/repository/releases/tag/v2.5.6-beta002");
            RejectMutation(valid, item => ((JArray)item["assets"]).RemoveAt(2));
            RejectMutation(valid, item => item["assets"][2]["size"] = UpdateFeedClient.MaximumChecksumBytes + 1);
            RejectMutation(valid, item => item["assets"][2]["state"] = "new");
            RejectMutation(valid, item => item["assets"][2]["size"] = "128");
            RejectMutation(valid, item => item["assets"][2]["size"] = 0);
            RejectMutation(valid, item => item["assets"][2]["browser_download_url"] = "http://github.com/unsafe");
            RejectMutation(valid, item => ((JArray)item["assets"]).Add(item["assets"][2].DeepClone()));
            RejectMutation(valid, item =>
            {
                item["assets"][0]["name"] = "unrelated.exe";
                item["assets"][1]["name"] = "unrelated.zip";
            });
            JObject portableOnly = (JObject)valid.DeepClone();
            ((JArray)portableOnly["assets"]).RemoveAt(0);
            release = UpdateFeedClient.ParseRelease(portableOnly);
            Require(release != null && release.Installer == null && release.IsCompatible(UpdateInstallKind.Portable, true), "Portable-only releases must remain available.");
            JObject installerOnly = (JObject)valid.DeepClone();
            ((JArray)installerOnly["assets"]).RemoveAt(1);
            release = UpdateFeedClient.ParseRelease(installerOnly);
            Require(release != null && release.Portable == null && release.IsCompatible(UpdateInstallKind.Installed, true), "Installer-only releases must remain available.");
            JObject oversize = (JObject)valid.DeepClone();
            oversize["assets"][0]["size"] = UpdateFeedClient.MaximumPackageBytes + 1;
            release = UpdateFeedClient.ParseRelease(oversize);
            Require(release != null && release.Installer == null && release.Portable != null, "An oversized asset must not be offered.");
            JObject stable = ReleaseJson("2.5.6", 24, 128);
            stable["prerelease"] = true;
            Require(UpdateFeedClient.ParseRelease(stable) == null && UpdateFeedClient.ParseRelease(null) == null, "Channel mismatch/null release accepted.");
        }

        private static void CheckUrisAndChecksums()
        {
            string filename = "SPI_Data_Comp_Tool_2.5.6.zip";
            Uri original = AssetUri("v2.5.6", filename);
            Require(UpdateFeedClient.IsAssetUri(original, "v2.5.6", filename), "The official repository asset URL was rejected.");
            foreach (string invalid in new[]
            {
                original.AbsoluteUri.Replace("https:", "http:"),
                original.AbsoluteUri.Replace("github.com", "github.com.evil.example"),
                original.AbsoluteUri.Replace("github.com", "github.com:444"),
                original.AbsoluteUri.Replace("github.com", "user:password@github.com"),
                original.AbsoluteUri.Replace("krishnabhunia/", "somebody/"),
                original.AbsoluteUri.Replace("v2.5.6/", "v2.5.7/"),
                original.AbsoluteUri.Replace(filename, "../unrelated.zip"),
                original.AbsoluteUri + "?download=1", original.AbsoluteUri + "#fragment", "file:///C:/unexpected.exe"
            })
                Require(!UpdateFeedClient.IsAssetUri(new Uri(invalid), "v2.5.6", filename), "An unsafe repository asset URL was accepted.");
            foreach (string allowed in new[] { "https://release-assets.githubusercontent.com/asset?id=synthetic", "https://objects.githubusercontent.com/asset?signature=synthetic" })
                Require(UpdateFeedClient.IsAllowedRedirect(new Uri(allowed), original), "A supported GitHub asset redirect was rejected.");
            foreach (string invalid in new[] { "http://release-assets.githubusercontent.com/asset", "https://release-assets.githubusercontent.com.evil.example/asset",
                "https://objects.githubusercontent.com:444/asset", "https://user:password@objects.githubusercontent.com/asset", "https://example.com/asset",
                "https://raw.githubusercontent.com/asset", "file:///C:/unexpected.exe", "https://github.com/another/repo/releases/download/v2.5.6/unrelated.exe" })
                Require(!UpdateFeedClient.IsAllowedRedirect(new Uri(invalid), original), "An unsafe download redirect was accepted.");
            Require(!UpdateFeedClient.IsAllowedRedirect(new Uri("https://objects.githubusercontent.com/asset"), null), "An unscoped redirect was accepted.");
            Require(UpdateFeedClient.IsReleaseApiUri(new Uri("https://api.github.com/repos/" + UpdateFeedClient.Repository + "/releases?per_page=100&page=2")), "Release pagination URL rejected.");
            Require(!UpdateFeedClient.IsReleaseApiUri(new Uri("https://api.github.com/repos/" + UpdateFeedClient.Repository + "/releases/latest")), "The updater must not use the latest-only API.");

            string hash = new string('a', 64);
            Require(UpdateFeedClient.ParseChecksum(hash.ToUpperInvariant() + "  " + filename + "\r\n", filename) == hash, "sha256sum text records were rejected.");
            Require(UpdateFeedClient.ParseChecksum(hash + " *" + filename + "\n", filename) == hash, "sha256sum binary records were rejected.");
            foreach (string invalid in new[]
            {
                "", new string('g', 64) + "  " + filename, hash.Substring(1) + "  " + filename,
                hash + "  " + filename.ToUpperInvariant(), hash + "  " + filename + ".extra",
                hash + "  ../" + filename, hash + "  .\\" + filename, hash + " " + filename,
                hash + "  " + filename + "\n" + hash + "  " + filename, hash + "  " + filename + "\ninvalid record",
                hash + "  " + filename + " ", hash + "  " + filename + "\0", new string('x', UpdateFeedClient.MaximumChecksumBytes + 1)
            }) ExpectInvalid(() => UpdateFeedClient.ParseChecksum(invalid, filename));
        }

        private static async Task CheckPagination(string directory)
        {
            var first = new JArray();
            for (int index = 0; index < 100; index++) first.Add(ReleaseJson("2.5.6", 24, 128));
            var second = new JArray(ReleaseJson("2.10.0", 24, 128), ReleaseJson("2.10.0-beta009", 24, 128));
            var transport = new FakeTransport((uri, metadata, token) => TextResponse(uri.Query.EndsWith("page=1", StringComparison.Ordinal) ? first.ToString(Formatting.None) : second.ToString(Formatting.None)));
            IList<UpdateRelease> releases = await new UpdateFeedClient(transport, directory).CheckAsync(CancellationToken.None).ConfigureAwait(false);
            Require(transport.Requests.Count == 2 && releases.Count == 3 && releases[0].Tag == "v2.10.0" &&
                releases[1].Tag == "v2.10.0-beta009" && releases[2].Tag == "v2.5.6", "Published release pagination/order/deduplication failed.");
            Require(transport.Requests[0].Query == "?per_page=100&page=1" && transport.Requests[1].Query == "?per_page=100&page=2", "The second release page was not requested.");

            var tooMany = new FakeTransport((uri, metadata, token) => TextResponse(first.ToString(Formatting.None)));
            await ExpectInvalidAsync(() => new UpdateFeedClient(tooMany, directory).CheckAsync(CancellationToken.None)).ConfigureAwait(false);
            Require(tooMany.Requests.Count == UpdateFeedClient.MaximumPages, "The pagination limit was not enforced.");
            foreach (string invalidJson in new[] { "{}", "[", "null", "[] []" })
            {
                var invalid = new FakeTransport((uri, metadata, token) => TextResponse(invalidJson));
                await ExpectInvalidAsync(() => new UpdateFeedClient(invalid, directory).CheckAsync(CancellationToken.None)).ConfigureAwait(false);
            }
            var tooLarge = new FakeTransport((uri, metadata, token) => new UpdateResponse(new MemoryStream(new byte[0]), UpdateFeedClient.MaximumMetadataBytes + 1));
            await ExpectInvalidAsync(() => new UpdateFeedClient(tooLarge, directory).CheckAsync(CancellationToken.None)).ConfigureAwait(false);
            var unknownLarge = new FakeTransport((uri, metadata, token) => new UpdateResponse(new MemoryStream(new byte[UpdateFeedClient.MaximumMetadataBytes + 1]), null));
            await ExpectInvalidAsync(() => new UpdateFeedClient(unknownLarge, directory).CheckAsync(CancellationToken.None)).ConfigureAwait(false);
            var badEncoding = new FakeTransport((uri, metadata, token) => new UpdateResponse(new MemoryStream(new byte[] { 0xff, 0xfe }), 2));
            await ExpectInvalidAsync(() => new UpdateFeedClient(badEncoding, directory).CheckAsync(CancellationToken.None)).ConfigureAwait(false);
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                var unused = new FakeTransport((uri, metadata, token) => TextResponse("[]"));
                await ExpectCancelledAsync(() => new UpdateFeedClient(unused, directory).CheckAsync(cancelled.Token)).ConfigureAwait(false);
                Require(unused.Requests.Count == 0, "A cancelled check opened a network request.");
            }
        }

        private static async Task CheckVerifiedDownloads(string directory)
        {
            foreach (UpdateInstallKind kind in new[] { UpdateInstallKind.Portable, UpdateInstallKind.Installed })
            {
                if (kind == UpdateInstallKind.Installed && !Environment.Is64BitOperatingSystem) continue;
                byte[] bytes = SampleBytes();
                UpdateRelease release = DownloadRelease(bytes);
                string name = UpdateFeedClient.GetAssetName(release.Version, kind);
                string manifest = Manifest(release, bytes);
                string cache = Path.Combine(directory, "verified-" + kind);
                var progress = new InlineProgress();
                var transport = new FakeTransport((uri, metadata, token) => uri.AbsolutePath.EndsWith(".sha256", StringComparison.Ordinal)
                    ? TextResponse(manifest) : new UpdateResponse(new MemoryStream(bytes), null));
                var client = new UpdateFeedClient(transport, cache);
                DownloadedUpdate downloaded = await client.DownloadAsync(release, kind, progress, CancellationToken.None).ConfigureAwait(false);
                Require(downloaded.FilePath.StartsWith(cache + Path.DirectorySeparatorChar, StringComparison.Ordinal) &&
                    Path.GetFileName(downloaded.FilePath) == name && File.Exists(downloaded.FilePath) && File.ReadAllBytes(downloaded.FilePath).Length == bytes.Length,
                    "The verified update was not placed in its isolated cache directory.");
                Guid id;
                Require(Guid.TryParseExact(new DirectoryInfo(Path.GetDirectoryName(downloaded.FilePath)).Name, "N", out id), "The update cache directory is not uniquely scoped.");
                Require(downloaded.Sha256 == Hash(bytes) && downloaded.Kind == kind && !ReferenceEquals(downloaded.Release, release), "The download result lacks a verified snapshot.");
                Require(!File.Exists(downloaded.FilePath + ".partial") && progress.Last != null && progress.Last.BytesReceived == bytes.Length &&
                    progress.Last.TotalBytes == bytes.Length && progress.Last.Percentage == 100, "The completed download retained a partial file or invalid progress.");
                Require(transport.Requests.Count == 2 && transport.Requests[0] == release.Checksums.DownloadUri &&
                    transport.Requests[1].AbsolutePath.EndsWith("/" + name, StringComparison.Ordinal), "The selected package/checksum request order is incorrect.");
            }

            byte[] data = SampleBytes();
            UpdateRelease mutable = DownloadRelease(data);
            string expectedName = mutable.Portable.Name;
            string expectedManifest = Manifest(mutable, data);
            var snapshotTransport = new FakeTransport((uri, metadata, token) =>
            {
                if (uri.AbsolutePath.EndsWith(".sha256", StringComparison.Ordinal))
                {
                    mutable.Portable.Name = "changed-after-start.zip";
                    mutable.Portable.DownloadUri = new Uri("https://example.com/changed.zip");
                    return TextResponse(expectedManifest);
                }
                Require(uri.AbsolutePath.EndsWith("/" + expectedName, StringComparison.Ordinal), "A mutable display object changed the package being downloaded.");
                return new UpdateResponse(new MemoryStream(data), data.Length);
            });
            DownloadedUpdate snapshot = await new UpdateFeedClient(snapshotTransport, Path.Combine(directory, "snapshot"))
                .DownloadAsync(mutable, UpdateInstallKind.Portable, null, CancellationToken.None).ConfigureAwait(false);
            Require(snapshot.Release.Portable.Name == expectedName && snapshot.Release.Portable.DownloadUri.Host == "github.com", "The download snapshot retained a mutated asset.");
        }

        private static async Task CheckDownloadFailures(string directory)
        {
            byte[] bytes = SampleBytes();
            UpdateRelease release = DownloadRelease(bytes);
            string manifest = Manifest(release, bytes);
            string cache = Path.Combine(directory, "rejected");
            foreach (string failure in new[] { "checksum", "truncated", "oversized", "length", "io" })
            {
                var transport = new FakeTransport((uri, metadata, token) =>
                {
                    if (uri.AbsolutePath.EndsWith(".sha256", StringComparison.Ordinal)) return TextResponse(manifest);
                    if (failure == "length") return new UpdateResponse(new MemoryStream(bytes), bytes.Length + 1);
                    if (failure == "io") return new UpdateResponse(new InterruptingStream(bytes, null), null);
                    byte[] altered = (byte[])bytes.Clone();
                    if (failure == "checksum") altered[0] ^= 1;
                    if (failure == "truncated") Array.Resize(ref altered, bytes.Length - 1);
                    if (failure == "oversized") Array.Resize(ref altered, bytes.Length + 1);
                    return new UpdateResponse(new MemoryStream(altered), null);
                });
                bool rejected = false;
                try { await new UpdateFeedClient(transport, cache).DownloadAsync(release, UpdateInstallKind.Portable, null, CancellationToken.None).ConfigureAwait(false); }
                catch (Exception error) when (error is IOException || error is InvalidDataException) { rejected = true; }
                Require(rejected, "An invalid package was accepted: " + failure);
                RequireNoStaging(cache);
            }
            using (var cancelled = new CancellationTokenSource())
            {
                var transport = new FakeTransport((uri, metadata, token) => uri.AbsolutePath.EndsWith(".sha256", StringComparison.Ordinal)
                    ? TextResponse(manifest) : new UpdateResponse(new InterruptingStream(bytes, cancelled), null));
                await ExpectCancelledAsync(() => new UpdateFeedClient(transport, cache).DownloadAsync(release, UpdateInstallKind.Portable, null, cancelled.Token)).ConfigureAwait(false);
                RequireNoStaging(cache);
            }
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                var unused = new FakeTransport((uri, metadata, token) => TextResponse(manifest));
                await ExpectCancelledAsync(() => new UpdateFeedClient(unused, cache).DownloadAsync(release, UpdateInstallKind.Portable, null, cancelled.Token)).ConfigureAwait(false);
                Require(unused.Requests.Count == 0, "A cancelled download opened a request.");
            }

            foreach (string failure in new[] { "missing", "duplicate", "malformed", "unsafe" })
            {
                UpdateRelease candidate = DownloadRelease(bytes);
                string bad = failure == "missing" ? Hash(bytes) + "  unrelated.zip\n" :
                    failure == "duplicate" ? manifest + manifest : failure == "malformed" ? "not a checksum" : Hash(bytes) + "  ../" + candidate.Portable.Name;
                candidate.Checksums.Size = Encoding.UTF8.GetByteCount(bad);
                var transport = new FakeTransport((uri, metadata, token) => TextResponse(bad));
                await ExpectInvalidAsync(() => new UpdateFeedClient(transport, cache).DownloadAsync(candidate, UpdateInstallKind.Portable, null, CancellationToken.None)).ConfigureAwait(false);
                Require(transport.Requests.Count == 1, "A package was downloaded before its checksum manifest was validated.");
                RequireNoStaging(cache);
            }
            UpdateRelease unsafeRelease = DownloadRelease(bytes);
            unsafeRelease.Portable.DownloadUri = new Uri("https://example.com/untrusted.zip");
            var never = new FakeTransport((uri, metadata, token) => TextResponse(manifest));
            await ExpectInvalidAsync(() => new UpdateFeedClient(never, cache).DownloadAsync(unsafeRelease, UpdateInstallKind.Portable, null, CancellationToken.None)).ConfigureAwait(false);
            Require(never.Requests.Count == 0, "An unsafe model was not revalidated before downloading.");
        }

        private static async Task CheckDisposal(string directory)
        {
            byte[] bytes = SampleBytes();
            UpdateRelease release = DownloadRelease(bytes);
            string manifest = Manifest(release, bytes);
            string cache = Path.Combine(directory, "disposal");
            Directory.CreateDirectory(cache);
            string unrelated = Path.Combine(cache, "unrelated.txt");
            File.WriteAllText(unrelated, "Synthetic content owned by a different operation.");
            var transport = new FakeTransport((uri, metadata, token) => uri.AbsolutePath.EndsWith(".sha256", StringComparison.Ordinal)
                ? TextResponse(manifest) : new UpdateResponse(new MemoryStream(bytes), bytes.Length));
            var client = new UpdateFeedClient(transport, cache);
            DownloadedUpdate downloaded = await client.DownloadAsync(release, UpdateInstallKind.Portable, null, CancellationToken.None).ConfigureAwait(false);
            client.Dispose();
            client.Dispose();
            Require(!File.Exists(downloaded.FilePath) && File.Exists(unrelated) && Directory.Exists(cache), "Updater disposal deleted unrelated data or retained its package.");
            bool rejected = false;
            try { await client.CheckAsync(CancellationToken.None).ConfigureAwait(false); } catch (ObjectDisposedException) { rejected = true; }
            Require(rejected, "A disposed feed accepted another request.");

            var active = new UpdateFeedClient(transport, Path.Combine(directory, "dispose-active"));
            var progress = new InlineProgress(value => active.Dispose());
            await ExpectCancelledAsync(() => active.DownloadAsync(release, UpdateInstallKind.Portable, progress, CancellationToken.None)).ConfigureAwait(false);
            RequireNoStaging(Path.Combine(directory, "dispose-active"));
        }

        private static async Task<bool> CheckCacheGuards(string directory)
        {
            byte[] bytes = SampleBytes();
            UpdateRelease release = DownloadRelease(bytes);
            string manifest = Manifest(release, bytes);
            var transport = new FakeTransport((uri, metadata, token) => uri.AbsolutePath.EndsWith(".sha256", StringComparison.Ordinal)
                ? TextResponse(manifest) : new UpdateResponse(new MemoryStream(bytes), bytes.Length));
            var client = new UpdateFeedClient(transport, Path.Combine(directory, "unexpected-children"));
            DownloadedUpdate package = await client.DownloadAsync(release, UpdateInstallKind.Portable, null, CancellationToken.None).ConfigureAwait(false);
            string owned = Path.GetDirectoryName(package.FilePath);
            string nested = Path.Combine(owned, "unexpected-child");
            Directory.CreateDirectory(nested);
            string sentinel = Path.Combine(nested, "preserved.txt");
            string direct = Path.Combine(owned, "unexpected.txt");
            File.WriteAllText(sentinel, "Synthetic unexpected child content.");
            File.WriteAllText(direct, "Synthetic unexpected direct content.");
            client.Dispose();
            Require(File.Exists(sentinel) && File.Exists(direct) && Directory.Exists(owned), "Cleanup recursively deleted unexpected cache content.");
            Require(!File.Exists(package.FilePath), "Known cache files were not cleaned up.");

            if (Environment.OSVersion.Platform != PlatformID.Win32NT) return false;
            string target = Path.Combine(directory, "link-target");
            Directory.CreateDirectory(target);
            string targetSentinel = Path.Combine(target, "target-preserved.txt");
            File.WriteAllText(targetSentinel, "Synthetic link target content.");
            string link = Path.Combine(directory, "linked-cache");
            if (!TryCreateDirectoryLink(link, target)) return false;
            string substitutedLink = null;
            try
            {
                using (var linked = new UpdateFeedClient(transport, link))
                    await ExpectInvalidAsync(() => linked.DownloadAsync(release, UpdateInstallKind.Portable, null, CancellationToken.None)).ConfigureAwait(false);
                Require(Directory.GetFileSystemEntries(target).Length == 1 && File.Exists(targetSentinel), "The download followed a cache-parent link.");

                var substituted = new UpdateFeedClient(transport, Path.Combine(directory, "substituted-cache"));
                try
                {
                    DownloadedUpdate downloaded = await substituted.DownloadAsync(release, UpdateInstallKind.Portable, null, CancellationToken.None).ConfigureAwait(false);
                    substitutedLink = Path.GetDirectoryName(downloaded.FilePath);
                    Directory.Move(substitutedLink, Path.Combine(directory, "moved-synthetic-cache"));
                    Require(TryCreateDirectoryLink(substitutedLink, target), "A synthetic cache substitution link could not be created.");
                    substituted.Dispose();
                    Require(File.Exists(targetSentinel) && Directory.Exists(substitutedLink), "Cleanup followed or removed a substituted cache link.");
                }
                finally { substituted.Dispose(); }
                return true;
            }
            finally
            {
                RemoveTestLink(link);
                if (substitutedLink != null) RemoveTestLink(substitutedLink);
            }
        }

        private static bool TryCreateDirectoryLink(string link, string target)
        {
            // Developer-mode support avoids requiring elevation where available; legacy systems may refuse both.
            return CreateSymbolicLink(link, target, 1 | 2) || CreateSymbolicLink(link, target, 1);
        }

        private static void RemoveTestLink(string path)
        {
            if (Directory.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                Directory.Delete(path, false);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool CreateSymbolicLink(string symbolicLink, string target, int flags);

        private static JObject ReleaseJson(string versionText, int size, int checksumSize)
        {
            UpdateVersion version = Parse(versionText);
            string tag = "v" + version;
            var assets = new JArray();
            foreach (string name in new[] { UpdateFeedClient.GetAssetName(version, UpdateInstallKind.Installed),
                UpdateFeedClient.GetAssetName(version, UpdateInstallKind.Portable), UpdateFeedClient.GetChecksumsName(version) })
                assets.Add(new JObject { ["name"] = name, ["state"] = "uploaded", ["size"] = name.EndsWith(".sha256", StringComparison.Ordinal) ? checksumSize : size,
                    ["browser_download_url"] = AssetUri(tag, name).AbsoluteUri });
            return new JObject { ["draft"] = false, ["prerelease"] = version.IsBeta, ["tag_name"] = tag, ["name"] = tag, ["body"] = "Synthetic release notes.",
                ["published_at"] = "2026-11-18T12:00:00Z", ["html_url"] = "https://github.com/" + UpdateFeedClient.Repository + "/releases/tag/" + tag, ["assets"] = assets };
        }

        private static UpdateRelease DownloadRelease(byte[] bytes)
        {
            UpdateRelease release = UpdateFeedClient.ParseRelease(ReleaseJson("2.5.6", bytes.Length, 128));
            release.Checksums.Size = Encoding.UTF8.GetByteCount(Manifest(release, bytes));
            return release;
        }

        private static string Manifest(UpdateRelease release, byte[] bytes)
        {
            string hash = Hash(bytes);
            return hash + "  " + release.Portable.Name + "\n" + hash + "  " + release.Installer.Name + "\n";
        }

        private static UpdateVersion Parse(string value)
        {
            UpdateVersion version;
            if (!UpdateVersion.TryParse(value, out version)) throw new InvalidOperationException("Invalid synthetic test version.");
            return version;
        }

        private static Uri AssetUri(string tag, string name) { return new Uri("https://github.com/" + UpdateFeedClient.Repository + "/releases/download/" + tag + "/" + name); }
        private static byte[] SampleBytes()
        {
            var bytes = new byte[100000];
            for (int index = 0; index < bytes.Length; index++) bytes[index] = (byte)(index % 251);
            return bytes;
        }
        private static string Hash(byte[] bytes)
        {
            using (var hash = SHA256.Create()) return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-", string.Empty).ToLowerInvariant();
        }
        private static UpdateResponse TextResponse(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return new UpdateResponse(new MemoryStream(bytes), bytes.Length);
        }
        private static void RejectMutation(JObject original, Action<JObject> mutate)
        {
            var candidate = (JObject)original.DeepClone();
            mutate(candidate);
            Require(UpdateFeedClient.ParseRelease(candidate) == null, "An invalid release record was accepted.");
        }
        private static void ExpectInvalid(Action action)
        {
            bool rejected = false;
            try { action(); } catch (InvalidDataException) { rejected = true; }
            Require(rejected, "Invalid update metadata was accepted.");
        }
        private static async Task ExpectInvalidAsync(Func<Task> action)
        {
            bool rejected = false;
            try { await action().ConfigureAwait(false); } catch (InvalidDataException) { rejected = true; }
            Require(rejected, "Invalid update metadata was accepted.");
        }
        private static async Task ExpectCancelledAsync(Func<Task> action)
        {
            bool cancelled = false;
            try { await action().ConfigureAwait(false); } catch (OperationCanceledException) { cancelled = true; }
            Require(cancelled, "Update cancellation was not honored.");
        }
        private static void RequireNoStaging(string cache)
        {
            Require(!Directory.Exists(cache) || Directory.GetFileSystemEntries(cache).Length == 0, "Failed download files were not removed.");
        }
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }

        private sealed class FakeTransport : IUpdateTransport
        {
            private readonly Func<Uri, bool, CancellationToken, UpdateResponse> open;
            internal readonly List<Uri> Requests = new List<Uri>();
            internal FakeTransport(Func<Uri, bool, CancellationToken, UpdateResponse> open) { this.open = open; }
            public Task<UpdateResponse> OpenAsync(Uri uri, bool releaseMetadata, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Requests.Add(uri);
                return Task.FromResult(open(uri, releaseMetadata, cancellationToken));
            }
        }

        private sealed class InlineProgress : IProgress<UpdateDownloadProgress>
        {
            private readonly Action<UpdateDownloadProgress> report;
            internal UpdateDownloadProgress Last;
            internal InlineProgress(Action<UpdateDownloadProgress> report = null) { this.report = report; }
            public void Report(UpdateDownloadProgress value) { Last = value; if (report != null) report(value); }
        }

        private sealed class InterruptingStream : MemoryStream
        {
            private readonly CancellationTokenSource cancellation;
            private bool returnedChunk;
            internal InterruptingStream(byte[] bytes, CancellationTokenSource cancellation) : base(bytes) { this.cancellation = cancellation; }
            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                if (returnedChunk)
                {
                    if (cancellation != null) { cancellation.Cancel(); cancellationToken.ThrowIfCancellationRequested(); }
                    throw new IOException("Synthetic interrupted connection.");
                }
                returnedChunk = true;
                return base.ReadAsync(buffer, offset, Math.Min(count, 16), cancellationToken);
            }
        }
    }
}
