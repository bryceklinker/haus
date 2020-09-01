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
            
            _handler = new GetSettingsQueryHandler(_context, InMemoryConfiguration.Get());
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
            AddAuthSettings("my-client-id");

            var settings = await _handler.Handle(new GetSettingsQuery());

            Assert.Equal("my-client-id", settings.ClientId);
            Assert.Equal(InMemoryConfiguration.AuthorityUrl, settings.Authority);
        }

        [Fact]
        public async Task WhenSettingsAreAvailableThenScopesComeFromAuthSettingsConstants()
        {
            AddAuthSettings("my-client-id");

            var settings = await _handler.Handle(new GetSettingsQuery());

            Assert.Contains(AuthSettings.Scopes[0], settings.Scope);
            Assert.Contains(AuthSettings.Scopes[1], settings.Scope);
        }

        [Fact]
        public async Task WhenSettingsAreAvailableThenScopesIncludeDefaultScopes()
        {
            AddAuthSettings("idk");

            var settings = await _handler.Handle(new GetSettingsQuery());

            Assert.Contains(OidcConstants.StandardScopes.Profile, settings.Scope);
            Assert.Contains(OidcConstants.StandardScopes.OpenId, settings.Scope);
        }

        [Fact]
        public async Task WhenSettingsAreAvailableThenResponseTypeIsCode()
        {
            AddAuthSettings();
            
            var settings = await _handler.Handle(new GetSettingsQuery());

            Assert.Equal(OidcConstants.ResponseTypes.Code, settings.ResponseType);
        }

        private void AddAuthSettings(string clientId = null)
        {
            var settings = new AuthSettings
            {
                ClientId = clientId
            };
            _context.Add(settings);
            _context.SaveChanges();
        }
    }
}