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
using System.Globalization;
using System.IO;

namespace SPI_Data_Comp_Tool
{
    public partial class Generate_IUD_Script_Form : Form
    {
        private OpenFileDialogData openFileDialogData;
        private DataSet dsGenerateScript;
        private Common_Data common_Data;
        private bool boolGenerate;
        private DataTable dtGenerateScript;
        private string strInsertColName, strInsertValues, strTableName, strFullPathFileName;
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
                var selectedFile = new OpenFileDialogData().openFileDialog(strOpenDialog);
                if (selectedFile.boolFileSelected)
                {
                    if (!FormValidation.Check(this, () => FormValidation.ReadableFile(selectedFile.strFileNameWithPath,
                        btn_ListTableExcel, "Input workbook", ".xlsx", ".xlsm"), ReportValidation)) return;
                    DataSet candidate = ReadExcel.LoadExcelFromDataReader(selectedFile.strFileNameWithPath);
                    if (!FormValidation.Check(this, () => ValidateScriptWorkbook(candidate), ReportValidation))
                    { candidate?.Dispose(); return; }
                    openFileDialogData = selectedFile;
                    Common_Data.DisplayMessage(rtb_Log, $"File Read : {openFileDialogData.strFileNameWithPath}");
                    SetWorkbook(candidate);
                    if (dtGenerateScript.Rows.Count == 0)
                        Common_Data.DisplayMessage(rtb_Log, "The selected worksheet has headers but no data. Select another worksheet before generating scripts.");
                }
                else
                {
                    Common_Data.DisplayMessage(rtb_Log, "Import cancelled; the previous input and generated output are unchanged.");
                    return;
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

        private void ValidateScriptWorkbook(DataSet candidate)
        {
            FormValidation.Require(candidate != null && candidate.Tables.Count > 0, btn_ListTableExcel,
                "The workbook contains no readable worksheets.");
            foreach (DataTable table in candidate.Tables)
                FormValidation.Table(table, btn_ListTableExcel, "Worksheet " + table.TableName, false);
        }

        internal void SetWorkbook(DataSet candidate)
        {
            // A header-only first sheet must not hide a populated later sheet. Import validates
            // the workbook structure; Generate validates the currently selected sheet's rows.
            ValidateScriptWorkbook(candidate);
            dsGenerateScript = candidate;
            dtGenerateScript = null;
            boolGenerate = false;
            strTableName = strFullPathFileName = strInsertColName = string.Empty;
            ListCols = new List<DataColumn>();
            cb_SelectSheet.Items.Clear();
            clb_Checked.Items.Clear();
            cb_SelectSheet.Items.AddRange(candidate.Tables.Cast<DataTable>().Select(table => table.TableName).ToArray());
            cb_SelectSheet.SelectedIndex = 0;
            llb_File.ResetText();
            llb_File.Visible = false;
        }

        internal string OutputWorksheetName()
        {
            // SQL identifiers may contain brackets or exceed Excel's 31-character sheet limit.
            // Retain the original Excel sheet name independently of the generated SQL target.
            return FormValidation.WorksheetName(Convert.ToString(cb_SelectSheet.SelectedItem), cb_SelectSheet);
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
                if (cb_SelectSheet.SelectedIndex < 0 || cb_SelectSheet.SelectedIndex >= dsGenerateScript.Tables.Count) return;
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
                    clb_Checked.Items.AddRange(InputColumns(dtGenerateScript));
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
                    strTableName = openFileDialogData.SimpleFileName_WithoutExt ?? string.Empty;

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
                    strTableName = openFileDialogData.SimpleFileName_WithoutExt ?? string.Empty;

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
                if (!FormValidation.Check(this, ValidateGenerationInput, ReportValidation)) return;
                strScriptFor = "InsertScriptFor";

                saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Excel File", $"{strScriptFor}_{common_Data.RemoveSpecialCharacters(OutputWorksheetName())}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}");

                if (saveFileDialogFile.BoolFileSaveStatus)
                {
                    if (!FormValidation.Check(this, ValidateOutput, ReportValidation)) return;
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
                if (dsGenerateScript != null && cb_SelectSheet.SelectedIndex >= 0)
                    cb_SelectSheetInsert_SelectedIndexChanged(this, EventArgs.Empty);
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
                boolGenerate = false;
                string strInsertScripts = string.Empty;
                dtGenerateScript = dtGenerateScript.Copy();
                string[] inputColumns = InputColumns(dtGenerateScript);
                strInsertColName = string.Join(", ", inputColumns.Select(QuoteColumn));

                dtGenerateScript = common_Data.RemoveColumn(dtGenerateScript, "Insert_Scripts");


                if (!(dtGenerateScript.Columns.Contains("Insert_Scripts")))
                {
                    Common_Data.DisplayMessage(rtb_Log, "Generating Insert Scripts");
                    dtGenerateScript.Columns.Add("Insert_Scripts", typeof(string));

                    strTableName = tb_TableName.Text.Trim();

                    foreach (DataRow dr in dtGenerateScript.Rows)
                    {
                        strInsertValues = string.Empty;
                        strInsertValues = string.Join(", ", inputColumns.Select(column => SqlLiteral(dr[column])));
                        strInsertScripts = $"Insert Into {strTableName} ({strInsertColName}) Values ({strInsertValues})";

                        dr["Insert_Scripts"] = strInsertScripts;
                    }

                    if (WriteExcel.ExportToExcel(dtGenerateScript, OutputWorksheetName(), strFullPathFileName))
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
                if (!FormValidation.Check(this, ValidateGenerationInput, ReportValidation)) return;
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

                saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Excel File", $"{strScriptFor}_{common_Data.RemoveSpecialCharacters(OutputWorksheetName())}_{common_Data.AppendDateInOutputFileName(DateTime.Now)}");

                if (saveFileDialogFile.BoolFileSaveStatus)
                {
                    if (!FormValidation.Check(this, ValidateOutput, ReportValidation)) return;
                    dtGenerateScript = dtGenerateScript.Copy();
                    strFullPathFileName = saveFileDialogFile.Str_saveFileNameWithPath;
                    IEnumerable<object> notCheckedValuesEnum = (from object item in clb_Checked.Items where !clb_Checked.CheckedItems.Contains(item) select item);
                    IEnumerable<object> checkedValuesEnum = (from object item in clb_Checked.CheckedItems select item);

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
                            strSetClause = $" SET {string.Join(" , ", notCheckedValuesEnum.Select(x => QuoteColumn(x.ToString()) + " = " + SqlLiteral(dr[x.ToString()])))} ";
                            strWhereClause = string.Empty;
                            if (checkedValuesEnum.Count() > 0)
                            {
                                strWhereClause = " WHERE " + string.Join(" AND ", checkedValuesEnum.Select(x => SqlPredicate(x.ToString(), dr[x.ToString()])));
                            }
                            strUpdateTable = $"{strUpdateClause}{strSetClause}{strWhereClause}";

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
                                strWhereClause = " WHERE " + string.Join(" AND ", checkedValuesEnum.Select(x => SqlPredicate(x.ToString(), dr[x.ToString()])));
                            }
                            strUpdateTable = $"{strUpdateClause}{strWhereClause}";

                            dr["Delete_Scripts"] = strUpdateTable;
                        }
                        boolGenerate = true;
                    }
                    else
                    {
                        Common_Data.DisplayMessage(rtb_Log, $"All checkboxes are selected, 'SET' clause can't be evaluated or there is some problem", true);
                        boolGenerate = false;
                    }

                    if (boolGenerate && WriteExcel.ExportToExcel(dtGenerateScript, OutputWorksheetName(), strFullPathFileName))
                    {
                        boolGenerate = true;
                        Common_Data.DisplayMessage(rtb_Log, $"Script File is Saved Successfully in : {strFullPathFileName}");
                    }
                    else
                    {
                        boolGenerate = false;
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
                if (!FormValidation.Check(this, () => FormValidation.ReadableFile(strFullPathFileName, llb_File,
                    "Generated workbook", ".xlsx"), ReportValidation)) return;
                Common_Data.DisplayMessage(rtb_Log, $"Opening File :{strFullPathFileName}");
                Process.Start(strFullPathFileName);
            }
            catch (Exception ex)
            {
                Common_Data.DisplayError(rtb_Log, ex);
            }
        }

