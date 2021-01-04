using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Devices.Repositories
{
    public interface IDeviceCommandRepository
    {
        Task<DeviceEntity> GetById(long id, CancellationToken token = default);
        Task<DeviceEntity> GetByExternalId(string externalId, CancellationToken token = default);
        Task<DeviceEntity> AddAsync(DeviceEntity device, CancellationToken token = default);
        Task SaveAsync(DeviceEntity device, CancellationToken token = default);
    }
    
    public class DeviceCommandRepository : IDeviceCommandRepository
    {
        private readonly HausDbContext _context;

        public DeviceCommandRepository(HausDbContext context)
        {
            _context = context;
        }

        public Task<DeviceEntity> GetById(long id, CancellationToken token = default)
        {
            return _context.FindByIdOrThrowAsync<DeviceEntity>(id, AddIncludes, token);
        }

        public Task<DeviceEntity> GetByExternalId(string externalId, CancellationToken token = default)
        {
            return _context.FindByAsync<DeviceEntity>(d => d.ExternalId == externalId,
                AddIncludes,
                token);
        }

        public async Task<DeviceEntity> AddAsync(DeviceEntity device, CancellationToken token = default)
        {
            _context.Add(device);
            await _context.SaveChangesAsync(token).ConfigureAwait(false);
            return device;
        }

        public Task SaveAsync(DeviceEntity device, CancellationToken token = default)
        {
            _context.Update(device);
            return _context.SaveChangesAsync(token);
        }

        private static IQueryable<DeviceEntity> AddIncludes(IQueryable<DeviceEntity> query)
        {
            return query
                .Include(d => d.Metadata)
                .Include(d => d.Room);
        }
    }
}