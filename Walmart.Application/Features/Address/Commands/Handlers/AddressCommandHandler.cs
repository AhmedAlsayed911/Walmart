using AutoMapper;
using MediatR;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Address.Commands.Models;

namespace Walmart.Application.Features.Address.Commands.Handlers
{
    public class AddressCommandHandler(IBaseRepository<Walmart.Domain.Entities.Address> repository, IMapper mapper)
        : IRequestHandler<AddAddressCommand, bool>,
          IRequestHandler<UpdateAddressCommand, bool>,
          IRequestHandler<DeleteAddressCommand, bool>
    {
        public async Task<bool> Handle(AddAddressCommand request, CancellationToken cancellationToken)
        {
            var address = mapper.Map<Walmart.Domain.Entities.Address>(request);
            await repository.AddAsync(address);
            return true;
        }

        public async Task<bool> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await repository.GetByIdAsync(request.Id);
            if (address is null)
                return false;

            address.UserId = request.UserId;
            address.Country = request.Country;
            address.City = request.City;
            address.Street = request.Street;
            address.ZIP = request.ZIP;
            address.IsDefault = request.IsDefault;

            await repository.UpdateAsync(address);
            return true;
        }

        public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var address = await repository.GetByIdAsync(request.Id);
            if (address is null)
                return false;

            await repository.DeleteAsync(address);
            return true;
        }
    }
}
