using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<TEntity> FindByIdAsync<TEntity>(object id, CancellationToken token = default) 
            where TEntity : class
        {
            return await FindAsync<TEntity>(new[] {id}, token).ConfigureAwait(false);
        }
    }
}