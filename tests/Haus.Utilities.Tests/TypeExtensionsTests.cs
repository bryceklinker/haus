using System;
using FluentAssertions;
using Haus.Core.Models;
using Haus.Utilities.TypeScript.GenerateModels;
using Xunit;

namespace Haus.Utilities.Tests;

public class TypeExtensionsTests
{
    [Theory]
    [InlineData(typeof(DateTime), "string")]
    [InlineData(typeof(DateTimeOffset), "string")]
    [InlineData(typeof(Guid), "string")]
    [InlineData(typeof(long), "number")]
    [InlineData(typeof(int), "number")]
    [InlineData(typeof(double), "number")]
    [InlineData(typeof(byte), "number")]
    [InlineData(typeof(object), "object")]
    [InlineData(typeof(bool), "boolean")]
    public void WhenTypeIsANativeTypescriptTypeThenReturnsTypescriptTypeAsString(Type type, string typescriptType)
    {
        type.IsNativeTypeScriptType().Should().BeTrue();
        type.ToTypeScriptType(new TypeScriptGeneratorContext()).Should().Be(typescriptType);
    }

    [Fact]
    public void WhenTypeIsStaticThenIsSkippableReturnsTrue()
    {
        typeof(TypeExtensions).IsSkippable().Should().BeTrue();
    }

    [Fact]
    public void WhenTypeIsAttributeThenIsSkippableReturnsTrue()
    {
        typeof(SkipGenerationAttribute).IsSkippable().Should().BeTrue();
    }

    [Fact]
    public void WhenTypeIsAnInterfaceThenIsSkippableReturnsTrue()
    {
        typeof(ITypeScriptGeneratorContext).IsSkippable().Should().BeTrue();
    }

    [Fact]
    public void PrimitiveArrayTypeShouldNotRequireATypescriptImport()
    {
        typeof(long[]).RequiresTypescriptImport().Should().BeFalse();
    }
}