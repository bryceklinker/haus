using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using Haus.Identity.Core.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Haus.Identity.Core.Users.CreateUser
{
    public class CreateAdminUserCommandHandler : CommandHandler<CreateAdminUserCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly UserManager<HausUser> _userManager;

        private string AdminUsername => _configuration.AdminUsername();
        private string AdminPassword => _configuration.AdminPassword();

        public CreateAdminUserCommandHandler(
            UserManager<HausUser> userManager,
            IConfiguration configuration,
            IMessageBus messageBus)
        {
            _userManager = userManager;
            _configuration = configuration;
            _messageBus = messageBus;
        }

        protected override async Task InnerHandle(CreateAdminUserCommand command,
            CancellationToken cancellationToken = default)
        {
            await _messageBus.ExecuteCommand(
                new CreateUserCommand(AdminUsername, AdminPassword, UserDefaults.AdminUserRole),
                cancellationToken);
        }
    }
}