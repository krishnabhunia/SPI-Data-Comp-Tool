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

        public Delegate GetExcelData;

        public ExcelFileReadCustomControl()
        {
            InitializeComponent();
            customException = new Exception();
            dt = new DataTable();
            openFileDialogData = new OpenFileDialogData();
            ds = new DataSet();
            common_Data = new Common_Data();
        }

        private void btn_Excel_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet candidate = LoadDataExcel();
                if (candidate == null) return;
                if (!FormValidation.Check(this, () => FormValidation.Require(GetExcelData != null,
                    btn_Excel, "This workbook control has no destination."), message => lbl_ExcelFileName.Text = message)) return;
                GetExcelData.DynamicInvoke(candidate, openFileDialogData.strFileNameWithPath);
                ds = candidate;
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
                if(openFileDialogData.boolFileSelected)
                {
                    string path = null;
                    if (!FormValidation.Check(this, () => path = FormValidation.ReadableFile(
                        openFileDialogData.strFileNameWithPath, btn_Excel, "Workbook", ".xlsx", ".xlsm"),
                        message => lbl_ExcelFileName.Text = message)) return null;
                    DataSet candidate = common_Data.LoadExcelFromDataReader(path);
                    if (!FormValidation.Check(this, () => ValidateWorkbook(candidate, btn_Excel),
                        message => lbl_ExcelFileName.Text = message)) { candidate?.Dispose(); return null; }
                    lbl_ExcelFileName.Text = path;
                    return candidate;
                }
                else
                {
                    return null; // Cancelling must not discard the previously loaded workbook.
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static void ValidateWorkbook(DataSet candidate, Control field)
        {
            FormValidation.Require(candidate != null && candidate.Tables.Count > 0, field,
                "The workbook contains no readable worksheets.");
            FormValidation.Table(candidate.Tables[0], field, "First worksheet");
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
