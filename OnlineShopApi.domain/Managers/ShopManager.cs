using AutoMapper;
using OnlineShopApi.common;
using OnlineShopApi.data.Models;
using OnlineShopApi.data.Repositories;
using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<List<ProductVM>> GetProducts(SortOption sortOption)
        {
            var products = _service.GetProducts();
            var productsVM = _mapper.Map<List<ProductVM>>(products);

            await SortProducts(productsVM, sortOption);

            return productsVM;
        }

        private async Task<List<ProductVM>> SortProducts(List<ProductVM> products, SortOption sortOption)
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
                    var recommended = await GetRecommendedProducts();
                    return _mapper.Map<List<ProductVM>>(recommended);
                default:
                    throw new System.Exception("Not all sort options available");
            }
        }

        private async Task<List<Product>> GetRecommendedProducts()
        {
            var history = await _service.GetShopperHistory();

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
    }
}
