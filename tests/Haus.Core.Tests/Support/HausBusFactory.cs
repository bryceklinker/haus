using System;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haus.Core.Tests.Support
{
    public static class HausBusFactory
    {
        public static IHausBus Create(HausDbContext context = null)
        {
            var services = new ServiceCollection()
                .AddHausCore(opts => opts.UseInMemoryDatabase($"{Guid.NewGuid()}"))
                .AddLogging();
            
            if (context != null)
            {
                services.RemoveAll(typeof(HausDbContext));
                services.AddSingleton(context);
            }

            return services.BuildServiceProvider()
                .GetRequiredService<IHausBus>();
        }
    }
}