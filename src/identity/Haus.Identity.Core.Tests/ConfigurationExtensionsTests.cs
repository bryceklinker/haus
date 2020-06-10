using System.Collections.Generic;
using FluentAssertions;
using Haus.Identity.Core.Tests.Support;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Haus.Identity.Core.Tests
{
    public class ConfigurationExtensionsTests
    {
        private readonly IConfiguration _emptyConfiguration;
        private readonly IConfiguration _populatedConfiguration;
        
        public ConfigurationExtensionsTests()
        {
            _emptyConfiguration = InMemoryConfigurationFactory.CreateEmpty();
            _populatedConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new []
                {
                    new KeyValuePair<string, string>("DB_CONNECTION_STRING", "in-memory"), 
                    new KeyValuePair<string, string>("ADMIN_USERNAME", "will"), 
                    new KeyValuePair<string, string>("ADMIN_PASSWORD", "some-password"), 
                    new KeyValuePair<string, string>("IDENTITY_CLIENT_ID", "haus.identity.custom"), 
                    new KeyValuePair<string, string>("IDENTITY_CLIENT_NAME", "HAUS Identity Custom"), 
                    new KeyValuePair<string, string>("IDENTITY_API_SCOPE", "haus.identity.scope.custom"), 
                    new KeyValuePair<string, string>("IDENTITY_API_NAME", "HAUS Identity Api Custom"), 
                })
                .Build();;
        }
        
        [Fact]
        public void WhenIdentityClientIdIsNotProvidedThenReturnsDefaultIdentityClientId()
        {
            _emptyConfiguration.IdentityClientId().Should().Be("haus.identity");
        }

        [Fact]
        public void WhenIdentityClientNameIsNotProvidedThenReturnsDefaultIdentityClientName()
        {
            _emptyConfiguration.IdentityClientName().Should().Be("HAUS Identity");
        }

        [Fact]
        public void WhenIdentityApiScopeIsNotProvidedThenReturnsDefaultIdentityApiScope()
        {
            _emptyConfiguration.IdentityApiScope().Should().Be("haus.identity");
        }

        [Fact]
        public void WhenIdentityApiNameIsNotProvidedThenReturnsDefaultIdentityApiName()
        {
            _emptyConfiguration.IdentityApiName().Should().Be("HAUS Identity Api");
        }

        [Fact]
        public void WhenAdminUsernameNotProvidedThenReturnsDefaultAdminUsername()
        {
            _emptyConfiguration.AdminUsername().Should().Be("admin");
        }

        [Fact]
        public void WhenAdminPasswordIsNotProvidedThenDefaultAdminPasswordIsUsed()
        {
            _emptyConfiguration.AdminPassword().Should().Be("9FhV7$P^cSuhKXBaHdkr");
        }

        [Fact]
        public void WhenAdminPasswordProvidedThenReturnsProvidedPassword()
        {
            _populatedConfiguration.AdminPassword().Should().Be("some-password");
        }

        [Fact]
        public void WhenAdminUsernameIsProvidedThenReturnsProvidedUsername()
        {
            _populatedConfiguration.AdminUsername().Should().Be("will");
        }

        [Fact]
        public void WhenIdentityClientIdIsProvidedThenReturnsProvidedClientId()
        {
            _populatedConfiguration.IdentityClientId().Should().Be("haus.identity.custom");
        }
        
        [Fact]
        public void WhenIdentityClientNameIsProvidedThenReturnsProvidedClientName()
        {
            _populatedConfiguration.IdentityClientName().Should().Be("HAUS Identity Custom");
        }
        
        [Fact]
        public void WhenIdentityApiScopeIsProvidedThenReturnsProvidedIdentityApiScope()
        {
            _populatedConfiguration.IdentityApiScope().Should().Be("haus.identity.scope.custom");
        }

        [Fact]
        public void WhenIdentityApiNameIsProvidedThenReturnsProvidedIdentityApiName()
        {
            _populatedConfiguration.IdentityApiName().Should().Be("HAUS Identity Api Custom");
        }
    }
}