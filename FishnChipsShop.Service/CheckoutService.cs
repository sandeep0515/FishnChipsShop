using FishnChips.Model;
using FishnChipsShop.Model;
using FishnChipsShop.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FishnChipsShop.Service
{
    public class CheckoutService : ICheckoutService
    {
        private List<Product> _products = new List<Product>();
        private List<ProductPricing> _productPricings = new List<ProductPricing>();
        private List<ProductDeals> _productDeals = new List<ProductDeals>();
        private List<ProductDealsMapping> _productDealMappings = new List<ProductDealsMapping>();
        private User _user = new User();
        private List<CartItem> _cartItems = new List<CartItem>();

        #region persisting data to run use cases in test environment
        public void AddProductDealMappings(IList<ProductDealsMapping> productDealMappings)
        {
            _productDealMappings.AddRange(productDealMappings);
        }

        public void AddProductDeals(IList<ProductDeals> productDeals)
        {
            _productDeals.AddRange(productDeals);
        }

        public void AddProductPricings(IList<ProductPricing> productPricings)
        {
            _productPricings.AddRange(productPricings);
        }

        public void AddProducts(IList<Product> products)
        {
            _products.AddRange(products);
        }

        public void AddUser(User user)
        {
            _user = user;
        }
        #endregion

        // Adds to cart
        public string AddToCart(Product product, int numberOfUnits)
        {
            if (!ValidateProductExpiry(product))
            {
                return Constants.ITEM_EXPIRED;
            }

            _cartItems.Add(
                new CartItem()
                {
                    Product = product,
                    NumberOfUnits = numberOfUnits,
                    TotalPrice = GetTotalPrice(product, numberOfUnits)
                }
                );
            return Constants.ADD_TO_CART_SUCCESS;
        }

        // Gets total price from product pricing table
        public decimal GetTotalPrice(Product product, int numOfUnits)
        {
            decimal total = 0.0m;
            ProductPricing pricing = _productPricings.Find(i => i.Product.Id == product.Id);
            if (pricing != null)
            {
                total = pricing.PricePerUnit * numOfUnits;
            }
            return total;
        }

        // Return checkout summary
        public CheckoutSummary GetCheckoutSummary()
        {
            CheckoutSummary cart = new CheckoutSummary();
            cart.CartItems = _cartItems;
            cart.TotalPriceBeforeDiscount = _cartItems.Sum(i => i.TotalPrice);

            decimal totalDiscountedPrice = 0m;

            // Calculate discounted price on expiry date which are added to cart
            _cartItems.ForEach(item =>
            {

                ProductPricing productPricing = _productPricings.Find(i => i.Product?.Id == item.Product?.Id && i.ExpiredDayDiscount > 0 && i.ExpiredDate.Date == DateTime.Today.Date);
                if (productPricing != null)
                {
                    item.DiscountPrice = Math.Round((item.TotalPrice * productPricing.ExpiredDayDiscount) / 100, 2);
                    totalDiscountedPrice += item.DiscountPrice;
                }
            });

            cart.DiscountAmount = totalDiscountedPrice;
            cart.FinalPrice = cart.TotalPriceBeforeDiscount - totalDiscountedPrice;

            List<int> cartProductIds = _cartItems.Select(i => i.Product.Id).ToList();
            List<ProductDeals> productDeals = _productDeals.Where(i => cartProductIds.Contains(i.Product.Id)).ToList();

            if (productDeals == null)
            {
                return cart;
            }

            // Applies deals on items which are added to cart
            // Applies single deal on multiple items as per requirement
            // ToDo: Apply multiple deals on muliple items (permutation and combination calculations)
            productDeals.ForEach((deal) =>
            {
                if (!deal.IsComboDeal)
                {
                    CartItem cartItem = _cartItems.FirstOrDefault(i => deal.Product.Id == i.Product.Id);
                    decimal value = (cartItem.TotalPrice * deal.Discount) / 100;
                    cartItem.DiscountPrice = cartItem.DiscountPrice > value ? cartItem.DiscountPrice : value;
                }
                else
                {
                    CheckoutSummary appliedDealCart = CalculateCart(CalculateCartItemsByDeal(_cartItems, deal));
                    if (appliedDealCart.FinalPrice < cart.FinalPrice)
                    {
                        cart.CartItems = appliedDealCart.CartItems;
                        cart.DiscountAmount = appliedDealCart.DiscountAmount;
                        cart.FinalPrice = appliedDealCart.FinalPrice;
                    }
                }
            });
            return cart;
        }

        // Calculates total price of each items inside cart
        public List<CartItem> CalculateTotal(List<CartItem> items, int? numberOfUnits = 0, decimal? discountPercentage = 0m)
        {
            items.ForEach(item =>
            {
                ProductPricing productPricing = _productPricings.Find(i => i.Product?.Id == item.Product?.Id);
                if (productPricing != null)
                {
                    item.TotalPrice = (productPricing.PricePerUnit * item.NumberOfUnits);
                    if (discountPercentage.HasValue)
                    {
                        item.DiscountPrice = Math.Round((productPricing.PricePerUnit * (numberOfUnits.HasValue ? numberOfUnits.Value : item.NumberOfUnits) * discountPercentage.Value) / 100, 2);
                    }
                }
            });
            return items;
        }

        // Calculates total price of cart items based on deal
        public List<CartItem> CalculateCartItemsByDeal(List<CartItem> cartItems, ProductDeals deal)
        {
            List<ProductDealsMapping> dealMappings = _productDealMappings.Where(i => i.Deal.Id == deal.Id).ToList();
            if (dealMappings == null)
            {
                return null;
            }

            List<int> productIds = new List<int>() { deal.Product.Id };
            productIds.AddRange(dealMappings.Select(i => i.Product.Id).ToList());
            bool isDealEligible = true;
            productIds.ForEach(i =>
            {
                isDealEligible = cartItems.Any(item => item.Product.Id == i);
            });

            if (!isDealEligible)
            {
                return null;
            }

            List<CartItem> dealCartItems = _cartItems.Where(i => productIds.Contains(i.Product.Id)).ToList();
            int totalMealDealQuantities = dealCartItems.Min(i => i.NumberOfUnits);
            dealCartItems = CalculateTotal(dealCartItems, totalMealDealQuantities, deal.Discount);
            return dealCartItems;
        }

        // Calculates Checkout summary
        public CheckoutSummary CalculateCart(IList<CartItem> cartItems)
        {
            CheckoutSummary cart = new CheckoutSummary()
            {
                User = _user,
                CartItems = _cartItems,
                TotalPriceBeforeDiscount = _cartItems.Sum(i => i.TotalPrice),
                DiscountAmount = _cartItems.Sum(i => i.DiscountPrice)
            };
            cart.FinalPrice = cart.TotalPriceBeforeDiscount - cart.DiscountAmount;
            return cart;
        }

        // Clears cart
        public void ClearCart()
        {
            _cartItems.Clear();
        }

        // Checks if product has exceeded expiration date
        public bool ValidateProductExpiry(Product product)
        {
            ProductPricing pricing = _productPricings.FirstOrDefault(i => i.Product.Id == product.Id);
            return DateTime.Today.Date <= pricing.ExpiredDate.Date;
        }
    }
}
