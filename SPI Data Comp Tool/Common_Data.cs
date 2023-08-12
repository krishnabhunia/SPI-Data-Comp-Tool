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
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Drawing;

namespace SPI_Data_Comp_Tool
{
    class Common_Data
    {
        Form_Rules form_Rules;

        private DataTable dt_transpose;
        private Exception myExceptionData;
        private readonly string cellValue;
        internal DataSet dsRules;
        private bool boolRules;
        private string strConnection;
        private string str_RuleFileName;

        private List<DBC> items;
        private static bool boolColorAlternate;

        private SqlDataAdapter sqlDA;
        private OracleDataAdapter oracleDA;
        private static int int_Static_Percentage;
        private DataSet ds;



        public Common_Data()
        {
            try
            {
                dt_transpose = new DataTable();
                cellValue = string.Empty;

                dsRules = new DataSet();
                boolRules = false;

                strConnection = string.Empty;

                boolColorAlternate = true;
                sqlDA = new SqlDataAdapter();
                oracleDA = new OracleDataAdapter();
                ds = new DataSet();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable getCommonColumns(DataTable dt_returnDataTable, DataTable dt_check, ListBox lb)
        {
            try
            {
                lb.Items.Clear();
                DataColumnCollection dcc = dt_check.Columns;

                foreach (DataColumn dc in dt_returnDataTable.Columns)
                {
                    if (!(dcc.Contains(dc.ColumnName.ToString())))
                    {
                        dt_returnDataTable.Columns.Remove(dc.ColumnName.ToString());
                        lb.Items.Add(dc.ColumnName.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt_returnDataTable;
        }
        internal ExcelWorksheet ChangeBlackCellToEmpty(ExcelWorksheet worksheet)
        {
            try
            {
                foreach (var cell in worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column])
                {
                    //object abc = cell.Value;
                    if (cell.Value.ToString() == "")
                    {
                        cell.Value = string.Empty;
                    }
                    //percentage = (((++progress) * 100) / totalProgress);
                    //backgroundWorker1.ReportProgress(percentage);

                    //foreach (var i in queryIenumerable)
                    //{
                    //    worksheet.Cells[i.int_Row, cell.Start.Column].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    //    worksheet.Cells[i.int_Row, cell.Start.Column].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    //    worksheet.Cells[i.int_Row, cell.Start.Column].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    //}
                }
                //foreach(DataColumn dc in _dt.Columns)
                //{
                //    foreach (DataRow dr in _dt.Rows)
                //    {
                //        if(dr[dc].GetType().FullName == "System.DBNull")
                //        {
                //            dr[dc] = string.Empty;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return worksheet;
        }
        internal static bool ContainWithIgnoreCase(string a, string b)
        {
            try
            {
                if ((a.ToLower().Contains(b.ToLower())) || (a.ToUpper().Contains(b.ToUpper())))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal static void ClearDisplay(RichTextBox rtb_Log)
        {
            try
            {
                rtb_Log.ResetText();
                DisplayMessage(rtb_Log, $"Logs Cleared");
            }
            catch (Exception ex)
            {
                DisplayError(rtb_Log, ex);
            }
        }
        internal static void RearrangeColumns(DataTable dtSRC, DataTable dtTRG)
        {
            try
            {
                foreach (DataColumn dc in dtSRC.Columns)
                {
                    dtTRG.Columns[dc.ColumnName].SetOrdinal(dc.Ordinal);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal static void DisplayError(RichTextBox rtb_Log, Exception ex)
        {
            try
            {
                DisplayMessage(rtb_Log, ex.Message.ToString(), true);
                if (GlobalDebug.boolIsGlobalDebug || Debugger.IsAttached)
                {
                    DisplayMessage(rtb_Log, ex.ToString(), true);
                }
            }
            catch (Exception ex1)
            {
                DisplayMessage(rtb_Log, ex1.Message.ToString(), true);
                if (GlobalDebug.boolIsGlobalDebug || Debugger.IsAttached)
                {
                    DisplayMessage(rtb_Log, ex1.ToString(), true);
                }
            }
        }

        internal static void UpdateProgressBar(ProgressBar pg, int int_c, int int_tc)
        {
            try
            {
                int_Static_Percentage = (int_c * 100) / int_tc;
                pg.Value = int_Static_Percentage;
                pg.Update();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal static void DisplayDebugMsg(RichTextBox rtb_Log, string strMsg)
        {
            try
            {
                if (GlobalDebug.boolIsGlobalDebug || Debugger.IsAttached)
                {
                    DisplayMessage(rtb_Log, $"Debug Msg : {strMsg}", false, -1, Color.BlueViolet);
                }
            }
            catch (Exception ex)
            {
                DisplayError(rtb_Log, ex);
            }
        }

        internal static void DisplayMessage(RichTextBox rtb_Log, string strMsg, bool boolError = false, int overwriteText = -1, Color? color = null)
        {
            try
            {
                rtb_Log.SelectionStart = rtb_Log.TextLength;

                if (boolColorAlternate)
                {
                    rtb_Log.SelectionColor = color ?? Color.Black;
                }
                else
                {
                    rtb_Log.SelectionColor = color ?? Color.DarkBlue;
                }

                boolColorAlternate = !boolColorAlternate;

                if (boolError)
                {
                    rtb_Log.SelectionColor = Color.Red;
                }

                if (overwriteText > 0)
                {
                    var charcf0 = rtb_Log.Rtf.LastIndexOf("\\cf0");
                    if (charcf0 != -1)
                    {
                        var last = rtb_Log.Rtf.LastIndexOf("Processing", charcf0, 20);
                        rtb_Log.SelectionColor = Color.Blue;
                        if (last > 0)
                        {
                            rtb_Log.Rtf = rtb_Log.Rtf.Remove(last, charcf0 - last);
                            charcf0 = rtb_Log.Rtf.LastIndexOf("\\cf0");
                            rtb_Log.Rtf = rtb_Log.Rtf.Insert(charcf0, strMsg);
                        }
                        else
                        {
                            rtb_Log.AppendText($"\n{string.Format("{0:T}", DateTime.Now)} : {strMsg}");
                            rtb_Log.ScrollToCaret();
                        }
                    }
                }

                if (rtb_Log.TextLength > 0 && overwriteText == -1)
                {
                    rtb_Log.AppendText($"\n{string.Format("{0:T}", DateTime.Now)} : {strMsg}");
                    rtb_Log.ScrollToCaret();
                }
                else if (overwriteText == -1)
                {
                    rtb_Log.Text = $"{string.Format("{0:T}", DateTime.Now)} : {strMsg}";
                }

                rtb_Log.Update();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            Logs.EnterLogsWithTime(strMsg);
        }

        private bool CheckDataTableExists(DataSet ds, string strTBName, int int_Count)
        {
            try
            {
                if (strTBName != null & strTBName.Trim() != "" & ds.Tables.Contains(strTBName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataSet InsertDataTable(DataSet ds, DataTable dataTable, string strInitialName)
        {
            try
            {
                int int_Count = 0;

                if (CheckDataTableExists(ds, strInitialName, int_Count))
                {
                    string strNewName = $"{strInitialName}_{int_Count}";
                    while (CheckDataTableExists(ds, strNewName, int_Count))
                    {
                        ++int_Count;
                        strNewName = $"{strInitialName}_{int_Count}";
                    }
                    dataTable.TableName = strNewName;
                    ds.Tables.Add(dataTable.Copy());
                }
                else
                {
                    ds.Tables.Add(dataTable.Copy());
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        internal DataTable CreateNewMergeColumn(DataTable dataTable, string strCols, string strColName, bool boolRemoveSpecialCharacters = false, bool boolIgnoreCase = false)
        {
            try
            {
                strCols = RemoveSpecialCharactersInColumn(strCols, ",");
                dataTable = AddColumns(dataTable, $"{strColName}", typeof(string));
                dataTable.Columns[strColName].SetOrdinal(0);

                if (boolRemoveSpecialCharacters)
                {
                    if (boolIgnoreCase)
                    {
                        foreach (DataRow drr in dataTable.Rows)
                        {
                            drr[strColName] = RemoveSpecialCharacters(string.Join("_", strCols.Split(',').AsEnumerable().Select(x => drr[x].ToString().ToLower())), string.Empty);
                            //string tess = RemoveSpecialCharacters(@"asd234_!~!@#$%^&*()_+_)(*&^{}|: <>?\][;'/.,']");
                        }
                    }
                    else
                    {
                        foreach (DataRow drr in dataTable.Rows)
                        {
                            drr[strColName] = RemoveSpecialCharacters(string.Join("_", strCols.Split(',').AsEnumerable().Select(x => drr[x].ToString())), string.Empty);
                            //string tess = RemoveSpecialCharacters(@"asd234_!~!@#$%^&*()_+_)(*&^{}|: <>?\][;'/.,']");
                        }
                    }
                }
                else
                {
                    if (boolIgnoreCase)
                    {
                        foreach (DataRow drr in dataTable.Rows)
                        {
                            drr[strColName] = string.Join("_", strCols.Split(',').AsEnumerable().Select(x => drr[x].ToString().ToLower()));
                        }
                    }
                    else
                    {
                        foreach (DataRow drr in dataTable.Rows)
                        {
                            drr[strColName] = string.Join("_", strCols.Split(',').AsEnumerable().Select(x => drr[x].ToString()));
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable CreateNewMergeColumnDouble(DataTable dataTable, string strCols, string strColName, bool boolRemoveSpecialCharacters = false)
        {
            try
            {
                strCols = RemoveSpecialCharactersInColumn(strCols, ",");
                dataTable = AddColumns(dataTable, $"{strColName}", typeof(double));
                dataTable.Columns[strColName].SetOrdinal(0);

                if (boolRemoveSpecialCharacters)
                {
                    foreach (DataRow drr in dataTable.Rows)
                    {
                        drr[strColName] = RemoveSpecialCharacters(string.Join("_", strCols.Split(',').AsEnumerable().Select(x => drr[x].ToString())), string.Empty);
                    }
                }
                else
                {
                    foreach (DataRow drr in dataTable.Rows)
                    {
                        drr[strColName] = string.Join("_", strCols.Split(',').AsEnumerable().Select(x => drr[x].ToString()));
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal static string GetDataFromRow(DataRow dr, string[] strColumns)
        {
            string strData = string.Empty;
            try
            {
                foreach (string c in strColumns)
                {
                    strData = string.Concat(strData, dr[c].ToString());
                }
                return strData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal DataTable FillDataTable(DataTable dt, string strDBType, string strConnectionStr, string strCMDText, string strTBName = null)
        {
            try
            {
                dt.Clear();

                if (strDBType.ToLower() == "oracle")
                {
                    using (oracleDA = new OracleDataAdapter(strCMDText, strConnectionStr))
                    {
                        oracleDA.SelectCommand.CommandTimeout = 0;
                        oracleDA.Fill(dt);
                    }
                }

                if (strDBType.ToLower() == "sql")
                {
                    using (sqlDA = new SqlDataAdapter(strCMDText, strConnectionStr))
                    {
                        sqlDA.SelectCommand.CommandTimeout = 0;
                        sqlDA.Fill(dt);
                    }
                }

                if (strTBName != null)
                {
                    dt.TableName = strTBName;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }


        internal DataTable FillDataTable(DataTable dt, DBConnectionStatus dBConnectionStatus)
        {
            try
            {
                dt.Reset();
                if (dBConnectionStatus.strDBType.ToLower() == "oracle")
                {
                    using (oracleDA = new OracleDataAdapter(dBConnectionStatus.strCMDText, dBConnectionStatus.strConnectionString))
                    {
                        oracleDA.SelectCommand.CommandTimeout = 0;
                        oracleDA.Fill(dt);
                    }
                }

                if (dBConnectionStatus.strDBType.ToLower() == "sql")
                {
                    using (sqlDA = new SqlDataAdapter(dBConnectionStatus.strCMDText, dBConnectionStatus.strConnectionString))
                    {
                        sqlDA.SelectCommand.CommandTimeout = 0;
                        sqlDA.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        internal DataSet FillDataTable(DataSet ds, DBConnectionStatus dBConnectionStatus)
        {
            try
            {
                ds.Reset();
                if (dBConnectionStatus.strDBType.ToLower() == "oracle")
                {
                    using (oracleDA = new OracleDataAdapter(dBConnectionStatus.strCMDText, dBConnectionStatus.strConnectionString))
                    {
                        oracleDA.SelectCommand.CommandTimeout = 0;
                        oracleDA.Fill(ds);
                    }
                }

                if (dBConnectionStatus.strDBType.ToLower() == "sql")
                {
                    using (sqlDA = new SqlDataAdapter(dBConnectionStatus.strCMDText, dBConnectionStatus.strConnectionString))
                    {
                        sqlDA.SelectCommand.CommandTimeout = 0;
                        sqlDA.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        private OracleConnection oracleConnection;
        private OracleCommand oracleCommand;

        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private int int_ExecutedRows;
        internal int ExecuteNonQuery(DBConnectionStatus dBConnectionStatus)
        {
            try
            {
                int_ExecutedRows = 0;
                if (dBConnectionStatus.strDBType.ToLower() == "oracle")
                {
                    using (oracleConnection = new OracleConnection(dBConnectionStatus.strConnectionString))
                    {
                        using (oracleCommand = new OracleCommand(dBConnectionStatus.strCMDText, oracleConnection))
                        {
                            oracleConnection.Open();
                            int_ExecutedRows = oracleCommand.ExecuteNonQuery();
                            oracleConnection.Close();
                        }
                    }
                }

                if (dBConnectionStatus.strDBType.ToLower() == "sql")
                {
                    using (sqlConnection = new SqlConnection(dBConnectionStatus.strConnectionString))
                    {
                        using (sqlCommand = new SqlCommand(dBConnectionStatus.strCMDText, sqlConnection))
                        {
                            sqlConnection.Open();
                            return sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return int_ExecutedRows;
        }

        internal static FolderBrowserDialog browserDialogInitialisation(string strDesc, string strSelectPath)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
                {
                    Description = strDesc,
                    RootFolder = Environment.SpecialFolder.Desktop,
                    SelectedPath = strSelectPath,
                    ShowNewFolderButton = true
                };
                return folderBrowserDialog;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool StringNotNullOrEmpty(string str, string strContains = null, bool boolStartsWith = false, bool boolEndsWith = false)
        {
            try
            {
                if (str != null && str != "")
                {
                    if (strContains != null && strContains != "")
                    {
                        if (boolStartsWith && (str.StartsWith(strContains)))
                        {
                            return true;
                        }
                        else if (boolEndsWith && (str.EndsWith(strContains)))
                        {
                            return true;
                        }
                        else if ((!boolStartsWith) && (!boolEndsWith) && str.Contains(strContains))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetComboBoxValue(ComboBox comboBox)
        {
            try
            {
                if (comboBox.SelectedValue != null && comboBox.SelectedValue.ToString() != "")
                {
                    return comboBox.SelectedValue.ToString().Trim();
                }
                else if (comboBox.Text != null && comboBox.Text != "")
                {
                    return comboBox.Text.Trim();
                }
                else if (comboBox.SelectedText != null && comboBox.SelectedText != "")
                {
                    return comboBox.SelectedText.Trim();
                }
                else if (comboBox.SelectedItem != null && comboBox.SelectedItem.ToString() != "")
                {
                    return comboBox.SelectedItem.ToString().Trim();
                }

                throw new Exception("No table able to detect from ComboBox");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ComboBox ReadUserProfile(ComboBox cb_UserProfileList)
        {
            try
            {
                //if (Properties.Settings.Default.ProfilePathUser != null && Properties.Settings.Default.ProfilePathUser != "" && Properties.Settings.Default.ProfilePathUser.Contains("json"))
                if (Common_Data.StringNotNullOrEmpty(Properties.Settings.Default.ProfilePathUser, ".json", false, true))
                {
                    using (StreamReader r = new StreamReader(Properties.Settings.Default.ProfilePathUser))
                    {
                        string json = r.ReadToEnd();
                        items = JsonConvert.DeserializeObject<List<DBC>>(json);
                        Profile.ProfileList = items;

                        if (Profile.ProfileList != null)
                        {
                            cb_UserProfileList.DataSource = null;
                            cb_UserProfileList.Items.Clear();
                            foreach (var i in Profile.ProfileList)
                            {
                                if (i.profileName != null)
                                {
                                    cb_UserProfileList.Items.Add(i.profileName.ToString());
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cb_UserProfileList;
        }
        public static ComboBox GetUserProfile(ComboBox cb_UserProfileList)
        {
            try
            {
                if (Profile.ProfileList != null)
                {
                    cb_UserProfileList.DataSource = null;
                    cb_UserProfileList.Items.Clear();
                    foreach (var i in Profile.ProfileList)
                    {
                        if (i.profileName != null)
                        {
                            cb_UserProfileList.Items.Add(i.profileName.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cb_UserProfileList;
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

        public static DataTable ChangeColumnToDefaultDataType(DataTable dt)
        {
            DataTable dt1 = new DataTable();
            try
            {
                dt1 = dt.Clone();

                foreach (DataColumn dc in dt1.Columns)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (DateTime.TryParse(dt.Rows[0][dc.ToString()].ToString(), out DateTime dati) && dt.Rows[0][dc.ToString()].ToString().Length > 6)
                        {
                            dt1.Columns[dc.ToString()].DataType = typeof(DateTime);
                        }
                    }
                }
                dt1.AcceptChanges();

                foreach (DataRow dr in dt.Rows)
                {
                    dt1.Rows.Add(dr.ItemArray);
                }
                dt1.AcceptChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt1;
        }

        public static DataSet ChangeColumnToDefaultDataType(DataSet ds)
        {
            DataSet ds1 = new DataSet();
            try
            {
                ds1 = ds.Clone();

                foreach (DataTable dt1 in ds1.Tables)
                {
                    foreach (DataColumn dc1 in dt1.Columns)
                    {
                        if (ds.Tables[dt1.TableName].Rows.Count > 0)
                        {
                            if (DateTime.TryParse(ds.Tables[dt1.TableName].Rows[0][dc1.ToString()].ToString(), out DateTime dati))
                            {
                                dt1.Columns[dc1.ToString()].DataType = typeof(DateTime);
                            }
                        }
                    }
                }
                ds1.AcceptChanges();

                foreach (DataTable dt in ds.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ds1.Tables[dt.TableName].Rows.Add(dr.ItemArray);
                    }
                    ds1.Tables[dt.TableName].AcceptChanges();
                }
                ds1.AcceptChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds1;
        }

        public string GetConcatColName(string strColNames)
        {
            string strAddColName = string.Empty;
            try
            {
                List<string> strCol = strColNames.Split(',').Select(x => x.Trim()).ToList();
                strAddColName = string.Join("_", strCol);

                strAddColName += "___PK___R";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strAddColName;
        }
        public DataTable ConcatColumnsAndData(DataTable _dt, string strColNames)
        {
            try
            {
                List<string> strCol = strColNames.Split(',').Select(x => x.Trim()).ToList();
                string strAddColName = string.Join("_", strCol);

                strAddColName += "___PK___R";

                if (!(_dt.Columns.Contains(strAddColName)))
                {
                    _dt = AddColumns(_dt, strAddColName, typeof(string));
                }
                _dt.Columns[strAddColName].SetOrdinal(0);

                string strData = string.Empty;

                foreach (DataRow dr in _dt.Rows)
                {
                    foreach (string s in strCol)
                    {
                        strData += dr[s].ToString().Trim();
                    }
                    dr[strAddColName] = strData.Trim();
                    strData = string.Empty;
                }
            }
            catch (Exception ex)
            {
                myExceptionData = ex;
            }
            return _dt;
        }
        public DataTable ChangeColumnToString(DataTable dt)
        {
            try
            {
                DataTable dt1 = dt.Copy();

                dt.Clear();
                dt.Columns.Clear();
                dt.Rows.Clear();

                dt = dt1.Clone();

                foreach (DataColumn dc in dt.Columns)
                {
                    dt.Columns[dc.ToString()].DataType = typeof(System.String);
                }
                dt.AcceptChanges();

                foreach (DataRow dr in dt1.Rows)
                {
                    dt.Rows.Add(dr.ItemArray);
                }
                dt.AcceptChanges();
            }
            catch (Exception ex)
            {
                myExceptionData = ex;
            }
            return dt;
        }

        public DataSet ChangeColumnToString(DataSet ds)
        {
            try
            {
                DataSet ds1 = ds.Copy();

                ds.Clear();
                ds.Tables.Clear();

                ds = ds1.Clone();

                foreach (DataTable dt in ds.Tables)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dt.Columns[dc.ToString()].DataType = typeof(System.String);
                    }
                    dt.AcceptChanges();
                }
                ds.AcceptChanges();

                foreach (DataTable dt in ds1.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ds.Tables[dt.TableName].Rows.Add(dr.ItemArray);
                    }
                    dt.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                myExceptionData = ex;
            }
            return ds;
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

                foreach (DataTable dt in dsResult.Tables)
                {
                    foreach (DataColumn dc in dt.Columns.Cast<DataColumn>().ToArray())
                    {
                        str_columnName = dt.Rows[0][dc.ColumnName.ToString()].ToString();
                        if (str_columnName != null && str_columnName != "")
                        {
                            dc.Caption = str_columnName;
                            dc.ColumnName = str_columnName;
                        }
                        else
                        {
                            dt.Columns.Remove(dc);
                        }
                    }
                    dt.Rows.RemoveAt(0);
                    if (dataTableName != null)
                    {
                        dt.TableName = dataTableName;
                        dataTableName = null;
                    }
                }
                dsResult.AcceptChanges();
                excelReader.Close();
            }
            catch (Exception ex)
            {
                myExceptionData = ex;
                throw ex;
            }
            return dsResult;
        }

        public delegate void SetDsRules(DataSet ds, string str_TableName);

        private event SetDsRules SetDsRulesEvent;
        public void OpenRuleForm()
        {
            try
            {
                form_Rules = new Form_Rules();
                SetDsRulesEvent += new SetDsRules(SetDsRulesCall);
                form_Rules.SetDsRules = SetDsRulesEvent;
                form_Rules.ShowDialog();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable AddColumns(DataTable dataTable, string strColName, Type typeDC = null)
        {
            try
            {
                if (!(dataTable.Columns.Contains(strColName)))
                {
                    if (typeDC != null)
                    {
                        dataTable.Columns.Add(strColName, typeDC);
                    }
                    else
                    {
                        dataTable.Columns.Add(strColName);
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable AddColumnsWithData(DataTable dataTable, string strColName, string strColNameForData, Type typeDC = null, int int_SetOrdinal = 0)
        {
            try
            {
                if (!(dataTable.Columns.Contains(strColName)))
                {
                    if (typeDC != null)
                    {
                        dataTable.Columns.Add(strColName, typeDC).SetOrdinal(int_SetOrdinal);
                    }
                    else
                    {
                        dataTable.Columns.Add(strColName).SetOrdinal(int_SetOrdinal);
                    }
                }

                foreach (DataRow dr in dataTable.Rows)
                {
                    dr[strColName] = dr[strColNameForData];
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal DataTable ChangeColumnTypeWithData(DataTable dt, string strColName, Type typeDC)
        {
            try
            {
                if (!(dt.Columns.Contains($"{strColName}__")))
                {
                    dt.Columns.Add($"{strColName}__", typeDC).SetOrdinal(0);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    dr[$"{strColName}__"] = Convert.ChangeType(dr[strColName].ToString(), typeDC);
                }

                dt.Columns.Remove(strColName);

                if (!(dt.Columns.Contains(strColName)))
                {
                    dt.Columns.Add(strColName, typeDC).SetOrdinal(0);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    dr[strColName] = Convert.ChangeType(dr[$"{strColName}__"].ToString(), typeDC);
                }

                dt.Columns.Remove($"{strColName}__");

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable AddColumns(DataTable dataTable, DataColumn dataColumn)
        {
            try
            {
                if (!(dataTable.Columns.Contains(dataColumn.ColumnName)))
                {
                    dataTable.Columns.Add(dataColumn);
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DBConnectionStatus DBConnectionTest(string str_DBType, string str_ServerName, string str_DBName, bool boolWinAuthType, string str_Username, string str_Password, string str_OraclePortNo)
        {
            DBConnectionStatus dBConnection = new DBConnectionStatus();
            try
            {
                dBConnection.strConnectionString = string.Empty;
                if (str_DBType.ToLower() == "oracle")
                {
                    strConnection = $"Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP) (HOST = {str_ServerName} )(PORT = {str_OraclePortNo})) (CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = {str_DBName}))); User Id={str_Username};Password={str_Password};";

                    using (OracleConnection oracleConnection = new OracleConnection(strConnection))
                    {
                        oracleConnection.Open();
                        if (oracleConnection.State == ConnectionState.Open)
                        {
                            //DisplayMessage("Successfully connected To Oracle DB", false, -1, Color.ForestGreen);
                            dBConnection.strConnectionMSG = "Successfully connected To Oracle DB";
                            dBConnection.strConnectionString = strConnection;
                            dBConnection.boolConnection = true;
                            oracleConnection.Close();
                            //return true;
                        }
                        else
                        {
                            //DisplayMessage("Connection with the oracle DB failed", false, -1, Color.Red);
                            dBConnection.strConnectionMSG = "Connection with the oracle DB failed";
                            dBConnection.boolConnection = false;
                            //return false;
                        }
                    }
                }
                else if (str_DBType.ToLower() == "sql")
                {
                    if (boolWinAuthType)
                    {
                        strConnection = $"Data Source = {str_ServerName};Initial Catalog = {str_DBName}; Integrated Security = true ";
                    }
                    else
                    {
                        strConnection = $"Data Source = {str_ServerName};Initial Catalog = {str_DBName}; User ID = {str_Username};Password = {str_Password};";
                    }

                    using (SqlConnection sqlConn = new SqlConnection(strConnection))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == ConnectionState.Open)
                        {
                            //DisplayMessage("Successfully connected To SQL DB", false, -1, Color.DarkGreen);
                            sqlConn.Close();
                            dBConnection.strConnectionMSG = "Successfully connected To SQL DB";
                            dBConnection.strConnectionString = strConnection;
                            dBConnection.boolConnection = true;
                            //return true;
                        }
                        else
                        {
                            //DisplayMessage("Connection with the SQL DB failed", false, -1, Color.DarkGreen);
                            dBConnection.strConnectionMSG = "Connection with the SQL DB failed";
                            dBConnection.boolConnection = false;
                            //return false;
                        }
                    }
                }
                else
                {
                    dBConnection.strConnectionMSG = "Error selection in DB";
                    dBConnection.strConnectionString = string.Empty;
                    dBConnection.boolConnection = false;
                    //return false;
                }
                return dBConnection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string RemoveSpecialCharacters(string str, string str_replace = "_")
        {
            try
            {
                Regex regex = new Regex("[^a-zA-Z0-9]");
                string str1 = regex.Replace(str, str_replace);
                return str1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string RemoveOtherCharacters(string str, string str_replace = "_")
        {
            try
            {
                Regex regex = new Regex("[^a-zA-Z0-9]");
                string str1 = regex.Replace(str, str_replace);
                return str1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RemoveSpecialCharactersInColumn(string str, string str_replace = "_")
        {
            try
            {
                Regex regex = new Regex("[^a-zA-Z0-9_]");
                string str1 = regex.Replace(str, str_replace);
                return str1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This is for date format in ddMMyyyy HHmmss
        /// </summary>
        /// <returns></returns>
        public string AppendDateInOutputFileName(DateTime dateTime, string str_format = "dd_MM_yyyy_HH_mm_ss")
        {
            string str_return;
            try
            {
                str_return = dateTime.ToString(str_format);
                str_return = RemoveSpecialCharacters(str_return);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return str_return;
        }

        public static string AppendDateInOutputFileName(string str_format = "dd_MMM_yyyy_HH_mm_ss")
        {
            string str_return;
            try
            {
                str_return = DateTime.Now.ToString(str_format);
                str_return = RemoveOtherCharacters(str_return);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return str_return;
        }
        private void SetDsRulesCall(DataSet ds, string str_TableName)
        {
            try
            {
                dsRules = ds.Copy();
                boolRules = true;
                dsRules = ChangeColumnToString(dsRules);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetDatasetRules(string str_Rule = null)
        {
            try
            {
                if (str_Rule != null && str_Rule != string.Empty)
                {
                    str_RuleFileName = str_Rule;
                }
                else
                {
                    str_RuleFileName = Properties.Settings.Default.RuleFilePath.ToString();
                }

                dsRules = LoadExcelFromDataReader(str_RuleFileName, "Rules for comparison");
                dsRules = ChangeColumnToString(dsRules);

                if (dsRules != null && dsRules.Tables.Count > 0 && dsRules.Tables[0].Rows.Count > 0)
                {
                    DataColumn[] dc_PK = new DataColumn[1];
                    dc_PK[0] = dsRules.Tables[0].Columns["srno"];
                    dsRules.Tables[0].PrimaryKey = dc_PK;
                }

                return dsRules;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GetRulesStatus()
        {
            try
            {
                str_RuleFileName = Properties.Settings.Default.RuleFilePath.ToString();

                if (Common_Data.StringNotNullOrEmpty(str_RuleFileName, ".xlsx", false, true))
                {
                    boolRules = true;
                    return boolRules;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable RemoveColumn(DataTable dt, string str_ColumnName)
        {
            try
            {
                if (dt.Columns.Contains(str_ColumnName))
                {
                    dt.Columns.Remove(str_ColumnName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        internal ExcelWorksheet AddNewWorksheet(ExcelPackage pck, ExcelWorksheet worksheet, DataTable dt, string strTableName = null)
        {
            try
            {
                if (strTableName != null)
                {
                    dt.TableName = strTableName;
                }

                worksheet = pck.Workbook.Worksheets.Add(dt.TableName);
                worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                worksheet.Cells.AutoFitColumns(22, 55);

                return worksheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal DataTable DeleteRowFromDatatable(DataTable dt, DataRow dr)
        {
            try
            {
                dt.Rows.Remove(dr);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public DataTable RemoveColumnExcept(DataTable dt_p, string str_ColumnName)
        public DataTable RemoveColumnExcept(DataTable dt, string str_ColumnName)
        {
            try
            {
                //dt = dt_p.Copy();
                bool boolPresent = false;
                string[] str = str_ColumnName.Split(',');

                foreach (DataColumn dc in dt.Columns.Cast<DataColumn>().ToList())
                {
                    boolPresent = false;
                    foreach (string str_ in str)
                    {
                        if (dc.ColumnName.ToString().Trim().ToUpper() == str_.Trim().ToUpper())
                        {
                            boolPresent = true;
                            break;
                        }
                    }
                    if (!(boolPresent))
                    {
                        dt.Columns.Remove(dc);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable RemoveColumnExcept(DataTable dt, string[] str)
        {
            try
            {
                bool boolPresent = false;
                string[] str_arr;

                foreach (DataColumn dc in dt.Columns.Cast<DataColumn>().ToList())
                {
                    boolPresent = false;
                    foreach (string str_ in str)
                    {
                        str_arr = str_.Split(',');

                        foreach (string str__ in str_arr)
                        {
                            if (dc.ColumnName == str__.Trim())
                            {
                                boolPresent = true;
                                break;
                            }
                        }

                    }
                    if (!(boolPresent))
                    {
                        dt.Columns.Remove(dc);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public static DataTable RemoveUncommonColumnInDataTable(DataTable dt1, DataTable dt2)
        {
            try
            {
                foreach (DataColumn dc in dt1.Columns.Cast<DataColumn>().ToArray())
                {
                    if (!(dt2.Columns.Contains(dc.ColumnName)))
                    {
                        dt1.Columns.Remove(dc);
                    }
                }

                dt1.AcceptChanges();

                return dt1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool boolCompareResult { get; private set; }
        public bool boolRulesCheck { get; private set; }

        public bool boolFixedRules { get; private set; }

        public string str_RuleSource { get; private set; }
        public string str_RuleTarget { get; private set; }

        private DataView dv;

        private DataRow[] dR_array;

        private Common_Data common_Data;

        private string str_SRC_end;
        private string str_TRG_end;
        private string str_SRC;
        private string str_TRG;
        private int int_no_of_Character;
        private DataTable dt_src;

        private bool boolCheckWithoutEndCharacters(string str_char, string str_Source, string str_Target)
        {
            try
            {
                bool boolSRC, boolTRG;
                boolSRC = boolTRG = false;
                boolFixedRules = false;

                str_char = str_char.Trim();
                str_Source = str_Source.Trim();
                str_Target = str_Target.Trim();

                int_no_of_Character = str_char.Length;

                #region New Code

                if (!(str_Source.EndsWith(str_char, StringComparison.OrdinalIgnoreCase)))
                {
                    //str_SRC = str_Source.Remove(str_Source.Length - str_char.Length,str_char.Length);
                    str_SRC = $"{str_Source}{str_char}";
                }
                else
                {
                    str_SRC = str_Source;
                    boolSRC = true;
                }

                if (!(str_Target.EndsWith(str_char, StringComparison.OrdinalIgnoreCase)))
                {
                    //str_TRG = str_Target.Remove(str_Target.Length - str_char.Length, str_char.Length);
                    str_TRG = $"{str_Target}{str_char}";
                }
                else
                {
                    str_TRG = str_Target;
                    boolTRG = true;
                }

                if (string.Equals(str_SRC.Trim(), str_TRG.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    if (boolSRC || boolTRG)
                    {
                        boolFixedRules = true;
                    }
                }
                else
                {
                    boolFixedRules = false;
                }

                return boolFixedRules;

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool CheckRulesInFile(string str_Source, string str_Target, string str_Source_ColName, string str_TargetCol_Name, DataRow drr_Source, DataRow drr_Target, DataColumnCollection dcc_Source, DataColumnCollection dcc_Target)
        {
            boolRulesCheck = false;
            string str_Fitler = string.Empty;

            if (str_Source.Contains("'"))
            {
                str_Source = str_Source.Replace("'", "''");
            }

            if (str_Target.Contains("'"))
            {
                str_Target = str_Target.Replace("'", "''");
            }

            try
            {
                if (dsRules != null && dsRules.Tables.Count > 0 && dsRules.Tables[0].Rows.Count > 0)
                {
                    #region Rule 1 when source column cell data is to match with target column cell data

                    str_Fitler = $"[Source] = '{str_Source}' And [Target] = '{str_Target}' And [RuleType] = '1'";
                    dR_array = dsRules.Tables[0].Select(str_Fitler);
                    if (dR_array.Length > 0)
                    {
                        return boolRulesCheck = true;
                    }
                    else
                    {
                        boolRulesCheck = false;
                    }

                    #endregion Rule 1 ends here

                    #region Rule 2 when source column cell data is to match with target column cell data for specific column in source and target which needed to filled in col Source_ColName and Target_ColName

                    str_Fitler = $"Source = '{str_Source}' and Target = '{str_Target}' and Source_ColName = '{str_Source_ColName}' and Target_Colname = '{str_TargetCol_Name}' and RuleType = '2'";
                    dR_array = dsRules.Tables[0].Select(str_Fitler);

                    if (dR_array.Length > 0)
                    {
                        return boolRulesCheck = true;
                    }
                    else
                    {
                        boolRulesCheck = false;
                    }

                    #endregion Rule 2 ends here

                    #region Rule 3 when source column cell data is to match with target column cell data for specific column in source and target which needed to filled in col Source_ColName and Target_ColName

                    str_Fitler = $"Source = '{str_Source}' and Target = '{str_Target}' and Source_ColName = '{str_Source_ColName}' and Target_Colname = '{str_TargetCol_Name}' and RuleType = '3'";

                    dR_array = dsRules.Tables[0].Select(str_Fitler);

                    if (dR_array.Length > 0)
                    {
                        if (dcc_Source.Contains(dR_array[0][6].ToString()) && dcc_Target.Contains(dR_array[0][7].ToString()) && drr_Source[dR_array[0][6].ToString()].ToString() == dR_array[0][8].ToString() && drr_Target[dR_array[0][7].ToString()].ToString() == dR_array[0][9].ToString())
                        {
                            return boolRulesCheck = true;
                        }
                        else
                        {
                            boolRulesCheck = false;
                        }
                    }
                    else
                    {
                        boolRulesCheck = false;
                    }

                    #endregion Rule 3 ends here

                    #region Rule Type Not Applicable 1 Bypassing mm and degree celcius 

                    if (boolCheckWithoutEndCharacters("mm", str_Source, str_Target))
                    {
                        return boolRulesCheck = true;
                    }
                    else
                    {
                        boolRulesCheck = false;
                    }

                    if (boolCheckWithoutEndCharacters("°C", str_Source, str_Target))
                    {
                        return boolRulesCheck = true;
                    }
                    else
                    {
                        boolRulesCheck = false;
                    }

                    if (boolCheckWithoutEndCharacters("C", str_Source, str_Target))
                    {
                        return boolRulesCheck = true;
                    }
                    else
                    {
                        boolRulesCheck = false;
                    }

                    #endregion Rule Type Not Applicable 1 ends here
                }

                //if (!(boolRulesCheck))
                //{
                //    if (!(str_Source.Equals(str_Target)))
                //    {
                //        return boolRulesCheck = true;
                //    }
                //}

                return boolRulesCheck;
            }
            catch (Exception ex)
            {
                throw new Exception($" {ex.Message.ToString()} Error for data :{str_Source} and {str_Target} ", ex);
            }
        }

        internal bool MismatchTest(string src, string trg, bool boolIgnoreCase, bool boolCheckRules, string str_Source_ColName, string str_TargetCol_Name, DataRow drr_Source, DataRow drr_Target, DataColumnCollection dcc_Source, DataColumnCollection dcc_Target)
        {
            bool boolres = false;
            try
            {
                //abc pqr
                if (boolCheckRules)
                {
                    boolres = CheckRulesInFile(src, trg, str_Source_ColName, str_TargetCol_Name, drr_Source, drr_Target, dcc_Source, dcc_Target);

                    if (boolres)
                    {
                        boolres = false;
                        return false;
                    }
                }

                //abc ABC
                if (!(string.Equals(src, trg)))
                {
                    if (boolIgnoreCase)
                    {
                        //ABC abc
                        if (string.Equals(src, trg, StringComparison.OrdinalIgnoreCase))
                        {
                            boolres = false;
                            return false;
                        }
                        else
                        {
                            boolres = true;
                            return true;
                        }
                    }
                    else
                    {
                        boolres = true;
                        return true;
                    }
                }
                else
                {
                    boolres = false;
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception($" {ex.Message.ToString()} Error for data :{src} and {trg} ", ex);
            }
            //return boolres;
        }

    }


    public class SaveFileDialogFile
    {
        private SaveFileDialog saveFileDialog;
        public SaveFileDialogFile()
        {
            BoolFileSaveStatus = false;
            Str_saveFileNameWithPath = string.Empty;
        }
        public bool BoolFileSaveStatus { get; set; }
        public string Str_saveFileNameWithPath { get; set; }
        public string Str_OnlyPath { get; set; }

        private string GetDirectory(string strPath)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(strPath);
                return directoryInfo.GetDirectories()[0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SaveFileDialogFile SaveFileDialogOnly(string str_Title, string excelFileNameSaved, string strExtension)
        {
            SaveFileDialogFile saveFileDialogFile = new SaveFileDialogFile();
            try
            {
                saveFileDialog = new SaveFileDialog
                {
                    CheckFileExists = false,
                    Filter = $"{strExtension} files (*.{strExtension})|*.{strExtension}",
                    Title = str_Title,
                    FileName = excelFileNameSaved,
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    saveFileDialogFile.BoolFileSaveStatus = true;
                    saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialog.FileName;
                }
                else
                {
                    saveFileDialogFile.BoolFileSaveStatus = false;
                    saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return saveFileDialogFile;
        }
        public SaveFileDialogFile SaveFileDialogExcelFileOnly(string str_Title, string excelFileNameSaved)
        {
            SaveFileDialogFile saveFileDialogFile = new SaveFileDialogFile();
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
                    saveFileDialogFile.BoolFileSaveStatus = true;
                    saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialog.FileName;
                    saveFileDialogFile.Str_OnlyPath = saveFileDialog.FileName.Substring(0, saveFileDialog.FileName.LastIndexOf(@"\"));
                }
                else
                {
                    saveFileDialogFile.BoolFileSaveStatus = false;
                    saveFileDialogFile.Str_saveFileNameWithPath = saveFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return saveFileDialogFile;
        }
        public SaveFileDialog SaveFileDialogExcelFile(string str_Title, string excelFileNameSaved, DataTable dt)
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
                        for (int i = 2; i < dt.Rows.Count + 2; i++)
                        {
                            worksheet.Row(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Distributed;
                        }
                        pck.SaveAs(new System.IO.FileInfo(saveFileDialog.FileName));

                        Str_saveFileNameWithPath = saveFileDialog.FileName;
                        BoolFileSaveStatus = true;
                    }
                    #endregion
                }
                else
                {
                    BoolFileSaveStatus = false;
                    throw new Exception("File Not Saved");
                }
            }
            catch (Exception)
            {
                throw;
            }

            return saveFileDialog;
        }

        public SaveFileDialog SaveFileDialogExcelFileForDataset(DataSet ds, string str_Title = null, string excelFileNameSaved = null)
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
                        foreach (DataTable dt in ds.Tables)
                        {
                            ExcelWorksheet worksheet = pck.Workbook.Worksheets.Add(dt.TableName.ToString());
                            worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                            worksheet.Cells.AutoFitColumns();
                        }
                        pck.SaveAs(new System.IO.FileInfo(saveFileDialog.FileName));
                        Str_saveFileNameWithPath = saveFileDialog.FileName;
                        BoolFileSaveStatus = true;
                    }
                    #endregion
                }
                else
                {
                    BoolFileSaveStatus = false;
                    throw new Exception("File Not Saved");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return saveFileDialog;
        }

    }

    public class OpenFileDialogData
    {
        private Exception myExceptionData;
        private OpenFileDialogData openFileDialogData;// = new OpenFileDialogData();
        public bool boolFileSelected { get; private set; }
        public string strFileNameWithPath { get; private set; }
        public string SimpleFileName { get; set; }
        public string SimpleFileName_WithoutExt { get; set; }
        public string PathName_withoutFileName { get; private set; }
        public string ExtensionFileName { get; private set; }
        public OpenFileDialogData()
        {
            boolFileSelected = false;
            strFileNameWithPath = string.Empty;
            SimpleFileName = string.Empty;
            PathName_withoutFileName = string.Empty;
            ExtensionFileName = string.Empty;

            myExceptionData = new Exception();
        }
        public string folderBrowser(string oldFilePath = null)
        {
            string fileName = string.Empty;
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
                else
                {
                    fileName = oldFilePath;
                }
            }
            catch (Exception ex)
            {
                fileName = ex.Message.ToString();
                myExceptionData = ex;
            }
            return fileName;
        }
        public OpenFileDialogData openFileDialog(string str_title = "Browse Files", string DefaultExtension = "xlsx")
        {
            openFileDialogData = new OpenFileDialogData();

            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = str_title,

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = DefaultExtension,
                Filter = $"{DefaultExtension} files (*.{DefaultExtension})|*.{DefaultExtension}",

                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (DefaultExtension.Contains(',') || DefaultExtension.Contains('|'))
            {
                openFileDialog1.Filter = string.Join("|", DefaultExtension.Split(',').Select(x => $"{x} files (*.{x})|*.{x}").AsEnumerable());
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                openFileDialogData.strFileNameWithPath = openFileDialog1.FileName.ToString();
                openFileDialogData.PathName_withoutFileName = Path.GetDirectoryName(openFileDialog1.FileName.ToString());
                openFileDialogData.SimpleFileName = openFileDialog1.SafeFileName.ToString();
                openFileDialogData.SimpleFileName_WithoutExt = openFileDialog1.SafeFileName.ToString().Remove(openFileDialog1.SafeFileName.Length - DefaultExtension.Length - 1, DefaultExtension.Length + 1);
                openFileDialogData.boolFileSelected = true;
            }
            else
            {
                openFileDialogData.strFileNameWithPath = "File Not Selected";
                openFileDialogData.boolFileSelected = false;
            }
            return openFileDialogData;
        }
    }

    //public class CheckRules
    //{
    //    public DataSet dsRules { get; private set; }
    //    public bool boolCompareResult { get; private set; }
    //    public bool boolRulesCheck { get; private set; }

    //    public bool boolFixedRules { get; private set; }

    //    public string str_RuleSource { get; private set; }
    //    private string str_RuleFileName;
    //    public string str_RuleTarget { get; private set; }

    //    private DataView dv;

    //    private DataRow[] dR_array;

    //    private Common_Data common_Data;

    //    private string str_SRC_end;
    //    private string str_TRG_end;
    //    private string str_SRC;
    //    private string str_TRG;
    //    private int int_no_of_Character;
    //    public CheckRules()
    //    {
    //        boolCompareResult = false;
    //        boolRulesCheck = false;
    //        boolFixedRules = false;

    //        str_SRC_end = string.Empty;
    //        str_TRG_end = string.Empty;
    //        str_SRC = string.Empty;
    //        str_TRG = string.Empty;

    //        common_Data = new Common_Data();
    //        if (common_Data.GetRulesStatus())
    //        {
    //            dsRules = common_Data.GetDatasetRules();
    //        }
    //        else
    //        {
    //            dsRules = new DataSet();
    //        }
    //    }

    //public bool CheckRulesInFile(string str_Source, string str_Target, string str_Source_ColName, string str_TargetCol_Name, DataRow drr_Source, DataRow drr_Target, DataColumnCollection dcc_Source, DataColumnCollection dcc_Target)
    //{
    //    boolRulesCheck = false;
    //    string str_Fitler = string.Empty;
    //    try
    //    {
    //        if (dsRules != null && dsRules.Tables.Count > 0 && dsRules.Tables[0].Rows.Count > 0)
    //        {
    //            #region Rule 1 when source column cell data is to match with target column cell data

    //            str_Fitler = $"[Source] = '{str_Source}' And [Target] = '{str_Target}' And [RuleType] = '1'";
    //            dR_array = dsRules.Tables[0].Select(str_Fitler);
    //            if (dR_array.Length > 0)
    //            {
    //                return boolRulesCheck = true;
    //            }
    //            else
    //            {
    //                boolRulesCheck = false;
    //            }

    //            #endregion Rule 1 ends here

    //            #region Rule 2 when source column cell data is to match with target column cell data for specific column in source and target which needed to filled in col Source_ColName and Target_ColName

    //            str_Fitler = $"Source = '{str_Source}' and Target = '{str_Target}' and Source_ColName = '{str_Source_ColName}' and Target_Colname = '{str_TargetCol_Name}' and RuleType = '2'";
    //            dR_array = dsRules.Tables[0].Select(str_Fitler);

    //            if (dR_array.Length > 0)
    //            {
    //                return boolRulesCheck = true;
    //            }
    //            else
    //            {
    //                boolRulesCheck = false;
    //            }

    //            #endregion Rule 2 ends here

    //            #region Rule 3 when source column cell data is to match with target column cell data for specific column in source and target which needed to filled in col Source_ColName and Target_ColName

    //            str_Fitler = $"Source = '{str_Source}' and Target = '{str_Target}' and Source_ColName = '{str_Source_ColName}' and Target_Colname = '{str_TargetCol_Name}' and RuleType = '3'";

    //            dR_array = dsRules.Tables[0].Select(str_Fitler);

    //            if (dR_array.Length > 0)
    //            {
    //                if (dcc_Source.Contains(dR_array[0][6].ToString()) && dcc_Target.Contains(dR_array[0][7].ToString()) && drr_Source[dR_array[0][6].ToString()].ToString() == dR_array[0][8].ToString() && drr_Target[dR_array[0][7].ToString()].ToString() == dR_array[0][9].ToString())
    //                {
    //                    return boolRulesCheck = true;
    //                }
    //                else
    //                {
    //                    boolRulesCheck = false;
    //                }
    //            }
    //            else
    //            {
    //                boolRulesCheck = false;
    //            }

    //            #endregion Rule 3 ends here

    //            #region Rule Type Not Applicable 1 Bypassing mm and degree celcius 

    //            if (boolCheckWithoutEndCharacters("mm", str_Source, str_Target))
    //            {
    //                return boolRulesCheck = true;
    //            }
    //            else
    //            {
    //                boolRulesCheck = false;
    //            }

    //            if (boolCheckWithoutEndCharacters("°C", str_Source, str_Target))
    //            {
    //                return boolRulesCheck = true;
    //            }
    //            else
    //            {
    //                boolRulesCheck = false;
    //            }

    //            if (boolCheckWithoutEndCharacters("C", str_Source, str_Target))
    //            {
    //                return boolRulesCheck = true;
    //            }
    //            else
    //            {
    //                boolRulesCheck = false;
    //            }

    //            #endregion Rule Type Not Applicable 1 ends here
    //        }

    //        if(!(boolRulesCheck))
    //        {
    //            if(!(str_Source.Equals(str_Target)))
    //            {
    //                return boolRulesCheck = true;
    //            }
    //        }

    //        return boolRulesCheck;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception($" {ex.Message.ToString()} Error for data :{str_Source} and {str_Target} ", ex);
    //    }
    //}


    //public DataSet GetDatasetRules(string str_Rule = null)
    //{
    //    try
    //    {
    //        if (str_Rule != null && str_Rule != string.Empty)
    //        {
    //            str_RuleFileName = str_Rule;
    //        }
    //        else
    //        {
    //            str_RuleFileName = Properties.Settings.Default.RuleFilePath.ToString();
    //        }

    //        dsRules = common_Data.LoadExcelFromDataReader(str_RuleFileName, "Rules for comparison");
    //        dsRules = common_Data.ChangeColumnToString(dsRules);

    //        if (dsRules != null && dsRules.Tables.Count > 0 && dsRules.Tables[0].Rows.Count > 0)
    //        {
    //            DataColumn[] dc_PK = new DataColumn[1];
    //            dc_PK[0] = dsRules.Tables[0].Columns["srno"];
    //            dsRules.Tables[0].PrimaryKey = dc_PK;
    //        }

    //        return dsRules;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}
    //    private bool boolCheckWithoutEndCharacters(string str_char, string str_Source, string str_Target)
    //    {
    //        try
    //        {
    //            bool boolSRC, boolTRG;
    //            boolSRC = boolTRG = false;
    //            boolFixedRules = false;

    //            str_char = str_char.Trim();
    //            str_Source = str_Source.Trim();
    //            str_Target = str_Target.Trim();

    //            int_no_of_Character = str_char.Length;

    //            #region New Code

    //            if (!(str_Source.EndsWith(str_char, StringComparison.OrdinalIgnoreCase)))
    //            {
    //                //str_SRC = str_Source.Remove(str_Source.Length - str_char.Length,str_char.Length);
    //                str_SRC = $"{str_Source}{str_char}";
    //            }
    //            else
    //            {
    //                str_SRC = str_Source;
    //                boolSRC = true;
    //            }

    //            if (!(str_Target.EndsWith(str_char, StringComparison.OrdinalIgnoreCase)))
    //            {
    //                //str_TRG = str_Target.Remove(str_Target.Length - str_char.Length, str_char.Length);
    //                str_TRG = $"{str_Target}{str_char}";
    //            }
    //            else
    //            {
    //                str_TRG = str_Target;
    //                boolTRG = true;
    //            }

    //            if (string.Equals(str_SRC.Trim(), str_TRG.Trim(), StringComparison.OrdinalIgnoreCase))
    //            {
    //                if (boolSRC || boolTRG)
    //                {
    //                    boolFixedRules = true;
    //                }
    //            }
    //            else
    //            {
    //                boolFixedRules = false;
    //            }

    //            return boolFixedRules;

    //            #endregion
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }

    //    public bool PNIDVariousRuleChecks(DataTable dt_Source, DataTable dt_Target)
    //    {
    //        int int_test_Count = 0;
    //        DataView dvPNIDSource;
    //        DataView dvPNIDTarget;
    //        string[] str_item_tag;

    //        string str_plant_group_name_src = string.Empty;
    //        string str_measured_variable_src = string.Empty;
    //        string str_tag_seq_no_src = string.Empty;
    //        string str_intru_type_modifier_src = string.Empty;
    //        string str_tag_suffix_src = string.Empty;
    //        string str_short_item_tag_src = string.Empty;
    //        string str_industrial_complex_no_src = string.Empty;
    //        string str_process_area_src = string.Empty;
    //        string str_subprocess_src = string.Empty;
    //        string str_item_tag_src = string.Empty;

    //        string str_plant_group_name_trg = string.Empty;
    //        string str_measured_variable_trg = string.Empty;
    //        string str_tag_seq_no_trg = string.Empty;
    //        string str_intru_type_modifier_trg = string.Empty;
    //        string str_tag_suffix_trg = string.Empty;
    //        string str_short_item_tag_trg = string.Empty;
    //        string str_item_tag_trg = string.Empty;
    //        string str_required_item_tag = string.Empty;

    //        int int_count_trg = 1;
    //        int int_suffix = 0;

    //        DataColumn dc_required_Item_Tag = new DataColumn("Required_Item_Tag", typeof(string));
    //        int int_getOrdinal = dt_Source.Columns["Item Tag"].Ordinal;
    //        if (!(dt_Source.Columns.Contains(dc_required_Item_Tag.ColumnName)))
    //        {
    //            dt_Source.Columns.Add(dc_required_Item_Tag);
    //            dc_required_Item_Tag.SetOrdinal(int_getOrdinal);
    //        }

    //        DataSet ds_Unmatched = new DataSet();
    //        DataTable dt_PNIDUnmatched_Source = new DataTable();
    //        DataTable dt_PNIDUnmatched_Target = new DataTable();

    //        DataSet ds1 = new DataSet();
    //        ds1.Tables.Add(dt_Source.Copy());
    //        ds1.Tables.Add(dt_Target.Copy());

    //        try
    //        {
    //            #region CONVERTING Tag Seq No INTO 4 DIGITS

    //            foreach (DataRow dr in dt_Source.Rows)
    //            {
    //                if (dr["Tag Seq No"].ToString().Length <= 4)
    //                {
    //                    dr["Tag Seq No"] = dr["Tag Seq No"].ToString().PadLeft(4, '0');
    //                }
    //            }

    //            #endregion

    //            //string str_Instrument_Class = "Control valves and regulators";

    //            dt_PNIDUnmatched_Source = dt_Source.Clone();
    //            dt_PNIDUnmatched_Target = dt_Target.Clone();

    //            #region check for instrument class for "Control valves and regulators"

    //            dvPNIDSource = dt_Source.DefaultView;

    //            #region checking required item tag and instrument class for Control valves and regulators

    //            if (dt_Source.Columns.Contains("Item Tag") && dt_Source.Columns.Contains("Instrument Class"))
    //            {
    //                dvPNIDSource.RowFilter = $"[Instrument Class] = 'Control valves and regulators'";
    //                foreach (DataRowView drv in dvPNIDSource)
    //                {
    //                    str_required_item_tag = $"{drv["Plant Group Name"]}-{drv["Measured Variable Code"]}{drv["Tag Seq No"]}-{drv["Measured Variable Code"]}{drv["Instr Type Modifier"]}";
    //                    drv["Required_Item_Tag"] = str_required_item_tag;
    //                }
    //            }

    //            #endregion

    //            dvPNIDTarget = dt_Target.DefaultView;
    //            if (dt_Target.Columns.Contains("Item Tag") && dt_Target.Columns.Contains("Instr Class"))
    //            {
    //                dvPNIDTarget.RowFilter = $"[Instr Class] = 'Control valves and regulators'";
    //            }

    //            dt_PNIDUnmatched_Target.TableName = "Unmatched ";
    //            dt_PNIDUnmatched_Target.Rows.Clear();

    //            foreach (DataRowView drv in dvPNIDSource)
    //            {
    //                dvPNIDTarget.RowFilter = $"[Item Tag] = '{drv["Required_Item_Tag"].ToString()}'";
    //                if (dvPNIDTarget.Count == 0)
    //                {
    //                    dt_PNIDUnmatched_Source.Rows.Add(drv.Row.ItemArray);
    //                }
    //            }

    //            ds_Unmatched.Tables.Add(dt_PNIDUnmatched_Source.Copy());

    //            foreach (DataRowView drv in dvPNIDTarget)
    //            {
    //                str_item_tag = drv["Item Tag"].ToString().Split('-');
    //                str_plant_group_name_trg = drv["Plant Group Name"].ToString();
    //                str_measured_variable_trg = drv["Measured Variable Code"].ToString();
    //                str_tag_seq_no_trg = drv["Tag Seq No"].ToString();
    //                str_intru_type_modifier_trg = drv["Instr Type Modifier"].ToString();
    //                str_tag_suffix_trg = $"-{int_count_trg}";
    //                int_count_trg++;

    //                if (str_item_tag.Length > 2)
    //                {

    //                    dvPNIDSource.RowFilter = $"[Instrument Class] = 'Control valves and regulators' And [Item Tag] = '{str_item_tag[0]}-{str_item_tag[1]}-{str_item_tag[2]}-{str_intru_type_modifier_trg}{str_tag_seq_no_trg}'";

    //                    if (dvPNIDSource.Count == 0)
    //                    {
    //                        dt_PNIDUnmatched_Target.Rows.Add(drv.Row.ItemArray);
    //                    }
    //                }
    //            }

    //            ds_Unmatched.Tables.Add(dt_PNIDUnmatched_Target.Copy());

    //            #endregion

    //            #region check for instrument class for "Other in-line instruments"

    //            dvPNIDSource = dt_Source.DefaultView;
    //            if (dt_Source.Columns.Contains("Item Tag") && dt_Source.Columns.Contains("Instrument Class"))
    //            {
    //                dvPNIDSource.RowFilter = $"[Instrument Class] = 'Other in-line instruments'";
    //            }

    //            dvPNIDTarget = dt_Target.DefaultView;
    //            if (dt_Target.Columns.Contains("Item Tag") && dt_Target.Columns.Contains("Instr Class"))
    //            {
    //                dvPNIDTarget.RowFilter = $"[Instr Class] = 'Other in-line instruments'";
    //            }

    //            string[] str_measurable_codes = { "A", "P", "L", "F", "T" };

    //            dt_PNIDUnmatched_Target.Clear();
    //            dt_PNIDUnmatched_Target.TableName = "Unmatched in Other in-line instruments";
    //            int_test_Count = 0;
    //            int_suffix = 0;

    //            DataView dv_duplicate = dt_Source.DefaultView;

    //            foreach (DataRowView drv in dvPNIDSource)
    //            {
    //                str_measured_variable_src = drv["Measured Variable Code"].ToString();

    //                if (str_measured_variable_src.Length >= 1)
    //                {
    //                    if (str_measurable_codes.Contains(str_measured_variable_src.Substring(0, 1)))
    //                    {
    //                        str_required_item_tag = $"{drv["Plant Group Name"]}-{drv["Measured Variable Code"]}{drv["Tag Seq No"]}-{drv["Measured Variable Code"]}T";
    //                    }
    //                    else if (!(str_measurable_codes.Contains(str_measured_variable_src.Substring(0, 1))))
    //                    {
    //                        str_required_item_tag = $"{drv["Plant Group Name"]}-{drv["Measured Variable Code"]}{drv["Tag Seq No"]}-{drv["Measured Variable Code"]}{drv["Instr Type Modifier"]}";
    //                    }

    //                    drv["Required_Item_Tag"] = str_required_item_tag;

    //                    dv_duplicate = new DataView(dvPNIDSource.ToTable());

    //                    int_suffix++;

    //                }
    //            }

    //            foreach (DataRowView drv in dvPNIDTarget)
    //            {
    //                int_test_Count++;
    //                str_intru_type_modifier_trg = drv["Instr Type Modifier"].ToString();
    //                str_measured_variable_trg = drv["Measured Variable Code"].ToString();
    //                str_item_tag = drv["Item Tag"].ToString().Split('-');
    //                str_tag_seq_no_trg = drv["Tag Seq No"].ToString(); // adding the leading zeroes

    //                if (str_intru_type_modifier_trg.Length >= 2)
    //                {
    //                    if (str_measurable_codes.Contains(str_intru_type_modifier_trg.Substring(0, 1)) && (str_intru_type_modifier_trg.Substring(1, 1) == "T"))
    //                    {
    //                        dvPNIDSource.RowFilter = $"[Instrument Class] = 'Other in-line instruments' And [Item Tag] = '{str_item_tag[0]}-{str_item_tag[1]}-{str_item_tag[2]}-{str_measured_variable_trg}E{str_tag_seq_no_trg}'";

    //                        if (dvPNIDSource.Count == 0)
    //                        {
    //                            dt_PNIDUnmatched_Target.Rows.Add(drv.Row.ItemArray);
    //                        }

    //                    }
    //                    else if ((!str_measurable_codes.Contains(str_intru_type_modifier_trg.Substring(0, 1))) && (str_intru_type_modifier_trg.Substring(1, 1) == "E"))
    //                    {
    //                        dvPNIDSource.RowFilter = $"[Instrument Class] = 'Other in-line instruments' And [Item Tag] = '{str_item_tag[0]}-{str_item_tag[1]}-{str_item_tag[2]}-{str_measured_variable_trg}E{str_tag_seq_no_trg}'";

    //                        if (dvPNIDSource.Count == 0)
    //                        {
    //                            dt_PNIDUnmatched_Target.Rows.Add(drv.Row.ItemArray);
    //                        }

    //                    }
    //                    else
    //                    {
    //                        dvPNIDSource.RowFilter = $"[Instrument Class] = 'Other in-line instruments' And [Item Tag] = '{str_item_tag[0]}-{str_item_tag[1]}-{str_item_tag[2]}-{str_measured_variable_trg}{str_intru_type_modifier_trg}{str_tag_seq_no_trg}'";

    //                        if (dvPNIDSource.Count == 0)
    //                        {
    //                            dt_PNIDUnmatched_Target.Rows.Add(drv.Row.ItemArray);
    //                        }

    //                    }
    //                }
    //            }

    //            ds_Unmatched.Tables.Add(dt_PNIDUnmatched_Target.Copy());

    //            #endregion

    //            #region check for instrument class for "Off-line instruments" or "System functions"

    //            dvPNIDSource = dt_Source.DefaultView;
    //            if (dt_Source.Columns.Contains("Item Tag") && dt_Source.Columns.Contains("Instrument Class"))
    //            {
    //                dvPNIDSource.RowFilter = $"[Instrument Class] = 'Off-line instruments' Or [Instrument Class] = 'System functions'";
    //                dvPNIDSource.Sort = "[Instrument Class] ASC";

    //                int_suffix = 0;

    //                DataTable dt_duplicate = dt_Source.Copy();

    //                dv_duplicate = dt_duplicate.DefaultView;

    //                foreach (DataRow dr in dt_Source.Rows)
    //                {

    //                    dv_duplicate.RowFilter = $"[Item Tag] = '{dr["Item Tag"].ToString()}'";

    //                    if (dv_duplicate.Count > 1)
    //                    {
    //                        int_suffix++;
    //                        str_required_item_tag = $"{dr["Item Tag"]}-{int_suffix}";
    //                    }
    //                    else
    //                    {
    //                        int_suffix = 0;
    //                        str_required_item_tag = $"{dr["Item Tag"]}";
    //                    }
    //                    dr["Required_Item_Tag"] = str_required_item_tag;
    //                }

    //                foreach (DataRowView drv in dvPNIDSource)
    //                {
    //                    str_item_tag_src = $"[Item Tag] = '{drv["Required_Item_Tag"].ToString()}'";

    //                    dvPNIDTarget.RowFilter = str_item_tag_src;
    //                    if (dvPNIDTarget.Count == 0)
    //                    {
    //                        dt_PNIDUnmatched_Source.Rows.Add(drv.Row.ItemArray);
    //                    }
    //                }
    //            }

    //            ds_Unmatched.Tables[0].TableName = "A";
    //            ds_Unmatched.Tables.Add(dt_PNIDUnmatched_Source.Copy());

    //            #endregion

    //            SaveFileDialogFile saveFileDialogFile = new SaveFileDialogFile();
    //            saveFileDialogFile.SaveFileDialogExcelFileForDataset(ds_Unmatched, "Save Excel File For Instrumentation", "Instrumentation Analysis");

    //            if (saveFileDialogFile.BoolFileSaveStatus)
    //            {
    //                Process.Start(saveFileDialogFile.Str_saveFileNameWithPath);
    //            }

    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //        return boolCompareResult;
    //    }
    //}

    public class WriteExcel
    {
        private static int progress = 0;
        private static int totalProgress;
        private static int percentage;
        private static int int_low = 0;
        private static int int_high = 0;
        public static bool ExportToExcel(DataTable dataTable, string strSheetName, string strPathFileName)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet worksheet;
                    worksheet = pck.Workbook.Worksheets.Add(strSheetName);

                    if (dataTable.Columns.Count > 0)
                    {
                        worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

                        foreach (DataColumn dc in dataTable.Columns)
                        {
                            if (dc.DataType == typeof(DateTime))
                            {
                                worksheet.Column((dc.Ordinal + 1)).Style.Numberformat.Format = "dd/MM/yyyy";
                            }
                        }

                        worksheet.Cells.AutoFitColumns(22, 55);
                        pck.SaveAs(new System.IO.FileInfo(strPathFileName));

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet ExportToExcelNoLimitRows(DataTable dataTable, string strSheetName, IQueryable<ColorPairMultiSheet> queryColorpairMS)
        {
            DataSet dsUnmatched = new DataSet();
            try
            {
                decimal dec_reqSheet = 0;
                int dec_rows = dataTable.Rows.Count;
                int int_low = 0;
                DataTable dt1;// = new DataTable();
                do
                {
                    dt1 = new DataTable();
                    dt1 = dataTable.Clone();

                    var test = dataTable.AsEnumerable().Select(x => x).Skip(int_low).Take(1020300);
                    if (dataTable.Rows.Count > 1020300)
                    {
                        dt1.TableName = $"{strSheetName} - {dec_reqSheet++}";
                    }
                    else
                    {
                        dt1.TableName = $"{strSheetName}";
                    }

                    int_low += 1020300;
                    test.AsEnumerable().ToList().ForEach(x => dt1.Rows.Add(x.ItemArray));
                    dsUnmatched.Tables.Add(dt1);

                } while (int_low < dec_rows);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsUnmatched;
        }

        public static ExcelWorksheet ColorDataset(ref ExcelWorksheet worksheet, ref DataSet dsUnmatched, ref List<string> strPKList, ref IQueryable<ColorPairMultiSheet> queryColorpairMS, ref BackgroundWorker backgroundWorker1, ref DoWorkEventArgs e, ExcelPackage pck)
        {
            try
            {
                int_low = 0;
                foreach (DataTable _dt in dsUnmatched.Tables)
                {
                    worksheet = pck.Workbook.Worksheets.Add(_dt.TableName);
                    worksheet.Cells["A1"].LoadFromDataTable(_dt, true);
                    worksheet.Cells.AutoFitColumns(22, 55);

                    worksheet.Cells[1, 2, 1, strPKList.Count + 1].Style.Font.Color.SetColor(Color.Red);
                    Logs.EnterLogsWithTime($"PK Formatting started");
                    totalProgress = worksheet.Dimension.End.Column;

                    IQueryable<ColorPairMultiSheet> qCP;
                    ++int_low;
                    qCP = queryColorpairMS.AsQueryable<ColorPairMultiSheet>().Where(x => x.int_sheet_no == int_low);

                    progress = 0;
                    foreach (var cell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                    {
                        IQueryable<ColorPairMultiSheet> queryIqueryable = qCP.Where(cp => cp.str_Col_Name.ToLower() == cell.Value.ToString().ToLower());

                        foreach (var i in queryIqueryable)
                        {
                            worksheet.Cells[i.int_Row_Source, cell.Start.Column].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[i.int_Row_Source, cell.Start.Column].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                            worksheet.Cells[i.int_Row_Source, cell.Start.Column].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                            worksheet.Cells[i.int_Row_Target, cell.Start.Column].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            worksheet.Cells[i.int_Row_Target, cell.Start.Column].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                            worksheet.Cells[i.int_Row_Target, cell.Start.Column].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return worksheet;
        }

        public static bool ExportToExcel(DataSet ds, string strFileName)
        {
            try
            {
                ds = Common_Data.ChangeColumnToDefaultDataType(ds);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet worksheet;

                    foreach (DataTable dt in ds.Tables)
                    {
                        worksheet = pck.Workbook.Worksheets.Add(dt.TableName);
                        worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                        foreach (DataColumn dc in dt.Columns)
                        {
                            if (dc.DataType == typeof(DateTime))
                            {
                                worksheet.Column((dc.Ordinal + 1)).Style.Numberformat.Format = "dd/MM/yyyy";
                            }
                        }
                        worksheet.Cells.AutoFitColumns(22, 55);
                    }

                    pck.SaveAs(new System.IO.FileInfo(strFileName));
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class ReadExcel
    {
        public static DataSet LoadExcelFromDataReader(string str_file_to_load_path, string dataTableName = null)
        {
            DataSet dsResult = new DataSet();
            try
            {
                using (FileStream stream = File.Open(str_file_to_load_path, FileMode.Open, FileAccess.Read))
                {
                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                    {
                        dsResult = excelReader.AsDataSet();
                        excelReader.Close();

                        string str_columnName = string.Empty;

                        if (dsResult.Tables.Count > 0)
                        {
                            foreach (DataTable dt in dsResult.Tables.Cast<DataTable>().ToArray())
                            {
                                if (dt.Columns.Count > 0 && dt.Rows.Count > 0)
                                {
                                    foreach (DataColumn dc in dt.Columns.Cast<DataColumn>().ToArray())
                                    {
                                        str_columnName = dt.Rows[0][dc.ColumnName.ToString()].ToString().Trim().ToUpper();
                                        if (str_columnName == null || str_columnName == "" || str_columnName == string.Empty)
                                        {
                                            dt.Columns.Remove(dc);
                                            //break;
                                        }
                                        else
                                        {
                                            dc.ColumnName = str_columnName;
                                            dc.Caption = str_columnName;
                                        }
                                    }
                                    dt.Rows.RemoveAt(0);
                                }
                                else
                                {
                                    dsResult.Tables.Remove(dt);
                                }
                            }
                        }

                        if (dataTableName != null && dsResult.Tables.Count > 0)
                        {
                            dsResult.Tables[0].TableName = dataTableName;
                        }
                        dsResult.AcceptChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsResult;
        }
    }
    public class GlobalDebug
    {
        public static string strUserName { get; private set; }
        public static string strPassword { get; private set; }
        public static bool boolIsGlobalDebug { get; private set; }
        public static bool ISGlobalDebug(string strUN, string strPassWD)
        {
            if (Debugger.IsAttached || Environment.UserName == "40009708" || Environment.UserName == "CXUKC" ||
                (strUN == "Krishna" || strPassWD == "DebugKrishnaDebug"))
            {
                strUserName = "Krishna";
                strPassword = "DebugKrishnaDebug";
                boolIsGlobalDebug = true;
                return true;
            }
            else
            {
                strUserName = "";
                strPassword = "";
                boolIsGlobalDebug = false;
                return false;
            }
        }
    }

    public class DBConnectionStatus
    {
        public string strDBType { get; set; }
        public bool boolConnection { get; set; }
        public string strConnectionMSG { get; set; }
        public string strConnectionString { get; set; }
        public string strCMDText { get; set; }
        public Exception exceptionConnection { get; set; }
        public DBConnectionStatus DatabaseConnection(string str_DBType, string str_ServerName, string str_DBName, bool boolWinAuthType, string str_Username, string str_Password, string str_OraclePortNo)
        {
            DBConnectionStatus dBConnectionStatus = new DBConnectionStatus();
            try
            {
                dBConnectionStatus.strDBType = str_DBType;
                if (str_DBType.ToLower() == "oracle")
                {
                    dBConnectionStatus.strConnectionString = $"Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP) (HOST = {str_ServerName} )(PORT = {str_OraclePortNo})) (CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = {str_DBName}))); User Id={str_Username};Password={str_Password};";

                    using (OracleConnection oracleConnection = new OracleConnection(dBConnectionStatus.strConnectionString))
                    {
                        oracleConnection.Open();
                        if (oracleConnection.State == ConnectionState.Open)
                        {
                            oracleConnection.Close();
                            dBConnectionStatus.strConnectionMSG = "Successfully connected To Oracle DB";
                            dBConnectionStatus.boolConnection = true;
                            return dBConnectionStatus;
                        }
                        else
                        {
                            dBConnectionStatus.strConnectionMSG = "Connection with the oracle DB failed";
                            dBConnectionStatus.boolConnection = false;
                            return dBConnectionStatus;
                        }
                    }
                }
                else if (str_DBType.ToLower() == "sql")
                {
                    if (boolWinAuthType)
                    {
                        dBConnectionStatus.strConnectionString = $"Data Source = {str_ServerName};Initial Catalog = {str_DBName}; Integrated Security = true ";
                    }
                    else
                    {
                        dBConnectionStatus.strConnectionString = $"Data Source = {str_ServerName};Initial Catalog = {str_DBName}; User ID = {str_Username};Password = {str_Password};";
                    }

                    using (SqlConnection sqlConn = new SqlConnection(dBConnectionStatus.strConnectionString))
                    {
                        sqlConn.Open();
                        if (sqlConn.State == ConnectionState.Open)
                        {
                            sqlConn.Close();
                            dBConnectionStatus.strConnectionMSG = "Successfully connected To SQL DB";
                            dBConnectionStatus.boolConnection = true;
                            return dBConnectionStatus;
                        }
                        else
                        {
                            dBConnectionStatus.strConnectionMSG = "Connection with the SQL DB failed";
                            dBConnectionStatus.boolConnection = false;
                            return dBConnectionStatus;
                        }
                    }
                }
                else
                {
                    dBConnectionStatus.strConnectionMSG = "There is some problem in connection string";
                    dBConnectionStatus.boolConnection = false;
                    return dBConnectionStatus;
                }
            }
            catch (Exception ex)
            {
                dBConnectionStatus.boolConnection = false;
                dBConnectionStatus.strConnectionMSG = ex.Message.ToString();
                dBConnectionStatus.exceptionConnection = ex;
                return dBConnectionStatus;
            }
        }
    }

    public class DBConnectionDetailsProfile
    {
        public string str_DBType { get; set; }
        public bool bool_Rb_Oracle { get; set; }
        public bool bool_Rb_SQL { get; set; }
        public bool bool_Oracle_DefaultPort { get; set; }
        public int int_Oracle_DefaultPort { get; set; }
        public bool bool_SQL_WA { get; set; }
        public string str_DataSource { get; set; }
        public string str_DBName { get; set; }
        public string str_UserName { get; set; }
        public string str_Password { get; set; }
        public string str_SchemaName { get; set; }
        public string str_TableName { get; set; }

    }

    public class DBC
    {
        public string profileName { get; set; }
        public DBConnectionDetailsProfile dbSRC { get; set; }
        public DBConnectionDetailsProfile dbTRG { get; set; }

        public DBC()
        {
            dbSRC = new DBConnectionDetailsProfile();
            dbTRG = new DBConnectionDetailsProfile();
        }

    }

    public class Profile
    {
        public static List<DBC> ProfileList { get; set; }
    }


    public class DBConnectionDetails
    {
        public bool bool_Rb_SRC_Oracle { get; private set; }
        public bool bool_Rb_SRC_SQL { get; private set; }
        public bool bool_SRC_Oracle_DefaultPort { get; private set; }
        public int int_SRC_Oracle_DefaultPort { get; private set; }
        public bool bool_SRC_SQL_WA { get; private set; }
        public string str_SRC_DataSource { get; private set; }
        public string str_SRC_DBName { get; private set; }
        public string str_SRC_UserName { get; private set; }
        public string str_SRC_Password { get; private set; }
        public string str_SRC_SchemaName { get; private set; }
        public string str_SRC_TableName { get; private set; }

        public bool bool_Rb_TRG_Oracle { get; private set; }
        public bool bool_Rb_TRG_SQL { get; private set; }
        public bool bool_TRG_Oracle_DefaultPort { get; private set; }
        public int int_TRG_Oracle_DefaultPort { get; private set; }
        public bool bool_TRG_SQL_WA { get; private set; }
        public string str_TRG_DataSource { get; private set; }
        public string str_TRG_DBName { get; private set; }
        public string str_TRG_UserName { get; private set; }
        public string str_TRG_Password { get; private set; }
        public string str_TRG_SchemaName { get; private set; }
        public string str_TRG_TableName { get; private set; }

        public DBConnectionDetails()
        {
            bool_Rb_SRC_Oracle = true;
            bool_Rb_SRC_SQL = false;
            bool_SRC_SQL_WA = false;
            bool_SRC_Oracle_DefaultPort = true;

            int_SRC_Oracle_DefaultPort = 1521;

            str_SRC_DataSource = string.Empty;
            str_SRC_DBName = string.Empty;
            str_SRC_UserName = string.Empty;
            str_SRC_Password = string.Empty;
            str_SRC_SchemaName = string.Empty;
            str_SRC_TableName = string.Empty;

            bool_Rb_TRG_Oracle = true;
            bool_Rb_TRG_SQL = false;
            bool_TRG_SQL_WA = false;
            bool_TRG_Oracle_DefaultPort = true;

            int_TRG_Oracle_DefaultPort = 1521;

            str_TRG_DataSource = string.Empty;
            str_TRG_DBName = string.Empty;
            str_TRG_UserName = string.Empty;
            str_TRG_Password = string.Empty;
            str_TRG_SchemaName = string.Empty;
            str_TRG_TableName = string.Empty;
        }

        public DBConnectionDetails(int switchIndex) : this()
        {
            try
            {
                if (Profile.ProfileList != null && switchIndex >= 0)
                {
                    List<DBC> items = Profile.ProfileList;

                    this.bool_Rb_SRC_Oracle = items[switchIndex].dbSRC.bool_Rb_Oracle;
                    this.bool_Rb_TRG_Oracle = items[switchIndex].dbTRG.bool_Rb_Oracle;

                    this.bool_Rb_SRC_SQL = items[switchIndex].dbSRC.bool_Rb_SQL;
                    this.bool_Rb_TRG_SQL = items[switchIndex].dbTRG.bool_Rb_SQL;

                    this.bool_SRC_Oracle_DefaultPort = items[switchIndex].dbSRC.bool_Oracle_DefaultPort;
                    this.bool_TRG_Oracle_DefaultPort = items[switchIndex].dbTRG.bool_Oracle_DefaultPort;

                    this.bool_SRC_SQL_WA = items[switchIndex].dbSRC.bool_SQL_WA;
                    this.bool_TRG_SQL_WA = items[switchIndex].dbTRG.bool_SQL_WA;

                    this.int_SRC_Oracle_DefaultPort = items[switchIndex].dbSRC.int_Oracle_DefaultPort;
                    this.int_TRG_Oracle_DefaultPort = items[switchIndex].dbTRG.int_Oracle_DefaultPort;

                    this.str_SRC_DataSource = items[switchIndex].dbSRC.str_DataSource;
                    this.str_TRG_DataSource = items[switchIndex].dbTRG.str_DataSource;

                    this.str_SRC_DBName = items[switchIndex].dbSRC.str_DBName;
                    this.str_TRG_DBName = items[switchIndex].dbTRG.str_DBName;

                    this.str_SRC_UserName = items[switchIndex].dbSRC.str_UserName;
                    this.str_TRG_UserName = items[switchIndex].dbTRG.str_UserName;

                    this.str_SRC_TableName = items[switchIndex].dbSRC.str_TableName;
                    this.str_TRG_TableName = items[switchIndex].dbTRG.str_TableName;

                    this.str_SRC_Password = items[switchIndex].dbSRC.str_Password;
                    this.str_TRG_Password = items[switchIndex].dbTRG.str_Password;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class ColorPair
    {
        public int int_Row { get; set; }
        public string str_Col_Name { get; set; }
        public ColorPair(int int_Row, string str_Col_Name)
        {
            this.int_Row = int_Row;
            this.str_Col_Name = str_Col_Name;
        }
    }

    public class ColorPairMultiSheet
    {
        public int int_Row_Source { get; set; }
        public int int_Row_Target { get; set; }
        public string str_Col_Name { get; set; }
        public int int_sheet_no { get; set; }
        public ColorPairMultiSheet(int int_Row_Source, int int_Row_Target, string str_Col_Name, int int_sheet_no = 1)
        {
            this.int_Row_Source = int_Row_Source;
            this.int_Row_Target = int_Row_Target;
            this.int_sheet_no = int_sheet_no;
            this.str_Col_Name = str_Col_Name;
        }
    }

    public class DBTOExcel
    {
        public string strTableName { get; private set; }
        public int int_No_Rows { get; private set; }
        public DBTOExcel(string str_CompleteTableName, int count)
        {
            this.strTableName = str_CompleteTableName;
            this.int_No_Rows = count;
        }
    }

    public class BackGroundWorkerUserObjectForReport
    {
        internal int int_Count { get; set; }
        internal int int_TotalCount { get; set; }
        internal string strMsg { get; set; }

        internal bool boolError { get; set; }

        internal Exception bgwException { get; set; }

        public BackGroundWorkerUserObjectForReport()
        {
            this.int_Count = 0;
            this.int_TotalCount = 0;
            this.strMsg = string.Empty;
        }
        public BackGroundWorkerUserObjectForReport(int intC, int intT, string str = null, bool Error = false)
        {
            this.int_Count = intC;
            this.int_TotalCount = intT;
            this.strMsg = str;
            this.boolError = Error;
        }

        public BackGroundWorkerUserObjectForReport(Exception exception)
        {
            this.bgwException = exception;
            this.boolError = true;
        }

        public BackGroundWorkerUserObjectForReport(string str)
        {
            this.strMsg = str;
            this.int_Count = 0;
            this.int_TotalCount = 1;
            this.boolError = false;
        }

    }
}
