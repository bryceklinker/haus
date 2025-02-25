using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haus.Testing.Support;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Replace<TService>(this IServiceCollection services, object implementation)
    {
        return services.Replace(new ServiceDescriptor(typeof(TService), implementation));
    }
}
