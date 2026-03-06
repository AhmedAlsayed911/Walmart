using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.ViewModels.Category
{
    public class AddCategoryVM
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        public int? ParentCategoryId { get; set; }
        
        public IEnumerable<SelectListItem> ParentCategories { get; set; } = new List<SelectListItem>();
    }
}
