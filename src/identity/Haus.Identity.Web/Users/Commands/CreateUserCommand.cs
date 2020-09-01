using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Identity.Web.Users.Entities;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Haus.Identity.Web.Users.Commands
{
    public class CreateUserCommand : ICommand<CreateUserResult>
    {
        public string Password { get; }
        public string Username { get; }

        public CreateUserCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public HausUser ToUser()
        {
            return new HausUser
            {
                UserName = Username
            };
        }
    }

    public class CreateUserResult
    {
        public bool WasSuccessful => Errors.IsNullOrEmpty();

        public string[] Errors { get; }

        public CreateUserResult(IEnumerable<string> errors)
        {
            Errors = errors.ToArray();
        }
    }
    
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly UserManager<HausUser> _userManager;

        public CreateUserCommandHandler(UserManager<HausUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.CreateAsync(request.ToUser(), request.Password);

            return new CreateUserResult(result.Errors.Select(e => e.Description));
        }
    }
}