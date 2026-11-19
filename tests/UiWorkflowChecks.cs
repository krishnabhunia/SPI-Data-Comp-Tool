using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
                ProfileWorkflowChecks.Run(passed);
                ImportSchemaChecks.Run(passed);
                CredentialRedactionChecks.Run(passed);
                CheckNavigation(passed);
                CheckSettings(passed);
                CheckAnalysis(passed);
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
            var navigator = new WindowNavigator();
            using (var owner = new Form())
            {
                owner.Show();
                int created = 0;
                Func<Form> create = () => { created++; return new Form(); };
                var first = navigator.Open(owner, create, "check");
                var repeat = navigator.Open(owner, create, "check");
                Require(ReferenceEquals(first, repeat) && created == 1, "Repeated navigation created a second window.");
                first.WindowState = FormWindowState.Minimized;
                navigator.Open(owner, create, "check");
                Require(first.WindowState != FormWindowState.Minimized, "An existing minimized window was not restored.");
                var configuration = navigator.Open(owner, create, "configuration");
                Require(!ReferenceEquals(first, configuration), "Different operating modes were incorrectly combined.");
                var separate = navigator.Open(owner, create, "check", true);
                Require(!ReferenceEquals(first, separate) && ReferenceEquals(first, navigator.Open(owner, create, "check")),
                    "A separate session replaced the default tool window.");
                separate.Close();
                first.Close();
                Require(!ReferenceEquals(first, navigator.Open(owner, create, "check")), "A closed window could not be reopened.");
                owner.Close();
            }
            passed("Window navigation reuses active tools, restores minimized windows, keeps operating modes separate and supports explicit parallel sessions.");
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

        private static DataSet ExampleAnalysis()
        {
            var data = new DataSet();
            foreach (string side in new[] { "Source", "Target" })
            {
                var table = new DataTable(side);
                table.Columns.Add("SPEC_FORM_CNUM", typeof(int));
                table.Columns.Add("SPEC_NAME", typeof(string));
                table.Columns.Add("COLUMN_NAME", typeof(string));
                table.Columns.Add("FORMAT_NAME", typeof(string));
                table.Columns.Add("HEADER_TEXT", typeof(string));
                table.Rows.Add(1, "Example specification", "TAG_NUMBER", "Instrument", "Tag number");
                data.Tables.Add(table);
            }
            return data;
        }

        private static void CheckAnalysis(Action<string> passed)
        {
            using (var analysis = new SPI_Analysis())
            using (var sample = ExampleAnalysis())
            {
                analysis.Show();
                Require(!Find<Button>(analysis, "btn_Show").Enabled, "Analysis can run before both inputs are loaded.");
                Application.DoEvents();
                var setup = Find<TabPage>(analysis, "setupPage");
                analysis.DisplayResults(sample);
                var resultsPage = Find<TabPage>(analysis, "resultsPage");
                var results = resultsPage.Controls.OfType<SPI_Analysis_View>().Single();
                Require(ReferenceEquals(setup, Find<TabPage>(analysis, "setupPage")), "Showing analysis results discarded connection setup.");
                Require(Find<TabControl>(analysis, "workspaceTabs").SelectedTab == resultsPage, "Results were not shown inside Analysis.");
                sample.Tables[0].Rows.Clear();
                Require(((DataTable)Find<DataGridView>(results, "dgv_Source").DataSource).Rows.Count == 1,
                    "Reloading connection data changed the displayed result snapshot.");
                bool rejected = false;
                try { analysis.DisplayResults(new DataSet()); }
                catch (ArgumentException) { rejected = true; }
                Require(rejected && !results.IsDisposed && resultsPage.Controls.Contains(results),
                    "An invalid reload destroyed existing results.");
                Find<Button>(results, "btn_Arrange_target").PerformClick();
                Require(Find<Button>(results, "btn_SaveInExcel").Enabled, "Synthetic analysis results could not be arranged for export.");
                var remarks = (DataTable)Find<DataGridView>(results, "dgv_Remarks").DataSource;
                Require(remarks.Rows.Count == 1 && remarks.Rows[0][0].ToString() == "Column and Header Match",
                    "The embedded Analysis changed the synthetic matching result.");
                var sourceFilter = Find<ComboBox>(results, "cb_Source");
                sourceFilter.Text = "999";
                Application.DoEvents();
                Require(((DataTable)Find<DataGridView>(results, "dgv_Source").DataSource).Rows.Count == 0 &&
                    !Find<Button>(results, "btn_Arrange_target").Enabled, "An unknown form selection left stale rows available to Arrange.");
                sourceFilter.Text = "1";
                Application.DoEvents();
                Require(Find<Button>(results, "btn_Arrange_target").Enabled, "Returning to a valid form did not enable Arrange.");
                analysis.Size = new Size(920, 660);
                analysis.PerformLayout();
                Application.DoEvents();
                Require(results.Dock == DockStyle.Fill, "Analysis results do not resize with their workspace.");
                using (var next = ExampleAnalysis()) analysis.DisplayResults(next);
                Require(results.IsDisposed && resultsPage.Controls.OfType<SPI_Analysis_View>().Count() == 1,
                    "Replacing results leaked the old result control or opened duplicate result panels.");
                analysis.Close();
            }
            passed("Analysis keeps setup/results in one window; snapshots survive input changes and failed reloads; replacement disposes old results and Arrange remains available.");
        }

    }
}
