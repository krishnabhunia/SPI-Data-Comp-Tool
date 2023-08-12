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
using System.Windows.Forms;
using DataTable = System.Data.DataTable;

namespace SPI_Data_Comp_Tool
{
    public partial class SQL_DB_To_Excel : Form
    {
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

        private Common_Data common_Data;

        private DBTOExcel dBTOExcel;

        private List<string> lstTableDetails;

        public SQL_DB_To_Excel()
        {
            InitializeComponent();

            ds = new DataSet();
            dt_sql = new DataTable();
            dt = new DataTable();
            sqlDA = new SqlDataAdapter();
            oracleDA = new OracleDataAdapter();
            dt_oracle = new DataTable();
            myException = new Exception();

            common_Data = new Common_Data();
            lstTableDetails = new List<string>();

            cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);

            if (Debugger.IsAttached)
            {
                if (Environment.UserName.ToString() == "KRISHNA")
                {
                    tb_SRC_DataSource.Text = @"KRISHNA-HP\KRISHNASQL2019";
                    tb_SRC_DBName.Text = "db_krishna";
                    tb_SRC_UserName.Text = "sa";
                    tb_SRC_Password.Text = "1234";
                }
                else if (Environment.MachineName.ToString() == "BRTSP00375")
                {
                    tb_SRC_DataSource.Text = @"IESWKCT247";
                    tb_SRC_DBName.Text = "training_p";
                    tb_SRC_UserName.Text = "sa";
                    tb_SRC_Password.Text = "abcde@1234567";
                }
                linkLabel1.Text = @"D:\Krishna\Output\Export DB To Excel";
            }

        }

