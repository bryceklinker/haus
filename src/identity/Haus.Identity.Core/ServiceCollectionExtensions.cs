using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Clients.CreateClient;
using Haus.Identity.Core.Common.Messaging;
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
                .AddTransient<IMessageBus, MessageBus>()
                .AddTransient<ICommandBus, CommandBus>()
                .AddTransient<IQueryBus, QueryBus>()
                .AddTransient<IEventBus, EventBus>()
                .AddTransient<IClientFactory, ClientFactory>()
                .AddIdentity<HausUser, HausRole>()
                .AddEntityFrameworkStores<HausIdentityDbContext>();
            return services;
        }
    }
}