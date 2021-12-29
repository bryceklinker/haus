using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Entities;
using Haus.Core.Discovery.Entities;
using Haus.Core.Rooms.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Common.Storage;

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

    public async Task<bool> HaveMigrationsBeenApplied(CancellationToken token = default)
    {
        var pendingMigrations = await Database.GetPendingMigrationsAsync(token).ConfigureAwait(false);
        return !pendingMigrations.Any();
    }

    public Task<DiscoveryEntity> GetDiscoveryEntityAsync(CancellationToken token)
    {
        return Set<DiscoveryEntity>().SingleOrDefaultAsync(token);
    }

    public IQueryable<TEntity> GetAll<TEntity>()
        where TEntity : class, IEntity
    {
        return Set<TEntity>();
    }

    public IQueryable<TEntity> QueryAll<TEntity>()
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
        return QueryAll<TEntity>()
            .AnyAsync(e => e.Id == id);
    }

    public async Task<bool> IsMissingAsync<TEntity>(long id)
        where TEntity : class, IEntity
    {
        return !await DoesExistAsync<TEntity>(id)
            .ConfigureAwait(false);
    }

    public async Task<TEntity> FindByAsync<TEntity>(Expression<Func<TEntity, bool>> expression,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery = null, CancellationToken token = default)
        where TEntity : class, IEntity
    {
        var queryable = GetAll<TEntity>();

        queryable = configureQuery?.Invoke(queryable) ?? queryable;
        return await queryable
            .SingleOrDefaultAsync(expression, token)
            .ConfigureAwait(false);
    }

    public async Task<TEntity> FindByIdAsync<TEntity>(long id,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery = null, CancellationToken token = default)
        where TEntity : class, IEntity
    {
        return await FindByAsync(e => e.Id == id, configureQuery, token)
            .ConfigureAwait(false);
    }

    public async Task<TEntity> FindByIdOrThrowAsync<TEntity>(long id,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> configureQuery = null,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        var entity = await FindByIdAsync(id, configureQuery, cancellationToken).ConfigureAwait(false);
        if (entity == null)
            throw new EntityNotFoundException<TEntity>(id);

        return entity;
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