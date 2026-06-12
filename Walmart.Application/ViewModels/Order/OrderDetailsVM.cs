namespace Walmart.Application.ViewModels.Order
{
    public class OrderDetailsVM
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int ShippingAddressId { get; set; }
        public string ShippingAddress { get; set; }
        public int Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderProductVM> Products { get; set; } = new();
    }

    public class OrderProductVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}
