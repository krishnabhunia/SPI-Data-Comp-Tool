using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Syncfusion.XlsIO;
using Color = System.Drawing.Color;
using System.Diagnostics;
//using GemBox.Spreadsheet;
//using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    public partial class SPI_Analysis_View : Form
    {
        private string str_filterExpression;
        private DataView dv_unique_source, dv_unique_target, dv_1, dv_2;
        private DataView dv_source, dv_target;
        private DataTable dt_unique_source, dt_unique_target, dt_source, dt_target;
        private DataTable dt, dt_target_arranged, dt_source_arranged, dt_source_spec, dt_target_spec;

        private DataTable dt_target_arranged_all, dt_source_arranged_all, dt_remarks_all;

        //private int int_rows_index_source, int_row_index_target, index_Scroll;
        private bool boolFormLoad;

        private DataTable dt_remarks, dt_Merge;// = new DataTable();
        private DataSet ds_Merge;

        public SPI_Analysis_View(DataSet ds)
        {
            try
            {
                InitializeComponent();
                ds_Merge = new DataSet();
                dt_remarks = new DataTable();
                dt_remarks.Columns.Add("Remarks");
                dt_remarks.TableName = "Remarks";

                dt_Merge = new DataTable();
                dt_Merge.TableName = "Merged Tables";

                boolFormLoad = false;
                dt = new DataTable();
                //boolStatus = false;

                dv_1 = new DataView();
                dv_2 = new DataView();

                dv_unique_source = new DataView(ds.Tables[0]);
                dv_unique_target = new DataView(ds.Tables[1]);

                dt_unique_source = dv_unique_source.ToTable(true, "SPEC_FORM_CNUM");
                dt_unique_target = dv_unique_target.ToTable(true, "SPEC_FORM_CNUM");

                dgv_Source.DataSource = ds.Tables[0];
                dgv_Target.DataSource = ds.Tables[1];

                dt_source = ds.Tables[0];

                dt_source_arranged_all = new DataTable();
                dt_target_arranged_all = new DataTable();
                dt_remarks_all = new DataTable();

                dt_target = ds.Tables[1];
                dv_source = new DataView();
                dv_target = new DataView();

                if (ds.Tables[0].Columns.Count > 0)
                {
                    dgv_Source.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    cb_Source.Items.Clear();

                    cb_Source.ValueMember = "SPEC_FORM_CNUM";
                    cb_Source.DisplayMember = "SPEC_FORM_CNUM";
                    cb_Source.DataSource = dt_unique_source;
                    //cb_Source.Items.Insert(0, new ListViewItem("Select SPEC_FORM_CNUM"));
                    //cb_Source.SelectedIndex = 0;
                }
                if (ds.Tables[1].Columns.Count > 0)
                {
                    dgv_Target.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    cb_Target.Items.Clear();
                    //cb_Target.Items.Add("Select SPEC_FORM_CNUM");
                    cb_Target.ValueMember = "SPEC_FORM_CNUM";
                    cb_Target.DisplayMember = "SPEC_FORM_CNUM";
                    cb_Target.DataSource = dt_unique_target;
                    //cb_Target.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
                lbl_Status.ForeColor = Color.Red;
            }
            finally
            {
                boolFormLoad = true;
            }
        }


        private void btn_SaveInExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string excelFileNameSaved = "Output.xlsx";
                SaveFileDialog saveFileDialogExcel = new SaveFileDialog
                {
                    CheckFileExists = false,
                    Filter = "xlsx files (*.xlsx)|*.xlsx",
                    Title = "Save As Excel File",
                    FileName = excelFileNameSaved,
                };

                //saveFileDialogExcel.CheckFileExists = true;
                //saveFileDialogExcel.Filter = "xlsx files (*.xlsx)|*.xlsx";
                //saveFileDialogExcel.Title = "Save As Excel File";
                //saveFileDialogExcel.FileName = excelFileNameSaved;

                if(saveFileDialogExcel.ShowDialog() == DialogResult.OK)
                {
                    #region epplus code assembly

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = pck.Workbook.Worksheets.Add("SPEC_Output");
                        worksheet.Cells["A1"].LoadFromDataTable(dt_source_arranged_all, true);
                        worksheet.Cells["F1"].LoadFromDataTable(dt_target_arranged_all, true);
                        worksheet.Cells["K1"].LoadFromDataTable(dt_remarks_all, true);

                        worksheet.Cells.AutoFitColumns();
                        pck.SaveAs(new System.IO.FileInfo(saveFileDialogExcel.FileName));
                        lbl_Status.Text = "File Saved Successfully : " + saveFileDialogExcel.FileName;
                        Process.Start(saveFileDialogExcel.FileName);
                        //pck.SaveAs(new System.IO.FileInfo(excelFileNameSaved));
                    }

                    #endregion
                }
                else
                {
                    lbl_Status.Text = "File was not save.";
                }
                
            }
            catch (Exception ex)
            {
                lbl_Error.Text = ex.Message.ToString();
            }
        }

        private void dgv_Remarks_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                Scroll_Sync(dgv_Remarks, dgv_Source);
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void dgv_Remarks_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                RowEnterSelectionChange(dgv_Source, e.RowIndex);
                RowEnterSelectionChange(dgv_Target, e.RowIndex);
            }
            catch (Exception ex)
            {
                lbl_Error.Text = ex.Message.ToString();
            }
        }

        

        private void RowEnterSelectionChange(DataGridView dgv1, int rowIndex)
        {
            try
            {
                dgv1.ClearSelection();
                if(dgv1.Rows.Count > rowIndex && boolFormLoad)
                {
                    dgv1.Rows[rowIndex].Selected = true;
                }
            }
            catch (Exception ex)
            {
                lbl_Error.Text = ex.Message.ToString();
            }
        }
        private void dgv_Target_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                RowEnterSelectionChange(dgv_Source, e.RowIndex);
                if(dgv_Remarks.DataSource != null)
                {
                    RowEnterSelectionChange(dgv_Remarks, e.RowIndex);
                }
                //if (boolFormLoad & e.RowIndex > 0)
                //{
                //dgv_Source.ClearSelection();
                //dgv_Source.Rows[e.RowIndex].Selected = true;
                //}

            }
            catch (Exception ex)
            {
                lbl_Error.Text = ex.Message.ToString();
            }
        }

        private void dgv_Source_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                RowEnterSelectionChange(dgv_Target, e.RowIndex);
                if (dgv_Remarks.DataSource != null)
                {
                    RowEnterSelectionChange(dgv_Remarks, e.RowIndex);
                }
                //if (boolFormLoad & e.RowIndex > 0)
                //{
                //dgv_Target.ClearSelection();
                //dgv_Target.Rows[e.RowIndex].Selected = true;
                //}

            }
            catch (Exception ex)
            {
                lbl_Error.Text = ex.Message.ToString();
            }
        }


        //private DataTable SelectUniqueValues()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        private void SPI_Analysis_View_Load(object sender, EventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private DataTable LoadDataGridView(string str_expression, DataTable dt_Load, DataView dv_Load, DataGridView dgv, DataTable dt_ToBeLoaded)
        {
            try
            {
                dgv_Remarks.DataSource = null;
                str_filterExpression = "SPEC_FORM_CNUM = '" + str_expression + "'";
                dv_Load = new DataView(dt_Load);
                dv_Load.RowFilter = str_filterExpression;
                dt = dv_Load.ToTable();
                if (dt.Rows.Count > 0)
                {
                    dgv.DataSource = dt.Copy();
                    dt_ToBeLoaded = dt;
                    lbl_Status.Text = str_expression + " Loaded successfully with row(s) = " + dt.Rows.Count;
                    //boolStatus = true;
                }
                else
                {
                    lbl_Status.Text = str_expression + " is not available";
                    //boolStatus = false;
                }



                //str_filterExpression = "SPEC_FORM_CNUM = '" + cb_Source.Text + "'";
                //dv_source = new DataView(dt_source);
                //dv_source.RowFilter = str_filterExpression;
                //dataGridView1.DataSource = dv_source.ToTable();
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
            return dt_ToBeLoaded;
        }

        private void cb_Source_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                dt_source_spec = LoadDataGridView(cb_Source.Text, dt_source, dv_source, dgv_Source, dt_source_spec);
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void cb_Source_TextChanged(object sender, EventArgs e)
        {
            dt_source_spec = LoadDataGridView(cb_Source.Text, dt_source, dv_source, dgv_Source, dt_source_spec);
        }

        private void cb_Target_TextChanged(object sender, EventArgs e)
        {
            try
            {
                dt_target_spec = LoadDataGridView(cb_Target.Text, dt_target, dv_target, dgv_Target, dt_target_spec);

                //LoadTargetArranged(cb_Target.Text, dt_target_spec, dv_target, dgv_Target);

                //dgv_Target.DataSource = dt_target_spec;

            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }
        private void cb_Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                dt_target_spec = LoadDataGridView(cb_Target.Text, dt_target, dv_target, dgv_Target, dt_target_spec);

                //LoadTargetArranged(cb_Target.Text, dt_target_spec, dv_target, dgv_Target);

            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void LoadTargetAndSourceArranged(string str_expression, DataTable dt_11, DataTable dt_22)
        {
            try
            {
                DataTable dt_1 = new DataTable();
                dt_1 = dt_11.Copy();

                DataTable dt_2 = new DataTable();
                dt_2 = dt_22.Copy();

                dt_remarks.Clear();

                DataView dv_Load = new DataView(dt_2);
                dt_source_arranged = dt_1.Clone();
                dt_target_arranged = dt_2.Clone();

                dt_source_arranged.Columns["SPEC_FORM_CNUM"].DataType = typeof(Int32);
                dt_target_arranged.Columns["SPEC_FORM_CNUM"].DataType = typeof(Int32);

                int count = 0;
                int int_source_row_count = dt_1.Rows.Count;

                lbl_Error.Text = string.Empty;

                if(dt_source_arranged_all.Rows.Count == 0)
                {
                    dt_source_arranged_all = dt_source_arranged.Clone();
                }

                if (dt_target_arranged_all.Rows.Count == 0)
                {
                    dt_target_arranged_all = dt_target_arranged.Clone();
                }

                if (dt_remarks_all.Rows.Count == 0)
                {
                    dt_remarks_all = dt_remarks.Clone();
                }

                #region column and header matching

                string COLUMN_NAME_1 = string.Empty;
                string COLUMN_NAME_2 = string.Empty;
                string HEADER_TEXT_1 = string.Empty;
                string HEADER_TEXT_2 = string.Empty;


                foreach (DataRow r1 in dt_1.Rows.Cast<DataRow>().ToArray())
                {
                    foreach (DataRow r2 in dt_2.Rows.Cast<DataRow>().ToArray())
                    {
                        COLUMN_NAME_1 = r1["COLUMN_NAME"].ToString().Trim();
                        COLUMN_NAME_2 = r2["COLUMN_NAME"].ToString().Trim();

                        HEADER_TEXT_1 = r1["HEADER_TEXT"].ToString().Trim();
                        HEADER_TEXT_2 = r2["HEADER_TEXT"].ToString().Trim();

                        //if ((r1["COLUMN_NAME"].ToString().Trim() == r2["COLUMN_NAME"].ToString().Trim())
                        //    && (r1["HEADER_TEXT"].ToString().Trim() == r2["HEADER_TEXT"].ToString().Trim()))
                        if (((COLUMN_NAME_1 == COLUMN_NAME_2) & (HEADER_TEXT_1 == HEADER_TEXT_2)))
                        {
                            dt_source_arranged.ImportRow(r1);
                            dt_target_arranged.ImportRow(r2);
                            dt_remarks.Rows.Add("Column and Header Match");
                            r1.Delete();
                            r2.Delete();
                            break;
                        }

                        if (((COLUMN_NAME_1 == "area_id") & (COLUMN_NAME_2 == "area_child") & 
                            (HEADER_TEXT_1 == "area_id") & (HEADER_TEXT_2 == "01 - Area child"))
                            |
                            ((COLUMN_NAME_1 == "plant_id") & (COLUMN_NAME_2 == "area_parent") &
                            (HEADER_TEXT_1 == "plant_id") & (HEADER_TEXT_2 == "01 - Area Parent"))
                            )
                        {
                            dt_source_arranged.ImportRow(r1);
                            dt_target_arranged.ImportRow(r2);
                            dt_remarks.Rows.Add("Column and Header Match");
                            r1.Delete();
                            r2.Delete();
                            break;
                        }

                    }
                }

                #endregion

                #region column and partial header matching header matching

                foreach (DataRow r1 in dt_1.Rows.Cast<DataRow>().ToArray())
                {
                    foreach (DataRow r2 in dt_2.Rows.Cast<DataRow>().ToArray())
                    {
                        if ((r1["COLUMN_NAME"].ToString().Trim() == r2["COLUMN_NAME"].ToString().Trim())
                            && (Common_Data.RemoveNumber(r1["HEADER_TEXT"].ToString()).Trim() == 
                            Common_Data.RemoveNumber(r2["HEADER_TEXT"].ToString()).Trim()))
                        {
                            dt_source_arranged.ImportRow(r1);
                            dt_target_arranged.ImportRow(r2);
                            dt_remarks.Rows.Add("Technically Match (Partial Match)");
                            r1.Delete();
                            r2.Delete();
                            break;
                        }
                    }
                }

                #endregion

                #region only column match

                foreach (DataRow r1 in dt_1.Rows.Cast<DataRow>().ToArray())
                {
                    foreach (DataRow r2 in dt_2.Rows.Cast<DataRow>().ToArray())
                    {
                        if (r1["COLUMN_NAME"].ToString().Trim() == r2["COLUMN_NAME"].ToString().Trim())
                        {
                            dt_source_arranged.ImportRow(r1);
                            dt_target_arranged.ImportRow(r2);
                            dt_remarks.Rows.Add("Column Match");
                            r1.Delete();
                            r2.Delete();
                            break;
                        }
                    }
                }

                #endregion

                #region No match found in target

                foreach (DataRow r1 in dt_1.Rows.Cast<DataRow>().ToArray())
                {
                    dt_source_arranged.ImportRow(r1);
                    dt_target_arranged.Rows.Add(dt_target_arranged.NewRow());
                    dt_remarks.Rows.Add("No Match Found in target");
                    r1.Delete();
                }

                #endregion

                #region No match found in source

                foreach (DataRow r2 in dt_2.Rows.Cast<DataRow>().ToArray())
                {
                    dt_source_arranged.Rows.Add(dt_source_arranged.NewRow());
                    dt_target_arranged.ImportRow(r2);
                    dt_remarks.Rows.Add("No Match Found in source");
                    r2.Delete();
                }

                #endregion

                dt_source_arranged_all.Merge(dt_source_arranged);
                dt_target_arranged_all.Merge(dt_target_arranged);
                dt_remarks_all.Merge(dt_remarks);

                lbl_Status.Text = "All arranged successfully";

                if (Debugger.IsAttached)
                {
                    lbl_Status.Text = "All arranged " + count + "   " + dt_1.Rows.Count + "   " + dt_2.Rows.Count + "   |   " +
                    dt_Merge.Rows.Count;
                }
                
            }
            catch (Exception ex)
            {
                lbl_Error.Text = ex.Message.ToString();
            }
        }

        private void Process_Arrangement_Task()
        {
            try
            {

            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void Scroll_Sync(DataGridView dgv_first, DataGridView dgv_second)
        {
            try
            {
                int index_Scroll = dgv_first.FirstDisplayedScrollingRowIndex;
                if (dgv_second.Rows.Count > 0 & dgv_first.Rows.Count > 0 & dgv_second.Rows.Count > index_Scroll)
                {
                    dgv_second.FirstDisplayedScrollingRowIndex = index_Scroll;
                }
                if(dgv_second != dgv_Remarks && dgv_first != dgv_Remarks)
                {
                    dgv_second.HorizontalScrollingOffset = dgv_first.HorizontalScrollingOffset;
                }
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void dgv_Source_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                Scroll_Sync(dgv_Source, dgv_Target);
                if (dgv_Remarks.DataSource != null)
                {
                    Scroll_Sync(dgv_Source, dgv_Remarks);
                }
                //int_rows_index_source = dgv_Source.FirstDisplayedScrollingRowIndex;
                //if (dgv_Target.Rows.Count > 0 & dgv_Source.Rows.Count > 0 & dgv_Target.Rows.Count > int_rows_index_source)
                //{
                //    dgv_Target.FirstDisplayedScrollingRowIndex = dgv_Source.FirstDisplayedScrollingRowIndex;
                //}
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void dgv_Target_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                Scroll_Sync(dgv_Target, dgv_Source);
                if (dgv_Remarks.DataSource != null)
                {
                    Scroll_Sync(dgv_Target, dgv_Remarks);
                }
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        public SPI_Analysis_View()
        {
            InitializeComponent();

            //dv_unique = new DataView();
        }

        private void btn_Arrange_target_Click(object sender, EventArgs e)
        {
            try
            {
                LoadTargetAndSourceArranged(cb_Target.Text, dt_source_spec, dt_target_spec);
                dgv_Source.DataSource = dt_source_arranged;
                dgv_Target.DataSource = dt_target_arranged;
                dgv_Remarks.DataSource = dt_remarks;

                //dgv_Source.DataSource = dt_source_arranged_all;
                //dgv_Target.DataSource = dt_target_arranged_all;
                //dgv_Remarks.DataSource = dt_remarks_all;

                lbl_Status.Text = "Arranged Successfully";
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }
    }
}
