using FluentAssertions;
using Haus.Utilities.TypeScript.GenerateModels;
using Xunit;

namespace Haus.Utilities.Tests.TypeScript.GenerateModels
{
    public class TypeScriptModelGeneratorTests
    {
        private readonly TypeScriptGeneratorContext _context;
        private readonly TypeScriptModelGenerator _generator;

        public TypeScriptModelGeneratorTests()
        {
            _context = new TypeScriptGeneratorContext();
            _generator = new TypeScriptModelGenerator();
        }
        
        [Fact]
        public void WhenTypeContainsSimplePropertiesThenReturnsSingleTypeScriptModel()
        {
            _generator.Generate(typeof(SimpleModel), _context);

            var models = _context.GetAll();
            models.Should().HaveCount(1)
                .And.Contain(m => m.ModelType == typeof(SimpleModel))
                .And.Contain(m => m.FileName == "simple-model.ts");
        }

        [Fact]
        public void WhenTypeScriptModelGeneratedThenTypescriptInterfaceDefinedForType()
        {
            _generator.Generate(typeof(SimpleModel), _context);

            var model = _context.GetModelForType(typeof(SimpleModel));
            model.Contents.Should().Contain("export interface SimpleModel");
        }

        [Fact]
        public void WhenTypescriptModelGeneratedThenEachPropertyIsInInterface()
        {
            _generator.Generate(typeof(SimpleModel), _context);

            var model = _context.GetModelForType(typeof(SimpleModel));
            model.Contents.Should()
                .Contain("id: number;")
                .And.Contain("name: string;")
                .And.Contain("value: number");
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
            model.Contents.Should()
                .Contain("import {SimpleModel} from './simple-model';");
        }

        [Fact]
        public void WhenTypeContainsGenericThenInterfaceIncludesGenericType()
        {
            _generator.Generate(typeof(GenericType<>), _context);

            var model = _context.GetModelForType(typeof(GenericType<>));
            model.Contents.Should()
                .Contain("export interface GenericType<T>")
                .And.Contain("item: T;");
        }

        [Fact]
        public void WhenTypeContainsMultipleGenericArgumentsThenInterfaceContainsMultipleGenericArguments()
        {
            _generator.Generate(typeof(GenericType<,,>), _context);

            var model = _context.GetModelForType(typeof(GenericType<,,>));
            model.Contents.Should()
                .Contain("export interface GenericType<T, R, U>")
                .And.Contain("first: T")
                .And.Contain("second: R")
                .And.Contain("third: U");
        }

        [Fact]
        public void WhenTypeContainsGenericArrayThenInterfaceContainsGenericArray()
        {
            _generator.Generate(typeof(ResultSet<>), _context);

            var model = _context.GetModelForType(typeof(ResultSet<>));
            model.Contents.Should()
                .Contain("export interface ResultSet<T>")
                .And.Contain("items: Array<T>;");
        }
    }

    public class SimpleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class SlightlyComplexModel
    {
        public long Id { get; set; }
        public SimpleModel Simple { get; set; }
    }

    public class GenericType<T>
    {
        public T Item { get; set; }
    }

    public class GenericType<T, R, U>
    {
        public T First { get; set; }
        public R Second { get; set; }
        public U Third { get; set; }
    }

    public class ResultSet<T>
    {
        public T[] Items { get; set; }
    }
}