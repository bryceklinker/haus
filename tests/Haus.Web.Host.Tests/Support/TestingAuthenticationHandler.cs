using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Haus.Web.Host.Tests.Support;

public class TestingAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string TestingScheme = "Testing";

    public TestingAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return IsAuthenticatedRequest()
            ? CreateSuccessResult()
            : CreateFailedResult();
    }

    private static Task<AuthenticateResult> CreateFailedResult()
    {
        return Task.FromResult(AuthenticateResult.Fail("You are not authenticated"));
    }

    private static Task<AuthenticateResult> CreateSuccessResult()
    {
        var claims = new[]
        {
            new Claim("sub", "me")
        };
        var identity = new ClaimsIdentity(claims, TestingScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestingScheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private bool IsAuthenticatedRequest()
    {
        return Request.Headers.TryGetValue("Authorization", out var value)
               && value.Any(v => v.Contains(TestingScheme));
    }
}