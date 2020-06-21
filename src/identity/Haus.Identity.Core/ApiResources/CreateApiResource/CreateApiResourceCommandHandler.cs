using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Common.Messaging.Commands;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.ApiResources.CreateApiResource
{
    public class CreateApiResourceCommandHandler : CreateCommandHandler<CreateApiResourceCommand, CreateApiResourceResult>
    {
        private readonly ConfigurationDbContext _context;

        public CreateApiResourceCommandHandler(ConfigurationDbContext context)
        {
            _context = context;
        }

        protected override async Task<CreateApiResourceResult> Create(CreateApiResourceCommand command, CancellationToken cancellationToken)
        {
            var resource = new ApiResource(command.Name, command.DisplayName);
            _context.Add(resource.ToEntity());
            await _context.SaveChangesAsync(cancellationToken);
            return CreateApiResourceResult.Success();
        }

        protected override async Task<bool> DoesExist(CreateApiResourceCommand command)
        {
            return await _context.ApiResources.AnyAsync(r => r.Name == command.Name);
        }

        protected override CreateApiResourceResult CreateDuplicateFailureResult(CreateApiResourceCommand command)
        {
            return CreateApiResourceResult.Failed($"Api resource '{command.Name}` already exists");
        }
    }
}