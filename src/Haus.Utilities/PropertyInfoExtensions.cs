using System.Reflection;
using Haus.Core.Models;

namespace Haus.Utilities
{
    public static class PropertyInfoExtensions
    {
        public static bool IsOptional(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<OptionalGenerationAttribute>() != null
                   || propertyInfo.PropertyType.IsNullable();
        }
    }
}