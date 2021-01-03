using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Commands;
using Haus.Core.Models.Lighting;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Lighting.Commands
{
    public class UpdateDefaultLightingConstraintsCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public UpdateDefaultLightingConstraintsCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenUpdatingDefaultConstraintsThenDefaultConstraintsUpdatedToMatchModel()
        {
            _context.AddDefaultLightingConstraints();
            
            await _hausBus.ExecuteCommandAsync(new UpdateDefaultLightingConstraintsCommand(new LightingConstraintsModel(7, 90, 12, 100)));

            var updated = _context.Set<DefaultLightingConstraintsEntity>().Single();
            updated.Constraints.Should().BeEquivalentTo(new LightingConstraintsEntity
            {
                MinBrightnessValue = 7,
                MaxBrightnessValue = 90,
                MinTemperature = 12,
                MaxTemperature = 100
            });
        }

        [Fact]
        public async Task WhenModelContainsInvalidConstraintsThenThrowsValidationException()
        {
            _context.AddDefaultLightingConstraints(10, 90);

            var command = new UpdateDefaultLightingConstraintsCommand(new LightingConstraintsModel(90, 10));
            Func<Task> act = () => _hausBus.ExecuteCommandAsync(command);

            await act.Should().ThrowAsync<HausValidationException>();
        }
    }
}