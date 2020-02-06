using OnlineShopApi.data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopApi.data.Repositories
{
    public interface IWooliesService
    {
        User GetUser(int id);
        Task<List<ShopperHistory>> GetShopperHistoryAsync();
        Task<List<Product>> GetProductsAsync();
    }
}
