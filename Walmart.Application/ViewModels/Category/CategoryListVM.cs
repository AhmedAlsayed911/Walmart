using Walmart.Domain.Entities;

namespace Walmart.Application.ViewModels.Category
{
    public class CategoryListVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
