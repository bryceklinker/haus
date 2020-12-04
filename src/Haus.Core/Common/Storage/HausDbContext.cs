using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Common.Storage
{
    public class HausDbContext : DbContext
    {
        public HausDbContext(DbContextOptions<HausDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HausDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public async Task<T> FindByAsync<T>(Expression<Func<T, bool>> expression) 
            where T : class
        {
            return await Set<T>().SingleOrDefaultAsync(expression).ConfigureAwait(false);
        }

        public async Task<TEntity> FindByIdAsync<TEntity>(long id, CancellationToken token = default) 
            where TEntity : class, IEntity
        {
            return await FindAsync<TEntity>(new object[]{id}, token).ConfigureAwait(false);
        }

        public async Task<bool> IsUniqueAsync<TEntity, TProperty>(
            long id,
            TProperty value,
            Expression<Func<TEntity, TProperty>> propertySelector,
            CancellationToken token = default)
            where TEntity : class, IEntity
        {
            return !await Set<TEntity>()
                .Where(e => e.Id != id)
                .Select(propertySelector)
                .AnyAsync(v => v.Equals(value), token)
                .ConfigureAwait(false);
        }
    }
}