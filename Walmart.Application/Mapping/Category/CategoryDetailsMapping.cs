using AutoMapper;
using Walmart.Application.ViewModels.Category;

namespace Walmart.Application.Mapping.Category
{
    public partial class CategoryProfile
    {
        public void CategoryDetailsMapping()
        {
            CreateMap<Walmart.Domain.Entities.Category, CategoryDetailsVM>()
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count))
                .ForMember(dest => dest.SubCategoryCount, opt => opt.MapFrom(src => src.SubCategories.Count));
        }
    }
}
