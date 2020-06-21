using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Messaging.Commands;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.Clients.CreateClient
{
    public class CreateClientCommandHandler : CreateCommandHandler<CreateClientCommand, CreateClientResult>
    {
        private readonly ConfigurationDbContext _context;
        private readonly IClientFactory _clientFactory;

        public CreateClientCommandHandler(ConfigurationDbContext context, IClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
        }

        protected override async Task<CreateClientResult> Create(CreateClientCommand command, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateFromCommand(command);
            _context.Add(client.ToEntity());
            await _context.SaveChangesAsync(cancellationToken);
            return CreateClientResult.Success(client.ClientId);
        }

        protected override async Task<bool> DoesExist(CreateClientCommand command)
        {
            return await _context.Clients.AnyAsync(c => c.ClientId == command.Id);
        }

        protected override CreateClientResult CreateDuplicateFailureResult(CreateClientCommand command)
        {
            return CreateClientResult.Failed($"Client with id '{command.Id}' already exists");
        }
    }
}