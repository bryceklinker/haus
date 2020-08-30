using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Identity.Models.Settings;
using Haus.Identity.Web.Settings.Commands;
using Haus.ServiceBus.Subscribe;
using MassTransit;

namespace Haus.Identity.Web.Settings.Subscribers
{
    public class RequestAuthoritySettingsPayloadSubscriber : IHausServiceBusSubscriber<RequestAuthoritySettingsPayload>
    {
        private readonly ICqrsBus _cqrsBus;

        public RequestAuthoritySettingsPayloadSubscriber(ICqrsBus cqrsBus)
        {
            _cqrsBus = cqrsBus;
        }

        public async Task Consume(ConsumeContext<RequestAuthoritySettingsPayload> context)
        {
            await _cqrsBus.ExecuteCommand(new PublishAuthSettingsCommand());
        }
    }
}