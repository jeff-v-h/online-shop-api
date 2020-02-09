using System.Collections.Generic;

namespace OnlineShopApi.data.Models
{
    public class Special
    {
        public List<ProductQuantity> Quantities { get; set; }
        public decimal Total { get; set; }
    }
}
