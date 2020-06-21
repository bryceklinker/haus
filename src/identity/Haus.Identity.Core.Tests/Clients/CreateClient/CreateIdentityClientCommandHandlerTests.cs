using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.Clients.CreateClient;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Tests.Support;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Identity.Core.Tests.Clients.CreateClient
{
    public class CreateIdentityClientCommandHandlerTests
    {
        private readonly IConfiguration _configuration;
        private readonly ConfigurationDbContext _context;

        public CreateIdentityClientCommandHandlerTests()
        {
            _configuration = InMemoryConfigurationFactory.CreateEmpty();
            _context = InMemoryDbContextFactory.CreateConfigurationDbContext();
        }

        [Fact]
        public async Task WhenIdentityClientCreatedThenClientAllowsIdentityApiScope()
        {
            await Handle(new CreateIdentityClientCommand());

            var client = _context.Clients.Single();
            client.ClientId.Should().Be("haus.identity");
            client.ClientName.Should().Be("HAUS Identity");
            client.AllowedScopes.Should().Contain(s => s.Scope == "haus.identity");
        }

        [Fact]
        public async Task WhenIdentityClientAlreadyExistsThenDuplicateClientIsNotAdded()
        {
            await Handle(new CreateIdentityClientCommand());
            await Handle(new CreateIdentityClientCommand());

            _context.Clients.Should().HaveCount(1);
        }
        
        [Fact]
        public async Task WhenIdentityClientCreatedThenClientIsAddedToAvailableClients()
        {
            await Handle(new CreateIdentityClientCommand());

            var client = _context.Clients.Single();
            client.RedirectUris.Should()
                .Contain(r => r.RedirectUri == InMemoryConfigurationFactory.DefaultIdentityClientRedirectUri);
        }

        private async Task Handle(CreateIdentityClientCommand command)
        {
            var messageBus = ServiceProviderFactory.CreateProvider(opts =>
            {
                opts.WithConfiguration(_configuration)
                    .WithConfigurationDb(_context);
            }).GetRequiredService<IMessageBus>();
            await messageBus.ExecuteCommand(command);
        }
    }
}