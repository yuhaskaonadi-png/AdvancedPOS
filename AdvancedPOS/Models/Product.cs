using System;

namespace AdvancedPOS.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }

        // මේක අපි පාවිච්චි කරන්නේ UI එකේ Category නම පෙන්වන්න (JOIN Query එකකින්)
        public string CategoryName { get; set; }

        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}
