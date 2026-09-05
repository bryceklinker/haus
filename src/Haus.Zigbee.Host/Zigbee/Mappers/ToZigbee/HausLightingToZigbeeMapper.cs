using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;

public class HausLightingToZigbeeMapper
{
    private const ushort HomeAutomationProfile = 0x0104;
    private const byte CoordinatorSourceEndpoint = 0x01;

    private const ushort OnOffCluster = 0x0006;
    private const ushort LevelControlCluster = 0x0008;
    private const ushort ColorControlCluster = 0x0300;

    private const byte OffCommand = 0x00;
    private const byte OnCommand = 0x01;
    private const byte MoveToLevelWithOnOffCommand = 0x04;
    private const byte MoveToColorCommand = 0x07;
    private const byte MoveToColorTemperatureCommand = 0x0a;

    private const ushort NoTransitionTime = 0;
    private const double MaxZclLevel = 254.0;
    private const double MicroKelvinsPerMired = 1_000_000.0;
    private const double ZclColorComponentScale = 65536.0;

    public bool IsSupported(string type)
    {
        return type == DeviceLightingChangedEvent.Type;
    }

    // Prefers whichever endpoint exposes the cluster a lighting command would target first --
    // On/Off, then Level, then Color -- so a split-cluster device still gets one destination that
    // can at least turn on/off, falling back to the first discovered endpoint if none of those
    // clusters are present, and to the caller-supplied default when no endpoints are known at all
    // (e.g. a device persisted before endpoint discovery existed).
    public byte ResolveDestinationEndpoint(IReadOnlyList<DeviceEndpointModel> endpoints, byte fallbackEndpoint)
    {
        if (endpoints.Count == 0)
            return fallbackEndpoint;

        var preferred =
            endpoints.FirstOrDefault(e => e.InClusters.Contains(OnOffCluster))
            ?? endpoints.FirstOrDefault(e => e.InClusters.Contains(LevelControlCluster))
            ?? endpoints.FirstOrDefault(e => e.InClusters.Contains(ColorControlCluster))
            ?? endpoints[0];

        return preferred.EndpointId;
    }

    public IEnumerable<ZigbeeCommandRequest> Map(ApsDestination destination, LightingModel lighting)
    {
        if (lighting.State == LightingState.Off)
        {
            yield return CreateRequest(destination, OnOffCluster, OffCommand, []);
            yield break;
        }

        yield return CreateRequest(destination, OnOffCluster, OnCommand, []);
        yield return CreateRequest(
            destination,
            LevelControlCluster,
            MoveToLevelWithOnOffCommand,
            LevelPayload(lighting.Level)
        );

        if (lighting.Temperature is { } temperature)
            yield return CreateRequest(
                destination,
                ColorControlCluster,
                MoveToColorTemperatureCommand,
                ColorTemperaturePayload(temperature)
            );

        if (lighting.Color is { } color)
            yield return CreateRequest(destination, ColorControlCluster, MoveToColorCommand, ColorPayload(color));
    }

    private static ZigbeeCommandRequest CreateRequest(
        ApsDestination destination,
        ushort clusterId,
        byte commandId,
        byte[] payload
    )
    {
        return new ZigbeeCommandRequest(
            destination,
            CoordinatorSourceEndpoint,
            HomeAutomationProfile,
            clusterId,
            commandId,
            payload,
            DisableDefaultResponse: false
        );
    }

    private static byte[] LevelPayload(LevelLightingModel level)
    {
        var zclLevel = (byte)Math.Round(level.Value / 100.0 * MaxZclLevel);
        return [zclLevel, .. LittleEndianBytes(NoTransitionTime)];
    }

    private static byte[] ColorTemperaturePayload(TemperatureLightingModel temperature)
    {
        var mireds = (ushort)Math.Round(MicroKelvinsPerMired / temperature.Value);
        return [.. LittleEndianBytes(mireds), .. LittleEndianBytes(NoTransitionTime)];
    }

    private static byte[] ColorPayload(ColorLightingModel color)
    {
        var (x, y) = ToXyChromaticity(color);
        var zclX = (ushort)Math.Round(x * ZclColorComponentScale);
        var zclY = (ushort)Math.Round(y * ZclColorComponentScale);
        return [.. LittleEndianBytes(zclX), .. LittleEndianBytes(zclY), .. LittleEndianBytes(NoTransitionTime)];
    }

    private static byte[] LittleEndianBytes(ushort value)
    {
        var bytes = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(bytes, value);
        return bytes;
    }

    // Wide RGB D65 conversion matrix, the same one zigbee2mqtt/zigbee-herdsman-converters use to
    // turn an sRGB color into the CIE 1931 xy chromaticity coordinates ZCL's color cluster expects.
    private static (double X, double Y) ToXyChromaticity(ColorLightingModel color)
    {
        var red = color.Red / 255.0;
        var green = color.Green / 255.0;
        var blue = color.Blue / 255.0;

        var x = red * 0.664511 + green * 0.154324 + blue * 0.162028;
        var y = red * 0.283881 + green * 0.668433 + blue * 0.047685;
        var z = red * 0.000088 + green * 0.07231 + blue * 0.986039;

        var sum = x + y + z;
        return sum == 0 ? (0, 0) : (x / sum, y / sum);
    }
}
