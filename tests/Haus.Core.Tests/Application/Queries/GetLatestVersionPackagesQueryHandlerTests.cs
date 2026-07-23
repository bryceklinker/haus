using System;
using System.Threading.Tasks;
using Haus.Core.Application;
using Haus.Core.Application.Queries;
using Haus.Core.Models.Application;
using Haus.Cqrs;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Core.Tests.Application.Queries;

public class GetLatestVersionPackagesQueryHandlerTests
{
    private readonly FakeLatestReleaseProvider _latestReleaseProvider;
    private readonly IHausBus _hausBus;

    public GetLatestVersionPackagesQueryHandlerTests()
    {
        _latestReleaseProvider = new FakeLatestReleaseProvider();
        _hausBus = HausBusFactory.Create(configureServices: services =>
            services.AddSingleton<ILatestReleaseProvider>(_latestReleaseProvider)
        );
    }

    [Fact]
    public async Task WhenGettingLatestReleasePackagesThenReturnsPackagesForLatestRelease()
    {
        _latestReleaseProvider.SetupLatestPackages(
            new ReleasePackageModel(5, "idk"),
            new ReleasePackageModel(9, "hello"),
            new ReleasePackageModel(7, "something")
        );

        var result = await _hausBus.ExecuteQueryAsync(new GetLatestVersionPackagesQuery());

        Assert.Equal(3, result.Count);
        Assert.Equal(3, result.Items.Length);
        Assert.Contains(new ApplicationPackageModel(5, "idk"), result.Items);
        Assert.Contains(new ApplicationPackageModel(9, "hello"), result.Items);
        Assert.Contains(new ApplicationPackageModel(7, "something"), result.Items);
    }

    [Fact]
    public async Task WhenGettingLatestReleasePackagesThenReturnsPackagesInAlphabeticalOrder()
    {
        _latestReleaseProvider.SetupLatestPackages(
            new ReleasePackageModel(5, "b"),
            new ReleasePackageModel(9, "c"),
            new ReleasePackageModel(7, "a")
        );

        var result = await _hausBus.ExecuteQueryAsync(new GetLatestVersionPackagesQuery());

        Assert.Equal("a", result.Items[0].Name);
        Assert.Equal("b", result.Items[1].Name);
        Assert.Equal("c", result.Items[2].Name);
    }

    [Fact]
    public async Task WhenGettingLatestReleasePackagesFailsThenReturnsEmptyResult()
    {
        _latestReleaseProvider.SetupFailure(new Exception());

        var result = await _hausBus.ExecuteQueryAsync(new GetLatestVersionPackagesQuery());

        Assert.Equal(0, result.Count);
        Assert.Empty(result.Items);
    }
}
