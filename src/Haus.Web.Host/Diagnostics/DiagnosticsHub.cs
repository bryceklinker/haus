using System.Threading.Tasks;
using Haus.Core.Models.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Web.Host.Diagnostics
{
    [Authorize]
    public class DiagnosticsHub : Hub
    {
        public async Task OnMqttMessage(MqttDiagnosticsMessageModel model)
        {
            await Clients.All.SendAsync("OnMqttMessage", model);
        } 
    }
}