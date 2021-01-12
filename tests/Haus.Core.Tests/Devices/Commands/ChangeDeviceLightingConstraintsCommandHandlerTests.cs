using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands
{
    public class ChangeDeviceLightingConstraintsCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly CapturingHausBus _hausBus;

        public ChangeDeviceLightingConstraintsCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.CreateCapturingBus(_context);
        }

        [Fact]
        public async Task WhenDeviceLightingConstraintsChangedThenDeviceLightingIsUpdated()
        {
            var device = _context.AddDevice(deviceType: DeviceType.Light);

            var command = new ChangeDeviceLightingConstraintsCommand(device.Id, new LightingConstraintsModel(12, 90));
            await _hausBus.ExecuteCommandAsync(command);

            var updated = await _context.FindByIdAsync<DeviceEntity>(device.Id);
            updated.Lighting.Level.Min.Should().Be(12);
            updated.Lighting.Level.Max.Should().Be(90);
        }

        [Fact]
        public async Task WhenDeviceLightingConstraintsChangedThenDeviceLightingChangedEventIsPublished()
        {
            var device = _context.AddDevice(deviceType: DeviceType.Light);

            var command = new ChangeDeviceLightingConstraintsCommand(device.Id, new LightingConstraintsModel(12, 100));
            await _hausBus.ExecuteCommandAsync(command);

            _hausBus.GetPublishedRoutableEvents<DeviceLightingChangedEvent>().Should().HaveCount(1);
        }
    }
}