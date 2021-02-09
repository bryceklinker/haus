using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.ServiceLocation;
using Haus.Testing.Support;
using Haus.Udp.Client.Tests.Support;
using Xunit;

namespace Haus.Udp.Client.Tests
{
    public class HausUdpClientTests : IDisposable
    {
        private readonly IHausUdpBroadcaster _broadcaster;
        private readonly IHausUdpSubscriber _subscriber;
        private readonly SupportFactory _supportFactory;

        public HausUdpClientTests()
        {
            _supportFactory = new SupportFactory();

            _broadcaster = _supportFactory.CreateBroadcaster();
            _subscriber = _supportFactory.CreateSubscriber();
        }
        
        [Fact]
        public async Task WhenBroadcastIsSentThenUdpMessageIsReceived()
        {
            ServiceLocationModel model = null;
            _subscriber.Subscribe<ServiceLocationModel>(m => model = m);

            await _broadcaster.BroadcastAsync(new ServiceLocationModel(KnownServices.Web, "192.168.1.1", 5000));
            
            Eventually.Assert(() =>
            {
                model.Name.Should().Be(KnownServices.Web);
            });
        }

        public void Dispose()
        {
            _supportFactory.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}