using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Models.Clients;
using Haus.Portal.Web.Common.Storage;
using Haus.Portal.Web.Settings.Commands;
using Haus.Portal.Web.Settings.Entities;
using Haus.Portal.Web.Tests.Support;
using Xunit;

namespace Haus.Portal.Web.Tests.Settings.Commands
{
    public class UpdateClientSettingsCommandHandlerTests
    {
        private readonly HausPortalDbContext _context;
        private readonly UpdateClientSettingsCommandHandler _handler;

        public UpdateClientSettingsCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreatePortalContext();
            _handler = new UpdateClientSettingsCommandHandler(_context);
        }

        [Fact]
        public async Task WhenNoAuthSettingsExistThenAuthSettingsAreAddedWithClientSettingsPopulated()
        {
            var settings = new ClientCreatedPayload(AuthSettings.ClientName, "my-new-client-id", "i-have-a-secret");
            await _handler.Handle(new UpdateClientSettingsCommand(settings));

            var authSettings = _context.Set<AuthSettings>().Single();
            Assert.Equal("my-new-client-id", authSettings.ClientId);
            Assert.Equal("i-have-a-secret", authSettings.ClientSecret);
        }

        [Fact]
        public async Task WhenAuthSettingsExistThenClientSettingsAreOverwritten()
        {
            AddAuthSettings("old-id", "old-secret");
            
            var settings = new ClientCreatedPayload(AuthSettings.ClientName, "my-new-client-id", "i-have-a-secret");
            await _handler.Handle(new UpdateClientSettingsCommand(settings));

            var authSettings = _context.Set<AuthSettings>().Single();
            Assert.Equal("my-new-client-id", authSettings.ClientId);
            Assert.Equal("i-have-a-secret", authSettings.ClientSecret);
        }

        [Fact]
        public async Task WhenClientPayloadIsForADifferentClientThenAuthSettingsAreNotUpdated()
        {
            AddAuthSettings("old-id", "old-secret");

            var settings = new ClientCreatedPayload("not-correct", "", "");
            await _handler.Handle(new UpdateClientSettingsCommand(settings));

            var authSettings = _context.Set<AuthSettings>().Single();
            Assert.Equal("old-id", authSettings.ClientId);
            Assert.Equal("old-secret", authSettings.ClientSecret);
        }
        
        private void AddAuthSettings(string clientId = null, string clientSecret = null)
        {
            _context.Add(new AuthSettings
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            });
            _context.SaveChanges();
        }
    }
}