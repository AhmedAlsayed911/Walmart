using AutoMapper;

namespace Walmart.Application.Mapping.Category
{
    public partial class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            GetProductsMapping();
            AddCategoryMapping();
            UpdateCategoryMapping();
            CategoryDetailsMapping();
        }
    }
}
