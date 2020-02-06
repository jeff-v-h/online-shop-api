using AutoMapper;
using OnlineShopApi.data.Models;
using OnlineShopApi.data.Repositories;
using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;

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

        public UserVM GetUser(int id)
        {
            var user = _service.GetUser(id);
            return _mapper.Map<UserVM>(user);
        }

        public List<ProductVM> GetProducts(string sortOption, int customerId)
        {
            var products = _service.GetProducts();
            var productsVM = _mapper.Map<List<ProductVM>>(products);

            SortProducts(productsVM, sortOption, customerId);

            return productsVM;
        }

        private void SortProducts(List<ProductVM> productsVM, string sortOption, int customerId)
        {

        }
    }
}
