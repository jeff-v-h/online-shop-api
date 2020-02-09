namespace OnlineShopApi.domain.Models.ViewModels
{
    public class ProductQuantityVM
    {
        public string Name { get; set; }
        public int Quantity { get; set; }

        public ProductQuantityVM() { }

        public ProductQuantityVM(ProductQuantityVM prevQuantity)
        {
            Name = prevQuantity.Name;
            Quantity = prevQuantity.Quantity;
        }
    }
}
