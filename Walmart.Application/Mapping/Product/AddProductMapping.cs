using Walmart.Application.Features.Product.Commands.Models;

namespace Walmart.Application.Mapping.Product
{
    public partial class ProductProfile
    {
        public void AddProductCommand()
        {
            CreateMap<AddProductCommand, Walmart.Domain.Entities.Product>()
                .ForMember(dest => dest.ProductPicture, opt => opt.Ignore());
        }
    }
}
