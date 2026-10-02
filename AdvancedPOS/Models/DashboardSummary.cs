using System;

namespace AdvancedPOS.Models
{
    public class DashboardSummary
    {
        public decimal TodaySales { get; set; }
        public int TodayInvoices { get; set; }
        public int LowStockCount { get; set; }
        public decimal TodayExpenses { get; set; }
    }
}