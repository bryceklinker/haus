using Haus.Identity.Core.Users.CreateUser;

namespace Haus.Identity.Web.Users.ViewModels
{
    public class CreateUserViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public CreateUserCommand ToCommand()
        {
            return new CreateUserCommand(Username, Password, Role);
        }
    }
}