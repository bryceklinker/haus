using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Common.Messaging.Commands;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.ApiResources.CreateApiResource
{
    public class CreateIdentityApiResourceCommandHandler : CommandHandler<CreateIdentityApiResourceCommand>
    {
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _config;

        public CreateIdentityApiResourceCommandHandler(ConfigurationDbContext context, IConfiguration config, IMessageBus messageBus)
        {
            _config = config;
            _messageBus = messageBus;
        }

        protected override async Task InnerHandle(CreateIdentityApiResourceCommand command, CancellationToken cancellationToken = default)
        {
            var createCommand = new CreateApiResourceCommand(_config.IdentityApiScope(), _config.IdentityApiName());
            await _messageBus.ExecuteCommand(createCommand, cancellationToken);
        }
    }
}