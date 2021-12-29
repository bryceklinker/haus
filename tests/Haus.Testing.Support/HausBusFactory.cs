using System;
using Haus.Core;
using Haus.Core.Common.Storage;
using Haus.Core.DeviceSimulator.State;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using Haus.Cqrs.Events;
using Haus.Cqrs.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haus.Testing.Support;

public static class HausBusFactory
{
    public static IHausBus Create(HausDbContext context = null, Action<IServiceCollection> configureServices = null)
    {
        var services = CreateServicesCollection(context, configureServices);

        return services.BuildServiceProvider()
            .GetRequiredService<IHausBus>();
    }

    public static CapturingHausBus CreateCapturingBus(IDeviceSimulatorStore simulatorStore)
    {
        return CreateCapturingBus(configureServices: services => services.AddSingleton(simulatorStore));
    }

    public static CapturingHausBus CreateCapturingBus(HausDbContext context = null,
        Action<IServiceCollection> configureServices = null)
    {
        var services = CreateServicesCollection(context, configureServices);

        services.RemoveAll<IHausBus>();
        services.AddSingleton<IHausBus>(p =>
        {
            var commandBus = p.GetRequiredService<ICommandBus>();
            var eventBus = p.GetRequiredService<IEventBus>();
            var queryBus = p.GetRequiredService<IQueryBus>();
            var domainEventBus = p.GetRequiredService<IDomainEventBus>();
            return new CapturingHausBus(commandBus, queryBus, eventBus, domainEventBus);
        });

        return (CapturingHausBus)services.BuildServiceProvider()
            .GetRequiredService<IHausBus>();
    }

    private static IServiceCollection CreateServicesCollection(HausDbContext context,
        Action<IServiceCollection> configureServices = null)
    {
        var services = new ServiceCollection()
            .AddHausCore(opts => opts.UseInMemoryDatabase($"{Guid.NewGuid()}"))
            .AddLogging();

        if (context != null)
        {
            services.RemoveAll(typeof(HausDbContext));
            services.AddSingleton(context);
        }

        configureServices?.Invoke(services);
        return services;
    }
}