using AutoMapper;

namespace Walmart.Application.Mapping.Order
{
    public partial class OrderProfile : Profile
    {
        public OrderProfile()
        {
            OrderListMapping();
            OrderDetailsMapping();
            AddOrderMapping();
            UpdateOrderMapping();
        }
    }
}
