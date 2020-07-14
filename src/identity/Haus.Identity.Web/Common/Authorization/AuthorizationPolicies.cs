using IdentityModel;
using Microsoft.AspNetCore.Authorization;

namespace Haus.Identity.Web.Common.Authorization
{
    public static class AuthorizationPolicies
    {
        public const string AdminPolicyName = "admin";

        public static readonly AuthorizationPolicy Admin = new AuthorizationPolicyBuilder()
            .RequireClaim(JwtClaimTypes.Role, "admin")
            .Build();

        public static readonly AuthorizationPolicy Default = Admin;
    }
}