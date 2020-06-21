using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Common.Messaging.Commands;
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

        protected override async Task InnerHandle(CreateAdminAccountCommand command,
            CancellationToken cancellationToken = default)
        {
            await _messageBus.ExecuteCommand(
                new CreateAccountCommand(AdminUsername, AdminPassword, AccountDefaults.AdminUserRole),
                cancellationToken);
        }
    }
}