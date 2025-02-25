using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Web.Host.Common.Events;

[Authorize]
public class EventsHub : Hub { }
