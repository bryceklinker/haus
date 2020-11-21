using Haus.Core.Common;
using Haus.Core.Diagnostics.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausCore(this IServiceCollection services)
        {
            return services.AddSingleton<IClock, Clock>()
                .AddTransient<IMqttDiagnosticsMessageFactory, MqttDiagnosticsMessageFactory>();
        }
    }
}