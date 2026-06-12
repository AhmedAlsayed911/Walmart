using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Walmart.Application.Features.Address.Queries.Models;
using Walmart.Application.Features.Order.Commands.Models;
using Walmart.Application.Features.Order.Queries.Models;
using Walmart.Application.ViewModels.Order;
using Walmart.Domain.Entities;
using Walmart.Services;

namespace Walmart.Controllers
{
    [Authorize]
    public class OrderController(IMediator mediator, UserManager<ApplicationUser> userManager, IClientOrderNotificationService notificationService) : Controller
    {
        private const int EditWindowMinutes = 10;

        [Authorize(Roles = "Admin,StoreManager,SupportAgent")]
        public async Task<IActionResult> Index(int page = 1, int? orderId = null, int? status = null, string? sortBy = null)
        {
            var orders = await mediator.Send(new GetAllOrdersQuery
            {
                PageNumber = page,
                PageSize = 10,
                OrderId = orderId,
                Status = status,
                SortBy = sortBy
            });

            ViewBag.OrderId = orderId;
            ViewBag.Status = status;
            ViewBag.SortBy = sortBy;

            foreach (var order in orders.Items)
                order.CanEdit = IsWithinEditWindow(order.OrderDate);

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

            foreach (var order in orders.Items)
                order.CanEdit = IsWithinEditWindow(order.OrderDate);

            var notices = notificationService.TakeMessages(user.Id);
            if (notices.Any())
                TempData["OrderNotice"] = string.Join("<br/>", notices);

            return View("MyOrders", orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await mediator.Send(new GetOrderByIdQuery { Id = id });
            if (order is null)
                return NotFound();

            var isPrivileged = User.IsInRole("Admin") || User.IsInRole("StoreManager") || User.IsInRole("SupportAgent");
            var currentUser = await userManager.GetUserAsync(User);
            if (!isPrivileged && currentUser?.Id != order.UserId)
                return Forbid();

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
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
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
                ModelState.AddModelError(string.Empty, "Order update failed. Ensure the order number is unique and status transition is valid.");
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
            var addresses = await GetAddressOptionsAsync(order.UserId, order.ShippingAddressId);
            var products = await mediator.Send(new Walmart.Application.Features.Product.Queries.Models.GetAllProductsQuery
            {
                PageNumber = 1,
                PageSize = 1000,
                SortBy = "mostsold_desc"
            });

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
                }),
                Addresses = addresses,
                ProductIds = order.Products.Select(p => p.ProductId).ToList(),
                Products = products.Items.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Sku})"
                })
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditOrderVM model)
        {
            var currentOrder = await mediator.Send(new GetOrderByIdQuery { Id = model.Id });
            if (currentOrder is null)
                return NotFound();

            if (model.ProductIds is null || !model.ProductIds.Any())
                ModelState.AddModelError(nameof(model.ProductIds), "Please select at least one product.");

            if (!ModelState.IsValid)
            {
                var users = userManager.Users.ToList();
                var addresses = await GetAddressOptionsAsync(model.UserId, model.ShippingAddressId);
                var products = await mediator.Send(new Walmart.Application.Features.Product.Queries.Models.GetAllProductsQuery
                {
                    PageNumber = 1,
                    PageSize = 1000,
                    SortBy = "mostsold_desc"
                });
                model.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                });
                model.Addresses = addresses;
                model.Products = products.Items.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Sku})"
                });
                return View(model);
            }

            var allowedAddressIds = await mediator.Send(new GetAllAddressesQuery { PageNumber = 1, PageSize = 1000 });
            var validAddress = allowedAddressIds.Items.Any(a => a.UserId == model.UserId && a.Id == model.ShippingAddressId);
            if (!validAddress)
            {
                ModelState.AddModelError(nameof(model.ShippingAddressId), "Please choose one of your saved addresses.");
                var users = userManager.Users.ToList();
                var products = await mediator.Send(new Walmart.Application.Features.Product.Queries.Models.GetAllProductsQuery
                {
                    PageNumber = 1,
                    PageSize = 1000,
                    SortBy = "mostsold_desc"
                });
                model.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                });
                model.Addresses = await GetAddressOptionsAsync(model.UserId, model.ShippingAddressId);
                model.Products = products.Items.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Sku})"
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
                TotalAmount = model.TotalAmount,
                ProductIds = model.ProductIds
            });

            if (!result)
            {
                ModelState.AddModelError("OrderNumber", "Order number already exists!");
                var users = userManager.Users.ToList();
                var products = await mediator.Send(new Walmart.Application.Features.Product.Queries.Models.GetAllProductsQuery
                {
                    PageNumber = 1,
                    PageSize = 1000,
                    SortBy = "mostsold_desc"
                });
                model.Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                });
                model.Addresses = await GetAddressOptionsAsync(model.UserId, model.ShippingAddressId);
                model.Products = products.Items.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Sku})"
                });
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool refundConfirmed, string deletionReason)
        {
            var order = await mediator.Send(new GetOrderByIdQuery { Id = id });
            if (order is null)
                return NotFound();

            if (!CanDeleteOrder(order))
            {
                TempData["ErrorMessage"] = "Only pending orders inside the edit window can be deleted.";
                return RedirectToAction(nameof(Index));
            }

            if (!refundConfirmed || string.IsNullOrWhiteSpace(deletionReason))
            {
                TempData["ErrorMessage"] = "Before deleting an order, you must confirm customer deposit refund and provide a deletion reason.";
                return RedirectToAction(nameof(Index));
            }

            var result = await mediator.Send(new DeleteOrderCommand { Id = id });
            if (!result)
                return NotFound();

            notificationService.AddMessage(
                order.UserId,
                $"Order {order.OrderNumber} was cancelled by admin. Reason: {deletionReason.Trim()}. Your deposit refund has been initiated.");

            TempData["SuccessMessage"] = "Order deleted after refund confirmation. The customer has been notified with the reason.";

            return RedirectToAction(nameof(Index));
        }

        private static bool IsWithinEditWindow(DateTime orderDate)
            => DateTime.UtcNow <= orderDate.AddMinutes(EditWindowMinutes);

        private static bool CanDeleteOrder(OrderDetailsVM order)
            => order.Status == Walmart.Application.Features.Order.OrderStatusFlow.Pending
               && IsWithinEditWindow(order.OrderDate);

        private bool CanEditOrder(OrderDetailsVM order, ApplicationUser? currentUser)
        {
            if (currentUser is null)
                return false;

            var isAdmin = User.IsInRole("Admin");
            var isOwner = currentUser.Id == order.UserId;

            if (!isAdmin && !isOwner)
                return false;

            return IsWithinEditWindow(order.OrderDate);
        }

        private async Task<IEnumerable<SelectListItem>> GetAddressOptionsAsync(string userId, int selectedAddressId)
        {
            var addresses = await mediator.Send(new GetAllAddressesQuery { PageNumber = 1, PageSize = 1000 });
            return addresses.Items
                .Where(a => a.UserId == userId)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Street}, {a.City}, {a.Country} {(a.IsDefault ? "(Default)" : string.Empty)}",
                    Selected = a.Id == selectedAddressId
                });
        }
    }
}
