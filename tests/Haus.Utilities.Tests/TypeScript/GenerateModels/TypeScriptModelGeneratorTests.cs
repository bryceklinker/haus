using FluentAssertions;
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
        models
            .Should()
            .HaveCount(1)
            .And.Contain(m => m.ModelType == typeof(SimpleModel))
            .And.Contain(m => m.FileName == "simple-model.ts");
    }

    [Fact]
    public void WhenTypeScriptModelGeneratedThenTypescriptInterfaceDefinedForType()
    {
        _generator.Generate(typeof(SimpleModel), _context);

        var model = _context.GetModelForType(typeof(SimpleModel));
        model?.Contents.Should().Contain("export interface SimpleModel");
    }

    [Fact]
    public void WhenTypescriptModelGeneratedThenEachPropertyIsInInterface()
    {
        _generator.Generate(typeof(SimpleModel), _context);

        var model = _context.GetModelForType(typeof(SimpleModel));
        model?.Contents.Should().Contain("id: number;").And.Contain("name: string;").And.Contain("value: number");
    }

    [Fact]
    public void WhenTypeReferencesAnotherModelThenReturnsTwoModels()
    {
        _generator.Generate(typeof(SlightlyComplexModel), _context);

        var models = _context.GetAll();
        models.Should().HaveCount(2);
    }

    [Fact]
    public void WhenTypeReferencesAnotherModelThenReturnsContentsWithImports()
    {
        _generator.Generate(typeof(SlightlyComplexModel), _context);

        var model = _context.GetModelForType(typeof(SlightlyComplexModel));
        model
            ?.Contents.Should()
            .Contain("import {SimpleModel} from './simple-model';")
            .And.Contain("simple: SimpleModel;");
    }

    [Fact]
    public void WhenTypeContainsGenericThenFileNameIsTypeNameExcludingGenericParameters()
    {
        _generator.Generate(typeof(GenericType<>), _context);

        var model = _context.GetModelForType(typeof(GenericType<>));
        model?.FileName.Should().Be("generic-type.ts");
    }

    [Fact]
    public void WhenTypeContainsGenericThenInterfaceIncludesGenericType()
    {
        _generator.Generate(typeof(GenericType<>), _context);

        var model = _context.GetModelForType(typeof(GenericType<>));
        model?.Contents.Should().Contain("export interface GenericType<T>").And.Contain("item: T;");
    }

    [Fact]
    public void WhenTypeContainsMultipleGenericArgumentsThenInterfaceContainsMultipleGenericArguments()
    {
        _generator.Generate(typeof(GenericType<,,>), _context);

        var model = _context.GetModelForType(typeof(GenericType<,,>));
        model
            ?.Contents.Should()
            .Contain("export interface GenericType<T, TR, TU>")
            .And.Contain("first: T")
            .And.Contain("second: TR")
            .And.Contain("third: TU");
    }

    [Fact]
    public void WhenTypeContainsGenericArrayThenInterfaceContainsGenericArray()
    {
        _generator.Generate(typeof(ResultSet<>), _context);

        var model = _context.GetModelForType(typeof(ResultSet<>));
        model?.Contents.Should().Contain("export interface ResultSet<T>").And.Contain("items: Array<T>;");
    }

    [Fact]
    public void WhenTypeIsStaticThenNoModelsShouldBeGenerated()
    {
        _generator.Generate(typeof(TypeExtensions), _context);

        _context.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void WhenTypeIsAnInterfaceThenNoModelsShouldBeGenerated()
    {
        _generator.Generate(typeof(ITypeScriptModelGenerator), _context);

        _context.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void WhenTypeIsEnumThenTypescriptEnumIsGenerated()
    {
        _generator.Generate(typeof(SimpleEnum), _context);

        var model = _context.GetModelForType(typeof(SimpleEnum));
        model?.FileName.Should().Be("simple-enum.ts");
        model?.Contents.Should().Contain("export enum SimpleEnum").And.Contain("Hello = 'Hello',");
    }

    [Fact]
    public void WhenTypeContainsNullablePropertyThenTypescriptPropertyIsOptional()
    {
        _generator.Generate(typeof(ModelWithNullable), _context);

        _context.GetAll().Should().HaveCount(1);
        var model = _context.GetModelForType(typeof(ModelWithNullable));
        model?.Contents.Should().Contain("id?: number");
    }

    [Fact]
    public void WhenTypeContainsArrayOfModelsThenTypeScriptContainsPropertyWithAnArrayOfModels()
    {
        _generator.Generate(typeof(ModelWithArrayOfModels), _context);

        _context.GetAll().Should().HaveCount(2);
        var model = _context.GetModelForType(typeof(ModelWithArrayOfModels));
        model
            ?.Contents.Should()
            .Contain("import {SimpleModel} from './simple-model'")
            .And.Contain("models: Array<SimpleModel>;");
    }

    [Fact]
    public void WhenTypeDerivesFromAnotherTypeThenTypeScriptInterfaceExtendsTheBaseType()
    {
        _generator.Generate(typeof(DerivedFromSimpleModel), _context);

        _context.GetAll().Should().HaveCount(2);
        var model = _context.GetModelForType(typeof(DerivedFromSimpleModel));
        model
            ?.Contents.Should()
            .Contain("export interface DerivedFromSimpleModel extends SimpleModel")
            .And.Contain("import {SimpleModel} from './simple-model';")
            .And.NotContain("id: number;")
            .And.Contain("stuff: string;");
    }

    [Fact]
    public void WhenTypeIsMarkedToBeSkippedThenTypeIsNotGenerated()
    {
        _generator.Generate(typeof(Skippable), _context);

        _context.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void WhenTypeIsAttributeThenSkipped()
    {
        _generator.Generate(typeof(SkipGenerationAttribute), _context);

        _context.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void WhenTypeContainsPropertyWithPrimitiveArrayThenArrayIsNotGenerated()
    {
        _generator.Generate(typeof(ModelWithPrimitiveArray), _context);

        _context.GetAll().Should().HaveCount(1);
    }

    [Fact]
    public void WhenTypeContainsOptionalGenerationAttributesThenOptionalPropertiesAreGeneratedAsOptional()
    {
        _generator.Generate(typeof(ModelWithOptionalProperty), _context);

        var model = _context.GetModelForType(typeof(ModelWithOptionalProperty));
        model?.Contents.Should().Contain("id?: number;").And.Contain("value?: number");
    }
}
