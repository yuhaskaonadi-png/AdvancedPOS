using System;
using System.Data.SqlClient;
using System.Configuration;

namespace AdvancedPOS.Helpers
{
    public class DatabaseConnection
    {
        
        public static SqlConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["POS_DB_Connection"].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}