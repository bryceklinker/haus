using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Accounts.Models;
using Haus.Identity.Core.Storage;
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
        private readonly HausIdentityDbContext _context;
        private readonly UserManager<HausUser> _userManager;

        private string AdminUsername => _configuration["ADMIN_USERNAME"];
        private string AdminPassword => _configuration["ADMIN_PASSWORD"];
        
        public AdminAccountSeeder(
            HausIdentityDbContext context,
            UserManager<HausUser> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        protected override async Task Handle(SeedAdminAccountRequest request, CancellationToken cancellationToken)
        {
            var usernames = await _context.Set<HausUser>()
                .Select(u => u.UserName)
                .ToArrayAsync(cancellationToken);

            if (ShouldCreateAdminAccount(AdminPassword, usernames))
                await _userManager.CreateAsync(CreateAdminAccount(AdminUsername), AdminPassword);

        }

        public static bool ShouldCreateAdminAccount(string adminUsername, string[] existingUsernames)
        {
            return !existingUsernames
                .Any(username => username.Equals(adminUsername, StringComparison.OrdinalIgnoreCase));
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