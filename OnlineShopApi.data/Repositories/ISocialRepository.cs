using OnlineShopApi.data.Models;
using System.Threading.Tasks;

namespace OnlineShopApi.data.Repositories
{
    public interface ISocialRepository : IDocumentRepository<SocialList>
    {
        SocialMediaListsDetails GetListsDetails();
        Task CreateListsDetailsAsync(SocialMediaListsDetails doc);
        Task UpdateListsDetailsAsync(string id, SocialMediaListsDetails listDetails);
        SocialList Get(string id);
        Task UpdateAsync(string id, SocialList list);
        Task DeleteAsync(string id);
    }
}
