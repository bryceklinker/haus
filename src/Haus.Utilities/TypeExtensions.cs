using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haus.Utilities
{
    public static class TypeExtensions
    {
        public const string Number = "number";
        public const string String = "string";
        public const string Any = "any";
        public const string Object = "object";
        
        public static readonly Dictionary<Type, string> ClrToTypeScript = new()
        {   
            {typeof(object), Object},
            {typeof(double), Number},
            {typeof(int), Number},
            {typeof(long), Number},
            {typeof(string), String},
            {typeof(Guid), String},
            {typeof(DateTime), String}
        };

        public static string ToTypeScriptType(this Type type)
        {
            if (ClrToTypeScript.ContainsKey(type))
                return ClrToTypeScript[type];

            if (type.IsGenericParameter)
                return type.Name;

            if (type.IsArray)
                return $"Array<{type.Name.Split('[')[0]}>";

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

            return ClrToTypeScript.ContainsKey(type);
        }
        
        public static string ToTypescriptInterfaceName(this Type type)
        {
            if (!type.IsGenericTypeDefinition)
                return type.Name;

            var typeName = type.GetGenericTypeDefinition().Name.Split('`')[0];
            var typeArguments = type.GetGenericArguments()
                .Select(t => t.Name);
            return $"{typeName}<{string.Join(", ", typeArguments)}>";
        }
    }
}