using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haus.Site.Host.Shared.Realtime;

public interface IRealtimeDataFactory
{
    Task<IRealtimeDataSubscriber> CreateSubscriber(string source);
}
