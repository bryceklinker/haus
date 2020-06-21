using System;
using Haus.Identity.Core.Accounts.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haus.Identity.Core.Tests.Support
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider CreateProvider(Action<ServiceProviderOptionsBuilder> configureOptions = null)
        {
            var builder = new ServiceProviderOptionsBuilder();
            configureOptions?.Invoke(builder);
            var options = builder.Build();
            return new ServiceCollection()
                .AddHausIdentityCore()
                .Replace(new ServiceDescriptor(typeof(UserManager<HausUser>), options.UserManager))
                .Replace(new ServiceDescriptor(typeof(IConfiguration), options.Configuration))
                .Replace(new ServiceDescriptor(typeof(ConfigurationDbContext), options.ConfigurationDbContext))
                .BuildServiceProvider();
        }
    }
}