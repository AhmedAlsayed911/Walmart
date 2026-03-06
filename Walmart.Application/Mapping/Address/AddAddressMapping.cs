using AutoMapper;
using Walmart.Application.Features.Address.Commands.Models;

namespace Walmart.Application.Mapping.Address
{
    public partial class AddressProfile
    {
        public void AddAddressMapping()
        {
            CreateMap<AddAddressCommand, Walmart.Domain.Entities.Address>();
        }
    }
}
