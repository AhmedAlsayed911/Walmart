using MediatR;
using Microsoft.EntityFrameworkCore;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Application.Features.Dashboard.Queries.Models;
using Walmart.Application.Features.Order;
using Walmart.Application.ViewModels.Dashboard;

namespace Walmart.Application.Features.Dashboard.Queries.Handlers
{
    public class DashboardQueryHandler(
        IBaseRepository<Walmart.Domain.Entities.Product> productRepository,
        IBaseRepository<Walmart.Domain.Entities.Category> categoryRepository,
        IBaseRepository<Walmart.Domain.Entities.Order> orderRepository,
        IBaseRepository<Walmart.Domain.Entities.OrderProduct> orderProductRepository,
        IBaseRepository<Walmart.Domain.Entities.ApplicationUser> userRepository) : IRequestHandler<GetAdminDashboardQuery, AdminDashboardVM>
    {
        public async Task<AdminDashboardVM> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

            var totalProducts = await productRepository.GetTableNoTracking().CountAsync(cancellationToken);
            var inStockProducts = await productRepository.GetTableNoTracking()
                .Where(p => p.StockQuantity > 0)
                .CountAsync(cancellationToken);
            var outOfStockProducts = await productRepository.GetTableNoTracking()
                .Where(p => p.StockQuantity <= 0)
                .CountAsync(cancellationToken);
            var totalCategories = await categoryRepository.GetTableNoTracking().CountAsync(cancellationToken);
            var totalOrders = await orderRepository.GetTableNoTracking().CountAsync(cancellationToken);
            var totalUsers = await userRepository.GetTableNoTracking().CountAsync(cancellationToken);

            var pendingOrders = await orderRepository.GetTableNoTracking()
                .Where(o => o.Status == OrderStatusFlow.Pending)
                .CountAsync(cancellationToken);

            var deliveredOrders = await orderRepository.GetTableNoTracking()
                .Where(o => o.Status == OrderStatusFlow.Delivered)
                .CountAsync(cancellationToken);

            var cancelledOrders = await orderRepository.GetTableNoTracking()
                .Where(o => o.Status == OrderStatusFlow.Cancelled)
                .CountAsync(cancellationToken);

            var lowStockProducts = await productRepository.GetTableNoTracking()
                .Where(p => p.StockQuantity > 0 && p.StockQuantity <= 10)
                .CountAsync(cancellationToken);

            var totalRevenue = await orderRepository.GetTableNoTracking()
                .Where(o => o.Status != OrderStatusFlow.Cancelled)
                .Select(o => (decimal?)o.TotalAmount)
                .SumAsync(cancellationToken) ?? 0m;

            var revenueThisMonth = await orderRepository.GetTableNoTracking()
                .Where(o => o.Status != OrderStatusFlow.Cancelled && o.OrderDate >= firstDayOfMonth)
                .Select(o => (decimal?)o.TotalAmount)
                .SumAsync(cancellationToken) ?? 0m;

            var topSellingProducts = await orderProductRepository.GetTableNoTracking()
                .Where(op => op.Order.Status != OrderStatusFlow.Cancelled)
                .GroupBy(op => new { op.ProductId, op.Product.Name, op.Product.Sku })
                .Select(group => new TopSellingProductVM
                {
                    ProductId = group.Key.ProductId,
                    Name = group.Key.Name,
                    Sku = group.Key.Sku,
                    UnitsSold = group.Sum(op => op.Quantity),
                    TotalRevenue = group.Sum(op => op.LineTotal)
                })
                .OrderByDescending(item => item.UnitsSold)
                .ThenByDescending(item => item.TotalRevenue)
                .Take(5)
                .ToListAsync(cancellationToken);

            return new AdminDashboardVM
            {
                TotalProducts = totalProducts,
                InStockProducts = inStockProducts,
                OutOfStockProducts = outOfStockProducts,
                TotalCategories = totalCategories,
                TotalOrders = totalOrders,
                TotalUsers = totalUsers,
                PendingOrders = pendingOrders,
                DeliveredOrders = deliveredOrders,
                CancelledOrders = cancelledOrders,
                LowStockProducts = lowStockProducts,
                TotalRevenue = totalRevenue,
                RevenueThisMonth = revenueThisMonth,
                TopSellingProducts = topSellingProducts
            };
        }
    }
}
