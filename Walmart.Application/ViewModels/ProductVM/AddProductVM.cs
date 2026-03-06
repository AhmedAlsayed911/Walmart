using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Walmart.Application.ViewModels.ProductVM
{
    public class AddProductVM
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }

        public IFormFile? ProductPicture { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
