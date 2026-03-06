using MediatR;
using Walmart.Application.ViewModels.Order;

namespace Walmart.Application.Features.Order.Queries.Models
{
    public class GetOrderByIdQuery : IRequest<OrderDetailsVM>
    {
        public int Id { get; set; }
    }
}
