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

        Assert.Equal("index.ts", barrel.FileName);
        Assert.Contains("export * from './simple-model'", barrel.Contents);
        Assert.Contains("export * from './string'", barrel.Contents);
        Assert.Contains("export * from './object'", barrel.Contents);
    }
}
