using System.Threading.Tasks;
using Haus.Identity.Core.Accounts.CreateAccount;
using Haus.Identity.Core.ApiResources.CreateApiResource;
using Haus.Identity.Core.Clients.CreateClient;
using Haus.Identity.Core.IdentityResources;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Identity.Web
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            await app.ExecuteRequest(new CreateAdminAccountCommand());
            await app.ExecuteRequest(new CreateIdentityClientCommand());
            await app.ExecuteRequest(new CreateApiResourceCommand());
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