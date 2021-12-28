using System.Threading.Tasks;
using FluentAssertions;
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
            await _hausClient.UpdateDeviceAsync(device.Id, new DeviceModel
            {
                Name = "some-name"
            });

            var updated = await _hausClient.GetDeviceAsync(device.Id);
            updated.Name.Should().Be("some-name");
        });
    }

    [Fact]
    public async Task WhenLightingOfDeviceIsChangedThenDeviceLightingChangedEventIsPublished()
    {
        HausCommand<DeviceLightingChangedEvent> hausCommand = null;
        await _factory.SubscribeToHausCommandsAsync<DeviceLightingChangedEvent>(
            DeviceLightingChangedEvent.Type,
            msg => hausCommand = msg
        );

        var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);
        await _hausClient.ChangeDeviceLightingAsync(device.Id, new LightingModel());

        Eventually.Assert(() => { hausCommand.Type.Should().Be(DeviceLightingChangedEvent.Type); });
    }

    [Fact]
    public async Task WhenDeviceIsTurnedOffThenPublishesChangeLightingWithOffState()
    {
        HausCommand<DeviceLightingChangedEvent> published = null;
        await _factory.SubscribeToHausCommandsAsync<DeviceLightingChangedEvent>(
            DeviceLightingChangedEvent.Type,
            msg => published = msg
        );

        var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);
        await _hausClient.TurnLightOffAsync(device.Id);

        Eventually.Assert(() => { published.Payload.Lighting.State.Should().Be(LightingState.Off); });
    }

    [Fact]
    public async Task WhenDeviceIsTurnedOnThenPublishesChangeLightingWithOnState()
    {
        HausCommand<DeviceLightingChangedEvent> published = null;
        await _factory.SubscribeToHausCommandsAsync<DeviceLightingChangedEvent>(
            DeviceLightingChangedEvent.Type,
            msg => published = msg
        );

        var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);
        await _hausClient.TurnLightOnAsync(device.Id);

        Eventually.Assert(() => { published.Payload.Lighting.State.Should().Be(LightingState.On); });
    }

    [Fact]
    public async Task WhenDeviceLightingConstraintsChangedThenDeviceLightingConstraintsAreUpdated()
    {
        var device = await _factory.WaitForDeviceToBeDiscovered(DeviceType.Light);

        await _hausClient.ChangeDeviceLightingConstraintsAsync(device.Id, new LightingConstraintsModel(50, 90));

        var updatedDevice = await _hausClient.GetDeviceAsync(device.Id);
        updatedDevice.Lighting.Level.Min.Should().Be(50);
        updatedDevice.Lighting.Level.Max.Should().Be(90);
    }
}