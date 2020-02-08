﻿using AutoMapper;
using OnlineShopApi.domain.Models.AppModels;
using OnlineShopApi.data.Models;
using OnlineShopApi.data.Repositories;
using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace OnlineShopApi.domain.Managers
{
    public class ShopManager : IShopManager
    {
        private readonly IMapper _mapper;
        private readonly IWooliesService _service;

        public ShopManager(IMapper mapper, IWooliesService service)
        {
            _mapper = mapper;
            _service = service;
        }

        public UserVM GetUser(int? id)
        {
            var customerId = (id.HasValue) ? id.Value : 1;
            var user = _service.GetUser(customerId);
            return _mapper.Map<UserVM>(user);
        }

        #region Product sorting
        public async Task<List<ProductVM>> GetProductsAsync(SortOption sortOption)
        {
            var products = await _service.GetProductsAsync();
            if (products == null) return null;

            var sortedProducts = await SortProducts(products, sortOption);
            if (sortedProducts == null) return null;

            return _mapper.Map<List<ProductVM>>(sortedProducts);
        }

        private async Task<List<Product>> SortProducts(List<Product> products, SortOption sortOption)
        {
            switch (sortOption)
            {
                case SortOption.Low:
                    products.Sort((x, y) => x.Price.CompareTo(y.Price));
                    return products;
                case SortOption.High:
                    products.Sort((x, y) => y.Price.CompareTo(x.Price));
                    return products;
                case SortOption.Ascending:
                    products.Sort((x, y) => x.Name.CompareTo(y.Name));
                    return products;
                case SortOption.Descending:
                    products.Sort((x, y) => y.Name.CompareTo(x.Name));
                    return products;
                case SortOption.Recommended:
                    var shoppedProducts = await GetShoppedProducts();
                    if (shoppedProducts == null) return null;
                    return GetRecommendedProducts(products, shoppedProducts);
                default:
                    throw new System.Exception("Not all sort options available");
            }
        }

        private async Task<List<Product>> GetShoppedProducts()
        {
            var history = await _service.GetShopperHistoryAsync();
            if (history == null) return null;
            return TotalProductQuantities(history);
        }

        private List<Product> TotalProductQuantities(List<ShopperHistory> history)
        {
            if (history.Count < 1) return new List<Product>();

            var reducedShopperHistory = history.Aggregate((acc, next) =>
            {
                foreach (var product in next.Products)
                {
                    var prodInAccumulator = acc.Products.Find(p => p.Name == product.Name);
                    // Add quantity of product if exists or add product to the list
                    if (prodInAccumulator != null)
                        prodInAccumulator.Quantity += product.Quantity;
                    else
                        acc.Products.Add(product);
                }

                return acc;
            });

            return reducedShopperHistory.Products;
        }

        // Add products which are not in ShopperHistory
        private List<Product> GetRecommendedProducts(List<Product> products, List<Product> shoppedProducts)
        {
            foreach (var product in products)
            {
                if (shoppedProducts.Find(p => p.Name == product.Name) == null)
                    shoppedProducts.Add(new Product
                    {
                        Name = product.Name,
                        Quantity = 0,
                        Price = product.Price
                    });
            }

            shoppedProducts.Sort((x, y) => y.Quantity.CompareTo(x.Quantity));
            return shoppedProducts;
        }
        #endregion

        public async Task<decimal> CalculateTrolleyTotalAsync(TrolleyVM trolleyVM)
        {
            var trolley = _mapper.Map<Trolley>(trolleyVM);
            return await _service.CalculateTrolleyTotal(trolley);
        }

        public decimal CalculateTrolleyTotal(TrolleyVM trolleyVM)
        {
            return GetTrolleyTotal(trolleyVM);
        }

        private decimal GetTrolleyTotal(TrolleyVM trolley)
        {
            var trolleyCheckout = new TrolleyCheckout { Total = 0 };

            foreach (ProductQuantityVM item in trolley.Quantities)
            {
                var special = trolley.Specials.Find(s => s.Quantities.Find(q => q.Name == item.Name) != null);

                // Recursive function since specials can apply more than once
                if (special != null) ConsiderSpecials(special, trolley.Quantities, trolleyCheckout);

                // Calculate the price after any specials have been considered & quantities deducted
                var product = trolley.Products.Find(p => p.Name == item.Name);
                if (product == null) throw new Exception("Item in trolley is not listed as a product");

                trolleyCheckout.Total += item.Quantity * product.Price;
                item.Quantity = 0;
            }

            return trolleyCheckout.Total;
        }

        // Recursive method for applying specials to a trolley
        private void ConsiderSpecials(SpecialVM special, List<ProductQuantityVM> items, TrolleyCheckout checkout)
        {
            // If special is available for item, see if quantities match before obtaining special price
            if (DoesSpecialApplyToTrolley(special, items))
            {
                checkout.Total += special.Total;
                ApplySpecial(special, items);

                // Call itself to see if the special applies again
                ConsiderSpecials(special, items, checkout);
            }

            return;
        }

        // A special may consist of multiple items
        private bool DoesSpecialApplyToTrolley(SpecialVM special, List<ProductQuantityVM> items)
        {
            foreach (var itemInSpecial in special.Quantities)
            {
                var item = items.Find(i => i.Name == itemInSpecial.Name);
                if (item == null || itemInSpecial.Quantity > item.Quantity) return false;
            }

            return true;
        }

        private void ApplySpecial(SpecialVM special, List<ProductQuantityVM> items)
        {
            foreach (var itemInSpecial in special.Quantities)
            {
                var item = items.Find(i => i.Name == itemInSpecial.Name);
                item.Quantity -= itemInSpecial.Quantity;
            }
        }
    }
}
