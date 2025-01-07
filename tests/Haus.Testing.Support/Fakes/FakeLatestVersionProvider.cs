using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Haus.Core.Application;

namespace Haus.Testing.Support.Fakes;

public class FakeLatestReleaseProvider : ILatestReleaseProvider
{
    private ReleaseModel Latest { get; set; }
    private ReleasePackageModel[] Packages { get; set; }
    private Dictionary<int, byte[]> PackageBytes { get; } = new();
    private Exception Exception { get; set; }

    public Task<ReleaseModel> GetLatestVersionAsync()
    {
        if (Exception != null)
            throw Exception;

        return Task.FromResult(Latest);
    }

    public Task<ReleasePackageModel[]> GetLatestPackages()
    {
        if (Exception != null)
            throw Exception;

        return Task.FromResult(Packages);
    }

    public Task<Stream> DownloadLatestPackage(int id)
    {
        if (Exception != null)
            throw Exception;

        return Task.FromResult<Stream>(new MemoryStream(PackageBytes[id]));
    }

    public void SetupLatestVersion(ReleaseModel model)
    {
        Latest = model;
    }

    public void SetupLatestPackages(params ReleasePackageModel[] packages)
    {
        Packages = packages;
    }

    public void SetupPackageDownload(int packageId, byte[] bytes)
    {
        PackageBytes.Add(packageId, bytes);
    }

    public void SetupFailure(Exception exception)
    {
        Exception = exception;
    }
}