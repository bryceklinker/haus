using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Core.Accounts;
using Haus.Identity.Core.ApiResources;
using Haus.Identity.Core.Clients;
using Haus.Identity.Core.IdentityResources;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Identity.Web
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSpaAuthentication(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                if (context.User.IsAuthenticated())
                {
                    await next();
                }
                else
                {
                    await context.ChallengeAsync();
                }
            });
        }
    
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app, string clientRedirectUri)
        {
            await app.ExecuteRequest(new SeedAdminAccountRequest());
            await app.ExecuteRequest(new SeedIdentityClientRequest(clientRedirectUri));
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