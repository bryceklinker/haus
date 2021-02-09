using System;
using System.Net.Sockets;
using Microsoft.Extensions.Options;

namespace Haus.Udp.Client
{
    public interface IHausUdpFactory : IDisposable
    {
        IHausUdpBroadcaster GetBroadcaster();
        IHausUdpSubscriber GetSubscriber();
    }

    internal class HausUdpFactory : IHausUdpFactory
    {
        private readonly IOptions<HausUdpSettings> _options;
        private readonly Lazy<UdpClient> _client;

        public HausUdpSettings Settings => _options.Value;
        public UdpClient Client => _client.Value;

        public HausUdpFactory(IOptions<HausUdpSettings> options)
        {
            _options = options;
            _client = new Lazy<UdpClient>(CreateUdpClient);
        }

        public IHausUdpBroadcaster GetBroadcaster()
        {
            return new HausUdpBroadcaster(Client, Settings.Port);
        }

        public IHausUdpSubscriber GetSubscriber()
        {
            return new HausUdpSubscriber(Client);
        }

        private UdpClient CreateUdpClient()
        {
            return new(Settings.Port);
        }

        public void Dispose()
        {
            if (_client.IsValueCreated) 
                Client.Dispose();
        }
    }
}