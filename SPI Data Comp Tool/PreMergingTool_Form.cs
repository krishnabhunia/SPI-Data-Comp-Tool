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
    public partial class PreMergingTool_Form : WorkspacePageForm
    {
        private OpenFileDialogData openFileDialogData;
        private SaveFileDialogFile saveFileDialogFile;
        private DataSet dsTableList, dsDelta;
        private Common_Data common_Data;
        private SaveFileDialog saveFileDialog;

        private DataTable dtFK, dtPK, dtDelta, dtDuplicateCheck;
        private string strConnection, str_DBName, str_SchemaName;

        //private string str_QueryFK, str_QueryPK;

        private bool boolConnection;

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
            connection.Mode = ConnectionMode.SourceOnly;
            connection.Source.Title = "Database";
            connection.ProfileApplied += Connection_ProfileApplied;
            connection.Failed += (sender, error) => Common_Data.DisplayError(rtb_Log, error);
            connection.InputChanged += (sender, args) => { boolConnection = false; strConnection = string.Empty; steps?.Later(); };
            tb_SchemaName.TextChanged += (sender, args) => { boolConnection = false; };
            UiTheme.Apply(this);
            BuildSteps();   // its main button sits in a tab kept as designed; too small for the bold blue style

            try
            {
                openFileDialogData = new OpenFileDialogData();
                dsTableList = new DataSet();

                strConnection = string.Empty;

                boolConnection = false;

                dtFK = new DataTable();
                dtPK = new DataTable();

                dsDelta = new DataSet();
                dtDelta = new DataTable();

                common_Data = new Common_Data();
                str_excelFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Output.xlsx"); // was a hard-coded D:\ drive path

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
                common_Data.ReadUserProfile(cb_UserProfileList);
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

        /// <summary>A saved profile was put into the Connection panel.</summary>
        private StepFlow steps;   // controlled steps, see BuildSteps

        /// <summary>Controlled steps: Connection (and schema), Connect, Run. A step unlocks when the one before is done.</summary>
        private void BuildSteps()
        {
            steps = new StepFlow(this, step_Strip)
                .Add("Connection", "Fill in the connection (and the schema name if ticked)", () => StepFlow.Passes(() =>
                {
                    connection.Source.ValidateInput();
                    if (cb_Schema.Checked) FormValidation.Required(tb_SchemaName, "Schema name");
                }))
                .Add("Connect", "Test the connection", () => boolConnection && !string.IsNullOrWhiteSpace(strConnection), btn_Connect)
                .Add("Run", "Run the pre-merging process or the duplicate check",
                    () => false, btn_ListTableExcel, btn_Duplicate, cb_IgnoreCase, cb_WithoutSpecialCharacters);
            steps.Watch(cb_Schema, tb_SchemaName);
            steps.Refresh();
        }

        private void Connection_ProfileApplied(object sender, DBConnectionDetails details)
        {
            dBConnectionDetails = details;
            tb_SchemaName.Text = details.str_SRC_TableName;   // the profile's table field holds this window's schema
            Common_Data.DisplayMessage(rtb_Log, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
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
                FormHelpers.ShowText(control, str_Msg);
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
                FormHelpers.ShowText(control, str_Msg, color);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

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
                if (!ValidatePreMergeInputs()) return;
                lb_link.Visible = false;
                llb_File.Visible = false;

                openFileDialogData = openFileDialogData.openFileDialog("Select Excel File To Check Duplicate");

                if (openFileDialogData.boolFileSelected)
                {
                    DataSet candidate = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);
                    if (!FormValidation.Check(this, () => ValidatePreMergeWorkbook(candidate, true),
                        message => Common_Data.DisplayMessage(rtb_Log, message, true))) return;
                    dsTableList = candidate;
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
                                if (!FormValidation.Check(this, () => FormValidation.ExistingDirectory(folderBrowserDialog.SelectedPath,
                                    btn_Duplicate, "Output folder"), message => Common_Data.DisplayMessage(rtb_Log, message, true))) return;
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
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"File Not Selected", true);
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"File Not Selected", true);
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
                if (!ValidatePreMergeInputs()) return;
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
                    DataSet candidate = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);
                    if (!FormValidation.Check(this, () => ValidatePreMergeWorkbook(candidate, false),
                        message => Common_Data.DisplayMessage(rtb_Log, message, true))) return;
                    dsTableList = candidate;

                    if (dsTableList != null && dsTableList.Tables.Count > 0 && dsTableList.Tables[0].Rows.Count > 0)
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"Selected File {openFileDialogData.strFileNameWithPath}");
                        if (boolConnection)
                        {
                            Common_Data.DisplayMessage(rtb_Log, "Select File To Save");

                            saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save PreMerging File", "PreMerge_Output_" + common_Data.AppendDateInOutputFileName(DateTime.Now).ToString());

                            if (saveFileDialogFile.BoolFileSaveStatus)
                            {
                                if (!FormValidation.Check(this, () =>
                                {
                                    FormValidation.OutputFile(saveFileDialogFile.Str_saveFileNameWithPath, btn_ListTableExcel, "Output workbook", ".xlsx");
                                    FormValidation.DifferentFiles(saveFileDialogFile.Str_saveFileNameWithPath, openFileDialogData.strFileNameWithPath, btn_ListTableExcel);
                                }, message => Common_Data.DisplayMessage(rtb_Log, message, true))) return;
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
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"File Not Selected", true);
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"File Not Selected", true);
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
                        FormValidation.ExcelTable(dtDelta, null, "Excel output");
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
                        FormValidation.ExcelTable(dtDelta, null, "Pre-merging output");
                        FormValidation.Require((long)int_lastCol + dtDelta.Columns.Count - 1 <= 16384, null,
                            "The combined pre-merging worksheet exceeds Excel's 16,384-column limit.");
                        worksheet.Cells[1, int_lastCol].LoadFromDataTable(dtDelta, true);
                    }
                    worksheet.Cells.AutoFitColumns(22, 50);
                    FormValidation.WriteAtomically(str_excelFileName, temporary => pck.SaveAs(new FileInfo(temporary)));
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
                boolConnection = false;
                // stop here when an input is missing or wrong: marks at the fields, reasons in the log
                if (!connection.AddChecks(InputCheck.For(this), target: false).Report()) return;
                if (!FormValidation.Check(this, () =>
                {
                    connection.Source.ValidateInput();
                    if (cb_Schema.Checked) FormValidation.Required(tb_SchemaName, "Schema name");
                }, message => Common_Data.DisplayMessage(rtb_Log, message, true))) return;
                str_DBName = tb_SRC_DBName.Text.Trim();
                str_SchemaName = tb_SchemaName.Text.Trim();

                if (rb_SRC_oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Oracle...");

                    boolConnection = DatabaseConnection(rb_SRC_oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text, tb_SRC_DefaultPort.Text.Trim());
                }
                else if (rb_SRC_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to SQL...");
                    boolConnection = DatabaseConnection(rb_SRC_SQL.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text, tb_SRC_DefaultPort.Text.Trim());
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
                    strConnection = ConnectionStrings.Build("oracle", str_ServerName, str_DBName, false, str_Username, str_Password, str_OraclePortNo);

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
                        strConnection = ConnectionStrings.Build("sql", str_ServerName, str_DBName, true, str_Username, str_Password, str_OraclePortNo);
                    }
                    else
                    {
                        strConnection = ConnectionStrings.Build("sql", str_ServerName, str_DBName, false, str_Username, str_Password, str_OraclePortNo);
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

        internal bool ValidatePreMergeInputs()
        {
            return FormValidation.Check(this, () =>
            {
                connection.Source.ValidateInput();
                if (cb_Schema.Checked) FormValidation.Required(tb_SchemaName, "Schema name");
                FormValidation.Require(boolConnection && !string.IsNullOrWhiteSpace(strConnection), btn_Connect,
                    "Connect using the current database and schema before processing a workbook.");
            }, message => Common_Data.DisplayMessage(rtb_Log, message, true));
        }

        internal void ValidatePreMergeWorkbook(DataSet candidate, bool duplicate)
        {
            FormValidation.Require(candidate != null && candidate.Tables.Count > 0, btn_ListTableExcel, "The workbook contains no worksheets.");
            DataTable list = candidate.Tables[0];
            FormValidation.Table(list, btn_ListTableExcel, "Pre-merging list");
            string[] columns = duplicate ? new[] { "TABLE_NAME", "DUPLICACY_COLUMNS" }
                : new[] { "TABLE_FK_PRIMARY_KEY", "FOREIGN_KEY_SRC", "TABLE_NAME_FK", "TABLE_NAME_PK", "FOREIGN_KEY_TRG" };
            FormValidation.Columns(list, columns, btn_ListTableExcel, "Pre-merging list");
            foreach (DataRow row in list.Rows)
                foreach (string column in columns)
                    FormValidation.Require(!string.IsNullOrWhiteSpace(Convert.ToString(row[column])), btn_ListTableExcel,
                        "Pre-merging rows must not contain blank table or key-column names.");
        }

    }
}
