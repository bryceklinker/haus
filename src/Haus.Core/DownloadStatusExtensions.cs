using System.Net;
using Haus.Core.Application.Queries;

namespace Haus.Core;

public static class DownloadStatusExtensions
{
    public static DownloadStatus ToDownloadStatus(this HttpStatusCode? status)
    {
        return status switch
        {
            HttpStatusCode.NotFound => DownloadStatus.NotFound,
            _ => DownloadStatus.Error,
        };
    }

    public static HttpStatusCode ToHttpStatus(this DownloadStatus status)
    {
        return status switch
        {
            DownloadStatus.Ok => HttpStatusCode.OK,
            DownloadStatus.NotFound => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError,
        };
    }
}
