using System.Linq;
using IdentityModel;
using IdentityServer4.Models;

namespace Haus.Identity.Core.Clients.CreateClient
{
    public interface IClientFactory
    {
        Client CreateFromCommand(CreateClientCommand command);
    }

    public class ClientFactory : IClientFactory
    {
        private static readonly string[] DefaultScopes = 
        {
            OidcConstants.StandardScopes.OpenId,
            OidcConstants.StandardScopes.Email,
            OidcConstants.StandardScopes.Profile,
            OidcConstants.StandardScopes.OfflineAccess
        };

        private static readonly string[] DefaultGrantTypes = 
        {
            OidcConstants.GrantTypes.AuthorizationCode,
            OidcConstants.GrantTypes.ClientCredentials
        };
        
        public Client CreateFromCommand(CreateClientCommand command)
        {
            return new Client
            {
                ClientId = command.Id,
                ClientName = command.Name,
                RequireClientSecret = false,
                RequireConsent = false,
                AllowAccessTokensViaBrowser = true,
                AllowedCorsOrigins = command.AllowedCorsOrigins.ToList(),
                RedirectUris = command.RedirectUris.ToList(),
                AllowedGrantTypes = DefaultGrantTypes.ToList(),
                AllowedScopes = DefaultScopes
                    .Concat(command.Scopes)
                    .ToList()
            };
        }
    }
}