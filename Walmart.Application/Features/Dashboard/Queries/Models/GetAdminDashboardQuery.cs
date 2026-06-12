using MediatR;
using Walmart.Application.ViewModels.Dashboard;

namespace Walmart.Application.Features.Dashboard.Queries.Models
{
    public class GetAdminDashboardQuery : IRequest<AdminDashboardVM>
    {
    }
}
