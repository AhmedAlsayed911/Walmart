using AutoMapper;
using Walmart.Application.ViewModels.Address;

namespace Walmart.Application.Mapping.Address
{
    public partial class AddressProfile
    {
        public void AddressDetailsMapping()
        {
            CreateMap<Walmart.Domain.Entities.Address, AddressDetailsVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));
        }
    }
}
