using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace SPI_Data_Comp_Tool
{
    public partial class MainForm : Form
    {
        private Exception MyException;
        public MainForm()
        {
            InitializeComponent();
            MyInitialization();

            MyException = new Exception();

            btn_Open_DB_Comparison.Enabled = false;
            btn_Open_DB_Comparison.Visible = false;

            btn_Excel_Comparison.Enabled = false;
            btn_Excel_Comparison.Visible = false;

            // The login screen was removed: its user name and password were hard-coded in this
            // source, so it protected nothing. The main window now opens ready to use.
            UnlockMainWindow();
        }

        private void MyInitialization()
        {
            label2.Text = "Compares two databases (Oracle or SQL Server) or two Excel files and reports the differences, for SPI data analysis.";
            label3.Text = "Use only on data you are allowed to access.";
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 abu = new AboutBox1();
            abu.ShowInTaskbar = false;
            abu.ShowIcon = false;
            abu.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_Exit_Logout_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void compareExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Excel_Excel_Comparison eec = new Excel_Excel_Comparison();
            eec.Show();
        }

        private void btn_Open_DB_Comparison_Click(object sender, EventArgs e)
        {
            DB_Comparison_Form db_comparison = new DB_Comparison_Form();
            db_comparison.Show();
        }

        private void btn_Excel_Comparison_Click(object sender, EventArgs e)
        {
            Excel_Excel_Comparison eec = new Excel_Excel_Comparison();
            eec.Show();
        }

        private void sPIAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SPI_Analysis sPI_Analysis = new SPI_Analysis();
                sPI_Analysis.Show();
            }
            catch (Exception ex)
            {
                MyException = ex;
            }
        }

        private void UnlockMainWindow()
        {
            try
            {
                ToggleMenuStripForms(true);
                gb_logincredentials.Hide();
                gb_AppDetails.Show();
                panel_control.Location = new Point(8, 393);
                this.Size = new Size(400, 530);
                // Developer mode comes only from an attached debugger or SPI_DATA_COMP_DEBUG=1.
                lbl_LoginStatus.Text = GlobalDebug.ISGlobalDebug(Environment.UserName, string.Empty)
                    ? "Developer mode"
                    : "Ready";
            }
            catch (Exception ex)
            {
                lbl_LoginStatus.Text = ex.Message.ToString();
            }
        }

        // Kept only because the designer still wires the (now hidden) Login button to it.
        private void btn_Login_Click(object sender, EventArgs e)
        {
            UnlockMainWindow();
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
                sPIDeltaChangeTrackerToolStripMenuItem.Enabled = boolEnableStatus;
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


                //hidden explicitly as other mode of forms are available

            }
            catch (Exception ex)
            {
                lbl_LoginStatus.Text = ex.Message.ToString();
            }
        }

        private void sPISpecNoteCreationUtilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SPI_Spec_Note_Creation_Utility sPI_Spec_Note_Creation_Utility_Form = new SPI_Spec_Note_Creation_Utility();
                sPI_Spec_Note_Creation_Utility_Form.Show();
            }
            catch (Exception ex)
            {
                MyException = ex;
            }
        }

        private void compareDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DB_Comparison_Form frm_ora_sql_compare = new DB_Comparison_Form();
            frm_ora_sql_compare.Show();
        }

        private void customisedExcelToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            CustomisedExcel customisedExcel = new CustomisedExcel();
            customisedExcel.Show();
        }

        private void sQLDBToExcelToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SQL_DB_To_Excel sQL_DB_To_Excel = new SQL_DB_To_Excel();
            sQL_DB_To_Excel.Show();
        }

        private void sPIDeltaChangeTrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SPIDeltaChangeTracker sPIDeltaChangeTracker = new SPIDeltaChangeTracker();
            sPIDeltaChangeTracker.Show();
        }

        private void compareListTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Compare_Multiples_List_Tables_Forms compare_Multiples_List_Tables_Forms = new Compare_Multiples_List_Tables_Forms();
            compare_Multiples_List_Tables_Forms.Show();
        }

        private void preMergingToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreMergingTool_Form preMergingTool_Form = new PreMergingTool_Form();
            preMergingTool_Form.Show();
        }

        private void preMergingDuplicacyCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pre_Merging_Duplicacy_Form pre_Merging_Duplicacy_Form = new Pre_Merging_Duplicacy_Form();
            pre_Merging_Duplicacy_Form.Show();
        }

        private void userProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserProfileForm userProfileForm = new UserProfileForm();
            userProfileForm.Show();
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //This calls the pre merging duplicacy form in configuration mode
            Pre_Merging_Duplicacy_Form pre_Merging_Duplicacy_Form = new Pre_Merging_Duplicacy_Form(true);// boolConfigurationMode = true
            pre_Merging_Duplicacy_Form.Show();
        }

        private void generateInsertScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Generate_IUD_Script_Form generate_IUD_Script_Form = new Generate_IUD_Script_Form();
            generate_IUD_Script_Form.Show();
        }

        private void executeScriptsFromExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Execute_Excel_Scripts_Form execute_Excel_Scripts_Form = new Execute_Excel_Scripts_Form();
            execute_Excel_Scripts_Form.Show();
        }

        private void tableExtractionAfterDeltaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Table_Extraction_After_Delta_Form table_Extraction_After_Delta_Form = new Table_Extraction_After_Delta_Form();
            table_Extraction_After_Delta_Form.Show();
        }

        private void mergeLogsExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MergeLogsExport mergeLogsExport = new MergeLogsExport();
            mergeLogsExport.Show();
        }

        private void sPIDeltaChangeLegacyIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SPIDeltaChangeLegacyID sPIDeltaChangeLegacyID = new SPIDeltaChangeLegacyID();
            sPIDeltaChangeLegacyID.Show();
        }

        private void sPIDeltaChangeTrackerOptimisedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SPIDeltaChangeTrackerOptimised spIDeltaChangeTrackerOptimised = new SPIDeltaChangeTrackerOptimised();
            spIDeltaChangeTrackerOptimised.Show();
        }

        private void userSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            User_Settings user_Settings = new User_Settings();
            user_Settings.Show();
        }

        private void sPIFKExtractionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SPI_FK_Extraction sPI_FK_Extraction = new SPI_FK_Extraction();
            sPI_FK_Extraction.Show();
        }
    }
}
