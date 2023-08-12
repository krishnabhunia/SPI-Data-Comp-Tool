using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class Pre_Merging_Duplicacy_Form : Form
    {
        private DBConnectionStatus dBConnectionStatus_SRC, dBConnectionStatus_TRG;
        private Exception myException_BG;
        private OracleDataAdapter oracleDA;
        private SqlDataAdapter sqlDA;

        private DataTable dt_SRC;
        private DataTable dt_LoadSRC;
        private DataTable dt_LoadTRG;
        private DataTable dt_sql;
        private DataTable dtDelta;
        private DataTable dtLoadFromDB;
        private DataTable dt_PM_Duplicacy, dt_PM_Duplicacy_SRC, dt_PM_Duplicacy_TRG, dt_PM_Details, dt_PM_Details_SRC, dt_PM_Details_TRG;

        private DataSet dsDelta;
        private DataSet dsTableList;

        private OpenFileDialogData openFileDialogData;
        private SaveFileDialogFile saveFileDialogFile;
        private Common_Data common_Data;
        private List<DBC> items;

        private string strQuery;
        private string strOracleConnection;
        private string str_tableName;
        private string strSQLConnection;
        private string str_excelFileName;
        private string strSourceName;

        private bool boolSavedFileOnce;
        //private bool boolConnection;
        private bool boolExcelFileList;
        private string str_SelectQuery_SRC;
        private string str_SelectQuery_TRG;

        private FolderBrowserDialog folderBrowserDialog1;
        private string strFolderPath;
        private bool boolFolderPathSelected, boolPreMergingDuplicacyTest, boolConfigurationMode;
        private DBConnectionDetails dBConnectionDetails;
        private DataTable dt_PM_TrimData;

        private int int_percentage, int_progressCount, int_totalCount, int_SrNo, int_DuplicacyCount;

        // variable for configuration mode
        private DataTable dtConfigurationTRGMissing, dtConfigurationSRCMissing, dtConfigurationMatchingSRC, dtConfigurationMatchingTRG;

        private bool boolConfigurationChecked;

        private string str_WhereClause, str_WhereClauseValues;
        public Pre_Merging_Duplicacy_Form(bool boolConfigurationMode) : this()
        {
            try
            {
                this.Text = "Configuration Form";
                cb_ConfigurationMode.CheckState = CheckState.Checked;

                dtConfigurationTRGMissing = new DataTable();
                dtConfigurationSRCMissing = new DataTable();
                dtConfigurationMatchingSRC = new DataTable();
                dtConfigurationMatchingTRG = new DataTable();

                if (GlobalDebug.boolIsGlobalDebug)
                {
                    cb_ConfigurationMode.Visible = true;
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        public Pre_Merging_Duplicacy_Form()
        {
            InitializeComponent();

            try
            {
                openFileDialogData = new OpenFileDialogData();
                dBConnectionStatus_SRC = new DBConnectionStatus();
                dBConnectionStatus_TRG = new DBConnectionStatus();
                saveFileDialogFile = new SaveFileDialogFile();
                common_Data = new Common_Data();
                dtLoadFromDB = new DataTable();
                dt_LoadSRC = new DataTable();
                dt_LoadTRG = new DataTable();
                common_Data = new Common_Data();
                folderBrowserDialog1 = new FolderBrowserDialog();
                dt_PM_Details = new DataTable();
                boolPreMergingDuplicacyTest = false;
                cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);

                dt_PM_TrimData = new DataTable();

                dt_PM_Duplicacy = new DataTable();

                dt_PM_Duplicacy_SRC = new DataTable();
                dt_PM_Duplicacy_TRG = new DataTable();

                if (GlobalDebug.ISGlobalDebug(GlobalDebug.strUserName, GlobalDebug.strPassword))
                {
                    folderBrowserDialog1.SelectedPath = @"D:\Krishna\Output";
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private DataTable InitialisePreMergeDuplicacyDT(DataTable dtt, string strCol, string strName)
        {
            try
            {
                if (dtt == null)
                {
                    dtt = new DataTable();
                }

                if (dtt.Rows.Count > 0)
                {
                    dtt.Rows.Clear();
                    dtt.Clear();
                }

                dtt.Columns.Clear();

                dtt = common_Data.AddColumns(dtt, "SRNO", typeof(int));
                dtt = common_Data.AddColumns(dtt, strCol);
                dtt = common_Data.AddColumns(dtt, "SOURCE_DUPLICACY_COUNT", typeof(int));
                dtt = common_Data.AddColumns(dtt, "TARGET_DUPLICACY_COUNT", typeof(int));

                dtt.TableName = strName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dtt;
        }



        private void DisplayError(Exception ex = null)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, ex.Message.ToString(), true);
                if (Debugger.IsAttached || GlobalDebug.boolIsGlobalDebug)
                {
                    Common_Data.DisplayMessage(rtb_Log, ex.ToString(), true);
                }
            }
            catch (Exception ex1)
            {
                Common_Data.DisplayMessage(rtb_Log, ex1.Message.ToString(), true);
                if (Debugger.IsAttached)
                {
                    Common_Data.DisplayMessage(rtb_Log, ex1.ToString(), true);
                }
            }
        }

        private void rb_SRC_oracle_CheckedChanged(object sender, EventArgs e)
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

        private void rb_SRC_SQL_CheckedChanged(object sender, EventArgs e)
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

        private void rb_TRG_Oracle_CheckedChanged(object sender, EventArgs e)
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

        private void rb_TRG_SQL_CheckedChanged(object sender, EventArgs e)
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

        private void cb_WindowAuthentication_TRG_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_WindowAuthentication_TRG.Checked)
                {
                    tb_TRG_UserName.Enabled = false;
                    tb_TRG_Password.Enabled = false;
                }
                else if (cb_WindowAuthentication_TRG.Checked == false)
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

        private void cb_WindowAuthentication_SRC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_WindowAuthentication_SRC.Checked)
                {
                    tb_SRC_UserName.Enabled = false;
                    tb_SRC_Password.Enabled = false;
                }
                else if (cb_WindowAuthentication_SRC.Checked == false)
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

        private void cb_SRC_DefaultPort_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_SRC_DefaultPort.Checked & rb_SRC_oracle.Checked)
                {
                    tb_SRC_DefaultPort.Text = "1521";
                    cb_SRC_DefaultPort.Enabled = true;
                    tb_SRC_DefaultPort.ReadOnly = true;
                }
                else
                {
                    tb_SRC_DefaultPort.ResetText();
                    tb_SRC_DefaultPort.ReadOnly = false;

                    if (rb_SRC_oracle.Checked)
                    {
                        tb_SRC_DefaultPort.Enabled = true;
                    }
                    else
                    {
                        tb_SRC_DefaultPort.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_TRG_DefaultPort_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_TRG_DefaultPort.Checked & rb_TRG_Oracle.Checked)
                {
                    tb_TRG_DefaultPort.Text = "1521";
                    tb_TRG_DefaultPort.Enabled = true;
                    tb_TRG_DefaultPort.ReadOnly = true;
                }
                else
                {
                    tb_TRG_DefaultPort.ResetText();
                    tb_TRG_DefaultPort.ReadOnly = false;

                    if (rb_TRG_Oracle.Checked)
                    {
                        tb_TRG_DefaultPort.Enabled = true;
                    }
                    else
                    {
                        tb_TRG_DefaultPort.Enabled = false;
                    }
                }
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
                //UpdateLabel(lblStatus, "Pls Wait, Loading !!!");
                Common_Data.DisplayMessage(rtb_Log, "Pls Wait, Loading !!!");
                if (rb_SRC_oracle.Checked)
                {
                    strQuery = "SELECT * FROM all_tables where OWNER != 'SYS' ORDER BY OWNER, TABLE_NAME";

                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_SRC_DataSource.Text + " )(PORT = " + tb_SRC_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_SRC_DBName.Text + ")));"
                        + "User Id=" + tb_SRC_UserName.Text + ";Password=" + tb_SRC_Password.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_SRC = new DataTable();
                    oracleDA.Fill(dt_SRC);
                    dt_SRC.TableName = "Load Table Name";
                    FillCheckListBox(cb_SRC_TableName, dt_SRC, "oracle");
                    dt_LoadSRC = dt_SRC.Copy();
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
                //lblStatus.Text = "Table Loaded in Source";
                Common_Data.DisplayMessage(rtb_Log, "Table Loaded in Source");
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
        private void resetLabelsToDefaultValue()
        {
            try
            {
                myException_BG = new Exception();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DBConnectionDetails dBConnectionDetails = new DBConnectionDetails(cb_UserProfileList.SelectedIndex);

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
                tb_TRG_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString(); ;

                tb_TRG_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                tb_TRG_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                tb_TRG_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                tb_TRG_Password.Text = dBConnectionDetails.str_TRG_Password;
                cb_TRG_TableName.Text = dBConnectionDetails.str_TRG_TableName;
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private DataTable GetDataTableFromDB(string strDBType, string strSelectQuery, string strConnectionString)
        {
            try
            {
                BG_DisplayDebugMsg($"Reading GetDataTableFromDB : {strSelectQuery} And {strConnectionString}");
                
                if (strDBType.ToLower() == "oracle")
                {
                    oracleDA = new OracleDataAdapter(strSelectQuery, strConnectionString);
                    dtLoadFromDB = new DataTable();
                    oracleDA.Fill(dtLoadFromDB);
                    dtLoadFromDB.TableName = "Oracle Table";
                }

                if (strDBType.ToUpper() == "SQL")
                {
                    dtLoadFromDB = new DataTable();
                    sqlDA = new SqlDataAdapter(strSelectQuery, strConnectionString);
                    sqlDA.Fill(dtLoadFromDB);
                    dtLoadFromDB.TableName = "SQL Table";
                }

                BG_DisplayDebugMsg($"Sucessfully GetDataTableFromDB : {strSelectQuery} And {strConnectionString}");

                dtLoadFromDB = common_Data.ChangeColumnToString(dtLoadFromDB);

                BG_DisplayDebugMsg("Returning from GetDataTableFromDB");

                return dtLoadFromDB;

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
                //UpdateLabel(lblStatus, "Pls Wait, Loading !!!");
                Common_Data.DisplayMessage(rtb_Log, "Pls Wait, Loading !!!");
                if (rb_TRG_Oracle.Checked)
                {
                    strQuery = "SELECT * FROM all_tables where OWNER != 'SYS' ORDER BY OWNER, TABLE_NAME";

                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_TRG_DataSource.Text + " )(PORT = " + tb_TRG_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_TRG_DBName.Text + ")));"
                        + "User Id=" + tb_TRG_UserName.Text + ";Password=" + tb_TRG_Password.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_SRC = new DataTable();
                    oracleDA.Fill(dt_SRC);
                    dt_SRC.TableName = "Load Table Name";
                    FillCheckListBox(cb_TRG_TableName, dt_SRC, "oracle");
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
                Common_Data.DisplayMessage(rtb_Log, "Table Loaded in Target");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void BG_DisplayMessage(string strMsg = null, bool boolError = false)
        {
            try
            {
                backgroundWorker1.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0,0,strMsg,boolError));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BG_DisplayError(Exception exbg)
        {
            try
            {
                backgroundWorker1.ReportProgress(2, new BackGroundWorkerUserObjectForReport(exbg));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BG_DisplayDebugMsg(string strMsg = null)
        {
            try
            {
                backgroundWorker1.ReportProgress(3, new BackGroundWorkerUserObjectForReport(0, 0, strMsg));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //private void DisplayMessage(string strMsg, bool boolError = false, int overwriteText = -1, Color? color = null)
        //{
        //    try
        //    {
        //        rtb_Log.SelectionStart = rtb_Log.TextLength;
        //        rtb_Log.SelectionColor = color ?? Color.Black;

        //        if (boolError)
        //        {
        //            rtb_Log.SelectionColor = Color.Red;
        //        }

        //        if (overwriteText > 0)
        //        {
        //            var charcf0 = rtb_Log.Rtf.LastIndexOf("\\cf0");
        //            if (charcf0 != -1)
        //            {
        //                var last = rtb_Log.Rtf.LastIndexOf("Processing", charcf0, 20);
        //                rtb_Log.SelectionColor = Color.Blue;
        //                if (last > 0)
        //                {
        //                    rtb_Log.Rtf = rtb_Log.Rtf.Remove(last, charcf0 - last);
        //                    charcf0 = rtb_Log.Rtf.LastIndexOf("\\cf0");
        //                    rtb_Log.Rtf = rtb_Log.Rtf.Insert(charcf0, strMsg);
        //                }
        //                else
        //                {
        //                    rtb_Log.AppendText($"\n{string.Format("{0:T}", DateTime.Now)} : {strMsg}");
        //                    rtb_Log.ScrollToCaret();
        //                }
        //            }
        //        }

        //        if (rtb_Log.TextLength > 0 && overwriteText == -1)
        //        {
        //            rtb_Log.AppendText($"\n{string.Format("{0:T}", DateTime.Now)} : {strMsg}");
        //            rtb_Log.ScrollToCaret();
        //        }
        //        else if (overwriteText == -1)
        //        {
        //            rtb_Log.Text = $"{string.Format("{0:T}", DateTime.Now)} : {strMsg}";
        //        }

        //        rtb_Log.Update();

        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayError(ex);
        //    }
        //}

        private DataTable CreateDuplicacyCol(DataTable dataTable, string strCols)
        {
            try
            {
                string strColsOriginal = strCols;
                strCols = strCols.Trim();
                strCols = common_Data.RemoveSpecialCharactersInColumn(strCols, "+");

                while (strCols.Contains("++"))
                {
                    strCols = strCols.Replace("++", "+");
                }

                //dataTable.Columns.Add("DUPLICACY_COLUMNS_ORG", typeof(string), strCols);
                dataTable.Columns.Add("DUPLICACY_COLUMNS_ORG", typeof(string));
                dataTable.Columns.Add("DUPLICACY_COLUMNS", typeof(string));

                foreach (DataRow drr in dataTable.Rows)
                {
                    drr["DUPLICACY_COLUMNS_ORG"] = Common_Data.GetDataFromRow(drr, strCols.Split('+'));
                    drr["DUPLICACY_COLUMNS"] = common_Data.RemoveSpecialCharacters(drr["DUPLICACY_COLUMNS_ORG"].ToString(), "");
                    //drr["DUPLICACY_COLUMNS"] = common_Data.RemoveSpecialCharacters(Common_Data.GetDataFromRow(drr, strCols.Split('+')),"");
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                foreach (DataRow dr in dsTableList.Tables[0].Rows)
                {
                    try
                    {
                        BG_DisplayMessage($"Processing Table : {dr["TABLE_NAME"]}, Pls Wait !!!");

                        str_WhereClause = str_WhereClauseValues = string.Empty;

                        if (dsTableList.Tables[0].Columns.Contains("WHERE_CLAUSE") && dsTableList.Tables[0].Columns.Contains("WHERE_CLAUSE_VALUE"))
                        {
                            if (dr["WHERE_CLAUSE"].ToString() != string.Empty || dr["WHERE_CLAUSE"].ToString().Trim() != "")
                            {
                                str_WhereClauseValues = string.Join(", ", dr["WHERE_CLAUSE_VALUE"].ToString().Split(',').Select(x => string.Concat("'", x.Trim(), "'")));
                                str_WhereClause = $"WHERE {dr["WHERE_CLAUSE"].ToString().Trim()} IN ({str_WhereClauseValues})";
                            }
                        }
                        else
                        {
                            BG_DisplayMessage($"This table doesn't have where clause columns", true);
                        }

                        str_SelectQuery_SRC = $"Select {dr["PRIMARY_KEY"]},{dr["DUPLICACY_COLUMNS"]} from {dr["TABLE_NAME"]} {str_WhereClause}";
                        str_SelectQuery_TRG = $"Select {dr["PRIMARY_KEY"]},{dr["DUPLICACY_COLUMNS"]} from {dr["TABLE_NAME"]} {str_WhereClause}";

                        if (boolConfigurationChecked)
                        {
                            str_SelectQuery_SRC = $"Select {dr["OUTPUT_COLUMNS"]} from {dr["TABLE_NAME"]}";
                            str_SelectQuery_TRG = $"Select {dr["OUTPUT_COLUMNS"]} from {dr["TABLE_NAME"]}";
                        }

                        //str_SelectQuery_TRG = $"Select {dr["PRIMARY_KEY"]},{dr["DUPLICACY_COLUMNS"]} from {dr["TABLE_NAME"]}";

                        BG_DisplayDebugMsg("Reading Datatable");
                        dt_LoadSRC = GetDataTableFromDB(dBConnectionStatus_SRC.strDBType, str_SelectQuery_SRC, dBConnectionStatus_SRC.strConnectionString);
                        dt_LoadTRG = GetDataTableFromDB(dBConnectionStatus_TRG.strDBType, str_SelectQuery_TRG, dBConnectionStatus_TRG.strConnectionString);

                        dt_LoadSRC.TableName = "Source Table";
                        dt_LoadTRG.TableName = "Target Table";

                        BG_DisplayDebugMsg("Both DataTable Read Successfully");

                        BG_DisplayDebugMsg("Trimming each cell in datatable");
                        dt_LoadSRC = TrimeDataInEachCell(dt_LoadSRC, "", false);
                        dt_LoadTRG = TrimeDataInEachCell(dt_LoadTRG, dr["PRIMARY_KEY"].ToString(), true);
                        BG_DisplayDebugMsg("Trimming Done !!!");

                        BG_DisplayDebugMsg("Create Duplicacy called");
                        dt_LoadSRC = CreateDuplicacyCol(dt_LoadSRC, dr["DUPLICACY_COLUMNS"].ToString());
                        dt_LoadTRG = CreateDuplicacyCol(dt_LoadTRG, dr["DUPLICACY_COLUMNS"].ToString());
                        BG_DisplayDebugMsg("Create Duplicacy executed successfully");

                        var querySRC = dt_LoadSRC.AsEnumerable()
                        .GroupBy(r => new { Name = r.Field<string>("DUPLICACY_COLUMNS"), Action = r.Field<string>("DUPLICACY_COLUMNS") })
                        .Select(grp => new
                        {
                            D_C = grp.ToList()[0]["DUPLICACY_COLUMNS"],
                            Name = grp.Key.Name,
                            Action = grp.Key.Action,
                            Count = grp.Count()
                        });

                        var queryTRG = dt_LoadTRG.AsEnumerable()
                        .GroupBy(r => new { Name = r.Field<string>("DUPLICACY_COLUMNS"), Action = r.Field<string>("DUPLICACY_COLUMNS") })
                        .Select(grp => new
                        {
                            D_C = grp.ToList()[0]["DUPLICACY_COLUMNS"],
                            Name = grp.Key.Name,
                            Action = grp.Key.Action,
                            Count = grp.Count()
                        });

                        if (cb_ConfigurationMode.CheckState == CheckState.Unchecked)
                        {
                            BG_DisplayDebugMsg("Initalising pre merging duplicacy");

                            dt_PM_Duplicacy_SRC = InitialisePreMergeDuplicacyDT(dt_PM_Duplicacy_SRC, dr["DUPLICACY_COLUMNS"].ToString(), "Duplicacy List SRC");
                            dt_PM_Duplicacy_TRG = InitialisePreMergeDuplicacyDT(dt_PM_Duplicacy_TRG, dr["DUPLICACY_COLUMNS"].ToString(), "Duplicacy List TRG");

                            dt_PM_Details_SRC = new DataTable();
                            dt_PM_Details_TRG = new DataTable();

                            BG_DisplayDebugMsg("Initalising pre merging duplicacy done");

                            dt_LoadSRC = common_Data.AddColumns(dt_LoadSRC, "Count", typeof(int));
                            dt_LoadTRG = common_Data.AddColumns(dt_LoadTRG, "Count", typeof(int));

                            int_SrNo = 0;
                            int_DuplicacyCount = 0;

                            BG_DisplayDebugMsg("Adding Count Column in Source Table");

                            foreach (var i in querySRC)
                            {
                                int_DuplicacyCount = GetCountOfValueInDT(dt_LoadTRG, "DUPLICACY_COLUMNS", i.Name);
                                //BG_DisplayDebugMsg($"Getting Count : {int_DuplicacyCount}");
                                if (int_DuplicacyCount > 0)
                                {
                                    dt_PM_Duplicacy_SRC.Rows.Add(++int_SrNo, i.Name, i.Count, int_DuplicacyCount);
                                    try
                                    {
                                        if (cb_IgnoreCase.Checked)
                                        {
                                            var queryCount = dt_LoadSRC.AsEnumerable().Where(x => x.Field<string>("DUPLICACY_COLUMNS").ToString().Equals(i.D_C.ToString()))
                                                .ToList();
                                            queryCount.ForEach(a => a.SetField("Count", i.Count));
                                        }
                                        else
                                        {
                                            var queryCount = dt_LoadSRC.AsEnumerable().Where(x => x.Field<string>("DUPLICACY_COLUMNS").ToString().ToUpper().Equals(i.D_C.ToString().ToUpper()))
                                                .ToList();
                                            queryCount.ForEach(a => a.SetField("Count", i.Count));
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        BG_DisplayError(ex);
                                        BG_DisplayDebugMsg("Error in LINQ Query");
                                    }
                                }
                            }

                            dt_LoadSRC.AcceptChanges();
                            BG_DisplayDebugMsg("Get Count Of Value Successfully executed");

                            // Processing 2nd sheet

                            int_SrNo = 0;

                            foreach (var i in queryTRG)
                            {
                                int_DuplicacyCount = GetCountOfValueInDT(dt_LoadSRC, "DUPLICACY_COLUMNS", i.Name);
                                if (int_DuplicacyCount > 0)
                                {
                                    dt_PM_Duplicacy_TRG.Rows.Add(++int_SrNo, i.Name, i.Count, int_DuplicacyCount);
                                    try
                                    {
                                        if (cb_IgnoreCase.Checked)
                                        {
                                            var queryCount = dt_LoadTRG.AsEnumerable().Where(x => x.Field<string>("DUPLICACY_COLUMNS").ToString().Equals(i.D_C.ToString()))
                                                .ToList();
                                            queryCount.ForEach(a => a.SetField("Count", i.Count));
                                        }
                                        else
                                        {
                                            var queryCount = dt_LoadTRG.AsEnumerable().Where(x => x.Field<string>("DUPLICACY_COLUMNS").ToString().ToUpper().Equals(i.D_C.ToString().ToUpper()))
                                                .ToList();
                                            queryCount.ForEach(a => a.SetField("Count", i.Count));
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        BG_DisplayError(ex);
                                        BG_DisplayDebugMsg("Error in LINQ Query");
                                    }
                                }
                            }

                            dt_LoadTRG.AcceptChanges();
                            BG_DisplayDebugMsg("Get Count Of Value Successfully executed");

                            dt_PM_Details_SRC.TableName = "Source Details";
                            dt_PM_Details_TRG.TableName = "Target Details";

                            dt_PM_Details_SRC = dt_LoadSRC.Copy();
                            dt_PM_Details_TRG = dt_LoadTRG.Copy();

                            if (!(GlobalDebug.boolIsGlobalDebug || Debugger.IsAttached))
                            {
                                dt_PM_Details_SRC = common_Data.RemoveColumn(dt_PM_Details_SRC, "DUPLICACY_COLUMNS");
                                dt_PM_Details_SRC = common_Data.RemoveColumn(dt_PM_Details_SRC, "DUPLICACY_COLUMNS_ORG");

                                dt_PM_Details_TRG = common_Data.RemoveColumn(dt_PM_Details_TRG, "DUPLICACY_COLUMNS");
                                dt_PM_Details_TRG = common_Data.RemoveColumn(dt_PM_Details_TRG, "DUPLICACY_COLUMNS_ORG");
                            }

                            dsDelta.Clear();
                            dsDelta.Tables.Clear();

                            dsDelta.Tables.Add(dt_PM_Duplicacy_SRC.Copy());
                            dsDelta.Tables.Add(dt_PM_Duplicacy_TRG.Copy());
                            dsDelta.Tables.Add(dt_PM_Details_SRC.Copy());
                            dsDelta.Tables.Add(dt_PM_Details_TRG.Copy());

                            if (dt_PM_TrimData != null)
                            {
                                dsDelta.Tables.Add(dt_PM_TrimData.Copy());
                            }

                            if (GlobalDebug.boolIsGlobalDebug)
                            {
                                dt_LoadSRC.TableName = "Debug Copy SRC Table";
                                dt_LoadTRG.TableName = "Debug Copy TRG Table";
                                dsDelta.Tables.Add(dt_LoadSRC.Copy());
                                dsDelta.Tables.Add(dt_LoadTRG.Copy());
                            }

                            //dsDelta = common_Data.ChangeColumnToString(dsDelta);
                            BG_DisplayDebugMsg("All process finished and saving to start");
                            if (ExportToExcel(dsDelta, dt_PM_Details.TableName.ToString(), dr["TABLE_NAME"].ToString()))
                            {
                                //DisplayMessage($"File saved successfully : {str_excelFileName}");
                                BG_DisplayMessage($"Processing finished for table : {dr["TABLE_NAME"]}");
                            }
                            else
                            {
                                BG_DisplayMessage($"There is some problem is saving the file", true);
                            }

                            boolPreMergingDuplicacyTest = true;
                            //return boolPreMergingDuplicacyTest;
                        }
                        else if (cb_ConfigurationMode.CheckState == CheckState.Checked)
                        {
                            BG_DisplayDebugMsg("Running in Configuration Mode");
                            dtConfigurationTRGMissing = dt_LoadSRC.Copy();
                            dtConfigurationSRCMissing = dt_LoadTRG.Copy();

                            dtConfigurationMatchingSRC = dtConfigurationTRGMissing.Clone();
                            dtConfigurationMatchingTRG = dtConfigurationSRCMissing.Clone();

                            BG_DisplayDebugMsg("Duplicacy Columns count and checking in Configuration Mode for Source");
                            foreach (var i in querySRC)
                            {
                                int_DuplicacyCount = GetCountOfValueInDT(dt_LoadTRG, "DUPLICACY_COLUMNS_ORG", i.Name);
                                if (int_DuplicacyCount > 0)
                                {
                                    DataRow[] drDelete = dtConfigurationTRGMissing.Select($"DUPLICACY_COLUMNS_ORG = '{i.Name}'");

                                    foreach (DataRow drrDel in drDelete)
                                    {
                                        dtConfigurationMatchingSRC.Rows.Add(drrDel.ItemArray);
                                        dtConfigurationTRGMissing.Rows.Remove(drrDel);
                                    }
                                }
                            }

                            BG_DisplayDebugMsg("Duplicacy Columns count and checking in Configuration Mode for Target");
                            foreach (var i in queryTRG)
                            {
                                int_DuplicacyCount = GetCountOfValueInDT(dt_LoadSRC, "DUPLICACY_COLUMNS_ORG", i.Name);
                                if (int_DuplicacyCount > 0)
                                {
                                    DataRow[] drDelete = dtConfigurationSRCMissing.Select($"DUPLICACY_COLUMNS_ORG = '{i.Name}'");

                                    foreach (DataRow drrDel in drDelete)
                                    {
                                        dtConfigurationMatchingTRG.Rows.Add(drrDel.ItemArray);
                                        dtConfigurationSRCMissing.Rows.Remove(drrDel);
                                    }
                                }
                            }

                            DataTable dtMatch = new DataTable();
                            dtMatch = dtConfigurationMatchingSRC.Copy();

                            BG_DisplayDebugMsg("Adding Matching Columns in Configuration Mode");
                            foreach (DataColumn dcInTRG in dtConfigurationMatchingTRG.Columns)
                            {
                                dtMatch.Columns.Add($"{dcInTRG.ColumnName}_TRG");
                            }

                            var queryEntry = dtMatch.AsEnumerable().ToList();

                            BG_DisplayDebugMsg("Starting Query Entry Mode in Configuration");
                            foreach (var a in queryEntry)
                            {
                                DataRow dataRowSelect = dtConfigurationMatchingTRG.Select($"DUPLICACY_COLUMNS_ORG = '{a["DUPLICACY_COLUMNS_ORG"]}'")[0];

                                foreach (DataColumn dcInTRG in dtConfigurationMatchingTRG.Columns)
                                {
                                    a[$"{dcInTRG.ColumnName}_TRG"] = dataRowSelect[dcInTRG.ColumnName];
                                }
                            }

                            dtConfigurationTRGMissing = common_Data.RemoveColumn(dtConfigurationTRGMissing, "DUPLICACY_COLUMNS_ORG");
                            dtConfigurationTRGMissing = common_Data.RemoveColumn(dtConfigurationTRGMissing, "DUPLICACY_COLUMNS");

                            dtConfigurationSRCMissing = common_Data.RemoveColumn(dtConfigurationSRCMissing, "DUPLICACY_COLUMNS_ORG");
                            dtConfigurationSRCMissing = common_Data.RemoveColumn(dtConfigurationSRCMissing, "DUPLICACY_COLUMNS");

                            dtConfigurationMatchingSRC = common_Data.RemoveColumn(dtConfigurationMatchingSRC, "DUPLICACY_COLUMNS_ORG");
                            dtConfigurationMatchingSRC = common_Data.RemoveColumn(dtConfigurationMatchingSRC, "DUPLICACY_COLUMNS");

                            dtConfigurationMatchingTRG = common_Data.RemoveColumn(dtConfigurationMatchingTRG, "DUPLICACY_COLUMNS_ORG");
                            dtConfigurationMatchingTRG = common_Data.RemoveColumn(dtConfigurationMatchingTRG, "DUPLICACY_COLUMNS");

                            dtMatch = common_Data.RemoveColumn(dtMatch, "DUPLICACY_COLUMNS_ORG");
                            dtMatch = common_Data.RemoveColumn(dtMatch, "DUPLICACY_COLUMNS");

                            dtMatch = common_Data.RemoveColumn(dtMatch, "DUPLICACY_COLUMNS_ORG_TRG");
                            dtMatch = common_Data.RemoveColumn(dtMatch, "DUPLICACY_COLUMNS_TRG");

                            dtConfigurationSRCMissing.TableName = "Missing In Source";
                            dtConfigurationTRGMissing.TableName = "Missing In Target";
                            dtMatch.TableName = "Matching";

                            BG_DisplayDebugMsg("Internal processing for duplicate columns");

                            dsDelta.Clear();
                            dsDelta.Tables.Clear();

                            dsDelta.Tables.Add(dtConfigurationSRCMissing.Copy());
                            dsDelta.Tables.Add(dtConfigurationTRGMissing.Copy());
                            dsDelta.Tables.Add(dtMatch.Copy());

                            BG_DisplayDebugMsg("All process finished and saving in configuration mode");
                            if (ExportToExcel(dsDelta, dr["TABLE_NAME"].ToString()))
                            {
                                //DisplayMessage($"File saved successfully : {str_excelFileName}");
                                BG_DisplayMessage($"Processing finished for table : {dr["TABLE_NAME"]}");
                            }
                            else
                            {
                                BG_DisplayMessage($"There is some problem is saving the file", true);
                            }

                            boolConfigurationMode = true;
                            //return boolConfigurationMode;
                        }
                    }
                    catch (Exception ex)
                    {
                        BG_DisplayError(ex);
                    }
                    backgroundWorker1.ReportProgress(1, new BackGroundWorkerUserObjectForReport(++int_progressCount, int_totalCount));
                }
            }
            catch (Exception ex)
            {
                BG_DisplayError(ex);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                BackGroundWorkerUserObjectForReport backGroundWorkerUserObjectForReport = (BackGroundWorkerUserObjectForReport)e.UserState;
                if(e.ProgressPercentage == 0)
                {
                    Common_Data.DisplayMessage(rtb_Log, backGroundWorkerUserObjectForReport.strMsg);
                }
                
                if(e.ProgressPercentage == 1)
                {
                    int_percentage = (int_progressCount * 100) / int_totalCount;
                    progressBar1.Value = int_percentage;
                    progressBar1.Update();
                    Common_Data.DisplayMessage(rtb_Log, $"Progress : {int_percentage} % Completed, Pls Wait !!!");
                }

                if(e.ProgressPercentage == 2)
                {
                    Common_Data.DisplayError(rtb_Log, backGroundWorkerUserObjectForReport.bgwException);
                }

                if(e.ProgressPercentage == 3)
                {
                    Common_Data.DisplayDebugMsg(rtb_Log, backGroundWorkerUserObjectForReport.strMsg);
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                groupBox3.Enabled = true;
                boolPreMergingDuplicacyTest = boolPreMergingDuplicacyTest || boolConfigurationMode;

                if (boolPreMergingDuplicacyTest)
                {
                    Common_Data.DisplayMessage(rtb_Log, "PreMerging Completed", false, -1, Color.ForestGreen);
                    Common_Data.DisplayMessage(rtb_Log, $"File Saved In Location : {str_excelFileName}");
                    llb_File.Visible = true;
                    llb_File.Text = strFolderPath;
                }
                else
                {
                    lb_link.Visible = false;
                    llb_File.Visible = false;
                    llb_File.Text = "No File";
                    Common_Data.DisplayMessage(rtb_Log, "There is some problem", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private DataTable TrimeDataInEachCell(DataTable dt, string str_PK, bool boolGenerateSheet)
        {
            string strTest = string.Empty;
            try
            {
                if (boolGenerateSheet)
                {
                    dt_PM_TrimData = new DataTable();
                    dt_PM_TrimData.TableName = "Trimming spaces removal list";
                    dt_PM_TrimData = common_Data.AddColumns(dt_PM_TrimData, str_PK);
                    dt_PM_TrimData = common_Data.AddColumns(dt_PM_TrimData, "TRIMMING_SPACES");
                    int int_rowCount = 0;
                    foreach (DataRow drow in dt.Rows)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (drow.Field<string>(dc) != null && drow.Field<string>(dc) != drow.Field<string>(dc).Trim())
                            {
                                drow.SetField<string>(dc, drow.Field<string>(dc).Trim());
                                dt_PM_TrimData = common_Data.AddColumns(dt_PM_TrimData, dc.ColumnName);

                                DataRow drNew = dt_PM_TrimData.NewRow();
                                drNew[str_PK] = drow[str_PK];
                                drNew["TRIMMING_SPACES"] = "REMOVED";
                                drNew[dc.ColumnName] = drow[dc];
                                dt_PM_TrimData.Rows.Add(drNew);
                            }
                        }
                    }
                }
                else
                {
                    foreach (DataRow drow in dt.Rows)
                    {
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (drow.Field<string>(dc) != null)
                            {
                                drow.SetField<string>(dc, drow.Field<string>(dc).Trim());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
                throw ex;
            }
            return dt;
        }

        private void PreMergingDuplicacyTest()
        {
            try
            {
                boolPreMergingDuplicacyTest = false;
                boolConfigurationMode = false;

                int_totalCount = dsTableList.Tables[0].Rows.Count;
                int_percentage = int_progressCount = 0;

                boolConfigurationChecked = cb_ConfigurationMode.Checked;

                if(!(backgroundWorker1.IsBusy))
                {
                    groupBox3.Enabled = false;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                progressBar1.Value = 0;
                progressBar1.Update();
                boolPreMergingDuplicacyTest = false;
                DisplayError(ex);
                throw ex;
            }
        }

        private bool ExportToExcel(DataTable dataTable, string strSheetName, string strFileName)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet worksheet;
                    worksheet = pck.Workbook.Worksheets.Add(strSheetName);
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                    worksheet.Cells.AutoFitColumns(22, 55);

                    str_excelFileName = $@"{strFolderPath}\PM_Duplicacy_{strFileName}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                    pck.SaveAs(new System.IO.FileInfo(str_excelFileName));
                    return true;
                    //DisplayMessage($"File Saved : {str_excelFileName}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ExportToExcel(DataSet ds, string strSheetName, string strFileName)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet worksheet;

                    foreach (DataTable dt in ds.Tables)
                    {
                        worksheet = pck.Workbook.Worksheets.Add(dt.TableName);
                        worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                        worksheet.Cells.AutoFitColumns(22, 55);

                    }
                    str_excelFileName = $@"{strFolderPath}\PM_Duplicacy_{strFileName}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                    pck.SaveAs(new System.IO.FileInfo(str_excelFileName));
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ExportToExcel(DataSet ds, string strFileName)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet worksheet;
                    foreach (DataTable dt in ds.Tables)
                    {
                        worksheet = pck.Workbook.Worksheets.Add(dt.TableName);
                        worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                        worksheet.Cells.AutoFitColumns(22, 55);
                    }
                    str_excelFileName = $@"{strFolderPath}\ConfigurationMode_{strFileName}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                    pck.SaveAs(new System.IO.FileInfo(str_excelFileName));
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ExportToExcelInSingleSheet(DataSet ds, string strFileName)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet worksheet;

                    worksheet = pck.Workbook.Worksheets.Add("Matching");
                    int int_Col = 1;

                    foreach (DataTable dt in ds.Tables)
                    {
                        //worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                        worksheet.Cells[1, int_Col].LoadFromDataTable(dt, true);
                        int_Col += dt.Columns.Count;
                    }

                    worksheet.Cells.AutoFitColumns(22, 55);
                    str_excelFileName = $@"{strFolderPath}\ConfigurationMode_{strFileName}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                    pck.SaveAs(new System.IO.FileInfo(str_excelFileName));
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int GetCountOfValueInDT(DataTable dataTable, string strdataColumn, string strValue)
        {
            try
            {
                if (strValue == null)
                {
                    return 0;
                }
                if (strValue.Contains(@"'"))
                {
                    strValue = strValue.Replace(@"'", @"''");
                }
                DataRow[] dataRows = dataTable.Select($"[{strdataColumn}] = '{strValue}'");

                return dataRows.Length;

            }
            catch (Exception ex)
            {
                Common_Data.DisplayDebugMsg(rtb_Log, "Error in GetCountOfValueInDT");
                throw ex;
            }
        }

        //private void DisplayDebugMsg(string strMsg)
        //{
        //    try
        //    {
        //        if (GlobalDebug.boolIsGlobalDebug)
        //        {
        //           Common_Data.DisplayMessage(rtb_Log,$"Debug Msg : {strMsg}", false, -1, Color.BlueViolet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayError(ex);
        //    }
        //}

        private void btn_ListTableExcel_Click(object sender, EventArgs e)
        {
            try
            {
                boolPreMergingDuplicacyTest = false;
                lb_link.Visible = false;
                llb_File.Visible = false;
                dtDelta = new DataTable();
                dsDelta = new DataSet();

                boolSavedFileOnce = false;
                //throw new Exception ("test");
                Common_Data.DisplayMessage(rtb_Log, "Reading Pre-Merging Excel File ... ");
                openFileDialogData = openFileDialogData.openFileDialog("Select Excel File Contains Pre Merging Data Duplicacy");

                if (openFileDialogData.boolFileSelected)
                {
                    dsTableList = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);

                    if (dsTableList != null && dsTableList.Tables.Count > 0 && dsTableList.Tables[0].Rows.Count > 0)
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"Selected File {openFileDialogData.strFileNameWithPath}");
                        if (dBConnectionStatus_SRC.boolConnection && dBConnectionStatus_TRG.boolConnection)
                        {
                            Common_Data.DisplayMessage(rtb_Log, "Select Folder To Save All Excel Output");

                            folderBrowserDialog1.SelectedPath = openFileDialogData.PathName_withoutFileName;

                            folderBrowserDialog1.Description = "Select Folder To Save All Excel Output";
                            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                            {
                                strFolderPath = folderBrowserDialog1.SelectedPath;
                                lb_link.Visible = true;
                                llb_File.Text = strFolderPath;
                                llb_File.Visible = true;
                                Common_Data.DisplayMessage(rtb_Log, $"Folder path set to :{strFolderPath}");
                                boolFolderPathSelected = true;
                            }
                            else
                            {
                                strFolderPath = string.Empty;
                                llb_File.Text = strFolderPath;
                                llb_File.Visible = false;
                                lb_link.Visible = false;
                                Common_Data.DisplayMessage(rtb_Log, $"Folder path not set, aborting all operation", true);
                                boolFolderPathSelected = false;
                            }

                            if (boolFolderPathSelected)
                            {
                                Common_Data.DisplayMessage(rtb_Log, "PreMerging Started");
                                boolPreMergingDuplicacyTest = false;
                                PreMergingDuplicacyTest();
                            }
                            else
                            {
                                Common_Data.DisplayMessage(rtb_Log, "File Not Saved");
                            }
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, "Pls connect with DB", true);
                        }
                        boolExcelFileList = true;
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"File Not Selected", true);
                        boolExcelFileList = false;
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"File Not Selected", true);
                    boolExcelFileList = false;
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void llb_File_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (strFolderPath != null && strFolderPath != string.Empty)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Folder is getting open : {strFolderPath}");
                    Process.Start(strFolderPath);
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
                string strJsonFileName = @"D:\Krishna\Projects\SPI\SPI Data Comp Tool\SPI Data Comp Tool\UserProfile\userProfile.json";
                //JObject jObject = JObject.Parse(File.ReadAllText(strJsonFileName));

                StreamReader streamReader = new StreamReader(strJsonFileName);
                JsonTextReader jsonTextReader = new JsonTextReader(streamReader);

                using (StreamReader r = new StreamReader(strJsonFileName))
                {
                    string json = r.ReadToEnd();
                    items = JsonConvert.DeserializeObject<List<DBC>>(json);
                }

                Profile.ProfileList = items;

                cb_UserProfileList.Items.Clear();

                foreach (var i in items)
                {
                    if (i.profileName != null)
                    {
                        cb_UserProfileList.Items.Add(i.profileName.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void btn_Clear_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                rtb_Log.Clear();
                Common_Data.DisplayMessage(rtb_Log, "Logs Cleared");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_UserProfileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, $"User Profile Selected : {cb_UserProfileList.SelectedItem}");

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

                Common_Data.DisplayMessage(rtb_Log, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
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

        private void btn_Connect_Reference_ID_Click(object sender, EventArgs e)
        {
            try
            {
                string str_DBName = tb_SRC_DBName.Text.Trim();
                //string str_SchemaName = tb_SchemaName.Text.Trim();

                if (rb_SRC_oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Source Oracle...");

                    dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_SRC_oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_SRC_DefaultPort.Text.Trim());
                }
                else if (rb_SRC_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Source SQL...");
                    dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_SRC_SQL.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_SRC_DefaultPort.Text.Trim());
                }

                Common_Data.DisplayMessage(rtb_Log, dBConnectionStatus_SRC.strConnectionMSG, (!dBConnectionStatus_SRC.boolConnection), -1, Color.DarkBlue);
                str_DBName = tb_TRG_DBName.Text.Trim();
                //str_SchemaName = tb_SchemaName.Text.Trim();

                if (rb_TRG_Oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Target Oracle...");

                    dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_Oracle.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                }
                else if (rb_TRG_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Target SQL...");
                    dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_SQL.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                }
                Common_Data.DisplayMessage(rtb_Log, dBConnectionStatus_TRG.strConnectionMSG, (!dBConnectionStatus_TRG.boolConnection), -1, Color.DarkBlue);

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }
}