        private void ReportValidation(string message) { Common_Data.DisplayMessage(rtb_Log, message, true); }

        private static string[] InputColumns(DataTable table)
        {
            return table.Columns.Cast<DataColumn>().Select(column => column.ColumnName)
                .Where(name => !new[] { "DBTYPE", "INSERT_SCRIPTS", "UPDATE_SCRIPTS", "DELETE_SCRIPTS" }
                    .Contains(name.ToUpperInvariant())).ToArray();
        }

        internal void ValidateGenerationInput()
        {
            FormValidation.Require(new[] { rb_Insert.Checked, rb_Update.Checked, rb_Delete.Checked }.Count(value => value) == 1,
                btn_Script, "Choose exactly one script type.");
            FormValidation.Selection(cb_SelectSheet, "Worksheet");
            FormValidation.Table(dtGenerateScript, cb_SelectSheet, "Selected worksheet");
            FormValidation.Required(tb_TableName, "Table name");
            ValidateTableName(tb_TableName.Text, tb_TableName);
            string[] columns = InputColumns(dtGenerateScript);
            FormValidation.Require(columns.Length > 0, cb_SelectSheet, "The worksheet has no data columns to generate scripts.");
            if (rb_Update.Checked || rb_Delete.Checked)
            {
                string[] keys = clb_Checked.CheckedItems.Cast<string>().ToArray();
                FormValidation.Require(keys.Length > 0, clb_Checked, "Select at least one WHERE column; unrestricted UPDATE/DELETE scripts are not generated.");
                FormValidation.Columns(dtGenerateScript, keys, clb_Checked, "WHERE columns");
                FormValidation.Require(keys.All(key => columns.Contains(key)), clb_Checked, "Generated script columns cannot be used as WHERE columns.");
                if (rb_Update.Checked)
                    FormValidation.Require(columns.Except(keys).Any(), clb_Checked, "Leave at least one data column for the UPDATE SET clause.");
            }
        }

