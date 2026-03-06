using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Walmart.Application.Features.Role.Queries.Models
{
    public class GetAllRolesQuery : IRequest<List<IdentityRole>>
    {
    }
}
