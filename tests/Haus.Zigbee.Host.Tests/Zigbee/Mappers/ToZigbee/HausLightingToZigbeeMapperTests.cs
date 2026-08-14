using System.Linq;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToZigbee;

public class HausLightingToZigbeeMapperTests
{
    private const ushort OnOffCluster = 0x0006;
    private const ushort LevelControlCluster = 0x0008;
    private const ushort ColorControlCluster = 0x0300;
    private const byte OffCommand = 0x00;
    private const byte OnCommand = 0x01;
    private const byte MoveToLevelWithOnOffCommand = 0x04;
    private const byte MoveToColorCommand = 0x07;
    private const byte MoveToColorTemperatureCommand = 0x0a;

    private readonly HausLightingToZigbeeMapper _mapper = new();
    private readonly ApsDestination _destination = ApsDestination.Ieee(new IeeeAddress(1), 1);

    [Fact]
    public void WhenTypeIsDeviceLightingChangedThenIsSupported()
    {
        Assert.True(_mapper.IsSupported(DeviceLightingChangedEvent.Type));
    }

    [Fact]
    public void WhenTypeIsNotDeviceLightingChangedThenUnsupported()
    {
        Assert.False(_mapper.IsSupported("anything else"));
    }

    [Fact]
    public void Map_OffState_ReturnsOnlyOffCommand()
    {
        var lighting = new LightingModel(
            LightingState.Off,
            new LevelLightingModel(54),
            new TemperatureLightingModel(4000),
            new ColorLightingModel(98, 54, 234)
        );

        var result = _mapper.Map(_destination, lighting).ToArray();

        var request = Assert.Single(result);
        Assert.Equal(OnOffCluster, request.ClusterId);
        Assert.Equal(OffCommand, request.CommandId);
        Assert.Empty(request.Payload);
    }

    [Fact]
    public void Map_OnStateNoTemperatureOrColor_ReturnsOnAndLevelCommands()
    {
        var lighting = new LightingModel(LightingState.On, new LevelLightingModel(54));

        var result = _mapper.Map(_destination, lighting).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Contains(result, r => r.ClusterId == OnOffCluster && r.CommandId == OnCommand && r.Payload.Length == 0);
        var level = Assert.Single(result, r => r.ClusterId == LevelControlCluster);
        Assert.Equal(MoveToLevelWithOnOffCommand, level.CommandId);
        Assert.Equal(137, level.Payload[0]);
    }

    [Fact]
    public void Map_WithTemperature_IncludesMoveToColorTemperatureCommand()
    {
        var lighting = new LightingModel(
            LightingState.On,
            new LevelLightingModel(54),
            new TemperatureLightingModel(4000)
        );

        var result = _mapper.Map(_destination, lighting).ToArray();

        var command = Assert.Single(
            result,
            r => r.ClusterId == ColorControlCluster && r.CommandId == MoveToColorTemperatureCommand
        );
        var mireds = (ushort)(command.Payload[0] | (command.Payload[1] << 8));
        Assert.Equal(250, mireds);
    }

    [Fact]
    public void Map_WithColor_IncludesMoveToColorCommandWithApproximateXy()
    {
        var lighting = new LightingModel(
            LightingState.On,
            new LevelLightingModel(54),
            Color: new ColorLightingModel(255, 0, 0)
        );

        var result = _mapper.Map(_destination, lighting).ToArray();

        var command = Assert.Single(
            result,
            r => r.ClusterId == ColorControlCluster && r.CommandId == MoveToColorCommand
        );
        var x = (ushort)(command.Payload[0] | (command.Payload[1] << 8));
        var y = (ushort)(command.Payload[2] | (command.Payload[3] << 8));
        Assert.InRange(x / 65536.0, 0.69, 0.71);
        Assert.InRange(y / 65536.0, 0.29, 0.31);
    }

    [Fact]
    public void Map_WithoutTemperatureOrColor_DoesNotIncludeColorControlCommands()
    {
        var lighting = new LightingModel(LightingState.On, new LevelLightingModel(54));

        var result = _mapper.Map(_destination, lighting).ToArray();

        Assert.DoesNotContain(result, r => r.ClusterId == ColorControlCluster);
    }
}
