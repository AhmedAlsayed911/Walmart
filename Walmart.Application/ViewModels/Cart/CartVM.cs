namespace Walmart.Application.ViewModels.Cart
{
    public class CartVM
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal SubTotal => Items.Sum(x => x.Total);
        public decimal Tax => SubTotal * 0.1m;
        public decimal GrandTotal => SubTotal + Tax;
    }
}
