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

            CreateMap<Product, ProductVM>();

            CreateMap<ProductBase, ProductBaseVM>();
            CreateMap<ProductBaseVM, ProductBase>();

            CreateMap<ProductQuantity, ProductQuantityVM>();
            CreateMap<ProductQuantityVM, ProductQuantity>();

            CreateMap<ShopperHistory, ShopperHistoryVM>();

            CreateMap<Special, SpecialVM>();
            CreateMap<SpecialVM, Special>();

            CreateMap<TrolleyVM, Trolley>();
        }
    }
}
