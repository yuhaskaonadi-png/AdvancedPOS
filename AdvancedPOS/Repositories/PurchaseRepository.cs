using System;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class PurchaseRepository
    {
        /// <summary>
        /// සම්පූර්ණ Purchase බිල්පත සහ Stock යාවත්කාලීන කිරීම Transaction එකක් ලෙස සිදු කරයි.
        /// </summary>
        public bool SavePurchaseTransaction(Purchase purchase)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Purchases Table එකට ඇතුළත් කිරීම
                        string purchaseQuery = @"INSERT INTO Purchases (SupplierID, PurchaseDate, TotalAmount, UserID) 
                                         VALUES (@supplierId, @date, @total, @userId);
                                         SELECT SCOPE_IDENTITY();";

                        int purchaseId;
                        using (SqlCommand cmdPurchase = new SqlCommand(purchaseQuery, conn, transaction))
                        {
                            cmdPurchase.Parameters.AddWithValue("@supplierId", purchase.SupplierID);
                            cmdPurchase.Parameters.AddWithValue("@date", purchase.PurchaseDate == default ? DateTime.Now : purchase.PurchaseDate);
                            cmdPurchase.Parameters.AddWithValue("@total", purchase.TotalAmount);
                            cmdPurchase.Parameters.AddWithValue("@userId", purchase.UserID);

                            purchaseId = Convert.ToInt32(cmdPurchase.ExecuteScalar());
                        }

                        // 2. Details එකතු කිරීම සහ StockQuantity වැඩි කිරීම
                        string detailQuery = @"INSERT INTO PurchaseDetails (PurchaseID, ProductID, Quantity, CostPrice, LineTotal) 
                                       VALUES (@purchaseId, @productId, @qty, @cost, @lineTotal);";

                        string updateStockQuery = @"UPDATE Products 
                                           SET StockQuantity = StockQuantity + @qty 
                                           WHERE ProductID = @productId;";

                        foreach (var item in purchase.Details)
                        {
                            using (SqlCommand cmdDetail = new SqlCommand(detailQuery, conn, transaction))
                            {
                                cmdDetail.Parameters.AddWithValue("@purchaseId", purchaseId);
                                cmdDetail.Parameters.AddWithValue("@productId", item.ProductID);
                                cmdDetail.Parameters.AddWithValue("@qty", item.Quantity);
                                cmdDetail.Parameters.AddWithValue("@cost", item.CostPrice);
                                cmdDetail.Parameters.AddWithValue("@lineTotal", item.LineTotal);
                                cmdDetail.ExecuteNonQuery();
                            }

                            using (SqlCommand cmdStock = new SqlCommand(updateStockQuery, conn, transaction))
                            {
                                cmdStock.Parameters.AddWithValue("@qty", item.Quantity);
                                cmdStock.Parameters.AddWithValue("@productId", item.ProductID);
                                cmdStock.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Transaction එක තාමත් සක්‍රියව පවතී නම් පමණක් Rollback කරයි (දෙවනුව Error ඒම වළක්වයි)
                        if (transaction?.Connection != null)
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch
                            {
                                // Rollback එක අසාර්ථක වුවද මුල් SQL Error එක පෙන්වීම සඳහා මෙය හිස්ව තබයි
                            }
                        }

                        // සැබෑ SQL Error එක විසි කරයි
                        throw new Exception("Database Error: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// මීට පෙර සිදුකළ සියලුම Purchase බිල්පත් ඉතිහාසය ලබා ගැනීම
        /// </summary>
        public DataTable GetAllPurchases()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT p.PurchaseID, s.SupplierName, p.PurchaseDate, p.TotalAmount, u.Username AS ReceivedBy
                                 FROM Purchases p
                                 INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
                                 INNER JOIN Users u ON p.UserID = u.UserID
                                 ORDER BY p.PurchaseDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}