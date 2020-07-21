using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SPI_Data_Comp_Tool
{
    public partial class LTTS_Data_Comparison : Form
    {
        private Exception MyException;

        public LTTS_Data_Comparison()
        {
            InitializeComponent();
            MyInitialization();

            MyException = new Exception();

            this.Size = new Size(350, 350);
            gb_logincredentials.Location = new Point(16, 28);
            panel_control.Location = new Point(8, 180);

            if (!Debugger.IsAttached)
            {
                btn_Open_DB_Comparison.Enabled = false;
                btn_Open_DB_Comparison.Visible = false;

                btn_Excel_Comparison.Enabled = false;
                btn_Excel_Comparison.Visible = false;

                if (Environment.UserName.ToString() == "40009708" && Environment.MachineName.ToString() == "BRTSP00375")
                {
                    tb_username.Text = "LTTS";
                    tb_password.Text = "P@ssw0rd";
                }
                else
                {
                    tb_username.Text = Environment.UserName.ToString();
                }

            }
            if(Debugger.IsAttached)
            {
                tb_username.Text = "LTTS";
                tb_password.Text = "P@ssw0rd";
            }
        }

        private void MyInitialization()
        {
            label2.Text = "Automation Tool is developed by LTTS to compare data between two database as well as excel file  (for internal purpose only)";
            label3.Text = "Unauthorized use is strictly prohibited";
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 abu = new AboutBox1();
            abu.Show();
        }

        private void compareDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 frm_ora_sql_compare = new Form1();
            frm_ora_sql_compare.Show();
            
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
            Form1 db_comparison = new Form1();
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
                if(tb_username.Text == "LTTS" & tb_password.Text == "P@ssw0rd")
                {
                    ToggleMenuStripForms(true);
                    gb_logincredentials.Hide();
                    gb_LTTSDetails.Show();
                    panel_control.Location = new Point(8, 393);
                    this.Size = new Size(400, 530);
                    lbl_LoginStatus.Text = "Logged in successfully";
                }
                else
                {
                    ToggleMenuStripForms(false);
                    lbl_LoginStatus.Text = "Username or Password is incorrect";
                }
            }
            catch(Exception ex)
            {
                lbl_LoginStatus.Text = ex.Message.ToString();
            }
        }

        private void ToggleMenuStripForms(bool boolEnableStatus)
        {
            try
            {
                compareDBToolStripMenuItem.Enabled = boolEnableStatus;
                compareExcelToolStripMenuItem.Enabled = boolEnableStatus;
                sPIAnalysisToolStripMenuItem.Enabled = boolEnableStatus;
                sPISpecNoteCreationUtilityToolStripMenuItem.Enabled = boolEnableStatus;
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
    }
}
