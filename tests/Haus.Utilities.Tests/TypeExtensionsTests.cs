using System;
using FluentAssertions;
using Xunit;

namespace Haus.Utilities.Tests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void WhenDateTimeIsConvertedToTypeScriptTypeThenReturnsString()
        {
            typeof(DateTime).ToTypeScriptType().Should().Be("string");
        }

        [Fact]
        public void WhenDateTimeThenIsNativeTypeScriptType()
        {
            typeof(DateTime).IsNativeTypeScriptType().Should().BeTrue();
        }
    }
}