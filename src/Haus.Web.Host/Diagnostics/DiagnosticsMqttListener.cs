using System.Threading.Tasks;
using Haus.Core.Diagnostics.Factories;
using Haus.Web.Host.Common.Mqtt;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;

namespace Haus.Web.Host.Diagnostics
{
    public class DiagnosticsMqttListener : MqttBackgroundServiceListener
    {
        public DiagnosticsMqttListener(
            IHausMqttClientFactory hausMqttClientFactory,
            IServiceScopeFactory scopeFactory, 
            ILogger<DiagnosticsMqttListener> logger) 
            : base(hausMqttClientFactory, scopeFactory, logger)
        {
        }

        protected override async Task OnMessageReceived(MqttApplicationMessage message)
        {
            using var scope = CreateScope();
            var hub = scope.GetService<IHubContext<DiagnosticsHub>>();
            var messageFactory = scope.GetService<IMqttDiagnosticsMessageFactory>();
            var model = messageFactory.Create(message.Topic, message.Payload);
            await hub.BroadcastAsync("OnMqttMessage", model);
        }
    }
}