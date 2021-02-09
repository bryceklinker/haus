using Microsoft.Extensions.DependencyInjection;

namespace Haus.Udp.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausUdp(this IServiceCollection services)
        {
            return services
                .AddSingleton<IHausUdpFactory, HausUdpFactory>()
                .AddTransient(p => p.GetRequiredService<IHausUdpFactory>().GetBroadcaster())
                .AddTransient(p => p.GetRequiredService<IHausUdpFactory>().GetSubscriber());
        }
    }
}