using System;
using System.Threading.Tasks;
using Haus.Hosting;
using Haus.Utilities.Common.Cli;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Utilities
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            HausLogger.Configure("Haus Utilities");
            try
            {
                await ExecuteCommand(args);
                return 0;
            }
            catch (Exception e)
            {
                HausLogger.Fatal(e, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                HausLogger.EnsureFlushed();
            }
        }

        private static async Task ExecuteCommand(string[] args)
        {
            var provider = BuildServiceProvider();
            using var scope = provider.CreateScope();
            var command = scope.ServiceProvider.GetRequiredService<ICommandFactory>().Create(args);
            await scope.ServiceProvider.GetRequiredService<IMediator>().Send(command);
        }

        private static IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddHausUtilities()
                .BuildServiceProvider();
        }
    }
}
