using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Haus.Portal.Web.Tests.Support
{
    public static class InMemoryConfiguration
    {
        public const string AuthorityUrl = "https://something.com";
        
        public static IConfiguration Get()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>(nameof(AuthorityUrl), AuthorityUrl), 
                })
                .Build();
        }
    }
}