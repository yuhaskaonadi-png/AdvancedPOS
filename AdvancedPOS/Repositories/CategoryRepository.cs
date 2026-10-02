using System;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class CategoryRepository
    {
        // Category එකක් අලුතින් Database එකට Save කිරීම (Insert)
        public bool AddCategory(Category category)
        {
            bool isSuccess = false;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "INSERT INTO Categories (CategoryName, Description) VALUES (@name, @desc)"; 
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", category.CategoryName);
                cmd.Parameters.AddWithValue("@desc", category.Description);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }
        // Categories ඔක්කොම Database එකෙන් අරගෙන එන Method එක
        public System.Data.DataTable GetCategories()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT CategoryID, CategoryName, Description FROM Categories";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
        // Category එකක් Update කිරීම
        public bool UpdateCategory(Category category)
        {
            bool isSuccess = false;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // ID එක පදනම් කරගෙන Name එකයි Description එකයි වෙනස් කරනවා
                string query = "UPDATE Categories SET CategoryName = @name, Description = @desc WHERE CategoryID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", category.CategoryName);
                cmd.Parameters.AddWithValue("@desc", category.Description);
                cmd.Parameters.AddWithValue("@id", category.CategoryID); // ID එක අනිවාර්යයි

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }
        // Category එකක් Database එකෙන් මකා දැමීම (Delete)
        public bool DeleteCategory(int id)
        {
            bool isSuccess = false;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // ID එක පදනම් කරගෙන අදාළ පේළිය මකා දමනවා
                string query = "DELETE FROM Categories WHERE CategoryID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }
    }
}