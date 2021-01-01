using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands
{
    public class TurnDeviceOnCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly CapturingHausBus _hausBus;

        public TurnDeviceOnCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.CreateCapturingBus(_context);
        }

        [Fact]
        public async Task WhenDeviceIsTurnedOnThenPublishedChangeLightingWithStateOn()
        {
            var device = _context.AddDevice(deviceType: DeviceType.Light, configure: d => d.TurnOff(_hausBus));

            await _hausBus.ExecuteCommandAsync(new TurnDeviceOnCommand(device.Id));

            var publishedCommand = _hausBus.GetPublishedHausCommands<DeviceLightingChangedEvent>().Single();
            publishedCommand.Payload.Device.Id.Should().Be(device.Id);
            publishedCommand.Payload.Lighting.Should().Should().Be(LightingState.On);
        }

        [Fact]
        public async Task WhenDeviceIsMissingThenThrowsNotFound()
        {
            var command = new TurnDeviceOnCommand(324);

            Func<Task> act = () => _hausBus.ExecuteCommandAsync(command);

            await act.Should().ThrowAsync<EntityNotFoundException<DeviceEntity>>();
        }
    }
}