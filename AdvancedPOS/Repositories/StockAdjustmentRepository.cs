using System;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class StockAdjustmentRepository
    {
        // ==========================================
        // 1. Stock Adjustment එකක් Save කිරීම (Transaction Safe)
        //    - StockAdjustments table එකට Log එකක් Insert කරනවා
        //    - Products table එකේ StockQuantity එක Update කරනවා
        //    - දෙකම එකවර Success වෙන්න ඕන, නැත්නම් දෙකම Rollback වෙනවා
        // ==========================================
        public bool SaveAdjustment(StockAdjustment adjustment)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. StockAdjustments Table එකට Log එක දැමීම
                    string insertQuery = @"INSERT INTO StockAdjustments 
                                           (ProductID, AdjustmentType, QuantityChanged, Reason, AdjustmentDate, UserID)
                                           VALUES (@productId, @type, @qty, @reason, GETDATE(), @userId)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@productId", adjustment.ProductID);
                        cmd.Parameters.AddWithValue("@type", adjustment.AdjustmentType);
                        cmd.Parameters.AddWithValue("@qty", adjustment.QuantityChanged);
                        cmd.Parameters.AddWithValue("@reason", (object)adjustment.Reason ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@userId", adjustment.UserID);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Products Table එකේ Stock එක Update කිරීම
                    string updateQuery = @"UPDATE Products 
                                           SET StockQuantity = StockQuantity + @qty
                                           WHERE ProductID = @productId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@qty", adjustment.QuantityChanged);
                        cmd.Parameters.AddWithValue("@productId", adjustment.ProductID);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // ==========================================
        // 2. Adjustment History එක ලබා ගැනීම
        // ==========================================
        public DataTable GetAdjustmentHistory(DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                DateTime endOfDay = toDate.Date.AddDays(1).AddTicks(-1);

                string query = @"SELECT 
                                    sa.AdjustmentID AS [ID],
                                    p.ProductName AS [Product],
                                    sa.AdjustmentType AS [Type],
                                    sa.QuantityChanged AS [Qty Change],
                                    sa.Reason AS [Reason],
                                    sa.AdjustmentDate AS [Date]
                                 FROM StockAdjustments sa
                                 INNER JOIN Products p ON sa.ProductID = p.ProductID
                                 WHERE sa.AdjustmentDate BETWEEN @from AND @to
                                 ORDER BY sa.AdjustmentDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@from", fromDate.Date);
                    cmd.Parameters.AddWithValue("@to", endOfDay);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // ==========================================
        // 3. Current Stock එක ලබා ගැනීම (Product එකක්)
        // ==========================================
        public int GetCurrentStock(int productId)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT StockQuantity FROM Products WHERE ProductID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", productId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
    }
}
