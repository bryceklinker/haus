using System;
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
        Assert.True(type.IsNativeTypeScriptType());
        Assert.Equal(typescriptType, type.ToTypeScriptType(new TypeScriptGeneratorContext()));
    }

    [Fact]
    public void WhenTypeIsStaticThenIsSkippableReturnsTrue()
    {
        Assert.True(typeof(TypeExtensions).IsSkippable());
    }

    [Fact]
    public void WhenTypeIsAttributeThenIsSkippableReturnsTrue()
    {
        Assert.True(typeof(SkipGenerationAttribute).IsSkippable());
    }

    [Fact]
    public void WhenTypeIsAnInterfaceThenIsSkippableReturnsTrue()
    {
        Assert.True(typeof(ITypeScriptGeneratorContext).IsSkippable());
    }

    [Fact]
    public void PrimitiveArrayTypeShouldNotRequireATypescriptImport()
    {
        Assert.False(typeof(long[]).RequiresTypescriptImport());
    }
}
