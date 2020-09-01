using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Web.IdentityResources.Commands
{
    public class CreateDefaultIdentityResourcesCommand : ICommand
    {
        
    }
    
    public class CreateDefaultIdentityResourcesCommandHandler : CommandHandler<CreateDefaultIdentityResourcesCommand>
    {
        private readonly ConfigurationDbContext _context;

        public CreateDefaultIdentityResourcesCommandHandler(ConfigurationDbContext context)
        {
            _context = context;
        }

        protected override async Task InnerHandle(CreateDefaultIdentityResourcesCommand command, CancellationToken token = default)
        {
            await AddResource(new IdentityServer4.Models.IdentityResources.Profile());
            await AddResource(new IdentityServer4.Models.IdentityResources.OpenId());
            await _context.SaveChangesAsync(token);
        }

        private async Task AddResource(IdentityResource resource)
        {
            if (await DoesIdentityResourceExist(resource))
                return;
            
            _context.Add(resource.ToEntity());
        }

        private async Task<bool> DoesIdentityResourceExist(IdentityResource resource)
        {
            return await _context.IdentityResources.AnyAsync(i => i.Name == resource.Name);
        }
    }
}