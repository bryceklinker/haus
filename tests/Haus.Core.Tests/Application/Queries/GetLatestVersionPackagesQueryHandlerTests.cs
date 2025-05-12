using System;
using System.Threading.Tasks;
using FluentAssertions;
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

        result.Count.Should().Be(3);
        result
            .Items.Should()
            .HaveCount(3)
            .And.ContainEquivalentOf(new ApplicationPackageModel(5, "idk"))
            .And.ContainEquivalentOf(new ApplicationPackageModel(9, "hello"))
            .And.ContainEquivalentOf(new ApplicationPackageModel(7, "something"));
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

        result.Items[0].Name.Should().Be("a");
        result.Items[1].Name.Should().Be("b");
        result.Items[2].Name.Should().Be("c");
    }

    [Fact]
    public async Task WhenGettingLatestReleasePackagesFailsThenReturnsEmptyResult()
    {
        _latestReleaseProvider.SetupFailure(new Exception());

        var result = await _hausBus.ExecuteQueryAsync(new GetLatestVersionPackagesQuery());

        result.Count.Should().Be(0);
        result.Items.Should().BeEmpty();
    }
}
