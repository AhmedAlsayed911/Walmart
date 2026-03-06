using AutoMapper;
using Walmart.Application.ViewModels.Order;

namespace Walmart.Application.Mapping.Order
{
    public partial class OrderProfile
    {
        public void OrderListMapping()
        {
            CreateMap<Walmart.Domain.Entities.Order, OrderListVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
        }
    }
}
