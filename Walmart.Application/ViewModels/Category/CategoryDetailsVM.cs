namespace Walmart.Application.ViewModels.Category
{
    public class CategoryDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public int ProductCount { get; set; }
        public int SubCategoryCount { get; set; }
    }
}
