using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Windows-only, synthetic-data checks. Never opens a database or saves user preferences.</summary>
    internal static class UiWorkflowChecks
    {
        public static int Run(string outputFile)
        {
            var messages = new List<string>();
            Action<string> passed = message => messages.Add("PASS " + message);
            try
            {
                // Run offline network/file checks before WinForms installs a UI synchronization context.
                UpdateFeedChecks.Run(passed);
                UpdateApplyChecks.Run(passed);
                LoggingChecks.Run(passed);
                ProfileWorkflowChecks.Run(passed);
                ImportSchemaChecks.Run(passed);
                CredentialRedactionChecks.Run(passed);
                // Independent synthetic suites clean up their own fixtures. Report every suite's
                // failure in one run rather than hiding later regressions behind the first one.
                bool validationPassed = RunValidationSuite("Core", () => { CoreValidationChecks.Run(passed); }, messages);
                validationPassed &= RunValidationSuite("Connections", () => { ConnectionValidationChecks.Run(passed); }, messages);
                validationPassed &= RunValidationSuite("Profile loading", () => { ProfileLoadChecks.Run(passed); }, messages);
                validationPassed &= RunValidationSuite("Comparison correctness", () => { ComparisonCorrectnessChecks.Run(passed); }, messages);
                validationPassed &= RunValidationSuite("Application preferences", () => { PreferenceChecks.Run(passed); }, messages);
                validationPassed &= RunValidationSuite("Database forms", () => { DatabaseFormValidationChecks.Run(passed); }, messages);
                validationPassed &= RunValidationSuite("Excel forms", () => { ExcelFormValidationChecks.Run(passed); }, messages);
                validationPassed &= RunValidationSuite("Workbook identity", () => { WorkbookIdentityChecks.Run(passed); }, messages);
                validationPassed &= RunValidationSuite("Workbook identity form", () => { WorkbookIdentityFormChecks.Run(passed); }, messages);
                if (!validationPassed)
                {
                    File.WriteAllLines(outputFile, messages);
                    return 1;
                }
                CheckNavigation(passed);
                CheckSettings(passed);
                UpdateUiChecks.Run(passed);
                File.WriteAllLines(outputFile, messages);
                return 0;
            }
            catch (Exception ex)
            {
                messages.Add("FAIL " + ex);
                File.WriteAllLines(outputFile, messages);
                return 1;
            }
        }

        private static bool RunValidationSuite(string name, Action run, List<string> messages)
        {
            try
            {
                run();
                return true;
            }
            catch (Exception ex)
            {
                messages.Add("FAIL " + name + " validation suite: " + ex);
                return false;
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static T Find<T>(Control root, string name) where T : Control
        {
            var result = root.Controls.Find(name, true).OfType<T>().SingleOrDefault();
            if (result == null) throw new InvalidOperationException("Missing control: " + name);
            return result;
        }

        private static void CheckNavigation(Action<string> passed)
        {
            WorkspaceUiChecks.Run(passed);
        }

        private static void CheckSettings(Action<string> passed)
        {
            string savedPath = Properties.Settings.Default.RuleFilePath;
            bool savedEpplus = Properties.Settings.Default.EPPlus_Excel_Save;
            bool savedExcel = Properties.Settings.Default.XL_Excel_Save;
            string missing = Path.Combine(Path.GetTempPath(), "spi-absent-rules-" + Guid.NewGuid().ToString("N") + ".xlsx");
            try
            {
                Properties.Settings.Default.RuleFilePath = missing;
                Properties.Settings.Default.EPPlus_Excel_Save = false;
                Properties.Settings.Default.XL_Excel_Save = true;
                using (var settings = new User_Settings(true))
                {
                    Require(settings.ImportedRules == null, "Opening Settings eagerly imported a rules workbook.");
                    var tabs = Find<TabControl>(settings, "tabs_Settings");
                    Require(tabs.SelectedTab.Name == "tab_Rules", "An Import Rules caller did not open the Rules tab.");
                    Require(Properties.Settings.Default.RuleFilePath == missing && Properties.Settings.Default.XL_Excel_Save &&
                        !Properties.Settings.Default.EPPlus_Excel_Save, "Opening Settings changed saved preferences.");
                    bool rejected = false;
                    try { settings.ImportRulesFile(missing); }
                    catch (IOException) { rejected = true; }
                    Require(rejected && settings.ImportedRules == null && Properties.Settings.Default.RuleFilePath == missing,
                        "An invalid import replaced previous comparison rules.");
                    settings.Show();
                    Application.DoEvents();
                    tabs.SelectedIndex = 0;
                    Application.DoEvents();
                    Require(tabs.TabPages.Count == 3 && tabs.TabPages[0].Name == "tab_General" && tabs.TabPages[1].Name == "tab_Rules"
                        && tabs.TabPages[2].Name == "tab_Updates", "The consolidated Settings window has missing sections.");
                    settings.Close();
                }
            }
            finally
            {
                Properties.Settings.Default.RuleFilePath = savedPath;
                Properties.Settings.Default.EPPlus_Excel_Save = savedEpplus;
                Properties.Settings.Default.XL_Excel_Save = savedExcel;
            }
            passed("Settings and Rules share one window; startup and failed rule imports preserve preferences.");
        }

    }
}
