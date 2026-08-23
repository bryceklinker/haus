using Haus.Core.Models.Zigbee;
using Haus.Site.Host.Tests.Support;
using Haus.Site.Host.Zigbee.Devices;
using Haus.Testing.Support;
using MudBlazor;

namespace Haus.Site.Host.Tests.Zigbee.Devices;

public class ZigbeeDeviceViewTests : HausSiteTestContext
{
    [Fact]
    public void WhenRenderedThenShowsIeeeAndNetworkAddress()
    {
        var device = HausModelFactory.ZigbeeKnownDeviceModel() with
        {
            IeeeAddress = "00:11:22:33:44:55:66:77",
            NetworkAddress = 4321,
        };

        var view = RenderView<ZigbeeDeviceView>(opts => opts.Add(p => p.Device, device));

        Eventually.Assert(() =>
        {
            Assert.Equal("00:11:22:33:44:55:66:77", view.FindMudTextFieldById<string>("ieeeAddress").GetValue());
            Assert.Equal("4321", view.FindMudTextFieldById<string>("networkAddress").GetValue());
        });
    }

    [Fact]
    public void WhenRenderedThenShowsEndpointsWithClusterIds()
    {
        var device = HausModelFactory.ZigbeeKnownDeviceModel() with
        {
            Endpoints = [new ZigbeeEndpointModel(EndpointId: 1, InClusters: [0, 6], OutClusters: [25])],
        };

        var view = RenderView<ZigbeeDeviceView>(opts => opts.Add(p => p.Device, device));

        Eventually.Assert(() =>
        {
            var endpointText = view.FindByComponent<MudText>(opts => opts.WithText("Endpoint 1"));
            Assert.Contains("In: 0, 6", endpointText.Markup);
            Assert.Contains("Out: 25", endpointText.Markup);
        });
    }
}
