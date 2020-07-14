using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.Tests.Support;
using Haus.Identity.Core.Users.CreateUser;
using Haus.Identity.Core.Users.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Haus.Identity.Core.Tests.Users.CreateUser
{
    public class CreateAdminUserHandlerTests
    {
        private readonly UserManager<HausUser> _userManager;
        private readonly IConfiguration _configuration;

        public CreateAdminUserHandlerTests()
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

            await Handle(new CreateAdminUserCommand());

            _userManager.Users.Should().HaveCount(1);
        }

        [Fact]
        public async Task WhenAdminAccountIsMissingThenShouldCreateAdminAccount()
        {
            await Handle(new CreateAdminUserCommand());

            _userManager.Users.Should().Contain(u => u.UserName == _configuration.AdminUsername());
        }

        [Fact]
        public async Task WhenAdminAccountIsMissingThenAdminUserIsAssignedAdminRoleClaim()
        {
            await Handle(new CreateAdminUserCommand());

            var adminUser = await _userManager.FindByNameAsync(_configuration.AdminUsername());
            (await _userManager.GetClaimsAsync(adminUser)).Should()
                .Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "admin");
        }
        
        private async Task Handle(CreateAdminUserCommand command)
        {
            var messageBus = MessageBusFactory.Create(opts =>
            {
                opts.WithUserManager(_userManager)
                    .WithConfiguration(_configuration);
            });
            await messageBus.ExecuteCommand(command);
        }
    }
}