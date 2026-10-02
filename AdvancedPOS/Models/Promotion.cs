namespace AdvancedPOS.Models
{
    public class Promotion
    {
        public int PromotionID { get; set; }
        public int ProductID { get; set; }
        public int BuyQty { get; set; }
        public int FreeQty { get; set; }
        public bool IsActive { get; set; }
    }
}
