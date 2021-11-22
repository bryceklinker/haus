using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Haus.Web.Host.Common.SignalR
{
    public static class JwtAuthEventHandlers
    {
        public static Task HandleMessageReceived(MessageReceivedContext context)
        {
            var accessToken = context.Request.Query["access_token"];
            if (string.IsNullOrEmpty(accessToken))
                return Task.CompletedTask;

            var path = context.Request.Path;
            if (HubPatterns.MatchesHub(path)) 
                context.Token = accessToken;
            
            return Task.CompletedTask;
        }
    }
}