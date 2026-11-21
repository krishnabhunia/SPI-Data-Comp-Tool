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
    public partial class SPI_Spec_Note_Creation_Utility : WorkspacePageForm
    {
        public delegate void GetExcelData(DataSet ds, string fileName = null);
        private event GetExcelData getExcelDataEvent;

        private DataTable dt_SpecNote;
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
            BuildSteps();
        }

        private StepFlow steps;   // controlled steps, see BuildSteps
        private bool exported;

        /// <summary>Controlled steps: Workbook, Create notes, Export. A step unlocks when the one before is done.</summary>
        private void BuildSteps()
        {
            steps = new StepFlow(this, step_Strip)
                .Add("Workbook", "Import the specification workbook",
                    () => ds_SpecNote != null && ds_SpecNote.Tables.Count > 0 && ds_SpecNote.Tables[0].Rows.Count > 0)
                .Add("Create notes", "Create and show the notes",
                    () => dt_SpecNote != null && dt_SpecNote.Columns.Contains("NOTE") && dt_SpecNote.Rows.Count > 0, btn_Spec_Create)
                .Add("Export", "Export the notes to Excel", () => exported, btn_GenerateExcel);
            steps.Refresh();
        }

        private void SetExcelData(DataSet ds, string fileName = null)
        {
            try
            {
                if (FormValidation.Check(this, () => ExcelFileReadCustomControl.ValidateWorkbook(ds, excelFileReadCustomControl1),
                    message => lbl_Status.Text = message))
                {
                    DataSet candidate = ds.Copy();
                    ds_SpecNote?.Dispose();
                    ds_SpecNote = candidate;
                    dt_SpecNote = new DataTable();
                    exported = false;
                    dataGridView1.DataSource = ds_SpecNote.Tables[0];
                    lbl_Status.Text = ds.Tables[0].TableName + " Loaded Successfully";
                    if(fileName != null)
                    {
                        lbl_Status.Text = fileName + " Loaded Successfully";
                    }
                }
                else
                {
                    // Keep the last valid input and result on an invalid reload.
                }
            }
            catch(Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
            steps?.Refresh();
        }

        public SPI_Spec_Note_Creation_Utility(DataSet ds) : this()
        {
            SetExcelData(ds);
        }

        private void btn_GenerateExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FormValidation.Check(this, () => FormValidation.Table(dt_SpecNote, btn_GenerateExcel,
                    "Generated notes"), message => lbl_Status.Text = message)) return;
                sfdf.SaveFileDialogExcelFile("Enter File Name", "SPEC_NOTE_OUTPUT",dt_SpecNote);
                if(sfdf.BoolFileSaveStatus)
                {
                    Process.Start(sfdf.Str_saveFileNameWithPath);
                    lbl_Status.Text = "Saved and opening file : " + sfdf.Str_saveFileNameWithPath;
                    exported = true;
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
                DataTable candidate = null;
                if (!FormValidation.Check(this, () =>
                {
                    ExcelFileReadCustomControl.ValidateWorkbook(ds_SpecNote, excelFileReadCustomControl1);
                    candidate = BuildNotes(ds_SpecNote.Tables[0], btn_Spec_Create);
                }, message => lbl_Status.Text = message)) return;
                dt_SpecNote = candidate;
                exported = false;
                dataGridView1.DataSource = dt_SpecNote;
                lbl_Status.Text = "Notes generated successfully.";
            }
            catch (Exception ex)
            {
                lbl_Status.Text = ex.Message.ToString();
            }
        }

        internal static DataTable BuildNotes(DataTable input, Control field)
        {
            FormValidation.Table(input, field, "Specification input");
            FormValidation.Require(input.Columns.Count >= 3 && input.Columns[0].ColumnName == "CMPNT_ID", field,
                "Specification input must begin with CMPNT_ID and contain the three identifier columns.");
            FormValidation.Require(!input.Columns.Contains("NOTE") && !input.Columns.Contains("LengthNote"), field,
                "Load the original input; NOTE and LengthNote are generated output columns.");
            DataTable result = input.Copy();
            result.Columns.Add("NOTE", typeof(string));
            result.Columns.Add("LengthNote", typeof(int));
            foreach (DataRow row in result.Rows)
            {
                string note = string.Join("\n", input.Columns.Cast<DataColumn>().Skip(3)
                    .Where(column => !string.IsNullOrWhiteSpace(row[column.ColumnName].ToString()) && row[column.ColumnName].ToString().Trim() != "*")
                    .Select(column => column.ColumnName + ":" + row[column.ColumnName]));
                FormValidation.Require(note.Length <= 32767, field, "A generated note exceeds Excel's 32,767-character cell limit.");
                row["NOTE"] = note;
                row["LengthNote"] = note.Length;
            }
            foreach (string name in input.Columns.Cast<DataColumn>().Skip(3).Select(column => column.ColumnName).ToArray())
                result.Columns.Remove(name);
            return result;
        }
    }
}
