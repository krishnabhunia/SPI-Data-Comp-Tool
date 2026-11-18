using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    internal enum UpdateChannel { Normal, Beta, All }
    internal enum UpdateInstallKind { Installed, Portable }

    internal sealed class UpdateAsset
    {
        public string Name { get; set; }
        public Uri DownloadUri { get; set; }
        public long Size { get; set; }
    }

    internal sealed class UpdateRelease
    {
        public UpdateVersion Version { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public DateTimeOffset PublishedAt { get; set; }
        public Uri PageUri { get; set; }
        public UpdateAsset Installer { get; set; }
        public UpdateAsset Portable { get; set; }
        public UpdateAsset Checksums { get; set; }

        public bool IsCompatible(UpdateInstallKind kind, bool is64BitOperatingSystem)
        {
            return Checksums != null && (kind == UpdateInstallKind.Portable
                ? Portable != null : is64BitOperatingSystem && Installer != null);
        }
    }

    internal sealed class UpdateDownloadProgress
    {
        public long BytesReceived { get; set; }
        public long? TotalBytes { get; set; }
        public int Percentage
        {
            get { return TotalBytes.HasValue && TotalBytes.Value > 0 ? (int)Math.Max(0, Math.Min(100, BytesReceived * 100.0 / TotalBytes.Value)) : 0; }
        }
    }

    internal sealed class DownloadedUpdate
    {
        public UpdateRelease Release { get; set; }
        public UpdateInstallKind Kind { get; set; }
        public string FilePath { get; set; }
        public string Sha256 { get; set; }
    }

    internal sealed class UpdateInstallation
    {
        public UpdateInstallKind Kind { get; set; }
        public string Directory { get; set; }
        public string ExecutablePath { get; set; }
        public bool IsMachineInstall { get; set; }
    }

    internal interface IUpdateFeed
    {
        Task<IList<UpdateRelease>> CheckAsync(CancellationToken cancellationToken);
        Task<DownloadedUpdate> DownloadAsync(UpdateRelease release, UpdateInstallKind kind,
            IProgress<UpdateDownloadProgress> progress, CancellationToken cancellationToken);
    }

    internal interface IUpdateInstaller
    {
        UpdateInstallation Installation { get; }
        bool CanInstall(out string reason);
        // Called only after the user explicitly confirms installation and has closed other tools.
        void InstallAndRestart(DownloadedUpdate package, IWin32Window owner);
    }
}
