using MediatR;
using Walmart.Application.ViewModels.ProductVM;

namespace Walmart.Application.Features.Product.Queries.Models
{
    public class GetTopSoldProductsQuery : IRequest<List<TopSoldProductVM>>
    {
        public int Take { get; set; } = 3;
    }
}
