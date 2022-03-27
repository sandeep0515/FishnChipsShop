using FishnChips.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishnChipsShop.Model
{
    public class CartItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int NumberOfUnits { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountPrice { get; set; }
    }
}
