using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Walmart.Application.Features.Order.Commands.Models;
using Walmart.Application.Features.Order.Queries.Models;
using Walmart.Application.ViewModels.Order;
using Walmart.Domain.Entities;

namespace Walmart.Controllers
{
    [Authorize]
    public class OrderController(IMediator mediator, UserManager<ApplicationUser> userManager) : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var orders = await mediator.Send(new GetAllOrdersQuery { PageNumber = page, PageSize = 10 });
            return View(orders);
        }

        public async Task<IActionResult> MyOrders(int page = 1)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var user = await userManager.GetUserAsync(User);
            var orders = await mediator.Send(new GetUserOrdersQuery 
            { 
                UserId = user.Id,
                PageNumber = page, 
                PageSize = 10 
            });

            return View("MyOrders", orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await mediator.Send(new GetOrderByIdQuery { Id = id });
            if (order is null)
                return NotFound();

            return View(order);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var users = userManager.Users.ToList();
            var model = new AddOrderVM
            {
                Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddOrderVM model)
        {
            if (!ModelState.IsValid)
            {
                var users = userManager.Users.ToList();
                model.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                });
                return View(model);
            }

            var result = await mediator.Send(new AddOrderCommand
            {
                UserId = model.UserId,
                ShippingAddressId = model.ShippingAddressId,
                OrderNumber = model.OrderNumber,
                Status = model.Status,
                TotalAmount = model.TotalAmount,
                ProductIds = model.ProductIds
            });

            if (!result)
            {
                ModelState.AddModelError("OrderNumber", "Order number already exists!");
                var users = userManager.Users.ToList();
                model.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                });
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await mediator.Send(new GetOrderByIdQuery { Id = id });
            if (order is null)
                return NotFound();

            var users = userManager.Users.ToList();
            var model = new EditOrderVM
            {
                Id = order.Id,
                UserId = order.UserId,
                ShippingAddressId = order.ShippingAddressId,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditOrderVM model)
        {
            if (!ModelState.IsValid)
            {
                var users = userManager.Users.ToList();
                model.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                });
                return View(model);
            }

            var result = await mediator.Send(new UpdateOrderCommand
            {
                Id = model.Id,
                UserId = model.UserId,
                ShippingAddressId = model.ShippingAddressId,
                OrderNumber = model.OrderNumber,
                Status = model.Status,
                TotalAmount = model.TotalAmount
            });

            if (!result)
            {
                ModelState.AddModelError("OrderNumber", "Order number already exists!");
                var users = userManager.Users.ToList();
                model.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                });
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await mediator.Send(new DeleteOrderCommand { Id = id });
            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
