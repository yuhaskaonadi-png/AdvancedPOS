using System;

namespace AdvancedPOS.Models
{
    public class SalePayment
    {
        public int PaymentID { get; set; }
        public int SaleID { get; set; }
        public string PaymentMethod { get; set; } // Cash, Card, QR
        public decimal Amount { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
