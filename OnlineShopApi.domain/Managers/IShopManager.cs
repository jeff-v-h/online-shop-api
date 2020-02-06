using OnlineShopApi.domain.Models.ViewModels;
using System.Collections.Generic;

namespace OnlineShopApi.domain.Managers
{
    public interface IShopManager
    {
        UserVM GetUser(int id);
        List<ProductVM> GetProducts(string sortOption, int customerId);
    }
}
