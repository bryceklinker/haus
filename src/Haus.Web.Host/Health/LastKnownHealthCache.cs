using Haus.Core.Models.Health;
using Microsoft.Extensions.Caching.Memory;

namespace Haus.Web.Host.Health
{
    public interface ILastKnownHealthCache
    {
        HausHealthReportModel GetLatestReport();
        void UpdateLatestReport(HausHealthReportModel report);
    }

    public class LastKnownHealthCache : ILastKnownHealthCache
    {
        private const string CacheKey = "last-known-health";
        private readonly IMemoryCache _cache;

        public LastKnownHealthCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public HausHealthReportModel GetLatestReport()
        {
            return _cache.TryGetValue(CacheKey, out HausHealthReportModel value) ? value : null;
        }
        
        public void UpdateLatestReport(HausHealthReportModel report)
        {
            _cache.Set(CacheKey, report);
        }
    }
}