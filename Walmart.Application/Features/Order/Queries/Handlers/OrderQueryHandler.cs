using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Order;
using Walmart.Application.Features.Order.Queries.Models;
using Walmart.Application.Pagination;
using Walmart.Application.ViewModels.Order;

namespace Walmart.Application.Features.Order.Queries.Handlers
{
    public class OrderQueryHandler(IBaseRepository<Walmart.Domain.Entities.Order> repository, IMapper mapper)
        : IRequestHandler<GetAllOrdersQuery, PaginatedResult<OrderListVM>>,
          IRequestHandler<GetOrderByIdQuery, OrderDetailsVM>,
          IRequestHandler<GetUserOrdersQuery, PaginatedResult<OrderListVM>>
    {
        private const int MoveToOnWayMinutes = 10;
        private const int MoveToDeliveredMinutes = 15;

        public async Task<PaginatedResult<OrderListVM>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            await AutoAdvanceOrderStatusesAsync(cancellationToken);

            var query = repository
                .GetTableNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .AsQueryable();

            if (request.OrderId.HasValue)
            {
                query = query.Where(o => o.Id == request.OrderId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(o => o.Status == request.Status.Value);
            }

            query = request.SortBy?.ToLower() switch
            {
                "date_asc" => query.OrderBy(x => x.OrderDate),
                "elapsed_desc" => query.OrderBy(x => x.OrderDate),
                "elapsed_asc" => query.OrderByDescending(x => x.OrderDate),
                _ => query.OrderByDescending(x => x.OrderDate)
            };

            return await query
                .ProjectTo<OrderListVM>(mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }

        public async Task<OrderDetailsVM> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            await AutoAdvanceOrderStatusesAsync(cancellationToken);

            var order = await repository
                .GetTableNoTracking()
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Where(o => o.Id == request.Id)
                .ProjectTo<OrderDetailsVM>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return order;
        }

        public async Task<PaginatedResult<OrderListVM>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
        {
            await AutoAdvanceOrderStatusesAsync(cancellationToken);

            return await repository
                .GetTableNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Where(o => o.UserId == request.UserId)
                .OrderByDescending(x => x.OrderDate)
                .ProjectTo<OrderListVM>(mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }

        private async Task AutoAdvanceOrderStatusesAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var trackedOrders = await repository
                .GetTableAsTracking()
                .Where(o => o.Status != OrderStatusFlow.Delivered && o.Status != OrderStatusFlow.Cancelled)
                .ToListAsync(cancellationToken);

            if (!trackedOrders.Any())
                return;

            var updatedOrders = new List<Walmart.Domain.Entities.Order>();

            foreach (var order in trackedOrders)
            {
                var elapsedMinutes = (now - order.OrderDate).TotalMinutes;
                var targetStatus = order.Status;

                if (elapsedMinutes >= MoveToDeliveredMinutes)
                    targetStatus = OrderStatusFlow.Delivered;
                else if (elapsedMinutes >= MoveToOnWayMinutes)
                    targetStatus = OrderStatusFlow.Shipped;

                if (targetStatus != order.Status)
                {
                    var advancedStatus = AdvanceOrderStatus(order.Status, targetStatus);
                    if (advancedStatus != order.Status)
                    {
                        order.Status = advancedStatus;
                        updatedOrders.Add(order);
                    }
                }
            }

            if (updatedOrders.Any())
                await repository.UpdateRangeAsync(updatedOrders);
        }

        private static int AdvanceOrderStatus(int currentStatus, int targetStatus)
        {
            var status = currentStatus;

            while (status != targetStatus)
            {
                status = status switch
                {
                    OrderStatusFlow.Pending => OrderStatusFlow.Processing,
                    OrderStatusFlow.Processing => OrderStatusFlow.Shipped,
                    OrderStatusFlow.Shipped => OrderStatusFlow.Delivered,
                    _ => status
                };

                if (status == currentStatus)
                    break;
            }

            return status;
        }
    }
}
