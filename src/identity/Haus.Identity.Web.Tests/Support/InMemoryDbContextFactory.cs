using System;
using Haus.Identity.Web.Common.Storage;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Web.Tests.Support
{
    public static class InMemoryDbContextFactory
    {
        public static ConfigurationDbContext CreateConfigurationContext()
        {
            var options = new DbContextOptionsBuilder<ConfigurationDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}")
                .Options;
            return new ConfigurationDbContext(options, new ConfigurationStoreOptions());
        }

        public static HausIdentityDbContext CreateIdentityContext()
        {
            var options = new DbContextOptionsBuilder<HausIdentityDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}")
                .Options;
            return new HausIdentityDbContext(options);
        }
    }
}