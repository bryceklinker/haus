using Haus.Cqrs.Commands;
using Haus.Identity.Core.Users.Entities;

namespace Haus.Identity.Core.Users.CreateUser
{
    public class CreateUserCommand : ICommand<CreateUserResult>
    {
        public string Username { get; }
        public string Password { get; }
        public string Role { get; }

        public CreateUserCommand(string username, string password, string role = UserDefaults.DefaultUserRole)
        {
            Username = username;
            Password = password;
            Role = role;
        }

        public HausUser ToUser()
        {
            return new HausUser
            {
                UserName = Username,
                EmailConfirmed = true
            };
        }
    }
}