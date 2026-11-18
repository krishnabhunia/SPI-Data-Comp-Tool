using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class Table_Extraction_After_Delta_Form : Form
    {
        private Common_Data common_Data;
        private DBConnectionDetails dBConnectionDetails;

        // The connection fields live in the shared ConnectionPanel; these keep the names the code below uses.
        private System.Windows.Forms.ComboBox cb_UserProfileList => connection.ProfileList;
        private System.Windows.Forms.RadioButton rb_SRC_oracle => connection.Source.Oracle;
        private System.Windows.Forms.RadioButton rb_SRC_SQL => connection.Source.Sql;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_SRC => connection.Source.WindowsLogin;
        private System.Windows.Forms.TextBox tb_SRC_DefaultPort => connection.Source.Port;
        private System.Windows.Forms.TextBox tb_SRC_DataSource => connection.Source.DataSource;
        private System.Windows.Forms.TextBox tb_SRC_DBName => connection.Source.Database;
        private System.Windows.Forms.TextBox tb_SRC_UserName => connection.Source.User;
        private System.Windows.Forms.TextBox tb_SRC_Password => connection.Source.Password;
        private System.Windows.Forms.RadioButton rb_TRG_Oracle => connection.Target.Oracle;
        private System.Windows.Forms.RadioButton rb_TRG_SQL => connection.Target.Sql;
        private System.Windows.Forms.CheckBox cb_WindowAuthentication_TRG => connection.Target.WindowsLogin;
        private System.Windows.Forms.TextBox tb_TRG_DefaultPort => connection.Target.Port;
        private System.Windows.Forms.TextBox tb_TRG_DataSource => connection.Target.DataSource;
        private System.Windows.Forms.TextBox tb_TRG_DBName => connection.Target.Database;
        private System.Windows.Forms.TextBox tb_TRG_UserName => connection.Target.User;
        private System.Windows.Forms.TextBox tb_TRG_Password => connection.Target.Password;

        private DBConnectionStatus dBConnectionStatus_SRC, dBConnectionStatus_TRG;
        private bool boolSRCConnection, boolTRGConnection;

        private OpenFileDialogData openFileDialogData;

        private DataTable dtTableList, dtResult;
        private string str_SelectQuery;
        private SqlDataAdapter sqlDA;
        private SaveFileDialogFile saveFileDialogFile;

        private string strSRC_TBName, strPK, strTableID;
        private int int_Percentage, int_Count_TableRow;

        private FolderBrowserDialog folderBrowserDialog;
        private string strFolderPath, strFileNameWithPath;
        private bool boolTRG_Folder;

        private DateTime start_Time;
        private TimeSpan time_to_execute;

        private string strSRCTypeSelected, strTRGTypeSelected;

        private BackGroundWorkerUserObjectForReport backGroundWorkerObject;
        private int timer2Count;
        private bool boolID;
        private string strSRC_Select, strTRG_Select, strTRG_TBName;
        private DataTable dtSRC, dtTRG, dtMergeProtocol, dtJoin;

        private bool boolFolderBrowserDialog;
        private DataSet dsTableList;

        private bool boolTableChangeFlag;
        private int int_Count_Table;
        private string strTable_List_TRG;
        private string strColNameSRC, strColNameTRG;
        private OpenFileDialogData openFileDialogDataExcel;
        private DataSet dsExcel;
        private DataTable dtExcel;

        public Table_Extraction_After_Delta_Form()
        {
            InitializeComponent();
            connection.ProfileApplied += Connection_ProfileApplied;
            connection.Failed += (sender, error) => Common_Data.DisplayError(rtb_Log, error);
            UiTheme.Apply(this);

            try
            {
                dBConnectionDetails = new DBConnectionDetails();
                common_Data = new Common_Data();
                dBConnectionStatus_SRC = new DBConnectionStatus();
                dBConnectionStatus_TRG = new DBConnectionStatus();
                boolSRCConnection = false;
                boolTRGConnection = false;
                openFileDialogData = new OpenFileDialogData();

                dtResult = new DataTable();
                dtTableList = new DataTable();
                saveFileDialogFile = new SaveFileDialogFile();
                folderBrowserDialog = new FolderBrowserDialog();
                common_Data.ReadUserProfile(cb_UserProfileList);
                displayElapsedTime(true);

                dtSRC = new DataTable();
                dtTRG = new DataTable();
                dtMergeProtocol = new DataTable();
                dtJoin = new DataTable();

                timer2Count = 1;
                boolID = boolFolderBrowserDialog = false;
                dsTableList = new DataSet();
                boolTableChangeFlag = false;
                int_Count_Table = 0;
                openFileDialogDataExcel = new OpenFileDialogData();
                dsExcel = new DataSet();
                dtExcel = new DataTable();

                if (GlobalDebug.ISGlobalDebug(GlobalDebug.strUserName, GlobalDebug.strPassword) || Debugger.IsAttached)
                {
                    int userProfileMembersCount = 11;
                    if (cb_UserProfileList.Items.Count > userProfileMembersCount)
                    {
                        cb_UserProfileList.SelectedIndex = userProfileMembersCount;
                        ConnectDB();
                    }
                    else if (cb_UserProfileList.Items.Count > 0)
                    {
                        cb_UserProfileList.SelectedIndex = 0;
                        ConnectDB();
                    }
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        /// <summary>A saved profile was put into the Connection panel: take over its table names.</summary>
        private void Connection_ProfileApplied(object sender, DBConnectionDetails details)
        {
            dBConnectionDetails = details;
            Common_Data.DisplayMessage(rtb_Log, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
        }

        private void btn_TestDBConnection_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectDB();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void ConnectDB()
        {
            try
            {
                string str_DBName = tb_SRC_DBName.Text.Trim();

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

                if (rb_TRG_Oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Target Oracle...");

                    dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_Oracle.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                }
                else if (rb_TRG_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Target SQL...");
                    dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_SQL.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                }
                Common_Data.DisplayMessage(rtb_Log, dBConnectionStatus_TRG.strConnectionMSG, (!dBConnectionStatus_TRG.boolConnection), -1, Color.DarkBlue);

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }






        private void displayElapsedTime(bool boolInitialize = false)
        {
            try
            {
                if (boolInitialize)
                {
                    start_Time = DateTime.Now;
                    UpdateProgressOnly(0, 1);
                    timer2Count = 1;
                }
                else
                {
                    UpdateProgressOnly(timer2Count++, 100);
                    if (timer2Count > 99)
                    {
                        timer2Count = 1;
                    }
                }

                time_to_execute = DateTime.Now.Subtract(start_Time);
                lblTimer.Text = FormHelpers.FormatElapsed(time_to_execute);
                lblTimer.Update();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
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
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void BGW_ProgressChanged(ProgressChangedEventArgs e)
        {
            try
            {
                //0 means only msg
                //1 means progress count
                //2 means DebugMsg
                //3 means red color msg or not satisfying condition
                //4 else conditions
                //5 error condition thrown to catch

                backGroundWorkerObject = (BackGroundWorkerUserObjectForReport)e.UserState;

                if (e.ProgressPercentage == 0)
                {
                    Common_Data.DisplayMessage(rtb_Log, backGroundWorkerObject.strMsg);
                }

                if (e.ProgressPercentage == 1)
                {
                    UpdateProgress(backGroundWorkerObject.int_Count, backGroundWorkerObject.int_TotalCount);
                    Common_Data.DisplayMessage(rtb_Log, backGroundWorkerObject.strMsg);
                }

                if (e.ProgressPercentage == 2)
                {
                    Common_Data.DisplayDebugMsg(rtb_Log, backGroundWorkerObject.strMsg);
                }

                if (e.ProgressPercentage == 3)
                {
                    Common_Data.DisplayMessage(rtb_Log, backGroundWorkerObject.strMsg, true);
                }

                if (e.ProgressPercentage == 4)
                {
                    UpdateProgressOnly(backGroundWorkerObject.int_Count, backGroundWorkerObject.int_TotalCount);
                    Common_Data.DisplayMessage(rtb_Log, backGroundWorkerObject.strMsg, true);
                }

                if (e.ProgressPercentage == 5 || backGroundWorkerObject.bgwException != null || backGroundWorkerObject.boolError)
                {
                    Common_Data.DisplayError(rtb_Log, backGroundWorkerObject.bgwException);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void btnOLDvsNew_Click(object sender, EventArgs e)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, $"Old Versur New Selected");

                strSRCTypeSelected = connection.Source.DatabaseType;
                strTRGTypeSelected = connection.Target.DatabaseType;

                openFileDialogData = openFileDialogData.openFileDialog("Browse Excel File For OLD Versus New");

                if (openFileDialogData.boolFileSelected)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Reading Data : {openFileDialogData.strFileNameWithPath}");

                    folderBrowserDialog = Common_Data.browserDialogInitialisation("Select Folder To Save Excel For OLD Versus NEW", $@"{openFileDialogData.PathName_withoutFileName}\Output");

                    boolFolderBrowserDialog = false;

                    if (Debugger.IsAttached)
                    {
                        folderBrowserDialog.SelectedPath = @"D:\Krishna\Projects\SPI\INPUT FILES\Old Versus New\Prasad K\Output";
                        boolFolderBrowserDialog = true;
                    }
                    else if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        boolFolderBrowserDialog = true;
                    }
                    else
                    {
                        boolFolderBrowserDialog = false;
                    }

                    if (boolFolderBrowserDialog)
                    {
                        dsTableList = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);
                        dtTableList = dsTableList.Tables[0];

                        strFolderPath = folderBrowserDialog.SelectedPath;
                        Common_Data.DisplayMessage(rtb_Log, $"Saving path selected : {strFolderPath}");

                        if (!(bgw_old_VS_new.IsBusy))
                        {
                            timer1.Enabled = true;
                            start_Time = DateTime.Now;
                            bgw_old_VS_new.RunWorkerAsync();
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, $"Already its running", true);
                        }
                    }
                    else
                    {
                        strFolderPath = string.Empty;
                        Common_Data.DisplayMessage(rtb_Log, $"To save folder path not set, aborting all operation", true);
                        boolTRG_Folder = false;
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"User cancelled reading file !!!", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void btn_Chg_Num_Click(object sender, EventArgs e)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, $"CHG_NUM Versus Columns Selected");

                strSRCTypeSelected = connection.Source.DatabaseType;
                strTRGTypeSelected = connection.Target.DatabaseType;

                openFileDialogData = openFileDialogData.openFileDialog("Browse Excel File For Change Versus Num");

                if (openFileDialogData.boolFileSelected)
                {
                    //if (cb_CHG_NUM_From_Excel.Checked)
                    //{
                    //    openFileDialogDataExcel = openFileDialogDataExcel.openFileDialog("Browse Excel File For Change Versus Num For SOURCE");
                    //}

                    //if (openFileDialogDataExcel.boolFileSelected)
                    //{
                    //    dtSRC = ReadExcel.LoadExcelFromDataReader(openFileDialogDataExcel.strFileNameWithPath).Tables[0];
                    //}

                    if (openFileDialogDataExcel.boolFileSelected || (!cb_CHG_NUM_From_Excel.Checked))
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"Reading Data : {openFileDialogData.strFileNameWithPath}");

                        folderBrowserDialog = Common_Data.browserDialogInitialisation("Select Folder To Save Excel For CHG_NUM", $@"{openFileDialogData.PathName_withoutFileName}\Output");

                        boolFolderBrowserDialog = false;

                        if (Debugger.IsAttached)
                        {
                            folderBrowserDialog.SelectedPath = @"D:\Krishna\Projects\SPI\INPUT FILES\Old Versus New\Prasad K\Output";
                            boolFolderBrowserDialog = true;
                        }
                        else if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            boolFolderBrowserDialog = true;
                        }
                        else
                        {
                            boolFolderBrowserDialog = false;
                        }

                        if (boolFolderBrowserDialog)
                        {
                            dsTableList = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);
                            dtTableList = dsTableList.Tables[0];

                            strFolderPath = folderBrowserDialog.SelectedPath;
                            Common_Data.DisplayMessage(rtb_Log, $"Saving path selected : {strFolderPath}");

                            if (!(bgw_Chg_Num.IsBusy))
                            {
                                timer1.Enabled = true;
                                start_Time = DateTime.Now;
                                bgw_Chg_Num.RunWorkerAsync();
                            }
                            else
                            {
                                Common_Data.DisplayMessage(rtb_Log, $"Already its running", true);
                            }
                        }
                        else
                        {
                            strFolderPath = string.Empty;
                            Common_Data.DisplayMessage(rtb_Log, $"To save folder path not set, aborting all operation", true);
                            boolTRG_Folder = false;
                        }
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"Source Excel File Not Selected, Cann't proceed further", true);
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"User cancelled reading file !!!", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }


        private void bgw_Chg_Num_DoWork(object sender, DoWorkEventArgs e)
        {
            bool boolSourceLoad = false;
            try
            {
                int_Count_TableRow = 0;
                dtResult.Reset();

                foreach (DataRow dr in dsTableList.Tables[0].Rows)
                {
                    if (bgw_Chg_Num.CancellationPending)
                    {
                        e.Cancel = true;
                        bgw_Chg_Num.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                        return;
                    }

                    strSRC_TBName = dr["src_table_name"].ToString().Trim().ToUpper();
                    strTRG_TBName = dr["trg_table_name"].ToString().Trim().ToUpper();
                    strColNameSRC = dr["src_column_name"].ToString().Trim().ToUpper();
                    strColNameTRG = dr["trg_column_name"].ToString().Trim().ToUpper();
                    strTableID = dr["table_id"].ToString().Trim().ToUpper();

                    try
                    {
                        strTRG_Select = $"SELECT * FROM {strTRG_TBName}";

                        bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Fetching Source and Target Table For : {strSRC_TBName} and {strTRG_TBName} respectively."));

                        dtTRG.Reset();
                        dtTRG = common_Data.FillDataTable(dtTRG, strTRGTypeSelected, dBConnectionStatus_TRG.strConnectionString, strTRG_Select, strTRG_TBName);

                        var distinctID = dtTRG.AsEnumerable().Select(x => x["CHG_NUM"]).Distinct();
                        string strPK_ID_TRG = string.Join(", ", distinctID.Select(x => string.Concat("'", x.ToString(), "'")));

                        boolID = false;
                        if (strPK_ID_TRG.Length > 0)
                        {
                            strSRC_Select = $"SELECT * FROM {strSRC_TBName} WHERE {strColNameSRC} IN ({strPK_ID_TRG})";
                            boolID = true;
                        }

                        if (boolID)
                        {
                            if (bgw_Chg_Num.CancellationPending)
                            {
                                e.Cancel = true;
                                bgw_Chg_Num.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                                return;
                            }

                            if (!(cb_CHG_NUM_From_Excel.Checked) && (!boolSourceLoad))
                            {
                                dtSRC.Reset();
                                dtSRC = common_Data.FillDataTable(dtSRC, strSRCTypeSelected, dBConnectionStatus_SRC.strConnectionString, strSRC_Select, strSRC_TBName);
                                boolSourceLoad = true;
                            }

                            if (dtSRC.Columns.Contains("CHG_NUM"))
                            {
                                dtSRC.Columns[strColNameSRC].SetOrdinal(dtSRC.Columns["CHG_NUM"].Ordinal);
                            }

                            if (dtSRC.Columns.Contains(strColNameSRC))
                            {
                                dtSRC.Columns[strColNameSRC].Caption = $"{strColNameSRC}_OLD";
                                dtSRC.Columns[strColNameSRC].ColumnName = $"{strColNameSRC}_OLD";
                            }

                            if (dtSRC.Columns.Contains("CHG_NUM"))
                            {
                                dtSRC.Columns["CHG_NUM"].Caption = $"{strColNameSRC}_NEW";
                                dtSRC.Columns["CHG_NUM"].ColumnName = $"{strColNameSRC}_NEW";
                                dtSRC.Columns[$"{strColNameSRC}_NEW"].SetOrdinal(dtSRC.Columns[$"{strColNameSRC}_OLD"].Ordinal);
                            }
                            else
                            {
                                dtSRC = common_Data.AddColumns(dtSRC, $"{strColNameSRC}_NEW", typeof(object));
                                dtSRC.Columns[$"{strColNameSRC}_NEW"].SetOrdinal(dtSRC.Columns[$"{strColNameSRC}_OLD"].Ordinal + 1);
                            }

                            bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Fetched all tables successfully and preprocess started ..."));

                            var varResult = (from s in dtSRC.AsEnumerable()
                                             join t in dtTRG.AsEnumerable() on s.Field<object>($"{strColNameSRC}_OLD").ToString().Trim() equals t.Field<object>("CHG_NUM").ToString().Trim()
                                             select new { s, t }).ToList();

                            bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Preprocess finished and process started ..."));

                            varResult.ForEach(x => x.s.SetField($"{strColNameSRC}_NEW", x.t.Field<object>($"{strColNameTRG}")));

                            #region Fixing for zeros entries

                            bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Fixing of zero entries started ... "));

                            varResult.Where(x => x.s.Field<object>($"{strColNameSRC}_OLD").ToString().Trim().Equals("0")).ToList().ForEach(x => x.s.SetField($"{strColNameSRC}_NEW", x.s.Field<object>($"{strColNameTRG}_OLD")));

                            bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Fixing of zero entries completed successfully"));

                            #endregion 

                            varResult.Take(2).ToList().ForEach(x => bgw_Chg_Num.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"UPDATE {dtSRC.TableName} SET {strColNameSRC}_NEW = '{x.t.Field<object>($"{strColNameTRG}")}' WHERE CHG_NUM = '{x.s.Field<object>($"{strColNameSRC}_OLD")}'")));

                            bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Resultant table processed successfully"));

                            dtResult.Reset();
                            dtResult = dtSRC.Copy();

                            #region Temporary save only for debug

                            if(GlobalDebug.boolIsGlobalDebug)
                            {
                                bgw_Chg_Num.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Global Debugger code started"));

                                strFileNameWithPath = $@"{strFolderPath}\Chg_Num_{dtSRC.TableName}_{strTRG_TBName}_{common_Data.AppendDateInOutputFileName(DateTime.Now).ToString()}.xlsx";

                                bgw_Chg_Num.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Path Generated to save file : {strFileNameWithPath}"));

                                if (WriteExcel.ExportToExcel(dtResult, strSRC_TBName, strFileNameWithPath))
                                {
                                    bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"File Save Successfully : {strFileNameWithPath}"));
                                }
                                else
                                {
                                    bgw_Chg_Num.ReportProgress(3, new BackGroundWorkerUserObjectForReport(0, 0, $"Error in saving file !!!"));
                                }

                                bgw_Chg_Num.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Global Debugger code finished"));
                            }

                            #endregion

                        }
                        else
                        {
                            bgw_Chg_Num.ReportProgress(3, new BackGroundWorkerUserObjectForReport(0, 0, $"No ID Found In Chg_Num Column : {strTRG_TBName}"));
                        }

                        bgw_Chg_Num.ReportProgress(1, new BackGroundWorkerUserObjectForReport(++int_Count_TableRow, dsTableList.Tables[0].Rows.Count, $"{strColNameSRC} in {strSRC_TBName} with {strColNameTRG} in {strTRG_TBName} : has been processed !!!"));
                        
                    }
                    catch (Exception ex)
                    {
                        bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(ex));
                    }

                }

                //bgw_Chg_Num.ReportProgress(1, new BackGroundWorkerUserObjectForReport(++int_Count_TableRow, dsTableList.Tables[0].Rows.Count, $"Data : {strTableID} Processed !!!!"));

                strFileNameWithPath = $@"{strFolderPath}\Chg_Num_{dtSRC.TableName}_{strTRG_TBName}_{common_Data.AppendDateInOutputFileName(DateTime.Now).ToString()}.xlsx";

                bgw_Chg_Num.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Path Generated to save file : {strFileNameWithPath}"));

                if (WriteExcel.ExportToExcel(dtResult, strSRC_TBName, strFileNameWithPath))
                {
                    bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"File Save Successfully : {strFileNameWithPath}"));
                }
                else
                {
                    bgw_Chg_Num.ReportProgress(3, new BackGroundWorkerUserObjectForReport(0, 0, $"Error in saving file !!!"));
                }

                timer1.Enabled = false;
                bgw_Chg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Process Completed Successfully"));
            }
            catch (Exception ex)
            {
                bgw_Chg_Num.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex));
                //throw ex;
            }
        }

        private void bgw_Chg_Num_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                BGW_ProgressChanged(e);
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void cb_CHG_NUM_From_Excel_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CommonCheckBoxCall();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }

        }

        private void CommonCheckBoxCall()
        {
            try
            {
                dsExcel.Reset();
                dsExcel.Clear();
                cb_ExcelSheetName.Items.Clear();
                cb_ExcelSheetName.Enabled = false;

                if (cb_CHG_NUM_From_Excel.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Browse Excel File For Input");
                    openFileDialogDataExcel = openFileDialogDataExcel.openFileDialog("Browse Excel File For SOURCE");
                    if (openFileDialogDataExcel.boolFileSelected)
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"File Loaded Successfully from : {openFileDialogDataExcel.strFileNameWithPath}");
                        dsExcel = ReadExcel.LoadExcelFromDataReader(openFileDialogDataExcel.strFileNameWithPath);

                        cb_ExcelSheetName.Items.AddRange(dsExcel.Tables.Cast<DataTable>().Select(x => x.TableName.ToString()).ToArray());
                        Common_Data.DisplayDebugMsg(rtb_Log, $"Combo Box Loaded");
                        if (cb_ExcelSheetName.Items.Count > 0)
                        {
                            cb_ExcelSheetName.Enabled = true;
                            cb_ExcelSheetName.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        cb_CHG_NUM_From_Excel.Checked = false;
                        Common_Data.DisplayMessage(rtb_Log, $"No File Selected", true);
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, "Excel Input CheckBox Unselected", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void cb_ExcelSheetName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (dsExcel.Tables.Count > 0)
                {
                    dtSRC = dsExcel.Tables[cb_ExcelSheetName.SelectedIndex];
                    Common_Data.DisplayMessage(rtb_Log, $"{dtSRC.TableName} Selected");
                }
                else
                {
                    dtSRC.Reset();
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 0 || e.TabPageIndex == 3)
            {
                gb_Selective.Visible = false;
            }
            else if (e.TabPageIndex == 1 || e.TabPageIndex == 2)
            {
                gb_Selective.Visible = true;
            }
        }

        private void btn_Chg_NumVsChg_Num_Click(object sender, EventArgs e)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, $"CHG_NUM Versus CHG_NUM");

                strSRCTypeSelected = connection.Source.DatabaseType;
                strTRGTypeSelected = connection.Target.DatabaseType;

                openFileDialogData = openFileDialogData.openFileDialog("Browse Excel File For Change Versus Num");

                if (openFileDialogData.boolFileSelected)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Reading Data : {openFileDialogData.strFileNameWithPath}");

                    folderBrowserDialog = Common_Data.browserDialogInitialisation("Select Folder To Save Excel For CHG_NUM", $@"{openFileDialogData.PathName_withoutFileName}\Output");

                    boolFolderBrowserDialog = false;

                    if (Debugger.IsAttached)
                    {
                        folderBrowserDialog.SelectedPath = @"D:\Krishna\Projects\SPI\INPUT FILES\Old Versus New\Prasad K\Output";
                        boolFolderBrowserDialog = true;
                    }
                    else if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        boolFolderBrowserDialog = true;
                    }
                    else
                    {
                        boolFolderBrowserDialog = false;
                    }

                    if (boolFolderBrowserDialog)
                    {
                        dsTableList = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);
                        dtTableList = dsTableList.Tables[0];

                        strFolderPath = folderBrowserDialog.SelectedPath;
                        Common_Data.DisplayMessage(rtb_Log, $"Saving path selected : {strFolderPath}");

                        if (!(bgw_Chg_numVsChg_Num.IsBusy))
                        {
                            timer1.Enabled = true;
                            start_Time = DateTime.Now;
                            bgw_Chg_numVsChg_Num.RunWorkerAsync();
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, $"Already its running", true);
                        }
                    }
                    else
                    {
                        strFolderPath = string.Empty;
                        Common_Data.DisplayMessage(rtb_Log, $"To save folder path not set, aborting all operation", true);
                        boolTRG_Folder = false;
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"User cancelled reading file !!!", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void bgw_Chg_numVsChg_Num_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int_Count_TableRow = 0;
                dtResult.Reset();

                foreach (DataRow dr in dsTableList.Tables[0].Rows)
                {
                    if (bgw_Chg_numVsChg_Num.CancellationPending)
                    {
                        e.Cancel = true;
                        bgw_Chg_numVsChg_Num.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                        return;
                    }

                    strSRC_TBName = dr["src_table_name"].ToString().Trim().ToUpper();
                    strTRG_TBName = dr["trg_table_name"].ToString().Trim().ToUpper();
                    strColNameSRC = dr["src_column_name"].ToString().Trim().ToUpper();
                    strColNameTRG = dr["trg_column_name"].ToString().Trim().ToUpper();
                    //strTableID = dr["table_id"].ToString().Trim().ToUpper();

                    try
                    {
                        strSRC_Select = $"SELECT * FROM {strSRC_TBName}";
                        strTRG_Select = $"SELECT * FROM {strTRG_TBName}";

                        bgw_Chg_numVsChg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Fetching Source and Target Table For : {strSRC_TBName} and {strTRG_TBName} respectively."));

                        dtSRC.Reset();
                        dtSRC = common_Data.FillDataTable(dtSRC, strSRCTypeSelected, dBConnectionStatus_SRC.strConnectionString, strSRC_Select, strSRC_TBName);

                        dtTRG.Reset();
                        dtTRG = common_Data.FillDataTable(dtTRG, strTRGTypeSelected, dBConnectionStatus_TRG.strConnectionString, strTRG_Select, strTRG_TBName);

                        dtSRC = common_Data.ChangeColumnTypeWithData(dtSRC,"CHG_NUM",typeof(int));
                        dtTRG = common_Data.ChangeColumnTypeWithData(dtTRG, "CHG_NUM", typeof(int));

                        var joinTable = (from s in dtSRC.AsEnumerable()
                                         join t in dtTRG.AsEnumerable() on s.Field<int>("CHG_NUM") equals t.Field<int>("CHG_NUM")
                                         select new { s, t });

                        dtSRC = common_Data.AddColumns(dtSRC, $"{strColNameSRC}_NEW", typeof(int));

                        dtResult.Reset();
                        dtResult = dtTRG.Clone();

                        dtResult = common_Data.AddColumns(dtResult, $"{strColNameTRG}_NEW", typeof(int));

                        joinTable.ToList().ForEach(x => SetData(x.s, strColNameTRG,Convert.ToInt32(x.t.Field<object>(strColNameTRG))));

                        dtResult.Columns[$"{strColNameTRG}_NEW"].SetOrdinal(dtResult.Columns[strColNameTRG].Ordinal + 1);

                    }
                    catch (Exception ex)
                    {
                        bgw_Chg_numVsChg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(ex));
                        continue;
                    }

                    strFileNameWithPath = $@"{strFolderPath}\Chg_Num_{dtSRC.TableName}_{strTRG_TBName}_{common_Data.AppendDateInOutputFileName(DateTime.Now).ToString()}.xlsx";
                    bgw_Chg_numVsChg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Path Generated to save file : {strFileNameWithPath}"));

                    if (WriteExcel.ExportToExcel(dtResult, strSRC_TBName, strFileNameWithPath))
                    {
                        bgw_Chg_numVsChg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"File Save Successfully : {strFileNameWithPath}"));
                    }
                    else
                    {
                        bgw_Chg_numVsChg_Num.ReportProgress(3, new BackGroundWorkerUserObjectForReport(0, 0, $"Error in saving file !!!"));
                    }

                    bgw_Chg_numVsChg_Num.ReportProgress(1, new BackGroundWorkerUserObjectForReport(++int_Count_TableRow, dsTableList.Tables[0].Rows.Count, $"{strSRC_TBName} : has been processed !!!"));
                }

                timer1.Enabled = false;
                bgw_Chg_numVsChg_Num.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Process Completed Successfully"));
            }
            catch (Exception ex)
            {
                bgw_Chg_numVsChg_Num.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex));
            }
        }

        private void SetData(DataRow dataRow, string strColName, int data)
        {
            try
            {
                dataRow[$"{strColName}_NEW"] = data;
                dtResult.Rows.Add(dataRow.ItemArray);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void bgw_Chg_numVsChg_Num_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                BGW_ProgressChanged(e);
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void bgw_Chg_numVsChg_Num_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                UpdateProgressOnly(int_Count_TableRow, dsTableList.Tables[0].Rows.Count);
                if (e.Error != null)
                {
                    Exception MyException = new Exception(e.Error.Message, e.Error.InnerException);
                    Common_Data.DisplayError(rtb_Log, MyException);
                }
                else if (e.Cancelled)
                {
                    Common_Data.DisplayMessage(rtb_Log, "All Process Cancelled Successfully !!!", true);
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Folder path opening :{strFolderPath}");
                    Process.Start(strFolderPath);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void bgw_Chg_Num_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                UpdateProgressOnly(int_Count_TableRow, dsTableList.Tables[0].Rows.Count);
                if (e.Error != null)
                {
                    Exception MyException = new Exception(e.Error.Message, e.Error.InnerException);
                    Common_Data.DisplayError(rtb_Log, MyException);
                }
                else if (e.Cancelled)
                {
                    Common_Data.DisplayMessage(rtb_Log, "All Process Cancelled Successfully !!!", true);
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Folder path opening :{strFolderPath}");
                    Process.Start(strFolderPath);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void bgw_old_VS_new_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                backGroundWorkerObject = new BackGroundWorkerUserObjectForReport();

                int_Count_Table = 0;

                foreach (DataTable dtt in dsTableList.Tables)
                {
                    boolTableChangeFlag = true;
                    dtResult.Reset();

                    int_Count_TableRow = 0;
                    foreach (DataRow dr in dtt.Rows)
                    {
                        if (bgw_old_VS_new.CancellationPending)
                        {
                            e.Cancel = true;
                            bgw_old_VS_new.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                            return;
                        }

                        if (boolTableChangeFlag)
                        {
                            strSRC_TBName = dr["src_table_name"].ToString().ToUpper();
                            strTRG_TBName = dr["trg_table_name"].ToString().ToUpper();
                            bgw_old_VS_new.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Processing Table : {strSRC_TBName}"));
                        }

                        strPK = dr["column_name"].ToString().ToUpper();
                        strTableID = dr["table_id"].ToString().ToUpper();

                        try
                        {
                            bgw_old_VS_new.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Fetching Merge Protocol Table For : {strSRC_TBName}"));

                            dtMergeProtocol = common_Data.FillDataTable(dtMergeProtocol, strTRGTypeSelected, dBConnectionStatus_TRG.strConnectionString, $"SELECT * FROM MERGE_PROTOCOL WHERE TABLE_ID = '{strTableID}'", "Merge Protocol Table");

                            dtMergeProtocol = common_Data.ChangeColumnToString(dtMergeProtocol);

                            string strNewID = string.Join(", ", dtMergeProtocol.AsEnumerable().Select(x => string.Concat("'", x["NEW_ID"].ToString(), "'")));

                            if (boolTableChangeFlag)
                            {
                                strSRC_Select = $"SELECT * FROM {strSRC_TBName}";
                                strTRG_Select = $"SELECT * FROM {strTRG_TBName}";

                                bgw_old_VS_new.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Fetching Source and Target Table For : {strTRG_TBName}"));

                                if (!(cb_CHG_NUM_From_Excel.Checked))
                                {
                                    dtSRC = common_Data.FillDataTable(dtSRC, strSRCTypeSelected, dBConnectionStatus_SRC.strConnectionString, strSRC_Select, strSRC_TBName);
                                }
                                dtTRG = common_Data.FillDataTable(dtTRG, strTRGTypeSelected, dBConnectionStatus_TRG.strConnectionString, strTRG_Select, strTRG_TBName);
                            }

                            if (dtSRC.Columns.Contains(strPK))
                            {
                                dtSRC = common_Data.AddColumnsWithData(dtSRC, $"{strPK}_OLD", strPK, typeof(string), dtSRC.Columns[strPK].Ordinal);
                                dtSRC = common_Data.AddColumnsWithData(dtSRC, $"{strPK}_NEW", strPK, typeof(string), dtSRC.Columns[strPK].Ordinal);

                                dtTRG = common_Data.AddColumnsWithData(dtTRG, $"{strPK}_OLD", strPK, typeof(string), dtTRG.Columns[strPK].Ordinal);
                                dtTRG = common_Data.AddColumnsWithData(dtTRG, $"{strPK}_NEW", strPK, typeof(string), dtTRG.Columns[strPK].Ordinal);

                                string strPK_ID_TRG = string.Join(", ", dtTRG.AsEnumerable().Select(x => string.Concat("'", x[strPK].ToString(), "'")));

                                boolID = false;
                                if (strNewID.Length > 0 & strPK_ID_TRG.Length > 0)
                                {
                                    str_SelectQuery = $"SELECT * from {strTRG_TBName} where {strPK} IN ( {strNewID} ) AND ( {strPK} IN ( {strPK_ID_TRG} ) )";
                                    boolID = true;
                                }
                                else if (strNewID.Length > 0)
                                {
                                    str_SelectQuery = $"SELECT * from {strTRG_TBName} where {strPK} IN ( {strNewID} )";
                                    boolID = true;
                                }
                                else if (strPK_ID_TRG.Length > 0)
                                {
                                    str_SelectQuery = $"SELECT * from {strTRG_TBName} where {strPK} IN ( {strPK_ID_TRG} )";
                                    boolID = true;
                                }

                                if (boolID)
                                {
                                    if (bgw_old_VS_new.CancellationPending)
                                    {
                                        e.Cancel = true;
                                        bgw_old_VS_new.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                                        return;
                                    }
                                    bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Resultant table processed successfully"));
                                    dtSRC = common_Data.RemoveColumn(dtSRC, strPK);

                                    bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Checking source and resultant table type"));

                                    var b = (from r in dtMergeProtocol.Columns.Cast<DataColumn>() join s in dtSRC.Columns.Cast<DataColumn>() on r.ColumnName equals s.ColumnName select new { r, s })
                                        .Where(x => x.r.DataType.Name != x.s.DataType.Name).ToList();

                                    if (b.Count > 0)
                                    {
                                        b.ForEach(x => bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, x.ToString(), false)));
                                    }
                                    else
                                    {
                                        bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"No Mismatch between mergeprotocol and source", true));
                                    }

                                    bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Checking target and resultant table type"));

                                    var c = (from r in dtMergeProtocol.Columns.Cast<DataColumn>() join s in dtSRC.Columns.Cast<DataColumn>() on r.ColumnName equals s.ColumnName where r.DataType.Name != s.DataType.Name select new { r, s }).ToList();

                                    if (c.Count > 0)
                                    {
                                        c.ForEach(x => bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, x.ToString(), false)));
                                    }
                                    else
                                    {
                                        bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"No Mismatch between mergeprotocol and target", true));
                                    }

                                    bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Processing variable result"));

                                    bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Joining of table started"));

                                    var varResult = (from s in dtSRC.AsEnumerable()
                                                     join m in dtMergeProtocol.AsEnumerable() on s.Field<string>($"{strPK}_OLD") equals m.Field<string>("OLD_ID")
                                                     select new { s, m }).ToList();

                                    bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Joining of table done"));

                                    varResult.ForEach(x => x.s.SetField($"{strPK}_NEW", x.m.Field<string>("NEW_ID")));

                                    bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Setting NEW_ID is done"));

                                    dtResult = dtSRC.Clone();

                                    varResult.ForEach(x => dtResult.Rows.Add(x.s.ItemArray));

                                    bgw_old_VS_new.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Joining operation done"));
                                }
                                else
                                {
                                    bgw_old_VS_new.ReportProgress(3, new BackGroundWorkerUserObjectForReport(0, 0, $"No ID Found In MergeProtocol : {strTRG_TBName}"));
                                }
                            }
                            else
                            {
                                bgw_old_VS_new.ReportProgress(3, new BackGroundWorkerUserObjectForReport(0, 0, $"'{strPK}' Column Not Found, Hence skipping this column"));
                                //Common_Data.DisplayMessage(rtb_Log, $"'{strPK}' Column Not Found, Hence skipping this column", true);
                            }
                        }
                        catch (Exception ex)
                        {
                            bgw_old_VS_new.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex));
                        }

                        boolTableChangeFlag = false;
                        bgw_old_VS_new.ReportProgress(1, new BackGroundWorkerUserObjectForReport(++int_Count_TableRow, dtt.Rows.Count, $"Data : {strTableID} Processed !!!!"));
                    }

                    strFileNameWithPath = $@"{strFolderPath}\OldVSNew_{dtSRC.TableName}_{strSRC_TBName}_{common_Data.AppendDateInOutputFileName(DateTime.Now).ToString()}.xlsx";
                    bgw_old_VS_new.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Path Generated to save file : {strFileNameWithPath}"));

                    if (cb_CHG_NUM_From_Excel.Checked)
                    {
                        dtResult.Reset();
                        dtResult = dtSRC.Copy();
                    }

                    if (WriteExcel.ExportToExcel(dtResult, strSRC_TBName, strFileNameWithPath))
                    {
                        bgw_old_VS_new.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"File Save Successfully : {strFileNameWithPath}"));
                    }
                    else
                    {
                        bgw_old_VS_new.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Error in saving file !!!"));
                    }

                    bgw_old_VS_new.ReportProgress(1, new BackGroundWorkerUserObjectForReport(++int_Count_Table, dsTableList.Tables.Count, $"{strSRC_TBName} : has been processed !!!"));

                    if (cb_CHG_NUM_From_Excel.Checked)
                    {
                        break;
                    }
                }

                timer1.Enabled = false;
                bgw_old_VS_new.ReportProgress(1, new BackGroundWorkerUserObjectForReport(int_Count_Table, int_Count_Table, $"Process Completed Successfully"));
                bgw_old_VS_new.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Folder path opening :{strFolderPath}"));
            }
            catch (Exception ex)
            {
                bgw_old_VS_new.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex));
                throw ex;
            }
        }

        private void bgw_TableExtractionAfterDelta_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                backGroundWorkerObject = new BackGroundWorkerUserObjectForReport();
                int_Count_TableRow = 0;
                foreach (DataRow dr in dtTableList.Rows)
                {
                    if (bgw_TableExtractionAfterDelta.CancellationPending)
                    {
                        e.Cancel = true;
                        bgw_TableExtractionAfterDelta.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                        return;
                    }

                    dtSRC = new DataTable();
                    dtTRG = new DataTable();
                    dtResult = new DataTable();

                    strSRC_TBName = dr["src_table_name"].ToString().ToUpper();
                    strTRG_TBName = dr["trg_table_name"].ToString().ToUpper();
                    strPK = dr["primary_key"].ToString().ToUpper();
                    strTableID = dr["table_id"].ToString().ToUpper();

                    bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Processing Table : {strSRC_TBName}"));
                    try
                    {
                        bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Fetching Merge_ProtocolTable For : {strSRC_TBName}"));

                        dtMergeProtocol = common_Data.FillDataTable(dtMergeProtocol, strTRGTypeSelected, dBConnectionStatus_TRG.strConnectionString, $"SELECT * FROM MERGE_PROTOCOL WHERE TABLE_ID = '{strTableID}'", "Merge Protocol Table");

                        string strNewID = string.Join(", ", dtMergeProtocol.AsEnumerable().Select(x => string.Concat("'", x["NEW_ID"].ToString(), "'")));

                        strSRC_Select = $"SELECT * FROM {strSRC_TBName}";
                        strTRG_Select = $"SELECT * FROM {strTRG_TBName}";

                        bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Fetching Source and Target Table For : {strTRG_TBName}"));

                        dtSRC = common_Data.FillDataTable(dtSRC, strSRCTypeSelected, dBConnectionStatus_SRC.strConnectionString, strSRC_Select, strSRC_TBName);
                        dtTRG = common_Data.FillDataTable(dtTRG, strTRGTypeSelected, dBConnectionStatus_TRG.strConnectionString, strTRG_Select, strTRG_TBName);

                        dtSRC = common_Data.CreateNewMergeColumn(dtSRC, strPK, "PK");
                        dtTRG = common_Data.CreateNewMergeColumn(dtTRG, strPK, "PK");

                        string strPK_ID_TRG = string.Join(", ", dtTRG.AsEnumerable().Select(x => string.Concat("'", x[strPK].ToString(), "'")));

                        boolID = false;
                        if (strNewID.Length > 0 & strPK_ID_TRG.Length > 0)
                        {
                            str_SelectQuery = $"SELECT * from {strTRG_TBName} where {strPK} IN ( {strNewID} ) AND ( {strPK} IN ( {strPK_ID_TRG} ) )";
                            boolID = true;
                        }
                        else if (strNewID.Length > 0)
                        {
                            str_SelectQuery = $"SELECT * from {strTRG_TBName} where {strPK} IN ( {strNewID} )";
                            boolID = true;
                        }
                        else if (strPK_ID_TRG.Length > 0)
                        {
                            str_SelectQuery = $"SELECT * from {strTRG_TBName} where {strPK} IN ( {strPK_ID_TRG} )";
                            boolID = true;
                        }

                        if (boolID)
                        {
                            bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Processing ResultantTable For : {strTRG_TBName}"));

                            dtResult = common_Data.FillDataTable(dtResult, strTRGTypeSelected, dBConnectionStatus_TRG.strConnectionString, str_SelectQuery, strTRG_TBName);
                            dtResult = common_Data.AddColumns(dtResult, "PK", typeof(string));
                            dtResult.Columns["PK"].SetOrdinal(0);

                            bgw_TableExtractionAfterDelta.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Resultant table processed successfully"));

                            //var b = (from r in dtResult.Columns.Cast<DataColumn>() join s in dtSRC.Columns.Cast<DataColumn>() on r.ColumnName equals s.ColumnName select new { r, s})
                            //    .Where(x => x.r.ColumnName.GetType().Name != x.s.ColumnName.GetType().Name);

                            //backgroundWorker1.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, b.ToString(), false));

                            //var c = from r in dtResult.Columns.Cast<DataColumn>() join s in dtSRC.Columns.Cast<DataColumn>() on r.ColumnName equals s.ColumnName where r.ColumnName.GetType().Name != s.ColumnName.GetType().Name select new {r,s};

                            //backgroundWorker1.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, c.ToString(), false));

                            //(from s in dtSRC.AsEnumerable()
                            // join t in dtTRG.AsEnumerable() on s.Field<string>("PK") equals t.Field<string>("PK")
                            // select t).ToList().ForEach(x => dtResult.Rows.Add(x.ItemArray));

                            var varResult = (from s in dtSRC.AsEnumerable()
                                             join t in dtTRG.AsEnumerable() on s.Field<string>("PK") equals t.Field<string>("PK")
                                             select t).ToList();

                            varResult.ForEach(x => dtResult.Rows.Add(x.ItemArray));

                            bgw_TableExtractionAfterDelta.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, $"Joining operation done"));

                            dtResult = common_Data.RemoveColumn(dtResult, "PK");

                            strFileNameWithPath = $@"{strFolderPath}\T_E_A_D_{strSRC_TBName}_{common_Data.AppendDateInOutputFileName(DateTime.Now).ToString()}.xlsx";
                            bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Path Generated to save file : {strFileNameWithPath}"));

                            if (WriteExcel.ExportToExcel(dtResult, strSRC_TBName, strFileNameWithPath))
                            {
                                bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"File Save Successfully : {strFileNameWithPath}"));
                            }
                            else
                            {
                                bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Error in saving file !!!"));
                            }
                        }
                        else
                        {
                            bgw_TableExtractionAfterDelta.ReportProgress(3, new BackGroundWorkerUserObjectForReport(0, 0, $"No ID Found In MergeProtocol : {strTRG_TBName}"));
                        }

                    }
                    catch (Exception ex)
                    {
                        bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(ex));
                    }
                    bgw_TableExtractionAfterDelta.ReportProgress(1, new BackGroundWorkerUserObjectForReport(++int_Count_TableRow, dtTableList.Rows.Count));

                    dtSRC.Dispose();
                    dtTRG.Dispose();
                    dtResult.Dispose();

                }
                timer1.Enabled = false;
                bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Process Completed Successfully"));
                bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Folder path opening :{strFolderPath}"));

            }
            catch (Exception ex)
            {
                //bgw_TableExtractionAfterDelta.ReportProgress(0, new BackGroundWorkerUserObjectForReport(ex));
                throw ex;
            }
        }

        private void bgw_TableExtractionAfterDelta_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                BGW_ProgressChanged(e);
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void bgw_TableExtractionAfterDelta_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                timer1.Enabled = false;

                if (e.Error != null)
                {
                    Exception MyException = new Exception(e.Error.Message, e.Error.InnerException);
                    Common_Data.DisplayError(rtb_Log, MyException);
                }
                else
                {
                    Process.Start(strFolderPath);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void bgw_old_VS_new_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                BGW_ProgressChanged(e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void bgw_old_VS_new_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                if (e.Error != null)
                {
                    Exception MyException = new Exception(e.Error.Message, e.Error.InnerException);
                    Common_Data.DisplayError(rtb_Log, MyException);
                }
                else if (e.Cancelled)
                {
                    Common_Data.DisplayMessage(rtb_Log, "All Process Cancelled Successfully !!!", true);
                }
                else
                {
                    Process.Start(strFolderPath);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (bgw_old_VS_new.IsBusy)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"User tried cancelling for Old Vs New", true);
                    bgw_old_VS_new.CancelAsync();
                }

                if (bgw_TableExtractionAfterDelta.IsBusy)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"User tried cancelling for Table extraction after delta", true);
                    bgw_TableExtractionAfterDelta.CancelAsync();
                }

                if (bgw_Chg_Num.IsBusy)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"User tried cancelling for Change Versus Num", true);
                    bgw_Chg_Num.CancelAsync();
                }

                if (!(bgw_old_VS_new.IsBusy || bgw_TableExtractionAfterDelta.IsBusy || bgw_Chg_Num.IsBusy))
                {
                    Common_Data.DisplayMessage(rtb_Log, $"No Process to cancel or all process are already cancelled", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }


        private void btn_Excel_Click(object sender, EventArgs e)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, $"Table Extraction After Delta Selected");

                strSRCTypeSelected = connection.Source.DatabaseType;
                strTRGTypeSelected = connection.Target.DatabaseType;

                openFileDialogData = openFileDialogData.openFileDialog("Browse Excel File For Table Extraction after Delta");
                if (openFileDialogData.boolFileSelected)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Reading Data : {openFileDialogData.strFileNameWithPath}");

                    folderBrowserDialog = Common_Data.browserDialogInitialisation("Select Folder To Save Excel For Table Extraction after Delta", $@"{openFileDialogData.PathName_withoutFileName}\Output");
                    //folderBrowserDialog.SelectedPath = openFileDialogData.PathName_withoutFileName;

                    if (Debugger.IsAttached)
                    {
                        folderBrowserDialog.SelectedPath = @"D:\Krishna\Projects\SPI\INPUT FILES\Table Extraction After Delta\Prasad K\Output";
                    }

                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        dtTableList = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath).Tables[0];

                        strFolderPath = folderBrowserDialog.SelectedPath;
                        Common_Data.DisplayMessage(rtb_Log, $"Saving path selected : {strFolderPath}");

                        if (!(bgw_TableExtractionAfterDelta.IsBusy))
                        {
                            timer1.Enabled = true;
                            start_Time = DateTime.Now;
                            bgw_TableExtractionAfterDelta.RunWorkerAsync();
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, $"Already its running", true);
                        }
                    }
                    else
                    {
                        strFolderPath = string.Empty;
                        Common_Data.DisplayMessage(rtb_Log, $"To save folder path not set, aborting all operation", true);
                        boolTRG_Folder = false;
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"User cancelled reading file !!!", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void UpdateProgress(int int_C, int int_TC)
        {
            try
            {
                int_Percentage = (int_C * 100) / int_TC;
                progressBar1.Value = int_Percentage;
                progressBar1.Update();
                Common_Data.DisplayMessage(rtb_Log, $"{int_Percentage} % Completed, Pls Wait !!!");
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
                throw ex;
            }
        }

        private void UpdateProgressOnly(int int_C, int int_TC)
        {
            try
            {
                int_Percentage = (int_C * 100) / int_TC;
                progressBar1.Value = int_Percentage;
                progressBar1.Update();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
                throw ex;
            }
        }


        private void btn_Clear_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(bgw_old_VS_new.IsBusy || bgw_TableExtractionAfterDelta.IsBusy))
                {
                    timer1.Enabled = false;
                    UpdateProgress(0, 1);
                    rtb_Log.ResetText();
                    displayElapsedTime(true);
                    Common_Data.DisplayMessage(rtb_Log, $"Logs Cleared");
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Cannot be cleared as process is in progress");
                }

                Common_Data.DisplayDebugMsg(rtb_Log, $"{Environment.UserName.ToString()}");
                Common_Data.DisplayDebugMsg(rtb_Log, $"{Environment.MachineName.ToString()}");

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
    }
}
