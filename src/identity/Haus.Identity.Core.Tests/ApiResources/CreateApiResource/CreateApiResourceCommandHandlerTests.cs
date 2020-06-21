using System.Threading.Tasks;
using FluentAssertions;
using Haus.Cqrs;
using Haus.Identity.Core.ApiResources.CreateApiResource;
using Haus.Identity.Core.Tests.Support;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Identity.Core.Tests.ApiResources.CreateApiResource
{
    public class CreateApiResourceCommandHandlerTests
    {
        private readonly ConfigurationDbContext _context;

        public CreateApiResourceCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreateConfigurationDbContext();
        }

        [Fact]
        public async Task WhenApiResourceCreatedThenResourceIsAddedToDatabase()
        {
            var command = new CreateApiResourceCommand("one", "three");

            await Handle(command);

            _context.ApiResources.Should().HaveCount(1);
        }

        [Fact]
        public async Task WhenApiResourceCreatedThenResourceIsPopulatedFromCommand()
        {
            var command = new CreateApiResourceCommand("scope.value", "Display Name Value");

            await Handle(command);

            _context.ApiResources
                .Should()
                .Contain(r => r.Name == "scope.value" && r.DisplayName == "Display Name Value");
        }

        [Fact]
        public async Task WhenApiResourceCreatedThenCommandSucceeds()
        {
            var command = new CreateApiResourceCommand("three", "four");

            var result = await Handle(command);

            result.WasSuccessful.Should().BeTrue();
        }
        
        [Fact]
        public async Task WhenApiResourceAlreadyExistsThenCommandFails()
        {
            var command = new CreateApiResourceCommand("one", "two");
            await Handle(command);
            
            var result = await Handle(command);

            result.WasSuccessful.Should().BeFalse();
            result.Errors.Should().ContainMatch("*one**already exists*");
        }

        private async Task<CreateApiResourceResult> Handle(CreateApiResourceCommand command)
        {
            var messageBus = MessageBusFactory.Create(opts =>
            {
                opts.WithConfigurationDb(_context);
            });

            return await messageBus.ExecuteCommand(command);
        }
    }
}