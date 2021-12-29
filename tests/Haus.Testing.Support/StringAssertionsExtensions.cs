using System;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace Haus.Testing.Support;

public static class StringAssertionsExtensions
{
    public static AndConstraint<StringAssertions> BeAGuid(this StringAssertions assertions)
    {
        Guid.TryParse(assertions.Subject, out _).Should().BeTrue($"{assertions.Subject} not parsable to Guid");
        return new AndConstraint<StringAssertions>(assertions);
    }
}