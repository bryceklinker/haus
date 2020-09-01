using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Web.IdentityResources.Commands;
using Haus.Identity.Web.Tests.Support;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Xunit;

namespace Haus.Identity.Web.Tests.IdentityResources.Commands
{
    public class CreateDefaultIdentityResourcesCommandHandlerTests
    {
        private readonly ConfigurationDbContext _context;
        private readonly CreateDefaultIdentityResourcesCommandHandler _handler;

        public CreateDefaultIdentityResourcesCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreateConfigurationContext();
            _handler = new CreateDefaultIdentityResourcesCommandHandler(_context);
        }

        [Fact]
        public async Task WhenDefaultIdentityResourcesAreAddedThenProfileResourceIsAdded()
        {
            await _handler.Handle(new CreateDefaultIdentityResourcesCommand());
            
            Assert.Contains(_context.IdentityResources, resource => resource.Name == OidcConstants.StandardScopes.Profile);
        }
        
        [Fact]
        public async Task WhenDefaultIdentityResourcesAreAddedThenOpenIdResourceIsAdded()
        {
            await _handler.Handle(new CreateDefaultIdentityResourcesCommand());
            
            Assert.Contains(_context.IdentityResources, resource => resource.Name == OidcConstants.StandardScopes.OpenId);
        }

        [Fact]
        public async Task WhenDefaultResourcesAreAlreadyAddedThenNoNewResourcesAreAdded()
        {
            _context.Add(new IdentityResource {Name = OidcConstants.StandardScopes.Profile});
            _context.Add(new IdentityResource {Name = OidcConstants.StandardScopes.OpenId});
            await _context.SaveChangesAsync();

            await _handler.Handle(new CreateDefaultIdentityResourcesCommand());

            Assert.Equal(2, _context.IdentityResources.Count());
        }
    }
}