using MediatR;
using Walmart.Application.Pagination;
using Walmart.Application.ViewModels.Category;

namespace Walmart.Application.Features.Category.Queries.Models
{
    public class GetAllCategoriesQuery : IRequest<PaginatedResult<CategoryListVM>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
