using FishnChips.Model;
using FishnChipsShop.Model;
using FishnChipsShop.Service;
using FishnChipsShop.Service.Interface;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FishnChipsShop.Test
{
    public class Tests
    {
        private ICheckoutService _checkoutService;
        [SetUp]
        public void Setup()
        {
            _checkoutService = new CheckoutService();
            Product chip = new Product()
            {
                Id = 1,
                ProductName = "Chips",
                ProductDescription = "Chips",
                UnitsInStock = 10
            };

            Product pie = new Product()
            {
                Id = 2,
                ProductName = "Pie",
                ProductDescription = "Pie",
                UnitsInStock = 10
            };

            Product fish = new Product()
            {
                Id = 3,
                ProductName = "Fish",
                ProductDescription = "Fish",
                UnitsInStock = 10
            };

            ProductPricing chipsPricing = new ProductPricing()
            {
                Id = 1,
                Product = chip,
                PricePerUnit = 1.80m,
                ManufacturedDate = DateTime.Today,
                ExpiredDate = DateTime.Today.AddMonths(3),
                ExpiredDayDiscount = 0m,
                Quantity = 100.00m
            };

            ProductPricing piePricing = new ProductPricing()
            {
                Id = 2,
                Product = pie,
                PricePerUnit = 3.20m,
                ManufacturedDate = DateTime.Today.AddDays(-5),
                ExpiredDate = DateTime.Today.AddMonths(3),
                ExpiredDayDiscount = 50m,
                Quantity = 100.00m
            };

            ProductPricing fishPricing = new ProductPricing()
            {
                Id = 3,
                Product = fish,
                PricePerUnit = 3.50m,
                ManufacturedDate = DateTime.Today.AddDays(-5),
                ExpiredDate = DateTime.Today.AddMonths(3),
                ExpiredDayDiscount = 0m,
                Quantity = 100.00m
            };

            IList<Product> defaultProducts = new List<Product>() { chip, fish, pie };
            _checkoutService.AddProducts(defaultProducts);

            IList<ProductPricing> defaultProductPricing = new List<ProductPricing>() { chipsPricing, piePricing, fishPricing };
            _checkoutService.AddProductPricings(defaultProductPricing);

            ProductDeals pieDeal = new ProductDeals()
            {
                Id = 1,
                Product = pie,
                Discount = 20.0m,
                ExpiresOn = DateTime.Today.AddMonths(1),
                IsComboDeal = true
            };

            ProductDeals fishDeal = new ProductDeals()
            {
                Id = 1,
                Product = fish,
                Discount = 10.0m,
                ExpiresOn = DateTime.Today.AddMonths(1),
                IsComboDeal = false
            };

            IList<ProductDeals> productDeals = new List<ProductDeals>() { pieDeal, fishDeal };
            _checkoutService.AddProductDeals(productDeals);

            IList<ProductDealsMapping> dealsMappings = new List<ProductDealsMapping>()
            {
                new ProductDealsMapping()
                {
                    Id = 1,
                    Deal = pieDeal,
                    Product = chip,
                    IsActive = true
                }
            };
            _checkoutService.AddProductDealMappings(dealsMappings);
        }

        // Use case 1:  A portion of chips costs £1.80
        [Test]
        public void GetCheckoutSummary_AddChips_ReturnExpectedValue()
        {
            // Arrange
            Product chip = new Product()
            {
                Id = 1,
                ProductName = "Chips",
                ProductDescription = "Chips",
                UnitsInStock = 1
            };

            // Act
            decimal expectedPrice = 1.80m;
            _checkoutService.AddToCart(chip, 1);
            CheckoutSummary cart = _checkoutService.GetCheckoutSummary();

            //  Assert
            Assert.AreEqual(expectedPrice, cart.FinalPrice);
        }

        // Use case 2:  A pie costs £3.20
        [Test]
        public void GetCheckoutSummary_AddPie_ReturnExpectedValue()
        {
            // Arrange
            Product pie = new Product()
            {
                Id = 2,
                ProductName = "Pie",
                ProductDescription = "Pie",
                UnitsInStock = 10
            };

            // Act
            decimal expectedPrice = 3.20m;
            _checkoutService.AddToCart(pie, 1);
            CheckoutSummary cart = _checkoutService.GetCheckoutSummary();

            //  Assert
            Assert.AreEqual(expectedPrice, cart.FinalPrice);
        }


        // Use case 3: A pie has an expiry date
        // A pie cannot be sold if it is past the expiry date
        [Test]
        public void AddToCart_AddExpiredPies_ReturnExpiredMessage()
        {
            // Arrange
            Product expiredPie = new Product()
            {
                Id = 4,
                ProductName = "Pie",
                ProductDescription = "Pie",
                UnitsInStock = 2
            };
            IList<Product> expiredPies = new List<Product>() { expiredPie };

            IList<ProductPricing> expiredPiesPricing = new List<ProductPricing>()
            {
                new ProductPricing()
                {
                    Id = 4,
                    Product = expiredPie,
                    ExpiredDate = DateTime.Today.AddDays(-1),
                    ExpiredDayDiscount = 50.0m,
                    ManufacturedDate = DateTime.Today.AddMonths(-6),
                    PricePerUnit = 3.20m,
                    Quantity = 250.0m
                }
            };
            _checkoutService.AddProducts(expiredPies);
            _checkoutService.AddProductPricings(expiredPiesPricing);
            string expectedMessage = Constants.ITEM_EXPIRED;

            // Act
            string actualMessage = _checkoutService.AddToCart(expiredPie, 1);

            //  Assert
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        // Use case 4: A pie is sold at a discounted rate of 50% on the day of expiry only
        [Test]
        public void AddToCart_AddExpiredTodayPies_ReturnPriceWithDiscount()
        {
            // Arrange
            Product expiredPie = new Product()
            {
                Id = 4,
                ProductName = "Pie",
                ProductDescription = "Pie",
                UnitsInStock = 2
            };
            IList<Product> expiredPies = new List<Product>() { expiredPie };

            IList<ProductPricing> expiredPiesPricing = new List<ProductPricing>()
            {
                new ProductPricing()
                {
                    Id = 4,
                    Product = expiredPie,
                    ExpiredDate = DateTime.Today,
                    ExpiredDayDiscount = 50.0m,
                    ManufacturedDate = DateTime.Today.AddMonths(-6),
                    PricePerUnit = 3.20m,
                    Quantity = 250.0m
                }
            };
            _checkoutService.AddProducts(expiredPies);
            _checkoutService.AddProductPricings(expiredPiesPricing);
            decimal expectedPrice = 1.60m;

            // Act
            _checkoutService.AddToCart(expiredPie, 1);
            CheckoutSummary cart = _checkoutService.GetCheckoutSummary();

            //  Assert
            Assert.AreEqual(expectedPrice, cart.FinalPrice);
        }

        // Use case 5: A pie and chips meal deal applies a 20% discount to both items
        [Test]
        public void GetCheckoutSummary_AddPieAndChips_ReturnExpectedValue()
        {
            // Arrange
            Product chip = new Product()
            {
                Id = 1,
                ProductName = "Chips",
                ProductDescription = "Chips",
                UnitsInStock = 10
            };

            Product pie = new Product()
            {
                Id = 2,
                ProductName = "Pie",
                ProductDescription = "Pie",
                UnitsInStock = 10
            };

            // Act
            decimal expectedPrice = 4.00m;
            _checkoutService.AddToCart(chip, 1);
            _checkoutService.AddToCart(pie, 1);
            CheckoutSummary cart = _checkoutService.GetCheckoutSummary();

            //  Assert
            Assert.AreEqual(expectedPrice, cart.FinalPrice);
        }

        // Use case 6: The discount can be applied to multiple meal deals
        [Test]
        public void GetCheckoutSummary_AddMultiplePieAndChips_ReturnExpectedValue()
        {
            // Arrange
            Product chip = new Product()
            {
                Id = 1,
                ProductName = "Chips",
                ProductDescription = "Chips",
                UnitsInStock = 10
            };

            Product pie = new Product()
            {
                Id = 2,
                ProductName = "Pie",
                ProductDescription = "Pie",
                UnitsInStock = 10
            };

            // Act
            decimal expectedPrice = 8.00m;
            _checkoutService.AddToCart(chip, 2);
            _checkoutService.AddToCart(pie, 2);
            CheckoutSummary cart = _checkoutService.GetCheckoutSummary();

            //  Assert
            Assert.AreEqual(expectedPrice, cart.FinalPrice);
        }

        // Use case 7: The discount is not applied to items outside of a meal deal (for example, if there are 2 pies
        // and 3 portions of chips in the basket, only 2 pies and 2 portions of chips should be
        // discounted)
        // Use case 8: Items in a meal deal are not eligible for any other discounts
        [Test]
        public void GetCheckoutSummary_AddMultiplePieAndChips_ApplyDiscountForEligibleMealDealAndReturnExpectedMeal()
        {
            // Arrange
            Product chip = new Product()
            {
                Id = 1,
                ProductName = "Chips",
                ProductDescription = "Chips",
                UnitsInStock = 10
            };

            Product pie = new Product()
            {
                Id = 2,
                ProductName = "Pie",
                ProductDescription = "Pie",
                UnitsInStock = 10
            };

            // Act
            decimal expectedPrice = 9.80m;
            _checkoutService.AddToCart(chip, 3);
            _checkoutService.AddToCart(pie, 2);
            CheckoutSummary cart = _checkoutService.GetCheckoutSummary();

            //  Assert
            Assert.AreEqual(expectedPrice, cart.FinalPrice);
        }

        // Use case 9: When multiple discounts may be applied, then the customer should always be offered the
        // lowest total price
        [Test]
        public void GetCheckoutSummary_AddMultiplePieAndChips_ReturnLowestPrice()
        {
            // Arrange
            Product chip = new Product()
            {
                Id = 1,
                ProductName = "Chips",
                ProductDescription = "Chips",
                UnitsInStock = 10
            };

            Product pie = new Product()
            {
                Id = 4,
                ProductName = "Pie",
                ProductDescription = "Pie",
                UnitsInStock = 10
            };

            ProductPricing piePricing = new ProductPricing()
            {
                Id = 4,
                Product = pie,
                PricePerUnit = 3.20m,
                ManufacturedDate = DateTime.Today.AddMonths(-5),
                ExpiredDate = DateTime.Today,
                ExpiredDayDiscount = 50m,
                Quantity = 100.00m
            };

            IList<Product> products = new List<Product>() { pie };
            _checkoutService.AddProducts(products);

            IList<ProductPricing> productPricing = new List<ProductPricing>() { piePricing };
            _checkoutService.AddProductPricings(productPricing);

            ProductDeals pieDeal = new ProductDeals()
            {
                Id = 1,
                Product = pie,
                Discount = 20.0m,
                ExpiresOn = DateTime.Today.AddMonths(1),
                IsComboDeal = true
            };

            IList<ProductDeals> productDeals = new List<ProductDeals>() { pieDeal };
            _checkoutService.AddProductDeals(productDeals);

            IList<ProductDealsMapping> dealsMappings = new List<ProductDealsMapping>()
            {
                new ProductDealsMapping()
                {
                    Id = 1,
                    Deal = pieDeal,
                    Product = chip,
                    IsActive = true
                }
            };
            _checkoutService.AddProductDealMappings(dealsMappings);

            // Act
            decimal expectedPrice = 8.60m;
            _checkoutService.AddToCart(chip, 3);
            _checkoutService.AddToCart(pie, 2);
            CheckoutSummary cart = _checkoutService.GetCheckoutSummary();

            //  Assert
            Assert.AreEqual(expectedPrice, cart.FinalPrice);
        }


    }
}