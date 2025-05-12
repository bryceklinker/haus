using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace Haus.Acceptance.Tests.Support;

public static class BrowserContextExtensions
{
    public static async Task StartTracingAsync(this IBrowserContext context)
    {
        await context.Tracing.StartAsync(
            new TracingStartOptions
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true,
            }
        );
    }

    public static async Task StopTracingAsync(this IBrowserContext context)
    {
        await context.Tracing.StopAsync(
            new TracingStopOptions
            {
                Path = Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    "playwright",
                    "traces",
                    $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
                ),
            }
        );
    }
}
