using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Product.Queries.Models;
using Walmart.Application.Pagination;
using Walmart.Application.ViewModels.ProductVM;

namespace Walmart.Application.Features.Product.Queries.Handlers
{
    public class ProductQueryHandler(
        IBaseRepository<Walmart.Domain.Entities.Product> repository,
        IBaseRepository<Walmart.Domain.Entities.OrderProduct> orderProductRepository,
        IMapper mapper)
        : IRequestHandler<GetAllProductsQuery, PaginatedResult<ProductsListVM>>,
          IRequestHandler<GetProductByIdQuery, ProductDetailsVM>,
          IRequestHandler<GetTopSoldProductsQuery, List<TopSoldProductVM>>
    {
        public async Task<PaginatedResult<ProductsListVM>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var query = repository
                .GetTableNoTracking()
                .Where(p => !p.Sku.StartsWith("DELETED-"));

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search) || p.Sku.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.CategoryName))
            {
                var categoryName = request.CategoryName.Trim().ToLower();
                query = query.Where(p => p.Category.Name.ToLower() == categoryName);
            }

            if (request.IsActive.HasValue)
            {
                query = request.IsActive.Value
                    ? query.Where(p => p.StockQuantity > 0)
                    : query.Where(p => p.StockQuantity <= 0);
            }

            if (!string.IsNullOrWhiteSpace(request.StockStatus))
            {
                var stockStatus = request.StockStatus.Trim().ToLowerInvariant();
                query = stockStatus switch
                {
                    "instock" => query.Where(p => p.StockQuantity > 0),
                    "outofstock" => query.Where(p => p.StockQuantity <= 0),
                    "lowstock" => query.Where(p => p.StockQuantity > 0 && p.StockQuantity <= 10),
                    _ => query
                };
            }

            var sortBy = request.SortBy?.Trim().ToLowerInvariant();

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "quantity_asc" => query.OrderBy(p => p.StockQuantity),
                "quantity_desc" => query.OrderByDescending(p => p.StockQuantity),
                "mostsold_desc" => ApplyMostSoldSort(query),
                _ => ApplyMostSoldSort(query)
            };

            return await
                query
                .ProjectTo<ProductsListVM>(mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }

        private IQueryable<Walmart.Domain.Entities.Product> ApplyMostSoldSort(IQueryable<Walmart.Domain.Entities.Product> productsQuery)
        {
            var soldQuantities = orderProductRepository
                .GetTableNoTracking()
                .Where(op => op.Order.Status != Walmart.Application.Features.Order.OrderStatusFlow.Cancelled)
                .GroupBy(op => op.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    UnitsSold = group.Sum(op => op.Quantity)
                });

            return productsQuery
                .GroupJoin(
                    soldQuantities,
                    product => product.Id,
                    sold => sold.ProductId,
                    (product, sold) => new
                    {
                        Product = product,
                        UnitsSold = sold.Select(x => x.UnitsSold).FirstOrDefault()
                    })
                .OrderByDescending(x => x.UnitsSold)
                .ThenByDescending(x => x.Product.CreatedAt)
                .Select(x => x.Product);
        }

        public async Task<ProductDetailsVM> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await repository
                .GetTableNoTracking()
                .Include(p => p.Category)
                .Where(p => p.Id == request.Id)
                .ProjectTo<ProductDetailsVM>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return product;
        }

        public async Task<List<TopSoldProductVM>> Handle(GetTopSoldProductsQuery request, CancellationToken cancellationToken)
        {
            var take = request.Take <= 0 ? 3 : request.Take;

            return await orderProductRepository
                .GetTableNoTracking()
                .Where(op => op.Order.Status != Walmart.Application.Features.Order.OrderStatusFlow.Cancelled)
                .GroupBy(op => new
                {
                    op.ProductId,
                    op.Product.Name,
                    op.Product.Sku,
                    op.Product.Price,
                    op.Product.SalePercentage,
                    op.Product.SaleEndDate
                })
                .Select(group => new TopSoldProductVM
                {
                    ProductId = group.Key.ProductId,
                    Name = group.Key.Name,
                    Sku = group.Key.Sku,
                    CurrentPrice = group.Key.Price,
                    SalePercentage = group.Key.SalePercentage,
                    SaleEndDate = group.Key.SaleEndDate,
                    UnitsSold = group.Sum(op => op.Quantity),
                    TotalRevenue = group.Sum(op => op.LineTotal)
                })
                .OrderByDescending(item => item.UnitsSold)
                .ThenByDescending(item => item.TotalRevenue)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}
