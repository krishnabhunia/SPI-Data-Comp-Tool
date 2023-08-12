using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using Newtonsoft;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using ClosedXML;
using ClosedXML.Excel;
using System.Diagnostics;
//using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using form = System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.IO;
using Timer = System.Windows.Forms.Timer;
using Label = System.Windows.Forms.Label;
using ListBox = System.Windows.Forms.ListBox;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    public partial class DB_Comparison_Form : Form
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

        #endregion

        #region Dataset and Datatable and Dataview

        private DataSet dsRules;

        private DataTable dt_oracle;
        private DataSet ds;
        private OracleDataAdapter oracleDA;

        private DataTable dt_sql;

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
        private bool boolOracle, boolSQL, boolCompareResult;
        //private bool boolIgnoreCase;

        int headRow, alternateRow, cellCol;
        private int int_Count_Comparison;

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

        private DataSet dsAllColumns;
        #endregion

        private bool boolRules, boolRulesCheck;
        private ToolTip toolTip;// = new ToolTip();

        private OpenFileDialogData OpenFileDialogData_OBJ;
        private DBConnectionDetails dBConnectionDetails;

        private SaveFileDialogFile saveFileDialogFile;

        public DB_Comparison_Form()
        {
            InitializeComponent();

            try
            {
                llbl_File_path.Text = @"D:\";

                lblExpressionStatus.Text = string.Empty;
                cd = new Common_Data();
                ofdd = new OpenFileDialogData();

                myException_BG = new Exception();
                common_Data = new Common_Data();

                boolRules = false;
                dsRules = new DataSet();
                //checkRules = new CheckRules();

                dsAllColumns = new DataSet();

                toolTip = new ToolTip();
                dt_LoadSRC = new DataTable();
                dt_LoadTRG = new DataTable();

                dt_returnDataTable = new DataTable();

                dt_CommonSRC = new DataTable();
                dt_CommonTRG = new DataTable();

                saveFileDialogFile = new SaveFileDialogFile();
                //tb_FileName.Text = $"{common_Data.RemoveSpecialCharacters(cb_SRC_TableName.Text)}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}";

                OpenFileDialogData_OBJ = new OpenFileDialogData();
                int_Count_Comparison = 0;

                cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);
                //cb_UserProfileList = Common_Data.GetUserProfile(cb_UserProfileList);

                if (Common_Data.StringNotNullOrEmpty(Properties.Settings.Default.ProfilePathUser, ".json", false, true))
                {
                    DisplayMessage(lblStatus, $"User Profile Loaded from {Properties.Settings.Default.ProfilePathUser}");
                }

                if (GlobalDebug.boolIsGlobalDebug || (cb_UserProfileList.Items.Count > 0 && Debugger.IsAttached))
                {
                    cb_UserProfileList.SelectedIndex = 0;
                }

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }

        }

        private void DebugDataSet(int intExplicitCall)
        {
            try
            {
                if ((Debugger.IsAttached) || (intExplicitCall == 1))
                {
                    cb_UserProfileList.Visible = true;
                }
                else
                {
                    dataGridView1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = ex.Message.ToString();
            }

        }

        private string GetTableNameFromCB(ComboBox comboBox)
        {
            try
            {
                if (comboBox.SelectedValue != null && comboBox.SelectedValue.ToString() != "")
                {
                    return comboBox.SelectedValue.ToString();
                }
                else if (comboBox.Text != null && comboBox.Text != "")
                {
                    return comboBox.Text;
                }
                else if (comboBox.SelectedText != null && comboBox.SelectedText != "")
                {
                    return comboBox.SelectedText;
                }
                else if (comboBox.SelectedItem != null && comboBox.SelectedItem.ToString() != "")
                {
                    return comboBox.SelectedItem.ToString();
                }

                throw new Exception("No table able to detect from ComboBox");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckLengthOfTextBox(TextBox textBox)
        {
            try
            {
                if (!(textBox.Text.Length > 0))
                {
                    DisplayMessage(textBox, $"{textBox.Name} is not validated");
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool ValidateMyControls()
        {
            try
            {

                if (rb_SRC_oracle.Checked)
                {
                    return (CheckLengthOfTextBox(tb_SRC_DefaultPort) && CheckLengthOfTextBox(tb_SRC_DataSource) && CheckLengthOfTextBox(tb_SRC_DBName) && CheckLengthOfTextBox(tb_SRC_UserName) && CheckLengthOfTextBox(tb_SRC_Password));
                }
                else if (rb_SRC_SQL.Checked)
                {
                    if (cb_WindowAuthentication_SRC.Checked)
                    {
                        return (CheckLengthOfTextBox(tb_SRC_DataSource) && CheckLengthOfTextBox(tb_SRC_DBName));
                    }
                    else
                    {
                        return (CheckLengthOfTextBox(tb_SRC_DataSource) && CheckLengthOfTextBox(tb_SRC_DBName) && CheckLengthOfTextBox(tb_SRC_UserName) && CheckLengthOfTextBox(tb_SRC_Password));
                    }
                }

                if (rb_TRG_Oracle.Checked)
                {
                    return (CheckLengthOfTextBox(tb_TRG_DefaultPort) && CheckLengthOfTextBox(tb_TRG_DataSource) && CheckLengthOfTextBox(tb_TRG_DBName) && CheckLengthOfTextBox(tb_TRG_UserName) && CheckLengthOfTextBox(tb_TRG_Password));
                }
                else if (rb_TRG_SQL.Checked)
                {
                    if (cb_WindowAuthentication_TRG.Checked)
                    {
                        return (CheckLengthOfTextBox(tb_TRG_DataSource) && CheckLengthOfTextBox(tb_TRG_DBName));
                    }
                    else
                    {
                        return (CheckLengthOfTextBox(tb_TRG_DataSource) && CheckLengthOfTextBox(tb_TRG_DBName) && CheckLengthOfTextBox(tb_TRG_UserName) && CheckLengthOfTextBox(tb_TRG_Password));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                DisplayError(ex);
                return false;
            }
        }
        private DataSet connect_Oracle_DB(string columnNames = " * ")
        {
            try
            {
                dt_oracle = new DataTable();
                //ds = new DataSet();
                strQuery = $"select {columnNames} from {GetTableNameFromCB(cb_SRC_TableName)}";

                // oracle 11g and sql selection as requirement changed

                if (rb_SRC_oracle.Checked)
                {
                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_SRC_DataSource.Text + " )(PORT = " + tb_SRC_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_SRC_DBName.Text + ")));"
                        + "User Id=" + tb_SRC_UserName.Text + ";Password=" + tb_SRC_Password.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    oracleDA.Fill(dt_oracle);
                    strSourceName = "Source Oracle 11G";
                }

                if (rb_SRC_SQL.Checked)
                {
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

                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_oracle);
                    strSourceName = "Source SQL";
                }

                ds.Tables.Add(dt_oracle);
                ds.Tables[0].TableName = strSourceName;

            }
            catch (Exception ex)
            {
                myException_BG = ex;
                boolStatus = false;
                throw ex;
            }

            return ds;
        }

        private DataSet connect_SQL_DB(string columnNames = " * ")
        {
            try
            {
                //strQuery = "select " + columnNames + " from " + cb_TRG_TableName.SelectedValue;
                strQuery = $"select {columnNames} from {GetTableNameFromCB(cb_TRG_TableName)}";

                if (rb_TRG_Oracle.Checked)
                {
                    strSQLConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_TRG_DataSource.Text + " )(PORT = " + tb_TRG_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_TRG_DBName.Text + ")));"
                        + "User Id=" + tb_TRG_UserName.Text + ";Password=" + tb_TRG_Password.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_sql = new DataTable();
                    oracleDA.Fill(dt_sql);
                    strTargetName = "Target Oracle 11G";
                    ds.Tables.Add(dt_sql);
                }
                else if (rb_TRG_SQL.Checked)
                {
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

                    dt_sql = new DataTable();
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_sql);
                    ds.Tables.Add(dt_sql);

                }

                ds.Tables[1].TableName = "Target SQL Table";
            }
            catch (Exception ex)
            {
                myException_BG = ex;
                boolStatus = false;
            }
            return ds;
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

        private void transposeXML()
        {
            try
            {
                var RangeTranspose = wb.Worksheet(4).Range("A1:F3");
                RangeTranspose.Transpose(XLTransposeOptions.MoveCells);
                wb.Worksheet(4).Columns().AdjustToContents();
            }
            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = ex.Message.ToString();
            }

        }

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

                for (int col = 0; col <= dt_oracle.Columns.Count; col++)
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
                //DataRow[] rowsDel = dt_sql_only.Select(str_unique_reference_key + " = '" + dv_sql[0][str_unique_reference_key].ToString() + "'");
                DataRow[] rowsDel = dt_sql_only.Select(str_unique_reference_key);

                foreach (DataRow dr in rowsDel)
                {
                    dr.Delete();
                }
                dt_sql_only.AcceptChanges();
            }
            catch (Exception ex)
            {
                //lblExpressionStatus.Text = ex.Message.ToString();
                //lblStatus.Text = ex.Message.ToString();
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

                DataColumnCollection dcc = dt_sql.Columns;

                str_col_to_include_in_comparison = string.Empty;

                CheckedListBox.CheckedItemCollection cic_ReferenceID = clb_ReferenceID.CheckedItems;

                foreach (var item in cic_ReferenceID)
                {
                    int index = cbL_SRC_Cols.FindStringExact(item.ToString());
                    bool boolCheck = clb_ReferenceID.GetItemChecked(index);
                    cbL_SRC_Cols.SetItemChecked(index, !boolCheck);
                }

                foreach (object i in cbL_SRC_Cols.Items)
                {
                    if ((cbL_SRC_Cols.GetItemChecked(cbL_SRC_Cols.FindString(i.ToString()))))
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

                dt_sql = new DataTable();
                sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                sqlDA.Fill(dt_sql);
                ds = new DataSet();
                ds.Tables.Add(dt_sql);
                ds.Tables[0].TableName = "List of SQL Tables";

                DataTable dt = new DataTable();

                //FillCheckListBox();

            }
            catch (Exception ex)
            {
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
                DisplayMemberInComboBox();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        //private DataTable getCommonColumns(DataTable dt, DataTable dt_check, ListBox lb)
        private DataTable getCommonColumns(DataTable dt_returnDataTable, DataTable dt_check, ListBox lb)
        {
            try
            {
                //dt_returnDataTable = dt.Copy();
                lb.Items.Clear();
                DataColumnCollection dcc = dt_check.Columns;

                //foreach (DataColumn dc in dt.Columns)
                foreach (DataColumn dc in dt_returnDataTable.Columns)
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
                DisplayError(ex);
            }
            return dt_returnDataTable;
        }

        private void btn_Connect_Reference_ID_Click(object sender, EventArgs e)
        {
            try
            {
                resetLabelsToDefaultValue();
                boolStatus = true;
                clearCheckBoxList(clb_ReferenceID);
                clearCheckBoxList(cbL_SRC_Cols, cb_selectAll_toggle);
                clearCheckBoxList(lb_TRG_Cols);
                clearCheckBoxList(lb_IgnoredCol_SRC);
                clearCheckBoxList(lb_IgnoredCol_TRG);
                toggleCondition(false);

                ds = new DataSet();
                ds = connect_Oracle_DB();

                // conncet with SQL Target

                ds = connect_SQL_DB();

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
                        dt_CommonSRC = getCommonColumns(ds.Tables[0], ds.Tables[1], lb_IgnoredCol_SRC);
                        dt_CommonTRG = getCommonColumns(ds.Tables[1], ds.Tables[0], lb_IgnoredCol_TRG);

                        //foreach (DataColumn dc in ds.Tables[0].Columns)
                        foreach (DataColumn dc in dt_CommonSRC.Columns)
                        {
                            clb_ReferenceID.Items.Add(dc.ToString());
                            cbL_SRC_Cols.Items.Add(dc.ToString());
                        }

                        //foreach (DataColumn dc in ds.Tables[1].Columns)
                        foreach (DataColumn dc in dt_CommonTRG.Columns)
                        {
                            lb_TRG_Cols.Items.Add(dc.ToString());
                        }
                        lblExpressionStatus.Text = "Data Loaded Successfully";

                        if (cbL_SRC_Cols.Items.Count == 0)
                        {
                            UpdateLabel(lblExpressionStatus, "Data Loaded Successfull but there is no common Reference ID, Pls use Different Column Name and RefID comparison");
                        }
                    }
                    dsAllColumns = ds.Clone();
                }
                else
                {
                    lblExpressionStatus.Text = "Some Error in SQL or Oracle";
                }
                cb_selectAll_toggle.Checked = true;
            }
            catch (Exception ex)
            {
                //lblExpressionStatus.Text = ex.Message.ToString();
                DisplayError(ex);
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
            groupBox14.Enabled = enabledStatus;
            //gb_Source.Enabled = enabledStatus;
            //gb_Source1.Enabled = enabledStatus;
            //tb_SRC_DBName.Enabled = enabledStatus;
            //tb_SRC_DataSource.Enabled = enabledStatus;
            //tb_SRC_UserName.Enabled = enabledStatus;
            //tb_SRC_Password.Enabled = enabledStatus;
            //cb_SRC_TableName.Enabled = enabledStatus;

            //gb_Target.Enabled = enabledStatus;
            //gb_Target1.Enabled = enabledStatus;
            //tb_TRG_DBName.Enabled = enabledStatus;
            //tb_TRG_DataSource.Enabled = enabledStatus;
            //tb_TRG_UserName.Enabled = enabledStatus;
            //tb_TRG_Password.Enabled = enabledStatus;
            //cb_TRG_TableName.Enabled = enabledStatus;

            btn_Connect_Reference_ID.Enabled = enabledStatus;
            btn_Set.Enabled = enabledStatus;
            btnGenerateOutput.Enabled = enabledStatus;

            //tb_FileName.Enabled = enabledStatus;

            cb_selectAll_toggle.Enabled = enabledStatus;
            btn_Clear_Reset.Enabled = enabledStatus;
            cb_UserProfileList.Enabled = enabledStatus;

            llbl_File_path.Enabled = enabledStatus;

            //gb_Source.Enabled = enabledStatus;

            //cb_SRC_DefaultPort.Enabled = enabledStatus;

            //if (enabledStatus & (!(cb_SRC_DefaultPort.Checked)))
            //{
            //    tb_SRC_DefaultPort.Enabled = enabledStatus;
            //}
            //else if (cb_SRC_DefaultPort.Checked)
            //{
            //    tb_SRC_DefaultPort.Enabled = !(enabledStatus);
            //}
            //else if (!(enabledStatus))
            //{
            //    tb_SRC_DefaultPort.Enabled = enabledStatus;
            //}

            if (enabledStatus)
            {
                cbL_SRC_Cols.SelectionMode = SelectionMode.One;
                lb_TRG_Cols.SelectionMode = SelectionMode.One;
                clb_ReferenceID.SelectionMode = SelectionMode.One;
            }
            else
            {
                cbL_SRC_Cols.SelectionMode = SelectionMode.None;
                lb_TRG_Cols.SelectionMode = SelectionMode.None;
                clb_ReferenceID.SelectionMode = SelectionMode.One;
            }

        }

        private void cb_selectAll_toggle_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxListToggle(cb_selectAll_toggle.Checked);
        }

        private void checkBoxListToggle(bool enabledState)
        {
            for (int i = 0; i < cbL_SRC_Cols.Items.Count; i++)
            {
                cbL_SRC_Cols.SetItemChecked(i, enabledState);
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
                DisplayError(ex);
            }
        }

        private void DisplayMessage(Control control, string str_Msg, Color color)
        {
            try
            {
                control.Text = str_Msg;
                control.ForeColor = color;
                control.Update();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
        private void btn_Set_Click(object sender, EventArgs e)
        {
            try
            {
                resetLabelsToDefaultValue();
                toggleCondition(false);

                ListBox.ObjectCollection lb_oc_target = lb_TRG_Cols.Items;
                CheckedListBox.ObjectCollection cic_oc_source = cbL_SRC_Cols.Items;

                DataColumnCollection dcc = dt_sql.Columns;

                if (cbL_SRC_Cols.Items.Count == 0)
                {
                    DisplayMessage(lblStatus, "There is no common ID, Pls use Different Column Name and RefID comparison");
                    return;
                }

                foreach (DataColumn dc in dt_oracle.Columns)
                {
                    if (!(dcc.Contains(dc.ColumnName.ToString())) && (cbL_SRC_Cols.Items.Contains(dc.ColumnName.ToString())))
                    {
                        cbL_SRC_Cols.SetItemCheckState(cbL_SRC_Cols.FindStringExact(dc.ColumnName.ToString()), CheckState.Unchecked);

                        #region New code 
                        //cbL_columns.Items.Remove(dc.ColumnName);
                        lb_IgnoredCol_SRC.Items.Add(dc.ColumnName);
                        #endregion
                    }
                }

                //cbL_columns.SetItemChecked(cbL_columns.FindString(cb_referenceID.Text), false);

                foreach (var checkRef in clb_ReferenceID.CheckedItems)
                {
                    cbL_SRC_Cols.SetItemChecked(cbL_SRC_Cols.FindString(checkRef.ToString()), false);
                }

                columns_to_Ignore();

                if (clb_ReferenceID.CheckedItems.Count > 0 & cbL_SRC_Cols.CheckedItems.Count > 0)
                {
                    boolStatus = true;
                    //tb_FileName.Text = $"{common_Data.RemoveSpecialCharacters(cb_SRC_TableName.Text)}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}";
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

                int_total_rows = dt_oracle.Rows.Count;
                int_total_columns = dt_oracle.Columns.Count;

                lblCal.Text = "Rows : " + int_total_rows.ToString() +
                    " , Columns : " + int_total_columns.ToString() + " , Total checks to be performed : " +
                    (int_total_columns * int_total_rows).ToString();

            }
            catch (Exception ex)
            {
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
                if (rb_SRC_oracle.Checked)
                {
                    CheckChangeRadioButton(1);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void rb_sql_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_SRC_SQL.Checked)
                {
                    CheckChangeRadioButton(1);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void CheckChangeRadioButton(int int_Group)
        {
            try
            {
                if (int_Group == 1)
                {
                    if (rb_SRC_oracle.Checked)
                    {
                        gb_Source1.Text = "Source: Oracle Connection and Settings";
                        cb_WindowAuthentication_SRC.Enabled = false;
                        cb_WindowAuthentication_SRC.Checked = false;
                        cb_SRC_DefaultPort.Enabled = true;
                        cb_SRC_DefaultPort.Checked = true;
                        tb_SRC_DefaultPort.Enabled = true;
                        tb_SRC_DefaultPort.ReadOnly = true;
                    }
                    else if (rb_SRC_SQL.Checked)
                    {
                        gb_Source1.Text = "Source: SQL Connection and Settings";
                        cb_WindowAuthentication_SRC.Enabled = true;
                        cb_WindowAuthentication_SRC.Checked = false;
                        cb_SRC_DefaultPort.Enabled = false;
                        cb_SRC_DefaultPort.Checked = false;
                        tb_SRC_DefaultPort.Enabled = false;
                        tb_SRC_DefaultPort.ReadOnly = true;
                    }
                }
                else if (int_Group == 2)
                {
                    if (rb_TRG_Oracle.Checked)
                    {
                        gb_Target1.Text = "Target : Oracle Connection and Settings";
                        cb_WindowAuthentication_TRG.Enabled = false;
                        cb_WindowAuthentication_TRG.Checked = false;
                        cb_TRG_DefaultPort.Enabled = true;
                        cb_TRG_DefaultPort.Checked = true;
                        tb_TRG_DefaultPort.Enabled = true;
                        tb_TRG_DefaultPort.ReadOnly = true;

                    }
                    else if (rb_TRG_SQL.Checked)
                    {
                        gb_Target1.Text = "Target : SQL Connection and Settings";
                        cb_WindowAuthentication_TRG.Enabled = true;
                        cb_WindowAuthentication_TRG.Checked = false;
                        cb_TRG_DefaultPort.Enabled = false;
                        cb_TRG_DefaultPort.Checked = false;
                        tb_TRG_DefaultPort.Enabled = false;
                        tb_TRG_DefaultPort.ReadOnly = true;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DisplayMessage(lblStatus, $"User Profile Selected : {cb_UserProfileList.SelectedItem}");

                dBConnectionDetails = new DBConnectionDetails(cb_UserProfileList.SelectedIndex);

                rb_SRC_oracle.Checked = dBConnectionDetails.bool_Rb_SRC_Oracle;
                rb_SRC_SQL.Checked = dBConnectionDetails.bool_Rb_SRC_SQL;

                cb_WindowAuthentication_SRC.Checked = dBConnectionDetails.bool_SRC_SQL_WA;
                cb_SRC_DefaultPort.Checked = dBConnectionDetails.bool_SRC_Oracle_DefaultPort;
                tb_SRC_DefaultPort.Text = dBConnectionDetails.bool_Rb_SRC_Oracle ? dBConnectionDetails.int_SRC_Oracle_DefaultPort.ToString() : string.Empty;

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

                DisplayMessage(lblStatus, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
            }
            catch (Exception ex)
            {
                DisplayError(lblExpressionStatus, ex);
            }

            //if (GlobalDebug.boolIsGlobalDebug)
            //{
            //    switch (cb_UserProfileList.SelectedIndex + 1)
            //    {
            //        case 1:
            //            rb_SRC_oracle.Checked = true;
            //            tb_SRC_DataSource.Text = "IESWKCT670";
            //            tb_SRC_DBName.Text = "SPEL11G";
            //            tb_SRC_UserName.Text = "system";
            //            tb_SRC_Password.Text = "SPEL11G";
            //            cb_SRC_TableName.Text = "tb_compare1";

            //            rb_TRG_SQL.Checked = true;
            //            tb_TRG_DataSource.Text = "IESWKCT247";
            //            tb_TRG_DBName.Text = "db_SPELSQL";
            //            tb_TRG_UserName.Text = "sa";
            //            tb_TRG_Password.Text = "abcde@1234567";
            //            cb_TRG_TableName.Text = "tb_compare1";

            //            break;

            //        case 2:
            //            rb_SRC_SQL.Checked = true;
            //            tb_SRC_DataSource.Text = "IESWKCT247";
            //            tb_SRC_DBName.Text = "db_krishna";
            //            tb_SRC_UserName.Text = "sa";
            //            tb_SRC_Password.Text = "abcde@1234567";
            //            cb_SRC_TableName.Text = "tb_compare1";
            //            cb_SRC_TableName.Text = "component";

            //            rb_TRG_SQL.Checked = true;
            //            tb_TRG_DataSource.Text = "IESWKCT247";
            //            tb_TRG_DBName.Text = "db_krishna";
            //            tb_TRG_UserName.Text = "sa";
            //            tb_TRG_Password.Text = "abcde@1234567";
            //            cb_TRG_TableName.Text = "component";

            //            break;

            //        case 3:
            //            rb_SRC_SQL.Checked = true;
            //            tb_SRC_DataSource.Text = "IESWKCT247";
            //            tb_SRC_DBName.Text = "SPELOLDP";
            //            tb_SRC_UserName.Text = "sa";
            //            tb_SRC_Password.Text = "abcde@1234567";
            //            cb_SRC_TableName.Text = "plant1el.t_plantitem";

            //            rb_TRG_SQL.Checked = true;
            //            tb_TRG_DataSource.Text = "IESWKCT247";
            //            tb_TRG_DBName.Text = "SPELOLDP_new";
            //            tb_TRG_UserName.Text = "sa";
            //            tb_TRG_Password.Text = "abcde@1234567";
            //            cb_TRG_TableName.Text = "plant1el.t_plantitem";

            //            break;

            //        case 4:
            //            rb_SRC_oracle.Checked = true;
            //            tb_SRC_DataSource.Text = "IESWKCT670";
            //            tb_SRC_DBName.Text = "SPEL11G";
            //            tb_SRC_UserName.Text = "system";
            //            tb_SRC_Password.Text = "SPEL11G";
            //            cb_SRC_TableName.Text = "DASISLANDEL.T_motor";

            //            rb_TRG_SQL.Checked = true;
            //            tb_TRG_DataSource.Text = "IESWKCT247";
            //            tb_TRG_DBName.Text = "JK1703_P";
            //            tb_TRG_UserName.Text = "sa";
            //            tb_TRG_Password.Text = "abcde@1234567";
            //            cb_TRG_TableName.Text = "Plant789el.T_Motor";

            //            break;

            //        case 5:
            //            rb_SRC_oracle.Checked = true;
            //            tb_SRC_DataSource.Text = "IESWKCT670";
            //            tb_SRC_DBName.Text = "SPEL11G";
            //            tb_SRC_UserName.Text = "system";
            //            tb_SRC_Password.Text = "SPEL11G";
            //            cb_SRC_TableName.Text = "DASISLANDEL.T_Plantitem";

            //            rb_TRG_SQL.Checked = true;
            //            tb_TRG_DataSource.Text = "IESWKCT247";
            //            tb_TRG_DBName.Text = "JK1703_P";
            //            tb_TRG_UserName.Text = "sa";
            //            tb_TRG_Password.Text = "abcde@1234567";
            //            cb_TRG_TableName.Text = "plant789elp1.t_Plantitem";

            //            break;

            //        case 6:
            //            rb_SRC_oracle.Checked = true;
            //            tb_SRC_DataSource.Text = "IESWKCT670";
            //            tb_SRC_DBName.Text = "SPEL11G";
            //            tb_SRC_UserName.Text = "system";
            //            tb_SRC_Password.Text = "SPEL11G";
            //            cb_SRC_TableName.Text = "tb_loop";

            //            rb_TRG_SQL.Checked = true;
            //            tb_TRG_DataSource.Text = "IESWKCT247";
            //            tb_TRG_DBName.Text = "db_SPELSQL";
            //            tb_TRG_UserName.Text = "sa";
            //            tb_TRG_Password.Text = "abcde@1234567";
            //            cb_TRG_TableName.Text = "loop";

            //            break;
            //        case 7:
            //            rb_SRC_oracle.Checked = true;
            //            tb_SRC_DataSource.Text = "IESWKCT670";
            //            tb_SRC_DBName.Text = "SPEL11G";
            //            tb_SRC_UserName.Text = "system";
            //            tb_SRC_Password.Text = "SPEL11G";
            //            cb_SRC_TableName.Text = "tb_loop_2k";

            //            tb_TRG_DataSource.Text = "IESWKCT247";
            //            tb_TRG_DBName.Text = "db_SPELSQL";
            //            tb_TRG_UserName.Text = "sa";
            //            tb_TRG_Password.Text = "abcde@1234567";
            //            cb_TRG_TableName.Text = "tb_loop_2k";

            //            break;
            //        case 8:
            //            rb_SRC_oracle.Checked = true;
            //            tb_SRC_DataSource.Text = "localhost";
            //            tb_SRC_DBName.Text = "orcl";
            //            tb_SRC_UserName.Text = "system";
            //            tb_SRC_Password.Text = "1234";
            //            cb_SRC_TableName.Text = "test";

            //            rb_TRG_Oracle.Checked = true;
            //            tb_TRG_DataSource.Text = "localhost";
            //            tb_TRG_DBName.Text = "orcl";
            //            tb_TRG_UserName.Text = "system";
            //            tb_TRG_Password.Text = "1234";
            //            cb_TRG_TableName.Text = "test1";

            //            break;

            //        case 9:
            //            rb_SRC_oracle.Checked = true;
            //            tb_SRC_DataSource.Text = "IESWKCT670";
            //            tb_SRC_DBName.Text = "SPEL11G";
            //            tb_SRC_UserName.Text = "system";
            //            tb_SRC_Password.Text = "SPEL11G";
            //            cb_SRC_TableName.Text = "DASISLANdEL.T_MOTOR";

            //            rb_TRG_SQL.Checked = true;
            //            tb_TRG_DataSource.Text = "IESWKCT247";
            //            tb_TRG_DBName.Text = "JK1703_P";
            //            tb_TRG_UserName.Text = "sa";
            //            tb_TRG_Password.Text = "abcde@1234567";
            //            cb_TRG_TableName.Text = "Plant789el.T_Motor";

            //            break;
            //        case 10:
            //            rb_SRC_oracle.Checked = true;
            //            tb_SRC_DataSource.Text = "DSGPY3002.dcap.glpoly.net";
            //            tb_SRC_DBName.Text = "MSPIAS";
            //            tb_SRC_UserName.Text = "COV_LTTS_VIEW";
            //            tb_SRC_Password.Text = "SP14LTTS";
            //            cb_SRC_TableName.Text = "MDITRAIN.FLUID_PROPERTY";

            //            rb_TRG_SQL.Checked = true;
            //            tb_TRG_DataSource.Text = @"DFFMZ08SOR01\TOOLS_QAQC";
            //            tb_TRG_DBName.Text = "Q_SPI_MDITRAIN_2018";
            //            tb_TRG_UserName.Text = "CAOJING";
            //            tb_TRG_Password.Text = "CAOJING1";
            //            cb_TRG_TableName.Text = "CAOJING.FLUID_PROPERTY";

            //            break;
            //    }
            //}
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

                //tb_FileName.Text = string.Empty;

                toggleCondition(true);

                resetLabelsToDefaultValue();

                resetCheckListBox();

                rb_SRC_oracle.Select();

                btnGenerateOutput.Enabled = false;

                boolStatus = false;

            }
            catch (Exception ex)
            {
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
                llbl_File_path.Visible = false;
                lblExpressionStatus.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void resetCheckListBox()
        {
            try
            {
                clb_ReferenceID.Items.Clear();
                cbL_SRC_Cols.Items.Clear();
                lb_TRG_Cols.Items.Clear();
                cb_selectAll_toggle.CheckState = CheckState.Unchecked;
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
                str_unique_reference_key = string.Empty;
                str_unique_reference_key = string.Join(" AND ", clb_ReferenceID.CheckedItems.Cast<string>().AsEnumerable().Select(x => string.Concat(x, " = ", $"'{dr[x.ToString()]}'")));
            }
            catch (Exception ex)
            {
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

                saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Output File", $"{common_Data.RemoveSpecialCharacters(cb_SRC_TableName.Text)}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx");

                myException_BG = new Exception();

                if (cb_IncludeRule.Checked && common_Data.GetRulesStatus())
                {
                    dsRules = common_Data.GetDatasetRules();
                    boolRules = common_Data.GetRulesStatus();
                }

                if (!backgroundWorker1.IsBusy)
                {
                    #region new code

                    if (!(boolStatus && saveFileDialogFile.BoolFileSaveStatus))
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
                        //ds = Connect_SRC_DB(str_to_fetch);
                        dt_oracle = common_Data.RemoveColumnExcept(ds.Tables[0], str_to_fetch);
                    }

                    //Call SQL server DB
                    if (boolStatus)
                    {
                        //ds = Connect_TRG_DB(str_to_fetch);
                        dt_sql = common_Data.RemoveColumnExcept(ds.Tables[1], str_to_fetch);
                    }
                    else
                    {
                        myException_BG.Source = "There is some error";
                        toggleCondition(true);
                        lblExpressionStatus.Text = myException_BG.Message.ToString();
                        lblStatus.Text = myException_BG.Source.ToString();
                        return;
                    }

                    #endregion
                    if (boolStatus)
                    {
                        int_Count_Comparison = 0;
                        start_Time = DateTime.Now;
                        timer1.Enabled = true;
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
            }

            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = ex.Message.ToString();
                Logs.EnterLogsForError(ex);
            }
        }

        private void StartCompareWithPrimaryKey(DoWorkEventArgs e)
        {
            try
            {
                string strPk = string.Join(", ", clb_ReferenceID.CheckedItems.Cast<string>()
                          .Select(s => s.ToString())
                          .ToArray());

                string str_colName = string.Join(", ", cbL_SRC_Cols.CheckedItems.Cast<string>()
                          .Select(s => s.ToString())
                          .ToArray());

                str_colName = string.Concat(strPk, ", ", str_colName);

                //dt_src = common_Data.RemoveColumnExcept(dt_src, str_colName);
                //dt_trg = common_Data.RemoveColumnExcept(dt_trg, str_colName);

                dt_oracle = Common_Data.RemoveUncommonColumnInDataTable(dt_oracle, dt_sql);
                dt_sql = Common_Data.RemoveUncommonColumnInDataTable(dt_sql, dt_oracle);

                CompareTwoDataTableByPrimaryKey(dt_oracle, dt_sql, strPk, e);
            }
            catch (Exception ex)
            {
                //myException = ex;
                //boolStatus = false;
                //DisplayError(ex);
                throw ex;

            }
        }

        private string to_fetch(string str_to_fetch)
        {
            try
            {
                str_to_fetch = string.Empty;
                //foreach (var item in clb_ReferenceID.CheckedItems)
                //{
                //    str_to_fetch = str_to_fetch + item + " , ";
                //}
                //str_to_fetch = str_to_fetch + str_col_to_include_in_comparison;

                str_to_fetch = string.Join(", ", string.Join(", ", clb_ReferenceID.CheckedItems.Cast<string>().AsEnumerable()), str_col_to_include_in_comparison);
            }
            catch (Exception ex)
            {
                boolStatus = false;
                myException_BG = ex;
            }
            return str_to_fetch;
        }

        private List<ColorPair> colorPairs;
        private List<string> strPKList;
        private DataColumn[] dc_PK_SRC;
        public DataColumn[] dc_PK_TRG;
        private int int_object;

        private DataTable dtUnmatched;
        private DataTable dtSRCOnly;
        private DataTable dtSummary;
        private int int_Color_Row, totalProgress;
        private DataRow dataRows_TRG;
        private StringComparison stringComparisonTypeOrdincalCase;

        private bool boolANewRow, boolInclude;

        private ExcelWorksheet worksheet;
        private IEnumerable<ColorPair> query;

        //private bool boolANewRow;

        private void CompareTwoDataTableByPrimaryKey(DataTable dtSRC, DataTable dtTRG, string strPK, DoWorkEventArgs e)
        {
            try
            {
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

                #region Converting all null values columns into object type

                foreach(DataColumn dc in dtUnmatched.Columns)
                {
                    dc.DataType = typeof(object);
                }

                #endregion

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

                    //Logs.EnterDataInLogs(vs_pk[0].ToString());

                    if (dataRows_TRG != null)
                    {
                        boolANewRow = true;

                        foreach (DataColumn dc in dataRows_TRG.Table.Columns)
                        {
                            if (!(strPKList.Any(a => string.Equals(a, dc.ColumnName))))
                            {
                                str_Data_Source_To_Compare = dr[dc.ColumnName].ToString().Trim();
                                str_Data_Target_To_Compare = dataRows_TRG[dc.ColumnName].ToString().Trim();

                                //Logs.EnterDataInLogs($"Source Data {str_Data_Source_To_Compare} || Target Data {str_Data_Target_To_Compare}");

                                bool boolTest = common_Data.MismatchTest(str_Data_Source_To_Compare, str_Data_Target_To_Compare, cb_IgnoreCase.Checked, cb_IncludeRule.Checked, dc.ColumnName.ToString(), dc.ColumnName.ToString(), dr, dr, dtSRC.Columns, dtTRG.Columns);
                                #region

                                //if (!(string.Equals(str_Data_Source_To_Compare, str_Data_Target_To_Compare, stringComparisonTypeOrdincalCase)))
                                if (boolTest)
                                {
                                    #region Check rules 
                                    //if (boolRules)
                                    //{
                                    //    if (str_Data_Source_To_Compare.Length > 0 && str_Data_Target_To_Compare.Length > 0 && dc.ColumnName.ToString().Length > 0)
                                    //    {
                                    //        boolRulesCheck = common_Data.CheckRulesInFile(str_Data_Source_To_Compare, str_Data_Target_To_Compare, dc.ColumnName.ToString(), dc.ColumnName.ToString(), dr, dr, dtSRC.Columns, dtTRG.Columns);

                                    //        if (boolRulesCheck)
                                    //        {
                                    //            continue;
                                    //        }
                                    //    }
                                    //}

                                    #endregion

                                    dtSummary.Rows[0][dc.ColumnName] = Convert.ToInt32(dtSummary.Rows[0][dc.ColumnName]) + 1;

                                    #endregion

                                    if (boolANewRow)
                                    {
                                        dtUnmatched.Rows.Add(dr.ItemArray);
                                        dtUnmatched.Rows.Add(dataRows_TRG.ItemArray);
                                        boolANewRow = false;
                                        int_Color_Row += 2;
                                    }
                                    colorPairs.Add(new ColorPair(int_Color_Row, dc.ColumnName.ToString()));
                                    colorPairs.Add(new ColorPair(int_Color_Row - 1, dc.ColumnName.ToString()));
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
                backgroundWorker1.ReportProgress(105);
                //Common_Data.DisplayDebugMsg("Main Loop Comparison Done");

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
                            }
                            dr.Delete();
                        }
                    }
                }

                backgroundWorker1.ReportProgress(110);
                //DisplayDebugMsg("Transpose of summary and initialisation is done");

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

                backgroundWorker1.ReportProgress(115);

                using (ExcelPackage pck = new ExcelPackage())
                {
                    worksheet = pck.Workbook.Worksheets.Add("Mismatch");
                    worksheet.Cells["A1"].LoadFromDataTable(dtUnmatched, true);
                    worksheet.Cells.AutoFitColumns(22, 55);

                    //DisplayDebugMsg("First Workseet Written");

                    #region coloring

                    worksheet.Cells[1, 2, 1, strPKList.Count + 1].Style.Font.Color.SetColor(Color.Red);

                    foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                    {
                        query = colorPairs.Where(cp => cp.str_Col_Name.ToLower() == cell.Value.ToString().ToLower());

                        foreach (var i in query)
                        {
                            worksheet.Cells[i.int_Row, cell.Start.Column].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[i.int_Row, cell.Start.Column].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                            worksheet.Cells[i.int_Row, cell.Start.Column].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                        }
                    }

                    backgroundWorker1.ReportProgress(120);
                    //DisplayDebugMsg("Yellow coloring of first worksheet is done");

                    #endregion

                    dtSRCOnly.TableName = "Source Rows Only";

                    worksheet = common_Data.AddNewWorksheet(pck, worksheet, dtSRCOnly);
                    worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                    //DisplayDebugMsg("Second worksheet written");

                    worksheet = common_Data.AddNewWorksheet(pck, worksheet, dtTRG, "Target Rows Only");
                    worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                    backgroundWorker1.ReportProgress(125);

                    //DisplayDebugMsg("Third worksheet written");

                    worksheet = common_Data.AddNewWorksheet(pck, worksheet, dtSummary, "Summary");
                    worksheet.Cells[2, 1, strPKList.Count + 1, 1].Style.Font.Color.SetColor(Color.Red);

                    foreach (var cell in worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column])
                    {
                        cell.Value = Convert.ToInt32(cell.Value);
                    }

                    backgroundWorker1.ReportProgress(130);
                    //DisplayDebugMsg("Fourth and last worksheet written");

                    //strFileNameWithPath = $@"{strFolderPath}\{dtSRC.TableName}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                    pck.SaveAs(new System.IO.FileInfo(saveFileDialogFile.Str_saveFileNameWithPath));
                    strFilePathChangeSave = saveFileDialogFile.Str_saveFileNameWithPath;
                    boolStatus = true;
                    //DisplayMessage($"File Saved : {strFileNameWithPath}");

                }
                #endregion
            }

            catch (Exception ex)
            {
                myException_BG = ex;
                e.Cancel = true;
                boolStatus = false;
                //throw ex;
                //DisplayError(ex);
                e.Result = ex.Message.ToString();
                Logs.EnterLogsForError(ex);
                Logs.EnterLogsWithTime($"Error Logged in : {str_Data_Source_To_Compare} and {str_Data_Target_To_Compare}");
                Logs.EnterLogsWithTime($"Error Logged in : {progress}");
                
            }

        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                colorPairs = new List<ColorPair>();
                progress = 0;

                if (cb_PrimaryKey.Checked)
                {
                    StartCompareWithPrimaryKey(e);
                    return;
                }

                ds_change = new DataSet();
                dt_change = new DataTable();

                count_match = 0;
                count_unmatched = 0;

                dt_sql.CaseSensitive = true;

                dv_sql = new DataView(dt_sql);
                dv_oracle = new DataView(dt_oracle);

                dt_sql_only = dt_sql.Copy();

                dt_unmatched_Capture = new DataTable();
                dt_unmatched_Capture = dt_oracle.Clone();

                DataGridView dataGridView2 = new DataGridView();

                foreach (DataColumn dc in dt_unmatched_Capture.Columns)
                {
                    dataGridView2.Columns.Add(dc.ColumnName, dc.ColumnName);
                }

                dt_oracle_only = dt_oracle.Clone();
                dt_summarised_difference = dt_oracle.Clone();

                foreach (DataColumn dc in dt_oracle.Columns)
                {
                    dt_summarised_difference.Columns[dc.ToString()].DataType = typeof(Int32);
                }
                DataRow dr_summarised = dt_summarised_difference.NewRow();
                string str_colName;

                foreach (DataColumn dc in dt_oracle.Columns)
                {
                    dr_summarised[dc.ToString()] = 0;
                }

                dt_summarised_difference.Rows.Add(dr_summarised);

                ds_unmatched_Capture = new DataSet();
                int_Color_Row = 1;
                headRow = -1;
                alternateRow = 0;
                newRow = true;

                #region check for match and unmatch columns

                totalProgressCount = dt_oracle.Rows.Count;

                string str_col_used = string.Empty;

                CheckedListBox.CheckedItemCollection cic_oracleTB = cbL_SRC_Cols.CheckedItems;
                CheckedListBox.CheckedItemCollection cic_reference = clb_ReferenceID.CheckedItems;

                #region main comparison part

                foreach (DataRow dr in dt_oracle.Rows)
                {

                    str_unique_reference_key = generateRowFilterString(dr);

                    //dv_sql.RowFilter = str_unique_reference_key + " = '" + dr[str_unique_reference_key] + "'";

                    dv_sql.RowFilter = str_unique_reference_key;
                    //Logs.EnterDataInLogs($"Unique reference key as : {str_unique_reference_key}");
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

                        foreach (var dc in cic_oracleTB)
                        {
                            int_Count_Comparison++;

                            if (!(cic_reference.Contains(dc.ToString())))
                            {
                                str_Data_Source_To_Compare = dr[dc.ToString()].ToString().Trim();
                                str_Data_Target_To_Compare = dv_sql[0][dc.ToString()].ToString().Trim();

                                bool boolTest = common_Data.MismatchTest(str_Data_Source_To_Compare, str_Data_Target_To_Compare, cb_IgnoreCase.Checked, cb_IncludeRule.Checked, dc.ToString(), dc.ToString(), dr, dr, dt_oracle.Columns, dt_sql.Columns);

                                #region old code

                                //boolCompareResult = false;

                                //str_Data_Source_To_Compare = str_Data_Source_To_Compare.Trim();
                                //str_Data_Target_To_Compare = str_Data_Target_To_Compare.Trim();

                                //#region Ignore case

                                //if (cb_IgnoreCase.Checked & (string.Equals(str_Data_Source_To_Compare, str_Data_Target_To_Compare, StringComparison.OrdinalIgnoreCase)))
                                //{
                                //    boolCompareResult = true;
                                //}
                                //else if (string.Equals(str_Data_Source_To_Compare, str_Data_Target_To_Compare, StringComparison.Ordinal))
                                //{
                                //    boolCompareResult = true;
                                //}

                                //#endregion

                                //#region Check rules 
                                ////boolRulesCheck = true;
                                //if (cb_IncludeRule.Checked && boolRules && (!boolCompareResult))
                                //{
                                //    if (str_Data_Source_To_Compare.Length > 0 && str_Data_Target_To_Compare.Length > 0 && dsRules.Tables[0].Rows.Count > 0 && dc.ToString().Length > 0 && dt_oracle.Rows.Count > 0 && dt_sql.Rows.Count > 0)
                                //    {
                                //        boolRulesCheck = common_Data.CheckRulesInFile(str_Data_Source_To_Compare, str_Data_Target_To_Compare, dc.ToString(), dc.ToString(), dsAllColumns.Tables[0].Select(str_unique_reference_key)[0], dsAllColumns.Tables[1].Select(str_unique_reference_key)[0], dsAllColumns.Tables[0].Columns, dsAllColumns.Tables[1].Columns);
                                //    }
                                //    else
                                //    {
                                //        boolRulesCheck = true;
                                //    }
                                //}
                                //else
                                //{
                                //    boolRulesCheck = true;
                                //}

                                //#endregion

                                ////if (!(dr[dc.ToString()].ToString() == dv_sql[0][dc.ToString()].ToString())) // previous code
                                ////if (!(boolCompareResult))

                                #endregion

                                //if ((!(boolCompareResult)) && (boolRulesCheck))
                                if (boolTest)
                                {
                                    #region old code
                                    //Console.WriteLine((dr[dc.ToString()].ToString()).GetType());
                                    //Console.WriteLine((dr[dc.ToString()].ToString()));

                                    #region exception handling of float values start here

                                    //boolOracle = float.TryParse((dr[dc.ToString()].ToString()), out float float_oracle);
                                    //boolSQL = float.TryParse((dv_sql[0][dc.ToString()].ToString()), out float float_sql);

                                    //if (boolOracle & boolSQL)
                                    //{
                                    //    if (float_oracle == float_sql)
                                    //    {
                                    //        continue;
                                    //    }
                                    //}
                                    #endregion exception hadling of float values end here

                                    cellCol = dt_oracle.Columns[dc.ToString()].Ordinal;
                                    str_colName = dc.ToString();
                                    if (newRow)
                                    {
                                        dt_unmatched_Capture.Rows.Add(dr.ItemArray);
                                        dt_unmatched_Capture.Rows.Add(dv_sql[0].Row.ItemArray);
                                        headRow += 2;
                                        alternateRow += 2;
                                        newRow = false;

                                        dataGridView2.Rows.Add(dt_unmatched_Capture.Rows[0].ItemArray);
                                        dataGridView2.Rows.Add(dt_unmatched_Capture.Rows[1].ItemArray);
                                        int_Color_Row += 2;
                                    }

                                    dataGridView2.Rows[headRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;
                                    dataGridView2.Rows[alternateRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;

                                    colorPairs.Add(new ColorPair(int_Color_Row, str_colName));
                                    colorPairs.Add(new ColorPair(int_Color_Row - 1, str_colName));

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
                #endregion

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
                            dataGridView2.Columns.Remove(dc.ColumnName.ToString());
                            adjust++;
                        }
                    }
                }

                #endregion

                generateTransposeTable();

                if (Properties.Settings.Default.EPPlus_Excel_Save)
                {
                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        worksheet = pck.Workbook.Worksheets.Add("Mismatch");
                        worksheet.Cells["A1"].LoadFromDataTable(dt_unmatched_Capture, true);
                        worksheet.Cells.AutoFitColumns(22, 55);

                        progress = 0;
                        totalProgress = worksheet.Dimension.End.Column;
                        foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                        {
                            percentage = (((++progress) * 100) / totalProgress);
                            backgroundWorker1.ReportProgress(percentage);

                            query = colorPairs.Where(cp => cp.str_Col_Name.ToLower() == cell.Value.ToString().ToLower());

                            foreach (var i in query)
                            {
                                worksheet.Cells[i.int_Row, cell.Start.Column].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                worksheet.Cells[i.int_Row, cell.Start.Column].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                worksheet.Cells[i.int_Row, cell.Start.Column].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            }
                        }

                        backgroundWorker1.ReportProgress(102);

                        #endregion

                        //dt_oracle_only.TableName = "Source Rows Only";

                        worksheet = common_Data.AddNewWorksheet(pck, worksheet, dt_oracle_only, "Source Rows Only");
                        //worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                        //Common_Data.DisplayDebugMsg(rtb_Log,"Second worksheet written");
                        backgroundWorker1.ReportProgress(101);

                        worksheet = common_Data.AddNewWorksheet(pck, worksheet, dt_sql_only, "Target Rows Only");
                        //worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                        backgroundWorker1.ReportProgress(102);

                        worksheet = common_Data.AddNewWorksheet(pck, worksheet, dt_transpose, "Summary");
                        //worksheet.Cells[2, 1, strPKList.Count + 1, 1].Style.Font.Color.SetColor(Color.Red);

                        foreach (var cell in worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column])
                        {
                            cell.Value = Convert.ToInt32(cell.Value);
                        }

                        backgroundWorker1.ReportProgress(103);

                        pck.SaveAs(new System.IO.FileInfo(saveFileDialogFile.Str_saveFileNameWithPath));
                    }
                }
                else if (Properties.Settings.Default.XL_Excel_Save)
                {
                    wb = new XLWorkbook();
                    wb.Worksheets.Add(dt_unmatched_Capture, "Mismatch");
                    wb.Worksheets.Add(dt_oracle_only, "Source rows only");
                    wb.Worksheets.Add(dt_sql_only, "Target rows only");
                    wb.Worksheets.Add(dt_transpose, "Summary");

                    backgroundWorker1.ReportProgress(105);

                    formatExcelTableByDataGridView(1, dataGridView2);

                    backgroundWorker1.ReportProgress(110);

                    formatExcelTable(2, dt_oracle_only);
                    formatExcelTable(3, dt_sql_only);
                    formatExcelTable(4, dt_transpose);

                    backgroundWorker1.ReportProgress(115);

                    strFilePathChangeSave = saveFileDialogFile.Str_saveFileNameWithPath;

                    wb.SaveAs(strFilePathChangeSave);

                    backgroundWorker1.ReportProgress(125);

                    wb.Save();
                }

                strFilePathChangeSave = saveFileDialogFile.Str_saveFileNameWithPath;
                boolStatus = true;

                backgroundWorker1.ReportProgress(130);
                stop_Time = DateTime.Now;
            }

            catch (Exception ex)
            {
                e.Result = ex.Message.ToString();
                myException_BG = ex;
                boolStatus = false;
                Logs.EnterLogsForError(ex);
                Logs.EnterLogsWithTime(str_unique_reference_key);
            }
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
                //int percentageEval;
                if (e.ProgressPercentage <= 100)
                {
                    lbl_PercentageShow.Text = e.ProgressPercentage + " % ";
                    progressBar1.Value = e.ProgressPercentage;

                    //displayElapsedTime();

                    lblStatus.Text = "Total matched :" + count_match + " and total unmatched : " + count_unmatched;
                }
                else if (e.ProgressPercentage == 105)
                {
                    lblStatus.Text = "All excel files are written and saved";
                    //displayElapsedTime();
                }
                else if (e.ProgressPercentage == 110)
                {
                    lblStatus.Text = "Unmatched formatting done";
                    //displayElapsedTime();
                }
                else if (e.ProgressPercentage == 115)
                {
                    lblStatus.Text = "Formatting done for other excel sheets";
                    //displayElapsedTime();
                }
                else if (e.ProgressPercentage == 120)
                {
                    lblStatus.Text = "Deleting of unwanted columns started";
                    //displayElapsedTime();
                }
                else if (e.ProgressPercentage == 125)
                {
                    lblStatus.Text = "Unwanted columns deleted";
                    //displayElapsedTime();
                }
                else if (e.ProgressPercentage == 125)
                {
                    lblStatus.Text = "Excel File Saved";
                    //displayElapsedTime();
                }
                //else if(e.ProgressPercentage >100 & e.ProgressPercentage <=200)
                //{
                //    percentageEval = e.ProgressPercentage - 100;

                //    lbl_PercentageShow.Text = percentageEval + " % ";
                //    progressBar1.Value = percentageEval;
                //    time_to_execute = DateTime.Now.Subtract(start_Time);
                //    lblTimer.Text = time_to_execute.Minutes.ToString() + " mins " + time_to_execute.Seconds.ToString() + " seconds";

                //    if(percentageEval < 9)
                //    {
                //        lblStatus.Text = "Formatting table";
                //    }
                //}
                else
                {
                    lblStatus.Text = "Excel file saved and all procedures completed successfully";
                    //displayElapsedTime();
                    lbl_PercentageShow.Text = "100 %";
                }

            }
            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = myException_BG.Message.ToString();
                Logs.EnterLogsForError(ex);
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
                if (e.Cancelled & e.Error != null)
                {
                    lblStatus.Text = "Process Cancelled";
                }
                else if (e.Error != null)
                {
                    lblExpressionStatus.Text = e.Error.Message.ToString();
                    lblStatus.Text = e.Error.Message.ToString();
                }
                else if (strFilePathChangeSave != "" && boolStatus)
                {
                    lblStatus.Text = "Done and Opening file in path " + strFilePathChangeSave;
                    lbl_PercentageShow.Text = lbl_PercentageShow.Text + " Done ";
                    //displayElapsedTime();
                    llbl_File_path.Text = strFilePathChangeSave;
                    llbl_File_path.Visible = true;
                    Process.Start(strFilePathChangeSave);
                }
                else if (myException_BG.Source != null)
                {
                    lblStatus.Text = $"{myException_BG.Source} : {myException_BG.Message}";
                    lblExpressionStatus.Text = myException_BG.Message.ToString();
                    lblExpressionStatus.ForeColor = Color.Red;
                }
                else
                {
                    lblExpressionStatus.Text = "Some other error ";
                    lblStatus.Text = "Error : " + myException_BG.Message.ToString();
                }

            }
            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
                Logs.EnterLogsForError(ex);
            }
            finally
            {
                toggleCondition(true);
                timer1.Enabled = false;
                if (myException_BG.InnerException != null)
                {
                    lblStatus.Text = myException_BG.Message.ToString();
                    lblExpressionStatus.Text = myException_BG.Message.ToString();
                }
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                DisplayError(ex);
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
                DisplayError(ex);
            }
        }

        private void rb_Target_Oracle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_TRG_Oracle.Checked)
                {
                    CheckChangeRadioButton(2);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
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

        private void cb_IncludeRule_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cb_IgnoreCase_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //test();
        }

        private void tb_Default_Port_TextChanged(object sender, EventArgs e)
        {

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

                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_SRC_DataSource.Text + " )(PORT = " + tb_SRC_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_SRC_DBName.Text + ")));"
                        + "User Id=" + tb_SRC_UserName.Text + ";Password=" + tb_SRC_Password.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_oracle = new DataTable();
                    oracleDA.Fill(dt_oracle);
                    dt_oracle.TableName = "Load Table Name";
                    FillCheckListBox(cb_SRC_TableName, dt_oracle, "oracle");
                    dt_LoadSRC = dt_oracle.Copy();
                }

                if (rb_SRC_SQL.Checked)
                {
                    strQuery = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";

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

                    dt_sql = new DataTable();
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_sql);

                    strSourceName = "Source SQL";
                    FillCheckListBox(cb_SRC_TableName, dt_sql, "sql");
                    dt_LoadSRC = dt_sql.Copy();
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

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void btn_LoadProfile_Click(object sender, EventArgs e)
        {
            try
            {
                //JObject jObject2;
                string str_UserProfile_Json_File = $@"{Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName)}\UserProfile\userProfile.json";

                OpenFileDialogData_OBJ = OpenFileDialogData_OBJ.openFileDialog("Browser Json File", "json");

                if (OpenFileDialogData_OBJ.boolFileSelected)
                {
                    str_UserProfile_Json_File = OpenFileDialogData_OBJ.strFileNameWithPath;
                    using (var reader = new StreamReader(str_UserProfile_Json_File))
                    {
                        using (var jsonReader = new JsonTextReader(reader))
                        {
                            var root = JToken.Load(jsonReader);
                            DisplayTreeView(root, Path.GetFileNameWithoutExtension(str_UserProfile_Json_File));
                        }
                    }
                }
                else
                {
                    DisplayMessage(lbl_ProfileStatus, "File Not Selected");
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void DisplayTreeView(JToken root, string rootName)
        {
            treeView1.BeginUpdate();
            try
            {
                treeView1.Nodes.Clear();
                var tNode = treeView1.Nodes[treeView1.Nodes.Add(new TreeNode(rootName))];
                tNode.Tag = root;

                AddNode(root, tNode);

                if (true)
                {
                    treeView1.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                treeView1.EndUpdate();
            }
        }

        private void AddNode(JToken token, TreeNode inTreeNode)
        {
            try
            {
                if (token == null)
                    return;
                if (token is JValue)
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString()))];
                    childNode.Tag = token;
                }
                else if (token is JObject)
                {
                    var obj = (JObject)token;
                    foreach (var property in obj.Properties())
                    {
                        var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                        childNode.Tag = property;
                        AddNode(property.Value, childNode);
                    }
                }
                else if (token is JArray)
                {
                    var array = (JArray)token;
                    for (int i = 0; i < array.Count; i++)
                    {
                        var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                        childNode.Tag = array[i];
                        AddNode(array[i], childNode);
                    }
                }
                else
                {
                    Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
                    if (Debugger.IsAttached)
                    {
                        DisplayMessage(lbl_ProfileStatus, $"{token.Type} not implemented", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_TRG_DataSource.Text + " )(PORT = " + tb_TRG_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_TRG_DBName.Text + ")));"
                        + "User Id=" + tb_TRG_UserName.Text + ";Password=" + tb_TRG_Password.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_oracle = new DataTable();
                    oracleDA.Fill(dt_oracle);
                    dt_oracle.TableName = "Load Table Name";
                    FillCheckListBox(cb_TRG_TableName, dt_oracle, "oracle");
                }

                if (rb_TRG_SQL.Checked)
                {
                    strQuery = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";

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

                    dt_sql = new DataTable();
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_sql);
                    dt_sql.TableName = "Load Table Name";
                    FillCheckListBox(cb_TRG_TableName, dt_sql, "sql");
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
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void llbl_Folder_path_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip.SetToolTip(llbl_File_path, llbl_File_path.Text);
                toolTip.Show(llbl_File_path.Text, this, 30000);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_Source_tableName_KeyUp(object sender, KeyEventArgs e)
        {
            //DataRow[] dataRows;
            //DataTable dt_filteredTable;
            //dt_filteredTable = new DataTable();
            //try
            //{
            //    string str_name_LIKE = $"{cb_Source_tableName.Text}";
            //    if (dt_LoadSRC.Rows.Count > 0)
            //    {
            //        //dataRows = dt_LoadSRC.Select($"TABLE_CATALOG LIKE '%{str_name_LIKE}%' OR TABLE_NAME LIKE '%{str_name_LIKE}%'");
            //        dataRows = dt_LoadSRC.Select($"TABLE_NAME LIKE '%{str_name_LIKE}%'");
            //        if(dataRows.Length == 0)
            //        {
            //            return;
            //        }
            //        dt_filteredTable = dataRows.CopyToDataTable();

            //        List<string> lst_str_fill = new List<string>();

            //        foreach (DataRow dr in dt_filteredTable.Rows)
            //        {
            //            str_tableName = $"{dr["TABLE_NAME"].ToString()}";
            //            lst_str_fill.Add(str_tableName);
            //        }

            //        AutoCompleteStringCollection autoCompleteStringCollection = new AutoCompleteStringCollection();
            //        autoCompleteStringCollection.AddRange(lst_str_fill.ToArray());

            //        cb_Source_tableName.AutoCompleteCustomSource = autoCompleteStringCollection;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    DisplayError(ex);
            //}
        }

        private void cb_Source_tableName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cb_Source_tableName_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void DisplayError(Exception ex = null)
        {
            try
            {
                if (ex != null)
                {
                    lblExpressionStatus.Text = ex.Message.ToString();
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
                lblStatus.Text = ex1.Message.ToString();
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
            try
            {
                if (Debugger.IsAttached)
                {
                    Random random = new Random(500);
                    tb_ProfileName.Text = random.Next(100, 10000).ToString();
                }

                List<JsonData> _Jsondata = new List<JsonData>();
                //string str_ProfileName = tb_ProfileName.Text;

                dynamic dynamic_obj;
                dynamic_obj = tb_ProfileName.Text;

                _Jsondata.Add(new JsonData()
                {
                    str_ProfileName1 = dynamic_obj,

                    source = new DB
                    {
                        str_dbType = "a1",
                        str_dbSource = "b1",
                        str_dbName = "c1",
                        str_dbUsername = "d1",
                        str_dbTablename = "e1"
                    },
                    target = new DB
                    {
                        str_dbType = "a2",
                        str_dbSource = "b2",
                        str_dbName = "c2",
                        str_dbUsername = "d2",
                        str_dbTablename = "e2"
                    }
                });

                using (StreamWriter file = File.CreateText(@"D:\path.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serialize object directly into file stream
                    serializer.Serialize(file, _Jsondata);
                }
            }
            catch (Exception ex)
            {
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
                DisplayError(ex);
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
                DisplayError(ex);
            }
        }

        private void llbl_File_path_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (boolStatus)
                {
                    Process.Start(llbl_File_path.Text);
                }
                else
                {
                    DisplayMessage(lblStatus, "No File Generated");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                displayElapsedTime();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        
        private void llbl_File_path_Click(object sender, EventArgs e)
        {
            try
            {
                //llbl_Folder_path.Text = ofdd.folderBrowser(llbl_Folder_path.Text);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_WindowAuthentication_SRC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_WindowAuthentication_SRC.Checked)
                {
                    tb_SRC_UserName.Enabled = false;
                    tb_SRC_Password.Enabled = false;
                }
                else if (cb_WindowAuthentication_SRC.CheckState == CheckState.Unchecked)
                {
                    tb_SRC_UserName.Enabled = true;
                    tb_SRC_Password.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void DisplayError(Control control, Exception ex = null)
        {
            try
            {
                if (ex != null)
                {
                    control.Text = ex.Message.ToString();
                    if (Debugger.IsAttached)
                    {
                        control.Text = ex.ToString();
                    }
                }
                else
                {
                    control.Text = string.Empty;
                }
                control.Update();
            }
            catch (Exception ex1)
            {
                control.Text = ex1.Message.ToString();
            }
        }

        private void rb_Target_SQL_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_TRG_SQL.Checked)
                {
                    CheckChangeRadioButton(2);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_Target_DefaultPort_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_TRG_DefaultPort.Checked)
                {
                    tb_TRG_DefaultPort.Text = "1521";
                    tb_TRG_DefaultPort.Enabled = true;
                    tb_TRG_DefaultPort.ReadOnly = true;
                }
                else
                {
                    tb_TRG_DefaultPort.ResetText();
                    tb_TRG_DefaultPort.Enabled = true;
                    tb_TRG_DefaultPort.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_Default_Port_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_SRC_DefaultPort.Checked)
                {
                    tb_SRC_DefaultPort.Text = "1521";
                    tb_SRC_DefaultPort.Enabled = true;
                    tb_SRC_DefaultPort.ReadOnly = true;
                }
                else
                {
                    tb_SRC_DefaultPort.ResetText();
                    tb_SRC_DefaultPort.Enabled = true;
                    tb_SRC_DefaultPort.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
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
            }
            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
            }
        }

        private void statusChangeforSetButton(bool enabledStatus)
        {
            cb_selectAll_toggle.Enabled = enabledStatus;

            if (enabledStatus)
            {
                cbL_SRC_Cols.SelectionMode = SelectionMode.One;
                lb_TRG_Cols.SelectionMode = SelectionMode.One;
                clb_ReferenceID.SelectionMode = SelectionMode.One;
            }
            else
            {
                cbL_SRC_Cols.SelectionMode = SelectionMode.None;
                lb_TRG_Cols.SelectionMode = SelectionMode.None;
                clb_ReferenceID.SelectionMode = SelectionMode.None;
            }

        }
    }
}
