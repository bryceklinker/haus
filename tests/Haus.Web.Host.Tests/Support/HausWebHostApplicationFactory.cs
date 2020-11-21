using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Testing.Support.Fakes;
using Haus.Web.Host.Common.Mqtt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Extensions.ManagedClient;

namespace Haus.Web.Host.Tests.Support
{
    public class HausWebHostApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly FakeClock _clock;

        public HausWebHostApplicationFactory()
        {
            _clock = new FakeClock();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IClock>(_clock);
                services.AddAuthentication(opts =>
                    {
                        opts.DefaultAuthenticateScheme = TestingAuthenticationHandler.TestingScheme;
                        opts.DefaultChallengeScheme = TestingAuthenticationHandler.TestingScheme;
                        opts.DefaultScheme = TestingAuthenticationHandler.TestingScheme;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestingAuthenticationHandler>(TestingAuthenticationHandler.TestingScheme, opts => { });
            });
        }

        public HttpClient CreateAuthenticatedClient()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(TestingAuthenticationHandler.TestingScheme);

            return client;
        }

        public async Task<HubConnection> CreateHubConnection(string hub)
        {
            CreateClient();
            var connection = new HubConnectionBuilder()
                .WithUrl(
                    $"http://localhost/hubs/{hub}",
                    o =>
                    {
                        o.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                        o.AccessTokenProvider = () => Task.FromResult(TestingAuthenticationHandler.TestingScheme);
                    })
                .Build();
            await connection.StartAsync();
            return connection;
        }

        public async Task<IManagedMqttClient> GetMqttClient()
        {
            using var scope = Services.CreateScope();
            var creator = scope.ServiceProvider.GetRequiredService<IMqttClientCreator>();
            return await creator.CreateClient();
        }

        public void SetClockTime(DateTime time)
        {
            _clock.SetNow(time);
        }
    }
}