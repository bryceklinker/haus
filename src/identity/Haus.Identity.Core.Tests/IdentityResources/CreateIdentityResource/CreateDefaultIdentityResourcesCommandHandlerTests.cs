using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.IdentityResources.CreateIdentityResource;
using Haus.Identity.Core.Tests.Support;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Xunit;

namespace Haus.Identity.Core.Tests.IdentityResources.CreateIdentityResource
{
    public class CreateDefaultIdentityResourcesCommandHandlerTests
    {
        private readonly ConfigurationDbContext _context;

        public CreateDefaultIdentityResourcesCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreateConfigurationDbContext();
        }

        [Fact]
        public async Task WhenIdentityResourceIsMissingThenProvidedResourcesAreAdded()
        {
            var command = new CreateDefaultIdentityResourceCommand(
                new IdentityResource("openid", new []{"openid"}),
                new IdentityResource("profile", new []{"profile"}));

            await Handle(command);

            _context.IdentityResources.Should().Contain(r => r.Name == "openid");
            _context.IdentityResources.Should().Contain(r => r.Name == "profile");
        }
        
        [Fact]
        public async Task WhenIdentityResourceExistsThenIdentityResourceShouldNotBeCreated()
        {
            var command = new CreateDefaultIdentityResourceCommand(
                new IdentityResource("profile", new []{"profile"})
            );
            await Handle(command);

            await Handle(command);

            _context.IdentityResources.Should().HaveCount(1);
        }

        private async Task Handle(CreateDefaultIdentityResourceCommand command)
        {
            var messageBus = MessageBusFactory.Create(opts => opts.WithConfigurationDb(_context));
            await messageBus.ExecuteCommand(command);
        }
    }
}