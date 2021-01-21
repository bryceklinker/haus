using System;

namespace Haus.Core.Application.Queries
{
    public record DownloadLatestPackageResult(DownloadStatus Status, byte[] Bytes)
    {
        public static DownloadLatestPackageResult Ok(byte[] bytes)
        {
            return new(DownloadStatus.Ok, bytes);
        }
        public static DownloadLatestPackageResult Error(DownloadStatus status = DownloadStatus.Error)
        {
            return new(status, Array.Empty<byte>());
        }
    }
}