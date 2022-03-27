using System;
using System.Collections.Generic;
using System.Text;

namespace FishnChips.Model
{
    public class ProductPricing
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal ExpiredDayDiscount { get; set; }
        public DateTime ManufacturedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public Product Product { get; set; }
    }
}
