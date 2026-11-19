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
    public partial class MergeLogsExport : WorkspacePageForm
    {
        private SaveFileDialogFile saveFileDialogFile;
        private Common_Data common_Data;
        private DataSet dsAll;

        public MergeLogsExport()
        {
            InitializeComponent();

            try
            {
                common_Data = new Common_Data();

                saveFileDialogFile = new SaveFileDialogFile();

                dsAll = new DataSet();
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
                var selected = new OpenFileDialogData().openFileDialog("Browse Text Log File", "txt,log");
                if (!selected.boolFileSelected) return;
                string path = null;
                if (!FormValidation.Check(this, () => path = FormValidation.ReadableFile(selected.strFileNameWithPath,
                    lbl_ReadTxtFile, "Merge log", ".txt", ".log"), ReportValidation)) return;
                DataSet candidate = null;
                if (!FormValidation.Check(this, () =>
                {
                    FormValidation.Require(new FileInfo(path).Length <= 64L * 1024L * 1024L, lbl_ReadTxtFile,
                        "This log is larger than the 64 MiB in-memory import limit. Split it at table boundaries first.");
                    candidate = ParseMergeLog(File.ReadAllText(path), lbl_ReadTxtFile);
                }, ReportValidation)) return;

                saveFileDialogFile = saveFileDialogFile.SaveFileDialogExcelFileOnly("Save Excel File",
                    "Merge_Log_Output_" + common_Data.AppendDateInOutputFileName(DateTime.Now));
                if (!saveFileDialogFile.BoolFileSaveStatus) { candidate.Dispose(); return; }
                if (!FormValidation.Check(this, () =>
                {
                    string output = FormValidation.OutputFile(saveFileDialogFile.Str_saveFileNameWithPath, lbl_ReadTxtFile,
                        "Merge-log output", ".xlsx");
                    FormValidation.DifferentFiles(output, path, lbl_ReadTxtFile);
                }, ReportValidation)) { candidate.Dispose(); return; }
                if (!WriteExcel.ExportToExcel(candidate, saveFileDialogFile.Str_saveFileNameWithPath))
                { candidate.Dispose(); ReportValidation("The output workbook could not be saved."); return; }
                dsAll?.Dispose();
                dsAll = candidate;
                lbl_ReadTxtFile.Text = path;
                progressBar1.Value = 100;
                Common_Data.DisplayMessage(rtb_Log, "File saved as: " + saveFileDialogFile.Str_saveFileNameWithPath);
                Process.Start(saveFileDialogFile.Str_saveFileNameWithPath);
            }
            catch (Exception ex) { Common_Data.DisplayError(rtb_Log, ex); }
        }

        private void ReportValidation(string message) { Common_Data.DisplayMessage(rtb_Log, message, true); }

        internal static DataSet ParseMergeLog(string text, Control field)
        {
            FormValidation.Require(!string.IsNullOrWhiteSpace(text), field, "The merge log is empty.");
            var options = System.Text.RegularExpressions.RegexOptions.IgnoreCase;
            TimeSpan timeout = TimeSpan.FromSeconds(2);
            var entries = System.Text.RegularExpressions.Regex.Matches(text, @"Table Name\s*:\s*([^;\r\n]+);", options, timeout);
            FormValidation.Require(entries.Count > 0, field, "No valid 'Table Name: ...;' records were found in the log.");
            var result = new DataSet();
            var summary = result.Tables.Add("Merge Log Out");
            summary.Columns.Add("Table Name");
            foreach (string name in new[] { "Total Source Rows", "Inserted Rows", " Updated Rows", "Rejected Rows" })
                summary.Columns.Add(name, typeof(int));
            summary.Columns.Add("Target Sql");
            summary.Columns.Add("Source Sql");
            for (int i = 0; i < entries.Count; i++)
            {
                string name = entries[i].Groups[1].Value.Trim();
                int end = i + 1 < entries.Count ? entries[i + 1].Index : text.Length;
                string block = text.Substring(entries[i].Index, end - entries[i].Index);
                int[] counts = new int[4];
                string[] labels = { "Total Source Rows", "Inserted Rows", "Updated Rows", "Rejected Rows" };
                for (int count = 0; count < labels.Length; count++)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(block,
                        System.Text.RegularExpressions.Regex.Escape(labels[count]) + @"\s*:\s*([^;\r\n]+)", options, timeout);
                    FormValidation.Require(match.Success && int.TryParse(match.Groups[1].Value.Trim(),
                        System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out counts[count]),
                        field, "Each table record must contain four non-negative whole-number row counts.");
                }
                var target = System.Text.RegularExpressions.Regex.Match(block, @"Target Sql\s*:\s*([^\r\n]*)", options, timeout);
                var source = System.Text.RegularExpressions.Regex.Match(block, @"Source Sql\s*:\s*([^\r\n]*)", options, timeout);
                FormValidation.Require(target.Success && source.Success, field, "A table record is missing its Source Sql or Target Sql line.");
                summary.Rows.Add(name, counts[0], counts[1], counts[2], counts[3], target.Groups[1].Value, source.Groups[1].Value);
                if (counts[3] > 0)
                {
                    // A numbered sheet avoids collisions/restrictions in external table names.
                    DataTable rejected = result.Tables.Add("Rejected " + (i + 1));
                    rejected.Columns.Add("Table Name");
                    rejected.Columns.Add("Rejection Remarks");
                    int first = block.IndexOf("First Id", StringComparison.OrdinalIgnoreCase);
                    int last = block.IndexOf("Total Source Rows", StringComparison.OrdinalIgnoreCase);
                    string details = first >= 0 && last > first ? block.Substring(first, last - first)
                        : "Rejected rows were reported but detailed First Id records are absent.";
                    foreach (string line in details.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                        rejected.Rows.Add(name, line);
                }
            }
            return result;
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
