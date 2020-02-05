using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopApi.domain.Managers
{
    public interface ISocialManager
    {
        SocialMediaListsDetailsVM GetListsDetails();
        Task<SocialMediaListsDetailsVM> CreateListsDetails(SocialMediaListsDetailsVM detailsVM);
        Task UpdateListsDetails(SocialMediaListsDetailsVM detailsVM);
        SocialMediaListsVM Get();
        SocialListVM Get(string id);
        Task<SocialListVM> Create(SocialListVM docVM);
        Task Update(string id, SocialListVM listVM);
        Task AddItemsToList(string listId, List<MediaPostVM> posts);
        Task AddItemToList(string listId, MediaPostVM post);
        Task DeleteItemFromList(string listId, string id);
        Task Delete(string id);
        Task UpdateAndPublish(string id, SocialListVM listVM);
        void PublishAllLists();
        bool? Publish(string id);
    }
}
