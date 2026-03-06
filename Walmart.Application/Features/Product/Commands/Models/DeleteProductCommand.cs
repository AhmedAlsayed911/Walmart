using MediatR;

namespace Walmart.Application.Features.Product.Commands.Models
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
