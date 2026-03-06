using AutoMapper;

namespace Walmart.Application.Mapping.Product
{
    public partial class ProductProfile : Profile
    {
        public ProductProfile()
        {
            GetAllProductsMapping();
            AddProductCommand();
            UpdateProductMapping();
            ProductDetailsMapping();
        }
    }
}
