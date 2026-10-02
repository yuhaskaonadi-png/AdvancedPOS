using System;

namespace AdvancedPOS.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int LoyaltyPoints { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}
