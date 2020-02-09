using System;
using System.Collections.Generic;

namespace OnlineShopApi.domain.Models.ViewModels
{
    public class SpecialVM : ICloneable
    {
        public List<ProductQuantityVM> Quantities { get; set; }
        public decimal Total { get; set; }

        public SpecialVM() { }

        public SpecialVM(SpecialVM previousSpecial)
        {
            Quantities = GetQuantities(previousSpecial.Quantities);
            Total = previousSpecial.Total;
        }

        private List<ProductQuantityVM> GetQuantities(List<ProductQuantityVM> quantities)
        {
            var list = new List<ProductQuantityVM>();
            foreach (var quantity in quantities)
            {
                list.Add(new ProductQuantityVM(quantity));
            }
            return list;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
