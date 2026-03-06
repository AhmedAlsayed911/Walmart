using MediatR;
using Microsoft.AspNetCore.Identity;
using Walmart.Application.Features.Role.Commands.Models;

namespace Walmart.Application.Features.Role.Commands.Handlers
{
    public class RoleCommandHandler(RoleManager<IdentityRole> roleManager) : IRequestHandler<AddRoleCommand, bool>
    {
        public async Task<bool> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            var checkRole = await roleManager.RoleExistsAsync(request.Name);
            if (checkRole)
                return false;

            await roleManager.CreateAsync(new IdentityRole(request.Name.Trim()));
            return true;
        }
    }
}
