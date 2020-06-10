using Haus.Identity.Core.Accounts.Models;
using Haus.Identity.Core.Common.Storage;
using MediatR;
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