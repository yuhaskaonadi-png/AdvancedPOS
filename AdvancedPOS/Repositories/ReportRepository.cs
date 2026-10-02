using System;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;

namespace AdvancedPOS.Repositories
{
    public class ReportRepository
    {
        // =========================================================
        // 1. Daily Sales Report (දෛනික ආදායම් වාර්තාව)
        // =========================================================
        public DataTable GetDailySalesReport(DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // දවසේ අවසානය දක්වා Filter වීමට කාලය 23:59:59 ලෙස සකසයි
                toDate = toDate.Date.AddDays(1).AddTicks(-1);

                // CAST(SaleDate AS DATE) මඟින් වේලාව ඉවත් කර දිනය පමණක් ගනී
                string query = @"SELECT 
                                    CAST(SaleDate AS DATE) AS [Date],
                                    COUNT(SaleID) AS [Total Invoices],
                                    SUM(SubTotal) AS [Total SubTotal],
                                    SUM(Discount) AS [Total Discount],
                                    SUM(GrandTotal) AS [Total Sales]
                                 FROM Sales
                                 WHERE SaleDate BETWEEN @from AND @to
                                 GROUP BY CAST(SaleDate AS DATE)
                                 ORDER BY CAST(SaleDate AS DATE) DESC";

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

        // =========================================================
        // 2. Monthly Sales Report (මාසික ආදායම් වාර්තාව)
        // =========================================================
        public DataTable GetMonthlySalesReport(DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                toDate = toDate.Date.AddDays(1).AddTicks(-1);

                // YEAR සහ MONTH අනුව Group කර, මාසයේ නම (DATENAME) ලබා ගනී
                string query = @"SELECT 
                                    YEAR(SaleDate) AS [Year],
                                    DATENAME(month, SaleDate) AS [Month],
                                    COUNT(SaleID) AS [Total Invoices],
                                    SUM(SubTotal) AS [Total SubTotal],
                                    SUM(Discount) AS [Total Discount],
                                    SUM(GrandTotal) AS [Total Sales]
                                 FROM Sales
                                 WHERE SaleDate BETWEEN @from AND @to
                                 GROUP BY YEAR(SaleDate), MONTH(SaleDate), DATENAME(month, SaleDate)
                                 ORDER BY YEAR(SaleDate) DESC, MONTH(SaleDate) DESC";

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
        // =========================================================
        // 3. Profit and Expense Report (ලාභ සහ වියදම් වාර්තාව)
        // =========================================================
        public DataTable GetProfitAndExpenseReport(DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // දවසේ අවසානය දක්වා Filter වීමට
                toDate = toDate.Date.AddDays(1).AddTicks(-1);

                // Sales සහ Expenses කියන Tables දෙකෙන් අදාළ දිනවල දත්ත වෙන වෙනම අරගෙන,
                // FULL OUTER JOIN මඟින් එකම දිනයට අදාළව පේළියකට ගෙන ඒම මෙහිදී සිදු වේ.
                string query = @"SELECT 
                            COALESCE(S.Date, E.Date) AS [Date],
                            ISNULL(S.TotalSales, 0) AS [Total Sales],
                            ISNULL(E.TotalExpenses, 0) AS [Total Expenses],
                            (ISNULL(S.TotalSales, 0) - ISNULL(E.TotalExpenses, 0)) AS [Net Profit]
                         FROM 
                            (SELECT CAST(SaleDate AS DATE) AS Date, SUM(GrandTotal) AS TotalSales
                             FROM Sales 
                             WHERE SaleDate BETWEEN @from AND @to 
                             GROUP BY CAST(SaleDate AS DATE)) S
                         FULL OUTER JOIN 
                            (SELECT CAST(ExpenseDate AS DATE) AS Date, SUM(Amount) AS TotalExpenses
                             FROM Expenses 
                             WHERE ExpenseDate BETWEEN @from AND @to 
                             GROUP BY CAST(ExpenseDate AS DATE)) E
                         ON S.Date = E.Date
                         ORDER BY [Date] DESC";

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
        // =========================================================
        // 4. Low Stock Report (අවසන් වෙමින් පවතින භාණ්ඩ වාර්තාව) - FIXED
        // =========================================================
        public DataTable GetLowStockReport(int threshold = 10)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // Category සහ UnitPrice ඉවත් කර, අත්‍යවශ්‍ය දත්ත පමණක් ලබා ගනිමු
                string query = @"SELECT 
                            ProductID AS [Item Code], 
                            ProductName AS [Item Name], 
                            StockQuantity AS [Current Stock]
                         FROM Products 
                         WHERE StockQuantity <= @limit
                         ORDER BY StockQuantity ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@limit", threshold);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}
