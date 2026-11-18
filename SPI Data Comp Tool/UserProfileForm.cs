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

        // The connection fields live in the shared ConnectionPanel; these keep the names the code below uses.
        private System.Windows.Forms.ComboBox cb_UserProfileList => connection.ProfileList;
        private System.Windows.Forms.RadioButton rb_SRC_oracle => connection.Source.Oracle;
        private System.Windows.Forms.RadioButton rb_SRC_SQL => connection.Source.Sql;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_SRC => connection.Source.WindowsLogin;
        private System.Windows.Forms.CheckBox cb_SRC_DefaultPort => connection.Source.DefaultPort;
        private System.Windows.Forms.TextBox tb_SRC_DefaultPort => connection.Source.Port;
        private System.Windows.Forms.TextBox tb_SRC_DataSource => connection.Source.DataSource;
        private System.Windows.Forms.TextBox tb_SRC_DBName => connection.Source.Database;
        private System.Windows.Forms.TextBox tb_SRC_UserName => connection.Source.User;
        private System.Windows.Forms.TextBox tb_SRC_Password => connection.Source.Password;
        private System.Windows.Forms.RadioButton rb_TRG_Oracle => connection.Target.Oracle;
        private System.Windows.Forms.RadioButton rb_TRG_SQL => connection.Target.Sql;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_TRG => connection.Target.WindowsLogin;
        private System.Windows.Forms.CheckBox cb_TRG_DefaultPort => connection.Target.DefaultPort;
        private System.Windows.Forms.TextBox tb_TRG_DefaultPort => connection.Target.Port;
        private System.Windows.Forms.TextBox tb_TRG_DataSource => connection.Target.DataSource;
        private System.Windows.Forms.TextBox tb_TRG_DBName => connection.Target.Database;
        private System.Windows.Forms.TextBox tb_TRG_UserName => connection.Target.User;
        private System.Windows.Forms.TextBox tb_TRG_Password => connection.Target.Password;
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
            connection.AllowNewProfileName = true;   // Save profile takes the name typed here
            connection.ProfileApplied += Connection_ProfileApplied;
            connection.Failed += (sender, error) => DisplayError(error);
            UiTheme.Apply(this, btnSaveUserProfile);

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

                common_Data.ReadUserProfile(cb_UserProfileList);

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

        /// <summary>A saved profile was put into the Connection panel: take over its table names.</summary>
        private void Connection_ProfileApplied(object sender, DBConnectionDetails details)
        {
            dBConnectionDetails = details;
            cb_SRC_TableName.Text = details.str_SRC_TableName;
            cb_TRG_TableName.Text = details.str_TRG_TableName;
            DisplayMessage($"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
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
                string str_lastName = FormHelpers.FillTableNames(comboBox, dataTable, str_dbType);
                if (str_lastName != null)
                {
                    str_tableName = str_lastName;
                }
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
                        // Never persist database passwords in an exportable JSON profile.
                        str_Password = ""
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
                        // Always require the password to be entered again when needed.
                        str_Password = ""
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
