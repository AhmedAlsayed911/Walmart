using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Order.Commands.Models;
using Walmart.Application.Features.Order;
using Walmart.Domain.Entities;

namespace Walmart.Application.Features.Order.Commands.Handlers
{
    public class OrderCommandHandler(
        IBaseRepository<Walmart.Domain.Entities.Order> repository,
        IBaseRepository<OrderProduct> orderProductRepository,
        IBaseRepository<Walmart.Domain.Entities.Product> productRepository,
        IMapper mapper)
        : IRequestHandler<AddOrderCommand, bool>,
          IRequestHandler<UpdateOrderCommand, bool>,
          IRequestHandler<DeleteOrderCommand, bool>
    {
        private const decimal TaxRate = 0.1m;

        public async Task<bool> Handle(AddOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.TotalAmount <= 0 || !OrderStatusFlow.IsValid(request.Status))
                return false;

            var exists = await repository
                .GetTableNoTracking()
                .AnyAsync(x => x.OrderNumber == request.OrderNumber, cancellationToken);

            if (exists)
                return false;

            var order = mapper.Map<Walmart.Domain.Entities.Order>(request);
            order.OrderDate = DateTime.UtcNow;

            await repository.AddAsync(order);
            return true;
        }

        public async Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.ProductIds is null || !request.ProductIds.Any())
                return false;

            var order = await repository.GetByIdAsync(request.Id);
            if (order is null)
                return false;

            if (request.TotalAmount <= 0 || !OrderStatusFlow.IsValid(request.Status))
                return false;

            if (!OrderStatusFlow.CanTransition(order.Status, request.Status))
                return false;

            var exists = await repository
                .GetTableNoTracking()
                .AnyAsync(x => x.OrderNumber == request.OrderNumber && x.Id != request.Id, cancellationToken);

            if (exists)
                return false;

            order.UserId = request.UserId;
            order.ShippingAddressId = request.ShippingAddressId;
            order.OrderNumber = request.OrderNumber;
            order.Status = request.Status;

            var selectedProductIds = request.ProductIds
                .Distinct()
                .ToList();

            var selectedProducts = await productRepository
                .GetTableNoTracking()
                .Where(p => selectedProductIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Price })
                .ToListAsync(cancellationToken);

            if (selectedProducts.Count != selectedProductIds.Count)
                return false;

            var existingLines = await orderProductRepository
                .GetTableAsTracking()
                .Where(op => op.OrderId == order.Id)
                .ToListAsync(cancellationToken);

            if (existingLines.Any())
                await orderProductRepository.DeleteRangeAsync(existingLines);

            var updatedLines = selectedProducts
                .Select(product => new OrderProduct
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    LineTotal = product.Price
                })
                .ToList();

            await orderProductRepository.AddRangeAsync(updatedLines);

            var subtotal = updatedLines.Sum(line => line.LineTotal);
            order.TotalAmount = subtotal + (subtotal * TaxRate);

            await repository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await repository.GetByIdAsync(request.Id);
            if (order is null)
                return false;

            await repository.DeleteAsync(order);
            return true;
        }
    }
}
