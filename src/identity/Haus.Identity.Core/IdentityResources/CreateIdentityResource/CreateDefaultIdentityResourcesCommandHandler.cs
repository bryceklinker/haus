using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.IdentityResources.CreateIdentityResource
{
    public class CreateDefaultIdentityResourcesCommandHandler : CommandHandler<CreateDefaultIdentityResourceCommand>
    {
        private readonly IMessageBus _messageBus;

        public CreateDefaultIdentityResourcesCommandHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        protected override async Task InnerHandle(CreateDefaultIdentityResourceCommand request, CancellationToken cancellationToken = default)
        {
            foreach (var resource in request.Resources) 
                await CreateResource(resource, cancellationToken);
        }

        private async Task CreateResource(IdentityResource resource, CancellationToken cancellationToken)
        {
            var command = new CreateIdentityResourceCommand(
                resource.Name, 
                resource.DisplayName, 
                resource.UserClaims.ToArray());

            await _messageBus.ExecuteCommand(command, cancellationToken);
        }
    }
}