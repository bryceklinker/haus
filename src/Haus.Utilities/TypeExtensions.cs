using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Haus.Core.Models;
using Haus.Utilities.TypeScript.GenerateModels;
using Humanizer;

namespace Haus.Utilities;

public static class TypeExtensions
{
    public const string Number = "number";
    public const string String = "string";
    public const string Any = "any";
    public const string Object = "object";
    public const string Boolean = "boolean";

    public static readonly Dictionary<Type, string> ClrToTypeScript = new()
    {
        { typeof(byte), Number },
        { typeof(double), Number },
        { typeof(int), Number },
        { typeof(long), Number },
        { typeof(string), String },
        { typeof(Guid), String },
        { typeof(DateTime), String },
        { typeof(DateTimeOffset), String },
        { typeof(object), Object },
        { typeof(bool), Boolean },
    };

    public static bool IsStatic(this Type type)
    {
        return type.IsAbstract && type.IsSealed;
    }

    public static string ToTypeScriptFileName(this Type type)
    {
        var name = type.IsGenericType ? type.Name.Split('`')[0] : type.Name;

        return name.Kebaberize();
    }

    public static bool RequiresTypescriptImport(this Type type)
    {
        return type.GetTypeThatRequiresImport() != null;
    }

    public static bool IsSkippable(this Type type)
    {
        return type.IsStatic()
            || type.IsInterface
            || type.IsAssignableTo(typeof(Attribute))
            || type.GetCustomAttributes<SkipGenerationAttribute>().Any();
    }

    public static Type? GetTypeThatRequiresImport(this Type type)
    {
        if (!type.IsNativeTypeScriptType())
            return type;

        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            ArgumentNullException.ThrowIfNull(elementType);
            return elementType.IsNativeTypeScriptType() ? null : elementType;
        }

        if (type.BaseType != null && !type.BaseType.IsNativeTypeScriptType())
            return type.BaseType;

        return null;
    }

    public static string ToTypeScriptType(this Type type, ITypeScriptGeneratorContext context)
    {
        if (ClrToTypeScript.ContainsKey(type))
            return ClrToTypeScript[type];

        if (type.IsGenericParameter)
            return type.Name;

        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            ArgumentNullException.ThrowIfNull(elementType);

            var elementTypeName = elementType.IsNativeTypeScriptType()
                ? elementType.ToTypeScriptType(context)
                : context.GetModelForType(elementType)!.ModelName;
            return $"Array<{elementTypeName}>";
        }

        if (type.IsNullable())
            return $"{type.GetGenericArguments()[0].ToTypeScriptType(context)}";

        return Any;
    }

    public static bool IsNativeTypeScriptType(this Type type)
    {
        if (type.IsPrimitive)
            return true;

        if (type.IsGenericParameter)
            return true;

        if (type.IsArray)
            return true;

        if (type.IsNullable())
            return true;

        if (type == typeof(ValueType))
            return true;

        return ClrToTypeScript.ContainsKey(type);
    }

    public static bool IsNullable(this Type type)
    {
        return type.BaseType == typeof(ValueType)
            && type.IsGenericType
            && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public static string ToTypescriptTypeName(this Type type)
    {
        if (!type.IsGenericTypeDefinition)
            return type.Name;

        var typeName = type.GetGenericTypeDefinition().Name.Split('`')[0];
        var typeArguments = type.GetGenericArguments().Select(t => t.Name);
        return $"{typeName}<{string.Join(", ", typeArguments)}>";
    }
}
