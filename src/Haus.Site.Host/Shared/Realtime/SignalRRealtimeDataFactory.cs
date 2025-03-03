using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace Haus.Site.Host.Shared.Realtime;

public class SignalRRealtimeDataFactory(IConfiguration config, IAccessTokenProvider tokenProvider)
    : IRealtimeDataFactory
{
    private string? ApiUrl => config.GetValue<string>("Api:BaseUrl");

    public Task<IRealtimeDataSubscriber> CreateSubscriber(string source)
    {
        ArgumentException.ThrowIfNullOrEmpty(ApiUrl);

        var connection = new HubConnectionBuilder()
            .WithAutomaticReconnect()
            .WithUrl(
                $"{ApiUrl}{source}",
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
        return Task.FromResult<IRealtimeDataSubscriber>(new SignalRRealtimeDataSubscriber(connection));
    }
}
