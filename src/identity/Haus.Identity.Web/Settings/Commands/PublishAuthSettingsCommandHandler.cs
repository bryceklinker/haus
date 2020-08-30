using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Identity.Models.Settings;
using Haus.ServiceBus.Publish;
using Microsoft.AspNetCore.Http.Features;

namespace Haus.Identity.Web.Settings.Commands
{
    public class PublishAuthSettingsCommand : ICommand
    {
        
    }
    
    public class PublishAuthSettingsCommandHandler : CommandHandler<PublishAuthSettingsCommand>
    {
        private readonly IHausServiceBusPublisher _publisher;

        public PublishAuthSettingsCommandHandler(IHausServiceBusPublisher publisher)
        {
            _publisher = publisher;
        }

        protected override async Task InnerHandle(PublishAuthSettingsCommand command, CancellationToken token = default)
        {
            await _publisher.PublishAsync(new AuthoritySettingsPayload($"https://localhost:{5003}"));
        }
    }
}