using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace SPI_Data_Comp_Tool
{
    public partial class MainForm : Form
    {
        private readonly WindowNavigator windows = new WindowNavigator();
        public MainForm()
        {
            InitializeComponent();
            MyInitialization();

            // The login screen was removed: its user name and password were hard-coded in this
            // source, so it protected nothing. The main window now opens ready to use.
            UnlockMainWindow();
            InitializeUpdates();
            // the header band and the status bar take their colours from the theme chosen in Settings
            UiTheme.MarkHeader(pnl_Header);
            UiTheme.MarkStatus(pnl_Status);
            UiTheme.Apply(this);
        }

        private void MyInitialization()
        {
            label3.Text = "Use only on data you are allowed to access. Every other tool is in the menus above. Opening a tool again returns to its active window; hold Ctrl when opening a tool for a separate session.";
            // the four tools most people start with; each opens the same window as its menu entry
            tile_CompareDb.Click += (sender, e) => OpenWindow(() => new DB_Comparison_Form());
            tile_CompareExcel.Click += (sender, e) => OpenWindow(() => new Excel_Excel_Comparison());
            tile_DeltaTracker.Click += (sender, e) => OpenWindow(() => new SPIDeltaChangeTrackerOptimised());
            tile_ExportExcel.Click += (sender, e) => OpenWindow(() => new SQL_DB_To_Excel());
        }

        private void OpenWindow<T>(Func<T> create, string mode = "", bool allowSeparateSession = true) where T : Form
        {
            try
            {
                bool separate = allowSeparateSession && (ModifierKeys & Keys.Control) == Keys.Control;
                windows.Open(this, create, mode, separate);
            }
            catch (Exception ex)
            {
                lbl_LoginStatus.Text = "Unable to open window";
                MessageBox.Show(this, Common_Data.RedactConnectionCredentials(ex.Message), "Unable to open window", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox1 abu = new AboutBox1())
            {
                abu.ShowInTaskbar = false;
                abu.ShowIcon = false;
                abu.ShowDialog(this);
            }
        }

        internal User_Settings ShowUpdatesSettings()
        {
            // The same key as File > Settings: never create a second settings window.
            User_Settings settings = windows.Open(this, () => new User_Settings());
            settings.ShowUpdates();
            return settings;
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { ShowUpdatesSettings(); }
            catch (Exception ex)
            {
                MessageBox.Show(this, Common_Data.RedactConnectionCredentials(ex.Message), "Unable to open Updates",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void compareExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new Excel_Excel_Comparison());
        }

        private void sPIAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new SPI_Analysis());
        }

        private void UnlockMainWindow()
        {
            try
            {
                ToggleMenuStripForms(true);
                // Developer mode comes only from an attached debugger or SPI_DATA_COMP_DEBUG=1.
                lbl_LoginStatus.Text = GlobalDebug.ISGlobalDebug(Environment.UserName, string.Empty)
                    ? "Developer mode"
                    : "Ready";
            }
            catch (Exception ex)
            {
                FormHelpers.ShowError(lbl_LoginStatus, ex);
            }
        }

        private void ToggleMenuStripForms(bool boolEnableStatus)
        {
            try
            {
                compareDBToolStripMenuItem.Enabled = boolEnableStatus;
                compareExcelToolStripMenuItem.Enabled = boolEnableStatus;
                customisedExcelToolStripMenuItem.Enabled = boolEnableStatus;
                sPIAnalysisToolStripMenuItem.Enabled = boolEnableStatus;
                sPISpecNoteCreationUtilityToolStripMenuItem.Enabled = boolEnableStatus;
                exportDBToExcelToolStripMenuItem.Enabled = boolEnableStatus;
                sQLDBToExcelToolStripMenuItem.Enabled = boolEnableStatus;
                sPIDeltaChangeTrackerOptimisedToolStripMenuItem.Enabled = boolEnableStatus;
                compareListTablesToolStripMenuItem.Enabled = boolEnableStatus;
                preMergingToolToolStripMenuItem.Enabled = boolEnableStatus;
                preMergingDuplicacyCheckToolStripMenuItem.Enabled = boolEnableStatus;
                userProfileToolStripMenuItem.Enabled = boolEnableStatus;
                configurationToolStripMenuItem.Enabled = boolEnableStatus;
                sPIDeltaChangeLegacyIDToolStripMenuItem.Enabled = boolEnableStatus;
                generateInsertScriptsToolStripMenuItem.Enabled = boolEnableStatus;
                executeScriptsFromExcelToolStripMenuItem.Enabled = boolEnableStatus;
                tableExtractionAfterDeltaToolStripMenuItem.Enabled = boolEnableStatus;
                mergeLogsExportToolStripMenuItem.Enabled = boolEnableStatus;
                sPIFKExtractionToolStripMenuItem.Enabled = boolEnableStatus;


            }
            catch (Exception ex)
            {
                lbl_LoginStatus.Text = ex.Message.ToString();
            }
        }

        private void sPISpecNoteCreationUtilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new SPI_Spec_Note_Creation_Utility());
        }

        private void compareDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new DB_Comparison_Form());
        }

        private void customisedExcelToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenWindow(() => new CustomisedExcel());
        }

        private void sQLDBToExcelToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenWindow(() => new SQL_DB_To_Excel());
        }

        private void compareListTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new Compare_Multiples_List_Tables_Forms());
        }

        private void preMergingToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new PreMergingTool_Form());
        }

        private void preMergingDuplicacyCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new Pre_Merging_Duplicacy_Form(), "check");
        }

        private void userProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new UserProfileForm(), allowSeparateSession: false);
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new Pre_Merging_Duplicacy_Form(true), "configuration", false);
        }

        private void generateInsertScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new Generate_IUD_Script_Form());
        }

        private void executeScriptsFromExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new Execute_Excel_Scripts_Form());
        }

        private void tableExtractionAfterDeltaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new Table_Extraction_After_Delta_Form());
        }

        private void mergeLogsExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new MergeLogsExport());
        }

        private void sPIDeltaChangeLegacyIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new SPIDeltaChangeLegacyID());
        }

        private void sPIDeltaChangeTrackerOptimisedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new SPIDeltaChangeTrackerOptimised());
        }

        private void userSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new User_Settings(), allowSeparateSession: false);
        }

        private void sPIFKExtractionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(() => new SPI_FK_Extraction());
        }
    }
}
