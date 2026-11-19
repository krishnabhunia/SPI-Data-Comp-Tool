using DocumentFormat.OpenXml.Office.CustomUI;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;

namespace SPI_Data_Comp_Tool
{
    public partial class SQL_DB_To_Excel : Form
    {
        private IDisposable operationInputs;
        private string strQuery;
        private string strSQLConnection;
        private string str_CompleteTableName;

        private DataTable dt_sql;
        private DataTable dt;
        private DataSet ds;
        private SqlDataAdapter sqlDA;

        //private SaveFileDialog saveFileDialogExcel;
        private ExcelWorksheet worksheet;
        private OracleDataAdapter oracleDA;
        private DataTable dt_oracle;
        private Exception myException;
        private DBConnectionDetails dBConnectionDetails;

        // The connection fields live in the shared ConnectionPanel; these keep the names the code below uses.
        private System.Windows.Forms.ComboBox cb_UserProfileList => connection.ProfileList;
        private System.Windows.Forms.RadioButton rb_Oracle => connection.Source.Oracle;
        private System.Windows.Forms.RadioButton rb_SQL => connection.Source.Sql;
        private System.Windows.Forms.CheckBox cb_windowsAuthentication => connection.Source.WindowsLogin;
        private System.Windows.Forms.CheckBox cb_DefaultPort => connection.Source.DefaultPort;
        private System.Windows.Forms.TextBox tb_DefaultPort => connection.Source.Port;
        private System.Windows.Forms.TextBox tb_SRC_DataSource => connection.Source.DataSource;
        private System.Windows.Forms.TextBox tb_SRC_DBName => connection.Source.Database;
        private System.Windows.Forms.TextBox tb_SRC_UserName => connection.Source.User;
        private System.Windows.Forms.TextBox tb_SRC_Password => connection.Source.Password;

        private Common_Data common_Data;

        private DBTOExcel dBTOExcel;

        private List<string> lstTableDetails;
        private bool tablesLoaded;
        private string[] exportEntries;
        private string exportFolder;
        private bool exportSkipEmpty;

        public SQL_DB_To_Excel()
        {
            InitializeComponent();
            FormClosing += (sender, args) =>
            {
                if (backgroundWorker1.IsBusy || backgroundWorker2.IsBusy)
                {
                    args.Cancel = true;
                    FormValidation.Check(this, () => FormValidation.Reject(btn_Cancel,
                        "Wait for the current operation to finish or cancel it before closing this window."),
                        message => DisplayMessage(message));
                }
            };
            connection.Mode = ConnectionMode.SourceOnly;
            connection.Source.Title = "Database";
            connection.ProfileHalf = rb_TRG.Checked ? ProfileHalf.Target : ProfileHalf.Source;
            connection.ProfileApplied += Connection_ProfileApplied;
            connection.Failed += (sender, error) => DisplayError(error);
            connection.InputChanged += (sender, args) => { tablesLoaded = false; };
            UiTheme.Apply(this, btn_ExportToExcel);

            ds = new DataSet();
            dt_sql = new DataTable();
            dt = new DataTable();
            sqlDA = new SqlDataAdapter();
            oracleDA = new OracleDataAdapter();
            dt_oracle = new DataTable();
            myException = new Exception();

            common_Data = new Common_Data();
            lstTableDetails = new List<string>();

            common_Data.ReadUserProfile(cb_UserProfileList);

            if (Debugger.IsAttached)
            {
                if (Environment.UserName.ToString() == "KRISHNA")
                {
                    tb_SRC_DataSource.Text = @"KRISHNA-HP\KRISHNASQL2019";
                    tb_SRC_DBName.Text = "db_krishna";
                    tb_SRC_UserName.Text = "sa";
                    tb_SRC_Password.Text = "";
                }
                else if (Environment.MachineName.ToString() == "localhost")
                {
                    tb_SRC_DataSource.Text = @"localhost";
                    tb_SRC_DBName.Text = "training_p";
                    tb_SRC_UserName.Text = "sa";
                    tb_SRC_Password.Text = "";
                }
                linkLabel1.Text = @"D:\Krishna\Output\Export DB To Excel";
            }

        }

