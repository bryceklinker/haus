using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Lighting.Queries;
using Haus.Core.Models.Lighting;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Lighting.Queries
{
    public class GetDefaultLightingSettingsQueryHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public GetDefaultLightingSettingsQueryHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenGettingDefaultSettingsThenReturnsDefaultLightingSettings()
        {
            _context.AddDefaultLightingConstraints(9, 98, 3000, 4500);
            
            var constraints = await _hausBus.ExecuteQueryAsync(new GetDefaultLightingConstraintsQuery());

            constraints.Should().BeEquivalentTo(new LightingConstraintsModel(9, 98, 3000, 4500));
        }

        [Fact]
        public async Task WhenDefaultsAreMissingThenReturnsNull()
        {
            var constraints = await _hausBus.ExecuteQueryAsync(new GetDefaultLightingConstraintsQuery());

            constraints.Should().BeNull();
        }
    }
}