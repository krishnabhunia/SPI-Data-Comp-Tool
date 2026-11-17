using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class SPIDeltaChangeLegacyID : Form
    {
        private DataSet dsTableList;
        private DataTable dtLegacyID;

        private OpenFileDialogData openFileDialogData;
        private DBConnectionDetails dBConnectionDetails;
        private Common_Data common_Data;

        private DBConnectionStatus dBConnectionStatus_SRC, dBConnectionStatus_TRG, dBConnectionStatus;

        private string strSourceRowsData;
        private SaveFileDialogFile saveFileDialogFile;

        private string strConnection, strSelectStmt, strValueToFind, strValueToUpdate;
        private DataTable dtFetch;
        private DataRow[] drToChangeValues;

        public SPIDeltaChangeLegacyID()
        {
            InitializeComponent();

            try
            {
                openFileDialogData = new OpenFileDialogData();
                dsTableList = new DataSet();
                dtLegacyID = new DataTable();
                common_Data = new Common_Data();

                dBConnectionStatus_SRC = new DBConnectionStatus();
                dBConnectionStatus_TRG = new DBConnectionStatus();
                dBConnectionStatus = new DBConnectionStatus();

                saveFileDialogFile = new SaveFileDialogFile();

                dtFetch = new DataTable();

                cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }



        private void btn_ListTableExcel_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialogData = openFileDialogData.openFileDialog("Select Excel File");

                if (openFileDialogData.boolFileSelected)
                {
                    saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Excel File", $"{openFileDialogData.SimpleFileName_WithoutExt}_{ common_Data.AppendDateInOutputFileName(DateTime.Now)}");

                    if (saveFileDialogFile.BoolFileSaveStatus)
                    {
                        Common_Data.DisplayMessage(rtb_Log, openFileDialogData.strFileNameWithPath);
                        dsTableList = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);

                        if (dsTableList.Tables.Contains("Source rows only"))
                        {
                            List<DataRow> rows = (from DataRow row in dsTableList.Tables["Source rows only"].Rows select row).ToList();
                            if (dsTableList.Tables.Contains("References_Source_Rows_Only"))
                            {
                                dtLegacyID = dsTableList.Tables["References_Source_Rows_Only"];
                                foreach (DataRow dr in dtLegacyID.Rows)
                                {
                                    strSourceRowsData = string.Empty;

                                    strSourceRowsData = string.Join(", ", rows.Select(x => string.Join(", ", x[dr["Field_to_be_taken"].ToString()])));

                                    strSelectStmt = $"Select {dr["Refrence_Field_Name_for_Mapping"]},{dr["Field_to_be_taken"]} from {dr["Reference_Table_Name"]} Where {dr["Refrence_Field_Name_for_Mapping"]} in ({strSourceRowsData})";

                                    strConnection = dBConnectionStatus_SRC.strConnectionString;

                                    using (SqlDataAdapter sqlDA = new SqlDataAdapter(strSelectStmt, strConnection))
                                    {
                                        sqlDA.Fill(dtFetch);
                                        dtFetch.TableName = dr["Reference_Table_Name"].ToString();
                                    }

                                    dsTableList.Tables["Source rows only"].Columns.Add($"{dr["Source_Rows_Field_Name"].ToString()}_NEW");

                                    foreach (DataRow drUpdate in dtFetch.Rows)
                                    {
                                        strValueToFind = drUpdate[dr["Refrence_Field_Name_for_Mapping"].ToString()].ToString();

                                        strValueToUpdate = drUpdate[dr["Field_to_be_taken"].ToString()].ToString();

                                        drToChangeValues = dsTableList.Tables["Source Rows only"].Select($"{dr["Source_Rows_Field_Name"].ToString()} = '{strValueToFind}'");

                                        foreach (DataRow drChange in drToChangeValues)
                                        {
                                            drChange[$"{dr["Source_Rows_Field_Name"].ToString()}_NEW"] = strValueToUpdate;
                                        }
                                    }
                                }
                                dsTableList.AcceptChanges();
                                Common_Data.DisplayMessage(rtb_Log, "Table Mapped Successfully");

                                if (WriteExcel.ExportToExcel(dsTableList, saveFileDialogFile.Str_saveFileNameWithPath))
                                {
                                    Common_Data.DisplayMessage(rtb_Log, $"Excel Output File Saved Successfully : {saveFileDialogFile.Str_saveFileNameWithPath}");
                                }
                                else
                                {
                                    Common_Data.DisplayMessage(rtb_Log, $"Error in saving output file", true);
                                }
                            }
                            else
                            {
                                Common_Data.DisplayMessage(rtb_Log, $"'References_Source_Rows_Only' Sheet Not Found in the input Excel.", true);
                            }
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, $"'Source rows only' Sheet Not Found in the input Excel.", true);
                        }
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"File Not Saved By User", true);
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"No File Selected", true);
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void DisplayError(Exception ex = null)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, ex.Message.ToString(), true);
                if (Debugger.IsAttached || GlobalDebug.boolIsGlobalDebug)
                {
                    Common_Data.DisplayMessage(rtb_Log, ex.ToString(), true);
                }
            }
            catch (Exception ex1)
            {
                Common_Data.DisplayMessage(rtb_Log, ex1.Message.ToString(), true);
                if (Debugger.IsAttached)
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

        private void btn_Clear_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                rtb_Log.Clear();
                Common_Data.DisplayMessage(rtb_Log, "Logs Cleared");
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

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

                tb_SRC_DataSource.Text = dBConnectionDetails.str_SRC_DataSource;
                tb_SRC_DBName.Text = dBConnectionDetails.str_SRC_DBName;
                tb_SRC_UserName.Text = dBConnectionDetails.str_SRC_UserName;
                tb_SRC_Password.Text = dBConnectionDetails.str_SRC_Password;
                //cb_SRC_TableName.Text = dBConnectionDetails.str_SRC_TableName;

                rb_TRG_Oracle.Checked = dBConnectionDetails.bool_Rb_TRG_Oracle;
                rb_TRG_SQL.Checked = dBConnectionDetails.bool_Rb_TRG_SQL;

                cb_WindowAuthentication_TRG.Checked = dBConnectionDetails.bool_TRG_SQL_WA;
                cb_TRG_DefaultPort.Checked = dBConnectionDetails.bool_TRG_Oracle_DefaultPort;
                tb_TRG_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString();

                tb_TRG_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                tb_TRG_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                tb_TRG_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                tb_TRG_Password.Text = dBConnectionDetails.str_TRG_Password;
                //cb_TRG_TableName.Text = dBConnectionDetails.str_TRG_TableName;

                Common_Data.DisplayMessage(rtb_Log, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
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
                string str_DBName = tb_SRC_DBName.Text.Trim();
                //string str_SchemaName = tb_SchemaName.Text.Trim();

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
                //str_SchemaName = tb_SchemaName.Text.Trim();

                if (rb_TRG_Oracle.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Target Oracle...");

                    dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_Oracle.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                }
                else if (rb_TRG_SQL.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, "Trying to connect to Target SQL...");
                    dBConnectionStatus_TRG = dBConnectionStatus_TRG.DatabaseConnection(rb_TRG_SQL.Text, tb_TRG_DataSource.Text.Trim(), str_DBName, cb_WindowAuthentication_SRC.Checked, tb_TRG_UserName.Text.Trim(), tb_TRG_Password.Text.Trim(), tb_TRG_DefaultPort.Text.Trim());
                }
                Common_Data.DisplayMessage(rtb_Log, dBConnectionStatus_TRG.strConnectionMSG, (!dBConnectionStatus_TRG.boolConnection), -1, Color.DarkBlue);

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }
}
