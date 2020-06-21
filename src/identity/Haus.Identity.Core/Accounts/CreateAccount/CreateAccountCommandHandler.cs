using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Common.Messaging.Commands;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace Haus.Identity.Core.Accounts.CreateAccount
{
    public class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, CreateAccountResult>
    {
        private readonly UserManager<HausUser> _userManager;

        public CreateAccountCommandHandler(UserManager<HausUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateAccountResult> Handle(CreateAccountCommand command, CancellationToken cancellationToken = default)
        {
            var hausUser = command.ToUser();
            var result = await _userManager.CreateAsync(hausUser, command.Password);
            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(hausUser, new Claim(JwtClaimTypes.Role, command.Role));
                return CreateAccountResult.Success(hausUser.Id);
            }

            var errors = result.Errors
                .Select(e => e.Description)
                .ToArray();
            return CreateAccountResult.Failed(errors);
        }
    }
}