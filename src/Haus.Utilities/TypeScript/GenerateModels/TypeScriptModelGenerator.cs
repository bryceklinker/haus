using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            if (type.IsStatic())
                return;

            if (type.IsInterface)
                return;

            var filename = $"{type.ToTypeScriptFileName()}.ts";
            var contents = type.IsEnum
                ? GenerateTypeScriptEnum(type)
                : GenerateTypeScriptInterface(type, context);
            context.Add(new TypeScriptModel(type, filename, contents));
        }

        private string GenerateTypeScriptEnum(Type type)
        {
            var values = Enum.GetNames(type)
                .Select(value => $"\t{value} = '{value}',");

            var enumerationValues = string.Join(Environment.NewLine, values);
            return new StringBuilder()
                .AppendLine($"export enum {type.ToTypescriptTypeName()} {{")
                .AppendLine(enumerationValues)
                .AppendLine("}")
                .ToString();

        }
        
        private string GenerateTypeScriptInterface(Type type, ITypeScriptGeneratorContext context)
        {
            return new StringBuilder()
                .Append(GenerateImportStatements(type, context))
                .AppendLine($"export interface {type.ToTypescriptTypeName()} {{")
                .Append(GenerateTypeScriptProperties(type, context))
                .AppendLine("}")
                .ToString();
        }

        private string GenerateImportStatements(Type type, ITypeScriptGeneratorContext context)
        {
            var imports = GetDependentTypes(type, context)
                .Select(GenerateImportStatement);
            return string.Join(Environment.NewLine, imports);
        }

        private string GenerateImportStatement(TypeScriptModel model)
        {
            var fileName = Path.GetFileNameWithoutExtension(model.FileName);
            return $"import {{{model.ModelName}}} from './{fileName}';{Environment.NewLine}";
        }

        private IEnumerable<TypeScriptModel> GetDependentTypes(Type type, ITypeScriptGeneratorContext context)
        {
            var propertyInfos = type.GetProperties();
            return propertyInfos
                .Where(p => !p.PropertyType.IsNativeTypeScriptType())
                .Select(p => p.PropertyType)
                .Select(dependentType =>
                {
                    if (context.IsMissingModelForType(dependentType))
                        Generate(dependentType, context);
                    
                    return context.GetModelForType(dependentType);
                });
        }

        private static string GenerateTypeScriptProperties(Type type, ITypeScriptGeneratorContext context)
        {
            var lines = type.GetProperties()
                .Select(p => GenerateTypeScriptProperty(p, context));
            return string.Join(Environment.NewLine, lines);
        }

        private static string GenerateTypeScriptProperty(PropertyInfo property, ITypeScriptGeneratorContext context)
        {
            var propertyName = property.PropertyType.IsNullable()
                ? $"{property.Name.Camelize()}?"
                : property.Name.Camelize();
            
            var typescriptPropertyType = property.PropertyType.IsNativeTypeScriptType()
                ? property.PropertyType.ToTypeScriptType()
                : context.GetModelForType(property.PropertyType).ModelName;
            return $"\t{propertyName}: {typescriptPropertyType};{Environment.NewLine}";
        }
    }
}