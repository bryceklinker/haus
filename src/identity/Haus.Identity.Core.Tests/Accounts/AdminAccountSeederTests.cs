using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.Accounts;
using Haus.Identity.Core.Accounts.Models;
using Haus.Identity.Core.Tests.Support;
using MediatR;
using Xunit;

namespace Haus.Identity.Core.Tests.Accounts
{
    public class AdminAccountSeederTests
    {
        [Fact]
        public void WhenAdminAccountExistsThenShouldNotCreateAdminAccount()
        {
            const string adminUsername = "jack";
            var existingUsernames = new[] {"bill", "jack"};
            
            AdminAccountSeeder.ShouldCreateAdminAccount(adminUsername, existingUsernames)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void WhenAdminAccountIsMissingThenShouldCreateAdminAccount()
        {
            const string adminUsername = "three";
            var existingUsernames = new[] {"bill", "jack"};
            
            AdminAccountSeeder.ShouldCreateAdminAccount(adminUsername, existingUsernames)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void WhenAdminAccountHasDifferentCasingThenShouldNotCreateAdminAccount()
        {
            const string adminUsername = "JACK";
            var existingUsernames = new[] {"bill", "jack"};
            
            AdminAccountSeeder.ShouldCreateAdminAccount(adminUsername, existingUsernames)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void WhenAdminAccountCreatedThenAdminAccountIsReadyToBeUsed()
        {
            var adminUser = AdminAccountSeeder.CreateAdminAccount("bill");

            adminUser.UserName.Should().Be("bill");
            adminUser.EmailConfirmed.Should().BeTrue();
        }

        [Fact]
        public async Task WhenAdminAccountSeedingIsRequestedThenAdminAccountIsAddedToDatabase()
        {
            var config = InMemoryConfigurationFactory.CreateEmpty();
            var userManager = InMemoryUserManagerFactory.Create();

            IRequestHandler<SeedAdminAccountRequest> handler = new AdminAccountSeeder(userManager, config);
            await handler.Handle(new SeedAdminAccountRequest(), CancellationToken.None);

            userManager.Users.Should().HaveCount(1);
        }
    }
}