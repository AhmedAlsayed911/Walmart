using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Walmart.Application.Features.Role.Commands.Models;
using Walmart.Application.Features.Role.Queries.Models;
using Walmart.Application.ViewModels.RoleVM;

namespace Walmart.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var roles = await mediator.Send(new GetAllRolesQuery());
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleFormViewModel model)
        {
            var roles = await mediator.Send(new GetAllRolesQuery());

            if (!ModelState.IsValid)
                return View("Index", roles);

            var command = new AddRoleCommand() { Name = model.Name };

            var result = await mediator.Send(command);
            if (!result)
            {
                ModelState.AddModelError("Name", "Role already Exists!");
                return View("Index", roles);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
