using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Common.Messaging;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.ApiResources.CreateApiResource
{
    public class CreateIdentityApiResourceCommandHandler : CommandHandler<CreateApiResourceCommand>
    {
        private readonly ConfigurationDbContext _context;
        private readonly IConfiguration _config;

        public CreateIdentityApiResourceCommandHandler(ConfigurationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        protected override async Task InnerHandle(CreateApiResourceCommand command, CancellationToken cancellationToken = default)
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