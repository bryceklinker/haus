using System;
using FluentValidation;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Repositories;
using Haus.Core.DeviceSimulator.State;
using Haus.Core.Diagnostics.Factories;
using Haus.Core.Rooms.Repositories;
using Haus.Cqrs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausCore(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
        {
            var coreAssembly = typeof(ServiceCollectionExtensions).Assembly;
            return services.AddSingleton<IClock, Clock>()
                .AddDbContext<HausDbContext>(configureDb)
                .AddTransient<IRoomCommandRepository, RoomCommandRepository>()
                .AddTransient<IDeviceCommandRepository, DeviceCommandRepository>()
                .AddValidatorsFromAssembly(coreAssembly)
                .AddHausCqrs(coreAssembly)
                .AddTransient(p => p.GetRequiredService<IDeviceSimulatorStore>().Current)
                .AddSingleton<IDeviceSimulatorStore, DeviceSimulatorStore>()
                .AddTransient<IRoutableEventFactory, RoutableEventFactory>()
                .AddTransient<IMqttDiagnosticsMessageFactory, MqttDiagnosticsMessageFactory>();
        }
    }
}