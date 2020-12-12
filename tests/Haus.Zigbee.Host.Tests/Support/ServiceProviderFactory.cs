using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Haus.Zigbee.Host.Tests.Support
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider Create(IConfiguration configuration = null)
        {
            return new ServiceCollection()
                .AddLogging(builder => builder.ClearProviders())
                .AddHausZigbee(configuration ?? ConfigurationFactory.CreateConfig())
                .BuildServiceProvider();
        }
    }
}