using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_Data_Comp_Tool
{
    public partial class MergeLogsExport : Form
    {
        private OpenFileDialogData openFileDialogData;
        private DataTable dtMerge_Log_Output;
        private SaveFileDialogFile saveFileDialogFile;
        private Common_Data common_Data;
        private DataSet dsAll;
        private DataTable dt;

        public MergeLogsExport()
        {
            InitializeComponent();

            try
            {
                openFileDialogData = new OpenFileDialogData();
                dtMerge_Log_Output = new DataTable();
                common_Data = new Common_Data();

                string[] strIntColumnNames = { "Total Source Rows", "Inserted Rows", " Updated Rows", "Rejected Rows" };
                var dcColumnNames = strIntColumnNames.AsEnumerable().Select(x => new DataColumn(x.ToString(), typeof(int))).ToArray();

                dtMerge_Log_Output.Columns.Add("Table Name");
                dtMerge_Log_Output.Columns.AddRange(dcColumnNames);

                string[] strSTRColumnNames = { "Target Sql", "Source Sql" };
                dcColumnNames = strSTRColumnNames.AsEnumerable().Select(x => new DataColumn(x.ToString())).ToArray();

                dtMerge_Log_Output.Columns.AddRange(dcColumnNames);
                dtMerge_Log_Output.TableName = "Merge Log Out";

                saveFileDialogFile = new SaveFileDialogFile();

                dsAll = new DataSet();
                dt = new DataTable();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }

        }

        private void lbl_ReadTxtFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                openFileDialogData = openFileDialogData.openFileDialog("Browse Text Log File", "txt,log");

                if (openFileDialogData.boolFileSelected)
                {
                    lbl_ReadTxtFile.Text = openFileDialogData.strFileNameWithPath;
                    Common_Data.DisplayMessage(rtb_Log, $"File Selected : {openFileDialogData.strFileNameWithPath}");

                    if (!(Debugger.IsAttached))
                    {
                        saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Excel File", $"Merge_Log_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}");
                    }
                    else
                    {
                        saveFileDialogFile.Str_saveFileNameWithPath = $@"D:\Krishna\Projects\SPI\INPUT FILES\Merge Log Export\Output\Merge_Log_Output_{common_Data.AppendDateInOutputFileName(DateTime.Now)}.xlsx";
                        saveFileDialogFile.BoolFileSaveStatus = true;
                    }

                    if (saveFileDialogFile.BoolFileSaveStatus)
                    {
                        string strAllText = File.ReadAllText(openFileDialogData.strFileNameWithPath);
                        string strAllTextToUse = strAllText.Replace("Table Name", "|Table Name");

                        //string[] strTableName = File.ReadAllLines(openFileDialogData.strFileNameWithPath).AsEnumerable().Where(x => x.Contains("Table Name") || x.Contains("Total Source Rows") || x.Contains("Target Sql") || x.Contains("Source Sql")).ToArray();

                        //List<string> lstTableName = strAllTextToUse.Split(new[] { "|" },StringSplitOptions.RemoveEmptyEntries).Where(x => x.Contains("Table Name")).ToList();

                        string[] arrTableName = strAllTextToUse.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Contains("Table Name")).ToArray();
                        string[] strRR = arrTableName.Where(x => x.Contains("Rejected Rows") & (!x.Contains("Rejected Rows: 0"))).ToArray();

                        dtMerge_Log_Output.Clear();
                        int int_index = 0;
                        foreach (string ss in arrTableName)
                        {
                            Common_Data.UpdateProgressBar(progressBar1, ++int_index,arrTableName.Length);
                            Common_Data.DisplayDebugMsg(rtb_Log, $"{ss}");
                            string[] strDetails = ss.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            string strTN = strDetails[0].Substring(strDetails[0].IndexOf(":") + 1, strDetails[0].IndexOf(";") - strDetails[0].IndexOf(":") - 1).Trim();
                            Common_Data.DisplayMessage(rtb_Log, $"Processing Table : {strTN}");
                            string strTRG = strDetails[2].Substring(strDetails[2].IndexOf(":") + 1).Trim();
                            string strSRC = strDetails[4].Substring(strDetails[4].IndexOf(":") + 1).Trim();

                            Common_Data.DisplayDebugMsg(rtb_Log, $"Evaluating {strDetails.Length - 1}:{strDetails[strDetails.Length - 1]}");
                            //string[] strRowDetails = strDetails[strDetails.Length - 1].Split(';');
                            string[] strRowDetails = strDetails.Where(x => x.Contains("Total Source Rows")).ToArray()[0].Split(';');

                            string strTotalSRCRows = strRowDetails[0].Substring(strRowDetails[0].IndexOf(":") + 1).Trim();
                            string strInsertedRows = strRowDetails[1].Substring(strRowDetails[1].IndexOf(":") + 1).Trim();
                            string strUpdatedRows = strRowDetails[2].Substring(strRowDetails[2].IndexOf(":") + 1).Trim();
                            string strRejectedRows = strRowDetails[3].Substring(strRowDetails[3].IndexOf(":") + 1).Trim();

                            dtMerge_Log_Output.Rows.Add(strTN, strTotalSRCRows, strInsertedRows, strUpdatedRows, strRejectedRows, strTRG, strSRC);
                        }

                        dsAll.Clear();
                        dsAll.Tables.Clear();
                        dsAll.Tables.Add(dtMerge_Log_Output.Copy());
                        
                        dt = common_Data.AddColumns(dt,"Rejection Remarks");

                        Common_Data.DisplayMessage(rtb_Log, $"Now Processing Rejected Rows", false, -1, Color.DarkBlue);
                        int_index = 0;

                        // Processing for Rejected Rows
                        foreach (string sr in strRR)
                        {
                            Common_Data.UpdateProgressBar(progressBar1, ++int_index, strRR.Length);
                            Common_Data.DisplayDebugMsg(rtb_Log, sr);
                            string[] strDetails = sr.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            string strTN = strDetails[0].Substring(strDetails[0].IndexOf(":") + 1, strDetails[0].IndexOf(";") - strDetails[0].IndexOf(":") - 1).Trim();

                            //int int_index_Source = sr.IndexOf("Testing") + 7;
                            //int int_count_Index = sr.IndexOf("Total Source Rows:") - sr.IndexOf("Testing") - 7;

                            int int_first_ID = sr.IndexOf("First Id");
                            int int_source_data_by_datastore = sr.IndexOf("Total Source Rows") - 25;

                            
                            //if (sr.IndexOf("Testing source data by datastore") < sr.LastIndexOf("Testing source data by datastore"))
                            if (int_source_data_by_datastore > int_first_ID)
                            {
                                //string txt = sr.Substring(sr.IndexOf("Testing source data by datastore") + 34, sr.IndexOf("Total Source Rows:") - sr.LastIndexOf("Testing source data by datastore") - 36);

                                //string txt = sr.Substring(int_index_Source, int_count_Index);
                                string txt = sr.Substring(int_first_ID, int_source_data_by_datastore);

                                dt.Clear();
                                dt.TableName = strTN;
                                txt.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => x != null && x.Trim() != "").ToList().ForEach(x => dt.Rows.Add(x));

                                dsAll = common_Data.InsertDataTable(dsAll, dt, strTN);

                                Common_Data.DisplayDebugMsg(rtb_Log, $"Added Table in Dataset : {dt.TableName}");
                            }

                        }

                        if (WriteExcel.ExportToExcel(dsAll, saveFileDialogFile.Str_saveFileNameWithPath))
                        {
                            Common_Data.DisplayMessage(rtb_Log, $"File saved as : {saveFileDialogFile.Str_saveFileNameWithPath}");
                            Process.Start(saveFileDialogFile.Str_saveFileNameWithPath);
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, $"There is some problem in saving the file : {saveFileDialogFile.Str_saveFileNameWithPath}", true);
                        }

                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"File Not Saved By User !!!", true);
                    }
                }
                else
                {
                    lbl_ReadTxtFile.Text = $"No File Selected";
                    Common_Data.DisplayMessage(rtb_Log, $"File Selected : {openFileDialogData.strFileNameWithPath}", true);
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void btn_Clear_Reset_Click(object sender, EventArgs e)
        {
            try
            {
                rtb_Log.Clear();
                Common_Data.DisplayMessage(rtb_Log, "Logs Cleared");
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
    }
}
