using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.IdentityResources.CreateIdentityResource;
using Haus.Identity.Core.Tests.Support;
using IdentityServer4.EntityFramework.DbContexts;
using Xunit;

namespace Haus.Identity.Core.Tests.IdentityResources.CreateIdentityResource
{
    public class CreateIdentityResourceCommandHandlerTests
    {
        private readonly ConfigurationDbContext _context;

        public CreateIdentityResourceCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreateConfigurationDbContext();
        }

        [Fact]
        public async Task WhenIdentityResourceCreatedThenResourceIsAddedToDatabase()
        {
            var command = new CreateIdentityResourceCommand("one", "three", new []{"idk"});

            await Handle(command);

            var resource = _context.IdentityResources.Single();
            resource.Name.Should().Be("one");
            resource.DisplayName.Should().Be("three");
            resource.UserClaims.Should().Contain(c => c.Type == "idk");
        }

        [Fact]
        public async Task WhenIdentityResourceCreatedThenCommandSucceeds()
        {
            var command = new CreateIdentityResourceCommand("idk", "i", new []{"lk"});

            var result = await Handle(command);

            result.WasSuccessful.Should().BeTrue();
        }

        [Fact]
        public async Task WhenIdentityResourceAlreadyExistsThenCommandFails()
        {
            var command = new CreateIdentityResourceCommand("one", "three", new []{"idk"});
            await Handle(command);
            
            var result = await Handle(command);

            result.WasSuccessful.Should().BeFalse();
            result.Errors.Should().ContainMatch("*'one'*already*exists*");
        }

        private async Task<CreateIdentityResourceResult> Handle(CreateIdentityResourceCommand command)
        {
            var messageBus = MessageBusFactory.Create(opts => opts.WithConfigurationDb(_context));
            return await messageBus.ExecuteCommand(command);
        }
    }
}