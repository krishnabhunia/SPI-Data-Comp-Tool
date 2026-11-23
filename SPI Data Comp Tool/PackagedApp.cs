using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Tells whether this copy runs from a Microsoft Store (MSIX) package. A Store copy is updated by
    /// the Store, so the app's own updater must never replace its files (they are read-only anyway).
    /// </summary>
    internal static class PackagedApp
    {
        internal const string StoreUpdateReason =
            "This copy was installed from the Microsoft Store. The Store installs its updates for you.";
        private const int AppModelErrorNoPackage = 15700;
        private const int ErrorInsufficientBuffer = 122;
        private static readonly Lazy<string> fullName = new Lazy<string>(ReadFullName);

        /// <summary>True when Windows reports a package identity for this process.</summary>
        internal static bool IsPackaged { get { return FullName != null; } }

        /// <summary>The package full name, or null when the app runs outside a package.</summary>
        internal static string FullName { get { return fullName.Value; } }

        private static string ReadFullName()
        {
            try
            {
                int length = 0;
                int result = GetCurrentPackageFullName(ref length, null);
                if (result == AppModelErrorNoPackage || result != ErrorInsufficientBuffer || length <= 0) return null;
                var buffer = new StringBuilder(length);
                result = GetCurrentPackageFullName(ref length, buffer);
                return result == 0 && buffer.Length > 0 ? buffer.ToString() : null;
            }
            catch (EntryPointNotFoundException) { return null; }   // Windows 7: no package support
            catch (DllNotFoundException) { return null; }           // non-Windows test runtime
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);
    }
}