        private void btn_ExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(backgroundWorker2.IsBusy))
                {
                    toggleControls(false);
                    backgroundWorker2.RunWorkerAsync();
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
                tb_Find.ResetText();
                cb_SelectAll.CheckState = CheckState.Unchecked;
                cb_SelectAll.Update();
                clb_TableList.Items.Clear();
                lstTableDetails.Clear();
                if (!(backgroundWorker1.IsBusy))
                {
                    toggleControls(false);
                    backgroundWorker1.RunWorkerAsync();
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

        private void rb_Oracle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_Oracle.Checked)
                {
                    cb_windowsAuthentication.Checked = false;
                    cb_windowsAuthentication.Enabled = false;
                    cb_windowsAuthentication.Visible = false;
                    cb_DefaultPort.Enabled = true;
                    cb_DefaultPort.Checked = true;
                    cb_DefaultPort.Visible = true;

                    tb_DefaultPort.Text = "1521";
                    tb_DefaultPort.Enabled = true;
                    tb_DefaultPort.Visible = true;
                    tb_DefaultPort.ReadOnly = true;

                }

                if (Debugger.IsAttached)
                {
                    tb_SRC_DataSource.Text = "IESWKCT670";
                    tb_SRC_DBName.Text = "SPEL11G";
                    tb_SRC_UserName.Text = "system";
                    tb_SRC_Password.Text = "SPEL11G";
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void rb_SQL_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_SQL.Checked)
                {
                    cb_windowsAuthentication.Checked = false;
                    cb_windowsAuthentication.Enabled = true;
                    cb_windowsAuthentication.Visible = true;
                    cb_DefaultPort.Enabled = false;
                    cb_DefaultPort.Checked = false;
                    cb_DefaultPort.Visible = false;

                    tb_DefaultPort.ResetText();
                    tb_DefaultPort.Enabled = false;
                    tb_DefaultPort.Visible = false;
                    tb_DefaultPort.ReadOnly = false;
                }

                if (Debugger.IsAttached)
                {
                    tb_SRC_DataSource.Text = @"IESWKCT247";
                    tb_SRC_DBName.Text = "training_p";
                    tb_SRC_UserName.Text = "sa";
                    tb_SRC_Password.Text = "abcde@1234567";
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_DefaultPort_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_DefaultPort.CheckState == CheckState.Checked)
                {
                    tb_DefaultPort.Text = "1521";
                    tb_DefaultPort.ReadOnly = true;
                }
                else
                {
                    tb_DefaultPort.ResetText();
                    tb_DefaultPort.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void tb_DefaultPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
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

                    strSQLConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_SRC_DataSource.Text + " )(PORT = " + tb_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_SRC_DBName.Text + ")));"
                        + "User Id=" + tb_SRC_UserName.Text + ";Password=" + tb_SRC_Password.Text + ";";

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
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                        + "; Integrated Security = true ";
                    }
                    else if (cb_windowsAuthentication.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                        + "; User ID = " + tb_SRC_UserName.Text + "; " + "Password = " + tb_SRC_Password.Text + ";";
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
            try
            {
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

                CheckedListBox.CheckedItemCollection clb_cic = clb_TableList.CheckedItems;
                int_totalCount = clb_cic.Count;
                string itemCheck;

                foreach (var item in clb_cic)
                {
                    if (backgroundWorker2.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker2.ReportProgress(0, "Cancel");
                        return;
                    }

                    int_count++;
                    itemCheck = (item.ToString().Substring(0, item.ToString().IndexOf('['))).Trim();

                    bool boolRowsCount = Int32.TryParse((item.ToString().Substring(item.ToString().IndexOf('[') + 1, item.ToString().IndexOf(']') - item.ToString().IndexOf('[') - 1)).Trim(), out int int_Rows_Count);

                    //if (cb_SkipEmptyTable.Checked && ds.Tables[itemCheck.ToString()].Rows.Count == 0)
                    //{
                    //    continue;
                    //}

                    if (cb_SkipEmptyTable.Checked && int_Rows_Count == 0)
                    {
                        continue;
                    }

                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        worksheet = pck.Workbook.Worksheets.Add(itemCheck.ToString());
                        worksheet.Cells["A1"].LoadFromDataTable(LoadTable(itemCheck.ToString()), true);

                        //DetectDateColumnAndFormat();

                        worksheet.Cells.AutoFitColumns();

                        SaveFileDialog saveFileDialogExcel = new SaveFileDialog
                        {
                            CheckFileExists = false,
                            Filter = "xlsx files (*.xlsx)|*.xlsx",
                            Title = "Save As Excel File",
                            FileName = linkLabel1.Text + @"\" + item.ToString() + ".xlsx",
                        };

                        pck.SaveAs(new System.IO.FileInfo(saveFileDialogExcel.FileName));

                        UpdateProgressBarReport(backgroundWorker2, int_count, int_totalCount, $" {int_count}/{int_totalCount}, Saving File :- {saveFileDialogExcel.FileName}");
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
            try
            {
                if (e.Cancelled && myException == null)
                {
                    DisplayMessage("All Progress cancelled");
                }
                else if (!(e.Cancelled))
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

        private void cb_UserProfileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //cb_UserProfileList = Common_Data.GetUserProfile(cb_UserProfileList);

                DisplayMessage($"User Profile Selected : {cb_UserProfileList.SelectedItem}");

                dBConnectionDetails = new DBConnectionDetails(cb_UserProfileList.SelectedIndex);

                if (rb_SRC.Checked)
                {
                    rb_Oracle.Checked = dBConnectionDetails.bool_Rb_SRC_Oracle;
                    rb_SQL.Checked = dBConnectionDetails.bool_Rb_SRC_SQL;

                    cb_windowsAuthentication.Checked = dBConnectionDetails.bool_SRC_SQL_WA;
                    cb_DefaultPort.Checked = dBConnectionDetails.bool_SRC_Oracle_DefaultPort;
                    tb_DefaultPort.Text = dBConnectionDetails.bool_Rb_SRC_Oracle ? dBConnectionDetails.int_SRC_Oracle_DefaultPort.ToString() : string.Empty;

                    tb_SRC_DataSource.Text = dBConnectionDetails.str_SRC_DataSource;
                    tb_SRC_DBName.Text = dBConnectionDetails.str_SRC_DBName;
                    tb_SRC_UserName.Text = dBConnectionDetails.str_SRC_UserName;
                    tb_SRC_Password.Text = dBConnectionDetails.str_SRC_Password;

                    //cb_SRC_TableName.Text = dBConnectionDetails.str_SRC_TableName;

                }
                else if (rb_TRG.Checked)
                {

                    rb_Oracle.Checked = dBConnectionDetails.bool_Rb_TRG_Oracle;
                    rb_SQL.Checked = dBConnectionDetails.bool_Rb_TRG_SQL;

                    cb_windowsAuthentication.Checked = dBConnectionDetails.bool_TRG_SQL_WA;
                    cb_DefaultPort.Checked = dBConnectionDetails.bool_TRG_Oracle_DefaultPort;
                    tb_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString();

                    tb_SRC_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                    tb_SRC_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                    tb_SRC_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                    tb_SRC_Password.Text = dBConnectionDetails.str_TRG_Password;
                    //cb_TRG_TableName.Text = dBConnectionDetails.str_TRG_TableName;
                }

                DisplayMessage($"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void rb_SRC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if(rb_SRC.Checked)
                {
                    rb_Oracle.Checked = dBConnectionDetails.bool_Rb_SRC_Oracle;
                    rb_SQL.Checked = dBConnectionDetails.bool_Rb_SRC_SQL;

                    cb_windowsAuthentication.Checked = dBConnectionDetails.bool_SRC_SQL_WA;
                    cb_DefaultPort.Checked = dBConnectionDetails.bool_SRC_Oracle_DefaultPort;
                    tb_DefaultPort.Text = dBConnectionDetails.bool_Rb_SRC_Oracle ? dBConnectionDetails.int_SRC_Oracle_DefaultPort.ToString() : string.Empty;

                    tb_SRC_DataSource.Text = dBConnectionDetails.str_SRC_DataSource;
                    tb_SRC_DBName.Text = dBConnectionDetails.str_SRC_DBName;
                    tb_SRC_UserName.Text = dBConnectionDetails.str_SRC_UserName;
                    tb_SRC_Password.Text = dBConnectionDetails.str_SRC_Password;
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
                if(rb_TRG.Checked)
                {
                    rb_Oracle.Checked = dBConnectionDetails.bool_Rb_TRG_Oracle;
                    rb_SQL.Checked = dBConnectionDetails.bool_Rb_TRG_SQL;

                    cb_windowsAuthentication.Checked = dBConnectionDetails.bool_TRG_SQL_WA;
                    cb_DefaultPort.Checked = dBConnectionDetails.bool_TRG_Oracle_DefaultPort;
                    tb_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString();

                    tb_SRC_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                    tb_SRC_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                    tb_SRC_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                    tb_SRC_Password.Text = dBConnectionDetails.str_TRG_Password;
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
    }
}
