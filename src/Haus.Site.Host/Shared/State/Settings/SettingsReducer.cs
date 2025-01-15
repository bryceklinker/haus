using Fluxor;

namespace Haus.Site.Host.Shared.State.Settings;

public static class SettingsReducer
{
    [ReducerMethod]
    public static SettingsState ReduceSettingsLoaded(SettingsState state, SettingsLoadedAction action)
    {
        return action.Payload;
    }
}