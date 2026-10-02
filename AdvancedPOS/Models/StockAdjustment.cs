using System;

namespace AdvancedPOS.Models
{
    public class StockAdjustment
    {
        public int AdjustmentID { get; set; }
        public int ProductID { get; set; }
        public string AdjustmentType { get; set; }   // Damaged / Expired / Lost / Correction
        public int QuantityChanged { get; set; }      // -5 = 5ක් අඩුවෙනවා, +5 = 5ක් වැඩිවෙනවා
        public string Reason { get; set; }
        public DateTime AdjustmentDate { get; set; }
        public int UserID { get; set; }
    }
}