using AutoMapper;
using Walmart.Application.Features.Address.Commands.Models;

namespace Walmart.Application.Mapping.Address
{
    public partial class AddressProfile
    {
        public void UpdateAddressMapping()
        {
            CreateMap<UpdateAddressCommand, Walmart.Domain.Entities.Address>();
        }
    }
}
