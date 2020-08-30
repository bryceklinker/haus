using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Models.Settings;
using Haus.Portal.Web.Common.Storage;
using Haus.Portal.Web.Settings.Commands;
using Haus.Portal.Web.Settings.Entities;
using Haus.Portal.Web.Tests.Support;
using Xunit;

namespace Haus.Portal.Web.Tests.Settings.Commands
{
    public class UpdateAuthSettingsCommandHandlerTests
    {
        private readonly HausPortalDbContext _context;
        private readonly UpdateAuthSettingsCommandHandler _handler;

        public UpdateAuthSettingsCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreatePortalContext();
            _handler = new UpdateAuthSettingsCommandHandler(_context);
        }

        [Fact]
        public async Task WhenAuthSettingsAreUpdatedThenAuthSettingsAreAddedToDatabase()
        {
            await _handler.Handle(new UpdateAuthSettingsCommand(new AuthoritySettingsPayload("https://something.com")));

            var settings = _context.Set<AuthSettings>().Single();
            Assert.Equal("https://something.com", settings.Authority);
        }

        [Fact]
        public async Task WhenAuthSettingsExistAndNewSettingsAreReceivedThenAuthorityIsUpdated()
        {
            _context.Add(new AuthSettings {Authority = "https://localhost"});
            await _context.SaveChangesAsync();

            await _handler.Handle(new UpdateAuthSettingsCommand(new AuthoritySettingsPayload("https://google.com")));

            var settings = _context.Set<AuthSettings>().Single();
            Assert.Equal("https://google.com", settings.Authority);
        }
    }
}