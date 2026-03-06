using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using Walmart.Application.Abstracts.Persistence;
using Walmart.Infrastructure.Data;

namespace Walmart.Infrastructure.Persistence.Repositories
{
    public class BaseRepositoryc<T>(ApplicationDbContext dbContext) : IBaseRepository<T> where T : class
    {
        public virtual async Task<T> GetByIdAsync(int id)
        {

            return await dbContext.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetTableNoTracking()
        {
            return dbContext.Set<T>().AsNoTracking().AsQueryable();
        }

        public virtual async Task AddRangeAsync(ICollection<T> entities)
        {
            await dbContext.Set<T>().AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();

        }
        public virtual async Task<T> AddAsync(T entity)
        {
            await dbContext.Set<T>().AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            dbContext.Set<T>().Update(entity);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        public virtual async Task DeleteRangeAsync(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                dbContext.Entry(entity).State = EntityState.Deleted;
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            dbContext.Database.CommitTransaction();
        }

        public void RollBack()
        {
            dbContext.Database.RollbackTransaction();
        }

        public IQueryable<T> GetTableAsTracking()
        {
            return dbContext.Set<T>().AsQueryable();
        }

        public virtual async Task UpdateRangeAsync(ICollection<T> entities)
        {
            dbContext.Set<T>().UpdateRange(entities);
            await dbContext.SaveChangesAsync();
        }

        public Task<T> FindByAsync(Expression<Func<T, bool>> expression)
        {
            return dbContext.Set<T>().Where(expression).FirstAsync();
        }
    }
}
