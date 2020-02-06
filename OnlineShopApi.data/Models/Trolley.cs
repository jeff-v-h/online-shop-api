using System.Collections.Generic;

namespace OnlineShopApi.data.Models
{
    public class Trolley
    {
        public List<ProductBase> Products { get; set; }
        public List<Special> Specials { get; set; }
        public List<ProductQuantity> Quantities { get; set; }
    }
}
