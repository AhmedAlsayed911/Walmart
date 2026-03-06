using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Walmart.Application.Features.Role.Commands.Models
{
    public class AddRoleCommand : IRequest<bool>
    {
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}
