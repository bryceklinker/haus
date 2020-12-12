using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands
{
    public class UpdateDeviceCommandTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public UpdateDeviceCommandTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public void WhenUpdateDeviceCommandCreatedThenModelIdIsSynchronizedWithCommandId()
        {
            var command = new UpdateDeviceCommand(43, new DeviceModel());

            Assert.Equal(43, command.Model.Id);
        }
        
        [Fact]
        public async Task WhenUpdateDeviceCommandExecutedThenDeviceIsUpdatedInDatabase()
        {
            var original = _context.AddDevice();
            
            var model = new DeviceModel{Name = "hi-bob"};

            await _hausBus.ExecuteCommandAsync(new UpdateDeviceCommand(original.Id, model));

            var updated = await _context.FindAsync<DeviceEntity>(original.Id);
            Assert.Equal("hi-bob", updated.Name);
        }

        [Fact]
        public async Task WhenUpdateDeviceCommandExecutedForMissingDeviceThenThrowsException()
        {
            var command = new UpdateDeviceCommand(-1, new DeviceModel{Name = "hello"});
            await Assert.ThrowsAsync<EntityNotFoundException<DeviceEntity>>(() => _hausBus.ExecuteCommandAsync(command));
        }

        [Fact]
        public async Task WhenUpdateDeviceCommandIsInvalidThenThrowsException()
        {
            var device = _context.AddDevice();
            var command = new UpdateDeviceCommand(device.Id, new DeviceModel {Name = null});
            await Assert.ThrowsAsync<HausValidationException>(() => _hausBus.ExecuteCommandAsync(command));
        }
    }
}