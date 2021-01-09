using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Commands;
using Haus.Core.Lighting.Entities;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Lighting.Commands
{
    public class InitializeDefaultLightingConstraintsHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public InitializeDefaultLightingConstraintsHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenInitializedThenDefaultLightingConstraintsIsAddedToDb()
        {
            await _hausBus.ExecuteCommandAsync(new InitializeDefaultLightingSettingsCommand());

            _context.Set<DefaultLightingConstraintsEntity>().Should().HaveCount(1)
                .And.ContainEquivalentOf(new DefaultLightingConstraintsEntity
                {
                    Constraints = new LightingConstraintsEntity()
                }, opts => opts.Excluding(c => c.Id));
        }

        [Fact]
        public async Task WhenDefaultsExistAndInitializedThenNothingIsChanged()
        {
            var existing = _context.AddDefaultLightingConstraints(9, 9, 9, 9);

            await _hausBus.ExecuteCommandAsync(new InitializeDefaultLightingSettingsCommand());

            _context.Set<DefaultLightingConstraintsEntity>().Should().HaveCount(1)
                .And.ContainEquivalentOf(existing);
        }
    }
}