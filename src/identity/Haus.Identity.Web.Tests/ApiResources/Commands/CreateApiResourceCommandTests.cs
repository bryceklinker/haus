using System;
using Haus.Identity.Web.ApiResources.Commands;
using Xunit;

namespace Haus.Identity.Web.Tests.ApiResources.Commands
{
    public class CreateApiResourceCommandTests
    {
        [Fact]
        public void WhenConvertedToApiResourceThenNameAndScopesArePopulatedFromCommand()
        {
            var resource = new CreateApiResourceCommand("some-identifier", new []{"some-identifier/read", "some-identifier/write"})
                .ToApiResource();

            Assert.Equal("some-identifier", resource.Name);
            Assert.Contains("some-identifier/read", resource.Scopes);
            Assert.Contains("some-identifier/write", resource.Scopes);
        }

        [Fact]
        public void WhenConvertedToApiResourceMissingDisplayNameThenIdentifierIsUsedAsDisplayName()
        {
            var resource = new CreateApiResourceCommand("hello", Array.Empty<string>())
                .ToApiResource();

            Assert.Equal("hello", resource.DisplayName);
        }

        [Fact]
        public void WhenConvertedToApiResourceThenDisplayNameIsPopulatedFromCommand()
        {
            var resource = new CreateApiResourceCommand("nope", Array.Empty<string>(), "Portal")
                .ToApiResource();

            Assert.Equal("Portal", resource.DisplayName);
        }

        [Fact]
        public void WhenConvertedToApiScopesThenScopesAreMappedToApiScopes()
        {
            var apiScopes = new CreateApiResourceCommand("", new[] {"hello", "buddy"})
                .ToApiScopes();

            Assert.Contains(apiScopes, scope => scope.Name == "hello");
            Assert.Contains(apiScopes, scope => scope.Name == "buddy");
        }
    }
}