using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Web
{
    public static class ConfigurationExtensions
    {
        public static string PortalRedirectUri(this IConfiguration config)
        {
            return config.GetValue<string>("Portal:RedirectUri");
        }

        public static string ServiceBusHostname(this IConfiguration config)
        {
            return config.GetValue<string>("ServiceBus:Hostname");
        }
        
        public static string ServiceBusUsername(this IConfiguration config)
        {
            return config.GetValue<string>("ServiceBus:Username");
        }
        
        public static string ServiceBusPassword(this IConfiguration config)
        {
            return config.GetValue<string>("ServiceBus:Password");
        }

        public static string DbConnectionString(this IConfiguration config)
        {
            return config.GetValue<string>("DbConnectionString");
        }

        public static string AdminUsername(this IConfiguration config)
        {
            return config.GetValue<string>("AdminUser:Username");
        }

        public static string AdminPassword(this IConfiguration config)
        {
            return config.GetValue<string>("AdminUser:Password");
        }
    }
}