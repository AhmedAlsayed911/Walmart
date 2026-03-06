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
        public async Task<IActionResult> Index(int page = 1)
        {
            var result = await mediator.Send(new GetAllProductsQuery { PageNumber = page, PageSize = 2 });

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
            var result = await mediator.Send(new DeleteProductCommand { Id = id });
            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

    }
}
