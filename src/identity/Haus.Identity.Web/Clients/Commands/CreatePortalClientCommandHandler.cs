using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using Haus.Identity.Models.Messages;
using Haus.ServiceBus.Common;
using Haus.ServiceBus.Publish;

namespace Haus.Identity.Web.Clients.Commands
{
    public class CreatePortalClientCommandHandler : CommandHandler<CreatePortalClientCommand>
    {
        private readonly ICqrsBus _cqrsBus;
        private readonly IHausServiceBusPublisher _publisher;

        public CreatePortalClientCommandHandler(ICqrsBus cqrsBus, IHausServiceBusPublisher publisher)
        {
            _cqrsBus = cqrsBus;
            _publisher = publisher;
        }

        protected override async Task InnerHandle(CreatePortalClientCommand command, CancellationToken token = default)
        {
            var result = await _cqrsBus.ExecuteCommand(command.CreateClientCommand, token);
            var payload = new PortalClientCreatedPayload(result.Id, result.Secret);
            _publisher.Publish(new ServiceBusMessage<PortalClientCreatedPayload>(PortalClientCreatedPayload.Type, payload));
        }
    }
}