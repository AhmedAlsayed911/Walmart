using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
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
        public async Task<PaginatedResult<OrderListVM>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            return await repository
                .GetTableNoTracking()
                .Include(o => o.User)
                .OrderByDescending(x => x.OrderDate)
                .ProjectTo<OrderListVM>(mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }

        public async Task<OrderDetailsVM> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await repository
                .GetTableNoTracking()
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.Products)
                .Where(o => o.Id == request.Id)
                .ProjectTo<OrderDetailsVM>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return order;
        }

        public async Task<PaginatedResult<OrderListVM>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
        {
            return await repository
                .GetTableNoTracking()
                .Include(o => o.User)
                .Where(o => o.UserId == request.UserId)
                .OrderByDescending(x => x.OrderDate)
                .ProjectTo<OrderListVM>(mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
