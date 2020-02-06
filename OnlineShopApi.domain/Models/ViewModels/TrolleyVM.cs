using System.Collections.Generic;

namespace OnlineShopApi.domain.Models.ViewModels
{
    public class TrolleyVM
    {
        public List<ProductBaseVM> Products { get; set; }
        public List<SpecialVM> Specials { get; set; }
        public List<ProductQuantityVM> Quantities { get; set; }
    }
}
