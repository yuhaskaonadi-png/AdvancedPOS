using System;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;
using AdvancedPOS.Models;

namespace AdvancedPOS.Repositories
{
    public class DashboardRepository
    {
        public DashboardSummary GetSummaryForToday()
        {
            DashboardSummary summary = new DashboardSummary();
            DateTime today = DateTime.Today;
            DateTime endOfDay = today.AddDays(1).AddTicks(-1);

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // 1. අද දවසේ මුළු ආදායම සහ බිල්පත් ගණන
                string salesQuery = @"SELECT ISNULL(SUM(GrandTotal), 0) AS TotalSales, 
                                             COUNT(SaleID) AS InvoiceCount 
                                      FROM Sales 
                                      WHERE SaleDate BETWEEN @start AND @end";
                using (SqlCommand cmd = new SqlCommand(salesQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@start", today);
                    cmd.Parameters.AddWithValue("@end", endOfDay);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.TodaySales = Convert.ToDecimal(reader["TotalSales"]);
                            summary.TodayInvoices = Convert.ToInt32(reader["InvoiceCount"]);
                        }
                    }
                }

                // 2. අද දවසේ මුළු වියදම
                string expenseQuery = @"SELECT ISNULL(SUM(Amount), 0) AS TotalExp 
                                        FROM Expenses 
                                        WHERE ExpenseDate BETWEEN @start AND @end";
                using (SqlCommand cmd = new SqlCommand(expenseQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@start", today);
                    cmd.Parameters.AddWithValue("@end", endOfDay);
                    summary.TodayExpenses = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                // 3. අවසන් වෙමින් පවතින භාණ්ඩ (Stock <= 10) ප්‍රමාණය
                string stockQuery = "SELECT COUNT(ProductID) FROM Products WHERE StockQuantity <= 10";
                using (SqlCommand cmd = new SqlCommand(stockQuery, conn))
                {
                    summary.LowStockCount = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return summary;
        }
    }
}
