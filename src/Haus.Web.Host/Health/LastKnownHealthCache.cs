using Haus.Core.Models.Health;
using Microsoft.Extensions.Caching.Memory;

namespace Haus.Web.Host.Health;

public interface ILastKnownHealthCache
{
    HausHealthReportModel GetLatestReport();
    void UpdateLatestReport(HausHealthReportModel report);
}

public class LastKnownHealthCache(IMemoryCache cache) : ILastKnownHealthCache
{
    private const string CacheKey = "last-known-health";

    public HausHealthReportModel GetLatestReport()
    {
        return cache.TryGetValue(CacheKey, out HausHealthReportModel value) ? value : null;
    }

    public void UpdateLatestReport(HausHealthReportModel report)
    {
        cache.Set(CacheKey, report);
    }
}