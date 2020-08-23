using System;
using System.Linq;
using Haus.Identity.Web.Clients.Commands;
using IdentityModel;
using Xunit;

namespace Haus.Identity.Web.Tests.Clients.Commands
{
    public class CreateClientCommandTests
    {
        [Fact]
        public void WhenConvertedToClientThenClientNameIsPopulated()
        {
            var client = new CreateClientCommand("hello")
                .ToClient();
            
            Assert.Equal("hello", client.ClientName);
        }

        [Fact]
        public void WhenConvertedToClientThenRedirectUriIsPopulated()
        {
            var client = new CreateClientCommand("", redirectUri: "https://localhost:5000")
                .ToClient();

            Assert.Contains("https://localhost:5000", client.RedirectUris);
        }

        [Fact]
        public void WhenConvertedToClientMissingRedirectUriThenClientHasEmptyRedirectUris()
        {
            var client = new CreateClientCommand("")
                .ToClient();

            Assert.Empty(client.RedirectUris);
        }

        [Fact]
        public void WhenConvertedToClientThenClientIdIsGenerated()
        {
            var command = new CreateClientCommand("");
            var client = command.ToClient();
            
            Assert.True(Guid.TryParse(client.ClientId, out _));
            Assert.Equal(command.ClientId, client.ClientId);
        }
        
        [Fact]
        public void WhenConvertedToClientWithoutSecretGenerationThenSecretGenerationIsSkipped()
        {
            var client = new CreateClientCommand("one", false)
                .ToClient();

            Assert.Empty(client.ClientSecrets);
        }

        [Fact]
        public void WhenConvertedToClientThenSecretIsGenerated()
        {
            var client = new CreateClientCommand("three")
                .ToClient();

            Assert.Single(client.ClientSecrets);
        }

        [Fact]
        public void WhenConvertedToClientThenGeneratedSecretIsAvailable()
        {
            var command = new CreateClientCommand("");
            var client = command.ToClient();
            
            Assert.Equal(client.ClientSecrets.Single().Value, command.Secret.ToSha256());
        }

        [Fact]
        public void WhenConvertedToClientThenDefaultGrantTypesAreAllowed()
        {
            var client = new CreateClientCommand("three")
                .ToClient();

            Assert.Contains(OidcConstants.GrantTypes.AuthorizationCode, client.AllowedGrantTypes);
            Assert.Contains(OidcConstants.GrantTypes.ClientCredentials, client.AllowedGrantTypes);
        }

        [Fact]
        public void WhenConvertedToClientThenDefaultScopesAreAllowed()
        {
            var client = new CreateClientCommand("")
                .ToClient();

            Assert.Contains(OidcConstants.StandardScopes.Profile, client.AllowedScopes);
            Assert.Contains(OidcConstants.StandardScopes.OpenId, client.AllowedScopes);
        }

        [Fact]
        public void WhenConvertedToClientThenDefaultOriginsAreAllowed()
        {
            var client = new CreateClientCommand("")
                .ToClient();

            Assert.Contains("*", client.AllowedCorsOrigins);
        }

        [Fact]
        public void WhenConvertedToResultThenSecretAndIdArePopulated()
        {
            var command = new CreateClientCommand("");
            var result = command
                .ToResult();

            Assert.Equal(command.ClientId, result.Id);
            Assert.Equal(command.Secret, result.Secret);
        }

        [Fact]
        public void WhenConvertedToClientWithoutNameThenNameIsClientId()
        {
            var command = new CreateClientCommand(null);
            var client = command.ToClient();

            Assert.Equal(command.ClientId, client.ClientName);
        }
    }
}