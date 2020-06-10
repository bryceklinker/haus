using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Clients.Models;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Clients
{
    public class SeedIdentityClientRequest : IRequest
    {
        public string RedirectUrl { get; }

        public SeedIdentityClientRequest(string redirectUrl)
        {
            RedirectUrl = redirectUrl;
        }
    }
    
    public class IdentityClientSeeder : AsyncRequestHandler<SeedIdentityClientRequest>
    {
        private readonly IConfiguration _configuration;
        private readonly ConfigurationDbContext _context;

        public IdentityClientSeeder(ConfigurationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        protected override async Task Handle(SeedIdentityClientRequest request, CancellationToken cancellationToken)
        {
            var clientIds = await _context.Set<IdentityServer4.EntityFramework.Entities.Client>()
                .Select(c => c.ClientId)
                .ToArrayAsync(cancellationToken);

            if (ShouldCreateIdentityClient(_configuration.IdentityClientId(), clientIds))
                await AddIdentityClientAsync(request.RedirectUrl);
        }

        private async Task AddIdentityClientAsync(string redirectUri)
        {
            var client = CreateIdentityClient(new CreateClientRequest
            {
                Id = _configuration.IdentityClientId(),
                Name = _configuration.IdentityClientName(),
                Scopes = new[] {_configuration.IdentityApiScope()},
                RedirectUris = new [] {redirectUri}
            });
            _context.Add(client.ToEntity());
            await _context.SaveChangesAsync();
        }

        public static bool ShouldCreateIdentityClient(string identityClientId, string[] clientIds)
        {
            return !clientIds.ContainsIgnoreCase(identityClientId);
        }

        public static Client CreateIdentityClient(CreateClientRequest request)
        {
            var defaultScopes = new[]
            {
                OidcConstants.StandardScopes.OpenId,
                OidcConstants.StandardScopes.Email,
                OidcConstants.StandardScopes.Profile
            };
            return new Client
            {
                ClientId = request.Id,
                ClientName = request.Name,
                RequireClientSecret = false,
                RequireConsent = false,
                RedirectUris = request.RedirectUris,
                AllowedGrantTypes = new List<string>
                {
                    OidcConstants.GrantTypes.Password,
                    OidcConstants.GrantTypes.ClientCredentials
                },
                AllowedScopes = request.Scopes
                    .Concat(defaultScopes)
                    .ToArray()
            };
        }
    }
}