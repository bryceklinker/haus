using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.ServiceLocation;
using Haus.Testing.Support;
using Haus.Udp.Client.Tests.Support;
using Xunit;

namespace Haus.Udp.Client.Tests;

public class HausUdpClientTests : IDisposable
{
    private readonly IHausUdpClient _client;
    private readonly SupportFactory _supportFactory;

    public HausUdpClientTests()
    {
        _supportFactory = new SupportFactory();

        _client = _supportFactory.CreateClient();
    }

    [Fact]
    public async Task WhenBroadcastIsSentThenUdpMessageIsReceived()
    {
        ServiceLocationModel model = null;
        await _client.SubscribeAsync<ServiceLocationModel>(m => model = m);

        await _client.BroadcastAsync(new ServiceLocationModel(KnownServices.Web, "192.168.1.1", 5000));

        Eventually.Assert(() =>
        {
            model.Name.Should().Be(KnownServices.Web);
        });
    }

    [Fact]
    public async Task WhenSubscriberUsesAsyncMethodThenSubscriberReceivesMessages()
    {
        ServiceLocationModel model = null;
        await _client.SubscribeAsync<ServiceLocationModel>(m =>
        {
            model = m;
            return Task.CompletedTask;
        });

        await _client.BroadcastAsync(new ServiceLocationModel(KnownServices.Web, "1.1.1.1", 600));

        Eventually.Assert(() =>
        {
            model.Name.Should().Be(KnownServices.Web);
        });
    }

    [Fact]
    public async Task WhenBroadcastIsSentAfterUnsubscribingThenNoMessageIsReceived()
    {
        ServiceLocationModel model = null;
        var subscription = await _client.SubscribeAsync<ServiceLocationModel>(m => model = m);
        await subscription.UnsubscribeAsync();

        await _client.BroadcastAsync(new ServiceLocationModel(KnownServices.Web, "192.168.1.1", 777));
        await Task.Delay(1000);

        model.Should().BeNull();
    }

    public void Dispose()
    {
        _supportFactory.Dispose();
        GC.SuppressFinalize(this);
    }
}
