using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
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
            : base(hausMqttClientFactory, scopeFactory, logger)
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

            var eventBus = scope.GetService<IEventBus>();
            await eventBus.PublishAsync(@event, CancellationToken.None);
        }
    }
}