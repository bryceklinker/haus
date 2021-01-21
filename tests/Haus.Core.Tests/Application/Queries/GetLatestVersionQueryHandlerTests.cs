using System;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Application;
using Haus.Core.Application.Queries;
using Haus.Cqrs;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Core.Tests.Application.Queries
{
    public class GetLatestVersionQueryHandlerTests
    {
        private readonly FakeLatestReleaseProvider _latestReleaseProvider;
        private readonly IHausBus _hausBus;

        public GetLatestVersionQueryHandlerTests()
        {
            _latestReleaseProvider = new FakeLatestReleaseProvider();
            _hausBus = HausBusFactory.Create(configureServices:
                services => services.AddSingleton<ILatestReleaseProvider>(_latestReleaseProvider)
            );
        }

        [Fact]
        public async Task WhenGettingLatestVersionThenReturnsVersionIsNewer()
        {
            var release = new ReleaseModel(Version.Parse("999.999.999"), true, DateTimeOffset.Now, "Big Release");
            _latestReleaseProvider.SetupLatestVersion(release);

            var version = await _hausBus.ExecuteQueryAsync(new GetLatestVersionQuery());

            version.Version.Should().Be("999.999.999");
            version.CreationDate.Should().Be(release.CreationDateTime);
            version.IsOfficialRelease.Should().BeTrue();
            version.IsNewer.Should().BeTrue();
            version.Description.Should().Be("Big Release");
        }

        [Fact]
        public async Task WhenGettingLatestVersionFailsThenReturnsEmptyVersion()
        {
            _latestReleaseProvider.SetupFailure(new Exception());

            var version = await _hausBus.ExecuteQueryAsync(new GetLatestVersionQuery());

            version.Version.Should().Be("0.0.0");
            version.CreationDate.Should().Be(DateTimeOffset.MinValue);
            version.IsNewer.Should().BeFalse();
            version.IsOfficialRelease.Should().BeFalse();
            version.Description.Should().BeEmpty();
        }
    }
}