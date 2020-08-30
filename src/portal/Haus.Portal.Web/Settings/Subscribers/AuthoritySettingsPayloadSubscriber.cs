using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Identity.Models.Settings;
using Haus.Portal.Web.Settings.Commands;
using Haus.ServiceBus.Subscribe;
using MassTransit;

namespace Haus.Portal.Web.Settings.Consumers
{
    public class AuthoritySettingsPayloadSubscriber : IHausServiceBusSubscriber<AuthoritySettingsPayload>
    {
        private readonly ICqrsBus _cqrsBus;

        public AuthoritySettingsPayloadSubscriber(ICqrsBus cqrsBus)
        {
            _cqrsBus = cqrsBus;
        }

        public async Task Consume(ConsumeContext<AuthoritySettingsPayload> context)
        {
            await _cqrsBus.ExecuteCommand(new UpdateAuthSettingsCommand(context.Message));
        }
    }
}