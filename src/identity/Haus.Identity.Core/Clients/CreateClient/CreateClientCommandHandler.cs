using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Messaging;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.Clients.CreateClient
{
    public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, CreateClientResult>
    {
        private readonly ConfigurationDbContext _context;
        private readonly IClientFactory _clientFactory;

        public CreateClientCommandHandler(ConfigurationDbContext context, IClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
        }

        public async Task<CreateClientResult> Handle(CreateClientCommand request, CancellationToken cancellationToken = default)
        {
            if (await DoesClientExist(request.Id))
                return CreateClientResult.Failed($"Client with id '{request.Id}' already exists");

            return await AddClientAsync(request, cancellationToken);
        }

        private async Task<CreateClientResult> AddClientAsync(CreateClientCommand request, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateFromCommand(request);
            _context.Add(client.ToEntity());
            await _context.SaveChangesAsync(cancellationToken);
            return CreateClientResult.Success(client.ClientId);
        }

        private async Task<bool> DoesClientExist(string clientId)
        {
            return await _context.Clients.AnyAsync(c => c.ClientId == clientId);
        }
    }
}