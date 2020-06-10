using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.Clients;
using Haus.Identity.Core.Clients.Models;
using Haus.Identity.Core.Tests.Support;
using IdentityModel;
using IdentityServer4.EntityFramework.Entities;
using MediatR;
using Xunit;

namespace Haus.Identity.Core.Tests.Clients
{
    public class IdentityClientSeederTests
    {
        [Fact]
        public void WhenIdentityClientExistsThenShouldNotCreateIdentityClient()
        {
            var existingClientNames = new[] {"haus.identity", "haus.other", "more"};

            IdentityClientSeeder.ShouldCreateIdentityClient("haus.identity", existingClientNames)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void WhenIdentityClientDoesNotExistThenIdentityClientShouldBeCreated()
        {
            IdentityClientSeeder.ShouldCreateIdentityClient("haus.identity", Array.Empty<string>())
                .Should()
                .BeTrue();
        }

        [Fact]
        public void WhenIdentityClientExistsWithDifferentCasingThenIdentityClientShouldNotBeCreated()
        {
            IdentityClientSeeder.ShouldCreateIdentityClient("haus.identity", new[] {"HAUS.identity"})
                .Should()
                .BeFalse();
        }

        [Fact]
        public void WhenIdentityClientCreatedThenClientHasTheCorrectApiResource()
        {
            var request = new CreateClientRequest
            {
                Id = "haus.identity",
                Name = "HAUS Identity",
                Scopes = new []
                {
                    "haus.identity"
                }
            };
            var client = IdentityClientSeeder.CreateIdentityClient(request);

            client.ClientId.Should().Be("haus.identity");
            client.ClientName.Should().Be("HAUS Identity");
            client.AllowedScopes.Should().Contain("haus.identity");
        }

        [Fact]
        public void WhenIdentityClientCreatedThenClientHasDefaultScopesAllowed()
        {
            var client = IdentityClientSeeder.CreateIdentityClient(new CreateClientRequest());

            client.AllowedScopes.Should().Contain(OidcConstants.StandardScopes.OpenId);
            client.AllowedScopes.Should().Contain(OidcConstants.StandardScopes.Email);
            client.AllowedScopes.Should().Contain(OidcConstants.StandardScopes.Profile);
            client.AllowedScopes.Should().Contain(OidcConstants.StandardScopes.OfflineAccess);
        }

        [Fact]
        public async Task WhenIdentityClientIsSeededThenClientIsAddedToAvailableClients()
        {
            var config = InMemoryConfigurationFactory.CreateEmpty();
            var context = InMemoryDbContextFactory.CreateConfigurationDbContext();
            
            IRequestHandler<SeedIdentityClientRequest> seeder = new IdentityClientSeeder(context, config);

            await seeder.Handle(new SeedIdentityClientRequest(), CancellationToken.None);
            
            context.Set<Client>().Should().HaveCount(1);
        }
    }
}