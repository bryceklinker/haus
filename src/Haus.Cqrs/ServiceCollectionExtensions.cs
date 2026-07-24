using System;
using System.Linq;
using System.Reflection;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using Haus.Cqrs.Events;
using Haus.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Haus.Cqrs;

public static class ServiceCollectionExtensions
{
    private static readonly Type[] HandlerInterfaceDefinitions =
    [
        typeof(ICommandHandler<>),
        typeof(ICommandHandler<,>),
        typeof(IQueryHandler<,>),
        typeof(IEventHandler<>),
        typeof(IDomainEventHandler<>),
    ];

    public static IServiceCollection AddHausCqrs(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services
            .AddHandlersFromAssemblies(assemblies)
            .AddTransient<IEventBus>(p => new LoggingEventBus(
                new EventBus(p),
                p.GetRequiredService<ILogger<LoggingEventBus>>()
            ))
            .AddTransient<IQueryBus>(p => new LoggingQueryBus(
                new QueryBus(p),
                p.GetRequiredService<ILogger<LoggingQueryBus>>()
            ))
            .AddTransient<ICommandBus>(p => new LoggingCommandBus(
                new CommandBus(p),
                p.GetRequiredService<ILogger<LoggingCommandBus>>()
            ))
            .AddTransient<IDomainEventBus>(p => new LoggingDomainEventBus(
                new DomainEventBus(p),
                p.GetRequiredService<ILogger<LoggingDomainEventBus>>()
            ))
            .AddTransient<IHausBus, HausBus>();
    }

    private static IServiceCollection AddHandlersFromAssemblies(this IServiceCollection services, Assembly[] assemblies)
    {
        var registrations = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .SelectMany(type =>
                type.GetInterfaces()
                    .Where(i => i.IsGenericType && HandlerInterfaceDefinitions.Contains(i.GetGenericTypeDefinition()))
                    .Select(handlerInterface =>
                        (
                            ConcreteType: type,
                            HandlerInterface: type.IsGenericTypeDefinition
                                ? handlerInterface.GetGenericTypeDefinition()
                                : handlerInterface
                        )
                    )
            );

        foreach (var (concreteType, handlerInterface) in registrations)
            services.AddTransient(handlerInterface, concreteType);

        return services;
    }
}
