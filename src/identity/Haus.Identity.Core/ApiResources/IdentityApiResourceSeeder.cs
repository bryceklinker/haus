using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.ApiResources
{
    public class SeedIdentityApiResourceRequest : IRequest
    {
    }

    public class IdentityApiResourceSeeder : AsyncRequestHandler<SeedIdentityApiResourceRequest>
    {
        private readonly ConfigurationDbContext _context;
        private readonly IConfiguration _config;

        public IdentityApiResourceSeeder(ConfigurationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        protected override async Task Handle(SeedIdentityApiResourceRequest request, CancellationToken cancellationToken)
        {
            var apiResourceScopes = await _context.ApiResources
                .Select(a => a.Name)
                .ToArrayAsync(cancellationToken);

            if (ShouldCreateIdentityApiResource(_config.IdentityApiScope(), apiResourceScopes))
            {
                await AddIdentityApiResource();
            }
        }

        private async Task AddIdentityApiResource()
        {
            var resource = CreateIdentityApiResource(_config.IdentityApiName(), _config.IdentityApiScope());
            _context.Add(resource.ToEntity());
            await _context.SaveChangesAsync();
        }

        public static bool ShouldCreateIdentityApiResource(string identityApiScope, string[] existingScopes)
        {
            return !existingScopes.ContainsIgnoreCase(identityApiScope);
        }

        public static ApiResource CreateIdentityApiResource(string name, string scope)
        {
            return new ApiResource(scope, name);
        }
    }
}