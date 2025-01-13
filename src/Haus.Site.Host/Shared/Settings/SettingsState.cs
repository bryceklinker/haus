using Fluxor;

namespace Haus.Site.Host.Shared.Settings;

public record ApiSettingsModel(string BaseUrl);

public record AuthSettingsModel(string Domain, string ClientId, string Audience);

[FeatureState]
public record SettingsState(ApiSettingsModel? Api = null, AuthSettingsModel? Auth = null);