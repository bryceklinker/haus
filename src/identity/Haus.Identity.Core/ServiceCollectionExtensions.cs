using Haus.Identity.Core.Accounts.Models;
using Haus.Identity.Core.Storage;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Identity.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausIdentityCore(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly)
                .AddIdentity<HausUser, HausRole>()
                .AddEntityFrameworkStores<HausIdentityDbContext>();
            return services;
        }
    }
}