using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Core.Accounts;
using Haus.Identity.Core.ApiResources;
using Haus.Identity.Core.Clients;
using Haus.Identity.Core.IdentityResources;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Identity.Web.Common.Storage
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            var serverAddresses = app.ServerFeatures.Get<IServerAddressesFeature>();
            var httpsAddress = serverAddresses.Addresses.Single(a => a.StartsWith("https://"));
            await app.ExecuteRequest(new SeedAdminAccountRequest());
            await app.ExecuteRequest(new SeedIdentityClientRequest($"{httpsAddress}/auth-callback"));
            await app.ExecuteRequest(new SeedIdentityApiResourceRequest());
            await app.ExecuteRequest(new SeedIdentityResourcesRequest(
                new IdentityResources.Profile(),
                new IdentityResources.OpenId(),
                new IdentityResources.Email()
            ));
        }
        
        private static async Task ExecuteRequest<TRequest>(this IApplicationBuilder app, TRequest request)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediatr.Send(request);
        }
    }
}