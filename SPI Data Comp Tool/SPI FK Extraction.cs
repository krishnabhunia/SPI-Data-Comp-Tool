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
        public List<DataColumn> ListCols;
        private DBConnectionDetails dBConnectionDetails;

        // The connection fields live in the shared ConnectionPanel; these keep the names the code below uses.
        private System.Windows.Forms.ComboBox cb_UserProfileList => connection.ProfileList;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_SRC => connection.Source.WindowsLogin;
        private System.Windows.Forms.TextBox tb_SRC_DataSource => connection.Source.DataSource;
        private System.Windows.Forms.TextBox tb_SRC_DBName => connection.Source.Database;
        private System.Windows.Forms.TextBox tb_SRC_UserName => connection.Source.User;
        private System.Windows.Forms.TextBox tb_SRC_Password => connection.Source.Password;
        private string str_DBNameSRC;
        private DBConnectionStatus dbConnection;
        private DBConnectionStatus dbSRCConnection;
        private bool boolSRCConnection;
        private string strSelectCol;
        private DataTable dtFetch;
        private DataTable dtResult;
        private string[] strRes;
        private FolderBrowserDialog folderBrowserDialog1;
        private SaveFileDialogFile saveFileDialogFile;

        public SPI_FK_Extraction()
        {
            InitializeComponent();
            connection.Mode = ConnectionMode.SourceOnly;
            connection.Source.Title = "Database";
            connection.SqlServerOnly = true;   // this window talks to SQL Server only
            connection.ShowProfile = false;
            connection.ProfileApplied += Connection_ProfileApplied;
            connection.Failed += (sender, error) => Common_Data.DisplayError(rtb_Log, error);
            UiTheme.Apply(this, btn_ImportExcel);

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
                    connection.ShowProfile = true;
                    common_Data.ReadUserProfile(cb_UserProfileList);
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        /// <summary>A saved profile was put into the Connection panel.</summary>
        private void Connection_ProfileApplied(object sender, DBConnectionDetails details)
        {
            dBConnectionDetails = details;
            Common_Data.DisplayDebugMsg(rtb_Log, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
        }

        private DataTable GetDataTable(DBConnectionStatus dBConnection, string strTableName)
        {
            try
            {

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
                // stop here when an input is missing or wrong: marks at the fields, reasons in the log
                if (!connection.AddChecks(InputCheck.For(this), target: false).Report()) return;
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
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        

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

    }
}
