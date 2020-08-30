using System.Reflection;
using GreenPipes;
using Haus.ServiceBus.Publish;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.ServiceBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausServiceBus(this IServiceCollection services, IConfiguration config, params Assembly[] assemblies)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumers(assemblies);
                x.SetKebabCaseEndpointNameFormatter();
                
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                    cfg.Host(config["ServiceBus:Hostname"], "/", rabbitConfig =>
                    {
                        rabbitConfig.Password(config["ServiceBus:Password"]);
                        rabbitConfig.Username(config["ServiceBus:Username"]);
                    });
                });
            });
            return services.AddMassTransitHostedService()
                .AddTransient<IHausServiceBusPublisher, HausServiceBusPublisher>();;
        }
    }
}