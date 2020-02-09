using System.Collections.Generic;

namespace OnlineShopApi.domain.Models.ViewModels
{
    public class TrolleyVM
    {
        public List<ProductBaseVM> Products { get; set; }
        public List<SpecialVM> Specials { get; set; }
        public List<ProductQuantityVM> Quantities { get; set; }

        public TrolleyVM() { }

        public TrolleyVM(TrolleyVM trolley)
        {
            Products = GetProducts(trolley.Products);
            Specials = GetSpecials(trolley.Specials);
            Quantities = GetQuantities(trolley.Quantities);
        }

        private List<ProductBaseVM> GetProducts(List<ProductBaseVM> products)
        {
            var list = new List<ProductBaseVM>();
            foreach (var product in products)
                list.Add(new ProductBaseVM(product));
            return list;
        }

        private List<SpecialVM> GetSpecials(List<SpecialVM> special)
        {
            var list = new List<SpecialVM>();
            foreach (var specia in special)
                list.Add(new SpecialVM(specia));
            return list;
        }

        private List<ProductQuantityVM> GetQuantities(List<ProductQuantityVM> products)
        {
            var list = new List<ProductQuantityVM>();
            foreach (var product in products)
                list.Add(new ProductQuantityVM(product));
            return list;
        }
    }
}
