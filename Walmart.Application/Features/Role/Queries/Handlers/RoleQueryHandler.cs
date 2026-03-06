using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Features.Role.Queries.Models;

namespace Walmart.Application.Features.Role.Queries.Handlers
{
    public class RoleQueryHandler(RoleManager<IdentityRole> roleManager)
        : IRequestHandler<GetAllRolesQuery, List<IdentityRole>>
    {
        public async Task<List<IdentityRole>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await roleManager.Roles.ToListAsync();
            return roles;
        }
    }
}