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

namespace SPI_Data_Comp_Tool
{
    public partial class Generate_IUD_Script_Form : Form
    {
        private OpenFileDialogData openFileDialogData;
        private DataSet dsGenerateScript;
        private Common_Data common_Data;
        private bool boolColorAlternate, boolGenerate;
        private DataTable dtGenerateScript;
        private string strInsertColName, strInsertValues, strTableName, strFullPathFileName;
        private FolderBrowserDialog folderBrowserDialog;
        private bool boolFolderPathSelected;
        private SaveFileDialogFile saveFileDialogFile;

        private List<DataColumn> ListCols;

        private string strUpdateClause, strSetClause, strWhereClause, strUpdateTable, strScriptFor;

        public Generate_IUD_Script_Form()
        {
            InitializeComponent();

            try
            {
                common_Data = new Common_Data();
                dsGenerateScript = new DataSet();
                openFileDialogData = new OpenFileDialogData();
                saveFileDialogFile = new SaveFileDialogFile();
                boolColorAlternate = false;
                strScriptFor = string.Empty;

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }

        }


        public void GenerateScriptProcess(string strOpenDialog, string strScriptFor)
        {
            try
            {
                boolGenerate = false;
                strTableName = string.Empty;
                strFullPathFileName = string.Empty;
                boolFolderPathSelected = false;

                openFileDialogData = openFileDialogData.openFileDialog(strOpenDialog);

                if (openFileDialogData.boolFileSelected)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"File Read : {openFileDialogData.strFileNameWithPath}");
                    dsGenerateScript = ReadExcel.LoadExcelFromDataReader(openFileDialogData.strFileNameWithPath);

                    if (dsGenerateScript.Tables.Count > 0)
                    {
                        dtGenerateScript = dsGenerateScript.Tables[0];
                        //dtGenerateScript = common_Data.RemoveColumn(dtGenerateScript, "DBType");
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, "No Excel Sheet Fetched", true);
                        return;
                    }

                    if (dtGenerateScript != null)
                    {
                        if (dtGenerateScript.Rows.Count > 0)
                        {
                            strInsertColName = string.Empty;
                            strTableName = dtGenerateScript.TableName;

                            ListCols = (from DataColumn col in dtGenerateScript.Columns select col).ToList();

                            if (strScriptFor == "InsertScriptFor")
                            {
                                cb_SelectSheet.Items.Clear();
                                cb_SelectSheet.Items.AddRange(dsGenerateScript.Tables.Cast<DataTable>().Select(x => x.TableName.ToString()).ToArray());
                            }
                            else if (strScriptFor == "UpdateScriptFor" || strScriptFor == "DeleteScriptFor")
                            {
                                cb_SelectSheet.Items.Clear();
                                cb_SelectSheet.Items.AddRange(dsGenerateScript.Tables.Cast<DataTable>().Select(x => x.TableName.ToString()).ToArray());

                                boolGenerate = false;
                                clb_Checked.Items.Clear();
                                clb_Checked.Items.AddRange(ListCols.Select(x => x.ColumnName).ToArray());

                                RemoveCustomiseDBType();
                            }
                        }
                        else
                        {
                            Common_Data.DisplayMessage(rtb_Log, "No rows to generate insert scripts");
                        }
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"No File Selected", true);
                }

