using System;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models.Devices;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class DeviceTypesApiTests(HausWebHostApplicationFactory factory)
{
    private readonly IHausApiClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task WhenGettingDeviceTypesThenAllDeviceTypesAreReturned()
    {
        var types = await _client.GetDeviceTypesAsync();

        Assert.Equal(Enum.GetValues<DeviceType>().Length, types.Count);
        foreach (var deviceType in Enum.GetValues<DeviceType>())
        {
            Assert.Contains(deviceType, types.Items);
        }
    }
}
