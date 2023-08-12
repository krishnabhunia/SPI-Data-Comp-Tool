using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class PreMergingTool_Form : Form
    {
        private OpenFileDialogData openFileDialogData;
        private SaveFileDialogFile saveFileDialogFile;
        private DataSet dsTableList, dsContains, dsDelta;
        private Common_Data common_Data;
        private SaveFileDialog saveFileDialog;

        private DataTable dtFK, dtPK, dtDelta, dtDuplicateCheck, dtCheck;
        private string strConnection, str_DBName, str_SchemaName;

        //private string str_QueryFK, str_QueryPK;

        private bool boolConnection, boolExcelFileList;

        private bool boolSavedFileOnce;

        private string str_excelFileName;
        private string str_SubQuery, str_tableName_SRC, str_tableName_TRG;
        private string[] strCol;
        private string strDup, strTBName;

        private DataTable dtDup;
        private FolderBrowserDialog folderBrowserDialog;
        private DataSet dsDuplicate;
        public PreMergingTool_Form()
        {
            InitializeComponent();

            try
            {
                openFileDialogData = new OpenFileDialogData();
                dsTableList = new DataSet();

                strConnection = string.Empty;

                boolExcelFileList = false;
                boolConnection = false;

                dtFK = new DataTable();
                dtPK = new DataTable();

                dsDelta = new DataSet();
                dtDelta = new DataTable();

                common_Data = new Common_Data();
                str_excelFileName = "D:\\Output.xlsx";

                boolSavedFileOnce = false;

                saveFileDialogFile = new SaveFileDialogFile();
                saveFileDialog = new SaveFileDialog();

                dtDuplicateCheck = new DataTable();
                dtDup = new DataTable();

                string[] strIntColumnNames = { "SrNo", "Duplicate_Col", "Count" };
                //var dcColumnNames = strIntColumnNames.AsEnumerable().Select(x => new DataColumn(x.ToString(), typeof(int))).ToArray();
                dtDup.Columns.Add("SrNo", typeof(int));
                dtDup.Columns.Add("Duplicate_Col", typeof(string));
                dtDup.Columns.Add("Count", typeof(int));

                folderBrowserDialog = new FolderBrowserDialog();
                cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);
                dtDuplicate = new DataTable();
                dtDuplicate.TableName = "Duplicate Table";
                dsDuplicate = new DataSet();


                if (GlobalDebug.ISGlobalDebug(GlobalDebug.strUserName, GlobalDebug.strPassword))
                {
                    //comboBox1.Visible = true;
                    //comboBox1.SelectedIndex = 1;

                    cb_Schema.Checked = true;
                    tb_SchemaName.Text = "SPITEST6";
                }

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
                Common_Data.DisplayMessage(rtb_Log, ex.Message.ToString(), true);
                if (GlobalDebug.boolIsGlobalDebug)
                {
                    Common_Data.DisplayMessage(rtb_Log, ex.ToString(), true);
                }
            }
            catch (Exception ex1)
            {
                Common_Data.DisplayMessage(rtb_Log, ex1.Message.ToString(), true);
                if (GlobalDebug.boolIsGlobalDebug)
                {
                    Common_Data.DisplayMessage(rtb_Log, ex1.ToString(), true);
                }
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

        private void rb_oracle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_SRC_oracle.Checked)
                {

                    Common_Data.DisplayMessage(rtb_Log, "Oracle is selected");
                    gb_Source1.Text = "Source: Oracle Connection and Settings";
                    cb_WindowAuthentication_SRC.CheckState = CheckState.Unchecked;
                    cb_WindowAuthentication_SRC.Enabled = false;
                    cb_SRC_DefaultPort.CheckState = CheckState.Checked;
                    cb_SRC_DefaultPort.Enabled = true;
                    tb_SRC_DefaultPort.Text = "1521";
                    tb_SRC_DefaultPort.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void rb_sql_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_SRC_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "SQL is selected");
                    gb_Source1.Text = "Source: SQL Connection and Settings";
                    cb_WindowAuthentication_SRC.Enabled = true;
                    cb_WindowAuthentication_SRC.CheckState = CheckState.Unchecked;
                    cb_SRC_DefaultPort.Enabled = false;
                    tb_SRC_DefaultPort.ResetText();
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_Default_Port_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_SRC_DefaultPort.Checked)
                {
                    tb_SRC_DefaultPort.Text = "1521";
                    tb_SRC_DefaultPort.Enabled = false;
                }
                else
                {
                    tb_SRC_DefaultPort.Text = string.Empty;
                    tb_SRC_DefaultPort.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cb_Schema_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                tb_SchemaName.ResetText();
                if (cb_Schema.Checked)
                {
                    tb_SchemaName.Enabled = true;
                }
                else
                {
                    tb_SchemaName.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void DisplayMessage(Control control, string str_Msg)
        {
            try
            {
                control.Text = str_Msg;
                control.Update();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void DisplayMessage(Control control, string str_Msg, Color color)
        {
            try
            {
                control.Text = str_Msg;
                control.ForeColor = color;
                control.Update();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private DBConnectionDetails dBConnectionDetails;
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
                tb_SRC_DefaultPort.Enabled = dBConnectionDetails.bool_Rb_SRC_Oracle;

                tb_SRC_DataSource.Text = dBConnectionDetails.str_SRC_DataSource;
                tb_SRC_DBName.Text = dBConnectionDetails.str_SRC_DBName;
                tb_SRC_UserName.Text = dBConnectionDetails.str_SRC_UserName;
                tb_SRC_Password.Text = dBConnectionDetails.str_SRC_Password;
                tb_SchemaName.Text = dBConnectionDetails.str_SRC_TableName;

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

                Common_Data.DisplayMessage(rtb_Log, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private DataTable dtDuplicate;
        private void ProcessDuplicate()
        {
            try
            {
                dtDuplicateCheck = common_Data.CreateNewMergeColumn(dtDuplicateCheck, strDup, "Duplicate_Col", cb_WithoutSpecialCharacters.Checked, cb_IgnoreCase.Checked);
                DataView dv = dtDuplicateCheck.DefaultView;
                dv.Sort = "Duplicate_Col ASC";

                dtDuplicateCheck = dv.ToTable();

                int int_s = 0;

                str_excelFileName = $@"{folderBrowserDialog.SelectedPath}\Duplicate_Output_{strTBName}_{common_Data.AppendDateInOutputFileName(DateTime.Now).ToString()}.xlsx";

                var duplicates = dtDuplicateCheck.AsEnumerable().GroupBy(r => r[0]).Where(gr => gr.Count() > 1).Select(g => new { SrNo = ++int_s, Duplicate_Col = g.Key.ToString(), Count = g.Count() }).ToList();

                var duplicates1 = dtDuplicateCheck.AsEnumerable().GroupBy(r => r[0]).Where(gr => gr.Count() == 1).Select(g => new { SrNo = ++int_s, Duplicate_Col = g.Key.ToString(), Count = g.Count() }).Where(x => x.Duplicate_Col.Contains("PSL")).ToList();

                dtDup.Clear();
                duplicates.ForEach(x => dtDup.Rows.Add(x.SrNo, x.Duplicate_Col.ToString(), x.Count));

                dtDuplicate.Clear();
                dtDuplicate = (from d in dtDuplicateCheck.AsEnumerable() join c in dtDup.AsEnumerable() on d.Field<string>("Duplicate_Col") equals c.Field<string>("Duplicate_Col") select d).ToList().CopyToDataTable();

                dtDup.TableName = "Duplicate";
                dtDuplicate.TableName = "Duplicate List";

                dsDuplicate.Tables.Clear();
                dsDuplicate.Clear();
                dsDuplicate.Tables.Add(dtDup);
                dsDuplicate.Tables.Add(dtDuplicate);

                //if(WriteExcel.ExportToExcel(dtDup,"Duplicate", str_excelFileName))
                if(WriteExcel.ExportToExcel(dsDuplicate, str_excelFileName))
                {
                    Common_Data.DisplayMessage(rtb_Log, $"File saved as : {str_excelFileName}");
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"File not saved", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
        private bool CheckDuplicate()
        {
            try
            {
                if (boolConnection)
                {
                    foreach (DataRow dr in dsTableList.Tables[0].Rows)
                    {
                        strDup = dr["DUPLICACY_COLUMNS"].ToString();

                        strTBName = dr["table_name"].ToString().Trim();
                        Common_Data.DisplayMessage(rtb_Log, $"Processing table : {strTBName}");

                        if (cb_Schema.Checked)
                        {
                            strTBName = $"{str_DBName}.{str_SchemaName}.{strTBName}";
                        }
                        else
                        {
                            //strTBName = $"{str_DBName}.{strTBName}";
                            strTBName = $"{strTBName}";
                        }

                        str_SubQuery = $"SELECT * FROM {strTBName}";

                        dtDuplicateCheck = new DataTable();
                        dtDuplicateCheck.TableName = strTBName;

                        dtDup.TableName = strTBName;

                        try
                        {
                            if (rb_SRC_oracle.Checked)
                            {
                                using (OracleConnection oracleConnection = new OracleConnection(strConnection))
                                {
                                    using (OracleDataAdapter oracleDA = new OracleDataAdapter(str_SubQuery, oracleConnection))
                                    {
                                        oracleDA.Fill(dtDuplicateCheck);
                                        ProcessDuplicate();
                                    }
                                }
                            }
                            else if (rb_SRC_SQL.Checked)
                            {
                                using (SqlConnection sqlConnection = new SqlConnection(strConnection))
                                {
                                    using (SqlDataAdapter sqlDA = new SqlDataAdapter(str_SubQuery, sqlConnection))
                                    {
                                        sqlDA.Fill(dtDuplicateCheck);
                                        ProcessDuplicate();
                                    }
                                }
                            }
                            Common_Data.DisplayMessage(rtb_Log, $"Processing table : {strTBName} Completed");
                        }
                        catch (Exception exp)
                        {
                            Common_Data.DisplayError(rtb_Log, exp);
                            if(exp.Message.Contains("Could not find server"))
                            {
                                throw exp;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, "Pls connect with DB", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
                return false;
            }
        }
        private void btn_Duplicate_Click(object sender, EventArgs e)
        {
            try
            {
                lb_link.Visible = false;
                llb_File.Visible = false;

                openFileDialogData = openFileDialogData.openFileDialog("Select Excel File To Check Duplicate");

                if (openFileDialogData.boolFileSelected)
                {
                    dsTableList = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);
                    if (dsTableList != null && dsTableList.Tables.Count > 0 && dsTableList.Tables[0].Rows.Count > 0)
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"Selected File :{openFileDialogData.strFileNameWithPath}");
                        if (boolConnection)
                        {
                            folderBrowserDialog.SelectedPath = openFileDialogData.PathName_withoutFileName;

                            if(Debugger.IsAttached)
                            {
                                folderBrowserDialog.SelectedPath = $@"{openFileDialogData.PathName_withoutFileName}\Output";
                            }

                            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                            {
                                Common_Data.DisplayMessage(rtb_Log, $"Output Folder Selected : {folderBrowserDialog.SelectedPath}");
                                Common_Data.DisplayMessage(rtb_Log, "Duplicate Checking Started, Pls Wait !!!");

                                if (CheckDuplicate())
                                {
                                    Common_Data.DisplayMessage(rtb_Log, "All List Entries executed !!!", false, -1, Color.ForestGreen);
                                    Common_Data.DisplayMessage(rtb_Log, $"File Save In Location : {folderBrowserDialog.SelectedPath}");
                                    llb_File.Text = folderBrowserDialog.SelectedPath;
                                    Process.Start(folderBrowserDialog.SelectedPath);
                                }
                                else
                                {
                                    llb_File.Text = "No File";
                                    Common_Data.DisplayMessage(rtb_Log, "There is some problem", true);
                                }
                            }
                            else
                            {
                                Common_Data.DisplayMessage(rtb_Log, "File Not Saved", true);
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
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void btn_ListTableExcel_Click(object sender, EventArgs e)
        {
            try
            {
                lb_link.Visible = false;
                llb_File.Visible = false;
                dtDelta = new DataTable();
                dsDelta = new DataSet();

                boolSavedFileOnce = false;
                //throw new Exception ("test");
                Common_Data.DisplayMessage(rtb_Log, "Reading Pre-Merging Excel File ... ");
                openFileDialogData = openFileDialogData.openFileDialog("Select Excel File Contains Pre Merging Details");

                if (openFileDialogData.boolFileSelected)
                {
                    dsTableList = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);

                    if (dsTableList != null && dsTableList.Tables.Count > 0 && dsTableList.Tables[0].Rows.Count > 0)
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"Selected File {openFileDialogData.strFileNameWithPath}");
                        if (boolConnection)
                        {
                            Common_Data.DisplayMessage(rtb_Log, "Select File To Save");

                            saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save PreMerging File", "PreMerge_Output_" + common_Data.AppendDateInOutputFileName(DateTime.Now).ToString());

                            if (saveFileDialogFile.BoolFileSaveStatus)
                            {
                                Common_Data.DisplayMessage(rtb_Log, "PreMerging Started");
                                str_excelFileName = saveFileDialogFile.Str_saveFileNameWithPath;
                                if (PreMergingTest())
                                {
                                    Common_Data.DisplayMessage(rtb_Log, "PreMerging Completed", false, -1, Color.ForestGreen);
                                    Common_Data.DisplayMessage(rtb_Log, $"File Save In Location : {str_excelFileName}");
                                    lb_link.Visible = true;
                                    llb_File.Visible = true;
                                    llb_File.Text = str_excelFileName;
                                }
                                else
                                {
                                    lb_link.Visible = false;
                                    llb_File.Visible = false;
                                    llb_File.Text = "No File";
                                    Common_Data.DisplayMessage(rtb_Log, "There is some problem", true);
                                }
                            }
                            else
                            {
                                Common_Data.DisplayMessage(rtb_Log, "File Not Saved", true);
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
                if (str_excelFileName != null && str_excelFileName != string.Empty)
                {
                    FileInfo fileInfo = new FileInfo(str_excelFileName);
                    if (fileInfo.Exists)
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"File is getting open : {str_excelFileName}");
                        Process.Start(str_excelFileName);
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, "File doesn't exists, there is some error in saving the file", true);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void TestTableForFKPK(string strFK, string strID)
        {
            try
            {
                DataRow[] drr;
                dtDelta = dtFK.Copy();

                dtDelta = common_Data.RemoveColumnExcept(dtDelta, new string[] { strFK, strID });

                foreach (DataRow dr in dtDelta.Rows.Cast<DataRow>().ToArray())
                {
                    drr = dtPK.Select($"{strFK} = '{dr[strFK].ToString()}'");

                    if (drr.Length != 0)
                    {
                        dr.Delete();
                    }
                    else
                    {

                    }
                }
                dtDelta.AcceptChanges();

                if (dtDelta.Rows.Count > 0)
                {
                    dsDelta.Tables.Add(dtDelta.Copy());
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private bool PreMergingTest()
        {
            try
            {
                if (boolConnection)
                {
                    dtFK.TableName = "Foreign Key Table";
                    dtPK.TableName = "Primary Key Table";

                    foreach (DataRow dr in dsTableList.Tables[0].Rows)
                    {
                        strCol = new string[] { dr["TABLE_FK_PRIMARY_KEY"].ToString(), dr["FOREIGN_KEY_SRC"].ToString() };

                        if (cb_Schema.Checked)
                        {
                            str_tableName_SRC = $"{str_DBName}.{str_SchemaName}.{dr["TABLE_NAME_FK"]}";
                            str_tableName_TRG = $"{str_DBName}.{str_SchemaName}.{dr["TABLE_NAME_PK"]}";
                        }
                        else
                        {
                            str_tableName_SRC = $"{str_DBName}.{dr["TABLE_NAME_FK"]}";
                            str_tableName_TRG = $"{str_DBName}.{dr["TABLE_NAME_PK"]}";
                        }

                        str_SubQuery = $"SELECT {strCol[0]},{strCol[1]}, * FROM {str_tableName_SRC} WHERE {dr["FOREIGN_KEY_SRC"]} NOT IN (SELECT {dr["FOREIGN_KEY_TRG"]} FROM {str_tableName_TRG})";

                        if (rb_SRC_oracle.Checked)
                        {
                            using (OracleConnection oracleConnection = new OracleConnection(strConnection))
                            {
                                using (OracleDataAdapter oracleDA = new OracleDataAdapter(str_SubQuery, oracleConnection))
                                {
                                    dtPK = new DataTable();
                                    oracleDA.Fill(dtPK);
                                    TestTableForFKPK(dtPK, strCol, dr["TABLE_NAME_FK"].ToString());
                                }

                            }
                        }
                        else if (rb_SRC_SQL.Checked)
                        {
                            using (SqlConnection sqlConnection = new SqlConnection(strConnection))
                            {
                                using (SqlDataAdapter sqlDA = new SqlDataAdapter(str_SubQuery, sqlConnection))
                                {
                                    dtPK = new DataTable();
                                    sqlDA.Fill(dtPK);
                                    TestTableForFKPK(dtPK, strCol, dr["TABLE_NAME_FK"].ToString());
                                }
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, "Pls connect with DB", true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
                return false;
            }
        }

        private void TestTableForFKPK(DataTable dataTable, string[] strColNames, string str_TableName)
        {
            try
            {
                dtDelta = dataTable.Copy();

                dtDelta = common_Data.RemoveColumnExcept(dtDelta, strColNames);
                dtDelta.TableName = str_TableName;

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet worksheet;

                    if (!(dsDelta.Tables.Contains(str_TableName)))
                    {
                        dsDelta.Tables.Add(dtDelta.Copy());

                        if (boolSavedFileOnce)
                        {
                            using (var stream = File.OpenRead(str_excelFileName))
                            {
                                pck.Load(stream);
                            }
                        }
                        else
                        {
                            boolSavedFileOnce = true;
                        }

                        worksheet = pck.Workbook.Worksheets.Add(dtDelta.TableName);
                        worksheet.Cells["A1"].LoadFromDataTable(dtDelta, true);
                    }
                    else
                    {
                        using (var stream = File.OpenRead(str_excelFileName))
                        {
                            pck.Load(stream);
                        }
                        worksheet = pck.Workbook.Worksheets[str_TableName];
                        //var lastRow = worksheet.Cells.Last(c => c.Start.Row == 1);
                        int int_lastCol = worksheet.Dimension.Columns + 2;
                        worksheet.Cells[1, int_lastCol].LoadFromDataTable(dtDelta, true);
                    }
                    worksheet.Cells.AutoFitColumns(22, 50);
                    pck.SaveAs(new System.IO.FileInfo(str_excelFileName));
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
                throw ex;
            }
        }


        private void btn_ClearLogs_Click(object sender, EventArgs e)
        {
            try
            {
                rtb_Log.ResetText();
                Common_Data.DisplayMessage(rtb_Log, $"Logs Cleared");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                str_DBName = tb_SRC_DBName.Text.Trim();
                str_SchemaName = tb_SchemaName.Text.Trim();

                if (rb_SRC_oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Oracle...");

                    boolConnection = DatabaseConnection(rb_SRC_oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_SRC_DefaultPort.Text.Trim());
                }
                else if (rb_SRC_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to SQL...");
                    boolConnection = DatabaseConnection(rb_SRC_SQL.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_SRC_DefaultPort.Text.Trim());
                }

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private bool DatabaseConnection(string str_DBType, string str_ServerName, string str_DBName, bool boolWinAuthType, string str_Username, string str_Password, string str_OraclePortNo)
        {
            try
            {
                if (str_DBType.ToLower() == "oracle")
                {
                    strConnection = $"Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP) (HOST = {str_ServerName} )(PORT = {str_OraclePortNo})) (CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = {str_DBName}))); User Id={str_Username};Password={str_Password};";

                    using (OracleConnection oracleConnection = new OracleConnection(strConnection))
                    {
                        oracleConnection.Open();
                        if (oracleConnection.State == ConnectionState.Open)
                        {
                            Common_Data.DisplayMessage(rtb_Log, "Successfully connected To Oracle DB", false, -1, Color.ForestGreen);
                            oracleConnection.Close();
                            return boolConnection = true;
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, "Connection with the oracle DB failed", false, -1, Color.Red);
                            return boolConnection = false;
                        }
                    }
                }
                else if (str_DBType.ToLower() == "sql")
                {
                    if (boolWinAuthType)
                    {
                        strConnection = $"Data Source = {str_DBName};Initial Catalog = {str_DBName}; Integrated Security = true ";
                    }
                    else
                    {
                        strConnection = $"Data Source = {str_ServerName};Initial Catalog = {str_DBName}; User ID = {str_Username};Password = {str_Password};";
                    }

                    using (SqlConnection sqlConn = new SqlConnection(strConnection))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == ConnectionState.Open)
                        {
                            Common_Data.DisplayMessage(rtb_Log, "Successfully connected To SQL DB", false, -1, Color.DarkGreen);
                            sqlConn.Close();
                            return boolConnection = true;
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, "Connection with the SQL DB failed", false, -1, Color.DarkGreen);
                            return boolConnection = false;
                        }

                    }
                }
                else
                {
                    return boolConnection = false;
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
                return boolConnection = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (true)//Debugger.IsAttached)
                {
                    switch (comboBox1.SelectedIndex + 1)
                    {

                        case 1:
                            rb_SRC_oracle.Checked = true;
                            rb_SRC_SQL.Checked = false;
                            tb_SRC_DataSource.Text = "BRTSP00278";
                            tb_SRC_DBName.Text = "SBM12";
                            tb_SRC_UserName.Text = "SBM";
                            tb_SRC_Password.Text = "SBM";

                            break;

                        case 2:
                            rb_SRC_SQL.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT247";
                            tb_SRC_DBName.Text = "SPI2016";
                            tb_SRC_UserName.Text = "sa";
                            tb_SRC_Password.Text = "abcde@1234567";

                            break;

                        case 3:
                            rb_SRC_SQL.Checked = true;
                            tb_SRC_DataSource.Text = @"DFFMZ08SORA01\TOOLS_QAQC";
                            tb_SRC_DBName.Text = "Q_SPI_MDITRAIN_MERGE";
                            tb_SRC_UserName.Text = "CAOPIC_M";
                            tb_SRC_Password.Text = "CAOPIC_M1";

                            break;

                        case 4:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";

                            break;

                        case 5:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";

                            break;

                        case 6:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";

                            break;
                        case 7:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";

                            break;
                        case 8:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "localhost";
                            tb_SRC_DBName.Text = "orcl";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "1234";

                            break;

                        case 9:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";

                            break;
                        case 10:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "DSGPY3002.dcap.glpoly.net";
                            tb_SRC_DBName.Text = "MSPIAS";
                            tb_SRC_UserName.Text = "COV_LTTS_VIEW";
                            tb_SRC_Password.Text = "SP14LTTS";

                            break;

                        case 11:
                            rb_SRC_oracle.Checked = false;
                            rb_SRC_SQL.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT247";
                            tb_SRC_DBName.Text = "db_krishna";
                            tb_SRC_UserName.Text = "sa";
                            tb_SRC_Password.Text = "abcde@1234567";

                            break;

                        case 12:
                            rb_SRC_oracle.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT670";
                            tb_SRC_DBName.Text = "SPEL11G";
                            tb_SRC_UserName.Text = "system";
                            tb_SRC_Password.Text = "SPEL11G";

                            break;

                        case 13:
                            rb_SRC_SQL.Checked = true;
                            tb_SRC_DataSource.Text = "IESWKCT247";
                            tb_SRC_DBName.Text = "db_SPELSQL";
                            tb_SRC_UserName.Text = "sa";
                            tb_SRC_Password.Text = "abcde@1234567";

                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }
}
