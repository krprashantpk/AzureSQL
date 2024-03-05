using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AzureSQL.Data
{

    public class Repository : IRepository
    {
        private readonly IDbContextFactory<SQLDbContext> contextFactory;

        public Repository(IDbContextFactory<SQLDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<IEnumerable<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> expression, 
            CancellationToken cancellationToken = default) where TEntity : class
        {
            var query = await GetQueryAsync<TEntity>(cancellationToken);
            return await query.Where(expression).ToListAsync(cancellationToken);

        }

        public async Task<TEntity?> FindAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) where TEntity : class
        {
            var query = await GetQueryAsync<TEntity>(cancellationToken);
            return await query.Where(expression).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            await context.Set<TEntity>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        public async Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            await context.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }


        public async Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            context.Entry<TEntity>(entity).State = EntityState.Modified;
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            context.Set<TEntity>().Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IQueryable<TEntity>> GetQueryAsync<TEntity>(CancellationToken cancellationToken) where TEntity : class
        {
            var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            context.ChangeTracker.LazyLoadingEnabled = true;
            return context.Set<TEntity>().AsQueryable();
        }
    }


}
