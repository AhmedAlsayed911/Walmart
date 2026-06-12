namespace Walmart.Application.ViewModels.ProductVM
{
    public class TopSoldProductVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal? SalePercentage { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public bool HasActiveSale => SalePercentage.HasValue
                         && SalePercentage.Value > 0
                         && SaleEndDate.HasValue
                         && SaleEndDate.Value >= DateTime.UtcNow;
        public decimal EffectivePrice => HasActiveSale
            ? Math.Round(CurrentPrice * (1 - (SalePercentage!.Value / 100m)), 2)
            : CurrentPrice;
    }
}
