using Haus.Core;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Haus.Web.Host.Auth;
using Haus.Web.Host.Common.Mqtt;
using Haus.Web.Host.Common.Services;
using Haus.Web.Host.Devices;
using Haus.Web.Host.DeviceSimulator;
using Haus.Web.Host.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Web.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausWebHost(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddHausCore(opts =>
                {
                    opts.UseInMemoryDatabase("Haus");
                })
                .Configure<AuthOptions>(configuration.GetSection("Auth"))
                .Configure<HausMqttSettings>(configuration.GetSection("Mqtt"))
                .AddHausMqtt()
                .AddMediatR(typeof(ServiceCollectionExtensions).Assembly)
                .AddHostedService<MqttMessageRouter>()
                .AddHostedService<DiagnosticsMqttListener>()
                .AddHostedService<DeviceSimulatorStatePublisher>()
                .AddHostedService<InitializerService>();
        }

        public static IServiceCollection AddAuthenticatedUserRequired(this IServiceCollection services,
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
            return services.AddAuthorization(opts =>
            {
                opts.DefaultPolicy = authenticatedUserPolicy;
            });
        }

        public static IServiceCollection AddRestApi(this IServiceCollection services)
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
    }
}