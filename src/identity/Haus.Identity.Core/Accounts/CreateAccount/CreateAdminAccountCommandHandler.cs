using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Common.Messaging;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Accounts.CreateAccount
{
    public class CreateAdminAccountCommandHandler : CommandHandler<CreateAdminAccountCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly UserManager<HausUser> _userManager;

        private string AdminUsername => _configuration.AdminUsername();
        private string AdminPassword => _configuration.AdminPassword();
        
        public CreateAdminAccountCommandHandler(
            UserManager<HausUser> userManager,
            IConfiguration configuration,
            IMessageBus messageBus)
        {
            _userManager = userManager;
            _configuration = configuration;
            _messageBus = messageBus;
        }

        protected override async Task InnerHandle(CreateAdminAccountCommand command, CancellationToken cancellationToken = default)
        {
            var usernames = await _userManager.Users
                .Select(u => u.UserName)
                .ToArrayAsync(cancellationToken);
            
            if (ShouldCreateAdminAccount(AdminPassword, usernames))
                await _messageBus.ExecuteCommand(new CreateAccountCommand(AdminUsername, AdminPassword, AccountDefaults.AdminUserRole), cancellationToken);
        }

        public static bool ShouldCreateAdminAccount(string adminUsername, string[] existingUsernames)
        {
            return !existingUsernames.ContainsIgnoreCase(adminUsername);
        }

        public static HausUser CreateAdminAccount(string username)
        {
            return new HausUser
            {
                UserName = username,
                EmailConfirmed = true
            };
        }

        public static Claim CreateAdminClaim()
        {
            return new Claim(JwtClaimTypes.Role, "admin");
        }
    }
}