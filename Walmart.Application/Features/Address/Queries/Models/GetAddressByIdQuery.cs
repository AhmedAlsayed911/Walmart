using MediatR;
using Walmart.Application.ViewModels.Address;

namespace Walmart.Application.Features.Address.Queries.Models
{
    public class GetAddressByIdQuery : IRequest<AddressDetailsVM>
    {
        public int Id { get; set; }
    }
}
