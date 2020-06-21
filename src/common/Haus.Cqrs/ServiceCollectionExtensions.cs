using System.Linq;
using System.Reflection;
using Haus.Cqrs.Commands;
using Haus.Cqrs.Events;
using Haus.Cqrs.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Cqrs
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausCqrs(this IServiceCollection services, params Assembly[] assemblies)
        {
            return services.AddMediatR(assemblies.Append(typeof(ServiceCollectionExtensions).Assembly).ToArray())
                .AddTransient<IMessageBus, MessageBus>()
                .AddTransient<ICommandBus, CommandBus>()
                .AddTransient<IQueryBus, QueryBus>()
                .AddTransient<IEventBus, EventBus>();
        }
    }
}