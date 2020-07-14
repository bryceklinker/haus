using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Haus.Logging
{
    public class HausLoggingProgram
    {
        private readonly string _application;
        private readonly Func<string[], IHostBuilder> _getHostBuilder;
        private readonly Func<IHost, Task> _runHost;

        public HausLoggingProgram(
            string application,
            Func<string[], IHostBuilder> getHostBuilder, 
            Func<IHost, Task> runHost)
        {
            _application = application;
            _getHostBuilder = getHostBuilder;
            _runHost = runHost;
        }

        public async Task<int> Main(string[] args)
        {
            HausLogging.Configure(_application);
            try
            {
                var hostBuilder = _getHostBuilder.Invoke(args);
                hostBuilder.UseHuasLogging();
                await _runHost(hostBuilder.Build());
                return 0;
            }
            catch (Exception ex)
            {
                HausLogging.LogFatal(ex, "Failed to start");
                return 1;
            }
            finally
            {
                HausLogging.CloseAndFlush();
            }
        }
    }
}