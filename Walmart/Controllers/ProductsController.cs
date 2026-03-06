using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Domain.Entities;

namespace Walmart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IBaseRepository<Product> repository) : ControllerBase
    {
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await repository.GetTableNoTracking()
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Sku,
                    p.Price,
                    p.StockQuantity,
                    p.CategoryId,
                    ProductPicture = p.ProductPicture != null ? Convert.ToBase64String(p.ProductPicture) : null
                })
                .ToListAsync();

            return Ok(products);
        }
    }
}
