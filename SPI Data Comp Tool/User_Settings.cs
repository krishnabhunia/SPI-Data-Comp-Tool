using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Application preferences, comparison rules and updates share one settings window.</summary>
    public partial class User_Settings : Form
    {
        private bool initializing = true;

        /// <summary>The last successfully imported rules in this window; null when nothing was imported.</summary>
        internal DataSet ImportedRules { get; private set; }

        public User_Settings() : this(false)
        {
        }

        public User_Settings(bool showRules)
        {
            InitializeComponent();
            btn_ImportRules.Click += btn_ImportRules_Click;
            btn_ClearLogs.Click += (sender, e) => rtb_Log.Clear();
            btn_Close.Click += (sender, e) => Close();
            CancelButton = btn_Close;
            tabs_Settings.SelectedIndexChanged += (sender, e) => UpdateAcceptButton();
            UiTheme.Apply(this, btn_ImportRules);

            try
            {
                // Setting radio buttons during startup must not overwrite saved preferences.
                rb_XLSave.Checked = Properties.Settings.Default.XL_Excel_Save;
                rb_EPPlus.Checked = !rb_XLSave.Checked;
                // Display the saved path only. Opening Settings must not read an Excel workbook.
                tb_RulesPath.Text = Properties.Settings.Default.RuleFilePath ?? string.Empty;
                lbl_RulesStatus.Text = string.IsNullOrWhiteSpace(tb_RulesPath.Text)
                    ? "No comparison rules file selected."
                    : "Saved file selected. It will be read when a comparison uses rules.";
                // list the colour themes and show the one in use; choosing another applies it at once
                cb_Theme.Items.AddRange(UiTheme.All);
                cb_Theme.SelectedItem = UiTheme.Current;
                cb_Theme.SelectedIndexChanged += cb_Theme_SelectedIndexChanged;
                if (showRules) ShowRules();
                UpdateAcceptButton();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
            finally
            {
                initializing = false;
            }
        }

        public void ShowRules()
        {
            tabs_Settings.SelectedTab = tab_Rules;
            btn_ImportRules.Select();
        }

        public void ShowUpdates()
        {
            tabs_Settings.SelectedTab = tab_Updates;
            updatesControl.FocusCheckButton();
        }

        private void UpdateAcceptButton()
        {
            bool showingUpdates = tabs_Settings.SelectedTab == tab_Updates;
            // Updates own their status and progress; do not waste their workspace on the rules log.
            gb_Logs.Visible = !showingUpdates;
            tlp_Root.RowStyles[1].SizeType = SizeType.Absolute;
            tlp_Root.RowStyles[1].Height = showingUpdates ? 0F : 140F;
            // Enter must never accidentally start installation.
            AcceptButton = showingUpdates ? null : tabs_Settings.SelectedTab == tab_Rules ? btn_ImportRules : btn_Close;
        }

        private void btn_ImportRules_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new OpenFileDialog
                {
                    Title = "Select comparison rules file",
                    Filter = "Excel workbook (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    RestoreDirectory = true,
                })
                {
                    if (File.Exists(tb_RulesPath.Text)) dialog.FileName = tb_RulesPath.Text;
                    // Cancel is not an error and must preserve the previous file and rules.
                    if (dialog.ShowDialog(this) != DialogResult.OK) return;
                    ImportRulesFile(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                lbl_RulesStatus.Text = Common_Data.RedactConnectionCredentials("Rules were not changed. " + ex.Message);
                lbl_RulesStatus.ForeColor = Color.DarkRed;
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        internal void ImportRulesFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Select a rules workbook first.", nameof(fileName));
            if (!string.Equals(Path.GetExtension(fileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Comparison rules must be an .xlsx workbook.");
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                if (stream.Length == 0) throw new InvalidDataException("The rules workbook is empty.");
            // Parse into a candidate first; a bad workbook must not replace the saved rules.
            DataSet candidate = new Common_Data().GetDatasetRules(fileName);
            try
            {
                ValidateRulesCandidate(candidate);
            }
            catch
            {
                candidate?.Dispose();
                throw;
            }

            string previousPath = Properties.Settings.Default.RuleFilePath;
            try
            {
                Properties.Settings.Default.RuleFilePath = fileName;
                Properties.Settings.Default.Save();
            }
            catch
            {
                Properties.Settings.Default.RuleFilePath = previousPath;
                candidate.Dispose();
                throw;
            }

            ImportedRules = candidate;
            tb_RulesPath.Text = fileName;
            lbl_RulesStatus.Text = $"Imported {candidate.Tables[0].Rows.Count} rules. You can close Settings and continue.";
            lbl_RulesStatus.ForeColor = UiTheme.TextColor;
            Common_Data.DisplayMessage(rtb_Log, "Comparison rules imported: " + fileName);
        }

        internal static void ValidateRulesCandidate(DataSet candidate)
        {
            if (candidate == null || candidate.Tables.Count == 0 || candidate.Tables[0].Rows.Count == 0)
                throw new InvalidOperationException("The rules workbook must contain at least one rule.");
            DataTable rules = candidate.Tables[0];
            // These names are used by Common_Data.CheckRulesInFile, even when the current
            // input does not match an earlier rule. Validate before persisting the new file path.
            string[] required = { "srno", "Source", "Target", "RuleType", "Source_ColName", "Target_Colname" };
            foreach (string column in required)
                if (!rules.Columns.Contains(column))
                    throw new InvalidOperationException("The rules workbook is missing the required column " + column + ".");

            var identifiers = new HashSet<string>(StringComparer.Ordinal);
            foreach (DataRow rule in rules.Rows)
            {
                string identifier = rule["srno"].ToString();
                if (string.IsNullOrWhiteSpace(identifier) || !identifiers.Add(identifier))
                    throw new InvalidOperationException("Every comparison rule needs a nonempty, unique srno value.");
                string ruleType = rule["RuleType"].ToString().Trim();
                if (ruleType != "1" && ruleType != "2" && ruleType != "3")
                    throw new InvalidOperationException("Comparison RuleType must be 1, 2 or 3.");
                if ((ruleType == "2" || ruleType == "3") &&
                    (string.IsNullOrWhiteSpace(rule["Source_ColName"].ToString()) || string.IsNullOrWhiteSpace(rule["Target_Colname"].ToString())))
                    throw new InvalidOperationException("Rule types 2 and 3 require Source_ColName and Target_Colname.");
                // The existing type-3 evaluator reads conditional fields by position (indexes6–9).
                // Preserve that file format; do not silently reinterpret or reorder the workbook.
                if (rule["RuleType"].ToString().Trim() == "3" && rules.Columns.Count < 10)
                    throw new InvalidOperationException("Rule type 3 requires the conditional source column, target column, source value and target value in columns 7–10.");
            }
        }

        private void cb_Theme_SelectedIndexChanged(object sender, EventArgs e)
        {
            var palette = cb_Theme.SelectedItem as UiPalette;
            if (initializing || palette == null || palette == UiTheme.Current) return;
            try
            {
                string notSaved = UiTheme.Use(palette);
                swatch_Theme.Invalidate();
                lbl_ThemeHelp.Text = notSaved.Length == 0
                    ? palette.Title + " is now used in every open window and is remembered for next time."
                    : palette.Title + " is used until the app closes. The choice could not be saved: " + notSaved;
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_EPPlus_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing && rb_EPPlus.Checked)
                ExcelChangeSave("EPPlus mode selected for saving Excel output");
        }

        private void rb_XLSave_CheckedChanged(object sender, EventArgs e)
        {
            if (!initializing && rb_XLSave.Checked)
                ExcelChangeSave("Microsoft Excel mode selected for saving Excel output");
        }

        private void ExcelChangeSave(string message)
        {
            bool previousEpplus = Properties.Settings.Default.EPPlus_Excel_Save;
            bool previousExcel = Properties.Settings.Default.XL_Excel_Save;
            try
            {
                Properties.Settings.Default.EPPlus_Excel_Save = rb_EPPlus.Checked;
                Properties.Settings.Default.XL_Excel_Save = rb_XLSave.Checked;
                Properties.Settings.Default.Save();
                Common_Data.DisplayMessage(rtb_Log, message);
            }
            catch (Exception ex)
            {
                Properties.Settings.Default.EPPlus_Excel_Save = previousEpplus;
                Properties.Settings.Default.XL_Excel_Save = previousExcel;
                initializing = true;
                try
                {
                    rb_XLSave.Checked = previousExcel;
                    rb_EPPlus.Checked = !previousExcel;
                }
                finally
                {
                    initializing = false;
                }
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
    }
}
