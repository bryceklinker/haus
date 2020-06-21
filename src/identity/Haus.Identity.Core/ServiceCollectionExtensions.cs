using Haus.Cqrs;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Clients.CreateClient;
using Haus.Identity.Core.Common.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Identity.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausIdentityCore(this IServiceCollection services)
        {
            services.AddHausCqrs(typeof(ServiceCollectionExtensions).Assembly)
                .AddTransient<IClientFactory, ClientFactory>()
                .AddIdentity<HausUser, HausRole>()
                .AddEntityFrameworkStores<HausIdentityDbContext>();
            return services;
        }
    }
}