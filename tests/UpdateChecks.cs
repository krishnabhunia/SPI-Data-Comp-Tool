using System;
using System.Collections.Generic;
using System.IO;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Compatibility entry point for --check-updates. It exercises the same canonical feed/apply
    /// checks as --check-ui, rather than retaining a second updater and a separate safety contract.
    /// Only fake transports and isolated temporary files are used; no live release is installed.
    /// </summary>
    internal static class UpdateChecks
    {
        public static int Run(string outputFile)
        {
            var lines = new List<string>();
            Action<string> passed = message => lines.Add("PASS " + message);
            try
            {
                // Blocking feed checks must finish before file checks can create a WinForms context.
                UpdateFeedChecks.Run(passed);
                UpdateApplyChecks.Run(passed);
                File.WriteAllLines(outputFile, lines);
                return 0;
            }
            catch (Exception ex)
            {
                lines.Add("FAIL " + ex);
                File.WriteAllLines(outputFile, lines);
                return 1;
            }
        }
    }
}
