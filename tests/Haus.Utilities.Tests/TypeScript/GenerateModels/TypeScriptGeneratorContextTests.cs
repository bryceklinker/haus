using FluentAssertions;
using Haus.Utilities.Tests.TypeScript.GenerateModels.SampleModels;
using Haus.Utilities.TypeScript.GenerateModels;
using Xunit;

namespace Haus.Utilities.Tests.TypeScript.GenerateModels;

public class TypeScriptGeneratorContextTests
{
    [Fact]
    public void WhenBarrelIsRetrievedThenReturnsBarrelTypeScriptModel()
    {
        var context = new TypeScriptGeneratorContext();
        context.Add(new TypeScriptModel(typeof(SimpleModel), "simple-model.ts", ""));
        context.Add(new TypeScriptModel(typeof(object), "string.ts", ""));
        context.Add(new TypeScriptModel(typeof(string), "object.ts", ""));

        var barrel = context.GetBarrel();

        barrel.FileName.Should().Be("index.ts");
        barrel.Contents.Should()
            .Contain("export * from './simple-model'")
            .And.Contain("export * from './string'")
            .And.Contain("export * from './object'");
    }
}