using FishnChipsShop.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishnChips.Model
{
    public class CheckoutSummary
    {
        public int Id { get; set; }
        public User User { get; set; }
        public List<CartItem> CartItems {get; set;}
        public decimal TotalPriceBeforeDiscount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
