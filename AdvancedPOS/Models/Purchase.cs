using System;
using System.Collections.Generic;

namespace AdvancedPOS.Models
{
    public class Purchase
    {
        public int PurchaseID { get; set; }
        public int SupplierID { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int UserID { get; set; }

        // අදාළ බිල්පතට අයත් භාණ්ඩ ලැයිස්තුව
        public List<PurchaseDetail> Details { get; set; } = new List<PurchaseDetail>();
    }
}