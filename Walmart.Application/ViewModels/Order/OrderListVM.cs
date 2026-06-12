namespace Walmart.Application.ViewModels.Order
{
    public class OrderListVM
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int ProductCount { get; set; }
        public bool CanEdit { get; set; }
        public List<OrderListItemVM> Items { get; set; } = new();
    }

    public class OrderListItemVM
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
