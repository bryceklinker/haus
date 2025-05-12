using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Haus.Site.Host.Shared.Realtime;

public enum RealtimeDataState
{
    Connected,
    Disconnected,
    Connecting,
    Reconnecting,
}

public static class RealtimeDataStateExtensions
{
    public static RealtimeDataState ToRealtimeDataState(this HubConnectionState state)
    {
        return state switch
        {
            HubConnectionState.Disconnected => RealtimeDataState.Disconnected,
            HubConnectionState.Connected => RealtimeDataState.Connected,
            HubConnectionState.Connecting => RealtimeDataState.Connecting,
            HubConnectionState.Reconnecting => RealtimeDataState.Reconnecting,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
        };
    }
}
