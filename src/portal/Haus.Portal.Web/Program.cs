using System;
using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Hosting;
using Haus.Identity.Models.ApiResources;
using Haus.Identity.Models.Clients;
using Haus.Portal.Web.Common.Storage;
using Haus.Portal.Web.Settings;
using Haus.Portal.Web.Settings.Entities;
using Haus.Portal.Web.Settings.Queries;
using Haus.ServiceBus.Publish;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Portal.Web
{
    public class Program : HausProgram
    {
        public Program(string[] args)
            : base("Haus Portal Web", args)
        {
        }

        public static async Task<int> Main(string[] args)
        {
            var program = new Program(args);
            return await program.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            GetHausHostBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        protected override void ConfigureHost(IHostBuilder builder)
        {
            builder.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

        protected override async Task BeforeRunAsync(IHost host)
        {
            await host.MigrateDatabaseAsync<HausPortalDbContext>();
            await WaitForClientSettingsToBeReady(host);
        }

        private async Task WaitForClientSettingsToBeReady(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IHausServiceBusPublisher>();
            await publisher.PublishAsync(new CreateApiResourcePayload(AuthSettings.ApiResourceIdentifier, AuthSettings.Scopes, AuthSettings.ClientName));
            await publisher.PublishAsync(new CreateClientPayload(AuthSettings.ClientName, "https://localhost:5001", AuthSettings.Scopes));
        }
    }
}