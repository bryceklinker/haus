using System.Collections.Generic;
using System.Threading.Tasks;
using Haus.Acceptance.Tests.Support;
using Haus.Testing.Support;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Haus.Acceptance.Tests.Flows;

[TestFixture]
public class AssignDevicesToRoomFlow : PageTest
{
    private const string UnassignedDevicesIdentifier = "'unassigned'";
    private const string UnassignedDropZoneSelector = $".mud-drop-zone[identifier={UnassignedDevicesIdentifier}]";
    private const string UnassignedDeviceDropItemSelector = $"{UnassignedDropZoneSelector} > .mud-drop-item";
    private const string FirstUnassignedDeviceDropItemSelector = $"{UnassignedDeviceDropItemSelector}:nth-of-type(1)";
    private const string FirstRoomDropZoneSelector = $".mud-drop-zone:not([identifier={UnassignedDevicesIdentifier}])";
    private const string DevicesAssignedToFirstRoomDropItemSelector = $"{FirstRoomDropZoneSelector} > .mud-drop-item";

    [SetUp]
    public async Task BeforeEach()
    {
        await Context.StartTracingAsync();
    }

    [Test]
    public async Task AssignDeviceToRoom()
    {
        await Page.PerformLoginAsync(HausUser.Default);

        await Page.ClickLinkAsync("Devices");
        await Page.ClickLinkAsync("Discovery");

        var originalUnassignedDevicesCount = 0;
        var originalAssignedDevicesCount = 0;
        await Eventually.AssertAsync(async () =>
        {
            originalUnassignedDevicesCount = (await Page.QuerySelectorAllAsync(UnassignedDeviceDropItemSelector)).Count;
            originalAssignedDevicesCount = (
                await Page.QuerySelectorAllAsync(DevicesAssignedToFirstRoomDropItemSelector)
            ).Count;
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
}