        private void ValidateOutput()
        {
            string output = FormValidation.OutputFile(saveFileDialogFile.Str_saveFileNameWithPath, btn_Script, "Output workbook", ".xlsx");
            FormValidation.DifferentFiles(output, openFileDialogData.strFileNameWithPath, btn_Script);
        }

        internal static void ValidateTableName(string name, Control field)
        {
            // Do not accept an executable SQL fragment in the table-name box. Quoted identifiers
            // are kept intact, including spaces/dots inside square brackets or double quotes.
            bool bracket = false, quoted = false, hasPart = false;
            int parts = 1;
            string text = (name ?? string.Empty).Trim();
            FormValidation.Require(text.Length > 0, field, "Enter a table name.");
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (bracket)
                {
                    if (c == ']') { if (i + 1 < text.Length && text[i + 1] == ']') i++; else bracket = false; }
                    continue;
                }
                if (quoted)
                {
                    if (c == '"') { if (i + 1 < text.Length && text[i + 1] == '"') i++; else quoted = false; }
                    continue;
                }
                if (c == '[' || c == '"') { FormValidation.Require(!hasPart, field, "Use one identifier per table-name component."); bracket = c == '['; quoted = c == '"'; hasPart = true; }
                else if (c == '.') { FormValidation.Require(hasPart && ++parts <= 4, field, "Enter a valid schema-qualified table name."); hasPart = false; }
                else { FormValidation.Require(char.IsLetterOrDigit(c) || c == '_' || c == '@' || c == '#' || c == '$', field,
                        "Use a table name, not a SQL expression; quote names containing spaces."); hasPart = true; }
            }
            FormValidation.Require(hasPart && !bracket && !quoted, field, "Complete the table name and its identifier quotes.");
        }

        internal static string QuoteColumn(string name) { return "[" + name.Replace("]", "]]") + "]"; }
        internal static string SqlLiteral(object value)
        {
            if (value == null || value == DBNull.Value) return "NULL";
            string text = value is DateTime ? ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
            return "N'" + text.Replace("'", "''") + "'";
        }
        internal static string SqlPredicate(string column, object value)
        {
            return QuoteColumn(column) + (value == null || value == DBNull.Value ? " IS NULL" : " = " + SqlLiteral(value));
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
