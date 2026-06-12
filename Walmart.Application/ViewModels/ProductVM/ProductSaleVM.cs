using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.ViewModels.ProductVM
{
    public class ProductSaleVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }

        [Range(0.01, 90.0, ErrorMessage = "Sale percentage must be between 0.01 and 90.")]
        public decimal? SalePercentage { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? SaleEndDate { get; set; }
    }
}
