using AutoMapper;
using Walmart.Application.ViewModels.ProductVM;

namespace Walmart.Application.Mapping.Product
{
    public partial class ProductProfile
    {
        public void ProductDetailsMapping()
        {
            CreateMap<Walmart.Domain.Entities.Product, ProductDetailsVM>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}
