using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Identity.Client;

namespace SPI_Data_Comp_Tool
{
    public partial class LTTS_Data_Comparison : Form
    {
        private Exception MyException;
        private Button btn_EntraLogin;

        public LTTS_Data_Comparison()
        {
            InitializeComponent();
            MyInitialization();

            MyException = new Exception();

            this.Size = new Size(400, 340);
            gb_logincredentials.Location = new Point(16, 28);
            gb_logincredentials.Size = new Size(350, 155);
            gb_logincredentials.Text = "Choose your sign-in method";
            panel_control.Location = new Point(16, 190);

            // The former shared user name and password must never be an entry point.
            tb_username.Visible = false;
            tb_password.Visible = false;
            tb_username.Clear();
            tb_password.Clear();
            label1.Visible = false;
            label4.Visible = false;

            btn_Login.Text = "A  Sign in with Windows / Active Directory";
            btn_Login.SetBounds(15, 32, 320, 42);
            btn_EntraLogin = new Button
            {
                Name = "btn_EntraLogin",
                Text = "B  Sign in with Microsoft Entra ID",
                UseVisualStyleBackColor = true
            };
            btn_EntraLogin.SetBounds(15, 88, 320, 42);
            btn_EntraLogin.Click += btn_EntraLogin_Click;
            gb_logincredentials.Controls.Add(btn_EntraLogin);

            lbl_LoginStatus.AutoSize = false;
            lbl_LoginStatus.SetBounds(5, 34, 335, 44);
            lbl_LoginStatus.Text = "Select a sign-in method to continue.";
            ToggleMenuStripForms(false);

            btn_Open_DB_Comparison.Enabled = false;
            btn_Open_DB_Comparison.Visible = false;

            btn_Excel_Comparison.Enabled = false;
            btn_Excel_Comparison.Visible = false;

            btn_Login.Select();
        }

        private void MyInitialization()
        {
            label2.Text = "Automation Tool is developed by LTTS for analysis and comparison of two databases or excel files. (For Internal Purpose Only)";
            label3.Text = "Unauthorized use is strictly prohibited";
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

        private void btn_Login_Click(object sender, EventArgs e)
        {
            try
            {
                ApplySignInResult(AuthenticationService.SignInWithWindows());
            }
            catch (Exception)
            {
                ApplySignInResult(SignInResult.Denied("Unable to verify the Windows account. Contact your administrator."));
            }
        }

        private async void btn_EntraLogin_Click(object sender, EventArgs e)
        {
            btn_Login.Enabled = false;
            btn_EntraLogin.Enabled = false;
            lbl_LoginStatus.Text = "Waiting for Microsoft Entra ID sign-in...";
            try
            {
                using (var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(5)))
                {
                    ApplySignInResult(await AuthenticationService.SignInWithEntraAsync(this, timeout.Token));
                }
            }
            catch (OperationCanceledException)
            {
                ApplySignInResult(SignInResult.Denied("Entra ID sign-in was cancelled or timed out."));
            }
            catch (MsalException)
            {
                ApplySignInResult(SignInResult.Denied("Entra ID sign-in failed. Contact your administrator."));
            }
            catch (Exception)
            {
                ApplySignInResult(SignInResult.Denied("Unable to complete Entra ID sign-in. Contact your administrator."));
            }
            finally
            {
                btn_Login.Enabled = gb_logincredentials.Visible;
                btn_EntraLogin.Enabled = gb_logincredentials.Visible;
            }
        }

        private void ApplySignInResult(SignInResult result)
        {
            lbl_LoginStatus.ForeColor = result.Authorized ? Color.DarkGreen : Color.DarkRed;
            lbl_LoginStatus.Text = result.Message;
            if (!result.Authorized)
            {
                ToggleMenuStripForms(false);
                return;
            }

            ToggleMenuStripForms(true);
            gb_logincredentials.Hide();
            gb_LTTSDetails.Show();
            panel_control.Location = new Point(8, 393);
            this.Size = new Size(400, 530);
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
                userSettingsToolStripMenuItem.Enabled = boolEnableStatus;
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
