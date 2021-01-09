using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands
{
    public class ChangeLightingConstraintsCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly CapturingHausBus _hausBus;

        public ChangeLightingConstraintsCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.CreateCapturingBus(_context);
        }

        [Fact]
        public async Task WhenDeviceIsNotALightThenThrowsInvalidOperationException()
        {
            var device = _context.AddDevice(deviceType: DeviceType.MotionSensor);
            
            Func<Task> act = () => _hausBus.ExecuteCommandAsync(new ChangeDeviceLightingConstraintsCommand(device.Id, LightingConstraintsModel.Default));

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task WhenDeviceLightingConstraintsAreChangedThenDeviceLightingConstraintsChangedEventPublished()
        {
            var device = _context.AddDevice(deviceType: DeviceType.Light);

            var command = new ChangeDeviceLightingConstraintsCommand(device.Id, LightingConstraintsModel.Default);
            await _hausBus.ExecuteCommandAsync(command);

            _hausBus.GetPublishedRoutableEvents<DeviceLightingConstraintsChangedEvent>().Should().HaveCount(1);
        }
    }
}