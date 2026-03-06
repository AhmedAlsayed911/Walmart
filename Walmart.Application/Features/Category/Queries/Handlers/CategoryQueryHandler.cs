using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Category.Queries.Models;
using Walmart.Application.Pagination;
using Walmart.Application.ViewModels.Category;

namespace Walmart.Application.Features.Category.Queries.Handlers
{
    public class CategoryQueryHandler(IBaseRepository<Walmart.Domain.Entities.Category> repository, IMapper mapper)
        : IRequestHandler<GetAllCategoriesQuery, PaginatedResult<CategoryListVM>>,
          IRequestHandler<GetAllCategoriesForDropdownList, List<CategoryListVM>>,
          IRequestHandler<GetCategoryByIdQuery, CategoryDetailsVM>
    {
        public async Task<PaginatedResult<CategoryListVM>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await
                repository
                .GetTableNoTracking()
                .ProjectTo<CategoryListVM>(mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }

        public async Task<List<CategoryListVM>> Handle(GetAllCategoriesForDropdownList request, CancellationToken cancellationToken)
        {
            return await
                repository
                .GetTableNoTracking()
                .ProjectTo<CategoryListVM>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<CategoryDetailsVM> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await repository
                .GetTableNoTracking()
                .Include(c => c.ParentCategory)
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .Where(c => c.Id == request.Id)
                .ProjectTo<CategoryDetailsVM>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return category;
        }
    }
}
