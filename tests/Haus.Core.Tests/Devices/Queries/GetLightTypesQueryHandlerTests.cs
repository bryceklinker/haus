using System.Threading.Tasks;
using Haus.Core.Devices.Queries;
using Haus.Core.Models.Devices;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Queries;

public class GetLightTypesQueryHandlerTests
{
    private readonly IHausBus _hausBus = HausBusFactory.Create();

    [Fact]
    public async Task WhenGettingLightTypesThenReturnsExcludesNone()
    {
        var result = await _hausBus.ExecuteQueryAsync(new GetLightTypesQuery());

        Assert.DoesNotContain(LightType.None, result.Items);
    }

    [Fact]
    public async Task WhenGettingLightTypesThenReturnsAvailableLightTypes()
    {
        var result = await _hausBus.ExecuteQueryAsync(new GetLightTypesQuery());

        Assert.Equal(3, result.Count);
        Assert.Equal(3, result.Items.Length);
        Assert.Contains(LightType.Color, result.Items);
        Assert.Contains(LightType.Level, result.Items);
        Assert.Contains(LightType.Temperature, result.Items);
    }
}
