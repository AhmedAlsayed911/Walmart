namespace Walmart.Application.ViewModels.ProductVM
{
    public class ProductsListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePercentage { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public int StockQuantity { get; set; }
        public byte[] ProductPicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; }
        public bool HasActiveSale => SalePercentage.HasValue
                                     && SalePercentage.Value > 0
                                     && SaleEndDate.HasValue
                                     && SaleEndDate.Value >= DateTime.UtcNow;
        public decimal EffectivePrice => HasActiveSale
            ? Math.Round(Price * (1 - (SalePercentage!.Value / 100m)), 2)
            : Price;

    }
}
