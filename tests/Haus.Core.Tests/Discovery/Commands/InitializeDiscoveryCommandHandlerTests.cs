using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Discovery.Commands;
using Haus.Core.Discovery.Entities;
using Haus.Core.Models.Discovery;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Discovery.Commands
{
    public class InitializeDiscoveryCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public InitializeDiscoveryCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenDiscoveryIsMissingThenDiscoveryIsAddedToDatabase()
        {
            await _hausBus.ExecuteCommandAsync(new InitializeDiscoveryCommand());

            _context.Set<DiscoveryEntity>().Should().HaveCount(1)
                .And.ContainEquivalentOf(new DiscoveryEntity(0, DiscoveryState.Disabled),
                    opts => opts.Excluding(d => d.Id)
                );
        }

        [Fact]
        public async Task WhenDiscoveryExistsThenDiscoveryIsLeftAlone()
        {
            _context.AddDiscovery(DiscoveryState.Enabled);

            await _hausBus.ExecuteCommandAsync(new InitializeDiscoveryCommand());

            _context.Set<DiscoveryEntity>().Should().HaveCount(1)
                .And.ContainEquivalentOf(new DiscoveryEntity(0, DiscoveryState.Enabled),
                    opts => opts.Excluding(d => d.Id)
                );
        }
    }
}