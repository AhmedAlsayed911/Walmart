using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Walmart.Application.Features.User.Commands.Models;
using Walmart.Application.Features.User.Queries.Models;
using Walmart.Application.ViewModels.UserVM;
using Walmart.Domain.Entities;

namespace Walmart.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController(IMediator mediator, UserManager<ApplicationUser> userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var users = await mediator.Send(new GetAllUsersQuery());
            return View(users);
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();

            var addresses = await mediator.Send(new Walmart.Application.Features.Address.Queries.Models.GetAllAddressesQuery
            {
                PageNumber = 1,
                PageSize = 1000
            });

            ViewBag.UserAddresses = addresses.Items.Where(a => a.UserId == user.Id).ToList();

            var model = new EditUserVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Username;
            user.PhoneNumber = model.PhoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            var viewModel = await mediator.Send(new ManageRoleCommand { Id = userId });
            if (viewModel is null)
                return NotFound("Something went Wrong!!");

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
        {
            var command = new UpdateUserRolesCommand
            {
                UserId = model.UserId,
                Roles = model.Roles
            };

            var success = await mediator.Send(command);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Role update failed. Make sure at least one role is selected and you are not removing the last Admin.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
