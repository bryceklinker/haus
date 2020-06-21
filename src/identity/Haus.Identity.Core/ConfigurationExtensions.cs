using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core
{
    public static class ConfigurationExtensions
    {
        public static string IdentityClientId(this IConfiguration configuration)
        {
            return configuration["IDENTITY_CLIENT_ID"] ?? ConfigurationDefaults.IdentityClientId;
        }

        public static string IdentityClientName(this IConfiguration configuration)
        {
            return configuration["IDENTITY_CLIENT_NAME"] ?? ConfigurationDefaults.IdentityClientName;
        }

        public static string IdentityClientRedirectUri(this IConfiguration configuration)
        {
            return configuration["IdentityClient:RedirectUri"];
        }

        public static string IdentityApiScope(this IConfiguration configuration)
        {
            return configuration["IDENTITY_API_SCOPE"] ?? ConfigurationDefaults.IdentityApiScope;
        }

        public static string IdentityApiName(this IConfiguration configuration)
        {
            return configuration["IDENTITY_API_NAME"] ?? ConfigurationDefaults.IdentityApiName;
        }

        public static string AdminUsername(this IConfiguration configuration)
        {
            return configuration["ADMIN_USERNAME"] ?? ConfigurationDefaults.AdminUsername;
        }

        public static string AdminPassword(this IConfiguration configuration)
        {
            return configuration["ADMIN_PASSWORD"] ?? ConfigurationDefaults.AdminPassword;
        }
    }
}