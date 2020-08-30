using System;
using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Models.Clients;
using Haus.Identity.Web.Clients.Commands;
using Haus.Identity.Web.Tests.Support;
using Haus.Testing.Utilities.ServiceBus;
using IdentityServer4.EntityFramework.DbContexts;
using Xunit;

namespace Haus.Identity.Web.Tests.Clients.Commands
{
    public class CreateClientCommandHandlerTests
    {
        private readonly FakeHausServiceBusPublisher _publisher;
        private readonly ConfigurationDbContext _context;
        private readonly CreateClientCommandHandler _handler;

        public CreateClientCommandHandlerTests()
        {
            _publisher = new FakeHausServiceBusPublisher();
            _context = InMemoryDbContextFactory.CreateConfigurationContext();
            _handler = new CreateClientCommandHandler(_context, _publisher);
        }

        [Fact]
        public async Task WhenClientCreatedThenClientIsAddedToDatabase()
        {
            var command = new CreateClientCommand("");

            await _handler.Handle(command);

            Assert.Single(_context.Clients);
        }

        [Fact]
        public async Task WhenClientCreatedThenClientCreatedIsPublished()
        {
            var command = new CreateClientCommand("somename");
            
            await _handler.Handle(command);

            var payload = _publisher.GetMessages<ClientCreatedPayload>().Single();
            Assert.Equal(command.ClientId, payload.ClientId);
            Assert.Equal(command.Secret, payload.ClientSecret);
            Assert.Equal("somename", payload.ClientName);
        }
    }
}