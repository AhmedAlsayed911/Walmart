using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.Features.Order.Commands.Models
{
    public class UpdateOrderCommand : IRequest<bool>
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public int ShippingAddressId { get; set; }
        
        [Required]
        public string OrderNumber { get; set; }
        
        public int Status { get; set; }
        
        public decimal TotalAmount { get; set; }

        public List<int> ProductIds { get; set; } = new();
    }
}
