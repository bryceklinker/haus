using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Tests.Support
{
    public static class InMemoryConfigurationFactory
    {
        public static IConfiguration CreateEmpty()
        {
            return new ConfigurationBuilder()
                .Build();
        }
    }
}