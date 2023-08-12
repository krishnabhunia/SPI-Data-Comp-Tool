using ClosedXML.Excel;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using form = System.Windows.Forms;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    public partial class SPIDeltaChangeTracker : Form
    {
        #region string variables

        private string str_oracle_dbSource = string.Empty;
        private string str_sql_dbSource = string.Empty;

        private string str_oracle_dbName = string.Empty;
        private string str_sql_dbName = string.Empty;

        private string str_oracle_Username = string.Empty;
        private string str_sql_Username = string.Empty;

        private string str_oracle_Password = string.Empty;
        private string str_sql_Password = string.Empty;

        private string strOracleConnection = string.Empty;
        private string strSQLConnection = string.Empty;
        private string strQuery = string.Empty;

        private string str_unique_reference_key = string.Empty;
        private string str_col_to_include_in_comparison = string.Empty;
        private string str_path = string.Empty;

        private string str_source_name_excel_sheet = string.Empty;

        private string strFilePathChangeSave = string.Empty;
        private string strSourceName = string.Empty;

        private string cellValue = string.Empty;

        private string str_to_fetch = string.Empty;
        private string strTargetName = string.Empty;

        private string str_Data_Source_To_Compare = string.Empty;
        private string str_Data_Target_To_Compare = string.Empty;
        private string str_CompleteTableName = string.Empty;
        private string str_tableName = string.Empty;
        private string str_colName = string.Empty;
        private string str_SRCTableName = string.Empty;
        private string str_TRGTableName = string.Empty;

        private string str_Where_Col, str_Where_Val_Oracle, str_Where_Val_SQL;

        #endregion

        #region Dataset and Datatable and Dataview

        private DataSet dsRules;

        private DataTable dt_src;
        private DataSet ds;
        private OracleDataAdapter oracleDA;

        private DataTable dt_trg;

        private DataSet ds_change;
        private DataTable dt_change;

        private DataSet ds_unmatched_Capture;
        private DataTable dt_unmatched_Capture;

        private DataTable dt_sql_only;
        private DataTable dt_oracle_only;
        private DataTable dt_summarised_difference;
        private DataTable dt_transpose;
        private DataTable dt_LoadSRC, dt_LoadTRG;
        private DataTable dt_returnDataTable;
        private DataTable dt_CommonSRC;
        private DataTable dt_CommonTRG;
        private DataTable dtSRC, dtTRG;


        private DataView dv_sql;
        private DataView dv_oracle;

        private Exception myException_BG;

        #endregion

        #region int and bool

        private bool boolStatus = true;
        private bool newRow;
        private int count_match = 0;
        private int count_unmatched = 0;

        private int progress, totalProgressCount;
        private int percentage;

        private int int_total_rows;
        private int int_total_columns;

        private bool boolCellValueIntType;
        private bool boolOracle, boolSQL, boolCompareResult;//, boolTest;
        private bool boolCompare_List;
        //private bool boolIgnoreCase;

        int cellCol;

        #endregion

        #region Excel related variables

        //private XLWorkbook xlworkBook;

        //private _Application excel = new _Excel.Application();

        private XLWorkbook wb;// = new XLWorkbook();

        #endregion

        #region other member variables
        private SqlDataAdapter sqlDA;
        private List<string> lst_col_to_ignore = new List<string>();
        private List<string> lst_col_to_include_in_comparison = new List<string>();
        private TimeSpan time_to_execute = new TimeSpan();
        private DateTime start_Time = new DateTime();
        private DateTime stop_Time = new DateTime();

        private Common_Data cd;// = new Common_Data();
        private OpenFileDialogData ofdd;// = new opOpenFileDialogData();
        private Common_Data common_Data;// = new Common_Data();
                                        //private CheckRules checkRules;

        //private DataSet dsAllColumns,
        private DataSet ds_CompleteSet, ds_Initial;
        #endregion

        private bool boolRules, boolRulesCheck;
        private ToolTip toolTip;// = new ToolTip();

        private OpenFileDialogData OpenFileDialogData_OBJ;
        private DataTable dt_SPIDeltaChangeTracker;

        private CheckedListBox.CheckedItemCollection cic_oracleTB;// = cbL_columns.CheckedItems;
        private CheckedListBox.CheckedItemCollection cic_reference;// = clb_ReferenceID.CheckedItems;
        private int int_Color_Row;
        private OpenFileDialogData openFileDialogData;
        private DataSet dsTableList;
        private bool boolExcelFileList;
        private DBConnectionDetails dBConnectionDetails;

        private DBConnectionStatus dbConnection, dbSRCConnection, dbTRGConnection;
        private string str_TableName, str_SRC_TableName, str_TRG_TableName;
        private DataTable dtFetch;
        private string strSelectCol;
        private string str_DBName;
        private bool boolToggle;
        private bool boolLinkLabel;

        private List<ColorPairMultiSheet> colorPairsMultiSheet;
        private int int_sheet;
        private DataSet dsUnmatched;
        private IQueryable<ColorPairMultiSheet> queryColorpairMS;
        private ExcelWorksheet worksheet;

        public SPIDeltaChangeTracker()
        {
            InitializeComponent();

            try
            {
                boolCompare_List = false;
                llbl_Folder_path.Text = @"D:\";

                lblExpressionStatus.Text = string.Empty;
                cd = new Common_Data();
                ofdd = new OpenFileDialogData();

                myException_BG = new Exception();
                common_Data = new Common_Data();

                boolRules = false;
                dsRules = new DataSet();
                //checkRules = new CheckRules();

                //dsAllColumns = new DataSet();
                boolLinkLabel = false;

                toolTip = new ToolTip();
                dt_LoadSRC = new DataTable();
                dt_LoadTRG = new DataTable();

                dt_returnDataTable = new DataTable();

                dt_CommonSRC = new DataTable();
                dt_CommonTRG = new DataTable();

                tb_FileName.Text = $"{common_Data.RemoveSpecialCharacters(cb_SRC_TableName.Text)}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}";

                OpenFileDialogData_OBJ = new OpenFileDialogData();

                dt_SPIDeltaChangeTracker = new DataTable();
                ds_CompleteSet = new DataSet();
                ds_Initial = new DataSet();

                openFileDialogData = new OpenFileDialogData();

                cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);

                if (cb_UserProfileList.Items.Count > 0)
                {
                    cb_UserProfileList.SelectedIndex = 0;
                    if (GlobalDebug.boolIsGlobalDebug || Debugger.IsAttached)
                    {
                        if (cb_UserProfileList.Items.Count >= 7)
                        {
                            cb_UserProfileList.SelectedIndex = 6;
                        }
                        //cb_ListProcess.CheckState = CheckState.Checked;
                    }
                }

                dbConnection = new DBConnectionStatus();
                dbSRCConnection = new DBConnectionStatus();
                dbTRGConnection = new DBConnectionStatus();
                boolToggle = true;

                if (GlobalDebug.ISGlobalDebug(Environment.UserName, Environment.UserName) || Debugger.IsAttached)
                {
                    gb_Debug.Visible = true;
                }

                if (Debugger.IsAttached)
                {
                    boolLinkLabel = true;
                }
                colorPairsMultiSheet = new List<ColorPairMultiSheet>();
                tabControl1.TabPages.RemoveAt(1);
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        delegate string DelGetTextValueOfComboBox(ComboBox comboBox);
        private string GetTextValueOfComboBox(ComboBox comboBox)
        {
            try
            {
                if (comboBox.InvokeRequired)
                {
                    DelGetTextValueOfComboBox delGetText = new DelGetTextValueOfComboBox(GetTextValueOfComboBox);
                    return this.Invoke(delGetText, new object[] { comboBox }).ToString();
                }
                else
                {
                    string strV = string.Empty;
                    if (comboBox.SelectedValue == null)
                    {
                        strV = comboBox.Text;
                    }
                    else
                    {
                        strV = comboBox.SelectedValue.ToString();
                    }
                    DisplayLog($"Fetched table from dropdown : {strV}");
                    return strV;
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
                throw ex;
            }
        }

        private DataTable Connect_SRC(string columnNames = " * ", bool boolDateFilter = false)
        {
            try
            {
                dt_src = new DataTable();
                //ds_Initial = new DataSet();
                str_SRCTableName = GetTextValueOfComboBox(cb_SRC_TableName);
                strQuery = $"select {columnNames} from {str_SRCTableName}";

                // oracle 11g and sql selection as requirement changed
                DisplayLog("Connecting Source");

                if (rb_SRC_oracle.Checked)
                {
                    if (boolDateFilter)
                    {
                        strQuery = $"select {columnNames} from {str_SRCTableName} Where {cb_DateCol.Text} >= '{dateTimePicker_Date.Value.ToString("dd-MMM-yy")}'";
                    }
                    DisplayLog("Connecting Oracle");
                    DisplayLog(strQuery);

                    strOracleConnection = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={ tb_SRC_DataSource.Text.Trim()})(PORT={ tb_SRC_DefaultPort.Text.Trim()}))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={tb_SRC_DBName.Text})));User Id={tb_SRC_UserName.Text.Trim()};Password={tb_SRC_Password.Text.Trim()};";

                    DisplayLog(strOracleConnection);

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    oracleDA.Fill(dt_src);
                    strSourceName = "Source Oracle 11G";
                    DisplayLog("Source Oracle connected successfully");
                }

                if (rb_SRC_SQL.Checked)
                {
                    DisplayLog("Connecting SQL");
                    if (boolDateFilter)
                    {
                        strQuery = $"select {columnNames} from {str_SRCTableName} Where {cb_DateCol.Text} >= '{dateTimePicker_Date.Value.ToString("yyyy-MM-dd")}'";
                    }
                    DisplayLog(strQuery);
                    if (cb_WindowAuthentication_TRG.CheckState == CheckState.Checked)
                    {
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    + "; Integrated Security = true ";
                    }
                    else if (cb_WindowAuthentication_TRG.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    + "; User ID = " + tb_SRC_UserName.Text + "; " + "Password = " + tb_SRC_Password.Text + ";";
                    }
                    DisplayLog(strSQLConnection);
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_src);
                    strSourceName = "Source SQL";
                    DisplayLog("Source SQL connected successfully");
                }
                //ds_Initial.Tables.Add(dt_src);
                //ds_Initial.Tables[0].TableName = strSourceName;
                //DisplayLog("Source Table Loading Finished");
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayLog(ex.Message.ToString());
                myException_BG = ex;
                boolStatus = false;
            }
            dt_src.TableName = strSourceName;
            return dt_src;
            //return ds_Initial;
        }

        private DataTable Connect_TRG(string columnNames = " * ")
        {
            try
            {
                dt_trg = new DataTable();

                str_TRGTableName = GetTextValueOfComboBox(cb_TRG_TableName);
                strQuery = "select " + columnNames + " from " + str_TRGTableName;

                DisplayLog("Connecting Target....");

                if (rb_TRG_Oracle.Checked)
                {
                    DisplayLog("Connecting Oracle");
                    strSQLConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_TRG_DataSource.Text + " )(PORT = " + tb_TRG_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_TRG_DBName.Text + ")));"
                        + "User Id=" + tb_TRG_UserName.Text + ";Password=" + tb_TRG_Password.Text + ";";

                    DisplayLog(strSQLConnection);
                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    //dt_trg = new DataTable();
                    oracleDA.Fill(dt_trg);
                    strTargetName = "Target Oracle 11G";
                    DisplayLog("Target Oracle Loaded successfully");
                    //ds.Tables.Add(dt_trg);
                }
                else if (rb_TRG_SQL.Checked)
                {
                    DisplayLog("Connecting SQL");
                    if (cb_WindowAuthentication_TRG.CheckState == CheckState.Checked)
                    {
                        strSQLConnection = "Data Source = " + tb_TRG_DataSource.Text + ";Initial Catalog = " + tb_TRG_DBName.Text
                        + "; Integrated Security = true ";
                    }
                    else if (cb_WindowAuthentication_TRG.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = "Data Source = " + tb_TRG_DataSource.Text + ";Initial Catalog = " + tb_TRG_DBName.Text
                        + "; User ID = " + tb_TRG_UserName.Text + "; " + "Password = " + tb_TRG_Password.Text + ";";
                    }
                    DisplayLog(strSQLConnection);

                    //dt_trg = new DataTable();
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_trg);
                    strTargetName = "Target SQL";
                    DisplayLog("Target SQL Loaded successfully");
                    //ds.Tables.Add(dt_trg);

                }
                //ds.Tables[1].TableName = "Target SQL Table";
                DisplayLog("Target Table Loading finished");
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayLog(ex.Message.ToString());
                myException_BG = ex;
                boolStatus = false;
            }
            dt_trg.TableName = strTargetName;
            return dt_trg;
        }

        private DataSet Connect_SRC_DB(string columnNames = " * ", bool boolDateFilter = false)
        {
            try
            {
                dt_src = new DataTable();
                ds_Initial = new DataSet();
                str_SRCTableName = GetTextValueOfComboBox(cb_SRC_TableName);
                strQuery = $"select {columnNames} from {str_SRCTableName}";

                // oracle 11g and sql selection as requirement changed
                DisplayLog("Connecting Source");

                if (rb_SRC_oracle.Checked)
                {
                    if (boolDateFilter)
                    {
                        strQuery = $"select {columnNames} from {str_SRCTableName} Where {cb_DateCol.Text} >= '{dateTimePicker_Date.Value.ToString("dd-MMM-yy")}'";
                    }
                    DisplayLog("Connecting Oracle");
                    DisplayLog(strQuery);

                    strOracleConnection = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={ tb_SRC_DataSource.Text.Trim()})(PORT={ tb_SRC_DefaultPort.Text.Trim()}))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={tb_SRC_DBName.Text})));User Id={tb_SRC_UserName.Text.Trim()};Password={tb_SRC_Password.Text.Trim()};";

                    DisplayLog(strOracleConnection);

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    oracleDA.Fill(dt_src);
                    strSourceName = "Source Oracle 11G";
                }

                if (rb_SRC_SQL.Checked)
                {
                    DisplayLog("Connecting SQL");
                    if (boolDateFilter)
                    {
                        strQuery = $"select {columnNames} from {str_SRCTableName} Where {cb_DateCol.Text} >= '{dateTimePicker_Date.Value.ToString("yyyy-MM-dd")}'";
                    }
                    DisplayLog(strQuery);
                    if (cb_WindowAuthentication_TRG.CheckState == CheckState.Checked)
                    {
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    + "; Integrated Security = true ";
                    }
                    else if (cb_WindowAuthentication_TRG.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    + "; User ID = " + tb_SRC_UserName.Text + "; " + "Password = " + tb_SRC_Password.Text + ";";
                    }
                    DisplayLog(strSQLConnection);
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_src);
                    strSourceName = "Source SQL";

                }
                ds_Initial.Tables.Add(dt_src);
                ds_Initial.Tables[0].TableName = strSourceName;
                DisplayLog("Source Table Loading Finished");
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayLog(ex.Message.ToString());
                myException_BG = ex;
                boolStatus = false;
            }
            //dt_src.TableName = strSourceName;
            //return dt_src;
            return ds_Initial;
        }

        private DataSet Connect_TRG_DB(string columnNames = " * ")
        {
            try
            {
                dt_trg = new DataTable();
                ds_Initial = new DataSet();
                str_TRGTableName = GetTextValueOfComboBox(cb_TRG_TableName);
                strQuery = "select " + columnNames + " from " + str_TRGTableName;

                DisplayLog("Connecting Target....");

                if (rb_TRG_Oracle.Checked)
                {
                    DisplayLog("Connecting Oracle");
                    strSQLConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_TRG_DataSource.Text + " )(PORT = " + tb_TRG_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_TRG_DBName.Text + ")));"
                        + "User Id=" + tb_TRG_UserName.Text + ";Password=" + tb_TRG_Password.Text + ";";

                    DisplayLog(strSQLConnection);
                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    oracleDA.Fill(dt_trg);
                    strTargetName = "Target Oracle 11G";
                    //ds.Tables.Add(dt_trg);
                }
                else if (rb_TRG_SQL.Checked)
                {
                    DisplayLog("Connecting SQL");
                    if (cb_WindowAuthentication_TRG.CheckState == CheckState.Checked)
                    {
                        strSQLConnection = "Data Source = " + tb_TRG_DataSource.Text + ";Initial Catalog = " + tb_TRG_DBName.Text
                        + "; Integrated Security = true ";
                    }
                    else if (cb_WindowAuthentication_TRG.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = "Data Source = " + tb_TRG_DataSource.Text + ";Initial Catalog = " + tb_TRG_DBName.Text
                        + "; User ID = " + tb_TRG_UserName.Text + "; " + "Password = " + tb_TRG_Password.Text + ";";
                    }
                    DisplayLog(strSQLConnection);

                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_trg);
                    strTargetName = "Target SQL";

                }
                DisplayLog("Target Table Loading finished");
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayLog(ex.Message.ToString());
                myException_BG = ex;
                boolStatus = false;
            }
            dt_trg.TableName = strTargetName;
            ds_Initial.Tables.Add(dt_trg);
            return ds_Initial;
        }


        private void deleteColumnInExcel(int int_sheet_no, int colNo)
        {
            try
            {
                wb.Worksheet(int_sheet_no).Column(colNo).Delete();
            }
            catch (Exception ex)
            {
                myException_BG = ex;
            }
        }

        private void generateTransposeTable()
        {
            try
            {
                dt_transpose = new DataTable();
                dt_transpose.Columns.Add(dt_summarised_difference.Columns[0].ColumnName.ToString());

                foreach (DataRow dr in dt_summarised_difference.Rows)
                {
                    string str_newColName = dr[0].ToString();
                    dt_transpose.Columns.Add(str_newColName);
                }

                for (int rCount = 1; rCount <= dt_summarised_difference.Columns.Count - 1; rCount++)
                {
                    DataRow dr = dt_transpose.NewRow();

                    dr[0] = dt_summarised_difference.Columns[rCount].ColumnName.ToString();

                    for (int cCount = 0; cCount <= dt_summarised_difference.Rows.Count - 1; cCount++)
                    {
                        string str_colValue = dt_summarised_difference.Rows[cCount][rCount].ToString();
                        dr[cCount + 1] = str_colValue;
                    }
                    dt_transpose.Rows.Add(dr);
                }
                dt_transpose.Columns[0].ColumnName = "Column Names";
                dt_transpose.Columns[1].ColumnName = "Summarize Count";

            }
            catch (Exception ex)
            {
                myException_BG = ex;
            }
        }

        //private void transposeXML()
        //{
        //    try
        //    {
        //        var RangeTranspose = wb.Worksheet(4).Range("A1:F3");
        //        RangeTranspose.Transpose(XLTransposeOptions.MoveCells);
        //        wb.Worksheet(4).Columns().AdjustToContents();
        //    }
        //    catch (Exception ex)
        //    {
        //        lblExpressionStatus.Text = ex.Message.ToString();
        //        lblStatus.Text = ex.Message.ToString();
        //    }

        //}

        private void formatExcelTable(int int_sheet_no, DataTable dt_calc)
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
                myException_BG = ex;
            }
        }

        private void formatExcelTableByDataGridView(int int_sheet_no, DataGridView dgv)
        {
            try
            {
                wb.Worksheet(1).Column(1).InsertColumnsBefore(1);
                wb.Worksheet(int_sheet_no).Cell(1, 1).Value = "DBType";

                for (int col = 0; col <= dt_src.Columns.Count; col++)
                {
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Fill.SetBackgroundColor(XLColor.Transparent);
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Border.OutsideBorderColor = XLColor.RichBlack;
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Font.SetFontColor(XLColor.RichBlack);
                }

                //wb.Worksheet(int_sheet_no).Cell(1, 1).Style.Fill.SetBackgroundColor(XLColor.Transparent);
                //wb.Worksheet(int_sheet_no).Cell(1, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                //wb.Worksheet(int_sheet_no).Cell(1, 1).Style.Border.OutsideBorderColor = XLColor.RichBlack;
                //wb.Worksheet(int_sheet_no).Cell(1, 1).Style.Font.SetFontColor(XLColor.RichBlack);

                for (int row = 1; row < dgv.RowCount; row++)
                {
                    wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Style.Fill.BackgroundColor = XLColor.Transparent;
                    wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Style.Border.OutsideBorderColor = XLColor.Black;

                    if (row % 2 == 0)
                    {
                        wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Value = "Target";
                    }
                    else
                    {
                        wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Value = "Source";
                    }

                }

                wb.Worksheet(int_sheet_no).Tables.FirstOrDefault().SetShowAutoFilter(false);
                wb.Worksheet(int_sheet_no).Columns().AdjustToContents();

                //int colAdjust = 1;

                for (int col = 1; col <= dgv.Columns.Count; col++)
                {
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Fill.SetBackgroundColor(XLColor.Transparent);
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Border.OutsideBorderColor = XLColor.RichBlack;
                    wb.Worksheet(int_sheet_no).Cell(1, col + 1).Style.Font.SetFontColor(XLColor.RichBlack);
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
                myException_BG = ex;
            }
        }

        private void delete_current_row_from_sql_table()
        {
            try
            {
                DataRow[] rowsDel = dt_sql_only.Select(str_unique_reference_key);

                foreach (DataRow dr in rowsDel)
                {
                    dr.Delete();
                }
                dt_sql_only.AcceptChanges();
            }
            catch (Exception ex)
            {
                myException_BG = ex;
            }
        }

        private void columns_to_Ignore()
        {
            try
            {
                lst_col_to_ignore.Clear();
                lst_col_to_include_in_comparison.Clear();
                //str_col_to_include_in_comparison = cb_referenceID.Text + " , ";

                DataColumnCollection dcc = dt_trg.Columns;

                str_col_to_include_in_comparison = string.Empty;

                CheckedListBox.CheckedItemCollection cic_ReferenceID = clb_ReferenceID.CheckedItems;

                foreach (var item in cic_ReferenceID)
                {
                    int index = cbL_columns.FindStringExact(item.ToString());
                    bool boolCheck = clb_ReferenceID.GetItemChecked(index);
                    cbL_columns.SetItemChecked(index, !boolCheck);
                }

                foreach (object i in cbL_columns.Items)
                {
                    if ((cbL_columns.GetItemChecked(cbL_columns.FindString(i.ToString()))))
                    {
                        str_col_to_include_in_comparison = str_col_to_include_in_comparison + i.ToString() + " , ";
                    }
                }

                if (str_col_to_include_in_comparison.Length > 2)
                {
                    str_col_to_include_in_comparison = str_col_to_include_in_comparison.Substring(0, str_col_to_include_in_comparison.Length - 2);
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = ex.Message.ToString();
                boolStatus = false;
            }

        }

        private void clearCheckBoxList(CheckedListBox cbl, form.CheckBox cb = null)
        {
            try
            {
                cbl.Items.Clear();
                if (cb != null)
                {
                    cb.Checked = false;
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = ex.Message.ToString();
            }
        }

        private void clearCheckBoxList(ListBox lb, form.CheckBox cb = null)
        {
            try
            {
                lb.Items.Clear();
                if (cb != null)
                {
                    cb.Checked = false;
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void test()
        {
            try
            {
                string str_CompleteTableName = string.Empty;
                strQuery = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA";

                if (cb_WindowAuthentication_TRG.CheckState == CheckState.Checked)
                {
                    strSQLConnection = "Data Source = " + tb_TRG_DataSource.Text + ";Initial Catalog = " + tb_TRG_DBName.Text
                    + "; Integrated Security = true ";
                }
                else if (cb_WindowAuthentication_TRG.CheckState == CheckState.Unchecked)
                {
                    strSQLConnection = "Data Source = " + tb_TRG_DataSource.Text + ";Initial Catalog = " + tb_TRG_DBName.Text
                    + "; User ID = " + tb_TRG_UserName.Text + "; " + "Password = " + tb_TRG_Password.Text + ";";
                }

                dt_trg = new DataTable();
                sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                sqlDA.Fill(dt_trg);
                ds = new DataSet();
                ds.Tables.Add(dt_trg);
                ds.Tables[0].TableName = "List of SQL Tables";

                DataTable dt = new DataTable();

                //FillCheckListBox();

            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void FillCheckListBox(ComboBox comboBox, DataTable dataTable, string str_dbType)
        {
            try
            {
                if (!(dataTable.Columns.Contains("CompleteName")))
                {
                    dataTable.Columns.Add("CompleteName");
                }
                List<string> lst_str_fill = new List<string>();

                if (str_dbType == "oracle")
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        str_tableName = $"{dr["OWNER"].ToString()}.{dr["TABLE_NAME"].ToString()}";
                        lst_str_fill.Add(str_tableName);
                        dr["CompleteName"] = str_tableName;
                    }
                }
                else if (str_dbType == "sql")
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        str_tableName = $"{dr["TABLE_CATALOG"].ToString()}.{dr["TABLE_SCHEMA"].ToString()}.{dr["TABLE_NAME"].ToString()}";
                        lst_str_fill.Add(str_tableName);
                        dr["CompleteName"] = str_tableName;
                    }
                }

                AutoCompleteStringCollection autoCompleteStringCollection = new AutoCompleteStringCollection();
                autoCompleteStringCollection.AddRange(lst_str_fill.ToArray());
                comboBox.AutoCompleteCustomSource = autoCompleteStringCollection;
                comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.DataSource = dataTable;
                //comboBox.DisplayMember = "TABLE_NAME";
                //comboBox.ValueMember = "CompleteName";
                DisplayMemberInComboBox();
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
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
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
            return dt_returnDataTable;
        }

        private async void btn_Connect_Reference_ID_Click(object sender, EventArgs e)
        {
            try
            {
                resetLabelsToDefaultValue();
                boolStatus = true;
                clearCheckBoxList(clb_ReferenceID);
                clearCheckBoxList(cbL_columns, cb_selectAll_toggle);
                clearCheckBoxList(lb_TargetColumns);
                clearCheckBoxList(lb_IgnoredCol_SRC);
                clearCheckBoxList(lb_IgnoredCol_TRG);
                toggleCondition(false);

                //ds = Connect_SRC_DB();
                
                Task<DataTable> taskLoadSrc = Task.Run<DataTable>(() => Connect_SRC());
                // conncet with SQL Target
                await Task.WhenAll(taskLoadSrc).ContinueWith(x =>
                {
                    ds = new DataSet();
                    ds.Tables.Add(taskLoadSrc.Result.Copy());
                });

                //ds = Connect_TRG_DB();
                Task < DataTable> taskLoadTrg = Task.Run<DataTable>(() => Connect_TRG());

                await Task.WhenAll(taskLoadTrg).ContinueWith(x =>
                {
                    ds.Tables.Add(taskLoadTrg.Result.Copy());
                });

                //await Task.WhenAll(taskLoadSrc, taskLoadTrg).ContinueWith(x =>
                //{
                //    ds = new DataSet();
                //    ds.Tables.Add(taskLoadSrc.Result.Copy());
                //    ds.Tables.Add(taskLoadTrg.Result.Copy());
                //});

                //statusChangeOfSomeItems(true);

                dt_CommonSRC = new DataTable();
                dt_CommonTRG = new DataTable();

                if (boolStatus)
                {
                    if (cb_ColumnMapping.Checked)
                    {
                        ViewForMultiReference viewForMultiReference = new ViewForMultiReference(ds, ds);
                        viewForMultiReference.Show();
                        lblExpressionStatus.Text = "Column mapping opted and Data loaded successfully in column mapping form";
                    }
                    else
                    {
                        dt_CommonSRC = common_Data.getCommonColumns(ds.Tables[0], ds.Tables[1], lb_IgnoredCol_SRC);
                        dt_CommonTRG = common_Data.getCommonColumns(ds.Tables[1], ds.Tables[0], lb_IgnoredCol_TRG);

                        foreach (DataColumn dc in dt_CommonSRC.Columns)
                        {
                            clb_ReferenceID.Items.Add(dc.ToString());
                            cbL_columns.Items.Add(dc.ToString());
                        }

                        foreach (DataColumn dc in dt_CommonTRG.Columns)
                        {
                            lb_TargetColumns.Items.Add(dc.ToString());
                        }
                        lblExpressionStatus.Text = "Data Loaded Successfully";

                        if (cbL_columns.Items.Count == 0)
                        {
                            UpdateLabel(lblExpressionStatus, "Data Loaded Successfull but there is no common Reference ID, Pls use Different Column Name and RefID comparison");
                        }
                    }
                }
                else
                {
                    lblExpressionStatus.Text = "Some Error in SQL or Oracle";
                }
                cb_selectAll_toggle.Checked = true;
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
                boolStatus = false;
            }
            finally
            {
                toggleCondition(true);
                btnGenerateOutput.Enabled = false;
            }
        }

        private void toggleCondition(bool enabledStatus)
        {
            gb_Source.Enabled = enabledStatus;
            gb_Source1.Enabled = enabledStatus;
            tb_SRC_DBName.Enabled = enabledStatus;
            tb_SRC_DataSource.Enabled = enabledStatus;
            tb_SRC_UserName.Enabled = enabledStatus;
            tb_SRC_Password.Enabled = enabledStatus;
            cb_SRC_TableName.Enabled = enabledStatus;

            gb_Target.Enabled = enabledStatus;
            gb_Target1.Enabled = enabledStatus;
            tb_TRG_DBName.Enabled = enabledStatus;
            tb_TRG_DataSource.Enabled = enabledStatus;
            tb_TRG_UserName.Enabled = enabledStatus;
            tb_TRG_Password.Enabled = enabledStatus;
            cb_TRG_TableName.Enabled = enabledStatus;

            btn_Connect_Reference_ID.Enabled = enabledStatus;
            btn_Set.Enabled = enabledStatus;
            btnGenerateOutput.Enabled = enabledStatus;
            btn_Connect_Reference_ID.Enabled = enabledStatus;

            tb_FileName.Enabled = enabledStatus;

            cb_selectAll_toggle.Enabled = enabledStatus;
            btn_Clear_Reset.Enabled = enabledStatus;
            comboBox1.Enabled = enabledStatus;

            llbl_Folder_path.Enabled = enabledStatus;

            gb_Source.Enabled = enabledStatus;

            cb_SRC_DefaultPort.Enabled = enabledStatus;

            if (enabledStatus & (!(cb_SRC_DefaultPort.Checked)))
            {
                tb_SRC_DefaultPort.Enabled = enabledStatus;
            }
            else if (cb_SRC_DefaultPort.Checked)
            {
                tb_SRC_DefaultPort.Enabled = !(enabledStatus);
            }
            else if (!(enabledStatus))
            {
                tb_SRC_DefaultPort.Enabled = enabledStatus;
            }

            if (enabledStatus)
            {
                cbL_columns.SelectionMode = SelectionMode.One;
                lb_TargetColumns.SelectionMode = SelectionMode.One;
                clb_ReferenceID.SelectionMode = SelectionMode.One;
            }
            else
            {
                cbL_columns.SelectionMode = SelectionMode.None;
                lb_TargetColumns.SelectionMode = SelectionMode.None;
                clb_ReferenceID.SelectionMode = SelectionMode.One;
            }

        }

        private void cb_selectAll_toggle_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxListToggle(cb_selectAll_toggle.Checked);
        }

        private void checkBoxListToggle(bool enabledState)
        {
            for (int i = 0; i < cbL_columns.Items.Count; i++)
            {
                cbL_columns.SetItemChecked(i, enabledState);
            }
        }

        private void DisplayMessage(Control control, string str_Msg)
        {
            try
            {
                control.Text = str_Msg;
                control.Update();
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        delegate void SetTextInRTB(string str);
        delegate void SetTextInRTB1(Exception ex1);
        private void DisplayLog(string strMessage)
        {
            try
            {
                if (this.rtb.InvokeRequired)
                {
                    SetTextInRTB setTextInRTB = new SetTextInRTB(DisplayLog);
                    this.Invoke(setTextInRTB, new object[] { strMessage });
                }
                else
                {
                    Common_Data.DisplayMessage(rtb, "");
                    Common_Data.DisplayMessage(rtb, strMessage);
                }
            }
            catch (Exception ex)
            {
                rtb.AppendText("\n" + string.Format("{0:T}", DateTime.Now) + " : " + ex.Message.ToString());
            }
        }

        private void DisplayLog(Exception exception)
        {
            try
            {
                if (this.rtb.InvokeRequired)
                {
                    SetTextInRTB1 setTextInRTB = new SetTextInRTB1(DisplayLog);
                    this.Invoke(setTextInRTB, new object[] { exception });
                }
                else
                {
                    Common_Data.DisplayDebugMsg(rtb, string.Empty);
                    Common_Data.DisplayError(rtb, exception);
                }
            }
            catch (Exception ex)
            {
                rtb.AppendText("\n" + string.Format("{0:T}", DateTime.Now) + " : " + ex.Message.ToString());
            }
        }

        private void btn_Set_Click(object sender, EventArgs e)
        {
            try
            {
                resetLabelsToDefaultValue();
                toggleCondition(false);

                ListBox.ObjectCollection lb_oc_target = lb_TargetColumns.Items;
                CheckedListBox.ObjectCollection cic_oc_source = cbL_columns.Items;

                DataColumnCollection dcc = dt_trg.Columns;

                if (cbL_columns.Items.Count == 0)
                {
                    DisplayMessage(lblStatus, "There is no common ID, Pls use Different Column Name and RefID comparison");
                    return;
                }

                foreach (DataColumn dc in dt_src.Columns)
                {
                    if (!(dcc.Contains(dc.ColumnName.ToString())) && (cbL_columns.Items.Contains(dc.ColumnName.ToString())))
                    {
                        cbL_columns.SetItemCheckState(cbL_columns.FindStringExact(dc.ColumnName.ToString()), CheckState.Unchecked);

                        #region New code 
                        //cbL_columns.Items.Remove(dc.ColumnName);
                        lb_IgnoredCol_SRC.Items.Add(dc.ColumnName);
                        #endregion
                    }
                }

                //cbL_columns.SetItemChecked(cbL_columns.FindString(cb_referenceID.Text), false);

                foreach (var checkRef in clb_ReferenceID.CheckedItems)
                {
                    cbL_columns.SetItemChecked(cbL_columns.FindString(checkRef.ToString()), false);
                }

                columns_to_Ignore();

                if (clb_ReferenceID.CheckedItems.Count > 0 & cbL_columns.CheckedItems.Count > 0)
                {
                    boolStatus = true;
                    //tb_FileName.Text = $"{cb_Source_tableName.Text}_Output_{DateTime.Now.ToString().Replace('-', '_').Replace(':', '_')}";
                    tb_FileName.Text = $"{common_Data.RemoveSpecialCharacters(cb_SRC_TableName.Text)}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}";
                    toggleCondition(true);
                    statusChangeforSetButton(false);
                    lblExpressionStatus.Text = "All Set";
                }
                else if (clb_ReferenceID.CheckedItems.Count == 0)
                {
                    boolStatus = false;
                    toggleCondition(true);
                    lblExpressionStatus.Text = "Pls select atleast one reference ID";
                    btnGenerateOutput.Enabled = false;
                }
                else
                {
                    boolStatus = false;
                    toggleCondition(true);
                    lblExpressionStatus.Text = "No columns header are matching with name, pls use column name mapping checkbox in 'control & output'.";
                    btnGenerateOutput.Enabled = false;
                }

                int_total_rows = dt_src.Rows.Count;
                int_total_columns = dt_src.Columns.Count;

                lblCal.Text = "Rows : " + int_total_rows.ToString() +
                    " , Columns : " + int_total_columns.ToString() + " , Total checks to be performed : " +
                    (int_total_columns * int_total_rows).ToString();

            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
            finally
            {
                toggleCondition(true);
            }

        }
        private void rb_oracle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gb_Source1.Text = "Source: Oracle Connection and Settings";
                cb_WindowAuthentication_SRC.CheckState = CheckState.Unchecked;
                cb_WindowAuthentication_SRC.Enabled = false;
                cb_SRC_DefaultPort.CheckState = CheckState.Checked;
                cb_SRC_DefaultPort.Enabled = true;
                tb_SRC_DefaultPort.Text = "1521";
                tb_SRC_DefaultPort.Enabled = false;
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = ex.Message.ToString();
            }
        }

        private void rb_sql_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gb_Source1.Text = "Source: SQL Connection and Settings";
                cb_WindowAuthentication_SRC.Enabled = true;
                cb_WindowAuthentication_SRC.CheckState = CheckState.Unchecked;
                cb_SRC_DefaultPort.Enabled = false;
                tb_SRC_DefaultPort.ResetText();
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = ex.Message.ToString();
            }
            finally
            {

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (true)//Debugger.IsAttached)
                {
                    switch (comboBox1.SelectedIndex + 1)
                    {
                        case 1:
                            rb_SRC_SQL.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT247";
                            tb_SRC_DBName.Text = "SPI2018";
                            tb_SRC_UserName.Text = "sa";
                            tb_SRC_Password.Text = "abcde@1234567";
                            cb_SRC_TableName.Text = "[DOMAIN2018].[LOOP]";

                            rb_TRG_SQL.Checked = true;
                            tb_TRG_DataSource.Text = "IESWKCT247";
                            tb_TRG_DBName.Text = "SPI2018";
                            tb_TRG_UserName.Text = "sa";
                            tb_TRG_Password.Text = "abcde@1234567";
                            cb_TRG_TableName.Text = "[DOMAIN2018].[COMPONENT]";

                            break;

                        case 2:
                            rb_SRC_SQL.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT247";
                            tb_SRC_DBName.Text = "SPI2018";
                            tb_SRC_UserName.Text = "DOMAIN2018";
                            tb_SRC_Password.Text = "DOMAIN20181";
                            cb_SRC_TableName.Text = "SPI2018.DOMAIN2018.LOOP";

                            rb_TRG_SQL.Checked = true;
                            tb_TRG_DataSource.Text = "IESWKCT247";
                            tb_TRG_DBName.Text = "SPI2018";
                            tb_TRG_UserName.Text = "TESTDOMAIN18";
                            tb_TRG_Password.Text = "TESTDOMAIN181";
                            cb_TRG_TableName.Text = "SPI2018.TESTDOMAIN18.LOOP";

                            break;

                        case 3:
                            rb_SRC_SQL.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT247";
                            tb_SRC_DBName.Text = "SPELOLDP";
                            tb_SRC_UserName.Text = "sa";
                            tb_SRC_Password.Text = "abcde@1234567";
                            cb_SRC_TableName.Text = "plant1el.t_plantitem";

                            rb_TRG_SQL.Checked = true;
                            tb_TRG_DataSource.Text = "IESWKCT247";
                            tb_TRG_DBName.Text = "SPELOLDP_new";
                            tb_TRG_UserName.Text = "sa";
                            tb_TRG_Password.Text = "abcde@1234567";
                            cb_TRG_TableName.Text = "plant1el.t_plantitem";

                            break;

                        case 4:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";
                            cb_SRC_TableName.Text = "DASISLANDEL.T_motor";

                            rb_TRG_SQL.Checked = true;
                            tb_TRG_DataSource.Text = "IESWKCT247";
                            tb_TRG_DBName.Text = "JK1703_P";
                            tb_TRG_UserName.Text = "sa";
                            tb_TRG_Password.Text = "abcde@1234567";
                            cb_TRG_TableName.Text = "Plant789el.T_Motor";

                            break;

                        case 5:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";
                            cb_SRC_TableName.Text = "DASISLANDEL.T_Plantitem";

                            rb_TRG_SQL.Checked = true;
                            tb_TRG_DataSource.Text = "IESWKCT247";
                            tb_TRG_DBName.Text = "JK1703_P";
                            tb_TRG_UserName.Text = "sa";
                            tb_TRG_Password.Text = "abcde@1234567";
                            cb_TRG_TableName.Text = "plant789elp1.t_Plantitem";

                            break;

                        case 6:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";
                            cb_SRC_TableName.Text = "tb_loop";

                            rb_TRG_SQL.Checked = true;
                            tb_TRG_DataSource.Text = "IESWKCT247";
                            tb_TRG_DBName.Text = "db_SPELSQL";
                            tb_TRG_UserName.Text = "sa";
                            tb_TRG_Password.Text = "abcde@1234567";
                            cb_TRG_TableName.Text = "loop";

                            break;
                        case 7:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";
                            cb_SRC_TableName.Text = "tb_loop_2k";

                            tb_TRG_DataSource.Text = "IESWKCT247";
                            tb_TRG_DBName.Text = "db_SPELSQL";
                            tb_TRG_UserName.Text = "sa";
                            tb_TRG_Password.Text = "abcde@1234567";
                            cb_TRG_TableName.Text = "tb_loop_2k";

                            break;
                        case 8:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "localhost";
                            tb_SRC_DBName.Text = "orcl";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "1234";
                            cb_SRC_TableName.Text = "test";

                            rb_TRG_Oracle.Checked = true;
                            tb_TRG_DataSource.Text = "localhost";
                            tb_TRG_DBName.Text = "orcl";
                            tb_TRG_UserName.Text = "system";
                            tb_TRG_Password.Text = "1234";
                            cb_TRG_TableName.Text = "test1";

                            break;

                        case 9:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";
                            cb_SRC_TableName.Text = "DASISLANdEL.T_MOTOR";

                            rb_TRG_SQL.Checked = true;
                            tb_TRG_DataSource.Text = "IESWKCT247";
                            tb_TRG_DBName.Text = "JK1703_P";
                            tb_TRG_UserName.Text = "sa";
                            tb_TRG_Password.Text = "abcde@1234567";
                            cb_TRG_TableName.Text = "Plant789el.T_Motor";

                            break;

                        case 10:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "DSGPY3002.dcap.glpoly.net";
                            tb_SRC_DBName.Text = "MSPIAS";
                            tb_SRC_UserName.Text = "COV_LTTS_VIEW";
                            tb_SRC_Password.Text = "SP14LTTS";
                            cb_SRC_TableName.Text = "MDITRAIN.FLUID_PROPERTY";

                            rb_TRG_SQL.Checked = true;
                            tb_TRG_DataSource.Text = @"DFFMZ08SORA01\TOOLS_QAQC";
                            tb_TRG_DBName.Text = "Q_SPI_MDITRAIN_2018";
                            tb_TRG_UserName.Text = "CAOJING";
                            tb_TRG_Password.Text = "CAOJING1";
                            cb_TRG_TableName.Text = "CAOJING.FLUID_PROPERTY";

                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
            }
        }

        private void btn_Clear_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                tb_SRC_DataSource.Text = string.Empty;
                tb_SRC_DBName.Text = string.Empty;
                tb_SRC_UserName.Text = string.Empty;
                tb_SRC_Password.Text = string.Empty;
                cb_SRC_TableName.Text = string.Empty;

                tb_TRG_DataSource.Text = string.Empty;
                tb_TRG_DBName.Text = string.Empty;
                tb_TRG_UserName.Text = string.Empty;
                tb_TRG_Password.Text = string.Empty;
                cb_TRG_TableName.Text = string.Empty;

                tb_FileName.Text = string.Empty;
                toggleCondition(true);
                resetLabelsToDefaultValue();
                resetCheckListBox();

                rb_SRC_oracle.Select();
                btnGenerateOutput.Enabled = false;
                boolStatus = false;
                rtb.ResetText();
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
            }
        }

        private void resetLabelsToDefaultValue()
        {
            try
            {
                lblExpressionStatus.ResetText();
                lblStatus.ResetText();
                lblCal.ResetText();
                lbl_PercentageShow.Text = "Progress Bar";
                progressBar1.Value = 0;
                myException_BG = new Exception();
                lblTimer.ResetText();

            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayLog(ex);
            }
        }

        private void resetCheckListBox()
        {
            try
            {
                clb_ReferenceID.Items.Clear();
                cbL_columns.Items.Clear();
                lb_TargetColumns.Items.Clear();
                cb_selectAll_toggle.CheckState = CheckState.Unchecked;
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
            }
        }

        private string generateRowFilterString(DataRow dr)
        {
            try
            {
                str_unique_reference_key = string.Empty;
                foreach (var a in clb_ReferenceID.CheckedItems)
                {
                    str_unique_reference_key = str_unique_reference_key + a.ToString() + " = '" + dr[a.ToString()] + "' AND ";
                }
                str_unique_reference_key = str_unique_reference_key.Substring(0, str_unique_reference_key.Length - 4);
            }
            catch (Exception ex)
            {
                //lblExpressionStatus.Text = ex.Message.ToString();
                myException_BG = ex;
            }
            return str_unique_reference_key;
        }

        private string generateRowFilterStringForAll(DataRow dataRowPK, DataRow dataRowData)
        {
            try
            {
                str_unique_reference_key = string.Empty;

                //string[] str_PK = dataRowPK["primary_key_columns"].ToString().Split(',');

                var q = dataRowPK["primary_key_columns"].ToString().Split(',').ToArray().AsEnumerable<string>().Select(p => $"{p.ToString()} = '{dataRowData[p.ToString()]}'");

                str_unique_reference_key = string.Join(" AND ", q.ToArray());

            }
            catch (Exception ex)
            {
                //lblExpressionStatus.Text = ex.Message.ToString();
                myException_BG = ex;
            }
            return str_unique_reference_key;
        }
        private void btnGenerateOutput_Click(object sender, EventArgs e)
        {
            try
            {
                resetLabelsToDefaultValue();
                toggleCondition(false);
                lblStatus.Text = "Pls wait, Processing started";
                lblStatus.Update();
                btn_Cancel.Enabled = true;

                if (!(cb_ListProcess.Checked))
                {
                    strFilePathChangeSave = llbl_Folder_path.Text + @"\" + tb_FileName.Text + @".xlsx";

                    if (Debugger.IsAttached || Environment.UserName == "40009708")
                    {
                        tb_FileName.Text = $"{common_Data.RemoveSpecialCharacters(cb_SRC_TableName.Text)}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}";
                    }

                    myException_BG = new Exception();

                    if (cb_IncludeRule.Checked && common_Data.GetRulesStatus())
                    {
                        dsRules = common_Data.GetDatasetRules();
                        boolRules = common_Data.GetRulesStatus();
                    }
                }

                if ((!backgroundWorker1.IsBusy) && (!(cb_ListProcess.Checked)))
                {
                    #region new code

                    if (!(cb_ListProcess.Checked))
                    {
                        if (!boolStatus)
                        {
                            myException_BG.Source = "There is some error";
                            toggleCondition(true);
                            lblExpressionStatus.Text = myException_BG.Message.ToString();
                            lblStatus.Text = myException_BG.Source.ToString();
                            return;
                        }

                        str_to_fetch = to_fetch(str_to_fetch);

                        // call oracle BD
                        if (boolStatus)
                        {
                            ds_CompleteSet = Connect_SRC_DB(str_to_fetch);
                            ds = Connect_SRC_DB(str_to_fetch, true);
                        }

                        //Call SQL server DB
                        if (boolStatus)
                        {
                            ds = Connect_TRG_DB(str_to_fetch);
                        }
                        else
                        {
                            myException_BG.Source = "There is some error";
                            toggleCondition(true);
                            lblExpressionStatus.Text = myException_BG.Message.ToString();
                            lblStatus.Text = myException_BG.Source.ToString();
                            return;
                        }
                    }

                    #endregion
                    if ((!(backgroundWorker1.IsBusy)) && boolStatus || cb_ListProcess.Checked)
                    {
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
                else if (cb_ListProcess.Checked)
                {
                    // New Code

                    str_to_fetch = " * ";

                    if (boolStatus)
                    {
                        ds_CompleteSet = Connect_SRC_DB(str_to_fetch);
                    }

                    //end of new code

                    if (!(backgroundWorker2.IsBusy))
                    {
                        str_Where_Col = cb_DateCol.Text.ToString().Trim();
                        str_Where_Val_Oracle = dateTimePicker_Date.Value.ToString("dd-MMM-yy");
                        str_Where_Val_SQL = dateTimePicker_Date.Value.ToString("yyyy-MM-dd");
                        backgroundWorker2.RunWorkerAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = ex.Message.ToString();
            }
        }

        private bool Compare_List(DoWorkEventArgs e)
        {
            try
            {
                if (dbSRCConnection.boolConnection && dbTRGConnection.boolConnection)
                {
                    foreach (DataRow drList in dsTableList.Tables[0].Rows)
                    {
                        if (backgroundWorker2.CancellationPending)
                        {
                            backgroundWorker2.ReportProgress(0);
                            boolCompare_List = false;
                            e.Cancel = true;
                            return boolCompare_List;
                        }

                        boolCompare_List = false;

                        str_SRC_TableName = drList["src_table_names"].ToString().ToUpper();
                        str_TRG_TableName = drList["trg_table_names"].ToString().ToUpper();

                        dtSRC = GetDataTable(rb_SRC_oracle, rb_SRC_SQL, dbSRCConnection, str_SRC_TableName, true);
                        dtTRG = GetDataTable(rb_TRG_Oracle, rb_TRG_SQL, dbTRGConnection, str_TRG_TableName, false);

                        dtSRC = Common_Data.RemoveUncommonColumnInDataTable(dtSRC, dtTRG);
                        dtTRG = Common_Data.RemoveUncommonColumnInDataTable(dtTRG, dtSRC);

                        ds = new DataSet();

                        dtSRC.TableName += "_SRC";
                        dtTRG.TableName += "_TRG";

                        ds_CompleteSet = new DataSet();
                        ds_CompleteSet.Tables.Add(GetDataTable(rb_SRC_oracle, rb_SRC_SQL, dbSRCConnection, str_SRC_TableName, false).Copy());

                        dt_src = dtSRC.Copy();
                        dt_trg = dtTRG.Copy();

                        ds.Tables.Add(dtSRC);
                        ds.Tables.Add(dtTRG);

                        boolCompare_List = backGroundWorkList(drList, e);
                    }
                }
                else
                {
                    DisplayMessage("DB Connections problem", true);
                    boolCompare_List = false;
                    return boolCompare_List;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return boolCompare_List;
        }

        delegate void delDisplayDebugMsg(string s);
        private void DisplayDebugMsg(string v)
        {
            try
            {
                if (this.lblDebugMsg.InvokeRequired)
                {
                    delDisplayDebugMsg d = new delDisplayDebugMsg(DisplayDebugMsg);
                    this.Invoke(d, new object[] { v });
                }
                else
                {
                    if (GlobalDebug.boolIsGlobalDebug || Debugger.IsAttached)
                    {
                        lblDebugMsg.Text = $"Debug Msg : {v}";
                        lblDebugMsg.Visible = true;
                        lblDebugMsg.ForeColor = Color.Green;
                    }
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private DataTable GetDataTable(RadioButton rb_oracle, RadioButton rb_SQL, DBConnectionStatus dBConnection, string strTableName, bool boolDateFilter = false)
        {
            try
            {
                string[] str_IgnoreColName;
                if (rb_oracle.Checked)
                {
                    //strSelectCol = $"SELECT table_name, column_name, data_type, data_length FROM USER_TAB_COLUMNS WHERE table_name = '{dsTableList.Tables[0].Rows[0]["table_names"].ToString().ToUpper()}'";

                    strSelectCol = $"SELECT * FROM {strTableName}";

                    if (boolDateFilter)
                    {
                        strSelectCol = $"SELECT * FROM {strTableName} Where {str_Where_Col} >= '{str_Where_Val_Oracle}'";
                    }

                    using (OracleConnection oracleConnection = new OracleConnection(dBConnection.strConnectionString))
                    {
                        using (OracleDataAdapter oracleDA = new OracleDataAdapter(strSelectCol, oracleConnection))
                        {
                            dtFetch = new DataTable();
                            backgroundWorker2.ReportProgress(-1, strSelectCol);
                            oracleDA.Fill(dtFetch);
                            dtFetch.TableName = strTableName;
                            if (dtFetch.Columns.Count > 0)
                            {
                                str_IgnoreColName = dsTableList.Tables[0].Rows[0]["ignore_columns"].ToString().ToUpper().Split(',');
                                foreach (string ignoreCol in str_IgnoreColName)
                                {
                                    if (dtFetch.Columns.Contains(ignoreCol))
                                    {
                                        dtFetch.Columns.Remove(ignoreCol);
                                    }
                                }

                            }

                            //DisplayMessage("Oracle PK Read");
                        }
                    }

                }
                else if (rb_SQL.Checked)
                {
                    //strSelectCol = $"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = '{dsTableList.Tables[0].Rows[0]["table_names"]}'";

                    strSelectCol = $"SELECT * FROM {strTableName}";

                    if (boolDateFilter)
                    {
                        strSelectCol = $"SELECT * FROM {strTableName} Where {str_Where_Col} >= '{str_Where_Val_SQL}'";
                    }

                    using (SqlConnection sqlConnection = new SqlConnection(dBConnection.strConnectionString))
                    {
                        using (SqlDataAdapter sqlDA = new SqlDataAdapter(strSelectCol, sqlConnection))
                        {
                            dtFetch = new DataTable();
                            backgroundWorker2.ReportProgress(-1, strSelectCol);
                            sqlDA.Fill(dtFetch);
                            dtFetch.TableName = strTableName;
                            if (dtFetch.Columns.Count > 0)
                            {
                                str_IgnoreColName = dsTableList.Tables[0].Rows[0]["ignore_columns"].ToString().ToUpper().Split(',');
                                foreach (string ignoreCol in str_IgnoreColName)
                                {
                                    if (dtFetch.Columns.Contains(ignoreCol))
                                    {
                                        dtFetch.Columns.Remove(ignoreCol);
                                    }
                                }

                            }

                            //DisplayMessage("Oracle PK Read");
                        }
                    }
                }
                return dtFetch;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void UpdateProgressBar(int count, int totalCount, string status)
        {
            try
            {
                if (status.ToLower() == "update")
                {
                    progressBar1.Value = (count * 100) / totalCount;
                    //DisplayMessage($"Processing....{count}", false, count.ToString().Length);
                }
                if (status.ToLower() == "tableupdate")
                {
                    progressBar1.Value = (count * 100) / totalCount;
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
            finally
            {
                progressBar1.Update();
            }
        }
        private string to_fetch(string str_to_fetch)
        {
            try
            {
                str_to_fetch = string.Empty;
                foreach (var item in clb_ReferenceID.CheckedItems)
                {
                    str_to_fetch = str_to_fetch + item + " , ";
                }
                str_to_fetch = str_to_fetch + str_col_to_include_in_comparison;

                //str_to_fetch = str_to_fetch.Substring(0, str_to_fetch.Length - 2);


            }
            catch (Exception ex)
            {
                boolStatus = false;
                myException_BG = ex;
            }
            return str_to_fetch;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                start_Time = DateTime.Now;

                ds_change = new DataSet();
                dt_change = new DataTable();

                count_match = 0;
                count_unmatched = 0;

                dt_trg.CaseSensitive = true;

                dv_sql = new DataView(dt_trg);
                dv_oracle = new DataView(dt_src);

                dt_sql_only = dt_trg.Copy();

                dt_unmatched_Capture = new DataTable();
                dt_unmatched_Capture = dt_src.Clone();

                //DataGridView dataGridView2 = new DataGridView();

                //foreach (DataColumn dc in dt_unmatched_Capture.Columns)
                //{
                //    dataGridView2.Columns.Add(dc.ColumnName, dc.ColumnName);
                //}

                dt_oracle_only = dt_src.Clone();
                dt_summarised_difference = dt_src.Clone();

                foreach (DataColumn dc in dt_src.Columns)
                {
                    dt_summarised_difference.Columns[dc.ToString()].DataType = typeof(Int32);
                }
                DataRow dr_summarised = dt_summarised_difference.NewRow();


                foreach (DataColumn dc in dt_src.Columns)
                {
                    dr_summarised[dc.ToString()] = 0;
                }

                dt_summarised_difference.Rows.Add(dr_summarised);

                ds_unmatched_Capture = new DataSet();

                //headRow = -1;
                //alternateRow = 0;
                newRow = true;

                #region check for match and unmatch columns

                progress = 0;
                totalProgressCount = dt_src.Rows.Count;

                string str_col_used = string.Empty;

                cic_oracleTB = cbL_columns.CheckedItems;
                cic_reference = clb_ReferenceID.CheckedItems;

                #endregion

                #region main comparison part

                int_Color_Row = 1;
                int int_sheet = 1;

                foreach (DataRow dr in dt_src.Rows)
                {
                    str_unique_reference_key = generateRowFilterString(dr);

                    dv_sql.RowFilter = str_unique_reference_key;

                    percentage = (((++progress) * 100) / totalProgressCount);
                    backgroundWorker1.ReportProgress(percentage);

                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker1.ReportProgress(0);
                        return;
                    }

                    if (dv_sql.Count > 0)
                    {
                        delete_current_row_from_sql_table();
                        if (int_Color_Row >= 1020300)
                        {
                            int_Color_Row = 1;
                            ++int_sheet;
                        }
                        foreach (var dc in cic_oracleTB)
                        {
                            if (!(cic_reference.Contains(dc.ToString())))
                            {
                                str_Data_Source_To_Compare = dr[dc.ToString()].ToString().Trim();
                                str_Data_Target_To_Compare = dv_sql[0][dc.ToString()].ToString().Trim();

                                boolCompareResult = false;

                                bool boolTest = common_Data.MismatchTest(str_Data_Source_To_Compare, str_Data_Target_To_Compare, cb_IgnoreCase.Checked, cb_IncludeRule.Checked, dc.ToString(), dc.ToString(), dr, dr, dt_src.Columns, dt_trg.Columns);

                                if (boolTest)
                                {
                                    Console.WriteLine((dr[dc.ToString()].ToString()).GetType());
                                    Console.WriteLine((dr[dc.ToString()].ToString()));

                                    #region exception handling of float values start here

                                    boolOracle = float.TryParse((dr[dc.ToString()].ToString()), out float float_oracle);
                                    boolSQL = float.TryParse((dv_sql[0][dc.ToString()].ToString()), out float float_sql);

                                    if (boolOracle & boolSQL)
                                    {
                                        if (float_oracle == float_sql)
                                        {
                                            continue;
                                        }
                                    }
                                    #endregion exception hadling of float values end here

                                    cellCol = dt_src.Columns[dc.ToString()].Ordinal;
                                    str_colName = dc.ToString();
                                    if (newRow)
                                    {
                                        dt_unmatched_Capture.Rows.Add(dr.ItemArray);
                                        dt_unmatched_Capture.Rows.Add(dv_sql[0].Row.ItemArray);
                                        newRow = false;
                                        int_Color_Row += 2;

                                    }

                                    colorPairsMultiSheet.Add(new ColorPairMultiSheet(int_Color_Row - 1, int_Color_Row, dc.ToString(), int_sheet));

                                    dt_summarised_difference.Rows[0][str_colName] =
                                            Convert.ToInt32((dt_summarised_difference.Rows[0][str_colName]).ToString()) + 1;

                                    count_unmatched++;
                                }
                                else
                                {
                                    count_match++;
                                }
                            }
                        }
                        newRow = true;
                    }
                    else
                    {
                        dt_oracle_only.Rows.Add(dr.ItemArray);
                    }

                }

                #endregion

                // writing excel sheet
                /////////////////////////////////////////////////////////////////////////////

                //==========================================================================================================

                #region writing and formatting excel sheets

                ds_change.Tables.Add(dt_change);

                #region for deleting columns which are not required

                int index = 0;
                int adjust = 0;
                foreach (DataColumn dc in dt_summarised_difference.Columns)
                {
                    index = dc.Ordinal - adjust;
                    if (!(clb_ReferenceID.GetItemChecked(clb_ReferenceID.FindString(dc.ColumnName.ToString()))))
                    {
                        if (Convert.ToInt32(dt_summarised_difference.Rows[0][dc.ColumnName.ToString()]) == 0)
                        {
                            dt_unmatched_Capture.Columns.Remove(dc.ColumnName);
                            //dataGridView2.Columns.Remove(dc.ColumnName.ToString());
                            adjust++;
                        }
                    }
                }

                #endregion

                generateTransposeTable();

                List<string> strPKList = new List<string>();

                foreach (var i in cic_reference)
                {
                    strPKList.Add(i.ToString());
                }

                dt_unmatched_Capture.Columns.Add("DBType", typeof(string)).SetOrdinal(0);

                bool boolToggle = true;

                foreach (DataRow dr in dt_unmatched_Capture.Rows)
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
                //if (Properties.Settings.Default.EPPlus_Excel_Save)
                //{
                using (ExcelPackage pck = new ExcelPackage())
                {
                    Logs.EnterLogsWithTime($"Saving File with EPPlus started under PK");

                    dsUnmatched = new DataSet();
                    queryColorpairMS = colorPairsMultiSheet.AsQueryable<ColorPairMultiSheet>();
                    dsUnmatched = WriteExcel.ExportToExcelNoLimitRows(dt_unmatched_Capture, "Mismatch", queryColorpairMS);

                    Logs.EnterLogsWithTime($"Mismatch Table loaded");
                    DisplayDebugMsg("First Workseet Written");

                    #region coloring

                    worksheet = WriteExcel.ColorDataset(ref worksheet, ref dsUnmatched, ref strPKList, ref queryColorpairMS, ref backgroundWorker1, ref e, pck);

                    #endregion
                    dt_oracle_only.TableName = "Source Rows Only";
                    worksheet = common_Data.AddNewWorksheet(pck, worksheet, dt_oracle_only);
                    worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                    DisplayDebugMsg("Second worksheet written");

                    dt_sql_only = new DataTable();
                    dt_sql_only = RowsOnlyInTable(e, ds_CompleteSet.Tables[0], dt_trg);
                    worksheet = common_Data.AddNewWorksheet(pck, worksheet, dt_sql_only, "Target Rows Only");
                    worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                    DisplayDebugMsg("Third worksheet written");

                    worksheet = common_Data.AddNewWorksheet(pck, worksheet, dt_transpose, "Mismatch Details");

                    worksheet.Cells[2, 1, strPKList.Count + 1, 1].Style.Font.Color.SetColor(Color.Red);

                    foreach (var cell in worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column])
                    {
                        cell.Value = Convert.ToInt32(cell.Value);
                    }

                    worksheet = common_Data.AddNewWorksheet(pck, worksheet, SPIDeltaChangeTracker_Sheet(dt_oracle_only.Rows.Count, dt_unmatched_Capture.Rows.Count / 2, dt_sql_only.Rows.Count), "Summary");

                    backgroundWorker1.ReportProgress(++percentage);
                    DisplayDebugMsg("Fourth and last worksheet written");

                    pck.SaveAs(new System.IO.FileInfo(strFilePathChangeSave));
                    boolStatus = true;
                    Logs.EnterLogsWithTime($"Saving File with EPPlus ended under PK");
                }
                //}

                /*
            else if (Properties.Settings.Default.XL_Excel_Save)
            {
                wb = new XLWorkbook();
                //wb.Worksheets.Add(SPIDeltaChangeTracker_Sheet(dt_oracle_only.Rows.Count, dt_unmatched_Capture.Rows.Count / 2, dt_sql_only.Rows.Count), "Summary");
                wb.Worksheets.Add(dt_unmatched_Capture, "Mismatch");
                wb.Worksheets.Add(dt_oracle_only, "Source rows only");

                dt_sql_only = new DataTable();
                dt_sql_only = RowsOnlyInTable(e, ds_CompleteSet.Tables[0], dt_trg);
                wb.Worksheets.Add(dt_sql_only, "Target rows only");
                //wb.Worksheets.Add(RowsOnlyInTable(e, ds_CompleteSet.Tables[0], dt_trg), "Sql rows only");

                wb.Worksheets.Add(dt_transpose, "Mismatch Details");
                wb.Worksheets.Add(SPIDeltaChangeTracker_Sheet(dt_oracle_only.Rows.Count, dt_unmatched_Capture.Rows.Count / 2, dt_sql_only.Rows.Count), "Summary");

                backgroundWorker1.ReportProgress(105);

                //formatExcelTableByDataGridView(1, dataGridView2);

                backgroundWorker1.ReportProgress(110);

                formatExcelTable(2, dt_oracle_only);
                formatExcelTable(3, dt_sql_only);
                formatExcelTable(4, dt_transpose);
                formatExcelTable(5, dt_SPIDeltaChangeTracker);

                backgroundWorker1.ReportProgress(115);

                wb.SaveAs(strFilePathChangeSave);

                #endregion

                #region removing columns which are not needed to be displayed as no mismatched found

                backgroundWorker1.ReportProgress(125);

                wb.Save();
            }
            */

                backgroundWorker1.ReportProgress(130);
                #endregion
                stop_Time = DateTime.Now;

            }

            catch (Exception ex)
            {
                e.Result = ex.Message.ToString();
                myException_BG = ex;
                boolStatus = false;
            }
        }

        private DataTable RowsOnlyInTable(DoWorkEventArgs e, DataTable dt_SRC = null, DataTable dt_TRG = null, DataSet ds_SRC_TRG = null)
        {
            DataTable dt_TRG_Only;
            try
            {
                progress = 0;
                if (ds_SRC_TRG != null && dt_SRC == null && dt_TRG == null)
                {
                    if (ds_SRC_TRG.Tables.Count > 1)
                    {
                        dt_SRC = ds_SRC_TRG.Tables[0].Copy();
                        dt_TRG = ds_SRC_TRG.Tables[1].Copy();
                    }
                }

                dt_TRG_Only = dt_TRG.Copy();
                DataView dv_TRG = new DataView(dt_TRG);
                DataView dv_SRC = new DataView(dt_SRC);
                totalProgressCount = dt_TRG.Rows.Count;

                foreach (DataRow dr in dt_TRG.Rows)
                {
                    str_unique_reference_key = generateRowFilterString(dr);
                    dv_SRC.RowFilter = str_unique_reference_key;

                    percentage = (((++progress) * 100) / totalProgressCount);
                    backgroundWorker1.ReportProgress(percentage);

                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker1.ReportProgress(0);
                        return dt_TRG_Only;
                    }

                    if (dv_SRC.Count > 0)
                    {
                        DataRow[] rowsDel = dt_TRG_Only.Select(str_unique_reference_key);

                        foreach (DataRow drr in rowsDel)
                        {
                            drr.Delete();
                        }
                        dt_TRG_Only.AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
                throw ex;
            }
            return dt_TRG_Only;
        }

        private DataTable RowsOnlyInTableList(DataRow drL, DoWorkEventArgs e, DataTable dt_SRC = null, DataTable dt_TRG = null, DataSet ds_SRC_TRG = null)
        {
            DataTable dt_TRG_Only;
            try
            {
                progress = 0;
                if (ds_SRC_TRG != null && dt_SRC == null && dt_TRG == null)
                {
                    if (ds_SRC_TRG.Tables.Count > 1)
                    {
                        dt_SRC = ds_SRC_TRG.Tables[0].Copy();
                        dt_TRG = ds_SRC_TRG.Tables[1].Copy();
                    }
                }

                dt_TRG_Only = dt_TRG.Copy();
                DataView dv_TRG = new DataView(dt_TRG);
                DataView dv_SRC = new DataView(dt_SRC);
                totalProgressCount = dt_TRG.Rows.Count;

                foreach (DataRow dr in dt_TRG.Rows)
                {
                    str_unique_reference_key = generateRowFilterStringForAll(drL, dr);
                    dv_SRC.RowFilter = str_unique_reference_key;

                    percentage = (((++progress) * 100) / totalProgressCount);
                    backgroundWorker2.ReportProgress(percentage);

                    if (backgroundWorker2.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker2.ReportProgress(0);
                        return dt_TRG_Only;
                    }

                    if (dv_SRC.Count > 0)
                    {
                        DataRow[] rowsDel = dt_TRG_Only.Select(str_unique_reference_key);

                        foreach (DataRow drr in rowsDel)
                        {
                            drr.Delete();
                        }
                        dt_TRG_Only.AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
                throw ex;
            }
            return dt_TRG_Only;
        }
        private DataTable SPIDeltaChangeTracker_Sheet(int int_NCLD, int int_DULD, int int_DLDM)
        {
            try
            {
                // int_NCLD - Newly Created in Legacy Database
                // int_DULD - Data Updated in  Legacy Database
                // int_DLDM - Deleted in  Legacy Database

                dt_SPIDeltaChangeTracker = new DataTable();
                dt_SPIDeltaChangeTracker.Columns.Add("SrNo", typeof(string));
                dt_SPIDeltaChangeTracker.Columns.Add("Item Description", typeof(string));
                dt_SPIDeltaChangeTracker.Columns.Add("Count", typeof(string));

                dt_SPIDeltaChangeTracker.Rows.Add("1", "Newly Created in Legacy Database post backup taken for Migration", $"{int_NCLD}");
                dt_SPIDeltaChangeTracker.Rows.Add("2", "Data Updated in  Legacy Database post backup taken for Migration", $"{int_DULD}");
                dt_SPIDeltaChangeTracker.Rows.Add("3", "Deleted in Legacy Database post backup taken for Migration", $"{int_DLDM}");

                dt_SPIDeltaChangeTracker.TableName = "SPI Delta Change Tracker";
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
                throw ex;
            }
            return dt_SPIDeltaChangeTracker;
        }
        private void displayElapsedTime()
        {
            try
            {
                time_to_execute = DateTime.Now.Subtract(start_Time);
                if (time_to_execute.Hours == 0)
                {
                    lblTimer.Text = time_to_execute.Minutes.ToString() + " min(s) " + time_to_execute.Seconds.ToString() + " second(s) ";
                }
                else
                {
                    lblTimer.Text = time_to_execute.Hours.ToString() + " hour(s) " +
                        time_to_execute.Minutes.ToString() + " min(s) " + time_to_execute.Seconds.ToString() + " second(s) ";
                }
                lblTimer.Update();
            }
            catch (Exception ex)
            {
                myException_BG = ex;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage <= 100)
                {
                    lbl_PercentageShow.Text = e.ProgressPercentage + " % ";
                    progressBar1.Value = e.ProgressPercentage;

                    displayElapsedTime();

                    lblStatus.Text = "Total matched :" + count_match + " and total unmatched : " + count_unmatched;
                }
                else if (e.ProgressPercentage == 105)
                {
                    lblStatus.Text = "All excel files are written and saved";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 110)
                {
                    lblStatus.Text = "Unmatched formatting done";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 115)
                {
                    lblStatus.Text = "Formatting done for other excel sheets";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 120)
                {
                    lblStatus.Text = "Deleting of unwanted columns started";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 125)
                {
                    lblStatus.Text = "Unwanted columns deleted";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 125)
                {
                    lblStatus.Text = "Excel File Saved";
                    displayElapsedTime();
                }

                else
                {
                    lblStatus.Text = "Excel file saved and all procedures completed successfully";
                    displayElapsedTime();
                    lbl_PercentageShow.Text = "100 %";
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = myException_BG.Message.ToString();
            }
            finally
            {
                progressBar1.Update();
                lbl_PercentageShow.Update();
                lblStatus.Update();
                lblExpressionStatus.Update();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    lblStatus.Text = "Process Cancelled";
                    DisplayLog(lblStatus.Text);
                }
                else if (e.Error != null)
                {
                    lblExpressionStatus.Text = e.Error.Message.ToString();
                    lblStatus.Text = e.Error.Message.ToString();
                    DisplayLog(e.Error);
                }
                else if (strFilePathChangeSave != "" && boolStatus)
                {
                    lblStatus.Text = "Done and Opening file in path " + strFilePathChangeSave;
                    lbl_PercentageShow.Text = lbl_PercentageShow.Text + " Done ";
                    displayElapsedTime();
                    Process.Start(strFilePathChangeSave);
                }
                else if (myException_BG.Source != null)
                {
                    lblStatus.Text = myException_BG.Source;
                    lblExpressionStatus.Text = myException_BG.Message.ToString();
                    DisplayLog(myException_BG);
                }
                else
                {
                    lblExpressionStatus.Text = "Some other error ";
                    lblStatus.Text = "Error : " + myException_BG.Message.ToString();
                    DisplayLog(lblStatus.Text);
                }

            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
            }
            finally
            {
                toggleCondition(true);
                btn_Cancel.Enabled = false;
                if (myException_BG.InnerException != null)
                {
                    lblStatus.Text = myException_BG.Message.ToString();
                    lblExpressionStatus.Text = myException_BG.Message.ToString();
                    DisplayLog(myException_BG);
                }
            }
        }

        private void cb_windowsAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_WindowAuthentication_TRG.Checked)
                {
                    tb_TRG_UserName.Enabled = false;
                    tb_TRG_Password.Enabled = false;
                }
                else if (cb_WindowAuthentication_TRG.CheckState == CheckState.Unchecked)
                {
                    tb_TRG_UserName.Enabled = true;
                    tb_TRG_Password.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblStatus.Text = ex.Message.ToString();
            }
        }

        private void rb_Target_Oracle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gb_Target1.Text = "Target : Oracle Connection and Settings";
                cb_WindowAuthentication_TRG.CheckState = CheckState.Unchecked;
                cb_WindowAuthentication_TRG.Enabled = false;
                cb_TRG_DefaultPort.CheckState = CheckState.Checked;
                cb_TRG_DefaultPort.Enabled = true;

                tb_TRG_DefaultPort.Enabled = false;
                tb_TRG_DefaultPort.Text = "1521";
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblStatus.Text = ex.Message.ToString();
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
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void UpdateLabel(Label lb, string str_msg = null)
        {
            try
            {
                lb.Text = str_msg;
                lb.Update();
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void btn_Load_SRC_Table_Click(object sender, EventArgs e)
        {
            try
            {
                resetLabelsToDefaultValue();
                UpdateLabel(lblStatus, "Pls Wait, Loading !!!");
                if (rb_SRC_oracle.Checked)
                {
                    strQuery = "SELECT * FROM all_tables where OWNER != 'SYS' ORDER BY OWNER, TABLE_NAME";
                    DisplayLog(strQuery);
                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_SRC_DataSource.Text + " )(PORT = " + tb_SRC_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_SRC_DBName.Text + ")));"
                        + "User Id=" + tb_SRC_UserName.Text + ";Password=" + tb_SRC_Password.Text + ";";
                    DisplayLog(strOracleConnection);
                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_src = new DataTable();
                    oracleDA.Fill(dt_src);
                    dt_src.TableName = "Load Table Name";
                    FillCheckListBox(cb_SRC_TableName, dt_src, "oracle");
                    dt_LoadSRC = dt_src.Copy();
                }

                if (rb_SRC_SQL.Checked)
                {
                    strQuery = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";
                    DisplayLog(strQuery);
                    if (cb_WindowAuthentication_SRC.CheckState == CheckState.Checked)
                    {
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    + "; Integrated Security = true ";
                    }
                    else if (cb_WindowAuthentication_SRC.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    + "; User ID = " + tb_SRC_UserName.Text + "; " + "Password = " + tb_SRC_Password.Text + ";";
                    }
                    DisplayLog(strSQLConnection);
                    dt_trg = new DataTable();
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_trg);

                    strSourceName = "Source SQL";
                    FillCheckListBox(cb_SRC_TableName, dt_trg, "sql");
                    dt_LoadSRC = dt_trg.Copy();
                }
                lblStatus.Text = "Table Loaded in Source";
                Timer t = new Timer();
                t.Interval = 3000;
                t.Tick += (s, e1) =>
                {
                    UpdateLabel(lblStatus);
                    t.Stop();
                };
                t.Start();
                DisplayLog("Loaded Success");
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void btn_Load_TRG_Table_Click(object sender, EventArgs e)
        {
            try
            {
                resetLabelsToDefaultValue();
                UpdateLabel(lblStatus, "Pls Wait, Loading !!!");
                if (rb_TRG_Oracle.Checked)
                {
                    strQuery = "SELECT * FROM all_tables where OWNER != 'SYS' ORDER BY OWNER, TABLE_NAME";
                    DisplayLog(strQuery);
                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_TRG_DataSource.Text + " )(PORT = " + tb_TRG_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_TRG_DBName.Text + ")));"
                        + "User Id=" + tb_TRG_UserName.Text + ";Password=" + tb_TRG_Password.Text + ";";
                    DisplayLog(strOracleConnection);
                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_src = new DataTable();
                    oracleDA.Fill(dt_src);
                    dt_src.TableName = "Load Table Name";
                    FillCheckListBox(cb_TRG_TableName, dt_src, "oracle");
                }

                if (rb_TRG_SQL.Checked)
                {
                    strQuery = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";
                    DisplayLog(strQuery);
                    if (cb_WindowAuthentication_TRG.CheckState == CheckState.Checked)
                    {
                        strSQLConnection = "Data Source = " + tb_TRG_DataSource.Text + ";Initial Catalog = " + tb_TRG_DBName.Text
                        + "; Integrated Security = true ";
                    }
                    else if (cb_WindowAuthentication_TRG.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = "Data Source = " + tb_TRG_DataSource.Text + ";Initial Catalog = " + tb_TRG_DBName.Text
                        + "; User ID = " + tb_TRG_UserName.Text + "; " + "Password = " + tb_TRG_Password.Text + ";";
                    }
                    DisplayLog(strSQLConnection);
                    dt_trg = new DataTable();
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_trg);
                    dt_trg.TableName = "Load Table Name";
                    FillCheckListBox(cb_TRG_TableName, dt_trg, "sql");
                }
                lblStatus.Text = "Table Loaded in Target";
                Timer t = new Timer();
                t.Interval = 3000;
                t.Tick += (s, e1) =>
                {
                    UpdateLabel(lblStatus);
                    t.Stop();
                };
                t.Start();
                DisplayLog("Loaded Success");
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void llbl_Folder_path_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip.SetToolTip(llbl_Folder_path, llbl_Folder_path.Text);
                toolTip.Show(llbl_Folder_path.Text, this, 30000);
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void DisplayError(Exception ex = null)
        {
            try
            {
                DisplayDebugMsg(ex.ToString());
                DisplayLog(ex.ToString());
                DisplayLog(ex.Message.ToString());
                if (ex != null)
                {
                    lblExpressionStatus.Text = ex.Message.ToString();
                    lblExpressionStatus.ForeColor = Color.Red;
                    if (Debugger.IsAttached)
                    {
                        lblExpressionStatus.Text = ex.ToString();
                    }
                }
                else
                {
                    lblExpressionStatus.Text = string.Empty;
                }
                lblExpressionStatus.Update();
            }
            catch (Exception ex1)
            {
                DisplayLog(ex);
                lblStatus.Text = ex1.Message.ToString();
            }
        }
        private void DisplayMemberInComboBox()
        {
            try
            {
                if (cb_FullName.Checked)
                {
                    cb_SRC_TableName.DisplayMember = "CompleteName";
                    cb_TRG_TableName.DisplayMember = "CompleteName";
                    cb_SRC_TableName.ValueMember = "CompleteName";
                    cb_TRG_TableName.ValueMember = "CompleteName";
                }
                else
                {
                    cb_SRC_TableName.DisplayMember = "TABLE_NAME";
                    cb_TRG_TableName.DisplayMember = "TABLE_NAME";
                    cb_SRC_TableName.ValueMember = "CompleteName";
                    cb_TRG_TableName.ValueMember = "CompleteName";
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void cb_FullName_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                DisplayMemberInComboBox();
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void btn_Copy_SRC_TRG_Click(object sender, EventArgs e)
        {
            try
            {
                rb_TRG_Oracle.Checked = rb_SRC_oracle.Checked;
                rb_TRG_SQL.Checked = rb_SRC_SQL.Checked;
                cb_WindowAuthentication_TRG.Checked = cb_WindowAuthentication_SRC.Checked;
                cb_TRG_DefaultPort.Checked = cb_SRC_DefaultPort.Checked;
                tb_TRG_DefaultPort.Text = tb_SRC_DefaultPort.Text;
                tb_TRG_DataSource.Text = tb_SRC_DataSource.Text;
                tb_TRG_DBName.Text = tb_SRC_DBName.Text;
                tb_TRG_UserName.Text = tb_SRC_UserName.Text;
                tb_TRG_Password.Text = tb_SRC_Password.Text;
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void btn_TRG_SRC_Click(object sender, EventArgs e)
        {
            try
            {
                rb_SRC_oracle.Checked = rb_TRG_Oracle.Checked;
                rb_SRC_SQL.Checked = rb_TRG_SQL.Checked;
                cb_WindowAuthentication_SRC.Checked = cb_WindowAuthentication_TRG.Checked;
                cb_SRC_DefaultPort.Checked = cb_TRG_DefaultPort.Checked;
                tb_SRC_DefaultPort.Text = tb_TRG_DefaultPort.Text;
                tb_SRC_DataSource.Text = tb_TRG_DataSource.Text;
                tb_SRC_DBName.Text = tb_TRG_DBName.Text;
                tb_SRC_UserName.Text = tb_TRG_UserName.Text;
                tb_SRC_Password.Text = tb_TRG_Password.Text;
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void llbl_Folder_path_Click(object sender, EventArgs e)
        {

        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            try
            {
                rtb.ResetText();
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        //public class JsonData
        //{
        //    public string str_ProfileName1 { get; set; }
        //    public JsonDataDB jsonDataDB { get; set; }
        //}
        public class JsonData
        {
            public dynamic str_ProfileName1 { get; set; }
            public DB source { get; set; }
            public DB target { get; set; }
        }


        private void ConnectToDB(string strFor = null)
        {
            try
            {
                if (strFor == null || strFor.ToUpper() == "SRC")
                {
                    str_DBName = tb_SRC_DBName.Text.Trim();
                    //string str_SchemaName = tb_SchemaName.Text.Trim();

                    if (rb_SRC_oracle.Checked)
                    {
                        DisplayMessage("Trying to connect to Source Oracle...", false);
                        dbSRCConnection = dbConnection.DatabaseConnection(rb_SRC_oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_SRC_DefaultPort.Text.Trim());
                    }
                    else if (rb_SRC_SQL.Checked)
                    {
                        DisplayMessage("Trying to connect to Source SQL...", false);
                        dbSRCConnection = dbConnection.DatabaseConnection(rb_SRC_SQL.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_SRC_DefaultPort.Text.Trim());
                    }
                    Common_Data.DisplayMessage(rtb, dbSRCConnection.strConnectionMSG, (!dbSRCConnection.boolConnection), -1, Color.DarkBlue);

                    if (dbSRCConnection.exceptionConnection != null)
                    {
                        DisplayError(dbSRCConnection.exceptionConnection);
                    }

                }

                if (strFor == null || strFor.ToUpper() == "TRG")
                {
                    str_DBName = tb_TRG_DBName.Text.Trim();
                    //str_SchemaName = tb_SchemaName.Text.Trim();

                    if (rb_TRG_Oracle.Checked)
                    {
                        DisplayMessage("Trying to connect to Target Oracle...", false);
                        dbTRGConnection = dbConnection.DatabaseConnection(rb_TRG_Oracle.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                    }
                    else if (rb_TRG_SQL.Checked)
                    {
                        DisplayMessage("Trying to connect to Target SQL...", false);
                        dbTRGConnection = dbConnection.DatabaseConnection(rb_TRG_SQL.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                    }
                    Common_Data.DisplayMessage(rtb, dbTRGConnection.strConnectionMSG, (!dbTRGConnection.boolConnection), -1, Color.DarkBlue);
                    if (dbTRGConnection.exceptionConnection != null)
                    {
                        DisplayError(dbTRGConnection.exceptionConnection);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                dbSRCConnection.boolConnection = false;
                dbTRGConnection.boolConnection = false;
                DisplayError(ex);
            }
        }

        private void llbl_Folder_path_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                llbl_Folder_path.Text = ofdd.folderBrowser(llbl_Folder_path.Text);
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void btn_ListTableExcel_Click(object sender, EventArgs e)
        {
            try
            {
                Common_Data.DisplayMessage(rtb, "Connecting to DB");
                ConnectToDB();
                Common_Data.DisplayMessage(rtb, "Process Done with DB");

                btnGenerateOutput.Enabled = false;
                DisplayMessage("Browsing Contained List Table Excel File ... ", false);
                openFileDialogData = openFileDialogData.openFileDialog("Select Excel File Contain Table List");

                if (openFileDialogData.boolFileSelected)
                {
                    dsTableList = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath, "Table List");

                    if (dsTableList != null && dsTableList.Tables.Count > 0 && dsTableList.Tables[0].Rows.Count > 0)
                    {
                        DisplayMessage($"Selected File {openFileDialogData.strFileNameWithPath}", false);
                        boolExcelFileList = true;
                        Common_Data.DisplayMessage(rtb, $"File Read Successfully", false, -1, Color.DarkBlue);
                        btnGenerateOutput.Enabled = true;
                    }
                    else
                    {
                        DisplayMessage($"File Not Selected", true);
                        boolExcelFileList = false;
                    }
                }
                else
                {
                    DisplayMessage($"File Not Selected", true);
                    boolExcelFileList = false;
                }

            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void DisplayMessage(string strMsg, bool boolError = false)
        {
            try
            {
                DisplayDebugMsg(strMsg);
                lblStatus.ForeColor = Color.Black;
                lblStatus.Text = strMsg;
                if (boolError)
                {
                    lblStatus.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private bool backGroundWorkList(DataRow drL, DoWorkEventArgs e)
        {
            string[] str_PK;
            try
            {
                //start_Time = DateTime.Now;

                str_PK = drL["primary_key_columns"].ToString().ToUpper().Split(',');

                ds_change = new DataSet();
                dt_change = new DataTable();

                count_match = 0;
                count_unmatched = 0;

                dt_trg.CaseSensitive = true;

                dv_sql = new DataView(dt_trg);
                dv_oracle = new DataView(dt_src);

                dt_sql_only = dt_trg.Copy();

                dt_unmatched_Capture = new DataTable();
                dt_unmatched_Capture = dt_src.Clone();

                DataGridView dataGridView2 = new DataGridView();

                foreach (DataColumn dc in dt_unmatched_Capture.Columns)
                {
                    dataGridView2.Columns.Add(dc.ColumnName, dc.ColumnName);
                }

                dt_oracle_only = dt_src.Clone();
                dt_summarised_difference = dt_src.Clone();

                foreach (DataColumn dc in dt_src.Columns)
                {
                    dt_summarised_difference.Columns[dc.ToString()].DataType = typeof(Int32);
                }
                DataRow dr_summarised = dt_summarised_difference.NewRow();

                foreach (DataColumn dc in dt_src.Columns)
                {
                    dr_summarised[dc.ToString()] = 0;
                }

                dt_summarised_difference.Rows.Add(dr_summarised);

                ds_unmatched_Capture = new DataSet();

                //headRow = -1;
                //alternateRow = 0;
                newRow = true;

                #region check for match and unmatch columns

                progress = 0;
                totalProgressCount = dt_src.Rows.Count;

                string str_col_used = string.Empty;

                cic_oracleTB = cbL_columns.CheckedItems;
                cic_reference = clb_ReferenceID.CheckedItems;

                // New Code
                //ds_CompleteSet = new DataSet();
                //ds_CompleteSet.Tables.Add(dt_src.Copy());
                //New Code Ends here

                #endregion

                #region main comparison part

                foreach (DataRow dr in dt_src.Rows)
                {
                    str_unique_reference_key = generateRowFilterStringForAll(drL, dr);

                    dv_sql.RowFilter = str_unique_reference_key;

                    percentage = (((++progress) * 100) / totalProgressCount);
                    backgroundWorker2.ReportProgress(percentage);

                    if (backgroundWorker2.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker2.ReportProgress(0);
                        boolCompare_List = false;
                        return boolCompare_List;
                    }

                    if (dv_sql.Count > 0)
                    {
                        delete_current_row_from_sql_table();

                        foreach (var dc in dt_src.Columns)
                        {
                            if (!(str_PK.Contains(dc.ToString())))
                            {
                                str_Data_Source_To_Compare = dr[dc.ToString()].ToString().Trim();
                                str_Data_Target_To_Compare = dv_sql[0][dc.ToString()].ToString().Trim();

                                bool boolTest = common_Data.MismatchTest(str_Data_Source_To_Compare, str_Data_Target_To_Compare, cb_IgnoreCase.Checked, cb_IncludeRule.Checked, dc.ToString(), dc.ToString(), dr, dr, dtSRC.Columns, dtTRG.Columns);

                                if (boolTest)
                                {
                                    Console.WriteLine((dr[dc.ToString()].ToString()).GetType());
                                    Console.WriteLine((dr[dc.ToString()].ToString()));

                                    #region exception handling of float values start here

                                    boolOracle = float.TryParse(str_Data_Source_To_Compare, out float float_oracle);
                                    boolSQL = float.TryParse(str_Data_Target_To_Compare, out float float_sql);

                                    if (boolOracle & boolSQL)
                                    {
                                        if (float_oracle == float_sql)
                                        {
                                            continue;
                                        }
                                    }
                                    #endregion exception hadling of float values end here

                                    cellCol = dt_src.Columns[dc.ToString()].Ordinal;
                                    str_colName = dc.ToString();
                                    if (newRow)
                                    {
                                        dt_unmatched_Capture.Rows.Add(dr.ItemArray);
                                        dt_unmatched_Capture.Rows.Add(dv_sql[0].Row.ItemArray);
                                        newRow = false;
                                        int_Color_Row += 2;

                                        //dataGridView2.Rows.Add(dt_unmatched_Capture.Rows[0].ItemArray);
                                        //dataGridView2.Rows.Add(dt_unmatched_Capture.Rows[1].ItemArray);
                                    }

                                    colorPairsMultiSheet.Add(new ColorPairMultiSheet(int_Color_Row - 1, int_Color_Row, dc.ToString(), int_sheet));
                                    //dataGridView2.Rows[headRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;
                                    //dataGridView2.Rows[alternateRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;

                                    dt_summarised_difference.Rows[0][str_colName] =
                                            Convert.ToInt32((dt_summarised_difference.Rows[0][str_colName]).ToString()) + 1;

                                    count_unmatched++;
                                }
                                else
                                {
                                    count_match++;
                                }
                            }
                        }
                        newRow = true;
                    }
                    else
                    {
                        dt_oracle_only.Rows.Add(dr.ItemArray);
                    }

                }

                #endregion

                // writing excel sheet
                /////////////////////////////////////////////////////////////////////////////

                //==========================================================================================================

                #region writing and formatting excel sheets

                ds_change.Tables.Add(dt_change);

                #region for deleting columns which are not required

                int index = 0;
                int adjust = 0;
                foreach (DataColumn dc in dt_summarised_difference.Columns)
                {
                    index = dc.Ordinal - adjust;
                    //if (!(clb_ReferenceID.GetItemChecked(clb_ReferenceID.FindString(dc.ColumnName.ToString()))))
                    if (!(str_PK.Contains(dc.ToString())))
                    {
                        if (Convert.ToInt32(dt_summarised_difference.Rows[0][dc.ColumnName.ToString()]) == 0)
                        {
                            dt_unmatched_Capture.Columns.Remove(dc.ColumnName);
                            dataGridView2.Columns.Remove(dc.ColumnName.ToString());
                            adjust++;
                        }
                    }
                }

                #endregion

                generateTransposeTable();

                wb = new XLWorkbook();
                wb.Worksheets.Add(dt_unmatched_Capture, "Mismatch");
                wb.Worksheets.Add(dt_oracle_only, "Source rows only");

                dt_sql_only = new DataTable();

                //ds_CompleteSet = new DataSet();
                //ds_CompleteSet.Tables.Add(dt_trg);

                dt_sql_only = RowsOnlyInTableList(drL, e, ds_CompleteSet.Tables[0], dt_trg);
                wb.Worksheets.Add(dt_sql_only, "Target rows only");
                //wb.Worksheets.Add(RowsOnlyInTable(e, ds_CompleteSet.Tables[0], dt_trg), "Sql rows only");

                wb.Worksheets.Add(dt_transpose, "Mismatch Details");
                wb.Worksheets.Add(SPIDeltaChangeTracker_Sheet(dt_oracle_only.Rows.Count, dt_unmatched_Capture.Rows.Count / 2, dt_sql_only.Rows.Count), "Summary");

                backgroundWorker2.ReportProgress(105);

                formatExcelTableByDataGridView(1, dataGridView2);

                backgroundWorker2.ReportProgress(110);

                formatExcelTable(2, dt_oracle_only);
                formatExcelTable(3, dt_sql_only);
                formatExcelTable(4, dt_transpose);
                formatExcelTable(5, dt_SPIDeltaChangeTracker);

                backgroundWorker2.ReportProgress(115);

                strFilePathChangeSave = $@"{llbl_Folder_path.Text}\{common_Data.RemoveSpecialCharacters(drL["src_table_names"].ToString())}_{common_Data.RemoveSpecialCharacters(drL["trg_table_names"].ToString())}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";

                wb.SaveAs(strFilePathChangeSave);

                #endregion

                #region  removing columns which are not needed to be displayed as no mismatched found

                backgroundWorker2.ReportProgress(125);

                wb.Save();

                backgroundWorker2.ReportProgress(130);
                #endregion
                //stop_Time = DateTime.Now;
                boolCompare_List = true;
            }

            catch (Exception ex)
            {
                e.Result = ex.Message.ToString();
                myException_BG = ex;
                boolCompare_List = false;
            }

            return boolCompare_List;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                start_Time = DateTime.Now;
                bool boolCompare_List = Compare_List(e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage == -1)
                {
                    DisplayLog(e.UserState.ToString());
                    return;
                }

                if (e.ProgressPercentage <= 100 & e.ProgressPercentage > 0)
                {
                    lbl_PercentageShow.Text = e.ProgressPercentage + " % ";
                    progressBar1.Value = e.ProgressPercentage;

                    displayElapsedTime();

                    lblStatus.Text = "Total matched :" + count_match + " and total unmatched : " + count_unmatched;
                }
                else if (e.ProgressPercentage == 105)
                {
                    lblStatus.Text = "All excel files are written and saved";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 110)
                {
                    lblStatus.Text = "Unmatched formatting done";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 115)
                {
                    lblStatus.Text = "Formatting done for other excel sheets";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 120)
                {
                    lblStatus.Text = "Deleting of unwanted columns started";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 125)
                {
                    lblStatus.Text = "Unwanted columns deleted";
                    displayElapsedTime();
                }
                else if (e.ProgressPercentage == 125)
                {
                    lblStatus.Text = "Excel File Saved";
                    displayElapsedTime();
                }
                else
                {
                    lblStatus.Text = "Excel file saved and all procedures completed successfully";
                    displayElapsedTime();
                    lbl_PercentageShow.Text = "100 %";
                }

            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = myException_BG.Message.ToString();
            }
            finally
            {
                progressBar1.Update();
                lbl_PercentageShow.Update();
                lblStatus.Update();
                lblExpressionStatus.Update();
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    lblStatus.Text = "Process Cancelled";
                    DisplayLog(lblStatus.Text);
                }
                else if (e.Error != null)
                {
                    lblExpressionStatus.Text = e.Error.Message.ToString();
                    lblStatus.Text = e.Error.Message.ToString();
                    DisplayLog(lblStatus.Text);
                }
                else if (strFilePathChangeSave != "" && boolStatus && cb_ListProcess.Checked)
                {
                    lblStatus.Text = "Done and Opening file in path " + llbl_Folder_path.Text;
                    lbl_PercentageShow.Text = lbl_PercentageShow.Text + " Done ";
                    stop_Time = DateTime.Now;
                    displayElapsedTime();
                    Process.Start(llbl_Folder_path.Text);
                }
                else if (myException_BG.Source != null)
                {
                    lblStatus.Text = myException_BG.Source;
                    lblExpressionStatus.Text = myException_BG.Message.ToString();
                    DisplayLog(lblStatus.Text);
                    DisplayLog(myException_BG);
                }
                else
                {
                    lblExpressionStatus.Text = "Some other error ";
                    lblStatus.Text = "Error : " + myException_BG.Message.ToString();
                    DisplayLog(lblStatus.Text);
                }

            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
            }
            finally
            {
                toggleCondition(true);
                btn_Cancel.Enabled = false;
                if (myException_BG.InnerException != null)
                {
                    lblStatus.Text = myException_BG.Message.ToString();
                    lblExpressionStatus.Text = myException_BG.Message.ToString();
                    DisplayLog(myException_BG);
                }
            }
        }

        private void cb_ListProcess_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_ListProcess.Checked)
                {
                    btn_ListTableExcel.Enabled = true;
                    tb_FileName.Enabled = false;
                    btn_Set.Enabled = false;
                    btn_Connect_Reference_ID.Enabled = false;
                    cb_ColumnMapping.Enabled = false;
                }
                else
                {
                    btn_ListTableExcel.Enabled = false;
                    tb_FileName.Enabled = true;
                    btn_Set.Enabled = true;
                    btn_Connect_Reference_ID.Enabled = true;
                    cb_ColumnMapping.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        private void cb_UserProfileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DisplayMessage($"User Profile Selected : {cb_UserProfileList.SelectedItem}", false);

                dBConnectionDetails = new DBConnectionDetails(cb_UserProfileList.SelectedIndex);

                rb_SRC_oracle.Checked = dBConnectionDetails.bool_Rb_SRC_Oracle;
                rb_SRC_SQL.Checked = dBConnectionDetails.bool_Rb_SRC_SQL;

                cb_WindowAuthentication_SRC.Checked = dBConnectionDetails.bool_SRC_SQL_WA;
                cb_SRC_DefaultPort.Checked = dBConnectionDetails.bool_SRC_Oracle_DefaultPort;
                tb_SRC_DefaultPort.Enabled = dBConnectionDetails.bool_Rb_SRC_Oracle ? dBConnectionDetails.bool_SRC_Oracle_DefaultPort ? false : true : false;
                //tb_SRC_DefaultPort.Text = dBConnectionDetails.bool_Rb_SRC_Oracle ? dBConnectionDetails.int_SRC_Oracle_DefaultPort.ToString() : string.Empty;
                tb_SRC_DefaultPort.Text = dBConnectionDetails.int_SRC_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_SRC_Oracle_DefaultPort.ToString();

                tb_SRC_DataSource.Text = dBConnectionDetails.str_SRC_DataSource;
                tb_SRC_DBName.Text = dBConnectionDetails.str_SRC_DBName;
                tb_SRC_UserName.Text = dBConnectionDetails.str_SRC_UserName;
                tb_SRC_Password.Text = dBConnectionDetails.str_SRC_Password;
                cb_SRC_TableName.Text = dBConnectionDetails.str_SRC_TableName;

                rb_TRG_Oracle.Checked = dBConnectionDetails.bool_Rb_TRG_Oracle;
                rb_TRG_SQL.Checked = dBConnectionDetails.bool_Rb_TRG_SQL;

                cb_WindowAuthentication_TRG.Checked = dBConnectionDetails.bool_TRG_SQL_WA;
                cb_TRG_DefaultPort.Checked = dBConnectionDetails.bool_TRG_Oracle_DefaultPort;
                tb_TRG_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString();

                tb_TRG_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                tb_TRG_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                tb_TRG_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                tb_TRG_Password.Text = dBConnectionDetails.str_TRG_Password;
                cb_TRG_TableName.Text = dBConnectionDetails.str_TRG_TableName;

                DisplayMessage($"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully", false);
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
            }
        }

        public class DB
        {
            public string str_dbType { get; set; }
            public string str_dbSource { get; set; }
            public string str_dbName { get; set; }
            public string str_dbUsername { get; set; }
            public string str_dbTablename { get; set; }
        }

        private void btn_SaveProfile_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (Debugger.IsAttached)
            //    {
            //        Random random = new Random(500);
            //        tb_ProfileName.Text = random.Next(100, 10000).ToString();
            //        tb_ProfileName.Update();
            //    }

            //    List<JsonData> _Jsondata = new List<JsonData>();
            //    //string str_ProfileName = tb_ProfileName.Text;

            //    dynamic dynamic_obj;
            //    dynamic_obj = tb_ProfileName.Text;

            //    _Jsondata.Add(new JsonData()
            //    {
            //        str_ProfileName1 = dynamic_obj,

            //        source = new DB
            //        {
            //            str_dbType = "a1",
            //            str_dbSource = "b1",
            //            str_dbName = "c1",
            //            str_dbUsername = "d1",
            //            str_dbTablename = "e1"
            //        },
            //        target = new DB
            //        {
            //            str_dbType = "a2",
            //            str_dbSource = "b2",
            //            str_dbName = "c2",
            //            str_dbUsername = "d2",
            //            str_dbTablename = "e2"
            //        }
            //    });

            //    using (StreamWriter file = File.CreateText(@"D:\path.json"))
            //    {
            //        JsonSerializer serializer = new JsonSerializer();
            //        //serialize object directly into file stream
            //        serializer.Serialize(file, _Jsondata);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    DisplayError(ex);
            //}
        }

        private void rb_Target_SQL_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gb_Target1.Text = "Target : SQL Connection and Settings";
                cb_WindowAuthentication_TRG.Enabled = true;
                cb_WindowAuthentication_TRG.CheckState = CheckState.Unchecked;

                cb_TRG_DefaultPort.CheckState = CheckState.Unchecked;
                cb_TRG_DefaultPort.Enabled = false;

                tb_TRG_DefaultPort.Enabled = false;
                tb_TRG_DefaultPort.ResetText();

            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblStatus.Text = ex.Message.ToString();
            }
        }

        private void cb_Target_DefaultPort_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_TRG_DefaultPort.Checked)
                {
                    tb_TRG_DefaultPort.Text = "1521";
                    tb_TRG_DefaultPort.Enabled = false;
                }
                else
                {
                    tb_TRG_DefaultPort.ResetText();
                    tb_TRG_DefaultPort.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblStatus.Text = ex.Message.ToString();
            }
        }

        private void cb_Default_Port_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_SRC_DefaultPort.Checked)
                {
                    tb_SRC_DefaultPort.Text = "1521";
                    tb_SRC_DefaultPort.Enabled = false;
                }
                else
                {
                    tb_SRC_DefaultPort.Text = string.Empty;
                    tb_SRC_DefaultPort.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                DisplayError(ex);
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

                if (backgroundWorker2.IsBusy)
                {
                    backgroundWorker2.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                DisplayLog(ex);
                lblExpressionStatus.Text = ex.Message.ToString();
            }
        }

        private void statusChangeforSetButton(bool enabledStatus)
        {
            cb_selectAll_toggle.Enabled = enabledStatus;

            if (enabledStatus)
            {
                cbL_columns.SelectionMode = SelectionMode.One;
                lb_TargetColumns.SelectionMode = SelectionMode.One;
                clb_ReferenceID.SelectionMode = SelectionMode.One;
            }
            else
            {
                cbL_columns.SelectionMode = SelectionMode.None;
                lb_TargetColumns.SelectionMode = SelectionMode.None;
                clb_ReferenceID.SelectionMode = SelectionMode.None;
            }

        }

    }
}
