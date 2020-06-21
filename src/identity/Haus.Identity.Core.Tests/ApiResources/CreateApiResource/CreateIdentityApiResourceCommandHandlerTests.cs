using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.ApiResources.CreateApiResource;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Tests.Support;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Identity.Core.Tests.ApiResources.CreateApiResource
{
    public class CreateIdentityApiResourceCommandHandlerTests
    {
        private readonly ConfigurationDbContext _context;
        private readonly IConfiguration _configuration;

        public CreateIdentityApiResourceCommandHandlerTests()
        {
            _configuration = InMemoryConfigurationFactory.CreateEmpty();
            _context = InMemoryDbContextFactory.CreateConfigurationDbContext();
        }
        
        [Fact]
        public async Task WhenIdentityApiResourceExistsThenIdentityApiResourceIsNotAdded()
        {
            _context.Add(new ApiResource(_configuration.IdentityApiScope(), _configuration.IdentityApiName()).ToEntity());
            await _context.SaveChangesAsync();

            await Handle(new CreateApiResourceCommand());

            _context.ApiResources.Should().HaveCount(1);
        }
        
        [Fact]
        public async Task WhenIdentityApiResourceDoesNotExistThenIdentityApiResourceIsAdded()
        {
            await Handle(new CreateApiResourceCommand());

            var resource = _context.ApiResources.Single();
            resource.Name.Should().Be(_configuration.IdentityApiScope());
            resource.DisplayName.Should().Be(_configuration.IdentityApiName());
            resource.Scopes.Should().Contain(s => s.Name == _configuration.IdentityApiScope());
        }
        
        [Fact]
        public async Task WhenIdentityApiResourceIsSeededThenIdentityApiResourceIsAddedToApiResources()
        {
            await Handle(new CreateApiResourceCommand());

            _context.ApiResources.Should().HaveCount(1);
        }

        private async Task Handle(CreateApiResourceCommand command)
        {
            var messageBus = ServiceProviderFactory.CreateProvider(opts =>
            {
                opts.WithConfiguration(_configuration)
                    .WithConfigurationDb(_context);
            }).GetRequiredService<IMessageBus>();
            await messageBus.ExecuteCommand(command);
        }
    }
}