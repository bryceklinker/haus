using System;
using Haus.ServiceBus.Common;
using Haus.ServiceBus.Publish;
using Haus.ServiceBus.Subscribe;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.ServiceBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausServiceBus(this IServiceCollection services, Action<ServiceBusOptions> configureOptions)
        {
            services.AddOptions<ServiceBusOptions>()
                .Configure(configureOptions);

            return services.AddTransient<IHausServiceBusPublisher, HausServiceBusPublisher>()
                .AddTransient<IHausServiceBusSubscriber, HausServiceBusSubscriber>();
        }
    }
}