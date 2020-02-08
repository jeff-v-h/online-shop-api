using System.Collections.Generic;

namespace OnlineShopApi.domain.Models.ViewModels
{
    public class SpecialVM
    {
        public List<ProductQuantityVM> Quantities { get; set; }
        public decimal Total { get; set; }
    }
}
