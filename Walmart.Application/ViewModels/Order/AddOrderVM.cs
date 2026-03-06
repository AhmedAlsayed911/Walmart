using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.ViewModels.Order
{
    public class AddOrderVM
    {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public int ShippingAddressId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }
        
        public int Status { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        
        public List<int> ProductIds { get; set; } = new();
        
        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Addresses { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Products { get; set; } = new List<SelectListItem>();
    }
}
