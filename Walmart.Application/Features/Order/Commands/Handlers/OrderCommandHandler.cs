using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Order.Commands.Models;

namespace Walmart.Application.Features.Order.Commands.Handlers
{
    public class OrderCommandHandler(IBaseRepository<Walmart.Domain.Entities.Order> repository, IMapper mapper)
        : IRequestHandler<AddOrderCommand, bool>,
          IRequestHandler<UpdateOrderCommand, bool>,
          IRequestHandler<DeleteOrderCommand, bool>
    {
        public async Task<bool> Handle(AddOrderCommand request, CancellationToken cancellationToken)
        {
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
            var order = await repository.GetByIdAsync(request.Id);
            if (order is null)
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
            order.TotalAmount = request.TotalAmount;

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
