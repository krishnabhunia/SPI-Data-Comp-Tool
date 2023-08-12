using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using DataTable = System.Data.DataTable;

namespace SPI_Data_Comp_Tool
{
    
    public partial class ViewForMultiReference : Form
    {
        private bool boolSet;
        private string str_query;
        private string str_unique_reference_key;
        private string str_query_source_id;
        //private string str_query_target_id;
        //private string str_query_source_col;
        //private string str_query_target_col;
        private string str_FileName;
        private string str_colName_SRC;
        private string str_colName_TRG;
        private string[] columnName;

        private bool boolNewRow, boolUniqueKey;
        private int rowIndex, columnIndex, intHeadRow, intAlternateRow;
        private int percentage, totalProgressCount, progress;

        private DataSet dsSource;
        private DataSet dsTarget;

        private DataSet dsSource_Com;
        private DataSet dsTarget_Com;

        private DataTable dtSource_Com;
        private DataTable dtTarget_Com;

        private DataTable dtSource_Unmatched;
        private DataTable dtTarget_Unmatched;
        private DataTable dt_unmatched;

        private DataTable dt_ONLYSourceData;
        private DataTable dt_ONLYTargetData;
        private DataTable dt_summarised_difference, dt_transpose;

        private DataView dvSource_Com;
        private DataView dvTarget_Com;

        private DataView dv;
        private DataGridView dgv;// = new DataGridView();
        private DataRow dr;

        private OpenFileDialogData ofdd;// = new opOpenFileDialogData();

        private Exception myException_BG;

        //private DateTime start_Time;
        private Stopwatch stopwatch;
        private TimeSpan time_to_execute;

        //private FileInfo file;
        //private ExcelWorksheet ws;

        private ExcelPackage excelPackage;

        private bool boolRules, boolRulesCheck;
        private DataSet dsRules;
        //private CheckRules checkRules;

        private Common_Data common_Data;

        private int int_count_src_id,int_count_trg_id,int_count_src_col,int_count_trg_col;
        private int int_Count_Comparison;

        public ViewForMultiReference()
        {
            InitializeComponent();

            dsSource = new DataSet();
            dsTarget = new DataSet();

            dsSource_Com = new DataSet();
            dsTarget_Com = new DataSet();

            dtSource_Com = new DataTable();
            dtTarget_Com = new DataTable();

            dtSource_Unmatched = new DataTable();
            dtTarget_Unmatched = new DataTable();

            dt_unmatched = new DataTable();

            dgv = new DataGridView();

            boolNewRow = true;
            boolSet = false;

            ofdd = new OpenFileDialogData();

            myException_BG = new Exception();
            time_to_execute = new TimeSpan();
            //start_Time = new DateTime();

            stopwatch = new Stopwatch();

            dt_ONLYSourceData = new DataTable();
            dt_ONLYTargetData = new DataTable();
            str_FileName = string.Empty;

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            excelPackage = new ExcelPackage();

            dt_summarised_difference = new DataTable();
            dt_transpose = new DataTable();

            boolRules = false;
            dsRules = new DataSet();

            //checkRules = new CheckRules();
            common_Data = new Common_Data();

            int_count_src_id = int_count_trg_id = int_count_src_col = int_count_trg_col = 0;

            tb_FileName.Text = $"{tb_FileName.Text}_{DateTime.Now.ToString().Replace('-','_').Replace(':','_')}";

            int_Count_Comparison = 0;
            //start_Time = new DateTime();
        }

