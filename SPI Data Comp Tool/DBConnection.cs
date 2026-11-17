using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SPI_Data_Comp_Tool
{
    public partial class DBConnection : UserControl
    {
        private string strQuery;
        private string strOracleConnection;
        private OracleDataAdapter oracleDA;
        private DataTable dt_oracle;
        private DataTable dt_LoadSRC;
        private string strSQLConnection;
        private DataTable dt_sql;
        private SqlDataAdapter sqlDA;
        private string strSourceName;
        private string str_tableName;
        private DBConnectionDetails dBConnectionDetails;
        private Common_Data common_Data;

        public DBConnection()
        {
            InitializeComponent();
            common_Data = new Common_Data();

            cb_UserProfileList = common_Data.ReadUserProfile(cb_UserProfileList);

            //if (cb_UserProfileList.Items.Count >= 0)
            //{
            //    cb_UserProfileList.SelectedIndex = 0;
            //}

        }

        private void DBConnection_Load(object sender, EventArgs e)
        {

        }

        private void btn_Load_SRC_Table_Click(object sender, EventArgs e)
        {
            try
            {
                resetLabelsToDefaultValue();
                //UpdateLabel(lblStatus, "Pls Wait, Loading !!!");
                if (rb_SRC_oracle.Checked)
                {
                    strQuery = "SELECT * FROM all_tables where OWNER != 'SYS' ORDER BY OWNER, TABLE_NAME";

                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_SRC_DataSource.Text + " )(PORT = " + tb_SRC_DefaultPort.Text + "))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_SRC_DBName.Text + ")));"
                        + "User Id=" + tb_SRC_UserName.Text + ";Password=" + tb_SRC_Password.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    dt_oracle = new DataTable();
                    oracleDA.Fill(dt_oracle);
                    dt_oracle.TableName = "Load Table Name";
                    FillCheckListBox(cb_SRC_TableName, dt_oracle, "oracle");
                    dt_LoadSRC = dt_oracle.Copy();
                }

                if (rb_SRC_SQL.Checked)
                {
                    strQuery = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";

                    if (cb_WindowAuthentication_SRC.CheckState == CheckState.Checked)
                    {
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    + "; Integrated Security = true ";
                    }
                    else if (cb_WindowAuthentication_SRC.CheckState == CheckState.Unchecked)
                    {
                        strSQLConnection = "Data Source = " + tb_SRC_DataSource.Text + ";Initial Catalog = " + tb_SRC_DBName.Text
                    + "; User ID = " + tb_SRC_UserName.Text + "; " + "Password = " + tb_SRC_Password.Text + ";";
                    }

                    dt_sql = new DataTable();
                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_sql);

                    strSourceName = "Source SQL";
                    FillCheckListBox(cb_SRC_TableName, dt_sql, "sql");
                    dt_LoadSRC = dt_sql.Copy();
                }
                //lblStatus.Text = "Table Loaded in Source";

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void FillCheckListBox(ComboBox comboBox, DataTable dataTable, string str_dbType)
        {
            try
            {
                if (!(dataTable.Columns.Contains("CompleteName")))
                {
                    dataTable.Columns.Add("CompleteName");
                }
                List<string> lst_str_fill = new List<string>();

                if (str_dbType == "oracle")
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        str_tableName = $"{dr["OWNER"].ToString()}.{dr["TABLE_NAME"].ToString()}";
                        lst_str_fill.Add(str_tableName);
                        dr["CompleteName"] = str_tableName;
                    }
                }
                else if (str_dbType == "sql")
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        str_tableName = $"{dr["TABLE_CATALOG"].ToString()}.{dr["TABLE_SCHEMA"].ToString()}.{dr["TABLE_NAME"].ToString()}";
                        lst_str_fill.Add(str_tableName);
                        dr["CompleteName"] = str_tableName;
                    }
                }

                AutoCompleteStringCollection autoCompleteStringCollection = new AutoCompleteStringCollection();
                autoCompleteStringCollection.AddRange(lst_str_fill.ToArray());
                comboBox.AutoCompleteCustomSource = autoCompleteStringCollection;
                comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.DataSource = dataTable;
                DisplayMemberInComboBox();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void DisplayMemberInComboBox()
        {
            try
            {
                if (cb_FullName.Checked)
                {
                    cb_SRC_TableName.DisplayMember = "CompleteName";
                    cb_TRG_TableName.DisplayMember = "CompleteName";
                    cb_SRC_TableName.ValueMember = "CompleteName";
                    cb_TRG_TableName.ValueMember = "CompleteName";
                }
                else
                {
                    cb_SRC_TableName.DisplayMember = "TABLE_NAME";
                    cb_TRG_TableName.DisplayMember = "TABLE_NAME";
                    cb_SRC_TableName.ValueMember = "CompleteName";
                    cb_TRG_TableName.ValueMember = "CompleteName";
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
        private void resetLabelsToDefaultValue()
        {
            try
            {
                //lblExpressionStatus.ResetText();
                //lblStatus.ResetText();
                //lblCal.ResetText();
                //lbl_PercentageShow.Text = "Progress Bar";
                //progressBar1.Value = 0;
                //myException_BG = new Exception();
                //lblTimer.ResetText();

            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void DisplayError(Exception ex = null)
        {
            //try
            //{
            //    if (ex != null)
            //    {
            //        lblExpressionStatus.Text = ex.Message.ToString();
            //        if (Debugger.IsAttached)
            //        {
            //            lblExpressionStatus.Text = ex.ToString();
            //        }
            //    }
            //    else
            //    {
            //        lblExpressionStatus.Text = string.Empty;
            //    }
            //    lblExpressionStatus.Update();
            //}
            //catch (Exception ex1)
            //{
            //    lblStatus.Text = ex1.Message.ToString();
            //}
        }

        private void groupBox14_Enter(object sender, EventArgs e)
        {

        }

        public void cb_UserProfileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //DisplayMessage(lblStatus, $"User Profile Selected : {cb_UserProfileList.SelectedItem}");

                dBConnectionDetails = new DBConnectionDetails(cb_UserProfileList.SelectedIndex);

                rb_SRC_oracle.Checked = dBConnectionDetails.bool_Rb_SRC_Oracle;
                rb_SRC_SQL.Checked = dBConnectionDetails.bool_Rb_SRC_SQL;

                cb_WindowAuthentication_SRC.Checked = dBConnectionDetails.bool_SRC_SQL_WA;
                cb_SRC_DefaultPort.Checked = dBConnectionDetails.bool_SRC_Oracle_DefaultPort;
                tb_SRC_DefaultPort.Text = dBConnectionDetails.bool_Rb_SRC_Oracle ? dBConnectionDetails.int_SRC_Oracle_DefaultPort.ToString() : string.Empty;

                tb_SRC_DataSource.Text = dBConnectionDetails.str_SRC_DataSource;
                tb_SRC_DBName.Text = dBConnectionDetails.str_SRC_DBName;
                tb_SRC_UserName.Text = dBConnectionDetails.str_SRC_UserName;
                tb_SRC_Password.Text = dBConnectionDetails.str_SRC_Password;
                cb_SRC_TableName.Text = dBConnectionDetails.str_SRC_TableName;

                rb_TRG_Oracle.Checked = dBConnectionDetails.bool_Rb_TRG_Oracle;
                rb_TRG_SQL.Checked = dBConnectionDetails.bool_Rb_TRG_SQL;

                cb_WindowAuthentication_TRG.Checked = dBConnectionDetails.bool_TRG_SQL_WA;
                cb_TRG_DefaultPort.Checked = dBConnectionDetails.bool_TRG_Oracle_DefaultPort;
                tb_TRG_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString();

                tb_TRG_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                tb_TRG_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                tb_TRG_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                tb_TRG_Password.Text = dBConnectionDetails.str_TRG_Password;
                cb_TRG_TableName.Text = dBConnectionDetails.str_TRG_TableName;

                //DisplayMessage(lblStatus, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");
            }
            catch (Exception ex)
            {
                //DisplayError(lblExpressionStatus, ex);
            }
        }
    }
}
