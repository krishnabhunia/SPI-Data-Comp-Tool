using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Diagnostics;
using System.Threading;
using _Excel = Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using ClosedXML.Excel;
using System.IO;
using ExcelDataReader;

namespace SPI_Data_Comp_Tool
{
    public partial class Excel_Excel_Comparison : Form
    {
        private bool boolStatus;
        private bool boolCellValueIntType;

        private string firstExcel;
        private string secondExcel;
        private string str_constr;
        private string str_wsName;
        private string strQuery;
        private string str_Reference_ID;
        private string fileNameWithPath;
        private string str_RowFilter;
        private string cellValue;

        private OpenFileDialogData openFileDialogData;// = new OpenFileDialogData();
        private OleDbConnection Econ;
        private DataSet ds;
        private _Excel.Application excelApp;// = new _Excel.Application();
        private _Excel.Workbook excelWorkbook;// = excelApp.Workbooks.Open(str_file_to_load_path, 0, true, 5, "", "", true, _Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        _Excel.Worksheet excelWorksheet;// = (_Excel.Worksheet)excelWorkbook.Sheets[1];

        private OleDbCommand Ecom;// = new OleDbCommand(Query, Econ);
        private OleDbDataAdapter oda;// = new OleDbDataAdapter(Query, Econ);
        private DataTable dt_firstTable;
        private DataTable dt_secondTable;
        private DataTable dt_returnDataTable;
        private OpenFileDialogData ofdd;
        private DataTable dt_firstTableOnly;
        private DataTable dt_secondTableOnly;

        private DataTable dt_unmatched_Capture;
        private DataTable dt_summarised_difference;

        private DataView dv_secondTableDataView;
        DateTime dateTime1;// = new DateTime();

        private bool boolValidate;
        private bool boolFileSelected;

        private Exception myException;
        private TimeSpan timerDuration;

        private DataTable dt_transpose;

        private Common_Data common_data;

        private XLWorkbook wb;

        public Excel_Excel_Comparison()
        {
            InitializeComponent();

            boolStatus = true;

            firstExcel = string.Empty;
            secondExcel = string.Empty;
            str_constr = string.Empty;
            str_wsName = string.Empty;
            strQuery = string.Empty;
            str_Reference_ID = string.Empty;
            fileNameWithPath = string.Empty;
            str_RowFilter = string.Empty;

            Econ = new OleDbConnection();
            ds = new DataSet();

            excelApp = new _Excel.Application();
            ofdd = new OpenFileDialogData();
            llbl_filePath.Text = Environment.SpecialFolder.Desktop.ToString();
            boolValidate = false;
            dateTime1 = new DateTime();

            myException = new Exception();

            dt_transpose = new DataTable();

            llbl_filePath.Text = @"D:\";

            dt_firstTableOnly = new DataTable();
            dt_secondTableOnly = new DataTable();

            common_data = new Common_Data();
            //wb = new XLWorkbook();

            if (Debugger.IsAttached)
            {
                btn_SecondExcelSheet.Visible = true;
                btn_FirstExcelSheet.Visible = true;
                btnLoad.Visible = true;

                dataGridView1.Visible = true;
            }
            else
            {
                btn_ProcessBy_MultiThreading.Enabled = false;
                btn_ProcessBy_MultiThreading.Visible = false;
            }
        }

        private void btn_FirstExcelSheet_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog();
                firstExcel = openFileDialogData.FileName_Only;
                lbl_FirstExcelFileName.Text = firstExcel;
                boolStatus = checkDuplicateSameFile(firstExcel, secondExcel);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private OpenFileDialogData openFileDialog(string str_title = "Browse Excel Files")
        {

            openFileDialogData = new OpenFileDialogData();

            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = str_title,

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "xlsx",
                Filter = "xlsx files (*.xlsx)|*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                openFileDialogData.FileName_Only = openFileDialog1.FileName.ToString();
                boolFileSelected = true;
            }
            else
            {
                openFileDialogData.FileName_Only = "File Not Selected";
                boolFileSelected = false;
            }
            return openFileDialogData;
        }

