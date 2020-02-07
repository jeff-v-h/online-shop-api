using OnlineShopApi.domain.Models.AppModels;
using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopApi.domain.Managers
{
    public interface IShopManager
    {
        UserVM GetUser(int? id);
        Task<List<ProductVM>> GetProductsAsync(SortOption sortOption);
        Task<decimal> CalculateTrolleyTotal(TrolleyVM trolleyVM);
    }
}
