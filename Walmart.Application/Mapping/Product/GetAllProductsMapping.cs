using Walmart.Application.ViewModels.ProductVM;

namespace Walmart.Application.Mapping.Product
{
    public partial class ProductProfile
    {
        public void GetAllProductsMapping()
        {
            CreateMap<Walmart.Domain.Entities.Product, ProductsListVM>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(x => x.Category.Name));

        }
    }
}
