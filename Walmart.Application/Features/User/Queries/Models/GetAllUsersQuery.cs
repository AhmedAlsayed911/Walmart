using MediatR;
using Walmart.Application.ViewModels.UserVM;

namespace Walmart.Application.Features.User.Queries.Models
{
    public class GetAllUsersQuery : IRequest<List<UserViewModel>>
    {
    }
}
