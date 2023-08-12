using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using ClosedXML.Excel;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using form = System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace SPI_Data_Comp_Tool
{
    public partial class MultiReferenceDBComparison : Form
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

        private string str_Data_Source_To_Compare, str_Data_Target_To_Compare;


        #endregion

        #region Dataset and Datatable and Dataview

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
        private DataSet dsRules;

        private Common_Data common_Data;


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

        private bool boolCellValueIntType;
        private bool boolOracle, boolSQL, boolRules, boolCompareResult, boolRulesCheck;

        int headRow, alternateRow, cellCol;

        #endregion

        #region Excel related variables

        //private XLWorkbook xlworkBook;

        private _Application excel = new _Excel.Application();

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
        private CheckRules checkRules;
        #endregion

        public MultiReferenceDBComparison()
        {
            InitializeComponent();
            //llbl_Folder_path.Text = @"D:\";

            lblExpressionStatus.Text = string.Empty;
            cd = new Common_Data();
            ofdd = new OpenFileDialogData();

            myException_BG = new Exception();

            common_Data = new Common_Data();
            boolRules = false;
            dsRules = new DataSet();

            str_Data_Source_To_Compare = string.Empty;
            str_Data_Target_To_Compare = string.Empty;

            //int_total_rows = 0;
            //int_total_columns = 0;

            boolRules = false;
            boolCompareResult = false;
            boolRulesCheck = false;

            if (Debugger.IsAttached || Environment.UserName == "40009708")
            {
                comboBox1.Visible = true;
                comboBox1.SelectedIndex = 8;
            }
        }

        private void DebugDataSet(int intExplicitCall)
        {
            try
            {
                if ((Debugger.IsAttached) || (intExplicitCall == 1) || (Environment.UserName == ""))
                {
                    comboBox1.Visible = true;
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

        private DataSet connect_Oracle_DB(string columnNames = " * ")
        {
            try
            {
                dt_oracle = new DataTable();
                ds = new DataSet();
                strQuery = "select " + columnNames + " from " + tb_Source_tableName.Text;

                // oracle 11g and sql selection as requirement changed

                if (rb_oracle.Checked)
                {
                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_oracle_dsource.Text + " )(PORT = " + tb_Default_Port.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_oracle_dbName.Text + ")));"
                        + "User Id=" + tb_oracle_username.Text + ";Password=" + tb_oracle_pwd.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    oracleDA.Fill(dt_oracle);
                    strSourceName = "Source Oracle 11G";
                }

                if (rb_sql.Checked)
                {
                    strSQLConnection = "Data Source = " + tb_oracle_dsource.Text + ";Initial Catalog = " + tb_oracle_dbName.Text
                    + "; User ID = " + tb_oracle_username.Text + "; " + "Password = " + tb_oracle_pwd.Text + ";";// providerName=System.Data.SqlClient";

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
            }

            return ds;
        }

        private DataSet connect_SQL_DB(string columnNames = " * ")
        {
            try
            {
                strQuery = "select " + columnNames + " from " + tb_destination_tableName.Text;

                if (rb_Target_Oracle.Checked)
                {
                    strSQLConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_sql_dsource.Text + " )(PORT = " + tb_Target_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_sql_dbName.Text + ")));"
                        + "User Id=" + tb_sql_username.Text + ";Password=" + tb_sql_pwd.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_sql = new DataTable();
                    oracleDA.Fill(dt_sql);
                    strTargetName = "Target Oracle 11G";
                    ds.Tables.Add(dt_sql);
                }
                else if (rb_Target_SQL.Checked)
                {
                    if (cb_windowsAuthentication.CheckState == CheckState.Checked)
                    {
                        strSQLConnection = "Data Source = " + tb_sql_dsource.Text + ";Initial Catalog = " + tb_sql_dbName.Text
                        + "; Integrated Security = true ";
                    }
                    else if (cb_windowsAuthentication.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = "Data Source = " + tb_sql_dsource.Text + ";Initial Catalog = " + tb_sql_dbName.Text
                        + "; User ID = " + tb_sql_username.Text + "; " + "Password = " + tb_sql_pwd.Text + ";";
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
                        wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Value = "SQL";
                    }
                    else
                    {
                        wb.Worksheet(int_sheet_no).Cell(row + 1, 1).Value = "Oracle";
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

        private void btn_Connect_Reference_ID_Click(object sender, EventArgs e)
        {
            try
            {
                resetLabelsToDefaultValue();
                boolStatus = true;
                clearCheckBoxList(clb_ReferenceID);
                clearCheckBoxList(cbL_columns, cb_selectAll_toggle);
                toggleCondition(false);

                ds = connect_Oracle_DB();

                if (boolStatus)
                {
                    foreach (DataColumn dc in ds.Tables[0].Columns)
                    {
                        clb_ReferenceID.Items.Add(dc.ToString());
                        cbL_columns.Items.Add(dc.ToString());
                    }
                }

                // conncet with SQL Target

                ds = connect_SQL_DB();

                //statusChangeOfSomeItems(true);
                cbL_TargetColumns.Items.Clear();

                if (boolStatus)
                {
                    foreach (DataColumn dc in ds.Tables[1].Columns)
                    {
                        cbL_TargetColumns.Items.Add(dc.ToString());
                    }
                    lblExpressionStatus.Text = "Data Loaded Successfully";
                }
                else
                {
                    lblExpressionStatus.Text = "Some Error in SQL or Oracle";
                }
                cb_selectAll_toggle.Checked = true;

                // New Code
                if (boolStatus)
                {
                    ViewForMultiReference viewForMultiReference = new ViewForMultiReference(ds, ds);
                    viewForMultiReference.Show();
                }
            }
            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
                boolStatus = false;
            }
            finally
            {
                toggleCondition(true);
                //btnGenerateOutput.Enabled = false;

            }
        }

        private void toggleCondition(bool enabledStatus)
        {
            tb_oracle_dbName.Enabled = enabledStatus;
            tb_oracle_dsource.Enabled = enabledStatus;
            tb_oracle_username.Enabled = enabledStatus;
            tb_oracle_pwd.Enabled = enabledStatus;
            tb_Source_tableName.Enabled = enabledStatus;

            tb_sql_dbName.Enabled = enabledStatus;
            tb_sql_dsource.Enabled = enabledStatus;
            tb_sql_username.Enabled = enabledStatus;
            tb_sql_pwd.Enabled = enabledStatus;
            tb_destination_tableName.Enabled = enabledStatus;

            btn_Connect_Reference_ID.Enabled = enabledStatus;
            //btn_Set.Enabled = enabledStatus;
            //btnGenerateOutput.Enabled = enabledStatus;
            btn_Connect_Reference_ID.Enabled = enabledStatus;

            //tb_FileName.Enabled = enabledStatus;

            cb_selectAll_toggle.Enabled = enabledStatus;
            btn_Clear_Reset.Enabled = enabledStatus;
            comboBox1.Enabled = enabledStatus;

            //llbl_Folder_path.Enabled = enabledStatus;

            groupBox8.Enabled = enabledStatus;

            cb_Default_Port.Enabled = enabledStatus;

            if (enabledStatus & (!(cb_Default_Port.Checked)))
            {
                tb_Default_Port.Enabled = enabledStatus;
            }
            else if (cb_Default_Port.Checked)
            {
                tb_Default_Port.Enabled = !(enabledStatus);
            }
            else if (!(enabledStatus))
            {
                tb_Default_Port.Enabled = enabledStatus;
            }

            if (enabledStatus)
            {
                cbL_columns.SelectionMode = SelectionMode.One;
                cbL_TargetColumns.SelectionMode = SelectionMode.One;
                clb_ReferenceID.SelectionMode = SelectionMode.One;
            }
            else
            {
                cbL_columns.SelectionMode = SelectionMode.None;
                cbL_TargetColumns.SelectionMode = SelectionMode.None;
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

        private void rb_oracle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                groupBox1.Text = "Source: Oracle Connection and Settings";
            }
            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = ex.Message.ToString();
            }
            finally
            {

            }
        }

        private void rb_sql_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //DebugDataSet(0);
                groupBox1.Text = "Source: SQL Connection and Settings";
            }
            catch (Exception ex)
            {
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
                            rb_oracle.Checked = true;
                            tb_oracle_dsource.Text = "IESWKCT670";
                            tb_oracle_dbName.Text = "SPEL11G";
                            tb_oracle_username.Text = "system";
                            tb_oracle_pwd.Text = "SPEL11G";
                            tb_Source_tableName.Text = "tb_compare1";

                            rb_Target_SQL.Checked = true;
                            tb_sql_dsource.Text = "IESWKCT600";
                            tb_sql_dbName.Text = "db_SPELSQL";
                            tb_sql_username.Text = "sa";
                            tb_sql_pwd.Text = "abcde@1234567";
                            tb_destination_tableName.Text = "tb_compare1";

                            break;

                        case 2:
                            rb_sql.Checked = true;
                            tb_oracle_dsource.Text = "IESWKCT600";
                            tb_oracle_dbName.Text = "db_SPELSQL";
                            tb_oracle_username.Text = "sa";
                            tb_oracle_pwd.Text = "abcde@1234567";
                            tb_Source_tableName.Text = "tb_compare1";

                            rb_Target_SQL.Checked = true;
                            tb_sql_dsource.Text = "IESWKCT600";
                            tb_sql_dbName.Text = "db_SPELSQL";
                            tb_sql_username.Text = "sa";
                            tb_sql_pwd.Text = "abcde@1234567";
                            tb_destination_tableName.Text = "tb_compare1";

                            break;

                        case 3:
                            rb_sql.Checked = true;
                            tb_oracle_dsource.Text = "IESWKCT600";
                            tb_oracle_dbName.Text = "SPELOLDP";
                            tb_oracle_username.Text = "sa";
                            tb_oracle_pwd.Text = "abcde@1234567";
                            tb_Source_tableName.Text = "plant1el.t_plantitem";

                            rb_Target_SQL.Checked = true;
                            tb_sql_dsource.Text = "IESWKCT600";
                            tb_sql_dbName.Text = "SPELOLDP_new";
                            tb_sql_username.Text = "sa";
                            tb_sql_pwd.Text = "abcde@1234567";
                            tb_destination_tableName.Text = "plant1el.t_plantitem";

                            break;

                        case 4:
                            rb_oracle.Checked = true;
                            tb_oracle_dsource.Text = "IESWKCT670";
                            tb_oracle_dbName.Text = "SPEL11G";
                            tb_oracle_username.Text = "system";
                            tb_oracle_pwd.Text = "SPEL11G";
                            tb_Source_tableName.Text = "DASISLANDEL.T_motor";

                            rb_Target_SQL.Checked = true;
                            tb_sql_dsource.Text = "IESWKCT600";
                            tb_sql_dbName.Text = "JK1703_P";
                            tb_sql_username.Text = "sa";
                            tb_sql_pwd.Text = "abcde@1234567";
                            tb_destination_tableName.Text = "Plant789el.T_Motor";

                            break;

                        case 5:
                            rb_oracle.Checked = true;
                            tb_oracle_dsource.Text = "IESWKCT670";
                            tb_oracle_dbName.Text = "SPEL11G";
                            tb_oracle_username.Text = "system";
                            tb_oracle_pwd.Text = "SPEL11G";
                            tb_Source_tableName.Text = "DASISLANDEL.T_Plantitem";

                            rb_Target_SQL.Checked = true;
                            tb_sql_dsource.Text = "IESWKCT600";
                            tb_sql_dbName.Text = "JK1703_P";
                            tb_sql_username.Text = "sa";
                            tb_sql_pwd.Text = "abcde@1234567";
                            tb_destination_tableName.Text = "plant789elp1.t_Plantitem";

                            break;

                        case 6:
                            rb_oracle.Checked = true;
                            tb_oracle_dsource.Text = "IESWKCT670";
                            tb_oracle_dbName.Text = "SPEL11G";
                            tb_oracle_username.Text = "system";
                            tb_oracle_pwd.Text = "SPEL11G";
                            tb_Source_tableName.Text = "tb_loop";

                            rb_Target_SQL.Checked = true;
                            tb_sql_dsource.Text = "IESWKCT600";
                            tb_sql_dbName.Text = "db_SPELSQL";
                            tb_sql_username.Text = "sa";
                            tb_sql_pwd.Text = "abcde@1234567";
                            tb_destination_tableName.Text = "loop";

                            break;
                        case 7:
                            rb_oracle.Checked = true;
                            tb_oracle_dsource.Text = "IESWKCT670";
                            tb_oracle_dbName.Text = "SPEL11G";
                            tb_oracle_username.Text = "system";
                            tb_oracle_pwd.Text = "SPEL11G";
                            tb_Source_tableName.Text = "tb_loop_2k";

                            rb_Target_SQL.Checked = true;
                            tb_sql_dsource.Text = "IESWKCT600";
                            tb_sql_dbName.Text = "db_SPELSQL";
                            tb_sql_username.Text = "sa";
                            tb_sql_pwd.Text = "abcde@1234567";
                            tb_destination_tableName.Text = "tb_loop_2k";

                            break;
                        case 8:
                            rb_oracle.Checked = true;
                            tb_oracle_dsource.Text = "localhost";
                            tb_oracle_dbName.Text = "orcl";
                            tb_oracle_username.Text = "system";
                            tb_oracle_pwd.Text = "1234";
                            tb_Source_tableName.Text = "test";

                            rb_Target_Oracle.Checked = true;
                            tb_sql_dsource.Text = "localhost";
                            tb_sql_dbName.Text = "orcl";
                            tb_sql_username.Text = "system";
                            tb_sql_pwd.Text = "1234";
                            tb_destination_tableName.Text = "test1";

                            break;
                        case 9:
                            rb_sql.Checked = true;
                            tb_oracle_dsource.Text = @"KRISHNA-HP\KRISHNASQL2019";
                            tb_oracle_dbName.Text = "db_krishna";
                            tb_oracle_username.Text = "sa";
                            tb_oracle_pwd.Text = "1234";
                            tb_Source_tableName.Text = "multitable1";

                            rb_Target_SQL.Checked = true;
                            tb_sql_dsource.Text = @"KRISHNA-HP\KRISHNASQL2019";
                            tb_sql_dbName.Text = "db_krishna";
                            tb_sql_username.Text = "sa";
                            tb_sql_pwd.Text = "1234";
                            tb_destination_tableName.Text = "multitable2";

                            break;

                        case 10:
                            rb_sql.Checked = true;
                            tb_oracle_dsource.Text = @"IESWKCT600";
                            tb_oracle_dbName.Text = "training_p";
                            tb_oracle_username.Text = "sa";
                            tb_oracle_pwd.Text = "abcde@1234567";
                            tb_Source_tableName.Text = "test_sql";

                            rb_Target_SQL.Checked = true;
                            tb_sql_dsource.Text = @"IESWKCT600";
                            tb_sql_dbName.Text = "training_p";
                            tb_sql_username.Text = "sa";
                            tb_sql_pwd.Text = "abcde@1234567";
                            tb_destination_tableName.Text = "test_sql_1";

                            break;


                    }
                }
            }
            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
            }
        }

        private void btn_Clear_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                tb_oracle_dsource.Text = string.Empty;
                tb_oracle_dbName.Text = string.Empty;
                tb_oracle_username.Text = string.Empty;
                tb_oracle_pwd.Text = string.Empty;
                tb_Source_tableName.Text = string.Empty;

                tb_sql_dsource.Text = string.Empty;
                tb_sql_dbName.Text = string.Empty;
                tb_sql_username.Text = string.Empty;
                tb_sql_pwd.Text = string.Empty;
                tb_destination_tableName.Text = string.Empty;

                //tb_FileName.Text = string.Empty;

                toggleCondition(true);

                resetLabelsToDefaultValue();

                resetCheckListBox();

                rb_oracle.Select();

                //btnGenerateOutput.Enabled = false;

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
                //lblCal.ResetText();
                lbl_PercentageShow.Text = "Progress Bar";
                progressBar1.Value = 0;
            }
            catch (Exception)
            {

            }
        }

        private void resetCheckListBox()
        {
            try
            {
                clb_ReferenceID.Items.Clear();
                cbL_columns.Items.Clear();
                cbL_TargetColumns.Items.Clear();
                cb_selectAll_toggle.CheckState = CheckState.Unchecked;
            }
            catch (Exception)
            {

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

                if (!boolStatus)
                {
                    myException_BG.Source = "There is some error";
                    return;
                }

                str_to_fetch = to_fetch(str_to_fetch);

                // call oracle BD
                if (boolStatus)
                {
                    ds = connect_Oracle_DB(str_to_fetch);
                }

                //Call SQL server DB
                if (boolStatus)
                {
                    ds = connect_SQL_DB(str_to_fetch);
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

                headRow = -1;
                alternateRow = 0;
                newRow = true;

                #region check for match and unmatch columns

                progress = 0;
                totalProgressCount = dt_oracle.Rows.Count;

                string str_col_used = string.Empty;

                CheckedListBox.CheckedItemCollection cic_oracleTB = cbL_columns.CheckedItems;
                CheckedListBox.CheckedItemCollection cic_reference = clb_ReferenceID.CheckedItems;

                #region main comparison part

                foreach (DataRow dr in dt_oracle.Rows)
                {
                    str_unique_reference_key = generateRowFilterString(dr);

                    //dv_sql.RowFilter = str_unique_reference_key + " = '" + dr[str_unique_reference_key] + "'";
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

                        foreach (var dc in cic_oracleTB)
                        {
                            if (!(cic_reference.Contains(dc.ToString())))
                            {
                                str_Data_Source_To_Compare = dr[dc.ToString()].ToString();
                                str_Data_Target_To_Compare = dv_sql[0][dc.ToString()].ToString();

                                boolCompareResult = false;

                                str_Data_Source_To_Compare = str_Data_Source_To_Compare.Trim();
                                str_Data_Target_To_Compare = str_Data_Target_To_Compare.Trim();

                                #region Ignore case

                                #endregion

                                if (!(dr[dc.ToString()].ToString() == dv_sql[0][dc.ToString()].ToString()))
                                //if ((!(boolCompareResult)) && (boolRulesCheck))
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

                                    }

                                    dataGridView2.Rows[headRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;
                                    dataGridView2.Rows[alternateRow - 1].Cells[cellCol].Style.BackColor = Color.Yellow;

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

                wb = new XLWorkbook();
                wb.Worksheets.Add(dt_unmatched_Capture, "Unmatched");
                wb.Worksheets.Add(dt_oracle_only, strSourceName + " rows only");
                wb.Worksheets.Add(dt_sql_only, "Sql rows only");
                wb.Worksheets.Add(dt_transpose, "Summary");

                backgroundWorker1.ReportProgress(105);

                formatExcelTableByDataGridView(1, dataGridView2);

                backgroundWorker1.ReportProgress(110);

                formatExcelTable(2, dt_oracle_only);
                formatExcelTable(3, dt_sql_only);
                formatExcelTable(4, dt_transpose);

                backgroundWorker1.ReportProgress(115);

                wb.SaveAs(strFilePathChangeSave);

                #endregion


                #region  removing columns which are not needed to be displayed as no mismatched found
                /*
                int colDelAdjust = 0;

                wb.Save();

                backgroundWorker1.ReportProgress(120);
                
                percentage = 0;
                totalProgressCount = dt_summarised_difference.Columns.Count;
                int eval;
                
                foreach (DataColumn dc in dt_summarised_difference.Columns)
                {
                    //if (dc.ColumnName.ToString() != str_unique_reference_key)
                    eval = ((((++percentage) * 100) / totalProgressCount) + 100);
                    backgroundWorker1.ReportProgress(eval);

                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker1.ReportProgress(0);
                        return;
                    }

                    if ((!(clb_ReferenceID.GetItemChecked(clb_ReferenceID.FindString(dc.ColumnName.ToString())))))
                    {
                        if (Convert.ToInt32(dt_summarised_difference.Rows[0][dc.ColumnName.ToString()]) == 0)
                        {
                            deleteColumnInExcel(1, dt_unmatched_Capture.Columns[dc.ColumnName.ToString()].Ordinal + 2 - colDelAdjust);
                            colDelAdjust++;
                        }
                    }
                }

                */
                backgroundWorker1.ReportProgress(125);

                wb.Save();

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

        private void displayElapsedTime()
        {
            try
            {
                time_to_execute = DateTime.Now.Subtract(start_Time);
                if (time_to_execute.Hours == 0)
                {
                    //lblTimer.Text = time_to_execute.Minutes.ToString() + " min(s) " + time_to_execute.Seconds.ToString() + " second(s) ";
                }
                else
                {
                    //lblTimer.Text = time_to_execute.Hours.ToString() + " hour(s) " +
                        //time_to_execute.Minutes.ToString() + " min(s) " + time_to_execute.Seconds.ToString() + " second(s) ";
                }
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
                    displayElapsedTime();
                    lbl_PercentageShow.Text = "100 %";
                }

            }
            catch (Exception ex)
            {
                lblExpressionStatus.Text = ex.Message.ToString();
                lblStatus.Text = myException_BG.Message.ToString();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
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
                    displayElapsedTime();
                    Process.Start(strFilePathChangeSave);
                }
                else if (myException_BG.Source != null)
                {
                    lblStatus.Text = myException_BG.Source;
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
            }
            finally
            {
                toggleCondition(true);
                //btn_Cancel.Enabled = false;
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
                lblStatus.Text = ex.Message.ToString();
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message.ToString();
            }
        }

        private void cb_windowsAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_windowsAuthentication.Checked)
                {
                    tb_sql_username.Enabled = false;
                    tb_sql_pwd.Enabled = false;
                }
                else if (cb_windowsAuthentication.CheckState == CheckState.Unchecked)
                {
                    tb_sql_username.Enabled = true;
                    tb_sql_pwd.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message.ToString();
            }
        }

        private void DisplayError(Exception ex)
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

        private void cb_Default_Port_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_Default_Port.Checked)
                {
                    tb_Default_Port.Text = "1521";
                    tb_Default_Port.Enabled = false;
                }
                else
                {
                    tb_Default_Port.Text = string.Empty;
                    tb_Default_Port.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }
}
