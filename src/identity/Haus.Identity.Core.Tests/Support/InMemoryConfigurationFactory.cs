using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Tests.Support
{
    public static class InMemoryConfigurationFactory
    {
        public const string DefaultIdentityClientRedirectUri = "https://localhost:5001/auth-callback";
        
        public static IConfiguration CreateEmpty()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("IdentityClient:RedirectUri", DefaultIdentityClientRedirectUri), 
                })
                .Build();
        }
    }
}