using System;
using System.Threading.Tasks;
using Haus.Portal.Web.Common.Storage;
using Haus.Portal.Web.Settings.Entities;
using Haus.Portal.Web.Settings.Queries;
using Haus.Portal.Web.Tests.Support;
using IdentityModel;
using Xunit;

namespace Haus.Portal.Web.Tests.Settings.Queries
{
    public class GetSettingsQueryHandlerTests
    {
        private readonly HausPortalDbContext _context;
        private readonly GetSettingsQueryHandler _handler;

        public GetSettingsQueryHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreatePortalContext();
            
            _handler = new GetSettingsQueryHandler(_context);
        }

        [Fact]
        public async Task WhenSettingsIsEmptyThenReturnsNull()
        {
            var settings = await _handler.Handle(new GetSettingsQuery());

            Assert.Null(settings);
        }

        [Fact]
        public async Task WhenSettingsAreAvailableThenGetsAuthSettingsFromDatabase()
        {
            AddAuthSettings("my-client-id", "https://authority.com");

            var settings = await _handler.Handle(new GetSettingsQuery());

            Assert.Equal("my-client-id", settings.ClientId);
            Assert.Equal("https://authority.com", settings.Authority);
        }

        [Fact]
        public async Task WhenSettingsAreAvailableThenResponseTypeIsCode()
        {
            AddAuthSettings();
            
            var settings = await _handler.Handle(new GetSettingsQuery());

            Assert.Equal(OidcConstants.ResponseTypes.Code, settings.ResponseType);
        }

        private void AddAuthSettings(string clientId = null, string authority = null)
        {
            var settings = new AuthSettings
            {
                Authority = authority,
                ClientId = clientId
            };
            _context.Add(settings);
            _context.SaveChanges();
        }
    }
}