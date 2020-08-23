using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace Haus.Identity.Web.Clients.Commands
{
    public class CreateClientResult
    {
        public string Secret { get; }
        public string Id { get; }

        public CreateClientResult(string id, string secret)
        {
            Secret = secret;
            Id = id;
        }
    }

    public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, CreateClientResult>
    {
        private readonly ConfigurationDbContext _context;

        public CreateClientCommandHandler(ConfigurationDbContext context)
        {
            _context = context;
        }

        public async Task<CreateClientResult> Handle(CreateClientCommand request,
            CancellationToken cancellationToken = default)
        {
            var client = request.ToClient();
            _context.Add(client.ToEntity());
            await _context.SaveChangesAsync(cancellationToken);
            return request.ToResult();
        }
    }
}