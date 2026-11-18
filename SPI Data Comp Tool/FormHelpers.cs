using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using DataTable = System.Data.DataTable;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Small helpers that used to be copied into several windows. Each window keeps a thin
    /// wrapper with its own error handling and calls these for the shared work.
    /// </summary>
    internal static class FormHelpers
    {
        /// <summary>Shows text in a label or status control and repaints it at once.</summary>
        internal static void ShowText(Control control, string text, Color? color = null)
        {
            control.Text = Common_Data.RedactConnectionCredentials(text);
            if (color.HasValue)
            {
                control.ForeColor = color.Value;
            }
            control.Update();
        }

        /// <summary>Shows an exception in a control: the message, or the full details while debugging. Null clears it.</summary>
        internal static void ShowError(Control control, Exception ex)
        {
            try
            {
                if (ex != null)
                {
                    control.Text = Common_Data.RedactConnectionCredentials(ex.Message);
                    if (Debugger.IsAttached)
                    {
                        control.Text = Common_Data.RedactConnectionCredentials(ex.ToString());
                    }
                }
                else
                {
                    control.Text = string.Empty;
                }
                control.Update();
            }
            catch (Exception ex1)
            {
                control.Text = Common_Data.RedactConnectionCredentials(ex1.Message);
            }
        }

        /// <summary>"3 min(s) 7 second(s) ", with hours in front when there are any.</summary>
        internal static string FormatElapsed(TimeSpan elapsed)
        {
            if (elapsed.Hours == 0)
            {
                return elapsed.Minutes.ToString() + " min(s) " + elapsed.Seconds.ToString() + " second(s) ";
            }
            return elapsed.Hours.ToString() + " hour(s) " +
                elapsed.Minutes.ToString() + " min(s) " + elapsed.Seconds.ToString() + " second(s) ";
        }

        /// <summary>
        /// Turns the summary table on its side: each of its rows becomes a column, and each of its
        /// columns after the first becomes a row.
        /// </summary>
        internal static DataTable Transpose(DataTable source)
        {
            DataTable transposed = new DataTable();
            transposed.Columns.Add(source.Columns[0].ColumnName.ToString());

            foreach (DataRow dr in source.Rows)
            {
                transposed.Columns.Add(dr[0].ToString());
            }

            for (int rCount = 1; rCount <= source.Columns.Count - 1; rCount++)
            {
                DataRow dr = transposed.NewRow();
                dr[0] = source.Columns[rCount].ColumnName.ToString();

                for (int cCount = 0; cCount <= source.Rows.Count - 1; cCount++)
                {
                    dr[cCount + 1] = source.Rows[cCount][rCount].ToString();
                }
                transposed.Rows.Add(dr);
            }
            transposed.Columns[0].ColumnName = "Column Names";
            transposed.Columns[1].ColumnName = "Summarize Count";
            return transposed;
        }

        /// <summary>
        /// Plain look for a sheet that holds one table: no filter buttons, fitted columns, thin black
        /// borders, no fill, and whole numbers stored as numbers.
        /// </summary>
        internal static void FormatTableSheet(IXLWorksheet sheet, DataTable table)
        {
            sheet.Tables.FirstOrDefault().SetShowAutoFilter(false);
            sheet.Columns().AdjustToContents();

            for (int col = 1; col <= table.Columns.Count; col++)
            {
                sheet.Cell(1, col).Style.Fill.SetBackgroundColor(XLColor.Transparent);
                sheet.Cell(1, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                sheet.Cell(1, col).Style.Border.OutsideBorderColor = XLColor.RichBlack;
                sheet.Cell(1, col).Style.Font.SetFontColor(XLColor.RichBlack);
            }

            for (int row = 2; row <= table.Rows.Count + 1; row++)
            {
                for (int col = 1; col < table.Columns.Count + 1; col++)
                {
                    sheet.Cell(row, col).Style.Fill.BackgroundColor = XLColor.Transparent;
                    sheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sheet.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.Black;
                    if (int.TryParse(sheet.Cell(row, col).Value.ToString(), out int _))
                    {
                        sheet.Cell(row, col).SetDataType(XLDataType.Number);
                    }
                }
            }
        }

        /// <summary>
        /// Fills a table picker from a list of tables: adds a "CompleteName" column (OWNER.TABLE for
        /// Oracle, CATALOG.SCHEMA.TABLE for SQL Server), binds the box to the list and turns on
        /// type-ahead. Returns the last full name, or null when the list is empty.
        /// </summary>
        internal static string FillTableNames(ComboBox comboBox, DataTable tables, string dbType)
        {
            if (!tables.Columns.Contains("CompleteName"))
            {
                tables.Columns.Add("CompleteName");
            }
            List<string> names = new List<string>();
            string last = null;

            foreach (DataRow dr in tables.Rows)
            {
                string name;
                if (dbType == "oracle")
                {
                    name = $"{dr["OWNER"]}.{dr["TABLE_NAME"]}";
                }
                else if (dbType == "sql")
                {
                    name = $"{dr["TABLE_CATALOG"]}.{dr["TABLE_SCHEMA"]}.{dr["TABLE_NAME"]}";
                }
                else
                {
                    continue;
                }
                names.Add(name);
                dr["CompleteName"] = name;
                last = name;
            }

            AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(names.ToArray());
            comboBox.AutoCompleteCustomSource = autoComplete;
            comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox.DataSource = tables;
            return last;
        }
    }
}
