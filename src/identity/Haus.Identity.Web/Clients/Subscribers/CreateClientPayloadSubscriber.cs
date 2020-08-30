using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Identity.Models.Clients;
using Haus.Identity.Web.Clients.Commands;
using Haus.ServiceBus.Subscribe;
using MassTransit;

namespace Haus.Identity.Web.Clients.Subscribers
{
    public class CreateClientPayloadSubscriber : IHausServiceBusSubscriber<CreateClientPayload>
    {
        private readonly ICqrsBus _cqrsBus;

        public CreateClientPayloadSubscriber(ICqrsBus cqrsBus)
        {
            _cqrsBus = cqrsBus;
        }

        public async Task Consume(ConsumeContext<CreateClientPayload> context)
        {
            await _cqrsBus.ExecuteCommand(new CreateClientCommand(context.Message));
        }
    }
}