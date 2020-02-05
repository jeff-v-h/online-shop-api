using AutoMapper;
using PLP.Social.DataService.data.Models.User;
using OnlineShopApi.domain.Models.ViewModels;

namespace OnlineShopApi.domain.Models.ModelMappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserVM>();
            CreateMap<UserVM, User>();
        }
    }
}