        public ViewForMultiReference(DataSet dsS, DataSet dsT) : this()
        {
            try
            {
                dsSource.Tables.Add(dsS.Tables[0].Copy());
                dsTarget.Tables.Add(dsT.Tables[1].Copy());

                foreach (DataColumn dc in dsS.Tables[0].Columns)
                {
                    clb_Source.Items.Add(dc.ColumnName);
                }

                foreach (DataColumn dc in dsT.Tables[1].Columns)
                {
                    clb_Target.Items.Add(dc.ColumnName);
                }

                lbl_Source.Text = string.Format("Rows : {0} and Columns : {1}", dsS.Tables[0].Rows.Count, dsS.Tables[0].Columns.Count);

                lbl_Target.Text = string.Format("Rows : {0} and Columns : {1}", dsT.Tables[0].Rows.Count, dsT.Tables[0].Columns.Count);

                DebuggerDataSet();

            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void DebuggerDataSet()
        {
            try
            {
                if (Debugger.IsAttached)
                {
                    lb_Source_ID.Items.Add(clb_Source.Items[0].ToString());
                    lb_Source_Col.Items.Add(clb_Source.Items[2].ToString());
                    lb_Target_ID.Items.Add(clb_Target.Items[0].ToString());
                    lb_Target_Col.Items.Add(clb_Target.Items[2].ToString());

                    btn_Set_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void generateTransposeTable()
        {
            try
            {
                dt_transpose = new DataTable();
                dt_transpose.Columns.Add(dt_summarised_difference.Columns[0].ColumnName.ToString());

                foreach (DataRow dr in dt_summarised_difference.Rows)
                {
                    string str_newColName = dr[0].ToString();
                    dt_transpose.Columns.Add(str_newColName);
                }

                for (int rCount = 1; rCount <= dt_summarised_difference.Columns.Count - 1; rCount++)
                {
                    DataRow dr = dt_transpose.NewRow();

                    dr[0] = dt_summarised_difference.Columns[rCount].ColumnName.ToString();

                    for (int cCount = 0; cCount <= dt_summarised_difference.Rows.Count - 1; cCount++)
                    {
                        string str_colValue = dt_summarised_difference.Rows[cCount][rCount].ToString();
                        dr[cCount + 1] = str_colValue;
                    }
                    dt_transpose.Rows.Add(dr);
                }
                dt_transpose.Columns[0].ColumnName = "Column Names";
                dt_transpose.Columns[1].ColumnName = "Summarize Count";

            }
            catch (Exception ex)
            {
                myException_BG = ex;
            }
        }
        private void DisplayError(Label lb, Exception ex = null, string strMessage = null)
        {
            try
            {
                lb.ForeColor = Color.Red;
                if (ex != null)
                {
                    lb.Text = ex.Message.ToString();
                }
                else
                {
                    lb.Text = strMessage;
                }
            }
            catch (Exception)
            {
                throw;
                //lbl_Status.Text = except.Message.ToString();
            }
        }

        private void DisplayMessage(Label lb, string strMessage = null)
        {
            try
            {
                {
                    lb.Text = strMessage;
                    lb.ForeColor = Color.Black;
                }
            }
            catch (Exception except)
            {
                lbl_Status.Text = except.Message.ToString();
            }
        }

        private void btn_Add_Source_ID_Click(object sender, EventArgs e)
        {
            try
            {
                AddObjectInListBox(clb_Source, lb_Source_ID, "Source ID", lb_Source_Col);
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void AddObjectInListBox(CheckedListBox clb, ListBox lbToAdd, string strMessageText, ListBox listBoxToCheck)
        {
            try
            {
                boolSet = false;
                CheckedListBox.CheckedItemCollection cic = clb.CheckedItems;

                foreach (object item in cic.Cast<object>().ToArray())
                {
                    if ((!(lbToAdd.Items.Contains(item.ToString()))) & (!(listBoxToCheck.Items.Contains(item))))
                    {
                        lbToAdd.Items.Add(item.ToString());
                    }
                    else
                    {
                        DisplayMessage(lbl_Status, strMessageText + " already added");
                    }
                    clb.SetItemCheckState(clb.Items.IndexOf(item), CheckState.Unchecked);
                }

            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void AddObjectInListBoxAutoMapping(object item  , ListBox lb_s, ListBox lb_t)
        {
            try
            {
                boolSet = false;

                if (!(lb_s.Items.Contains(item.ToString()) || lb_t.Items.Contains(item.ToString())))
                {
                    lb_s.Items.Add(item.ToString());
                    lb_t.Items.Add(item.ToString());
                    lb_s.Update();
                    lb_t.Update();
                }
                else
                {
                    DisplayMessage(lbl_Status, item.ToString() + " already added");
                }
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        //private bool CheckObjectInListBox()
        //{
        //    try
        //    {
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayError(lbl_Status, ex);
        //    }
        //}

        private void btn_Add_Source_Col_Click(object sender, EventArgs e)
        {
            try
            {
                AddObjectInListBox(clb_Source, lb_Source_Col, "Source Column", lb_Source_ID);
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void btn_Add_Target_ID_Click(object sender, EventArgs e)
        {
            try
            {
                AddObjectInListBox(clb_Target, lb_Target_ID, "Target ID", lb_Target_Col);
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void btn_Add_Target_Col_Click(object sender, EventArgs e)
        {
            try
            {
                AddObjectInListBox(clb_Target, lb_Target_Col, "Target Column", lb_Target_ID);
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void btn_Set_Click(object sender, EventArgs e)
        {
            try
            {
                boolSet = false;
                progressBar1.Value = 0;
                dsSource_Com.Tables.Clear();
                dsTarget_Com.Tables.Clear();

                str_query_source_id = Str_Query(lb_Source_ID);

                columnName = str_Col_Name(lb_Source_ID, lb_Source_Col);
                dv = new DataView(dsSource.Tables[0]);
                dsSource_Com.Tables.Add(dv.ToTable(false, columnName));

                columnName = str_Col_Name(lb_Target_ID, lb_Target_Col);
                dv = new DataView(dsTarget.Tables[0]);
                dsTarget_Com.Tables.Add(dv.ToTable(false, columnName));

                dtSource_Com = dsSource_Com.Tables[0];
                dtTarget_Com = dsTarget_Com.Tables[0];

                dvSource_Com = new DataView(dtSource_Com);
                dvTarget_Com = new DataView(dtTarget_Com);

                tb_FileName.Text = $"Output_{DateTime.Now.ToString().Replace('-', '_').Replace(':', '_')}";

                if (lb_Source_ID.Items.Count == lb_Target_ID.Items.Count & lb_Source_Col.Items.Count == lb_Target_Col.Items.Count)
                {
                    DisplayMessage(lbl_Status, "All set successfully");
                    boolSet = true;
                }
                else
                {
                    DisplayMessage(lbl_Status, "There is some error in column mapping");
                    boolSet = false;
                }
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private string Str_Query(ListBox listBox)
        {
            try
            {
                str_query = string.Empty;
                foreach (var item in listBox.Items)
                {
                    str_query = str_query + item.ToString() + ",";
                }
                str_query = str_query.Substring(0, str_query.Length - 1);

            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
                throw;
            }
            return str_query;
        }

        private string[] str_Col_Name(ListBox listBoxID, ListBox listBoxCol)
        {
            try
            {
                columnName = new string[listBoxID.Items.Count + listBoxCol.Items.Count];
                int int_index = listBoxID.Items.Count;

                for (int i = 0; i < listBoxID.Items.Count; i++)
                {
                    columnName[i] = listBoxID.Items[i].ToString();
                }

                for (int i = 0; i < listBoxCol.Items.Count; i++)
                {
                    columnName[i + int_index] = listBoxCol.Items[i].ToString();
                }

            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
                throw;
            }
            return columnName;
        }

        private void btn_Compare_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (Debugger.IsAttached || Environment.UserName == "40009708")
                {
                    tb_FileName.Text = $"Output_{common_Data.RemoveSpecialCharacters(DateTime.Now.ToString(), "_")}";
                }

                if (common_Data.GetRulesStatus())
                {
                    dsRules = common_Data.GetDatasetRules();
                    boolRules = common_Data.GetRulesStatus();
                }

                if (!(boolSet))
                {
                    DisplayMessage(lbl_Status, "Some settings error");
                    return;
                }
                stopwatch.Start();

                if (!backgroundWorker1.IsBusy)
                {
                    int_Count_Comparison = 0;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void llbl_Folder_path_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                llbl_Folder_path.Text = ofdd.folderBrowser(llbl_Folder_path.Text);// folderBrowser();
                DisplayMessage(lbl_Status, "Path Set");
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }

        }

        private void btn_Remove_Click(object sender, EventArgs e)
        {
            try
            {
                boolSet = false;
                RemoveSelectedItems(lb_Source_ID);
                RemoveSelectedItems(lb_Source_Col);
                RemoveSelectedItems(lb_Target_ID);
                RemoveSelectedItems(lb_Target_Col);
                DisplayMessage(lbl_Status, "");
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void RemoveSelectedItems(ListBox lb, string str_cleartype = null)
        {
            try
            {
                ListBox.SelectedObjectCollection selectedObjectCollection = lb.SelectedItems;

                foreach (object item in selectedObjectCollection.Cast<object>().ToArray())
                {
                    lb.Items.Remove(item);
                }
                DisplayMessage(lbl_Status, "items Removed");

                if(str_cleartype.ToLower() == "all")
                {
                    foreach (object item in lb.Items.Cast<object>().ToArray())
                    {
                        lb.Items.Remove(item);
                    }
                }

            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private string generateRowFilterString(DataRow dr, ListBox.ObjectCollection lb_oc)
        {
            try
            {
                str_unique_reference_key = string.Empty;

                for (int i = 0; i < lb_oc.Count; i++)
                {
                    //str_unique_reference_key = str_unique_reference_key + lb_oc[i].ToString() + " = '" + dr[i] + "' AND ";
                    if(dr[i].ToString().Trim().Length > 0)
                    {
                        str_unique_reference_key = $"{str_unique_reference_key} [{lb_oc[i].ToString()}] = '{dr[i]}' AND ";
                        boolUniqueKey = true;
                    }
                    else
                    {
                        str_unique_reference_key = $"{str_unique_reference_key} [{lb_oc[i].ToString()}] = '{dr[i]}' AND ";
                        boolUniqueKey = false;
                    }
                    
                }

                str_unique_reference_key = str_unique_reference_key.Substring(0, str_unique_reference_key.Length - 4);
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
                throw;
            }
            return str_unique_reference_key;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                DisplayElapsedTime();
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void btn_ClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                boolSet = false;
                RemoveSelectedItems(lb_Source_ID,"all");
                RemoveSelectedItems(lb_Source_Col, "all");
                RemoveSelectedItems(lb_Target_ID, "all");
                RemoveSelectedItems(lb_Target_Col, "all");
                DisplayMessage(lbl_Status, "Cleared All");
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void btn_ID_For_SRC_TRG_Click(object sender, EventArgs e)
        {
            try
            {
                int_count_src_id = clb_Source.CheckedItems.Count;
                int_count_trg_id = clb_Target.CheckedItems.Count;

                if(int_count_src_id == int_count_trg_id && int_count_src_id != 0)
                {
                    AddObjectInListBox(clb_Source, lb_Source_ID, "Source ID", lb_Source_Col);
                    AddObjectInListBox(clb_Target, lb_Target_ID, "Target ID", lb_Target_Col);
                }
                else
                {
                    DisplayMessage(lbl_Status, "ID Selected in Source and Target doesn't match in count or zero");
                }
            }
            catch(Exception ex)
            {
                DisplayError(lbl_Status,ex);
            }
        }

        private void btn_Col_For_SRC_TRG_Click(object sender, EventArgs e)
        {
            try
            {
                int_count_src_col = clb_Source.CheckedItems.Count;
                int_count_trg_col = clb_Target.CheckedItems.Count;

                if(int_count_src_col == int_count_trg_col && int_count_src_col != 0)
                {
                    AddObjectInListBox(clb_Source, lb_Source_Col, "Source Column", lb_Source_ID);
                    AddObjectInListBox(clb_Target, lb_Target_Col, "Target Column", lb_Target_ID);
                }
                else
                {
                    DisplayMessage(lbl_Status, "ID Selected in Source and Target doesn't match in count or zero");
                }
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void btn_AutoMappingCols_Click(object sender, EventArgs e)
        {
            try
            {
                foreach(object o in clb_Source.Items)
                {
                    if(clb_Target.Items.Contains(o))
                    {
                        if(!(lb_Source_ID.Items.Contains(o) || lb_Target_ID.Items.Contains(o)))
                        {
                            AddObjectInListBoxAutoMapping(o, lb_Source_Col,lb_Target_Col);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void btn_AutoMappingID_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (object o in clb_Source.Items)
                {
                    if (clb_Target.Items.Contains(o))
                    {
                        if (!(lb_Source_Col.Items.Contains(o) || lb_Target_Col.Items.Contains(o)))
                        {
                            AddObjectInListBoxAutoMapping(o, lb_Source_ID, lb_Target_ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }
        private void cb_Select_All_SRC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                checkBoxListToggle(clb_Source, cb_Select_All_SRC.Checked);
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void checkBoxListToggle(CheckedListBox clb, bool enabledState)
        {
            for (int i = 0; i < clb.Items.Count; i++)
            {
                clb.SetItemChecked(i, enabledState);
            }
        }

        private void cb_Select_All_TRG_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                checkBoxListToggle(clb_Target, cb_Select_All_TRG.Checked);
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void btn_RuleForm_Click(object sender, EventArgs e)
        {
            try
            {
                common_Data.OpenRuleForm();
            }
            catch (Exception ex)
            {
                DisplayError(label2,ex);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region this is for flag for duplicate in multiple reference key
                string str_boolCheckedDC = "boolChecked";

                if(!(dtSource_Com.Columns.Contains(str_boolCheckedDC)))
                {
                    dtSource_Com.Columns.Add(str_boolCheckedDC, typeof(bool));
                }

                foreach(DataRow dr in dtSource_Com.Rows)
                {
                    dr[str_boolCheckedDC] = false;
                }

                if(!(dtTarget_Com.Columns.Contains(str_boolCheckedDC)))
                {
                    dtTarget_Com.Columns.Add(str_boolCheckedDC, typeof(bool));
                }

                foreach (DataRow dr in dtTarget_Com.Rows)
                {
                    dr[str_boolCheckedDC] = false;
                }

                #endregion

                string str_dc = string.Empty;
                myException_BG = new Exception();

                ListBox.ObjectCollection lbSource_oc_ID = lb_Source_ID.Items;
                ListBox.ObjectCollection lbTarget_oc_ID = lb_Target_ID.Items;

                dt_unmatched = dtSource_Com.Clone();
                dt_unmatched.TableName = "Mismatch";

                dt_ONLYSourceData = dtSource_Com.Clone();
                dt_ONLYTargetData = dtTarget_Com.Copy();

                columnIndex = lb_Target_ID.Items.Count;

                intHeadRow = -1;
                intAlternateRow = 0;
                progress = 0;
                totalProgressCount = dtSource_Com.Rows.Count;

                dgv = new DataGridView();

                #region summarised datatable

                dt_summarised_difference = dtSource_Com.Clone();

                foreach (DataColumn dc in dtSource_Com.Columns)
                {
                    dt_summarised_difference.Columns[dc.ToString()].DataType = typeof(Int32);
                }
                DataRow dr_summarised = dt_summarised_difference.NewRow();
                string str_colName;
                foreach (DataColumn dc in dtSource_Com.Columns)
                {
                    dr_summarised[dc.ToString()] = 0;
                }

                dt_summarised_difference.Rows.Add(dr_summarised);

                #endregion


                foreach (DataColumn dc in dt_unmatched.Columns)
                {
                    dgv.Columns.Add(dc.ColumnName, dc.ColumnName);
                }

                boolNewRow = true;

                for (rowIndex = 0; rowIndex < dtSource_Com.Rows.Count; rowIndex++)
                {
                    percentage = (((++progress) * 100) / totalProgressCount);
                    backgroundWorker1.ReportProgress(percentage);

                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        backgroundWorker1.ReportProgress(0);
                        return;
                    }

                    dr = dtSource_Com.Rows[rowIndex];
                    str_unique_reference_key = generateRowFilterString(dr, lbTarget_oc_ID);

                    str_unique_reference_key = $"{str_unique_reference_key} And {str_boolCheckedDC} = 'False'";

                    // new code

                    if(!(boolUniqueKey))
                    {
                        break;
                    }

                    //

                    dvTarget_Com.RowFilter = str_unique_reference_key;

                    string str_Data_Source_To_Compare;
                    string str_Data_Target_To_Compare;

                    bool boolCompareResult;

                    if (dvTarget_Com.Count > 0)
                    {
                        delete_current_row_from_sql_table();

                        for (columnIndex = lb_Target_ID.Items.Count; columnIndex < dtTarget_Com.Columns.Count; columnIndex++)
                        {
                            int_Count_Comparison++;
                            str_colName = dtSource_Com.Columns[columnIndex].ColumnName.ToString();
                            str_colName_SRC = dtSource_Com.Columns[columnIndex].ColumnName.ToString();
                            str_colName_TRG = dtTarget_Com.Columns[columnIndex].ColumnName.ToString();

                            DataRow drr_SRC = dsSource.Tables[0].Rows[rowIndex];
                            DataRow drr_TRG = dsTarget.Tables[0].Rows[rowIndex];

                            str_Data_Source_To_Compare = dtSource_Com.Rows[rowIndex][columnIndex].ToString();
                            str_Data_Target_To_Compare = dvTarget_Com[0][columnIndex].ToString();

                            boolCompareResult = false;

                            str_Data_Source_To_Compare = str_Data_Source_To_Compare.Trim();
                            str_Data_Target_To_Compare = str_Data_Target_To_Compare.Trim();

                            if (cb_IgnoreCase.Checked & (string.Equals(str_Data_Source_To_Compare, str_Data_Target_To_Compare, StringComparison.OrdinalIgnoreCase)))
                            {
                                boolCompareResult = true;
                            }
                            else if (string.Equals(str_Data_Source_To_Compare, str_Data_Target_To_Compare, StringComparison.Ordinal))
                            {
                                boolCompareResult = true;
                            }

                            #region Check rules 

                            if (cb_IncludeRule.Checked && boolRules && (!boolCompareResult))
                            {
                                if (str_Data_Source_To_Compare.Length > 0 && str_Data_Target_To_Compare.Length > 0 && dsRules.Tables[0].Rows.Count > 0 && str_colName_SRC.Length > 0 && str_colName_TRG.Length > 0 && dtSource_Com.Rows.Count > 0 && dtTarget_Com.Rows.Count > 0)
                                {
                                    boolRulesCheck = common_Data.CheckRulesInFile(str_Data_Source_To_Compare, str_Data_Target_To_Compare, str_colName_SRC, str_colName_TRG, drr_SRC,drr_TRG, dsSource.Tables[0].Columns, dsTarget.Tables[0].Columns);
                                }
                                else
                                {
                                    boolRulesCheck = true;
                                }
                            }
                            else
                            {
                                boolRulesCheck = true;
                            }

                            #endregion

                            //if (!(dtSource_Com.Rows[rowIndex][columnIndex].ToString() == dvTarget_Com[0][columnIndex].ToString()))
                            if ((!(boolCompareResult)) && (boolRulesCheck))
                            {
                                if (boolNewRow)
                                {
                                    dt_unmatched.Rows.Add(dr.ItemArray);
                                    dt_unmatched.Rows.Add(dvTarget_Com.ToTable().Rows[0].ItemArray);

                                    intHeadRow += 2;
                                    intAlternateRow += 2;

                                    boolNewRow = false;

                                    dgv.Rows.Add(dt_unmatched.Rows[0].ItemArray);
                                    dgv.Rows.Add(dt_unmatched.Rows[1].ItemArray);
                                }

                                dgv.Rows[intHeadRow - 1].Cells[columnIndex].Style.BackColor = Color.Yellow;
                                dgv.Rows[intAlternateRow - 1].Cells[columnIndex].Style.BackColor = Color.Yellow;

                                dt_summarised_difference.Rows[0][str_colName] =
                                            Convert.ToInt32((dt_summarised_difference.Rows[0][str_colName]).ToString()) + 1;
                            }
                        }
                        boolNewRow = true;
                        if (dvTarget_Com.Count > 1)
                        {
                            dvTarget_Com[0][str_boolCheckedDC] = true;
                        }
                    }
                    else
                    {
                        dt_ONLYSourceData.Rows.Add(dr.ItemArray);
                    }
                }

                backgroundWorker1.ReportProgress(100);

                dt_unmatched = common_Data.RemoveColumn(dt_unmatched, str_boolCheckedDC);
                dt_ONLYSourceData = common_Data.RemoveColumn(dt_ONLYSourceData, str_boolCheckedDC);
                dt_ONLYTargetData = common_Data.RemoveColumn(dt_ONLYTargetData, str_boolCheckedDC);
                dt_summarised_difference = common_Data.RemoveColumn(dt_summarised_difference, str_boolCheckedDC);

                str_FileName = llbl_Folder_path.Text + @"\" + tb_FileName.Text + @".xlsx";
                backgroundWorker1.ReportProgress(101);

                generateTransposeTable();

                //excelPackage = new ExcelPackage();

                ExportTOExcelCustomised(dt_unmatched, dgv, str_FileName);
                backgroundWorker1.ReportProgress(102);

                Process.Start(str_FileName);
            }
            catch (Exception ex)
            {
                myException_BG = ex;
                if (backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.CancelAsync();
                }
            }
        }

        private ExcelPackage ExportTOExcel(ExcelPackage pck,DataTable dataTable, string str_DTTableName = null)
        {
            try
            {

                if (str_DTTableName == null)
                {
                    str_DTTableName = dataTable.TableName.ToString();
                }
                //using(ExcelPackage pck = new ExcelPackage())
                //{
                    pck.Workbook.Worksheets.Add(str_DTTableName).Cells["A1"].LoadFromDataTable(dataTable, true).AutoFitColumns();

                    pck.SaveAs(new FileInfo(str_FileName));

                    excelPackage = pck;
                //}
            }
            catch(Exception ex)
            {
                myException_BG = ex;
                if (backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.CancelAsync();
                }
            }
            return excelPackage;
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                DisplayElapsedTime();
                if (e.ProgressPercentage <= 100)
                {
                    progressBar1.Value = e.ProgressPercentage;
                    DisplayMessage(lbl_Status,$"{e.ProgressPercentage.ToString()} % and Compared : {int_Count_Comparison}");
                }
                else if (e.ProgressPercentage == 101)
                {
                    DisplayMessage(lbl_Status, "Writing Excel File");
                }
                else if (e.ProgressPercentage == 102)
                {
                    DisplayMessage(lbl_Status, "Excel File Open Successfully");
                }

                if (myException_BG.InnerException != null)
                {
                    DisplayError(lbl_Status, myException_BG);
                }

            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                DisplayElapsedTime();
                stopwatch.Stop();
                stopwatch.Reset();

                //DisplayMessage(lbl_Status, "Done, Excel File Open Successfully");
                if (myException_BG.Message != null && myException_BG.Message != "Exception of type 'System.Exception' was thrown.")
                {
                    DisplayError(lbl_Status, myException_BG);
                }
            }
            catch (Exception ex)
            {
                DisplayError(lbl_Status, ex);
            }
        }

        private void delete_current_row_from_sql_table()
        {
            try
            {
                DataRow[] rowsDel = dt_ONLYTargetData.Select(str_unique_reference_key);

                foreach (DataRow dr in rowsDel)
                {
                    dr.Delete();
                }
                dt_ONLYTargetData.AcceptChanges();
            }
            catch (Exception ex)
            {
                myException_BG = ex;
            }
        }

        private void ExportTOExcelCustomised(DataTable dt1, DataGridView dgv1 = null, string fileName = null)
        {
            try
            {
                DataTable dt = dt1.Copy();
                if (!(dt.Columns.Contains("ExcelType")))
                {
                    dt.Columns.Add("ExcelType", typeof(string)).SetOrdinal(0);
                }
                bool boolToggle = true;

                foreach(DataRow dr in dt.Rows)
                {
                    if(boolToggle)
                    {
                        dr["ExcelType"] = "Source Input";
                        boolToggle = false;
                    }
                    else
                    {
                        dr["ExcelType"] = "Target Input";
                        boolToggle = true;
                    }
                }


                FileInfo file = new FileInfo(fileName);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage pck = new ExcelPackage())
                {

                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(dt.TableName.ToString());
                    ws.Cells["A1"].LoadFromDataTable(dt, true);

                    int int_columnAdjust = 1;

                    if (dgv1 != null)
                    {
                        for (rowIndex = 0; rowIndex < dgv1.Rows.Count - 1; rowIndex++)
                        {
                            for (columnIndex = 1; columnIndex < dgv1.Columns.Count; columnIndex++)
                            {
                                string testColor = dgv1.Rows[rowIndex].Cells[columnIndex].Style.BackColor.ToString();
                                string testdata1 = dgv1.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                                string testdata2 = ws.Cells[rowIndex + 2, columnIndex + 1].Value.ToString();

                                if (dgv1.Rows[rowIndex].Cells[columnIndex].Style.BackColor == Color.Yellow)
                                {
                                    ws.Cells[rowIndex + 2, columnIndex + 1 + int_columnAdjust].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[rowIndex + 2, columnIndex + 1 + int_columnAdjust].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                                    ws.Cells[rowIndex + 2, columnIndex + 1 + int_columnAdjust].Style.Fill.BackgroundColor.SetColor(dgv1.Rows[rowIndex].Cells[columnIndex].Style.BackColor);
                                }
                            }
                        }
                    }

                    ws.Cells.AutoFitColumns();

                    pck.Workbook.Worksheets.Add("Source Data").Cells["A1"].LoadFromDataTable(dt_ONLYSourceData, true).AutoFitColumns();

                    pck.Workbook.Worksheets.Add("Target Data").Cells["A1"].LoadFromDataTable(dt_ONLYTargetData, true).AutoFitColumns();

                    pck.Workbook.Worksheets.Add("Summarised data").Cells["A1"].LoadFromDataTable(dt_transpose, true).AutoFitColumns();

                    pck.SaveAs(new FileInfo(fileName));
                }
            }
            catch (Exception ex)
            {
                myException_BG = ex;
                throw;
            }
        }

        //private void ExportTOExcel(DataTable dt, string fileName = null)
        //{
        //    try
        //    {
        //        //FileInfo file = new FileInfo(fileName);
        //        //ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
        //        using (ExcelPackage pck = new ExcelPackage())
        //        {
        //            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(dt.TableName.ToString());
        //            ws.Cells["A1"].LoadFromDataTable(dt, true);
        //            ws.Cells.AutoFitColumns();
        //            pck.SaveAs(new FileInfo(fileName));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        myException_BG = ex;
        //        throw;
        //    }
        //}

        private void DisplayElapsedTime()
        {
            try
            {
                //time_to_execute = DateTime.Now.Subtract(start_Time);
                time_to_execute = stopwatch.Elapsed;

                if (time_to_execute.Hours == 0)
                {
                    lblTimer.Text = time_to_execute.Minutes.ToString() + " min(s) " + time_to_execute.Seconds.ToString() + " second(s) ";
                }
                else
                {
                    lblTimer.Text = time_to_execute.Hours.ToString() + " hour(s) " +
                        time_to_execute.Minutes.ToString() + " min(s) " + time_to_execute.Seconds.ToString() + " second(s) ";
                }
            }
            catch (Exception ex)
            {
                myException_BG = ex;
            }
        }

    }
}
