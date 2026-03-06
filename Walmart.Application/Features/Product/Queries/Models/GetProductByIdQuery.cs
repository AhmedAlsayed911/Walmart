using MediatR;
using Walmart.Application.ViewModels.ProductVM;

namespace Walmart.Application.Features.Product.Queries.Models
{
    public class GetProductByIdQuery : IRequest<ProductDetailsVM>
    {
        public int Id { get; set; }
    }
}
