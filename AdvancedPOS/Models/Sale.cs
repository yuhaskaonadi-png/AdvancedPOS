using System;
using System.Collections.Generic;

namespace AdvancedPOS.Models
{
    public class Sale
    {
        public int SaleID { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public int UserID { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public string PaymentStatus { get; set; } = "Paid";
        public List<SalePayment> Payments { get; set; } = new List<SalePayment>();
    }
}