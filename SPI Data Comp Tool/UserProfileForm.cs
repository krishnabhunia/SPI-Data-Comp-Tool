using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class UserProfileForm : Form
    {
        private string strQuery;
        private string str_DBName;
        private string strSQLConnection;
        private string strOracleConnection;

        private DBConnectionStatus dBConnectionStatus_SRC, dBConnectionStatus_TRG;
        private DBConnectionDetails dBConnectionDetails;
        private OpenFileDialogData openFileDialogData;
        private DBConnectionDetailsProfile dBConnectionDetailsProfile;

        private OracleDataAdapter oracleDA;
        private SqlDataAdapter sqlDA;

        private DataTable dt_oracle;
        private DataTable dt_LoadSRC;
        private DataTable dt_sql;
        private string str_tableName;

        private List<DBC> items;

        private SaveFileDialogFile saveFileDialogFile;
        private Common_Data common_Data;
        private DBC dbc_AddNewUserProfile;
        public UserProfileForm()
        {
            InitializeComponent();

            try
            {
                dBConnectionDetails = new DBConnectionDetails();
                dBConnectionStatus_SRC = new DBConnectionStatus();
                dBConnectionStatus_TRG = new DBConnectionStatus();
                openFileDialogData = new OpenFileDialogData();
                dBConnectionDetailsProfile = new DBConnectionDetailsProfile();
                saveFileDialogFile = new SaveFileDialogFile();
                common_Data = new Common_Data();
                dbc_AddNewUserProfile = new DBC();

                if(Properties.Settings.Default.ProfilePathUser != null & Properties.Settings.Default.ProfilePathUser != "")
                {
                    Common_Data.DisplayMessage(rtb_Log, $"User Profile Loaded from path : {Properties.Settings.Default.ProfilePathUser}");
                }

                cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);

                if (GlobalDebug.ISGlobalDebug(GlobalDebug.strUserName, GlobalDebug.strPassword))
                {
                    
                }

                if (cb_UserProfileList.Items.Count > 0)
                {
                    if(items == null || items != Profile.ProfileList)
                    {
                        items = Profile.ProfileList;
                    }
                    cb_UserProfileList.SelectedIndex = 0;
                }

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
                DisplayMessage($"User Profile Selected : {cb_UserProfileList.SelectedItem}", false);

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
                tb_TRG_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString(); ;

                tb_TRG_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                tb_TRG_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                tb_TRG_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                tb_TRG_Password.Text = dBConnectionDetails.str_TRG_Password;
                cb_TRG_TableName.Text = dBConnectionDetails.str_TRG_TableName;

                DisplayMessage($"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");

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
                DisplayMessage(ex.Message.ToString(), true);
                if (Debugger.IsAttached)
                {
                    DisplayMessage(ex.ToString(), true);
                }
            }
            catch (Exception ex1)
            {
                DisplayMessage(ex1.Message.ToString(), true);
                if (Debugger.IsAttached)
                {
                    DisplayMessage(ex1.ToString(), true);
                }
            }
        }

        private void DisplayMessage(string strMsg, bool boolError = false, int overwriteText = -1, Color? color = null)
        {
            try
            {
                rtb_Log.SelectionStart = rtb_Log.TextLength;
                rtb_Log.SelectionColor = color ?? Color.Black;

                if (boolError)
                {
                    rtb_Log.SelectionColor = Color.Red;
                }

                if (overwriteText > 0)
                {
                    var charcf0 = rtb_Log.Rtf.LastIndexOf("\\cf0");
                    if (charcf0 != -1)
                    {
                        var last = rtb_Log.Rtf.LastIndexOf("Processing", charcf0, 20);
                        rtb_Log.SelectionColor = Color.Blue;
                        if (last > 0)
                        {
                            rtb_Log.Rtf = rtb_Log.Rtf.Remove(last, charcf0 - last);
                            charcf0 = rtb_Log.Rtf.LastIndexOf("\\cf0");
                            rtb_Log.Rtf = rtb_Log.Rtf.Insert(charcf0, strMsg);
                        }
                        else
                        {
                            rtb_Log.AppendText($"\n{string.Format("{0:T}", DateTime.Now)} : {strMsg}");
                            rtb_Log.ScrollToCaret();
                        }
                    }
                }

                if (rtb_Log.TextLength > 0 && overwriteText == -1)
                {
                    rtb_Log.AppendText($"\n{string.Format("{0:T}", DateTime.Now)} : {strMsg}");
                    rtb_Log.ScrollToCaret();
                }
                else if (overwriteText == -1)
                {
                    rtb_Log.Text = $"{string.Format("{0:T}", DateTime.Now)} : {strMsg}";
                }

                rtb_Log.Update();

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void LoadTable(DBConnectionStatus DB_CS, ComboBox comboBox, CheckBox cbSQLWA)
        {
            try
            {
                DisplayMessage("Pls Wait, Loading !!!");

                if (DB_CS.strDBType.ToLower() == "oracle")
                {
                    strQuery = "SELECT * FROM all_tables where OWNER != 'SYS' ORDER BY OWNER, TABLE_NAME";

                    //strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                    //    "(HOST = " + tb_SRC_DataSource.Text + " )(PORT = " + tb_SRC_DefaultPort.Text + "))"
                    //    + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_SRC_DBName.Text + ")));"
                    //    + "User Id=" + tb_SRC_UserName.Text + ";Password=" + tb_SRC_Password.Text + ";";

                    strOracleConnection = DB_CS.strConnectionString;

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_oracle = new DataTable();
                    oracleDA.Fill(dt_oracle);
                    dt_oracle.TableName = "Load Table Name";
                    FillCheckListBox(comboBox, dt_oracle, "oracle");
                    dt_LoadSRC = dt_oracle.Copy();
                }

                //if (rb_SRC_SQL.Checked)
                if (DB_CS.strDBType.ToLower() == "sql")
                {
                    strQuery = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";

                    //if (cbSQLWA.CheckState == CheckState.Checked)
                    //{
                    //    strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    //+ "; Integrated Security = true ";
                    //}
                    //else if (cbSQLWA.CheckState == CheckState.Unchecked)
                    //{
                    //    strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    //+ "; User ID = " + tb_SRC_UserName.Text + "; " + "Password = " + tb_SRC_Password.Text + ";";
                    //}

                    strSQLConnection = DB_CS.strConnectionString;

                    dt_sql = new DataTable();
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_sql);

                    dt_sql.TableName = "Source SQL";
                    FillCheckListBox(comboBox, dt_sql, "sql");
                    dt_LoadSRC = dt_sql.Copy();
                }
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

        private void btn_Load_SRC_Table_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectToDB("SRC");
                if (dBConnectionStatus_SRC.boolConnection)
                {
                    DisplayMessage("Loading Table/Schema for Source");
                    LoadTable(dBConnectionStatus_SRC, cb_SRC_TableName, cb_WindowAuthentication_SRC);
                    DisplayMessage("Source Loaded Successfully", false, -1, Color.DarkBlue);
                }
                else
                {
                    cb_SRC_TableName.DataSource = null;
                }

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

        private void btn_Load_TRG_Table_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectToDB("TRG");
                if (dBConnectionStatus_TRG.boolConnection)
                {
                    DisplayMessage("Loading Table/Schema for Target");
                    LoadTable(dBConnectionStatus_TRG, cb_TRG_TableName, cb_WindowAuthentication_TRG);
                    DisplayMessage("Target Loaded Successfully", false, -1, Color.DarkBlue);
                }
                else
                {
                    cb_TRG_TableName.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
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
                        DisplayMessage("Trying to connect to Source Oracle...");

                        dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_SRC_oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_SRC_DefaultPort.Text.Trim());
                    }
                    else if (rb_SRC_SQL.Checked)
                    {
                        DisplayMessage("Trying to connect to Source SQL...");
                        dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_SRC_SQL.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_SRC_DefaultPort.Text.Trim());
                    }
                    DisplayMessage(dBConnectionStatus_SRC.strConnectionMSG, (!dBConnectionStatus_SRC.boolConnection), -1, Color.DarkBlue);

                    if (dBConnectionStatus_SRC.exceptionConnection != null)
                    {
                        DisplayError(dBConnectionStatus_SRC.exceptionConnection);
                    }

                }

                if (strFor == null || strFor.ToUpper() == "TRG")
                {
                    str_DBName = tb_TRG_DBName.Text.Trim();
                    //str_SchemaName = tb_SchemaName.Text.Trim();

                    if (rb_TRG_Oracle.Checked)
                    {
                        DisplayMessage("Trying to connect to Target Oracle...");

                        dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_Oracle.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                    }
                    else if (rb_TRG_SQL.Checked)
                    {
                        DisplayMessage("Trying to connect to Target SQL...");
                        dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_SQL.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                    }
                    DisplayMessage(dBConnectionStatus_TRG.strConnectionMSG, (!dBConnectionStatus_TRG.boolConnection), -1, Color.DarkBlue);
                    if (dBConnectionStatus_TRG.exceptionConnection != null)
                    {
                        DisplayError(dBConnectionStatus_TRG.exceptionConnection);
                    }
                }
            }
            catch (Exception ex)
            {
                dBConnectionStatus_SRC.boolConnection = false;
                dBConnectionStatus_TRG.boolConnection = false;
                DisplayError(ex);
            }
        }

        private void btn_ClearLogs_Click(object sender, EventArgs e)
        {
            try
            {
                rtb_Log.ResetText();
                DisplayMessage($"Logs Cleared");
                //string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"UserProfile\UserProfile.json");
                //string[] files = File.ReadAllLines(path);
                //DisplayMessage(path);
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
                DisplayMessage($"Data copied from source to target");
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
                DisplayMessage($"Data copied from target to source");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void rb_SRC_oracle_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_SRC_oracle.Checked)
            {
                CheckChangeRadioButton(1);
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
                        DisplayMessage($"Source Oracle Selected");
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
                        DisplayMessage($"Source SQL Selected");
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
                        DisplayMessage($"Target Oracle Selected");
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
                        DisplayMessage($"Target SQL Selected");
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

        private void cb_WindowAuthentication_SRC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_WindowAuthentication_SRC.Checked)
                {
                    DisplayMessage($"Source WA Selected");
                    tb_SRC_UserName.Enabled = false;
                    tb_SRC_Password.Enabled = false;
                }
                else if (cb_WindowAuthentication_SRC.CheckState == CheckState.Unchecked)
                {
                    DisplayMessage($"Source WA Not Selected");
                    tb_SRC_UserName.Enabled = true;
                    tb_SRC_Password.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_WindowAuthentication_TRG_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_WindowAuthentication_TRG.Checked)
                {
                    DisplayMessage($"Target WA Selected");
                    tb_TRG_UserName.Enabled = false;
                    tb_TRG_Password.Enabled = false;
                }
                else if (cb_WindowAuthentication_TRG.CheckState == CheckState.Unchecked)
                {
                    DisplayMessage($"Target WA Not Selected");
                    tb_TRG_UserName.Enabled = true;
                    tb_TRG_Password.Enabled = true;
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
                if (cb_SRC_DefaultPort.Checked)
                {
                    DisplayMessage($"Source DefaultPort Selected");
                    tb_SRC_DefaultPort.Text = "1521";
                    tb_SRC_DefaultPort.Enabled = true;
                    tb_SRC_DefaultPort.ReadOnly = true;
                }
                else
                {
                    DisplayMessage($"Source DefaultPort Not Selected");
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

        private void cb_TRG_DefaultPort_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_TRG_DefaultPort.Checked)
                {
                    DisplayMessage($"Target DefaultPort Selected");
                    tb_TRG_DefaultPort.Text = "1521";
                    tb_TRG_DefaultPort.Enabled = true;
                    tb_TRG_DefaultPort.ReadOnly = true;
                }
                else
                {
                    DisplayMessage($"Target DefaultPort Not Selected");
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

        private void l_lb_ReadProfile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                DisplayMessage($"Reading File...");
                openFileDialogData = openFileDialogData.openFileDialog("User Profile", "json");
                if (openFileDialogData.boolFileSelected)
                {
                    DisplayMessage($"Reading json file : '{openFileDialogData.strFileNameWithPath}'");
                    //StreamReader streamReader = new StreamReader(openFileDialogData.strFileNameWithPath);
                    //JsonTextReader jsonTextReader = new JsonTextReader(streamReader);
                    using (StreamReader r = new StreamReader(openFileDialogData.strFileNameWithPath))
                    {
                        string json = r.ReadToEnd();
                        items = JsonConvert.DeserializeObject<List<DBC>>(json);
                    }

                    Profile.ProfileList = items;

                    Properties.Settings.Default.ProfilePathUser = $@"{openFileDialogData.strFileNameWithPath}";
                    Properties.Settings.Default.Save();

                    //string str = Properties.Settings.Default.ProfilePathUser;

                    cb_UserProfileList.Items.Clear();

                    foreach (var i in items)
                    {
                        if (i.profileName != null)
                        {
                            cb_UserProfileList.Items.Add(i.profileName.ToString());
                        }
                    }
                    if (cb_UserProfileList.Items.Count > 0)
                    {
                        cb_UserProfileList.SelectedIndex = 0;
                    }

                    l_lb_ReadProfile.Text = openFileDialogData.SimpleFileName;
                    DisplayMessage($"Profile list loaded successfully from : {openFileDialogData.strFileNameWithPath}", false, -1, Color.DarkBlue);
                }
                else
                {
                    DisplayMessage($"No File Selected To Load User Profile", true);
                    l_lb_ReadProfile.Text = "No File Selected To Load User Profile, Pls click again to load User Profile";
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void boolToggleControlEnable(bool boolEnableStatus)
        {
            try
            {
                groupBox14.Enabled = boolEnableStatus;
                groupBox1.Enabled = boolEnableStatus;
                groupBox2.Enabled = boolEnableStatus;
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void btnSaveUserProfile_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectToDB();

                if (GlobalDebug.boolIsGlobalDebug || (dBConnectionStatus_SRC.boolConnection && dBConnectionStatus_TRG.boolConnection))
                {
                    dbc_AddNewUserProfile.profileName = cb_UserProfileList.Text;
                    dbc_AddNewUserProfile.dbSRC = GetDBConnectionDetailsProfile("SRC");
                    dbc_AddNewUserProfile.dbTRG = GetDBConnectionDetailsProfile("TRG");

                    if (items != null)
                    {
                        foreach (var item in items.ToArray())
                        {
                            if (item.profileName == dbc_AddNewUserProfile.profileName)
                            {
                                items.Remove(item);
                            }
                        }
                        items.Add(dbc_AddNewUserProfile);
                    }
                    else
                    {
                        items = new List<DBC>();
                        items.Add(dbc_AddNewUserProfile);
                    }

                    DisplayMessage($"Saving Profile, Pls wait !!!");

                    saveFileDialogFile = saveFileDialogFile.SaveFileDialogOnly("Save Profile in Json Format", $"Profile_{Environment.UserName}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}", "json");

                    if (saveFileDialogFile.BoolFileSaveStatus)
                    {
                        File.WriteAllText(saveFileDialogFile.Str_saveFileNameWithPath, JsonConvert.SerializeObject(items, Formatting.Indented));

                        Properties.Settings.Default.ProfilePathUser = $@"{saveFileDialogFile.Str_saveFileNameWithPath}";
                        Properties.Settings.Default.Save();

                        DisplayMessage($"Profile Saved Successfully :{saveFileDialogFile.Str_saveFileNameWithPath},Profile Name as : {dbc_AddNewUserProfile.profileName}");
                    }
                    else
                    {
                        DisplayMessage($"File Not Saved By User", true);
                    }
                }
                else
                {
                    DisplayMessage($"Either Source or Target connection has problem, pls check all settings", true);
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private DBConnectionDetailsProfile GetDBConnectionDetailsProfile(string strFor)
        {
            try
            {
                if (strFor.ToUpper() == "SRC")
                {
                    dBConnectionDetailsProfile = new DBConnectionDetailsProfile
                    {
                        bool_Oracle_DefaultPort = cb_SRC_DefaultPort.Checked,
                        bool_Rb_Oracle = rb_SRC_oracle.Checked,
                        bool_Rb_SQL = rb_SRC_SQL.Checked,
                        bool_SQL_WA = cb_WindowAuthentication_SRC.Checked,
                        int_Oracle_DefaultPort = tb_SRC_DefaultPort.Text == "" ? 1521 : Convert.ToInt32(tb_SRC_DefaultPort.Text),
                        str_DataSource = tb_SRC_DataSource.Text,
                        str_DBName = tb_SRC_DBName.Text,
                        str_DBType = rb_SRC_oracle.Checked ? "Oracle" : "SQL",
                        str_SchemaName = cb_SRC_TableName.Text,
                        str_TableName = cb_SRC_TableName.Text,
                        str_UserName = tb_SRC_UserName.Text,
                        str_Password = GlobalDebug.boolIsGlobalDebug ? tb_SRC_Password.Text : ""
                    };
                }
                else if (strFor.ToUpper() == "TRG")
                {
                    dBConnectionDetailsProfile = new DBConnectionDetailsProfile
                    {
                        bool_Oracle_DefaultPort = cb_TRG_DefaultPort.Checked,
                        bool_Rb_Oracle = rb_TRG_Oracle.Checked,
                        bool_Rb_SQL = rb_TRG_SQL.Checked,
                        bool_SQL_WA = cb_WindowAuthentication_TRG.Checked,
                        int_Oracle_DefaultPort = tb_TRG_DefaultPort.Text == "" ? 1521 : Convert.ToInt32(tb_TRG_DefaultPort.Text),
                        str_DataSource = tb_TRG_DataSource.Text,
                        str_DBName = tb_TRG_DBName.Text,
                        str_DBType = rb_TRG_Oracle.Checked ? "Oracle" : "SQL",
                        str_SchemaName = cb_TRG_TableName.Text,
                        str_TableName = cb_TRG_TableName.Text,
                        str_UserName = tb_TRG_UserName.Text,
                        str_Password = GlobalDebug.boolIsGlobalDebug ? tb_TRG_Password.Text : ""
                    };
                }
                return dBConnectionDetailsProfile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDeleteUserProfile_Click(object sender, EventArgs e)
        {
            try
            {
                string strDeleteProfileName = Common_Data.GetComboBoxValue(cb_UserProfileList);
                DisplayMessage($"Deleting Profile :{strDeleteProfileName}");
                foreach (var item in items.ToArray())
                {
                    if (item.profileName.Trim() == strDeleteProfileName)
                    {
                        items.Remove(item);
                        DisplayMessage($"Profile Deleted SuccessFully : {strDeleteProfileName}", false, -1, Color.DarkBlue);
                    }
                }

                cb_UserProfileList.Items.Clear();

                foreach (var i in items)
                {
                    if (i.profileName != null)
                    {
                        cb_UserProfileList.Items.Add(i.profileName.ToString());
                    }
                }
                if (cb_UserProfileList.Items.Count > 0)
                {
                    cb_UserProfileList.SelectedIndex = 0;
                }

                DisplayMessage($"Profile reloaded after deletion");

                if (Common_Data.StringNotNullOrEmpty(Properties.Settings.Default.ProfilePathUser, ".json"))
                {
                    DisplayMessage($"Saving Profile in {Properties.Settings.Default.ProfilePathUser}");
                    Profile.ProfileList = items;
                    File.WriteAllText(Properties.Settings.Default.ProfilePathUser, JsonConvert.SerializeObject(items, Formatting.Indented));
                    DisplayMessage($"Profile saved after deletion", false, -1, Color.DarkBlue);
                }
                else
                {
                    DisplayMessage($"Some problem in saving the profile", true);
                }

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
                boolToggleControlEnable(false);
                ConnectToDB();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                boolToggleControlEnable(true);
            }
        }
    }
}
