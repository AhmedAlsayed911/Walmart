using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Address.Queries.Models;
using Walmart.Application.Pagination;
using Walmart.Application.ViewModels.Address;

namespace Walmart.Application.Features.Address.Queries.Handlers
{
    public class AddressQueryHandler(IBaseRepository<Walmart.Domain.Entities.Address> repository, IMapper mapper)
        : IRequestHandler<GetAllAddressesQuery, PaginatedResult<AddressListVM>>,
          IRequestHandler<GetAddressByIdQuery, AddressDetailsVM>
    {
        public async Task<PaginatedResult<AddressListVM>> Handle(GetAllAddressesQuery request, CancellationToken cancellationToken)
        {
            return await repository
                .GetTableNoTracking()
                .Include(a => a.User)
                .ProjectTo<AddressListVM>(mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }

        public async Task<AddressDetailsVM> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var address = await repository
                .GetTableNoTracking()
                .Include(a => a.User)
                .Where(a => a.Id == request.Id)
                .ProjectTo<AddressDetailsVM>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return address;
        }
    }
}
