using MediatR;

namespace Walmart.Application.Features.Order.Commands.Models
{
    public class DeleteOrderCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
