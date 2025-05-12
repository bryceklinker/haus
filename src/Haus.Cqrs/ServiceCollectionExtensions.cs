using System.Reflection;
using Haus.Cqrs.Commands;
using Haus.Cqrs.DomainEvents;
using Haus.Cqrs.Events;
using Haus.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Haus.Cqrs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausCqrs(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies);
            })
            .AddTransient<IEventBus>(p => new LoggingEventBus(
                new EventBus(p.GetRequiredService<IMediator>()),
                p.GetRequiredService<ILogger<LoggingEventBus>>()
            ))
            .AddTransient<IQueryBus>(p => new LoggingQueryBus(
                new QueryBus(p.GetRequiredService<IMediator>()),
                p.GetRequiredService<ILogger<LoggingQueryBus>>()
            ))
            .AddTransient<ICommandBus>(p => new LoggingCommandBus(
                new CommandBus(p.GetRequiredService<IMediator>()),
                p.GetRequiredService<ILogger<LoggingCommandBus>>()
            ))
            .AddTransient<IDomainEventBus>(p => new LoggingDomainEventBus(
                new DomainEventBus(p.GetRequiredService<IMediator>()),
                p.GetRequiredService<ILogger<LoggingDomainEventBus>>()
            ))
            .AddTransient<IHausBus, HausBus>();
    }
}