        /// <summary>A saved profile was put into the Connection panel.</summary>
        private void Connection_ProfileApplied(object sender, DBConnectionDetails details)
        {
            dBConnectionDetails = details;
            DisplayMessage($"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
        }

        /// <summary>Fill the connection from the profile's Source or Target half (the "Fill from profile half" choice).</summary>
        private void UseProfileHalf(ProfileHalf half)
        {
            connection.ProfileHalf = half;
            if (cb_UserProfileList.SelectedIndex >= 0)
            {
                connection.ApplySelectedProfile();
            }
        }

        private void btn_ExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateExportInputs()) return;
                myException = null;
                exportEntries = new string[clb_TableList.CheckedItems.Count];
                for (int index = 0; index < exportEntries.Length; index++)
                    exportEntries[index] = Convert.ToString(clb_TableList.CheckedItems[index]);
                exportFolder = linkLabel1.Text;
                exportSkipEmpty = cb_SkipEmptyTable.Checked;
                if (!(backgroundWorker2.IsBusy))
                {
                    toggleControls(false);
                    FormValidation.StartWorker(this, backgroundWorker2, ref operationInputs, btn_Cancel);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void DetectDateColumnAndFormat()
        {
            try
            {
                DateTime var_date;
                int col = 0;
                foreach (DataTable dt_ in ds.Tables)
                {
                    col = 0;
                    foreach (DataColumn dc in dt_.Columns)
                    {
                        if (dt_.Rows.Count > 0)
                        {
                            col++;
                            Type type_type = dc.GetType();
                            bool boolIsConvertedIntoDate = DateTime.TryParse(dt_.Rows[0][dc].ToString(), out var_date);
                            if (boolIsConvertedIntoDate)
                            {
                                worksheet.Column(col).Style.Numberformat.Format = "dd/MM/yyyy";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }

        }

        private void toggleControls(bool boolEnableStatus)
        {
            try
            {
                connection.Enabled = boolEnableStatus;
                btn_ExportToExcel.Enabled = boolEnableStatus;
                tb_Find.Enabled = boolEnableStatus;
                linkLabel1.Enabled = boolEnableStatus;
                cb_UserProfileList.Enabled = boolEnableStatus;
                rb_Oracle.Enabled = boolEnableStatus;
                rb_SQL.Enabled = boolEnableStatus;
                cb_windowsAuthentication.Enabled = boolEnableStatus;
                cb_DefaultPort.Enabled = boolEnableStatus;
                tb_DefaultPort.Enabled = boolEnableStatus;

                tb_SRC_DataSource.Enabled = boolEnableStatus;
                tb_SRC_DBName.Enabled = boolEnableStatus;
                tb_SRC_UserName.Enabled = boolEnableStatus;
                tb_SRC_Password.Enabled = boolEnableStatus;
                btn_Connect.Enabled = boolEnableStatus;

                groupBox3.Enabled = boolEnableStatus;

                cb_SelectAll.Enabled = boolEnableStatus;
                cb_SkipEmptyTable.Enabled = boolEnableStatus;
                if (boolEnableStatus)
                {
                    clb_TableList.SelectionMode = SelectionMode.One;
                }
                else
                {
                    clb_TableList.SelectionMode = SelectionMode.None;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateLoadInputs()) return;
                tablesLoaded = false;
                tb_Find.ResetText();
                cb_SelectAll.CheckState = CheckState.Unchecked;
                cb_SelectAll.Update();
                clb_TableList.Items.Clear();
                lstTableDetails.Clear();
                if (!(backgroundWorker1.IsBusy))
                {
                    toggleControls(false);
                    FormValidation.StartWorker(this, backgroundWorker1, ref operationInputs, btn_Cancel);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private string TableToExclude(string tableName = null)
        {
            string str_return = string.Empty;
            try
            {
                //string[] str_tableName = { "SYS", "SYSTEM", "SYSMAN", "ORDDATA", "EXFSYS", "MDSYS", "WMSYS", "CTXSYS", "APEX_030200" };
                string[] str_tableName = { "SYSAUX", "SYSTEM" };

                //tableName.Split(',');  // {"SYS" , "SYSTEM", "SYSMAN" , "ORDDATA" };
                foreach (string str in str_tableName)
                {
                    //str_return = $"OWNER != '{str}' AND {str_return}";
                    str_return = $"tablespace_name != '{str}' AND {str_return}";
                }

                str_return = str_return.Substring(0, str_return.Length - 4);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return str_return;
        }

        private void UpdateProgressBar(int Value, int TotalValue, string str_Status)
        {
            try
            {
                int int_percentage = (Value * 100) / TotalValue;
                progressBar1.Value = int_percentage;
                progressBar1.Update();
                lbl_Status.Text = $" {int_percentage} % Completed, {str_Status}";
                lbl_Status.Update();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void UpdateProgressBarReport(BackgroundWorker bgw, int Value, int TotalValue, string str_Status)
        {
            try
            {
                int int_percentage = (Value * 100) / TotalValue;
                bgw.ReportProgress(int_percentage, $" {int_percentage} % Completed, {str_Status}");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void UpdateProgressBarReport(BackgroundWorker bgw, int Value, int TotalValue, DBTOExcel dBTO)
        {
            try
            {
                int int_percentage = (Value * 100) / TotalValue;
                bgw.ReportProgress(int_percentage, dBTO);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_SelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                lbl_Status.ResetText();
                checkBoxListToggle(cb_SelectAll.Checked);
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void checkBoxListToggle(bool enabledState)
        {
            for (int i = 0; i < clb_TableList.Items.Count; i++)
            {
                clb_TableList.SetItemChecked(i, enabledState);
            }
        }

        //private void FillCheckListBox(DataTable _dt, string str_dbType)
        //{
        //    try
        //    {
        //        clb_TableList.Items.Clear();
        //        UpdateProgressBar(1, 1, "All Updated");
        //        string str_tableName;

        //        if (str_dbType == "oracle")
        //        {
        //            foreach (DataRow dr in _dt.Rows)
        //            {
        //                if (dr["OWNER"].ToString() != "SYS")
        //                {
        //                    str_tableName = $"{dr["OWNER"].ToString()}.{dr["TABLE_NAME"].ToString()}";
        //                    clb_TableList.Items.Add(str_tableName + "  [ " + ds.Tables[str_tableName].Rows.Count + " ]");
        //                }
        //            }
        //        }
        //        else if (str_dbType == "sql")
        //        {
        //            foreach (DataRow dr in _dt.Rows)
        //            {
        //                str_tableName = $"{dr["TABLE_CATALOG"].ToString()}.{dr["TABLE_SCHEMA"].ToString()}.{dr["TABLE_NAME"].ToString()}";
        //                clb_TableList.Items.Add(str_tableName + "  [ " + ds.Tables[str_tableName].Rows.Count + " ]");
        //            }
        //        }
        //        clb_TableList.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        lbl_Status.Text = ex.Message.ToString();
        //    }
        //}

        private void FillCheckListBox(string strData, int int_No_Rows)
        {
            try
            {
                string s = strData + "  [ " + int_No_Rows + " ]";
                lstTableDetails.Add(s);
                clb_TableList.Items.Add(s);
                clb_TableList.Update();
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                FolderBrowserDialog folderDlg = new FolderBrowserDialog
                {
                    ShowNewFolderButton = true,
                    SelectedPath = linkLabel1.Text,
                    Description = "Save Excel"
                };

                if (folderDlg.ShowDialog() == DialogResult.OK)
                {
                    linkLabel1.Text = folderDlg.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void DisplayError(Exception ex)
        {
            lbl_Status.Text = ex.Message.ToString();
            if (Debugger.IsAttached)
            {
                lbl_Status.Text = ex.ToString();
            }
            lbl_Status.Update();
            lbl_Status.ForeColor = Color.Red;
        }

        private void DisplayMessage(string strMsg)
        {
            lbl_Status.ForeColor = Color.Black;
            lbl_Status.Text = strMsg;
            lbl_Status.Update();
        }


        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            myException = null;
            int count = 0;
            try
            {
                backgroundWorker1.ReportProgress(0, "Load");

                ds = new DataSet();
                dt = new DataTable();
                dt_sql = new DataTable();
                dt_oracle = new DataTable();

                if (rb_Oracle.Checked)
                {
                    strQuery = $"select * from all_tables where {TableToExclude()} ORDER BY OWNER, TABLE_NAME";

                    if (Debugger.IsAttached)
                    {
                        strQuery = $"select * from all_tables where {TableToExclude()} AND ROWNUM <= 50 ORDER BY OWNER, TABLE_NAME";
                    }
                    strSQLConnection = ConnectionStrings.Build("oracle", tb_SRC_DataSource.Text, tb_SRC_DBName.Text, cb_windowsAuthentication.Checked, tb_SRC_UserName.Text, tb_SRC_Password.Text, tb_DefaultPort.Text);

                    oracleDA = new OracleDataAdapter(strQuery, strSQLConnection);
                    oracleDA.Fill(dt_oracle);
                    dt_oracle.TableName = "List of DB Tables";

                    foreach (DataRow dr in dt_oracle.Rows)
                    {

                        UpdateProgressBarReport(backgroundWorker1, ++count, dt_oracle.Rows.Count, $"{count}/{dt_oracle.Rows.Count} - Updating Tables");

                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            backgroundWorker1.ReportProgress(0, "Cancel");
                            return;
                        }

                        str_CompleteTableName = $"{dr["OWNER"].ToString()}.{dr["TABLE_NAME"].ToString()}";
                        strQuery = $"Select count(*) \"count\" From {str_CompleteTableName}";
                        dt = new DataTable();
                        oracleDA.SelectCommand.CommandText = strQuery;
                        oracleDA.SelectCommand.CommandType = CommandType.Text;
                        oracleDA.Fill(dt);
                        dt.TableName = str_CompleteTableName;
                        dBTOExcel = new DBTOExcel(str_CompleteTableName, Convert.ToInt32(dt.Rows[0]["count"].ToString()));
                        UpdateProgressBarReport(backgroundWorker1, count, dt_oracle.Rows.Count, dBTOExcel);
                    }

                }
                else if (rb_SQL.Checked)
                {
                    strQuery = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";
                    if (cb_windowsAuthentication.CheckState == CheckState.Checked)
                    {
                        strSQLConnection = ConnectionStrings.Build("sql", tb_SRC_DataSource.Text, tb_SRC_DBName.Text, cb_windowsAuthentication.Checked, tb_SRC_UserName.Text, tb_SRC_Password.Text, tb_DefaultPort.Text);
                    }
                    else if (cb_windowsAuthentication.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = ConnectionStrings.Build("sql", tb_SRC_DataSource.Text, tb_SRC_DBName.Text, cb_windowsAuthentication.Checked, tb_SRC_UserName.Text, tb_SRC_Password.Text, tb_DefaultPort.Text);
                    }

                    dt_sql = new DataTable();
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_sql);
                    ds.Tables.Add(dt_sql);
                    ds.Tables[0].TableName = "List of DB Tables";

                    foreach (DataRow dr in dt_sql.Rows)
                    {
                        UpdateProgressBarReport(backgroundWorker1, ++count, dt_sql.Rows.Count, $"{count}/{dt_sql.Rows.Count} - Updating Tables");

                        if (backgroundWorker1.CancellationPending)
                        {
                            e.Cancel = true;
                            backgroundWorker1.ReportProgress(0, "Cancel");
                            return;
                        }

                        str_CompleteTableName = dr["TABLE_CATALOG"].ToString() + "." + dr["TABLE_SCHEMA"].ToString() + "." + dr["TABLE_NAME"].ToString();

                        strQuery = "if object_id('" + str_CompleteTableName + "', 'U') IS NOT NULL" +
                            " BEGIN " +
                            "select count(*) 'count' from " + str_CompleteTableName + " " +
                            "END " +
                            "ELSE " +
                            "BEGIN " +
                            "PRINT 'NOT OK' " +
                            "END";

                        sqlDA.SelectCommand.CommandText = strQuery;
                        dt.Clear();
                        sqlDA.Fill(dt);
                        dt.TableName = str_CompleteTableName;

                        dBTOExcel = new DBTOExcel(str_CompleteTableName, Convert.ToInt32(dt.Rows[0]["count"].ToString()));
                        UpdateProgressBarReport(backgroundWorker1, count, dt_sql.Rows.Count, dBTOExcel);
                    }
                }
            }
            catch (Exception ex)
            {
                if (backgroundWorker1.IsBusy)
                {
                    e.Cancel = true;
                    backgroundWorker1.ReportProgress(0, "Error");
                    myException = ex;
                    return;
                }
            }
        }



        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            try
            {
                progressBar1.Value = e.ProgressPercentage;
                progressBar1.Update();

                if (e.UserState.ToString() == "SPI_Data_Comp_Tool.DBTOExcel")
                {
                    DBTOExcel dBTOExcel_Fill = (DBTOExcel)e.UserState;
                    FillCheckListBox(dBTOExcel_Fill.strTableName, dBTOExcel_Fill.int_No_Rows);
                }
                else
                {
                    DisplayMessage(e.UserState.ToString());
                }

                if (e.UserState.ToString() == "Error")
                {
                    DisplayError(myException);
                }

                if (e.UserState.ToString() == "Load")
                {
                    DisplayMessage("Loading, Please wait !!!");
                }

                if (e.UserState.ToString() == "Cancel")
                {
                    DisplayMessage("User clicked cancel, Cancelling all progress !!!");
                }

                if (e.UserState.ToString() == "Uncheck")
                {
                    if (cb_SelectAll.Checked)
                    {
                        cb_SelectAll.Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            operationInputs?.Dispose();
            operationInputs = null;
            try
            {
                tablesLoaded = e.Error == null && !e.Cancelled && myException == null;
                if (e.Error != null) { DisplayError(e.Error); return; }
                if (e.Cancelled && myException == null)
                {
                    DisplayMessage("All Progress cancelled");
                }
                else if (!(e.Cancelled))
                {
                    //if (rb_Oracle.Checked)
                    //{
                    //    FillCheckListBox(dt_src, "oracle");
                    //}
                    //else if (rb_SQL.Checked)
                    //{
                    //    FillCheckListBox(dt_trg, "sql");
                    //}

                    cb_SelectAll.CheckState = CheckState.Checked;
                    lbl_Status.Text = "All loaded successfully";
                    lbl_Status.Update();
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                toggleControls(true);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.CancelAsync();
                    DisplayMessage("Cancelled clicked, cancelling, pls wait !!!");
                }

                if (backgroundWorker2.IsBusy)
                {
                    backgroundWorker2.CancelAsync();
                    DisplayMessage("Cancelled clicked, cancelling, pls wait !!!");
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private DataTable LoadTable(string str_CompleteTableName)
        {
            try
            {
                dt = new DataTable();

                if (rb_Oracle.Checked)
                {
                    strQuery = $"Select * From {str_CompleteTableName}";
                    oracleDA.SelectCommand.CommandText = strQuery;
                    oracleDA.SelectCommand.CommandType = CommandType.Text;
                    oracleDA.Fill(dt);
                }

                if (rb_SQL.Checked)
                {
                    strQuery = "if object_id('" + str_CompleteTableName + "', 'U') IS NOT NULL" +
                            " BEGIN " +
                            "select* from " + str_CompleteTableName + " " +
                            "END " +
                            "ELSE " +
                            "BEGIN " +
                            "PRINT 'NOT OK' " +
                            "END";

                    sqlDA.SelectCommand.CommandText = strQuery;
                    sqlDA.Fill(dt);
                    dt.TableName = str_CompleteTableName;
                }

                return dt;
            }
            catch (Exception ex)
            {
                if (backgroundWorker2.IsBusy)
                {
                    backgroundWorker2.ReportProgress(0, "Error");
                    myException = ex;
                }
                throw ex;
            }
        }
        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int int_count = 0;
            int int_totalCount = 0;
            try
            {
                //lbl_Status.Text = "Please wait excel file is being written and saved";
                //lbl_Status.Update();

                //UpdateProgressBarReport(backgroundWorker2,1, 100, "Please wait excel file is being written and saved");
                backgroundWorker2.ReportProgress(0, "Please wait excel file is being written and saved");

                #region epplus code assembly
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                int_totalCount = exportEntries.Length;
                string itemCheck;

                foreach (string item in exportEntries)
                {
                    if (backgroundWorker2.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker2.ReportProgress(0, "Cancel");
                        return;
                    }

                    int_count++;
                    int int_Rows_Count;
                    FormValidation.Require(TryParseTableEntry(item, out itemCheck, out int_Rows_Count), null,
                        "Reload the table list: its table/count entry is invalid.");

                    //if (cb_SkipEmptyTable.Checked && ds.Tables[itemCheck.ToString()].Rows.Count == 0)
                    //{
                    //    continue;
                    //}

                    if (exportSkipEmpty && int_Rows_Count == 0)
                    {
                        continue;
                    }

                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        DataTable table = LoadTable(itemCheck);
                        FormValidation.ExcelTable(table, null, "Export table " + itemCheck);
                        worksheet = pck.Workbook.Worksheets.Add(FormValidation.WorksheetNameFromFile(itemCheck + ".xlsx"));
                        worksheet.Cells["A1"].LoadFromDataTable(table, true);

                        //DetectDateColumnAndFormat();

                        worksheet.Cells.AutoFitColumns();

                        string output = Path.Combine(exportFolder, item + ".xlsx");
                        FormValidation.WriteAtomically(output, temporary => pck.SaveAs(new FileInfo(temporary)));

                        UpdateProgressBarReport(backgroundWorker2, int_count, int_totalCount, $" {int_count}/{int_totalCount}, Saving File :- {output}");
                    }
                }


                #endregion

            }
            catch (Exception ex)
            {
                if (backgroundWorker2.IsBusy)
                {
                    e.Cancel = true;
                    backgroundWorker2.ReportProgress(0, "Error");
                    myException = ex;
                    return;
                }
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            try
            {
                progressBar1.Value = e.ProgressPercentage;
                progressBar1.Update();
                DisplayMessage(e.UserState.ToString());
                if (e.UserState.ToString() == "Error")
                {
                    DisplayError(myException);
                }

                if (e.UserState.ToString() == "Load")
                {
                    DisplayMessage("Loading, Please wait !!!");
                }

                if (e.UserState.ToString() == "Cancel")
                {
                    DisplayMessage("User clicked cancel, Cancelling all progress !!!");
                }

                if (e.UserState.ToString() == "Uncheck")
                {
                    if (cb_SelectAll.Checked)
                    {
                        cb_SelectAll.Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            operationInputs?.Dispose();
            operationInputs = null;
            try
            {
                if (e.Error != null) { DisplayError(e.Error); return; }
                if (e.Cancelled && myException == null)
                {
                    DisplayMessage("All Progress cancelled");
                }
                else if (!e.Cancelled && myException == null)
                {
                    UpdateProgressBar(1, 1, "All Excel File Saved Successfully");
                    Process.Start(linkLabel1.Text);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                toggleControls(true);
            }
        }


        private void rb_SRC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_SRC.Checked)
                {
                    UseProfileHalf(ProfileHalf.Source);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void rb_TRG_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_TRG.Checked)
                {
                    UseProfileHalf(ProfileHalf.Target);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void tb_Find_TextChanged(object sender, EventArgs e)
        {
            try
            {
                cb_SelectAll.Checked = false;
                clb_TableList.Items.Clear();
                lstTableDetails.FindAll(x => Common_Data.ContainWithIgnoreCase(x,tb_Find.Text)).ForEach(a => clb_TableList.Items.Add(a));
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
        internal bool ValidateLoadInputs()
        {
            return FormValidation.Check(this, () =>
            {
                FormValidation.IsBusy(backgroundWorker1, btn_Connect);
                FormValidation.IsBusy(backgroundWorker2, btn_ExportToExcel);
                connection.Source.ValidateInput();
            }, DisplayMessage);
        }

        internal bool ValidateExportInputs()
        {
            return FormValidation.Check(this, () =>
            {
                FormValidation.IsBusy(backgroundWorker1, btn_Connect);
                FormValidation.IsBusy(backgroundWorker2, btn_ExportToExcel);
                connection.Source.ValidateInput();
                FormValidation.Require(tablesLoaded, btn_Connect, "Load the table list again after changing the connection.");
                FormValidation.Require(clb_TableList.CheckedItems.Count > 0, clb_TableList, "Select at least one table to export.");
                FormValidation.ExistingDirectory(linkLabel1.Text, linkLabel1, "Output folder");
                foreach (object item in clb_TableList.CheckedItems)
                {
                    string label = Convert.ToString(item);
                    FormValidation.Require(lstTableDetails.Contains(label), clb_TableList, "A selected table is no longer in the loaded list.");
                    string table;
                    int rows;
                    FormValidation.Require(TryParseTableEntry(label, out table, out rows), clb_TableList,
                        "Reload the table list: its table/count entry is invalid.");
                    FormValidation.OutputFile(Path.Combine(linkLabel1.Text, label + ".xlsx"), linkLabel1, "Export workbook", ".xlsx");
                }
            }, DisplayMessage);
        }

        internal static bool TryParseTableEntry(string label, out string table, out int rows)
        {
            table = string.Empty;
            rows = 0;
            if (string.IsNullOrEmpty(label) || !label.EndsWith(" ]", StringComparison.Ordinal)) return false;
            int suffix = label.LastIndexOf("  [ ", StringComparison.Ordinal);
            if (suffix <= 0 || label.Length - suffix - 6 <= 0) return false;
            table = label.Substring(0, suffix);
            string count = label.Substring(suffix + 4, label.Length - suffix - 6);
            return !string.IsNullOrWhiteSpace(table) && int.TryParse(count, out rows) && rows >= 0;
        }

    }
}
