using System;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Win32;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Checks a downloaded application's runtime before replacing a working copy.</summary>
    internal static class RuntimeRequirements
    {
        internal const int Framework48Release = 528040;

        internal static int InstalledFrameworkRelease()
        {
            int release = 0;
            foreach (RegistryView view in new[] { RegistryView.Registry32, RegistryView.Registry64 })
            {
                if (view == RegistryView.Registry64 && !Environment.Is64BitOperatingSystem) continue;
                try
                {
                    using (var machine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                    using (var key = machine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full", false))
                    {
                        object value = key == null ? null : key.GetValue("Release");
                        if (value is int) release = Math.Max(release, (int)value);
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is SecurityException || ex is UnauthorizedAccessException)
                {
                    // An inaccessible view cannot establish compatibility. A readable other
                    // view may do so; otherwise the update fails before any application changes.
                }
            }
            return release;
        }

        internal static void ValidateConfiguration(string xml, int installedRelease)
        {
            var configuration = UpdatePreferences.ParseXml(xml);
            var startup = configuration.Root == null || configuration.Root.Name != "configuration" ? null :
                configuration.Root.Elements("startup").ToArray();
            if (startup == null || startup.Length != 1)
                throw new InvalidDataException("The update must declare exactly one startup configuration section.");
            var runtimes = startup[0].Elements("supportedRuntime").ToArray();
            if (runtimes.Length != 1 || (string)runtimes[0].Attribute("version") != "v4.0")
                throw new InvalidDataException("The update must declare exactly one supported .NET Framework runtime.");
            string sku = (string)runtimes[0].Attribute("sku");
            int required;
            switch (sku)
            {
                case ".NETFramework,Version=v4.6.2": required = 394802; break;
                case ".NETFramework,Version=v4.7": required = 460798; break;
                case ".NETFramework,Version=v4.7.1": required = 461308; break;
                case ".NETFramework,Version=v4.7.2": required = 461808; break;
                case ".NETFramework,Version=v4.8": required = Framework48Release; break;
                case ".NETFramework,Version=v4.8.1": required = 533320; break;
                default: throw new InvalidDataException("The update requires an unrecognized runtime. Install it manually after checking its system requirements.");
            }
            if (installedRelease < required)
                throw new InvalidOperationException("This update needs Microsoft " + sku.Replace(".NETFramework,Version=v", ".NET Framework ") +
                    " or later. Install it from https://dotnet.microsoft.com/download/dotnet-framework, then retry. " +
                    "Your existing SPI application has not been changed.");
        }
    }
}
