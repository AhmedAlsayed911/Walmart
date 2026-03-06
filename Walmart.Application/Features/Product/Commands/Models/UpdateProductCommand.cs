using MediatR;
using Microsoft.AspNetCore.Http;

namespace Walmart.Application.Features.Product.Commands.Models
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? ProductPicture { get; set; }
        public byte[]? ExistingProductPicture { get; set; }
    }
}
