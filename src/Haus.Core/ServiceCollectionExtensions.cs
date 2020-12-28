using System;
using AutoMapper;
using FluentValidation;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Common.Queries;
using Haus.Core.Common.Storage;
using Haus.Core.Diagnostics.Factories;
using Haus.Cqrs;
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
                .AddValidatorsFromAssembly(coreAssembly)
                .AddHausCqrs(coreAssembly)
                .AddTransient<IRoutableEventFactory, RoutableEventFactory>()
                .AddTransient<IMqttDiagnosticsMessageFactory, MqttDiagnosticsMessageFactory>();
        }
    }
}