        private void btn_SecondExcelSheet_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog();
                secondExcel = openFileDialogData.FileName_Only;
                lbl_SecondExcelFileName.Text = secondExcel;
                boolStatus = checkDuplicateSameFile(firstExcel, secondExcel);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private bool checkDuplicateSameFile(string filename1, string filename2)
        {
            try
            {
                if (filename1 == filename2)
                {
                    DialogResult dialogResult = MessageBox.Show("Both Files are same, Do you still want to continue", "Same File", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.Yes)
                    {
                        boolStatus = true;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        boolStatus = false;
                    }
                }
                else
                {
                    boolStatus = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            return boolStatus;
        }

        private DataSet LoadExcelFromDataReader(string str_file_to_load_path, string dataTableName)
        {
            DataSet dsResult = new DataSet();
            try
            {
                FileStream stream = File.Open(str_file_to_load_path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                dsResult = excelReader.AsDataSet();

                string str_columnName = string.Empty;

                foreach (DataColumn dc in dsResult.Tables[0].Columns)
                {
                    str_columnName = dsResult.Tables[0].Rows[0][dc.ColumnName.ToString()].ToString();
                    dc.Caption = str_columnName;
                    dc.ColumnName = str_columnName;
                    //dsResult.Tables[0].Columns[dc.ToString()].ColumnName = str_columnName;
                }
                dsResult.Tables[0].Rows.RemoveAt(0);
                dsResult.Tables[0].TableName = dataTableName;
                dsResult.AcceptChanges();

                excelReader.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            return dsResult;
        }

        private DataSet LoadExcel(string str_file_to_load_path, string dataTableName)
        {
            try
            {
                str_constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", str_file_to_load_path);
                Econ = new OleDbConnection(str_constr);

                #region add select query to select the data from this Excel sheet and open this Excel connection

                excelApp = new _Excel.Application();
                excelWorkbook = excelApp.Workbooks.Open(str_file_to_load_path, 0, true, 5, "", "", true, _Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                excelWorksheet = (_Excel.Worksheet)excelWorkbook.Sheets[1];
                str_wsName = excelWorksheet.Name;

                strQuery = string.Format("Select * FROM [{0}]", str_wsName + "$");

                Ecom = new OleDbCommand(strQuery, Econ);
                Econ.Open();

                #endregion

                #region Create one dataset and fill this data set with this selected items

                oda = new OleDbDataAdapter(strQuery, Econ);
                Econ.Close();
                oda.Fill(ds, dataTableName);

                #endregion
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            return ds;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                changeEnabledStatus(false);
                lbl_LoadStatus.Text = "Loading started";
                ds = new DataSet();
                ds = LoadExcel(firstExcel, "firstExcel");
                ds = LoadExcel(secondExcel, "secondExcel");

                dt_firstTable = getCommonColumns(ds.Tables[0], ds.Tables[1], lb_IgnoreTable1);
                dt_secondTable = getCommonColumns(ds.Tables[1], ds.Tables[0], lb_IgnoreTable2);

                LoadCheckListBox(clb_reference_ID, dt_firstTable);
                LoadCheckListBox(clb_firstExcel, dt_firstTable);
                LoadCheckListBox(clb_secondExcel, dt_secondTable);

                tb_fileName.Text = excelWorksheet.Name + "_output";

                lbl_LoadStatus.Text = "Loaded successfully";
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            finally
            {
                changeEnabledStatus(true);
            }
        }

        private DataTable getCommonColumns(DataTable dt, DataTable dt_check, ListBox lb)
        {
            try
            {
                dt_returnDataTable = dt.Copy();
                lb.Items.Clear();
                DataColumnCollection dcc = dt_check.Columns;

                foreach (DataColumn dc in dt.Columns)
                {
                    if (!(dcc.Contains(dc.ColumnName.ToString())))
                    {
                        dt_returnDataTable.Columns.Remove(dc.ColumnName.ToString());
                        lb.Items.Add(dc.ColumnName.ToString());
                    }

                }

                //foreach (DataColumn dc in ds.Tables[1].Columns)
                //{
                //    if (!(dcc1.Contains(dc.ColumnName.ToString())))
                //    {
                //        dt_secondTable.Columns.Remove(dc.ColumnName.ToString());
                //        lb_IgnoreTable2.Items.Add(dc.ColumnName.ToString());
                //    }

                //}

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            return dt_returnDataTable;
        }

        private void LoadCheckListBox(CheckedListBox cbl, DataTable dt)
        {
            try
            {
                //DataColumnCollection[] dcc = new [] dt.Columns;
                //cbl.Items.AddRange(dcc);
                cbl.Items.Clear();
                foreach (DataColumn dc in dt.Columns)
                {
                    cbl.Items.Add(dc.ColumnName.ToString());
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private void clb_reference_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //setData();
                //clb_reference_ID.SelectionMode = SelectionMode.None;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private string generateRowFilterString(DataRow dr)
        {
            try
            {
                str_Reference_ID = string.Empty;
                foreach (var a in clb_reference_ID.CheckedItems)
                {
                    str_Reference_ID = str_Reference_ID + a.ToString() + " = '" + dr[a.ToString()] + "' AND ";
                }
                str_Reference_ID = str_Reference_ID.Substring(0, str_Reference_ID.Length - 4);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            return str_Reference_ID;
        }

        private void btn_Generate_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                llbl_filePath.Text = ofdd.folderBrowser(llbl_filePath.Text);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private void toggleCheckBoxCheckedState()
        {
            int countUncheck = 0;
            int countCheck = 0;
            try
            {
                foreach (var item in clb_firstExcel.Items)
                {
                    if (clb_firstExcel.GetItemChecked(clb_firstExcel.FindStringExact(item.ToString())))
                    {
                        countCheck++;
                    }
                    else
                    {
                        countUncheck++;
                    }
                }

                if (countCheck > 0 && countUncheck > 0)
                {
                    cb_select_first_clb.CheckState = CheckState.Indeterminate;
                }
                else if (countCheck > 0)
                {
                    cb_select_first_clb.CheckState = CheckState.Checked;
                }
                else if (countUncheck > 0)
                {
                    cb_select_first_clb.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }

        private void cb_select_first_clb_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_select_first_clb.CheckState != CheckState.Indeterminate)
                {
                    checkAllNoneToggle(clb_firstExcel, cb_select_first_clb.Checked);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private void checkAllNoneToggle(CheckedListBox clb, bool boolCheckState)
        {
            try
            {
                for (int i = 0; i < clb.Items.Count; i++)
                {
                    clb.SetItemChecked(i, boolCheckState);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private void btn_SelectLoad_Click(object sender, EventArgs e)
        {
            try
            {
                //bool boolLoopWhileStatus = true;

                //resetControlsToDefaultValue(1);

                resetControlsToDefaultValue(10);

                changeEnabledStatus(false);
                openFileDialog("Select First Excel File");
                firstExcel = openFileDialogData.FileName_Only;
                lbl_FirstExcelFileName.Text = firstExcel;

                // ============================================================================
                if (boolFileSelected)
                {
                    do
                    {
                        openFileDialog("Select Second Excel File");
                        secondExcel = openFileDialogData.FileName_Only;
                        lbl_SecondExcelFileName.Text = secondExcel;
                        boolStatus = checkDuplicateSameFile(firstExcel, secondExcel);
                        //boolLoopWhileStatus = !(boolStatus);

                    } while (!boolStatus);
                }

                // ============================================================================

                if (boolStatus & boolFileSelected)
                {
                    lbl_LoadStatus.Text = "Loading started";
                    ds = new DataSet();

                    DataTable dt = new DataTable();

                    dt = LoadExcelFromDataReader(firstExcel, "firstExcel").Tables[0];
                    ds.Tables.Add(dt.Copy());
                    dt = LoadExcelFromDataReader(secondExcel, "secondExcel").Tables[0];
                    ds.Tables.Add(dt.Copy());

                    //ds = LoadExcel(firstExcel, "firstExcel");
                    //ds = LoadExcel(secondExcel, "secondExcel");

                    dt_firstTable = getCommonColumns(ds.Tables[0], ds.Tables[1], lb_IgnoreTable1);
                    dt_secondTable = getCommonColumns(ds.Tables[1], ds.Tables[0], lb_IgnoreTable2);

                    LoadCheckListBox(clb_reference_ID, dt_firstTable);
                    LoadCheckListBox(clb_firstExcel, dt_firstTable);
                    LoadCheckListBox(clb_secondExcel, dt_secondTable);

                    //tb_fileName.Text = excelWorksheet.Name + "_output";
                    int index1 = firstExcel.LastIndexOf('\\') + 1;
                    int index2 = firstExcel.LastIndexOf('.');

                    tb_fileName.Text = firstExcel.Substring(index1, index2 - index1) + "_output";

                    if (clb_firstExcel.Items.Count > 0)
                    {
                        cb_select_first_clb.Checked = true;
                    }

                    lbl_Parameter.Text = "Rows : " + ds.Tables[0].Rows.Count + " and Columns : " + ds.Tables[0].Columns.Count
                    + ", Total comparison : " + ds.Tables[0].Rows.Count * ds.Tables[0].Columns.Count;

                    lbl_LoadStatus.Text = "Loaded successfully";
                    //boolLoopWhileStatus = false;
                }
                else
                {
                    lbl_LoadStatus.Text = "One of the two files OR both files not selected";
                }



            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            finally
            {
                changeEnabledStatus(true);
            }
        }

        private void btn_Validate_Click(object sender, EventArgs e)
        {
            try
            {
                changeEnabledStatus(false);
                lbl_Status.ResetText();
                if (clb_reference_ID.CheckedItems.Count >= 1)
                {
                    boolValidate = true;
                }
                else
                {
                    boolValidate = false;
                    lbl_Status.Text = "Pls select atleast one reference key";
                }

                if (boolValidate)
                {

                    CheckedListBox.CheckedItemCollection cic_referencekey = clb_reference_ID.CheckedItems;
                    CheckedListBox.CheckedItemCollection cic_firstExcel = clb_firstExcel.CheckedItems;

                    foreach (var a in cic_referencekey)
                    {
                        clb_firstExcel.SetItemChecked(clb_firstExcel.FindStringExact(a.ToString()), false);
                    }

                    if (boolValidate && clb_firstExcel.CheckedItems.Count >= 1)
                    {
                        boolValidate = true;
                    }
                    else
                    {
                        boolValidate = false;
                    }
                }

                changeEnabledStatus(true);

                if (boolValidate)
                {
                    btn_Compare.Enabled = true;
                    btn_Validate.Enabled = false;
                }
                else
                {
                    lbl_Status.Text = "Pls select atleast one column to check mismatch";
                    btn_Compare.Enabled = false;
                    btn_Validate.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            finally
            {
                //changeEnabledStatus(true);
            }
        }

        private void changeEnabledStatus(bool boolEnabledStatus)
        {
            try
            {
                btn_FirstExcelSheet.Enabled = boolEnabledStatus;
                btn_SecondExcelSheet.Enabled = boolEnabledStatus;
                btn_SelectLoad.Enabled = boolEnabledStatus;
                btnLoad.Enabled = boolEnabledStatus;

                btn_Validate.Enabled = boolEnabledStatus;
                btn_Generate.Enabled = boolEnabledStatus;

                cb_select_first_clb.Enabled = boolEnabledStatus;

                if (boolEnabledStatus)
                {
                    clb_reference_ID.SelectionMode = SelectionMode.One;
                    clb_firstExcel.SelectionMode = SelectionMode.One;
                    clb_secondExcel.SelectionMode = SelectionMode.One;
                }
                else
                {
                    clb_reference_ID.SelectionMode = SelectionMode.None;
                    clb_firstExcel.SelectionMode = SelectionMode.None;
                    clb_secondExcel.SelectionMode = SelectionMode.None;
                }

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private void btn_Compare_Click(object sender, EventArgs e)
        {
            try
            {
                changeEnabledStatus(false);
                dateTime1 = DateTime.Now;

                lblError.Text = string.Empty;
                lbl_Status.Text = "Process start";
                fileNameWithPath = llbl_filePath.Text + @"\" + tb_fileName.Text + @".xlsx";
                if (!backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }

        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int progress = 0;
            int totalProgress, percentage;

            dateTime1 = DateTime.Now;
            DataGridView dataGridView11 = new DataGridView();

            try
            {
                if (boolValidate)
                {
                    dt_secondTable.CaseSensitive = true;
                    dv_secondTableDataView = new DataView(dt_secondTable);

                    dt_firstTableOnly = dt_firstTable.Clone();
                    dt_secondTableOnly = dt_secondTable.Copy();

                    int cellCol, headRow, alternateRow;
                    headRow = -1;
                    cellCol = alternateRow = 0;
                    string str_colName;
                    bool newRow = true;

                    dt_unmatched_Capture = dt_firstTable.Clone();
                    dt_summarised_difference = dt_firstTable.Clone();

                    foreach (DataColumn dc in dt_firstTable.Columns)
                    {
                        dt_summarised_difference.Columns[dc.ToString()].DataType = typeof(Int32);
                    }
                    DataRow dr_summarised = dt_summarised_difference.NewRow();
                    foreach (DataColumn dc in dt_firstTable.Columns)
                    {
                        dr_summarised[dc.ToString()] = 0;
                    }
                    dt_summarised_difference.Rows.Add(dr_summarised);

                    totalProgress = dt_firstTable.Rows.Count;
                    string colNameToAdd;
                    for (int i = 0; i < dt_unmatched_Capture.Columns.Count; i++)
                    {
                        colNameToAdd = dt_unmatched_Capture.Columns[i].ColumnName.ToString();
                        dataGridView11.Columns.Add(colNameToAdd, colNameToAdd);
                    }

                    foreach (DataRow dr in dt_firstTable.Rows)
                    {
                        percentage = (((++progress) * 100) / totalProgress);
                        backgroundWorker1.ReportProgress(percentage);

                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            backgroundWorker1.CancelAsync();
                            return;
                        }

                        str_RowFilter = generateRowFilterString(dr);
                        dv_secondTableDataView.RowFilter = str_RowFilter;

                        if (dv_secondTableDataView.Count > 0)
                        {
                            delete_current_row_from_second_table();

                            CheckedListBox.CheckedItemCollection cic_firsttableColumns = clb_firstExcel.CheckedItems;
                            CheckedListBox.CheckedItemCollection cic_reference = clb_reference_ID.CheckedItems;
                            foreach (var a in cic_firsttableColumns)
                            {
                                if (!(cic_reference.Contains(a)))
                                {
                                    if (!(dr[a.ToString()].ToString() == dv_secondTableDataView[0][a.ToString()].ToString()))
                                    {
                                        cellCol = dt_firstTable.Columns[a.ToString()].Ordinal;
                                        str_colName = a.ToString();
                                        if (newRow)
                                        {
                                            dt_unmatched_Capture.Rows.Add(dr.ItemArray);
                                            dt_unmatched_Capture.Rows.Add(dv_secondTableDataView[0].Row.ItemArray);
                                            headRow += 2;
                                            alternateRow += 2;
                                            newRow = false;

                                            //dataGridView11.DataSource = dt_unmatched_Capture;
                                            dataGridView11.Rows.Add(dt_unmatched_Capture.Rows[0].ItemArray);
                                            dataGridView11.Rows.Add(dt_unmatched_Capture.Rows[1].ItemArray);
                                        }

                                        dataGridView11.Rows[headRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;
                                        dataGridView11.Rows[alternateRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;

                                        dt_summarised_difference.Rows[0][str_colName] =
                                                Convert.ToInt32((dt_summarised_difference.Rows[0][str_colName]).ToString()) + 1;
                                    }
                                }
                            }
                            newRow = true;
                        }
                        else
                        {
                            dt_firstTableOnly.Rows.Add(dr.ItemArray);
                        }

                    }

                    #region for deleting columns which are not required

                    int index = 0;
                    int adjust = 0;
                    foreach (DataColumn dc in dt_summarised_difference.Columns)
                    {
                        index = dc.Ordinal - adjust;
                        if (!(clb_reference_ID.GetItemChecked(clb_reference_ID.FindString(dc.ColumnName.ToString()))))
                        {
                            if (Convert.ToInt32(dt_summarised_difference.Rows[0][dc.ColumnName.ToString()]) == 0)
                            {
                                dt_unmatched_Capture.Columns.Remove(dc.ColumnName);
                                dataGridView11.Columns.Remove(dc.ColumnName.ToString());
                                adjust++;
                            }
                        }
                    }

                    #endregion

                    backgroundWorker1.ReportProgress(105);

                    dt_transpose = common_data.GenerateTransposeTable(dt_summarised_difference);
                    wb = new XLWorkbook();

                    wb.Worksheets.Add(dt_unmatched_Capture, "Unmatched");

                    wb.Worksheets.Add(dt_firstTableOnly, "Rows in first excelsheet");
                    wb.Worksheets.Add(dt_secondTableOnly, "Rows in second excelsheet");
                    wb.Worksheets.Add(dt_transpose, "Summarised");

                    backgroundWorker1.ReportProgress(106);

                    formatExcelTableByDataGridView(1, dataGridView11, dt_unmatched_Capture, clb_reference_ID.CheckedItems, "First Excel", "Second Excel");

                    formatExcelTable(2, dt_firstTableOnly);
                    formatExcelTable(3, dt_secondTableOnly);
                    formatExcelTable(4, dt_transpose);

                    backgroundWorker1.ReportProgress(107);

                    wb.SaveAs(fileNameWithPath);

                    backgroundWorker1.ReportProgress(108);

                    boolStatus = true;
                }
            }
            catch (Exception ex)
            {
                myException = ex;
                boolStatus = false;
                //lblError.Text = ex.Message.ToString();
            }
            finally
            {
                //changeEnabledStatus(true);
            }
        }

        public void formatExcelTable(int int_sheet_no, DataTable dt_calc)
        {
            try
            {
                wb.Worksheet(int_sheet_no).Tables.FirstOrDefault().SetShowAutoFilter(false);
                wb.Worksheet(int_sheet_no).Columns().AdjustToContents();

                for (int col = 1; col <= dt_calc.Columns.Count; col++)
                {
                    wb.Worksheet(int_sheet_no).Cell(1, col).Style.Fill.SetBackgroundColor(XLColor.Transparent);
                    wb.Worksheet(int_sheet_no).Cell(1, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wb.Worksheet(int_sheet_no).Cell(1, col).Style.Border.OutsideBorderColor = XLColor.RichBlack;
                    wb.Worksheet(int_sheet_no).Cell(1, col).Style.Font.SetFontColor(XLColor.RichBlack);
                }

                for (int row = 2; row <= dt_calc.Rows.Count + 1; row++)
                {
                    for (int col = 1; col < dt_calc.Columns.Count + 1; col++)
                    {
                        wb.Worksheet(int_sheet_no).Cell(row, col).Style.Fill.BackgroundColor = XLColor.Transparent;
                        wb.Worksheet(int_sheet_no).Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(int_sheet_no).Cell(row, col).Style.Border.OutsideBorderColor = XLColor.Black;
                        cellValue = wb.Worksheet(int_sheet_no).Cell(row, col).Value.ToString();
                        boolCellValueIntType = int.TryParse(cellValue, out int res);
                        if (boolCellValueIntType)
                        {
                            wb.Worksheet(int_sheet_no).Cell(row, col).SetDataType(XLDataType.Number);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }

        private void formatExcelTableByDataGridView(int int_sheet_no, DataGridView dgv, DataTable dt,
            CheckedListBox.CheckedItemCollection cic, string str_oracle = "Oracle", string str_sql = "SQL")
        {
            try
            {
                wb.Worksheet(1).Column(1).InsertColumnsBefore(1);
                wb.Worksheet(int_sheet_no).Cell(1, 1).Value = "DBType";

                for (int col = 0; col <= dt.Columns.Count; col++)
                {
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Fill.SetBackgroundColor(XLColor.Transparent);
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Border.OutsideBorderColor = XLColor.RichBlack;
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Font.SetFontColor(XLColor.RichBlack);
                }


                for (int row = 1; row < dgv.RowCount; row++)
                {
                    wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Style.Fill.BackgroundColor = XLColor.Transparent;
                    wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Style.Border.OutsideBorderColor = XLColor.Black;

                    if (row % 2 == 0)
                    {
                        wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Value = str_sql;
                    }
                    else
                    {
                        wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Value = str_oracle;
                    }

                }

                wb.Worksheet(int_sheet_no).Tables.FirstOrDefault().SetShowAutoFilter(false);
                wb.Worksheet(int_sheet_no).Columns().AdjustToContents();

                for (int col = 1; col <= dgv.Columns.Count; col++)
                {
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Fill.SetBackgroundColor(XLColor.Transparent);
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Border.OutsideBorderColor = XLColor.RichBlack;
                    cellValue = wb.Worksheet(int_sheet_no).Cell(1, col + 1).Value.ToString();
                    if (cic.Contains(cellValue))
                    {
                        wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Font.SetFontColor(XLColor.Red);
                    }
                    else
                    {
                        wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Font.SetFontColor(XLColor.RichBlack);
                    }

                }

                for (int row = 0; row < dgv.Rows.Count - 1; row++)
                {
                    for (int col = 0; col < dgv.Columns.Count; col++)
                    {
                        cellValue = wb.Worksheet(int_sheet_no).Cell(row + 2, col + 2).Value.ToString();
                        boolCellValueIntType = int.TryParse(cellValue, out int res);
                        if (boolCellValueIntType)
                        {
                            wb.Worksheet(int_sheet_no).Cell(row + 2, col + 2).SetDataType(XLDataType.Number);
                        }


                        if (dgv.Rows[row].Cells[col].Style.BackColor == Color.Yellow)
                        {
                            wb.Worksheet(1).Cell(row + 2, col + 2).Style.Fill.BackgroundColor = XLColor.Yellow;
                        }
                        else
                        {
                            wb.Worksheet(1).Cell(row + 2, col + 1 + 1).Style.Fill.BackgroundColor = XLColor.Transparent;
                        }
                        wb.Worksheet(int_sheet_no).Cell(row + 2, col + 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        wb.Worksheet(int_sheet_no).Cell(row + 2, col + 2).Style.Border.OutsideBorderColor = XLColor.Black;

                    }
                }

            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }

        private void delete_current_row_from_second_table()
        {
            try
            {
                DataRow[] rowsDel = dt_secondTableOnly.Select(str_RowFilter);

                foreach (DataRow dr in rowsDel)
                {
                    dr.Delete();
                }
                dt_secondTableOnly.AcceptChanges();
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }

        private void showElapsedTime()
        {
            try
            {
                timerDuration = DateTime.Now.Subtract(dateTime1);
                if (timerDuration.Hours == 0)
                {
                    lbl_Timer.Text = timerDuration.Minutes + " minute(s) and " + timerDuration.Seconds + " second(s)";
                }
                else
                {
                    lbl_Timer.Text = timerDuration.Hours + " Hour(s) " + timerDuration.Minutes + " minute(s) and " + timerDuration.Seconds + " second(s)";
                }
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (myException.InnerException != null)
            {
                lbl_Status.Text = myException.Message.ToString();
            }
            if (e.ProgressPercentage <= 100)
            {
                progressBar1.Value = e.ProgressPercentage;
                lbl_Percentage.Text = e.ProgressPercentage + " % ";
            }
            else if (e.ProgressPercentage >= 100)
            {
                progressBar1.Value = 100;
                switch (e.ProgressPercentage)
                {
                    case 105:
                        lbl_Percentage.Text = "Writing Excel File";
                        break;

                    case 106:
                        lbl_Percentage.Text = "Formatting Excel File";
                        break;

                    case 107:
                        lbl_Percentage.Text = "Saving Excel File";
                        break;

                    case 108:
                        lbl_Percentage.Text = "File Saved Successfully";
                        break;

                }
            }
            //else
            //{
            //    progressBar1.Value = 100;
            //    lbl_Percentage.Text = e.ProgressPercentage + " % Done";
            //}
            showElapsedTime();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    lbl_Status.Text = "Process Cancelled";
                    progressBar1.Value = 0;
                    lbl_Percentage.Text = "0 % and Processed cancelled";
                }
                else if (boolStatus)
                {
                    lbl_Status.Text = "Opening " + fileNameWithPath;
                    Process.Start(fileNameWithPath);
                    progressBar1.Value = 100;
                    lbl_Percentage.Text = "100 % Done";
                }
                else
                {
                    lbl_Status.Text = myException.Message.ToString();
                }
                showElapsedTime();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            finally
            {
                changeEnabledStatus(true);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbl_Timer.Text = DateTime.Now.Subtract(dateTime1).TotalSeconds.ToString();
        }

        private void resetControlsToDefaultValue(int resetType)
        {
            try
            {
                if (resetType <= 10)
                {
                    lbl_FirstExcelFileName.Text = "No File Selected";
                    lbl_SecondExcelFileName.Text = "No File Selected";
                    lbl_LoadStatus.Text = "Status: Pls select both files";
                    lbl_Timer.ResetText();

                    clb_reference_ID.Items.Clear();
                    cb_select_first_clb.CheckState = CheckState.Unchecked;
                    clb_firstExcel.Items.Clear();
                    clb_secondExcel.Items.Clear();
                    lb_IgnoreTable1.Items.Clear();
                    lb_IgnoreTable2.Items.Clear();

                    btn_Generate.Enabled = false;
                    progressBar1.Value = 0;
                    lbl_Percentage.Text = "Progress Bar";
                    lbl_Status.ResetText();
                    lblError.ResetText();
                    lbl_Parameter.ResetText();

                }
                else if (resetType <= 2)
                {

                }

            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }

        private void clb_firstExcel_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                toggleCheckBoxCheckedState();
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.CancelAsync();
                }
                if (bgw_multiThreading.IsBusy)
                {
                    bgw_multiThreading.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }

        private void Excel_Excel_Comparison_Load(object sender, EventArgs e)
        {

        }

        private void btn_ProcessBy_MultiThreading_Click(object sender, EventArgs e)
        {
            try
            {
                changeEnabledStatus(false);
                dateTime1 = DateTime.Now;

                lblError.Text = string.Empty;
                lbl_Status.Text = "Process start";
                fileNameWithPath = llbl_filePath.Text + @"\" + tb_fileName.Text + @".xlsx";
                if (!bgw_multiThreading.IsBusy)
                {
                    bgw_multiThreading.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private void bgw_multiThreading_DoWork(object sender, DoWorkEventArgs e)
        {
            //int progress = 0;
            int totalProgress;//, percentage;

            dateTime1 = DateTime.Now;
            DataGridView dataGridView11 = new DataGridView();

            try
            {
                if (boolValidate)
                {
                    dt_secondTable.CaseSensitive = true;
                    dv_secondTableDataView = new DataView(dt_secondTable);

                    dt_firstTableOnly = dt_firstTable.Clone();
                    dt_secondTableOnly = dt_secondTable.Copy();

                    //int cellCol, headRow, alternateRow;
                    //headRow = -1;
                    //cellCol = alternateRow = 0;
                    //string str_colName;
                    //bool newRow = true;

                    dt_unmatched_Capture = dt_firstTable.Clone();

                    dt_summarised_difference = dt_firstTable.Clone();

                    foreach (DataColumn dc in dt_firstTable.Columns)
                    {
                        dt_summarised_difference.Columns[dc.ToString()].DataType = typeof(Int32);
                    }
                    DataRow dr_summarised = dt_summarised_difference.NewRow();
                    foreach (DataColumn dc in dt_firstTable.Columns)
                    {
                        dr_summarised[dc.ToString()] = 0;
                    }
                    dt_summarised_difference.Rows.Add(dr_summarised);

                    totalProgress = dt_firstTable.Rows.Count;
                    string colNameToAdd;
                    for (int i = 0; i < dt_unmatched_Capture.Columns.Count; i++)
                    {
                        colNameToAdd = dt_unmatched_Capture.Columns[i].ColumnName.ToString();
                        dataGridView11.Columns.Add(colNameToAdd, colNameToAdd);
                    }

                    #region multithreading tak started

                    //ParameterizedThreadStart pts = new ParameterizedThreadStart(multiThreadingParameter);
                    //Thread t = new Thread(multiThreadingParameter);
                    //t.Start();

                    //Thread tc = Thread.CurrentThread;
                    //Thread.Sleep(10000);


                    //multiThreading(1, dt_firstTable, dt_secondTable, dataGridView11, e);

                    #endregion

                    #region for deleting columns which are not required

                    int index = 0;
                    int adjust = 0;
                    foreach (DataColumn dc in dt_summarised_difference.Columns)
                    {
                        index = dc.Ordinal - adjust;
                        if (!(clb_reference_ID.GetItemChecked(clb_reference_ID.FindString(dc.ColumnName.ToString()))))
                        {
                            if (Convert.ToInt32(dt_summarised_difference.Rows[0][dc.ColumnName.ToString()]) == 0)
                            {
                                dt_unmatched_Capture.Columns.Remove(dc.ColumnName);
                                dataGridView11.Columns.Remove(dc.ColumnName.ToString());
                                adjust++;
                            }
                        }
                    }

                    #endregion

                    bgw_multiThreading.ReportProgress(105);

                    dt_transpose = common_data.GenerateTransposeTable(dt_summarised_difference);
                    wb = new XLWorkbook();

                    wb.Worksheets.Add(dt_unmatched_Capture, "Unmatched");

                    wb.Worksheets.Add(dt_firstTableOnly, "Rows in first excelsheet");
                    wb.Worksheets.Add(dt_secondTableOnly, "Rows in second excelsheet");
                    wb.Worksheets.Add(dt_transpose, "Summarised");

                    bgw_multiThreading.ReportProgress(106);

                    formatExcelTableByDataGridView(1, dataGridView11, dt_unmatched_Capture, clb_reference_ID.CheckedItems, "First Excel", "Second Excel");

                    formatExcelTable(2, dt_firstTableOnly);
                    formatExcelTable(3, dt_secondTableOnly);
                    formatExcelTable(4, dt_transpose);

                    bgw_multiThreading.ReportProgress(107);

                    wb.SaveAs(fileNameWithPath);

                    bgw_multiThreading.ReportProgress(108);

                    boolStatus = true;
                }
            }
            catch (Exception ex)
            {
                myException = ex;
                boolStatus = false;
                //lblError.Text = ex.Message.ToString();
            }
            finally
            {
                //changeEnabledStatus(true);
            }
        }

        public void multiThreadingParameter()
        {
            Thread.Sleep(15000);
        }

        private void multiThreading(int int_status, DataTable dt_source, DataTable dt_target, DataGridView dgv, DoWorkEventArgs e)
        {
            try
            {

                if (int_status == 1)
                {
                    int percentage, progress = 0;
                    int totalProgress = dt_source.Rows.Count;
                    int cellCol, headRow, alternateRow;
                    string str_colName;
                    bool newRow = true;

                    headRow = -1;
                    cellCol = alternateRow = 0;

                    //dt_unmatched_Capture = dt_source.Clone();
                    //dt_summarised_difference = dt_source.Clone();


                    foreach (DataRow dr in dt_source.Rows)
                    {
                        percentage = (((++progress) * 100) / totalProgress);
                        bgw_multiThreading.ReportProgress(percentage);

                        if (bgw_multiThreading.CancellationPending)
                        {
                            e.Cancel = true;
                            bgw_multiThreading.CancelAsync();
                            return;
                        }

                        str_RowFilter = generateRowFilterString(dr);
                        dv_secondTableDataView.RowFilter = str_RowFilter;

                        if (dv_secondTableDataView.Count > 0)
                        {
                            delete_current_row_from_second_table();

                            CheckedListBox.CheckedItemCollection cic_firsttableColumns = clb_firstExcel.CheckedItems;
                            CheckedListBox.CheckedItemCollection cic_reference = clb_reference_ID.CheckedItems;
                            foreach (var a in cic_firsttableColumns)
                            {
                                if (!(cic_reference.Contains(a)))
                                {
                                    if (!(dr[a.ToString()].ToString() == dv_secondTableDataView[0][a.ToString()].ToString()))
                                    {
                                        cellCol = dt_firstTable.Columns[a.ToString()].Ordinal;
                                        str_colName = a.ToString();
                                        if (newRow)
                                        {
                                            dt_unmatched_Capture.Rows.Add(dr.ItemArray);
                                            dt_unmatched_Capture.Rows.Add(dv_secondTableDataView[0].Row.ItemArray);
                                            headRow += 2;
                                            alternateRow += 2;
                                            newRow = false;

                                            //dataGridView11.DataSource = dt_unmatched_Capture;
                                            dgv.Rows.Add(dt_unmatched_Capture.Rows[0].ItemArray);
                                            dgv.Rows.Add(dt_unmatched_Capture.Rows[1].ItemArray);
                                        }

                                        dgv.Rows[headRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;
                                        dgv.Rows[alternateRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;

                                        dt_summarised_difference.Rows[0][str_colName] =
                                                Convert.ToInt32((dt_summarised_difference.Rows[0][str_colName]).ToString()) + 1;
                                    }
                                }
                            }
                            newRow = true;
                        }
                        else
                        {
                            dt_firstTableOnly.Rows.Add(dr.ItemArray);
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                myException = ex;
            }
        }
        private void bgw_multiThreading_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (myException.InnerException != null)
                {
                    lbl_Status.Text = myException.Message.ToString();
                }
                if (e.ProgressPercentage <= 100)
                {
                    progressBar1.Value = e.ProgressPercentage;
                    lbl_Percentage.Text = e.ProgressPercentage + " % ";
                }
                else if (e.ProgressPercentage >= 100)
                {
                    progressBar1.Value = 100;
                    switch (e.ProgressPercentage)
                    {
                        case 105:
                            lbl_Percentage.Text = "Writing Excel File";
                            break;

                        case 106:
                            lbl_Percentage.Text = "Formatting Excel File";
                            break;

                        case 107:
                            lbl_Percentage.Text = "Saving Excel File";
                            break;

                        case 108:
                            lbl_Percentage.Text = "File Saved Successfully";
                            break;

                    }
                }
                //else
                //{
                //    progressBar1.Value = 100;
                //    lbl_Percentage.Text = e.ProgressPercentage + " % Done";
                //}
                showElapsedTime();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }

        private void bgw_multiThreading_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    lbl_Status.Text = "Process Cancelled";
                    progressBar1.Value = 0;
                    lbl_Percentage.Text = "0 % and Processed cancelled";
                }
                else if (boolStatus)
                {
                    lbl_Status.Text = "Opening " + fileNameWithPath;
                    Process.Start(fileNameWithPath);
                    progressBar1.Value = 100;
                    lbl_Percentage.Text = "100 % Done";
                }
                else
                {
                    lbl_Status.Text = myException.Message.ToString();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
            finally
            {
                changeEnabledStatus(true);
            }
        }
    }
}
