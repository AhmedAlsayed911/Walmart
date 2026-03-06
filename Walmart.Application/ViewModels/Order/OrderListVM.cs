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
    }
}
