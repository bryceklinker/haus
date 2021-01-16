using System;
using Haus.Core;
using Haus.Core.Common.Storage;
using Haus.Core.Models;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Haus.Web.Host.Auth;
using Haus.Web.Host.Common.Mqtt;
using Haus.Web.Host.DeviceSimulator;
using Haus.Web.Host.Diagnostics;
using Haus.Web.Host.Health;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Haus.Web.Host
{
    public static class ServiceCollectionExtensions
    {
        private static readonly string MigrationsAssembly = typeof(HausDbContext).Assembly.GetName().Name;
        
        public static IServiceCollection AddHausWebHost(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddHausCore(opts =>
                {
                    opts.UseSqlite(configuration["Database:ConnectionString"], sqlite =>
                    {
                        sqlite.MigrationsAssembly(MigrationsAssembly);
                    });
                })
                .Configure<AuthOptions>(configuration.GetSection("Auth"))
                .Configure<HausMqttSettings>(configuration.GetSection("Mqtt"))
                .AddHausMqtt()
                .AddMediatR(typeof(ServiceCollectionExtensions).Assembly)
                .AddHostedService<MqttMessageRouter>()
                .AddHostedService<DiagnosticsMqttListener>()
                .AddHostedService<DeviceSimulatorStatePublisher>()
                .AddSingleton<IHealthCheckPublisher, HealthPublisher>();
        }

        public static IServiceCollection AddHausAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            var authenticatedUserPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opts =>
            {
                opts.Audience = configuration.GetValue<string>("Auth:Audience");
                opts.Authority = $"https://{configuration.GetValue<string>("Auth:Domain")}";
            });
            return services.AddAuthorization(opts => { opts.DefaultPolicy = authenticatedUserPolicy; });
        }

        public static IServiceCollection AddHausRestApi(this IServiceCollection services)
        {
            services.AddControllers()
                .AddControllersAsServices();
            return services.AddCors(opts =>
            {
                opts.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });
        }

        public static IServiceCollection AddHausRealtimeApi(this IServiceCollection services)
        {
            services.AddSignalR(opts =>
            {
                opts.EnableDetailedErrors = true;
            }).AddJsonProtocol(opts =>
            {
                opts.PayloadSerializerOptions = HausJsonSerializer.DefaultOptions;
            });
            return services;
        }

        public static IServiceCollection AddHausSpa(this IServiceCollection services, string rootPath)
        {
            services.AddSpaStaticFiles(spa => spa.RootPath = rootPath);
            return services;
        }
        
        public static IServiceCollection AddHausHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddHausMqttHealthChecks();
            
            services.AddSingleton<ILastKnownHealthCache, LastKnownHealthCache>()
                .AddMemoryCache(opts => opts.ExpirationScanFrequency = TimeSpan.FromMinutes(1));
            services.Configure<HealthCheckPublisherOptions>(opts =>
            {
                opts.Period = TimeSpan.FromSeconds(10);
            });

            return services;
        }
    }
}