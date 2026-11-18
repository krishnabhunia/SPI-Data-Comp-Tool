using System.Reflection;

namespace SPI_Data_Comp_Tool
{
    /// <summary>The version CI stamped into the exe, e.g. "2.4.0" or "2.4.0-beta003".</summary>
    internal static class AppVersion
    {
        public static string Full
        {
            get
            {
                var asm = Assembly.GetExecutingAssembly();
                var info = (AssemblyInformationalVersionAttribute[])asm.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                if (info.Length > 0 && !string.IsNullOrEmpty(info[0].InformationalVersion)) return info[0].InformationalVersion;
                var v = asm.GetName().Version;
                return v.Major + "." + v.Minor + "." + v.Build;
            }
        }

        public static bool IsBeta { get { return Full.Contains("-beta"); } }

        public static string Display { get { return "v" + Full + (IsBeta ? "  ·  Beta" : ""); } }
    }
}
