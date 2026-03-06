using AutoMapper;

namespace Walmart.Application.Mapping.Address
{
    public partial class AddressProfile : Profile
    {
        public AddressProfile()
        {
            AddressListMapping();
            AddressDetailsMapping();
            AddAddressMapping();
            UpdateAddressMapping();
        }
    }
}
