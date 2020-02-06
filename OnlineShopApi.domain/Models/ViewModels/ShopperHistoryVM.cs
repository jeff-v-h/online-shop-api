using System.Collections.Generic;

namespace OnlineShopApi.domain.Models.ViewModels
{
    public class ShopperHistoryVM
    {
        public int CustomerId { get; set; }
        public List<ProductVM> Products { get; set; }
    }
}
