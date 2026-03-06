using System.ComponentModel.DataAnnotations.Schema;

namespace Walmart.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        [ForeignKey(nameof(Address))]
        public int ShippingAddressId { get; set; }
        public virtual Address Address { get; set; }
        public string OrderNumber { get; set; }
        public int Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
