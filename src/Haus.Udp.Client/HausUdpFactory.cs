using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Haus.Udp.Client
{
    public interface IHausUdpFactory : IAsyncDisposable
    {
        IHausUdpClient CreateClient();
    }

    internal class HausUdpFactory : IHausUdpFactory
    {
        private readonly IOptions<HausUdpSettings> _options;
        private readonly Lazy<IHausUdpClient> _client;

        public HausUdpFactory(IOptions<HausUdpSettings> options)
        {
            _options = options;
            _client = new Lazy<IHausUdpClient>(CreateClientInstance);
        }

        public IHausUdpClient CreateClient()
        {
            return _client.Value;
        }

        private IHausUdpClient CreateClientInstance()
        {
            var udpClient = new UdpClient(_options.Value.Port);
            return new HausUdpClient(udpClient, _options);
        }

        public ValueTask DisposeAsync()
        {
            return _client.IsValueCreated 
                ? _client.Value.DisposeAsync() 
                : ValueTask.CompletedTask;
        }
    }
}