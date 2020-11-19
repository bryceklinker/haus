using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.Tests.Support
{
    public class TestingAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string TestingScheme = "Testing";
        
        public TestingAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new []
            {
                new Claim("sub", "me"), 
            };
            var identity = new ClaimsIdentity(claims, TestingScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, TestingScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}