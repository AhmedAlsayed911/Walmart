using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.Features.Category.Commands.Models
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        public int? ParentCategoryId { get; set; }
    }
}
