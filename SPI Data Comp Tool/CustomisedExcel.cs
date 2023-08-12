using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class CustomisedExcel : Form
    {
        string str_CustomisedExcelName;
        private DataSet dsCustomisedExcel;
        private OpenFileDialogData openFileDialogData;

        private Common_Data common_Data;
        public CustomisedExcel()
        {
            InitializeComponent();
            str_CustomisedExcelName = string.Empty;


            dsCustomisedExcel = new DataSet();
            common_Data = new Common_Data();
            openFileDialogData = new OpenFileDialogData();
        }

        private void btn_ReadExcel_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayMessage("Pls select a excel file to import");
                openFileDialogData = openFileDialogData.openFileDialog("Select Rule File");
                str_CustomisedExcelName = openFileDialogData.strFileNameWithPath;

                if (openFileDialogData.boolFileSelected)
                {
                    dsCustomisedExcel.Clear();
                    dsCustomisedExcel.Tables.Clear();
                    dsCustomisedExcel = common_Data.LoadExcelFromDataReader(str_CustomisedExcelName, "Rules for comparison");
                    FillCheckBoxList(dsCustomisedExcel);
                    DisplayMessage(string.Format("Rule file {0} imported successfully", str_CustomisedExcelName));
                }
                else
                {
                    DisplayMessage(string.Format("Rule file {0} failed to import", str_CustomisedExcelName));
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void FillCheckBoxList(DataSet ds)
        {
            try
            {
                checkedListBox1.Items.Clear();
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            checkedListBox1.Items.Add(dc.ColumnName.ToString());
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
        private void DisplayMessage(string str_Msg)
        {
            try
            {
                lbl_Status.ForeColor = Color.Black;
                lbl_Status.Text = str_Msg;
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
            catch (Exception ex1)
            {
                lbl_Status.Text = ex1.Message.ToString();
            }
        }

        private void btn_CreateExcel_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = dsCustomisedExcel.Copy();

                foreach (DataTable dataTable in ds.Tables.Cast<DataTable>().ToArray())
                {
                    foreach (DataColumn dataColumn in dataTable.Columns.Cast<DataColumn>().ToArray())
                    {
                        if ((checkedListBox1.Items.Contains(dataColumn.ColumnName.ToString())) && (!(checkedListBox1.GetItemChecked(checkedListBox1.FindStringExact(dataColumn.ColumnName.ToString())))))
                        {
                            dataTable.Columns.Remove(dataColumn);
                        }
                    }
                    dataTable.AcceptChanges();
                }

                DisplayMessage("Saving File, Pls Wait !!!");
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    CheckFileExists = false,
                    Filter = "xlsx files (*.xlsx)|*.xlsx",
                    Title = "Save Excel File",
                    FileName = $"Raw Input File {common_Data.AppendDateInOutputFileName(DateTime.Now)}",
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    #region epplus code assembly

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = pck.Workbook.Worksheets.Add("InputSheet");
                        worksheet.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);

                        worksheet.Cells.AutoFitColumns(22,50);
                        pck.SaveAs(new System.IO.FileInfo(saveFileDialog.FileName));
                        DisplayMessage("File Saved Successfully !!! Path : " + saveFileDialog.FileName);
                    }
                    #endregion

                    DisplayMessage("Opening File :" + saveFileDialog.FileName);
                    Process.Start(saveFileDialog.FileName);
                }
                else
                {
                    DisplayMessage("File Not Saved");
                }


            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_SelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, cb_SelectAll.Checked);
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }
}
