using System.Linq;
using System.Text;
using FluentAssertions;
using FluentAssertions.Collections;

namespace Haus.Testing.Support;

public static class ByteCollectionAssertionExtensions
{
    public static AndConstraint<GenericCollectionAssertions<byte>> BeEncodedString(
        this GenericCollectionAssertions<byte> assertions,
        string value,
        Encoding encoding = null)
    {
        var decoded = (encoding ?? Encoding.UTF8).GetString(assertions.Subject.ToArray());
        decoded.Should().Be(value);
        return new AndConstraint<GenericCollectionAssertions<byte>>(assertions);
    }
}