using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;

namespace SPI_Data_Comp_Tool
{
    public partial class Compare_Multiples_List_Tables_Forms : WorkspacePageForm
    {
        private IDisposable operationInputs;
        private OpenFileDialogData openFileDialogData;

        private DataSet dsTableList;
        private DataTable dtFetch;
        private DataTable dtTableList;
        private DataTable dtSRC, dtTRG;
        private DataTable dtUnmatched;
        private DataTable dtSRCOnly;
        private DataTable dtSummary, dtSummaryForList;

        private RichTextBox RichTextBox_Retain;
        private Common_Data common_Data;
        private DBConnectionStatus dbConnection, dbSRCConnection, dbTRGConnection;

        private int int_Color_Row, int_object;
        private bool boolLoadDataBase, boolExcelFileList, boolExcelFileForComparison, boolSRCConnection, boolTRGConnection;
        private bool boolANewRow;
        private bool boolFolderPathSelected, boolSRC_ExcelFile, boolTRG_ExcelFile;

        private string str_DBNameSRC, str_DBNameTRG;
        private string strSelectCol;
        private string str_Data_Source_To_Compare;
        private string str_Data_Target_To_Compare;
        private string str_SRCTableName, str_TRGTableName;
        private string strFolderPath;
        private string strFileName;

        private List<ColorPair> colorPairs;
        public Exception myException_BG { get; private set; }

        private DBConnectionDetails dBConnectionDetails;

        // The connection fields live in the shared ConnectionPanel; these keep the names the code below uses.
        private System.Windows.Forms.ComboBox cb_UserProfileList => connection.ProfileList;
        private System.Windows.Forms.GroupBox gb_Source1 => connection.Source.Group;
        private System.Windows.Forms.GroupBox gb_Target1 => connection.Target.Group;
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

        private DataRow dataRows_TRG;

        private List<string> strPKList;

        private DataColumn[] dc_PK_SRC;
        private DataColumn[] dc_PK_TRG;

        private ExcelWorksheet worksheet;
        private IEnumerable<ColorPair> query;

        private static Point point_db, point_excel, point_logs;

        private int int_Table_Progress_Count, int_timer;
        private bool boolCompare_List;
        private bool operationFailed;
        private BackGroundWorkerUserObjectForReport backGroundWorkerObject;
        private bool boolSelectTabDB, boolSelectTabExcel;

        private bool boolRules;
        private DataSet dsRules;
        //private CheckRules checkRules;
        private DataSet dsAllColumns;
        private StepFlow steps;   // controlled steps, see BuildSteps
        private bool compareFinished;

        /// <summary>
        /// Controlled steps. Databases tab: Connections, Table list, Output folder, Compare.
        /// Excel folders tab: Source folder, Target folder, Table list, Output folder, Compare.
        /// </summary>
        private void BuildSteps()
        {
            Func<bool> databases = () => tabControl1.SelectedTab == tabPage_DB;
            Func<bool> folders = () => tabControl1.SelectedTab == tabPage_Excel;
            steps = new StepFlow(this, step_Strip)
                .Add(() => "Connections", () => "Test both connections", () => boolLoadDataBase && boolSRCConnection && boolTRGConnection, databases)
                .Add(() => "Source folder", () => "Choose the source workbook folder",
                    () => boolSRC_ExcelFile && System.IO.Directory.Exists(llb_Excel_SRC.Text), folders, llb_Excel_SRC)
                .Add(() => "Target folder", () => "Choose the target workbook folder",
                    () => boolTRG_ExcelFile && System.IO.Directory.Exists(llb_Excel_TRG.Text), folders, llb_Excel_TRG)
                .Add("Table list", "Choose the list-table Excel", () => boolExcelFileList, btn_ListTableExcel)
                .Add("Output folder", "Choose the output folder",
                    () => boolFolderPathSelected && System.IO.Directory.Exists(l_lbl_outputFolder.Text), l_lb_Output_Set, l_lbl_outputFolder)
                .Add("Compare", "Choose the options, then compare and generate Excel", () => compareFinished,
                    cb_PK_As_ReferenceKey, cb_IncludeDiff_Columns, cb_IgnoreCase, cb_IncludeRule, btn_RuleForm, btn_Compare);
            steps.Watch(tabControl1, btn_TestDBConnection);
            steps.Refresh();
        }

