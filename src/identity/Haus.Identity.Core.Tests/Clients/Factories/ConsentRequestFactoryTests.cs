using System.Collections.Generic;
using FluentAssertions;
using Haus.Identity.Core.Clients.Factories;
using IdentityServer4.Models;
using Xunit;

namespace Haus.Identity.Core.Tests.Clients.Factories
{
    public class ConsentRequestFactoryTests
    {
        private const string ReturnUrl = "https://netflix.com";
        private readonly ConsentRequestFactory _factory;
        private readonly Client _client;

        public ConsentRequestFactoryTests()
        {
            _client = new Client
            {
                ClientName = "Booba",
                ClientId = "Netflix",
            };
            _factory = new ConsentRequestFactory();
        }
        
        [Fact]
        public void WhenCreatedThenClientValuesAreMappedToRequest()
        {
            var request = _factory.Create(_client,ReturnUrl);

            request.ClientId.Should().Be(_client.ClientId);
            request.ClientName.Should().Be(_client.ClientName);
        }

        [Fact]
        public void WhenCreatedThenReturnUrlIsSetOnRequest()
        {
            var request = _factory.Create(_client, ReturnUrl);

            request.ReturnUrl.Should().Be(ReturnUrl);
        }

        [Fact]
        public void WhenCreatedThenIdentityResourcesAreMappedToRequest()
        {
            var identityResources = new[]
            {
                new IdentityResource
                {
                    Name = "profile",
                    DisplayName = "User profile information",
                    Required = true,
                    Emphasize = true
                },
            };

            var request = _factory.Create(_client, ReturnUrl, identityResources);

            request.IdentityScopes.Should().HaveCount(1);
            request.IdentityScopes[0].Name.Should().Be("profile");
            request.IdentityScopes[0].DisplayName.Should().Be("User profile information");
            request.IdentityScopes[0].Required.Should().BeTrue();
            request.IdentityScopes[0].Emphasize.Should().BeTrue();
        }

        [Fact]
        public void WhenCreatedThenApiResourceScopesAreMappedToRequest()
        {
            var apiResources = new[]
            {
                new ApiResource
                {
                    Scopes = new List<Scope>
                    {
                        new Scope("haus.identity", "HAUS Identity")
                        {
                            Description = "Used for something",
                            Emphasize = true,
                            Required = true
                        }
                    }
                }
            };

            var request = _factory.Create(_client, ReturnUrl, apiResources: apiResources);

            request.ResourceScopes.Should().HaveCount(1);
            request.ResourceScopes[0].Name.Should().Be("haus.identity");
            request.ResourceScopes[0].DisplayName.Should().Be("HAUS Identity");
            request.ResourceScopes[0].Emphasize.Should().BeTrue();
            request.ResourceScopes[0].Required.Should().BeTrue();
        }
    }
}