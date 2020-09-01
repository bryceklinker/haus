using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Identity.Models.Settings;
using Haus.ServiceBus.Publish;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Web.Settings.Commands
{
    public class PublishAuthSettingsCommand : ICommand
    {
        
    }
    
    public class PublishAuthSettingsCommandHandler : CommandHandler<PublishAuthSettingsCommand>
    {
        private readonly IHausServiceBusPublisher _publisher;
        private readonly IConfiguration _config;

        public PublishAuthSettingsCommandHandler(IHausServiceBusPublisher publisher, IConfiguration config)
        {
            _publisher = publisher;
            _config = config;
        }

        protected override async Task InnerHandle(PublishAuthSettingsCommand command, CancellationToken token = default)
        {
            await _publisher.PublishAsync(new AuthoritySettingsPayload(_config.GetValue<string>("AuthorityUrl")));
        }
    }
}