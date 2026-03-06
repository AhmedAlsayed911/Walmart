using MediatR;
using Walmart.Application.Pagination;
using Walmart.Application.ViewModels.ProductVM;

namespace Walmart.Application.Features.Product.Queries.Models
{
    public class GetAllProductsQuery : IRequest<PaginatedResult<ProductsListVM>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? SortBy { get; set; }

    }
}
