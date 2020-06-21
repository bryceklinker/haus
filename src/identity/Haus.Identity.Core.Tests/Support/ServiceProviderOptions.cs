using Haus.Identity.Core.Accounts.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Tests.Support
{
    public class ServiceProviderOptions
    {
        public UserManager<HausUser> UserManager { get; set; } = InMemoryUserManagerFactory.Create();
        public IConfiguration Configuration { get; set; } = InMemoryConfigurationFactory.CreateEmpty();

        public ConfigurationDbContext ConfigurationDbContext { get; set; } =
            InMemoryDbContextFactory.CreateConfigurationDbContext();
    }
}