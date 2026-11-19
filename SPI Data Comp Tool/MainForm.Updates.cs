namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Displays the running build's version and channel. Checking, selecting, downloading and
    /// installing updates are explicit actions in the single Settings > Updates workspace.
    /// </summary>
    public partial class MainForm
    {
        private void InitializeUpdates()
        {
            lbl_Version.Text = BuildVersionStatus(AppVersion.Full);
        }

        internal static string BuildVersionStatus(string fullVersion)
        {
            UpdateVersion version;
            if (!UpdateVersion.TryParse(fullVersion, out version))
                return (string.IsNullOrWhiteSpace(fullVersion) ? "Version unavailable" : fullVersion) +
                    "  ·  Manual / unrecognised build";

            // This describes the running executable, not the user's release-list filter.
            return "v" + version + "  ·  " + (version.IsBeta ? "Beta" : "Normal");
        }
    }
}
