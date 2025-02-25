using System;
using System.Threading.Tasks;
using FluentAssertions;
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

        types.Count.Should().Be(Enum.GetValues<DeviceType>().Length);
        types.Items.Should().Contain(Enum.GetValues<DeviceType>());
    }
}
