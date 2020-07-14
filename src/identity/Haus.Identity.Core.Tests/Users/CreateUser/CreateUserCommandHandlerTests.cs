using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.Tests.Support;
using Haus.Identity.Core.Users.CreateUser;
using Haus.Identity.Core.Users.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Haus.Identity.Core.Tests.Users.CreateUser
{
    public class CreateUserCommandHandlerTests
    {
        private const string GoodPassword = "abc123XYZ$";
        private readonly UserManager<HausUser> _userManager;

        public CreateUserCommandHandlerTests()
        {
            _userManager = InMemoryUserManagerFactory.Create();
        }

        [Fact]
        public async Task WhenAccountCreatedSuccessfullyThenReturnsUserId()
        {
            var request = new CreateUserCommand("bill", GoodPassword);
            
            var result = await Handle(request);

            result.WasSuccessful.Should().BeTrue();
            _userManager.Users.Select(u => u.Id).Should().Contain(result.Id);
        }

        [Fact]
        public async Task WhenAccountCreatedWithoutRoleThenDefaultUserRoleClaimIsAddedToUser()
        {
            var request = new CreateUserCommand("bill", GoodPassword);

            await Handle(request);
            
            var user = await _userManager.FindByNameAsync("bill");
            var claims = await _userManager.GetClaimsAsync(user);
            claims.Should().Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "user");
        }
        
        [Fact]
        public async Task WhenAccountCreatedWithSpecificRoleThenRoleClaimIsAddedToNewUser()
        {
            var request = new CreateUserCommand("bill", GoodPassword, "admin");

            await Handle(request);

            var user = await _userManager.FindByNameAsync("bill");
            var claims = await _userManager.GetClaimsAsync(user);
            claims.Should().Contain(c => c.Type == JwtClaimTypes.Role && c.Value == "admin");
        }
        
        [Fact]
        public async Task WhenPasswordProvidedIsInsufficientThenReturnsErrorMessages()
        {
            var request = new CreateUserCommand("bill", "notgoodenough");

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
            var request = new CreateUserCommand("bill", GoodPassword);

            await Handle(request);
            var result = await Handle(request);

            result.WasSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.Should().ContainMatch("*already taken*");
        }

        private async Task<CreateUserResult> Handle(CreateUserCommand request)
        {
            var messageBus = MessageBusFactory.Create(opts =>
                {
                    opts.WithUserManager(_userManager);
                });
            return await messageBus.ExecuteCommand(request);
        }
    }
}