namespace OnlineShopApi.domain.Models.ViewModels
{
    public class ProductBaseVM
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public ProductBaseVM() { }

        public ProductBaseVM(ProductBaseVM prevProduct)
        {
            Name = prevProduct.Name;
            Price = prevProduct.Price;
        }
    }
}
