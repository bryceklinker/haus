using System;
using System.Linq;
using System.Threading.Tasks;
using Haus.Identity.Models.ApiResources;
using Haus.Identity.Web.ApiResources.Commands;
using Haus.Identity.Web.Tests.Support;
using Haus.Testing.Utilities.ServiceBus;
using IdentityServer4.EntityFramework.DbContexts;
using Xunit;

namespace Haus.Identity.Web.Tests.ApiResources.Commands
{
    public class CreateApiResourceCommandHandlerTests
    {
        private readonly ConfigurationDbContext _context;
        private readonly FakeHausServiceBusPublisher _publisher;
        private readonly CreateApiResourceCommandHandler _handler;

        public CreateApiResourceCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreateConfigurationContext();
            _publisher = new FakeHausServiceBusPublisher();
            _handler = new CreateApiResourceCommandHandler(_context, _publisher);
        }

        [Fact]
        public async Task WhenApiResourceCreatedThenResourceIsAddedToDatabase()
        {
            await _handler.Handle(new CreateApiResourceCommand("one", Array.Empty<string>()));

            Assert.Single(_context.ApiResources);
        }

        [Fact]
        public async Task WhenApiResourceCreatedThenApiScopesAreAddedToDatabase()
        {
            await _handler.Handle(new CreateApiResourceCommand("one", new[] {"one", "five"}));

            Assert.Equal(2, _context.ApiScopes.Count());
        }

        [Fact]
        public async Task WhenApiResourceCreatedThenApiResourceCreatedIsPublished()
        {
            await _handler.Handle(new CreateApiResourceCommand("idk", Array.Empty<string>()));

            Assert.Single(_publisher.GetMessages<ApiResourceCreatedPayload>());
        }
    }
}