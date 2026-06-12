using MediatR;
using Walmart.Application.ViewModels.RoleVM;

namespace Walmart.Application.Features.User.Commands.Models
{
    public class UpdateUserRolesCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }
}
