using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvancedPOS.Helpers;

namespace AdvancedPOS.Repositories
{
    public class SalesForecastRepository
    {
        // ==========================================
        // Last 'days' ගණන් දින වල Sales Data ගැනීම
        // Sales නැති දවස් වලට 0 දාලා Gaps Fill කරනවා
        // ==========================================
        public List<(DateTime Date, float Total)> GetDailySalesForForecast(int days)
        {
            DataTable dt = new DataTable();
            DateTime fromDate = DateTime.Today.AddDays(-days + 1);

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"SELECT CAST(SaleDate AS DATE) AS SaleDay, SUM(GrandTotal) AS Total
                                 FROM Sales
                                 WHERE SaleDate >= @from
                                 GROUP BY CAST(SaleDate AS DATE)
                                 ORDER BY SaleDay ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@from", fromDate);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            // Database එකෙන් ආපු Sales Data Dictionary එකකට දැමීම
            Dictionary<DateTime, float> salesDict = new Dictionary<DateTime, float>();
            foreach (DataRow row in dt.Rows)
            {
                DateTime d = Convert.ToDateTime(row["SaleDay"]).Date;
                float total = row["Total"] != DBNull.Value ? (float)Convert.ToDouble(row["Total"]) : 0f;
                salesDict[d] = total;
            }

            // දිනපතා ලැයිස්තුවක් හදලා, Sales නැති දවස් වලට 0 දාලා Gaps Fill කිරීම
            List<(DateTime, float)> result = new List<(DateTime, float)>();
            for (int i = 0; i < days; i++)
            {
                DateTime day = fromDate.AddDays(i);
                float val = salesDict.ContainsKey(day) ? salesDict[day] : 0f;
                result.Add((day, val));
            }

            return result;
        }
    }
}
