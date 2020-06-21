using FluentAssertions;
using Haus.Identity.Core.Clients.CreateClient;
using IdentityModel;
using Xunit;

namespace Haus.Identity.Core.Tests.Clients.CreateClient
{
    public class ClientFactoryTests
    {
        private readonly ClientFactory _clientFactory;

        public ClientFactoryTests()
        {
            _clientFactory = new ClientFactory();
        }

        [Fact]
        public void WhenClientCreatedThenClientIsPopulatedFromCommand()
        {
            var client = _clientFactory.CreateFromCommand(new CreateClientCommand("haus.identity", "Haus"));

            client.ClientId.Should().Be("haus.identity");
            client.ClientName.Should().Be("Haus");
        }

        [Fact]
        public void WhenClientCreatedThenProvidedScopesAreAddedToClient()
        {
            var client =
                _clientFactory.CreateFromCommand(new CreateClientCommand("", "",
                    scopes: new[] {"some.one", "read.only"}));

            client.AllowedScopes.Should().Contain("some.one");
            client.AllowedScopes.Should().Contain("read.only");
        }

        [Fact]
        public void WhenClientCreatedThenProvidedRedirectUrisAreAddedToClient()
        {
            var client = _clientFactory.CreateFromCommand(new CreateClientCommand("", "", redirectUris: new[]
            {
                "https://something.com/auth",
                "http://google.com"
            }));

            client.RedirectUris.Should().Contain("https://something.com/auth");
            client.RedirectUris.Should().Contain("http://google.com");
        }

        [Fact]
        public void WhenClientCreatedThenProvidedCorsOriginsAreAddedToClient()
        {
            var client = _clientFactory.CreateFromCommand(new CreateClientCommand("", "", allowedCorsOrigins: new[]
            {
                "https://bob.com"
            }));

            client.AllowedCorsOrigins.Should().Contain("https://bob.com");
        }
        
        [Fact]
        public void WhenClientCreatedThenAccessTokensAreAllowedInTheBrowser()
        {
            var client = _clientFactory.CreateFromCommand(new CreateClientCommand("", ""));

            client.AllowAccessTokensViaBrowser.Should().BeTrue();
        }
        
        [Fact]
        public void WhenClientCreatedThenDefaultScopesAreAdded()
        {
            var client = _clientFactory.CreateFromCommand(new CreateClientCommand("", ""));
            
            client.AllowedScopes.Should().Contain(OidcConstants.StandardScopes.OpenId);
            client.AllowedScopes.Should().Contain(OidcConstants.StandardScopes.Email);
            client.AllowedScopes.Should().Contain(OidcConstants.StandardScopes.Profile);
            client.AllowedScopes.Should().Contain(OidcConstants.StandardScopes.OfflineAccess);
        }

        [Fact]
        public void WhenClientCreatedThenHasDefaultGrantTypes()
        {
            var client = _clientFactory.CreateFromCommand(new CreateClientCommand("", ""));
            
            client.AllowedGrantTypes.Should().Contain(OidcConstants.GrantTypes.Implicit);
            client.AllowedGrantTypes.Should().Contain(OidcConstants.GrantTypes.Password);
            client.AllowedGrantTypes.Should().Contain(OidcConstants.GrantTypes.ClientCredentials);
        }

        [Fact]
        public void WhenClientCreatedThenClientSecretIsOptional()
        {
            var client = _clientFactory.CreateFromCommand(new CreateClientCommand("", ""));

            client.RequireClientSecret.Should().BeFalse();
        }

        [Fact]
        public void WhenClientCreatedThenConsentIsOptional()
        {
            var client = _clientFactory.CreateFromCommand(new CreateClientCommand("", ""));

            client.RequireConsent.Should().BeFalse();
        }
    }
}