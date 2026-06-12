using MediatR;

namespace Walmart.Application.Features.Product.Commands.Models
{
    public class SetProductSaleCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public decimal? SalePercentage { get; set; }
        public DateTime? SaleEndDate { get; set; }
    }
}
