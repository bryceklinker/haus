using Haus.Identity.Web.Users.Commands;
using Xunit;

namespace Haus.Identity.Web.Tests.Users.Commands
{
    public class CreateUserCommandTests
    {
        [Fact]
        public void WhenConvertedToUserThenUsernameIsPopulated()
        {
            var user = new CreateUserCommand("bill", "")
                .ToUser();
            
            Assert.Equal("bill", user.UserName);
        }
    }
}