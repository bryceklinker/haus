using System;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Udp.Client.Tests.Support
{
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
        
        public IHausUdpBroadcaster CreateBroadcaster()
        {
            return _scope.ServiceProvider.GetRequiredService<IHausUdpBroadcaster>();
        }

        public IHausUdpSubscriber CreateSubscriber()
        {
            return _scope.ServiceProvider.GetRequiredService<IHausUdpSubscriber>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}