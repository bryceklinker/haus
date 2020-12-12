using System;
using Haus.Core;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.DomainEvents;
using Haus.Core.Common.Events;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haus.Testing.Support
{
    public static class HausBusFactory
    {
        public static IHausBus Create(HausDbContext context = null)
        {
            var services = CreateServicesCollection(context);

            return services.BuildServiceProvider()
                .GetRequiredService<IHausBus>();
        }

        public static CapturingHausBus CreateCapturingBus(HausDbContext context = null)
        {
            var services = CreateServicesCollection(context);

            services.RemoveAll<IHausBus>();
            services.AddSingleton<IHausBus>(p =>
            {
                var commandBus = p.GetRequiredService<ICommandBus>();
                var eventBus = p.GetRequiredService<IEventBus>();
                var queryBus = p.GetRequiredService<IQueryBus>();
                var domainEventBus = p.GetRequiredService<IDomainEventBus>();
                return new CapturingHausBus(new HausBus(commandBus, queryBus, eventBus, domainEventBus));
            });

            return (CapturingHausBus) services.BuildServiceProvider()
                .GetRequiredService<IHausBus>();
        }

        private static IServiceCollection CreateServicesCollection(HausDbContext context)
        {
            var services = new ServiceCollection()
                .AddHausCore(opts => opts.UseInMemoryDatabase($"{Guid.NewGuid()}"))
                .AddLogging();

            if (context != null)
            {
                services.RemoveAll(typeof(HausDbContext));
                services.AddSingleton(context);
            }

            return services;
        }
    }
}