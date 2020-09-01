using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Identity.Models.ApiResources;
using Haus.ServiceBus.Publish;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace Haus.Identity.Web.ApiResources.Commands
{
    public class CreateApiResourceCommandHandler : CommandHandler<CreateApiResourceCommand>
    {
        private readonly ConfigurationDbContext _context;
        private readonly IHausServiceBusPublisher _hausServiceBusPublisher;

        public CreateApiResourceCommandHandler(ConfigurationDbContext context,
            IHausServiceBusPublisher hausServiceBusPublisher)
        {
            _context = context;
            _hausServiceBusPublisher = hausServiceBusPublisher;
        }

        protected override async Task InnerHandle(CreateApiResourceCommand command, CancellationToken token = default)
        {
            var resource = command.ToApiResource();
            _context.Add(resource.ToEntity());

            var scopes = command.ToApiScopes();
            foreach (var scope in scopes) _context.Add(scope.ToEntity());
            
            await _context.SaveChangesAsync(token);
            await _hausServiceBusPublisher.PublishAsync(new ApiResourceCreatedPayload(command.Identifier));
        }
    }
}