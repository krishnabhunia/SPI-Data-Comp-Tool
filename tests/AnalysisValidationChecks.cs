using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Exercises actual Analysis controls and synthetic workbook imports, with no database access.</summary>
    internal static class AnalysisValidationChecks
    {
        internal static void Run(Action<string> passed)
        {
            CheckAnalysis(passed);
            CheckWorkbookReader(passed);
        }

        private static void Require(bool value, string message)
        {
            if (!value) throw new InvalidOperationException(message);
        }

        private static T Find<T>(Control owner, string name) where T : Control
        {
            return owner.Controls.Find(name, true).OfType<T>().Single();
        }

        private static DataSet Example(Type formNumberType = null)
        {
            var data = new DataSet();
            foreach (string name in new[] { "Source", "Target" })
            {
                DataTable table = data.Tables.Add(name);
                table.Columns.Add("SPEC_FORM_CNUM", formNumberType ?? typeof(string));
                table.Columns.Add("SPEC_NAME");
                table.Columns.Add("COLUMN_NAME");
                table.Columns.Add("FORMAT_NAME");
                table.Columns.Add("HEADER_TEXT");
                table.Rows.Add(1, "Synthetic", "TAG_NUMBER", "Instrument", "Tag number");
            }
            return data;
        }

        private static void CheckAnalysis(Action<string> passed)
        {
            int parsed;
            foreach (object valid in new object[] { 1.00m, 1.0d, 1.0f, int.MinValue, int.MaxValue, "001", "-1" })
                Require(SPI_Analysis_View.TryGetFormNumber(valid, out parsed), "An integral Analysis identifier was rejected.");
            foreach (object invalid in new object[] { null, DBNull.Value, 1.01m, 1.5d, double.NaN, double.PositiveInfinity,
                2147483648m, -2147483649m, ulong.MaxValue, true, '1', "1.00", "NaN" })
                Require(!SPI_Analysis_View.TryGetFormNumber(invalid, out parsed), "A nonintegral or invalid Analysis identifier was accepted.");
            using (var numeric = Example(typeof(decimal)))
            using (var owner = new Form())
            {
                numeric.Tables[0].Rows[0]["SPEC_FORM_CNUM"] = 1.00m;
                numeric.Tables[1].Rows[0]["SPEC_FORM_CNUM"] = 1.00m;
                SPI_Analysis_View.ValidateInput(numeric);
                using (var view = new SPI_Analysis_View(numeric))
                {
                    owner.Controls.Add(view);
                    owner.Show();
                    Application.DoEvents();
                    Require(view.ValidateArrangement(), "Valid decimal database identifiers could not be selected for Analysis.");
                    Find<Button>(view, "btn_Arrange_target").PerformClick();
                    Require(view.ValidateExport(), "Integral decimal identifiers could not be arranged into valid Analysis output.");
                    owner.Close();
                }
            }
            using (var host = new SPI_Analysis())
            using (var empty = new SPI_Analysis_View())
            {
                Require(!host.ValidateShowResults(), "Analysis allowed Show before current inputs were loaded.");
                Require(!empty.ValidateArrangement() && !empty.ValidateExport(), "An empty analysis enabled arrangement/export.");
            }
            using (var data = Example())
            {
                foreach (object invalid in new object[] { DBNull.Value, "", "1.5", "2147483648", "NaN" })
                {
                    data.Tables[1].Rows[0]["SPEC_FORM_CNUM"] = invalid;
                    bool rejected = false;
                    try { SPI_Analysis_View.ValidateInput(data); }
                    catch (ArgumentException) { rejected = true; }
                    Require(rejected, "An invalid Analysis form number was accepted.");
                }
                data.Tables[1].Rows[0]["SPEC_FORM_CNUM"] = 1;
                data.Tables[0].Rows[0]["COLUMN_NAME"] = " ";
                bool blankRejected = false;
                try { SPI_Analysis_View.ValidateInput(data); }
                catch (ArgumentException) { blankRejected = true; }
                Require(blankRejected, "A blank Analysis column key was accepted.");
                data.Tables[0].Rows[0]["COLUMN_NAME"] = "TAG_NUMBER";
                using (var owner = new Form())
                using (var view = new SPI_Analysis_View(data) { Dock = DockStyle.Fill })
                {
                    owner.Controls.Add(view);
                    owner.Show();
                    Application.DoEvents();
                    Require(view.ValidateArrangement(), "A valid Analysis pair was rejected.");
                    Find<Button>(view, "btn_Arrange_target").PerformClick();
                    Require(view.ValidateExport(), "A successfully arranged pair could not export.");
                    var remarks = (DataTable)Find<DataGridView>(view, "dgv_Remarks").DataSource;
                    Require(remarks.Rows.Count == 1, "The first arrangement did not produce the expected row.");
                    Require(!view.ValidateArrangement(), "A repeated arrangement would duplicate the output.");
                    Find<Button>(view, "btn_Arrange_target").PerformClick();
                    Require(remarks.Rows.Count == 1 && view.ValidateExport(), "Rejected duplicate arrangement changed valid results.");
                    Find<ComboBox>(view, "cb_Target").Text = "unknown";
                    Application.DoEvents();
                    Require(!view.ValidateArrangement(), "An unknown Analysis selection reused stale rows.");
                    owner.Close();
                }
            }
            passed("Analysis rejects incomplete inputs, invalid form numbers, blank column keys and duplicate arrangements; valid arranged output survives rejected actions.");
        }

        private static void CheckWorkbookReader(Action<string> passed)
        {
            string folder = Path.Combine(Path.GetTempPath(), "spi-workbook-validation-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, "input.xlsx");
            try
            {
                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Input");
                    sheet.Cells[1, 1].Value = "Column1";
                    sheet.Cells[1, 2].Value = " Id ";
                    sheet.Cells[1, 4].Value = "Label";
                    sheet.Cells[2, 1].Value = "001";
                    sheet.Cells[2, 2].Value = 1;
                    sheet.Cells[2, 4].Value = "नाम";
                    package.SaveAs(new FileInfo(path));
                }
                byte[] original = File.ReadAllBytes(path);
                using (DataSet loaded = ReadExcel.LoadExcelFromDataReader(path))
                {
                    Require(loaded.Tables.Count == 1 && loaded.Tables[0].Columns.Count == 3 && loaded.Tables[0].Rows.Count == 1,
                        "The workbook reader did not ignore only the unused spacer column.");
                    Require(loaded.Tables[0].Columns.Contains("COLUMN1") && loaded.Tables[0].Columns.Contains("ID"),
                        "Header normalization collided with the reader's generated column names.");
                    Require((string)loaded.Tables[0].Rows[0]["COLUMN1"] == "001" && (string)loaded.Tables[0].Rows[0]["LABEL"] == "नाम",
                        "Workbook validation changed leading zeros or Unicode data.");
                }
                Require(original.SequenceEqual(File.ReadAllBytes(path)), "Reading a workbook modified the input.");
                using (DataSet preserved = new Common_Data().LoadExcelFromDataReader(path))
                {
                    Require(preserved.Tables[0].Columns[0].ColumnName == "Column1"
                        && preserved.Tables[0].Columns[1].ColumnName == " Id ",
                        "The case-preserving reader changed existing header spelling or whitespace.");
                }
                using (var package = new ExcelPackage(new FileInfo(path)))
                {
                    package.Workbook.Worksheets[0].Cells[1, 4].Value = " id ";
                    package.Save();
                }
                ExpectInvalidWorkbook(path);
                using (var package = new ExcelPackage(new FileInfo(path)))
                {
                    package.Workbook.Worksheets[0].Cells[1, 4].Value = null;
                    package.Save();
                }
                ExpectInvalidWorkbook(path); // Nonempty data under a blank header must not disappear.
                File.WriteAllBytes(path, new byte[0]);
                ExpectInvalidWorkbook(path);
            }
            finally
            {
                if (File.Exists(path)) File.Delete(path);
                Directory.Delete(folder);
            }
            passed("Real synthetic workbook imports preserve Unicode/leading zeros, handle generated-name collisions and blank spacers, reject duplicate/missing headers and empty files, and never modify inputs.");
        }

        private static void ExpectInvalidWorkbook(string path)
        {
            bool rejected = false;
            try { using (ReadExcel.LoadExcelFromDataReader(path)) { } }
            catch (InvalidDataException) { rejected = true; }
            Require(rejected, "An invalid synthetic workbook was accepted.");
            rejected = false;
            try { using (new Common_Data().LoadExcelFromDataReader(path)) { } }
            catch (InvalidDataException) { rejected = true; }
            Require(rejected, "The case-preserving reader accepted an invalid synthetic workbook.");
        }
    }
}
