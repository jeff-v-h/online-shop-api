using AutoMapper;
using OnlineShopApi.domain.Models.AppModels;
using OnlineShopApi.data.Models;
using OnlineShopApi.data.Repositories;
using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using OnlineShopApi.domain.Helpers;

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

        #region Calculate Trolley
        public async Task<decimal> CalculateTrolleyTotalAsync(TrolleyVM trolleyVM)
        {
            var trolley = _mapper.Map<Trolley>(trolleyVM);
            return await _service.CalculateTrolleyTotal(trolley);
        }

        public decimal CalculateTrolleyTotal(TrolleyVM trolleyVM)
        {
            var trolleyCheckout = new TrolleyCheckout();
            return GetTrolleyTotal(trolleyVM, trolleyCheckout);
        }

        // Get total for current trolley - Recursive function
        private decimal GetTrolleyTotal(TrolleyVM trolley, TrolleyCheckout trolleyCheckout)
        {
            // 1. Get all specials that apply to trolley. 
            var potentialSpecials = GetPotentialSpecials(trolley);

            // 2. Calculate end cost of each potential special to get the cheapest result
            if (potentialSpecials.Count > 0)
            {
                return GetCheapestTotal(trolley, potentialSpecials, trolleyCheckout);
            }
            else
            {
                return GetSingularItemsTotal(trolley, trolleyCheckout);
            }
        }

        private List<SpecialVM> GetPotentialSpecials(TrolleyVM trolley)
        {
            // Keep track of potential specials since there can be overlap of specials that include the same item
            var potentialSpecials = new List<SpecialVM>();
            // Deep clone of specials to search through below so no specials are repeated
            List<SpecialVM> specialsToSearchThrough = trolley.Specials.Clone().ToList();

            // In reality there can be more specials than checkout, so loop through checkout items to determine which ones apply.
            foreach (ProductQuantityVM item in trolley.Quantities)
            {
                // Check if item applies and if it does also remove it from the specials to look through.
                // Loop through backwards since removing list being iterated
                for (int i = specialsToSearchThrough.Count - 1; i >= 0; i--)
                {
                    var special = specialsToSearchThrough[i];
                    // Only add special to list of potentials 
                    if (DoesSpecialIncludeItem(special, item.Name) && DoesSpecialApplyToTrolley(special, trolley.Quantities))
                    {
                        potentialSpecials.Add(special);
                        specialsToSearchThrough.RemoveAt(i);
                    }
                }
            }

            return potentialSpecials;
        }

        private bool DoesSpecialIncludeItem(SpecialVM special, string itemName)
        {
            return special.Quantities.Find(q => q.Name == itemName) != null;
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

        private decimal GetCheapestTotal(TrolleyVM trolley, List<SpecialVM> potentialSpecials, TrolleyCheckout trolleyCheckout)
        {
            int indexCheapest = 0;
            decimal cheapest = 0;

            for (int i = 0; i < potentialSpecials.Count; i++)
            {
                var special = potentialSpecials[i];
                var checkoutForSpecial = new TrolleyCheckout(trolleyCheckout.Total);
                var trolleyForSpecial = new TrolleyVM(trolley);

                checkoutForSpecial.Total += special.Total;
                ApplySpecial(special, trolleyForSpecial.Quantities);

                // Recursive call to continue calculating with reduced items
                var totalForSpecial = GetTrolleyTotal(trolleyForSpecial, checkoutForSpecial);

                if (i == 0)
                {
                    cheapest = totalForSpecial;
                }
                else if (totalForSpecial < cheapest)
                {
                    indexCheapest = i;
                    cheapest = totalForSpecial;
                }
            }

            // Apply the special that has cheapest end value
            return cheapest;
        }

        private void ApplySpecial(SpecialVM special, List<ProductQuantityVM> items)
        {
            foreach (var itemInSpecial in special.Quantities)
            {
                var item = items.Find(i => i.Name == itemInSpecial.Name);
                item.Quantity -= itemInSpecial.Quantity;
            }
        }

        private decimal GetSingularItemsTotal(TrolleyVM trolley, TrolleyCheckout trolleyCheckout)
        {
            // finally calculate the remaining trolley items
            foreach (ProductQuantityVM item in trolley.Quantities)
            {
                var product = trolley.Products.Find(p => p.Name == item.Name);
                if (product == null) throw new Exception("Item in trolley is not listed as a product");

                trolleyCheckout.Total += item.Quantity * product.Price;
                item.Quantity = 0;
            }

            return trolleyCheckout.Total;
        }
        #endregion
    }
}
