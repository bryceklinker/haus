using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Haus.Core.Models.Zigbee;

namespace Haus.Core.Zigbee.State;

public interface IZigbeeState
{
    ZigbeeConnectionStatusModel ConnectionStatus { get; }
    ImmutableArray<ZigbeeActivityEntryModel> RecentActivity { get; }
    ImmutableDictionary<string, ZigbeeKnownDeviceModel> KnownDevices { get; }
    IZigbeeState UpdateConnectionStatus(ZigbeeConnectionStatusModel status);
    IZigbeeState RecordActivity(ZigbeeActivityEntryModel entry);
    IZigbeeState RecordDeviceJoined(string ieeeAddress, ushort networkAddress, DateTimeOffset seenAt);
    IZigbeeState RecordDeviceInfoDiscovered(
        string ieeeAddress,
        string manufacturerName,
        string modelIdentifier,
        IReadOnlyList<ZigbeeEndpointModel> endpoints,
        DateTimeOffset seenAt
    );
}

public record ZigbeeState(
    ZigbeeConnectionStatusModel ConnectionStatus,
    ImmutableArray<ZigbeeActivityEntryModel> RecentActivity,
    ImmutableDictionary<string, ZigbeeKnownDeviceModel> KnownDevices
) : IZigbeeState
{
    public const int MaxRecentActivity = 100;

    public static readonly ZigbeeState Initial = new(
        ZigbeeConnectionStatusModel.Unknown,
        ImmutableArray<ZigbeeActivityEntryModel>.Empty,
        ImmutableDictionary<string, ZigbeeKnownDeviceModel>.Empty
    );

    public IZigbeeState UpdateConnectionStatus(ZigbeeConnectionStatusModel status)
    {
        return this with { ConnectionStatus = status };
    }

    public IZigbeeState RecordActivity(ZigbeeActivityEntryModel entry)
    {
        var updated = RecentActivity.Add(entry);
        if (updated.Length > MaxRecentActivity)
            updated = updated.RemoveAt(0);

        return this with
        {
            RecentActivity = updated,
        };
    }

    public IZigbeeState RecordDeviceJoined(string ieeeAddress, ushort networkAddress, DateTimeOffset seenAt)
    {
        var existing = KnownDevices.GetValueOrDefault(ieeeAddress);
        var device = existing is null
            ? new ZigbeeKnownDeviceModel(ieeeAddress, networkAddress, null, null, [], seenAt)
            : existing with
            {
                NetworkAddress = networkAddress,
                LastSeenAt = seenAt,
            };

        return this with
        {
            KnownDevices = KnownDevices.SetItem(ieeeAddress, device),
        };
    }

    public IZigbeeState RecordDeviceInfoDiscovered(
        string ieeeAddress,
        string manufacturerName,
        string modelIdentifier,
        IReadOnlyList<ZigbeeEndpointModel> endpoints,
        DateTimeOffset seenAt
    )
    {
        var existing = KnownDevices.GetValueOrDefault(ieeeAddress);
        var device = existing is null
            ? new ZigbeeKnownDeviceModel(ieeeAddress, null, manufacturerName, modelIdentifier, endpoints, seenAt)
            : existing with
            {
                ManufacturerName = manufacturerName,
                ModelIdentifier = modelIdentifier,
                Endpoints = endpoints,
                LastSeenAt = seenAt,
            };

        return this with
        {
            KnownDevices = KnownDevices.SetItem(ieeeAddress, device),
        };
    }
}
