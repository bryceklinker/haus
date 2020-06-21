using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.Accounts.CreateAccount;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Tests.Support;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Identity.Core.Tests.Accounts.CreateAccount
{
    public class CreateAccountCommandHandlerTests
    {
        private const string GoodPassword = "abc123XYZ$";
        private readonly UserManager<HausUser> _userManager;

        public CreateAccountCommandHandlerTests()
        {
            _userManager = InMemoryUserManagerFactory.Create();
        }

        [Fact]
        public async Task WhenAccountCreatedSuccessfullyThenReturnsUserId()
        {
            var request = new CreateAccountCommand("bill", GoodPassword);
            
            var result = await Handle(request);

            result.WasSuccessful.Should().BeTrue();
            _userManager.Users.Select(u => u.Id).Should().Contain(result.Id);
        }

        [Fact]
        public async Task WhenAccountCreatedWithoutRoleThenDefaultUserRoleClaimIsAddedToUser()
        {
            var request = new CreateAccountCommand("bill", GoodPassword);

            await Handle(request);
            
            var user = await _userManager.FindByNameAsync("bill");
            var claims = await _userManager.GetClaimsAsync(user);
            claims.Should().Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "user");
        }
        
        [Fact]
        public async Task WhenAccountCreatedWithSpecificRoleThenRoleClaimIsAddedToNewUser()
        {
            var request = new CreateAccountCommand("bill", GoodPassword, "admin");

            await Handle(request);

            var user = await _userManager.FindByNameAsync("bill");
            var claims = await _userManager.GetClaimsAsync(user);
            claims.Should().Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "admin");
        }
        
        [Fact]
        public async Task WhenPasswordProvidedIsInsufficientThenReturnsErrorMessages()
        {
            var request = new CreateAccountCommand("bill", "notgoodenough");

            var result = await Handle(request);

            result.WasSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(3);
            result.Errors.Should().ContainMatch("*one non alphanumeric*");
            result.Errors.Should().ContainMatch("*one digit*");
            result.Errors.Should().ContainMatch("*one uppercase*");
        }

        [Fact]
        public async Task WhenUsernameAlreadyExistsThenReturnsErrorMessage()
        {
            var request = new CreateAccountCommand("bill", GoodPassword);

            await Handle(request);
            var result = await Handle(request);

            result.WasSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().ContainMatch("*already taken*");
        }

        private async Task<CreateAccountResult> Handle(CreateAccountCommand request)
        {
            var messageBus = ServiceProviderFactory.CreateProvider(opts =>
                {
                    opts.WithUserManager(_userManager);
                })
                .GetRequiredService<IMessageBus>();
            return await messageBus.ExecuteCommand(request);
        }
    }
}