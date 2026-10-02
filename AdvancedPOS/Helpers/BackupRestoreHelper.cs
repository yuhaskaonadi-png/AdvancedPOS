using System;
using System.Data.SqlClient;

namespace AdvancedPOS.Helpers
{
    public static class BackupRestoreHelper
    {
        // ==========================================
        // 1. Database එක Backup කිරීම (.bak file එකක් හදනවා)
        // ==========================================
        public static void BackupDatabase(string filePath)
        {
            string connString = DatabaseConnection.GetConnection().ConnectionString;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connString);
            string dbName = builder.InitialCatalog;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = $@"BACKUP DATABASE [{dbName}] 
                                  TO DISK = @path 
                                  WITH FORMAT, INIT, 
                                  NAME = 'Full Backup of {dbName}', 
                                  SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = 300; // විශාල Database වලට කාලය දෙනවා (5 minutes)
                    cmd.Parameters.AddWithValue("@path", filePath);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ==========================================
        // 2. Database එක Restore කිරීම (.bak file එකකින්)
        // ==========================================
        public static void RestoreDatabase(string filePath)
        {
            string connString = DatabaseConnection.GetConnection().ConnectionString;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connString);
            string dbName = builder.InitialCatalog;

            // Restore කරන්න කලින්, 'master' Database එකට Connect වෙන්න ඕන
            // (මොකද Restore කරන Database එකටම Connect වෙලා ඉන්නකොට එය Restore කරන්න බැහැ)
            builder.InitialCatalog = "master";

            using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();

                // 1. Database එකට වෙනත් කිසිවෙකුට Access වීම වළක්වා, තනි පරිශීලකයෙකුට පමණක් සීමා කිරීම
                string singleUserQuery = $@"ALTER DATABASE [{dbName}] 
                                            SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                using (SqlCommand cmd = new SqlCommand(singleUserQuery, conn))
                {
                    cmd.CommandTimeout = 60;
                    cmd.ExecuteNonQuery();
                }

                // 2. Restore කිරීම
                string restoreQuery = $@"RESTORE DATABASE [{dbName}] 
                                         FROM DISK = @path 
                                         WITH REPLACE, STATS = 10";
                using (SqlCommand cmd = new SqlCommand(restoreQuery, conn))
                {
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@path", filePath);
                    cmd.ExecuteNonQuery();
                }

                // 3. නැවත සාමාන්‍ය (Multi-User) තත්ත්වයට හැරවීම
                string multiUserQuery = $@"ALTER DATABASE [{dbName}] SET MULTI_USER";
                using (SqlCommand cmd = new SqlCommand(multiUserQuery, conn))
                {
                    cmd.CommandTimeout = 60;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}