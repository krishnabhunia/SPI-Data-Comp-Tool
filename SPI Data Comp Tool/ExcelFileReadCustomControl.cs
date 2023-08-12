using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class ExcelFileReadCustomControl : UserControl
    {
        private Exception customException;
        private OpenFileDialogData openFileDialogData;
        private DataTable dt;
        private DataSet ds;
        private Common_Data common_Data;

        private bool boolFileSelected;
        public Delegate GetExcelData;

        public ExcelFileReadCustomControl()
        {
            InitializeComponent();
            customException = new Exception();
            dt = new DataTable();
            openFileDialogData = new OpenFileDialogData();
            boolFileSelected = false;
            ds = new DataSet();
            common_Data = new Common_Data();
        }

        private void btn_Excel_Click(object sender, EventArgs e)
        {
            try
            {
                ds = LoadDataExcel();
                GetExcelData.DynamicInvoke(ds,openFileDialogData.strFileNameWithPath);
            }
            catch (Exception ex)
            {
                lbl_ExcelFileName.Text = ex.Message.ToString();
            }
        }

        /*
         try
            {

            }
            catch(Exception ex)
            {
                customException = ex;
            }

         */

        private DataSet LoadDataExcel()
        {
            try
            {
                openFileDialogData = openFileDialogData.openFileDialog();
                lbl_ExcelFileName.Text = openFileDialogData.strFileNameWithPath;
                if(openFileDialogData.boolFileSelected)
                {
                    ds = common_Data.LoadExcelFromDataReader(lbl_ExcelFileName.Text);
                }
                else
                {
                    ds.Tables.Clear();
                    ds.Clear();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ds;
        }

        //private OpenFileDialogData openFileDialog(string str_title = "Browse Excel Files")
        //{
        //    openFileDialogData = new OpenFileDialogData();
        //    OpenFileDialog openFileDialog1 = new OpenFileDialog
        //    {
        //        InitialDirectory = @"D:\",
        //        Title = str_title,

        //        CheckFileExists = true,
        //        CheckPathExists = true,

        //        DefaultExt = "xlsx",
        //        Filter = "xlsx files (*.xlsx)|*.xlsx",
        //        FilterIndex = 2,
        //        RestoreDirectory = true,

        //        ReadOnlyChecked = true,
        //        ShowReadOnly = true
        //    };

        //    if (openFileDialog1.ShowDialog() == DialogResult.OK)
        //    {
        //        openFileDialogData.strFileNameWithPath = openFileDialog1.FileName.ToString();
        //        openFileDialogData.FileName_Only = openFileDialog1.SafeFileName.ToString();
        //        boolFileSelected = true;
        //    }
        //    else
        //    {
        //        openFileDialogData.strFileNameWithPath = "File Not Selected";
        //        openFileDialogData.strFileNameWithPath = "File Not Selected";
        //        boolFileSelected = false;
        //    }
        //    return openFileDialogData;
        //}
    }
}
