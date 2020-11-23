using System;
using Haus.Core.Common;
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
            return services.AddSingleton<IClock, Clock>()
                .AddDbContext<HausDbContext>(configureDb)
                .AddMediatR(typeof(ServiceCollectionExtensions).Assembly)
                .AddTransient<IEventBus>(p => new LoggingEventBus(new EventBus(p.GetRequiredService<IMediator>()), p.GetRequiredService<ILogger<LoggingEventBus>>()))
                .AddTransient<IQueryBus>(p => new LoggingQueryBus(new QueryBus(p.GetRequiredService<IMediator>()), p.GetRequiredService<ILogger<LoggingQueryBus>>()))
                .AddTransient<IRoutableEventFactory, RoutableEventFactory>()
                .AddTransient<IMqttDiagnosticsMessageFactory, MqttDiagnosticsMessageFactory>();
        }
    }
}