using AutoMapper;
using Walmart.Application.Features.Product.Commands.Models;

namespace Walmart.Application.Mapping.Product
{
    public partial class ProductProfile
    {
        public void UpdateProductMapping()
        {
            CreateMap<UpdateProductCommand, Walmart.Domain.Entities.Product>();
        }
    }
}
