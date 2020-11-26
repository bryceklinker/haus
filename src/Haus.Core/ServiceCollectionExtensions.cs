using System;
using AutoMapper;
using FluentValidation;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Diagnostics.Factories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Haus.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausCore(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
        {
            var coreAssembly = typeof(ServiceCollectionExtensions).Assembly;
            return services.AddSingleton<IClock, Clock>()
                .AddAutoMapper(coreAssembly)
                .AddDbContext<HausDbContext>(configureDb)
                .AddMediatR(coreAssembly)
                .AddValidatorsFromAssembly(coreAssembly)
                .AddTransient<IEventBus>(p => new LoggingEventBus(new EventBus(p.GetRequiredService<IMediator>()), p.GetRequiredService<ILogger<LoggingEventBus>>()))
                .AddTransient<IQueryBus>(p => new LoggingQueryBus(new QueryBus(p.GetRequiredService<IMediator>()), p.GetRequiredService<ILogger<LoggingQueryBus>>()))
                .AddTransient<ICommandBus>(p => new LoggingCommandBus(new CommandBus(p.GetRequiredService<IMediator>()), p.GetRequiredService<ILogger<LoggingCommandBus>>()))
                .AddTransient<IHausBus, HausBus>()
                .AddTransient<IRoutableEventFactory, RoutableEventFactory>()
                .AddTransient<IMqttDiagnosticsMessageFactory, MqttDiagnosticsMessageFactory>();
        }
    }
}