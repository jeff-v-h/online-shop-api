using System.Collections.Generic;

namespace OnlineShopApi.data.Models
{
    public class ShopperHistory
    {
        public int CustomerId { get; set; }
        public List<Product> Products { get; set; }
    }
}
