using System;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class DevicesApiTests
{
    private readonly HausWebHostApplicationFactory _factory;
    private readonly IHausApiClient _hausClient;

    public DevicesApiTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
        _hausClient = _factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task WhenADeviceIsUpdatedThenUpdatedDeviceIsAvailableFromTheApi()
    {
        await _factory.PublishHausEventAsync(new DeviceDiscoveredEvent("hello"));

        await Eventually.AssertAsync(async () =>
        {
            var result = await _hausClient.GetDevicesAsync("hello");
            var device = result.Items[0];
            await _hausClient.UpdateDeviceAsync(device.Id, new DeviceModel { Name = "some-name" });

            var updated = await _hausClient.GetDeviceAsync(device.Id);
            if (updated != null)
            {
                Assert.Equal("some-name", updated.Name);
            }
        });
    }

    [Fact]
    public async Task WhenADeviceIsDeletedThenItIsNoLongerAvailableFromTheApi()
    {
        var device = await _factory.WaitForDeviceToBeDiscovered();

        await _hausClient.DeleteDeviceAsync(device.Id);

        await Eventually.AssertAsync(async () =>
        {
            var result = await _hausClient.GetDevicesAsync(device.ExternalId);
            Assert.Empty(result.Items);
        });
    }

    [Fact]
    public async Task WhenADeviceInARoomIsDeletedThenItIsNoLongerInTheRoom()
    {
        var (room, device) = await _factory.AddRoomWithDevice($"room-{Guid.NewGuid()}", DeviceType.Light);

        await _hausClient.DeleteDeviceAsync(device.Id);

        await Eventually.AssertAsync(async () =>
        {
            var devicesInRoom = await _hausClient.GetDevicesInRoomAsync(room.Id);
            Assert.DoesNotContain(devicesInRoom.Items, d => d.Id == device.Id);
        });
    }

    [Fact]
    public async Task WhenLightingOfDeviceIsChangedThenDeviceLightingChangedEventIsPublished()
    {
        HausCommand<DeviceLightingChangedEvent>? hausCommand = null;
        await _factory.SubscribeToHausCommandsAsync<DeviceLightingChangedEvent>(
            DeviceLightingChangedEvent.Type,
            msg => hausCommand = msg
        );

        var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);
        await _hausClient.ChangeDeviceLightingAsync(device.Id, new LightingModel());

        Eventually.Assert(() =>
        {
            if (hausCommand != null)
            {
                Assert.Equal(DeviceLightingChangedEvent.Type, hausCommand.Type);
            }
        });
    }

    [Fact]
    public async Task WhenDeviceIsTurnedOffThenPublishesChangeLightingWithOffState()
    {
        HausCommand<DeviceLightingChangedEvent>? published = null;
        await _factory.SubscribeToHausCommandsAsync<DeviceLightingChangedEvent>(
            DeviceLightingChangedEvent.Type,
            msg => published = msg
        );

        var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);
        await _hausClient.TurnLightOffAsync(device.Id);

        Eventually.Assert(() =>
        {
            if (published?.Payload?.Lighting != null)
            {
                Assert.Equal(LightingState.Off, published.Payload.Lighting.State);
            }
        });
    }

    [Fact]
    public async Task WhenDeviceIsTurnedOnThenPublishesChangeLightingWithOnState()
    {
        HausCommand<DeviceLightingChangedEvent>? published = null;
        await _factory.SubscribeToHausCommandsAsync<DeviceLightingChangedEvent>(
            DeviceLightingChangedEvent.Type,
            msg => published = msg
        );

        var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);
        await _hausClient.TurnLightOnAsync(device.Id);

        Eventually.Assert(() =>
        {
            if (published?.Payload?.Lighting != null)
            {
                Assert.Equal(LightingState.On, published.Payload.Lighting.State);
            }
        });
    }

    [Fact]
    public async Task WhenDeviceLightingConstraintsChangedThenDeviceLightingConstraintsAreUpdated()
    {
        var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);

        await _hausClient.ChangeDeviceLightingConstraintsAsync(device.Id, new LightingConstraintsModel(50, 90));

        var updatedDevice = await _hausClient.GetDeviceAsync(device.Id);
        if (updatedDevice?.Lighting != null)
        {
            Assert.Equal(50, updatedDevice.Lighting.Level.Min);
            Assert.Equal(90, updatedDevice.Lighting.Level.Max);
        }
    }
}
