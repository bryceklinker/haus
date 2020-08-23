using System;
using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Web.Clients.Commands;
using Haus.Identity.Web.Tests.Support;
using IdentityServer4.EntityFramework.DbContexts;
using Xunit;

namespace Haus.Identity.Web.Tests.Clients.Commands
{
    public class CreateClientCommandHandlerTests
    {
        private readonly ConfigurationDbContext _context;
        private readonly CreateClientCommandHandler _handler;

        public CreateClientCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreateConfigurationContext();
            _handler = new CreateClientCommandHandler(_context);
        }

        [Fact]
        public async Task WhenClientCreatedThenClientIsAddedToDatabase()
        {
            var command = new CreateClientCommand("");

            await _handler.Handle(command);

            Assert.Single(_context.Clients);
        }
    }
}