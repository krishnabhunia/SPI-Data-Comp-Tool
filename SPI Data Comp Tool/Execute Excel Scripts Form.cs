using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace SPI_Data_Comp_Tool
{
    public partial class Execute_Excel_Scripts_Form : WorkspacePageForm
    {
        private OpenFileDialogData openFileDialogData;
        private DataTable dtGenerateScript, dtExecute;
        private List<DataColumn> ListCols;
        private DBConnectionStatus dBConnectionStatus, dBConnectionStatus_SRC;
        private DBConnectionDetails dBConnectionDetails;

        // The connection fields live in the shared ConnectionPanel; these keep the names the code below uses.
        private System.Windows.Forms.ComboBox cb_UserProfileList => connection.ProfileList;
        private System.Windows.Forms.RadioButton rb_Oracle => connection.Source.Oracle;
        private System.Windows.Forms.RadioButton rb_SQL => connection.Source.Sql;
        private System.Windows.Forms.CheckBox cb_windowsAuthentication => connection.Source.WindowsLogin;
        private System.Windows.Forms.TextBox tb_DefaultPort => connection.Source.Port;
        private System.Windows.Forms.TextBox tb_SRC_DataSource => connection.Source.DataSource;
        private System.Windows.Forms.TextBox tb_SRC_DBName => connection.Source.Database;
        private System.Windows.Forms.TextBox tb_SRC_UserName => connection.Source.User;
        private System.Windows.Forms.TextBox tb_SRC_Password => connection.Source.Password;

        private Common_Data common_Data;
        private SqlCommand sqlCmd;
        private SqlConnection sqlConn;
        private DataTable dtSelect;
        private int int_Percentage, int_Count;
        private FolderBrowserDialog folderBrowserDialog1;
        private string strFolderPath;
        private bool boolSRC_Folder, boolTRG_Folder, boolPNIDFolderPath;

        private DirectoryInfo directoryInfo_SRC;
        private DirectoryInfo directoryInfo_TRG;

        private string strQuery, str_FileNameWithPath, strFileName;
        private static Point point_gbTextFile;

        private string[] strPNID_Reports;
        private int int_RowsAffected;

        private DataSet dsPNIDReports, dsSelect;
        public Execute_Excel_Scripts_Form()
        {
            InitializeComponent();
            connection.Mode = ConnectionMode.SourceOnly;
            connection.Source.Title = "Database";
            connection.ProfileHalf = rb_TRG.Checked ? ProfileHalf.Target : ProfileHalf.Source;
            connection.ProfileApplied += Connection_ProfileApplied;
            connection.Failed += (sender, error) => Common_Data.DisplayError(rtb_Log, error);
            UiTheme.Apply(this);

            try
            {
                openFileDialogData = new OpenFileDialogData();
                dtGenerateScript = new DataTable();
                dBConnectionDetails = new DBConnectionDetails();
                dBConnectionStatus = new DBConnectionStatus();

                common_Data = new Common_Data();
                sqlCmd = new SqlCommand();

                dBConnectionStatus_SRC = new DBConnectionStatus();
                sqlConn = new SqlConnection();
                dtExecute = new DataTable();

                dtSelect = new DataTable();
                folderBrowserDialog1 = new FolderBrowserDialog();

                boolSRC_Folder = boolTRG_Folder = false;
                connection.InputChanged += (sender, args) =>
                {
                    // A test of an older target must never authorize execution on edited fields.
                    if (dBConnectionStatus_SRC != null) dBConnectionStatus_SRC.boolConnection = false;
                    steps?.Later();
                };

                point_gbTextFile = gb_ExcelFile.Location;

                point_gbTextFile.X += 10;
                point_gbTextFile.Y += 10;

                strPNID_Reports = new string[] { "Instrument", "Piperun", "Equipment", "Nozzle", "Piping Component", "Representation", "OPC", "Drawing", "PlantItem", "PlantItem_Antwerp" };

                clb_PNID_Reports.Items.AddRange(strPNID_Reports);
                int_RowsAffected = 0;

                dsPNIDReports = new DataSet();
                boolPNIDFolderPath = false;

                ProfileLoadResult profileLoad = connection.LoadSavedProfiles();
                if (!string.IsNullOrEmpty(profileLoad.Message))
                    Common_Data.DisplayMessage(rtb_Log, profileLoad.Message, profileLoad.HasError);

                if (!SelfTest.IsRunning && (GlobalDebug.boolIsGlobalDebug || Debugger.IsAttached))
                {
                    linkLabel1.Text = @"D:\Krishna\Output\Export Select Query to Excel";
                    llb_PNID_Report_Path.Text = @"D:\Krishna\Output";

                    if (cb_UserProfileList.Items.Count > 3)
                    {
                        cb_UserProfileList.SelectedIndex = 3;
                    }
                    llb_Excel_SRC.Text = @"D:\Krishna\Projects\SPI\INPUT FILES\Generate Scripts\Select Scripts from TextFiles\Input Folder";
                    llb_Excel_TRG.Text = @"D:\Krishna\Projects\SPI\INPUT FILES\Generate Scripts\Select Scripts from TextFiles\Output Folder";

                    boolSRC_Folder = boolTRG_Folder = boolPNIDFolderPath = true;

                    tabControl1.SelectedTab = tabPage_PNID_Reports;

                    //btnTestConnecton_Click(null, null);

                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
            BuildSteps();
        }

        /// <summary>A saved profile was put into the Connection panel.</summary>
        private StepFlow steps;   // controlled steps, see BuildSteps

        /// <summary>
        /// Controlled steps: Connection, Test connection, then by tab - Excel: Script workbook, Scripts,
        /// Execute; Text: Folders and execute; PNID reports: Report. A step unlocks when the one before is done.
        /// </summary>
        private void BuildSteps()
        {
            Func<bool> excel = () => tabControl1.SelectedTab == tabPage_Excel;
            Func<bool> text = () => tabControl1.SelectedTab == tabPage_Txt;
            Func<bool> pnid = () => tabControl1.SelectedTab == tabPage_PNID_Reports;
            steps = new StepFlow(this, step_Strip)
                .Add("Connection", "Fill in the database connection", () => StepFlow.Passes(() => connection.ValidateInput()))
                .Add("Test connection", "Test the connection", () => dBConnectionStatus_SRC != null && dBConnectionStatus_SRC.boolConnection,
                    btnTestConnecton)
                .Add(() => "Script workbook", () => "Browse for the script workbook", () => dtGenerateScript != null && dtGenerateScript.Columns.Count > 0,
                    excel, btn_ExecuteScript)
                .Add(() => "Scripts", () => "Tick the script columns to run", () => clb_Checked.CheckedItems.Count > 0, excel,
                    clb_Checked, cmb_SelectFileName, chkb_SelectQuery)
                .Add(() => "Execute", () => "Execute the ticked scripts", () => false, excel, btn_Execute)
                .Add(() => "Folders and execute", () => "Choose both folders and execute", () => false, text,
                    llb_Excel_SRC, llb_Excel_TRG, btn_ExecuteFromTextFiles)
                .Add(() => "Report", () => "Choose the reports and generate them", () => false, pnid, clb_PNID_Reports, btn_PNID_Report);
            steps.Watch(tabControl1);
            steps.Refresh();
        }

        private void Connection_ProfileApplied(object sender, DBConnectionDetails details)
        {
            dBConnectionDetails = details;
            Common_Data.DisplayMessage(rtb_Log, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
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

        private void btn_ExecuteScript_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialogData = openFileDialogData.openFileDialog("Open File To Execute Scripts");

                if (openFileDialogData.boolFileSelected)
                {
                    if (!FormValidation.Check(this, () => FormValidation.ReadableFile(openFileDialogData.strFileNameWithPath,
                        btn_ExecuteScript, "Script workbook", ".xlsx", ".xlsm"), ReportValidation)) return;
                    DataSet candidate = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);
                    if (!FormValidation.Check(this, () => FormValidation.ValidateWorkbook(candidate, btn_ExecuteScript), ReportValidation))
                    { candidate?.Dispose(); return; }
                    lbl_ExcelFile.Text = $"Read File : {openFileDialogData.SimpleFileName}";
                    lbl_ExcelFile.Visible = true;
                    Common_Data.DisplayMessage(rtb_Log, $"Input Script File : {openFileDialogData.strFileNameWithPath}");
                    dtGenerateScript = candidate.Tables[0].Copy();
                    candidate.Dispose();
                    ListCols = (from DataColumn col in dtGenerateScript.Columns select col).ToList();
                    clb_Checked.Items.Clear();
                    clb_Checked.Items.AddRange(ListCols.Select(x => x.ColumnName).ToArray());

                    cmb_SelectFileName.Items.Clear();
                    cmb_SelectFileName.Items.AddRange(ListCols.Select(x => x.ColumnName).ToArray());
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, "Import cancelled; the previously loaded script workbook is unchanged.");
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        
        private void btn_Execute_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FormValidation.Check(this, () =>
                {
                    ValidateTestedConnection();
                    ValidateScriptTable(dtGenerateScript, clb_Checked.CheckedItems.Cast<string>().ToArray(),
                        chkb_SelectQuery.Checked, cmb_SelectFileName.Text, linkLabel1.Text, clb_Checked);
                }, ReportValidation)) return;
                int_RowsAffected = 0;
                int_Count = 0;
                Common_Data.DisplayMessage(rtb_Log, "Execution Started");
                dsSelect = new DataSet();

                if (dBConnectionStatus_SRC.boolConnection)
                {
                    List<string> checkedValuesEnum = clb_Checked.CheckedItems.Cast<string>().ToList();

                    if (checkedValuesEnum.Count() > 0)
                    {
                        if (chkb_SelectQuery.CheckState == CheckState.Unchecked)
                        {
                            dtExecute = dtGenerateScript.Copy();
                            Common_Data.DisplayDebugMsg(rtb_Log, $"Connecting : {dBConnectionStatus_SRC.strConnectionString}");
                            foreach (DataRow dr in dtExecute.Rows)
                            {
                                dBConnectionStatus_SRC.strCMDText = dr[checkedValuesEnum[0].ToString()].ToString();
                                Common_Data.DisplayMessage(rtb_Log, $"Executing script : {dBConnectionStatus_SRC.strCMDText}");
                                int_RowsAffected = common_Data.ExecuteNonQuery(dBConnectionStatus_SRC);
                                Common_Data.DisplayMessage(rtb_Log, $"{int_RowsAffected} : Rows affected");
                                UpdateProgress(++int_Count, dtExecute.Rows.Count);
                            }
                        }
                        else if (chkb_SelectQuery.CheckState == CheckState.Checked)
                        {
                            dtExecute = dtGenerateScript.Copy();

                            Common_Data.DisplayDebugMsg(rtb_Log, $"Connecting : {dBConnectionStatus_SRC.strConnectionString}");
                            foreach (DataRow dr in dtExecute.Rows)
                            {

                                dBConnectionStatus_SRC.strCMDText = dr[checkedValuesEnum[0].ToString()].ToString();
                                //dtSelect.TableName = dr[cmb_SelectFileName.Text].ToString();
                                //dtSelect = common_Data.FillDataTable(dtSelect, dBConnectionStatus_SRC);

                                dsSelect = common_Data.FillDataTable(new DataSet(), dBConnectionStatus_SRC);

                                Common_Data.DisplayMessage(rtb_Log, $"Executing Select Script : {dBConnectionStatus_SRC.strCMDText}");
                                str_FileNameWithPath = $@"{linkLabel1.Text}\{dr[cmb_SelectFileName.Text]}.xlsx";
                                if (WriteExcel.ExportToExcel(dsSelect, str_FileNameWithPath))
                                {
                                    Common_Data.DisplayMessage(rtb_Log, $"File saved as : {str_FileNameWithPath}");
                                }
                                else
                                {
                                    Common_Data.DisplayMessage(rtb_Log, $"There is some problem in saving the file : {str_FileNameWithPath}", true);
                                }
                                UpdateProgress(++int_Count, dtExecute.Rows.Count);
                            }
                            Common_Data.DisplayMessage(rtb_Log, $"Opening folder : {linkLabel1.Text}");
                            Process.Start(linkLabel1.Text);
                        }
                        Common_Data.DisplayMessage(rtb_Log, $"Process Completed");
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"No Checkbox selected to execute query", true);
                    }
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
        private void btn_Clear_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateProgress(0, 1);
                rtb_Log.Clear();
                Common_Data.DisplayMessage(rtb_Log, "Logs Cleared");
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }



        private void chkb_SelectQuery_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cmb_SelectFileName.Enabled = linkLabel1.Enabled = chkb_SelectQuery.Checked;
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void ToggleSchemas()
        {
            try
            {
                cb_CommonSchema.Enabled = rb_SRC.Checked;
                cb_CommonSchema.Checked = rb_SRC.Checked;
                cb_Schema1.Enabled = rb_SRC.Checked;
                cb_Schema2.Enabled = rb_SRC.Checked;
                cb_Schema3.Enabled = rb_SRC.Checked;

                //cb_CommonSchema.Checked = false;
                //cb_Schema1.Checked = false;
                //cb_Schema2.Checked = false;
                //cb_Schema3.Checked = false;

                cb_IpepSchema.Enabled = rb_TRG.Checked;
                cb_IpepSchema.Checked = rb_TRG.Checked;
                cb_Path.Enabled = rb_TRG.Checked;
                cb_Path.Checked = rb_TRG.Checked;

                tb_CommonSchema.ResetText();
                tb_Schema_1.ResetText();
                tb_Schema_2.ResetText();
                tb_Schema_3.ResetText();

                tb_Path.ResetText();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void togglePNIDControls(string str_src_trg)
        {
            try
            {
                if(tabControl1.SelectedTab == tabPage_PNID_Reports)
                {
                    if (str_src_trg.ToLower() == "src")
                    {

                    }
                    else if (str_src_trg.ToLower() == "trg")
                    {

                    }
                }
                
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_SRC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_SRC.Checked)
                {
                    UseProfileHalf(ProfileHalf.Source);
                    ToggleSchemas();
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_TRG_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_TRG.Checked)
                {
                    UseProfileHalf(ProfileHalf.Target);
                    ToggleSchemas();
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private DataTable dtPNIDReports;
        private string strPathGen;
        private void btn_PNID_Report_Click(object sender, EventArgs e)
        {
            try
            {
                var queries = new Dictionary<string, string>();
                if (!FormValidation.Check(this, () =>
                {
                    ValidateTestedConnection();
                    FormValidation.ExistingDirectory(llb_PNID_Report_Path.Text, llb_PNID_Report_Path, "Report output folder");
                    FormValidation.Require(rb_SRC.Checked != rb_TRG.Checked, rb_SRC, "Choose Source or Target reports.");
                    FormValidation.Require(clb_PNID_Reports.CheckedItems.Count > 0, clb_PNID_Reports, "Select at least one report.");
                    foreach (string report in clb_PNID_Reports.CheckedItems)
                    {
                        string query = FetchSQLQueryForText(report, rb_SRC.Checked ? "Source" : "Target_IPEP");
                        FormValidation.Require(!string.IsNullOrWhiteSpace(query) && !query.Contains("###"), clb_PNID_Reports,
                            "A report template is missing or has unresolved schema/path placeholders. Complete the selected template options.");
                        queries.Add(report, query);
                    }
                    string suffix = (rb_TRG.Checked ? tb_Path.Text : "") + tb_CommonSchema.Text;
                    FormValidation.Require(suffix.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && !suffix.EndsWith(".", StringComparison.Ordinal),
                        tb_CommonSchema, "Report path/schema options contain characters that cannot be used in an output filename.");
                }, ReportValidation)) return;
                Common_Data.DisplayMessage(rtb_Log, "Execution Started, Fetching PNID Reports from Database");

                if (boolPNIDFolderPath)
                {
                    if (dBConnectionStatus_SRC.boolConnection)
                    {
                        List<string> checkedValuesEnum = clb_PNID_Reports.CheckedItems.Cast<string>().ToList();

                        if (checkedValuesEnum.Count() > 0)
                        {
                            Common_Data.DisplayDebugMsg(rtb_Log, $"Connecting : {dBConnectionStatus_SRC.strConnectionString}");

                            dtPNIDReports = new DataTable();
                            string strPath = string.Empty;
                            if(rb_SRC.Checked)
                            {
                                strPath = "Source";
                                strPathGen = strPath;
                            }
                            else if(rb_TRG.Checked)
                            {
                                strPath = $"Target_IPEP";
                                strPathGen = $"{strPath}_{tb_Path.Text}";
                            }

                            foreach (string str in checkedValuesEnum)
                            {
                                try
                                {
                                    dBConnectionStatus_SRC.strCMDText = queries[str];

                                    Common_Data.DisplayDebugMsg(rtb_Log, $"Executing script : {dBConnectionStatus_SRC.strCMDText}");
                                    dtPNIDReports.Reset();

                                    //if (Debugger.IsAttached)
                                    //{
                                    //    dBConnectionStatus_SRC.strCMDText = "SELECT * FROM SOURCE1";
                                    //}

                                    dtPNIDReports = common_Data.FillDataTable(dtPNIDReports, dBConnectionStatus_SRC);
                                    dtPNIDReports.TableName = str;

                                    string strCustom = tb_CommonSchema.Text != ""?$"_{tb_CommonSchema.Text}":"";

                                    //if(Debugger.IsAttached)
                                    //{
                                    //    dtPNIDReports = dtPNIDReports.Clone();
                                    //}

                                    str_FileNameWithPath = $@"{llb_PNID_Report_Path.Text}\{strPathGen}_{str}{strCustom}_Report_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                                    FormValidation.OutputFile(str_FileNameWithPath, llb_PNID_Report_Path, "Report output", ".xlsx");
                                    if (WriteExcel.ExportToExcel(dtPNIDReports, dtPNIDReports.TableName, str_FileNameWithPath))
                                    {
                                        Common_Data.DisplayMessage(rtb_Log, $"File saved as : {str_FileNameWithPath}");
                                    }
                                    else
                                    {
                                        Common_Data.DisplayMessage(rtb_Log, $"There is some problem in saving the file : {str_FileNameWithPath}", true);
                                    }
                                }
                                catch (Exception ex1)
                                {
                                    Common_Data.DisplayError(rtb_Log, ex1);
                                }
                            }

                            Common_Data.DisplayMessage(rtb_Log, $"Opening folder : {llb_PNID_Report_Path.Text}");
                            Process.Start(llb_PNID_Report_Path.Text);
                            Common_Data.DisplayMessage(rtb_Log, $"Process Completed");
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, $"No Checkbox selected to execute query", true);
                        }
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"DATABASE Not Connected !!!, Pls connect to DB", true);
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Folder Path Not Selected", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void cb_Schema1_CheckedChanged(object sender, EventArgs e)
        {
            tb_Schema_1.Enabled = cb_Schema1.Checked;
        }

        private void cb_Schema2_CheckedChanged(object sender, EventArgs e)
        {
            tb_Schema_2.Enabled = cb_Schema2.Checked;
        }

        private void cb_Schema3_CheckedChanged(object sender, EventArgs e)
        {
            tb_Schema_3.Enabled = cb_Schema3.Checked;
        }

        private void cb_Path_CheckedChanged(object sender, EventArgs e)
        {
            tb_Path.Enabled = cb_Path.Checked;
        }

        private void cb_CommonSchema_CheckedChanged(object sender, EventArgs e)
        {
            tb_CommonSchema.Enabled = cb_CommonSchema.Checked;
        }

        private void tb_CommonSchema_TextChanged(object sender, EventArgs e)
        {
            //tb_Schema_1.Text = tb_CommonSchema.Text + "pidd";
            //tb_Schema_2.Text = tb_CommonSchema.Text + "pid";
            //tb_Schema_3.Text = tb_CommonSchema.Text;

            tb_Schema_1.Text = tb_CommonSchema.Text;
            tb_Schema_2.Text = tb_CommonSchema.Text;
            tb_Schema_3.Text = tb_CommonSchema.Text;
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                tb_CommonSchema.ResetText();
                tb_Schema_1.ResetText();
                tb_Schema_2.ResetText();
                tb_Schema_3.ResetText();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void cb_IpepSchema_CheckedChanged(object sender, EventArgs e)
        {
            tb_IpepSchema.Enabled = cb_IpepSchema.Checked;
        }

        private void cb_SelectAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (int i in Enumerable.Range(0, clb_PNID_Reports.Items.Count))
                {
                    clb_PNID_Reports.SetItemChecked(i, cb_SelectAll.Checked);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void cb_Toggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (int i in Enumerable.Range(0, clb_PNID_Reports.Items.Count))
                {
                    clb_PNID_Reports.SetItemChecked(i, !clb_PNID_Reports.GetItemChecked(i));
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private string FetchSQLQueryForText(string str, string strSourceORTarget)
        {
            string strAllText = string.Empty;
            try
            {
                string strFileName = string.Empty;

                if(Debugger.IsAttached)
                {
                    strFileName = $@"{Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName)}\PNID_Reports\{strSourceORTarget}";
                    //@"D:\Krishna\Projects\SPI\SPI Data Comp Tool\SPI Data Comp Tool\PNID_Reports\Source";
                }
                else
                {
                    strFileName = $@"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory)}\PNID_Reports\{strSourceORTarget}";
                    //@"D:\Krishna\Projects\SPI\SPI Data Comp Tool\SPI Data Comp Tool\bin\Debug\\PNID_Reports\Source";
                }

                switch (str.ToLower())
                {
                    case "instrument":
                        strFileName = $@"{strFileName}\instrument.txt";
                        break;

                    case "equipment":
                        strFileName = $@"{strFileName}\equipment.txt";
                        break;

                    case "nozzle":
                        strFileName = $@"{strFileName}\nozzle.txt";
                        break;

                    case "piperun":
                        strFileName = $@"{strFileName}\piperun.txt";
                        break;

                    case "piping component":
                        strFileName = $@"{strFileName}\piping_component.txt";
                        break;

                    case "representation":
                        strFileName = $@"{strFileName}\representation.txt";
                        break;

                    case "opc":
                        strFileName = $@"{strFileName}\opc.txt";
                        break;

                    case "drawing":
                        strFileName = $@"{strFileName}\drawing.txt";
                        break;

                    case "plantitem":
                        strFileName = $@"{strFileName}\plantitem.txt";
                        break;

                    case "plantitem_antwerp":
                        strFileName = $@"{strFileName}\plantitem_antwerp.txt";
                        break;

                    default:
                        strFileName = $@"{strFileName}\empty.txt";
                        break;

                }

                Common_Data.DisplayMessage(rtb_Log, strFileName);

                if(GlobalDebug.boolIsGlobalDebug)
                {
                    Process.Start(strFileName);
                }

                strAllText = File.ReadAllText(strFileName);

                if(cb_Schema1.Checked || cb_CommonSchema.Checked)
                {
                    strAllText = strAllText.Replace("###Schema_1###", tb_Schema_1.Text);
                }

                if (cb_Schema2.Checked || cb_CommonSchema.Checked)
                {
                    strAllText = strAllText.Replace("###Schema_2###", tb_Schema_2.Text);
                }

                if (cb_Schema3.Checked || cb_CommonSchema.Checked)
                {
                    strAllText = strAllText.Replace("###Schema_3###", tb_Schema_3.Text);
                }

                if (cb_Path.Checked)
                {
                    strAllText = strAllText.Replace("###path###", tb_Path.Text);
                }

                if(cb_IpepSchema.Checked)
                {
                    strAllText = strAllText.Replace("###schemaname###", tb_IpepSchema.Text);
                }

                strAllText = strAllText.Replace("###fullconstraint_1###", tb_UserCondition.Text);

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
            
            return strAllText;
        }

        private void llb_Path_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    strFolderPath = folderBrowserDialog1.SelectedPath;
                    llb_PNID_Report_Path.Text = strFolderPath;
                    Common_Data.DisplayMessage(rtb_Log, $"Folder path set to :{strFolderPath}");
                    boolPNIDFolderPath = true;
                }
                else
                {
                    strFolderPath = string.Empty;
                    llb_PNID_Report_Path.Text = "No Source Folder Selected";
                    Common_Data.DisplayMessage(rtb_Log, $"Source folder path not set, aborting all operation", true);
                    boolPNIDFolderPath = false;
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
                    Common_Data.DisplayMessage(rtb_Log, $"Folder path set to :{strFolderPath}");
                    boolSRC_Folder = true;
                }
                else
                {
                    strFolderPath = string.Empty;
                    llb_Excel_SRC.Text = "No Source Folder Selected";
                    Common_Data.DisplayMessage(rtb_Log, $"Source folder path not set, aborting all operation", true);
                    boolSRC_Folder = false;
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }


        private void btn_ExecuteFromTextFiles_Click(object sender, EventArgs e)
        {
            try
            {
                FileInfo[] inputFiles = null;
                var queries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (!FormValidation.Check(this, () =>
                {
                    ValidateTestedConnection();
                    FormValidation.ExistingDirectory(llb_Excel_SRC.Text, llb_Excel_SRC, "SQL text input folder");
                    FormValidation.ExistingDirectory(llb_Excel_TRG.Text, llb_Excel_TRG, "Workbook output folder");
                    inputFiles = new DirectoryInfo(llb_Excel_SRC.Text).GetFiles("*.txt", SearchOption.TopDirectoryOnly);
                    FormValidation.Require(inputFiles.Length > 0, llb_Excel_SRC, "The input folder contains no SQL text files.");
                    foreach (FileInfo file in inputFiles)
                    {
                        FormValidation.ReadableFile(file.FullName, llb_Excel_SRC, "SQL text file", ".txt");
                        string query = File.ReadAllText(file.FullName);
                        FormValidation.Require(!string.IsNullOrWhiteSpace(query) && !query.Contains("\0"), llb_Excel_SRC,
                            "Every SQL text file must contain a non-empty statement without null characters.");
                        FormValidation.OutputFile(Path.Combine(llb_Excel_TRG.Text, Path.GetFileNameWithoutExtension(file.Name) + ".xlsx"),
                            llb_Excel_TRG, "Export workbook", ".xlsx");
                        queries.Add(file.FullName, query);
                    }
                }, ReportValidation)) return;
                int_Count = 0;
                if (dBConnectionStatus_SRC.boolConnection)
                {
                    if (boolSRC_Folder && boolTRG_Folder)
                    {

                        directoryInfo_SRC = new DirectoryInfo(llb_Excel_SRC.Text);
                        directoryInfo_TRG = new DirectoryInfo(llb_Excel_TRG.Text);

                        foreach (FileInfo fileInfo in inputFiles)
                        {
                            strQuery = queries[fileInfo.FullName]; // Preserve line comments and literal whitespace.
                            dBConnectionStatus_SRC.strCMDText = strQuery;
                            Common_Data.DisplayMessage(rtb_Log, $"Executing Select Script From File : {fileInfo.FullName}");
                            dtSelect.Reset();

                            try
                            {
                                dtSelect = common_Data.FillDataTable(dtSelect, dBConnectionStatus_SRC);
                                strFileName = fileInfo.Name;
                                strFileName = strFileName.Substring(0, strFileName.LastIndexOf(".txt"));
                                dtSelect.TableName = strFileName;
                                str_FileNameWithPath = $@"{llb_Excel_TRG.Text}\{strFileName}.xlsx";
                                Common_Data.DisplayMessage(rtb_Log, $"Exporting to File : {fileInfo.FullName}");
                                if (WriteExcel.ExportToExcel(dtSelect, strFileName, str_FileNameWithPath))
                                {
                                    Common_Data.DisplayMessage(rtb_Log, $"File saved as : {str_FileNameWithPath}");
                                }
                                else
                                {
                                    Common_Data.DisplayMessage(rtb_Log, $"There is some problem in saving the file : {str_FileNameWithPath}", true);
                                }
                            }
                            catch (Exception ex)
                            {
                                Common_Data.DisplayError(rtb_Log, ex);
                                Common_Data.DisplayMessage(rtb_Log, "Skipping current file and processing further", true);
                            }
                            UpdateProgress(++int_Count, inputFiles.Length);
                        }
                        Common_Data.DisplayMessage(rtb_Log, $"Opening folder : {llb_Excel_TRG.Text}");
                        Process.Start(llb_Excel_TRG.Text);
                        Common_Data.DisplayMessage(rtb_Log, $"Process Completed");
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"Source or Target or Both Folder has not been set !!!", true);
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Database Connection Problem, Either it is not connected or config is incorrect !!!", true);
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
                    Common_Data.DisplayMessage(rtb_Log, $"Folder path set to :{strFolderPath}");
                    boolTRG_Folder = true;
                }
                else
                {
                    strFolderPath = string.Empty;
                    llb_Excel_TRG.Text = strFolderPath;
                    Common_Data.DisplayMessage(rtb_Log, $"Target folder path not set, aborting all operation", true);
                    boolTRG_Folder = false;
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
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
                    Common_Data.DisplayMessage(rtb_Log, $"Path selected to save excel files : {linkLabel1.Text}");
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Path NOT selected", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }



        private void btnTestConnecton_Click(object sender, EventArgs e)
        {
            try
            {
                // stop here when an input is missing or wrong: marks at the fields, reasons in the log
                if (!connection.AddChecks(InputCheck.For(this), target: false).Report()) return;
                if (!FormValidation.Check(this, () => connection.ValidateInput(), ReportValidation)) return;
                dBConnectionStatus_SRC.boolConnection = false;
                string str_DBName = tb_SRC_DBName.Text.Trim();

                if (rb_Oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Oracle...");

                    dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_Oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_windowsAuthentication.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text, tb_DefaultPort.Text.Trim());
                }
                else if (rb_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to SQL...");
                    dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_SQL.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_windowsAuthentication.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text, tb_DefaultPort.Text.Trim());
                }

                Common_Data.DisplayMessage(rtb_Log, dBConnectionStatus_SRC.strConnectionMSG, (!dBConnectionStatus_SRC.boolConnection), -1, (dBConnectionStatus_SRC.boolConnection) ? Color.DarkGreen : Color.Red);
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }

        }

        private void ReportValidation(string message) { Common_Data.DisplayMessage(rtb_Log, message, true); }

        internal void ValidateTestedConnection()
        {
            connection.ValidateInput();
            FormValidation.Require(dBConnectionStatus_SRC != null && dBConnectionStatus_SRC.boolConnection,
                btnTestConnecton, "Test the current database connection before executing. Editing connection fields invalidates the previous test.");
        }

        internal static void ValidateScriptTable(DataTable table, string[] selectedColumns, bool export,
            string fileNameColumn, string outputFolder, Control field)
        {
            FormValidation.Table(table, field, "Script worksheet");
            FormValidation.Require(selectedColumns != null && selectedColumns.Length == 1, field,
                "Select exactly one SQL statement column; only one statement column is executed per run.");
            FormValidation.Columns(table, selectedColumns, field, "SQL statement column");
            foreach (DataRow row in table.Rows)
                FormValidation.Require(!row.IsNull(selectedColumns[0]) && !string.IsNullOrWhiteSpace(row[selectedColumns[0]].ToString())
                    && !row[selectedColumns[0]].ToString().Contains("\0"), field,
                    "The selected SQL column contains an empty statement or a null character. Correct every row before running.");
            if (!export) return;
            FormValidation.Columns(table, new[] { fileNameColumn }, field, "Output filename column");
            FormValidation.ExistingDirectory(outputFolder, field, "Output folder");
            var outputs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in table.Rows)
            {
                string name = row[fileNameColumn].ToString();
                FormValidation.Require(!string.IsNullOrWhiteSpace(name) && name == Path.GetFileName(name)
                    && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && !name.EndsWith(".", StringComparison.Ordinal), field,
                    "Every output filename must be a non-empty filename, not a path.");
                string path = FormValidation.OutputFile(Path.Combine(outputFolder, name + ".xlsx"), field, "Output workbook", ".xlsx");
                FormValidation.Require(outputs.Add(path), field, "Output filenames must be unique (ignoring letter case) to avoid overwriting an earlier result.");
            }
        }
    }
}
