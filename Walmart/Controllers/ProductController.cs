using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Walmart.Application.Features.Category.Queries.Models;
using Walmart.Application.Features.Product.Commands.Models;
using Walmart.Application.Features.Product.Queries.Models;
using Walmart.Application.ViewModels.ProductVM;
namespace Walmart.Controllers
{
    public class ProductController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index(int page = 1, string? categoryName = null, string? isActive = null, string? stockStatus = null, string? sortBy = null)
        {
            bool? activeFilter = null;
            if (string.Equals(isActive, "true", StringComparison.OrdinalIgnoreCase))
                activeFilter = true;
            else if (string.Equals(isActive, "false", StringComparison.OrdinalIgnoreCase))
                activeFilter = false;

            var result = await mediator.Send(new GetAllProductsQuery
            {
                PageNumber = page,
                PageSize = 8,
                CategoryName = categoryName,
                IsActive = activeFilter,
                StockStatus = stockStatus,
                SortBy = sortBy
            });

            var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
            ViewBag.CategoryOptions = categories.Select(c => c.Name).ToList();
            ViewBag.SelectedCategoryName = categoryName;
            ViewBag.SelectedIsActive = isActive;
            ViewBag.SelectedStockStatus = stockStatus;
            ViewBag.SortBy = sortBy;

            return View(result);
        }

        public async Task<IActionResult> Add()
        {
            var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
            var model = new AddProductVM
            {
                Categories = categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(AddProductVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await mediator.Send(new AddProductCommand
            {
                Name = model.Name,
                Sku = model.Sku,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                CategoryId = model.CategoryId,
                ProductPicture = model.ProductPicture
            });

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await mediator.Send(new GetProductByIdQuery { Id = id });
            if (product is null)
                return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await mediator.Send(new GetProductByIdQuery { Id = id });
            if (product is null)
                return NotFound();

            var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
            var model = new EditProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                ExistingProductPicture = product.ProductPicture,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditProductVM model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(model);
            }

            var result = await mediator.Send(new UpdateProductCommand
            {
                Id = model.Id,
                Name = model.Name,
                Sku = model.Sku,
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                CategoryId = model.CategoryId,
                ProductPicture = model.ProductPicture,
                ExistingProductPicture = model.ExistingProductPicture
            });

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await mediator.Send(new GetProductByIdQuery { Id = id });
            if (product is null)
                return NotFound();

            var result = await mediator.Send(new DeleteProductCommand { Id = id });
            if (!result)
            {
                TempData["Error"] = "This product cannot be deleted because it is already included in one or more orders.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sale(int id)
        {
            var product = await mediator.Send(new GetProductByIdQuery { Id = id });
            if (product is null)
                return NotFound();

            var model = new ProductSaleVM
            {
                ProductId = product.Id,
                ProductName = product.Name,
                BasePrice = product.Price,
                SalePercentage = product.SalePercentage,
                SaleEndDate = product.SaleEndDate
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sale(ProductSaleVM model)
        {
            if (model.SaleEndDate.HasValue && model.SaleEndDate.Value <= DateTime.UtcNow)
                ModelState.AddModelError(nameof(model.SaleEndDate), "Sale end date must be in the future.");

            if (!ModelState.IsValid)
                return View(model);

            var result = await mediator.Send(new SetProductSaleCommand
            {
                ProductId = model.ProductId,
                SalePercentage = model.SalePercentage,
                SaleEndDate = model.SaleEndDate
            });

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Could not save sale details.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Product sale was updated successfully.";
            return RedirectToAction(nameof(Details), new { id = model.ProductId });
        }

        [Authorize(Roles = "Admin,StoreManager,SupportAgent")]
        public async Task<IActionResult> TopSold()
        {
            var model = await mediator.Send(new GetTopSoldProductsQuery { Take = 3 });
            return View(model);
        }

    }
}
