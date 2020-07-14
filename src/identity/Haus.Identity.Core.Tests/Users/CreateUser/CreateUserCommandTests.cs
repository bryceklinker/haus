using FluentAssertions;
using Haus.Identity.Core.Users.CreateUser;
using Xunit;

namespace Haus.Identity.Core.Tests.Users.CreateUser
{
    public class CreateUserCommandTests
    {
        [Fact]
        public void WhenConvertedToUserThenReturnsUserWithUsername()
        {
            var command = new CreateUserCommand("jack", null);

            command.ToUser().UserName.Should().Be("jack");
        }
        
        [Fact]
        public void WhenConvertedToUserThenReturnsUserWithConfirmedEmail()
        {
            var command = new CreateUserCommand("bill", "1231");

            command.ToUser().EmailConfirmed.Should().BeTrue();
        }
    }
}