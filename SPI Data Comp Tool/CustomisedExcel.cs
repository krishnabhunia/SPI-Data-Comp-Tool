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
    public partial class CustomisedExcel : WorkspacePageForm
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
            BuildSteps();
        }

        private StepFlow steps;   // controlled steps, see BuildSteps
        private bool created;

        /// <summary>Controlled steps: Workbook, Columns, Create. A step unlocks when the one before is done.</summary>
        private void BuildSteps()
        {
            steps = new StepFlow(this, StepStrip.Attach(this))
                .Add("Workbook", "Read the Excel workbook",
                    () => dsCustomisedExcel != null && dsCustomisedExcel.Tables.Count > 0 && checkedListBox1.Items.Count > 0, btn_ReadExcel)
                .Add("Columns", "Tick the columns to keep", () => checkedListBox1.CheckedItems.Count > 0, checkedListBox1, cb_SelectAll)
                .Add("Create", "Create the customised Excel", () => created, btn_CreateExcel);
            steps.Refresh();
        }

        private void btn_ReadExcel_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayMessage("Pls select a excel file to import");
                openFileDialogData = openFileDialogData.openFileDialog("Select Rule File");
                if (openFileDialogData.boolFileSelected)
                {
                    string path = null;
                    if (!FormValidation.Check(this, () => path = FormValidation.ReadableFile(openFileDialogData.strFileNameWithPath,
                        btn_ReadExcel, "Workbook", ".xlsx", ".xlsm"), DisplayMessage)) return;
                    DataSet candidate = common_Data.LoadExcelFromDataReader(path, "Rules for comparison");
                    if (!FormValidation.Check(this, () => FormValidation.ValidateWorkbook(candidate, btn_ReadExcel), DisplayMessage))
                    { candidate?.Dispose(); return; }
                    DataSet previous = dsCustomisedExcel;
                    dsCustomisedExcel = candidate;
                    previous?.Dispose();
                    str_CustomisedExcelName = path;
                    created = false;
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
                    // This workflow exports the first worksheet; do not offer columns from other sheets.
                        foreach (DataColumn dc in ds.Tables[0].Columns)
                        {
                            checkedListBox1.Items.Add(dc.ColumnName.ToString());
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
                if (!FormValidation.Check(this, ValidateExportInput, DisplayMessage)) return;
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
                    if (!FormValidation.Check(this, () =>
                    {
                        string output = FormValidation.OutputFile(saveFileDialog.FileName, btn_CreateExcel, "Output workbook", ".xlsx");
                        FormValidation.Require(!string.Equals(output, System.IO.Path.GetFullPath(str_CustomisedExcelName),
                            StringComparison.OrdinalIgnoreCase), btn_CreateExcel, "Choose an output file different from the input workbook.");
                    }, DisplayMessage)) return;
                    #region epplus code assembly

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = pck.Workbook.Worksheets.Add("InputSheet");
                        worksheet.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);

                        worksheet.Cells.AutoFitColumns(22,50);
                        FormValidation.WriteAtomically(saveFileDialog.FileName, path => pck.SaveAs(new System.IO.FileInfo(path)));
                        DisplayMessage("File Saved Successfully !!! Path : " + saveFileDialog.FileName);
                        created = true;
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

        internal void ValidateExportInput()
        {
            FormValidation.ValidateWorkbook(dsCustomisedExcel, btn_ReadExcel);
            FormValidation.Require(checkedListBox1.CheckedItems.Count > 0, checkedListBox1, "Select at least one column to export.");
            FormValidation.Columns(dsCustomisedExcel.Tables[0], checkedListBox1.CheckedItems.Cast<string>(), checkedListBox1, "Export columns");
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
