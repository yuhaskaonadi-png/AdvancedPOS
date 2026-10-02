using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class SaleRepository
    {
        // =========================================================
        // 1. Transaction Save කිරීම (නව බිල්පතක් සැකසීම සහ තොග අඩු කිරීම)
        // =========================================================
        public bool SaveTransaction(Sale sale, List<SaleDetail> saleDetails, List<SalePayment> payments)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Sales Table එකට මූලික විස්තර දැමීම
                        string saleQuery = @"INSERT INTO Sales (SubTotal, Discount, GrandTotal, PaidAmount, ChangeAmount, PaymentStatus, UserID) 
                                             VALUES (@sub, @disc, @grand, @paid, @change, @status, @uid);
                                             SELECT SCOPE_IDENTITY();";

                        int newSaleId;
                        using (SqlCommand cmdSale = new SqlCommand(saleQuery, conn, transaction))
                        {
                            cmdSale.Parameters.AddWithValue("@sub", sale.SubTotal);
                            cmdSale.Parameters.AddWithValue("@disc", sale.Discount);
                            cmdSale.Parameters.AddWithValue("@grand", sale.GrandTotal);
                            cmdSale.Parameters.AddWithValue("@paid", sale.PaidAmount);
                            cmdSale.Parameters.AddWithValue("@change", sale.ChangeAmount);
                            cmdSale.Parameters.AddWithValue("@status", string.IsNullOrEmpty(sale.PaymentStatus) ? "Paid" : sale.PaymentStatus);
                            cmdSale.Parameters.AddWithValue("@uid", sale.UserID);

                            newSaleId = Convert.ToInt32(cmdSale.ExecuteScalar());
                        }

                        // 2. SaleDetails එකට දැමීම සහ තොගය (Stock) අඩු කිරීම
                        string detailQuery = @"INSERT INTO SaleDetails (SaleID, ProductID, UnitPrice, Quantity, TotalPrice) 
                                               VALUES (@sid, @pid, @price, @qty, @total);";

                        string stockQuery = @"UPDATE Products 
                                              SET StockQuantity = StockQuantity - @qty 
                                              WHERE ProductID = @pid;";

                        foreach (var detail in saleDetails)
                        {
                            using (SqlCommand cmdDetail = new SqlCommand(detailQuery, conn, transaction))
                            {
                                cmdDetail.Parameters.AddWithValue("@sid", newSaleId);
                                cmdDetail.Parameters.AddWithValue("@pid", detail.ProductID);
                                cmdDetail.Parameters.AddWithValue("@price", detail.UnitPrice);
                                cmdDetail.Parameters.AddWithValue("@qty", detail.Quantity);
                                cmdDetail.Parameters.AddWithValue("@total", detail.TotalPrice);
                                cmdDetail.ExecuteNonQuery();
                            }

                            using (SqlCommand cmdStock = new SqlCommand(stockQuery, conn, transaction))
                            {
                                cmdStock.Parameters.AddWithValue("@qty", detail.Quantity);
                                cmdStock.Parameters.AddWithValue("@pid", detail.ProductID);
                                cmdStock.ExecuteNonQuery();
                            }
                        }

                        // 3. ගෙවීම් ක්‍රම (Payments) ඇතුළත් කිරීම
                        if (payments != null && payments.Count > 0)
                        {
                            string paymentQuery = @"INSERT INTO SalePayments (SaleID, PaymentMethod, Amount, ReferenceNumber) 
                                                    VALUES (@sid, @method, @amount, @ref);";

                            foreach (var pay in payments)
                            {
                                using (SqlCommand cmdPay = new SqlCommand(paymentQuery, conn, transaction))
                                {
                                    cmdPay.Parameters.AddWithValue("@sid", newSaleId);
                                    cmdPay.Parameters.AddWithValue("@method", pay.PaymentMethod);
                                    cmdPay.Parameters.AddWithValue("@amount", pay.Amount);
                                    cmdPay.Parameters.AddWithValue("@ref", (object)pay.ReferenceNumber ?? DBNull.Value);
                                    cmdPay.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        if (transaction?.Connection != null)
                        {
                            try { transaction.Rollback(); } catch { }
                        }
                        throw new Exception("Sale Transaction Failed: " + ex.Message);
                    }
                }
            }
        }

        // =========================================================
        // 2. Invoice History සඳහා අදාළ Methods
        // =========================================================

        public DataTable GetSalesHistory(DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                toDate = toDate.Date.AddDays(1).AddTicks(-1);

                string query = @"SELECT SaleID, SaleDate, SubTotal, Discount, GrandTotal, PaidAmount, PaymentStatus 
                                 FROM Sales 
                                 WHERE SaleDate BETWEEN @from AND @to 
                                 ORDER BY SaleID DESC";

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

        public Sale GetSaleByID(int saleId)
        {
            Sale sale = new Sale();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM Sales WHERE SaleID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", saleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sale.SaleID = Convert.ToInt32(reader["SaleID"]);
                            sale.SaleDate = Convert.ToDateTime(reader["SaleDate"]);
                            sale.SubTotal = Convert.ToDecimal(reader["SubTotal"]);
                            sale.Discount = Convert.ToDecimal(reader["Discount"]);
                            sale.GrandTotal = Convert.ToDecimal(reader["GrandTotal"]);
                            sale.PaidAmount = Convert.ToDecimal(reader["PaidAmount"]);
                            sale.ChangeAmount = Convert.ToDecimal(reader["ChangeAmount"]);
                            sale.PaymentStatus = reader["PaymentStatus"].ToString();
                        }
                    }
                }
            }
            return sale;
        }

        public List<SaleDetail> GetSaleDetails(int saleId)
        {
            List<SaleDetail> details = new List<SaleDetail>();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT sd.*, p.ProductName 
                                 FROM SaleDetails sd
                                 INNER JOIN Products p ON sd.ProductID = p.ProductID
                                 WHERE sd.SaleID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", saleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            details.Add(new SaleDetail
                            {
                                ProductName = reader["ProductName"].ToString(),
                                ProductID = Convert.ToInt32(reader["ProductID"]),
                                UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),

                                // අලුත් ReturnedQty එක ලබා ගැනීම (null නම් 0 ලෙස ගනී)
                                ReturnedQty = reader["ReturnedQty"] != DBNull.Value ? Convert.ToInt32(reader["ReturnedQty"]) : 0
                            });
                        }
                    }
                }
            }
            return details;
        }

        public List<SalePayment> GetSalePayments(int saleId)
        {
            List<SalePayment> payments = new List<SalePayment>();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM SalePayments WHERE SaleID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", saleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(new SalePayment
                            {
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                Amount = Convert.ToDecimal(reader["Amount"])
                            });
                        }
                    }
                }
            }
            return payments;
        }

        // =========================================================
        // 3. Returns & Refunds (භාණ්ඩ ආපසු භාරගැනීම සහ තොගය වැඩි කිරීම)
        // =========================================================
        public bool ProcessReturn(SaleReturn returnData)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. SaleReturns Table එකට වාර්තාවක් දැමීම
                        string insertReturnQuery = @"INSERT INTO SaleReturns (SaleID, ProductID, Quantity, RefundAmount, Reason) 
                                                     VALUES (@sid, @pid, @qty, @refund, @reason)";
                        using (SqlCommand cmd = new SqlCommand(insertReturnQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@sid", returnData.SaleID);
                            cmd.Parameters.AddWithValue("@pid", returnData.ProductID);
                            cmd.Parameters.AddWithValue("@qty", returnData.Quantity);
                            cmd.Parameters.AddWithValue("@refund", returnData.RefundAmount);
                            cmd.Parameters.AddWithValue("@reason", string.IsNullOrEmpty(returnData.Reason) ? (object)DBNull.Value : returnData.Reason);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. SaleDetails Table එකේ ReturnedQty එක Update කිරීම (+ කිරීම)
                        string updateDetailsQuery = @"UPDATE SaleDetails 
                                                      SET ReturnedQty = ReturnedQty + @qty 
                                                      WHERE SaleID = @sid AND ProductID = @pid";
                        using (SqlCommand cmd = new SqlCommand(updateDetailsQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@qty", returnData.Quantity);
                            cmd.Parameters.AddWithValue("@sid", returnData.SaleID);
                            cmd.Parameters.AddWithValue("@pid", returnData.ProductID);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Products Table එකේ තොගය (Stock) නැවත වැඩි කිරීම (+)
                        string updateStockQuery = @"UPDATE Products 
                                                    SET StockQuantity = StockQuantity + @qty 
                                                    WHERE ProductID = @pid";
                        using (SqlCommand cmd = new SqlCommand(updateStockQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@qty", returnData.Quantity);
                            cmd.Parameters.AddWithValue("@pid", returnData.ProductID);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        if (transaction?.Connection != null)
                        {
                            try { transaction.Rollback(); } catch { }
                        }
                        throw new Exception("Return Processing Failed: " + ex.Message);
                    }
                }
            }
        }
    }
}