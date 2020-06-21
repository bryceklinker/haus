using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.Accounts.CreateAccount;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Tests.Support;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Identity.Core.Tests.Accounts.CreateAccount
{
    public class CreateAdminAccountHandlerTests
    {
        private readonly UserManager<HausUser> _userManager;
        private readonly IConfiguration _configuration;

        public CreateAdminAccountHandlerTests()
        {
            _userManager = InMemoryUserManagerFactory.Create();
            _configuration = InMemoryConfigurationFactory.CreateEmpty();
        }
        
        [Fact]
        public async Task WhenAdminAccountExistsThenShouldNotCreateAdminAccount()
        {
            await _userManager.CreateAsync(new HausUser
            {
                UserName = _configuration.AdminUsername()
            }, _configuration.AdminPassword());

            await Handle(new CreateAdminAccountCommand());

            _userManager.Users.Should().HaveCount(1);
        }

        [Fact]
        public async Task WhenAdminAccountIsMissingThenShouldCreateAdminAccount()
        {
            await Handle(new CreateAdminAccountCommand());

            _userManager.Users.Should().Contain(u => u.UserName == _configuration.AdminUsername());
        }

        [Fact]
        public async Task WhenAdminAccountIsMissingThenAdminUserIsAssignedAdminRoleClaim()
        {
            await Handle(new CreateAdminAccountCommand());

            var adminUser = await _userManager.FindByNameAsync(_configuration.AdminUsername());
            (await _userManager.GetClaimsAsync(adminUser)).Should()
                .Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "admin");
        }
        
        private async Task Handle(CreateAdminAccountCommand command)
        {
            var messageBus = ServiceProviderFactory.CreateProvider(opts =>
            {
                opts.WithUserManager(_userManager)
                    .WithConfiguration(_configuration);
            }).GetRequiredService<IMessageBus>();
            await messageBus.ExecuteCommand(command);
        }
    }
}