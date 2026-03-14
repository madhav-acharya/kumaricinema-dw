using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;

namespace Oracle.ManagedDataAccess.Client
{
    public static class OracleParameterCollectionExtensions
    {
        public static OracleParameter AddWithValue(this OracleParameterCollection parameters, string parameterName, object value)
        {
            var parameter = new OracleParameter(parameterName, value ?? DBNull.Value);
            parameters.Add(parameter);
            return parameter;
        }
    }
}

namespace KumariCinema.Database
{
    public class OracleDatabaseConnection
    {
        /// <summary>
        /// Attempts to open a connection to the Oracle database and returns a message indicating success or error.
        /// </summary>
        /// <returns>String message about connection status</returns>
        public static string ConnectDb()
        {
            string connString = ConfigurationManager.ConnectionStrings["OracleConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connString))
                return "Connection string not found in Web.config.";

            try
            {
                using (var conn = new OracleConnection(connString))
                {
                    conn.Open();
                    return $"Successfully connected to Oracle database.";
                }
            }
            catch (OracleException ex)
            {
                // Handle Oracle-specific errors
                return $"Oracle Database Error: {ex.Number} - {ex.Message}";
            }
            catch (Exception ex)
            {
                // Handle general errors
                return $"Application Error: {ex.Message}";
            }
        }
    }
}