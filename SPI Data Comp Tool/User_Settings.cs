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
    public partial class User_Settings : Form
    {
        public User_Settings()
        {
            InitializeComponent();

            try
            {
                rb_EPPlus.Checked = Properties.Settings.Default.EPPlus_Excel_Save;
                rb_XLSave.Checked = Properties.Settings.Default.XL_Excel_Save;
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_EPPlus_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if(rb_EPPlus.Checked)
                {
                    ExcelChangeSave("EPPlus mode selected for saving excel output");
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_XLSave_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if(rb_XLSave.Checked)
                {
                    ExcelChangeSave("XL mode selected for saving excel output");
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void ExcelChangeSave(string strMsg)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, strMsg);
                Properties.Settings.Default.EPPlus_Excel_Save = rb_EPPlus.Checked;
                Properties.Settings.Default.XL_Excel_Save = rb_XLSave.Checked;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
    }
}
