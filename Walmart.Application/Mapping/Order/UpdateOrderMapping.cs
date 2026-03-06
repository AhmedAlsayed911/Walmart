using AutoMapper;
using Walmart.Application.Features.Order.Commands.Models;

namespace Walmart.Application.Mapping.Order
{
    public partial class OrderProfile
    {
        public void UpdateOrderMapping()
        {
            CreateMap<UpdateOrderCommand, Walmart.Domain.Entities.Order>();
        }
    }
}
