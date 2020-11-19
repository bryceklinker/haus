using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Web.Host
{
    public static class HubContextExtensions
    {
        public static async Task BroadcastAsync<T>(this IHubContext<T> hub, string method, object arg) 
            where T : Hub
        {
            await hub.Clients.All.SendAsync(method, arg);
        }
    }
}