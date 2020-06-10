using System;
using Haus.Identity.Core.Common.Storage;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.Tests.Support
{
    public static class InMemoryDbContextFactory
    {
        public static ConfigurationDbContext CreateConfigurationDbContext()
        {
            var options = CreateInMemoryOptions<ConfigurationDbContext>();
            return new ConfigurationDbContext(options, new ConfigurationStoreOptions());
        }

        public static HausIdentityDbContext CreateIdentityDbContext()
        {
            var options = CreateInMemoryOptions<HausIdentityDbContext>();
            return new HausIdentityDbContext(options);
        }

        private static DbContextOptions<TContext> CreateInMemoryOptions<TContext>()
            where TContext : DbContext
        {
            return new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}")
                .Options;
        }
    }
}