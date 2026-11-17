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
    public partial class Execute_Excel_Scripts_Form : Form
    {
        private bool boolColorAlternate;
        private OpenFileDialogData openFileDialogData;
        private DataTable dtGenerateScript, dtExecute;
        private List<DataColumn> ListCols;
        private DBConnectionStatus dBConnectionStatus, dBConnectionStatus_SRC;
        private DBConnectionDetails dBConnectionDetails;

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
        private SqlDataAdapter sqlDA;

        private string strQuery, str_FileNameWithPath, strFileName;
        private static Point point_gbTextFile;

        private string[] strPNID_Reports;
        private int int_RowsAffected;

        private DataSet dsPNIDReports, dsSelect;
        public Execute_Excel_Scripts_Form()
        {
            InitializeComponent();

            try
            {
                boolColorAlternate = true;
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
                cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);

                point_gbTextFile = gb_ExcelFile.Location;

                point_gbTextFile.X += 10;
                point_gbTextFile.Y += 10;

                strPNID_Reports = new string[] { "Instrument", "Piperun", "Equipment", "Nozzle", "Piping Component", "Representation", "OPC", "Drawing", "PlantItem", "PlantItem_Antwerp" };

                clb_PNID_Reports.Items.AddRange(strPNID_Reports);
                int_RowsAffected = 0;

                dsPNIDReports = new DataSet();
                boolPNIDFolderPath = false;

                if (GlobalDebug.boolIsGlobalDebug || Debugger.IsAttached)
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
        }

        private void btn_ExecuteScript_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialogData = openFileDialogData.openFileDialog("Open File To Execute Scripts");

                if (openFileDialogData.boolFileSelected)
                {
                    lbl_ExcelFile.Text = $"Read File : {openFileDialogData.SimpleFileName}";
                    lbl_ExcelFile.Visible = true;
                    Common_Data.DisplayMessage(rtb_Log, $"Input Script File : {openFileDialogData.strFileNameWithPath}");
                    dtGenerateScript = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath).Tables[0];
                    ListCols = (from DataColumn col in dtGenerateScript.Columns select col).ToList();
                    clb_Checked.Items.Clear();
                    clb_Checked.Items.AddRange(ListCols.Select(x => x.ColumnName).ToArray());

                    cmb_SelectFileName.Items.Clear();
                    cmb_SelectFileName.Items.AddRange(ListCols.Select(x => x.ColumnName).ToArray());
                }
                else
                {
                    lbl_ExcelFile.ResetText();
                    lbl_ExcelFile.Visible = false;
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
                            dtExecute = common_Data.RemoveColumnExcept(dtGenerateScript, string.Join(",", checkedValuesEnum));
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
                            dtExecute = common_Data.RemoveColumnExcept(dtGenerateScript, string.Join(", ", string.Join(",", checkedValuesEnum), cmb_SelectFileName.Text));

                            Common_Data.DisplayDebugMsg(rtb_Log, $"Connecting : {dBConnectionStatus_SRC.strConnectionString}");
                            foreach (DataRow dr in dtExecute.Rows)
                            {

                                dBConnectionStatus_SRC.strCMDText = dr[checkedValuesEnum[0].ToString()].ToString();
                                //dtSelect.TableName = dr[cmb_SelectFileName.Text].ToString();
                                //dtSelect = common_Data.FillDataTable(dtSelect, dBConnectionStatus_SRC);

                                dsSelect = common_Data.FillDataTable(dsSelect, dBConnectionStatus_SRC);

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

        private void cb_UserProfileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, $"User Profile Selected : {cb_UserProfileList.SelectedItem}");

                dBConnectionDetails = new DBConnectionDetails(cb_UserProfileList.SelectedIndex);

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

        private void rb_SRC_oracle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_Oracle.Checked)
                {
                    CheckChangeRadioButton(1);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_SRC_SQL_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_SQL.Checked)
                {
                    CheckChangeRadioButton(1);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void CheckChangeRadioButton(int int_Group)
        {
            try
            {
                if (int_Group == 1)
                {
                    if (rb_Oracle.Checked)
                    {
                        gb_Source1.Text = "Source: Oracle Connection and Settings";
                        cb_windowsAuthentication.Enabled = false;
                        cb_windowsAuthentication.Checked = false;
                        cb_DefaultPort.Enabled = true;
                        cb_DefaultPort.Checked = true;
                        tb_DefaultPort.Enabled = true;
                        tb_DefaultPort.ReadOnly = true;
                    }
                    else if (rb_SQL.Checked)
                    {
                        gb_Source1.Text = "Source: SQL Connection and Settings";
                        cb_windowsAuthentication.Enabled = true;
                        cb_windowsAuthentication.Checked = false;
                        cb_DefaultPort.Enabled = false;
                        cb_DefaultPort.Checked = false;
                        tb_DefaultPort.Enabled = false;
                        tb_DefaultPort.ReadOnly = true;
                    }
                }
                //else if (int_Group == 2)
                //{
                //    if (rb_TRG_Oracle.Checked)
                //    {
                //        gb_Target1.Text = "Target : Oracle Connection and Settings";
                //        cb_WindowAuthentication_TRG.Enabled = false;
                //        cb_WindowAuthentication_TRG.Checked = false;
                //        cb_TRG_DefaultPort.Enabled = true;
                //        cb_TRG_DefaultPort.Checked = true;
                //        tb_TRG_DefaultPort.Enabled = true;
                //        tb_TRG_DefaultPort.ReadOnly = true;

                //    }
                //    else if (rb_TRG_SQL.Checked)
                //    {
                //        gb_Target1.Text = "Target : SQL Connection and Settings";
                //        cb_WindowAuthentication_TRG.Enabled = true;
                //        cb_WindowAuthentication_TRG.Checked = false;
                //        cb_TRG_DefaultPort.Enabled = false;
                //        cb_TRG_DefaultPort.Checked = false;
                //        tb_TRG_DefaultPort.Enabled = false;
                //        tb_TRG_DefaultPort.ReadOnly = true;
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cb_SRC_DefaultPort_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_DefaultPort.Checked & rb_Oracle.Checked)
                {
                    tb_DefaultPort.Text = "1521";
                    cb_DefaultPort.Enabled = true;
                    tb_DefaultPort.ReadOnly = true;
                }
                else
                {
                    tb_DefaultPort.ResetText();
                    tb_DefaultPort.ReadOnly = false;

                    if (rb_Oracle.Checked)
                    {
                        tb_DefaultPort.Enabled = true;
                    }
                    else
                    {
                        tb_DefaultPort.Enabled = false;
                    }
                }
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
                    rb_Oracle.Checked = dBConnectionDetails.bool_Rb_SRC_Oracle;
                    rb_SQL.Checked = dBConnectionDetails.bool_Rb_SRC_SQL;

                    cb_windowsAuthentication.Checked = dBConnectionDetails.bool_SRC_SQL_WA;
                    cb_DefaultPort.Checked = dBConnectionDetails.bool_SRC_Oracle_DefaultPort;
                    tb_DefaultPort.Text = dBConnectionDetails.bool_Rb_SRC_Oracle ? dBConnectionDetails.int_SRC_Oracle_DefaultPort.ToString() : string.Empty;

                    tb_SRC_DataSource.Text = dBConnectionDetails.str_SRC_DataSource;
                    tb_SRC_DBName.Text = dBConnectionDetails.str_SRC_DBName;
                    tb_SRC_UserName.Text = dBConnectionDetails.str_SRC_UserName;
                    tb_SRC_Password.Text = dBConnectionDetails.str_SRC_Password;

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
                    rb_Oracle.Checked = dBConnectionDetails.bool_Rb_TRG_Oracle;
                    rb_SQL.Checked = dBConnectionDetails.bool_Rb_TRG_SQL;

                    cb_windowsAuthentication.Checked = dBConnectionDetails.bool_TRG_SQL_WA;
                    cb_DefaultPort.Checked = dBConnectionDetails.bool_TRG_Oracle_DefaultPort;
                    tb_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString();

                    tb_SRC_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                    tb_SRC_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                    tb_SRC_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                    tb_SRC_Password.Text = dBConnectionDetails.str_TRG_Password;

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
                                    dBConnectionStatus_SRC.strCMDText = FetchSQLQueryForText(str, strPath);

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
                int_Count = 0;
                if (dBConnectionStatus_SRC.boolConnection)
                {
                    if (boolSRC_Folder && boolTRG_Folder)
                    {

                        directoryInfo_SRC = new DirectoryInfo(llb_Excel_SRC.Text);
                        directoryInfo_TRG = new DirectoryInfo(llb_Excel_TRG.Text);

                        foreach (FileInfo fileInfo in directoryInfo_SRC.GetFiles("*.txt", SearchOption.TopDirectoryOnly))
                        {
                            strQuery = File.ReadAllText(fileInfo.FullName);
                            strQuery = strQuery.Replace("\r", " ").Replace("\n", " ");
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
                            UpdateProgress(++int_Count, directoryInfo_SRC.EnumerateFiles("*.txt", SearchOption.TopDirectoryOnly).Count());
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

        private void cb_WindowAuthentication_SRC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (cb_windowsAuthentication.Checked)
                {
                    tb_SRC_UserName.Enabled = false;
                    tb_SRC_Password.Enabled = false;
                }
                else if (cb_windowsAuthentication.Checked == false)
                {
                    tb_SRC_UserName.Enabled = true;
                    tb_SRC_Password.Enabled = true;
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
                string str_DBName = tb_SRC_DBName.Text.Trim();

                if (rb_Oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Oracle...");

                    dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_Oracle.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_windowsAuthentication.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_DefaultPort.Text.Trim());
                }
                else if (rb_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to SQL...");
                    dBConnectionStatus_SRC = dBConnectionStatus_SRC.DatabaseConnection(rb_SQL.Text, tb_SRC_DataSource.Text.Trim(), str_DBName, cb_windowsAuthentication.Checked, tb_SRC_UserName.Text.Trim(), tb_SRC_Password.Text.Trim(), tb_DefaultPort.Text.Trim());
                }

                Common_Data.DisplayMessage(rtb_Log, dBConnectionStatus_SRC.strConnectionMSG, (!dBConnectionStatus_SRC.boolConnection), -1, (dBConnectionStatus_SRC.boolConnection) ? Color.DarkGreen : Color.Red);
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }

        }
    }
}
