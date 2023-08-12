using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class Form_Rules : Form
    {
        private string str_RuleFileName;

        private Common_Data Common_Data;
        //private CheckRules checkRules;
        private OpenFileDialogData openFileDialogData;// = new OpenFileDialogData();
        public DataSet dsRules { get; private set; }
        //private DataTable dtRules;

        public Delegate SetDsRules;
        public Form_Rules()
        {
            InitializeComponent();
            Common_Data = new Common_Data();
            openFileDialogData = new OpenFileDialogData();

            dsRules = new DataSet();
            //checkRules = new CheckRules();

            try
            {
                str_RuleFileName = Properties.Settings.Default.RuleFilePath.ToString();
                if (str_RuleFileName != null && str_RuleFileName != "" && str_RuleFileName != string.Empty)
                {
                    //dsRules = Common_Data.LoadExcelFromDataReader(str_RuleFileName, "Rules for comparison");
                    dsRules = Common_Data.GetDatasetRules(str_RuleFileName);
                    DisplayMessage($"Rule file {str_RuleFileName} has already been set.", Color.Red);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void btn_ImportRules_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayMessage("Pls select a excel file to import", Color.Red);
                openFileDialogData = openFileDialogData.openFileDialog("Select Rule File");
                str_RuleFileName = openFileDialogData.strFileNameWithPath;

                if(openFileDialogData.boolFileSelected)
                {
                    //dsRules = Common_Data.LoadExcelFromDataReader(str_RuleFileName, "Rules for comparison");
                    dsRules = Common_Data.GetDatasetRules(str_RuleFileName);
                    //dsRules = checkRules.GetDatasetRules(str_RuleFileName);
                    //SetDsRules.DynamicInvoke(dsRules, dsRules.Tables[0].TableName.ToString());

                    Properties.Settings.Default.RuleFilePath = openFileDialogData.strFileNameWithPath;
                    Properties.Settings.Default.Save();

                    DisplayMessage(string.Format("Rule file {0} imported successfully, You can now exit this form", str_RuleFileName), Color.Black);
                }
                else
                {
                    DisplayMessage(string.Format("Rule file {0} failed to import", str_RuleFileName), Color.Red);
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        public DataSet GetDataSet()
        {
            try
            {
                return dsRules;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void DisplayMessage(string str_Msg, Color color)
        {
            try
            {
                lbl_Status.Text = str_Msg;
                lbl_Status.ForeColor = color;
                lbl_Status.Update();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void DisplayError(Exception ex)
        {
            try
            {
                lbl_Status.Text = ex.Message.ToString();
                lbl_Status.ForeColor = Color.Red;
            }
            catch(Exception ex1)
            {
                lbl_Status.Text = ex1.Message.ToString();
            }
        }

        private void Form_Rules_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }
}
