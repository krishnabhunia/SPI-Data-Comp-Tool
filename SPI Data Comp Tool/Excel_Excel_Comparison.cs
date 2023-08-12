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
using OfficeOpenXml;

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

        private string str_Data_Source_To_Compare;
        private string str_Data_Target_To_Compare;

        private OpenFileDialogData openFileDialogData, firstExcelData, secondExcelData;// = new OpenFileDialogData();
        //private Form_Rules form_Rules;// = new Form_Rules();
        //private CheckRules checkRules;
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
        private DataTable dt_Instrument_Input_File;
        private DataTable dt_reinitialize;

        private DataSet ds_Instrument_Input_File;

        private DataView dv_secondTableDataView;
        DateTime dateTime1;// = new DateTime();

        private bool boolValidate, boolInstrumentFileGenerationStatus;
        private bool boolFileSelected, boolCompareResult, boolRules, boolRulesCheck, boolInclude;

        private Exception myException;
        private TimeSpan timerDuration;

        private DataTable dt_transpose;

        private Common_Data common_Data;

        private XLWorkbook wb;
        private DataSet dsRules;

        private int percentage;
        private bool boolCancelInstrumentProcess;
        private ToolTip toolTip;
        private int int_Count_Comparison;

        private int progress;// = 0;
        private int totalProgress;

        private DataGridView dataGridView11;

        private string str_boolCheckedDC;

        private int cellCol, headRow, alternateRow;

        private string str_colName;
        private bool newRow;// = true;
        private List<string> strPKList;
        private DataColumn[] dc_PK_SRC;

        public DataColumn[] dc_PK_TRG;

        private int int_object;
        private DataTable dtUnmatched;
        private DataTable dtSRCOnly;
        private DataTable dtSummary;
        private int int_Color_Row;
        private DataRow dataRows_TRG;
        private bool boolANewRow;

        private List<ColorPair> colorPairs;
        private List<ColorPairMultiSheet> colorPairsMultiSheet;
        private ExcelWorksheet worksheet;
        private IEnumerable<ColorPair> queryIenumerable;
        private IQueryable<ColorPair> queryIqueryable;
        private IQueryable<ColorPair> queryColorpair;
        private IQueryable<ColorPairMultiSheet> queryColorpairMS;
        private string strFileName, strFolderPath;
        private Exception myException_BG;

        private StringComparison stringComparisonTypeOrdincalCase;

        private SaveFileDialogFile saveFileDialogFile;

        private bool boolSelectedBothFile;
        private DataSet dsUnmatched;

        public Excel_Excel_Comparison()
        {
            InitializeComponent();

            lblError.ForeColor = Color.Red;
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
            openFileDialogData = new OpenFileDialogData();
            firstExcelData = new OpenFileDialogData();
            secondExcelData = new OpenFileDialogData();

            //llbl_filePath.Text = Environment.SpecialFolder.Desktop.ToString();
            boolValidate = false;
            dateTime1 = new DateTime();

            myException = new Exception();

            dt_transpose = new DataTable();

            //llbl_filePath.Text = @"D:\";

            dt_firstTableOnly = new DataTable();
            dt_secondTableOnly = new DataTable();

            common_Data = new Common_Data();
            str_Data_Source_To_Compare = string.Empty;
            str_Data_Target_To_Compare = string.Empty;
            dsRules = new DataSet();
            //checkRules = new CheckRules();

            if (common_Data.dsRules != null && common_Data.dsRules.Tables.Count > 0 && common_Data.dsRules.Tables[0].Rows.Count > 0)
            {
                dsRules = common_Data.dsRules;
            }

            dt_Instrument_Input_File = new DataTable();
            ds_Instrument_Input_File = new DataSet();
            boolRules = false;
            boolCancelInstrumentProcess = false;
            int_Count_Comparison = 0;

            dt_reinitialize = new DataTable();

            progress = 0;

            newRow = true;
            colorPairs = new List<ColorPair>();
            colorPairsMultiSheet = new List<ColorPairMultiSheet>();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

            saveFileDialogFile = new SaveFileDialogFile();

            if (Debugger.IsAttached)
            {
                btnLoad.Visible = true;

                //Properties.Settings.Default.XL_Excel_Save = true;
                //Properties.Settings.Default.EPPlus_Excel_Save = false;
            }
            else
            {
                btn_ProcessBy_MultiThreading.Enabled = false;
                btn_ProcessBy_MultiThreading.Visible = false;
            }

            toolTip = new ToolTip();

            lbl_InstrumentText.Text = "The Below Button will generate Required Item Tag and Short Item Tag for specific Input Source File which doesn't have any Unique reference ID other that Item Tag which get matched with Target Input File. Pls use this in old datasets.";

            if (Debugger.IsAttached)
            {
                button1.Visible = true;
                btn_TestSystemMemoryOutException.Visible = true;
            }

            dsUnmatched = new DataSet();
        }

        private void btn_FirstExcelSheet_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialogData.openFileDialog();
                firstExcel = openFileDialogData.strFileNameWithPath;
                lbl_FirstExcelFileName.Text = firstExcel;
                boolStatus = checkDuplicateSameFile(firstExcel, secondExcel);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
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
        //        boolFileSelected = true;
        //    }
        //    else
        //    {
        //        openFileDialogData.strFileNameWithPath = "File Not Selected";
        //        boolFileSelected = false;
        //    }
        //    return openFileDialogData;
        //}

        private void btn_SecondExcelSheet_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialogData.openFileDialog();
                secondExcel = openFileDialogData.strFileNameWithPath;
                lbl_SecondExcelFileName.Text = secondExcel;
                boolStatus = checkDuplicateSameFile(firstExcel, secondExcel);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
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
                DisplayError(ex);
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
                }
                dsResult.Tables[0].Rows.RemoveAt(0);
                dsResult.Tables[0].TableName = dataTableName;
                dsResult.AcceptChanges();

                excelReader.Close();

                dsResult = common_Data.ChangeColumnToString(dsResult);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            return dsResult;
        }


        private DataTable LoadExcelFromDataReader(string str_file_to_load_path, string dataTableName, bool b = true)
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
                }
                dsResult.Tables[0].Rows.RemoveAt(0);
                dsResult.Tables[0].TableName = dataTableName;
                dsResult.AcceptChanges();

                excelReader.Close();

                dsResult = common_Data.ChangeColumnToString(dsResult);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            return dsResult.Tables[0];
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
                DisplayError(ex);
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

                dt_firstTable = common_Data.getCommonColumns(ds.Tables[0], ds.Tables[1], lb_IgnoreTable1);
                dt_secondTable = common_Data.getCommonColumns(ds.Tables[1], ds.Tables[0], lb_IgnoreTable2);

                LoadCheckListBox(clb_reference_ID, dt_firstTable);
                LoadCheckListBox(clb_firstExcel, dt_firstTable);
                LoadCheckListBox(clb_secondExcel, dt_secondTable);

                //tb_fileName.Text = $"{excelWorksheet.Name}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}";

                lbl_LoadStatus.Text = "Loaded successfully";
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                changeEnabledStatus(true);
            }
        }

        //private DataTable getCommonColumns(DataTable dt, DataTable dt_check, ListBox lb)
        //{
        //    try
        //    {
        //        dt_returnDataTable = dt.Copy();
        //        lb.Items.Clear();
        //        DataColumnCollection dcc = dt_check.Columns;

        //        foreach (DataColumn dc in dt.Columns)
        //        {
        //            if (!(dcc.Contains(dc.ColumnName.ToString())))
        //            {
        //                dt_returnDataTable.Columns.Remove(dc.ColumnName.ToString());
        //                lb.Items.Add(dc.ColumnName.ToString());
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayError(ex);
        //    }
        //    return dt_returnDataTable;
        //}

        private void LoadCheckListBox(CheckedListBox cbl, DataTable dt)
        {
            try
            {
                cbl.Items.Clear();
                foreach (DataColumn dc in dt.Columns)
                {
                    cbl.Items.Add(dc.ColumnName.ToString());
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void clb_reference_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (clb_reference_ID.CheckedItems.Count > 0)
                {
                    btn_Validate.Enabled = true;
                }
                else
                {
                    btn_Validate.Enabled = false;
                }
                //setData();
                //clb_reference_ID.SelectionMode = SelectionMode.None;
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private string generateRowFilterString(DataRow dr)
        {
            try
            {
                str_Reference_ID = string.Empty;
                foreach (var a in clb_reference_ID.CheckedItems)
                {
                    //str_Reference_ID = str_Reference_ID + a.ToString() + " = '" + dr[a.ToString()] + "' AND ";
                    str_Reference_ID = $"{str_Reference_ID} [{a.ToString()}] = '{dr[a.ToString()]}' AND ";
                }
                str_Reference_ID = str_Reference_ID.Substring(0, str_Reference_ID.Length - 4);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
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
                //llbl_filePath.Text = ofdd.folderBrowser(llbl_filePath.Text);
                if (saveFileDialogFile.BoolFileSaveStatus)
                {
                    Process.Start(llbl_filePath.Text);
                    DisplayMessage("File Opened Again", true);
                }
                else
                {
                    DisplayMessage("There is some problem in saving or fetching the file", true);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
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
                DisplayError(ex);
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
                DisplayError(ex);
            }
        }


        private async void btn_SelectLoad_Click(object sender, EventArgs e)
        {
            try
            {
                resetControlsToDefaultValue(10);
                changeEnabledStatus(false);
                boolSelectedBothFile = false;
                btn_Compare.Enabled = false;
                Logs.EnterLogsWithTime($"Selecting both input files");

                firstExcelData = firstExcelData.openFileDialog("Select First Excel File");
                firstExcel = firstExcelData.strFileNameWithPath;
                lbl_FirstExcelFileName.Text = firstExcel;

                // ============================================================================
                if (firstExcelData.boolFileSelected)
                {
                    do
                    {
                        secondExcelData = secondExcelData.openFileDialog("Select Second Excel File");
                        secondExcel = secondExcelData.strFileNameWithPath;
                        lbl_SecondExcelFileName.Text = secondExcel;
                        //lbl_SecondExcelFileName.Update();
                        boolStatus = checkDuplicateSameFile(firstExcel, secondExcel);
                        //boolLoopWhileStatus = !(boolStatus);

                    } while (!boolStatus);
                }

                // ============================================================================

                if (boolStatus & firstExcelData.boolFileSelected & secondExcelData.boolFileSelected)
                {
                    Logs.EnterLogsWithTime($"Source : {firstExcelData.strFileNameWithPath}, Target : {secondExcelData.strFileNameWithPath}");
                    lbl_LoadStatus.Text = "Loading started, pls wait !!!";
                    lbl_LoadStatus.Update();
                    ds = new DataSet();

                    DataTable dt = new DataTable();

                    //dt = LoadExcelFromDataReader(firstExcel, firstExcelData.SimpleFileName_WithoutExt).Tables[0];

                    Task<DataTable> taskLoadSRCExcelFile = Task.Run<DataTable>(() => LoadExcelFromDataReader(firstExcel, firstExcelData.SimpleFileName_WithoutExt, true));

                    #region test memory exception

                    //await Task.WhenAll(taskLoadSRCExcelFile).ContinueWith(x => {
                    //    ds.Tables.Add(taskLoadSRCExcelFile.Result.Copy());
                    //    Test_SYSTEM_OUT_OF_MEMORYEXCEPTION();
                    //});

                    #endregion

                    //ds.Tables.Add(dt.Copy());

                    //dt = new DataTable();

                    //dt = LoadExcelFromDataReader(secondExcel, secondExcelData.SimpleFileName_WithoutExt).Tables[0];
                    Task<DataTable> taskLoadTRGExcelFile = Task.Run<DataTable>(() => LoadExcelFromDataReader(secondExcel, secondExcelData.SimpleFileName_WithoutExt, true));

                    await Task.WhenAll(taskLoadSRCExcelFile, taskLoadTRGExcelFile).ContinueWith(x =>
                    {
                        ds.Tables.Add(taskLoadSRCExcelFile.Result.Copy());
                        ds.Tables.Add(taskLoadTRGExcelFile.Result.Copy());
                        //Test_SYSTEM_OUT_OF_MEMORYEXCEPTION();
                    });

                    //await Task.WhenAll(taskLoadSRCExcelFile, taskLoadTRGExcelFile);

                    if (string.Equals(firstExcelData.SimpleFileName_WithoutExt, secondExcelData.SimpleFileName_WithoutExt, StringComparison.OrdinalIgnoreCase))
                    {
                        //dt.TableName += "_Second";
                        ds.Tables[1].TableName += "_Second";
                    }
                    Logs.EnterLogsWithTime($"Both source and target data loaded");
                    Logs.EnterLogsWithTime($"Source file raw :Rows: {ds.Tables[0].Rows.Count}, Columns: {ds.Tables[0].Columns.Count}");
                    Logs.EnterLogsWithTime($"Target file raw :Rows: {ds.Tables[1].Rows.Count}, Columns: {ds.Tables[1].Columns.Count}");
                    //ds.Tables.Add(dt.Copy());

                    //ds = LoadExcel(firstExcel, "firstExcel");
                    //ds = LoadExcel(secondExcel, "secondExcel");

                    dt_firstTable = new DataTable();
                    dt_firstTable = common_Data.getCommonColumns(ds.Tables[0], ds.Tables[1], lb_IgnoreTable1);
                    dt_secondTable = new DataTable();
                    dt_secondTable = common_Data.getCommonColumns(ds.Tables[1], ds.Tables[0], lb_IgnoreTable2);

                    Logs.EnterLogsWithTime($"Source file :Rows: {dt_firstTable.Rows.Count}, Columns: {dt_firstTable.Columns.Count}");

                    Logs.EnterLogsWithTime($"Target file :Rows: {dt_secondTable.Rows.Count}, Columns: {dt_secondTable.Columns.Count}");

                    if (cb_MultiReference.Checked)
                    {
                        Logs.EnterLogsWithTime($"Comparison started with ViewForMultiReference");
                        ViewForMultiReference viewForMultiReference = new ViewForMultiReference(ds, ds);
                        viewForMultiReference.Show();
                    }
                    else
                    {
                        Logs.EnterLogsWithTime($"Comparison started with Normal mode");
                        LoadCheckListBox(clb_reference_ID, dt_firstTable);
                        LoadCheckListBox(clb_firstExcel, dt_firstTable);
                        LoadCheckListBox(clb_secondExcel, dt_secondTable);
                        Logs.EnterLogsWithTime($"All data loaded and comparison ready to start");
                    }

                    //int index1 = firstExcel.LastIndexOf('\\') + 1;
                    //int index2 = firstExcel.LastIndexOf('.');

                    //tb_fileName.Text = firstExcel.Substring(index1, index2 - index1) + "_output_" + DateTime.Now.ToString().Replace(':', '_').Replace('-', '_');

                    //tb_fileName.Text = $"{firstExcel.Substring(index1, index2 - index1)}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}";

                    if (clb_firstExcel.Items.Count > 0)
                    {
                        cb_select_first_clb.Checked = true;
                    }

                    lbl_Parameter.Text = "Rows : " + ds.Tables[0].Rows.Count + " and Columns : " + ds.Tables[0].Columns.Count
                    + ", Total comparison : " + ds.Tables[0].Rows.Count * ds.Tables[0].Columns.Count;

                    lbl_LoadStatus.Text = "Loaded successfully";

                    boolSelectedBothFile = true;

                    //if (Debugger.IsAttached & false)
                    //{
                    //    checkRules.PNIDVariousRuleChecks(ds.Tables[0], ds.Tables[1]);
                    //}
                    //boolLoopWhileStatus = false;

                }
                else
                {
                    lbl_LoadStatus.Text = "One of the two files OR both files not selected";
                    Logs.EnterLogsWithTime($"{lbl_LoadStatus.Text}");
                }
                // New Code
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                changeEnabledStatus(true);
                btn_Compare.Enabled = false;
                btn_Validate.Enabled = false;
            }
        }

        private void btn_Validate_Click(object sender, EventArgs e)
        {
            try
            {
                if (boolSelectedBothFile)
                {

                    changeEnabledStatus(false);
                    lbl_Status.ResetText();

                    int index1 = firstExcel.LastIndexOf('\\') + 1;
                    int index2 = firstExcel.LastIndexOf('.');
                    //tb_fileName.Text = $"{firstExcel.Substring(index1, index2 - index1)}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}";

                    if (clb_reference_ID.CheckedItems.Count >= 1)
                    {
                        boolValidate = true;
                    }
                    else
                    {
                        boolValidate = false;
                        lbl_Status.Text = "Pls select atleast one reference key";
                        Logs.EnterLogsWithTime(lbl_Status.Text);
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
                    lblError.Text = string.Empty;

                    if (boolValidate)
                    {
                        btn_Compare.Enabled = true;
                    }
                    else
                    {
                        lbl_Status.Text = "Pls select atleast one column to check mismatch";
                        btn_Compare.Enabled = false;
                        Logs.EnterLogsWithTime(lbl_Status.Text);
                    }
                }
                else
                {
                    lbl_Status.Text = "No Files Selected Or there is some problem in Excel File";
                    btn_Compare.Enabled = false;
                    btn_Validate.Enabled = false;
                    Logs.EnterLogsWithTime(lbl_Status.Text);
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
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
                btn_Compare.Enabled = boolEnabledStatus;
                btn_SelectLoad.Enabled = boolEnabledStatus;
                btnLoad.Enabled = boolEnabledStatus;
                btn_RuleForm.Enabled = boolEnabledStatus;
                btn_Validate.Enabled = boolEnabledStatus;
                cb_select_first_clb.Enabled = boolEnabledStatus;

                llbl_filePath.Enabled = boolEnabledStatus;
                //tb_fileName.Enabled = boolEnabledStatus;

                cb_IncludeDiff_Columns.Enabled = boolEnabledStatus;
                cb_IgnoreCase.Enabled = boolEnabledStatus;
                cb_IncludeRule.Enabled = boolEnabledStatus;
                btn_RuleForm.Enabled = boolEnabledStatus;

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
                DisplayError(ex);
            }
        }

        private void btn_Compare_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalDebug.ISGlobalDebug(Environment.UserName, Environment.UserName))
                {
                    //tb_fileName.Text = $"Output_{common_Data.RemoveSpecialCharacters(DateTime.Now.ToString())}";
                    //tb_fileName.Text = $"{excelWorksheet.Name}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}";
                }
                llbl_filePath.Visible = false;
                label1.Visible = false;

                int_Count_Comparison = 0;

                dt_firstTable = new DataTable();
                dt_firstTable = common_Data.getCommonColumns(ds.Tables[0], ds.Tables[1], lb_IgnoreTable1);
                dt_secondTable = new DataTable();
                dt_secondTable = common_Data.getCommonColumns(ds.Tables[1], ds.Tables[0], lb_IgnoreTable2);

                lblError.Text = string.Empty;
                lbl_Status.Text = "Process start";
                Logs.EnterLogsWithTime(lbl_Status.Text);
                //strFileNameWithPath = $@"{llbl_filePath.Text}\{tb_fileName.Text}.xlsx";

                DisplayMessage("Saving Output File !!!");

                if (!(Debugger.IsAttached))
                {
                    saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Output File", $"{firstExcelData.SimpleFileName_WithoutExt}_{secondExcelData.SimpleFileName_WithoutExt}_{common_Data.AppendDateInOutputFileName(DateTime.Now).ToString()}");
                }
                else
                {
                    saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Output File", $"{firstExcelData.SimpleFileName_WithoutExt}_{secondExcelData.SimpleFileName_WithoutExt}_{common_Data.AppendDateInOutputFileName(DateTime.Now).ToString()}###A######B######C###");
                }

                DisplayMessage("Checking Rules");

                if (cb_IncludeRule.Checked)
                {
                    boolRules = common_Data.GetRulesStatus();
                    dsRules = common_Data.GetDatasetRules();

                    if (dsRules != null && dsRules.Tables[0] != null && dsRules.Tables[0].Rows.Count > 0 && dt_firstTable.Rows.Count > 0 && dt_secondTable.Rows.Count > 0)
                    {
                        //boolRules = true;
                        if (GlobalDebug.boolIsGlobalDebug)
                        {
                            saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialogFile.Str_saveFileNameWithPath.Replace("###A###", "_RuleIncluded");
                        }
                    }
                    else
                    {
                        //boolRules = false;
                        if (GlobalDebug.boolIsGlobalDebug)
                        {
                            saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialogFile.Str_saveFileNameWithPath.Replace("###A###", "_RuleNOTIncluded");
                        }
                    }
                }
                else
                {
                    boolRules = false;
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialogFile.Str_saveFileNameWithPath.Replace("###A###", "_RuleNOTIncluded").ToString();
                    }
                }

                DisplayMessage("Checking Cases and Ignored Columns");

                if (cb_IncludeDiff_Columns.Checked)
                {
                    boolInclude = true;
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialogFile.Str_saveFileNameWithPath.Replace("###B###", "_DiffSelected");
                    }
                }
                else
                {
                    boolInclude = false;
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialogFile.Str_saveFileNameWithPath.Replace("###B###", "_DiffNOTSelected");
                    }
                }

                if (cb_IgnoreCase.Checked)
                {
                    stringComparisonTypeOrdincalCase = StringComparison.OrdinalIgnoreCase;
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialogFile.Str_saveFileNameWithPath.Replace("###C###", "_IgnoreCase");
                    }
                }
                else
                {
                    stringComparisonTypeOrdincalCase = StringComparison.Ordinal;
                    if (GlobalDebug.boolIsGlobalDebug)
                    {
                        saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialogFile.Str_saveFileNameWithPath.Replace("###C###", "_Ordinal");
                    }
                }

                saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialogFile.Str_saveFileNameWithPath.Replace("###A######B######C###", "");

                DisplayMessage("Initializing process to run");

                if (saveFileDialogFile.BoolFileSaveStatus)
                {
                    if (!backgroundWorker1.IsBusy)
                    {
                        DisplayMessage("Processing, Pls Wait !!!");
                        changeEnabledStatus(false);
                        dateTime1 = DateTime.Now;
                        timer1.Enabled = true;
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
                else
                {
                    changeEnabledStatus(true);
                    DisplayMessage("File Not Saved By User", true);
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        /// <summary>
        /// this function is remodified for optimisation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                myException = myException_BG = null;
                Common_Data.RearrangeColumns(dt_firstTable, dt_secondTable);

                if (cb_Primary_Key.Checked)
                {
                    string strPk = string.Join(", ", clb_reference_ID.CheckedItems.Cast<string>()
                          .Select(s => s.ToString())
                          .ToArray());

                    str_colName = string.Join(", ", clb_firstExcel.CheckedItems.Cast<string>()
                              .Select(s => s.ToString())
                              .ToArray());

                    str_colName = string.Concat(str_colName, ", ", strPk);

                    dt_firstTable = common_Data.RemoveColumnExcept(dt_firstTable, str_colName);
                    dt_secondTable = common_Data.RemoveColumnExcept(dt_secondTable, str_colName);

                    //Common_Data.RearrangeColumns(dt_firstTable, dt_secondTable);

                    CompareTwoDataTableWithPrimaryKey(dt_firstTable, dt_secondTable, strPk, e);
                    return;
                }
                strPKList = clb_reference_ID.CheckedItems.Cast<string>().Select(p => p.Trim()).ToList();

                #region reinitialization

                dt_reinitialize = new DataTable();
                dt_reinitialize = dt_secondTable.Copy();

                #endregion
                progress = 0;

                dateTime1 = DateTime.Now;
                dataGridView11 = new DataGridView();

                str_boolCheckedDC = "boolChecked";

                if (boolValidate)
                {
                    #region adding a flag column for checking

                    if (dt_reinitialize.Columns.Contains(str_boolCheckedDC))
                    {
                        dt_reinitialize.Columns.Remove(str_boolCheckedDC);
                    }

                    dt_secondTable = new DataTable();
                    dt_secondTable = dt_reinitialize.Copy();

                    if (!(dt_secondTable.Columns.Contains(str_boolCheckedDC)))
                    {
                        dt_secondTable.Columns.Add(str_boolCheckedDC, typeof(bool));
                    }

                    foreach (DataRow dr_TRG in dt_secondTable.Rows)
                    {
                        dr_TRG[str_boolCheckedDC] = false;
                    }

                    if (!(dt_firstTable.Columns.Contains(str_boolCheckedDC)))
                    {
                        dt_firstTable.Columns.Add(str_boolCheckedDC);
                    }

                    foreach (DataRow dr_SRC in dt_firstTable.Rows)
                    {
                        dr_SRC[str_boolCheckedDC] = false;
                    }

                    #endregion
                    dt_secondTable.CaseSensitive = true;
                    dv_secondTableDataView = new DataView(dt_secondTable);

                    dt_firstTableOnly = dt_firstTable.Clone();
                    dt_secondTableOnly = dt_secondTable.Copy();

                    headRow = -1;
                    cellCol = alternateRow = 0;

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
                    newRow = true;
                    int_Color_Row = 1;

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

                        str_RowFilter = $"{str_RowFilter} And {str_boolCheckedDC} = 'False'";

                        dv_secondTableDataView.RowFilter = str_RowFilter;

                        if (dv_secondTableDataView.Count > 0)
                        {
                            delete_current_row_from_second_table();

                            CheckedListBox.CheckedItemCollection cic_firsttableColumns = clb_firstExcel.CheckedItems;
                            CheckedListBox.CheckedItemCollection cic_reference = clb_reference_ID.CheckedItems;
                            foreach (var a in cic_firsttableColumns)
                            {
                                int_Count_Comparison++;

                                if (!(cic_reference.Contains(a)))
                                {
                                    str_Data_Source_To_Compare = dr[a.ToString()].ToString().Trim();
                                    str_Data_Target_To_Compare = dv_secondTableDataView[0][a.ToString()].ToString().Trim();

                                    //boolCompareResult = false;

                                    bool boolTest = common_Data.MismatchTest(str_Data_Source_To_Compare, str_Data_Target_To_Compare, cb_IgnoreCase.Checked, cb_IncludeRule.Checked, a.ToString(), a.ToString(), dr, dv_secondTableDataView.ToTable().Rows[0], dt_firstTable.Columns, dt_secondTable.Columns);

                                    if (boolTest)
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
                                            int_Color_Row += 2;

                                            dataGridView11.Rows.Add(dt_unmatched_Capture.Rows[0].ItemArray);
                                            dataGridView11.Rows.Add(dt_unmatched_Capture.Rows[1].ItemArray);
                                        }

                                        dataGridView11.Rows[headRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;
                                        dataGridView11.Rows[alternateRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;

                                        colorPairs.Add(new ColorPair(int_Color_Row, str_colName));
                                        colorPairs.Add(new ColorPair(int_Color_Row - 1, str_colName));

                                        dt_summarised_difference.Rows[0][str_colName] =
                                                Convert.ToInt32((dt_summarised_difference.Rows[0][str_colName]).ToString()) + 1;

                                    }
                                }
                            }
                            newRow = true;
                            if (dv_secondTableDataView.Count > 1)
                            {
                                dv_secondTableDataView[0][str_boolCheckedDC] = true;
                            }
                        }
                        else
                        {
                            dt_firstTableOnly.Rows.Add(dr.ItemArray);
                        }
                        //dv_secondTableDataView[0][dc_boolChecked.ColumnName] = true;
                    }

                    #region removing boolChecked Column

                    if (dt_unmatched_Capture.Columns.Contains(str_boolCheckedDC))
                    {
                        dt_unmatched_Capture.Columns.Remove(str_boolCheckedDC);
                    }

                    if (dt_firstTable.Columns.Contains(str_boolCheckedDC))
                    {
                        dt_firstTable.Columns.Remove(str_boolCheckedDC);
                    }

                    if (dt_secondTable.Columns.Contains(str_boolCheckedDC))
                    {
                        dt_secondTable.Columns.Remove(str_boolCheckedDC);
                    }

                    if (dt_firstTableOnly.Columns.Contains(str_boolCheckedDC))
                    {
                        dt_firstTableOnly.Columns.Remove(str_boolCheckedDC);
                    }

                    if (dt_secondTableOnly.Columns.Contains(str_boolCheckedDC))
                    {
                        dt_secondTableOnly.Columns.Remove(str_boolCheckedDC);
                    }

                    if (dt_summarised_difference.Columns.Contains(str_boolCheckedDC))
                    {
                        dt_summarised_difference.Columns.Remove(str_boolCheckedDC);
                    }

                    #endregion

                    #region for deleting columns which are not required

                    if (cb_IncludeDiff_Columns.Checked)
                    {
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
                    }

                    #endregion

                    backgroundWorker1.ReportProgress(105);

                    dt_transpose = common_Data.GenerateTransposeTable(dt_summarised_difference);
                    #region New saving excel code

                    //dt_unmatched_Capture = common_Data.ChangeBlackCellToEmpty(dt_unmatched_Capture);

                    Logs.EnterLogsWithTime($"Data in dt_unmatched_Capture datatable :Rows : {dt_unmatched_Capture.Rows.Count}, Columns : {dt_unmatched_Capture.Columns.Count}");

                    Logs.EnterLogsWithTime($"Data in dt_firstTableOnly datatable :Rows : {dt_firstTableOnly.Rows.Count}, Columns : {dt_firstTableOnly.Columns.Count}");

                    Logs.EnterLogsWithTime($"Data in dt_secondTableOnly datatable :Rows : {dt_secondTableOnly.Rows.Count}, Columns : {dt_secondTableOnly.Columns.Count}");

                    Logs.EnterLogsWithTime($"Data in dt_transpose datatable :Rows : {dt_transpose.Rows.Count}, Columns : {dt_transpose.Columns.Count}");

                    if (Properties.Settings.Default.EPPlus_Excel_Save)
                    {
                        Logs.EnterLogsWithTime($"Saving File with EPPlus started");
                        using (ExcelPackage pck = new ExcelPackage())
                        {
                            worksheet = pck.Workbook.Worksheets.Add("Mismatch");
                            worksheet.Cells["A1"].LoadFromDataTable(dt_unmatched_Capture, true);
                            worksheet.Cells.AutoFitColumns(22, 55);

                            //Common_Data.DisplayDebugMsg(rtb_Log,"First Workseet Written");
                            backgroundWorker1.ReportProgress(101);

                            #region coloring

                            //worksheet.Cells[1, 2, 1, strPKList.Count + 1].Style.Font.Color.SetColor(Color.Red);
                            progress = 0;
                            totalProgress = worksheet.Dimension.End.Column;

                            queryColorpair = colorPairs.AsQueryable<ColorPair>();

                            foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                            {
                                percentage = (((++progress) * 100) / totalProgress);
                                backgroundWorker1.ReportProgress(percentage);

                                if (backgroundWorker1.CancellationPending)
                                {
                                    e.Cancel = true;
                                    backgroundWorker1.CancelAsync();
                                    return;
                                }

                                //queryIenumerable = colorPairs.Where(cp => cp.str_Col_Name.ToLower() == cell.Value.ToString().ToLower());

                                //queryIqueryable = colorPairs.AsQueryable().Where(cp => cp.str_Col_Name.ToLower() == cell.Value.ToString().ToLower());

                                queryIqueryable = queryColorpair.Where(cp => cp.str_Col_Name.ToLower() == cell.Value.ToString().ToLower());

                                foreach (var i in queryIqueryable)
                                {
                                    worksheet.Cells[i.int_Row, cell.Start.Column].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    worksheet.Cells[i.int_Row, cell.Start.Column].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                    worksheet.Cells[i.int_Row, cell.Start.Column].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                }
                            }

                            //worksheet = common_Data.ChangeBlackCellToEmpty(worksheet);

                            //Common_Data.DisplayDebugMsg(rtb_Log,"Yellow coloring of first worksheet is done");
                            backgroundWorker1.ReportProgress(102);

                            #endregion

                            dt_firstTableOnly.TableName = "Source Rows Only";

                            worksheet = common_Data.AddNewWorksheet(pck, worksheet, dt_firstTableOnly);
                            worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                            //Common_Data.DisplayDebugMsg(rtb_Log,"Second worksheet written");
                            backgroundWorker1.ReportProgress(101);


                            worksheet = common_Data.AddNewWorksheet(pck, worksheet, dt_secondTableOnly, "Target Rows Only");
                            worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                            //Common_Data.DisplayDebugMsg(rtb_Log,"Third worksheet written");
                            backgroundWorker1.ReportProgress(102);


                            worksheet = common_Data.AddNewWorksheet(pck, worksheet, dt_transpose, "Summary");
                            worksheet.Cells[2, 1, strPKList.Count + 1, 1].Style.Font.Color.SetColor(Color.Red);

                            foreach (var cell in worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column])
                            {
                                cell.Value = Convert.ToInt32(cell.Value);
                            }

                            //Common_Data.DisplayDebugMsg(rtb_Log,"Fourth and last worksheet written");
                            backgroundWorker1.ReportProgress(103);

                            // New Code for dtSummaryForList

                            //strFileName = $@"{strFolderPath}\{dtSRC.TableName}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                            pck.SaveAs(new System.IO.FileInfo(saveFileDialogFile.Str_saveFileNameWithPath));
                            //Common_Data.DisplayMessage(rtb_Log, $"File Saved : {strFileName}", false, -1, Color.DarkBlue);
                            backgroundWorker1.ReportProgress(104);
                            Logs.EnterLogsWithTime($"Saving File with EPPlus ended");
                        }

                        #endregion

                    }
                    else if (Properties.Settings.Default.XL_Excel_Save)
                    {
                        #region Old excel saving code
                        Logs.EnterLogsWithTime($"Saving File with XL Started");
                        wb = new XLWorkbook();

                        wb.Worksheets.Add(dt_unmatched_Capture, "Mismatch");

                        wb.Worksheets.Add(dt_firstTableOnly, "Rows in first excelsheet");
                        wb.Worksheets.Add(dt_secondTableOnly, "Rows in second excelsheet");
                        wb.Worksheets.Add(dt_transpose, "Summarised");

                        backgroundWorker1.ReportProgress(106);

                        formatExcelTableByDataGridView(1, dataGridView11, dt_unmatched_Capture, clb_reference_ID.CheckedItems, "First Excel", "Second Excel");

                        formatExcelTable(2, dt_firstTableOnly);
                        formatExcelTable(3, dt_secondTableOnly);
                        formatExcelTable(4, dt_transpose);

                        backgroundWorker1.ReportProgress(107);

                        wb.SaveAs(saveFileDialogFile.Str_saveFileNameWithPath);

                        backgroundWorker1.ReportProgress(108);
                        #endregion end code
                        Logs.EnterLogsWithTime($"Saving File with XL Ended");
                        boolStatus = true;
                    }

                }

            }
            catch (Exception ex)
            {
                myException = ex;
                boolStatus = false;
            }
        }

        private void CompareTwoDataTableWithPrimaryKey(DataTable dtSRC, DataTable dtTRG, string strPK, DoWorkEventArgs e)
        {
            try
            {
                #region for datagridview
                //dataGridView11 = new DataGridView();
                #endregion
                #region main comparison part

                colorPairs = new List<ColorPair>();

                strPKList = strPK.Split(',').Select(p => p.Trim()).ToList();

                dc_PK_SRC = new DataColumn[strPKList.Count];
                dc_PK_TRG = new DataColumn[strPKList.Count];

                int_object = 0;

                foreach (string item in strPKList)
                {
                    dtSRC.Columns[item].SetOrdinal(int_object);
                    dtTRG.Columns[item].SetOrdinal(int_object);
                    dc_PK_SRC[int_object] = dtSRC.Columns[item];
                    dc_PK_TRG[int_object] = dtTRG.Columns[item];
                    int_object++;
                }

                object[] vs_pk = new object[int_object];
                dtUnmatched = dtSRC.Clone();
                dtSRCOnly = dtSRC.Clone();
                dtSummary = dtSRC.Clone();

                foreach (DataColumn dc in dtSummary.Columns)
                {
                    dc.DataType = typeof(Int32);
                }

                dtSummary.Rows.Add();

                foreach (DataColumn dc in dtSummary.Columns)
                {
                    dtSummary.Rows[0][dc] = 0;
                }

                dtSRC.PrimaryKey = dc_PK_SRC;
                dtTRG.PrimaryKey = dc_PK_TRG;

                int_Color_Row = 1;
                totalProgress = dtSRC.Rows.Count;
                int int_sheet = 1;
                foreach (DataRow dr in dtSRC.Rows.Cast<DataRow>().ToArray())
                {
                    percentage = (((++progress) * 100) / totalProgress);
                    backgroundWorker1.ReportProgress(percentage);

                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker1.CancelAsync();
                        return;
                    }

                    int_object = 0;
                    foreach (string item in strPKList)
                    {
                        vs_pk[int_object++] = dr[item];
                    }

                    dataRows_TRG = dtTRG.Rows.Find(vs_pk);

                    if (dataRows_TRG != null)
                    {
                        boolANewRow = true;
                        if(int_Color_Row >= 1020300)
                        {
                            int_Color_Row = 1;
                            ++int_sheet;
                        }

                        foreach (DataColumn dc in dataRows_TRG.Table.Columns)
                        {
                            if (!(strPKList.Any(a => string.Equals(a, dc.ColumnName))))
                            {
                                str_Data_Source_To_Compare = dr[dc.ColumnName].ToString().Trim();
                                str_Data_Target_To_Compare = dataRows_TRG[dc.ColumnName].ToString().Trim();

                                bool boolTest = common_Data.MismatchTest(str_Data_Source_To_Compare, str_Data_Target_To_Compare, cb_IgnoreCase.Checked, cb_IncludeRule.Checked, dc.ColumnName.ToString(), dc.ColumnName.ToString(), dr, dr, dtSRC.Columns, dtTRG.Columns);

                                if (boolTest)
                                {
                                    //cellCol = dtSRCOnly.Columns[dc.ColumnName].Ordinal;
                                    dtSummary.Rows[0][dc.ColumnName] = Convert.ToInt32(dtSummary.Rows[0][dc.ColumnName]) + 1;
                                    if (boolANewRow)
                                    {
                                        dtUnmatched.Rows.Add(dr.ItemArray);
                                        dtUnmatched.Rows.Add(dataRows_TRG.ItemArray);
                                        boolANewRow = false;
                                        int_Color_Row += 2;
                                    }
                                    colorPairs.Add(new ColorPair(int_Color_Row - 1, dc.ColumnName.ToString()));
                                    colorPairs.Add(new ColorPair(int_Color_Row, dc.ColumnName.ToString()));

                                    colorPairsMultiSheet.Add(new ColorPairMultiSheet(int_Color_Row - 1, int_Color_Row, dc.ColumnName.ToString(), int_sheet));

                                }
                            }
                        }
                        dtSRC = common_Data.DeleteRowFromDatatable(dtSRC, dr);
                        dtTRG = common_Data.DeleteRowFromDatatable(dtTRG, dataRows_TRG);
                    }
                    else
                    {
                        dtSRCOnly.Rows.Add(dr.ItemArray);
                    }
                }

                percentage = 100;
                backgroundWorker1.ReportProgress(++percentage);
                
                DisplayDebugMsg("Main Loop Comparison Done");

                dtSummary = common_Data.GenerateTransposeTable(dtSummary);

                if (boolInclude)
                {
                    foreach (DataRow dr in dtSummary.Rows.Cast<DataRow>().ToArray())
                    {
                        if (Convert.ToInt32(dr["Summarize Count"].ToString()) == 0 &&
                            (!(strPKList.Any(a => string.Equals(a, dr["Column Names"].ToString(), StringComparison.OrdinalIgnoreCase)))))
                        {
                            if (dtUnmatched.Columns.Contains(dr["Column Names"].ToString()))
                            {
                                dtUnmatched.Columns.Remove(dr["Column Names"].ToString());
                                //dataGridView11.Columns.Remove(dr["Column Names"].ToString());
                            }
                            dr.Delete();
                        }
                    }
                }

                backgroundWorker1.ReportProgress(++percentage);
                DisplayDebugMsg("Transpose of summary and initialisation is done");

                Logs.EnterLogsWithTime($"Data in dtUnmatched datatable :Rows : {dtUnmatched.Rows.Count}, Columns : {dtUnmatched.Columns.Count}");

                Logs.EnterLogsWithTime($"Data in dtSRCOnly datatable :Rows : {dtSRCOnly.Rows.Count}, Columns : {dtSRCOnly.Columns.Count}");

                Logs.EnterLogsWithTime($"Data in dtTRG datatable :Rows : {dtTRG.Rows.Count}, Columns : {dtTRG.Columns.Count}");

                Logs.EnterLogsWithTime($"Data in dtSummary datatable :Rows : {dtSummary.Rows.Count}, Columns : {dtSummary.Columns.Count}");

                if (Properties.Settings.Default.EPPlus_Excel_Save)
                {
                    //var r1 = colorPairs.AsQueryable().Skip(1020300).Take(5);

                    dtUnmatched.Columns.Add("DBType", typeof(string)).SetOrdinal(0);

                    bool boolToggle = true;

                    foreach (DataRow dr in dtUnmatched.Rows)
                    {
                        if (boolToggle)
                        {
                            dr["DBType"] = "Source";
                            boolToggle = false;
                        }
                        else
                        {
                            dr["DBType"] = "Target";
                            boolToggle = true;
                        }
                    }

                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        Logs.EnterLogsWithTime($"Saving File with EPPlus started under PK");

                        dsUnmatched = new DataSet();
                        queryColorpairMS = colorPairsMultiSheet.AsQueryable<ColorPairMultiSheet>();
                        dsUnmatched = WriteExcel.ExportToExcelNoLimitRows(dtUnmatched, "Mismatch", queryColorpairMS);

                        Logs.EnterLogsWithTime($"Mismatch Table loaded");
                        DisplayDebugMsg("First Workseet Written");

                        #region coloring
                        queryColorpair = colorPairs.AsQueryable<ColorPair>();

                        worksheet = WriteExcel.ColorDataset(ref worksheet,ref dsUnmatched,ref strPKList,ref queryColorpairMS, ref backgroundWorker1,ref e, pck);

                        #endregion
                        dtSRCOnly.TableName = "Source Rows Only";

                        worksheet = common_Data.AddNewWorksheet(pck, worksheet, dtSRCOnly);
                        worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                        DisplayDebugMsg("Second worksheet written");

                        worksheet = common_Data.AddNewWorksheet(pck, worksheet, dtTRG, "Target Rows Only");
                        worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                        DisplayDebugMsg("Third worksheet written");

                        worksheet = common_Data.AddNewWorksheet(pck, worksheet, dtSummary, "Summary");
                        worksheet.Cells[2, 1, strPKList.Count + 1, 1].Style.Font.Color.SetColor(Color.Red);

                        foreach (var cell in worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column])
                        {
                            cell.Value = Convert.ToInt32(cell.Value);
                        }

                        backgroundWorker1.ReportProgress(++percentage);
                        DisplayDebugMsg("Fourth and last worksheet written");

                        //strFileNameWithPath = $@"{strFolderPath}\{dtSRC.TableName}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                        pck.SaveAs(new System.IO.FileInfo(saveFileDialogFile.Str_saveFileNameWithPath));
                        boolStatus = true;
                        //DisplayMessage($"File Saved : {strFileNameWithPath}");
                        Logs.EnterLogsWithTime($"Saving File with EPPlus ended under PK");
                    }
                }
                else if (Properties.Settings.Default.XL_Excel_Save)
                {
                    #region Old excel saving code
                    Logs.EnterLogsWithTime($"Saving File with XL Started");
                    wb = new XLWorkbook();

                    wb.Worksheets.Add(dtUnmatched, "Mismatch");

                    wb.Worksheets.Add(dtSRCOnly, "Rows in first excelsheet");
                    wb.Worksheets.Add(dtTRG, "Rows in second excelsheet");
                    wb.Worksheets.Add(dtSummary, "Summarised");

                    backgroundWorker1.ReportProgress(106);

                    //formatExcelTableByDataGridView(1, dataGridView11, dtUnmatched, clb_reference_ID.CheckedItems, "First Excel", "Second Excel");

                    formatExcelTableByColorPair(1, dtUnmatched, clb_reference_ID.CheckedItems, "First Excel", "Second Excel");

                    formatExcelTable(2, dtSRCOnly);
                    formatExcelTable(3, dtTRG);
                    formatExcelTable(4, dtSummary);

                    backgroundWorker1.ReportProgress(107);

                    wb.SaveAs(saveFileDialogFile.Str_saveFileNameWithPath);

                    backgroundWorker1.ReportProgress(108);
                    #endregion end code
                    Logs.EnterLogsWithTime($"Saving File with XL Ended");
                    boolStatus = true;
                }
                #endregion

            }
            catch (Exception ex)
            {
                myException_BG = ex;
                boolStatus = false;
            }

        }
        //private ExcelWorksheet AddNewWorksheet(ExcelPackage pck, ExcelWorksheet worksheet, DataTable dt, string strTableName = null)
        //{
        //    try
        //    {
        //        if (strTableName != null)
        //        {
        //            dt.TableName = strTableName;
        //        }

        //        worksheet = pck.Workbook.Worksheets.Add(dt.TableName);
        //        worksheet.Cells["A1"].LoadFromDataTable(dt, true);
        //        worksheet.Cells.AutoFitColumns(22, 55);

        //        return worksheet;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        private void DisplayDebugMsg(string v)
        {
            try
            {
                Logs.EnterLogsWithTime($"{v}");
                if(backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.ReportProgress(105, (object)v);
                }
                if (GlobalDebug.boolIsGlobalDebug)
                {
                    //DisplayMessage(v);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        //private DataTable DeleteRowFromDatatable(DataTable dt, DataRow dr)
        //{
        //    try
        //    {
        //        dt.Rows.Remove(dr);
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
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

        private void formatExcelTableByColorPair(int int_sheet_no, DataTable dt,
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

                for (int row = 1; row <= dt.Rows.Count; row++)
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

                for (int col = 1; col <= dt.Columns.Count + 1; col++)
                {
                    cellValue = wb.Worksheet(int_sheet_no).Cell(1, col + 1).Value.ToString();
                    if (cic.Contains(cellValue))
                    {
                        wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Font.SetFontColor(XLColor.Red);
                    }

                    for (int row = 1; row <= dt.Rows.Count + 1; row++)
                    {
                        wb.Worksheet(int_sheet_no).Cell(row, col).Style.Fill.SetBackgroundColor(XLColor.Transparent);
                        wb.Worksheet(int_sheet_no).Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        cellValue = wb.Worksheet(int_sheet_no).Cell(row, col).Value.ToString();
                        boolCellValueIntType = int.TryParse(cellValue, out int res);
                        if (boolCellValueIntType)
                        {
                            wb.Worksheet(int_sheet_no).Cell(row, col).SetDataType(XLDataType.Number);
                        }
                    }

                }

                queryColorpair = colorPairs.AsQueryable<ColorPair>();

                int wbCol = cic.Count + 1;

                foreach (DataColumn dc in dt.Columns)
                {
                    var colInCP = queryColorpair.Where(x => x.str_Col_Name.ToUpper() == dc.ColumnName.ToString().ToUpper());
                    wbCol = cic.Count + 1 + dc.Ordinal;
                    foreach (var item in colInCP)
                    {
                        wb.Worksheet(1).Cell(item.int_Row, wbCol).Style.Fill.BackgroundColor = XLColor.Yellow;
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
            try
            {
                if (myException != null && myException.InnerException != null)
                {
                    lbl_Status.Text = myException.Message.ToString();
                    Logs.EnterLogsWithTime(myException.ToString());
                }
                if (e.ProgressPercentage <= 100 & e.ProgressPercentage > 0)
                {
                    progressBar1.Value = e.ProgressPercentage;
                    //lbl_Percentage.Text = $"{e.ProgressPercentage} % and Compared : {int_Count_Comparison}";
                    lbl_Percentage.Text = $"Progress....{e.ProgressPercentage} % ";
                    //lbl_Percentage.Update();
                }
                else if (e.ProgressPercentage >= 100 && e.ProgressPercentage <= 200)
                {
                    progressBar1.Value = e.ProgressPercentage - 100;
                    switch (e.ProgressPercentage)
                    {
                        case 101:
                            lbl_Percentage.Text = "Writing Excel File";
                            break;

                        case 102:
                            lbl_Percentage.Text = "Formatting Excel File";
                            break;

                        case 103:
                            lbl_Percentage.Text = "Saving Excel File";
                            break;

                        case 104:
                            lbl_Percentage.Text = "File Saved Successfully";
                            break;

                        case 105:
                            lbl_Status.Text = e.UserState.ToString();
                            break;

                    }
                }
                else if (e.ProgressPercentage == 500)
                {
                    lbl_Timer.Text = "This is debug mode";
                    lbl_Timer.Update();
                }
                else if (e.ProgressPercentage == -1)
                {
                    BackGroundWorkerUserObjectForReport backGroundWorkerUserObjectForReport = (BackGroundWorkerUserObjectForReport)e.UserState;

                    if (backGroundWorkerUserObjectForReport.strMsg != null)
                    {
                        lbl_Status.Text = backGroundWorkerUserObjectForReport.strMsg;
                        Logs.EnterLogsWithTime(backGroundWorkerUserObjectForReport.strMsg);
                    }

                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                lbl_Status.ForeColor = Color.Red;
                if (e.Cancelled)
                {
                    lbl_Status.Text = "Process Cancelled";
                    Logs.EnterLogsWithTime(lbl_Status.Text);
                    progressBar1.Value = 0;
                    lbl_Percentage.Text = "0 % and Processed cancelled";
                    if (myException_BG != null && myException_BG.StackTrace != null)
                    {
                        DisplayMessage(myException_BG.Message.ToString(), true);
                        Logs.EnterLogsWithTime(myException_BG.ToString());
                    }
                }
                else if (boolStatus)
                {
                    lbl_Status.ForeColor = Color.Black;
                    llbl_filePath.Visible = true;
                    label1.Visible = true;

                    if (!(Debugger.IsAttached))
                    {
                        Process.Start(saveFileDialogFile.Str_saveFileNameWithPath);
                    }

                    progressBar1.Value = 100;
                    llbl_filePath.Text = saveFileDialogFile.Str_saveFileNameWithPath;
                    lbl_Status.Text = $"File Opened : {saveFileDialogFile.Str_saveFileNameWithPath}";
                    Logs.EnterLogsWithTime(lbl_Status.Text);
                    lbl_Percentage.Text = $"File Successfully written and opened";
                }
                else
                {
                    if (myException != null && myException.StackTrace != null)
                    {
                        lbl_Status.Text = myException.Message.ToString();
                        Logs.EnterLogsWithTime(myException.ToString());
                    }

                    if (myException_BG != null && myException_BG.StackTrace != null)
                    {
                        lbl_Status.Text = myException_BG.Message.ToString();
                        Logs.EnterLogsWithTime(myException_BG.ToString());
                    }
                    //Logs.EnterLogsWithTime(myException_BG.ToString());
                }
                //showElapsedTime();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                changeEnabledStatus(true);
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                lbl_Timer.Text = DateTime.Now.Subtract(dateTime1).TotalSeconds.ToString();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
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
                if (backgroundWorkerInstrument.IsBusy)
                {
                    backgroundWorkerInstrument.CancelAsync();
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
                int test = 0;
                for (int i = 0; i < 100; i++)
                {
                    if (i > 50)
                    {
                        continue;
                        test = i;
                    }
                    test = i;
                }

                Console.WriteLine(test);

                int test1 = 0;
                for (int i = 0; i < 100; i++)
                {
                    if (i > 50)
                    {
                        test1 = i;
                        break;
                    }
                    test1 = i;
                }

                Console.WriteLine(test1);

                //changeEnabledStatus(false);
                //dateTime1 = DateTime.Now;

                //lblError.Text = string.Empty;
                //lbl_Status.Text = "Process start";
                //strFileNameWithPath = llbl_filePath.Text + @"\" + tb_fileName.Text + @".xlsx";
                //if (!bgw_multiThreading.IsBusy)
                //{
                //    bgw_multiThreading.RunWorkerAsync();
                //}
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void bgw_multiThreading_DoWork(object sender, DoWorkEventArgs e)
        {
            ////int progress = 0;
            //int totalProgress;//, percentage;

            //dateTime1 = DateTime.Now;
            //DataGridView dataGridView11 = new DataGridView();

            //try
            //{
            //    if (boolValidate)
            //    {
            //        dt_secondTable.CaseSensitive = true;
            //        dv_secondTableDataView = new DataView(dt_secondTable);

            //        dt_firstTableOnly = dt_firstTable.Clone();
            //        dt_secondTableOnly = dt_secondTable.Copy();

            //        //int cellCol, headRow, alternateRow;
            //        //headRow = -1;
            //        //cellCol = alternateRow = 0;
            //        //string str_colName;
            //        //bool newRow = true;

            //        dt_unmatched_Capture = dt_firstTable.Clone();

            //        dt_summarised_difference = dt_firstTable.Clone();

            //        foreach (DataColumn dc in dt_firstTable.Columns)
            //        {
            //            dt_summarised_difference.Columns[dc.ToString()].DataType = typeof(Int32);
            //        }
            //        DataRow dr_summarised = dt_summarised_difference.NewRow();
            //        foreach (DataColumn dc in dt_firstTable.Columns)
            //        {
            //            dr_summarised[dc.ToString()] = 0;
            //        }
            //        dt_summarised_difference.Rows.Add(dr_summarised);

            //        totalProgress = dt_firstTable.Rows.Count;
            //        string colNameToAdd;
            //        for (int i = 0; i < dt_unmatched_Capture.Columns.Count; i++)
            //        {
            //            colNameToAdd = dt_unmatched_Capture.Columns[i].ColumnName.ToString();
            //            dataGridView11.Columns.Add(colNameToAdd, colNameToAdd);
            //        }

            //        #region multithreading tak started

            //        //ParameterizedThreadStart pts = new ParameterizedThreadStart(multiThreadingParameter);
            //        //Thread t = new Thread(multiThreadingParameter);
            //        //t.Start();

            //        //Thread tc = Thread.CurrentThread;
            //        //Thread.Sleep(10000);


            //        //multiThreading(1, dt_firstTable, dt_secondTable, dataGridView11, e);

            //        #endregion

            //        #region for deleting columns which are not required

            //        int index = 0;
            //        int adjust = 0;
            //        foreach (DataColumn dc in dt_summarised_difference.Columns)
            //        {
            //            index = dc.Ordinal - adjust;
            //            if (!(clb_reference_ID.GetItemChecked(clb_reference_ID.FindString(dc.ColumnName.ToString()))))
            //            {
            //                if (Convert.ToInt32(dt_summarised_difference.Rows[0][dc.ColumnName.ToString()]) == 0)
            //                {
            //                    dt_unmatched_Capture.Columns.Remove(dc.ColumnName);
            //                    dataGridView11.Columns.Remove(dc.ColumnName.ToString());
            //                    adjust++;
            //                }
            //            }
            //        }

            //        #endregion

            //        bgw_multiThreading.ReportProgress(105);

            //        dt_transpose = common_Data.GenerateTransposeTable(dt_summarised_difference);
            //        wb = new XLWorkbook();

            //        wb.Worksheets.Add(dt_unmatched_Capture, "Mismatch");

            //        wb.Worksheets.Add(dt_firstTableOnly, "Rows in first excelsheet");
            //        wb.Worksheets.Add(dt_secondTableOnly, "Rows in second excelsheet");
            //        wb.Worksheets.Add(dt_transpose, "Summarised");

            //        bgw_multiThreading.ReportProgress(106);

            //        formatExcelTableByDataGridView(1, dataGridView11, dt_unmatched_Capture, clb_reference_ID.CheckedItems, "First Excel", "Second Excel");

            //        formatExcelTable(2, dt_firstTableOnly);
            //        formatExcelTable(3, dt_secondTableOnly);
            //        formatExcelTable(4, dt_transpose);

            //        bgw_multiThreading.ReportProgress(107);

            //        wb.SaveAs(fileNameWithPath);

            //        bgw_multiThreading.ReportProgress(108);

            //        boolStatus = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    myException = ex;
            //    boolStatus = false;
            //    //DisplayError(ex);
            //}
            //finally
            //{
            //    //changeEnabledStatus(true);
            //}
        }

        public void multiThreadingParameter()
        {
            //Thread.Sleep(15000);
        }

        private void multiThreading(int int_status, DataTable dt_source, DataTable dt_target, DataGridView dgv, DoWorkEventArgs e)
        {
            //try
            //{

            //    if (int_status == 1)
            //    {
            //        int percentage, progress = 0;
            //        int totalProgress = dt_source.Rows.Count;
            //        int cellCol, headRow, alternateRow;
            //        string str_colName;
            //        bool newRow = true;

            //        headRow = -1;
            //        cellCol = alternateRow = 0;

            //        //dt_unmatched_Capture = dt_source.Clone();
            //        //dt_summarised_difference = dt_source.Clone();


            //        foreach (DataRow dr in dt_source.Rows)
            //        {
            //            percentage = (((++progress) * 100) / totalProgress);
            //            bgw_multiThreading.ReportProgress(percentage);

            //            if (bgw_multiThreading.CancellationPending)
            //            {
            //                e.Cancel = true;
            //                bgw_multiThreading.CancelAsync();
            //                return;
            //            }

            //            str_RowFilter = generateRowFilterString(dr);
            //            dv_secondTableDataView.RowFilter = str_RowFilter;

            //            if (dv_secondTableDataView.Count > 0)
            //            {
            //                delete_current_row_from_second_table();

            //                CheckedListBox.CheckedItemCollection cic_firsttableColumns = clb_firstExcel.CheckedItems;
            //                CheckedListBox.CheckedItemCollection cic_reference = clb_reference_ID.CheckedItems;
            //                foreach (var a in cic_firsttableColumns)
            //                {
            //                    if (!(cic_reference.Contains(a)))
            //                    {
            //                        if (!(dr[a.ToString()].ToString() == dv_secondTableDataView[0][a.ToString()].ToString()))
            //                        {
            //                            cellCol = dt_firstTable.Columns[a.ToString()].Ordinal;
            //                            str_colName = a.ToString();
            //                            if (newRow)
            //                            {
            //                                dt_unmatched_Capture.Rows.Add(dr.ItemArray);
            //                                dt_unmatched_Capture.Rows.Add(dv_secondTableDataView[0].Row.ItemArray);
            //                                headRow += 2;
            //                                alternateRow += 2;
            //                                newRow = false;

            //                                //dataGridView11.DataSource = dt_unmatched_Capture;
            //                                dgv.Rows.Add(dt_unmatched_Capture.Rows[0].ItemArray);
            //                                dgv.Rows.Add(dt_unmatched_Capture.Rows[1].ItemArray);
            //                            }

            //                            dgv.Rows[headRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;
            //                            dgv.Rows[alternateRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;

            //                            dt_summarised_difference.Rows[0][str_colName] =
            //                                    Convert.ToInt32((dt_summarised_difference.Rows[0][str_colName]).ToString()) + 1;
            //                        }
            //                    }
            //                }
            //                newRow = true;
            //            }
            //            else
            //            {
            //                dt_firstTableOnly.Rows.Add(dr.ItemArray);
            //            }

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    myException = ex;
            //}
        }
        private void bgw_multiThreading_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (myException.InnerException != null)
                {
                    lbl_Status.Text = myException.Message.ToString();
                    Logs.EnterLogsWithTime(lbl_Status.Text);
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
                DisplayError(ex);
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
                Logs.EnterLogsWithTime(lbl_Status.Text);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                changeEnabledStatus(true);
            }
        }

        private void btn_RuleForm_Click(object sender, EventArgs e)
        {
            try
            {
                common_Data.OpenRuleForm();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            try
            {
                //checkRules.PNIDVariousRuleChecks(ds.Tables[0], ds.Tables[1]);
                changeEnabledStatus(false);
                dateTime1 = DateTime.Now;

                DisplayError();
                DisplayMessage("Instrument File Creation Process Started");

                if (!backgroundWorkerInstrument.IsBusy)
                {
                    backgroundWorkerInstrument.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void Test_SYSTEM_OUT_OF_MEMORYEXCEPTION()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //using (ExcelPackage pck = new ExcelPackage(ms))
                    //{
                    //    pck.SaveAs(new FileInfo($@"C:\Users\Krishna Bhunia\Desktop\LogFolder\Test.xlsx"));
                    //    Logs.EnterLogsWithTime($"Saving File with EPPlus started under PK");
                    //    worksheet = pck.Workbook.Worksheets.Add("Mismatch");
                    //    worksheet.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
                    //    worksheet.Cells["A1"].LoadFromDataTable(ds.Tables[1], true);
                    //}

                    //ExcelPackage excel = new ExcelPackage();

                    //// name of the sheet
                    //var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                    //excel.SaveAs(new FileInfo($@"C:\Users\Krishna Bhunia\Desktop\LogFolder\Test.xlsx"));

                    //workSheet.Cells["A1"].LoadFromDataTable(ds.Tables[0],true);
                    //excel.Save();

                    //workSheet = excel.Workbook.Worksheets.Add("Sheet2");
                    //workSheet.Cells["A1"].LoadFromDataTable(ds.Tables[1], true);

                    //excel.Save();
                    //excel.Dispose();
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
        private void btn_TestSystemMemoryOutException_Click(object sender, EventArgs e)
        {
            try
            {
                Test_SYSTEM_OUT_OF_MEMORYEXCEPTION();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }

        }

        private void DisplayMessage(string str_Msg, bool boolError = false)
        {
            try
            {
                lbl_Status.Text = str_Msg;
                lbl_Status.ForeColor = Color.Black;
                if (boolError)
                {
                    lbl_Status.ForeColor = Color.Red;
                }
                lbl_Status.Update();
                Logs.EnterLogsWithTime(lbl_Status.Text);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void backgroundWorkerInstrument_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                PNIDVariousRuleChecks(ds.Tables[0], ds.Tables[1], e);
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            try
            {
                showElapsedTime();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void backgroundWorkerInstrument_ProgressChanged(object sender, ProgressChangedEventArgs e)
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
                    lbl_Percentage.Text = "Processing " + e.ProgressPercentage + " % ";
                }
                else if (e.ProgressPercentage >= 100)
                {
                    progressBar1.Value = 100;
                    //switch (e.ProgressPercentage)
                    //{
                    //    case 105:
                    //        lbl_Percentage.Text = "Writing Excel File";
                    //        break;

                    //    case 106:
                    //        lbl_Percentage.Text = "Formatting Excel File";
                    //        break;

                    //    case 107:
                    //        lbl_Percentage.Text = "Saving Excel File";
                    //        break;

                    //    case 108:
                    //        lbl_Percentage.Text = "File Saved Successfully";
                    //        break;
                    //}
                }

                showElapsedTime();
                Logs.EnterLogsWithTime(lbl_Status.Text);
            }
            catch (Exception ex)
            {
                myException = ex;
            }
        }

        private void backgroundWorkerInstrument_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                string str_InputFileName = "Instrumentation Analysis " + DateTime.Now.ToString().Replace(':', '_').Replace('-', '_').Replace('\\', '_').Replace('/', '_');
                SaveFileDialogFile saveFileDialogFile = new SaveFileDialogFile();
                if (boolInstrumentFileGenerationStatus)
                {
                    saveFileDialogFile.SaveFileDialogExcelFileForDataset(dt_Instrument_Input_File.DataSet, "Save Excel File For Instrumentation", str_InputFileName);
                }

                if (e.Cancelled)
                {
                    lbl_Status.Text = "Process Cancelled by user";
                    progressBar1.Value = 0;
                    lbl_Percentage.Text = "0 % and Processed cancelled by user";
                }
                else if (boolStatus & boolInstrumentFileGenerationStatus)
                {
                    lbl_Status.Text = "Opening Instrument Generated File : " + saveFileDialogFile.Str_saveFileNameWithPath;
                    progressBar1.Value = 100;
                    lbl_Percentage.Text = "100 % Done";
                    if (saveFileDialogFile.BoolFileSaveStatus)
                    {
                        Process.Start(saveFileDialogFile.Str_saveFileNameWithPath);
                    }
                }
                else if (myException != null && myException.InnerException != null)
                {
                    lbl_Status.Text = myException.Message.ToString();
                }
                else
                {
                    lbl_Status.Text = "Cancelled";
                    progressBar1.Value = 0;
                    lbl_Percentage.Text = "0 % and Processed cancelled";
                }
                showElapsedTime();
                Logs.EnterLogsWithTime(lbl_Status.Text);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                changeEnabledStatus(true);
            }
        }

        private void llbl_filePath_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip.SetToolTip(llbl_filePath, llbl_filePath.Text);
                toolTip.Show(llbl_filePath.Text, this, 30000);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_IncludeRule_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void DisplayError(Exception ex = null)
        {
            try
            {
                if (ex != null)
                {
                    lblError.Visible = true;
                    lblError.ForeColor = Color.Red;
                    if (Debugger.IsAttached)
                    {
                        lblError.Text = ex.ToString();
                    }
                }
                else
                {
                    lblError.ResetText();
                }
                lblError.Update();
            }
            catch (Exception ex1)
            {
                lblError.Text = ex1.Message.ToString();
            }
            Logs.EnterLogsForError(ex);
        }

        public bool PNIDVariousRuleChecks(DataTable dt_Source, DataTable dt_Target, DoWorkEventArgs e)
        {
            //int int_test_Count = 0;
            DataView dvPNIDSource;
            //DataView dvPNIDTarget;
            //string[] str_item_tag;

            string str_plant_group_name_src = string.Empty;
            string str_measured_variable_src = string.Empty;
            string str_tag_seq_no_src = string.Empty;
            string str_intru_type_modifier_src = string.Empty;
            string str_tag_suffix_src = string.Empty;
            string str_short_item_tag_src = string.Empty;
            string str_industrial_complex_no_src = string.Empty;
            string str_process_area_src = string.Empty;
            string str_subprocess_src = string.Empty;
            string str_item_tag_src = string.Empty;
            string str_required_short_item_tag = string.Empty;

            string str_plant_group_name_trg = string.Empty;
            string str_measured_variable_trg = string.Empty;
            string str_tag_seq_no_trg = string.Empty;
            string str_intru_type_modifier_trg = string.Empty;
            string str_tag_suffix_trg = string.Empty;
            string str_short_item_tag_trg = string.Empty;
            string str_item_tag_trg = string.Empty;
            string str_required_item_tag = string.Empty;

            //int int_count_trg = 1;
            int int_suffix = 0;

            DataColumn dc_required_Item_Tag = new DataColumn("Required_Item_Tag", typeof(string));
            int int_getOrdinal = dt_Source.Columns["Item Tag"].Ordinal;
            if (!(dt_Source.Columns.Contains(dc_required_Item_Tag.ColumnName)))
            {
                dt_Source.Columns.Add(dc_required_Item_Tag);
                dc_required_Item_Tag.SetOrdinal(int_getOrdinal);
            }

            DataSet ds_Unmatched = new DataSet();
            DataTable dt_PNIDUnmatched_Source = new DataTable();
            DataTable dt_PNIDUnmatched_Target = new DataTable();

            DataSet ds1 = new DataSet();
            ds1.Tables.Add(dt_Source.Copy());
            ds1.Tables.Add(dt_Target.Copy());
            DataTable dt_duplicate;

            int int_progress_count = 0;

            try
            {
                boolInstrumentFileGenerationStatus = false;
                #region CONVERTING Tag Seq No INTO 4 DIGITS

                foreach (DataRow dr in dt_Source.Rows)
                {
                    sendProgressForInstrumentation(++int_progress_count, dt_Source.Rows.Count, e);
                    if (boolCancelInstrumentProcess)
                    {
                        return boolInstrumentFileGenerationStatus;
                    }
                    if (dr["Tag Seq No"].ToString().Trim().Length != 0)
                    {
                        dr["Tag Seq No"] = dr["Tag Seq No"].ToString().PadLeft(4, '0');
                    }
                    else
                    {
                        dr["Tag Seq No"] = "";
                    }
                }

                #endregion

                #region creation of plant group name

                if (!(dt_Source.Columns.Contains("Plant Group Name")))
                {
                    dt_Source.Columns.Add("Plant Group Name", typeof(string)).SetOrdinal(dt_Source.Columns["Item Tag"].Ordinal);
                    foreach (DataRow dr in dt_Source.Rows)
                    {
                        dr["Plant Group Name"] = $"{dr["Industrial complex (site and unit)"]}-{dr["Process area"]}-{dr["Subprocess"]}";
                    }
                }

                #endregion

                #region creation of required short item tag

                if (!(dt_Source.Columns.Contains("Required Short ItemTag")))
                {
                    dt_Source.Columns.Add("Required Short ItemTag", typeof(string)).SetOrdinal(dt_Source.Columns["Short ItemTag"].Ordinal);
                    //foreach (DataRow dr in dt_Source.Rows)
                    //{
                    //    dr["Required Short ItemTag"] = $"{dr["Industrial complex (site and unit)"]}-{dr["Process area"]}-{dr["Subprocess"]}";
                    //}
                }

                #endregion 

                //string str_Instrument_Class = "Control valves and regulators";

                dt_PNIDUnmatched_Source = dt_Source.Clone();
                dt_PNIDUnmatched_Target = dt_Target.Clone();

                #region checking required item tag and instrument class for Control valves and regulators

                dvPNIDSource = dt_Source.DefaultView;
                dvPNIDSource.Sort = "[Item Tag] ASC";

                if (dt_Source.Columns.Contains("Item Tag") && dt_Source.Columns.Contains("Instrument Class"))
                {
                    dvPNIDSource.RowFilter = $"[Instrument Class] = 'Control valves and regulators'";

                    int_progress_count = 0;
                    foreach (DataRowView drv in dvPNIDSource)
                    {
                        sendProgressForInstrumentation(++int_progress_count, dvPNIDSource.Count, e);

                        if (boolCancelInstrumentProcess)
                        {
                            return boolInstrumentFileGenerationStatus;
                        }

                        str_required_item_tag = $"{drv["Plant Group Name"]}-{drv["Measured Variable Code"]}{drv["Tag Seq No"]}-{drv["Measured Variable Code"]}{drv["Instr Type Modifier"]}";
                        drv["Required_Item_Tag"] = str_required_item_tag;
                        str_required_short_item_tag = $"{drv["Measured Variable Code"]}{drv["Tag Seq No"]}";
                        drv["Required Short ItemTag"] = str_required_short_item_tag;
                    }
                }

                #endregion

                #region check for instrument class for "Other in-line instruments"

                dvPNIDSource = dt_Source.DefaultView;
                if (dt_Source.Columns.Contains("Item Tag") && dt_Source.Columns.Contains("Instrument Class"))
                {
                    dvPNIDSource.RowFilter = $"[Instrument Class] = 'Other in-line instruments'";
                }

                string[] str_measurable_codes = { "A", "P", "L", "F", "T" };

                dt_PNIDUnmatched_Target.Clear();
                dt_PNIDUnmatched_Target.TableName = "Unmatched in Other in-line instruments";
                int_suffix = 0;

                DataView dv_duplicate = dt_Source.DefaultView;

                dvPNIDSource.Sort = "[Item Tag] ASC";

                int_progress_count = 0;
                foreach (DataRowView drv in dvPNIDSource)
                {
                    sendProgressForInstrumentation(++int_progress_count, dvPNIDSource.Count, e);

                    if (boolCancelInstrumentProcess)
                    {
                        return boolInstrumentFileGenerationStatus;
                    }

                    str_measured_variable_src = drv["Measured Variable Code"].ToString();

                    if (str_measured_variable_src.Length >= 1)
                    {
                        if (str_measurable_codes.Contains(str_measured_variable_src.Substring(0, 1)))
                        {
                            str_required_item_tag = $"{drv["Plant Group Name"]}-{drv["Measured Variable Code"]}{drv["Tag Seq No"]}-{drv["Measured Variable Code"]}T";
                        }
                        else if (!(str_measurable_codes.Contains(str_measured_variable_src.Substring(0, 1))))
                        {
                            str_required_item_tag = $"{drv["Plant Group Name"]}-{drv["Measured Variable Code"]}{drv["Tag Seq No"]}-{drv["Measured Variable Code"]}{drv["Instr Type Modifier"]}";
                        }

                        if (str_required_item_tag.Length > 4)
                        {
                            drv["Required_Item_Tag"] = str_required_item_tag;
                        }
                    }


                }

                #region for initialising duplicate values

                if (dt_Source.Columns.Contains("Item Tag") && dt_Source.Columns.Contains("Instrument Class"))
                {
                    dvPNIDSource.RowFilter = $"[Instrument Class] = 'Other in-line instruments'";
                }

                int_suffix = 0;
                dt_duplicate = dt_Source.Copy();
                dv_duplicate = dt_duplicate.DefaultView;

                int_progress_count = 0;
                foreach (DataRowView drv in dvPNIDSource)
                {
                    sendProgressForInstrumentation(++int_progress_count, dvPNIDSource.Count, e);

                    if (boolCancelInstrumentProcess)
                    {
                        return boolInstrumentFileGenerationStatus;
                    }

                    dv_duplicate.RowFilter = $"([Instrument Class] = 'Other in-line instruments') And [Item Tag] = '{drv["Required_Item_Tag"].ToString()}'";

                    if (dv_duplicate.Count > 1 && drv["Required_Item_Tag"].ToString().Length > 4)
                    {
                        int_suffix++;
                        str_required_item_tag = $"{drv["Required_Item_Tag"]}-{int_suffix}";
                    }
                    else if (drv["Required_Item_Tag"].ToString().Length > 4)
                    {
                        int_suffix = 0;
                        str_required_item_tag = $"{drv["Required_Item_Tag"]}";
                    }

                    if (str_required_item_tag.Length > 4 && drv["Tag Seq No"].ToString().Trim().Length >= 1)
                    {
                        drv["Required_Item_Tag"] = str_required_item_tag;
                    }

                    str_required_short_item_tag = $"{drv["Measured Variable Code"]}{drv["Tag Seq No"]}";
                    drv["Required Short ItemTag"] = str_required_short_item_tag;
                }
                #endregion

                #endregion

                #region check for instrument class for "Off-line instruments" or "System functions"

                dvPNIDSource = dt_Source.DefaultView;
                if (dt_Source.Columns.Contains("Item Tag") && dt_Source.Columns.Contains("Instrument Class"))
                {
                    dvPNIDSource.RowFilter = $"[Instrument Class] = 'Off-line instruments' Or [Instrument Class] = 'System functions'";
                    dvPNIDSource.Sort = "[Instrument Class] ASC";

                    int_suffix = 0;

                    dt_duplicate = dt_Source.Copy();

                    dv_duplicate = dt_duplicate.DefaultView;
                    int_progress_count = 0;
                    foreach (DataRowView drv in dvPNIDSource)
                    {
                        sendProgressForInstrumentation(++int_progress_count, dvPNIDSource.Count, e);
                        if (boolCancelInstrumentProcess)
                        {
                            return boolInstrumentFileGenerationStatus;
                        }
                        dv_duplicate.RowFilter = $"([Instrument Class] = 'Off-line instruments' Or [Instrument Class] = 'System functions') And [Item Tag] = '{drv["Item Tag"].ToString()}'";

                        if (dv_duplicate.Count > 1)
                        {
                            int_suffix++;
                            str_required_item_tag = $"{drv["Item Tag"]}-{int_suffix}";
                            str_required_short_item_tag = $"{drv["Measured Variable Code"]}{drv["Tag Seq No"]}-{int_suffix}";
                        }
                        else
                        {
                            int_suffix = 0;
                            str_required_item_tag = $"{drv["Item Tag"]}";
                            str_required_short_item_tag = $"{drv["Measured Variable Code"]}{drv["Tag Seq No"]}";
                        }

                        if (str_required_item_tag.Length > 4)
                        {
                            drv["Required_Item_Tag"] = str_required_item_tag;
                            drv["Required Short ItemTag"] = str_required_short_item_tag;
                        }
                    }

                }

                dt_Instrument_Input_File = dt_Source.Copy();
                ds_Instrument_Input_File = new DataSet();
                ds_Instrument_Input_File.Tables.Add(dt_Instrument_Input_File);

                boolInstrumentFileGenerationStatus = true;

                //SaveFileDialogFile saveFileDialogFile = new SaveFileDialogFile();
                //saveFileDialogFile.SaveFileDialogExcelFileForDataset(dt_Source.DataSet, "Save Excel File For Instrumentation", "Instrumentation Analysis");

                //if (saveFileDialogFile.BoolFileSaveStatus)
                //{
                //    Process.Start(saveFileDialogFile.Str_saveFileName);
                //}

                #endregion

            }
            catch (Exception)
            {
                throw;
            }
            return boolInstrumentFileGenerationStatus;
        }

        private bool sendProgressForInstrumentation(int progress, int totalProgress, DoWorkEventArgs e)
        {
            try
            {
                percentage = (((++progress) * 100) / totalProgress);
                backgroundWorkerInstrument.ReportProgress(percentage);

                if (backgroundWorkerInstrument.CancellationPending)
                {
                    e.Cancel = true;
                    backgroundWorkerInstrument.CancelAsync();
                    boolCancelInstrumentProcess = true;
                }
                else
                {
                    boolCancelInstrumentProcess = false;
                }
                return boolCancelInstrumentProcess;
            }
            catch (Exception)
            {
                boolCancelInstrumentProcess = true;
                boolInstrumentFileGenerationStatus = false;
                throw;
            }
        }

    }
}
