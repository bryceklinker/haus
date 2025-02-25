using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Humanizer;

namespace Haus.Utilities.TypeScript.GenerateModels;

public interface ITypeScriptModelGenerator
{
    void Generate(Type type, ITypeScriptGeneratorContext context);
}

public class TypeScriptModelGenerator : ITypeScriptModelGenerator
{
    public void Generate(Type type, ITypeScriptGeneratorContext context)
    {
        if (type.IsSkippable())
            return;

        var filename = $"{type.ToTypeScriptFileName()}.ts";
        var contents = type.IsEnum ? GenerateTypeScriptEnum(type) : GenerateTypeScriptInterface(type, context);
        context.Add(new TypeScriptModel(type, filename, contents));
    }

    private string GenerateTypeScriptEnum(Type type)
    {
        var values = Enum.GetNames(type).Select(value => $"\t{value} = '{value}',");

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
            .AppendLine(GenerateImportStatements(type, context))
            .AppendLine()
            .AppendLine(GenerateTypeScriptInterfaceDeclaration(type, context))
            .AppendLine(GenerateTypeScriptProperties(type, context))
            .AppendLine("}")
            .ToString();
    }

    private string GenerateImportStatements(Type type, ITypeScriptGeneratorContext context)
    {
        var imports = GetTypesToImport(type, context).Select(GenerateImportStatement);
        return string.Join(Environment.NewLine, imports);
    }

    private string GenerateTypeScriptInterfaceDeclaration(Type type, ITypeScriptGeneratorContext context)
    {
        if (!type.BaseType.RequiresTypescriptImport())
            return $"export interface {type.ToTypescriptTypeName()} {{";

        var baseModel = context.GetModelForType(type.BaseType);
        return $"export interface {type.ToTypescriptTypeName()} extends {baseModel.ModelName} {{";
    }

    private string GenerateImportStatement(TypeScriptModel model)
    {
        var fileName = Path.GetFileNameWithoutExtension(model.FileName);
        return $"import {{{model.ModelName}}} from './{fileName}';";
    }

    private IEnumerable<TypeScriptModel> GetTypesToImport(Type type, ITypeScriptGeneratorContext context)
    {
        var propertyInfos = type.GetProperties();
        var importTypes = propertyInfos
            .Select(p => p.PropertyType)
            .Where(t => t.RequiresTypescriptImport())
            .Select(t => t.GetTypeThatRequiresImport())
            .Select(t => GetOrGenerateModelForType(t, context));

        if (type.BaseType != null && type.BaseType.RequiresTypescriptImport())
            importTypes = importTypes.Append(
                GetOrGenerateModelForType(type.BaseType.GetTypeThatRequiresImport(), context)
            );

        return importTypes.ToArray();
    }

    private TypeScriptModel GetOrGenerateModelForType(Type type, ITypeScriptGeneratorContext context)
    {
        if (context.IsMissingModelForType(type))
            Generate(type, context);

        return context.GetModelForType(type);
    }

    private static string GenerateTypeScriptProperties(Type type, ITypeScriptGeneratorContext context)
    {
        var lines = type.GetProperties()
            .Where(property => property.DeclaringType == type)
            .Select(p => GenerateTypeScriptProperty(p, context));
        return string.Join(Environment.NewLine, lines);
    }

    private static string GenerateTypeScriptProperty(PropertyInfo property, ITypeScriptGeneratorContext context)
    {
        var propertyName = property.Name.Camelize();
        var typescriptPropertyType = property.PropertyType.IsNativeTypeScriptType()
            ? property.PropertyType.ToTypeScriptType(context)
            : context.GetModelForType(property.PropertyType).ModelName;

        if (property.IsOptional())
            return $"\t{propertyName}?: {typescriptPropertyType};";
        return $"\t{propertyName}: {typescriptPropertyType};";
    }
}
