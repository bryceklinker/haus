using System;
using System.IO;
using System.Threading.Tasks;

namespace Haus.Core.Application;

public record ReleaseModel(Version Version, bool IsOfficial, DateTimeOffset CreationDateTime, string Description);

public record ReleasePackageModel(int Id, string Name);

public interface ILatestReleaseProvider
{
    Task<ReleaseModel?> GetLatestVersionAsync();
    Task<ReleasePackageModel[]> GetLatestPackages();
    Task<Stream?> DownloadLatestPackage(int id);
}
