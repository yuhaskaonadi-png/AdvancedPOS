using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class HeldSaleRepository
    {
        // ==========================================
        // 1. Cart එකක් Hold කිරීම (Transaction Safe)
        // ==========================================
        public bool HoldSale(HeldSale heldSale)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. HeldSales Table එකට Header එක Insert කිරීම
                    string insertHeader = @"INSERT INTO HeldSales (Note, Discount, HeldDate, UserID)
                                            OUTPUT INSERTED.HeldSaleID
                                            VALUES (@note, @discount, GETDATE(), @userId)";

                    int newHeldSaleId;
                    using (SqlCommand cmd = new SqlCommand(insertHeader, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@note", (object)heldSale.Note ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@discount", heldSale.Discount);
                        cmd.Parameters.AddWithValue("@userId", heldSale.UserID);
                        newHeldSaleId = (int)cmd.ExecuteScalar();
                    }

                    // 2. HeldSaleItems Table එකට Cart Items ටික Insert කිරීම
                    string insertItem = @"INSERT INTO HeldSaleItems 
                                          (HeldSaleID, ProductID, ProductName, Price, Qty, Total)
                                          VALUES (@heldId, @pid, @pname, @price, @qty, @total)";

                    foreach (var item in heldSale.Items)
                    {
                        using (SqlCommand cmd = new SqlCommand(insertItem, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@heldId", newHeldSaleId);
                            cmd.Parameters.AddWithValue("@pid", item.ProductID);
                            cmd.Parameters.AddWithValue("@pname", item.ProductName);
                            cmd.Parameters.AddWithValue("@price", item.Price);
                            cmd.Parameters.AddWithValue("@qty", item.Qty);
                            cmd.Parameters.AddWithValue("@total", item.Total);
                            cmd.ExecuteNonQuery();
                        }
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
        // 2. සියලුම Held Sales ලයිස්තුව ලබා ගැනීම
        // ==========================================
        public DataTable GetAllHeldSales()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT 
                                    h.HeldSaleID AS [ID],
                                    ISNULL(h.Note, 'No Note') AS [Note],
                                    COUNT(i.HeldSaleItemID) AS [Items],
                                    SUM(i.Total) - h.Discount AS [Total],
                                    h.HeldDate AS [Held At]
                                 FROM HeldSales h
                                 INNER JOIN HeldSaleItems i ON h.HeldSaleID = i.HeldSaleID
                                 GROUP BY h.HeldSaleID, h.Note, h.Discount, h.HeldDate
                                 ORDER BY h.HeldDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // ==========================================
        // 3. Held Sale එකක සම්පූර්ණ විස්තර ලබා ගැනීම (Load කිරීමට)
        // ==========================================
        public HeldSale GetHeldSaleById(int heldSaleId)
        {
            HeldSale heldSale = null;

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // 1. Header එක ගැනීම
                string headerQuery = "SELECT * FROM HeldSales WHERE HeldSaleID = @id";
                using (SqlCommand cmd = new SqlCommand(headerQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", heldSaleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            heldSale = new HeldSale
                            {
                                HeldSaleID = Convert.ToInt32(reader["HeldSaleID"]),
                                Note = reader["Note"] != DBNull.Value ? reader["Note"].ToString() : "",
                                Discount = Convert.ToDecimal(reader["Discount"]),
                                HeldDate = Convert.ToDateTime(reader["HeldDate"]),
                                UserID = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : 0
                            };
                        }
                    }
                }

                if (heldSale == null) return null;

                // 2. Items ටික ගැනීම
                string itemsQuery = "SELECT * FROM HeldSaleItems WHERE HeldSaleID = @id";
                using (SqlCommand cmd = new SqlCommand(itemsQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", heldSaleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            heldSale.Items.Add(new HeldSaleItem
                            {
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                ProductName = reader["ProductName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Qty = Convert.ToInt32(reader["Qty"]),
                                Total = Convert.ToDecimal(reader["Total"])
                            });
                        }
                    }
                }
            }

            return heldSale;
        }

        // ==========================================
        // 4. Held Sale එකක් Delete කිරීම (Load කරාට පස්සේ, හෝ Cancel කරද්දී)
        // ==========================================
        public bool DeleteHeldSale(int heldSaleId)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // HeldSaleItems ON DELETE CASCADE නිසා ඒවා ඉබේම Delete වෙනවා
                string query = "DELETE FROM HeldSales WHERE HeldSaleID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", heldSaleId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
