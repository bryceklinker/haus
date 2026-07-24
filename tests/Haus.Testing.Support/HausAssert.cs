using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haus.Testing.Support;

public static class HausAssert
{
    public static void IsGuid(string? value)
    {
        if (!Guid.TryParse(value, out _))
            throw new InvalidOperationException($"{value} not parsable to Guid");
    }

    public static void IsEncodedString(string expected, IEnumerable<byte> actual, Encoding? encoding = null)
    {
        var decoded = (encoding ?? Encoding.UTF8).GetString(actual.ToArray());
        if (decoded != expected)
            throw new InvalidOperationException($"Expected encoded string '{expected}' but got '{decoded}'");
    }
}
