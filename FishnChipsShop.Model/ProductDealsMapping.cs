using System;
using System.Collections.Generic;
using System.Text;

namespace FishnChips.Model
{
    public class ProductDealsMapping
    {
        public int Id { get; set; }
        public ProductDeals Deal { get; set; }
        public Product Product { get; set; }
        public bool IsActive { get; set; }
    }
}
