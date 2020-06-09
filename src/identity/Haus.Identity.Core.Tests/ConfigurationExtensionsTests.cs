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
            _emptyConfiguration = new ConfigurationBuilder().Build();
            _populatedConfiguration = InMemoryConfigurationFactory.Create();
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
    }
}