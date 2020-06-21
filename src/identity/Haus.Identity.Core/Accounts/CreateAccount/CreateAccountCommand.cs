using Haus.Cqrs.Commands;
using Haus.Identity.Core.Accounts.Entities;
using MediatR;

namespace Haus.Identity.Core.Accounts.CreateAccount
{
    public class CreateAccountCommand : ICommand<CreateAccountResult>
    {
        public string Username { get; }
        public string Password { get; }
        public string Role { get; }

        public CreateAccountCommand(string username, string password, string role = AccountDefaults.DefaultUserRole)
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