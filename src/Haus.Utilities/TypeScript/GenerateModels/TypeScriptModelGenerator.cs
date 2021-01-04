using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Humanizer;

namespace Haus.Utilities.TypeScript.GenerateModels
{
    
    public interface ITypeScriptModelGenerator
    {
        void Generate(Type type, ITypeScriptGeneratorContext context);
    }
    
    public class TypeScriptModelGenerator : ITypeScriptModelGenerator
    {
        public void Generate(Type type, ITypeScriptGeneratorContext context)
        {
            var filename = $"{type.Name.Kebaberize()}.ts";
            var dependentTypes = GetDependentTypes(type, context);
            var contents = GenerateTypeScriptInterface(type, dependentTypes);

            context.Add(new TypeScriptModel(type, filename, contents));
        }

        private IEnumerable<TypeScriptModel> GetDependentTypes(Type type, ITypeScriptGeneratorContext generatorContext)
        {
            var propertyInfos = type.GetProperties();
            var dependentTypes = propertyInfos
                .Where(p => !p.PropertyType.IsNativeTypeScriptType())
                .Select(p => p.PropertyType);
            foreach (var dependentType in dependentTypes)
            {
                if (generatorContext.IsMissingModelForType(dependentType))
                    Generate(dependentType, generatorContext);

                yield return generatorContext.GetModelForType(dependentType);
            }
        }
        
        private static string GenerateTypeScriptInterface(Type type, IEnumerable<TypeScriptModel> dependencies)
        {
            return new StringBuilder()
                .Append(GenerateImportStatements(dependencies))
                .Append($"export interface {type.ToTypescriptInterfaceName()} ")
                .AppendLine("{")
                .Append(GenerateTypeScriptProperties(type))
                .AppendLine("}")
                .ToString();
        }

        private static string GenerateImportStatements(IEnumerable<TypeScriptModel> dependencies)
        {
            var builder = new StringBuilder();
            foreach (var dependency in dependencies)
            {
                builder.Append("import {")
                    .Append($"{dependency.ModelName}")
                    .Append("} from './")
                    .Append($"{Path.GetFileNameWithoutExtension(dependency.FileName)}';");
            }
            return builder.ToString();
        }
        
        private static string GenerateTypeScriptProperties(Type type)
        {
            var builder = new StringBuilder();
            foreach (var property in type.GetProperties())
                builder.AppendLine($"{property.Name.Camelize()}: {property.PropertyType.ToTypeScriptType()};");
            return builder.ToString();
        }
    }
}