using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.Features.Category.Commands.Models
{
    public class AddCategoryCommand : IRequest<bool>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        public int? ParentCategoryId { get; set; }
    }
}
