using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Identity.Models.ApiResources;
using Haus.Identity.Web.ApiResources.Commands;
using Haus.ServiceBus.Subscribe;
using MassTransit;

namespace Haus.Identity.Web.ApiResources.Subscribers
{
    public class CreateApiResourceSubscriber : IHausServiceBusSubscriber<CreateApiResourcePayload>
    {
        private readonly ICqrsBus _cqrsBus;

        public CreateApiResourceSubscriber(ICqrsBus cqrsBus)
        {
            _cqrsBus = cqrsBus;
        }

        public async Task Consume(ConsumeContext<CreateApiResourcePayload> context)
        {
            var message = context.Message;
            await _cqrsBus.ExecuteCommand(new CreateApiResourceCommand(message.Identifier, message.Scopes, message.DisplayName));
        }
    }
}