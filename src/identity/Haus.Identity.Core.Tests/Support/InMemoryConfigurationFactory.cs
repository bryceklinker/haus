using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Tests.Support
{
    public static class InMemoryConfigurationFactory
    {
        public static IConfiguration Create()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("DB_CONNECTION_STRING", "in-memory"), 
                    new KeyValuePair<string, string>("ADMIN_USERNAME", "admin"), 
                    new KeyValuePair<string, string>("ADMIN_PASSWORD", "some-password"), 
                    new KeyValuePair<string, string>("IDENTITY_CLIENT_ID", "haus.identity"), 
                    new KeyValuePair<string, string>("IDENTITY_CLIENT_NAME", "HAUS Identity"), 
                    new KeyValuePair<string, string>("IDENTITY_API_SCOPE", "haus.identity.scope"), 
                })
                .Build();
        }
    }
}