using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Haus.Core.Models;

namespace Haus.Udp.Client
{
    public interface IHausUdpBroadcaster
    {
        Task BroadcastAsync<T>(T value);
    }

    internal class HausUdpBroadcaster : IHausUdpBroadcaster
    {
        private readonly UdpClient _client;
        private readonly int _port;

        public HausUdpBroadcaster(UdpClient client, int port)
        {
            _client = client;
            _port = port;
        }

        public Task BroadcastAsync<T>(T value)
        {
            var bytes = HausJsonSerializer.SerializeToBytes(value);
            return _client.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, _port));
        }
    }
}