using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Identity.Core.Users.Entities;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace Haus.Identity.Core.Users.CreateUser
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly UserManager<HausUser> _userManager;

        public CreateUserCommandHandler(UserManager<HausUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
        {
            var hausUser = command.ToUser();
            var result = await _userManager.CreateAsync(hausUser, command.Password);
            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(hausUser, new Claim(JwtClaimTypes.Role, command.Role));
                return CreateUserResult.Success(hausUser.Id);
            }

            var errors = result.Errors
                .Select(e => e.Description)
                .ToArray();
            return CreateUserResult.Failed(errors);
        }
    }
}