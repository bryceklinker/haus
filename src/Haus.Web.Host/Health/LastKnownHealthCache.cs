using Haus.Core.Models.Health;
using Microsoft.Extensions.Caching.Memory;

namespace Haus.Web.Host.Health
{
    public interface ILastKnownHealthCache
    {
        HausHealthReportModel LastKnownHealth { get; set; }
    }

    public class LastKnownHealthCache : ILastKnownHealthCache
    {
        private const string CacheKey = "last-known-health";
        private readonly IMemoryCache _cache;

        public HausHealthReportModel LastKnownHealth
        {
            get => _cache.TryGetValue(CacheKey, out HausHealthReportModel value) ? value : null;
            set => _cache.Set(CacheKey, value);
        }

        public LastKnownHealthCache(IMemoryCache cache)
        {
            _cache = cache;
        }
    }
}