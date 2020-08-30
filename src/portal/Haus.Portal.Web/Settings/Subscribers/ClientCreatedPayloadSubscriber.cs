using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Identity.Models.Clients;
using Haus.Portal.Web.Settings.Commands;
using Haus.ServiceBus.Subscribe;
using MassTransit;

namespace Haus.Portal.Web.Settings.Consumers
{
    public class ClientCreatedPayloadSubscriber : IHausServiceBusSubscriber<ClientCreatedPayload>
    {
        private readonly ICqrsBus _cqrsBus;

        public ClientCreatedPayloadSubscriber(ICqrsBus cqrsBus)
        {
            _cqrsBus = cqrsBus;
        }

        public async Task Consume(ConsumeContext<ClientCreatedPayload> context)
        {
            await _cqrsBus.ExecuteCommand(new UpdateClientSettingsCommand(context.Message));
        }
    }
}