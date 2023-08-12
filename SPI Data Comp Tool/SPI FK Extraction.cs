using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SPI_Data_Comp_Tool
{
    public partial class SPI_FK_Extraction : Form
    {
        Common_Data common_Data;
        private OpenFileDialogData openFileDialogData;
        private DataTable dtGenerateScript;
        private string str_SRCTableName;
        private string strPK;
        private DataTable dtSRC;
        private DataTable dtTRG;
        public List<DataColumn> ListCols;
        private DBConnectionDetails dBConnectionDetails;
        private string str_DBNameSRC;
        private string str_DBNameTRG;
        private DBConnectionStatus dbConnection;
        private DBConnectionStatus dbSRCConnection;
        private bool boolSRCConnection;
        private DBConnectionStatus dbTRGConnection;
        private bool boolTRGConnection;
        private bool boolLoadDataBase;
        private string strSelectCol;
        private DataTable dtFetch;
        private DataSet dsTableList;
        private DataTable dtResult;
        private DataColumn[] dcResult;
        private string[] strRes;
        private FolderBrowserDialog folderBrowserDialog1;
        private string strFolderPath;
        private bool boolFolderPathSelected;
        private string strFileName;
        private SaveFileDialogFile saveFileDialogFile;

        public SPI_FK_Extraction()
        {
            InitializeComponent();

            try
            {
                openFileDialogData = new OpenFileDialogData();
                dtGenerateScript = new DataTable();
                ListCols = new List<DataColumn>();
                dBConnectionDetails = new DBConnectionDetails();
                common_Data = new Common_Data();
                dbConnection = new DBConnectionStatus();
                dtSRC = new DataTable();
                dtResult = new DataTable();
                folderBrowserDialog1 = new FolderBrowserDialog();
                strRes = new string[] { "TABLE_NAME_FK", "TABLE_FK_PRIMARY_KEY", "FOREIGN_KEY_SRC", "TABLE_NAME", "FOREIGN_KEY_TRG" };
                saveFileDialogFile = new SaveFileDialogFile();
                dtResult.TableName = "Result";
                foreach (string s in strRes)
                {
                    dtResult.Columns.Add(new DataColumn(s, typeof(string)));
                }

                if(Debugger.IsAttached || GlobalDebug.boolIsGlobalDebug)
                {
                    cb_UserProfileList.Visible = true;
                    cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private DataTable GetDataTable(DBConnectionStatus dBConnection, string strTableName)
        {
            try
            {
                string[] str_IgnoreColName;

                strSelectCol = $"SELECT * FROM {strTableName} WHERE 1 = 0";

                using (SqlConnection sqlConnection = new SqlConnection(dBConnection.strConnectionString))
                {
                    using (SqlDataAdapter sqlDA = new SqlDataAdapter(strSelectCol, sqlConnection))
                    {
                        dtFetch = new DataTable();
                        sqlDA.Fill(dtFetch);
                        dtFetch.TableName = strTableName;
                    }
                }
                return dtFetch;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetData(string strQuery, string strResName)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(dbSRCConnection.strConnectionString))
                {
                    using (SqlDataAdapter sqlDA = new SqlDataAdapter(strQuery, sqlConnection))
                    {
                        dtFetch = new DataTable();
                        sqlDA.Fill(dtFetch);
                        dtFetch.TableName = "Fetch";
                    }
                }

                if(dtFetch.Rows.Count > 0)
                {
                    return dtFetch.Rows[0][strResName].ToString();
                }
                else
                {
                    return "";
                }

                //return dtFetch;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btn_ImportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialogData = openFileDialogData.openFileDialog("Open File To Execute Scripts");

                if (openFileDialogData.boolFileSelected)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Input Script File : {openFileDialogData.strFileNameWithPath}");
                    dtGenerateScript = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath).Tables[0];
                    dtResult.Rows.Clear();

                    Common_Data.DisplayMessage(rtb_Log, $"Processing started !!!");
                    foreach (DataRow dr in dtGenerateScript.Rows)
                    {
                        str_SRCTableName = dr["table_name_fk"].ToString().ToUpper();
                        strPK = dr["table_fk_primary_key"].ToString().ToUpper();

                        Common_Data.DisplayMessage(rtb_Log, $"{str_SRCTableName} with {strPK} is being processed");

                        dtSRC = GetDataTable(dbSRCConnection, str_SRCTableName);

                        ListCols = (from DataColumn col in dtSRC.Columns select col).Where(x => x.ColumnName.ToUpper().EndsWith("_ID")).ToList();

                        string[] strPKarr = strPK.Split(',').Select(x => x.Trim()).ToArray();

                        foreach (DataColumn dc in ListCols.ToArray())
                        {
                            if (strPKarr.Contains(dc.ToString()))
                            {
                                ListCols.Remove(dc);
                            }
                        }

                        foreach (DataColumn dc in ListCols)
                        {
                            dtResult.Rows.Add(str_SRCTableName, strPK, dc.ToString());
                        }

                        Common_Data.DisplayMessage(rtb_Log, $"Initial process complete");
                    }

                    dtResult = GetTableNameByPrimaryKey(dtResult);

                    saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Excel File", $"List of IDs_{common_Data.AppendDateInOutputFileName(DateTime.Now)}");

                    if (saveFileDialogFile.BoolFileSaveStatus)
                    {
                        if (WriteExcel.ExportToExcel(dtResult, "List of IDs", saveFileDialogFile.Str_saveFileNameWithPath))
                        {
                            Common_Data.DisplayMessage(rtb_Log, $"File saved as : {saveFileDialogFile.Str_saveFileNameWithPath}");
                            Process.Start(saveFileDialogFile.Str_saveFileNameWithPath);
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, $"There is some problem in saving the file : {saveFileDialogFile.Str_saveFileNameWithPath}", true);
                        }
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"User cancelled for saving the file : {saveFileDialogFile.Str_saveFileNameWithPath}", true);
                    }

                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, "Either readfile or save file is not set", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private DataTable GetTableNameByPrimaryKey(DataTable dt)
        {
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string strQuery = $"SELECT T.CONSTRAINT_CATALOG,T.CONSTRAINT_SCHEMA,T.TABLE_NAME,C.COLUMN_NAME,T.CONSTRAINT_NAME,C.CONSTRAINT_NAME, T.CONSTRAINT_TYPE FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C ON C.CONSTRAINT_NAME = T.CONSTRAINT_NAME and C.TABLE_NAME = T.TABLE_NAME WHERE T.CONSTRAINT_TYPE = 'PRIMARY KEY' AND C.COLUMN_NAME = '{dr["FOREIGN_KEY_SRC"]}' ";

                    Common_Data.DisplayMessage(rtb_Log, $"Code : {strQuery}");

                    dr["TABLE_NAME"] = GetData(strQuery, "TABLE_NAME");
                    dr["FOREIGN_KEY_TRG"] = dr["FOREIGN_KEY_SRC"];

                    Common_Data.DisplayDebugMsg(rtb_Log, $"Code Execution complete");
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }

            return dt;
        }

        //private void CheckChangeRadioButton(int int_Group)
        //{
        //    try
        //    {
        //        if (int_Group == 1)
        //        {
        //            if (rb_SRC_oracle.Checked)
        //            {
        //                gb_Source1.Text = "Source: Oracle Connection and Settings";
        //                cb_WindowAuthentication_SRC.Enabled = false;
        //                cb_WindowAuthentication_SRC.Checked = false;
        //                cb_SRC_DefaultPort.Enabled = true;
        //                cb_SRC_DefaultPort.Checked = true;
        //                tb_SRC_DefaultPort.Enabled = true;
        //                tb_SRC_DefaultPort.ReadOnly = true;
        //            }
        //            else if (rb_SRC_SQL.Checked)
        //            {
        //                gb_Source1.Text = "Source: SQL Connection and Settings";
        //                cb_WindowAuthentication_SRC.Enabled = true;
        //                cb_WindowAuthentication_SRC.Checked = false;
        //                cb_SRC_DefaultPort.Enabled = false;
        //                cb_SRC_DefaultPort.Checked = false;
        //                tb_SRC_DefaultPort.Enabled = false;
        //                tb_SRC_DefaultPort.ReadOnly = true;
        //            }
        //        }
        //        else if (int_Group == 2)
        //        {
        //            if (rb_TRG_Oracle.Checked)
        //            {
        //                gb_Target1.Text = "Target : Oracle Connection and Settings";
        //                cb_WindowAuthentication_TRG.Enabled = false;
        //                cb_WindowAuthentication_TRG.Checked = false;
        //                cb_TRG_DefaultPort.Enabled = true;
        //                cb_TRG_DefaultPort.Checked = true;
        //                tb_TRG_DefaultPort.Enabled = true;
        //                tb_TRG_DefaultPort.ReadOnly = true;

        //            }
        //            else if (rb_TRG_SQL.Checked)
        //            {
        //                gb_Target1.Text = "Target : SQL Connection and Settings";
        //                cb_WindowAuthentication_TRG.Enabled = true;
        //                cb_WindowAuthentication_TRG.Checked = false;
        //                cb_TRG_DefaultPort.Enabled = false;
        //                cb_TRG_DefaultPort.Checked = false;
        //                tb_TRG_DefaultPort.Enabled = false;
        //                tb_TRG_DefaultPort.ReadOnly = true;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private bool CheckLengthOfTextBox(TextBox textBox)
        {
            try
            {
                if (!(textBox.Text.Length > 0))
                {
                    Common_Data.DisplayMessage(rtb_Log, $"{textBox.Name} is not validated", true);
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
                //if (rb_SRC_oracle.Checked)
                //{
                //    return (CheckLengthOfTextBox(tb_SRC_DefaultPort) && CheckLengthOfTextBox(tb_SRC_DataSource) && CheckLengthOfTextBox(tb_SRC_DBName) && CheckLengthOfTextBox(tb_SRC_UserName) && CheckLengthOfTextBox(tb_SRC_Password));
                //}
                //else if (rb_SRC_SQL.Checked)
                //{
                if (cb_WindowAuthentication_SRC.Checked)
                {
                    return (CheckLengthOfTextBox(tb_SRC_DataSource) && CheckLengthOfTextBox(tb_SRC_DBName));
                }
                else
                {
                    return (CheckLengthOfTextBox(tb_SRC_DataSource) && CheckLengthOfTextBox(tb_SRC_DBName) && CheckLengthOfTextBox(tb_SRC_UserName) && CheckLengthOfTextBox(tb_SRC_Password));
                }
                //}

                //if (rb_TRG_Oracle.Checked)
                //{
                //    return (CheckLengthOfTextBox(tb_TRG_DefaultPort) && CheckLengthOfTextBox(tb_TRG_DataSource) && CheckLengthOfTextBox(tb_TRG_DBName) && CheckLengthOfTextBox(tb_TRG_UserName) && CheckLengthOfTextBox(tb_TRG_Password));
                //}
                //else if (rb_TRG_SQL.Checked)
                //{
                //if (cb_WindowAuthentication_TRG.Checked)
                //{
                //    return (CheckLengthOfTextBox(tb_TRG_DataSource) && CheckLengthOfTextBox(tb_TRG_DBName));
                //}
                //else
                //{
                //    return (CheckLengthOfTextBox(tb_TRG_DataSource) && CheckLengthOfTextBox(tb_TRG_DBName) && CheckLengthOfTextBox(tb_TRG_UserName) && CheckLengthOfTextBox(tb_TRG_Password));
                //}
                //}

                //return true;
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
                return false;
            }
        }

        private Color GetStatusColor(bool boolStatus)
        {
            try
            {
                if (boolStatus)
                {
                    return Color.DarkBlue;
                }
                else
                {
                    return Color.Red;
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
                throw ex;
            }

        }

        private void btn_TestDBConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(ValidateMyControls()))
                {
                    return;
                }
                str_DBNameSRC = tb_SRC_DBName.Text.Trim();
                //str_DBNameTRG = tb_TRG_DBName.Text.Trim();

                //if (rb_SRC_oracle.Checked)
                //{
                //    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Oracle Source...");

                //    dbConnection = common_Data.DBConnectionTest(rb_SRC_oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBNameSRC, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_SRC_DefaultPort.Text.Trim());

                //}
                //else if (rb_SRC_SQL.Checked)
                //{
                Common_Data.DisplayMessage(rtb_Log, "Trying to connect to SQL Source...");
                dbConnection = common_Data.DBConnectionTest("sql", tb_SRC_DataSource.Text.Trim(), str_DBNameSRC, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), "1521");
                //}

                dbSRCConnection = dbConnection;
                boolSRCConnection = dbConnection.boolConnection;
                Common_Data.DisplayMessage(rtb_Log, $"Source : {dbConnection.strConnectionMSG}", (!dbConnection.boolConnection), -1, GetStatusColor(dbConnection.boolConnection));

                //if (rb_TRG_Oracle.Checked)
                //{
                //    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Oracle Target...");

                //    dbConnection = common_Data.DBConnectionTest(rb_TRG_Oracle.Text, tb_TRG_DataSource.Text.Trim(), str_DBNameTRG, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                //}
                //else if (rb_TRG_SQL.Checked)
                //{
                //    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to SQL Target...");
                //    dbConnection = common_Data.DBConnectionTest(rb_TRG_SQL.Text, tb_TRG_DataSource.Text.Trim(), str_DBNameTRG, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                //}

                //dbTRGConnection = dbConnection;
                //boolTRGConnection = dbConnection.boolConnection;
                //Common_Data.DisplayMessage(rtb_Log, $"Target : {dbConnection.strConnectionMSG}", (!dbConnection.boolConnection), -1, GetStatusColor(dbConnection.boolConnection));

                if (boolSRCConnection)
                {
                    boolLoadDataBase = true;
                }
                else
                {
                    boolLoadDataBase = false;
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        

        //private void rb_SRC_oracle_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (rb_SRC_oracle.Checked)
        //        {
        //            CheckChangeRadioButton(1);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common_Data.DisplayError(rtb_Log, ex);
        //    }
        //}

        //private void rb_SRC_SQL_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (rb_SRC_SQL.Checked)
        //        {
        //            CheckChangeRadioButton(1);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common_Data.DisplayError(rtb_Log, ex);
        //    }
        //}

        //private void rb_TRG_Oracle_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (rb_TRG_Oracle.Checked)
        //        {
        //            CheckChangeRadioButton(2);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common_Data.DisplayError(rtb_Log, ex);
        //    }
        //}

        //private void rb_TRG_SQL_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (rb_TRG_SQL.Checked)
        //        {
        //            CheckChangeRadioButton(2);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common_Data.DisplayError(rtb_Log, ex);
        //    }
        //}

        private void cb_WindowAuthentication_SRC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_WindowAuthentication_SRC.Checked)
                {
                    tb_SRC_UserName.Enabled = false;
                    tb_SRC_Password.Enabled = false;
                }
                else
                {
                    tb_SRC_UserName.Enabled = true;
                    tb_SRC_Password.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        //private void cb_SRC_DefaultPort_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cb_SRC_DefaultPort.Checked)
        //        {
        //            tb_SRC_DefaultPort.Text = "1521";
        //            tb_SRC_DefaultPort.Enabled = true;
        //            tb_SRC_DefaultPort.ReadOnly = true;
        //        }
        //        else
        //        {
        //            tb_SRC_DefaultPort.ResetText();
        //            tb_SRC_DefaultPort.Enabled = true;
        //            tb_SRC_DefaultPort.ReadOnly = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common_Data.DisplayError(rtb_Log, ex);
        //    }
        //}

        //private void cb_WindowAuthentication_TRG_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cb_WindowAuthentication_TRG.Checked)
        //        {
        //            tb_TRG_UserName.Enabled = false;
        //            tb_TRG_Password.Enabled = false;
        //        }
        //        else if (cb_WindowAuthentication_TRG.CheckState == CheckState.Unchecked)
        //        {
        //            tb_TRG_UserName.Enabled = true;
        //            tb_TRG_Password.Enabled = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common_Data.DisplayError(rtb_Log, ex);
        //    }
        //}

        //private void cb_TRG_DefaultPort_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (cb_TRG_DefaultPort.Checked)
        //        {
        //            tb_TRG_DefaultPort.Text = "1521";
        //            tb_TRG_DefaultPort.Enabled = true;
        //            tb_TRG_DefaultPort.ReadOnly = true;
        //        }
        //        else
        //        {
        //            tb_TRG_DefaultPort.ResetText();
        //            tb_TRG_DefaultPort.Enabled = true;
        //            tb_TRG_DefaultPort.ReadOnly = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common_Data.DisplayError(rtb_Log, ex);
        //    }
        //}

        //private void btn_Copy_SRC_TRG_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        rb_TRG_Oracle.Checked = rb_SRC_oracle.Checked;
        //        rb_TRG_SQL.Checked = rb_SRC_SQL.Checked;
        //        cb_WindowAuthentication_TRG.Checked = cb_WindowAuthentication_SRC.Checked;
        //        cb_TRG_DefaultPort.Checked = cb_SRC_DefaultPort.Checked;
        //        tb_TRG_DefaultPort.Text = tb_SRC_DefaultPort.Text;
        //        tb_TRG_DataSource.Text = tb_SRC_DataSource.Text;
        //        tb_TRG_DBName.Text = tb_SRC_DBName.Text;
        //        tb_TRG_UserName.Text = tb_SRC_UserName.Text;
        //        tb_TRG_Password.Text = tb_SRC_Password.Text;
        //    }
        //    catch (Exception ex)
        //    {
        //        Common_Data.DisplayError(rtb_Log, ex);
        //    }
        //}

        //private void btn_TRG_SRC_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        rb_SRC_oracle.Checked = rb_TRG_Oracle.Checked;
        //        rb_SRC_SQL.Checked = rb_TRG_SQL.Checked;
        //        cb_WindowAuthentication_SRC.Checked = cb_WindowAuthentication_TRG.Checked;
        //        cb_SRC_DefaultPort.Checked = cb_TRG_DefaultPort.Checked;
        //        tb_SRC_DefaultPort.Text = tb_TRG_DefaultPort.Text;
        //        tb_SRC_DataSource.Text = tb_TRG_DataSource.Text;
        //        tb_SRC_DBName.Text = tb_TRG_DBName.Text;
        //        tb_SRC_UserName.Text = tb_TRG_UserName.Text;
        //        tb_SRC_Password.Text = tb_TRG_Password.Text;
        //    }
        //    catch (Exception ex)
        //    {
        //        Common_Data.DisplayError(rtb_Log, ex);
        //    }
        //}

        private void btn_ClearLogs_Click(object sender, EventArgs e)
        {
            try
            {
                rtb_Log.Clear();
                Common_Data.DisplayMessage(rtb_Log, $"Logs Cleared");
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void cb_UserProfileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Common_Data.DisplayDebugMsg(rtb_Log, $"User Profile Selected : {cb_UserProfileList.SelectedItem}");

                dBConnectionDetails = new DBConnectionDetails(cb_UserProfileList.SelectedIndex);

                //rb_SRC_oracle.Checked = dBConnectionDetails.bool_Rb_SRC_Oracle;
                //rb_SRC_SQL.Checked = dBConnectionDetails.bool_Rb_SRC_SQL;

                cb_WindowAuthentication_SRC.Checked = dBConnectionDetails.bool_SRC_SQL_WA;
                //cb_SRC_DefaultPort.Checked = dBConnectionDetails.bool_SRC_Oracle_DefaultPort;
                //tb_SRC_DefaultPort.Text = dBConnectionDetails.bool_Rb_SRC_Oracle ? dBConnectionDetails.int_SRC_Oracle_DefaultPort.ToString() : string.Empty;

                tb_SRC_DataSource.Text = dBConnectionDetails.str_SRC_DataSource;
                tb_SRC_DBName.Text = dBConnectionDetails.str_SRC_DBName;
                tb_SRC_UserName.Text = dBConnectionDetails.str_SRC_UserName;
                tb_SRC_Password.Text = dBConnectionDetails.str_SRC_Password;
                //cb_SRC_TableName.Text = dBConnectionDetails.str_SRC_TableName;

                //rb_TRG_Oracle.Checked = dBConnectionDetails.bool_Rb_TRG_Oracle;
                //rb_TRG_SQL.Checked = dBConnectionDetails.bool_Rb_TRG_SQL;

                //cb_WindowAuthentication_TRG.Checked = dBConnectionDetails.bool_TRG_SQL_WA;
                //cb_TRG_DefaultPort.Checked = dBConnectionDetails.bool_TRG_Oracle_DefaultPort;
                //tb_TRG_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString();

                //tb_TRG_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                //tb_TRG_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                //tb_TRG_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                //tb_TRG_Password.Text = dBConnectionDetails.str_TRG_Password;
                //cb_TRG_TableName.Text = dBConnectionDetails.str_TRG_TableName;

                Common_Data.DisplayDebugMsg(rtb_Log, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
    }
}
