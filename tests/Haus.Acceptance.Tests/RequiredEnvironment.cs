using System;
using System.Collections.Generic;
using System.Linq;

namespace Haus.Acceptance.Tests;

// Fails the whole acceptance suite immediately (before any browser opens) when the Auth0 login
// credentials aren't present in the environment. Without this guard a missing AUTH_USERNAME /
// AUTH_PASSWORD silently logs in with empty strings, Auth0 rejects it, and every test dies with a
// confusing 30s navigation timeout instead of telling you what's actually wrong. These are injected
// from secrets on CI; locally you have to export them yourself (see the env var block in readme.md).
[SetUpFixture]
public class RequiredEnvironment
{
    private static readonly string[] RequiredVariables = ["AUTH_USERNAME", "AUTH_PASSWORD"];

    [OneTimeSetUp]
    public void EnsureAuthCredentialsArePresent()
    {
        var missing = RequiredVariables
            .Where(name => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name)))
            .ToArray();

        if (missing.Length == 0)
            return;

        throw new InvalidOperationException(BuildMessage(missing));
    }

    private static string BuildMessage(IReadOnlyCollection<string> missing)
    {
        return $"Acceptance tests can't run: required Auth0 credential environment variable(s) not set: "
            + $"{string.Join(", ", missing)}. These are injected from secrets on CI; to run locally, export "
            + "them first (see the env var block in readme.md), e.g. `export AUTH_USERNAME=... AUTH_PASSWORD=...`.";
    }
}
