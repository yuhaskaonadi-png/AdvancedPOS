using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class ExpenseRepository
    {
        // =========================================================
        // 1. Expense Categories (වියදම් වර්ග කළමනාකරණය)
        // =========================================================

        // නව වියදම් වර්ගයක් ඇතුළත් කිරීම
        public bool AddCategory(ExpenseCategory category)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "INSERT INTO ExpenseCategories (CategoryName, Description) VALUES (@name, @desc)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", category.CategoryName);
                    cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(category.Description) ? (object)DBNull.Value : category.Description);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        // සියලුම වියදම් වර්ග ලබා ගැනීම (Dropdown එකට දැමීමට)
        public List<ExpenseCategory> GetAllCategories()
        {
            List<ExpenseCategory> categories = new List<ExpenseCategory>();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT * FROM ExpenseCategories ORDER BY CategoryName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new ExpenseCategory
                            {
                                CategoryID = Convert.ToInt32(reader["CategoryID"]),
                                CategoryName = reader["CategoryName"].ToString(),
                                Description = reader["Description"].ToString()
                            });
                        }
                    }
                }
            }
            return categories;
        }

        // =========================================================
        // 2. Expenses (දෛනික වියදම් කළමනාකරණය)
        // =========================================================

        // නව වියදමක් ඇතුළත් කිරීම
        public bool AddExpense(Expense expense)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"INSERT INTO Expenses (CategoryID, Amount, ExpenseDate, Description, UserID) 
                                 VALUES (@catId, @amount, @date, @desc, @userId)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@catId", expense.CategoryID);
                    cmd.Parameters.AddWithValue("@amount", expense.Amount);
                    cmd.Parameters.AddWithValue("@date", expense.ExpenseDate);
                    cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(expense.Description) ? (object)DBNull.Value : expense.Description);
                    cmd.Parameters.AddWithValue("@userId", expense.UserID == 0 ? (object)DBNull.Value : expense.UserID);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        // වියදමක් යාවත්කාලීන කිරීම (Edit)
        public bool UpdateExpense(Expense expense)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"UPDATE Expenses 
                                 SET CategoryID = @catId, Amount = @amount, ExpenseDate = @date, Description = @desc 
                                 WHERE ExpenseID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", expense.ExpenseID);
                    cmd.Parameters.AddWithValue("@catId", expense.CategoryID);
                    cmd.Parameters.AddWithValue("@amount", expense.Amount);
                    cmd.Parameters.AddWithValue("@date", expense.ExpenseDate);
                    cmd.Parameters.AddWithValue("@desc", string.IsNullOrEmpty(expense.Description) ? (object)DBNull.Value : expense.Description);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        // වියදමක් මකා දැමීම (Delete)
        public bool DeleteExpense(int expenseId)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Expenses WHERE ExpenseID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", expenseId);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        // දිනයක් හෝ දින පරාසයක් අනුව වියදම් ලැයිස්තුව ලබා ගැනීම (Data Grid එකට පෙන්වීමට)
        public DataTable GetExpensesHistory(DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // දවසේ අවසානය දක්වා Filter වීමට කාලය සකසයි
                toDate = toDate.Date.AddDays(1).AddTicks(-1);

                // JOIN මඟින් අදාළ Category Name එක ලබා ගනී
                string query = @"SELECT e.ExpenseID, c.CategoryName, e.Amount, e.ExpenseDate, e.Description, e.CategoryID 
                                 FROM Expenses e
                                 INNER JOIN ExpenseCategories c ON e.CategoryID = c.CategoryID
                                 WHERE e.ExpenseDate BETWEEN @from AND @to
                                 ORDER BY e.ExpenseDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@from", fromDate);
                    cmd.Parameters.AddWithValue("@to", toDate);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}