                if (boolGenerate)
                {
                    llb_File.Text = strFullPathFileName;
                    llb_File.Visible = true;
                }
                else
                {
                    llb_File.ResetText();
                    llb_File.Visible = false;
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void RemoveCustomiseDBType()
        {
            try
            {
                clb_Checked.Items.Remove("DBTYPE");
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void ProcessUpdateScript()
        {
            try
            {


            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void cb_SelectSheetInsert_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_Insert.Checked)
                {
                    dtGenerateScript = dsGenerateScript.Tables[cb_SelectSheet.SelectedIndex];
                    setTableNameInsertScript();
                    ListCols = (from DataColumn col in dtGenerateScript.Columns select col).ToList();
                }
                else if (rb_Update.Checked || rb_Delete.Checked)
                {
                    dtGenerateScript = dsGenerateScript.Tables[cb_SelectSheet.SelectedIndex];
                    setTableNameUpdateDeleteScript();
                    ListCols = (from DataColumn col in dtGenerateScript.Columns select col).ToList();
                    
                    clb_Checked.Items.Clear();
                    clb_Checked.Items.AddRange(ListCols.Select(x => x.ColumnName).Where(x => x.ToUpper() != "DBTYPE").ToArray());
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void setTableNameInsertScript()
        {
            try
            {
                //dtGenerateScript = common_Data.RemoveColumn(dtGenerateScript, "DBType");
                if (rb_FromFileName.Checked)
                {
                    strTableName = openFileDialogData.SimpleFileName_WithoutExt;

                    if (strTableName.IndexOf("_Output", 0, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        strTableName = strTableName.Substring(0, strTableName.IndexOf("_Output", 0, StringComparison.OrdinalIgnoreCase));
                    }
                }
                else
                {
                    strTableName = Common_Data.GetComboBoxValue(cb_SelectSheet);
                }

                tb_TableName.Text = strTableName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void setTableNameUpdateDeleteScript()
        {
            try
            {
                //dtGenerateScript = common_Data.RemoveColumn(dtGenerateScript, "DBType");
                if (rb_FromFileName.Checked)
                {
                    strTableName = openFileDialogData.SimpleFileName_WithoutExt;

                    if (strTableName.IndexOf("_Output", 0, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        strTableName = strTableName.Substring(0, strTableName.IndexOf("_Output", 0, StringComparison.OrdinalIgnoreCase));
                    }
                }
                else
                {
                    strTableName = Common_Data.GetComboBoxValue(cb_SelectSheet);
                }

                tb_TableName.Text = strTableName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cb_UpdateDeleteSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void rb_FromFileName_Insert_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CallForOption();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_SheetName_Insert_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CallForOption();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void CallForOption()
        {
            try
            {
                if (rb_Insert.Checked)
                {
                    setTableNameInsertScript();
                }
                else if (rb_Update.Checked || rb_Delete.Checked)
                {
                    setTableNameUpdateDeleteScript();
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
        private void rb_FromFileName_UD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                setTableNameUpdateDeleteScript();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_SheetName_UD_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void btn_Insert_Script_Click(object sender, EventArgs e)
        {

        }

        private void InsertScripts()
        {
            try
            {
                strScriptFor = "InsertScriptFor";

                saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Excel File", $"{strScriptFor}_{strTableName}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}");

                if (saveFileDialogFile.BoolFileSaveStatus)
                {
                    strFullPathFileName = saveFileDialogFile.Str_saveFileNameWithPath;
                    Common_Data.DisplayMessage(rtb_Log, $"Table identified as : '{strTableName}', TableName is read from the 1st sheet name of excel workbook");

                    GenerateInsertScripts();
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"File Not Saved By User", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void btn_Script_Click(object sender, EventArgs e)
        {
            try
            {
                if(rb_Insert.Checked)
                {
                    InsertScripts();
                }
                else if(rb_Update.Checked || rb_Delete.Checked)
                {
                    UpdateDeleteScripts();
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_Insert_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                TextInButton();
            }
            catch(Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void TextInButton()
        {
            try
            {
                label3.Visible = !rb_Insert.Checked;
                clb_Checked.Visible = !rb_Insert.Checked;
                if (rb_Insert.Checked)
                {
                    btn_Script.Text = "Generate Insert Scripts";
                }
                else if (rb_Update.Checked)
                {
                    btn_Script.Text = "Generate Update Scripts";
                }
                else if (rb_Delete.Checked)
                {
                    btn_Script.Text = "Generate Delete Scripts";
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_Update_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                TextInButton();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void rb_Delete_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                TextInButton();
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void GenerateInsertScripts()
        {
            try
            {
                string strInsertScripts = string.Empty;
                strInsertColName = string.Join(", ", ListCols.Where(y => y.ColumnName.ToUpper() != "DBTYPE").Select(x => string.Join(", ", x.ColumnName)));

                dtGenerateScript = common_Data.RemoveColumn(dtGenerateScript, "Insert_Scripts");


                if (!(dtGenerateScript.Columns.Contains("Insert_Scripts")))
                {
                    Common_Data.DisplayMessage(rtb_Log, "Generating Insert Scripts");
                    dtGenerateScript.Columns.Add("Insert_Scripts", typeof(string));

                    strTableName = tb_TableName.Text;

                    foreach (DataRow dr in dtGenerateScript.Rows)
                    {
                        strInsertValues = string.Empty;
                        strInsertValues = string.Join(", ", ListCols.Where(y => y.ColumnName.ToUpper() != "DBTYPE").Select(x => string.Join(", ", $"N'{dr[x.ColumnName].ToString().Trim()}'")));
                        strInsertScripts = $"Insert Into {strTableName} ({strInsertColName}) Values ({strInsertValues})";

                        if (strInsertScripts.Contains("N''"))
                        {
                            strInsertScripts = strInsertScripts.Replace("N''", "NULL");
                        }

                        while (strInsertScripts.Contains("  "))
                        {
                            strInsertScripts = strInsertScripts.Replace("  ", " ");
                        }

                        dr["Insert_Scripts"] = strInsertScripts;
                    }

                    if (WriteExcel.ExportToExcel(dtGenerateScript, strTableName, strFullPathFileName))
                    {
                        boolGenerate = true;
                        Common_Data.DisplayMessage(rtb_Log, $"Insert Script File is Saved Successfully in : {strFullPathFileName}");
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"There is some problem is saving the file", true);
                    }

                    if (boolGenerate)
                    {
                        llb_File.Text = strFullPathFileName;
                        llb_File.Visible = true;
                    }
                    else
                    {
                        llb_File.ResetText();
                        llb_File.Visible = false;
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Insert_Scripts Column already present, pls delete manually and re run the utility", true);
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
                throw ex;
            }
        }

        private void cb_UpdateDelete_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rb_Update.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Checkbox selected, 'UPDATE' Scripts will be generated");
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Checkbox unselected, 'DELETE' Scripts will be generated");
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
                if (rb_Insert.Checked)
                {
                    GenerateScriptProcess("Select Excel File To Generate Insert Scripts", "InsertScriptFor");
                }
                else if (rb_Update.Checked)
                {
                    GenerateScriptProcess("Select Excel File To Generate Update Scripts", "UpdateScriptFor");
                }
                else if (rb_Delete.Checked)
                {
                    GenerateScriptProcess("Select Excel File To Generate Delete Scripts", "DeleteScriptFor");
                }
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void btn_UpdateScript_Click(object sender, EventArgs e)
        {

        }

        private void UpdateDeleteScripts()
        {
            try
            {
                boolGenerate = false;

                if (rb_Update.Checked)
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Generating 'UPDATE' Scripts for the input file");
                    strScriptFor = "UpdateScriptFor";
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"Generating 'DELETE' Scripts for the input file");
                    strScriptFor = "DeleteScriptFor";
                }

                saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Excel File", $"{strScriptFor}_{strTableName}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}");

                if (saveFileDialogFile.BoolFileSaveStatus)
                {
                    strFullPathFileName = saveFileDialogFile.Str_saveFileNameWithPath;
                    IEnumerable<object> notCheckedValuesEnum = (from object item in clb_Checked.Items where !clb_Checked.CheckedItems.Contains(item) select item);
                    IEnumerable<object> checkedValuesEnum = (from object item in clb_Checked.CheckedItems select item);

                    if (checkedValuesEnum.Count() == 0)
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"Pls Note, No checkbox selected, so scripts will be generated without 'WHERE' clause !!!", true);
                    }

                    strTableName = tb_TableName.Text;

                    if (rb_Update.Checked && notCheckedValuesEnum.Count() > 0)
                    {
                        strUpdateClause = $"UPDATE {strTableName} ";
                        dtGenerateScript = common_Data.AddColumns(dtGenerateScript, "Update_Scripts", typeof(string));

                        foreach (DataRow dr in dtGenerateScript.Rows)
                        {
                            if(dtGenerateScript.Columns.Contains("DBTYPE") && dr["DBTYPE"].ToString() == "Target")
                            {
                                continue;
                            }
                            strSetClause = $" SET {string.Join(" , ", notCheckedValuesEnum.Select(x => string.Join(" = ", x, $"N'{dr[x.ToString()].ToString().Trim()}'")))} ";
                            strWhereClause = string.Empty;
                            if (checkedValuesEnum.Count() > 0)
                            {
                                strWhereClause = $" WHERE {string.Join(" AND ", checkedValuesEnum.Select(x => string.Join(" = ", x, $"N'{dr[x.ToString()].ToString().Trim()}'")))} ";
                            }
                            strUpdateTable = $"{strUpdateClause}{strSetClause}{strWhereClause}";

                            if (strUpdateTable.Contains("N''"))
                            {
                                strUpdateTable = strUpdateTable.Replace("N''", "NULL");
                            }

                            while (strUpdateTable.Contains("  "))
                            {
                                strUpdateTable = strUpdateTable.Replace("  ", " ");
                            }

                            dr["Update_Scripts"] = strUpdateTable;
                        }
                        boolGenerate = true;
                    }
                    else if ((!rb_Update.Checked))
                    {
                        strUpdateClause = $"DELETE FROM {strTableName} ";

                        dtGenerateScript = common_Data.AddColumns(dtGenerateScript, "Delete_Scripts", typeof(string));

                        foreach (DataRow dr in dtGenerateScript.Rows)
                        {
                            strWhereClause = string.Empty;
                            if (checkedValuesEnum.Count() > 0)
                            {
                                strWhereClause = $" WHERE {string.Join(" AND ", checkedValuesEnum.Select(x => string.Join(" = ", x, $"N'{dr[x.ToString()].ToString().Trim()}'")))} ";
                            }
                            strUpdateTable = $"{strUpdateClause}{strWhereClause}";

                            if (strUpdateTable.Contains("N''"))
                            {
                                strUpdateTable = strUpdateTable.Replace("N''", "NULL");
                            }

                            if (strUpdateTable.Contains("  "))
                            {
                                strUpdateTable = strUpdateTable.Replace("  ", " ");
                            }

                            dr["Delete_Scripts"] = strUpdateTable;
                        }
                        boolGenerate = true;
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"All checkboxes are selected, 'SET' clause can't be evaluated or there is some problem", true);
                        boolGenerate = false;
                    }

                    if (boolGenerate && WriteExcel.ExportToExcel(dtGenerateScript, strTableName, strFullPathFileName))
                    {
                        boolGenerate = true;
                        Common_Data.DisplayMessage(rtb_Log, $"Script File is Saved Successfully in : {strFullPathFileName}");
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"There is some problem is saving the file", true);
                    }

                    if (boolGenerate)
                    {
                        llb_File.Text = strFullPathFileName;
                        llb_File.Visible = true;
                    }
                    else
                    {
                        llb_File.ResetText();
                        llb_File.Visible = false;
                    }
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, $"File Not Saved By User", true);
                }

            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
        private void btn_Update_Click(object sender, EventArgs e)
        {
            
        }

        private void llb_File_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Common_Data.DisplayMessage(rtb_Log, $"Opening File :{strFullPathFileName}");
                Process.Start(strFullPathFileName);
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        //private void DisplayError(Exception ex = null)
        //{
        //    try
        //    {
        //       Common_Data.DisplayMessage(rtb_Log,ex.Message.ToString(), true);
        //        if (Debugger.IsAttached || GlobalDebug.boolIsGlobalDebug)
        //        {
        //           Common_Data.DisplayMessage(rtb_Log,ex.ToString(), true);
        //        }
        //    }
        //    catch (Exception ex1)
        //    {
        //       Common_Data.DisplayMessage(rtb_Log,ex1.Message.ToString(), true);
        //        if (Debugger.IsAttached)
        //        {
        //           Common_Data.DisplayMessage(rtb_Log,ex1.ToString(), true);
        //        }
        //    }
        //}

        //private void DisplayMessage(string strMsg, bool boolError = false, int overwriteText = -1, Color? color = null)
        //{
        //    try
        //    {
        //        rtb_Log.SelectionStart = rtb_Log.TextLength;

        //        if (boolColorAlternate)
        //        {
        //            rtb_Log.SelectionColor = color ?? Color.Black;
        //        }
        //        else
        //        {
        //            rtb_Log.SelectionColor = color ?? Color.DarkBlue;
        //        }

        //        boolColorAlternate = !boolColorAlternate;

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
        //        Common_Data.DisplayError(rtb_Log,ex);
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
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }
    }
}
