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

        public bool HasLoadedData { get { return boolStatus && ds.Tables.Count == 2; } }
        public event EventHandler DataLoadStateChanged;

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
                boolStatus = false;
                DataLoadStateChanged?.Invoke(this, EventArgs.Empty);
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
                DataLoadStateChanged?.Invoke(this, EventArgs.Empty);
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


    }
}
