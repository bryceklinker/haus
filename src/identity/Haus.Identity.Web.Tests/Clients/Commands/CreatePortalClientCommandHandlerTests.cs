using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Models.Messages;
using Haus.Identity.Web.Clients.Commands;
using Haus.Testing.Utilities.Cqrs;
using Haus.Testing.Utilities.ServiceBus;
using Xunit;

namespace Haus.Identity.Web.Tests.Clients.Commands
{
    public class CreatePortalClientCommandHandlerTests
    {
        private readonly FakeHausServiceBusPublisher _publisher;
        private readonly FakeCqrsBus _cqrsBus;
        private readonly CreatePortalClientCommandHandler _handler;

        public CreatePortalClientCommandHandlerTests()
        {
            _publisher = new FakeHausServiceBusPublisher();
            _cqrsBus = new FakeCqrsBus();
            _handler = new CreatePortalClientCommandHandler(_cqrsBus, _publisher);
        }

        [Fact]
        public async Task WhenPortalClientCreatedThenPortalClientCreatedMessagePublished()
        {
            var command = new CreatePortalClientCommand("https://something.com");
            _cqrsBus.SetupCommandResult(command.CreateClientCommand, new CreateClientResult("id", "secret"));

            await _handler.Handle(command);

            var message = _publisher.GetMessages<PortalClientCreatedPayload>().Single();
            Assert.Equal("id", message.Payload.ClientId);
            Assert.Equal("secret", message.Payload.ClientSecret);
        }

        [Fact]
        public async Task WhenCreatePortalClientHandledThenClientIsCreated()
        {
            var command = new CreatePortalClientCommand("https://idk.com");
            _cqrsBus.SetupCommandResult(command.CreateClientCommand, new CreateClientResult("one", "three"));

            await _handler.Handle(command);

            Assert.Single(_cqrsBus.GetExecutedCommand<CreateClientCommand, CreateClientResult>());
        }
    }
}