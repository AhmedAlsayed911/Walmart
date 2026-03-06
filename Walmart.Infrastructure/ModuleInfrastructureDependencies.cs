using Microsoft.Extensions.DependencyInjection;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Infrastructure.Persistence.Repositories;

namespace Walmart.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)

             => services
                .AddTransient(typeof(IBaseRepository<>), typeof(BaseRepositoryc<>));
    }
}
