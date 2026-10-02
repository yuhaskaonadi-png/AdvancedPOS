using System;

namespace AdvancedPOS.Models
{
    public class SaleReturn
    {
        public int ReturnID { get; set; }
        public int SaleID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Reason { get; set; }
    }
}