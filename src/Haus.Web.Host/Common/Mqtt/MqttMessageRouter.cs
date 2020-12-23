using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Events;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Haus.Web.Host.Common.Mqtt
{
    public class MqttMessageRouter : MqttBackgroundServiceListener
    {
        public MqttMessageRouter(IHausMqttClientFactory hausMqttClientFactory,
            IServiceScopeFactory scopeFactory,
            ILogger<MqttMessageRouter> logger)
            : base(hausMqttClientFactory, scopeFactory)
        {
        }
        
        protected override async Task OnMessageReceived(MqttApplicationMessage message)
        {
            await RouteMqttMessage(message);
        }

        private async Task RouteMqttMessage(MqttApplicationMessage message)
        {
            using var scope = CreateScope();
            var eventFactory = scope.GetService<IRoutableEventFactory>();
            var @event = eventFactory.Create(message.Payload);
            if (@event == null)
                return;

            var hausBus = scope.GetService<IHausBus>();
            await hausBus.PublishAsync(@event, CancellationToken.None);
        }
    }
}