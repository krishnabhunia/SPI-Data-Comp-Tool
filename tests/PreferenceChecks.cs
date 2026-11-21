using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Optional settings failures with isolated stores/configuration files; never saves real user preferences.</summary>
    internal static class PreferenceChecks
    {
        internal static void Run(Action<string> passed)
        {
            CheckWriterSelection();
            passed("Preferences: all legacy writer-flag pairs select exactly one Excel writer without changing saved settings.");
            CheckUnavailableReads();
            passed("Preferences: unavailable settings keep Rules, themes and shown Settings controls usable with safe defaults.");
            CheckFailedSaves(false);
            CheckFailedSaves(true);
            CheckPartialSetterFailure();
            passed("Preferences: shown radio events handle save and rollback failures once without unhandled errors or repair writes.");
            WithDirectory(CheckRulesImportFailure);
            passed("Preferences: a valid rules workbook is not published when its remembered path cannot be saved.");
            WithDirectory(CheckMappedConfiguration);
            passed("Preferences: real malformed/locked mapped user.config files fall back safely and retain their exact bytes.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static T Find<T>(Control root, string name) where T : Control
        {
            return root.Controls.Find(name, true).OfType<T>().Single();
        }

        private static void WithDirectory(Action<string> check)
        {
            string folder = Path.Combine(Path.GetTempPath(), "spi-preferences-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            try { check(folder); }
            finally { Directory.Delete(folder, true); }
        }

        private sealed class FaultStore : IAppPreferenceStore
        {
            internal string Rules = "previous-rules.xlsx";
            internal bool Epplus = true;
            internal bool Excel;
            internal Exception ReadError;
            internal Exception SaveError;
            internal bool FailRestoration;
            internal int FailWriteNumber;
            internal int Reads, Writes, Saves;

            private void Read()
            {
                Reads++;
                if (ReadError != null) throw ReadError;
            }

            private void Write()
            {
                Writes++;
                if ((FailRestoration && Saves != 0) || Writes == FailWriteNumber)
                    throw new ConfigurationErrorsException("Synthetic settings setter failure.");
            }

            public string RuleFilePath { get { Read(); return Rules; } set { Write(); Rules = value; } }
            public bool EPPlusExcelSave { get { Read(); return Epplus; } set { Write(); Epplus = value; } }
            public bool MicrosoftExcelSave { get { Read(); return Excel; } set { Write(); Excel = value; } }
            public void Save()
            {
                Saves++;
                if (SaveError != null) throw SaveError;
            }
        }

        private static void CheckWriterSelection()
        {
            foreach (bool epplus in new[] { false, true })
            foreach (bool excel in new[] { false, true })
            {
                var store = new FaultStore { Epplus = epplus, Excel = excel };
                using (AppPreferences.UseForChecks(store))
                {
                    AppPreferenceValues preferences = AppPreferences.Read();
                    ExcelWriterMode expected = !epplus && excel ? ExcelWriterMode.MicrosoftExcel : ExcelWriterMode.EPPlus;
                    Require(preferences.ExcelWriter == expected && AppPreferences.ExcelWriter == expected,
                        "An invalid flag pair disabled output or selected inconsistent writers.");
                    Require((preferences.Warning.Length != 0) == (epplus == excel),
                        "An inconsistent saved writer pair did not explain its fallback.");
                    Require(AppPreferences.RuleFilePath == store.Rules && store.Saves == 0 && store.Writes == 0,
                        "Reading defaults changed the saved preferences.");
                }
            }
        }

        private static void CheckUnavailableReads()
        {
            const string privateDetail = "synthetic-private-error-detail";
            Exception[] failures =
            {
                new ConfigurationErrorsException(privateDetail), new IOException(privateDetail),
                new UnauthorizedAccessException(privateDetail), new SettingsPropertyWrongTypeException(privateDetail),
                new TypeInitializationException("SyntheticSettings", new ConfigurationErrorsException(privateDetail)),
            };
            foreach (Exception failure in failures)
            {
                var store = new FaultStore { ReadError = failure };
                using (AppPreferences.UseForChecks(store))
                {
                    AppPreferenceValues preferences = AppPreferences.Read();
                    Require(preferences.RuleFilePath == string.Empty && preferences.ExcelWriter == ExcelWriterMode.EPPlus
                        && preferences.Warning.Length != 0 && !preferences.Warning.Contains(privateDetail),
                        "Unavailable preferences threw, exposed raw contents, or failed to select safe defaults.");
                    string warning;
                    Require(!AppPreferences.TrySaveRulesPath("candidate.xlsx", out warning)
                        && !AppPreferences.TrySaveExcelWriter(ExcelWriterMode.MicrosoftExcel, out warning)
                        && store.Writes == 0 && store.Saves == 0,
                        "An unreadable settings provider was modified during recovery.");
                    if (failure is ConfigurationErrorsException)
                    {
                        using (var settings = new User_Settings(true))
                        {
                            settings.Show();
                            Application.DoEvents();
                            AssertInitialized(settings);
                            Find<TabControl>(settings, "tabs_Settings").SelectedIndex = 0;
                            Find<RadioButton>(settings, "rb_XLSave").Checked = true;
                            Application.DoEvents();
                            Require(Find<RadioButton>(settings, "rb_EPPlus").Checked
                                && Find<RichTextBox>(settings, "rtb_Log").Text.Contains("could not be saved")
                                && store.Writes == 0 && store.Saves == 0,
                                "A real radio event escaped the failed preference-read boundary or attempted a repair write.");
                            settings.Close();
                        }
                    }
                }
            }
        }

        private static void AssertInitialized(User_Settings settings)
        {
            ComboBox themes = Find<ComboBox>(settings, "cb_Theme");
            TabControl tabs = Find<TabControl>(settings, "tabs_Settings");
            Require(settings.Visible && settings.IsHandleCreated && themes.Items.Count == UiTheme.All.Length
                && ReferenceEquals(themes.SelectedItem, UiTheme.Current)
                && tabs.TabPages.Count == 3 && tabs.SelectedTab.Name == "tab_Rules"
                && ReferenceEquals(settings.AcceptButton, Find<Button>(settings, "btn_ImportRules"))
                && ReferenceEquals(settings.CancelButton, Find<Button>(settings, "btn_Close")),
                "An unavailable preference left the shown Settings form partially initialized.");
        }

        private static void CheckFailedSaves(bool failRestoration)
        {
            var store = new FaultStore { SaveError = new IOException("Synthetic read-only settings file."), FailRestoration = failRestoration };
            using (AppPreferences.UseForChecks(store))
            using (var settings = new User_Settings(true))
            {
                settings.Show();
                Application.DoEvents();
                AssertInitialized(settings);
                Find<TabControl>(settings, "tabs_Settings").SelectedIndex = 0;
                Find<RadioButton>(settings, "rb_XLSave").Checked = true;
                Application.DoEvents();
                Require(store.Saves == 1 && store.Writes == 4
                    && Find<RichTextBox>(settings, "rtb_Log").Text.Contains("could not be saved"),
                    "A failed save/rollback escaped its event handler, retried saving, or omitted a visible warning.");
                Require(Find<RadioButton>(settings, "rb_EPPlus").Checked != Find<RadioButton>(settings, "rb_XLSave").Checked,
                    "A rollback error left both Excel output modes selected or neither selected.");
                if (!failRestoration)
                    Require(store.Epplus && !store.Excel && Find<RadioButton>(settings, "rb_EPPlus").Checked,
                        "A failed save did not restore its earlier in-memory values and visible selection.");
                settings.Close();
            }
        }

        private static void CheckPartialSetterFailure()
        {
            var store = new FaultStore { FailWriteNumber = 2 };
            using (AppPreferences.UseForChecks(store))
            {
                string warning;
                Require(!AppPreferences.TrySaveExcelWriter(ExcelWriterMode.MicrosoftExcel, out warning)
                    && store.Epplus && !store.Excel && store.Saves == 0 && store.Writes == 4 && warning.Length != 0,
                    "A second-setter failure saved or retained a partially changed writer selection.");
            }
        }

        private static void CheckRulesImportFailure(string folder)
        {
            string workbook = Path.Combine(folder, "candidate.xlsx");
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Rules");
                string[] headers = { "srno", "Source", "Target", "RuleType", "Source_ColName", "Target_Colname" };
                string[] values = { "1", "source", "target", "1", "", "" };
                for (int column = 0; column < headers.Length; column++)
                {
                    sheet.Cells[1, column + 1].Value = headers[column];
                    sheet.Cells[2, column + 1].Value = values[column];
                }
                package.SaveAs(new FileInfo(workbook));
            }
            var store = new FaultStore { SaveError = new IOException("Synthetic rules-path persistence failure.") };
            using (AppPreferences.UseForChecks(store))
            using (var settings = new User_Settings(true))
            {
                bool failed = false;
                try { settings.ImportRulesFile(workbook); }
                catch (InvalidOperationException error) { failed = error.Message.Contains("could not be saved"); }
                Require(failed && settings.ImportedRules == null && store.Rules == "previous-rules.xlsx"
                    && Find<TextBox>(settings, "tb_RulesPath").Text == store.Rules && store.Saves == 1 && store.Writes == 2,
                    "A valid candidate rules file was published despite failure to save its path.");
            }
        }

        // Mapped configuration uses the real .NET XML/configuration stack but every path belongs
        // to this temporary fixture; no real LocalFileSettingsProvider path is discovered or saved.
        private sealed class MappedConfigStore : IAppPreferenceStore
        {
            private readonly ExeConfigurationFileMap files;
            internal int Writes;
            internal MappedConfigStore(string application, string local, string roaming)
            {
                files = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = application, LocalUserConfigFilename = local, RoamingUserConfigFilename = roaming,
                };
            }
            private string Read(string name)
            {
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(files, ConfigurationUserLevel.PerUserRoamingAndLocal);
                var section = (ClientSettingsSection)config.GetSection("userSettings/" + UpdatePreferences.SettingsSection);
                return section.Settings.Get(name).Value.ValueXml.InnerText;
            }
            private void RejectWrite() { Writes++; throw new InvalidOperationException("Read-only mapped fixture."); }
            public string RuleFilePath { get { return Read("RuleFilePath"); } set { RejectWrite(); } }
            public bool EPPlusExcelSave { get { return bool.Parse(Read("EPPlus_Excel_Save")); } set { RejectWrite(); } }
            public bool MicrosoftExcelSave { get { return bool.Parse(Read("XL_Excel_Save")); } set { RejectWrite(); } }
            public void Save() { RejectWrite(); }
        }

        private static void CheckMappedConfiguration(string folder)
        {
            string application = Path.Combine(folder, "fixture.exe.config");
            string local = Path.Combine(folder, "user.config");
            string roaming = Path.Combine(folder, "absent-roaming.config");
            string applicationText = "<configuration><configSections><sectionGroup name=\"userSettings\" " +
                "type=\"" + typeof(UserSettingsGroup).AssemblyQualifiedName + "\"><section name=\"" + UpdatePreferences.SettingsSection +
                "\" type=\"" + typeof(ClientSettingsSection).AssemblyQualifiedName + "\" allowExeDefinition=\"MachineToLocalUser\" " +
                "requirePermission=\"false\" /></sectionGroup></configSections></configuration>";
            string valid = "<configuration><userSettings><" + UpdatePreferences.SettingsSection + ">" +
                "<setting name=\"RuleFilePath\" serializeAs=\"String\"><value>fixture-rules.xlsx</value></setting>" +
                "<setting name=\"EPPlus_Excel_Save\" serializeAs=\"String\"><value>False</value></setting>" +
                "<setting name=\"XL_Excel_Save\" serializeAs=\"String\"><value>True</value></setting>" +
                "</" + UpdatePreferences.SettingsSection + "></userSettings></configuration>";
            File.WriteAllText(application, applicationText);
            File.WriteAllText(local, valid);
            var store = new MappedConfigStore(application, local, roaming);
            // Prove the actual fixture/provider can load before testing recovery. Reading only
            // through AppPreferences would hide a setup error behind its intended safe fallback.
            string baselineRules = store.RuleFilePath;
            bool baselineEpplus = store.EPPlusExcelSave;
            bool baselineExcel = store.MicrosoftExcelSave;
            Require(baselineRules == "fixture-rules.xlsx" && !baselineEpplus && baselineExcel,
                "The mapped configuration fixture did not load its expected baseline values.");
            using (AppPreferences.UseForChecks(store))
            {
                Require(AppPreferences.RuleFilePath == "fixture-rules.xlsx" && AppPreferences.ExcelWriter == ExcelWriterMode.MicrosoftExcel,
                    "The mapped configuration fixture did not read real user-scoped settings.");
                const string malformed = "<configuration><userSettings><unfinished";
                File.WriteAllText(local, malformed);
                AppPreferenceValues invalid = AppPreferences.Read();
                Require(invalid.RuleFilePath.Length == 0 && invalid.ExcelWriter == ExcelWriterMode.EPPlus && invalid.Warning.Length != 0
                    && File.ReadAllText(local) == malformed, "Malformed mapped settings were not safely isolated/preserved.");
                File.WriteAllText(local, valid);
                using (var held = new FileStream(local, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    AppPreferenceValues locked = AppPreferences.Read();
                    Require(locked.RuleFilePath.Length == 0 && locked.ExcelWriter == ExcelWriterMode.EPPlus && locked.Warning.Length != 0,
                        "Locked mapped settings did not use the same read-recovery path.");
                }
                Require(store.Writes == 0 && File.ReadAllText(local) == valid && File.ReadAllText(application) == applicationText
                    && !File.Exists(roaming), "Recovering from unavailable configuration changed a fixture file.");
            }
        }
    }
}
