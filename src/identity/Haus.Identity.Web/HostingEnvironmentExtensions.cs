using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Haus.Identity.Web
{
    public static class HostingEnvironmentExtensions
    {
        public static bool IsNonProd(this IWebHostEnvironment env)
        {
            return !env.IsEnvironment("Production");
        }
    }
}