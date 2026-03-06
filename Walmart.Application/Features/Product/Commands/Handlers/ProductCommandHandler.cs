using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Product.Commands.Models;

namespace Walmart.Application.Features.Product.Commands.Handlers
{
    public class ProductCommandHandler(IBaseRepository<Walmart.Domain.Entities.Product> repository, IMapper mapper)
        : IRequestHandler<AddProductCommand, bool>,
          IRequestHandler<UpdateProductCommand, bool>,
          IRequestHandler<DeleteProductCommand, bool>
    {
        public async Task<bool> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var exists =
                await repository
                .GetTableNoTracking()
                .AnyAsync(x => x.Name == request.Name, cancellationToken);

            if (exists)
                return false;

            var mappedProduct = mapper.Map<Walmart.Domain.Entities.Product>(request);
            if (request.ProductPicture is not null)
            {
                var file = request.ProductPicture;
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    mappedProduct.ProductPicture = dataStream.ToArray();
                }
            }

            await repository.AddAsync(mappedProduct);
            return true;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await repository.GetByIdAsync(request.Id);
            if (product is null)
                return false;

            var exists = await repository
                .GetTableNoTracking()
                .AnyAsync(x => x.Name == request.Name && x.Id != request.Id, cancellationToken);

            if (exists)
                return false;

            product.Name = request.Name;
            product.Sku = request.Sku;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.CategoryId = request.CategoryId;

            if (request.ProductPicture is not null)
            {
                using (var dataStream = new MemoryStream())
                {
                    await request.ProductPicture.CopyToAsync(dataStream, cancellationToken);
                    product.ProductPicture = dataStream.ToArray();
                }
            }

            await repository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await repository.GetByIdAsync(request.Id);
            if (product is null)
                return false;

            await repository.DeleteAsync(product);
            return true;
        }
    }
}