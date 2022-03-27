using FishnChips.Model;
using FishnChipsShop.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishnChipsShop.Service.Interface
{
    public interface ICheckoutService
    {
        void AddProducts(IList<Product> products);
        void AddProductPricings(IList<ProductPricing> productPricings);
        void AddProductDeals(IList<ProductDeals> productDeals);
        void AddProductDealMappings(IList<ProductDealsMapping> productDealMappings);
        void AddUser(User user);
        string AddToCart(Product product, int numberOfUnits);
        decimal GetTotalPrice(Product product, int numOfUnits);
        CheckoutSummary GetCheckoutSummary();
        List<CartItem> CalculateTotal(List<CartItem> items, int? numberOfUnits = 0, decimal? discountPercentage = 0m);
        List<CartItem> CalculateCartItemsByDeal(List<CartItem> cartItems, ProductDeals deal);
        CheckoutSummary CalculateCart(IList<CartItem> cartItems);
        bool ValidateProductExpiry(Product product);
        void ClearCart();
    }
}
