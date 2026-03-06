using AutoMapper;
using Walmart.Application.ViewModels.Order;

namespace Walmart.Application.Mapping.Order
{
    public partial class OrderProfile
    {
        public void OrderDetailsMapping()
        {
            CreateMap<Walmart.Domain.Entities.Order, OrderDetailsVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => 
                    $"{src.Address.Street}, {src.Address.City}, {src.Address.Country} {src.Address.ZIP}"))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

            CreateMap<Walmart.Domain.Entities.Product, OrderProductVM>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name));
        }
    }
}
