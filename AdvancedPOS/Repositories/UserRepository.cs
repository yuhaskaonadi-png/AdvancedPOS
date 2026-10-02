using System;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;
using System.Data;

namespace AdvancedPOS.Repositories
{
    public class UserRepository
    {
        // Login වෙන කෙනාගේ විස්තර පරීක්ෂා කරන Method එක
        public User AuthenticateUser(string username, string password)
        {
            User user = null;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // SQL Injection වලින් බේරෙන්න Parameters පාවිච්චි කරලා තියෙන්නේ
                string query = "SELECT UserID, Username, Role FROM Users WHERE Username = @user AND Password = @pass";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // Username සහ Password හරියටම මැච් වුණොත් විතරක් දත්ත ගන්නවා
                if (reader.Read())
                {
                    user = new User
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        Username = reader["Username"].ToString(),
                        Role = reader["Role"].ToString()
                    };
                }
            }
            // වැරදි නම් null රිටන් වෙනවා (එතකොට අපිට Error Message එකක් පෙන්වන්න පුළුවන්)
            return user;
        }
        // සියලුම Users ලා බලාගන්න
        public DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT UserID, Username, Role FROM Users";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // අලුත් කෙනෙක් දාන්න
        public bool AddUser(User user)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "INSERT INTO Users (Username, Password, Role) VALUES (@u, @p, @r)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@u", user.Username);
                cmd.Parameters.AddWithValue("@p", user.Password);
                cmd.Parameters.AddWithValue("@r", user.Role);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // කෙනෙක්ව මකන්න
        public bool DeleteUser(int userId)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Users WHERE UserID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

    }
}