using System;
using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Hosting;
using Haus.Utilities.Common.Cli;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Utilities
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            HausLogger.ConfigureConsoleOnly("Haus Utilities");
            try
            {
                var bus = BuildHausBus();
                await bus.ExecuteCommandAsync(new ExecuteCommand(args));
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

        private static IHausBus BuildHausBus()
        {
            return new ServiceCollection()
                .AddHausUtilities()
                .AddHausCqrs(typeof(Program).Assembly)
                .BuildServiceProvider()
                .GetRequiredService<IHausBus>();
        }
    }
}
