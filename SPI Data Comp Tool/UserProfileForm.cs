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
    public partial class UserProfileForm : WorkspacePageForm
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

        private SaveFileDialogFile saveFileDialogFile;
        private Common_Data common_Data;
        public UserProfileForm()
        {
            InitializeComponent();
            connection.AllowNewProfileName = true;   // Save profile takes the name typed here
            connection.ProfileApplied += Connection_ProfileApplied;
            connection.Failed += (sender, error) => DisplayError(error);
            connection.InputChanged += (sender, e) =>
            {
                // A prior successful test never authorizes edited connection values.
                if (dBConnectionStatus_SRC != null) dBConnectionStatus_SRC.boolConnection = false;
                if (dBConnectionStatus_TRG != null) dBConnectionStatus_TRG.boolConnection = false;
            };
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

                string profilePath = Properties.Settings.Default.ProfilePathUser;
                if (!SelfTest.IsRunning && !string.IsNullOrWhiteSpace(profilePath) && File.Exists(profilePath))
                {
                    Profile.ProfileList = ReadProfileFile(profilePath);
                    connection.RefreshProfiles();
                    l_lb_ReadProfile.Text = Path.GetFileName(profilePath);
                    Common_Data.DisplayMessage(rtb_Log, $"User profile loaded from: {profilePath}");
                }
                else
                {
                    connection.RefreshProfiles();
                }
                GlobalDebug.ISGlobalDebug(GlobalDebug.strUserName, GlobalDebug.strPassword);
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
            strMsg = Common_Data.RedactConnectionCredentials(strMsg);
            try
            {
                rtb_Log.SelectionStart = rtb_Log.TextLength;
                rtb_Log.SelectionColor = UiTheme.Readable(color ?? Color.Black);   // readable in every colour theme

                if (boolError)
                {
                    rtb_Log.SelectionColor = UiTheme.Readable(Color.Red);
                }

                if (overwriteText > 0)
                {
                    var charcf0 = rtb_Log.Rtf.LastIndexOf("\\cf0");
                    if (charcf0 != -1)
                    {
                        var last = rtb_Log.Rtf.LastIndexOf("Processing", charcf0, 20);
                        rtb_Log.SelectionColor = UiTheme.Readable(Color.Blue);
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
                // stop here when an input is missing or wrong: marks at the fields, reasons in the log
                if (!connection.AddChecks(InputCheck.For(this), target: false).Report()) return;
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
                // stop here when an input is missing or wrong: marks at the fields, reasons in the log
                if (!connection.AddChecks(InputCheck.For(this), source: false).Report()) return;
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
                bool source = strFor == null || string.Equals(strFor, "SRC", StringComparison.OrdinalIgnoreCase);
                bool target = strFor == null || string.Equals(strFor, "TRG", StringComparison.OrdinalIgnoreCase);
                if (source) dBConnectionStatus_SRC = new DBConnectionStatus();
                if (target) dBConnectionStatus_TRG = new DBConnectionStatus();
                if (!FormValidation.Check(this, () => connection.ValidateInput(source, target),
                    message => DisplayMessage(message, true))) return;
                if (strFor == null || strFor.ToUpper() == "SRC")
                {
                    str_DBName = tb_SRC_DBName.Text.Trim();
                    //string str_SchemaName = tb_SchemaName.Text.Trim();

                    if (rb_SRC_oracle.Checked)
                    {
                        DisplayMessage("Trying to connect to Source Oracle...");

                        dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_SRC_oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text, tb_SRC_DefaultPort.Text.Trim());
                    }
                    else if (rb_SRC_SQL.Checked)
                    {
                        DisplayMessage("Trying to connect to Source SQL...");
                        dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_SRC_SQL.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text, tb_SRC_DefaultPort.Text.Trim());
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

                        dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_Oracle.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text, tb_TRG_DefaultPort.Text.Trim());
                    }
                    else if (rb_TRG_SQL.Checked)
                    {
                        DisplayMessage("Trying to connect to Target SQL...");
                        dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_SQL.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text, tb_TRG_DefaultPort.Text.Trim());
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
                    if (!FormValidation.Check(this, () => FormValidation.ReadableFile(openFileDialogData.strFileNameWithPath,
                        l_lb_ReadProfile, "profile", ".json"), message => DisplayMessage(message, true))) return;
                    DisplayMessage($"Reading json file : '{openFileDialogData.strFileNameWithPath}'");
                    // Reject an incomplete file before replacing the currently usable catalog.
                    List<DBC> imported = ReadProfileFile(openFileDialogData.strFileNameWithPath);
                    SetProfilePath(openFileDialogData.strFileNameWithPath);
                    Profile.ProfileList = imported;
                    connection.RefreshProfiles();

                    l_lb_ReadProfile.Text = openFileDialogData.SimpleFileName;
                    DisplayMessage($"Profile list loaded successfully from : {openFileDialogData.strFileNameWithPath}", false, -1, Color.DarkBlue);
                }
                else
                {
                    DisplayMessage("Profile import cancelled; current profiles are unchanged.");
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
                string profileName = cb_UserProfileList.Text.Trim();
                if (!FormValidation.Check(this, () =>
                {
                    FormValidation.Required(cb_UserProfileList, "profile name");
                    FormValidation.Require(!ConnectionSide.ContainsControlCharacters(profileName), cb_UserProfileList,
                        "Profile names cannot contain line breaks or control characters.");
                    connection.ValidateInput(true, true, !GlobalDebug.boolIsGlobalDebug);
                }, message => DisplayMessage(message, true))) return;
                ConnectToDB();

                if (GlobalDebug.boolIsGlobalDebug || (dBConnectionStatus_SRC.boolConnection && dBConnectionStatus_TRG.boolConnection))
                {
                    // Each save gets a new object. Reusing one instance mutates the profile saved
                    // by the previous click when a second profile is entered.
                    var replacement = new DBC
                    {
                        profileName = profileName,
                        dbSRC = GetDBConnectionDetailsProfile("SRC"),
                        dbTRG = GetDBConnectionDetailsProfile("TRG"),
                    };
                    List<DBC> candidate = ProfileStore.Upsert(Profile.ProfileList, replacement);

                    DisplayMessage($"Saving Profile, Pls wait !!!");

                    saveFileDialogFile = saveFileDialogFile.SaveFileDialogOnly("Save Profile in Json Format", $"Profile_{Environment.UserName}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}", "json");

                    if (saveFileDialogFile.BoolFileSaveStatus)
                    {
                        if (!FormValidation.Check(this, () => FormValidation.OutputFile(saveFileDialogFile.Str_saveFileNameWithPath,
                            btnSaveUserProfile, "profile output", ".json"), message => DisplayMessage(message, true))) return;
                        ProfileStore.WriteAtomic(saveFileDialogFile.Str_saveFileNameWithPath,
                            ProfileStore.SerializeForExport(candidate));
                        try
                        {
                            SetProfilePath(saveFileDialogFile.Str_saveFileNameWithPath);
                        }
                        catch (Exception ex)
                        {
                            DisplayMessage("The profile file was saved, but its path could not be remembered. The active profiles are unchanged; use Load profile to open the saved file. " + ex.Message, true);
                            return;
                        }
                        // The dialog was accepted and persistence succeeded: publish the new list.
                        Profile.ProfileList = candidate;
                        connection.RefreshProfiles(profileName, false);
                        l_lb_ReadProfile.Text = Path.GetFileName(saveFileDialogFile.Str_saveFileNameWithPath);
                        DisplayMessage($"Profile saved successfully: {saveFileDialogFile.Str_saveFileNameWithPath}; profile name: {profileName}");
                    }
                    else
                    {
                        DisplayMessage("Save cancelled; current profiles and the existing file are unchanged.");
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
                        // SQL profiles do not use an Oracle port; preserve the compatible unset sentinel.
                        int_Oracle_DefaultPort = rb_SRC_oracle.Checked ? FormValidation.Port(tb_SRC_DefaultPort, "Source Oracle port") : 0,
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
                        int_Oracle_DefaultPort = rb_TRG_Oracle.Checked ? FormValidation.Port(tb_TRG_DefaultPort, "Target Oracle port") : 0,
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
                string selectedName = cb_UserProfileList.SelectedItem as string;
                if (selectedName == null || Profile.ProfileList == null ||
                    !Profile.ProfileList.Any(p => p != null && string.Equals(p.profileName, selectedName, StringComparison.Ordinal)))
                {
                    DisplayMessage("Select a saved profile to delete.", true);
                    return;
                }
                string profilePath = Properties.Settings.Default.ProfilePathUser;
                if (string.IsNullOrWhiteSpace(profilePath) || !File.Exists(profilePath))
                {
                    DisplayMessage("Load or save a profile file before deleting a profile.", true);
                    return;
                }
                // Deleting from a stale catalog must not overwrite edits another window/process saved.
                List<DBC> currentFile = ReadProfileFile(profilePath);
                if (!string.Equals(ProfileStore.SerializeForExport(currentFile), ProfileStore.SerializeForExport(Profile.ProfileList), StringComparison.Ordinal))
                {
                    DisplayMessage("The profile file changed after it was loaded. Reload it before deleting a profile.", true);
                    return;
                }
                if (MessageBox.Show(this,
                    $"Delete profile '{selectedName}' from the saved profile file?\n\nThis cannot be undone in the application.",
                    "Delete saved profile", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    DisplayMessage("Deletion cancelled; current profiles are unchanged.");
                    return;
                }

                List<DBC> candidate = ProfileStore.Remove(Profile.ProfileList, selectedName);
                ProfileStore.WriteAtomic(profilePath, ProfileStore.SerializeForExport(candidate));
                Profile.ProfileList = candidate;
                connection.RefreshProfiles();
                DisplayMessage($"Profile deleted successfully: {selectedName}", false, -1, Color.DarkBlue);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private static void SetProfilePath(string path)
        {
            string previousPath = Properties.Settings.Default.ProfilePathUser;
            if (string.Equals(previousPath, path, StringComparison.Ordinal)) return;
            try
            {
                Properties.Settings.Default.ProfilePathUser = path;
                Properties.Settings.Default.Save();
            }
            catch
            {
                Properties.Settings.Default.ProfilePathUser = previousPath;
                throw;
            }
        }

        private static List<DBC> ReadProfileFile(string path)
        {
            if (!string.Equals(Path.GetExtension(path), ".json", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Select a JSON profile file.");
            var file = new FileInfo(path);
            if (file.Length == 0 || file.Length > 16 * 1024 * 1024)
                throw new InvalidDataException("Select a nonempty profile JSON file no larger than 16 MiB.");
            return ProfileStore.ParseJson(File.ReadAllText(path));
        }

        private void btn_Connect_Reference_ID_Click(object sender, EventArgs e)
        {
            try
            {
                // stop here when an input is missing or wrong: marks at the fields, reasons in the log
                if (!connection.AddChecks(InputCheck.For(this)).Report()) return;
                if (!FormValidation.Check(this, () => connection.ValidateInput(),
                    message => DisplayMessage(message, true))) return;
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
