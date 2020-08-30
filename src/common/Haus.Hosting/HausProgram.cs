using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Haus.Hosting
{
    public abstract class HausProgram
    {
        public string Name { get; }
        public string[] Args { get; }

        protected HausProgram(string name, string[] args)
        {
            Name = name;
            Args = args;
        }

        public async Task<int> RunAsync()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", Name)
                .WriteTo.Console()
                .CreateLogger();
            try
            {
                Log.Information($"Starting {Name}...");
                var builder = GetHausHostBuilder(Args);
                ConfigureHost(builder);
                var host = builder.Build();
                await BeforeRunAsync(host);
                await host.RunAsync();
                return 0;
            }
            catch (Exception e)
            {
                Log.Fatal(e, $"{Name} stopped unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder GetHausHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog();
        
        protected abstract void ConfigureHost(IHostBuilder builder);

        protected virtual Task BeforeRunAsync(IHost host)
        {
            return Task.CompletedTask;
        }
    }
}