namespace Walmart.Application.ViewModels.Dashboard
{
    public class AdminDashboardVM
    {
        public int TotalProducts { get; set; }
        public int InStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int LowStockProducts { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public List<TopSellingProductVM> TopSellingProducts { get; set; } = new();
    }

    public class TopSellingProductVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int UnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
