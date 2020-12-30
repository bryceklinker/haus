using System;
using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands
{
    public class ChangeDeviceLightingCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly CapturingHausBus _hausBus;

        public ChangeDeviceLightingCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.CreateCapturingBus(_context);
        }

        [Fact]
        public async Task WhenDeviceLightingChangedThenDeviceLightingIsSavedToDatabase()
        {
            var device = _context.AddDevice(deviceType: DeviceType.Light);
            
            var lighting = new LightingModel{ BrightnessPercent = 54};
            await _hausBus.ExecuteCommandAsync(new ChangeDeviceLightingCommand(device.Id, lighting));

            var updated = await _context.FindByIdAsync<DeviceEntity>(device.Id);
            Assert.Equal(54, updated.Lighting.BrightnessPercent);
        }

        [Fact]
        public async Task WhenDeviceIsMissingThenThrowsEntityNotFoundException()
        {
            var command = new ChangeDeviceLightingCommand(4213, new LightingModel());
            await Assert.ThrowsAsync<EntityNotFoundException<DeviceEntity>>(() => _hausBus.ExecuteCommandAsync(command));
        }

        [Fact]
        public async Task WhenDeviceIsNotALightThenThrowsInvalidOperationException()
        {
            var device = _context.AddDevice();
            var lighting = new LightingModel();
            var command = new ChangeDeviceLightingCommand(device.Id, lighting);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _hausBus.ExecuteCommandAsync(command));
        }

        [Fact]
        public async Task WhenDeviceLightingIsChangedThenDeviceLightingChangedIsPublished()
        {
            var device = _context.AddDevice(deviceType: DeviceType.Light);
            
            var lighting = new LightingModel{BrightnessPercent = 65};
            await _hausBus.ExecuteCommandAsync(new ChangeDeviceLightingCommand(device.Id, lighting));

            var hausCommand = _hausBus.GetPublishedHausCommands<DeviceLightingChangedEvent>().Single();
            Assert.Equal(device.Id, hausCommand.Payload.Device.Id);
            Assert.Equal(65, hausCommand.Payload.Lighting.BrightnessPercent);
        }
    }
}