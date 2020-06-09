using Haus.Identity.Core.Accounts.Models;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core
{
    public static class ConfigurationExtensions
    {
        public static string IdentityClientId(this IConfiguration configuration)
        {
            return configuration["IDENTITY_CLIENT_ID"] ?? "haus.identity";
        }

        public static string IdentityClientName(this IConfiguration configuration)
        {
            return configuration["IDENTITY_CLIENT_NAME"] ?? "HAUS Identity";
        }

        public static string IdentityApiScope(this IConfiguration configuration)
        {
            return configuration["IDENTITY_API_SCOPE"] ?? "haus.identity";
        }
    }
}