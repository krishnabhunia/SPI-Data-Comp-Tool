using OfficeOpenXml;
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
    public partial class SPI_Spec_Note_Creation_Utility : Form
    {
        private int int_SpecColCount, itemi;
        private string str_Note = string.Empty;
        public delegate void GetExcelData(DataSet ds, string fileName = null);
        private event GetExcelData getExcelDataEvent;

        private DataTable dt_SpecNote, dt_rowHeight;
        private DataSet ds_SpecNote;

        private SaveFileDialogFile sfdf;

        public SPI_Spec_Note_Creation_Utility()
        {
            InitializeComponent();

            getExcelDataEvent += new GetExcelData(SetExcelData);
            excelFileReadCustomControl1.GetExcelData = getExcelDataEvent;

            dt_SpecNote = new DataTable();
            ds_SpecNote = new DataSet();
            sfdf = new SaveFileDialogFile();
            dt_rowHeight = new DataTable();
        }

        private void SetExcelData(DataSet ds, string fileName = null)
        {
            try
            {
                if(ds.Tables.Count > 0 && ds.Tables[0]!=null)
                {
                    dataGridView1.DataSource = ds.Tables[0];
                    lbl_Status.Text = ds.Tables[0].TableName + " Loaded Successfully";
                    if(fileName != null)
                    {
                        ds_SpecNote = ds.Copy();
                        lbl_Status.Text = fileName + " Loaded Successfully";
                    }
                }
                else
                {
                    dataGridView1.DataSource = null;
                    lbl_Status.Text = "File Not Selected.";
                }
            }
            catch(Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        public SPI_Spec_Note_Creation_Utility(DataSet ds)
        {
            InitializeComponent();
        }

        private void btn_GenerateExcel_Click(object sender, EventArgs e)
        {
            try
            {
                sfdf.SaveFileDialogExcelFile("Enter File Name", "SPEC_NOTE_OUTPUT",dt_SpecNote);
                if(sfdf.BoolFileSaveStatus)
                {
                    Process.Start(sfdf.Str_saveFileNameWithPath);
                    lbl_Status.Text = "Saved and opening file : " + sfdf.Str_saveFileNameWithPath;
                }
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        private void btn_Spec_Create_Click(object sender, EventArgs e)
        {
            try
            {
                if(ds_SpecNote.Tables.Count > 0 && ds_SpecNote.Tables[0].Rows.Count > 0)
                {
                    if(ds_SpecNote.Tables[0].Columns[0].ColumnName == "CMPNT_ID")
                    {
                        dt_SpecNote = ds_SpecNote.Tables[0].Copy();
                        dt_SpecNote.Columns.Add("NOTE", typeof(string)).SetOrdinal(3);
                        dt_SpecNote.Columns.Add("LengthNote", typeof(Int32)).SetOrdinal(4);
                        int_SpecColCount = dt_SpecNote.Columns.Count;

                        foreach(DataRow dr in dt_SpecNote.Rows)
                        {
                            str_Note = string.Empty;
                            for (itemi = 4; itemi < int_SpecColCount; itemi++)
                            {
                                if(dr[itemi].ToString().Trim() != "" & dr[itemi].ToString().Trim() != "*")
                                {
                                    str_Note = str_Note + dt_SpecNote.Columns[itemi].ColumnName + ":" + dr[itemi] + "\n";
                                }
                            }
                            str_Note = str_Note.Substring(0, str_Note.Length - 1);
                            dr["NOTE"] = str_Note;
                            dr["LengthNote"] = str_Note.Length;
                        }

                        int_SpecColCount = dt_SpecNote.Columns.Count;

                        for (itemi = int_SpecColCount - 1; itemi >= 5; itemi--)
                        {
                            dt_SpecNote.Columns.RemoveAt(itemi);
                        }

                        dt_SpecNote.AcceptChanges();
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = dt_SpecNote;
                    }
                    else
                    {
                        lbl_Status.Text = "SPEC columns are not proper";
                    }
                }
                else
                {
                    lbl_Status.Text = "SPEC Data Not Found";
                }
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }
    }
}
