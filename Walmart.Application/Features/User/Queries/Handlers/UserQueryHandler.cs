using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Features.User.Queries.Models;
using Walmart.Application.ViewModels.UserVM;
using Walmart.Domain.Entities;

namespace Walmart.Application.Features.User.Queries.Handlers
{
    public class UserQueryHandler(UserManager<ApplicationUser> userManager)
        : IRequestHandler<GetAllUsersQuery, List<UserViewModel>>
    {
        public async Task<List<UserViewModel>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(user =>
                    user.FirstName.ToLower().Contains(search)
                    || user.LastName.ToLower().Contains(search)
                    || user.Email.ToLower().Contains(search)
                    || user.UserName.ToLower().Contains(search));
            }

            var users = await query.Select(user => new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber
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
