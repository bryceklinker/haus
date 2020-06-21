using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.Clients.CreateClient;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Tests.Support;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Identity.Core.Tests.Clients.CreateClient
{
    public class CreateClientCommandHandlerTests
    {
        private readonly ConfigurationDbContext _context;

        public CreateClientCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreateConfigurationDbContext();
        }

        [Fact]
        public async Task WhenClientCreatedThenClientIsAddedToDatabase()
        {
            var command = new CreateClientCommand(
                "something",
                "Billy");

            var result = await Handle(command);

            result.WasSuccessful.Should().BeTrue();
            _context.Clients.Should().Contain(c => c.ClientId == "something" && c.ClientName == "Billy");
        }

        [Fact]
        public async Task WhenDuplicateClientCreatedThenReturnsError()
        {
            var command = new CreateClientCommand("bob", "jack");
            await Handle(command);
            
            var result = await Handle(command);

            result.WasSuccessful.Should().BeFalse();
            result.Errors.Should().ContainMatch("*'bob'*already*exists*");
        }
        
        private async Task<CreateClientResult> Handle(CreateClientCommand command)
        {
            var messageBus = ServiceProviderFactory.CreateProvider(opts =>
            {
                opts.WithConfigurationDb(_context);
            }).GetRequiredService<IMessageBus>();
            return await messageBus.ExecuteCommand(command);
        }
    }
}