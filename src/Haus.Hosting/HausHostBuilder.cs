using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Haus.Hosting
{
    public interface IHausHostBuilder
    {
        IHausHostBuilder WithArgs(string[] args);
        Task<int> RunAsync();
        IHausHostBuilder ConfigureHost(Action<IHostBuilder> configure);
        IHausHostBuilder ConfigureServices(Action<IConfiguration, IServiceCollection> configureServices);
    }
    
    public class HausHostBuilder : IHausHostBuilder
    {
        public string AppName { get; }
        public string[] Args { get; private set; }
        public Action<IHostBuilder> Configure { get; private set; }
        
        public Action<IConfiguration, IServiceCollection> ConfigureHostServices { get; private set; }

        public HausHostBuilder(string appName)
        {
            AppName = appName;
        }

        public IHausHostBuilder WithArgs(string[] args)
        {
            Args = args;
            return this;
        }

        public IHausHostBuilder ConfigureHost(Action<IHostBuilder> configure)
        {
            Configure = configure;
            return this;
        }

        public IHausHostBuilder ConfigureServices(Action<IConfiguration, IServiceCollection> configureServices)
        {
            ConfigureHostServices = configureServices;
            return this;
        }

        public IHost Build()
        {
            Log.Logger = CreateLogger();
            var builder = Host.CreateDefaultBuilder(Args)
                .UseSerilog()
                .ConfigureServices((host, services) =>
                {
                    ConfigureHostServices?.Invoke(host.Configuration, services);
                });
                
            Configure?.Invoke(builder);
            return builder.Build();
        }
        
        public async Task<int> RunAsync()
        {
            Log.Logger = CreateLogger();

            try
            {
                Log.Information("Starting haus host");
                var builder = Host.CreateDefaultBuilder(Args)
                    .UseSerilog()
                    .ConfigureServices((host, services) =>
                    {
                        ConfigureHostServices?.Invoke(host.Configuration, services);
                    });
                
                Configure?.Invoke(builder);
                await builder.Build().RunAsync();
                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host failed unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private Logger CreateLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", AppName)
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}