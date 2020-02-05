using AutoMapper;
using PLP.Social.DataService.data.Models;
using OnlineShopApi.domain.Models.ViewModels;

namespace OnlineShopApi.domain.Models.ModelMappers
{
    public class SocialProfile : Profile
    {
        public SocialProfile()
        {
            CreateMap<SocialList, SocialListVM>();
            CreateMap<SocialListVM, SocialList>();

            CreateMap<MediaPost, MediaPostVM>();
            CreateMap<MediaPostVM, MediaPost>();

            CreateMap<Attachment, AttachmentVM>();
            CreateMap<AttachmentVM, Attachment>();

            CreateMap<SocialListBase, SocialListBaseVM>();
            CreateMap<SocialListBaseVM, SocialListBase>();

            CreateMap<SocialMediaListsDetails, SocialMediaListsDetailsVM>();
            CreateMap<SocialMediaListsDetailsVM, SocialMediaListsDetails>();

            CreateMap<SocialList, SocialListBase>();
            CreateMap<SocialListBase, SocialList>();
        }
    }
}
