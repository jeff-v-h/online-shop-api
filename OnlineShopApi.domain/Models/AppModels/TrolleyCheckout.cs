namespace OnlineShopApi.domain.Models.AppModels
{
    internal class TrolleyCheckout
    {
        public decimal Total { get; set; }

        public TrolleyCheckout()
        {
            Total = 0;
        }

        public TrolleyCheckout(decimal value)
        {
            Total = value;
        }
    }
}
