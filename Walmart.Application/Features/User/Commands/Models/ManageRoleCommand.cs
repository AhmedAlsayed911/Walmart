using MediatR;
using Walmart.Application.ViewModels.UserVM;

namespace Walmart.Application.Features.User.Commands.Models
{
    public class ManageRoleCommand : IRequest<UserRolesViewModel>
    {
        public string Id { get; set; }
    }
}
