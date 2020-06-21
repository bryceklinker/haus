using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Messaging;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Clients.CreateClient
{
    public class CreateIdentityClientCommandHandler : CommandHandler<CreateIdentityClientCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;

        public CreateIdentityClientCommandHandler(IConfiguration configuration, IMessageBus messageBus)
        {
            _configuration = configuration;
            _messageBus = messageBus;
        }

        protected override async Task InnerHandle(CreateIdentityClientCommand command, CancellationToken cancellationToken = default)
        {
            var createCommand = new CreateClientCommand(
                _configuration.IdentityClientId(),
                _configuration.IdentityClientName(),
                new []
                {
                    _configuration.IdentityApiScope()
                },
                new []
                {
                    _configuration.IdentityClientRedirectUri()
                });
            await _messageBus.ExecuteCommand(createCommand, cancellationToken);
        }
    }
}