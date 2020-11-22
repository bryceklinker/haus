using Haus.Core;
using Haus.Core.Common;
using Haus.Web.Host.Auth;
using Haus.Web.Host.Common.Mqtt;
using Haus.Web.Host.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Diagnostics;

namespace Haus.Web.Host
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausWebHost(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddHausCore()
                .Configure<AuthOptions>(configuration.GetSection("Auth"))
                .Configure<MqttOptions>(configuration.GetSection("Mqtt"))
                .AddTransient<IMqttNetLogger, MqttLogger>()
                .AddSingleton<IMqttClientCreator, MqttClientCreator>()
                .AddHostedService<DiagnosticsMqttListener>();
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
            return services;
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