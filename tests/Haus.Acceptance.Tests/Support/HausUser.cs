using System;

namespace Haus.Acceptance.Tests.Support;

public record HausUser(string Email, string Password)
{
    public static readonly HausUser Default = new(
        Environment.GetEnvironmentVariable("AUTH_USERNAME") ?? "",
        Environment.GetEnvironmentVariable("AUTH_PASSWORD") ?? ""
    );
}
