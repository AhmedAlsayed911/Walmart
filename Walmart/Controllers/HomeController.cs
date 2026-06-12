using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Walmart.Application.Features.Dashboard.Queries.Models;
using Walmart.Application.Features.Product.Queries.Models;
using Walmart.Application.ViewModels.ProductVM;
using Walmart.Models;

namespace Walmart.Controllers
{
    public class HomeController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var featured = await mediator.Send(new GetAllProductsQuery
            {
                PageNumber = 1,
                PageSize = 8,
                SortBy = "mostsold_desc"
            });

            return View(featured.Items.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            var model = await mediator.Send(new GetAdminDashboardQuery());
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
