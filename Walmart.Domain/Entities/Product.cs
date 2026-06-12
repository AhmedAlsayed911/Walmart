using System.ComponentModel.DataAnnotations.Schema;

namespace Walmart.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public byte[]? ProductPicture { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePercentage { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive => StockQuantity > 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new HashSet<OrderProduct>();
    }
}
