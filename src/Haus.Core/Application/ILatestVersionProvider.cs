using System;
using System.Threading.Tasks;

namespace Haus.Core.Application;

public record ReleaseModel(Version Version, bool IsOfficial, DateTimeOffset CreationDateTime, string Description);

public record ReleasePackageModel(int Id, string Name);

public interface ILatestReleaseProvider
{
    Task<ReleaseModel> GetLatestVersionAsync();
    Task<ReleasePackageModel[]> GetLatestPackages();
    Task<byte[]> DownloadLatestPackage(int id);
}