using Haus.Core.Models;
using Haus.Utilities.Tests.TypeScript.GenerateModels.SampleModels;
using Haus.Utilities.TypeScript.GenerateModels;
using Xunit;

namespace Haus.Utilities.Tests.TypeScript.GenerateModels;

public class TypeScriptModelGeneratorTests
{
    private readonly TypeScriptGeneratorContext _context = new();
    private readonly TypeScriptModelGenerator _generator = new();

    [Fact]
    public void WhenTypeContainsSimplePropertiesThenReturnsSingleTypeScriptModel()
    {
        _generator.Generate(typeof(SimpleModel), _context);

        var models = _context.GetAll();
        Assert.Single(models);
        Assert.Contains(models, m => m.ModelType == typeof(SimpleModel));
        Assert.Contains(models, m => m.FileName == "simple-model.ts");
    }

    [Fact]
    public void WhenTypeScriptModelGeneratedThenTypescriptInterfaceDefinedForType()
    {
        _generator.Generate(typeof(SimpleModel), _context);

        var model = _context.GetModelForType(typeof(SimpleModel));
        Assert.Contains("export interface SimpleModel", model?.Contents);
    }

    [Fact]
    public void WhenTypescriptModelGeneratedThenEachPropertyIsInInterface()
    {
        _generator.Generate(typeof(SimpleModel), _context);

        var model = _context.GetModelForType(typeof(SimpleModel));
        Assert.Contains("id: number;", model?.Contents);
        Assert.Contains("name: string;", model?.Contents);
        Assert.Contains("value: number", model?.Contents);
    }

    [Fact]
    public void WhenTypeReferencesAnotherModelThenReturnsTwoModels()
    {
        _generator.Generate(typeof(SlightlyComplexModel), _context);

        var models = _context.GetAll();
        Assert.Equal(2, models.Length);
    }

    [Fact]
    public void WhenTypeReferencesAnotherModelThenReturnsContentsWithImports()
    {
        _generator.Generate(typeof(SlightlyComplexModel), _context);

        var model = _context.GetModelForType(typeof(SlightlyComplexModel));
        Assert.Contains("import {SimpleModel} from './simple-model';", model?.Contents);
        Assert.Contains("simple: SimpleModel;", model?.Contents);
    }

    [Fact]
    public void WhenTypeContainsGenericThenFileNameIsTypeNameExcludingGenericParameters()
    {
        _generator.Generate(typeof(GenericType<>), _context);

        var model = _context.GetModelForType(typeof(GenericType<>));
        Assert.Equal("generic-type.ts", model?.FileName);
    }

    [Fact]
    public void WhenTypeContainsGenericThenInterfaceIncludesGenericType()
    {
        _generator.Generate(typeof(GenericType<>), _context);

        var model = _context.GetModelForType(typeof(GenericType<>));
        Assert.Contains("export interface GenericType<T>", model?.Contents);
        Assert.Contains("item: T;", model?.Contents);
    }

    [Fact]
    public void WhenTypeContainsMultipleGenericArgumentsThenInterfaceContainsMultipleGenericArguments()
    {
        _generator.Generate(typeof(GenericType<,,>), _context);

        var model = _context.GetModelForType(typeof(GenericType<,,>));
        Assert.Contains("export interface GenericType<T, TR, TU>", model?.Contents);
        Assert.Contains("first: T", model?.Contents);
        Assert.Contains("second: TR", model?.Contents);
        Assert.Contains("third: TU", model?.Contents);
    }

    [Fact]
    public void WhenTypeContainsGenericArrayThenInterfaceContainsGenericArray()
    {
        _generator.Generate(typeof(ResultSet<>), _context);

        var model = _context.GetModelForType(typeof(ResultSet<>));
        Assert.Contains("export interface ResultSet<T>", model?.Contents);
        Assert.Contains("items: Array<T>;", model?.Contents);
    }

    [Fact]
    public void WhenTypeIsStaticThenNoModelsShouldBeGenerated()
    {
        _generator.Generate(typeof(TypeExtensions), _context);

        Assert.Empty(_context.GetAll());
    }

    [Fact]
    public void WhenTypeIsAnInterfaceThenNoModelsShouldBeGenerated()
    {
        _generator.Generate(typeof(ITypeScriptModelGenerator), _context);

        Assert.Empty(_context.GetAll());
    }

    [Fact]
    public void WhenTypeIsEnumThenTypescriptEnumIsGenerated()
    {
        _generator.Generate(typeof(SimpleEnum), _context);

        var model = _context.GetModelForType(typeof(SimpleEnum));
        Assert.Equal("simple-enum.ts", model?.FileName);
        Assert.Contains("export enum SimpleEnum", model?.Contents);
        Assert.Contains("Hello = 'Hello',", model?.Contents);
    }

    [Fact]
    public void WhenTypeContainsNullablePropertyThenTypescriptPropertyIsOptional()
    {
        _generator.Generate(typeof(ModelWithNullable), _context);

        Assert.Single(_context.GetAll());
        var model = _context.GetModelForType(typeof(ModelWithNullable));
        Assert.Contains("id?: number", model?.Contents);
    }

    [Fact]
    public void WhenTypeContainsArrayOfModelsThenTypeScriptContainsPropertyWithAnArrayOfModels()
    {
        _generator.Generate(typeof(ModelWithArrayOfModels), _context);

        Assert.Equal(2, _context.GetAll().Length);
        var model = _context.GetModelForType(typeof(ModelWithArrayOfModels));
        Assert.Contains("import {SimpleModel} from './simple-model'", model?.Contents);
        Assert.Contains("models: Array<SimpleModel>;", model?.Contents);
    }

    [Fact]
    public void WhenTypeDerivesFromAnotherTypeThenTypeScriptInterfaceExtendsTheBaseType()
    {
        _generator.Generate(typeof(DerivedFromSimpleModel), _context);

        Assert.Equal(2, _context.GetAll().Length);
        var model = _context.GetModelForType(typeof(DerivedFromSimpleModel));
        Assert.Contains("export interface DerivedFromSimpleModel extends SimpleModel", model?.Contents);
        Assert.Contains("import {SimpleModel} from './simple-model';", model?.Contents);
        Assert.DoesNotContain("id: number;", model?.Contents);
        Assert.Contains("stuff: string;", model?.Contents);
    }

    [Fact]
    public void WhenTypeIsMarkedToBeSkippedThenTypeIsNotGenerated()
    {
        _generator.Generate(typeof(Skippable), _context);

        Assert.Empty(_context.GetAll());
    }

    [Fact]
    public void WhenTypeIsAttributeThenSkipped()
    {
        _generator.Generate(typeof(SkipGenerationAttribute), _context);

        Assert.Empty(_context.GetAll());
    }

    [Fact]
    public void WhenTypeContainsPropertyWithPrimitiveArrayThenArrayIsNotGenerated()
    {
        _generator.Generate(typeof(ModelWithPrimitiveArray), _context);

        Assert.Single(_context.GetAll());
    }

    [Fact]
    public void WhenTypeContainsOptionalGenerationAttributesThenOptionalPropertiesAreGeneratedAsOptional()
    {
        _generator.Generate(typeof(ModelWithOptionalProperty), _context);

        var model = _context.GetModelForType(typeof(ModelWithOptionalProperty));
        Assert.Contains("id?: number;", model?.Contents);
        Assert.Contains("value?: number", model?.Contents);
    }
}
