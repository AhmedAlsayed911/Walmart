using MediatR;
using Walmart.Application.Pagination;
using Walmart.Application.ViewModels.Address;

namespace Walmart.Application.Features.Address.Queries.Models
{
    public class GetAllAddressesQuery : IRequest<PaginatedResult<AddressListVM>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
