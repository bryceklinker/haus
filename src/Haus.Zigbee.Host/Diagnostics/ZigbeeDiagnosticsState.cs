using System.Collections.Immutable;
using Haus.Zigbee.Models.Diagnostics;

namespace Haus.Zigbee.Host.Diagnostics;

public record ZigbeeDiagnosticsState(
    ZigbeeConnectionStatusModel ConnectionStatus,
    ImmutableArray<ZigbeeActivityEntryModel> RecentActivity
)
{
    public const int MaxRecentActivity = 100;

    public static readonly ZigbeeDiagnosticsState Initial = new(
        ZigbeeConnectionStatusModel.Unknown,
        ImmutableArray<ZigbeeActivityEntryModel>.Empty
    );

    public ZigbeeDiagnosticsState UpdateConnectionStatus(ZigbeeConnectionStatusModel status)
    {
        return this with { ConnectionStatus = status };
    }

    public ZigbeeDiagnosticsState RecordActivity(ZigbeeActivityEntryModel entry)
    {
        var updated = RecentActivity.Add(entry);
        if (updated.Length > MaxRecentActivity)
            updated = updated.RemoveAt(0);

        return this with
        {
            RecentActivity = updated,
        };
    }
}
