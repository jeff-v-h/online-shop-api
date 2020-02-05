using AutoMapper;
using OnlineShopApi.data.Models;
using OnlineShopApi.domain.Models.ViewModels;

namespace OnlineShopApi.domain.Models.ModelMappers
{
    public class ShopProfile : Profile
    {
        public ShopProfile()
        {
            CreateMap<User, UserVM>();
            CreateMap<UserVM, User>();

            CreateMap<Product, ProductVM>();
            CreateMap<ProductVM, Product>();
        }
    }
}
