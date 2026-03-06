using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Features.User.Queries.Models;
using Walmart.Application.ViewModels.UserVM;
using Walmart.Domain.Entities;

namespace Walmart.Application.Features.User.Queries.Handlers
{
    public class UserQueryHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        : IRequestHandler<GetAllUsersQuery, List<UserViewModel>>
    {
        public async Task<List<UserViewModel>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await userManager.Users.Select(user => new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.UserName,
            }).ToListAsync();

            foreach (var user in users)
            {
                var entity = await userManager.FindByIdAsync(user.Id);
                user.Roles = await userManager.GetRolesAsync(entity);
            }

            return users;
        }
    }
}
