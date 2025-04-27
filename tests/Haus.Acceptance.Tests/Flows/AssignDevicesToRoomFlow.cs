using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Haus.Acceptance.Tests.Support.Zigbee2Mqtt;
using Haus.Testing.Support;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class AssignDevicesToRoomFlow : HausPageTest
{
    private const string UnassignedDevicesIdentifier = "'unassigned'";
    private const string UnassignedDropZoneSelector = $".mud-drop-zone[identifier={UnassignedDevicesIdentifier}]";
    private const string UnassignedDeviceDropItemSelector = $"{UnassignedDropZoneSelector} > .mud-drop-item";
    private const string FirstUnassignedDeviceDropItemSelector = $"{UnassignedDeviceDropItemSelector}:nth-of-type(1)";
    private const string FirstRoomDropZoneSelector = $".mud-drop-zone:not([identifier={UnassignedDevicesIdentifier}])";
    private const string DevicesAssignedToFirstRoomDropItemSelector = $"{FirstRoomDropZoneSelector} > .mud-drop-item";

    private Zigbee2MqttPublisher _zigbee2MqttPublisher;

    [SetUp]
    public async Task BeforeEach()
    {
        await Context.StartTracingAsync();
        _zigbee2MqttPublisher = await GetZigbee2MqttPublisher();
    }

    [Test]
    public async Task AssignDeviceToRoom()
    {
        await Page.PerformLoginAsync(HausUser.Default);

        await PublishPhillipsLight("new_light");
        await AddRoom("new_hotness");

        await Page.ClickLinkAsync("Devices");
        await Page.ClickLinkAsync("Discovery");

        var originalUnassignedDevicesCount = 0;
        var originalAssignedDevicesCount = 0;
        await Eventually.AssertAsync(async () =>
        {
            originalUnassignedDevicesCount = await Page.CssLocator(UnassignedDeviceDropItemSelector).CountAsync();
            originalAssignedDevicesCount = await Page.CssLocator(DevicesAssignedToFirstRoomDropItemSelector)
                .CountAsync();
            Assert.That(originalAssignedDevicesCount, Is.GreaterThanOrEqualTo(1));
            Assert.That(originalUnassignedDevicesCount, Is.GreaterThanOrEqualTo(1));
        });

        await Page.DragAndDropAsync(FirstUnassignedDeviceDropItemSelector, FirstRoomDropZoneSelector);

        await Expect(Page.CssLocator(UnassignedDeviceDropItemSelector))
            .ToHaveCountAsync(originalUnassignedDevicesCount - 1);
        await Expect(Page.CssLocator(DevicesAssignedToFirstRoomDropItemSelector))
            .ToHaveCountAsync(originalAssignedDevicesCount + 1);
    }

    [TearDown]
    public async Task AfterEach()
    {
        await Context.StopTracingAsync();
    }

    private async Task PublishPhillipsLight(string friendlyName)
    {
        var message = new Zigbee2MqttMessageBuilder()
            .WithLogTopic()
            .WithInterviewSuccessful()
            .WithPairing()
            .WithPhillipsLightMeta(friendlyName)
            .Build();
        await _zigbee2MqttPublisher.PublishAsJsonAsync(message);
    }

    private async Task AddRoom(string roomName)
    {
        await Page.ClickLinkAsync("Rooms");
        await Page.CssLocator(".mud-fab").ClickAsync();
        await Page.EnterTextAsync("name", roomName);
        await Page.ClickButtonAsync("Save");
    }
}
