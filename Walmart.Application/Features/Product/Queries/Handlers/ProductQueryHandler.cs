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
    public class ProductQueryHandler(IBaseRepository<Walmart.Domain.Entities.Product> repository, IMapper mapper)
        : IRequestHandler<GetAllProductsQuery, PaginatedResult<ProductsListVM>>,
          IRequestHandler<GetProductByIdQuery, ProductDetailsVM>
    {
        public async Task<PaginatedResult<ProductsListVM>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await
                repository
                .GetTableNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ProjectTo<ProductsListVM>(mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
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
    }
}
