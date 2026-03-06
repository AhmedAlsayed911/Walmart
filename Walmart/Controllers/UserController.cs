using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Walmart.Application.Features.User.Commands.Models;
using Walmart.Application.Features.User.Queries.Models;
using Walmart.Application.ViewModels.UserVM;

namespace Walmart.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var users = await mediator.Send(new GetAllUsersQuery());
            return View(users);
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            var viewModel = await mediator.Send(new ManageRoleCommand { Id = userId });
            if (viewModel is null)
                return NotFound("Something went Wrong!!");

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
        {
            var command = new UpdateUserRolesCommand
            {
                UserId = model.UserId,
                Roles = model.Roles
            };

            await mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }
    }
}
