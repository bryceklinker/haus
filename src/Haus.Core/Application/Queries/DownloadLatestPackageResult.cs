using System.IO;

namespace Haus.Core.Application.Queries;

public record DownloadLatestPackageResult(DownloadStatus Status, Stream Stream)
{
    public static DownloadLatestPackageResult Ok(Stream stream)
    {
        return new DownloadLatestPackageResult(DownloadStatus.Ok, stream);
    }

    public static DownloadLatestPackageResult Error(DownloadStatus status = DownloadStatus.Error)
    {
        return new DownloadLatestPackageResult(status, new MemoryStream());
    }
}
