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
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OrderProducts));

            CreateMap<Walmart.Domain.Entities.OrderProduct, OrderProductVM>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Deleted Product"))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.LineTotal));
        }
    }
}
