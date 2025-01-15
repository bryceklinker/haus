using System;
using System.Threading.Tasks;
using Fluxor;
using Haus.Site.Host.Shared.State.Settings;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace Haus.Site.Host.Shared.SignalR;

public interface ISignalRConnectionFactory
{
    Task<HubConnection> CreateAsync(string hubName);
}

public class SignalRConnectionFactory(IState<SettingsState> settingsState, IAccessTokenProvider tokenProvider) : ISignalRConnectionFactory
{
    private string? ApiUrl => settingsState.Value.Api?.BaseUrl;
    
    public Task<HubConnection> CreateAsync(string hubName)
    {
        ArgumentException.ThrowIfNullOrEmpty(ApiUrl);
        
        var connection =  new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl($"{ApiUrl}/hubs/{hubName}", opts =>
            {
                opts.AccessTokenProvider = async () =>
                {
                    var token = await tokenProvider.RequestAccessToken().ConfigureAwait(false);
                    return token.TryGetToken(out var accessToken) 
                        ? accessToken.Value 
                        : null;
                };
            })
            .Build();
        
        return Task.FromResult(connection);
    }
}