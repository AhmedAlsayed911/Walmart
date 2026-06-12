using MediatR;
using Walmart.Application.Pagination;
using Walmart.Application.ViewModels.Order;

namespace Walmart.Application.Features.Order.Queries.Models
{
    public class GetAllOrdersQuery : IRequest<PaginatedResult<OrderListVM>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? OrderId { get; set; }
        public int? Status { get; set; }
        public string? SortBy { get; set; }
    }
}
