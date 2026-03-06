using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Category.Commands.Models;

namespace Walmart.Application.Features.Category.Commands.Handlers
{
    public class CategoryCommandHandler(IBaseRepository<Walmart.Domain.Entities.Category> repository, IMapper mapper)
        : IRequestHandler<AddCategoryCommand, bool>,
          IRequestHandler<UpdateCategoryCommand, bool>,
          IRequestHandler<DeleteCategoryCommand, bool>
    {
        public async Task<bool> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var exists = await repository
                .GetTableNoTracking()
                .AnyAsync(x => x.Name == request.Name, cancellationToken);

            if (exists)
                return false;

            var category = mapper.Map<Walmart.Domain.Entities.Category>(request);
            await repository.AddAsync(category);
            return true;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await repository.GetByIdAsync(request.Id);
            if (category is null)
                return false;

            var exists = await repository
                .GetTableNoTracking()
                .AnyAsync(x => x.Name == request.Name && x.Id != request.Id, cancellationToken);

            if (exists)
                return false;

            category.Name = request.Name;
            category.ParentCategoryId = request.ParentCategoryId;

            await repository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await repository.GetByIdAsync(request.Id);
            if (category is null)
                return false;

            await repository.DeleteAsync(category);
            return true;
        }
    }
}
