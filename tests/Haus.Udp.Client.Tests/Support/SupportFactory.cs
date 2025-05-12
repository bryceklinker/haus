using System;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Udp.Client.Tests.Support;

public class SupportFactory : IDisposable
{
    private readonly IServiceScope _scope;

    public SupportFactory(int port = 6000)
    {
        _scope = new ServiceCollection()
            .AddHausUdp()
            .AddLogging()
            .Configure<HausUdpSettings>(opts =>
            {
                opts.Port = port;
            })
            .BuildServiceProvider()
            .CreateScope();
    }

    public IHausUdpClient CreateClient()
    {
        return _scope.ServiceProvider.GetRequiredService<IHausUdpClient>();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
}
