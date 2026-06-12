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
        , IRequestHandler<UpdateUserRolesCommand, bool>
    {
        public async Task<UserRolesViewModel> Handle(ManageRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user is null)
                return null;

            var roles = await roleManager.Roles
                .Where(r => r.Name != "Admin")
                .ToListAsync();

            var result = new UserRolesViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Roles = new List<RoleViewModel>()
            };

            foreach (var role in roles)
            {
                result.Roles.Add(new RoleViewModel
                {
                    RoleName = role.Name,
                    IsSelected = await userManager.IsInRoleAsync(user, role.Name)
                });
            }

            return result;
        }

        public async Task<bool> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);

            if (user is null)
                return false;

            if (request.Roles is null || !request.Roles.Any(r => r.IsSelected))
                return false;

            var currentRoles = await userManager.GetRolesAsync(user);
            var selectedRoles = request.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToHashSet();

            var removingAdmin = currentRoles.Contains("Admin") && !selectedRoles.Contains("Admin");
            if (removingAdmin)
            {
                var adminRoleUsers = await userManager.GetUsersInRoleAsync("Admin");
                var activeAdminsCount = adminRoleUsers.Count;
                if (activeAdminsCount <= 1)
                    return false;
            }

            foreach (var role in request.Roles)
            {
                if (role.RoleName == "Admin")
                    continue;

                if (currentRoles.Contains(role.RoleName) && !role.IsSelected)
                    await userManager.RemoveFromRoleAsync(user, role.RoleName);

                if (!currentRoles.Contains(role.RoleName) && role.IsSelected)
                    await userManager.AddToRoleAsync(user, role.RoleName);
            }

            return true;
        }
    }
}
