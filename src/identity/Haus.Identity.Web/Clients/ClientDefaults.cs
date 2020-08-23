using IdentityModel;

namespace Haus.Identity.Web.Clients
{
    public static class ClientDefaults
    {
        public static readonly string[] DefaultGrantTypes =
        {
            OidcConstants.GrantTypes.AuthorizationCode,
            OidcConstants.GrantTypes.ClientCredentials,
        };

        public static readonly string[] DefaultScopes =
        {
            OidcConstants.StandardScopes.Profile,
            OidcConstants.StandardScopes.OpenId,
        };

        public static readonly string[] DefaultAllowedOrigins =
        {
            "*"
        };
    }
}