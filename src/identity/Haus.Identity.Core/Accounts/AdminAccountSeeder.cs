using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Accounts.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Accounts
{
    public class SeedAdminAccountRequest : IRequest
    {
        
    }
    
    public class AdminAccountSeeder : AsyncRequestHandler<SeedAdminAccountRequest>
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<HausUser> _userManager;

        private string AdminUsername => _configuration.AdminUsername();
        private string AdminPassword => _configuration.AdminPassword();
        
        public AdminAccountSeeder(
            UserManager<HausUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        protected override async Task Handle(SeedAdminAccountRequest request, CancellationToken cancellationToken)
        {
            var usernames = await _userManager.Users
                .Select(u => u.UserName)
                .ToArrayAsync(cancellationToken);

            if (ShouldCreateAdminAccount(AdminPassword, usernames))
                await _userManager.CreateAsync(CreateAdminAccount(AdminUsername), AdminPassword);
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
    }
}