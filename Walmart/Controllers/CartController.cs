using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.ViewModels.Cart;
using Walmart.Domain.Entities;

namespace Walmart.Controllers
{
    [Authorize(Roles = "Client")]
    public class CartController(
        IBaseRepository<Product> productRepository,
        IBaseRepository<Address> addressRepository,
        IBaseRepository<Order> orderRepository,
        IBaseRepository<OrderProduct> orderProductRepository,
        UserManager<ApplicationUser> userManager) : Controller
    {
        private const string CartSessionKey = "ShoppingCart";
        private const decimal TaxRate = 0.1m;

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await productRepository.GetByIdAsync(productId);
            if (product == null || product.StockQuantity < quantity)
                return Json(new { success = false, message = "Product not available" });

            var cart = GetCart();
            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem != null)
            {
                if (existingItem.Quantity + quantity > product.StockQuantity)
                    return Json(new { success = false, message = $"Only {product.StockQuantity} unit(s) available" });

                existingItem.Quantity += quantity;
                existingItem.Price = product.Price;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ProductPicture = product.ProductPicture
                });
            }

            SaveCart(cart);
            return Json(new { success = true, message = "Product added to cart", cartCount = cart.Items.Sum(x => x.Quantity) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                if (quantity > 0)
                {
                    var product = await productRepository.GetByIdAsync(productId);
                    if (product is null)
                    {
                        cart.Items.Remove(item);
                    }
                    else
                    {
                        if (product.StockQuantity < quantity)
                        {
                            return Json(new
                            {
                                success = false,
                                message = $"Only {product.StockQuantity} unit(s) available.",
                                cartCount = cart.Items.Sum(x => x.Quantity),
                                subTotal = cart.SubTotal,
                                grandTotal = cart.GrandTotal
                            });
                        }

                        item.Quantity = quantity;
                        item.Price = product.Price;
                    }
                }
                else
                    cart.Items.Remove(item);

                SaveCart(cart);
            }

            return Json(new { success = true, cartCount = cart.Items.Sum(x => x.Quantity), subTotal = cart.SubTotal, grandTotal = cart.GrandTotal });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCart(cart);
            }

            return Json(new { success = true, cartCount = cart.Items.Sum(x => x.Quantity) });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return Json(new { success = true });
        }

        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            if (!cart.Items.Any())
                return RedirectToAction(nameof(Index));

            var user = await userManager.GetUserAsync(User);
            var userAddresses = await addressRepository.GetTableNoTracking()
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            ViewBag.Addresses = userAddresses;
            ViewBag.UserId = user.Id;

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteCheckout(int addressId)
        {
            var cart = GetCart();
            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.GetUserAsync(User);

            var address = await addressRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == user.Id);

            if (address == null)
            {
                TempData["ErrorMessage"] = "Invalid shipping address!";
                return RedirectToAction(nameof(Checkout));
            }

            var productsToUpdate = new List<Product>();
            var orderLines = new List<OrderProduct>();
            decimal subtotal = 0m;

            foreach (var cartItem in cart.Items)
            {
                var product = await productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null || product.StockQuantity < cartItem.Quantity)
                {
                    TempData["ErrorMessage"] = $"Sorry, {cartItem.ProductName} is no longer available in the requested quantity.";
                    return RedirectToAction(nameof(Checkout));
                }

                var lineTotal = product.Price * cartItem.Quantity;
                subtotal += lineTotal;

                orderLines.Add(new OrderProduct
                {
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price,
                    LineTotal = lineTotal
                });

                product.StockQuantity -= cartItem.Quantity;
                productsToUpdate.Add(product);
            }

            var totalAmount = subtotal + (subtotal * TaxRate);

            using var transaction = orderRepository.BeginTransaction();
            try
            {
                var order = new Order
                {
                    UserId = user.Id,
                    ShippingAddressId = addressId,
                    OrderNumber = $"WM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    Status = 0,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount
                };

                await orderRepository.AddAsync(order);

                foreach (var line in orderLines)
                {
                    line.OrderId = order.Id;
                }

                await orderProductRepository.AddRangeAsync(orderLines);
                await productRepository.UpdateRangeAsync(productsToUpdate);

                orderRepository.Commit();

                TempData["SuccessMessage"] = $"Order placed successfully! Order Number: {order.OrderNumber}";
                HttpContext.Session.Remove(CartSessionKey);
            }
            catch
            {
                orderRepository.RollBack();
                TempData["ErrorMessage"] = "Could not place order right now. Please try again.";
                return RedirectToAction(nameof(Checkout));
            }

            return RedirectToAction("MyOrders", "Order");
        }

        private CartVM GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new CartVM();

            return JsonConvert.DeserializeObject<CartVM>(cartJson) ?? new CartVM();
        }

        private void SaveCart(CartVM cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(new { count = cart.Items.Sum(x => x.Quantity) });
        }
    }
}
