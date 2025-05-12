using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Application;
using Haus.Core.Application.Queries;
using Haus.Cqrs;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Core.Tests.Application.Queries;

public class DownloadLatestPackageQueryHandlerTests
{
    private readonly FakeLatestReleaseProvider _latestReleaseProvider;
    private readonly IHausBus _hausBus;

    public DownloadLatestPackageQueryHandlerTests()
    {
        _latestReleaseProvider = new FakeLatestReleaseProvider();
        _hausBus = HausBusFactory.Create(configureServices: services =>
            services.AddSingleton<ILatestReleaseProvider>(_latestReleaseProvider)
        );
    }

    [Fact]
    public async Task WhenDownloadingPackageThenReturnsPackageBytesFromProvider()
    {
        _latestReleaseProvider.SetupPackageDownload(6, [3, 2, 1]);

        var result = await _hausBus.ExecuteQueryAsync(new DownloadLatestPackageQuery(6));

        result.Status.Should().Be(DownloadStatus.Ok);
        var bytes = await ReadResultAsByteArrayAsync(result);
        bytes.Should().BeEquivalentTo(new byte[] { 3, 2, 1 });
    }

    [Fact]
    public async Task WhenDownloadingPackageFailsWithNotFoundStatusThenReturnsNotFoundDownloadResult()
    {
        _latestReleaseProvider.SetupFailure(new HttpRequestException("", new Exception(), HttpStatusCode.NotFound));

        var result = await _hausBus.ExecuteQueryAsync(new DownloadLatestPackageQuery(8));

        result.Status.Should().Be(DownloadStatus.NotFound);
        var bytes = await ReadResultAsByteArrayAsync(result);
        bytes.Should().BeEmpty();
    }

    [Fact]
    public async Task WhenDownloadingPackageFailsWithInternalServerErrorThenReturnsErrorDownloadResult()
    {
        _latestReleaseProvider.SetupFailure(
            new HttpRequestException("", new Exception(), HttpStatusCode.InternalServerError)
        );

        var result = await _hausBus.ExecuteQueryAsync(new DownloadLatestPackageQuery(8));

        result.Status.Should().Be(DownloadStatus.Error);
        var bytes = await ReadResultAsByteArrayAsync(result);
        bytes.Should().BeEmpty();
    }

    [Fact]
    public async Task WhenDownloadingPackageFailsThenReturnsErrorDownloadResult()
    {
        _latestReleaseProvider.SetupFailure(new Exception());

        var result = await _hausBus.ExecuteQueryAsync(new DownloadLatestPackageQuery(8));

        result.Status.Should().Be(DownloadStatus.Error);
        var bytes = await ReadResultAsByteArrayAsync(result);
        bytes.Should().BeEmpty();
    }

    private static async Task<byte[]> ReadResultAsByteArrayAsync(DownloadLatestPackageResult result)
    {
        using var stream = new MemoryStream();
        await result.Stream.CopyToAsync(stream);
        return stream.ToArray();
    }
}
