using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Walmart.Application.Features.Address.Commands.Models;
using Walmart.Application.Features.Address.Queries.Models;
using Walmart.Application.ViewModels.Address;
using Walmart.Domain.Entities;

namespace Walmart.Controllers
{
    [Authorize]
    public class AddressController(IMediator mediator, UserManager<ApplicationUser> userManager) : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var addresses = await mediator.Send(new GetAllAddressesQuery { PageNumber = page, PageSize = 10 });
            return View(addresses);
        }

        [AllowAnonymous]
        public async Task<IActionResult> MyAddresses()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var user = await userManager.GetUserAsync(User);
            var addresses = await mediator.Send(new GetAllAddressesQuery { PageNumber = 1, PageSize = 100 });
            addresses.Items = addresses.Items.Where(a => a.UserId == user.Id).ToList();

            return View("Index", addresses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var address = await mediator.Send(new GetAddressByIdQuery { Id = id });
            if (address is null)
                return NotFound();

            var currentUser = await userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && currentUser?.Id != address.UserId)
                return Forbid();

            return View(address);
        }

        public async Task<IActionResult> Add(string returnUrl = null)
        {
            var user = await userManager.GetUserAsync(User);
            var model = new AddAddressVM();

            if (User.IsInRole("Admin"))
            {
                var users = userManager.Users.ToList();
                model.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                });
            }
            else
            {
                model.UserId = user.Id;
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddAddressVM model, string returnUrl = null)
        {
            var user = await userManager.GetUserAsync(User);

            if (!User.IsInRole("Admin"))
            {
                model.UserId = user.Id;
            }

            if (!ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    var users = userManager.Users.ToList();
                    model.Users = users.Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = u.Email
                    });
                }
                return View(model);
            }

            var result = await mediator.Send(new AddAddressCommand
            {
                UserId = model.UserId,
                Country = model.Country,
                City = model.City,
                Street = model.Street,
                ZIP = model.ZIP,
                IsDefault = model.IsDefault
            });

            if (!string.IsNullOrEmpty(returnUrl) && returnUrl == "checkout")
            {
                return RedirectToAction("Checkout", "Cart");
            }

            return User.IsInRole("Admin") ? RedirectToAction(nameof(Index)) : RedirectToAction("MyAddresses");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var address = await mediator.Send(new GetAddressByIdQuery { Id = id });
            if (address is null)
                return NotFound();

            var currentUser = await userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && currentUser?.Id != address.UserId)
                return Forbid();

            var users = userManager.Users.ToList();
            var model = new EditAddressVM
            {
                Id = address.Id,
                UserId = address.UserId,
                Country = address.Country,
                City = address.City,
                Street = address.Street,
                ZIP = address.ZIP,
                IsDefault = address.IsDefault,
                Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                })
            };

            if (!User.IsInRole("Admin"))
                model.Users = users.Where(u => u.Id == address.UserId).Select(u => new SelectListItem { Value = u.Id, Text = u.Email });

            ViewBag.IsAdminEdit = User.IsInRole("Admin");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditAddressVM model)
        {
            var existing = await mediator.Send(new GetAddressByIdQuery { Id = model.Id });
            if (existing is null)
                return NotFound();

            var currentUser = await userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && currentUser?.Id != existing.UserId)
                return Forbid();

            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin)
                model.UserId = existing.UserId;

            if (!ModelState.IsValid)
            {
                var users = userManager.Users.ToList();
                model.Users = (isAdmin ? users : users.Where(u => u.Id == existing.UserId)).Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                });

                ViewBag.IsAdminEdit = isAdmin;
                return View(model);
            }

            var result = await mediator.Send(new UpdateAddressCommand
            {
                Id = model.Id,
                UserId = model.UserId,
                Country = model.Country,
                City = model.City,
                Street = model.Street,
                ZIP = model.ZIP,
                IsDefault = model.IsDefault
            });

            if (!result)
                return NotFound();

            return RedirectToAction(isAdmin ? nameof(Index) : nameof(MyAddresses));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            TempData["ErrorMessage"] = "Address deletion is disabled. Addresses can be edited but not deleted.";
            return RedirectToAction(User.IsInRole("Admin") ? nameof(Index) : nameof(MyAddresses));
        }
    }
}
