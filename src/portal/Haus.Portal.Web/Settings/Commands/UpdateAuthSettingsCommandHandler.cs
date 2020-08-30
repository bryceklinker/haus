using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Haus.Identity.Models.Settings;
using Haus.Portal.Web.Common.Storage;
using Haus.Portal.Web.Settings.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Portal.Web.Settings.Commands
{
    public class UpdateAuthSettingsCommand : ICommand
    {
        public AuthoritySettingsPayload Payload { get; }

        public UpdateAuthSettingsCommand(AuthoritySettingsPayload payload)
        {
            Payload = payload;
        }
    }
    public class UpdateAuthSettingsCommandHandler : CommandHandler<UpdateAuthSettingsCommand>
    {
        private readonly HausPortalDbContext _context;

        public UpdateAuthSettingsCommandHandler(HausPortalDbContext context)
        {
            _context = context;
        }

        protected override async Task InnerHandle(UpdateAuthSettingsCommand command, CancellationToken token = default)
        {
            var existingSettings = await _context.Set<AuthSettings>().SingleOrDefaultAsync(token);
            var authSettings = MapToAuthSettings(existingSettings, command.Payload);
            if (existingSettings == null) _context.Add(authSettings);
            await _context.SaveChangesAsync(token);
        }

        private AuthSettings MapToAuthSettings(AuthSettings settings, AuthoritySettingsPayload payload)
        {
            var authSettings = settings ?? new AuthSettings();
            authSettings.Authority = payload.Authority;
            return authSettings;
        }
    }
}