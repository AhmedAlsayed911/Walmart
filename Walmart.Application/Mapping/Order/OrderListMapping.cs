using AutoMapper;
using Walmart.Application.ViewModels.Order;

namespace Walmart.Application.Mapping.Order
{
    public partial class OrderProfile
    {
        public void OrderListMapping()
        {
            CreateMap<Walmart.Domain.Entities.OrderProduct, OrderListItemVM>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : "Deleted Product"))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice));

            CreateMap<Walmart.Domain.Entities.Order, OrderListVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderProducts))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.OrderProducts.Sum(op => op.Quantity)));
        }
    }
}
