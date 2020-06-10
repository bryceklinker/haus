using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.IdentityResources;
using Haus.Identity.Core.Tests.Support;
using IdentityModel;
using IdentityServer4.Models;
using MediatR;
using Xunit;

namespace Haus.Identity.Core.Tests.IdentityResources
{
    public class IdentityResourceSeederTests
    {
        [Fact]
        public void WhenIdentityResourceIsMissingThenProfileResourceShouldBeCreated()
        {
            IdentityResourceSeeder.ShouldCreateIdentityResource("profile", Array.Empty<string>())
                .Should()
                .BeTrue();
        }
        
        [Fact]
        public void WhenIdentityResourceExistsThenIdentityResourceShouldNotBeCreated()
        {
            IdentityResourceSeeder.ShouldCreateIdentityResource("profile", new []{"profile"})
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task WhenSeedIdentityResourcesRequestedThenProfileIdentityResourceIsAvailable()
        {
            var resource = new IdentityResource
            {
                Name = "bobo"
            };
            var context = InMemoryDbContextFactory.CreateConfigurationDbContext();
            
            IRequestHandler<SeedIdentityResourcesRequest> handler = new IdentityResourceSeeder(context);
            await handler.Handle(new SeedIdentityResourcesRequest(resource), CancellationToken.None);

            context.Set<IdentityServer4.EntityFramework.Entities.IdentityResource>().Should().HaveCount(1);
        }
    }
}