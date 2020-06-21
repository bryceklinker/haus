using Haus.Identity.Core.Accounts.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Tests.Support
{
    public class ServiceProviderOptionsBuilder
    {
        private ServiceProviderOptions _options = new ServiceProviderOptions();
        
        public ServiceProviderOptionsBuilder WithUserManager(UserManager<HausUser> userManager)
        {
            _options.UserManager = userManager;
            return this;
        }

        public ServiceProviderOptionsBuilder WithConfiguration(IConfiguration configuration)
        {
            _options.Configuration = configuration;
            return this;
        }

        public ServiceProviderOptionsBuilder WithConfigurationDb(ConfigurationDbContext context)
        {
            _options.ConfigurationDbContext = context;
            return this;
        }

        public ServiceProviderOptions Build()
        {
            try
            {
                return _options;
            }
            finally
            {
                _options = new ServiceProviderOptions();
            }
        }
    }
}