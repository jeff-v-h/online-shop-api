using OnlineShopApi.common;
using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopApi.domain.Managers
{
    public interface IShopManager
    {
        UserVM GetUser(int? id);
        Task<List<ProductVM>> GetProducts(SortOption sortOption);
    }
}
