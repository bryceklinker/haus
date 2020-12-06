using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Entities;
using Haus.Core.Common.Queries;
using Haus.Core.Rooms.Entities;
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

        public IQueryable<TEntity> GetAll<TEntity>()
            where TEntity : class, IEntity
        {
            return Set<TEntity>();
        }
        
        public IQueryable<TEntity> GetAllReadOnly<TEntity>()
            where TEntity : class, IEntity
        {
            return GetAll<TEntity>()
                .AsNoTracking();
        }
        
        public IQueryable<RoomEntity> GetRoomsIncludeDevices()
        {
            return GetAll<RoomEntity>()
                .Include(r => r.Devices);
        }

        public Task<bool> DoesExistAsync<TEntity>(long id)
            where TEntity : class, IEntity
        {
            return GetAllReadOnly<TEntity>()
                .AnyAsync(e => e.Id == id);
        }

        public async Task<bool> IsMissingAsync<TEntity>(long id)
            where TEntity : class, IEntity
        {
            return !await DoesExistAsync<TEntity>(id)
                .ConfigureAwait(false);
        }
        
        public async Task<TEntity> FindByAsync<TEntity>(Expression<Func<TEntity, bool>> expression) 
            where TEntity : class, IEntity
        {
            return await GetAll<TEntity>()
                .SingleOrDefaultAsync(expression)
                .ConfigureAwait(false);
        }

        public async Task<TEntity> FindByIdAsync<TEntity>(long id, CancellationToken token = default) 
            where TEntity : class, IEntity
        {
            return await FindAsync<TEntity>(new object[]{id}, token)
                .ConfigureAwait(false);
        }

        public async Task<TEntity[]> FindAllById<TEntity>(long[] ids, CancellationToken token = default)
            where TEntity : class, IEntity
        {
            return await GetAll<TEntity>()
                .Where(e => ids.Contains(e.Id))
                .ToArrayAsync(token)
                .ConfigureAwait(false);
        }

        public async Task<bool> IsUniqueAsync<TEntity, TProperty>(
            long id,
            TProperty value,
            Expression<Func<TEntity, TProperty>> propertySelector,
            CancellationToken token = default)
            where TEntity : class, IEntity
        {
            return !await GetAll<TEntity>()
                .Where(e => e.Id != id)
                .Select(propertySelector)
                .AnyAsync(v => v.Equals(value), token)
                .ConfigureAwait(false);
        }
    }
}