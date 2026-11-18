using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Public release discovery and verified downloads; never installs or executes a package.</summary>
    internal sealed class UpdateFeedClient : IUpdateFeed, IDisposable
    {
        internal const string Repository = "krishnabhunia/SPI-Data-Comp-Tool";
        internal const int PageSize = 100;
        internal const int MaximumPages = 10;
        internal const int MaximumMetadataBytes = 4 * 1024 * 1024;
        internal const int MaximumChecksumBytes = 64 * 1024;
        internal const long MaximumPackageBytes = 512L * 1024 * 1024;
        private static readonly Encoding Utf8 = new UTF8Encoding(false, true);
        private static readonly Regex ChecksumLine = new Regex(@"\A([a-fA-F0-9]{64}) [ *]([^\r\n]+)\z", RegexOptions.CultureInvariant);
        private readonly IUpdateTransport transport;
        private readonly string cacheRoot;
        private readonly object cacheGate = new object();
        private readonly Dictionary<string, string> ownedDirectories = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly CancellationTokenSource disposal = new CancellationTokenSource();
        private bool disposed;

        public UpdateFeedClient() : this(new HttpUpdateTransport(), Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SPI Data Comp Tool", "Updates")) { }

        // A synthetic transport and isolated cache are used by self-tests; production uses only the public repository.
        internal UpdateFeedClient(IUpdateTransport transport, string cacheRoot)
        {
            if (transport == null) throw new ArgumentNullException(nameof(transport));
            if (string.IsNullOrWhiteSpace(cacheRoot) || !Path.IsPathRooted(cacheRoot)) throw new ArgumentException("An absolute update cache directory is required.", nameof(cacheRoot));
            this.transport = transport;
            this.cacheRoot = Path.GetFullPath(cacheRoot);
        }

        public async Task<IList<UpdateRelease>> CheckAsync(CancellationToken cancellationToken)
        {
            using (var deadline = CreateDeadline(cancellationToken))
            {
                deadline.CancelAfter(TimeSpan.FromMinutes(3));
                try
                {
                    var releases = new List<UpdateRelease>();
                    var tags = new HashSet<string>(StringComparer.Ordinal);
                    for (int page = 1; page <= MaximumPages; page++)
                    {
                        deadline.Token.ThrowIfCancellationRequested();
                        var uri = new Uri("https://api.github.com/repos/" + Repository + "/releases?per_page=" +
                            PageSize.ToString(CultureInfo.InvariantCulture) + "&page=" + page.ToString(CultureInfo.InvariantCulture));
                        byte[] data;
                        using (UpdateResponse response = await transport.OpenAsync(uri, true, deadline.Token).ConfigureAwait(false))
                            data = await ReadBoundedAsync(response, MaximumMetadataBytes, null, deadline.Token).ConfigureAwait(false);
                        JArray items = ParseArray(DecodeUtf8(data));
                        if (items.Count > PageSize) throw new InvalidDataException("The release server returned an oversized page.");
                        foreach (JToken item in items)
                        {
                            UpdateRelease release = ParseRelease(item as JObject);
                            if (release != null && tags.Add(release.Tag)) releases.Add(release);
                        }
                        if (items.Count < PageSize)
                        {
                            deadline.Token.ThrowIfCancellationRequested();
                            releases.Sort((left, right) => right.Version.CompareTo(left.Version));
                            return releases;
                        }
                    }
                    throw new InvalidDataException("The release list exceeds the updater's safety limit. Open the repository's Releases page instead.");
                }
                catch (OperationCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested || IsDisposed) throw;
                    throw new TimeoutException("Checking for updates took too long. Please try again.");
                }
            }
        }

        public async Task<DownloadedUpdate> DownloadAsync(UpdateRelease release, UpdateInstallKind kind,
            IProgress<UpdateDownloadProgress> progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Snapshot mutable display models before the first await, then revalidate everything used for a download.
            UpdateRelease snapshot = SnapshotForDownload(release, kind);
            UpdateAsset asset = kind == UpdateInstallKind.Installed ? snapshot.Installer : snapshot.Portable;
            string directory = null;
            bool complete = false;
            using (var deadline = CreateDeadline(cancellationToken))
            {
                deadline.CancelAfter(TimeSpan.FromMinutes(20));
                try
                {
                    byte[] manifest;
                    using (UpdateResponse response = await transport.OpenAsync(snapshot.Checksums.DownloadUri, false, deadline.Token).ConfigureAwait(false))
                        manifest = await ReadBoundedAsync(response, MaximumChecksumBytes, snapshot.Checksums.Size, deadline.Token).ConfigureAwait(false);
                    string expectedHash = ParseChecksum(DecodeUtf8(manifest), asset.Name);
                    deadline.Token.ThrowIfCancellationRequested();

                    UpdateApplyWorker.RejectReparsePoints(cacheRoot);
                    directory = Path.Combine(cacheRoot, Guid.NewGuid().ToString("N"));
                    lock (cacheGate)
                    {
                        deadline.Token.ThrowIfCancellationRequested();
                        if (disposed) throw new OperationCanceledException("The updater has been closed.");
                        UpdateApplyWorker.RejectReparsePoints(directory);
                        if (Directory.Exists(directory) || File.Exists(directory)) throw new IOException("The update cache directory already exists.");
                        Directory.CreateDirectory(directory);
                        ownedDirectories.Add(directory, asset.Name);
                        UpdateApplyWorker.RejectReparsePoints(directory);
                    }
                    string partialPath = Path.Combine(directory, asset.Name + ".partial");
                    string finalPath = Path.Combine(directory, asset.Name);
                    long received = 0;
                    string actualHash;
                    using (UpdateResponse response = await transport.OpenAsync(asset.DownloadUri, false, deadline.Token).ConfigureAwait(false))
                    {
                        CheckLength(response.ContentLength, MaximumPackageBytes, asset.Size);
                        UpdateApplyWorker.RejectReparsePoints(partialPath);
                        using (var hash = SHA256.Create())
                        using (var output = new FileStream(partialPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 65536, true))
                        {
                            var buffer = new byte[65536];
                            int reportedPercentage = 0;
                            Report(progress, received, asset.Size);
                            while (true)
                            {
                                int count = await response.ReadAsync(buffer, 0, buffer.Length, deadline.Token).ConfigureAwait(false);
                                if (count == 0) break;
                                received += count;
                                if (received > asset.Size || received > MaximumPackageBytes)
                                    throw new InvalidDataException("The update download is larger than the published package.");
                                await output.WriteAsync(buffer, 0, count, deadline.Token).ConfigureAwait(false);
                                hash.TransformBlock(buffer, 0, count, buffer, 0);
                                int percentage = (int)(received * 100.0 / asset.Size);
                                if (percentage != reportedPercentage || received == asset.Size)
                                {
                                    Report(progress, received, asset.Size);
                                    reportedPercentage = percentage;
                                }
                            }
                            if (received != asset.Size) throw new InvalidDataException("The update download is incomplete.");
                            hash.TransformFinalBlock(new byte[0], 0, 0);
                            actualHash = ToHex(hash.Hash);
                            if (!string.Equals(actualHash, expectedHash, StringComparison.Ordinal))
                                throw new InvalidDataException("The update SHA-256 checksum does not match. The package was discarded.");
                            await output.FlushAsync(deadline.Token).ConfigureAwait(false);
                        }
                    }
                    deadline.Token.ThrowIfCancellationRequested();
                    UpdateApplyWorker.RejectReparsePoints(partialPath);
                    UpdateApplyWorker.RejectReparsePoints(finalPath);
                    File.Move(partialPath, finalPath);
                    deadline.Token.ThrowIfCancellationRequested();
                    var downloaded = new DownloadedUpdate { Release = snapshot, Kind = kind, FilePath = finalPath, Sha256 = actualHash };
                    lock (cacheGate)
                    {
                        deadline.Token.ThrowIfCancellationRequested();
                        if (disposed) throw new OperationCanceledException("The updater has been closed.");
                        complete = true;
                    }
                    return downloaded;
                }
                catch (OperationCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested || IsDisposed) throw;
                    throw new TimeoutException("Downloading the update took too long. The partial package was discarded.");
                }
                finally
                {
                    // Only this operation's GUID directory is removed, never an existing installation or cache parent.
                    if (!complete && directory != null) ReleaseOwnedDirectory(directory);
                }
            }
        }

        internal static string GetAssetName(UpdateVersion version, UpdateInstallKind kind)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            if (kind != UpdateInstallKind.Installed && kind != UpdateInstallKind.Portable) throw new ArgumentOutOfRangeException(nameof(kind));
            return "SPI_Data_Comp_Tool_" + version + (kind == UpdateInstallKind.Installed ? "_x64_setup.exe" : ".zip");
        }

        internal static string GetChecksumsName(UpdateVersion version)
        {
            return GetAssetName(version, UpdateInstallKind.Portable) + ".sha256";
        }

        internal static bool IsAssetUri(Uri uri, string tag, string filename)
        {
            return IsRepositoryAssetUri(uri) &&
                string.Equals(uri.AbsolutePath, "/" + Repository + "/releases/download/" + tag + "/" + filename, StringComparison.Ordinal);
        }

        internal static bool IsAllowedRedirect(Uri destination, Uri original)
        {
            if (!IsRepositoryAssetUri(original)) return false;
            // CDN URLs contain short-lived signatures, so queries are allowed only on these two GitHub asset hosts.
            if (IsHttps(destination, "release-assets.githubusercontent.com") || IsHttps(destination, "objects.githubusercontent.com")) return true;
            return IsHttps(destination, "github.com") && string.IsNullOrEmpty(destination.Query) &&
                original != null && string.Equals(destination.AbsolutePath, original.AbsolutePath, StringComparison.Ordinal);
        }

        internal static bool IsReleaseApiUri(Uri uri)
        {
            return IsHttps(uri, "api.github.com") && uri.AbsolutePath == "/repos/" + Repository + "/releases" &&
                Regex.IsMatch(uri.Query, @"\A\?per_page=100&page=([1-9]|10)\z", RegexOptions.CultureInvariant);
        }

        internal static bool IsRepositoryAssetUri(Uri uri)
        {
            if (!IsHttps(uri, "github.com") || !string.IsNullOrEmpty(uri.Query)) return false;
            string prefix = "/" + Repository + "/releases/download/";
            if (!uri.AbsolutePath.StartsWith(prefix, StringComparison.Ordinal)) return false;
            string[] parts = uri.AbsolutePath.Substring(prefix.Length).Split('/');
            UpdateVersion version;
            if (parts.Length != 2 || !UpdateVersion.TryParse(parts[0], out version) || parts[0] != "v" + version) return false;
            return parts[1] == GetAssetName(version, UpdateInstallKind.Installed) ||
                parts[1] == GetAssetName(version, UpdateInstallKind.Portable) || parts[1] == GetChecksumsName(version);
        }

        internal static UpdateRelease ParseRelease(JObject item)
        {
            if (item == null || item["draft"] == null || item["draft"].Type != JTokenType.Boolean || (bool)item["draft"] ||
                item["prerelease"] == null || item["prerelease"].Type != JTokenType.Boolean) return null;
            string tag = Text(item["tag_name"]);
            UpdateVersion version;
            if (!UpdateVersion.TryParse(tag, out version) || tag != "v" + version || (bool)item["prerelease"] != version.IsBeta) return null;
            DateTimeOffset published;
            if (!DateTimeOffset.TryParse(Text(item["published_at"]), CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out published)) return null;
            Uri page;
            if (!Uri.TryCreate(Text(item["html_url"]), UriKind.Absolute, out page) || !IsPageUri(page, tag)) return null;
            var assets = item["assets"] as JArray;
            if (assets == null) return null;
            UpdateAsset installer = FindAsset(assets, tag, GetAssetName(version, UpdateInstallKind.Installed), MaximumPackageBytes);
            UpdateAsset portable = FindAsset(assets, tag, GetAssetName(version, UpdateInstallKind.Portable), MaximumPackageBytes);
            UpdateAsset checksums = FindAsset(assets, tag, GetChecksumsName(version), MaximumChecksumBytes);
            if (checksums == null || (installer == null && portable == null)) return null;
            return new UpdateRelease
            {
                Version = version, Tag = tag, Name = BoundedText(item["name"], 512) ?? tag,
                Notes = BoundedText(item["body"], 256 * 1024) ?? string.Empty, PublishedAt = published, PageUri = page,
                Installer = installer, Portable = portable, Checksums = checksums
            };
        }

        internal static string ParseChecksum(string manifest, string filename)
        {
            if (manifest == null || manifest.Length > MaximumChecksumBytes) throw new InvalidDataException("The checksum file is invalid.");
            string found = null;
            var seen = new HashSet<string>(StringComparer.Ordinal);
            using (var reader = new StringReader(manifest))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0) continue;
                    Match match = ChecksumLine.Match(line);
                    if (!match.Success) throw new InvalidDataException("The checksum file contains an invalid SHA-256 record.");
                    string name = match.Groups[2].Value;
                    if (name.IndexOfAny(new[] { '/', '\\', '\0' }) >= 0 || name != name.Trim() || !seen.Add(name))
                        throw new InvalidDataException("The checksum file contains an unsafe or duplicate filename.");
                    if (string.Equals(name, filename, StringComparison.Ordinal)) found = match.Groups[1].Value.ToLowerInvariant();
                }
            }
            if (found == null) throw new InvalidDataException("The checksum file does not identify the selected package exactly.");
            return found;
        }

        private static UpdateRelease SnapshotForDownload(UpdateRelease release, UpdateInstallKind kind)
        {
            UpdateVersion version;
            if (release == null || release.Version == null || !UpdateVersion.TryParse(release.Tag, out version) ||
                release.Tag != "v" + version || version.CompareTo(release.Version) != 0 || !IsPageUri(release.PageUri, release.Tag))
                throw new InvalidDataException("The selected release is invalid. Check for updates again.");
            if (kind != UpdateInstallKind.Installed && kind != UpdateInstallKind.Portable) throw new ArgumentOutOfRangeException(nameof(kind));
            var result = new UpdateRelease
            {
                Version = version, Tag = release.Tag, Name = release.Name, Notes = release.Notes, PublishedAt = release.PublishedAt,
                PageUri = release.PageUri, Installer = CopyAsset(release.Installer), Portable = CopyAsset(release.Portable), Checksums = CopyAsset(release.Checksums)
            };
            UpdateAsset selected = kind == UpdateInstallKind.Installed ? result.Installer : result.Portable;
            if (!ValidAsset(selected, result.Tag, GetAssetName(version, kind), MaximumPackageBytes) ||
                !ValidAsset(result.Checksums, result.Tag, GetChecksumsName(version), MaximumChecksumBytes))
                throw new InvalidDataException("The selected release does not contain a valid package and checksum file.");
            if (kind == UpdateInstallKind.Installed && !Environment.Is64BitOperatingSystem)
                throw new InvalidOperationException("The installer requires 64-bit Windows. Use the portable package instead.");
            return result;
        }

        private static UpdateAsset CopyAsset(UpdateAsset asset)
        {
            return asset == null ? null : new UpdateAsset { Name = asset.Name, DownloadUri = asset.DownloadUri, Size = asset.Size };
        }

        private static UpdateAsset FindAsset(JArray assets, string tag, string filename, long limit)
        {
            UpdateAsset found = null;
            bool matched = false;
            foreach (JToken token in assets)
            {
                var item = token as JObject;
                if (item == null || Text(item["name"]) != filename) continue;
                if (matched) return null;
                matched = true;
                long size;
                Uri uri;
                if (Text(item["state"]) != "uploaded" || item["size"] == null || item["size"].Type != JTokenType.Integer ||
                    !long.TryParse(item["size"].ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out size) ||
                    !Uri.TryCreate(Text(item["browser_download_url"]), UriKind.Absolute, out uri)) continue;
                var asset = new UpdateAsset { Name = filename, Size = size, DownloadUri = uri };
                if (ValidAsset(asset, tag, filename, limit)) found = asset;
            }
            return found;
        }

        private static bool ValidAsset(UpdateAsset asset, string tag, string filename, long limit)
        {
            return asset != null && asset.Name == filename && asset.Size > 0 && asset.Size <= limit && IsAssetUri(asset.DownloadUri, tag, filename);
        }

        private static bool IsPageUri(Uri uri, string tag)
        {
            return IsHttps(uri, "github.com") && string.IsNullOrEmpty(uri.Query) &&
                uri.AbsolutePath == "/" + Repository + "/releases/tag/" + tag;
        }

        private static bool IsHttps(Uri uri, string host)
        {
            return uri != null && uri.IsAbsoluteUri && uri.Scheme == Uri.UriSchemeHttps && uri.Port == 443 &&
                string.IsNullOrEmpty(uri.UserInfo) && string.IsNullOrEmpty(uri.Fragment) &&
                string.Equals(uri.Host, host, StringComparison.OrdinalIgnoreCase);
        }

        private static string Text(JToken token) { return token != null && token.Type == JTokenType.String ? (string)token : null; }
        private static string BoundedText(JToken token, int maximum)
        {
            string value = Text(token);
            return value == null || value.Length <= maximum ? value : value.Substring(0, maximum);
        }

        private static JArray ParseArray(string json)
        {
            try
            {
                using (var reader = new JsonTextReader(new StringReader(json)) { MaxDepth = 64, DateParseHandling = DateParseHandling.None })
                {
                    var result = JToken.ReadFrom(reader) as JArray;
                    if (result == null || reader.Read()) throw new InvalidDataException("The release server returned an invalid list.");
                    return result;
                }
            }
            catch (JsonException) { throw new InvalidDataException("The release server returned invalid JSON."); }
        }

        private static string DecodeUtf8(byte[] bytes)
        {
            try { return Utf8.GetString(bytes); }
            catch (DecoderFallbackException) { throw new InvalidDataException("The update server returned invalid UTF-8 text."); }
        }

        private static async Task<byte[]> ReadBoundedAsync(UpdateResponse response, int limit, long? expected, CancellationToken token)
        {
            CheckLength(response.ContentLength, limit, expected);
            using (var output = new MemoryStream())
            {
                var buffer = new byte[8192];
                while (true)
                {
                    int count = await response.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);
                    if (count == 0) break;
                    if (output.Length + count > limit || (expected.HasValue && output.Length + count > expected.Value))
                        throw new InvalidDataException("The update metadata exceeds its published size or safety limit.");
                    output.Write(buffer, 0, count);
                }
                if (expected.HasValue && output.Length != expected.Value) throw new InvalidDataException("The checksum download is incomplete.");
                return output.ToArray();
            }
        }

        private static void CheckLength(long? length, long limit, long? expected)
        {
            if (length.HasValue && (length.Value < 0 || length.Value > limit || (expected.HasValue && length.Value != expected.Value)))
                throw new InvalidDataException("The update response size does not match its published size or safety limit.");
        }

        private static string ToHex(byte[] bytes) { return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant(); }
        private static void Report(IProgress<UpdateDownloadProgress> progress, long received, long total)
        {
            if (progress != null) progress.Report(new UpdateDownloadProgress { BytesReceived = received, TotalBytes = total });
        }

        private CancellationTokenSource CreateDeadline(CancellationToken token)
        {
            lock (cacheGate)
            {
                if (disposed) throw new ObjectDisposedException(nameof(UpdateFeedClient));
                return CancellationTokenSource.CreateLinkedTokenSource(token, disposal.Token);
            }
        }

        private bool IsDisposed { get { lock (cacheGate) return disposed; } }

        private void ReleaseOwnedDirectory(string directory)
        {
            string filename;
            lock (cacheGate) { if (!ownedDirectories.TryGetValue(directory, out filename)) return; }
            try
            {
                UpdateApplyWorker.RejectReparsePoints(directory);
                if (Directory.Exists(directory))
                {
                    string partial = Path.Combine(directory, filename + ".partial");
                    string finished = Path.Combine(directory, filename);
                    UpdateApplyWorker.RejectReparsePoints(partial);
                    UpdateApplyWorker.RejectReparsePoints(finished);
                    if (File.Exists(partial)) File.Delete(partial);
                    if (File.Exists(finished)) File.Delete(finished);
                    // Unexpected files/subdirectories belong to neither this download nor its cleanup.
                    Directory.Delete(directory, false);
                }
                lock (cacheGate) ownedDirectories.Remove(directory);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException ||
                error is InvalidDataException || error is System.Security.SecurityException)
            {
                // Active streams are retried after cancellation. Unsafe/unexpected paths are retained,
                // not followed recursively, and cleanup must not conceal the original download error.
            }
        }

        public void Dispose()
        {
            string[] directories;
            lock (cacheGate)
            {
                if (disposed) return;
                disposed = true;
                directories = new string[ownedDirectories.Count];
                ownedDirectories.Keys.CopyTo(directories, 0);
            }
            try { disposal.Cancel(); }
            finally
            {
                foreach (string directory in directories) ReleaseOwnedDirectory(directory);
                disposal.Dispose();
            }
        }
    }

    internal interface IUpdateTransport
    {
        Task<UpdateResponse> OpenAsync(Uri uri, bool releaseMetadata, CancellationToken cancellationToken);
    }

    /// <summary>A bounded-reader transport response. The abort callback makes legacy asynchronous reads cancellable.</summary>
    internal sealed class UpdateResponse : IDisposable
    {
        private readonly Stream content;
        private readonly IDisposable owner;
        private readonly Action abort;
        public long? ContentLength { get; private set; }

        internal UpdateResponse(Stream content, long? length, IDisposable owner = null, Action abort = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            this.content = content;
            ContentLength = length;
            this.owner = owner;
            this.abort = abort;
        }

        internal async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                timeout.CancelAfter(TimeSpan.FromSeconds(30));
                using (timeout.Token.Register(() => { if (abort != null) abort(); }))
                {
                    try
                    {
                        int result = await content.ReadAsync(buffer, offset, count, timeout.Token).ConfigureAwait(false);
                        timeout.Token.ThrowIfCancellationRequested();
                        return result;
                    }
                    catch (Exception error) when (error is IOException || error is WebException || error is OperationCanceledException || error is ObjectDisposedException)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (timeout.IsCancellationRequested) throw new TimeoutException("The update server did not send data within 30 seconds.");
                        throw new IOException("The update download was interrupted. Please try again.");
                    }
                }
            }
        }

        public void Dispose()
        {
            try { content.Dispose(); }
            finally { if (owner != null) owner.Dispose(); }
        }
    }

    internal sealed class HttpUpdateTransport : IUpdateTransport
    {
        public async Task<UpdateResponse> OpenAsync(Uri uri, bool releaseMetadata, CancellationToken cancellationToken)
        {
            if (!(releaseMetadata ? UpdateFeedClient.IsReleaseApiUri(uri) : UpdateFeedClient.IsRepositoryAssetUri(uri)))
                throw new InvalidDataException("Only this application's public GitHub release feed and packages are allowed.");
            Uri original = uri;
            for (int redirects = 0; redirects <= 5; redirects++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var request = WebRequest.CreateHttp(uri);
                request.Method = "GET";
                request.UserAgent = "SPI-Data-Comp-Tool-Updater/1.0";
                request.Accept = releaseMetadata ? "application/vnd.github+json" : "application/octet-stream";
                request.AllowAutoRedirect = false;
                request.UseDefaultCredentials = false;
                request.Credentials = null;
                request.Timeout = 30000;
                request.ReadWriteTimeout = 30000;
                if (releaseMetadata) request.Headers["X-GitHub-Api-Version"] = "2022-11-28";
                HttpWebResponse response;
                using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                {
                    timeout.CancelAfter(TimeSpan.FromSeconds(30));
                    using (timeout.Token.Register(request.Abort))
                    {
                        try { response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false); }
                        catch (WebException error)
                        {
                            int status = error.Response is HttpWebResponse ? (int)((HttpWebResponse)error.Response).StatusCode : 0;
                            if (error.Response != null) error.Response.Dispose();
                            cancellationToken.ThrowIfCancellationRequested();
                            if (timeout.IsCancellationRequested) throw new TimeoutException("The update server did not respond within 30 seconds.");
                            if (status == 403 || status == 429) throw new IOException("GitHub has limited update requests. Wait and try again later.");
                            if (status == 404) throw new IOException("The release package is no longer available. Check for updates again.");
                            throw new IOException("The update server could not be reached or rejected the request. Check your connection and try again.");
                        }
                    }
                }
                int code = (int)response.StatusCode;
                if (code == 301 || code == 302 || code == 303 || code == 307 || code == 308)
                {
                    string location = response.Headers[HttpResponseHeader.Location];
                    response.Dispose();
                    Uri next;
                    if (releaseMetadata || redirects == 5 || !Uri.TryCreate(uri, location, out next) || !UpdateFeedClient.IsAllowedRedirect(next, original))
                        throw new InvalidDataException("The update server redirected outside the allowed GitHub release hosts.");
                    uri = next;
                    continue;
                }
                if (code != 200)
                {
                    response.Dispose();
                    throw new IOException("The update server returned an unexpected response.");
                }
                if (cancellationToken.IsCancellationRequested) { response.Dispose(); cancellationToken.ThrowIfCancellationRequested(); }
                try
                {
                    return new UpdateResponse(response.GetResponseStream(), response.ContentLength < 0 ? (long?)null : response.ContentLength, response, request.Abort);
                }
                catch { response.Dispose(); throw; }
            }
            throw new InvalidDataException("The update server returned too many redirects.");
        }
    }
}
