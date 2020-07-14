using System;
using Haus.Identity.Core.Common.Storage;
using Haus.Identity.Core.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Identity.Core.Tests.Support
{
    public static class InMemoryUserManagerFactory
    {
        public static UserManager<HausUser> Create()
        {
            var services = new ServiceCollection()
                .AddHausIdentityCore()
                .AddLogging()
                .AddDbContext<HausIdentityDbContext>(opts =>
                    opts.UseInMemoryDatabase($"{Guid.NewGuid()}")
                );
            return services.BuildServiceProvider().GetRequiredService<UserManager<HausUser>>();
        }
    }
}