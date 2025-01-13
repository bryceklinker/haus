using Fluxor;

namespace Haus.Site.Host.Shared.Settings;

public static class SettingsReducer
{
    [ReducerMethod]
    public static SettingsState Reduce(SettingsState state, SettingsLoadedAction action)
    {
        return action.Payload;
    }
}