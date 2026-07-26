using Haus.Core.Models.Discovery;
using Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToZigbee;

public class HausDiscoveryToZigbeeMapperTests
{
    private readonly HausDiscoveryToZigbeeMapper _mapper = new();

    [Fact]
    public void WhenTypeIsStartDiscoveryThenIsSupported()
    {
        Assert.True(_mapper.IsSupported(StartDiscoveryModel.Type));
    }

    [Fact]
    public void WhenTypeIsStopDiscoveryThenIsSupported()
    {
        Assert.True(_mapper.IsSupported(StopDiscoveryModel.Type));
    }

    [Fact]
    public void WhenTypeIsSyncDiscoveryThenIsSupported()
    {
        Assert.True(_mapper.IsSupported(SyncDiscoveryModel.Type));
    }

    [Fact]
    public void WhenTypeIsNotADiscoveryTypeThenUnsupported()
    {
        Assert.False(_mapper.IsSupported("not discovery"));
    }

    [Fact]
    public void Map_StartDiscovery_ReturnsSetPermitJoinIntentEnabled()
    {
        var intent = _mapper.Map(StartDiscoveryModel.Type);

        Assert.Equal(ZigbeeDiscoveryIntentType.SetPermitJoin, intent.Type);
        Assert.True(intent.PermitJoinEnabled);
    }

    [Fact]
    public void Map_StopDiscovery_ReturnsSetPermitJoinIntentDisabled()
    {
        var intent = _mapper.Map(StopDiscoveryModel.Type);

        Assert.Equal(ZigbeeDiscoveryIntentType.SetPermitJoin, intent.Type);
        Assert.False(intent.PermitJoinEnabled);
    }

    [Fact]
    public void Map_SyncDiscovery_ReturnsSyncDevicesIntent()
    {
        var intent = _mapper.Map(SyncDiscoveryModel.Type);

        Assert.Equal(ZigbeeDiscoveryIntentType.SyncDevices, intent.Type);
    }
}
