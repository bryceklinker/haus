using System.Collections.Generic;
using FluentAssertions;
using Haus.Identity.Core.Clients.Factories;
using IdentityServer4.Models;
using Xunit;

namespace Haus.Identity.Core.Tests.Clients.Models
{
    public class ConsentRequestTests
    {
        private readonly ConsentRequestFactory _factory;

        public ConsentRequestTests()
        {
            _factory = new ConsentRequestFactory();
        }
        
        [Fact]
        public void WhenRequestHasIdentityResourcesThenReturnsIdentityResourceScopeNamesInConsentedScopes()
        {
            var identityResources = new []
            {
                new IdentityResource { Name = "something" },
                new IdentityResource { Name = "One" },
                new IdentityResource { Name = "Two" },
            };
            var request = _factory.Create(new Client(), "", identityResources);

            request.ConsentedScopes.Should().Contain("something");
            request.ConsentedScopes.Should().Contain("One");
            request.ConsentedScopes.Should().Contain("Two");
        }

        [Fact]
        public void WhenRequestHasApiResourcesThenReturnsApiResourceScopesInConsentedScopes()
        {
            var apiResources = new[]
            {
                new ApiResource
                {
                    Scopes = new List<Scope>
                    {
                        new Scope("hello"),
                        new Scope("jack"),
                    }
                },
                new ApiResource
                {
                    Scopes = new List<Scope>
                    {
                        new Scope("bob")
                    }
                },
            };

            var request = _factory.Create(new Client(), "", apiResources: apiResources);

            request.ConsentedScopes.Should().Contain("hello");
            request.ConsentedScopes.Should().Contain("jack");
            request.ConsentedScopes.Should().Contain("bob");
        }
    }
}