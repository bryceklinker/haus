using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Messaging;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Identity.Core.IdentityResources
{
    public class SeedIdentityResourcesRequest : ICommand
    {
        public IdentityResource[] Resources { get; }

        public SeedIdentityResourcesRequest(params IdentityResource[] resources)
        {
            Resources = resources;
        }
    }

    public class IdentityResourceSeeder : CommandHandler<SeedIdentityResourcesRequest>
    {
        private readonly ConfigurationDbContext _context;

        public IdentityResourceSeeder(ConfigurationDbContext context)
        {
            _context = context;
        }

        protected override async Task InnerHandle(SeedIdentityResourcesRequest request, CancellationToken cancellationToken)
        {
            foreach (var resource in request.Resources) 
                await AddResource(resource, cancellationToken);
        }

        public static bool ShouldCreateIdentityResource(string name, string[] existingResources)
        {
            return !existingResources.ContainsIgnoreCase(name);
        }

        private async Task AddResource(IdentityResource resource, CancellationToken cancellationToken)
        {
            var resourceNames = await _context.IdentityResources
                .Select(r => r.Name)
                .ToArrayAsync(cancellationToken);

            if (ShouldCreateIdentityResource(resource.Name, resourceNames))
            {
                _context.Add(resource.ToEntity());
                await _context.SaveChangesAsync();
            }
        }
    }
}