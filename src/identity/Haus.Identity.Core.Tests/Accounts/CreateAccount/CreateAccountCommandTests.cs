using FluentAssertions;
using Haus.Identity.Core.Accounts.CreateAccount;
using Xunit;

namespace Haus.Identity.Core.Tests.Accounts.CreateAccount
{
    public class CreateAccountCommandTests
    {
        [Fact]
        public void WhenConvertedToUserThenReturnsUserWithUsername()
        {
            var command = new CreateAccountCommand("jack", null);

            command.ToUser().UserName.Should().Be("jack");
        }
        
        [Fact]
        public void WhenConvertedToUserThenReturnsUserWithConfirmedEmail()
        {
            var command = new CreateAccountCommand("bill", "1231");

            command.ToUser().EmailConfirmed.Should().BeTrue();
        }
    }
}