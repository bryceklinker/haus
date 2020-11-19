using System.Threading.Tasks;
using Haus.Web.Host.Diagnostics.Models;
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