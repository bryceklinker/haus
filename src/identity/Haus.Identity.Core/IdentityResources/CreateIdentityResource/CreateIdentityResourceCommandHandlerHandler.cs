using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Common.Messaging.Commands;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.IdentityResources.CreateIdentityResource
{
    public class CreateIdentityResourceCommandHandlerHandler : CreateCommandHandler<CreateIdentityResourceCommand, CreateIdentityResourceResult>
    {
        private readonly ConfigurationDbContext _context;

        public CreateIdentityResourceCommandHandlerHandler(ConfigurationDbContext context)
        {
            _context = context;
        }

        protected override async Task<CreateIdentityResourceResult> Create(CreateIdentityResourceCommand command, CancellationToken cancellationToken)
        {
            var resource = new IdentityResource(command.Name, command.DisplayName, command.ClaimTypes);
            _context.Add(resource.ToEntity());
            await _context.SaveChangesAsync(cancellationToken);
            return CreateIdentityResourceResult.Success();
        }

        protected override async Task<bool> DoesExist(CreateIdentityResourceCommand command)
        {
            return await _context.IdentityResources.AnyAsync(r => r.Name == command.Name);
        }

        protected override CreateIdentityResourceResult CreateDuplicateFailureResult(CreateIdentityResourceCommand command)
        {
            return CreateIdentityResourceResult.Failed($"Identity resource '{command.Name}' already exists");
        }
    }
}