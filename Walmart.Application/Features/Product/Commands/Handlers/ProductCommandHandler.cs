using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Product.Commands.Models;

namespace Walmart.Application.Features.Product.Commands.Handlers
{
        public class ProductCommandHandler(
                IBaseRepository<Walmart.Domain.Entities.Product> repository,
                IBaseRepository<Walmart.Domain.Entities.OrderProduct> orderProductRepository,
                IMapper mapper)
        : IRequestHandler<AddProductCommand, bool>,
          IRequestHandler<UpdateProductCommand, bool>,
            IRequestHandler<DeleteProductCommand, bool>,
            IRequestHandler<SetProductSaleCommand, bool>
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

            var orderLines = await orderProductRepository
                .GetTableAsTracking()
                .Where(op => op.ProductId == request.Id)
                .ToListAsync(cancellationToken);

            if (orderLines.Any())
            {
                // Keep historical order lines by moving them to an archived placeholder product.
                var archivedSku = $"DELETED-{product.Id}-{Guid.NewGuid():N}";
                if (archivedSku.Length > 450)
                    archivedSku = archivedSku[..450];

                var archivedProduct = new Walmart.Domain.Entities.Product
                {
                    Name = product.Name,
                    Sku = archivedSku,
                    ProductPicture = product.ProductPicture,
                    Price = product.Price,
                    StockQuantity = 0,
                    CategoryId = product.CategoryId,
                    CreatedAt = product.CreatedAt,
                    SalePercentage = null,
                    SaleEndDate = null
                };

                await repository.AddAsync(archivedProduct);

                foreach (var line in orderLines)
                {
                    line.ProductId = archivedProduct.Id;
                }

                await orderProductRepository.UpdateRangeAsync(orderLines);
            }

            await repository.DeleteAsync(product);
            return true;
        }

        public async Task<bool> Handle(SetProductSaleCommand request, CancellationToken cancellationToken)
        {
            var product = await repository.GetByIdAsync(request.ProductId);
            if (product is null)
                return false;

            if (!request.SalePercentage.HasValue || request.SalePercentage <= 0)
            {
                product.SalePercentage = null;
                product.SaleEndDate = null;
                await repository.UpdateAsync(product);
                return true;
            }

            var saleEndDate = request.SaleEndDate ?? DateTime.UtcNow.AddDays(7);
            if (saleEndDate <= DateTime.UtcNow)
                return false;

            product.SalePercentage = request.SalePercentage;
            product.SaleEndDate = saleEndDate;

            await repository.UpdateAsync(product);
            return true;
        }
    }
}