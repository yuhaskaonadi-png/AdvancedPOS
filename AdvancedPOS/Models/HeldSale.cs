using System;
using System.Collections.Generic;

namespace AdvancedPOS.Models
{
    public class HeldSale
    {
        public int HeldSaleID { get; set; }
        public string Note { get; set; }
        public decimal Discount { get; set; }
        public DateTime HeldDate { get; set; }
        public int UserID { get; set; }
        public List<HeldSaleItem> Items { get; set; } = new List<HeldSaleItem>();
    }
}
