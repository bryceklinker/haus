using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace Haus.Site.Host.Shared.SignalR;

public interface ISignalRConnectionFactory
{
    Task<HubConnection> CreateAsync(string hubName);
}

public class SignalRConnectionFactory(IConfiguration config, IAccessTokenProvider tokenProvider)
    : ISignalRConnectionFactory
{
    private string? ApiUrl => config.GetValue<string>("Api:BaseUrl");

    public Task<HubConnection> CreateAsync(string hubName)
    {
        ArgumentException.ThrowIfNullOrEmpty(ApiUrl);

        var connection = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl(
                $"{ApiUrl}/hubs/{hubName}",
                opts =>
                {
                    opts.AccessTokenProvider = async () =>
                    {
                        var token = await tokenProvider.RequestAccessToken().ConfigureAwait(false);
                        return token.TryGetToken(out var accessToken) ? accessToken.Value : null;
                    };
                }
            )
            .Build();

        return Task.FromResult(connection);
    }
}
