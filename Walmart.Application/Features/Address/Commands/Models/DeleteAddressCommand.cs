using MediatR;

namespace Walmart.Application.Features.Address.Commands.Models
{
    public class DeleteAddressCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
