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

        private DataTable dt_oracle;
        private DataTable dt_sql;
        private bool boolStatus;
        private Exception myException;
        private DataSet ds;
        private bool loading;

        // Keep the workflow's field names while using the common connection presentation.
        private TextBox tb_oracle_dsource => connection.Source.DataSource;
        private TextBox tb_oracle_dbName => connection.Source.Database;
        private TextBox tb_oracle_username => connection.Source.User;
        private TextBox tb_oracle_pwd => connection.Source.Password;
        private TextBox tb_sql_dsource => connection.Target.DataSource;
        private TextBox tb_sql_dbName => connection.Target.Database;
        private TextBox tb_sql_username => connection.Target.User;
        private TextBox tb_sql_pwd => connection.Target.Password;
        private RadioButton rb_SRC_oracle => connection.Source.Oracle;
        private RadioButton rb_SRC_SQL => connection.Source.Sql;

        private const string DefaultSpecificationQuery = @"SELECT
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

        public bool HasLoadedData { get { return boolStatus && ds.Tables.Count == 2; } }
        public event EventHandler DataLoadStateChanged;

        public OracleAndSQLConnection()
        {
            InitializeComponent();
            connection.Target.SqlServerOnly = true;
            // Keep existing automation/control lookup names; accessible captions remain descriptive.
            tb_oracle_dsource.Name = "tb_oracle_dsource";
            tb_oracle_dbName.Name = "tb_oracle_dbName";
            tb_oracle_username.Name = "tb_oracle_username";
            tb_oracle_pwd.Name = "tb_oracle_pwd";
            tb_sql_dsource.Name = "tb_sql_dsource";
            tb_sql_dbName.Name = "tb_sql_dbName";
            tb_sql_username.Name = "tb_sql_username";
            tb_sql_pwd.Name = "tb_sql_pwd";
            lbl_Oracle_SQL_Connection.Text = string.Empty;

            myException = new Exception();
            ds = new DataSet();

            tb_querybox.Text = DefaultSpecificationQuery;
            UseQueryForCheckBoxFunction();
            connection.InputChanged += InvalidateLoadedData;
            connection.ProfileApplied += (sender, details) =>
            {
                tb_Source_tableName.Text = details.str_SRC_TableName;
                tb_destination_tableName.Text = details.str_TRG_TableName;
            };
            connection.Failed += (sender, error) => lbl_Oracle_SQL_Connection.Text = Common_Data.RedactConnectionCredentials(error.Message);
            foreach (TextBox field in new[] { tb_Source_tableName, tb_destination_tableName, tb_querybox })
                field.TextChanged += InvalidateLoadedData;
            cb_query.CheckedChanged += InvalidateLoadedData;
            Disposed += (sender, e) => ds?.Dispose();
            try { new Common_Data().ReadUserProfile(connection.ProfileList); }
            catch (Exception error) { lbl_Oracle_SQL_Connection.Text = Common_Data.RedactConnectionCredentials(error.Message); }
        }

        private void InvalidateLoadedData(object sender, EventArgs e)
        {
            boolStatus = false;
            lbl_Oracle_SQL_Connection.Text = "Connection or query changed. Reload source and target data.";
            DataLoadStateChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void ValidateInput()
        {
            FormValidation.Require(!loading, btn_Connect_Reference_ID, "Wait for the current load to finish.");
            connection.ValidateInput();
            if (cb_query.Checked) FormValidation.Required(tb_querybox, "the query to run on both databases");
            else
            {
                ValidateTableField(tb_Source_tableName, rb_SRC_oracle.Checked);
                ValidateTableField(tb_destination_tableName, false);
            }
        }

        private static void ValidateTableField(TextBox field, bool oracle)
        {
            FormValidation.Required(field, "table name");
            try { ValidateTableName(field.Text, oracle); }
            catch (ArgumentException) { FormValidation.Reject(field, "Enter a valid qualified table name. Use query mode for expressions or database links."); }
        }

        /// <summary>Validates identifiers, not arbitrary SQL. Quoted names keep punctuation as literal name text.</summary>
        internal static string ValidateTableName(string value, bool oracle)
        {
            string text = (value ?? string.Empty).Trim();
            int index = 0;
            int parts = 0;
            while (index < text.Length)
            {
                while (index < text.Length && char.IsWhiteSpace(text[index])) index++;
                if (index == text.Length) throw new ArgumentException("A table name cannot have an empty part.");
                int characters = 0;
                if (text[index] == '"' || (!oracle && text[index] == '['))
                {
                    char close = text[index++] == '[' ? ']' : '"';
                    bool closed = false;
                    while (index < text.Length)
                    {
                        char current = text[index++];
                        if (char.IsControl(current)) throw new ArgumentException("Invalid table name character.");
                        if (current != close) { characters++; continue; }
                        if (index < text.Length && text[index] == close) { index++; characters++; continue; }
                        closed = true;
                        break;
                    }
                    if (!closed) throw new ArgumentException("The quoted table name is incomplete.");
                }
                else
                {
                    if (!(char.IsLetter(text[index]) || text[index] == '_' || (!oracle && text[index] == '#')))
                        throw new ArgumentException("Invalid first table-name character.");
                    while (index < text.Length && (char.IsLetterOrDigit(text[index]) || text[index] == '_' || text[index] == '$' || text[index] == '#'))
                    { index++; characters++; }
                }
                if (characters == 0 || ++parts > (oracle ? 3 : 4)) throw new ArgumentException("Invalid qualified table name.");
                while (index < text.Length && char.IsWhiteSpace(text[index])) index++;
                if (index == text.Length) return text;
                if (text[index++] != '.' || index == text.Length) throw new ArgumentException("Unexpected text after a table name.");
            }
            throw new ArgumentException("Enter a table name.");
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
                    strOracleConnection = ConnectionStrings.Build("Oracle", tb_oracle_dsource.Text.Trim(), tb_oracle_dbName.Text.Trim(),
                        false, tb_oracle_username.Text.Trim(), tb_oracle_pwd.Text, connection.Source.Port.Text);
                    using (var adapter = new OracleDataAdapter(strQuery, strOracleConnection)) adapter.Fill(dt_oracle);
                    strSourceName = "Source Oracle 11G";
                }

                if (rb_SRC_SQL.Checked)
                {
                    strSQLConnection = ConnectionStrings.Build("SQL", tb_oracle_dsource.Text.Trim(), tb_oracle_dbName.Text.Trim(),
                        connection.Source.WindowsLogin.Checked, tb_oracle_username.Text.Trim(), tb_oracle_pwd.Text, null);
                    using (var adapter = new SqlDataAdapter(strQuery, strSQLConnection)) adapter.Fill(dt_oracle);
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
            // Disable the container, not individual fields: platform/authentication enablement survives a load.
            connection.Enabled = boolEnabledCase;
            cb_query.Enabled = boolEnabledCase;
            tb_querybox.Enabled = boolEnabledCase && cb_query.Checked;
            tb_Source_tableName.Enabled = boolEnabledCase && !cb_query.Checked;
            tb_destination_tableName.Enabled = boolEnabledCase && !cb_query.Checked;
            btn_ResetQuery.Enabled = boolEnabledCase && cb_query.Checked;
            btn_Connect_Reference_ID.Enabled = boolEnabledCase;
            if (!(boolEnabledCase))
            {
                lbl_Oracle_SQL_Connection.Text = string.Empty;
            }
        }

        public void Load_DB_Oracle_SQL()
        {
            if (!FormValidation.Check(this, ValidateInput, message => lbl_Oracle_SQL_Connection.Text = message)) return;
            loading = true;
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
                    var candidate = new DataSet();
                    candidate.Tables.Add(dt_oracle);
                    candidate.Tables.Add(dt_sql);
                    try
                    {
                        foreach(DataTable dt_ in candidate.Tables)
                        {
                            foreach(DataColumn dc_ in dt_.Columns)
                            {
                                foreach(DataRow dr_ in dt_.Rows)
                                {
                                    if (dc_.DataType == typeof(string) && dr_[dc_] != DBNull.Value)
                                        dr_[dc_] = dr_[dc_].ToString().Trim();
                                }
                            }
                        }
                        DataSet previous = ds;
                        ds = candidate;
                        previous.Dispose();
                    }
                    catch { candidate.Dispose(); throw; }
                    lbl_Oracle_SQL_Connection.Text = "All data table loaded";
                }
                else
                {
                    lbl_Oracle_SQL_Connection.Text = Common_Data.RedactConnectionCredentials("Data was not loaded: " + myException.Message);
                }

            }
            catch (Exception ex)
            {
                lbl_Oracle_SQL_Connection.Text = Common_Data.RedactConnectionCredentials(ex.Message);
                boolStatus = false;
            }
            finally
            {
                loading = false;
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
                lbl_Oracle_SQL_Connection.Text = Common_Data.RedactConnectionCredentials(ex.Message);
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

                strSQLConnection = ConnectionStrings.Build("SQL", tb_sql_dsource.Text.Trim(), tb_sql_dbName.Text.Trim(),
                    connection.Target.WindowsLogin.Checked, tb_sql_username.Text.Trim(), tb_sql_pwd.Text, null);

                dt_sql = new DataTable();
                using (var adapter = new SqlDataAdapter(strQuery, strSQLConnection)) adapter.Fill(dt_sql);
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
            // Switching modes must not erase either draft. Resetting the template is explicit.
            tb_querybox.Enabled = !loading && cb_query.Checked;
            btn_ResetQuery.Enabled = !loading && cb_query.Checked;
            tb_Source_tableName.Enabled = !loading && !cb_query.Checked;
            tb_destination_tableName.Enabled = !loading && !cb_query.Checked;
        }

        private void btn_ResetQuery_Click(object sender, EventArgs e)
        {
            tb_querybox.Text = DefaultSpecificationQuery;
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

    }
}
