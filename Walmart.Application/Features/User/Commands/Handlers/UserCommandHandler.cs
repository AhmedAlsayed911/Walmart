using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Features.User.Commands.Models;
using Walmart.Application.ViewModels.RoleVM;
using Walmart.Application.ViewModels.UserVM;
using Walmart.Domain.Entities;

namespace Walmart.Application.Features.User.Commands.Handlers
{
    public class UserCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        : IRequestHandler<ManageRoleCommand, UserRolesViewModel>
        , IRequestHandler<UpdateUserRolesCommand>
    {
        public async Task<UserRolesViewModel> Handle(ManageRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user is null)
                return null;

            var roles = await roleManager.Roles.ToListAsync();

            var result = new UserRolesViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = roles.Select(role => new RoleViewModel
                {
                    RoleName = role.Name,
                    IsSelected = userManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };

            return result;
        }

        public async Task Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);

            if (user is null)
                throw new Exception("User not found");

            var currentRoles = await userManager.GetRolesAsync(user);

            foreach (var role in request.Roles)
            {
                if (currentRoles.Contains(role.RoleName) && !role.IsSelected)
                    await userManager.RemoveFromRoleAsync(user, role.RoleName);

                if (!currentRoles.Contains(role.RoleName) && role.IsSelected)
                    await userManager.AddToRoleAsync(user, role.RoleName);
            }
        }
    }
}
