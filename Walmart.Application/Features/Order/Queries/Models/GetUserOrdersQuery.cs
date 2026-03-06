using MediatR;
using Walmart.Application.Pagination;
using Walmart.Application.ViewModels.Order;

namespace Walmart.Application.Features.Order.Queries.Models
{
    public class GetUserOrdersQuery : IRequest<PaginatedResult<OrderListVM>>
    {
        public string UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
