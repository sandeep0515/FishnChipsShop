using System;
using System.Collections.Generic;
using System.Text;

namespace FishnChips.Model
{
    public class ProductDeals
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public decimal Discount { get; set; }
        public bool IsComboDeal { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
