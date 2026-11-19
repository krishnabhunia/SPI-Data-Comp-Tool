using System;
using System.Data.SqlClient;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;

namespace SPI_Data_Comp_Tool
{
    /// <summary>Constructs connection strings from values, never by appending credential syntax.</summary>
    internal static class ConnectionStrings
    {
        internal static string Build(string type, string server, string database, bool windowsAuthentication,
            string user, string password, string oraclePort)
        {
            bool oracle = string.Equals(type, "oracle", StringComparison.OrdinalIgnoreCase);
            bool sql = string.Equals(type, "sql", StringComparison.OrdinalIgnoreCase);
            if (!oracle && !sql) throw new ArgumentException("Select Oracle or SQL Server before connecting.");
            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database))
                throw new ArgumentException("Enter the database data source and database or service name.");
            foreach (string value in windowsAuthentication ? new[] { server, database } : new[] { server, database, user })
                if (ConnectionSide.ContainsControlCharacters(value))
                    throw new ArgumentException("Connection names cannot contain line breaks or control characters.");
            if (oracle && windowsAuthentication)
                throw new ArgumentException("Windows authentication is available only for SQL Server.");
            if (!windowsAuthentication && (string.IsNullOrWhiteSpace(user) || string.IsNullOrEmpty(password)))
                throw new ArgumentException("Enter the database username and password.");

            if (sql)
            {
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = server, InitialCatalog = database, IntegratedSecurity = windowsAuthentication,
                };
                if (!windowsAuthentication) { builder.UserID = user; builder.Password = password; }
                return builder.ConnectionString;
            }

            int port;
            if (!int.TryParse((oraclePort ?? string.Empty).Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out port)
                || port < 1 || port > 65535)
                throw new ArgumentException("Oracle port must be a whole number from 1 to 65535.");
            if (server.IndexOfAny(new[] { '(', ')', '=' }) >= 0 || database.IndexOfAny(new[] { '(', ')', '=' }) >= 0)
                throw new ArgumentException("Enter an Oracle host and service name, not a connection descriptor.");
            var oracleBuilder = new OracleConnectionStringBuilder
            {
                DataSource = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + server + ")(PORT="
                    + port.ToString(CultureInfo.InvariantCulture) + "))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME="
                    + database + ")))",
                UserID = user,
                Password = password,
            };
            return oracleBuilder.ConnectionString;
        }
    }
}
