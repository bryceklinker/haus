using Microsoft.Extensions.Configuration;

namespace Haus.Portal.Web
{
    public static class ConfigurationExtensions
    {
        public static string AuthorityUrl(this IConfiguration config)
        {
            return config.GetValue<string>("AuthorityUrl");
        }
    }
}