        public Compare_Multiples_List_Tables_Forms()
        {
            InitializeComponent();
            FormClosing += (sender, args) =>
            {
                if (bgw_List.IsBusy)
                {
                    args.Cancel = true;
                    FormValidation.Check(this, () => FormValidation.Reject(btnCancel,
                        "Wait for the current operation to finish or cancel it before closing this window."),
                        message => Common_Data.DisplayMessage(rtb_Log, message, true));
                }
            };
            connection.ProfileApplied += Connection_ProfileApplied;
            connection.Failed += (sender, error) => Common_Data.DisplayError(rtb_Log, error);
            connection.InputChanged += (sender, args) => { boolLoadDataBase = boolSRCConnection = boolTRGConnection = compareFinished = false; steps?.Later(); };
            UiTheme.Apply(this, btn_Compare);

            try
            {
                point_db = gb_DBConnection.Location;
                point_logs = gb_Logs.Location;
                point_excel = new Point(gb_Excel.Location.X, gb_Excel.Location.Y + gb_Excel.Size.Height);

                lbl_Progress.ResetText();
                openFileDialogData = new OpenFileDialogData();

                dtTableList = new DataTable();
                dsTableList = new DataSet();

                boolLoadDataBase = false;
                boolExcelFileList = false;
                boolExcelFileForComparison = false;


                RichTextBox_Retain = new RichTextBox();
                Common_Data.DisplayMessage(rtb_Log, ("DB Selected"));
                common_Data = new Common_Data();
                dbConnection = new DBConnectionStatus();

                dtSRC = new DataTable();
                dtTRG = new DataTable();

                dbSRCConnection = new DBConnectionStatus();
                dbTRGConnection = new DBConnectionStatus();

                dtFetch = new DataTable();

                boolSRC_ExcelFile = false;
                boolTRG_ExcelFile = false;

                colorPairs = new List<ColorPair>();
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                dsRules = new DataSet();

                dtSummaryForList = new DataTable();

                dtSummaryForList = common_Data.AddColumns(dtSummaryForList, "SrNo", typeof(int));
                dtSummaryForList = common_Data.AddColumns(dtSummaryForList, "Table Name");
                dtSummaryForList = common_Data.AddColumns(dtSummaryForList, "Mismatch Summary Count", typeof(int));
                dtSummaryForList = common_Data.AddColumns(dtSummaryForList, "Source Rows Count", typeof(int));
                dtSummaryForList = common_Data.AddColumns(dtSummaryForList, "Target Rows Count", typeof(int));

                boolCompare_List = false;

                int_Table_Progress_Count = 0;

                boolSelectTabDB = false;
                boolSelectTabExcel = false;

                //checkRules = new CheckRules();

                dsAllColumns = new DataSet();
                ProfileLoadResult profileLoad = connection.LoadSavedProfiles();
                if (cb_UserProfileList.Items.Count > 0)
                {
                    cb_UserProfileList.SelectedIndex = 0;
                }

                if (!string.IsNullOrEmpty(profileLoad.Message))
                    Common_Data.DisplayMessage(rtb_Log, profileLoad.Message, profileLoad.HasError);

                if (GlobalDebug.ISGlobalDebug(Environment.UserName, Environment.UserName))
                {
                    if (tabControl1.SelectedTab == tabPage_DB)
                    {
                        if (cb_UserProfileList.Items.Count > 2)
                        {
                            cb_UserProfileList.SelectedIndex = 1;
                        }

                        boolFolderPathSelected = true;
                        strFolderPath = @"D:\Krishna\Output";
                        l_lbl_outputFolder.Text = strFolderPath;

                    }
                    else if (tabControl1.SelectedTab == tabPage_Excel)
                    {
                        folderBrowserDialog1.SelectedPath = @"D:\Krishna\Projects\SPI\INPUT FILES\compare_multiple\MultiLevel Output\Source";

                        l_lbl_outputFolder.Text = folderBrowserDialog1.SelectedPath;
                        strFolderPath = folderBrowserDialog1.SelectedPath;

                        boolFolderPathSelected = true;

                        llb_Excel_SRC.Text = @"D:\Krishna\Projects\SPI\INPUT FILES\compare_multiple\MultiLevel Output\Source";
                        llb_Excel_TRG.Text = @"D:\Krishna\Projects\SPI\INPUT FILES\compare_multiple\MultiLevel Output\Target";

                        boolSRC_ExcelFile = true;
                        boolTRG_ExcelFile = true;
                    }
                }

                this.Size = new Size(1223, this.Size.Height);
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
            BuildSteps();
        }

        /// <summary>A saved profile was put into the Connection panel: take over its table names.</summary>
        private void Connection_ProfileApplied(object sender, DBConnectionDetails details)
        {
            dBConnectionDetails = details;
            Common_Data.DisplayMessage(rtb_Log, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
        }

        private void btn_ListTableExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FormValidation.Check(this, () => FormValidation.IsBusy(bgw_List, btn_ListTableExcel),
                    message => Common_Data.DisplayMessage(rtb_Log, message, true))) return;
                Common_Data.DisplayMessage(rtb_Log, ("Browsing Contained List Table Excel File ... "));
                openFileDialogData = openFileDialogData.openFileDialog("Select Excel File Contain Table List");

                if (openFileDialogData.boolFileSelected)
                {
                    DataSet candidate = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath, "Table List");
                    if (!FormValidation.Check(this, () => ValidateBatchWorkbook(candidate),
                        message => Common_Data.DisplayMessage(rtb_Log, message, true))) return;
                    dsTableList = candidate;

                    if (dsTableList != null && dsTableList.Tables.Count > 0 && dsTableList.Tables[0].Rows.Count > 0)
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"Selected File {openFileDialogData.strFileNameWithPath}");
                        boolExcelFileList = true;
                        Common_Data.DisplayMessage(rtb_Log, $"File Read Successfully", false, -1, Color.DarkBlue);
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
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private DataTable GetDataTable(string str_FullPathTableName)
        {
            try
            {
                dtFetch = new DataTable();
                dtFetch = ReadExcel.LoadExcelFromDataReader(str_FullPathTableName, "Table List").Tables[0];
                //Common_Data.DisplayMessage(rtb_Log, $"Table Loaded {str_FullPathTableName}");
                bgw_List.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Table Loaded {str_FullPathTableName}"));
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
            return dtFetch;
        }

        private DataTable GetDataTable(RadioButton rb_oracle, RadioButton rb_SQL, DBConnectionStatus dBConnection, string strTableName)
        {
            try
            {
                string[] str_IgnoreColName;
                if (rb_oracle.Checked)
                {
                    //strSelectCol = $"SELECT table_name, column_name, data_type, data_length FROM USER_TAB_COLUMNS WHERE table_name = '{dsTableList.Tables[0].Rows[0]["table_names"].ToString().ToUpper()}'";

                    strSelectCol = $"SELECT * FROM {strTableName}";

                    using (OracleConnection oracleConnection = new OracleConnection(dBConnection.strConnectionString))
                    {
                        using (OracleDataAdapter oracleDA = new OracleDataAdapter(strSelectCol, oracleConnection))
                        {
                            dtFetch = new DataTable();
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
                        }
                    }

                }
                else if (rb_SQL.Checked)
                {
                    //strSelectCol = $"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = '{dsTableList.Tables[0].Rows[0]["table_names"]}'";

                    strSelectCol = $"SELECT * FROM {strTableName}";

                    using (SqlConnection sqlConnection = new SqlConnection(dBConnection.strConnectionString))
                    {
                        using (SqlDataAdapter sqlDA = new SqlDataAdapter(strSelectCol, sqlConnection))
                        {
                            dtFetch = new DataTable();
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

                            //Common_Data.DisplayMessage(rtb_Log,"Oracle PK Read");
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

        private bool Compare_List(DoWorkEventArgs e)
        {
            try
            {
                int_Table_Progress_Count = 0;
                dtSummaryForList.Clear();

                bgw_List.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, "Starting compairing from excel list"));

                if (boolSelectTabDB)
                {
                    if (dbSRCConnection.boolConnection && dbTRGConnection.boolConnection)
                    {
                        dsAllColumns = new DataSet();
                        foreach (DataRow dr in dsTableList.Tables[0].Rows)
                        {
                            try
                            {
                                if (bgw_List.CancellationPending)
                                {
                                    e.Cancel = true;
                                    bgw_List.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                                    return false;
                                }

                                str_SRCTableName = dr["src_table_names"].ToString().ToUpper();
                                str_TRGTableName = dr["trg_table_names"].ToString().ToUpper();

                                bgw_List.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Processing Table {str_SRCTableName} From Source AND {str_TRGTableName} From Target In Database"));

                                dtSRC = GetDataTable(rb_SRC_oracle, rb_SRC_SQL, dbSRCConnection, str_SRCTableName);
                                dtTRG = GetDataTable(rb_TRG_Oracle, rb_TRG_SQL, dbTRGConnection, str_TRGTableName);

                                bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Removing uncommon column if any"));

                                dtSRC = Common_Data.RemoveUncommonColumnInDataTable(dtSRC, dtTRG);
                                dtTRG = Common_Data.RemoveUncommonColumnInDataTable(dtTRG, dtSRC);

                                dsAllColumns.Tables.Add(dtSRC.Copy());
                                dsAllColumns.Tables.Add(dtTRG.Copy());

                                if (cb_PK_As_ReferenceKey.Checked)
                                {
                                    CompareTwoDataTableBYPrimaryKey(dtSRC, dtTRG, dr["primary_key_columns"].ToString().ToUpper(), e);
                                }
                                else
                                {
                                    CompareTwoDataTable(dtSRC, dtTRG, dr["primary_key_columns"].ToString().ToUpper(), e);
                                }

                                bgw_List.ReportProgress(6, new BackGroundWorkerUserObjectForReport(int_Table_Progress_Count, dsTableList.Tables[0].Rows.Count, "update"));
                            }
                            catch (Exception ex1)
                            {
                                operationFailed = true;
                                bgw_List.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex1));
                            }
                        }
                        return true;
                    }
                    else
                    {
                        bgw_List.ReportProgress(3, new BackGroundWorkerUserObjectForReport(0, 0, "DB Connections problem", true));
                        return false;
                    }

                }
                else if (boolSelectTabExcel)
                {
                    if (boolSRC_ExcelFile && boolTRG_ExcelFile)
                    {
                        foreach (DataRow dr in dsTableList.Tables[0].Rows)
                        {
                            try
                            {
                                if (bgw_List.CancellationPending)
                                {
                                    e.Cancel = true;
                                    bgw_List.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                                    return false;
                                }

                                str_SRCTableName = dr["src_table_names"].ToString().ToUpper();
                                str_TRGTableName = dr["trg_table_names"].ToString().ToUpper();

                                bgw_List.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Processing Table {str_SRCTableName} From Source AND {str_TRGTableName} From Target In Excel Selected Folders"));

                                dtSRC = GetDataTable($@"{llb_Excel_SRC.Text}\{str_SRCTableName}.xlsx");
                                dtSRC.TableName = str_SRCTableName;
                                dtTRG = GetDataTable($@"{llb_Excel_TRG.Text}\{str_TRGTableName}.xlsx");
                                dtTRG.TableName = str_TRGTableName;

                                bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Removing uncommon column if any"));

                                if (dtSRC != null && dtSRC.Rows.Count > 0 && dtTRG != null && dtTRG.Rows.Count > 0)
                                {
                                    dtSRC = Common_Data.RemoveUncommonColumnInDataTable(dtSRC, dtTRG);
                                    dtTRG = Common_Data.RemoveUncommonColumnInDataTable(dtTRG, dtSRC);

                                    if (cb_PK_As_ReferenceKey.Checked)
                                    {
                                        CompareTwoDataTableBYPrimaryKey(dtSRC, dtTRG, dr["primary_key_columns"].ToString().ToUpper(), e);
                                    }
                                    else
                                    {
                                        CompareTwoDataTable(dtSRC, dtTRG, dr["primary_key_columns"].ToString().ToUpper(), e);
                                    }
                                }
                                else
                                {
                                    bgw_List.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"Either Source Table or Target Table Not Found or Have No Rows for {str_SRCTableName} AND {str_TRGTableName}", true));
                                }

                                bgw_List.ReportProgress(6, new BackGroundWorkerUserObjectForReport(int_Table_Progress_Count, dsTableList.Tables[0].Rows.Count, "update"));
                            }
                            catch (Exception ex1)
                            {
                                operationFailed = true;
                                bgw_List.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex1));

                            }
                        }
                        return true;
                    }
                    else
                    {
                        bgw_List.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 0, $"Either Source Excel File or Target Excel File has not been set", true));
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                operationFailed = true;
                bgw_List.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex));
                return false;
            }
        }

        private void RemoveUncommonColumn()
        {
            try
            {
                foreach (DataColumn dc in dtSRC.Columns.Cast<DataColumn>().ToArray())
                {
                    if (!(dtTRG.Columns.Contains(dc.ColumnName)))
                    {
                        dtSRC.Columns.Remove(dc);
                        Common_Data.DisplayDebugMsg(rtb_Log, $"Removing columns : {dc.ColumnName}");
                    }
                }

                dtTRG.AcceptChanges();

                foreach (DataColumn dc in dtTRG.Columns.Cast<DataColumn>().ToArray())
                {
                    if (!(dtSRC.Columns.Contains(dc.ColumnName)))
                    {
                        dtTRG.Columns.Remove(dc);
                        Common_Data.DisplayDebugMsg(rtb_Log, $"Removing columns : {dc.ColumnName}");
                    }
                }

                dtSRC.AcceptChanges();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable DeleteRowFromDatatable(DataTable dt, DataRow dr)
        {
            try
            {
                dt.Rows.Remove(dr);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ExcelWorksheet AddNewWorksheet(ExcelPackage pck, ExcelWorksheet worksheet, DataTable dt, string strTableName = null)
        {
            try
            {
                if (strTableName != null)
                {
                    dt.TableName = strTableName;
                }

                worksheet = pck.Workbook.Worksheets.Add(dt.TableName);
                FormValidation.ExcelTable(dt, null, "Excel output");
                worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                worksheet.Cells.AutoFitColumns(22, 55);

                return worksheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CompareTwoDataTableBYPrimaryKey(DataTable dtSRC, DataTable dtTRG, string strPK, DoWorkEventArgs e)
        {
            try
            {
                #region main comparison part

                if (bgw_List.CancellationPending)
                {
                    e.Cancel = true;
                    bgw_List.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                    return;
                }

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

                foreach (DataColumn dc in dtUnmatched.Columns)
                {
                    dc.DataType = typeof(object);
                }

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

                foreach (DataRow dr in dtSRC.Rows.Cast<DataRow>().ToArray())
                {
                    int_object = 0;
                    foreach (string item in strPKList)
                    {
                        vs_pk[int_object++] = dr[item];
                    }

                    dataRows_TRG = dtTRG.Rows.Find(vs_pk);

                    if (dataRows_TRG != null)
                    {
                        boolANewRow = true;

                        foreach (DataColumn dc in dataRows_TRG.Table.Columns)
                        {
                            if (!(strPKList.Any(a => string.Equals(a, dc.ColumnName))))
                            {
                                str_Data_Source_To_Compare = dr[dc.ColumnName].ToString().Trim();
                                str_Data_Target_To_Compare = dataRows_TRG[dc.ColumnName].ToString().Trim();

                                bool boolTest = common_Data.MismatchTest(str_Data_Source_To_Compare, str_Data_Target_To_Compare, cb_IgnoreCase.Checked, cb_IncludeRule.Checked, dc.ColumnName.ToString(), dc.ColumnName.ToString(), dr, dataRows_TRG, dtSRC.Columns, dtTRG.Columns);

                                if (boolTest)
                                {
                                    //#region Check rules 
                                    //if (boolRules)
                                    //{
                                    //    if (str_Data_Source_To_Compare.Length > 0 && str_Data_Target_To_Compare.Length > 0 && dc.ColumnName.ToString().Length > 0)
                                    //    {
                                    //        boolRulesCheck = common_Data.CheckRulesInFile(str_Data_Source_To_Compare, str_Data_Target_To_Compare, dc.ColumnName.ToString(), dc.ColumnName.ToString(), dr, dataRows_TRG, dtSRC.Columns, dtTRG.Columns);

                                    //        if (boolRulesCheck)
                                    //        {
                                    //            continue;
                                    //        }
                                    //    }
                                    //}

                                    //#endregion

                                    dtSummary.Rows[0][dc.ColumnName] = Convert.ToInt32(dtSummary.Rows[0][dc.ColumnName]) + 1;
                                    if (boolANewRow)
                                    {
                                        //str_Data_Source_To_Compare = dr[dc.ColumnName].ToString().Trim();
                                        //str_Data_Target_To_Compare = dataRows_TRG[dc.ColumnName].ToString().Trim();

                                        //boolCompareResult = false;

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
                                        //    if (str_Data_Source_To_Compare.Length >= 0 && str_Data_Target_To_Compare.Length >= 0 && dsRules.Tables[0].Rows.Count > 0 && dc.ToString().Length > 0 && dtSRC.Rows.Count > 0 && dtTRG.Rows.Count > 0)
                                        //    {
                                        //        boolRulesCheck = common_Data.CheckRulesInFile(str_Data_Source_To_Compare, str_Data_Target_To_Compare, dc.ToString(), dc.ToString(), dsAllColumns.Tables[0].Select(str_unique_reference_key)[0], dsAllColumns.Tables[1].Select(str_unique_reference_key)[0], dsAllColumns.Tables[0].Columns, dsAllColumns.Tables[1].Columns);

                                        //        if (boolRulesCheck)
                                        //        {
                                        //            continue;
                                        //        }
                                        //    }
                                        //}


                                        //#endregion
                                        ////    if (!(string.Equals(str_Data_Source_To_Compare, str_Data_Target_To_Compare, StringComparison.Ordinal)))
                                        ////{

                                        //if ((!(boolCompareResult)) && (boolRulesCheck))
                                        //{
                                        dtSummary.Rows[0][dc.ColumnName] = Convert.ToInt32(dtSummary.Rows[0][dc.ColumnName]) + 1;
                                        //if (boolANewRow)
                                        //{
                                        dtUnmatched.Rows.Add(dr.ItemArray);
                                        dtUnmatched.Rows.Add(ComparisonRows.ValuesForColumns(dataRows_TRG, dtUnmatched));
                                        boolANewRow = false;
                                        int_Color_Row += 2;
                                    }
                                    colorPairs.Add(new ColorPair(int_Color_Row, dc.ColumnName.ToString()));
                                    colorPairs.Add(new ColorPair(int_Color_Row - 1, dc.ColumnName.ToString()));
                                }
                            }
                        }
                        dtSRC = DeleteRowFromDatatable(dtSRC, dr);
                        dtTRG = DeleteRowFromDatatable(dtTRG, dataRows_TRG);
                    }
                    else
                    {
                        dtSRCOnly.Rows.Add(dr.ItemArray);
                    }
                }

                //Common_Data.DisplayDebugMsg(rtb_Log,"Main Loop Comparison Done");
                bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Main Loop Comparison Done"));

                dtSummary = common_Data.GenerateTransposeTable(dtSummary);
                foreach (DataRow dr in dtSummary.Rows.Cast<DataRow>().ToArray())
                {
                    if (Convert.ToInt32(dr["Summarize Count"].ToString()) == 0 && (!(strPKList.Any(a => string.Equals(a, dr["Column Names"])))))
                    {
                        if (dtUnmatched.Columns.Contains(dr["Column Names"].ToString()))
                        {
                            dtUnmatched.Columns.Remove(dr["Column Names"].ToString());
                        }
                        dr.Delete();
                    }
                }

                //Common_Data.DisplayDebugMsg(rtb_Log,"Transpose of summary and initialisation is done");
                bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Transpose of summary and initialisation is done"));

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

                using (ExcelPackage pck = new ExcelPackage())
                {
                    worksheet = pck.Workbook.Worksheets.Add("Mismatch");
                    FormValidation.ExcelTable(dtUnmatched, null, "Excel output");
                    worksheet.Cells["A1"].LoadFromDataTable(dtUnmatched, true);
                    worksheet.Cells.AutoFitColumns(22, 55);

                    //Common_Data.DisplayDebugMsg(rtb_Log,"First Workseet Written");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "First Workseet Written"));

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

                    //Common_Data.DisplayDebugMsg(rtb_Log,"Yellow coloring of first worksheet is done");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Yellow coloring of first worksheet is done"));

                    #endregion

                    dtSRCOnly.TableName = "Source Rows Only";

                    worksheet = AddNewWorksheet(pck, worksheet, dtSRCOnly);
                    worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                    //Common_Data.DisplayDebugMsg(rtb_Log,"Second worksheet written");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Second worksheet written"));


                    worksheet = AddNewWorksheet(pck, worksheet, dtTRG, "Target Rows Only");
                    worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                    //Common_Data.DisplayDebugMsg(rtb_Log,"Third worksheet written");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Third worksheet written"));


                    worksheet = AddNewWorksheet(pck, worksheet, dtSummary, "Summary");
                    worksheet.Cells[2, 1, strPKList.Count + 1, 1].Style.Font.Color.SetColor(Color.Red);

                    foreach (var cell in worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column])
                    {
                        cell.Value = Convert.ToInt32(cell.Value);
                    }

                    //Common_Data.DisplayDebugMsg(rtb_Log,"Fourth and last worksheet written");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Fourth and last worksheet written"));

                    // New Code for dtSummaryForList

                    dtSummaryForList.Rows.Add(int_Table_Progress_Count, dtSRC.TableName, dtSummary.AsEnumerable().Sum(x => Int32.Parse(x.Field<string>("Summarize Count"))), dtSRCOnly.Rows.Count, dtTRG.Rows.Count);

                    strFileName = $@"{strFolderPath}\{dtSRC.TableName}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                    FormValidation.WriteAtomically(strFileName, temporary => pck.SaveAs(new FileInfo(temporary)));
                    //Common_Data.DisplayMessage(rtb_Log, $"File Saved : {strFileName}", false, -1, Color.DarkBlue);
                    bgw_List.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"File Saved : {strFileName}"));

                }
                #endregion
            }

            catch (Exception ex)
            {
                operationFailed = true;
                bgw_List.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex));
            }

        }

        private void CompareTwoDataTable(DataTable dtSRC, DataTable dtTRG, string strPK, DoWorkEventArgs e)
        {
            try
            {
                #region main comparison part

                if (bgw_List.CancellationPending)
                {
                    e.Cancel = true;
                    bgw_List.ReportProgress(4, new BackGroundWorkerUserObjectForReport(0, 1, "User Cancelled Process", true));
                    return;
                }

                strPKList = strPK.Split(',').Select(p => p.Trim()).ToList();

                int_object = 0;

                foreach (string item in strPKList)
                {
                    dtSRC.Columns[item].SetOrdinal(int_object);
                    dtTRG.Columns[item].SetOrdinal(int_object);
                    int_object++;
                }

                dtUnmatched = dtSRC.Clone();
                dtSRCOnly = dtSRC.Clone();
                dtSummary = dtSRC.Clone();

                foreach (DataColumn dc in dtUnmatched.Columns)
                {
                    dc.DataType = typeof(object);
                }

                foreach (DataColumn dc in dtSummary.Columns)
                {
                    dc.DataType = typeof(Int32);
                }

                dtSummary.Rows.Add();

                foreach (DataColumn dc in dtSummary.Columns)
                {
                    dtSummary.Rows[0][dc] = 0;
                }

                int_Color_Row = 1;

                var remainingTargets = new ComparisonRows(dtTRG, strPKList);
                foreach (DataRow dr in dtSRC.Rows.Cast<DataRow>().ToArray())
                {
                    int_object = 0;
                    DataRow targetRow = remainingTargets.TakeNext(dr);
                    DataColumn[] dc_TRG = dtTRG.Columns.Cast<DataColumn>().ToArray();

                    if (targetRow != null)
                    {
                        boolANewRow = true;

                        foreach (DataColumn dc in dc_TRG.Cast<DataColumn>().ToArray())
                        {
                            if (!(strPKList.Any(a => string.Equals(a, dc.ColumnName))))
                            {
                                str_Data_Source_To_Compare = dr[dc.ColumnName].ToString().Trim();
                                str_Data_Target_To_Compare = targetRow[dc.ColumnName].ToString().Trim();

                                //boolCompareResult = false;

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
                                //boolRulesCheck = true;
                                //if (cb_IncludeRule.Checked && boolRules && (!boolCompareResult))
                                //{
                                //    if (str_Data_Source_To_Compare.Length >= 0 && str_Data_Target_To_Compare.Length >= 0 && dsRules.Tables[0].Rows.Count > 0 && dc.ToString().Length > 0 && dtSRC.Rows.Count > 0 && dtTRG.Rows.Count > 0)
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
                                //if (!(string.Equals(str_Data_Source_To_Compare, str_Data_Target_To_Compare, StringComparison.Ordinal)))
                                //{
                                bool boolTest = common_Data.MismatchTest(str_Data_Source_To_Compare, str_Data_Target_To_Compare, cb_IgnoreCase.Checked, cb_IncludeRule.Checked, dc.ColumnName.ToString(), dc.ColumnName.ToString(), dr, targetRow, dtSRC.Columns, dtTRG.Columns);

                                if (boolTest)
                                {
                                    dtSummary.Rows[0][dc.ColumnName] = Convert.ToInt32(dtSummary.Rows[0][dc.ColumnName]) + 1;
                                    if (boolANewRow)
                                    {
                                        dtUnmatched.Rows.Add(dr.ItemArray);
                                        dtUnmatched.Rows.Add(ComparisonRows.ValuesForColumns(targetRow, dtUnmatched));
                                        boolANewRow = false;
                                        int_Color_Row += 2;
                                    }
                                    colorPairs.Add(new ColorPair(int_Color_Row, dc.ColumnName.ToString()));
                                    colorPairs.Add(new ColorPair(int_Color_Row - 1, dc.ColumnName.ToString()));
                                }
                            }
                        }
                        dtSRC = DeleteRowFromDatatable(dtSRC, dr);
                    }
                    else
                    {
                        dtSRCOnly.Rows.Add(dr.ItemArray);
                    }
                }

                dtTRG = remainingTargets.Remaining();

                //Common_Data.DisplayDebugMsg(rtb_Log,"Main Loop Comparison Done");
                bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Main Loop Comparison Done"));

                dtSummary = common_Data.GenerateTransposeTable(dtSummary);
                foreach (DataRow dr in dtSummary.Rows.Cast<DataRow>().ToArray())
                {
                    if (Convert.ToInt32(dr["Summarize Count"].ToString()) == 0 && (!(strPKList.Any(a => string.Equals(a.ToLower(), dr["Column Names"].ToString().ToLower())))))
                    {
                        if (dtUnmatched.Columns.Contains(dr["Column Names"].ToString()))
                        {
                            dtUnmatched.Columns.Remove(dr["Column Names"].ToString());
                        }
                        dr.Delete();
                    }
                }

                //Common_Data.DisplayDebugMsg(rtb_Log,"Transpose of summary and initialisation is done");
                bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Transpose of summary and initialisation is done"));

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

                using (ExcelPackage pck = new ExcelPackage())
                {
                    worksheet = pck.Workbook.Worksheets.Add("Mismatch");
                    FormValidation.ExcelTable(dtUnmatched, null, "Excel output");
                    worksheet.Cells["A1"].LoadFromDataTable(dtUnmatched, true);
                    worksheet.Cells.AutoFitColumns(22, 55);

                    //Common_Data.DisplayDebugMsg(rtb_Log,"First Workseet Written");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "First Workseet Written"));

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

                    //Common_Data.DisplayDebugMsg(rtb_Log,"Yellow coloring of first worksheet is done");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Yellow coloring of first worksheet is done"));

                    #endregion

                    dtSRCOnly.TableName = "Source Rows Only";

                    worksheet = AddNewWorksheet(pck, worksheet, dtSRCOnly);
                    worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                    //Common_Data.DisplayDebugMsg(rtb_Log,"Second worksheet written");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Second worksheet written"));


                    worksheet = AddNewWorksheet(pck, worksheet, dtTRG, "Target Rows Only");
                    worksheet.Cells[1, 1, 1, strPKList.Count].Style.Font.Color.SetColor(Color.Red);

                    //Common_Data.DisplayDebugMsg(rtb_Log,"Third worksheet written");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Third worksheet written"));


                    worksheet = AddNewWorksheet(pck, worksheet, dtSummary, "Summary");
                    worksheet.Cells[2, 1, strPKList.Count + 1, 1].Style.Font.Color.SetColor(Color.Red);

                    foreach (var cell in worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column])
                    {
                        cell.Value = Convert.ToInt32(cell.Value);
                    }

                    //Common_Data.DisplayDebugMsg(rtb_Log,"Fourth and last worksheet written");
                    bgw_List.ReportProgress(2, new BackGroundWorkerUserObjectForReport(0, 0, "Fourth and last worksheet written"));

                    // New Code for dtSummaryForList

                    dtSummaryForList.Rows.Add(int_Table_Progress_Count + 1, dtSRC.TableName, dtSummary.AsEnumerable().Sum(x => Int32.Parse(x.Field<string>("Summarize Count"))), dtSRCOnly.Rows.Count, dtTRG.Rows.Count);

                    strFileName = $@"{strFolderPath}\{dtSRC.TableName}_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                    FormValidation.WriteAtomically(strFileName, temporary => pck.SaveAs(new FileInfo(temporary)));
                    //Common_Data.DisplayMessage(rtb_Log, $"File Saved : {strFileName}", false, -1, Color.DarkBlue);
                    bgw_List.ReportProgress(0, new BackGroundWorkerUserObjectForReport(0, 0, $"File Saved : {strFileName}"));

                }
                #endregion
            }

            catch (Exception ex)
            {
                operationFailed = true;
                bgw_List.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex));
            }

        }

        private void btn_ClearLogs_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(bgw_List.IsBusy))
                {
                    rtb_Log.ResetText();
                    UpdateProgressBar(0, 1, "Reset");
                    Common_Data.DisplayMessage(rtb_Log, $"Logs Cleared");
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, "Process is in Progress, Logs cann't be cleared", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }


        private void UpdateProgressBar(int count, int totalCount, string status)
        {
            try
            {
                if (status.ToLower() == "update")
                {
                    progressBar1.Value = (count * 100) / totalCount;
                    Common_Data.DisplayMessage(rtb_Log, $"Processing....{progressBar1.Value} % Completed", false, count.ToString().Length);
                }
                if (status.ToLower() == "tableupdate")
                {
                    progressBar1.Value = (count * 100) / totalCount;
                }
                if (status.ToLower() == "reset")
                {
                    progressBar1.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
            finally
            {
                progressBar1.Update();
            }
        }

        private void UpdateProgressBar(int count, int totalCount)
        {
            try
            {
                progressBar1.Value = (count * 100) / totalCount;
                progressBar1.Update();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_DB_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab == tabPage_DB)
                {
                    Common_Data.DisplayMessage(rtb_Log, "DB Selected");
                    DefaultSettings(1);
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DefaultSettings(int int_default_Case)
        {
            try
            {
                //Point point = gb_DBConnection.Location;
                switch (int_default_Case)
                {
                    case 1:
                        GroupBox[] gblist = { gb_Source1, gb_Target1 };

                        foreach (GroupBox gb in gblist)
                        {
                            foreach (Control c in gb.Controls)
                            {
                                if (c is TextBox)
                                {
                                    c.ResetText();
                                }
                            }
                        }

                        rb_SRC_oracle.Checked = true;
                        cb_WindowAuthentication_SRC.Checked = false;
                        cb_SRC_DefaultPort.Checked = true;
                        tb_SRC_DefaultPort.Text = "1521";

                        rb_TRG_Oracle.Checked = true;
                        cb_WindowAuthentication_TRG.Checked = false;
                        cb_TRG_DefaultPort.Checked = true;
                        tb_TRG_DefaultPort.Text = "1521";

                        gb_DBConnection.Visible = true;
                        gb_Excel.Visible = false;

                        gb_Logs.Location = point_logs;

                        break;

                    case 2:
                        llb_Excel_SRC.Text = "No Source Folder Selected";
                        llb_Excel_TRG.Text = "No Target Folder Selected";
                        gb_DBConnection.Visible = false;
                        gb_Excel.Visible = true;
                        gb_Excel.Location = point_db;

                        point_excel = new Point(gb_Excel.Location.X, gb_Excel.Location.Y + gb_Excel.Size.Height);
                        gb_Logs.Location = point_excel;

                        break;

                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_Excel_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab == tabPage_Excel)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Excel Selected");
                    DefaultSettings(2);
                }

                if (GlobalDebug.boolIsGlobalDebug & tabControl1.SelectedTab == tabPage_Excel)
                {
                    folderBrowserDialog1.SelectedPath = @"D:\Krishna\Projects\SPI\INPUT FILES\compare_multiple\MultiLevel Output\Output";

                    l_lbl_outputFolder.Text = folderBrowserDialog1.SelectedPath;
                    strFolderPath = folderBrowserDialog1.SelectedPath;

                    boolFolderPathSelected = true;

                    llb_Excel_SRC.Text = @"D:\Krishna\Projects\SPI\INPUT FILES\compare_multiple\MultiLevel Output\Source";
                    llb_Excel_TRG.Text = @"D:\Krishna\Projects\SPI\INPUT FILES\compare_multiple\MultiLevel Output\Target";

                    boolSRC_ExcelFile = true;
                    boolTRG_ExcelFile = true;
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void llb_Excel_SRC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    strFolderPath = folderBrowserDialog1.SelectedPath;
                    llb_Excel_SRC.Text = strFolderPath;
                    Common_Data.DisplayMessage(rtb_Log, $"Folder path set to :{strFolderPath}", false, -1, Color.DarkBlue);
                    boolSRC_ExcelFile = true;
                }
                else
                {
                    strFolderPath = string.Empty;
                    llb_Excel_SRC.Text = "No Source Folder Selected";
                    Common_Data.DisplayMessage(rtb_Log, $"Source folder path not set, aborting all operation", true);
                    boolSRC_ExcelFile = false;
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void llb_Excel_TRG_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    strFolderPath = folderBrowserDialog1.SelectedPath;
                    llb_Excel_TRG.Text = strFolderPath;
                    Common_Data.DisplayMessage(rtb_Log, $"Folder path set to :{strFolderPath}", false, -1, Color.DarkBlue);
                    boolTRG_ExcelFile = true;
                }
                else
                {
                    strFolderPath = string.Empty;
                    llb_Excel_TRG.Text = strFolderPath;
                    Common_Data.DisplayMessage(rtb_Log, $"Target folder path not set, aborting all operation", true);
                    boolTRG_ExcelFile = false;
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }





        private void bgw_List_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                operationFailed = false;
                boolCompare_List = false;
                boolCompare_List = Compare_List(e);
            }
            catch (Exception ex)
            {
                operationFailed = true;
                bgw_List.ReportProgress(5, new BackGroundWorkerUserObjectForReport(ex));
            }
        }

        private void bgw_List_ProgressChanged(object sender, ProgressChangedEventArgs e)
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

        private void BGW_ProgressChanged(ProgressChangedEventArgs e)
        {
            try
            {
                //0 means only msg
                //1 means progress count
                //2 means DebugMsg
                //3 means red color msg or not satisfying condition
                //4 reset

                backGroundWorkerObject = (BackGroundWorkerUserObjectForReport)e.UserState;

                if (e.ProgressPercentage == 0)
                {
                    Common_Data.DisplayMessage(rtb_Log, backGroundWorkerObject.strMsg);
                }

                if (e.ProgressPercentage == 1)
                {
                    Common_Data.UpdateProgressBar(progressBar1, backGroundWorkerObject.int_Count, backGroundWorkerObject.int_TotalCount);
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
                    Common_Data.UpdateProgressBar(progressBar1, backGroundWorkerObject.int_Count, backGroundWorkerObject.int_TotalCount);
                    Common_Data.DisplayMessage(rtb_Log, backGroundWorkerObject.strMsg, true);
                }

                if (e.ProgressPercentage == 5 || backGroundWorkerObject.bgwException != null)
                {
                    Common_Data.DisplayError(rtb_Log, backGroundWorkerObject.bgwException);
                }

                if (e.ProgressPercentage == 6)
                {
                    UpdateProgressBar(++int_Table_Progress_Count, dsTableList.Tables[0].Rows.Count, backGroundWorkerObject.strMsg);
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void bgw_List_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            operationInputs?.Dispose();
            operationInputs = null;
            try
            {
                timer1.Enabled = false;
                if (e.Error != null)
                {
                    Common_Data.DisplayError(rtb_Log, e.Error);
                    return;
                }
                if (operationFailed)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Batch comparison finished with errors; review the log. Partial outputs may remain.", true);
                    return;
                }
                if (boolCompare_List && !e.Cancelled)
                {
                    if (dtSummaryForList.Rows.Count > 0)
                    {
                        strFileName = $@"{strFolderPath}\Summary List of All Tables_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                        WriteExcel.ExportToExcel(dtSummaryForList, "Summary List of All Tables", strFileName);
                        Common_Data.DisplayMessage(rtb_Log, "Summary List of All Tables Saved Successfully !!!");
                    }
                    Common_Data.DisplayMessage(rtb_Log, "List Processed Completed SuccessFully, Output Folder can be opened now !!!", false, -1, Color.DarkGreen);
                    compareFinished = true;
                    UpdateProgressBar(1, 1, "tableupdate");
                }
                else if (e.Cancelled)
                {
                    Common_Data.DisplayMessage(rtb_Log, "All Process Cancelled Successfully", true);
                }
                else
                {
                    UpdateProgressBar(0, 1, "tableupdate");
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
            finally
            {
                steps.Busy = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (bgw_List.IsBusy)
                {
                    Common_Data.DisplayMessage(rtb_Log, "User tried to cancel the process", true);
                    bgw_List.CancelAsync();
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, "No Process To Cancel !!!", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void tabPage_Excel_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateProgressBar(++int_timer, 100);
                if (int_timer > 99)
                {
                    timer1.Interval = 500;
                    int_timer = 1;
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }


        private void l_lbl_outputFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (strFolderPath != null & strFolderPath != string.Empty & boolFolderPathSelected != false)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Opening folder path : {strFolderPath}");
                    Process.Start(strFolderPath);
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, "Pls set the output folder to open", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
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
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }


        private void l_lb_Output_Open_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (strFolderPath != null & strFolderPath != string.Empty & boolFolderPathSelected != false)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Opening folder path : {strFolderPath}");
                    Process.Start(strFolderPath);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void l_lb_Output_Set_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    strFolderPath = folderBrowserDialog1.SelectedPath;
                    l_lbl_outputFolder.Text = strFolderPath;
                    Common_Data.DisplayMessage(rtb_Log, $"Folder path set to :{strFolderPath}");
                    boolFolderPathSelected = true;
                }
                else
                {
                    strFolderPath = string.Empty;
                    l_lbl_outputFolder.Text = strFolderPath;
                    Common_Data.DisplayMessage(rtb_Log, $"Folder path not set, aborting all operation");
                    boolFolderPathSelected = false;
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
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
            return FormValidation.Check(this, () =>
            {
                FormValidation.IsBusy(bgw_List, btn_TestDBConnection);
                connection.ValidateInput();
            }, message => Common_Data.DisplayMessage(rtb_Log, message, true));
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
                if (!connection.AddChecks(InputCheck.For(this)).Report()) return;
                if (!(ValidateMyControls()))
                {
                    return;
                }
                str_DBNameSRC = tb_SRC_DBName.Text.Trim();
                str_DBNameTRG = tb_TRG_DBName.Text.Trim();

                if (rb_SRC_oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Oracle Source...");

                    dbConnection = common_Data.DBConnectionTest(rb_SRC_oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBNameSRC, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text, tb_SRC_DefaultPort.Text.Trim());

                }
                else if (rb_SRC_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to SQL Source...");
                    dbConnection = common_Data.DBConnectionTest(rb_SRC_SQL.Text, tb_SRC_DataSource.Text.Trim(), str_DBNameSRC, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text, tb_SRC_DefaultPort.Text.Trim());
                }

                dbSRCConnection = dbConnection;
                boolSRCConnection = dbConnection.boolConnection;
                Common_Data.DisplayMessage(rtb_Log, $"Source : {dbConnection.strConnectionMSG}", (!dbConnection.boolConnection), -1, GetStatusColor(dbConnection.boolConnection));

                if (rb_TRG_Oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Oracle Target...");

                    dbConnection = common_Data.DBConnectionTest(rb_TRG_Oracle.Text, tb_TRG_DataSource.Text.Trim(), str_DBNameTRG, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text, tb_TRG_DefaultPort.Text.Trim());
                }
                else if (rb_TRG_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to SQL Target...");
                    dbConnection = common_Data.DBConnectionTest(rb_TRG_SQL.Text, tb_TRG_DataSource.Text.Trim(), str_DBNameTRG, cb_WindowAuthentication_TRG.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text, tb_TRG_DefaultPort.Text.Trim());
                }
                dbTRGConnection = dbConnection;
                boolTRGConnection = dbConnection.boolConnection;
                Common_Data.DisplayMessage(rtb_Log, $"Target : {dbConnection.strConnectionMSG}", (!dbConnection.boolConnection), -1, GetStatusColor(dbConnection.boolConnection));

                if (boolSRCConnection & boolTRGConnection)
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

        private void btn_Compare_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateBatchInputs()) return;
                strFolderPath = l_lbl_outputFolder.Text;
                boolRules = common_Data.GetRulesStatus();

                if (cb_IncludeRule.Checked && boolRules)
                {
                    //boolRules = common_Data.GetRulesStatus();
                    dsRules = common_Data.GetDatasetRules();
                }

                if (tabControl1.SelectedTab == tabPage_Excel)
                {
                    boolLoadDataBase = false;
                    if (boolSRC_ExcelFile && boolTRG_ExcelFile)
                    {
                        boolExcelFileForComparison = true;
                    }
                    else
                    {
                        boolExcelFileForComparison = false;
                    }
                }

                if (boolExcelFileList & boolFolderPathSelected & (boolExcelFileForComparison || boolLoadDataBase))
                {
                    if (!(bgw_List.IsBusy))
                    {
                        timer1.Enabled = true;
                        boolSelectTabDB = tabControl1.SelectedTab == tabPage_DB ? true : false;
                        boolSelectTabExcel = tabControl1.SelectedTab == tabPage_Excel ? true : false;
                        compareFinished = false;
                        steps.Busy = true;
                        try { FormValidation.StartWorker(this, bgw_List, ref operationInputs, btnCancel); }
                        catch { steps.Busy = false; throw; }
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, "Process is already in progress, pls cancel current process and rerun the new list if required !!!", true);
                    }
                }
                else
                {
                    if (!(boolExcelFileList))
                    {
                        Common_Data.DisplayMessage(rtb_Log, "Input table list excel file is not selected", true);
                    }

                    if (!(boolLoadDataBase) && tabControl1.SelectedTab == tabPage_DB)
                    {
                        Common_Data.DisplayMessage(rtb_Log, "DB Not Set Properly", true);
                    }

                    if (!(boolExcelFileForComparison) && tabControl1.SelectedTab == tabPage_Excel)
                    {
                        Common_Data.DisplayMessage(rtb_Log, "Excel Source Or Target folders is Not Set.", true);
                    }

                    if (!(boolFolderPathSelected))
                    {
                        Common_Data.DisplayMessage(rtb_Log, "Output folder not set", true);
                    }
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
        internal void ValidateBatchWorkbook(DataSet candidate)
        {
            FormValidation.Require(candidate != null && candidate.Tables.Count > 0, btn_ListTableExcel, "Load a workbook containing a table list.");
            DataTable table = candidate.Tables[0];
            FormValidation.Table(table, btn_ListTableExcel, "Table list");
            FormValidation.Columns(table, new[] { "src_table_names", "trg_table_names", "primary_key_columns", "ignore_columns" }, btn_ListTableExcel, "Table list");
            foreach (DataRow row in table.Rows)
                foreach (string column in new[] { "src_table_names", "trg_table_names", "primary_key_columns" })
                    FormValidation.Require(!string.IsNullOrWhiteSpace(Convert.ToString(row[column])), btn_ListTableExcel,
                        "Every table-list row needs source, target and reference-key column names.");
        }

        internal bool ValidateBatchInputs()
        {
            return FormValidation.Check(this, () =>
            {
                FormValidation.IsBusy(bgw_List, btn_Compare);
                FormValidation.Require(boolExcelFileList, btn_ListTableExcel, "Load a valid table-list workbook first.");
                ValidateBatchWorkbook(dsTableList);
                FormValidation.ExistingDirectory(l_lbl_outputFolder.Text, l_lbl_outputFolder, "Output folder");
                if (tabControl1.SelectedTab == tabPage_DB)
                {
                    connection.ValidateInput();
                    FormValidation.Require(boolLoadDataBase && boolSRCConnection && boolTRGConnection, btn_TestDBConnection,
                        "Test both current database connections again before comparing.");
                }
                else
                {
                    FormValidation.Require(tabControl1.SelectedTab == tabPage_Excel, tabControl1, "Select Database or Excel comparison.");
                    FormValidation.ExistingDirectory(llb_Excel_SRC.Text, llb_Excel_SRC, "Source workbook folder");
                    FormValidation.ExistingDirectory(llb_Excel_TRG.Text, llb_Excel_TRG, "Target workbook folder");
                    foreach (DataRow row in dsTableList.Tables[0].Rows)
                    {
                        string source = Convert.ToString(row["src_table_names"]);
                        string target = Convert.ToString(row["trg_table_names"]);
                        FormValidation.FileName(source, btn_ListTableExcel, "Source workbook name");
                        FormValidation.FileName(target, btn_ListTableExcel, "Target workbook name");
                        FormValidation.ReadableFile(System.IO.Path.Combine(llb_Excel_SRC.Text, source + ".xlsx"), llb_Excel_SRC, "Source workbook", ".xlsx");
                        FormValidation.ReadableFile(System.IO.Path.Combine(llb_Excel_TRG.Text, target + ".xlsx"), llb_Excel_TRG, "Target workbook", ".xlsx");
                    }
                }
                if (cb_IncludeRule.Checked) FormValidation.Require(common_Data.GetRulesStatus(), cb_IncludeRule,
                    "Import valid rules, or clear Include rules before comparing.");
            }, message => Common_Data.DisplayMessage(rtb_Log, message, true));
        }
    }
}
