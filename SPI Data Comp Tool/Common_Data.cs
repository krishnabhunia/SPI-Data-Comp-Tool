using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using System.IO;
using ExcelDataReader;
using OfficeOpenXml;

namespace SPI_Data_Comp_Tool
{
    class Common_Data
    {
        private DataTable dt_transpose;
        private Exception myExceptionData;
        private string cellValue;

        public Common_Data()
        {
            dt_transpose = new DataTable();
            cellValue = string.Empty;
        }

        public static string RemoveNumber(string str)
        {
            try
            {
                str = Regex.Replace(str, @"\d", "").TrimStart().Trim('-').Trim();
            }
            catch (Exception ex)
            {
                str = ex.Message.ToString();
            }
            return str;
        }
        public DataTable GenerateTransposeTable(DataTable dt_summarised_difference)
        {
            try
            {
                dt_transpose = new DataTable();

                dt_transpose.Columns.Add("Column Names");
                dt_transpose.Columns.Add("Summarize Count");

                for (int rCount = 0; rCount <= dt_summarised_difference.Columns.Count - 1; rCount++)
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
            }
            catch (Exception ex)
            {
                myExceptionData = ex;
            }
            return dt_transpose;
        }

        

        public DataSet LoadExcelFromDataReader(string str_file_to_load_path, string dataTableName = null)
        {
            DataSet dsResult = new DataSet();
            try
            {
                FileStream stream = File.Open(str_file_to_load_path, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                dsResult = excelReader.AsDataSet();

                string str_columnName = string.Empty;

                foreach (DataColumn dc in dsResult.Tables[0].Columns)
                {
                    str_columnName = dsResult.Tables[0].Rows[0][dc.ColumnName.ToString()].ToString();
                    dc.Caption = str_columnName;
                    dc.ColumnName = str_columnName;
                    //dsResult.Tables[0].Columns[dc.ToString()].ColumnName = str_columnName;
                }
                dsResult.Tables[0].Rows.RemoveAt(0);
                if (dataTableName != null)
                {
                    dsResult.Tables[0].TableName = dataTableName;
                }
                dsResult.AcceptChanges();

                excelReader.Close();
            }
            catch (Exception ex)
            {
                myExceptionData = ex;
                throw new Exception(ex.Message.ToString(), ex);
            }
            return dsResult;
        }


    }

    public class SaveFileDialogFile
    {
        private SaveFileDialog saveFileDialog;

        public SaveFileDialogFile()
        {
            boolFileSaveStatus = false;
            str_saveFileName = string.Empty;
        }
        public bool boolFileSaveStatus { get; set; }
        public string str_saveFileName { get; set; }

        public SaveFileDialog saveFileDialogExcelFile(string str_Title, string excelFileNameSaved, DataTable dt)
        {
            try
            {
                saveFileDialog = new SaveFileDialog
                {
                    CheckFileExists = false,
                    Filter = "xlsx files (*.xlsx)|*.xlsx",
                    Title = str_Title,
                    FileName = excelFileNameSaved,
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    #region epplus code assembly

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                    using (ExcelPackage pck = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = pck.Workbook.Worksheets.Add(excelFileNameSaved);
                        worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                        worksheet.Cells.AutoFitColumns();
                        for (int i=2; i<dt.Rows.Count + 2; i++)
                        {
                            //worksheet.Row(i).Height = 120;
                            worksheet.Row(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Distributed;
                        }

                        //worksheet.Row(4).Height = 55;

                        pck.SaveAs(new System.IO.FileInfo(saveFileDialog.FileName));

                        str_saveFileName = saveFileDialog.FileName;
                        boolFileSaveStatus = true;
                    }
                    #endregion
                }
                else
                {
                    boolFileSaveStatus = false;
                    throw new Exception("File Not Saved");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return saveFileDialog;
        }
    }

    public class OpenFileDialogData
    {
        private string fileName_Only { get; set; }
        private string pathName_withoutFileName;
        private string extensionFileName;
        private string fileName { get; set; }
        private bool boolSelectedFile { get; set; }
        private Exception myExceptionData;
        private DataTable dt_transpose;


        public OpenFileDialogData()
        {
            fileName_Only = string.Empty;
            PathName_withoutFileName = string.Empty;
            extensionFileName = string.Empty;
            fileNameWithPath = string.Empty;

            myExceptionData = new Exception();
            dt_transpose = new DataTable();
            boolSelectedFile = false;
        }

        public string FileName_Only
        {
            get { return fileName_Only; }
            set { fileName_Only = value; }
        }

        public string PathName_withoutFileName
        {
            get { return pathName_withoutFileName; }
            set { pathName_withoutFileName = value; }
        }

        public string ExtensionFileName
        {
            get { return extensionFileName; }
            set { extensionFileName = value; }
        }

        public string fileNameWithPath
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public string folderBrowser(string oldFilePath = null)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog.Description = "Select path to save the output excel file";
                folderBrowserDialog.ShowNewFolderButton = true;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = folderBrowserDialog.SelectedPath;
                }
                else// if(oldFilePath != null)
                {
                    fileName = oldFilePath;
                }
            }
            catch (Exception ex)
            {
                fileName = ex.Message.ToString();
                myExceptionData = ex;
                //lblStatus.Text = ex.Message.ToString();
            }
            return fileName;
        }


    }
}
