using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.ViewModels.Address
{
    public class EditAddressVM
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Country { get; set; }
        
        [Required]
        [StringLength(100)]
        public string City { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Street { get; set; }
        
        [Required]
        [StringLength(20)]
        public string ZIP { get; set; }
        
        public bool IsDefault { get; set; }
        
        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    }
}
