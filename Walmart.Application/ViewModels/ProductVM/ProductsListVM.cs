namespace Walmart.Application.ViewModels.ProductVM
{
    public class ProductsListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public byte[] ProductPicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; }

    }
}
