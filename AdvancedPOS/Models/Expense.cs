using System;

namespace AdvancedPOS.Models
{
    public class Expense
    {
        public int ExpenseID { get; set; }
        public int CategoryID { get; set; }

        // UI එකේදී ID එක වෙනුවට Category එකේ නම පෙන්වීමට 
        public string CategoryName { get; set; }

        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
    }
}
