using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.ApiResources;
using Haus.Identity.Core.Tests.Support;
using IdentityServer4.EntityFramework.Entities;
using MediatR;
using Xunit;

namespace Haus.Identity.Core.Tests.ApiResources
{
    public class IdentityApiResourceSeederTests
    {
        [Fact]
        public void WhenIdentityApiResourceDoesNotExistThenShouldCreateIdentityApiResource()
        {
            var resources = Array.Empty<string>();

            IdentityApiResourceSeeder.ShouldCreateIdentityApiResource("Haus Identity Api", resources)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void WhenIdentityApiResourceExistsThenShouldNotCreateIdentityApiResource()
        {
            var resources = new []{"Haus Identity Api"};

            IdentityApiResourceSeeder.ShouldCreateIdentityApiResource("Haus Identity Api", resources)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void WhenIdentityApiResourceIsCreatedThenReturnsPopulatedApiResource()
        {
            var resource = IdentityApiResourceSeeder.CreateIdentityApiResource("Haus Identity Api", "haus.identity");

            resource.Name.Should().Be("haus.identity");
            resource.DisplayName.Should().Be("Haus Identity Api");
            resource.Scopes.Select(s => s.Name).Should().Contain("haus.identity");
        }

        [Fact]
        public async Task WhenIdentityApiResourceIsSeededThenIdentityApiResourceIsAddedToApiResources()
        {
            var config = InMemoryConfigurationFactory.CreateEmpty();
            var context = InMemoryDbContextFactory.CreateConfigurationDbContext();
            
            IRequestHandler<SeedIdentityApiResourceRequest> handler = new IdentityApiResourceSeeder(context, config);
            await handler.Handle(new SeedIdentityApiResourceRequest(), CancellationToken.None);

            context.Set<ApiResource>().Should().HaveCount(1);
        }
    }
}