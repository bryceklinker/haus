using System;

namespace Haus.ServiceBus
{
    public static class ObjectExtensions
    {
        public static string GetPayloadType(this object instance)
        {
            return instance.GetType().GetPayloadType();
        }

        public static string GetPayloadType(this Type type)
        {
            return type.FullName.ToLowerInvariant();
        }
        
    }
}