using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Haus.Hosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHausRequestLogging(this IApplicationBuilder app)
        {
            return app.UseSerilogRequestLogging();
        }
    }
}