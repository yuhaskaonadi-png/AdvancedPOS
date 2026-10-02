using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class PromotionRepository
    {
        // ==========================================
        // 1. Product එකකට Active Promotion එකක් තියෙනවද බැලීම
        // ==========================================
        public Promotion GetActivePromotion(int productId)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT TOP 1 * FROM Promotions 
                                 WHERE ProductID = @pid AND IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", productId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Promotion
                            {
                                PromotionID = Convert.ToInt32(reader["PromotionID"]),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                BuyQty = Convert.ToInt32(reader["BuyQty"]),
                                FreeQty = Convert.ToInt32(reader["FreeQty"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        // ==========================================
        // 2. සියලුම Promotions ලබා ගැනීම (Management Screen සඳහා)
        // ==========================================
        public DataTable GetAllPromotions()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT pr.PromotionID AS [ID], p.ProductName AS [Product],
                                        pr.BuyQty AS [Buy Qty], pr.FreeQty AS [Free Qty],
                                        pr.IsActive AS [Active]
                                 FROM Promotions pr
                                 INNER JOIN Products p ON pr.ProductID = p.ProductID
                                 ORDER BY p.ProductName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // ==========================================
        // 3. Promotion එකක් Add කිරීම
        // ==========================================
        public bool AddPromotion(Promotion promo)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"INSERT INTO Promotions (ProductID, BuyQty, FreeQty, IsActive)
                                 VALUES (@pid, @buy, @free, 1)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@pid", promo.ProductID);
                    cmd.Parameters.AddWithValue("@buy", promo.BuyQty);
                    cmd.Parameters.AddWithValue("@free", promo.FreeQty);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ==========================================
        // 4. Promotion එකක් Delete කිරීම
        // ==========================================
        public bool DeletePromotion(int promotionId)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "DELETE FROM Promotions WHERE PromotionID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", promotionId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
