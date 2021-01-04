using System;
using FluentAssertions;
using Xunit;

namespace Haus.Utilities.Tests
{
    public class TypeExtensionsTests
    {
        [Theory]
        [InlineData(typeof(DateTime), "string")]
        [InlineData(typeof(Guid), "string")]
        [InlineData(typeof(long), "number")]
        [InlineData(typeof(int), "number")]
        [InlineData(typeof(double), "number")]
        [InlineData(typeof(byte), "number")]
        [InlineData(typeof(object), "object")]
        public void WhenTypeIsANativeTypescriptTypeThenReturnsTypescriptTypeAsString(Type type, string typescriptType)
        {
            type.IsNativeTypeScriptType().Should().BeTrue();
            type.ToTypeScriptType().Should().Be(typescriptType);
        }
    }
}