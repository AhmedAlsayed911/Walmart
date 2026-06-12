using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Walmart.Application.Features.Category.Commands.Models;
using Walmart.Application.Features.Category.Queries.Models;
using Walmart.Application.ViewModels.Category;

namespace Walmart.Controllers
{
    public class CategoryController(IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index(int page = 1)
        {
            var categories = await mediator.Send(new GetAllCategoriesQuery
            {
                PageNumber = page,
                PageSize = 10
            });
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await mediator.Send(new GetCategoryByIdQuery { Id = id });
            if (category is null)
                return NotFound();

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
            var model = new AddCategoryVM
            {
                ParentCategories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(AddCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
                model.ParentCategories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(model);
            }

            var result = await mediator.Send(new AddCategoryCommand
            {
                Name = model.Name,
                ParentCategoryId = model.ParentCategoryId
            });

            if (!result)
            {
                ModelState.AddModelError("Name", "Category already exists!");
                var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
                model.ParentCategories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await mediator.Send(new GetCategoryByIdQuery { Id = id });
            if (category is null)
                return NotFound();

            var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
            var model = new EditCategoryVM
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategories = categories.Where(c => c.Id != id).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
                model.ParentCategories = categories.Where(c => c.Id != model.Id).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(model);
            }

            var result = await mediator.Send(new UpdateCategoryCommand
            {
                Id = model.Id,
                Name = model.Name,
                ParentCategoryId = model.ParentCategoryId
            });

            if (!result)
            {
                ModelState.AddModelError("Name", "Category already exists!");
                var categories = await mediator.Send(new GetAllCategoriesForDropdownList());
                model.ParentCategories = categories.Where(c => c.Id != model.Id).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await mediator.Send(new GetCategoryByIdQuery { Id = id });
            if (existing is null)
                return NotFound();

            var result = await mediator.Send(new DeleteCategoryCommand { Id = id });
            if (!result)
            {
                TempData["ErrorMessage"] = "This category cannot be deleted because it has products.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
