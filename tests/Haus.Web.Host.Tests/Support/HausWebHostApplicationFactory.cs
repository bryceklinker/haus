using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Api.Client.Options;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Models.ExternalMessages;
using Haus.Testing.Support.Fakes;
using Haus.Web.Host.Common.Mqtt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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
                services.AddHausApiClient(opts =>
                    {
                        opts.BaseUrl = Server.BaseAddress.ToString();
                    })
                    .RemoveAll(typeof(IHttpClientFactory))
                    .AddSingleton<IHttpClientFactory>(new FakeHttpClientFactory(CreateHttpClientWithAuth));
                
                services.AddAuthentication(opts =>
                    {
                        opts.DefaultAuthenticateScheme = TestingAuthenticationHandler.TestingScheme;
                        opts.DefaultChallengeScheme = TestingAuthenticationHandler.TestingScheme;
                        opts.DefaultScheme = TestingAuthenticationHandler.TestingScheme;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestingAuthenticationHandler>(TestingAuthenticationHandler.TestingScheme, opts => { });
                services.AddDbContext<HausDbContext>(opts => opts.UseInMemoryDatabase("HausDB"));
            });
        }

        public IHausApiClient CreateAuthenticatedClient()
        {
            return Services.GetRequiredService<IHausApiClient>();
        }

        public IHausApiClient CreateUnauthenticatedClient()
        {
            var factory = new FakeHttpClientFactory(CreateClient);
            var options = Services.GetRequiredService<IOptions<HausApiClientSettings>>();
            return new HausApiClientFactory(factory, options).Create();
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

        public async Task<IHausMqttClient> GetMqttClient()
        {
            var creator = Services.GetRequiredService<IHausMqttClientFactory>();
            return await creator.CreateClient();
        }

        public async Task PublishHausEventAsync<T>(IHausEventConverter<T> converter)
        {
            var client = await GetMqttClient();
            await client.PublishAsync("haus/events", converter.AsHausEvent());
        }
        
        public void SetClockTime(DateTime time)
        {
            _clock.SetNow(time);
        }

        private HttpClient CreateHttpClientWithAuth()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestingAuthenticationHandler.TestingScheme);
            return client;
        }
    }
}