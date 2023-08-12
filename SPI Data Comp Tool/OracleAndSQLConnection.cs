using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace SPI_Data_Comp_Tool
{
    public partial class OracleAndSQLConnection : UserControl
    {
        private string strOracleConnection;
        private string strQuery;
        private string strSourceName;
        private string strSQLConnection;

        private OracleDataAdapter oracleDA;
        private SqlDataAdapter sqlDA;

        private DataTable dt_oracle;
        private DataTable dt_sql;
        private bool boolStatus;
        private Exception myException;
        private DataSet ds;

        public OracleAndSQLConnection()
        {
            InitializeComponent();


            lbl_Oracle_SQL_Connection.Text = string.Empty;
            lbl_Oracle_SQL_Connection.Text = string.Empty;

            myException = new Exception();
            ds = new DataSet();

            UseQueryForCheckBoxFunction();
        }

        public DataTable DataTableLoad()
        {
            return dt_oracle;
        }

        public void LoadDebuggingInformation()
        {
            if (Debugger.IsAttached)
            {
                cb_UserProfileList.SelectedIndex = 0;
                cb_query.CheckState = CheckState.Checked;
                Load_DB_Oracle_SQL();
            }
        }
        public DataSet FetchDataSet()
        {
            return ds;
        }

        private DataTable connect_Oracle_DB(string columnNames = " * ")
        {
            try
            {
                dt_oracle = new DataTable();
                strQuery = "select " + columnNames + " from " + tb_Source_tableName.Text;

                if (cb_query.CheckState == CheckState.Checked)
                {
                    strQuery = tb_querybox.Text;
                }

                // oracle 11g and sql selection as requirement changed

                if (rb_SRC_oracle.Checked)
                {
                    strOracleConnection = "Data Source=(DESCRIPTION =" + "(ADDRESS = (PROTOCOL = TCP)" +
                        "(HOST = " + tb_oracle_dsource.Text + " )(PORT = 1521))"
                        + "(CONNECT_DATA =" + "(SERVER = DEDICATED)" + "(SERVICE_NAME = " + tb_oracle_dbName.Text + ")));"
                        + "User Id=" + tb_oracle_username.Text + ";Password=" + tb_oracle_pwd.Text + ";";

                    oracleDA = new OracleDataAdapter(strQuery, strOracleConnection);
                    oracleDA.Fill(dt_oracle);
                    strSourceName = "Source Oracle 11G";
                }

                if (rb_SRC_SQL.Checked)
                {
                    strSQLConnection = "Data Source = " + tb_oracle_dsource.Text + ";Initial Catalog = " + tb_oracle_dbName.Text
                    + "; User ID = " + tb_oracle_username.Text + "; " + "Password = " + tb_oracle_pwd.Text + ";";// providerName=System.Data.SqlClient";

                    sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                    sqlDA.Fill(dt_oracle);
                    strSourceName = "Source SQL";
                }

                dt_oracle.TableName = strSourceName;
                boolStatus = true;
            }
            catch (Exception ex)
            {
                myException = ex;
                boolStatus = false;
            }

            return dt_oracle;
        }

        private void controlEnableToggle(bool boolEnabledCase)
        {
            tb_oracle_dbName.Enabled = boolEnabledCase;
            tb_oracle_dsource.Enabled = boolEnabledCase;
            tb_oracle_username.Enabled = boolEnabledCase;
            tb_oracle_pwd.Enabled = boolEnabledCase;
            tb_Source_tableName.Enabled = boolEnabledCase;

            tb_sql_dbName.Enabled = boolEnabledCase;
            tb_sql_dsource.Enabled = boolEnabledCase;
            tb_sql_username.Enabled = boolEnabledCase;
            tb_sql_pwd.Enabled = boolEnabledCase;
            tb_destination_tableName.Enabled = boolEnabledCase;

            rb_SRC_oracle.Enabled = boolEnabledCase;
            rb_SRC_SQL.Enabled = boolEnabledCase;

            cb_UserProfileList.Enabled = boolEnabledCase;
            btn_Save.Enabled = boolEnabledCase;
            btn_Delete.Enabled = boolEnabledCase;

            btn_Connect_Reference_ID.Enabled = boolEnabledCase;
            if (!(boolEnabledCase))
            {
                lbl_Oracle_SQL_Connection.Text = string.Empty;
            }
        }

        public void Load_DB_Oracle_SQL()
        {
            try
            {
                controlEnableToggle(false);
                lbl_Oracle_SQL_Connection.Text = string.Empty;
                lbl_Oracle_SQL_Connection.Text = string.Empty;
                boolStatus = true;

                // connect with oracle
                dt_oracle = connect_Oracle_DB();

                // conncet with SQL Target
                if (boolStatus)
                {
                    dt_sql = connect_SQL_DB();
                }

                if (boolStatus)
                {
                    ds.Tables.Clear();
                    ds.Tables.Add(dt_oracle);
                    ds.Tables.Add(dt_sql);
                    foreach(DataTable dt_ in ds.Tables)
                    {
                        foreach(DataColumn dc_ in dt_.Columns)
                        {
                            foreach(DataRow dr_ in dt_.Rows)
                            {
                                dr_[dc_] = dr_[dc_].ToString().Trim();
                            }
                        }
                    }

                    lbl_Oracle_SQL_Connection.Text = "All data table loaded";
                }
                else
                {
                    lbl_Oracle_SQL_Connection.Text = "There is some error : " + myException.Message.ToString();
                }

            }
            catch (Exception ex)
            {
                lbl_Oracle_SQL_Connection.Text = ex.Message.ToString();
                boolStatus = false;
            }
            finally
            {
                controlEnableToggle(true);
            }
        }
        private void btn_Connect_Reference_ID_Click(object sender, EventArgs e)
        {
            try
            {
                Load_DB_Oracle_SQL();
            }
            catch (Exception ex)
            {
                lbl_Oracle_SQL_Connection.Text = ex.Message.ToString();
                boolStatus = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void DisplayMessage(Control control, string str_Msg)
        {
            try
            {
                control.Text = str_Msg;
                control.Update();
            }
            catch (Exception ex)
            {
                DisplayError(control, ex);
            }
        }
        private void DisplayError(Control control, Exception ex = null)
        {
            try
            {
                if (ex != null)
                {
                    control.Text = ex.Message.ToString();
                    if (Debugger.IsAttached)
                    {
                        control.Text = ex.ToString();
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
                control.Text = ex1.Message.ToString();
            }
        }
        private DataTable connect_SQL_DB(string columnNames = " * ")
        {
            try
            {
                strQuery = "select " + columnNames + " from " + tb_destination_tableName.Text;
                if (cb_query.CheckState == CheckState.Checked)
                {
                    strQuery = tb_querybox.Text;
                }

                strSQLConnection = "Data Source = " + tb_sql_dsource.Text + ";Initial Catalog = " + tb_sql_dbName.Text
                    + "; User ID = " + tb_sql_username.Text + "; " + "Password = " + tb_sql_pwd.Text + ";";

                dt_sql = new DataTable();
                sqlDA = new SqlDataAdapter(strQuery, strSQLConnection);
                sqlDA.Fill(dt_sql);
                dt_sql.TableName = "Target SQL Table";

                boolStatus = true;
            }
            catch (Exception ex)
            {
                myException = ex;
                boolStatus = false;
            }
            return dt_sql;
        }

        private void UseQueryForCheckBoxFunction()
        {
            try
            {
                if (cb_query.CheckState == CheckState.Checked)
                {
                    tb_querybox.Enabled = true;
                    tb_querybox.Text = @"SELECT
SPEC_FORM.SPEC_FORM_CNUM, 
SPEC_FORM.SPEC_NAME, 
SPEC_FORMAT_ATTRIBUTES.COLUMN_NAME, 
SPEC_FORMAT.FORMAT_NAME, 
SPEC_FORMAT_ATTRIBUTES.HEADER_TEXT
FROM
(SPEC_FORM INNER JOIN SPEC_FORMAT ON SPEC_FORM.SPEC_FORM_ID = SPEC_FORMAT.SPEC_FORM_ID)
INNER JOIN SPEC_FORMAT_ATTRIBUTES ON SPEC_FORMAT.FORMAT_ID = SPEC_FORMAT_ATTRIBUTES.FORMAT_ID

GROUP BY
SPEC_FORM.SPEC_FORM_CNUM, 
SPEC_FORM.SPEC_NAME, 
SPEC_FORMAT_ATTRIBUTES.COLUMN_NAME, 
SPEC_FORMAT.FORMAT_NAME, 
SPEC_FORMAT_ATTRIBUTES.HEADER_TEXT

ORDER BY
SPEC_FORM.SPEC_FORM_CNUM";

                    tb_Source_tableName.Enabled = false;
                    tb_Source_tableName.ResetText();

                    tb_destination_tableName.Enabled = false;
                    tb_destination_tableName.ResetText();
                }
                else if (cb_query.CheckState == CheckState.Unchecked)
                {
                    tb_querybox.Enabled = false;
                    tb_querybox.ResetText();
                    tb_Source_tableName.Enabled = true;
                    tb_destination_tableName.Enabled = true;
                }
            }
            catch (Exception)
            {

            }
        }
        private void cb_query_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UseQueryForCheckBoxFunction();
            }
            catch (Exception ex)
            {
                lbl_Oracle_SQL_Connection.Text = ex.Message.ToString();
            }
        }

        private void rb_oracle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                groupBox1.Text = "Source: Oracle Connection and Settings";
            }
            catch (Exception ex)
            {
                lbl_Oracle_SQL_Connection.Text = ex.Message.ToString();
            }
        }

        private void rb_sql_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                groupBox1.Text = "Source: SQL Connection and Settings";
            }
            catch (Exception ex)
            {
                lbl_Oracle_SQL_Connection.Text = ex.Message.ToString();
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {

        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {

        }

        //private DBConnectionDetails dBConnectionDetails;
        private void cb_UserProfileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //DisplayMessage(lbl_Oracle_SQL_Connection, $"User Profile Selected : {cb_UserProfileList.SelectedItem}");

                //dBConnectionDetails = new DBConnectionDetails(cb_UserProfileList.SelectedIndex);

                //rb_SRC_oracle.Checked = dBConnectionDetails.bool_Rb_SRC_Oracle;
                //rb_SRC_SQL.Checked = dBConnectionDetails.bool_Rb_SRC_SQL;

                //cb_WindowAuthentication_SRC.Checked = dBConnectionDetails.bool_SRC_SQL_WA;
                //cb_SRC_DefaultPort.Checked = dBConnectionDetails.bool_SRC_Oracle_DefaultPort;
                //tb_SRC_DefaultPort.Text = dBConnectionDetails.bool_Rb_SRC_Oracle ? dBConnectionDetails.int_SRC_Oracle_DefaultPort.ToString() : string.Empty;

                //tb_SRC_DataSource.Text = dBConnectionDetails.str_SRC_DataSource;
                //tb_SRC_DBName.Text = dBConnectionDetails.str_SRC_DBName;
                //tb_SRC_UserName.Text = dBConnectionDetails.str_SRC_UserName;
                //tb_SRC_Password.Text = dBConnectionDetails.str_SRC_Password;
                //cb_SRC_TableName.Text = dBConnectionDetails.str_SRC_TableName;

                //rb_TRG_Oracle.Checked = dBConnectionDetails.bool_Rb_TRG_Oracle;
                //rb_TRG_SQL.Checked = dBConnectionDetails.bool_Rb_TRG_SQL;

                //cb_WindowAuthentication_TRG.Checked = dBConnectionDetails.bool_TRG_SQL_WA;
                //cb_TRG_DefaultPort.Checked = dBConnectionDetails.bool_TRG_Oracle_DefaultPort;
                //tb_TRG_DefaultPort.Text = dBConnectionDetails.int_TRG_Oracle_DefaultPort == 0 ? "1521" : dBConnectionDetails.int_TRG_Oracle_DefaultPort.ToString();

                //tb_TRG_DataSource.Text = dBConnectionDetails.str_TRG_DataSource;
                //tb_TRG_DBName.Text = dBConnectionDetails.str_TRG_DBName;
                //tb_TRG_UserName.Text = dBConnectionDetails.str_TRG_UserName;
                //tb_TRG_Password.Text = dBConnectionDetails.str_TRG_Password;
                //cb_TRG_TableName.Text = dBConnectionDetails.str_TRG_TableName;

                //DisplayMessage(lblStatus, $"User Profile :{cb_UserProfileList.SelectedItem}, Loaded Successfully");

                //if (true)//Debugger.IsAttached)
                //{
                //    switch (comboBox1.SelectedIndex + 1)
                //    {
                //        case 1:
                //            rb_oracle.Checked = true;
                //            tb_oracle_dsource.Text = "IESWKCT670";
                //            tb_oracle_dbName.Text = "SPEL11G";
                //            tb_oracle_username.Text = "system";
                //            tb_oracle_pwd.Text = "SPEL11G";
                //            tb_Source_tableName.Text = "tb_compare1";

                //            tb_sql_dsource.Text = "IESWKCT247";
                //            tb_sql_dbName.Text = "db_SPELSQL";
                //            tb_sql_username.Text = "sa";
                //            tb_sql_pwd.Text = "abcde@1234567";
                //            tb_destination_tableName.Text = "tb_compare1";

                //            break;

                //        case 2:
                //            rb_sql.Checked = true;
                //            tb_oracle_dsource.Text = "IESWKCT247";
                //            tb_oracle_dbName.Text = "db_SPELSQL";
                //            tb_oracle_username.Text = "sa";
                //            tb_oracle_pwd.Text = "abcde@1234567";
                //            tb_Source_tableName.Text = "tb_compare1";

                //            tb_sql_dsource.Text = "IESWKCT247";
                //            tb_sql_dbName.Text = "db_SPELSQL";
                //            tb_sql_username.Text = "sa";
                //            tb_sql_pwd.Text = "abcde@1234567";
                //            tb_destination_tableName.Text = "tb_compare1";

                //            break;

                //        case 3:
                //            rb_sql.Checked = true;
                //            tb_oracle_dsource.Text = "IESWKCT247";
                //            tb_oracle_dbName.Text = "SPELOLDP";
                //            tb_oracle_username.Text = "sa";
                //            tb_oracle_pwd.Text = "abcde@1234567";
                //            tb_Source_tableName.Text = "plant1el.t_plantitem";

                //            tb_sql_dsource.Text = "IESWKCT247";
                //            tb_sql_dbName.Text = "SPELOLDP_new";
                //            tb_sql_username.Text = "sa";
                //            tb_sql_pwd.Text = "abcde@1234567";
                //            tb_destination_tableName.Text = "plant1el.t_plantitem";

                //            break;

                //        case 4:
                //            rb_oracle.Checked = true;
                //            tb_oracle_dsource.Text = "IESWKCT670";
                //            tb_oracle_dbName.Text = "SPEL11G";
                //            tb_oracle_username.Text = "system";
                //            tb_oracle_pwd.Text = "SPEL11G";
                //            tb_Source_tableName.Text = "DASISLANDEL.T_motor";

                //            tb_sql_dsource.Text = "IESWKCT247";
                //            tb_sql_dbName.Text = "JK1703_P";
                //            tb_sql_username.Text = "sa";
                //            tb_sql_pwd.Text = "abcde@1234567";
                //            tb_destination_tableName.Text = "Plant789el.T_Motor";

                //            break;

                //        case 5:
                //            rb_oracle.Checked = true;
                //            tb_oracle_dsource.Text = "IESWKCT670";
                //            tb_oracle_dbName.Text = "SPEL11G";
                //            tb_oracle_username.Text = "system";
                //            tb_oracle_pwd.Text = "SPEL11G";
                //            tb_Source_tableName.Text = "DASISLANDEL.T_Plantitem";

                //            tb_sql_dsource.Text = "IESWKCT247";
                //            tb_sql_dbName.Text = "JK1703_P";
                //            tb_sql_username.Text = "sa";
                //            tb_sql_pwd.Text = "abcde@1234567";
                //            tb_destination_tableName.Text = "plant789elp1.t_Plantitem";

                //            break;

                //        case 6:
                //            rb_oracle.Checked = true;
                //            tb_oracle_dsource.Text = "IESWKCT670";
                //            tb_oracle_dbName.Text = "SPEL11G";
                //            tb_oracle_username.Text = "system";
                //            tb_oracle_pwd.Text = "SPEL11G";
                //            tb_Source_tableName.Text = "tb_loop";

                //            tb_sql_dsource.Text = "IESWKCT247";
                //            tb_sql_dbName.Text = "db_SPELSQL";
                //            tb_sql_username.Text = "sa";
                //            tb_sql_pwd.Text = "abcde@1234567";
                //            tb_destination_tableName.Text = "loop";

                //            break;
                //        case 7:
                //            rb_oracle.Checked = true;
                //            tb_oracle_dsource.Text = "IESWKCT670";
                //            tb_oracle_dbName.Text = "SPEL11G";
                //            tb_oracle_username.Text = "system";
                //            tb_oracle_pwd.Text = "SPEL11G";
                //            tb_Source_tableName.Text = "tb_loop_2k";

                //            tb_sql_dsource.Text = "IESWKCT247";
                //            tb_sql_dbName.Text = "db_SPELSQL";
                //            tb_sql_username.Text = "sa";
                //            tb_sql_pwd.Text = "abcde@1234567";
                //            tb_destination_tableName.Text = "tb_loop_2k";

                //            break;
                //        case 8:
                //            rb_oracle.Checked = true;
                //            tb_oracle_dsource.Text = "IESWKCT670";
                //            tb_oracle_dbName.Text = "SPEL11G";
                //            tb_oracle_username.Text = "system";
                //            tb_oracle_pwd.Text = "SPEL11G";
                //            tb_Source_tableName.Text = "tb_loop_2k";

                //            tb_sql_dsource.Text = "IESWKCT247";
                //            tb_sql_dbName.Text = "db_SPELSQL";
                //            tb_sql_username.Text = "sa";
                //            tb_sql_pwd.Text = "abcde@1234567";
                //            tb_destination_tableName.Text = "tb_loop_2k";

                //            break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                lbl_Oracle_SQL_Connection.Text = ex.Message.ToString();
            }
        }
    }
}
