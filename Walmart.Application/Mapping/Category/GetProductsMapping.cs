using Walmart.Application.ViewModels.Category;

namespace Walmart.Application.Mapping.Category
{
    public partial class CategoryProfile
    {
        public void GetProductsMapping()
        {
            CreateMap<Walmart.Domain.Entities.Category, CategoryListVM>();
        }
    }
}
